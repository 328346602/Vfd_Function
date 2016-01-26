using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;

namespace Function
{
    public class Function : Visual_Form_Designer.Class.IFunction
    {
        private string Rev="";
		private string ErrorMsg="";
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
                SendSMS(_Page, _Context, _Service, _WebPageConfig, ParamaterList, _CustomObject);
                
                
                return true;
            }
            catch(Exception ex)
            {
                Log.WriteError(ex.Message);
                return false;
            }
			
		}

        public bool SendSMS(System.Web.UI.Page _Page, System.Web.HttpContext _Context, Visual_Form_Designer.Class.VFDServiceObject _Service, Visual_Form_Designer.Class.WebPageConfig _WebPageConfig, System.Collections.Hashtable ParamaterList, object _CustomObject)
        {
            try
            {
                Tool.ShowLoading(_Page);
                #region 定义变量
                TextBox tbDepartID = (TextBox)_Page.FindControl("txtDepartID");
                TextBox tbFromUser=(TextBox)_Page.FindControl("txtFromUser");
                TextBox tbMsg=(TextBox)_Page.FindControl("txtMsg");
                TextBox tbFromDepart = (TextBox)_Page.FindControl("txtFromDepart");
                Button btnSend = (Button)_Page.FindControl("btnSend");
                Button btnSelectPerson = (Button)_Page.FindControl("btnSelectPerson");
                string departList = tbDepartID.Text;//用户及机构ID列表
                string FromUser = tbFromUser.Text;//短信发送人
                string FromDepart = tbFromDepart.Text;//信息发送部门
                string Msg = string.Format(tbMsg.Text + "--局{0}", FromDepart);//短信内容
                ArrayList arrUserList = SMS.GetToUserList(departList);//用户列表
                int smsLength;
                try
                {
                    smsLength = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SMSLength"].ToString());//配置文件读取短信总长度限制
                }
                catch (Exception ex)
                {
                    smsLength = 140;//默认140
                    Log.WriteDebug("获取SMSLength配置出差>>>>>" + ex.Message);
                }
                Log.WriteDebug("arrUserList>>>>>"+arrUserList.Count.ToString());
                Log.WriteDebug("departList>>>>>" + departList);
                #endregion

                #region 检查页面填写内容
                if (FromUser.Length < 1)
                {
                    ErrorMsg = "未能取得当前用户信息，请联系管理员！";
                    Tool.HiddenLoading(_Page);
                    return false;
                } 
                else if (FromDepart.Length < 1)
                {
                    ErrorMsg = "请填写发送部门！";
                    Tool.HiddenLoading(_Page);
                    return false;
                }
                else if (departList.Length < 1 || arrUserList.Count < 1)
                {
                    //Tool.Alert("请选择接收人！",_Page);
                    ErrorMsg = "请选择接收人！";
                    Tool.HiddenLoading(_Page);
                    return false;
                }
                else if (Msg.Length == string.Format("--局{0}", FromDepart).Length)
                {
                    ErrorMsg = "请填写短信正文内容！";
                    Tool.HiddenLoading(_Page);
                    return false;
                }
                else if (Msg.Length > smsLength)
                {
                    ErrorMsg = string.Format("短信内容不能多于{0}字，当前共计{1}字，请重新编辑！", smsLength - string.Format("--局{0}", FromDepart).Length, Msg.Length);
                    Tool.HiddenLoading(_Page);
                    return false;
                }
                #endregion

                else
                {
                    try
                    {
                        SMS.SnedSMS(arrUserList, Msg, FromUser,_Page);
                        btnSend.Visible = false;
                        btnSelectPerson.Visible = false;
                        //Tool.ShowSuspendMsg("发送成功！", _Page);
                        Tool.Alert("发送成功！", _Page);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteError(ex.Message);
                        Tool.Alert("发送失败，请联系管理员查看错误原因！", _Page);
                        Tool.HiddenLoading(_Page);
                        return false;
                    }
                }
                Tool.HiddenLoading(_Page);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteError(ex.Message);
                ErrorMsg = ex.Message;
                Tool.Alert("发送失败，请联系管理员查看错误原因！", _Page);
                Tool.HiddenLoading(_Page);
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
	}
}
