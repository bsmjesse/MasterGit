<%@ Page language="c#" Inherits="SentinelFM.Admin.frmAdminMenu" CodeFile="frmAdminMenu.aspx.cs" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>frmAdminMenu</title>
		<meta name="GENERATOR" content="Microsoft Visual Studio 7.0" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
		<link href="GlobalStyle.css" type="text/css" rel="stylesheet" />
	</head>
	<body style="font-family: Arial, Verdana">
		<form method="post" runat="server">
			<div id="Table1" class="formtext"  style="border: outset 1px #009933; margin: 30px 20px 20px 250px; width: 630px; padding: 20px 20px 20px 20px">
				<h3>SentinelFM Administration System v 2.0</h3>
				<hr style="color: #009933;"/>
				<br />
				<h4>Main Menu:</h4>
				<br />
                    <ul style="list-style:none;">
                    
                    <!--
                     <li>
                            <a href="frmLogViewer.aspx" target="_blank" class="link" style="color: black">View Log data</a>
                            <br /><br />
                            Displaying logfiles data.
                            <br /><br />
                        </li>
                        -->
                        
                        
                         <li>
                            <a href="frmBoxIncomingCmds.aspx" target="_blank" class="link" style="color: black">Incoming Messages from Box</a>
                            <br /><br />
                            Displaying Incoming messgae like: Box Firmware...    
                            <br /><br />
                        </li>
                        
                        
                         <li>
                            <a href="frmBoxSentCmds.aspx" target="_blank" class="link" style="color: black">Box Outgoing commands</a>
                            <br /><br />
                            Displaying Box Outgoing commands sent by user. Like: Set Odometer..
                            <br /><br />
                        </li>
                        
                        
                        <li>
                            <a href="frmHistoryMsgs.aspx" target="_blank" class="link" style="color: black">View Messages</a>
                            <br /><br />
                            *Displaying messages (incoming/outgoing) with advanced filtering.
                            <br /><br />
                        </li>
                        <li>
                            <a class="link" href="frmFWUpdate.aspx" style="color: black" target="_blank">FirmWare Upload</a>
                            <br /><br />
                            *Manage box firmware - OTA functions
                            <br /><br />
                        </li>
                        <li>
                            <a class="link" style="color: black" href="frmFWResults.aspx" target="_blank">FirmWare Upload Results</a>
                            <br /><br />
                            *View upload firmware status
                            <br /><br />
                        </li>
                        <li>
                            <a class="link" href="frmReports.aspx" style="color: black" target="_blank">Reports</a>
                            <br /><br />
                            <!--*Organization's air usage report <br />-->
	                        *Box air usage report<br />
                            *User Login report<br />
	                        *Exception Usage Report<br/>
	                        *Map Usage Report<br/>
                            *Network Latency Report<br /><br />
                        </li>
                        <li>
                            <a class="link" href="frmDiagnostic.aspx" style="color: black" target="_blank">Box Diagnostics</a>
                            <br /><br />
                            *Box Diagnostic Report 
                            <br /><br />
                        </li>
                        <li>
                            <a class="link" href="frmBoxFindMsgs.aspx" style="color: black" target="_blank">Box Find Missing Messages</a>
                            <br /><br />
                            *Box Diagnostics Utility 
                            <br /><br />
                        </li>

                        <li>
                            <a class="link" style="color: black" href="frmLoginFaileds.aspx" target="_blank">SentinelFM Login Manager</a>
                            <br /><br />
                            *Reset Locked SentinelFM Accounts (Security Manager locks the account after 3 invalid retries)
                            <br /><br />
                        </li>
                        <li>
                            <a class="link" style="color: black" href="frmKMLHistory.aspx" target="_blank">KML History</a>
                            <br /><br />
                            *Export History to KML format
                            <br /><br />
                        </li>
                        <li>
                            <a class="link" style="color: black" href="http://asi1.sentinelfm.com/TrafficViewer/TrafficViewer.aspx" target="_blank">Traffic Viewer</a>
                            <br /><br />
                            *View communication/air traffic
                            <br /><br />
                        </li>
<%--                        <li>
                            <a class="link" style="color: black" href="frmLandmarks.aspx" target="_blank">Manage Landmarks</a>
                            <br /><br />
                            *Import landmarks from Excel (a specific format is required)
                            <br /><br />
                        </li>--%>
                        <li>
                            <a class="link" style="color: black" href="frmInfo.aspx" target="_blank">Boxes Report</a>
                            <br /><br />
                            *Advanced filtering utility
                            <br /><br />
                        </li>
                        <li>
                            <a class="link" style="color: black" href="frmInstallJobs.aspx" target="_blank">Install Jobs</a>
                            <br /><br />
                            *Install jobs management
                            <br /><br />
                        </li>
                        
                        
                        <li>
                            <a class="link" style="color: black" href="frmSystemUpdate.aspx" target="_blank">System Updates</a>
                            <br /><br />
                            System Updates
                            <br /><br />
                        </li>
                        
                        
                          <li>
                            <a class="link" style="color: black" href="frmMdtOTAFirmware.aspx" target="_blank">MDT OTA Firmware</a>  &nbsp;&nbsp;/&nbsp;&nbsp;<a class="link" style="color: black" href="frmMdtOTAForms.aspx" target="_blank">MDT OTA Forms</a>
                            <br /><br />
                            MDT OTA
                            <br /><br />
                        </li>
                        
                        
                         <li>
                            <a class="link" style="color: black" href="StatisticReports/frmOrganizationStatistic.aspx" target="_blank">Organization Statistics</a> 
                             &nbsp;&nbsp;<br /><br />
                             Organization Statistics Info
                            <br /><br />
                        </li>


			      <li>
                        
                            <a class="link" style="color: black" href="FirmwareMinder.aspx" target="_blank">Firmware Minder</a> 
                             &nbsp;&nbsp;<br /><br />
                            Firmware Minder
                            <br /><br />
                        
                            
                        </li>

			      <li>
                        
                            <a class="link" style="color: black" href="frmPanicRegistry.aspx" target="_blank">Panic Registry</a> 
                             &nbsp;&nbsp;<br /><br />
                            Panic Device Settings
                            <br /><br />
                        
                            
                        </li>

  <li>
                        
                            <a class="link" style="color: black" href="frmCNInvoice.aspx" target="_blank">CN Electronic Invoice</a> 
                             &nbsp;&nbsp;<br /><br />
                           CN Electronic Invoice
                            <br /><br />
                        
                            
                        </li>

                         <li>
                            <a class="link" style="color: black" href="frmHOSForms.aspx" target="_blank">HOS Dynamic Forms</a> 
                             &nbsp;&nbsp;<br /><br />
                             Define HOS Dynamic Forms, forms assignment
                            <br /><br />
                        </li>


                   </ul>
                   <hr style="color: #009933;"/>
                   <a class="link" href="frmAdminLogin.aspx" style="color: black; text-indent: 40px;">Logout</a>
            </div>
		</form>
	</body>
</html>
