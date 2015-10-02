<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMesssagesExtended.aspx.cs" Inherits="SentinelFM.Messages_frmMesssagesExtended"  Culture="en-US"  UICulture="auto" meta:resourcekey="PageResource2" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>

<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
   TagPrefix="ISWebInput" %>

<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>

<%@ Register assembly="ISNet.WebUI.WebCombo" namespace="ISNet.WebUI.WebCombo" tagprefix="ISWebCombo" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>


<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >



<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>frmMessages</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script language="javascript">
		<!--

			function ScrollColor()
			{
				with(document.body.style)
				{
				scrollbarDarkShadowColor="003366";
				scrollbar3dLightColor="gray";
				scrollbarArrowColor="gray";
				scrollbarBaseColor="FFFFFF";
				scrollbarFaceColor="FFFFFF";
				scrollbarHighlightColor="gray";
				scrollbarShadowColor="black";
				scrollbarTrackColor="whitesmoke";
				}
			}
			
			
		function MessageInfoWindow(MsgKey) { 
					var mypage='frmMessageInfo.aspx?MsgKey='+MsgKey
					var myname='';
					var w=335;
					var h=360;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
				function NewDriverMsg() { 
    				
                    var combo = ISGetObject("cboDrivers");
                    var driverId=combo.Value;
					var mypage='frmNewDriverMsg.aspx?driverId='+driverId;
					var myname='';
					var w=735;
					var h=480;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
					function NewLocationsWindow() { 
					var mypage='frmNewLocation.aspx'
					var myname='';
					var w=735;
					var h=360;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
				
					function MsgDetails(mt,id,mode) { 
					var mypage='MessageDetail.aspx?mt='+mt+'&id='+id+'&mode='+mode;
					var myname='';
					var w=400;
					var h=600;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=yes,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
		//-->
    </script>

   

</head>
<body  >
    <form id="frmMessagesForm" method="post" runat="server">
    
    
            <fieldset>
                <table class="formtext" style="width: 720px" >
                                <tr>
                                   <td align="left" colspan="4">
                                
                                         <asp:RadioButtonList ID="optMessageType" runat="server" CssClass="formtext" 
                                             RepeatDirection="Horizontal" AutoPostBack="True" 
                                             OnSelectedIndexChanged="optMessageType_SelectedIndexChanged" meta:resourcekey="optMessageTypeResource3" 
                                              >
                                  <asp:ListItem Selected="True" Value="0"  Text="Text Messages" 
                                                 meta:resourcekey="ListItemResource17"></asp:ListItem>
                                  <asp:ListItem Value="1"  Text="Alarms" meta:resourcekey="ListItemResource18"></asp:ListItem>
                                  <asp:ListItem Value="2"  Text="Scheduled Tasks" meta:resourcekey="ListItemResource19"></asp:ListItem>
                                   <asp:ListItem Value="4" meta:resourcekey="ListItemResource20">Garmin</asp:ListItem>
                                            
                               </asp:RadioButtonList>
                                       
                                       </td>
                                </tr>
                                <tr>
                                   <td style="width: 100px" align="left">
                                
                                <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1"
                                    Text="From:"></asp:Label></td>
                                   <td align="left">
                                   
                                      <table  border=0px cellpadding=0 cellspacing=0   >
                                         <tr>
                                            <td style="height: 21px" Width="181px">
                                            <ISWebInput:WebInput ID="txtFrom" runat="server" Width="171px"  Height="17px" 
                                                    Wrap="Off" MaxDate="12/31/9998 23:59:59" meta:resourcekey="txtFromResource2" 
                                                    MinDate="1753-01-01"  >

<CultureInfo CultureName="en-US"></CultureInfo>

<DisplayFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</DisplayFormat>

                                                  <HighLight IsEnabled="True" Type="Phrase" />
<EditFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</EditFormat>

					                                    <DateTimeEditor IsEnabled="True" AccessKey="Space">
                                              </DateTimeEditor>   
                            
                                             </ISWebInput:WebInput>
                                             </td>
                                            <td >
                                              <asp:DropDownList ID="cboHoursFrom" runat="server" CssClass="RegularText" Width="76px"
                                    meta:resourcekey="cboHoursFromResource1" Height="21px">
                                </asp:DropDownList></td>
                                         </tr>
                                      </table>
                                   </td>
                                   <td align="left">
                                <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1"
                                    Text="To:"></asp:Label>
                                     
                                   </td>
                                   <td align="left" >
                                    
                                      <table border=0px cellpadding=0 cellspacing=0 >
                                         <tr>
                                            <td style="height: 21px" Width="181px">
                                            <ISWebInput:WebInput ID="txtTo" runat="server" Width="171px"  Height="17px" 
                                                    Wrap="Off" MaxDate="12/31/9998 23:59:59" meta:resourcekey="txtToResource2" 
                                                    MinDate="1753-01-01"  >

<CultureInfo CultureName="en-US"></CultureInfo>

<DisplayFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</DisplayFormat>

                                                        <HighLight IsEnabled="True" Type="Phrase" />
<EditFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</EditFormat>

					                                      <DateTimeEditor IsEnabled="True" AccessKey="Space">
                                                 </DateTimeEditor>   
                        </ISWebInput:WebInput>
                                </td>
                                            <td style="height: 21px" >
                                               <asp:DropDownList ID="cboHoursTo" runat="server" CssClass="RegularText" Width="75px" meta:resourcekey="cboHoursToResource1" Height="21px">
                                </asp:DropDownList></td>
                                         </tr>
                                      </table>
                                   </td>
                                </tr>
                                <tr>
                                   <td align="left" >
                                
                                <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1"
                                    Text="Fleet:"></asp:Label>
                                <asp:RangeValidator ID="valFleet" runat="server" MaximumValue="999999999999999" MinimumValue="1"
                                    ErrorMessage="Please select a Fleet" ControlToValidate="cboFleet" meta:resourcekey="valFleetResource1"
                                    Text="*"></asp:RangeValidator></td>
                                   <td style="width: 100px">
                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="250px"
                                    DataValueField="FleetId" DataTextField="FleetName" AutoPostBack="True" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                    meta:resourcekey="cboFleetResource1">
                                </asp:DropDownList></td>
                                   <td style="width: 100px" align="left">
                            
                                <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" Width="30px" 
                                    Visible="False" meta:resourcekey="lblVehicleNameResource1" Text=" Vehicle:"></asp:Label>
                                <asp:RequiredFieldValidator ID="valVehicle" runat="server" ControlToValidate="cboVehicle"
                                    ErrorMessage="Please select a Vehicle" meta:resourcekey="RequiredFieldValidator1Resource1"
                                    Text="*"></asp:RequiredFieldValidator></td>
                                   <td style="width: 100px">
                                <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" Width="250px"
                                    DataValueField="BoxId" DataTextField="Description" Visible="False"
                                     meta:resourcekey="cboVehicleResource1">
                                </asp:DropDownList></td>
                                </tr>
                                <tr>
                                   <td align="left" valign=top colspan="4" >
                             
                                       <busyboxdotnet:busybox
                                       id="BusyReport" runat="server" anchorcontrol="" compressscripts="False" gzipcompression="False"
                                       meta:resourcekey="BusyReportResource1" slideeasing="BackIn"
                                       text=""></busyboxdotnet:busybox>
                                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="formtext" 
                                           meta:resourcekey="valSummaryResource1">
                                </asp:ValidationSummary>
                              
                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="138px" 
                                    Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                </tr>
                             </table>
                    </fieldset> 
                    
                    
                     <asp:MultiView ID="MultiviewMessages" runat="server" ActiveViewIndex="0">
                        <asp:View ID="TextMessages" runat="server">
                        <table ID="tblMsgButtons" runat="server" cellpadding="2" cellspacing="2">
                                <tr runat="server">
                                    <td align="left"  runat="server">
                                        <asp:Label ID="lblFolderListTitle" runat="server" CssClass="formtext" 
                                            meta:resourcekey="lblFolderListTitleResource1" Text="Folder List:"></asp:Label>
                                    </td>
                                    <td align="left" runat="server">
                                        <asp:DropDownList ID="cboDirection" runat="server" CssClass="RegularText" 
                                            meta:resourcekey="cboDirectionResource1" Width="250px">
                                            <asp:ListItem meta:resourcekey="ListItemResource1" Text="All" Value="0"></asp:ListItem>
                                            <asp:ListItem meta:resourcekey="ListItemResource2" Text="Incoming" Value="1"></asp:ListItem>
                                            <asp:ListItem meta:resourcekey="ListItemResource3" Text="Outgoing" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td align="center" runat="server">
                                        <asp:Button ID="cmdNewMessage" runat="server" CausesValidation="False" 
                                            CommandName="25" CssClass="combutton" meta:resourcekey="cmdNewMessageResource1" 
                                            OnClick="cmdNewMessage_Click" Text="New Text Message" Width="136px" />
                                        <asp:Button ID="cmdShowMessages" runat="server" CssClass="combutton" 
                                            meta:resourcekey="cmdShowMessagesResource1" OnClick="cmdShowMessages_Click" 
                                            Text="View Text Messages" Width="136px" />
                                        <asp:Button ID="cmdMarkAsRead" runat="server" CssClass="combutton" 
                                            meta:resourcekey="cmdMarkAsReadResource1" OnClick="cmdMarkAsRead_Click" 
                                            Text="Mark as read" Width="136px" />
                                        <asp:CheckBox ID="chkAuto" runat="server" CssClass="formtext" Font-Bold="False" 
                                            meta:resourcekey="chkAutoResource1" Text="Auto Time Refresh" />
                                    </td>
                                </tr>
                            </table>
                            
                           
                           
                           <ISWebGrid:WebGrid ID="dgMessages" runat="server" Height="450px" 
                                meta:resourcekey="dgMessagesResource2" 
                                OnInitializeDataSource="dgMessages_InitializeDataSource" 
                                OnInitializeLayout="dgMessages_InitializeLayout" UseDefaultStyle="True" 
                                ViewStateStorage="Session" Width="100%"><RootTable DataKeyField="MsgId"><Columns><ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" 
                                            Bound="False" Caption="chkBox" ColumnType="CheckBox" DataMember="chkBox" 
                                            EditType="NoEdit" IsRowChecker="True" Name="Select Row" 
                                            ShowInSelectColumns="No" Width="25px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_MsgDateTime %>" 
                                            DataMember="MsgDate" DataType="System.DateTime" Name="MsgDate" Width="70px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="Address" DataMember="StreetAddress" 
                                            DataType="System.String" Name="StreetAddress" Width="150px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_From %>" 
                                            DataMember="From" Name="From" Width="70px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_To %>" 
                                            DataMember="To" Name="To" Width="70px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_MsgDirection %>" 
                                            DataMember="MsgDirection" Name="MsgDirection" Width="35px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_MsgBody %>" 
                                            DataMember="MsgBody" Name="MsgBody" Width="200px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_MsgResponse %>" 
                                            DataMember="MsgResponse" Name="MsgResponse" Width="35px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_Acknowledged %>" 
                                            DataMember="Acknowledged" Name="Acknowledged" Width="35px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="<%$ Resources:dgMessages_UserName %>" 
                                            DataMember="UserName" Name="UserName" Width="50px"></ISWebGrid:WebGridColumn></Columns></RootTable><LayoutSettings AllowColumnMove="Yes" AllowContextMenu="False" 
                                    AllowExport="Yes" AllowFilter="Yes" AllowSorting="Yes" 
                                    AutoFilterSuggestion="True" AutoFitColumns="True" ColumnSetHeaders="Default" 
                                    DisplayDetailsOnUnhandledError="False" HideColumnsWhenGrouped="Default" 
                                    PersistRowChecker="True" ResetNewRowValuesOnError="False" 
                                    RowChangedAction="OnTheFlyPostback" RowHeaders="Default" 
                                    RowHeightDefault="25px" RowLostFocusAction="NeverUpdate"><FreezePaneSettings AbsoluteScrolling="True" /></LayoutSettings></ISWebGrid:WebGrid>
                                    
                                    
                            
                        </asp:View>
                           <asp:View ID="ScheduledTasks" runat="server">
                           
                            <table ID="tblSchButtons" runat="server" cellpadding="2" cellspacing="2">
                                       <tr runat="server">
                                           <td align="left" runat="server">
                                               <asp:Button ID="cmdScheduledTasks" runat="server" CommandName="25" 
                                                   CssClass="combutton" meta:resourcekey="cmdScheduledTasksResource1" 
                                                   OnClick="cmdScheduledTasks_Click" Text="View Scheduled Tasks" Width="170px" />
                                               <asp:Button ID="cmdDeleteScheduledTasks" runat="server" CssClass="combutton" 
                                                   onclick="cmdDeleteScheduledTasks_Click" Text="Delete Scheduled Tasks"  meta:resourcekey="cmdDeleteScheduledTasksResource1" 
                                                   Width="170px" />
                                               <asp:Label ID="lblScheduledMessageFilter" runat="server" CssClass="formtext" 
                                                   meta:resourcekey="lblScheduledMessageFilterResource1" 
                                                   Text="Scheduled Messages:" Visible="False"></asp:Label>
                                               <asp:DropDownList ID="cboScheduledMessageFilter" runat="server" 
                                                   CssClass="formtext" meta:resourcekey="cboFormsResource1" Visible="False" 
                                                   Width="250px">
                                                   <asp:ListItem meta:resourcekey="ListItemResource9" Text="All" Value="1"></asp:ListItem>
                                                   <asp:ListItem meta:resourcekey="ListItemResource10" Text="Sent" Value="2"></asp:ListItem>
                                               </asp:DropDownList>
                                               <asp:Label ID="lblDeleteScheduleTaskMsg" runat="server" CssClass="formtext" 
                                                   Text="* Please note: only &quot;Pending&quot; tasks can be deleted" meta:resourcekey="lblDeleteScheduleTaskMsgResource1" ></asp:Label>
                                           </td>
                                       </tr>
                                   </table>
                                   
                                  
                                  <ISWebGrid:WebGrid ID="dgSched" runat="server" UseDefaultStyle="True" 
                    Width="100%" Height="450px" ViewStateStorage="Session" 
                    OnInitializeDataSource="dgSched_InitializeDataSource" 
                    OnInitializeLayout="dgSched_InitializeLayout" 
                    meta:resourcekey="dgSchedResource1"><RootTable DataKeyField="TaskId"><Columns><ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                               Caption="chkBox" ColumnType="CheckBox" DataMember="chkBox" EditType="NoEdit" IsRowChecker="True"
                               Name="Select Row" ShowInSelectColumns="No" Width="25px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Date %>' Name="RequestDateTime" DataMember="RequestDateTime" DataType="System.DateTime"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Vehicle %>' Name="Description" DataMember="Description" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Command %>' Name="BoxCmdOutTypeName" DataMember="BoxCmdOutTypeName" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_LastTryDate %>' Name="LastDateTimeSent" DataMember="LastDateTimeSent" DataType="System.DateTime"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Sent %>' DataMember="MsgOutDateTime" Name="MsgOutDateTime" DataType="System.DateTime"
                               ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgSched_Status %>' DataMember="CommandStatus" Name="CommandStatus" DataType="System.String"
                               ></ISWebGrid:WebGridColumn></Columns></RootTable><LayoutSettings  AllowColumnMove="Yes" AllowContextMenu="False" AllowFilter="Yes"   RowHeightDefault=25px      
                      AllowSorting="Yes"    AutoFilterSuggestion="True" AutoFitColumns="True" PersistRowChecker="True"
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default" ResetNewRowValuesOnError="False" RowHeaders="Default"  AllowExport="Yes" DisplayDetailsOnUnhandledError="False" ><FreezePaneSettings AbsoluteScrolling="True" /></LayoutSettings></ISWebGrid:WebGrid>
                      
                                  
                        </asp:View>
                           <asp:View ID="Alarms" runat="server">
                           
                            <table ID="tblAlarms" runat="server" cellpadding="2" cellspacing="2" 
                                      style="width: 328px">
                                      <tr runat="server">
                                          <td align="center" colspan="1" runat="server">
                                              <asp:Button ID="cmdViewAlarms" runat="server" CommandName="25" 
                                                  CssClass="combutton" meta:resourcekey="cmdViewAlarmsResource1" 
                                                  OnClick="cmdViewAlarms_Click" Text="View Alarms" Width="136px" />
                                          </td>
                                          <td align="center" colspan="1" runat="server">
                                              <asp:Button ID="cmdAccept" runat="server" CommandName="25" CssClass="combutton" 
                                                  meta:resourcekey="cmdAcceptResource1" OnClick="cmdAccept_Click" 
                                                  Text="Accept Alarms" Width="136px" />
                                          </td>
                                          <td align="center" colspan="1" runat="server">
                                              <asp:Button ID="cmdCloseAlarms" runat="server" CommandName="25" 
                                                  CssClass="combutton" meta:resourcekey="cmdCloseAlarmsResource1" 
                                                  OnClick="cmdCloseAlarms_Click" Text="Close Alarms" Width="136px" />
                                          </td>
                                      </tr>
                                  </table>
                                  
                                  
                                  <ISWebGrid:WebGrid ID="dgAlarms" runat="server" UseDefaultStyle="True" 
                    Width="100%" OnInitializeDataSource="dgAlarms_InitializeDataSource" 
                    Height="450px" ViewStateStorage="Session" 
                    OnInitializeLayout="dgAlarms_InitializeLayout" 
                    meta:resourcekey="dgAlarmsResource2" ><RootTable DataKeyField="AlarmId"><Columns><ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                                Caption="chkBox" ColumnType="CheckBox" DataMember="chkBox" EditType="NoEdit" IsRowChecker="True"
                                Name="Select Row" ShowInSelectColumns="No" Width="25px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_Date %>' DataMember="AlarmDate" DataType="System.DateTime"
                                Name="AlarmDate" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption="Address" DataMember="StreetAddress" DataType="System.String"
                                Name="StreetAddress" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_vehicleDescription %>' DataMember="vehicleDescription" Name="vehicleDescription"
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_AlarmDescription %>' DataMember="AlarmDescription" Name="AlarmDescription"
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_AlarmLevel %>' DataMember="AlarmLevel" Name="AlarmLevel"
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_AlarmState %>' DataMember="AlarmState" Name="AlarmState"
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_UserName %>' DataMember="UserName" Name="UserName" Width="50px"></ISWebGrid:WebGridColumn></Columns></RootTable><LayoutSettings  AllowColumnMove="Yes" AllowContextMenu="False" AllowFilter="Yes" RowChangedAction=OnTheFlyPostback  RowHeightDefault=25px      
                      AllowSorting="Yes"    AutoFilterSuggestion="True" AutoFitColumns="True"
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default" ResetNewRowValuesOnError="False" RowHeaders="Default"  PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" DisplayDetailsOnUnhandledError="False"><FreezePaneSettings AbsoluteScrolling="True" /></LayoutSettings></ISWebGrid:WebGrid>
                      
                      
                                 
                        </asp:View>
                           <asp:View ID="DriverMessages" runat="server">
                           
                                <table ID="tblDriverViewButtons" runat="server" cellpadding="2" cellspacing="2">
                                              <tr runat="server">
                                                  <td align="center" colspan="1" runat="server">
                                                  <asp:Label ID="lblDriver" runat="server" meta:resourcekey="lblDriverResource1" 
                                                          Text="Driver:" CssClass="formtext"></asp:Label>
                                                      
                                                  </td>
                                                  <td align="center" runat="server">
                                                      &nbsp;</td>
                                                  <td align="center" runat="server">
                                                      <ISWebCombo:WebCombo ID="cboDrivers" runat="server" 
                                                          DataTextField="FullNameAndEmail" DataValueField="DriverId" Height="20px" 
                                                          meta:resourcekey="cboDriversResource1" 
                                                          OnInitializeDataSource="cboDrivers_InitializeDataSource" UseDefaultStyle="True" 
                                                          Width="250px"></ISWebCombo:WebCombo>
                                                      <asp:Button ID="cmdViewDriverMsgs" runat="server" CssClass="combutton" 
                                                          meta:resourcekey="cmdViewDriverMsgsResource1" OnClick="cmdViewDriverMsgs_Click" 
                                                          Text="View Driver Messages" Width="145px" />
                                                  </td>
                                                  <td runat="server">
                                                      <asp:Button ID="cmdNewDriverMessage" runat="server" CssClass="combutton" 
                                                          meta:resourcekey="cmdNewDriverMessageResource1" OnClientClick="NewDriverMsg();" 
                                                          Text="New Messages" Width="145px" />
                                                  </td>
                                              </tr>
                                          </table>
                                          
                                           <ISWebGrid:WebGrid ID="dgDriverMsgs" runat="server" 
                        UseDefaultStyle="True" Width="100%" 
                        OnInitializeDataSource="dgDriverMsgs_InitializeDataSource" 
                    Height="450px" meta:resourcekey="dgDriverMsgsResource1"  ><RootTable ><Columns><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgDriverMsgs_DateTime %>' DataMember="MsgDateTime" DataType="System.DateTime"
                                Name="MsgDateTime" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgDriverMsgs_Message %>' DataMember="MsgBody" Name="MsgBody"  DataType="System.String"
                                 ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgDriverMsgs_Email %>' DataMember="Email" DataType="System.String"
                                Name="Email" ></ISWebGrid:WebGridColumn></Columns></RootTable><LayoutSettings  AllowColumnMove="Yes" AllowContextMenu="False" AllowFilter="Yes" RowChangedAction=OnTheFlyPostback  RowHeightDefault=25px      
                      AllowSorting="Yes"    AutoFilterSuggestion="True" AutoFitColumns="True"
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default" ResetNewRowValuesOnError="False" RowHeaders="Default"  PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" DisplayDetailsOnUnhandledError="False"><FreezePaneSettings AbsoluteScrolling="True" /></LayoutSettings></ISWebGrid:WebGrid>
                      
                      
                   
                                     
                    
                   
                        </asp:View>
                         <asp:View ID="Garmin" runat="server">
                                <table cellpadding=2 cellspacing=2  >
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblGarmin" runat="server" CssClass="formtext" 
                                                Text="Vehicle:" meta:resourcekey="lblGarminResource2"></asp:Label>
                                            <asp:RangeValidator ID="RequiredGarminVehicle" runat="server" 
                                                ControlToValidate="cboVehicleGarmin" Enabled="False" 
                                                ErrorMessage="Please select a vehicle!" MaximumValue="99999999" 
                                                MinimumValue="1" meta:resourcekey="RequiredGarminVehicleResource2">*</asp:RangeValidator>
                                        </td>
                                         <td>
                                             <asp:DropDownList ID="cboVehicleGarmin" runat="server" CssClass="formtext" 
                                                 DataTextField="Description" DataValueField="BoxId" Width="250px" 
                                                 meta:resourcekey="cboVehicleGarminResource2">
                                             </asp:DropDownList>
                                        </td>   
                                        <td>
                                            <asp:CheckBox ID="chkShowStatusInfo" runat="server" CssClass="formtext" 
                                                Text="Show Status" meta:resourcekey="chkShowStatusInfoResource2" />
                                        </td>
                                        
                                        <td>
                                            <asp:CheckBox ID="chkSendToAllVehicles" runat="server" CssClass="formtext" 
                                                Text="Send to All Vehicles"  />
                                        </td>
                                        
                                    </tr>
                                </table>
                             <ISWebDesktop:WebTab ID="WebGarminTab" runat="server" Height="100%" 
                                 
                                 Width="100%" meta:resourcekey="WebGarminTabResource2"><TabPages><ISWebDesktop:WebTabItem 
                                        ContentMode="UseInlineContent" Name="StandardReports" Text='<%$ Resources:WebTabTextMessages %>' ><PageTemplate><table 
                                            ID="Table2" runat="server" cellpadding="2" cellspacing="2"><tr 
                                            runat="server"><td runat="server" align="center"><asp:Button ID="cmdNewGarminMsg" 
                                                    runat="server" CssClass="combutton" OnClick="cmdNewGarminMsg_Click" 
                                                    Text="New Text Message" Width="136px" meta:resourcekey="cmdNewGarminMsgResource1"  />
                                                &#160;<asp:Button ID="cmdViewGarminHistory" runat="server" CssClass="combutton" 
                                                    OnClick="cmdViewGarminHistory_Click" Text="View Text Messages" Width="136px" meta:resourcekey="cmdViewGarminHistoryResource1"  />
                                            </td>
                                        </tr>
                                        </table>
                                        <ISWebGrid:WebGrid ID="dgGarminHist" runat="server" Height="450px" 
                                            meta:resourcekey="dgGarminHistResource2" 
                                            OnInitializeDataSource="dgGarminHist_InitializeDataSource" 
                                            UseDefaultStyle="True" Width="95%" 
                                             oninitializelayout="dgGarminHist_InitializeLayout">
                                            <LayoutSettings AllowColumnMove="Yes" AllowContextMenu="False" 
                                                AllowExport="Yes" AllowFilter="Yes"
                                                AutoFilterSuggestion="True" AutoFitColumns="True" ColumnSetHeaders="Default" 
                                                DisplayDetailsOnUnhandledError="False" HideColumnsWhenGrouped="Default" 
                                                PersistRowChecker="True" ResetNewRowValuesOnError="False" RowHeaders="Default" 
                                                RowLostFocusAction="NeverUpdate">
                                                <FreezePaneSettings AbsoluteScrolling="True" />
                                            </LayoutSettings>
                                            <RootTable>
                                                <Columns>
                                                    <ISWebGrid:WebGridColumn Caption=" " ColumnType="Image" DataMember="imageUrl" 
                                                        Name="imageUrl" Width="15px">
                                                    </ISWebGrid:WebGridColumn>
                                                    <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_DateTime %>' DataMember="originDateTime" 
                                                        DataType="System.DateTime" Name="originDateTime">
                                                    </ISWebGrid:WebGridColumn>
                                                    
                                                            <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_Message %>' ColumnType="Template" DataMember="data"
                                                               Name="data">
                                                            <CellTemplate>
                                                             <asp:HyperLink ID="HyperLink_Site" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>' Text='<%# DataBinder.Eval(Container.DataItem, "data") %>'  ></asp:HyperLink>
                                                            </CellTemplate>
                                                         </ISWebGrid:WebGridColumn>
                                                         
                                                    <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_Status  %>' DataMember="status" Name="status">
                                                    </ISWebGrid:WebGridColumn>
                                                    <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_Direction  %>' DataMember="direction" 
                                                        Name="direction" Width="20px">
                                                    </ISWebGrid:WebGridColumn>
                                                </Columns>
                                            </RootTable>
                                        </ISWebGrid:WebGrid>
                                    </PageTemplate>
                                    </ISWebDesktop:WebTabItem>
                                    <ISWebDesktop:WebTabItem ContentMode="UseInlineContent" Name="ExtendedReports" 
                                          Text='<%$ Resources:WebTabTextLocations %>' >
                                        <PageTemplate>
                                            <table ID="Table3" runat="server" cellpadding="2" cellspacing="2">
                                                <tr runat="server">
                                                    <td runat="server" align="center">
                                                        <asp:Button ID="cmdNewLocation" runat="server" CssClass="combutton" 
                                                            OnClick="cmdNewLocation_Click" Text="New Location" meta:resourcekey="cmdNewLocationResource1" Width="136px" />
                                                        &#160;<asp:Button ID="cmdViewLocations" runat="server" CssClass="combutton" 
                                                            OnClick="cmdViewLocations_Click" Text="View Locations" meta:resourcekey="cmdViewLocationsResource1" Width="136px" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <ISWebGrid:WebGrid ID="dgGarminHistLocations" runat="server" Height="450px" 
                                                meta:resourcekey="dgGarminHistLocationsResource2" 
                                                OnInitializeDataSource="dgGarminHistLocations_InitializeDataSource" 
                                                UseDefaultStyle="True" Width="95%" 
                                                oninitializelayout="dgGarminHistLocations_InitializeLayout">
                                                <LayoutSettings AllowColumnMove="Yes" AllowContextMenu="False" 
                                                    AllowExport="Yes" AllowFilter="Yes" 
                                                    AutoFilterSuggestion="True" AutoFitColumns="True" ColumnSetHeaders="Default" 
                                                    DisplayDetailsOnUnhandledError="False" HideColumnsWhenGrouped="Default" 
                                                    PersistRowChecker="True" ResetNewRowValuesOnError="False" RowHeaders="Default" 
                                                    RowLostFocusAction="NeverUpdate">
                                                    <FreezePaneSettings AbsoluteScrolling="True" />
                                                </LayoutSettings>
                                                <RootTable>
                                                    <Columns>
                                                        <ISWebGrid:WebGridColumn Caption=" " ColumnType="Image" DataMember="imageUrl" 
                                                            Name="imageUrl" Width="20px">
                                                        </ISWebGrid:WebGridColumn>
                                                        <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_DateTime %>' DataMember="originDateTime" 
                                                            DataType="System.DateTime" Name="originDateTime">
                                                        </ISWebGrid:WebGridColumn>
                                                        
                                                        
                                                        
                                                        <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_Message %>' ColumnType="Template" DataMember="data"
                                                               Name="data">
                                                            <CellTemplate>
                                                             <asp:HyperLink ID="HyperLink_Site" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>' Text='<%# DataBinder.Eval(Container.DataItem, "data") %>'  ></asp:HyperLink>
                                                            </CellTemplate>
                                                         </ISWebGrid:WebGridColumn>
                                                         
                                                        
                                                        <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_Address %>' DataMember="location" Name="location">
                                                        </ISWebGrid:WebGridColumn>
                                                        
                                                        
                                                        <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgGarminHist_Status  %>' DataMember="status" Name="status">
                                                        </ISWebGrid:WebGridColumn>
                                                    </Columns>
                                                </RootTable>
                                            </ISWebGrid:WebGrid>
                                        </PageTemplate>
                                    </ISWebDesktop:WebTabItem>
                                </TabPages>
                                <FrameStyle Overflow="Hidden" OverflowX="Hidden" OverflowY="Hidden"></FrameStyle><TabItemStyle><Normal BackColor="Gainsboro" BorderColor="Gray" BorderStyle="Solid" 
                                         BorderWidth="1px" Cursor="Hand" Font-Names="Tahoma" Font-Size="8pt" 
                                         Height="100%" Width="100%"><Padding Bottom="0px" Left="10px" Right="10px" Top="2px" /></Normal><Over BackColor="WhiteSmoke" BaseStyle="Normal"></Over><Active BackColor="White" BackColor2="WhiteSmoke" BaseStyle="Normal" 
                                         BorderColor="Navy" BorderStyle="Solid" BorderWidth="1px" 
                                         GradientType="Vertical"></Active></TabItemStyle>
                                <ContainerStyle BackColor="WhiteSmoke" BorderColor="Navy" BorderStyle="Solid" 
                                    BorderWidth="1px" Height="550px" Width="100%">
                                    <Padding Bottom="5px" Left="5px" Right="5px" Top="5px" />
                                </ContainerStyle>
                                <RoundCornerSettings FillerBorderColor="255, 199, 60" 
                                    TopBorderColor="230, 139, 44" />
                                <DisabledStyle ForeColor="Gray">
                                </DisabledStyle>
                                </ISWebDesktop:WebTab>
                         </asp:View> 
                    </asp:MultiView>  
                    
           
              
                    
                          

    
    </form>
</body>
</html>
