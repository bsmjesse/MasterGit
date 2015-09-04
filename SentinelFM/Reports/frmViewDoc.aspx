<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmViewDoc.aspx.cs" Inherits="SentinelFM.Reports_frmViewDoc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <script type="text/javascript" >
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }
       
    </script>
    <div>
    <iframe style="border:0; height:100%; width:100%" src="<%=messageUrl %>" ></iframe>
    </div>
    </form>
</body>
</html>
