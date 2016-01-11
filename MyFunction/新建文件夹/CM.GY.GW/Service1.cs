using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System;

namespace CM.GY.GW
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Proccess.Start();
            }
            catch (Exception ex)
            {
                Log.WriteDebug("OnStart Error>>>>>" + ex.Message);
                throw ex;
            }
        }

        protected override void OnStop()
        {
        }



        public static class Proccess
        {
            public static void Start()
            {
                try
                {
                    ThreadStart start = new ThreadStart(ThreadAction);
                    Thread th = new Thread(start);
                    th.IsBackground = true;
                    th.Start();
                }
                catch(Exception ex)
                {
                    Log.WriteDebug("Start Error>>>>>" + ex.Message);
                    throw ex;
                }
                   
            }
            public static void ThreadAction()
            {
                while (true)
                {
                    try
                    {
                        SendMsg.Exec(); //发送短信
                        System.Threading.Thread.Sleep(SendMsg.SleepTime());//进程睡眠
                    }
                    catch (Exception ex)
                    {
                        Log.WriteDebug("ThreadAction Error>>>>>" + ex.Message);
                    }
                }
            }

        }


    }
}
