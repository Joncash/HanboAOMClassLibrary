using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanbo.Log
{
    /// <summary>
    /// 使用 NLog
    /// </summary>
    public static class LogManager
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Error(Exception ex)
        {
            logger.Error(ex);
        }
        public static void Error(string msg)
        {
            logger.Error(msg);
        }

        public static void Debug(Exception ex)
        {
            logger.Debug(ex);
        }
        public static void Debug(string msg)
        {
            logger.Debug(msg);
        }

        public static void Fatal(Exception ex)
        {
            logger.Fatal(ex);
        }
        public static void Fatal(string msg)
        {
            logger.Fatal(msg);
        }

        public static void Trace(Exception ex)
        {
            logger.Trace(ex);
        }
        public static void Trace(string msg)
        {
            logger.Trace(msg);
        }

        public static void Info(Exception ex)
        {
            logger.Info(ex);
        }
        public static void Info(string msg)
        {
            logger.Info(msg);
        }

        public static void Warn(Exception ex)
        {
            logger.Warn(ex);
        }
        public static void Warn(string msg)
        {
            logger.Warn(msg);
        }


    }
}
