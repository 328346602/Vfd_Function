using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Data;
using System.Net;
namespace CM.GY.DownloadFile
{
    public partial class DownloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Page.Request.QueryString["CASENO"] != null)
                {
                    //string CaseNo = Page.Request.QueryString["CASENO"];//获取页面参数
                    //string filePath = Tool.OutputFiles(CaseNo);//通过页面参数获取文件地址
                    string filePath = Tool.OutputRarFile(Page.Request.QueryString["CASENO"]);//通过页面参数获取文件地址
                    if (filePath != "0")
                    {
                        Response.Redirect("~" + filePath);
                    }
                    else
                    {
                        Response.Write("<script language='javascript'>alert('当前案卷未上传文件！');window.close();</script>");
                    }
                }
            }
            catch(Exception ex)
            {
                //Tool.WriteLog("Page_Load>>>>>"+ex.Message);
                throw ex;
            }
        }
    }
}