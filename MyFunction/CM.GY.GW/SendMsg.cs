﻿using System;
using System.Data;
using System.IO;



namespace CM.GY.GW
{
    class SendMsg
    {

        /// <summary>
        /// 
        /// </summary>
        public static void Exec()
        {
            try
            {
                DatabaseORC db = new DatabaseORC();
                string SQL = "select t.receiveid as \"接收人\",count(t.signflag)-sum(t.signflag)  as \"已签数目\" ,count(t.signflag) as \"案卷总数\" from cm_lc_sign t group by t.receiveid ";
                DataSet ds = db.GetDataSet(SQL);
                DataRow dr;
                if(ds.Tables.Count>0)
                {
                    DataTable dt = db.GetDataSet(SQL).Tables[0];
                    if(dt.Rows.Count>0)
                    {
                        for (int i = 0; i < dt.Rows.Count; ++i)
                        {
                            dr = dt.Rows[i];
                            if (int.Parse(dr[1].ToString()) != 0)//遍历查到的数据，若未签数目！=0，则发送通知
                            {
                                SendToUser(uint.Parse(dr[0].ToString()), db, dr);
                            }
                        }
                    }
                }
                return;
            }
            catch(Exception ex)
            {
                Log.WriteError("Exec>>>>>" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 向普通用户发送自己的未签收文件情况
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="db"></param>
        /// <param name="dr"></param>
        private static void SendToUser(uint userID,DatabaseORC db,DataRow dr)
        {
            try
            {
                ///查询
                string SQL = "select t.showflg from messagesys t where t.msgtype='签收统计' and t.showflg=0 and t.receiveid=" + userID;
                DataSet ds = db.GetDataSet(SQL);
                if(ds.Tables.Count>0)
                {
                    DataTable dt = ds.Tables[0];
                    if(dt.Rows.Count>0)
                    {
                        Update(userID, db,dr);//当前有未查看的系统消息时，update
                    }
                    else
                    {
                        Insert(userID, db,dr);//当前没有未查看的系统消息时，insert
                    }
                }
            }
            catch(Exception ex)
            {
                Log.WriteError("SendToUser>>>>>" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="db"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static bool Update(uint userID,DatabaseORC db,DataRow dr)
        {
            try
            {
                //uint numUnRead = uint.Parse(dr[2].ToString()) - uint.Parse(dr[1].ToString());
                string Msg = "共收到文件" + dr[2] + "个，其中" + dr[1] + "个未读，点击查看详情。";
                string MsgInfo = "&nbsp;&nbsp;&nbsp;&nbsp;<a class='grid-pager' href='#' onclick='top.redirectUrl(\"SubSysFlowFile/BanGongShi/SendFiles/MyFilesList.vfd?ENDTIME=2100-12-24&STARTTIME=2000-01-01&FAQIREN=&FILENAME=&JINJICHENGDU=&SIGNFLAG=\"); return false;'>" + Msg + "</a>";
                string SQL = "update messagesys t set t.cometime=to_date('" + DateTime.Now + "','yyyy-mm-dd hh24:mi:ss'), t.messageinfo='" + MsgInfo.Replace("'", "''") + "',t.message='" + Msg + "' where t.receiveid=" + dr[0] + " and t.msgtype='签收统计' and t.showflg=0";
                Log.WriteDebug("update>>>>>"+SQL);
                db.ExecuteSql(SQL);
                return true;
            }
            catch(Exception ex)
            {
                Log.WriteError("Update>>>>>" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="db"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private static bool Insert(uint userID,DatabaseORC db,DataRow dr)
        {
            try
            {
                string Msg = "共收到文件" + dr[2] + "个，其中" + dr[1] + "个未读，点击查看详情。";
                string MsgInfo = "&nbsp;&nbsp;&nbsp;&nbsp;<a class='grid-pager' href='#' onclick='top.redirectUrl(\"SubSysFlowFile/BanGongShi/SendFiles/MyFilesList.vfd?ENDTIME=2100-12-24&STARTTIME=2000-01-01&FAQIREN=&FILENAME=&JINJICHENGDU=&SIGNFLAG=\"); return false;'>" + Msg + "</a>";
                //MsgInfo.Replace("'","''");
                string SQL = "insert into messagesys t (t.receiveid,t.msgtype,t.messageinfo,t.message,t.cometime) values('" + dr[0] + "','签收统计','" + MsgInfo.Replace("'", "''") + "','" + Msg + "',to_date('" + DateTime.Now + "','yyyy-mm-dd hh24:mi:ss'))";
                //SQL.Replace("'","''");
                Log.WriteDebug("insert>>>>>" + SQL);
                db.ExecuteSql(SQL);
                return true;
            }
            catch(Exception ex)
            {
                Log.WriteError("Insert>>>>>" + ex.Message);
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
                Log.WriteDebug("SleepTime>>>>>"+iSleepTime);
                return iSleepTime;
            }
            catch (Exception oExcept)
            {
                Log.WriteError("SleepTime>>>>>" + oExcept.Message);
                return 24 * 60 * 60 * 1000;//默认每天运行一次
            }
        }

    }
}
