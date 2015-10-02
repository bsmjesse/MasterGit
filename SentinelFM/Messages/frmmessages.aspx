<%@ Page Language="c#" Inherits="SentinelFM.frmMessages" CodeFile="frmMessages.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
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
				
		//-->
    </script>

    <%if (LoadVehiclesBasedOn == "hierarchy")
  {%>
  
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>     

    <script language="javascript">
		<!--
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
			var myname='OrganizationHierarchy';
			var w=740;
			var h=440;
			var winl = (screen.width - w) / 2; 
			var wint = (screen.height - h) / 2; 
			winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
			win = window.open(mypage, myname, winprops) 
			if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }  
            return false;
        }

        function OrganizationHierarchyNodeSelected(nodecode, fleetId)
        {            
            var myVal = document.getElementById('<%=valVehicle.ClientID %>');
            ValidatorEnable(myVal, false); 

            
            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
            $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
            $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
        }
            
			//-->
    </script>
<%} %>

</head>
<body onload="ScrollColor()" >
    <form id="frmMessagesForm" method="post" runat="server">
    <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />
         <div style="border-right: gray 2px outset; border-top: gray 2px outset;
            z-index: 101; left: 12px; border-left: gray 2px outset; border-bottom: gray 2px outset;
            position: absolute; top: 4px; height: 97%; width: 100%;  background-color: #fffff0">
            
        <table id="Table2"  cellpadding="0" width="100%" border="0">
            <tr>
                <td style="height: 2px" align="center">
                    <table id="Table5" cellspacing="0" cellpadding="0" border="0" style="width: 853px">
                        <tr>
                           <td class="formtext" height="19" style="height: 19px;" align="center" colspan="6">
                               <asp:RadioButtonList ID="optMessageType" runat="server" CssClass="formtext" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="optMessageType_SelectedIndexChanged" meta:resourcekey="optMessageTypeResource1" BorderColor="Black" BorderStyle="Inset" BorderWidth="1px">
                                  <asp:ListItem Selected="True" Value="0" meta:resourcekey="ListItemResource4" Text="Text Messages"></asp:ListItem>
                                  <asp:ListItem Value="3" meta:resourcekey="ListItemResource11">Alarms</asp:ListItem>
                                  <asp:ListItem Value="1" meta:resourcekey="ListItemResource5" Text="Scheduled Tasks"></asp:ListItem>
                                  <asp:ListItem meta:resourcekey="ListItemResource6" Text="Form Messsages" Value="2"></asp:ListItem>
                               </asp:RadioButtonList></td>
                            
                           
                            
                        </tr>
                        <tr>
                           <td class="formtext" style="font-weight: bold;">
                           </td>
                            <td class="formtext" style="font-weight: bold;">
                                
                                <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1"
                                    Text="From:"></asp:Label></td>
                            <td >
                                <asp:TextBox ID="txtFrom" runat="server" CssClass="RegularText" Width="150px" ReadOnly="True"
                                    meta:resourcekey="txtFromResource1"></asp:TextBox><a onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtFrom','cal','width=220,height=200,left=270,top=180')"
                                        href="javascript:;"><img src="../images/SmallCalendar.gif" border="0"></a>&nbsp;
                                <asp:DropDownList ID="cboHoursFrom" runat="server" CssClass="RegularText" Width="76px"
                                    meta:resourcekey="cboHoursFromResource1">
                                </asp:DropDownList></td>
                            <td style="height: 30px;">
                                <a onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtFrom','cal','width=220,height=200,left=270,top=180')"
                                    href="javascript:;"></a>
                            </td>
                            <td class="formtext" style="font-weight: bold;">
                                <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1"
                                    Text="To:"></asp:Label></td>
                            <td >
                                <asp:TextBox ID="txtTo" runat="server" CssClass="RegularText" Width="150px" ReadOnly="True"
                                    meta:resourcekey="txtToResource1"></asp:TextBox><a onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtTo','cal','width=220,height=200,left=570,top=180')"
                                        href="javascript:;"><img src="../images/SmallCalendar.gif" border="0"></a>&nbsp;
                                <asp:DropDownList ID="cboHoursTo" runat="server" CssClass="RegularText" Width="75px"
                                    Height="14px" meta:resourcekey="cboHoursToResource1">
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                           <td class="formtext" style="font-weight: bold;">
                           </td>
                            <td class="formtext" style="font-weight: bold;">
                                
                                <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1"
                                    Text="Fleet:"></asp:Label>
                                <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" Text=" Hierarchy Node:" Visible="false"  /></td>
                                <asp:RangeValidator ID="valFleet" runat="server" MaximumValue="999999999999999" MinimumValue="1"
                                    ErrorMessage="Please select a Fleet" ControlToValidate="cboFleet" meta:resourcekey="valFleetResource1"
                                    Text="*"></asp:RangeValidator></td>
                            <td height="30">
                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="250px"
                                    DataValueField="FleetId" DataTextField="FleetName" AutoPostBack="True" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                    meta:resourcekey="cboFleetResource1">
                                </asp:DropDownList>
                                <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" Text="" CssClass="combutton" Visible="false" OnClientClick="return onOrganizationHierarchyNodeCodeClick();" />
                            </td>
                            <td height="30">
                            </td>
                            <td valign=middle  >
                            
                                <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" Width="30px" Font-Bold="True"
                                    Visible="False" meta:resourcekey="lblVehicleNameResource1" Text=" Vehicle:"></asp:Label>
                                <asp:RequiredFieldValidator ID="valVehicle" runat="server" ControlToValidate="cboVehicle"
                                    ErrorMessage="Please select a Vehicle" meta:resourcekey="RequiredFieldValidator1Resource1"
                                    Text="*"></asp:RequiredFieldValidator></td>
                            <td height="30">
                                <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" Width="250px"
                                    DataValueField="BoxId" DataTextField="Description" AutoPostBack="True" Visible="False"
                                    OnSelectedIndexChanged="cboVehicle_SelectedIndexChanged" meta:resourcekey="cboVehicleResource1">
                                </asp:DropDownList></td>
                        </tr>
                       <tr>
                          <td class="formtext"  style="font-weight: bold;" valign="top">
                          </td>
                          <td class="formtext"  style="font-weight: bold;" valign="top">
                             
                                <asp:Label ID="lblFolderListTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFolderListTitleResource1"
                                    Text="Folder List:"></asp:Label>
                             <asp:Label ID="lblMDTForm" runat="server" Text="Form:" Visible="False" meta:resourcekey="lblMDTFormResource1"></asp:Label>
                             <asp:Label ID="lblScheduledMessageFilter" runat="server" Text="Scheduled Messages:" Visible="False" meta:resourcekey="lblScheduledMessageFilterResource1" ></asp:Label>
                             </td>
                          <td  valign="top">
                                <asp:DropDownList ID="cboDirection" runat="server" CssClass="RegularText" Width="250px"
                                    meta:resourcekey="cboDirectionResource1">
                                    <asp:ListItem Value="0" meta:resourcekey="ListItemResource1" Text="All"></asp:ListItem>
                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="Incoming"></asp:ListItem>
                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="Outgoing"></asp:ListItem>
                                </asp:DropDownList>
                             <asp:DropDownList ID="cboForms" runat="server" CssClass="formtext" Visible="False"
                                Width="250px" meta:resourcekey="cboFormsResource1">
                                <asp:ListItem Value="1" meta:resourcekey="ListItemResource7" Text="Product Loading"></asp:ListItem>
                                <asp:ListItem Value="2" meta:resourcekey="ListItemResource8" Text="Destination"></asp:ListItem>
                             </asp:DropDownList>
                             <asp:DropDownList ID="cboScheduledMessageFilter" runat="server" CssClass="formtext" Visible="False"
                                Width="250px" meta:resourcekey="cboFormsResource1">
                                <asp:ListItem  Text="All" Value="1" meta:resourcekey="ListItemResource9"></asp:ListItem>
                                <asp:ListItem  Text="Sent" Value="2" meta:resourcekey="ListItemResource10"></asp:ListItem>
                             </asp:DropDownList></td>
                          <td height="30">
                          </td>
                          
                          <td height="30" colspan=2 >
                          </td>
                       </tr>
                        <tr>
                           <td class="formtext" height="12" style="font-weight: bold; height: 12px"
                              valign="top">
                           </td>
                            <td valign="top" class="formtext" style="font-weight: bold; height: 12px;"
                                height="12" align="center" colspan="5">
                                <table cellpadding="2" cellspacing="2" style="width: 328px" id="tblAlarms" runat="server">
                                   <tr>
                                      <td align="center" colspan="1">
                                         <asp:Button ID="cmdViewAlarms" runat="server" CommandName="25"
                                            CssClass="combutton"  
                                            Text="View Alarms" Width="136px" OnClick="cmdViewAlarms_Click" meta:resourcekey="cmdViewAlarmsResource1"  /></td>
                                      <td align="center" colspan="1">
                                         <asp:Button ID="cmdAccept" runat="server" CommandName="25"
                                            CssClass="combutton"  
                                            Text="Accept Alarms" Width="136px" OnClick="cmdAccept_Click" meta:resourcekey="cmdAcceptResource1"  /></td>
                                      <td align="center" colspan="1">
                                         <asp:Button ID="cmdCloseAlarms" runat="server" CommandName="25"
                                            CssClass="combutton"  
                                            Text="Close Alarms" Width="136px" meta:resourcekey="cmdCloseAlarmsResource1" OnClick="cmdCloseAlarms_Click"  /></td>
                                   </tr>
                                </table>
                               
                               <table cellpadding="2" cellspacing="2" style="width: 328px" id="tblMDTFormMessages" runat="server">
                                   <tr>
                                      <td align="center" colspan="1" >
                                         <asp:Button ID="cmdFormMessages" runat="server" CommandName="25"
                                            CssClass="combutton"  OnClick="cmdFormMessages_Click"
                                            Text="View Form Messages" Width="136px" meta:resourcekey="cmdFormMessagesResource1" /></td>
                                   </tr>
                                </table>
                                <table cellpadding="2" cellspacing="2" style="width: 328px" id="tblMsgButtons" runat="server">
                                    <tr>
                                        <td align="left" >
                                            <asp:Button ID="cmdNewMessage" runat="server" CssClass="combutton" CausesValidation="False"
                                                Text="New Text Message" CommandName="25" OnClick="cmdNewMessage_Click" Width="136px"
                                                meta:resourcekey="cmdNewMessageResource1"></asp:Button></td>
                                        <td  align="center" style="height: 23px">
                                            &nbsp;<asp:Button ID="cmdShowMessages" runat="server" CssClass="combutton" OnClick="cmdShowMessages_Click"
                                                Text="View Text Messages" Width="136px" meta:resourcekey="cmdShowMessagesResource1" /></td>
                                       <td align="center" style="height: 23px">
                                           <asp:Button ID="cmdMarkAsRead" runat="server" CssClass="combutton" OnClick="cmdMarkAsRead_Click"
                                                Text="Mark as read" Width="136px" meta:resourcekey="cmdMarkAsReadResource1"  /></td>
                                    </tr>
                                    <tr>
                                     
                                        <td align="left"  style="height: 23px; width: 165px;">
                                           </td>
                                                
                                                <td align="center"  style="height: 23px">
                                            <asp:CheckBox ID="chkAuto" runat="server" Text="Auto Time Refresh" CssClass="formtext"
                                                meta:resourcekey="chkAutoResource1" /></td>
                                       <td align="center" style="height: 23px">
                                       </td>
                                                
                                    </tr>
                                </table><table cellpadding="2" cellspacing="2" style="width: 328px" id="tblSchButtons" runat="server">
                                   <tr>
                                      <td align="center" colspan="1">
                                         <asp:Button ID="cmdScheduledTasks" runat="server" CommandName="25"
                                            CssClass="combutton"  OnClick="cmdScheduledTasks_Click"
                                            Text="View Scheduled Tasks" Width="145px" meta:resourcekey="cmdScheduledTasksResource1" /></td>
                                    
                                   </tr>
                                </table>
                                </td>
                        </tr>
                        <tr>
                           <td class="formtext" height="76" >
                           </td>
                            <td class="formtext" style="height: 76px;"  colspan="5">
                                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1">
                                </asp:ValidationSummary>
                                &nbsp;
                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="138px" Height="19px"
                                    Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    &nbsp;<table id="Table3" style="width: 945px" cellspacing="0" cellpadding="0" width="945"
                        border="0">
                        <tr>
                            <td style="width: 935px" align="center" valign=top >
                                <table id="tblNoData" style="border-right: gray 1px outset; border-top: gray 1px outset;
                                    border-left: gray 1px outset; border-bottom: gray 1px outset" cellspacing="0"
                                    width="400" border="0" runat="server">
                                    <tr>
                                        <td valign="middle" align="center">
                                            <table id="Table1" style="width: 396px; height: 40px" cellspacing="0" cellpadding="0"
                                                width="396" bgcolor="#ffffff" border="0">
                                                <tr>
                                                    <td valign="middle" align="center">
                                                        <font face="Arial, Verdana" color="gray" size="4"><b class="tableheading"><font
                                                            color="gray" size="4">
                                                            <asp:Label ID="lblNoDataMessageTitle" runat="server" meta:resourcekey="lblNoDataMessageTitleResource1"
                                                                Text="No messages matching the selected criteria."></asp:Label>
                                                        </font>
                                                            <br>
                                                        </b></font>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table><asp:DataGrid ID="dgFormMessages" runat="server" AllowPaging="True"
                        BackColor="White" BorderColor="Black" BorderStyle="Ridge" BorderWidth="1px" CellPadding="3"
                        CellSpacing="1" GridLines="None" OnPageIndexChanged="dgFormMessages_PageIndexChanged"
                        PageSize="12" Width="943px" meta:resourcekey="dgMessagesResource1">
                                   <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                                   <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                                   <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right"
                            Mode="NumericPages" />
                                   <AlternatingItemStyle BackColor="Beige" CssClass="gridtext" />
                                   <ItemStyle BackColor="White" CssClass="gridtext" />
                                   <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                </asp:DataGrid></td>
                        </tr>
                        <tr>
                            <td style="width: 935px; height: 5px;" align="center"><asp:DataGrid ID="dgSched" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                        BackColor="White" BorderColor="Black" BorderStyle="Ridge" BorderWidth="1px" CellPadding="3"
                        CellSpacing="1" GridLines="None" OnPageIndexChanged="dgSched_PageIndexChanged"
                        PageSize="12" Width="943px" meta:resourcekey="dgMessagesResource1">
                               <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                               <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                               <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right"
                            Mode="NumericPages" />
                               <AlternatingItemStyle BackColor="Beige" CssClass="gridtext" />
                               <ItemStyle BackColor="White" CssClass="gridtext" />
                               <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                               <Columns>
                                  <asp:BoundColumn DataField="RequestDateTime" HeaderText='<%$ Resources:dgSched_Date %>'></asp:BoundColumn>
                                  <asp:BoundColumn DataField="Description" HeaderText='<%$ Resources:dgSched_Vehicle %>'></asp:BoundColumn>
                                  <asp:BoundColumn DataField="BoxCmdOutTypeName" HeaderText='<%$ Resources:dgSched_Command %>'></asp:BoundColumn>
                                  <asp:BoundColumn DataField="LastDateTimeSent" HeaderText='<%$ Resources:dgSched_LastTryDate %>'></asp:BoundColumn>
                                  <asp:BoundColumn DataField="MsgOutDateTime" HeaderText='<%$ Resources:dgSched_Sent %>'></asp:BoundColumn>
                               </Columns>
                            </asp:DataGrid></td>
                        </tr>
                    </table>
                    <asp:DataGrid ID="dgMessages" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                        BackColor="White" BorderColor="Black" BorderStyle="Ridge" BorderWidth="1px" CellPadding="3"
                        CellSpacing="1" GridLines="None" OnPageIndexChanged="dgMessages_PageIndexChanged"
                        PageSize="12" Width="943px" meta:resourcekey="dgMessagesResource1">
                        <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                        <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                        <AlternatingItemStyle BackColor="Beige" CssClass="gridtext" />
                        <ItemStyle BackColor="White" CssClass="gridtext" />
                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                        <Columns>
                        
                        <asp:BoundColumn DataField="MsgId" Visible=False >
                            </asp:BoundColumn>
                            
                                   <asp:TemplateColumn>
															<HeaderStyle Width="20px"></HeaderStyle>
															<ItemStyle HorizontalAlign="Center" Width="20px"></ItemStyle>
															<ItemTemplate>
																<asp:CheckBox ID="chkBox" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBox") %>' runat="server" meta:resourcekey="chkBoxResource1" />
															</ItemTemplate>
														</asp:TemplateColumn>
														
                            <asp:HyperLinkColumn DataNavigateUrlField="MsgKey" DataNavigateUrlFormatString="javascript:var w =MessageInfoWindow('{0}')"
                                DataTextField="MsgDateTime" HeaderText='<%$ Resources:dgMessages_MsgDateTime %>'
                                meta:resourcekey="HyperLinkColumnResource1">
                                <ItemStyle Wrap="False" />
                            </asp:HyperLinkColumn>
                         
														
                            <asp:BoundColumn DataField="From" HeaderText='<%$ Resources:dgMessages_From %>'>
                                <ItemStyle Wrap="False" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="To" HeaderText='<%$ Resources:dgMessages_To %>'>
                                <ItemStyle Wrap="False" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="MsgDirection" HeaderText='<%$ Resources:dgMessages_MsgDirection %>'>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="MsgBody" HeaderText='<%$ Resources:dgMessages_MsgBody %>'>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="MsgResponse" HeaderText='<%$ Resources:dgMessages_MsgResponse %>'>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Acknowledged" HeaderText='<%$ Resources:dgMessages_Acknowledged %>'>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="UserName" HeaderText='<%$ Resources:dgMessages_UserName %>'>
                                <ItemStyle Wrap="False" />
                            </asp:BoundColumn>
                        </Columns>
                        <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right"
                            Mode="NumericPages" />
                    </asp:DataGrid></td>
            </tr>
           <tr>
              <td align="center" valign="top"><asp:DataGrid ID="dgAlarms" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                        BackColor="White" BorderColor="Black" BorderStyle="Ridge" BorderWidth="1px" CellPadding="3"
                        CellSpacing="1" GridLines="None" 
                        PageSize="12" Width="943px" OnPageIndexChanged="dgAlarms_PageIndexChanged" meta:resourcekey="dgAlarmsResource1">
                 <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                 <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                 <AlternatingItemStyle BackColor="Beige" CssClass="gridtext" />
                 <ItemStyle BackColor="White" CssClass="gridtext" />
                 <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                 <Columns>
                     <asp:BoundColumn DataField="AlarmId" Visible=False ></asp:BoundColumn>
                       <asp:TemplateColumn>
                          <HeaderStyle Width="20px" />
                          <ItemStyle HorizontalAlign="Center" Width="20px" />
                          <ItemTemplate>
                             <asp:CheckBox ID="chkBox" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBox") %>' runat="server" meta:resourcekey="chkBoxResource2" />
                          </ItemTemplate>
                       </asp:TemplateColumn>
                      
                       <asp:BoundColumn DataField="TimeCreated" HeaderText='<%$ Resources:dgAlarms_Date %>'>
                          <ItemStyle Wrap="False" />
                       </asp:BoundColumn>
                       <asp:BoundColumn DataField="vehicleDescription" HeaderText='<%$ Resources:dgAlarms_vehicleDescription %>'>
                       </asp:BoundColumn>
                       <asp:BoundColumn DataField="AlarmDescription" HeaderText='<%$ Resources:dgAlarms_AlarmDescription %>'>
                          <ItemStyle Wrap="False" />
                       </asp:BoundColumn>
                       <asp:BoundColumn DataField="AlarmLevel" HeaderText='<%$ Resources:dgAlarms_AlarmLevel %>'>
                       </asp:BoundColumn>
                       
                       <asp:BoundColumn DataField="AlarmState" HeaderText='<%$ Resources:dgAlarms_AlarmState %>'>
                       </asp:BoundColumn>
                 </Columns>
                 <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right"
                            Mode="NumericPages" />
              </asp:DataGrid></td>
           </tr>
        </table>
        </div>
        <%if (LoadVehiclesBasedOn == "hierarchy")
          {%>        
            <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" style="display:none;" AutoPostBack="True"
            OnClick="hidOrganizationHierarchyPostBack_Click" />
        <%} %>
    </form>
</body>
</html>
