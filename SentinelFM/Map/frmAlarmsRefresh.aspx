<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmAlarmsRefresh.aspx.cs" Inherits="SentinelFM.Map_frmAlarmsRefresh" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
        <script language="javascript">
			<!--
		
   			 setTimeout('location.reload(true)',<%=sn.User.AlarmRefreshFrequency%>)
			//-->
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
