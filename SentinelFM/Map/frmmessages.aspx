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

</head>
<body onload="ScrollColor()">
    <form method="post" runat="server">
        <table id="Table2" style="border-right: gray 2px outset; border-top: gray 2px outset;
            z-index: 101; left: 8px; border-left: gray 2px outset; border-bottom: gray 2px outset;
            position: absolute; top: 4px; height: 575px; background-color: #fffff0" cellspacing="0"
            cellpadding="0" width="1000" border="0">
            <tr>
                <td style="height: 2px" align="center">
                    <table id="Table5" cellspacing="0" cellpadding="0" width="844" border="0">
                        <tr>
                            <td class="formtext" style="height: 19px" height="19">
                            </td>
                            <td style="width: 255px; height: 19px" height="19">
                            </td>
                            <td style="width: 79px; height: 19px" height="19">
                            </td>
                            <td class="formtext" style="width: 64px; height: 19px" height="19">
                            </td>
                            <td style="height: 19px" height="19">
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext" style="font-weight: bold" height="30">
                                &nbsp;
                                <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1">From:</asp:Label></td>
                            <td style="width: 255px" height="30">
                                <asp:TextBox ID="txtFrom" runat="server" CssClass="RegularText" Width="150px" ReadOnly="True"
                                    meta:resourcekey="txtFromResource1"></asp:TextBox><a onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtFrom','cal','width=220,height=200,left=270,top=180')"
                                        href="javascript:;"><img src="../images/SmallCalendar.gif" border="0"></a>&nbsp;
                                <asp:DropDownList ID="cboHoursFrom" runat="server" CssClass="RegularText" Width="76px"
                                    meta:resourcekey="cboHoursFromResource1">
                                </asp:DropDownList></td>
                            <td style="width: 79px" height="30">
                                <a onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtFrom','cal','width=220,height=200,left=270,top=180')"
                                    href="javascript:;"></a>
                            </td>
                            <td class="formtext" style="font-weight: bold; width: 64px" height="30">
                                <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1">To:</asp:Label></td>
                            <td height="30">
                                <asp:TextBox ID="txtTo" runat="server" CssClass="RegularText" Width="150px" ReadOnly="True"
                                    meta:resourcekey="txtToResource1"></asp:TextBox><a onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtTo','cal','width=220,height=200,left=570,top=180')"
                                        href="javascript:;"><img src="../images/SmallCalendar.gif" border="0"></a>&nbsp;
                                <asp:DropDownList ID="cboHoursTo" runat="server" CssClass="RegularText" Width="75px"
                                    Height="14px" meta:resourcekey="cboHoursToResource1">
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td class="formtext" style="font-weight: bold" height="30">
                                &nbsp;
                                <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1">Fleet:</asp:Label>
                                <asp:RangeValidator ID="valFleet" runat="server" MaximumValue="999999999999999" MinimumValue="1"
                                    ErrorMessage="Please select a Fleet" ControlToValidate="cboFleet" meta:resourcekey="valFleetResource1">*</asp:RangeValidator></td>
                            <td style="width: 255px" height="30">
                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="250px"
                                    DataValueField="FleetId" DataTextField="FleetName" AutoPostBack="True" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                    meta:resourcekey="cboFleetResource1">
                                </asp:DropDownList></td>
                            <td style="width: 79px" height="30">
                            </td>
                            <td style="width: 64px" height="30">
                                <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" Width="30px" Font-Bold="True"
                                    Visible="False" meta:resourcekey="lblVehicleNameResource1"> Vehicle:</asp:Label>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="cboVehicle"
                                    ErrorMessage="Please select a Vehicle" meta:resourcekey="RequiredFieldValidator1Resource1">*</asp:RequiredFieldValidator></td>
                            <td height="30">
                                <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" Width="250px"
                                    DataValueField="BoxId" DataTextField="Description" AutoPostBack="True" Visible="False"
                                    OnSelectedIndexChanged="cboVehicle_SelectedIndexChanged" meta:resourcekey="cboVehicleResource1">
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td class="formtext" style="font-weight: bold; height: 12px" height="12">
                                &nbsp;
                                <asp:Label ID="lblFolderListTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFolderListTitleResource1">Folder List:</asp:Label></td>
                            <td style="width: 255px; height: 12px" height="12">
                                <asp:DropDownList ID="cboDirection" runat="server" CssClass="RegularText" Width="250px"
                                    meta:resourcekey="cboDirectionResource1">
                                    <asp:ListItem Value="0" meta:resourcekey="ListItemResource1">All</asp:ListItem>
                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Incoming</asp:ListItem>
                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource3">Outgoing</asp:ListItem>
                                </asp:DropDownList></td>
                            <td style="width: 79px; height: 12px" height="12">
                            </td>
                            <td style="width: 64px; height: 12px" height="12">
                            </td>
                            <td style="height: 12px" height="12">
                                <asp:Button ID="cmdNewMessage" runat="server" CssClass="combutton" Width="125px"
                                    CausesValidation="False" Text="New Text Message" CommandName="25" OnClick="cmdNewMessage_Click"
                                    meta:resourcekey="cmdNewMessageResource1"></asp:Button>
                                <asp:Button ID="cmdShowMessages" runat="server" CssClass="combutton" Width="128px"
                                    Text="View Text Messages" OnClick="cmdShowMessages_Click" meta:resourcekey="cmdShowMessagesResource1">
                                </asp:Button></td>
                        </tr>
                        <tr>
                            <td class="formtext" style="height: 76px" height="76">
                            </td>
                            <td style="width: 255px; height: 76px" height="76">
                            </td>
                            <td style="width: 79px; height: 76px" height="76">
                            </td>
                            <td style="width: 64px; height: 76px" height="76">
                            </td>
                            <td style="height: 76px" height="76">
                                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1">
                                </asp:ValidationSummary>
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
                            <td style="width: 952px" align="center">
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
                                                            <asp:Label ID="lblNoMatchesMessage" runat="server" meta:resourcekey="lblNoMatchesMessageResource1">No messages matching the selected criteria.</asp:Label>
                                                        </font>
                                                            <br>
                                                        </b></font>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 952px" align="center" height="5">
                            </td>
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
                            <asp:HyperLinkColumn DataNavigateUrlField="MsgKey" DataNavigateUrlFormatString="javascript:var w =MessageInfoWindow('{0}')"
                                DataTextField="MsgDateTime" HeaderText='<%$ Resources:dgMessages_DateTime %>'
                                meta:resourcekey="HyperLinkColumnResource1">
                                <ItemStyle Wrap="False" />
                            </asp:HyperLinkColumn>
                            <asp:BoundColumn DataField="From" HeaderText='<%$ Resources:dgMessages_From %>'>
                                <ItemStyle Wrap="False" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="To" HeaderText='<%$ Resources:dgMessages_To %>'>
                                <ItemStyle Wrap="False" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="MsgDirection" HeaderText='<%$ Resources:dgMessages_Direction %>'>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="MsgBody" HeaderText='<%$ Resources:dgMessages_Body %>'></asp:BoundColumn>
                            <asp:BoundColumn DataField="MsgResponse" HeaderText='<%$ Resources:dgMessages_Response %>'>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="Acknowledged" HeaderText='<%$ Resources:dgMessages_Ack %>'>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="UserName" HeaderText='<%$ Resources:dgMessages_UserName %>'>
                                <ItemStyle Wrap="False" />
                            </asp:BoundColumn>
                        </Columns>
                        <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right"
                            Mode="NumericPages" />
                    </asp:DataGrid></td>
            </tr>
        </table>
    </form>
</body>
</html>
