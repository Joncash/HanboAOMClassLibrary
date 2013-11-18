using HalconDotNet;
using PylonC.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hanbo.Image.Grab
{
	public enum PylonDrivenMode { Chunk, SingleFrame, Stream };
	internal delegate void PylonGrabImageInitHandler();
	internal delegate void PylonGrabImageStartHandler();
	internal delegate void PylonGrabImageCleanHandler();
	internal delegate void PylonGrabImageStopHandler();
	public class PylonImageProvider : IDisposable
	{
		public PylonImageProvider()
		{
			init();
		}

		#region 公有屬性
		public bool Connected { get { return _isConnected; } }
		public HImage Image { get { return _lastestImage; } }
		public int GevSCPSPacketSize = 8192;
		#endregion

		#region 公開方法
		public string GetLatestMessage()
		{
			return _lastestMessage;
		}

		public void Connect()
        {
            Hanbo.Log.LogManager.Debug("Pylon Connect");
			if (hDev == null)
			{
                Hanbo.Log.LogManager.Debug("Pylon Device IS Null");
				hDev = createDevice();
			}
			if (hDev != null)
			{
                Hanbo.Log.LogManager.Debug("Pylon Device IS Not Null");
				_isConnected = Pylon.DeviceIsOpen(hDev);
                Hanbo.Log.LogManager.Debug("Pylon DeviceIsOpen Check :" + _isConnected);
				if (!_isConnected)
				{
                    Hanbo.Log.LogManager.Debug("Pylon DeviceOpen");
					Pylon.DeviceOpen(hDev, Pylon.cPylonAccessModeControl | Pylon.cPylonAccessModeStream);
                    Hanbo.Log.LogManager.Debug("Pylon resetDriveModeFeatures");
					resetDrivenModeFeatures();
				}
				_isConnected = Pylon.DeviceIsOpen(hDev);
			}
		}

		private PYLON_DEVICE_HANDLE createDevice()
		{
			PYLON_DEVICE_HANDLE device = null;
			numDevices = Pylon.EnumerateDevices();
			if (0 == numDevices)
			{
                Hanbo.Log.LogManager.Debug("找不到影像裝置");
				_lastestMessage = "找不到影像裝置";
			}
			else
			{
                Hanbo.Log.LogManager.Debug("建立 Pylon Device Handle");
				device = Pylon.CreateDeviceByIndex(0);
			}
			return device;
		}
		public void DisConnect()
		{
			if (clearMethod != null)
			{
				clearMethod();
			}
			Pylon.DeviceClose(hDev);
			Pylon.DestroyDevice(hDev);
		}
		public void Stop()
		{
			if (stopMethod != null)
			{
				stopMethod();
			}
		}

		/// <summary>
		/// 取得影像
		/// </summary>
		public void GrabImage()
		{
			if (grabImage != null && _isModelValid && _isConnected)
			{
				_startFlag = true;
				grabImage();
			}
		}

		public void SetFeatures(object features)
		{
			throw new NotImplementedException();
		}

		public bool IsCurrentModelValid()
		{
			return _isModelValid;
		}
		public void SetDrivenMode(PylonDrivenMode mode)
		{
			_drivenMode = mode;
		}

		private void initStreamMode()
		{
			_width = (int)Pylon.DeviceGetIntegerFeature(hDev, "Width");
			_height = (int)Pylon.DeviceGetIntegerFeature(hDev, "Height");
			/* Print out the name of the camera we are using. */
			bool isReadable = Pylon.DeviceFeatureIsReadable(hDev, "DeviceModelName");
			if (isReadable)
			{
				string name = Pylon.DeviceFeatureToString(hDev, "DeviceModelName");
				Console.WriteLine("Using camera {0}.", name);
			}

			/* Set the pixel format to Mono8, where gray values will be output as 8 bit values for each pixel. */
			/* ... Check first to see if the device supports the Mono8 format. */
			isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_PixelFormat_Mono8");

			if (!isAvail)
			{
				/* Feature is not available. */
				throw new Exception("Device doesn't support the Mono8 pixel format.");
			}
			/* ... Set the pixel format to Mono8. */
			bool isWritable = Pylon.DeviceFeatureIsWritable(hDev, "PixelFormat");
			if (isWritable)
				Pylon.DeviceFeatureFromString(hDev, "PixelFormat", "Mono8");

			/* Disable acquisition start trigger if available */
			isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_TriggerSelector_AcquisitionStart");
			if (isAvail)
			{
				isWritable = Pylon.DeviceFeatureIsWritable(hDev, "TriggerSelector");
				if (isWritable)
					Pylon.DeviceFeatureFromString(hDev, "TriggerSelector", "AcquisitionStart");

				isWritable = Pylon.DeviceFeatureIsWritable(hDev, "TriggerMode");
				if (isWritable)
					Pylon.DeviceFeatureFromString(hDev, "TriggerMode", "Off");
			}

			/* Disable frame start trigger if available */
			isAvail = Pylon.DeviceFeatureIsAvailable(hDev, "EnumEntry_TriggerSelector_FrameStart");
			if (isAvail)
			{
				isWritable = Pylon.DeviceFeatureIsWritable(hDev, "TriggerSelector");
				if (isWritable)
					Pylon.DeviceFeatureFromString(hDev, "TriggerSelector", "FrameStart");

				isWritable = Pylon.DeviceFeatureIsWritable(hDev, "TriggerMode");
				if (isWritable)
					Pylon.DeviceFeatureFromString(hDev, "TriggerMode", "Off");
			}

			/* We will use the Continuous frame mode, i.e., the camera delivers
			images continuously. */
			isWritable = Pylon.DeviceFeatureIsWritable(hDev, "AcquisitionMode");
			if (isWritable)
				Pylon.DeviceFeatureFromString(hDev, "AcquisitionMode", "Continuous");

			/* For GigE cameras, we recommend increasing the packet size for better 
			   performance. When the network adapter supports jumbo frames, set the packet 
			   size to a value > 1500, e.g., to 8192. In this sample, we only set the packet size
			   to 1500. */
			/* ... Check first to see if the GigE camera packet size parameter is supported and if it is writable. */
			isAvail = Pylon.DeviceFeatureIsWritable(hDev, "GevSCPSPacketSize");
			if (isAvail)
			{
				/* ... The device supports the packet size feature. Set a value. */
				isWritable = Pylon.DeviceFeatureIsWritable(hDev, "GevSCPSPacketSize");
				if (isWritable)
					Pylon.DeviceSetIntegerFeature(hDev, "GevSCPSPacketSize", GevSCPSPacketSize);
			}
			_isModelValid = isAvail;
			/* Determine the required size of the grab buffer. */
			payloadSize = checked((uint)Pylon.DeviceGetIntegerFeature(hDev, "PayloadSize"));

			/* Image grabbing is done using a stream grabber.  
			  A device may be able to provide different streams. A separate stream grabber must 
			  be used for each stream. In this sample, we create a stream grabber for the default 
			  stream, i.e., the first stream ( index == 0 ).
			  */

			/* Get the number of streams supported by the device and the transport layer. */
			nStreams = Pylon.DeviceGetNumStreamGrabberChannels(hDev);

			if (nStreams < 1)
			{
				throw new Exception("The transport layer doesn't support image streams.");
			}

			/* Create and open a stream grabber for the first channel. */
			hGrabber = Pylon.DeviceGetStreamGrabber(hDev, 0);
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
			int i;
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
		private void streamModeStartMethod()
		{
			if (_startFlag)
			{
				Pylon.DeviceExecuteCommandFeature(hDev, "AcquisitionStart");
				_startFlag = false;
			}
			int bufferIndex;                /* Index of the buffer. */
			/* Wait for the next buffer to be filled. Wait up to 1000 ms. */
			isReady = Pylon.WaitObjectWait(hWait, 1000);

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

			nGrabs++;

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

				/* Perform processing. */
				_lastestImage = new HImage("byte", _width, _height, buffer.Pointer);

			}
			else if (grabResult.Status == EPylonGrabStatus.Failed)
			{
				Console.Error.WriteLine("Frame {0} wasn't grabbed successfully.  Error code = {1}",
					nGrabs, grabResult.ErrorCode);
			}

			/* Once finished with the processing, requeue the buffer to be filled again. */
			Pylon.StreamGrabberQueueBuffer(hGrabber, grabResult.hBuffer, bufferIndex);
		}
		private void streamModeStopMethod()
		{
			Pylon.DeviceExecuteCommandFeature(hDev, "AcquisitionStop");
		}
		private void streamModeClearMethod()
		{
			streamModeStopMethod();

			/* ... We must issue a cancel call to ensure that all pending buffers are put into the
			   stream grabber's output queue. */
			Pylon.StreamGrabberCancelGrab(hGrabber);

			/* ... The buffers can now be retrieved from the stream grabber. */
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
		}


		private void initSingleFrameMode()
		{
			throw new NotImplementedException();
		}


		private void initChunkMode()
		{
			throw new NotImplementedException();
		}

		private void resetDrivenModeFeatures()
		{
            Hanbo.Log.LogManager.Debug("Pylon DriveMode: " + _drivenMode);
			switch (_drivenMode)
			{
				case PylonDrivenMode.Chunk:
					initChunkMode();
					break;
				case PylonDrivenMode.SingleFrame:
					initSingleFrameMode();
					break;
				case PylonDrivenMode.Stream:
					initStreamMode();
					grabImage = new PylonGrabImageStartHandler(streamModeStartMethod);
					clearMethod = new PylonGrabImageCleanHandler(streamModeClearMethod);
					stopMethod = new PylonGrabImageStopHandler(streamModeStopMethod);
					break;
			}
		}

		#endregion


		#region 私有屬性
		private bool _isModelValid = false;
		private bool _isConnected = false;
		private bool _startFlag = false;
		private int _width;
		private int _height;
		private string _lastestMessage;
		private PylonGrabImageStartHandler grabImage;
		private PylonGrabImageCleanHandler clearMethod;
		private PylonGrabImageStopHandler stopMethod;
		private HImage _lastestImage;
		private PylonDrivenMode _drivenMode = PylonDrivenMode.Stream; //預設為 Stream

		private const uint NUM_BUFFERS = 5;         /* Number of buffers used for grabbing. */
		private PYLON_DEVICE_HANDLE hDev = null;
		private uint numDevices;               /* Number of available devices. */
		private PYLON_STREAMGRABBER_HANDLE hGrabber;  /* Handle for the pylon stream grabber. */
		private PYLON_WAITOBJECT_HANDLE hWait;        /* Handle used for waiting for a grab to be finished. */
		private uint payloadSize;              /* Size of an image frame in bytes. */
		private Dictionary<PYLON_STREAMBUFFER_HANDLE, PylonBuffer<Byte>> buffers; /* Holds handles and buffers used for grabbing. */
		private PylonGrabResult_t grabResult;               /* Stores the result of a grab operation. */
		private int nGrabs;                   /* Counts the number of buffers grabbed. */
		private uint nStreams;                 /* The number of streams provides by the device.  */
		private bool isAvail;                  /* Used for checking feature availability. */
		private bool isReady;                  /* Used as an output parameter. */
		#endregion
		#region 私有方法
		private void init()
		{
			Pylon.Initialize();
		}
		#endregion

		public void Dispose()
		{
			/* ... Shut down the pylon runtime system. Don't call any pylon method after 
				   calling Pylon.Terminate(). */
			Pylon.Terminate();
		}
	}
}
