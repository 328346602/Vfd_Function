﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace CM.GY.DownloadFile
{
    /// <summary>
    /// 写日志方法
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteLog(string sMsg)
        {
            Write("Log", sMsg);
        }

        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteError(string sMsg)
        {
            Write("Error", sMsg);
        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteDebug(string sMsg)
        {
            string debug = string.Empty;
            try
            {
                debug = System.Configuration.ConfigurationManager.AppSettings["Debug"].ToString();
            }
            catch (Exception)
            {
                debug = "false";
            }

            if (debug == "true")
            {
                Write("Debug", sMsg);
            }
            //Write("Debug", sMsg);
        }

        /// <summary>
        /// 写异常信息
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteException(Exception ex)
        {
            Write("Exception", ex.Message);
        }

        /// <summary>
        /// 写日志基础方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sMsg"></param>
        public static void Write(string type, string sMsg)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Path.Combine(path, "Logs");

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            //string fileFullName = System.IO.Path.Combine(path, string.Format("{0}.log", DateTime.Now.ToString("yyMMdd-HHmmss")));
            string fileFullName = System.IO.Path.Combine(path, string.Format("{0}{1}.log", type, DateTime.Now.ToString("yyMMdd")));

            using (StreamWriter output = System.IO.File.AppendText(fileFullName))
            {
                output.WriteLine(DateTime.Now.ToString() + ">>>>" + sMsg);

                output.Close();
            }
        }
    }
}