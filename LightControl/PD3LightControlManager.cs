using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace LightControl
{
    /// <summary>
    /// 光源控制管理員
    /// </summary>
    public class PD3LightControlManager
    {
        public IPAddress ControllerIPAddress { get; set; }
        public int ControllerPort { get; set; }
        public IPEndPoint ControllerEndPoint;
        private Socket _client;

        /// <summary>
        /// Command 標頭
        /// </summary>
        protected string header = "@";

        /// <summary>
        /// Command 結束符號
        /// </summary>
        protected string delimiter = "\r\n";

        public PD3LightControlManager(IPAddress ipAddress, int port)
        {
            ControllerEndPoint = new IPEndPoint(ipAddress, port);
            this.ControllerIPAddress = ipAddress;
            this.ControllerPort = port;
        }

        /// <summary>
        /// 預設 IPAdress = 192.168.0.2; Port = 40001
        /// </summary>
        public PD3LightControlManager()
        {
            //Default
            this.ControllerIPAddress = new IPAddress(new byte[] { 192, 168, 0, 2 });
            this.ControllerPort = 40001;
            ControllerEndPoint = new IPEndPoint(this.ControllerIPAddress, this.ControllerPort);
        }
        public object GetSettingStatus()
        {
            return "";
        }

        public string StatusCheck()
        {
            var command = "00CE3";
            return sendData(header + command + delimiter);
        }

        /// <summary>
        /// 送出訊息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SendMessage(string data)
        {
            return sendData(data);
        }

        /// <summary>
        /// Reset
        /// </summary>
        /// <returns></returns>
        public string ResetAllChannel()
        {
            var command = "00RF2";
            return sendData(header + command + delimiter);
        }

        public string SetIpAddress(string ipaddress)
        {
            var instruction = "E01";
            var channel = "00";
            var command = header + channel + instruction + ipaddress;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }

        public string SetSubMask(string submask)
        {
            var instruction = "E02";
            var channel = "00";
            var command = header + channel + instruction + submask;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }
        public string SetDefaultGatway(string gatway)
        {
            var instruction = "E03";
            var channel = "00";
            var command = header + channel + instruction + gatway;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }
        public string SetReceptionPort(int port)
        {
            var instruction = "E04";
            var channel = "00";
            var command = header + channel + instruction + port;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }

        public string SetReplyIPAddress(string ipaddress)
        {
            var instruction = "E05";
            var channel = "00";
            var command = header + channel + instruction + ipaddress;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }
        public string SetReplyPort(int port)
        {
            var instruction = "E06";
            var channel = "00";
            var command = header + channel + instruction + port;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }

        /// <summary>
        /// 設定光源大小
        /// </summary>
        /// <param name="intensity"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string SetLightIntensity(string intensity, string channel)
        {
            var instruction = "F";
            int value;
            Int32.TryParse(intensity, out value);
            if (value < 0 || value > 255) return "";

            var command = header + channel + instruction + intensity;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }

        /// <summary>
        /// 設定光源模式
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string SetLightMode(string mode, string channel)
        {
            /*
             * mode range 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10
             */
            var instruction = "S";
            var command = header + channel + instruction + mode;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }

        /// <summary>
        /// 設定光源開關
        /// </summary>
        /// <param name="onOff"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public string SetLightOnOff(string onOff, string channel)
        {
            /*
             * On/Off : 1/0
             */
            var instruction = "L";
            var command = header + channel + instruction + onOff;
            var checksum = getCheckSum(command);
            return sendData(command + checksum + delimiter);
        }

        #region private
        private string getCheckSum(string command)
        {
            //取低位元 2 碼
            var checkSumCharArray = Encoding.ASCII.GetBytes(command)
                                    .Select(p => (int)p).Sum()
                                    .ToString("X").ToCharArray().Reverse().Take(2).Reverse().ToArray();
            return new string(checkSumCharArray);
        }
        private string sendData(string data)
        {
            byte[] bytes = new byte[1024];
            var replyMsg = "";
            try
            {
                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _client.Connect(this.ControllerEndPoint);
                byte[] msg = Encoding.ASCII.GetBytes(data);

                // Send the data through the socket.
                int bytesSent = _client.Send(msg);

                // Receive the response from the remote device.
                int bytesRec = _client.Receive(bytes);
                replyMsg = (Encoding.ASCII.GetString(bytes, 0, bytesRec));

                // Release the socket.
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
            }
            catch (ArgumentNullException ae)
            {
                replyMsg = ("ArgumentNullException : " + ae.ToString());
            }
            catch (SocketException se)
            {
                replyMsg = ("SocketException : " + se.ToString());
            }
            catch (Exception ex)
            {
                replyMsg = ("Unexpected exception : " + ex.ToString());
            }
            return replyMsg;
        }
        #endregion
    }

}
