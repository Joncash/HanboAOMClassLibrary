using HalconDotNet;
using PylonC.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
    public delegate void GrabImageRunningMessageEventHandler(object sender, string message);
    public class LineScanGrabImageWorkingMan
    {
        public event GrabImageRunningMessageEventHandler On_RunningMessageOccur;

        #region Events (事件)
        //
        public event GrabImageStatusNotifyEventHandler On_GrabImageStatusChanged;
        //
        public event GrabImageConnectingEventHandler On_GrabImageConnecting;
        public event GrabImageConnectedEventHandler On_GrabImageConnected;
        public event GrabImageExceptionEventHandler GrabImageException;


        /// <summary>
        /// GrabImageStart, Start()
        /// </summary>
        public event GrabImageStartEventHandler GrabImageStart;

        /// <summary>
        /// GrabImageChanged
        /// </summary>
        public event GrabImageChangedEventHandler GrabImageChanged;

        /// <summary>
        /// GrabImageStopped, Stop()
        /// </summary>
        public event GrabImageStoppedEventHandler GrabImageStopped;
        //
        #endregion

        #region Public member (公有成員)
        //

        /// <summary>
        /// 是否連線
        /// </summary>
        public bool Connected { get { return Status.IsConnection; } }
        public WorkingManStatus Status { get; set; }

        /// <summary>
        /// 是否忙碌
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _bgworker.IsBusy;
            }
        }

        public string Message { get; set; }

        /// <summary>
        /// Frame per second
        /// </summary>
        public int FPS { get; set; }
        //
        #endregion

        #region properties
        private BackgroundWorker _bgworker;
        private uint _timeout = 1000 * 1;// timeout 時間, 預設為 1 秒

        const uint NUM_BUFFERS = 2;         /* Number of buffers used for grabbing. */
        private uint _numDevices;                       /* Number of available devices. */
        private PYLON_STREAMGRABBER_HANDLE _hGrabber;   /* Handle for the pylon stream grabber. */
        private PYLON_CHUNKPARSER_HANDLE _hChunkParser; /* Handle for the parser extracting the chunk data. */
        private PYLON_WAITOBJECT_HANDLE _hWait;         /* Handle used for waiting for a grab to be finished. */
        private uint _payloadSize;                      /* Size of an image frame in bytes. */
        private Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> _buffers; /* Holds handles and buffers used for grabbing. */
        private PylonGrabResult_t _grabResult;          /* Stores the result of a grab operation. */
        private PYLON_DEVICE_HANDLE _hDev;

        int nGrabs;                            /* Counts the number of buffers grabbed. */
        uint nStreams;                         /* The number of streams the device provides. */
        bool isAvail;                           /* Used for checking feature availability */
        bool isReady;                           /* Used as an output parameter */
        int i;                                 /* Counter. */
        string triggerSelectorValue = "FrameStart"; /* Preselect the trigger for image acquisition */
        bool isAvailFrameStart;                /* Used for checking feature availability */
        bool isAvailAcquisitionStart;          /* Used for checking feature availability */

        #endregion

        private bool _initSuccess { get; set; }
        private long _PEGX = 4096;  //pixels
        private long _PEGY = 4096;  //pixels
        public LineScanGrabImageWorkingMan()
        {
            initStatus();

            // Pylon 
            _hDev = new PYLON_DEVICE_HANDLE(); /* Handle for the pylon device. */
            //_ExceptionMessageList = new List<string>();
#if DEBUG
            /* This is a special debug setting needed only for GigE cameras.
                See 'Building Applications with pylon' in the programmers guide */
            Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "300000" /*ms*/);
