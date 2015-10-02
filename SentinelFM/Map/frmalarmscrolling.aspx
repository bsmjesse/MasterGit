<%@ Page language="c#" Inherits="SentinelFM.Map.frmAlarmScrolling" CodeFile="frmAlarmScrolling.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<head runat="server">
		<title>frmAlarmScrolling</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript">
		<!--
				setTimeout('location.reload(true)',60000)
		//-->
		</script>
	</HEAD>
	<body>
		<form method="post" runat="server">
			<table style="Z-INDEX: 101; LEFT: 16px; POSITION: absolute; TOP: 4px">
				<tr>
					<td>
						<script language="javascript">
							<!--
								var _Width="1000px";
								var _Height="20px";
								var _BackColor="white";
								var _PauseOnHover=true;
								var _Speed="2";
								var _ItemSpacing="15";
								var _Items=new Array();
								<%=AlarmList%>
							
							//-->
						</script>
						<script language="Javascript" src="uiscroller.js"></script>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
