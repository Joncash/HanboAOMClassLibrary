using HalconDotNet;
using PylonC.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public enum GrabMode { OneShot, Continuously };
	
	public class PylonGrabImageWorkingMan : IGrabImage, IDisposable
	{
		public PylonGrabImageWorkingMan()
		{
			pylonInitialize();
			initializeBackgroundWorker();
		}

		#region Events (事件)
		//
		public event GrabImageStatusNotifyEventHandler On_GrabImageStatusChanged;
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

		#endregion

		#region 公有屬性
		//
		private PylonGrabImageStartHandler _grabImage;

		/// <summary>
		/// 是否忙碌(唯讀)
		/// </summary>
		public bool IsBusy { get { return _bgworker.IsBusy; } }
		#endregion

		#region Private property member (私有屬性成員)
		/**********************************************************************************/
		private BackgroundWorker _bgworker;
		private PYLON_DEVICE_HANDLE _pylonDevHandle;
		private List<PylonFeature> _simpleGrabFeatures; //Simple Grab Features
		private List<PylonFeature> _streamGrabFeatures; //Stream Grab Features
		private string _latestMessage;	//最後的錯誤訊息
		private HImage _latestImage;	//最後的影像
		private uint _singleFrameWaitup = 500; // "single frame" acquisition mode.Wait up  (ms)

		PylonGrabResult_t grabResult;

		PYLON_STREAMGRABBER_HANDLE hGrabber;
		PYLON_WAITOBJECT_HANDLE hWait;
		Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> buffers;

		private PylonGrabImageStartHandler _grabImageStartMethod;
		private PylonGrabImageInitHandler _grabImageInitMethod;
		private PylonGrabImageCleanHandler _grabImageCleanMethod;
		#endregion


		#region 非同步方法
		private void initializeBackgroundWorker()
		{
			_bgworker = new BackgroundWorker();
			_bgworker.WorkerReportsProgress = true;
			_bgworker.WorkerSupportsCancellation = true;
			_bgworker.DoWork += new DoWorkEventHandler(_bgworker_DoWork);
			_bgworker.ProgressChanged += new ProgressChangedEventHandler(_bgworker_ProgressChanged);
			_bgworker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgworker_RunWorkerCompleted);
		}
		protected void OnGrabImageChanged(ProgressChangedEventArgs e)
		{
			if (GrabImageChanged != null)
			{
				GrabImageChanged(this, e);
			}
		}

		private void _bgworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				if (_grabImageCleanMethod != null)
				{
					_grabImageCleanMethod();
				}
			}
			if (GrabImageStopped != null)
				GrabImageStopped(sender, e);
		}

		private void _bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			OnGrabImageChanged(e);
		}

		private void _bgworker_DoWork(object sender, DoWorkEventArgs e)
		{
			var worker = sender as BackgroundWorker;
			try
			{
				if (GrabImageStart != null) GrabImageStart(sender, e);
				var i = 0;
				while (true)
				{
					if (worker.CancellationPending)
					{
						e.Cancel = true;
						break;
					}
					else
					{
						_grabImageStartMethod();
						var himage = new HImage(_latestImage);
						worker.ReportProgress(i++, himage);
					}
				}
			}
			catch (HalconDotNet.HOperatorException ex)
			{
				Hanbo.Log.LogManager.Error(ex);
				if (GrabImageException != null)
				{
					GrabImageException(ex);
				}
			}
		}

		/// <summary>
		/// 連續擷取(非同步)
		/// </summary>
		public void Start()
		{
			ContinuouslyGrab();
		}

		/// <summary>
		/// 中斷連線(非同步)
		/// </summary>
		public void DisConnection()
		{
		}

		/// <summary>
		/// 停止連線(非同步)
		/// </summary>
		public void Cancel()
		{
			_bgworker.CancelAsync();
		}
		#endregion



		private void pylonInitialize()
		{
			/* Before using any pylon methods, the pylon runtime must be initialized. */
			Pylon.Initialize();
			_pylonDevHandle = GetPylonDeviceHandle();
			initDefaultGrabFeatures();
			setGevSCPSPacketSize();


		}

		private void initDefaultGrabFeatures()
		{
			_simpleGrabFeatures = new List<PylonFeature>() { 
			new PylonFeature(){Name = "EnumEntry_PixelFormat_Mono8", Key = "PixelFormat", Value = "Mono8"},
			new PylonFeature(){Name = "EnumEntry_TriggerSelector_AcquisitionStart", Key = "TriggerSelector", Value = "AcquisitionStart"},
			new PylonFeature(){Name = "EnumEntry_TriggerSelector_AcquisitionStart", Key = "TriggerMode", Value = "Off"},
			new PylonFeature(){Name = "EnumEntry_TriggerSelector_FrameStart", Key = "TriggerSelector", Value = "FrameStart"},
			new PylonFeature(){Name = "EnumEntry_TriggerSelector_FrameStart", Key = "TriggerMode", Value = "Off"},			
			};

			_streamGrabFeatures = new List<PylonFeature>() { 
			new PylonFeature(){Name = "EnumEntry_PixelFormat_Mono8", Key = "PixelFormat", Value = "Mono8"},
			};
		}

		private void setGevSCPSPacketSize()
		{
			/* For GigE cameras, we recommend increasing the packet size for better 
				   performance. If the network adapter supports jumbo frames, set the packet 
				   size to a value > 1500, e.g., to 8192. In this sample, we only set the packet size
				   to 1500. */
			/* ... Check first to see if the GigE camera packet size parameter is supported 
				and if it is writable. 
			 
			 開啟 pylonViewr, 找到裝置的 Transport Layer 節點, 可以設定 Packet Size, 
			 在裝置的的 Stream Parameter 節點，觀察 Statistic 節點中的 Failed Buffer cout,
			 若無 Fail, 則可調高 packet size
			 
			 */
			bool isAvail = Pylon.DeviceFeatureIsWritable(_pylonDevHandle, "GevSCPSPacketSize");
			if (isAvail)
			{
				/* ... The device supports the packet size feature. Set a value. */
				Pylon.DeviceSetIntegerFeature(_pylonDevHandle, "GevSCPSPacketSize", 1500);
			}
		}


		/// <summary>
		/// 取得第一個找到的 Device
		/// </summary>
		/// <returns></returns>
		public PYLON_DEVICE_HANDLE GetPylonDeviceHandle()
		{
			if (_pylonDevHandle == null)
			{
				_latestMessage = "";
				uint numDevices = Pylon.EnumerateDevices();
				var deviceNotExists = (0 == numDevices);
				if (deviceNotExists)
				{
					_latestMessage = "No devices found.";
				}
				else
				{
					/* Get a handle for the first device found.  */
					_pylonDevHandle = Pylon.CreateDeviceByIndex(0);
				}
			}
			return _pylonDevHandle;
		}

		/// <summary>
		/// 開啟裝置
		/// </summary>
		public void DeviceOpen()
		{
			/*	Before using the device, it must be opened. Open it for configuring
					parameters and for grabbing images. */
			if (!Pylon.DeviceIsOpen(_pylonDevHandle))
				Pylon.DeviceOpen(_pylonDevHandle, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);
		}

		/// <summary>
		/// 關閉裝置
		/// </summary>
		public void DeviceClose()
		{
			/*	Close and release the pylon device. The stream grabber becomes invalid
					after closing the pylon device. Don't call stream grabber related methods after 
					closing or releasing the device. */
			if (Pylon.DeviceIsOpen(_pylonDevHandle))
				Pylon.DeviceClose(_pylonDevHandle);
		}

		/// <summary>
		/// 設定 Device Handle 功能
		/// </summary>
		/// <param name="hDev"></param>
		/// <param name="features"></param>
		/// <returns></returns>
		public bool SetPylonDeviceHandleFeatures(List<PylonFeature> features)
		{
			var success = true;
			bool isAvail, isWritable;
			_latestMessage = "";
			foreach (var feature in features)
			{
				isAvail = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, feature.Name);
				isWritable = Pylon.DeviceFeatureIsWritable(_pylonDevHandle, feature.Key);
				if (!isAvail)
				{
					_latestMessage += "Device doesn't support the " + feature.Name + ";";
					success = false;
				}
				else if (!isWritable)
				{
					_latestMessage += "Writable doesn't support the " + feature.Name + ";";
					success = false;
				}
				else
				{
					Type valueType = feature.Value.GetType();
					switch (valueType.Name)
					{
						case "Boolean":
							Pylon.DeviceSetBooleanFeature(_pylonDevHandle, feature.Key, (bool)feature.Value);
							break;
						case "String":
							Pylon.DeviceFeatureFromString(_pylonDevHandle, feature.Key, (string)feature.Value);
							break;
						case "Int32":
							Pylon.DeviceSetIntegerFeature(_pylonDevHandle, feature.Key, (int)feature.Value);
							break;
						case "Double":
							Pylon.DeviceSetFloatFeature(_pylonDevHandle, feature.Key, (double)feature.Value);
							break;
					}
				}
			}
			return success;
		}

		public string GetLatestMessage()
		{
			return _latestMessage;
		}

		public HImage GetLastestImage()
		{
			return _latestImage;
		}

		private void chunkGrabImageMethod()
		{
			string triggerSelectorValue = "FrameStart"; /* Preselect the trigger for image acquisition */

			/* Check the available camera trigger mode(s) to select the appropriate one: 
			 * acquisition start trigger mode (used by previous cameras;
			do not confuse with acquisition start command) or 
			 * frame start trigger mode (equivalent to previous acquisition start trigger mode). */


			bool isAvailAcquisitionStart = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "EnumEntry_TriggerSelector_AcquisitionStart");
			bool isAvailFrameStart = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "EnumEntry_TriggerSelector_FrameStart");

			/* Check to see if the camera implements the acquisition start trigger mode only. */
			bool isAcqStartTriggerModeOnly = (isAvailAcquisitionStart && !isAvailFrameStart);
			if (isAcqStartTriggerModeOnly)
			{
				/* Camera uses the acquisition start trigger as the only trigger mode. */
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerSelector", "AcquisitionStart");
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerMode", "On");
				triggerSelectorValue = "AcquisitionStart";
			}
			else
			{
				/* Camera may have the acquisition start trigger mode and the frame start trigger mode implemented.
				In this case, the acquisition trigger mode must be switched off. */
				if (isAvailAcquisitionStart)
				{
					Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerSelector", "AcquisitionStart");
					Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerMode", "Off");
				}
				/* To trigger each single frame by software or external hardware trigger: Enable the frame start trigger mode. */
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerSelector", "FrameStart");
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerMode", "On");
			}

			/* Note: the trigger selector must be set to the appropriate trigger mode 
			before setting the trigger source or issuing software triggers.
			Frame start trigger mode for newer cameras, acquisition start trigger mode for previous cameras. */
			Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerSelector", triggerSelectorValue);

			/* Enable software triggering. */
			/* ... Select the software trigger as the trigger source. */
			Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerSource", "Software");

			/* When using software triggering, the Continuous frame mode should be used. Once 
			   acquisition is started, the camera sends one image each time a software trigger is 
			   issued. */
			Pylon.DeviceFeatureFromString(_pylonDevHandle, "AcquisitionMode", "Continuous");


			//********************************************************************************************************
			setChunkModeFeatures();

			/* The data block containing the image chunk and the other chunks has a self-descriptive layout. 
			   A chunk parser is used to extract the appended chunk data from the grabbed image frame.
			   Create a chunk parser. */
			PYLON_CHUNKPARSER_HANDLE hChunkParser = Pylon.DeviceCreateChunkParser(_pylonDevHandle);

			if (!hChunkParser.IsValid)
			{
				/* The transport layer doesn't provide a chunk parser. */
				throw new Exception("No chunk parser available.");
			}


			/* Image grabbing is done using a stream grabber.  
			  A device may be able to provide different streams. A separate stream grabber must 
			  be used for each stream. In this sample, we create a stream grabber for the default 
			  stream, i.e., the first stream ( index == 0 ).
			  */

			/* Get the number of streams supported by the device and the transport layer. */
			var nStreams = Pylon.DeviceGetNumStreamGrabberChannels(_pylonDevHandle);

			if (nStreams < 1)
			{
				throw new Exception("The transport layer doesn't support image streams.");
			}

			/* Create and open a stream grabber for the first channel. */
			uint firstChannel = 0;
			PYLON_STREAMGRABBER_HANDLE hGrabber = Pylon.DeviceGetStreamGrabber(_pylonDevHandle, firstChannel);
			Pylon.StreamGrabberOpen(hGrabber);

			/* Get a handle for the stream grabber's wait object. The wait object
			   allows waiting for buffers to be filled with grabbed data. */
			PYLON_WAITOBJECT_HANDLE hWait = Pylon.StreamGrabberGetWaitObject(hGrabber);

			/* Determine the required size of the grab buffer. Since activating chunks will increase the
			   payload size and thus the required buffer size, do this after enabling the chunks. */
			uint payloadSize = checked((uint)Pylon.DeviceGetIntegerFeature(_pylonDevHandle, "PayloadSize"));
			/* We must tell the stream grabber the number and size of the buffers 
				we are using. */
			/* .. We will not use more than NUM_BUFFERS for grabbing. */
			uint NUM_BUFFERS = 2;
			Pylon.StreamGrabberSetMaxNumBuffer(hGrabber, NUM_BUFFERS);

			/* .. We will not use buffers bigger than payloadSize bytes. */
			Pylon.StreamGrabberSetMaxBufferSize(hGrabber, payloadSize);

			/*  Allocate the resources required for grabbing. After this, critical parameters 
				that impact the payload size must not be changed until FinishGrab() is called. */
			Pylon.StreamGrabberPrepareGrab(hGrabber);

			/* Before using the buffers for grabbing, they must be registered at
			   the stream grabber. For each registered buffer, a buffer handle
			   is returned. After registering, these handles are used instead of the
			   buffer objects pointers. The buffer objects are held in a dictionary,
			   that provides access to the buffer using a handle as key.
			 */
			var buffers = new Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>>();
			for (int i = 0; i < NUM_BUFFERS; ++i)
			{
				PylonBuffer<Byte> buffer = new PylonBuffer<byte>(payloadSize, true);
				PYLON_STREAMBUFFER_HANDLE handle = Pylon.StreamGrabberRegisterBuffer(hGrabber, ref buffer);
				buffers.Add(handle, buffer);
			}

			/* Feed the buffers into the stream grabber's input queue. For each buffer, the API 
			   allows passing in an integer as additional context information. This integer
			   will be returned unchanged when the grab is finished. In our example, we use the index of the 
			   buffer as context information. */
			var index = 0;
			foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in buffers)
			{
				Pylon.StreamGrabberQueueBuffer(hGrabber, pair.Key, index++);
			}

			/* Issue an acquisition start command. Because the trigger mode is enabled, issuing the start command
			   itself will not trigger any image acquisitions. Issuing the start command simply prepares the camera. 
			   Once the camera is prepared it will acquire one image for every trigger it receives. */
			Pylon.DeviceExecuteCommandFeature(_pylonDevHandle, "AcquisitionStart");

			/* Trigger the first image. */
			Pylon.DeviceExecuteCommandFeature(_pylonDevHandle, "TriggerSoftware");

			/* Grab NUM_GRABS images. */
			PylonGrabResult_t grabResult;
			int NUM_GRABS = 1;
			int nGrabs = 0;                           /* Counts the number of images grabbed. */
			while (nGrabs < NUM_GRABS)
			{
				int bufferIndex;              /* Index of the buffer. */

				/* Wait for the next buffer to be filled. Wait up to 1000 ms. */
				bool isReady = Pylon.WaitObjectWait(hWait, 1000);

				if (!isReady)
				{
					/* Timeout occurred. */
					throw new Exception("Grab timeout occurred.\n");
				}

				/* Since the wait operation was successful, the result of at least one grab 
				   operation is available. Retrieve it. */
				isReady = Pylon.StreamGrabberRetrieveResult(hGrabber, out grabResult);

				if (!isReady)
				{
					/* Oops. No grab result available? We should never have reached this point. 
					   Since the wait operation above returned without a timeout, a grab result 
					   should be available. */
					throw new Exception("Failed to retrieve a grab result.\n");
				}

				nGrabs++;

				/* Trigger the next image. Since we passed more than one buffer to the stream grabber, 
				   the triggered image will be grabbed while the image processing is performed.  */
				Pylon.DeviceExecuteCommandFeature(_pylonDevHandle, "TriggerSoftware");

				/* Get the buffer index from the context information. */
				bufferIndex = (int)grabResult.Context;

				/* Check to see if the image was grabbed successfully. */

				if (grabResult.Status == EPylonGrabStatus.Grabbed)
				{
					/*  The grab is successful.  */

					PylonBuffer<Byte> buffer;        /* Reference to the buffer attached to the grab result. */

					/* Get the buffer from the dictionary. Since we also got the buffer index, 
					   we could alternatively use an array, e.g. buffers[bufferIndex]. */
					if (!buffers.TryGetValue(grabResult.hBuffer, out buffer))
					{
						/* Oops. No buffer available? We should never have reached this point. Since all buffers are
						   in the dictionary. */
						throw new Exception("Failed to find the buffer associated with the handle returned in grab result.");
					}

					//Console.WriteLine("Grabbed frame {0} into buffer {1}.", nGrabs, bufferIndex);



					/* Check to see if we really got image data plus chunk data. */
					if (grabResult.PayloadType != EPylonPayloadType.PayloadType_ChunkData)
					{
						Console.WriteLine("Received a buffer not containing chunk data?");
					}
					else
					{
						/* Process the chunk data. This is done by passing the grabbed image buffer
						   to the chunk parser. When the chunk parser has processed the buffer, the chunk 
						   data can be accessed in the same manner as "normal" camera parameters. 
						   The only exception is the CRC checksum feature. There are dedicated functions for
						   checking the CRC checksum. */

						bool hasCRC;

						/* Let the parser extract the data. */
						Pylon.ChunkParserAttachBuffer(hChunkParser, buffer);


						/* Check the CRC checksum. */
						hasCRC = Pylon.ChunkParserHasCRC(hChunkParser);
						bool isOk = (hasCRC) ? Pylon.ChunkParserCheckCRC(hChunkParser) : true;

						bool isAvail = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "ChunkWidth");
						long chunkWidth = 0, chunkHeight = 0;
						//Console.WriteLine("Frame {0} {1} a frame width chunk.", nGrabs, isAvail ? "contains" : "doesn't contain");
						if (isAvail)
						{
							/* ... Get the value. */
							chunkWidth = Pylon.DeviceGetIntegerFeature(_pylonDevHandle, "ChunkWidth");

							//Console.WriteLine("Width of frame {0}: {1}.", nGrabs, chunkWidth);
						}

						/* Retrieve the chunk height value. */
						/* ... Check the availability. */
						isAvail = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "ChunkHeight");

						Console.WriteLine("Frame {0} {1} a frame height chunk.", nGrabs, isAvail ? "contains" : "doesn't contain");
						if (isAvail)
						{
							/* ... Get the value. */
							chunkHeight = Pylon.DeviceGetIntegerFeature(_pylonDevHandle, "ChunkHeight");

							//Console.WriteLine("Height of frame {0}: {1}.", nGrabs, chunkHeight);
						}

						if (isOk)
						{
							_latestImage = new HImage("byte", (int)chunkWidth, (int)chunkHeight, buffer.Pointer);
						}
					}
					/* Before requeueing the buffer, you should detach it from the chunk parser. */
					Pylon.ChunkParserDetachBuffer(hChunkParser);  /* Now the chunk data in the buffer is no longer accessible. */
				}
				else if (grabResult.Status == EPylonGrabStatus.Failed)
				{
					Console.Error.WriteLine("Frame {0} wasn't grabbed successfully.  Error code = {1}", nGrabs, grabResult.ErrorCode);
				}

				/* Once finished with the processing, requeue the buffer to be filled again. */
				Pylon.StreamGrabberQueueBuffer(hGrabber, grabResult.hBuffer, bufferIndex);
			}
		}

		private void setChunkModeFeatures()
		{
			/* Before enabling individual chunks, the chunk mode in general must be activated. */
			bool isAvail = Pylon.DeviceFeatureIsWritable(_pylonDevHandle, "ChunkModeActive");

			if (!isAvail)
			{
				throw new Exception("The device doesn't support the chunk mode.");
			}

			/* Activate the chunk mode. */
			Pylon.DeviceSetBooleanFeature(_pylonDevHandle, "ChunkModeActive", true);

			/* Enable some individual chunks... */

			/* ... The frame counter chunk feature. */
			/* Is the chunk feature available? */
			isAvail = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "EnumEntry_ChunkSelector_Framecounter");

			if (isAvail)
			{
				/* Select the frame counter chunk feature. */
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "ChunkSelector", "Framecounter");

				/* Can the chunk feature be activated? */
				isAvail = Pylon.DeviceFeatureIsWritable(_pylonDevHandle, "ChunkEnable");

				if (isAvail)
				{
					/* Activate the chunk feature. */
					Pylon.DeviceSetBooleanFeature(_pylonDevHandle, "ChunkEnable", true);
				}
			}
			/* ... The CRC checksum chunk feature. */
			/*  Note: Enabling the CRC checksum chunk feature is not a prerequisite for using
			   chunks. Chunks can also be handled when the CRC checksum chunk feature is disabled. */
			isAvail = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "EnumEntry_ChunkSelector_PayloadCRC16");

			if (isAvail)
			{
				/* Select the CRC checksum chunk feature. */
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "ChunkSelector", "PayloadCRC16");

				/* Can the chunk feature be activated? */
				isAvail = Pylon.DeviceFeatureIsWritable(_pylonDevHandle, "ChunkEnable");

				if (isAvail)
				{
					/* Activate the chunk feature. */
					Pylon.DeviceSetBooleanFeature(_pylonDevHandle, "ChunkEnable", true);
				}
			}
		}

		private void singleFrameGrabMethod()
		{
			PylonGrabResult_t grabResult;
			PylonBuffer<Byte> imgBuf = null;  /* Buffer used for grabbing. */

			try
			{
				/* Grab one single frame from stream channel 0. The 
				camera is set to "single frame" acquisition mode.
				Wait up to 500 ms for the image to be grabbed. 
				If imgBuf is null a buffer is automatically created with the right size.*/

				var channel = 0;
				if (!Pylon.DeviceGrabSingleFrame(_pylonDevHandle, channel, ref imgBuf, out grabResult, _singleFrameWaitup))
				{
					/* Timeout occurred. */
					_latestMessage = "Timeout";
				}

				/* Check to see if the image was grabbed successfully. */
				if (grabResult.Status == EPylonGrabStatus.Grabbed)
				{
					/* Success. Perform image processing. */
					_latestImage = new HImage("byte", grabResult.SizeX, grabResult.SizeY, imgBuf.Pointer);
				}
				else if (grabResult.Status == EPylonGrabStatus.Failed)
				{
					_latestMessage = "Frame wasn't grabbed successfully.  Error code = " + grabResult.ErrorCode;
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				/* Release the buffer. */
				imgBuf.Dispose();

				/* Free memory for grabbing. */
				imgBuf = null;
			}
		}

		const uint NUM_BUFFERS = 5;         /* Number of buffers used for grabbing. */
		private void streamdGrabModelStartMethod()
		{
			bool isReady;

			int bufferIndex;                /* Index of the buffer. */
			/* Wait for the next buffer to be filled. Wait up to 1000 ms. */
			isReady = Pylon.WaitObjectWait(hWait, 200);

			if (!isReady)
			{
				/* Timeout occurred. */
				throw new Exception("Grab timeout occurred.");
			}

			/* Since the wait operation was successful, the result of at least one grab 
			   operation is available. Retrieve it. */
			isReady = Pylon.StreamGrabberRetrieveResult(hGrabber, out grabResult);

			if (!isReady)
			{
				/* Oops. No grab result available? We should never have reached this point. 
				   Since the wait operation above returned without a timeout, a grab result 
				   should be available. */
				throw new Exception("Failed to retrieve a grab result");
			}


			/* Get the buffer index from the context information. */
			bufferIndex = (int)grabResult.Context;

			/* Check to see if the image was grabbed successfully. */
			if (grabResult.Status == EPylonGrabStatus.Grabbed)
			{
				/*  Success. Perform image processing. Since we passed more than one buffer
				to the stream grabber, the remaining buffers are filled in the background while
				we do the image processing. The processed buffer won't be touched by
				the stream grabber until we pass it back to the stream grabber. */

				PylonBuffer<Byte> buffer;        /* Reference to the buffer attached to the grab result. */

				/* Get the buffer from the dictionary. Since we also got the buffer index, 
				   we could alternatively use an array, e.g. buffers[bufferIndex]. */
				if (!buffers.TryGetValue(grabResult.hBuffer, out buffer))
				{
					/* Oops. No buffer available? We should never have reached this point. Since all buffers are
					   in the dictionary. */
					throw new Exception("Failed to find the buffer associated with the handle returned in grab result.");
				}
				var width = (int)Pylon.DeviceGetIntegerFeature(_pylonDevHandle, "Width");
				var height = (int)Pylon.DeviceGetIntegerFeature(_pylonDevHandle, "Height");

				/* Perform processing. */
				_latestImage = new HImage("byte", width, height, buffer.Pointer);

			}
			else if (grabResult.Status == EPylonGrabStatus.Failed)
			{
				_latestMessage = String.Format("Frame wasn't grabbed successfully.  Error code = {0}",
				grabResult.ErrorCode);
			}

			/* Once finished with the processing, requeue the buffer to be filled again. */
			Pylon.StreamGrabberQueueBuffer(hGrabber, grabResult.hBuffer, bufferIndex);
		}

		private void initStreamMode(uint NUM_BUFFERS, out PYLON_STREAMGRABBER_HANDLE hGrabber, out PYLON_WAITOBJECT_HANDLE hWait, out Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> buffers)
		{
			uint numDevices;               /* Number of available devices. */
			/* Handle for the pylon stream grabber. */
			/* Handle used for waiting for a grab to be finished. */
			uint payloadSize;              /* Size of an image frame in bytes. */
			/* Holds handles and buffers used for grabbing. */
			/* Stores the result of a grab operation. */
			/* Counts the number of buffers grabbed. */
			uint nStreams;                 /* The number of streams provides by the device.  */
			bool isAvail;                  /* Used for checking feature availability. */
			/* Used as an output parameter. */
			int i;                        /* Counter. */

#if DEBUG
                /* This is a special debug setting needed only for GigE cameras.
                See 'Building Applications with pylon' in the programmer's guide. */
                Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "300000" /*ms*/);
#endif


			isAvail = setStreamModeFeature();

			//*******************************設定 Stream 參數 Start *****************************************************************************
			/* Determine the required size of the grab buffer. */
			payloadSize = checked((uint)Pylon.DeviceGetIntegerFeature(_pylonDevHandle, "PayloadSize"));



			/* Get the number of streams supported by the device and the transport layer. */
			nStreams = Pylon.DeviceGetNumStreamGrabberChannels(_pylonDevHandle);

			if (nStreams < 1)
			{
				throw new Exception("The transport layer doesn't support image streams.");
			}

			/* Create and open a stream grabber for the first channel. */
			hGrabber = Pylon.DeviceGetStreamGrabber(_pylonDevHandle, 0);
			Pylon.StreamGrabberOpen(hGrabber);

			/* Get a handle for the stream grabber's wait object. The wait object
			   allows waiting for buffers to be filled with grabbed data. */
			hWait = Pylon.StreamGrabberGetWaitObject(hGrabber);

			/* We must tell the stream grabber the number and size of the buffers 
				we are using. */
			/* .. We will not use more than NUM_BUFFERS for grabbing. */
			Pylon.StreamGrabberSetMaxNumBuffer(hGrabber, NUM_BUFFERS);

			/* .. We will not use buffers bigger than payloadSize bytes. */
			Pylon.StreamGrabberSetMaxBufferSize(hGrabber, payloadSize);

			/*  Allocate the resources required for grabbing. After this, critical parameters 
				that impact the payload size must not be changed until FinishGrab() is called. */
			Pylon.StreamGrabberPrepareGrab(hGrabber);

			/* Before using the buffers for grabbing, they must be registered at
			   the stream grabber. For each registered buffer, a buffer handle
			   is returned. After registering, these handles are used instead of the
			   buffer objects pointers. The buffer objects are held in a dictionary,
			   that provides access to the buffer using a handle as key.
			 */
			buffers = new Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>>();
			for (i = 0; i < NUM_BUFFERS; ++i)
			{
				PylonBuffer<Byte> buffer = new PylonBuffer<byte>(payloadSize, true);
				PYLON_STREAMBUFFER_HANDLE handle = Pylon.StreamGrabberRegisterBuffer(hGrabber, ref buffer);
				buffers.Add(handle, buffer);
			}

			/* Feed the buffers into the stream grabber's input queue. For each buffer, the API 
			   allows passing in an integer as additional context information. This integer
			   will be returned unchanged when the grab is finished. In our example, we use the index of the 
			   buffer as context information. */
			i = 0;
			foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in buffers)
			{
				Pylon.StreamGrabberQueueBuffer(hGrabber, pair.Key, i++);
			}
		}

		private bool setStreamModeFeature()
		{
			bool isAvail;
			/* Set the pixel format to Mono8, where gray values will be output as 8 bit values for each pixel. */
			/* ... Check first to see if the device supports the Mono8 format. */
			isAvail = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "EnumEntry_PixelFormat_Mono8");

			if (!isAvail)
			{
				/* Feature is not available. */
				throw new Exception("Device doesn't support the Mono8 pixel format.");
			}
			/* ... Set the pixel format to Mono8. */
			Pylon.DeviceFeatureFromString(_pylonDevHandle, "PixelFormat", "Mono8");

			/* Disable acquisition start trigger if available */
			isAvail = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "EnumEntry_TriggerSelector_AcquisitionStart");
			if (isAvail)
			{
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerSelector", "AcquisitionStart");
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerMode", "Off");
			}

			/* Disable frame start trigger if available */
			isAvail = Pylon.DeviceFeatureIsAvailable(_pylonDevHandle, "EnumEntry_TriggerSelector_FrameStart");
			if (isAvail)
			{
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerSelector", "FrameStart");
				Pylon.DeviceFeatureFromString(_pylonDevHandle, "TriggerMode", "Off");
			}

			/* We will use the Continuous frame mode, i.e., the camera delivers
			images continuously. */
			Pylon.DeviceFeatureFromString(_pylonDevHandle, "AcquisitionMode", "Continuous");

			/* For GigE cameras, we recommend increasing the packet size for better 
			   performance. When the network adapter supports jumbo frames, set the packet 
			   size to a value > 1500, e.g., to 8192. In this sample, we only set the packet size
			   to 1500. */
			/* ... Check first to see if the GigE camera packet size parameter is supported and if it is writable. */
			isAvail = Pylon.DeviceFeatureIsWritable(_pylonDevHandle, "GevSCPSPacketSize");
			if (isAvail)
			{
				/* ... The device supports the packet size feature. Set a value. */
				Pylon.DeviceSetIntegerFeature(_pylonDevHandle, "GevSCPSPacketSize", 8192);
			}
			return isAvail;
		}

		#region IGrabImage 實作
		/*******************************************************************************************/

		public void OneShot()
		{
			DeviceOpen();

			SetPylonDeviceHandleFeatures(_simpleGrabFeatures);

			singleFrameGrabMethod();

			DeviceClose();
		}

		/// <summary>
		/// 設定抓取影像驅動方式
		/// </summary>
		public void SetGrabbingImageDrivenMode()
		{
		}


		public void SetGrabImageMode(GrabMode mode)
		{

		}

		public void ContinuouslyGrab()
		{
			DeviceOpen();

			_grabImageInitMethod = new PylonGrabImageInitHandler(streamGrabModeInitMethod);
			_grabImageStartMethod = new PylonGrabImageStartHandler(streamdGrabModelStartMethod);
			_grabImageCleanMethod = new PylonGrabImageCleanHandler(streamGrabModeCleanMethod);

			streamGrabModeInitMethod();
			if (!_bgworker.IsBusy)
			{
				/* Let the camera acquire images. */
				Pylon.DeviceExecuteCommandFeature(_pylonDevHandle, "AcquisitionStart");

				_bgworker.RunWorkerAsync();
			}
		}

		private void streamGrabModeInitMethod()
		{
			initStreamMode(NUM_BUFFERS, out hGrabber, out hWait, out buffers);
		}

		private void streamGrabModeCleanMethod()
		{
			//Clean
			/* Clean up. */

			/*  ... Stop the camera. */
			Pylon.DeviceExecuteCommandFeature(_pylonDevHandle, "AcquisitionStop");

			/* ... We must issue a cancel call to ensure that all pending buffers are put into the
			   stream grabber's output queue. */
			Pylon.StreamGrabberCancelGrab(hGrabber);

			/* ... The buffers can now be retrieved from the stream grabber. */
			bool isReady;
			do
			{
				isReady = Pylon.StreamGrabberRetrieveResult(hGrabber, out grabResult);

			} while (isReady);

			/* ... When all buffers are retrieved from the stream grabber, they can be deregistered.
				   After deregistering the buffers, it is safe to free the memory. */

			foreach (KeyValuePair<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> pair in buffers)
			{
				Pylon.StreamGrabberDeregisterBuffer(hGrabber, pair.Key);
				pair.Value.Dispose();
			}
			buffers = null;

			/* ... Release grabbing related resources. */
			Pylon.StreamGrabberFinishGrab(hGrabber);

			/* After calling PylonStreamGrabberFinishGrab(), parameters that impact the payload size (e.g., 
			the AOI width and height parameters) are unlocked and can be modified again. */

			/* ... Close the stream grabber. */
			Pylon.StreamGrabberClose(hGrabber);

			DeviceClose();
		}

		public void Stop()
		{
			Pylon.DeviceExecuteCommandFeature(_pylonDevHandle, "AcquisitionStop");
		}
		#endregion

		#region IDispsable 實作
		/*******************************************************************************************/

		public void Dispose()
		{
			/* Shut down the pylon runtime system. Don't call any pylon method after 
				   calling Pylon.Terminate(). */

			DeviceClose();
			Pylon.DestroyDevice(_pylonDevHandle);
			Pylon.Terminate();  /* Releases all pylon resources. */
		}
		#endregion

	}
}
