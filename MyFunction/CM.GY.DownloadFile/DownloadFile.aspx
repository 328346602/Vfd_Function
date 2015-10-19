<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DownloadFile.aspx.cs" Inherits="CM.GY.DownloadFile.DownloadFile" %>

<!DOCTYPE html>
<style>
    #d_Wait_Back
    {
        height: 0;
        width: 0;
        position: absolute;
        top: 0px;
        z-index: 98;
        display: none;
        background-image: url('../Content/themes/default/images/bg.png');
    }
    #d_Wait
    {
        height: 100%;
        width: 100%;
        position: absolute;
        background-image: url('../Content/themes/default/images/loading.gif');
        background-repeat: no-repeat;
        background-position: center;
        z-index: 99;
    }
</style>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>稍后开始下载……</title>
    <link href="@Url.Content("~/Content/themes/default/Site.css")" rel="stylesheet" type="text/css" />
</head>
<body>
    <p>
        稍后开始下载……</p>
</body>
</html>
