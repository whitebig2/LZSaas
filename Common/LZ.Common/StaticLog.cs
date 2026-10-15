using log4net;
using System;
using System.Diagnostics;
using log4net.Repository;
using log4net.Config;
using System.IO;
using log4net.Appender;

namespace LZ.Common
{
    public  class StaticLog
    {
        public static ILoggerRepository repository = null;
        static StaticLog()
        {
            //log4net
            repository = LogManager.CreateRepository("NETCoreRepository");
            //指定配置文件
            XmlConfigurator.Configure(repository, new FileInfo("config/log4net.config"));
        }
        

        public static void Debug(object message)
        {
            LogManager.GetLogger(repository.Name, GetCurrentMethodFullName()).Debug(message);
        }

        public static void Debug(object message, Exception ex)
        {
            LogManager.GetLogger(repository.Name, GetCurrentMethodFullName()).Debug(message, ex);
        }

        public static void Error(object message)
        {
            LogManager.GetLogger(repository.Name, GetCurrentMethodFullName()).Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            LogManager.GetLogger(repository.Name, GetCurrentMethodFullName()).Error(message, exception);
        }

        private static string GetCurrentMethodFullName()
        {
            try
            {
                StackFrame frame;
                string str2;
                int num = 2;
                StackTrace trace = new StackTrace();
                int length = trace.GetFrames().Length;
                do
                {
                    frame = trace.GetFrame(num++);
                    str2 = frame.GetMethod().DeclaringType.ToString();
                }
                while (str2.EndsWith("Exception") && (num < length));
                string name = frame.GetMethod().Name;
                return (str2 + "." + name);
            }
            catch
            {
                return null;
            }
        }

        public static void Info(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName(), "").Info(message);
        }

        public static void Info(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName(), "").Info(message, ex);
        }

        public static void Warn(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName(), "").Warn(message);
        }

        public static void Warn(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName(), "").Warn(message, ex);
        }
    }
}
