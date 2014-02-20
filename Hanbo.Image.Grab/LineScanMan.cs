using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Hanbo.Image.Grab
{
    public class LineScanMan
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
        private bool _initSuccess = true;
        #endregion


        public LineScanMan()
        {
            initialize();
        }

        #region Initial===============================================================================================

        private void initialize()
        {
            initStatus();
            setStatus(GrabInstruction.Initialize, GrabStage.Connected, GrabState.Idle);
            initializeBackgroundWorker();
            notifyStateChange(_bgworker, new GrabImageStatusChangedEventArgs() { Status = this.Status });
        }
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
            //noitfyGrabImage(e.UserState as HImage);
        }

        private void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
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
                        Thread.Sleep(200);
                        worker.ReportProgress(0, "");
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


        }

        #endregion

        #region 設定======================================================================================================

        private void prepareDevice()
        {

        }

        private void setStreamGrabber()
        {

        }

        private void setFreeRunParams()
        {

        }

        /// <summary>
        /// setPEGParams
        /// </summary>
        /// <param name="pegW">Pixel Width ( 影像寬度 pixel)</param>
        /// <param name="pegH">Pixel Height ( 影像長度 pixel, 多少 pixel 出一張圖)</param>
        private void setPEGParams(long pegW, long pegH)
        {

        }

        #endregion

        #region 釋放======================================================================================================
        private bool releaseStreamAndBuffers()
        {
            bool isRelease = true;
            return isRelease;
        }

        private void releaseResource()
        {

        }

        private void stopGrab()
        {

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

        }

        public string[] GetSettings()
        {
            return new string[] { };
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
