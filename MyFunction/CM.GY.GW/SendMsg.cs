using System;
using System.Data;
using System.IO;

namespace CM.GY.GW
{
    class SendMsg
    {
        /// <summary>
        /// 写日志方法
        /// </summary>
        /// <param name="sMsg">日志内容</param>
        public static void WriteLog(string sMsg)
        {
            //在服务目录下创建日志文件夹
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory.ToString() + "/Log/");
            //根据日期生成日志文件
            string sPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "/Log/" + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
            //写日志
            WebUse.Logs.WriteLog(sPath, sMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Send()
        {
            try
            {
                DatabaseORC db = new DatabaseORC();
                string SQL = "";
                DataTable dt = db.GetDataSet(SQL).Tables[0];
                for(int i=0;i<dt.Rows.Count;++i)
                {
                    //遍历查到的数据，逐个更新发送的通知
                }
            }
            catch(Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 向普通用户发送自己的未签收文件情况
        /// </summary>
        /// <returns></returns>
        private static bool SendToUser(uint userID)
        {
            try
            {

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        private static bool SendToAdmin(uint userID)
        {
            try
            {

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int SleepTime()
        {
            try
            {
                int iSleepTime = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SleepTime"].ToString()) * 1000 * 60;//休眠时间单位为分钟
                return iSleepTime;
            }
            catch (Exception oExcept)
            {
                WriteLog("获取服务休眠时间错误，错误信息如下：" + oExcept.Message);
                return 24 * 60 * 60 * 1000;//默认每天运行一次
            }
        }
    }
}
