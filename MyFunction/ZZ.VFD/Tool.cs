using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.IO;
using System.Data.OracleClient;

namespace Function
{
    /// <summary>
    /// 简单工具
    /// </summary>
    public class Tool
    {

        

        public static void ShowModalDialog(string sUrl, System.Web.UI.Page _Page)
        {
            try
            {
                string strScript = "<script language='javascript'>window.showModalDialog('" + sUrl + "')</script>";
                _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            }
            catch (Exception oExcept)
            {
                //Tool.WriteLog(oExcept.Message);
            }
        }

        

        public static void Alert(string Message, System.Web.UI.Page _Page)
        {
            try
            {
                string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
                _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            }
            catch (Exception oExcept)
            {
                Log.WriteError(oExcept.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Page"></param>
        public static void ShowLoading(System.Web.UI.Page _Page)
        {
            try
            {
                string strScript = "<script language='javascript'>showLoading();</script>";
                _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            }
            catch (Exception oExcept)
            {
                Log.WriteError(oExcept.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Page"></param>
        public static void HiddenLoading(System.Web.UI.Page _Page)
        {
            try
            {
                string strScript = "<script language='javascript'>hiddenLoading();</script>";
                _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            }
            catch (Exception oExcept)
            {
                Log.WriteError(oExcept.Message);
            }
        }


        public static void ShowSuspendMsg(string Msg,System.Web.UI.Page _Page)
        {
             try
            {
                string strScript = "<script language='javascript'>showMessage({ id: \"msgSuccess\", title: \"友情提示\", text: \""+Msg+"\" });</script>";
                _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            }
            catch (Exception oExcept)
            {
                Log.WriteError(oExcept.Message);
            }
                
        }


        public static void Alert(string Message, string id, string title, System.Web.UI.Page _Page)
        {
            string strScript = "<script language='javascript'>$(function(){var obj = { id: \"" + id + "\", title: \"" + title + "\", text: \"" + Message + "\"};showMessage(obj);})</script>";
            _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            Log.WriteError(strScript);
            //var obj = { id: "message", title: "提示", text: "请选择接收人！"};
            //showMessage(obj);
        }

        /// <summary>
        /// 发送系统消息方法
        /// </summary>
        /// <param name="sUserList">用户机构列表，例如"P-1,P-1-1,U-78,U-79,U-80"</param>
        /// <param name="sMsg">要发送的消息内容，例如"栾川县地质矿产局产学研基地竣工仪式通知"</param>
        /// <param name="sUrl">要发送的消息中包含的超链接地址，例如"SubSysFlowFile\BanGongShi\MeetingManagement\MeetingInfo.vfd?CASENO="</param>
        /// <param name="sCaseNo">传递进来的关键字，可与sUrl写在一起</param>
        /// <param name="sMsgType">发送消息备注类型</param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static string SendMessage(string sUserList, string sMsg, string sUrl, string sGuid, string sMsgType, System.Web.UI.Page _Page)
        {
            try
            {
                ArrayList arrSendedUsers = new ArrayList();//保存已经发送过的用户ID，避免重复发送
                ArrayList arrUserInGroup = new ArrayList();//保存根据机构代码查到的用户ID，分别发送
                string sSql = "select 用户ID from flow_user_role where 机构ID=(select 机构ID from flow_groups t where t.编码='";//根据机构查找用户代码的SQL
                DataSet ds = new DataSet();
                string sDateTime = DateTime.Now.ToString();
                DatabaseORC db = new DatabaseORC();
                string sReturn = string.Empty;
                string sUser = string.Empty;
                string sNoticeType = "会议";//通知类型
                #region sUserList中包含有全局选项时
                if (sUserList.Contains("P-1,") || sUserList == "P-1")
                {
                    ds = db.GetDataSet("select * from flow_users t");
                    for (int m = 0; m < ds.Tables[0].Rows.Count; m++)//循环向用户发送消息
                    {
                        sUser = ds.Tables[0].Rows[m][0].ToString();
                        sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                    }
                }
                #endregion

                #region sUserList包含逗号时
                else if (sUserList.Contains(","))//有逗号
                {
                    //Tool.WriteLog("有逗号:"+sUserList);
                    string[] strUserList = sUserList.Split(',');
                    for (int i = 0; i < strUserList.Length; i++)
                    {
                        if (strUserList[i].Split('-')[0] == "P")//取得机构ID时
                        {
                            sUser = strUserList[i].Remove(0, 2);
                            ds = db.GetDataSet(sSql + sUser + "')");
                            //Tool.WriteLog(sSql+sUser+"')");
                            //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                            {
                                sUser = ds.Tables[0].Rows[j][0].ToString();
                                if (arrSendedUsers.Contains("@" + ds.Tables[0].Rows[j][0] + "@") == false)//判断用户是否已收到消息，向未收到用户发送
                                {
                                    arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                    //Tool.WriteLog("第"+(j+1)+"次发送"+sUser);
                                    //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                    sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                    //sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString().Remove(0,2), sMsg, sDateTime, sMsgType, sUrl, _Page);

                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else if (strUserList[i].Split('-')[0] == "U")
                        {
                            sUser = strUserList[i].Remove(0, 2);
                            if (arrSendedUsers.Contains("@" + sUser + "@") == false)
                            {
                                arrSendedUsers.Add("@" + sUser + "@");
                                //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                //sReturn = Tool.SendMessage(db, strUserList[i], sMsg, sDateTime, sMsgType, sUrl, _Page);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                #endregion

                #region sUserList中无逗号
                else
                {
                    if (sUserList.Split('-')[0] == "P")
                    {
                        sUser = sUserList.Remove(0, 2);
                        ds = db.GetDataSet(sSql + sUser + "')");
                        //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                        {
                            if (arrSendedUsers.Contains(ds.Tables[0].Rows[j][0]) == false)//判断用户是否已收到消息，向未收到用户发送
                            {
                                arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                sUser = ds.Tables[0].Rows[j][0].ToString();
                                //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                //sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString().Remove(0,2), sMsg, sDateTime, sMsgType, sUrl, _Page);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else if (sUserList.Split('-')[0] == "U")
                    {
                        //arrSendedUsers.Add("@" + sUserList + "@");
                        sUser = sUserList.Remove(0, 2);
                        //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                        sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                    }
                }
                #endregion
                string sSendedUsers = string.Empty;
                for (int n = 0; n < arrSendedUsers.Count; n++)
                {
                    sSendedUsers = sSendedUsers + arrSendedUsers[n];
                }
                Log.WriteDebug(sSendedUsers);
                if (sReturn == "")
                {
                    return "会议通知发送完毕！";
                }
                else
                {
                    return sReturn;
                }
            }
            catch (Exception oExcept)
            {
                //ShowMessage.Alert(oExcept.Message, "1", "发送消息失败", _Page);
                Alert(oExcept.Message, _Page);
                Log.WriteError(oExcept.Message);
                return "SendMessage错误：" + oExcept.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sUserID">用户ID</param>
        /// <param name="sMsg">发送消息的具体内容</param>
        /// <param name="sDateTime">发送消息的时间</param>
        /// <param name="sMsgType">发送消息的类型</param>
        /// <param name="sUrl">消息提示超链接</param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static string SendMessage(DatabaseORC db, string sUserID, string sMsg, string sDateTime, string sMsgType, string sUrl, System.Web.UI.Page _Page)
        {
            try
            {
                string sMsgInfo = "&nbsp;&nbsp;&nbsp;&nbsp;<a class=''grid-pager'' href=''#'' onclick=''top.redirectUrl(\"" + sUrl + "\"); return false;''>" + sMsg + "</a>";
                //DatabaseORC db = new DatabaseORC();
                string sSql = "insert into messagesys(RECEIVEID,MESSAGE,COMETIME,MSGTYPE,MESSAGEINFO,SHOWFLG) values('" + sUserID + "','" + sMsg + "',to_date('" + sDateTime + "','yyyy-mm-dd hh24:mi:ss'),'" + sMsgType + "','" + sMsgInfo + "','0')";
                //WriteLog(sSql);//检查sql语句是否拼写正确
                db.ExecuteSql(sSql);
                //ShowMessage.Alert("发送消息成功", "1", "提示", _Page);
                //Tool.Alert("发送消息成功",_Page);
                return "";
            }
            catch (Exception oExcept)
            {
                //ShowMessage.Alert(oExcept.Message, "1", "发送消息失败", _Page);
                //Alert(oExcept.Message, _Page);
                Log.WriteError("===================================报错===================================\n" + oExcept.Message);
                return "发送失败：" + oExcept.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db">数据库对象</param>
        /// <param name="sUserID">用户ID</param>
        /// <param name="sDateTime">时间字符串，格式为'yyyy-mm-dd hh24:mi:ss'</param>
        /// <param name="sGUID">对应记录条目的GUID</param>
        /// <param name="sNoticeType">通知类型</param>
        /// <param name="_page">页面对象</param>
        /// <returns></returns>
        public static string SendMeetingNotice(DatabaseORC db, string sUserID, string sDateTime, string sGuid, string sNoticeType, System.Web.UI.Page _page)
        {
            try
            {
                string sSql = "insert into CM_LC_MEETINGNOTICE(RECEIVEID,COMETIME,GUID,NOTICETYPE) values('" + sUserID + "',to_date('" + sDateTime + "','yyyy-mm-dd hh24:mi:ss'),'" + sGuid + "','" + sNoticeType + "')";
                db.ExecuteSql(sSql);
                //WriteLog("SendMeetingNotice----" + sSql);
                return "发送通知成功!";
            }
            catch (Exception oExcept)
            {
                return "发送通知失败:" + oExcept.Message;
            }
        }

        /// <summary>
        /// 向用户发送文件，并发送系统消息提示
        /// </summary>
        /// <param name="sUserList"></param>
        /// <param name="sMsg"></param>
        /// <param name="sUrl"></param>
        /// <param name="sGuid"></param>
        /// <param name="sMsgType"></param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static string SendFiles(string sUserList, string sMsg, string sUrl, string sGuid, string sMsgType, System.Web.UI.Page _Page)
        {
            try
            {
                ArrayList arrSendedUsers = new ArrayList();//保存已经发送过的用户ID，避免重复发送
                ArrayList arrUserInGroup = new ArrayList();//保存根据机构代码查到的用户ID，分别发送
                string sSql = "select 用户ID from flow_user_role where 机构ID=(select 机构ID from flow_groups t where t.编码='";//根据机构查找用户代码的SQL
                DataSet ds = new DataSet();
                string sDateTime = DateTime.Now.ToString();
                DatabaseORC db = new DatabaseORC();
                string sReturn = string.Empty;
                string sUser = string.Empty;
                string sNoticeType = sMsgType;
                #region sUserList中包含有全局选项时
                if (sUserList.Contains("P-1,") || sUserList == "P-1")
                {
                    ds = db.GetDataSet("select * from flow_users t");
                    for (int m = 0; m < ds.Tables[0].Rows.Count; m++)//循环向用户发送消息
                    {
                        sUser = ds.Tables[0].Rows[m][0].ToString();
                        sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                    }
                }
                #endregion

                #region sUserList包含逗号时
                else if (sUserList.Contains(","))//有逗号
                {
                    //Tool.WriteLog("有逗号:"+sUserList);
                    string[] strUserList = sUserList.Split(',');
                    for (int i = 0; i < strUserList.Length; i++)
                    {
                        if (strUserList[i].Split('-')[0] == "P")//取得机构ID时
                        {
                            sUser = strUserList[i].Remove(0, 2);
                            ds = db.GetDataSet(sSql + sUser + "')");
                            //Tool.WriteLog(sSql+sUser+"')");
                            //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                            {
                                sUser = ds.Tables[0].Rows[j][0].ToString();
                                if (arrSendedUsers.Contains("@" + ds.Tables[0].Rows[j][0] + "@") == false)//判断用户是否已收到消息，向未收到用户发送
                                {
                                    arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                    //Tool.WriteLog("第"+(j+1)+"次发送"+sUser);
                                    //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                    sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                    //sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString().Remove(0,2), sMsg, sDateTime, sMsgType, sUrl, _Page);

                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else if (strUserList[i].Split('-')[0] == "U")
                        {
                            sUser = strUserList[i].Remove(0, 2);
                            if (arrSendedUsers.Contains("@" + sUser + "@") == false)
                            {
                                arrSendedUsers.Add("@" + sUser + "@");
                                //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                //sReturn = Tool.SendMessage(db, strUserList[i], sMsg, sDateTime, sMsgType, sUrl, _Page);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                #endregion

                #region sUserList中无逗号
                else
                {
                    if (sUserList.Split('-')[0] == "P")
                    {
                        sUser = sUserList.Remove(0, 2);
                        ds = db.GetDataSet(sSql + sUser + "')");
                        //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                        {
                            if (arrSendedUsers.Contains(ds.Tables[0].Rows[j][0]) == false)//判断用户是否已收到消息，向未收到用户发送
                            {
                                arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                sUser = ds.Tables[0].Rows[j][0].ToString();
                                //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                //sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString().Remove(0,2), sMsg, sDateTime, sMsgType, sUrl, _Page);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else if (sUserList.Split('-')[0] == "U")
                    {
                        //arrSendedUsers.Add("@" + sUserList + "@");
                        sUser = sUserList.Remove(0, 2);
                        //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                        sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                    }
                }
                #endregion
                string sSendedUsers = string.Empty;
                for (int n = 0; n < arrSendedUsers.Count; n++)
                {
                    sSendedUsers = sSendedUsers + arrSendedUsers[n];
                }
                Log.WriteDebug(sSendedUsers);
                if (sReturn == "")
                {
                    return "会议通知发送完毕！";
                }
                else
                {
                    return sReturn;
                }
            }
            catch (Exception oExcept)
            {
                //ShowMessage.Alert(oExcept.Message, "1", "发送消息失败", _Page);
                Alert(oExcept.Message, _Page);
                Log.WriteError("===================================报错===================================\n" + oExcept.Message);
                return "SendMessage错误：" + oExcept.Message;
            }
        }

        /// <summary>
        /// 向用户分发文件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sUserID"></param>
        /// <param name="sDateTime"></param>
        /// <param name="sGuid"></param>
        /// <param name="sSignType"></param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static string SendFiles(DatabaseORC db, string sUserID, string sDateTime, string sGuid, string sSignType, System.Web.UI.Page _Page)
        {
            try
            {
                string sSql = "insert into CM_LC_SIGN(RECEIVEID,COMETIME,GUID,SIGNTYPE) values('" + sUserID + "',to_date('" + sDateTime + "','yyyy-mm-dd hh24:mi:ss'),'" + sGuid + "','" + sSignType + "')";
                db.ExecuteSql(sSql);
                //WriteLog("SendMeetingNotice----" + sSql);
                return "文件发送成功!";
            }
            catch (Exception oExcept)
            {
                return "文件发送失败:" + oExcept.Message;
            }
        }

        /// <summary>
        /// 向用户分发信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sUserID"></param>
        /// <param name="sDateTime"></param>
        /// <param name="sGuid"></param>
        /// <param name="sSignType"></param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static string SendInfo(DatabaseORC db, string sUserID, string sDateTime, string sGuid, string sSignType, System.Web.UI.Page _Page)
        {
            try
            {
                string sSql = "insert into CM_LC_SIGN(RECEIVEID,COMETIME,GUID,SIGNTYPE) values('" + sUserID + "',to_date('" + sDateTime + "','yyyy-mm-dd hh24:mi:ss'),'" + sGuid + "','" + sSignType + "')";
                db.ExecuteSql(sSql);
                return "发送通知成功!";
            }
            catch (Exception oExcept)
            {
                Log.WriteError(oExcept.Message);
                return "发送通知失败:" + oExcept.Message;
            }
        }


        /// <summary>
        /// 向用户发送信息，并发送系统消息提示
        /// </summary>
        /// <param name="sUserList"></param>
        /// <param name="sMsg"></param>
        /// <param name="sUrl"></param>
        /// <param name="sGuid"></param>
        /// <param name="sMsgType"></param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static string SendInfo(string sUserList, string sMsg, string sUrl, string sGuid, string sMsgType, System.Web.UI.Page _Page)
        {
            try
            {
                ArrayList arrSendedUsers = new ArrayList();//保存已经发送过的用户ID，避免重复发送
                ArrayList arrUserInGroup = new ArrayList();//保存根据机构代码查到的用户ID，分别发送
                string sSql = "select 用户ID from flow_user_role where 机构ID=(select 机构ID from flow_groups t where t.编码='";//根据机构查找用户代码的SQL
                DataSet ds = new DataSet();
                string sDateTime = DateTime.Now.ToString();
                DatabaseORC db = new DatabaseORC();
                string sReturn = string.Empty;
                string sUser = string.Empty;
                string sNoticeType = sMsgType;
                #region sUserList中包含有全局选项时
                if (sUserList.Contains("P-1,") || sUserList == "P-1")
                {
                    ds = db.GetDataSet("select * from flow_users t");
                    for (int m = 0; m < ds.Tables[0].Rows.Count; m++)//循环向用户发送消息
                    {
                        sUser = ds.Tables[0].Rows[m][0].ToString();
                        sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                    }
                }
                #endregion

                #region sUserList包含逗号时
                else if (sUserList.Contains(","))//有逗号
                {
                    //Tool.WriteLog("有逗号:"+sUserList);
                    string[] strUserList = sUserList.Split(',');
                    for (int i = 0; i < strUserList.Length; i++)
                    {
                        if (strUserList[i].Split('-')[0] == "P")//取得机构ID时
                        {
                            sUser = strUserList[i].Remove(0, 2);
                            ds = db.GetDataSet(sSql + sUser + "')");
                            //Tool.WriteLog(sSql+sUser+"')");
                            //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                            {
                                sUser = ds.Tables[0].Rows[j][0].ToString();
                                if (arrSendedUsers.Contains("@" + ds.Tables[0].Rows[j][0] + "@") == false)//判断用户是否已收到消息，向未收到用户发送
                                {
                                    arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                    //Tool.WriteLog("第"+(j+1)+"次发送"+sUser);
                                    //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                    sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                    //sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString().Remove(0,2), sMsg, sDateTime, sMsgType, sUrl, _Page);

                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        else if (strUserList[i].Split('-')[0] == "U")
                        {
                            sUser = strUserList[i].Remove(0, 2);
                            if (arrSendedUsers.Contains("@" + sUser + "@") == false)
                            {
                                arrSendedUsers.Add("@" + sUser + "@");
                                //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                //sReturn = Tool.SendMessage(db, strUserList[i], sMsg, sDateTime, sMsgType, sUrl, _Page);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                #endregion

                #region sUserList中无逗号
                else
                {
                    if (sUserList.Split('-')[0] == "P")
                    {
                        sUser = sUserList.Remove(0, 2);
                        ds = db.GetDataSet(sSql + sUser + "')");
                        //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                        {
                            if (arrSendedUsers.Contains(ds.Tables[0].Rows[j][0]) == false)//判断用户是否已收到消息，向未收到用户发送
                            {
                                arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                sUser = ds.Tables[0].Rows[j][0].ToString();
                                //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                //sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString().Remove(0,2), sMsg, sDateTime, sMsgType, sUrl, _Page);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else if (sUserList.Split('-')[0] == "U")
                    {
                        //arrSendedUsers.Add("@" + sUserList + "@");
                        sUser = sUserList.Remove(0, 2);
                        //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                        sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                    }
                }
                #endregion

                #region Debug
                //string sSendedUsers = string.Empty;
                //for (int n = 0; n < arrSendedUsers.Count; n++)
                //{
                //    sSendedUsers = sSendedUsers + arrSendedUsers[n];
                //}
                //Tool.WriteLog(sSendedUsers);
                #endregion
                if (sReturn == "")
                {
                    return "会议通知发送完毕！";
                }
                else
                {
                    return sReturn;
                }
            }
            catch (Exception oExcept)
            {
                //ShowMessage.Alert(oExcept.Message, "1", "发送消息失败", _Page);
                Alert(oExcept.Message, _Page);
                Log.WriteError("===================================报错===================================\n" + oExcept.Message);
                return "SendMessage错误：" + oExcept.Message;
            }
        }

        /// <summary>
        /// 向用户发送信息，并发送系统消息提示
        /// </summary>
        /// <param name="sUserList"></param>
        /// <param name="sMsg"></param>
        /// <param name="_Page"></param>
        /// <returns></returns>
        public static ArrayList GetUserList(string sUserList, System.Web.UI.Page _Page)
        {
            try
            {
                ArrayList arrSendedUsers = new ArrayList();//保存已经发送过的用户ID，避免重复发送
                ArrayList arrUserInGroup = new ArrayList();//保存根据机构代码查到的用户ID，分别发送
                string sSql = "select 用户ID from flow_user_role where 机构ID=(select 机构ID from flow_groups t where t.编码='";//根据机构查找用户代码的SQL
                DataSet ds = new DataSet();
                string sDateTime = DateTime.Now.ToString();
                DatabaseORC db = new DatabaseORC();
                string sReturn = string.Empty;
                string sUser = string.Empty;
                #region sUserList中包含有全局选项时
                if (sUserList.Contains("P-1,") || sUserList == "P-1")
                {
                    ds = db.GetDataSet("select * from flow_users t");
                    for (int m = 0; m < ds.Tables[0].Rows.Count; m++)//循环向用户发送消息
                    {
                        sUser = ds.Tables[0].Rows[m][0].ToString();
                        //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                    }
                }
                #endregion

                #region sUserList包含逗号时
                else if (sUserList.Contains(","))//有逗号
                {
                    //Tool.WriteLog("有逗号:"+sUserList);
                    string[] strUserList = sUserList.Split(',');
                    for (int i = 0; i < strUserList.Length; i++)
                    {
                        #region 机构
                        if (strUserList[i].Split('-')[0] == "P")//取得机构ID时
                        {
                            sUser = strUserList[i].Remove(0, 2);
                            ds = db.GetDataSet(sSql + sUser + "')");
                            //Tool.WriteLog(sSql+sUser+"')");
                            //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                            {
                                sUser = ds.Tables[0].Rows[j][0].ToString();
                                if (arrSendedUsers.Contains(sUser) == false)//判断用户是否已收到消息，向未收到用户发送
                                {
                                    arrSendedUsers.Add(sUser);
                                    //Tool.WriteLog("第"+(j+1)+"次发送"+sUser);
                                    //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                    ////sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                    //sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString().Remove(0,2), sMsg, sDateTime, sMsgType, sUrl, _Page);

                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                        #endregion

                        #region 人员
                        else if (strUserList[i].Split('-')[0] == "U")
                        {
                            sUser = strUserList[i].Remove(0, 2);
                            if (arrSendedUsers.Contains(sUser) == false)
                            {
                                Tool.Alert(sUser,_Page);
                                arrSendedUsers.Add(sUser);
                                //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                ////sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                //sReturn = Tool.SendMessage(db, strUserList[i], sMsg, sDateTime, sMsgType, sUrl, _Page);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region sUserList中无逗号
                else
                {
                    if (sUserList.Split('-')[0] == "P")
                    {
                        sUser = sUserList.Remove(0, 2);
                        ds = db.GetDataSet(sSql + sUser + "')");
                        //arrUserInGroup = db.GetDataSet(sSql+strUserList[i]+"'").Tables[0].Columns[0];
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)//循环向用户发送消息，并用户ID存入arrUserInGroup
                        {
                            sUser = ds.Tables[0].Rows[j][0].ToString();
                            if (arrSendedUsers.Contains(sUser) == false)//判断用户是否已收到消息，向未收到用户发送
                            {
                                arrSendedUsers.Add(sUser);
                                //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                ////sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                                //sReturn = Tool.SendMessage(db, ds.Tables[0].Rows[j][0].ToString().Remove(0,2), sMsg, sDateTime, sMsgType, sUrl, _Page);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    else if (sUserList.Split('-')[0] == "U")
                    {
                        sUser = sUserList.Remove(0, 2);
                        if (arrSendedUsers.Contains(sUser))
                        {
                            arrSendedUsers.Add(sUser);
                        }
                        //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                        ////sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl + sGuid, _Page) + Tool.SendFiles(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                    }
                }
                #endregion

                #region Debug
                //string sSendedUsers = string.Empty;
                //for (int n = 0; n < arrSendedUsers.Count; n++)
                //{
                //    sSendedUsers = sSendedUsers + arrSendedUsers[n];
                //}
                //Tool.Alert(sSendedUsers,_Page);
                #endregion
                return arrSendedUsers;
            }
            catch (Exception oExcept)
            {
                //ShowMessage.Alert(oExcept.Message, "1", "发送消息失败", _Page);
                Alert(oExcept.Message, _Page);
                Log.WriteError(oExcept.Message);
                throw oExcept;
            }
        }



    }

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
            string debug=string.Empty;
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

    /// <summary>
    /// 数据库通用操作类
    /// </summary>
    public class DatabaseORC
    {
        public new OracleConnection Conn = new OracleConnection();

        public DatabaseORC()
        {
            //测试方法
            //string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString();
            //中地使用方法
            string connStr = System.Configuration.ConfigurationManager.AppSettings["connectionString"].ToString();
            Conn = new OracleConnection(connStr);
        }

        public DatabaseORC(string constr)
        {
            Conn = new OracleConnection(constr);
        }

        #region 打开数据库连接
        /// <summary>
        /// 打开数据库连接
        /// </summary>
        private void Open()
        {
            //打开数据库连接
            if (Conn.State == ConnectionState.Closed)
            {
                try
                {
                    //打开数据库连接
                    Conn.Open();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion

        #region 关闭数据库连接
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        private void Close()
        {
            //判断连接的状态是否已经打开
            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();
            }
        }
        #endregion

        #region 执行带参数的SQL语句
        /// <summary>
        /// 执行不带参数的SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>     
        public void ExecuteSql(string sql)
        {
            OracleCommand cmd = new OracleCommand(sql, Conn);
            try
            {
                Open();
                cmd.ExecuteNonQuery();
                Close();
            }
            catch (System.Data.OracleClient.OracleException e)
            {
                Close();
                throw e;
            }
        }
        #endregion

        #region 执行带参数的SQL语句
        /// <summary>
        /// 执行不带参数的SQL语句
        /// </summary>
        /// <param name="sql">SQL语句</param>     
        public object ExecuteScalarSql(string sql)
        {
            object o = null;
            OracleCommand cmd = new OracleCommand(sql, Conn);
            try
            {
                Open();
                o = cmd.ExecuteScalar();
                Close();
            }
            catch (System.Data.OracleClient.OracleException e)
            {
                Close();
                throw e;
            }
            return o;
        }
        #endregion

        #region 执行SQL语句，返回数据到DataSet中
        /// <summary>
        /// 执行SQL语句，返回数据到DataSet中
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回DataSet</returns>
        public DataSet GetDataSet(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                Open();//打开数据连接
                OracleDataAdapter adapter = new OracleDataAdapter(sql, Conn);
                adapter.Fill(ds);
            }
            catch//(Exception ex)
            {
            }
            finally
            {
                Close();//关闭数据库连接
            }
            return ds;
        }
        #endregion

        #region 执行SQL语句，返回数据到自定义DataSet中
        /// <summary>
        /// 执行SQL语句，返回数据到DataSet中
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="DataSetName">自定义返回的DataSet表名</param>
        /// <returns>返回DataSet</returns>
        public DataSet GetDataSet(string sql, string DataSetName)
        {
            DataSet ds = new DataSet();
            Open();//打开数据连接
            OracleDataAdapter adapter = new OracleDataAdapter(sql, Conn);
            adapter.Fill(ds, DataSetName);
            Close();//关闭数据库连接
            return ds;
        }
        #endregion


    }


    public class SMS
    {
        /// <summary>
        /// 发送对象列表
        /// </summary>
        public static ArrayList toUserList { get; set; }

        /// <summary>
        /// 发送短信内容
        /// </summary>
        public static string msg { get; set; }

        /// <summary>
        /// 发送人
        /// </summary>
        public static string fromUser { get; set; }

        public SMS(string ToDepartList,string Msg,string FromUser)
        {
            toUserList = GetToUserList(ToDepartList);
            msg = Msg;
            fromUser = FromUser;
        }

        /// <summary>
        /// 数据库实例
        /// </summary>
        private static DatabaseORC db = new DatabaseORC();

        /// <summary>
        /// 获取机构所有下属子机构(包括本级)
        /// </summary>
        /// <param name="parentDepartCode">机构编码</param>
        /// <returns>包含本级的下属机构列表</returns>
        public static ArrayList GetChildDepartList(string parentDepartCode)
        {
            try
            {
                ArrayList childDepartList = new ArrayList();
                string Sql = "select t.机构ID from flow_groups t where t.编码 like '" + parentDepartCode + "%'";
                DataSet ds=db.GetDataSet(Sql);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];//获取所有的相关机构ID数据
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            childDepartList.Add(dt.Rows[i][0].ToString());
                        }
                    }
                    else
                    {
                        childDepartList.Add(parentDepartCode);
                    }
                }
                Log.WriteDebug("GetChildDepartList>>>>>" + childDepartList.Count);
                return childDepartList;
            }
            catch (Exception ex)
            {
                Log.WriteError("GetChildDepart失败>>>>>");
                throw ex;
            }
        }

        /// <summary>
        /// 获取本级机构下直属的用户ID
        /// </summary>
        /// <param name="parentDepartID">所属机构的ID</param>
        /// <returns>本级机构下直属的用户ID列表</returns>
        public static ArrayList GetChildUserList(string parentDepartID)
        {
            try
            {
                ArrayList childUserList = new ArrayList();
                string Sql = "select t.用户ID from flow_user_role t where t.机构ID='" + parentDepartID + "'";//取得本级机构下直属的用户ID列表
                DataTable dt = db.GetDataSet(Sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++) 
                    {
                        if (!childUserList.Contains(dt.Rows[i][0].ToString()))//判断用户在列表中是否已存在
                        {
                            childUserList.Add(dt.Rows[i][0].ToString());
                        }
                    }
                }

                return childUserList;
            }
            catch (Exception ex)
            {
                Log.WriteError("GetChildUserList错误>>>>>" + ex.Message);
                throw ex;
            }
        }

        public static void AddUser(ref ArrayList userIDList,string userID)
        {
            try
            {
                if (!userIDList.Contains(userID))
                {
                    userIDList.Add(userID);
                }
                //return userIDList;
            }
            catch (Exception ex)
            {
                Log.WriteError("AddUser错误>>>>>" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// 根据页面中获取到的用户/机构编码字符串查找所有的下属用户ID
        /// </summary>
        /// <param name="departIDList">用户/机构编码字符串</param>
        /// <returns>用户ID列表</returns>
        public static ArrayList GetToUserList(string strDepartIDList)
        {
            try
            {
                ArrayList toUserList = new ArrayList();
                ArrayList tempDepartList = new ArrayList();
                ArrayList tempUserList=new ArrayList();
                if (strDepartIDList.Contains(","))
                {
                    string[] depart = strDepartIDList.Split(',');//获取由机构ID构成的数组
                    for (int i = 0; i < depart.Length; i++)
                    {
                        #region 用户
                        if (depart[i].ToUpper().StartsWith("U"))
                        {
                            AddUser(ref toUserList, depart[i].Remove(0, 2));
                        }
                        #endregion

                        #region 机构
                        else if (depart[i].ToUpper().StartsWith("P"))
                        {
                            tempDepartList = GetChildDepartList(depart[i].Remove(0, 2));//获取下属机构ID列表
                            for (int j = 0; j < tempDepartList.Count; j++)
                            {
                                tempUserList = GetChildUserList(tempDepartList[j].ToString());//获取所有相关机构直属用户ID列表
                                if (tempUserList.Count > 0)
                                {
                                    for (int n = 0; n < tempUserList.Count; n++)
                                    {
                                        AddUser(ref toUserList, tempUserList[n].ToString());
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    #region 用户
                    if (strDepartIDList.ToUpper().StartsWith("U"))
                    {
                        AddUser(ref toUserList, strDepartIDList.Remove(0, 2));
                    }
                    #endregion

                    #region 机构
                    else if (strDepartIDList.ToUpper().StartsWith("P"))
                    {
                        Log.WriteDebug("GetToUserList>>>>>" + strDepartIDList.ToUpper());
                        Log.WriteDebug("GetToUserList>strDepartIDList>>>>>" + strDepartIDList.ToUpper());
                        tempDepartList = GetChildDepartList(strDepartIDList.Remove(0, 2));//获取下属机构ID列表
                        Log.WriteDebug("tempDepartList>>>>>" + tempDepartList.Count);
                        for (int j = 0; j < tempDepartList.Count; j++)
                        {
                            tempUserList = GetChildUserList(tempDepartList[j].ToString());//获取所有相关机构直属用户ID列表
                            if (tempUserList.Count > 0)
                            {
                                for (int n = 0; n < tempUserList.Count; n++)
                                {
                                    AddUser(ref toUserList, tempUserList[n].ToString());
                                }
                            }
                        }
                    }
                    #endregion
                }

                return toUserList;
            }
            catch (Exception ex)
            {
                Log.WriteError("GetToUserList错误>>>>>" + ex.Message);
                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserList"></param>
        /// <param name="Msg"></param>
        /// <param name="FromUser"></param>
        /// <returns></returns>
        public static bool SnedSMS(ArrayList UserList, string Msg, string FromUser, System.Web.UI.Page _Page)
        {
            try
            {
                DataTable dt = new DataTable();
                string Sql = "select t1.移动电话 from flow_user_address t1 join  flow_users t2 on t1.用户id=t2.用户id where t1.用户ID=";
                string sendSql = string.Empty;
                for (int i = 0; i < UserList.Count; i++)
                {
                    //Log.WriteDebug("select t1.移动电话 from flow_user_address t1 join  flow_users t2 on t1.用户id=t2.用户id where t1.用户ID="+ UserList[i].ToString());
                    dt = db.GetDataSet(Sql + UserList[i].ToString()).Tables[0];//获取用户电话
                    if (dt.Rows.Count > 0&&dt.Rows[0][0].ToString().Length>0)
                    {
                        sendSql = string.Format("insert into sms_waitsend (mobileno,fromuser,smstxt) values ({0},'{1}','{2}')", dt.Rows[0][0].ToString(), FromUser, Msg);
                        db.ExecuteSql(sendSql);
                        Log.WriteDebug(sendSql);
                    }
                    else
                    {
                        Tool.Alert(string.Format("未向用户{0}发送短信，请录入电话信息!", GetUserName(UserList[i].ToString())),_Page);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError("SnedSMS错误>>>>>" + ex.Message);
                throw ex;
            }
        }


        public static string GetUserName(string userID)
        {
            string Sql = string.Format("select t.名称 from flow_users t where t.用户ID={0}", userID);
            try
            {
                Log.WriteDebug("GetUserName" + Sql);
                return db.GetDataSet(Sql).Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                Log.WriteError("GetUserName错误>>>>>" + ex.Message);
                throw ex;
            }
        }

        public static bool SendSMS(System.Web.UI.Page _Page)
        {
            try
            {
                DataTable dt = new DataTable();
                string Sql = "select t1.移动电话 from flow_user_address t1 join  flow_users t2 on t1.用户id=t2.用户id where t1.用户ID=";
                string sendSql = string.Empty;
                for (int i = 0; i < toUserList.Count; i++)
                {
                    //Log.WriteDebug("select t1.移动电话 from flow_user_address t1 join  flow_users t2 on t1.用户id=t2.用户id where t1.用户ID="+ UserList[i].ToString());
                    dt = db.GetDataSet(Sql + toUserList[i].ToString()).Tables[0];//获取用户电话
                    if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString().Length > 0)
                    {
                        sendSql = string.Format("insert into sms_waitsend (mobileno,fromuser,smstxt) values ({0},'{1}','{2}')", dt.Rows[0][0].ToString(), fromUser, msg);
                        db.ExecuteSql(sendSql);
                        Log.WriteDebug(sendSql);
                    }
                    else
                    {
                        Tool.Alert(string.Format("未向用户{0}发送短信，请录入电话信息!", GetUserName(toUserList[i].ToString())), _Page);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError("SnedSMS错误>>>>>" + ex.Message);
                throw ex;
            }
        }
    }
}