#endif
            initialize();
            initializeBackgroundWorker();
        }
        /// <summary>
        /// 解構子
        /// </summary>
        ~LineScanGrabImageWorkingMan()
        {
            releaseResource();
        }

        #region Initial===============================================================================================
        private void initStatus()
        {
            Status = new WorkingManStatus()
            {
                IsConnection = false,
                Message = "",
                Instruction = GrabInstruction.Initialize,
                Stage = GrabStage.Initial,
                State = GrabState.Idle
            };
        }
        private void initialize()
        {
            _initSuccess = true;
            /* Before using any pylon methods, the pylon runtime must be initialized. */
            Pylon.Initialize();

            /* Enumerate all camera devices. You must call 
                PylonEnumerateDevices() before creating a device. */
            _numDevices = Pylon.EnumerateDevices();

            if (0 == _numDevices)
            {
                _initSuccess = false;
                notifyError("No devices found!");
            }

            /* Get a handle for the first device found.  */
            _hDev = Pylon.CreateDeviceByIndex(0);

            /* Before using the device, it must be opened. Open it for configuring
            parameters and for grabbing images. */
            Pylon.DeviceOpen(_hDev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);

            /* Set the pixel format to Mono8, where gray values will be output as 8 bit values for each pixel. */
            /* ... Check first to see if the device supports the Mono8 format. */
            isAvail = Pylon.DeviceFeatureIsAvailable(_hDev, "EnumEntry_PixelFormat_Mono8");

            if (!isAvail)
            {
                /* Feature is not available. */
                _initSuccess = false;
                notifyError("Device doesn't support the Mono8 pixel format.");
            }
            else
            {
                /* ... Set the pixel format to Mono8. */
                Pylon.DeviceFeatureFromString(_hDev, "PixelFormat", "Mono8");
            }

            /* For GigE cameras, we recommend increasing the packet size for better 
                   performance. If the network adapter supports jumbo frames, set the packet 
                   size to a value > 1500, e.g., to 8192. In this sample, we only set the packet size
                   to 1500. */
            /* ... Check first to see if the GigE camera packet size parameter is supported and if it is writable. */
            isAvail = Pylon.DeviceFeatureIsWritable(_hDev, "GevSCPSPacketSize");

            if (isAvail)
            {
                /* ... The device supports the packet size feature. Set a value. */
                Pylon.DeviceSetIntegerFeature(_hDev, "GevSCPSPacketSize", 1500);
            }

            setStreamGrabber();

            //Free Run Settings
            //setFreeRunParams();
            setPEGParams(_PEGX, _PEGY);


            prepareDevice();
            setStatus(GrabInstruction.Initialize, GrabStage.Connected, GrabState.Idle);
            notifyStateChange(_bgworker, new GrabImageStatusChangedEventArgs() { Status = this.Status });
        }

        private void initializeBackgroundWorker()
        {
            _bgworker = new BackgroundWorker();
            _bgworker.WorkerSupportsCancellation = true;
            _bgworker.WorkerReportsProgress = true;
            _bgworker.DoWork += _bgWorker_DoWork;
            _bgworker.ProgressChanged += _bgWorker_ProgressChanged;
            _bgworker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
        }

        private void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                setStatus(GrabInstruction.GrabImage, GrabStage.Connected, GrabState.Idle);
                notifyStateChange(_bgworker, new GrabImageStatusChangedEventArgs() { Status = this.Status });
                stopGrab();
                setStatus(GrabInstruction.Connect, GrabStage.Connected, GrabState.Idle);
                notifyStateChange(sender, new GrabImageStatusChangedEventArgs() { Status = this.Status });
                notifyGrabImageStop(sender, e);
            }
            catch (Exception ex)
            {
                notifyError(ex.Message);
            }
        }

        private void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            noitfyGrabImage(e.UserState as HImage);
        }

        private void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            Pylon.DeviceFeatureFromString(_hDev, "AcquisitionMode", "Continuous");
            Pylon.DeviceExecuteCommandFeature(_hDev, "AcquisitionStart");
            notifyRunningMessage("Start and Wait....");
            while (true)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    try
                    {
                        start(worker);
                    }
                    catch (Exception ex)
                    {
                        notifyError(ex.Message);
                    }
                }
            }
        }
        //===========
        private void start(BackgroundWorker worker)
        {
            /* Issue an acquisition start command. Because the trigger mode is enabled, issuing the start command
                   itself will not trigger any image acquisitions. Issuing the start command simply prepares the camera. 
                   Once the camera is prepared it will acquire one image for every trigger it receives. */
            /* Trigger the first image. */
            //Pylon.DeviceExecuteCommandFeature(hDev, "TriggerSoftware");

            nGrabs = 0;                           /* Counts the number of images grabbed. */
            //while (true)
            //{
            int bufferIndex;              /* Index of the buffer. */
            long chunkWidth = _PEGX; /* data retrieved from the chunk parser */
            long chunkHeight = _PEGY; /* data retrieved from the chunk parser */

            /* Wait for the next buffer to be filled. Wait up to 1000 ms. */
            notifyError("Grab waiting!");
            isReady = Pylon.WaitObjectWait(_hWait, _timeout);

            if (!isReady)
            {
                //notifyError("Grab timeout occurred");
                //worker.CancelAsync();
                return;//break;
            }

            /* Since the wait operation was successful, the result of at least one grab 
               operation is available. Retrieve it. */
            isReady = Pylon.StreamGrabberRetrieveResult(_hGrabber, out _grabResult);
            if (!isReady)
            {
                /* Oops. No grab result available? We should never have reached this point. 
                   Since the wait operation above returned without a timeout, a grab result 
                   should be available. */
                //notifyError("Failed to retrieve a grab result.");
                return;//break;
            }

            nGrabs++;
            //fpath = Path.Combine(fdir, nGrabs.ToString(@"D2"));
            /* Trigger the next image. Since we passed more than one buffer to the stream grabber, 
               the triggered image will be grabbed while the image processing is performed.  */
            //Pylon.DeviceExecuteCommandFeature(hDev, "TriggerSoftware");

            /* Get the buffer index from the context information. */
            bufferIndex = (int)_grabResult.Context;

            /* Check to see if the image was grabbed successfully. */
            if (_grabResult.Status == EPylonGrabStatus.Grabbed)
            {
                /*  The grab is successful.  */
                PylonBuffer<Byte> buffer;        /* Reference to the buffer attached to the grab result. */

                /* Get the buffer from the dictionary. Since we also got the buffer index, 
                   we could alternatively use an array, e.g. buffers[bufferIndex]. */
                if (!_buffers.TryGetValue(_grabResult.hBuffer, out buffer))
                {
                    /* Oops. No buffer available? We should never have reached this point. Since all buffers are
                       in the dictionary. */
                    //notifyError("Failed to find the buffer associated with the handle returned in grab result.");
                    return;//break;
                }
                // notifyRunningMessage(String.Format("Grabbed frame {0} into buffer {1}.", nGrabs, bufferIndex));




                //Save Image
                // notifyRunningMessage("SAVE Image");
                IntPtr pointer = buffer.Pointer;
                try
                {
                    /**/
                    var myBuf = buffer.Array;
                    notifyRunningMessage(String.Format("W: {0} H: {1}  Length: {2}", chunkWidth, chunkHeight, myBuf.Length));
                    if (myBuf.Length >= chunkHeight * chunkWidth)
                    {
                        /* Perform the image processing. */
                        //getMinMax(buffer.Array, chunkWidth, chunkHeight, out min, out max);
                        //notifyRunningMessage(String.Format("Min. gray value  = {0}, Max. gray value = {1}", min, max));

                        var hImag = new HImage("byte", (int)chunkWidth, (int)chunkHeight, pointer);
                        worker.ReportProgress(nGrabs, new HImage(hImag));
                    }
                    //HOperatorSet.WriteImage(hImag, "png", 0, fpath);
                }
                catch (Exception ex)
                {
                    // Console.WriteLine("SAVE Image Error : " + ex.Message);
                }
                /* Before requeueing the buffer, you should detach it from the chunk parser. */
                Pylon.ChunkParserDetachBuffer(_hChunkParser);  /* Now the chunk data in the buffer is no longer accessible. */
            }
            else if (_grabResult.Status == EPylonGrabStatus.Failed)
            {
                notifyRunningMessage(String.Format("Frame {0} wasn't grabbed successfully.  Error code = {1}", nGrabs, _grabResult.ErrorCode));
            }

            /* Once finished with the processing, requeue the buffer to be filled again. */
            Pylon.StreamGrabberQueueBuffer(_hGrabber, _grabResult.hBuffer, bufferIndex);
            //}//endwhile

        }

        #endregion

        #region 設定======================================================================================================

        private void prepareDevice()
        {
            //StreamGrabberPrepareGrab 執行後，就不能再設定 pixel Width and pixel Height , 要注意
            /*  Allocate the resources required for grabbing. After this, critical parameters 
                         that impact the payload size must not be changed until FinishGrab() is called. */
            Pylon.StreamGrabberPrepareGrab(_hGrabber);

            /* Before using the buffers for grabbing, they must be registered at
               the stream grabber. For each registered buffer, a buffer handle
               is returned. After registering, these handles are used instead of the
               buffer objects pointers. The buffer objects are held in a dictionary,
               that provides access to the buffer using a handle as key.
             */
            _buffers = new Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>>();
            for (i = 0; i < NUM_BUFFERS; ++i)
            {
                PylonBuffer<Byte> buffer = new PylonBuffer<byte>(_payloadSize, true);
                PYLON_STREAMBUFFER_HANDLE handle = Pylon.StreamGrabberRegisterBuffer(_hGrabber, ref buffer);
                _buffers.Add(handle, buffer);
            }

            /* Feed the buffers into the stream grabber's input queue. For each buffer, the API 
               allows passing in an integer as additional context information. This integer
               will be returned unchanged when the grab is finished. In our example, we use the index of the 
               buffer as context information. */
            i = 0;
            foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in _buffers)
            {
                Pylon.StreamGrabberQueueBuffer(_hGrabber, pair.Key, i++);
            }
        }

        private void setStreamGrabber()
        {
            /* Image grabbing is done using a stream grabber.  
                       A device may be able to provide different streams. A separate stream grabber must 
                       be used for each stream. In this sample, we create a stream grabber for the default 
                       stream, i.e., the first stream ( index == 0 ).
                       */

            /* Get the number of streams supported by the device and the transport layer. */
            nStreams = Pylon.DeviceGetNumStreamGrabberChannels(_hDev);

            if (nStreams < 1)
            {
                throw new Exception("The transport layer doesn't support image streams.");
            }

            /* Create and open a stream grabber for the first channel. */
            _hGrabber = Pylon.DeviceGetStreamGrabber(_hDev, 0);
            Pylon.StreamGrabberOpen(_hGrabber);

            /* Get a handle for the stream grabber's wait object. The wait object
               allows waiting for buffers to be filled with grabbed data. */
            _hWait = Pylon.StreamGrabberGetWaitObject(_hGrabber);

            /* Determine the required size of the grab buffer. Since activating chunks will increase the
               payload size and thus the required buffer size, do this after enabling the chunks. */
            _payloadSize = checked((uint)Pylon.DeviceGetIntegerFeature(_hDev, "PayloadSize"));
            /* We must tell the stream grabber the number and size of the buffers 
                we are using. */
            /* .. We will not use more than NUM_BUFFERS for grabbing. */
            Pylon.StreamGrabberSetMaxNumBuffer(_hGrabber, NUM_BUFFERS);

            /* .. We will not use buffers bigger than payloadSize bytes. */
            Pylon.StreamGrabberSetMaxBufferSize(_hGrabber, _payloadSize);
        }

        private void setFreeRunParams()
        {
            var iswritable = Pylon.DeviceFeatureIsWritable(_hDev, "Height");
            if (iswritable)
            {
                Pylon.DeviceSetIntegerFeature(_hDev, "Height", 4096);
            }
            iswritable = Pylon.DeviceFeatureIsWritable(_hDev, "Width");
            if (iswritable)
            {
                Pylon.DeviceSetIntegerFeature(_hDev, "Width", 4096);
            }


            /* Check the available camera trigger mode(s) to select the appropriate one: acquisition start trigger mode (used by previous cameras;
           do not confuse with acquisition start command) or frame start trigger mode (equivalent to previous acquisition start trigger mode). */
            isAvailAcquisitionStart = Pylon.DeviceFeatureIsAvailable(_hDev, "EnumEntry_TriggerSelector_AcquisitionStart");
            isAvailFrameStart = Pylon.DeviceFeatureIsAvailable(_hDev, "EnumEntry_TriggerSelector_FrameStart");

            /* Check to see if the camera implements the acquisition start trigger mode only. */
            if (isAvailAcquisitionStart && !isAvailFrameStart)
            {
                /* Camera uses the acquisition start trigger as the only trigger mode. */
                Pylon.DeviceFeatureFromString(_hDev, "TriggerSelector", "AcquisitionStart");
                Pylon.DeviceFeatureFromString(_hDev, "TriggerMode", "On");
                triggerSelectorValue = "AcquisitionStart";
            }
            else
            {
                /* Camera may have the acquisition start trigger mode and the frame start trigger mode implemented.
                In this case, the acquisition trigger mode must be switched off. */
                if (isAvailAcquisitionStart)
                {
                    Pylon.DeviceFeatureFromString(_hDev, "TriggerSelector", "AcquisitionStart");
                    Pylon.DeviceFeatureFromString(_hDev, "TriggerMode", "Off");
                }
                /* To trigger each single frame by software or external hardware trigger: Enable the frame start trigger mode. */
                Pylon.DeviceFeatureFromString(_hDev, "TriggerSelector", "FrameStart");
                Pylon.DeviceFeatureFromString(_hDev, "TriggerMode", "On");
            }
            /* Note: the trigger selector must be set to the appropriate trigger mode 
                before setting the trigger source or issuing software triggers.
                Frame start trigger mode for newer cameras, acquisition start trigger mode for previous cameras. */
            Pylon.DeviceFeatureFromString(_hDev, "TriggerSelector", triggerSelectorValue);

            /* Enable software triggering. */
            /* ... Select the software trigger as the trigger source. */
            Pylon.DeviceFeatureFromString(_hDev, "TriggerSource", "Software");
            /* When using software triggering, the Continuous frame mode should be used. Once 
                  acquisition is started, the camera sends one image each time a software trigger is 
                  issued. */
            Pylon.DeviceFeatureFromString(_hDev, "AcquisitionMode", "Continuous");
        }

        /// <summary>
        /// setPEGParams
        /// </summary>
        /// <param name="pegW">Pixel Width ( 影像寬度 pixel)</param>
        /// <param name="pegH">Pixel Height ( 影像長度 pixel, 多少 pixel 出一張圖)</param>
        private void setPEGParams(long pegW, long pegH)
        {
            var iswritable = Pylon.DeviceFeatureIsWritable(_hDev, "Height");
            if (iswritable)
            {
                Pylon.DeviceSetIntegerFeature(_hDev, "Height", pegH);
            }
            iswritable = Pylon.DeviceFeatureIsWritable(_hDev, "Width");
            if (iswritable)
            {
                Pylon.DeviceSetIntegerFeature(_hDev, "Width", pegW);
            }
            /* Disable acquisition start trigger if available. */
            if (Pylon.DeviceFeatureIsAvailable(_hDev, "EnumEntry_TriggerSelector_AcquisitionStart"))
            {
                Pylon.DeviceFeatureFromString(_hDev, "TriggerSelector", "AcquisitionStart");
                Pylon.DeviceFeatureFromString(_hDev, "TriggerMode", "Off");
            }

            /* Disable frame start trigger if available. */
            if (Pylon.DeviceFeatureIsAvailable(_hDev, "EnumEntry_TriggerSelector_FrameStart"))
            {
                Pylon.DeviceFeatureFromString(_hDev, "TriggerSelector", "FrameStart");
                Pylon.DeviceFeatureFromString(_hDev, "TriggerMode", "Off");
            }
            /*Enable Line Start trigger if available */

            try
            {
                if (Pylon.DeviceFeatureIsAvailable(_hDev, "EnumEntry_TriggerSelector_LineStart"))
                {
                    Pylon.DeviceFeatureFromString(_hDev, "TriggerSelector", "LineStart");
                    Pylon.DeviceFeatureFromString(_hDev, "TriggerMode", "On");
                }

                if (Pylon.DeviceFeatureIsAvailable(_hDev, "EnumEntry_ExposureMode_TriggerWidth"))
                {
                    Pylon.DeviceFeatureFromString(_hDev, "ExposureMode", "TriggerWidth");
                    //Pylon.DeviceFeatureFromString(m_hDevice, "TriggerMode", "On");
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        /* Simple "image processing" function returning the minimum and maximum gray 
        value of an 8 bit gray value image. */
        private void getMinMax(Byte[] imageBuffer, long width, long height, out Byte min, out Byte max)
        {
            min = 255; max = 0;
            long imageDataSize = width * height;

            for (long i = 0; i < imageDataSize; ++i)
            {
                Byte val = imageBuffer[i];
                if (val > max)
                    max = val;
                if (val < min)
                    min = val;
            }
        }


        #endregion

        #region 釋放======================================================================================================
        private bool releaseStreamAndBuffers()
        {
            bool isRelease = true;
            try
            {
                /* ... We must issue a cancel call to ensure that all pending buffers are put into the
               stream grabber's output queue. */
                Pylon.StreamGrabberCancelGrab(_hGrabber);

                /* ... The buffers can now be retrieved from the stream grabber. */
                do
                {
                    isReady = Pylon.StreamGrabberRetrieveResult(_hGrabber, out _grabResult);

                } while (isReady);

                foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in _buffers)
                {
                    Pylon.StreamGrabberDeregisterBuffer(_hGrabber, pair.Key);
                    pair.Value.Dispose();
                }
                _buffers = null;

                /* ... Release grabbing related resources. */
                Pylon.StreamGrabberFinishGrab(_hGrabber);
            }
            catch (Exception ex)
            {
                isRelease = false;
            }

            return isRelease;
        }

        private void releaseResource()
        {
            /* ... When all buffers are retrieved from the stream grabber, they can be deregistered.
                      After deregistering the buffers, it is safe to free the memory */
            try
            {
                foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in _buffers)
                {
                    Pylon.StreamGrabberDeregisterBuffer(_hGrabber, pair.Key);
                    pair.Value.Dispose();
                }
                _buffers = null;

                /* ... Release grabbing related resources. */
                Pylon.StreamGrabberFinishGrab(_hGrabber);


                /* After calling PylonStreamGrabberFinishGrab(), parameters that impact the payload size (e.g., 
                the AOI width and height parameters) are unlocked and can be modified again. */

                /* ... Close the stream grabber. */
                Pylon.StreamGrabberClose(_hGrabber);


                /* ... Release the chunk parser. */
                Pylon.DeviceDestroyChunkParser(_hDev, _hChunkParser);


                /*  Disable the software trigger and chunk mode. */
                if (_hDev.IsValid)
                {
                    Pylon.DeviceSetBooleanFeature(_hDev, "ChunkModeActive", false);
                    Pylon.DeviceFeatureFromString(_hDev, "TriggerMode", "Off");
                }

                /* ... Close and release the pylon device. The stream grabber becomes invalid
                   after closing the pylon device. Don't call stream grabber related methods after 
                   closing or releasing the device. */
                Pylon.DeviceClose(_hDev);
                Pylon.DestroyDevice(_hDev);
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                /* ... Shut down the pylon runtime system. Don't call any pylon method after 
                   calling PylonTerminate(). */
                Pylon.Terminate();
            }
        }

        private void stopGrab()
        {
            /*  ... Stop the camera. */
            Pylon.DeviceExecuteCommandFeature(_hDev, "AcquisitionStop");

            /* ... We must issue a cancel call to ensure that all pending buffers are put into the
               stream grabber's output queue. */
            Pylon.StreamGrabberCancelGrab(_hGrabber);

            /* ... The buffers can now be retrieved from the stream grabber. */
            do
            {
                isReady = Pylon.StreamGrabberRetrieveResult(_hGrabber, out _grabResult);

            } while (isReady);
            // releaseStreamAndBuffers();
        }
        #endregion


        #region Notify====================================================================================================
        private void notifyError(string p)
        {
            if (GrabImageException != null)
            {
                GrabImageException(new Exception(p));
            }
        }
        private void noitfyGrabImage(HImage hImage)
        {
            if (GrabImageChanged != null)
            {
                GrabImageChanged(_bgworker, new ProgressChangedEventArgs(0, hImage));
            }
        }
        private void notifyRunningMessage(string msg)
        {
            if (On_RunningMessageOccur != null)
            {
                On_RunningMessageOccur(null, msg);
            }
        }
        private void notifyGrabImageStop(object sender, RunWorkerCompletedEventArgs e)
        {
            if (GrabImageStopped != null)
            {
                GrabImageStopped(sender, e);
            }
        }
        private void notifyStateChange(object sender, GrabImageStatusChangedEventArgs e)
        {
            if (On_GrabImageStatusChanged != null)
            {
                On_GrabImageStatusChanged(sender, e);
            }
        }

        #endregion
        private void setStatus(GrabInstruction instruction, GrabStage stage, GrabState state)
        {
            Status.Instruction = instruction;
            Status.Stage = stage;
            Status.State = state;
        }

        #region Public APIs=======================================================================
        /// <summary>
        /// Free Run Mode ( trigger off)
        /// </summary>
        public void SetFreeRunMode()
        {
            releaseStreamAndBuffers();
            setFreeRunParams();
            prepareDevice();
        }

        /// <summary>
        /// PEG Mode，設定模式
        /// pegW, means the width of each frame (pixel)
        /// pegH, means the height of each frame (pixel)
        /// </summary>
        /// <param name="pegW">Width (pixel)</param>
        /// <param name="pegH">Height (pixel)</param>
        public void SetPEGMode(long pegW, long pegH)
        {
            _PEGX = pegW;
            _PEGY = 4096;
            releaseStreamAndBuffers();
            setPEGParams(pegW, pegH);
            prepareDevice();
        }

        public string[] GetSettings()
        {
            //var iswritable = Pylon.DeviceFeatureIsWritable(_hDev, "Height");
            var isReadble = Pylon.DeviceFeatureIsReadable(_hDev, "Height");
            var height = (isReadble) ? Pylon.DeviceFeatureToString(_hDev, "Height") : "N/A";
            isReadble = Pylon.DeviceFeatureIsReadable(_hDev, "Width");
            var width = (isReadble) ? Pylon.DeviceFeatureToString(_hDev, "Height") : "N/A";

            var triggerS = Pylon.DeviceFeatureIsReadable(_hDev, "TriggerSelector") ?
                            Pylon.DeviceFeatureToString(_hDev, "TriggerSelector") : "N/A";
            var triggerMode = Pylon.DeviceFeatureIsReadable(_hDev, "TriggerMode") ?
                Pylon.DeviceFeatureToString(_hDev, "TriggerMode") : "N/A";

            return new string[] { width, height };
        }

        #endregion

        #region public menthod (一定要的公開方法)
        public void Connect()
        {
            if (_initSuccess)
            {
                setStatus(GrabInstruction.Connect, GrabStage.Connected, GrabState.Busy);
                this.Status.IsConnection = true;
                notifyStateChange(_bgworker, new GrabImageStatusChangedEventArgs()
                {
                    Status = this.Status
                });
            }
        }

        /// <summary>
        /// 連續擷取
        /// </summary>
        public void Start()
        {
            setStatus(GrabInstruction.GrabImage, GrabStage.Connected, GrabState.Busy);
            notifyStateChange(_bgworker, new GrabImageStatusChangedEventArgs() { Status = this.Status });
            if (!_bgworker.IsBusy)
            {
                _bgworker.RunWorkerAsync();
            }

        }

        /// <summary>
        /// 單張擷取
        /// </summary>
        public void SnapShot()
        {

        }

        /// <summary>
        /// NotImplementedException
        /// </summary>
        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 停止擷取
        /// </summary>
        public void Cancel()
        {
            _bgworker.CancelAsync();
        }

        /// <summary>
        /// 中斷連線
        /// </summary>
        public void DisConnection()
        {

        }
        public void Dispose()
        {

        }
        public HObject GetCurrentImage()
        {
            return null;
        }

        /// <summary>
        /// Celar all Register evetns
        /// </summary>
        public void RemoveAllRegisterEvent()
        {
            On_GrabImageStatusChanged = null;
            On_GrabImageConnecting = null;
            On_GrabImageConnected = null;
            GrabImageException = null;
            GrabImageStart = null;
            GrabImageChanged = null;
            GrabImageStopped = null;
        }
        #endregion
    }
}
