using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Collections;
using System.Data;

namespace Function
{
    /// <summary>
    /// Function 的摘要说明。
    /// </summary>
    public class Function : Visual_Form_Designer.Class.IFunction
    {
        private string Rev = "";
        private string ErrorMsg = "";
        public Function() { }

        #region IFunction 成员

        public object ReturnValue
        {
            get
            {
                return Rev;
            }
        }
        
        public bool Exec(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                Hashtable arr = _CustomObject as Hashtable;
                WebControl Sender;         //触发此调用的页面控件，如果是页面调用的话Sender为空；
                StateBag ViewState;        //本页面的ViewState;
                if (arr != null)
                {
                    Sender = arr["Sender"] as WebControl;
                    ViewState = arr["ViewState"] as StateBag;
                }

                return true;
            }
            catch(Exception oExcept)
            {
                return false;
            }
        }

        public string GetUserID(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                /*
                Hashtable arr = _CustomObject as Hashtable;
                WebControl Sender;         //触发此调用的页面控件，如果是页面调用的话Sender为空；
                StateBag ViewState;        //本页面的ViewState;
                if (arr != null)
                {
                    Sender = arr["Sender"] as WebControl;
                    ViewState = arr["ViewState"] as StateBag;
                }
                 
                //string sUserID = _Context.User.Identity.Name;//取用户ID
                */
                //_Page.Request.QueryString["USERID"] = _Context.User.Identity.Name;
                TextBox tbUserID = (TextBox)_Page.FindControl("txt1");
                Rev = _Context.User.Identity.Name;
                tbUserID.Text = Rev;
                
                return Rev;
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog(oExcept.Message);
                ErrorMsg = oExcept.Message;
                return ErrorMsg;
            }
        }

        public bool GetUserSignFlag(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                //TextBox tbCaseNo = (TextBox)_Page.FindControl("txtCaseNo");
                Button bMeetingSign=(Button)_Page.FindControl("btnMeetingSign");
                string sUserID = _Context.User.Identity.Name;
                string sCaseNo = _Page.Request.QueryString["CASENO"];
                DatabaseORC db = new DatabaseORC();
                string sUserSignFlag = "1";
                string sSql = "select t.signflag from cm_lc_meetingnotice t where t.receiveid='" + sUserID + "' and t.guid='" + sCaseNo + "'";
                //Tool.WriteLog(sSql);
                try
                {
                    sUserSignFlag = db.GetDataSet(sSql).Tables[0].Rows[0][0].ToString();
                    if (sUserSignFlag == "1")
                    {
                        bMeetingSign.Visible = false;
                    }
                    else if(sUserSignFlag=="0")
                    {
                        bMeetingSign.Visible = true;
                    }
                }
                catch
                {
                    bMeetingSign.Visible = false;
                }
                return true;
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog(oExcept.Message);
                return false;
            }
        }

        public bool 测试(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                Hashtable arr = _CustomObject as Hashtable;
                WebControl Sender;         //触发此调用的页面控件，如果是页面调用的话Sender为空；
                StateBag ViewState;        //本页面的ViewState;
                if (arr != null)
                {
                    Sender = arr["Sender"] as WebControl;
                    ViewState = arr["ViewState"] as StateBag;
                }
                string sUserID = _Context.User.Identity.Name;//取用户ID
                string sUserName = "";//初始化用户名
                string strSql = string.Format("select 名称 from FLOW_USERS where 用户ID='{0}'", sUserID);//生成SQL
                DatabaseORC db = new DatabaseORC();//初始化数据连接
                DataSet ds = db.GetDataSet(strSql);//执行SQL获取查询结果
                if (ds.Tables[0].Rows.Count > 0)//若数据不为空，读取用户名
                {
                    sUserName = ds.Tables[0].Rows[0][0].ToString();
                }
                else
                {
                    sUserName = "未取到用户名!";//若查询结果为空，则提示错误
                }
                ShowMessage s = new ShowMessage();//初始化Alert对象
                TextBox tbuser = (TextBox)_Page.FindControl("txtUser");//初始化页面中ID为"txtUser"的TextBox控件
                tbuser.Text = sUserName;//将用户名字段赋予页面中ID为"txtUser"的控件
                TextBox tbAlert = (TextBox)_Page.FindControl("txtAlert");//初始化页面中ID为"txtAlert"的TextBox控件
                if (tbAlert.Text != "")//当"txtAlert"控件中无任何内容时，不弹出提示，若有内容，则提示内容
                {
                    //string sUrl = System.Web.HttpContext.Current.Server.MapPath("\\XinXiFaBu").ToString();
                    string sUrl = System.Web.HttpContext.Current.Server.MapPath("~/");
                    string x = System.Configuration.ConfigurationManager.AppSettings["IGSLandService"];
                    Tool.ShowModalDialog(sUrl + "\\XXFB.VFD", _Page);
                    //Tool.ShowModalDialog(sUrl + "..\\..\\SubSysFlowFile\\XinXiFaBu\\XXFB_YGD.VFD", _Page);
                    Tool.WriteLog(sUrl + "SubSysFlowFile\\XinXiFaBu\\XXFB_YGD.VFD");
                    try
                    {
                        if (System.Int32.Parse(tbAlert.Text.ToString()) > 0)
                        {
                            Tool.WriteLog(System.Int32.Parse(tbAlert.Text).ToString());
                            Tool.Recurrence(System.Int32.Parse(tbAlert.Text));
                            Tool.Alert(tbAlert.Text,_Page);
                        }
                        else return false;
                    }
                    catch(Exception oExcept)
                    {
                        Tool.WriteLog(oExcept.Message);
                    }
                }
                else
                {
                    //Tool.ShowModalDialog("开发科.VFD", _Page);
                    //Tool.ShowModalDialog("..\\..\\SubSysFlowFile\\开发科.VFD", _Page);
                    Tool.Alert("未传递数字",_Page);
                }
                Tool.WriteLog(_Page.ToString() + "\n" + _Context.ToString() + "\n" + _Service.ToString() + "\n" + //换行
                    _WebPageConfig.ToString() + "\n" + ParamaterList.ToString() + "\n" + _CustomObject.ToString());
                return true;
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog(oExcept.Message);
                return false;
            }

        }


        public bool MeetingSign(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                //Tool.WriteLog("开始签收");
                string sUserID=_Context.User.Identity.Name;
                string sCaseNo = ((TextBox)_Page.FindControl("txtCaseNo")).Text;
                string sSql = "update cm_lc_meetingnotice t set t.signflag = '1' , t.signtime=(to_date('"+DateTime.Now+"','yyyy-mm-dd hh24:mi:ss')) where t.receiveid = '" + sUserID + "' and t.guid='" + sCaseNo + "'";
                Tool.WriteLog(sSql);
                DatabaseORC db = new DatabaseORC();
                try
                {
                    db.ExecuteSql(sSql);
                    ((Button)_Page.FindControl("btnMeetingSign")).Visible = false;
                    Tool.Alert("签收成功！",_Page);
                    return true;
                }
                catch
                {
                    Tool.Alert("签收失败，请联系管理员！",_Page);
                    return false;
                }
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog("签收失败"+oExcept.Message);
                Tool.Alert("签收失败，请联系管理员！", _Page);
                return false;
            }
        }

        /// <summary>
        /// 签收通用方法
        /// </summary>
        /// <param name="_Page"></param>
        /// <param name="_Context"></param>
        /// <param name="_Service"></param>
        /// <param name="_WebPageConfig"></param>
        /// <param name="ParamaterList"></param>
        /// <param name="_CustomObject"></param>
        /// <returns></returns>
        public bool Sign(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                //Tool.WriteLog("开始签收");
                string sUserID = _Context.User.Identity.Name;
                string sCaseNo = _Page.Request.QueryString["CASENO"];
                string sSignType = ((TextBox)_Page.FindControl("txtSignType")).Text;
                string sSql = "update cm_lc_sign t set t.signflag = '1' , t.signtime=(to_date('" + DateTime.Now + "','yyyy-mm-dd hh24:mi:ss')) where t.receiveid = '" + sUserID + "' and t.guid='" + sCaseNo + "' and t.signtype='"+sSignType+"'";
                Tool.WriteLog(sSql);
                DatabaseORC db = new DatabaseORC();
                try
                {
                    db.ExecuteSql(sSql);
                    ((Button)_Page.FindControl("btnSign")).Visible = false;
                    Tool.Alert("签收成功！", _Page);
                    return true;
                }
                catch
                {
                    Tool.Alert("签收失败，请联系管理员！", _Page);
                    return false;
                }
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog("签收失败" + oExcept.Message);
                Tool.Alert("签收失败，请联系管理员！", _Page);
                return false;
            }
        }

        /// <summary>
        /// 根据CaseNo,SignFlag以及用户ID判断当前用户是否显示签收按钮
        /// </summary>
        /// <param name="_Page"></param>
        /// <param name="_Context"></param>
        /// <param name="_Service"></param>
        /// <param name="_WebPageConfig"></param>
        /// <param name="ParamaterList"></param>
        /// <param name="_CustomObject"></param>
        /// <returns></returns>
        public bool GetSignFlag(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                //TextBox tbCaseNo = (TextBox)_Page.FindControl("txtCaseNo");
                Button bSign = (Button)_Page.FindControl("btnSign");
                string sUserID = _Context.User.Identity.Name;
                string sCaseNo = _Page.Request.QueryString["CASENO"];
                string sSignType = ((TextBox)_Page.FindControl("txtSignType")).Text;
                DatabaseORC db = new DatabaseORC();
                string sUserSignFlag = "1";
                string sSql = "select t.signflag from cm_lc_sign t where t.receiveid='" + sUserID + "' and t.guid='" + sCaseNo + "' and t.signtype='" + sSignType + "'";
                try
                {
                    sUserSignFlag = db.GetDataSet(sSql).Tables[0].Rows[0][0].ToString();
                    if (sUserSignFlag == "1")
                    {
                        bSign.Visible = false;
                    }
                    else if (sUserSignFlag == "0")
                    {
                        bSign.Visible = true;
                    }
                }
                catch
                {
                    bSign.Visible = false;
                }
                return true;
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog("SignFlag错误：" + oExcept.Message);
                return false;
            }
        }


        public bool SendFiles(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                string sCurrentUserName = ((TextBox)_Page.FindControl("txtCurrentUserName")).Text;

                string sCaseNo = ((TextBox)_Page.FindControl("txtCaseNo")).Text;

                string sUserList = ((TextBox)_Page.FindControl("txtUserID")).Text;

                string sMsg = "收到文件：《"+((TextBox)_Page.FindControl("txtFileName")).Text+"》，请及时签收！";

                string sMsgType = ((TextBox)_Page.FindControl("txtSignType")).Text;

                string sUrl = "SubSysFlowFile/BanGongShi/SendFiles/FilesInfo.vfd?CASENO=";
                Tool.WriteLog("username="+sCurrentUserName+",caseno="+sCaseNo+",userlist"+sUserList+",sMsg"+sMsg+",MsgType"+sMsgType);
                DatabaseORC db = new DatabaseORC();
                if (sCurrentUserName == string.Empty || sCaseNo == string.Empty || sUserList == string.Empty || sMsg == string.Empty)
                {
                    Tool.Alert("请完整填写信息", _Page);
                }
                else
                {
                    Tool.WriteLog("开始发送文件");
                    Tool.Alert(Tool.SendInfo(sUserList, sMsg, sUrl, sCaseNo, sMsgType, _Page), _Page);
                }
                return true;
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog("+++++++++报错+++++++++\n" + oExcept.Message);
                ErrorMsg = oExcept.Message;
                return false;
            }
        }


        public bool SendInfo(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                //发送用户名
                string sCurrentUserName = ((TextBox)_Page.FindControl("txtSendUsers")).Text;
                //GUID标记
                string sCaseNo = ((TextBox)_Page.FindControl("txtCaseNo")).Text;
                //发送消息对象列表
                string sUserList = ((TextBox)_Page.FindControl("txtUserID")).Text;
                //发送消息内容
                string sMsg = "收到信息：《" + ((TextBox)_Page.FindControl("txtInfoTitle")).Text + "》，请及时签收！";
                //消息类型
                string sMsgType = ((TextBox)_Page.FindControl("txtSignType")).Text;
                //系统消息中超链接页面地址
                string sUrl = "SubSysFlowFile/BanGongShi/SendInfo/ViewInfo.vfd?CASENO=";
                DatabaseORC db = new DatabaseORC();
                /*
                if (sCurrentUserName == string.Empty || sCaseNo == string.Empty || sUserList == string.Empty || sMsg == string.Empty)
                {
                    Tool.Alert("请完整填写信息", _Page);
                }
                 * */
                    Tool.Alert(Tool.SendInfo(sUserList, sMsg, sUrl, sCaseNo, sMsgType, _Page), _Page);
                return true;
            }
            catch (Exception oExcept)
            {
                Tool.WriteLog("+++++++++报错+++++++++\n" + oExcept.Message);
                ErrorMsg = oExcept.Message;
                return false;
            }
        }
		


        public bool SendMeeting(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                TextBox tbCurrentUserName = (TextBox)_Page.FindControl("txtCurrentUserName");
                string sCurrentUserName = tbCurrentUserName.Text;
                TextBox tbCaseNo = (TextBox)_Page.FindControl("txtCaseNo");
                string sCaseNo = tbCaseNo.Text;
                TextBox tbUserList = (TextBox)_Page.FindControl("txtUserID");
                string sUserList = tbUserList.Text;
                TextBox tbMsg = (TextBox)_Page.FindControl("txtContent");
                string sMsg = "收到会议通知：《"+tbMsg.Text+"》，请及时查收！";
                string sMsgType = "会议通知";
                string sUrl="SubSysFlowFile/BanGongShi/MeetingManagement/MeetingInfo.vfd?CASENO=";
                DatabaseORC db = new DatabaseORC();
                //string sSignedUser = GetSignedUser(sCaseNo);
                //string sUnsignedUser = GetUnsignedUser(sCaseNo);
                //bool bSigned = (sSignedUser.StartsWith(sCurrentUserName)) || (sSignedUser.Contains(sCurrentUserName) && sSignedUser.Contains("," + sCurrentUserName));//判断用户是否存在数据库中
                if (sCurrentUserName == string.Empty || sCaseNo == string.Empty || sUserList == string.Empty || sMsg == string.Empty)
                {
                    Tool.Alert("请完整填写信息",_Page);
                }
                else
                {
                    Tool.WriteLog("开始发送消息");
                    Tool.Alert(Tool.SendMessage(db,sUserList, sMsg, sUrl, sCaseNo, sMsgType, _Page), _Page);
                }
                return true;
            }
            catch (Exception oExcept)
            {
                ShowMessage s = new ShowMessage();
                //Tool.Alert(oExcept.Message,_Page);
                Tool.WriteLog("+++++++++报错+++++++++\n" + oExcept.Message);
                ErrorMsg = oExcept.Message;
                return false;
            }
        }



        public bool DeleteFiles(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                TextBox tbCaseNo = (TextBox)_Page.FindControl("txtCaseNo");
                string sCaseNo = tbCaseNo.Text;
                DatabaseORC db = new DatabaseORC();
                //删除签收信息SQL
                string deleteSignSQL = "Delete cm_lc_sign where GUID='" + sCaseNo + "'";
                //删除发文信息SQL
                string deleteFilesSQL="Delete cm_lc_sendfiles where GUID='" + sCaseNo + "'";
                //删除系统消息SQL
                string deleteMsgSQL = "Delete messagesys where MESSAGEINFO like '%"+sCaseNo+"%'";
                try
                {
                    //删除签收信息
                    db.ExecuteSql(deleteSignSQL);
                    //删除发文信息
                    db.ExecuteSql(deleteFilesSQL);
                    //删除系统消息
                    db.ExecuteSql(deleteMsgSQL);
                    Tool.Alert("删除成功！", _Page);
                }
                catch(Exception ex)
                {
                    Tool.Alert("删除失败，请联系系统管理员！",_Page);
                    throw ex;
                }
                return true;
            }
            catch (Exception oExcept)
            {
                ShowMessage s = new ShowMessage();
                //Tool.Alert(oExcept.Message,_Page);
                Tool.WriteLog("+++++++++报错+++++++++\n" + oExcept.Message);
                ErrorMsg = oExcept.Message;
                return false;
            }
        }
		


        public string LastError
        {
            get
            {
                return ErrorMsg;
            }
        }
        #endregion
        public static string GetSignedUser(string CaseNo)
        {
            try
            {
                string sUser = "";
                string sSql = "select SIGNEDUSER from CM_LC_MEETINGMANAGEMENT where GUID='" + CaseNo + "'";
                DatabaseORC db = new DatabaseORC();
                sUser = db.GetDataSet(sSql).Tables[0].Rows[0][0].ToString();
                return sUser;
            }
            catch (Exception oExcept)
            {
                return oExcept.Message;
            }
        }

        public static string GetUnsignedUser(string CaseNo)
        {
            try
            {
                string sUser = "";
                string sSql = "select UNSIGNEDUSER from CM_LC_MEETINGMANAGEMENT where GUID='" + CaseNo + "'";
                DatabaseORC db = new DatabaseORC();
                sUser = db.GetDataSet(sSql).Tables[0].Rows[0][0].ToString();
                return sUser;
            }
            catch (Exception oExcept)
            {
                return oExcept.Message;
            }
        }
    }
           
    public class ShowMessage
    {
        public void Alert(string Message, System.Web.UI.Page _Page)
        {
            string strScript = "<script language='javascript'>alert('" + Message + "')</script>";
            _Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), strScript);
        }

         public static void Alert(string Message,string id,string title, System.Web.UI.Page _Page)
        {
            string strScript = "<script language='javascript'>$(function(){var obj = { id: \"" + id + "\", title: \"" + title + "\", text: \"" + Message + "\"};showMessage(obj);})</script>";
            _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            Tool.WriteLog(strScript);
            //var obj = { id: "message", title: "提示", text: "请选择接收人！"};
            //showMessage(obj);
        }
    }

    public class Tool
    {
        public static void WriteLog(string sMsg)
        {
            try
            {
                string sUrl = System.Web.HttpContext.Current.Server.MapPath("~/TempFile");
                string sPath = sUrl + "/Test" + System.DateTime.Today.ToString("yyyy-MM-dd")+".txt";
                WebUse.Logs.WriteLog(sPath, sMsg);
            }
            catch (Exception oExcept)
            {
                WebUse.Logs.WriteLog(System.Web.HttpContext.Current.Server.MapPath("~/TempFile")+".txt", "写日志方法出错--------" + oExcept.Message);
            }
        }

        public static void ShowModalDialog(string sUrl,System.Web.UI.Page _Page)
        {
            try
            {
                string strScript = "<script language='javascript'>window.showModalDialog('" + sUrl + "')</script>";
                _Page.ClientScript.RegisterStartupScript(_Page.GetType(), Guid.NewGuid().ToString(), strScript);
            }
            catch(Exception oExcept)
            {
                //Tool.WriteLog(oExcept.Message);
            }
        }

        public static void Recurrence(int iNum)
        {
            try
            {
                //WriteLog(iNum.ToString());
                int iNumA = iNum;
                iNum = iNum-1;
                if (iNum > 0)
                {
                    Tool.Recurrence(iNum);
                }
                else return;
            }
            catch (Exception oExcept)
            {
                WriteLog("=================递归算法出错=================\n" + oExcept.Message);
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
                Tool.WriteLog(oExcept.Message);
            }
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
        public static string SendMessage(string sUserList, string sMsg, string sUrl,string sGuid, string sMsgType, System.Web.UI.Page _Page)
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
                string sNoticeType="会议";//通知类型
                #region sUserList中包含有全局选项时
                if (sUserList.Contains("P-1,") || sUserList == "P-1")
                {
                    ds = db.GetDataSet("select * from flow_users t");
                    for (int m = 0; m < ds.Tables[0].Rows.Count; m++)//循环向用户发送消息
                    {
                        sUser = ds.Tables[0].Rows[m][0].ToString();
                        sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl+sGuid, _Page)+Tool.SendMeetingNotice(db,sUser,sDateTime,sGuid,sNoticeType,_Page);
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
                                if (arrSendedUsers.Contains("@" + ds.Tables[0].Rows[j][0] + "@")==false)//判断用户是否已收到消息，向未收到用户发送
                                {
                                    arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                    //Tool.WriteLog("第"+(j+1)+"次发送"+sUser);
                                    //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                    sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl+sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
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
                                sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl+sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
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
                                if (arrSendedUsers.Contains(ds.Tables[0].Rows[j][0])==false)//判断用户是否已收到消息，向未收到用户发送
                                {
                                    arrSendedUsers.Add("@" + ds.Tables[0].Rows[j][0] + "@");
                                    sUser = ds.Tables[0].Rows[j][0].ToString();
                                    //sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl, _Page);
                                    sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl+sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
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
                            sReturn = Tool.SendMessage(db, sUser, sMsg, sDateTime, sMsgType, sUrl+sGuid, _Page) + Tool.SendMeetingNotice(db, sUser, sDateTime, sGuid, sNoticeType, _Page);
                        }
                    }
                #endregion
                string sSendedUsers = string.Empty;
                for (int n = 0; n < arrSendedUsers.Count; n++)
                {
                    sSendedUsers = sSendedUsers + arrSendedUsers[n];
                }
                Tool.WriteLog(sSendedUsers);
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
                Alert(oExcept.Message,_Page);
                WriteLog("===================================报错===================================\n"+oExcept.Message);
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
                WriteLog("===================================报错===================================\n" + oExcept.Message);
                return "发送失败："+oExcept.Message;
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
        public static string SendMeetingNotice(DatabaseORC db,string sUserID,string sDateTime,string sGuid,string sNoticeType,System.Web.UI.Page _page)
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
                Tool.WriteLog(sSendedUsers);
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
                WriteLog("===================================报错===================================\n" + oExcept.Message);
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
        public static string SendFiles(DatabaseORC db,string sUserID,string sDateTime,string sGuid,string sSignType,System.Web.UI.Page _Page)
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
                WriteLog(oExcept.Message);
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
                WriteLog("===================================报错===================================\n" + oExcept.Message);
                return "SendMessage错误：" + oExcept.Message;
            }
        }

    }
}
