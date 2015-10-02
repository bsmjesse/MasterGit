<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmSensorsInfo_Reefer.aspx.cs" Inherits="SentinelFM.Map_frmSensorsInfo_Reefer" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head id="Head1" runat="server">
    <title>Sensors-Command-Vehicle Info</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <script type="text/javascript" src="../Scripts/Telerik_AddIn.js" ></script>

    
    <script type="text/javascript">
        var isShift = false;
        function isNumeric(keyCode) {
            if (keyCode == 16) 
                isShift = true; 
            
            var isnumeric = ((keyCode >= 48 && keyCode <= 57 || keyCode == 8 || keyCode == 173 || keyCode == 189 || keyCode == 190 || keyCode == 110 ||
                  (keyCode >= 96 && keyCode <= 105)) && isShift == false);
            
            if (isnumeric == true)
                document.getElementById("lblErrorMessage").innerHTML = ''; 
            else
                document.getElementById("lblErrorMessage").innerHTML = '<%= ResTempratureAllowedInNumbers%>'; 
               

            return isnumeric;
        }

        function ConvertTemprature(degree, keyCode) {

            if (degree == "C") {
                if (document.getElementById("txtbxcelsius").value != '') {                   
                        F = document.getElementById("txtbxcelsius").value * 9 / 5 + 32;
                        document.getElementById("txtbxfahrenheit").value = F.toFixed(2);
                }
                else {
                    document.getElementById("txtbxfahrenheit").value = '';
                }
                
            } else {
                if (document.getElementById("txtbxfahrenheit").value != '') {
                        C = (document.getElementById("txtbxfahrenheit").value - 32) * 5 / 9;                    
                        document.getElementById("txtbxcelsius").value = C.toFixed(2);
                }
                else {
                    document.getElementById("txtbxcelsius").value = '';
                   
                }
            }

            if (keyCode == 16)
                isShift = true;

            return ((keyCode >= 48 && keyCode <= 57 || keyCode == 8 ||
                  (keyCode >= 96 && keyCode <= 105)) && isShift == false);

        }

        </script>
       
</head>
<body onload="self.focus()">
    <form id="frmSensorsInfoForm" method="post" runat="server">
        <table id="tblCommands" style="z-index: 101; left: 15px; width: 390px; position: absolute;
            top: 11px; height: 400px" cellspacing="0" cellpadding="0" width="390" border="0">
            <tr>
                <td>
                    <table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdSensorCommands" runat="server" CssClass="selectedbutton" Text="Commands"
                                    CausesValidation="False" meta:resourcekey="cmdSensorCommandsResource1"></asp:Button></td>
                            <td>
                                <asp:Button ID="cmdVehicleInfo" runat="server" CssClass="confbutton" Text="Vehicle Info"
                                    CausesValidation="False" OnClick="cmdVehicleInfo_Click" meta:resourcekey="cmdVehicleInfoResource1">
                                </asp:Button></td>
                                
                                <td>
                                <asp:Button ID="cmdSettings" runat="server" CssClass="confbutton" Text="Settings"
                                    CausesValidation="False" OnClick="cmdSettings_Click" CommandName="49" meta:resourcekey="cmdSettingsfoResource1" >
                                </asp:Button></td>
                                
                            <td>
                                <asp:Button ID="cmdReefer" runat="server" CssClass="confbutton" Text="Reefer"
                                    CausesValidation="False" meta:resourcekey="cmdReeferResource1" OnClick="cmdReefer_Click" Visible="False">
                                </asp:Button>
                            </td>
                            
                                <td>
                                <asp:Button ID="cmdServices" runat="server" CssClass="confbutton" Text="Services"
                                    CausesValidation="False" onclick="cmdServices_Click" CommandName="52" >
                                </asp:Button>
                            </td>
                            
                            <td><asp:Button ID="cmdUnitInfo" Text="Unit Info" runat="server" 
                                    CausesValidation="False" CssClass="confbutton" CommandName="53" 
                                    onclick="cmdUnitInfo_Click"/></td>
                            
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" style="width: 513px; height: 284px" cellspacing="0" cellpadding="0"
                         align="left" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" style="border-right: gray 2px solid; border-top: gray 2px solid;
                                    border-left: gray 2px solid; width: 490px; border-bottom: gray 2px solid;
                                    height: 265px" border="0">
                                    <tr>
                                        <td align="center" class="configTabBackground">
                                            <table id="Table1" style="border-top-width: thin; border-left-width: thin; border-left-color: gray;
                                                border-bottom-width: thin; border-bottom-color: gray; border-top-color: gray;
                                                border-right-width: thin; border-right-color: gray" cellspacing="0" cellpadding="0"
                                                width="100%" border="0">
                                                <tr height="25">
                                                    <td class="tableheading" style="width: 412px" align="left">
                                                        <asp:Label ID="lblSensorsCommandsTitle" runat="server" meta:resourcekey="lblSensorsCommandsTitleResource1" Text="Sensors/Commands info for vehicle:"></asp:Label>
                                                        <asp:Label ID="lblVehicleName" runat="server" CssClass="Regulartext" meta:resourcekey="lblVehicleNameResource1"></asp:Label>
                                                       <asp:Label
                                                            ID="lblBoxTitle" runat="server" CssClass="formtext" meta:resourcekey="lblBoxTitleResource1" Text="Box:"></asp:Label><asp:Label
                                                                ID="lblBoxId" runat="server" CssClass="Regulartext" meta:resourcekey="lblBoxIdResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="width: 412px; height: 4px" align="left">
                                                        <asp:Label ID="lblLastCommTitle" runat="server" meta:resourcekey="lblLastCommTitleResource1" Text="Last Communicated Time:"></asp:Label>
                                                        <asp:Label ID="lblLastComm" runat="server" CssClass="RegularText" meta:resourcekey="lblLastCommResource1"></asp:Label><asp:Label
                                                            ID="lblVehicleId" runat="server" CssClass="Regulartext" Visible="False" meta:resourcekey="lblVehicleIdResource1"></asp:Label>
                                                        <asp:Label ID="lblSensorMask" runat="server"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="width: 412px; height: 2px" align="left">
                                                        <asp:Label ID="lblNotes" runat="server" CssClass="RegularText" meta:resourcekey="lblNotesResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td align="center" style="width: 490px">
                                                        <table id="tblSensors" style="width: 100%" cellspacing="0" cellpadding="0" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td style="width: 697px" align="center">
                                                                    <div id="divSensors" title="Box Setup:" style="border-top-width: thin; border-left-width: thin;
                                                                        border-left-color: gray; border-bottom-width: thin; border-bottom-color: gray;
                                                                        overflow: auto; width: 25.41pc; border-top-color: gray; height: 208px; border-right-width: thin;
                                                                        border-right-color: gray" align="center">
                                                                        <asp:DataGrid ID="dgSensors" runat="server" Width="380px" AutoGenerateColumns="False"
                                                                            BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" BackColor="White" CellPadding="3"
                                                                            meta:resourcekey="dgSensorsResource1">
                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                            <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                            <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black"
                                                                                BackColor="#DEDFDE"></ItemStyle>
                                                                            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                            <Columns>
                                                                                <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgSensors_Sensor %>'>
                                                                                </asp:BoundColumn>
                                                                                <asp:BoundColumn DataField="SensorAction" HeaderText='<%$ Resources:dgSensors_Status %>'>
                                                                                </asp:BoundColumn>
                                                                            </Columns>
                                                                            <PagerStyle Mode="NumericPages"></PagerStyle>
                                                                        </asp:DataGrid></div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblWait" runat="server" style="width: 256px; height: 113px">
                                                            <tr>
                                                                <td class="RegularText" align="center">
                                                                    <asp:Label ID="lblPleaseWaitMessage" runat="server" meta:resourcekey="lblPleaseWaitMessageResource1" Text="Please wait..."></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" align="center">
                                                                    <asp:Label ID="lblSendingCommandMessage" runat="server" meta:resourcekey="lblSendingCommandMessageResource1" Text="Sending Command..."></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="height: 22px" align="center">
                                                                    <asp:Image ID="imgWait" runat="server" Width="105px" ImageUrl="../images/prgBar.gif"
                                                                        Height="5px" meta:resourcekey="imgWaitResource1"></asp:Image></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" align="center">
                                                                    <p>
                                                                        <u>
                                                                            <asp:Label ID="lblNoteTitle" runat="server" meta:resourcekey="lblNoteTitleResource1" Text="Note"></asp:Label></u>:
                                                                        <asp:Label ID="lblNoteContents1" runat="server" meta:resourcekey="lblNoteContents1Resource1" Text="For satellite communication sending command may take up to 6 minutes."></asp:Label></p>
                                                                    <p>
                                                                        <asp:Label ID="lblNoteContents2" runat="server" meta:resourcekey="lblNoteContents2Resource1" Text="Closing the screen will not affect this operation."></asp:Label></p>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="cmdCancelSend" runat="server" CssClass="combutton" Text="Cancel"
                                                                        Width="97px" OnClick="cmdCancelSend_Click" meta:resourcekey="cmdCancelSendResource1">
                                                                    </asp:Button></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" align="center" style="width: 412px">
                                                    </td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="formtext" style="width: 412px">
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <table id="Table7" height="50" cellspacing="0" cellpadding="0" width="100%" border="0">
                                                            <tr>
                                                                <td class="formtext">
                                                                    <asp:Label ID="lblOutputTitle" runat="server" CssClass="formtext" meta:resourcekey="lblOutputTitleResource1" Text="Output:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboOutput" runat="server" CssClass="RegularText" Width="200px"
                                                                        DataTextField="OutputName" DataValueField="OutputId" AutoPostBack="True" OnSelectedIndexChanged="cboOutput_SelectedIndexChanged"
                                                                        meta:resourcekey="cboOutputResource1">
                                                                    </asp:DropDownList></td>
                                                                <td>
                                                                    <asp:Button ID="cmdSendOutput" runat="server" CssClass="combutton" Text="Send Output"
                                                                        Width="120px" OnClick="cmdSendOutput_Click" meta:resourcekey="cmdSendOutputResource1">
                                                                    </asp:Button></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="height: 16px">
                                                                    <asp:Label ID="lblCommandTitle" runat="server" CssClass="formtext" meta:resourcekey="lblCommandTitleResource1" Text="Command:"></asp:Label></td>
                                                                <td style="height: 16px">
                                                                    <asp:DropDownList ID="cboCommand" runat="server" CssClass="RegularText" Width="200px"
                                                                        DataTextField="BoxCmdOutTypeName" DataValueField="BoxCmdOutTypeId" AutoPostBack="True"
                                                                        OnSelectedIndexChanged="cboCommand_SelectedIndexChanged" meta:resourcekey="cboCommandResource1">
                                                                    </asp:DropDownList></td>
                                                                <td style="height: 16px">
                                                                    <asp:Button ID="cmdSendCommand" runat="server" CssClass="combutton" Text="Send Command"
                                                                        Width="120px" OnClick="cmdSendCommand_Click" meta:resourcekey="cmdSendCommandResource1">
                                                                    </asp:Button></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="width: 412px">
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RadioButtonList ID="optCommMode"
                                                          runat="server" AutoPostBack="True" CssClass="formtext" 
                                                          OnSelectedIndexChanged="optCommMode_SelectedIndexChanged" RepeatDirection="Horizontal"
                                                          Width="100%" meta:resourcekey="optCommModeResource1">
                                                          <asp:ListItem Selected="True" Text="Primary"
                                                             Value="0" meta:resourcekey="ListItemResource128"></asp:ListItem>
                                                           <asp:ListItem Value="1" meta:resourcekey="ListItemResource129">Secondary</asp:ListItem>
                                                          <asp:ListItem  Text="Both"
                                                             Value="2" meta:resourcekey="ListItemResource130"></asp:ListItem>
                                                       </asp:RadioButtonList>
                                                       <table id="tblSchedule" runat="server"
                                                          class="formtext" style="width: 208px">
                                                          <tr>
                                                             <td style="width: 446px; height: 21px" colspan=3 >
                                                                <asp:CheckBox ID="chkScheduleTask" runat="server" Text="Schedule Task:" TextAlign="Left" AutoPostBack="True" Font-Bold="True" OnCheckedChanged="chkScheduleTask_CheckedChanged" meta:resourcekey="chkScheduleTaskResource1" /></td>
                                                             
                                                          </tr>
                                                          <tr>
                                                             <td style="width: 446px; height: 21px">
                                                                <asp:Label ID="lblKeepTry" runat="server" meta:resourcekey="lblKeepTryResource1"
                                                                   Text="Keep trying for:"></asp:Label></td>
                                                             <td style="width: 22px; height: 21px">
                                                             </td>
                                                             <td style="width: 224px; height: 21px">
                                                                <asp:DropDownList ID="cboSchPeriod" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                                   meta:resourcekey="cboSchPeriodResource1" OnSelectedIndexChanged="cboSchPeriod_SelectedIndexChanged"
                                                                   Width="74px" Enabled="False">
                                                                   <asp:ListItem meta:resourcekey="ListItemResource3" Text="No Retry" Value="0"></asp:ListItem>
                                                                   <asp:ListItem meta:resourcekey="ListItemResource4" Text="15 Min" Value="900"></asp:ListItem>
                                                                   <asp:ListItem meta:resourcekey="ListItemResource5" Text="30 Min" Value="1800"></asp:ListItem>
                                                                   <asp:ListItem meta:resourcekey="ListItemResource6" Text="1 Hour" Value="3600"></asp:ListItem>
                                                                   <asp:ListItem meta:resourcekey="ListItemResource7" Text="2 Hours" Value="7200"></asp:ListItem>
                                                                   <asp:ListItem Text="3 Hours" Value="10800" meta:resourcekey="ListItemResource131"></asp:ListItem>
                                                                   <asp:ListItem  Text="4 Hours" Value="14400" meta:resourcekey="ListItemResource132"></asp:ListItem>
                                                                   <asp:ListItem  Text="5 Hours" Value="18000" meta:resourcekey="ListItemResource133"></asp:ListItem>
                                                                   <asp:ListItem  Text="6 Hours" Value="21600" meta:resourcekey="ListItemResource134"></asp:ListItem>
                                                                   <asp:ListItem  Text="7 Hours" Value="25200" meta:resourcekey="ListItemResource135"></asp:ListItem>
                                                                   <asp:ListItem  Text="8 Hours" Value="28800" meta:resourcekey="ListItemResource136"></asp:ListItem>
                                                                   <asp:ListItem  Text="12 Hours" Value="43200" meta:resourcekey="ListItemResource137"></asp:ListItem>
                                                                   <asp:ListItem  Text="1 Day" Value="86400" meta:resourcekey="ListItemResource138"></asp:ListItem>
                                                                   <asp:ListItem  Text="3 Days" Value="259200" meta:resourcekey="ListItemResource139"></asp:ListItem>
                                                                   <asp:ListItem  Text="1 Week" Value="604800" meta:resourcekey="ListItemResource140"></asp:ListItem>
                                                                </asp:DropDownList></td>
                                                          </tr>
                                                          <tr>
                                                             <td style="width: 446px; height: 21px">
                                                                <asp:Label ID="lblRetryEveryTitle" runat="server" meta:resourcekey="lblRetryEveryTitleResource1"
                                                                   Text="Retry every:"></asp:Label></td>
                                                             <td style="width: 22px; height: 21px">
                                                             </td>
                                                             <td style="width: 224px; height: 21px">
                                                                <asp:DropDownList ID="cboSchInterval" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                                   meta:resourcekey="cboSchIntervalResource1" OnSelectedIndexChanged="cboSchInterval_SelectedIndexChanged"
                                                                   Width="74px" Enabled="False">
                                                                   <asp:ListItem meta:resourcekey="ListItemResource8" Text="No Retry" Value="0"></asp:ListItem>
                                                                   <asp:ListItem meta:resourcekey="ListItemResource9" Text="1 min" Value="60"></asp:ListItem>
                                                                   <asp:ListItem meta:resourcekey="ListItemResource10" Text="5 min" Value="300"></asp:ListItem>
                                                                   <asp:ListItem meta:resourcekey="ListItemResource11" Text="10 min" Value="600"></asp:ListItem>
                                                                   <asp:ListItem Text="15 min" Value="900" meta:resourcekey="ListItemResource141"></asp:ListItem>
                                                                   <asp:ListItem Text="30 min" Value="1800" meta:resourcekey="ListItemResource142"></asp:ListItem>
                                                                   <asp:ListItem Text="1 hour" Value="3600" meta:resourcekey="ListItemResource143"></asp:ListItem>
                                                                </asp:DropDownList></td>
                                                          </tr>
                                                          <tr>
                                                             
                                                             <td style="width: 224px; height: 21px" colspan=3>
                                                                <asp:CheckBox ID="chkSendToFleet" runat="server" Text="Send to selected Fleet: " TextAlign="Left" meta:resourcekey="chkSendToFleetResource1" />
                                                                <asp:CheckBox ID="chkSendToVehicles" runat="server" Text="Send to selected Vehicles: " TextAlign="Left" meta:resourcekey="chkSendToVehiclesResource1" /></td>
                                                          </tr>
                                                       </table>
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="width: 412px" align="center" height="30">
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <asp:Label ID="lblCommandStatus" runat="server" CssClass="regulartext" meta:resourcekey="lblCommandStatusResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="width: 412px" align="center">
                                                    
                                                    
                                                     <table id="tblTrailerWakeSleep" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                             border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" colspan="1">
                                                                    <asp:Label ID="lblTrailerWakeTime" runat="server" CssClass="formtext" Text="Trailer Wake Up Time:"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtTrailerWakeUpTime" runat="server" CssClass="formtext" Width="74px"
                                                                        ></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 152px">
                                                                    <asp:Label ID="lblTrailerSleepTime" runat="server" CssClass="formtext" Text="Trailer Sleep Time:"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtTrailerSleepTime" runat="server" CssClass="formtext" Width="74px" 
                                                                        ></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                        
                                                        
                                                        <table id="tblSecCode" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" colspan="1">
                                                                    <asp:Label ID="lblGlobalUnarmCodeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblGlobalUnarmCodeTitleResource1" Text="Global Unarm Code:"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtGlobalUnarmCode" runat="server" CssClass="formtext" Width="74px"
                                                                        TextMode="Password" meta:resourcekey="txtGlobalUnarmCodeResource1"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 152px">
                                                                    <asp:Label ID="lblTARCode" runat="server" CssClass="formtext" meta:resourcekey="lblTARCodeResource1" Text="TAR Code:"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtTARCode" runat="server" CssClass="formtext" Width="74px" TextMode="Password"
                                                                        meta:resourcekey="txtTARCodeResource1"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblProxyCards" style="width: 244px" cellspacing="0" cellpadding="0" width="244"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 44px">
                                                                </td>
                                                                <td class="formtext" style="width: 69px" colspan="1">
                                                                    <asp:Label ID="lblFacilityCodeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFacilityCodeTitleResource1" Text="Facility Code"></asp:Label></td>
                                                                <td class="formtext">
                                                                    <asp:Label ID="lblIDNumberTitle" runat="server" CssClass="formtext" meta:resourcekey="lblIDNumberTitleResource1" Text="ID Number"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 44px;">
                                                                    <asp:Label ID="lblCard1" runat="server" CssClass="formtext" meta:resourcekey="lblCard1Resource1" Text="Card 1:"></asp:Label></td>
                                                                <td class="formtext" style="width: 69px; ">
                                                                    <asp:TextBox ID="txtFC1" runat="server" CssClass="formtext" Width="30px" MaxLength="3"
                                                                        meta:resourcekey="txtFC1Resource1" Text="0"></asp:TextBox></td>
                                                                <td style="height: 22px">
                                                                    <asp:TextBox ID="txtIDN1" runat="server" CssClass="formtext" Width="40px" MaxLength="5"
                                                                        meta:resourcekey="txtIDN1Resource1" Text="0"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 44px">
                                                                    <asp:Label ID="lblCard2" runat="server" CssClass="formtext" meta:resourcekey="lblCard2Resource1" Text="Card 2:"></asp:Label></td>
                                                                <td class="formtext" style="width: 69px">
                                                                    <asp:TextBox ID="txtFC2" runat="server" CssClass="formtext" Width="30px" MaxLength="3"
                                                                        meta:resourcekey="txtFC2Resource1" Text="0"></asp:TextBox></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtIDN2" runat="server" CssClass="formtext" Width="40px" MaxLength="5"
                                                                        meta:resourcekey="txtIDN2Resource1" Text="0"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 44px">
                                                                    <asp:Label ID="lblCard3" runat="server" CssClass="formtext" meta:resourcekey="lblCard3Resource1" Text="Card 3:"></asp:Label></td>
                                                                <td class="formtext" style="width: 69px">
                                                                    <asp:TextBox ID="txtFC3" runat="server" CssClass="formtext" Width="30px" MaxLength="5"
                                                                        meta:resourcekey="txtFC3Resource1" Text="0"></asp:TextBox></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtIDN3" runat="server" CssClass="formtext" Width="40px" MaxLength="5"
                                                                        meta:resourcekey="txtIDN3Resource1" Text="0"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 44px">
                                                                    <asp:Label ID="lblCard4" runat="server" CssClass="formtext" meta:resourcekey="lblCard4Resource1" Text="Card 4:"></asp:Label></td>
                                                                <td class="formtext" style="width: 69px">
                                                                    <asp:TextBox ID="txtFC4" runat="server" CssClass="formtext" Width="30px" MaxLength="3"
                                                                        meta:resourcekey="txtFC4Resource1" Text="0"></asp:TextBox></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtIDN4" runat="server" CssClass="formtext" Width="40px" MaxLength="5"
                                                                        meta:resourcekey="txtIDN4Resource1" Text="0"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 44px">
                                                                    <asp:Label ID="lblCard5" runat="server" CssClass="formtext" meta:resourcekey="lblCard5Resource1" Text="Card 5:"></asp:Label></td>
                                                                <td class="formtext" style="width: 69px">
                                                                    <asp:TextBox ID="txtFC5" runat="server" CssClass="formtext" Width="30px" MaxLength="3"
                                                                        meta:resourcekey="txtFC5Resource1" Text="0"></asp:TextBox></td>
                                                                <td id="TD1" runat="server">
                                                                    <asp:TextBox ID="txtIDN5" runat="server" CssClass="formtext" Width="40px" MaxLength="5"
                                                                        meta:resourcekey="txtIDN5Resource1" Text="0"></asp:TextBox></td>
                                                            </tr>
                                                        </table><table id="tblFuelConfiguration" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 207px; height: 18px;" colspan="1">
                                                                    <asp:Label ID="lblFuelType" runat="server" Text="Fuel Type:"></asp:Label></td>
                                                                <td style="height: 18px">
                                                                    <asp:DropDownList ID="cboFuelType" runat="server" CssClass="RegularText" Width="137px" OnSelectedIndexChanged="cboFuelType_SelectedIndexChanged" AutoPostBack="True">
                                                                        <asp:ListItem Value="0">Regular</asp:ListItem>
                                                                        <asp:ListItem Value="1">Mid</asp:ListItem>
                                                                        <asp:ListItem Value="2">Premium</asp:ListItem>
                                                                        <asp:ListItem Value="3">Natural Gas</asp:ListItem>
                                                                        <asp:ListItem Value="4">Diesel</asp:ListItem>
                                                                        
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" align="left" colspan="2">
                                                                    <table id="tblFuel" runat="server" cellspacing="0" cellpadding="0" border=0  class="formtext" width="100%">
                                                                        <tr>
                                                                            <td style="width: 139px">
                                                                                <asp:Label ID="lblDisplacement" runat="server" Text="Engine Displacement (ml):" Width="160px"></asp:Label></td>
                                                                            <td style="width: 100px">
                                                                                <asp:TextBox ID="txtDisplacement" runat="server" CssClass="formtext">3000</asp:TextBox></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 139px">
                                                                                <asp:Label ID="lblVolumeEfficency" runat="server" Text="Volume Efficency (0-100%):"
                                                                                    Width="122%"></asp:Label></td>
                                                                            <td style="width: 100px">
                                                                                <asp:TextBox ID="txtVolumeEfficency" runat="server" CssClass="formtext">80</asp:TextBox></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td style="width: 139px">
                                                                                <asp:Label ID="lblAirFuelRatio" runat="server" Text="Air/Fuel Ratio x 10:"></asp:Label></td>
                                                                            <td style="width: 100px">
                                                                                <asp:TextBox ID="txtAirFuelRatio" runat="server" CssClass="formtext">140</asp:TextBox></td>
                                                                        </tr>
                                                                    </table>
                                                                    <table id="tblDiesel" runat="server" cellspacing="0" cellpadding="0" border=0 class="formtext" width="100%">
                                                                        <tr>
                                                                            <td style="width: 161px">
                                                                                <asp:Label ID="Label2" runat="server" Text="Denominator:"></asp:Label></td>
                                                                            <td style="width: 100px">
                                                                                <asp:TextBox ID="txtDenominator" runat="server" CssClass="formtext">132500000</asp:TextBox></td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 207px">
                                                                </td>
                                                                <td>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 207px">
                                                                </td>
                                                                <td>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblVCRDelay" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" colspan="1">
                                                                    <asp:Label ID="lblDelayPeriodTitle" runat="server" CssClass="formtext" meta:resourcekey="lblDelayPeriodTitleResource1" Text="Delay Period:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboVCRDelayPeriod" runat="server" CssClass="RegularText" Width="99px"
                                                                        meta:resourcekey="cboVCRDelayPeriodResource1">
                                                                        <asp:ListItem Value="-1" Selected="True" meta:resourcekey="ListItemResource13" Text="Disabled"></asp:ListItem>
                                                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource14" Text="0 min"></asp:ListItem>
                                                                        <asp:ListItem Value="60" meta:resourcekey="ListItemResource15" Text="1 min"></asp:ListItem>
                                                                        <asp:ListItem Value="120" meta:resourcekey="ListItemResource16" Text="2 min"></asp:ListItem>
                                                                        <asp:ListItem Value="180" meta:resourcekey="ListItemResource17" Text="3 min"></asp:ListItem>
                                                                        <asp:ListItem Value="240" meta:resourcekey="ListItemResource18" Text="4 min"></asp:ListItem>
                                                                        <asp:ListItem Value="300" meta:resourcekey="ListItemResource19" Text="5 min"></asp:ListItem>
                                                                        <asp:ListItem Value="600" meta:resourcekey="ListItemResource20" Text="10 min"></asp:ListItem>
                                                                        <asp:ListItem Value="900" meta:resourcekey="ListItemResource21" Text="15 min"></asp:ListItem>
                                                                        <asp:ListItem Value="1200" meta:resourcekey="ListItemResource22" Text="20 min"></asp:ListItem>
                                                                        <asp:ListItem Value="1800" meta:resourcekey="ListItemResource23" Text="30 min"></asp:ListItem>
                                                                        <asp:ListItem Value="3600" meta:resourcekey="ListItemResource24" Text="1 hour"></asp:ListItem>
                                                                        <asp:ListItem Value="7200" meta:resourcekey="ListItemResource25" Text="2 hours"></asp:ListItem>
                                                                        <asp:ListItem Value="10800" meta:resourcekey="ListItemResource26" Text="3 hours"></asp:ListItem>
                                                                        <asp:ListItem Value="14400" meta:resourcekey="ListItemResource27" Text="4 hours"></asp:ListItem>
                                                                        <asp:ListItem Value="18000" meta:resourcekey="ListItemResource28" Text="5 hours"></asp:ListItem>
                                                                        <asp:ListItem Value="21600" meta:resourcekey="ListItemResource29" Text="6 hours"></asp:ListItem>
                                                                        <asp:ListItem Value="25200" meta:resourcekey="ListItemResource30" Text="7 hours"></asp:ListItem>
                                                                        <asp:ListItem Value="28800" meta:resourcekey="ListItemResource31" Text="8 hours"></asp:ListItem>
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblTAR" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px; height: 19px;" colspan="1">
                                                                    <asp:Label ID="lblTARPeriodTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTARPeriodTitleResource1" Text="TAR Mode Period:"></asp:Label></td>
                                                                <td style="height: 19px">
                                                                    <asp:DropDownList ID="cboTARPeriod" runat="server" CssClass="RegularText" Width="99px"
                                                                        AutoPostBack="True" OnSelectedIndexChanged="cboTARPeriod_SelectedIndexChanged"
                                                                        meta:resourcekey="cboTARPeriodResource1">
                                                                        <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource32" Text="Off"></asp:ListItem>
                                                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource33" Text="1 Hour"></asp:ListItem>
                                                                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource34" Text="2 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="4" meta:resourcekey="ListItemResource35" Text="4 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="6" meta:resourcekey="ListItemResource36" Text="6 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="12" meta:resourcekey="ListItemResource37" Text="12 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="24" meta:resourcekey="ListItemResource38" Text="1 Day"></asp:ListItem>
                                                                        <asp:ListItem Value="48" meta:resourcekey="ListItemResource39" Text="2 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="72" meta:resourcekey="ListItemResource40" Text="3 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="96" meta:resourcekey="ListItemResource41" Text="4 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="120" meta:resourcekey="ListItemResource42" Text="5 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="144" meta:resourcekey="ListItemResource43" Text="6 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="168" meta:resourcekey="ListItemResource44" Text="7 Days"></asp:ListItem>
                                                                    </asp:DropDownList><asp:Label ID="lblTarMode" runat="server" CssClass="formtext"
                                                                        meta:resourcekey="lblTarModeResource1"></asp:Label></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblChangeBoxID" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" colspan="1">
                                                                    <asp:Label ID="lblBoxIdTitle" runat="server" CssClass="formtext" meta:resourcekey="lblBoxIdTitleResource1" Text="BoxId:"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtBoxId" runat="server" CssClass="formtext" Width="74px" meta:resourcekey="txtBoxIdResource1"></asp:TextBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" style="width: 152px">
                                                                    <asp:Label ID="lblSIMNumberTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSIMNumberTitleResource1" Text="SIM/ESN:"></asp:Label></td>
                                                                <td>
                                                                    <asp:TextBox ID="txtSIMNumber" runat="server" CssClass="formtext" Width="185px" meta:resourcekey="txtSIMNumberResource1"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblControllerStatus" style="width: 384px" cellspacing="0" cellpadding="0"
                                                            width="384" border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" colspan="1">
                                                                    <asp:Label ID="lblControllerVersionTitle" runat="server" CssClass="formtext" meta:resourcekey="lblControllerVersionTitleResource1" Text="Controller Version:"></asp:Label></td>
                                                                <td>
                                                                    <asp:Label ID="lblControllerVersion" runat="server" CssClass="formtext" meta:resourcekey="lblControllerVersionResource1"></asp:Label></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblServiceRequired" style="width: 384px" cellspacing="0" cellpadding="0"
                                                            width="384" border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" colspan="1">
                                                                    <asp:Label ID="lblServiceRequiredTitle" runat="server" CssClass="formtext" meta:resourcekey="lblServiceRequiredTitleResource1" Text="Service Required:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboServiceRequired" runat="server" CssClass="RegularText" Width="99px"
                                                                        OnSelectedIndexChanged="cboServiceRequired_SelectedIndexChanged" meta:resourcekey="cboServiceRequiredResource1">
                                                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource45" Text="Disabled"></asp:ListItem>
                                                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource46" Text="1 Hour"></asp:ListItem>
                                                                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource47" Text="3 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="5" meta:resourcekey="ListItemResource48" Text="5 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="12" meta:resourcekey="ListItemResource49" Text="12 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="24" meta:resourcekey="ListItemResource50" Text="1 Day"></asp:ListItem>
                                                                        <asp:ListItem Value="120" meta:resourcekey="ListItemResource51" Text="5 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="240" meta:resourcekey="ListItemResource52" Text="10 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="480" meta:resourcekey="ListItemResource53" Text="20 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="720" meta:resourcekey="ListItemResource54" Text="30 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="960" meta:resourcekey="ListItemResource55" Text="40 Days"></asp:ListItem>
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblSetIdle" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" colspan="1">
                                                                    <asp:Label ID="lblSetIdleTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSetIdleTitleResource1" Text="Set Idle:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboSetIdle" runat="server" CssClass="RegularText" Width="99px"
                                                                        meta:resourcekey="cboSetIdleResource1">
                                                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource56" Text="Disabled"></asp:ListItem>
                                                                        <asp:ListItem Value="5" meta:resourcekey="ListItemResource57" Text="5 min"></asp:ListItem>
                                                                        <asp:ListItem Value="15" meta:resourcekey="ListItemResource58" Text="15 min"></asp:ListItem>
                                                                        <asp:ListItem Value="30" meta:resourcekey="ListItemResource59" Text="30 min"></asp:ListItem>
                                                                        <asp:ListItem Value="60" meta:resourcekey="ListItemResource60" Text="1 hour"></asp:ListItem>
                                                                        <asp:ListItem Value="120" meta:resourcekey="ListItemResource61" Text="2 hours"></asp:ListItem>
                                                                        <asp:ListItem Value="180" meta:resourcekey="ListItemResource62" Text="3 hours"></asp:ListItem>
                                                                        <asp:ListItem Value="240" meta:resourcekey="ListItemResource63" Text="4 hours"></asp:ListItem>
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblSetRecordCount" style="width: 384px" cellspacing="0" cellpadding="0"
                                                            width="384" border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" colspan="1">
                                                                    <asp:Label ID="lblSetRecordCountTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSetRecordCountTitleResource1" Text="Set Record Count:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboSetRecordCount" runat="server" CssClass="RegularText" Width="99px"
                                                                        meta:resourcekey="cboSetRecordCountResource1">
                                                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource64" Text="1"></asp:ListItem>
                                                                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource65" Text="2"></asp:ListItem>
                                                                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource66" Text="3"></asp:ListItem>
                                                                        <asp:ListItem Value="4" meta:resourcekey="ListItemResource67" Text="4"></asp:ListItem>
                                                                        <asp:ListItem Value="5" meta:resourcekey="ListItemResource68" Text="5"></asp:ListItem>
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblSetOdometer" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                    <asp:Label ID="lblOdometerTitle" runat="server" CssClass="formtext" meta:resourcekey="lblOdometerTitleResource1" Text="Odometer"></asp:Label>
                                                                    (<asp:Label ID="lblUnit" runat="server" CssClass="formtext" meta:resourcekey="lblUnitResource1"></asp:Label>):</td>
                                                                <td align="left">
                                                                    <asp:TextBox ID="txtSetOdometer" runat="server" CssClass="formtext" meta:resourcekey="txtSetOdometerResource1"></asp:TextBox>
                                                                    <asp:Label ID="lblOdometer" runat="server" CssClass="formtext" meta:resourcekey="lblOdometerResource1"></asp:Label>
                                                                    <asp:DropDownList ID="cboMesUnits" runat="server" CssClass="formtext">
                                                                        <asp:ListItem Selected="True" Value="1">Km</asp:ListItem>
                                                                        <asp:ListItem Value="0.6214">Mi</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblPowerOffDelay" style="width: 384px" cellspacing="0" cellpadding="0"
                                                            width="384" border="0" runat="server">
                                                            <tr>
                                                                <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                    <asp:Label ID="lblPowerOffDelay" runat="server" CssClass="formtext" meta:resourcekey="lblPowerOffDelayResource1" Text="Set Power Off Delay:"></asp:Label></td>
                                                                <td align="left">
                                                                    <asp:TextBox ID="txtPowerOffDelay" runat="server" CssClass="formtext" meta:resourcekey="txtPowerOffDelayResource1"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table><table id="tblEEPROMSettings" style="width: 384px" cellspacing="0" cellpadding="0"
                                                            width="384" border="0" runat="server">
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblEEPROMOffset" runat="server" CssClass="formtext" 
                                                                    Text="Offset:" meta:resourcekey="lblEEPROMOffsetResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtEEPROMOffset" runat="server" CssClass="formtext" meta:resourcekey="txtEEPROMOffsetResource1" ></asp:TextBox>
                                                              </td>
                                                           </tr>
                                                           <tr>
                                                              <td align="left" class="formtext" colspan="1" style="width: 152px">
                                                                 <asp:Label ID="lblEEPROMLenght" runat="server" CssClass="formtext" 
                                                                    Text="Length:" meta:resourcekey="lblEEPROMLenghtResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtEEPROMLength" runat="server" CssClass="formtext" meta:resourcekey="txtEEPROMLengthResource1" ></asp:TextBox></td>
                                                           </tr>
                                                           <tr>
                                                              <td align="left" class="formtext" colspan="1" style="width: 152px">
                                                                 <asp:Label ID="lblEEPROMData" runat="server" CssClass="formtext" 
                                                                    Text="Data:" meta:resourcekey="lblEEPROMDataResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtEEPROMData" runat="server" CssClass="formtext" TextMode="MultiLine" Width="228px" meta:resourcekey="txtEEPROMDataResource1" ></asp:TextBox></td>
                                                           </tr>
                                                        </table><table id="tblKeyFobSetup" style="width: 384px" cellspacing="0" cellpadding="0"
                                                            width="384" border="0" runat="server">
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1" valign="top">
                                                                 <asp:Label ID="lblKeyFobSetupDesc" runat="server" CssClass="formtext" Text="KeyFob Setup:" meta:resourcekey="lblKeyFobSetupDescResource1"></asp:Label></td>
                                                              <td align="left" valign="top">
                                                                 <asp:RadioButtonList ID="optKeyFobSetup" runat="server" CssClass="formtext" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" meta:resourcekey="optKeyFobSetupResource1">
                                                                    <asp:ListItem Selected="True" Value="0" meta:resourcekey="ListItemResource144">Disable</asp:ListItem>
                                                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource145">Enable</asp:ListItem>
                                                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource146">Learn</asp:ListItem>
                                                                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource147">Erase</asp:ListItem>
                                                                 </asp:RadioButtonList></td>
                                                           </tr>
                                                        </table><table id="tblBoxSleepTime" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblBoxSleepTime" runat="server" CssClass="formtext" 
                                                                    Text="Sleep Time" meta:resourcekey="lblBoxSleepTimeResource1"></asp:Label>
                                                                 :</td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtSleepTime" runat="server" CssClass="formtext" meta:resourcekey="txtSleepTimeResource1"></asp:TextBox>
                                                              </td>
                                                           </tr>
                                                        </table><table id="tblIridiumFilter" class="formtext" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblIridiumSensorMask" runat="server" Text="Iridium Sensor Mask (Hex: FFFFFF)" meta:resourcekey="lblIridiumSensorMaskResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtIridiumSensorMask" runat="server" meta:resourcekey="txtIridiumSensorMaskResource1"></asp:TextBox></td>
                                                           </tr>
                                                           
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblIridiumMsgMask" runat="server" Text="Iridium Msg Mask (Hex: FFFFFF):" meta:resourcekey="lblIridiumMsgMaskResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtIridiumMsgMask" runat="server" meta:resourcekey="txtIridiumMsgMaskResource1"></asp:TextBox></td>
                                                           </tr>
                                                           
                                                           
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblIridiumAlarmMask" runat="server" Text="Iridium Alarm Mask (Hex: FFFFFF)" meta:resourcekey="lblIridiumAlarmMaskResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtIridiumAlarmMask" runat="server" meta:resourcekey="txtIridiumAlarmMaskResource1"></asp:TextBox></td>
                                                           </tr>
                                                           
                                                           
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblIridiumInitDelay" runat="server" Text="Iridium Init Delay (sec):" meta:resourcekey="lblIridiumInitDelayResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtIridiumInitDelay" runat="server" meta:resourcekey="txtIridiumInitDelayResource1"></asp:TextBox></td>
                                                           </tr>
                                                           
                                                              <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblIridiumMultiplier" runat="server" Text="Iridium Multiplier (0-Off; 1,2..10):" meta:resourcekey="lblIridiumMultiplierResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:TextBox ID="txtIridiumMultiplier" runat="server" meta:resourcekey="txtIridiumMultiplierResource1"></asp:TextBox></td>
                                                           </tr>
                                                        </table>
                                                        
                                                        
                                                        <table id="tblPowerSave" class="formtext" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
                                                            border="0" runat="server">
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblPowerCfgSleepTime" runat="server" Text="Sleep Time (1-24 hours):" meta:resourcekey="lblPowerCfgSleepTimeResource1"></asp:Label></td>
                                                              <td align="left">
                                                                 <asp:DropDownList
                                                                                    ID="cboPowerCfgSleepTime" runat="server" CssClass="RegularText" meta:resourcekey="cboPowerCfgSleepTimeResource1" >
                                                                                    <asp:ListItem Value="1"  Text="1 Hour" meta:resourcekey="ListItemResource148"></asp:ListItem>
                                                                                    <asp:ListItem Value="2"  Text="2 Hours" meta:resourcekey="ListItemResource149"></asp:ListItem>
                                                                                    <asp:ListItem Value="3"  Text="3 Hours" meta:resourcekey="ListItemResource150"></asp:ListItem>
                                                                                    <asp:ListItem Value="4"  Text="4 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="5"  Text="5 Hours" meta:resourcekey="ListItemResource151"></asp:ListItem>
                                                                                    <asp:ListItem Value="6"  Text="6 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="8"  Text="8 Hours" meta:resourcekey="ListItemResource152"></asp:ListItem>
                                                                                    <asp:ListItem Value="9"  Text="9 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="10"  Text="10 Hours" meta:resourcekey="ListItemResource153"></asp:ListItem>
                                                                                    <asp:ListItem Value="11"  Text="11 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="12"  Text="12 Hours" meta:resourcekey="ListItemResource154"></asp:ListItem>
                                                                                    <asp:ListItem Value="13"  Text="13 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="14"  Text="14 Hours" meta:resourcekey="ListItemResource155"></asp:ListItem>
                                                                                    <asp:ListItem Value="15"  Text="15 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="16"  Text="16 Hours" meta:resourcekey="ListItemResource156"></asp:ListItem>
                                                                                    <asp:ListItem Value="17"  Text="17 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="18"  Text="18 Hours" meta:resourcekey="ListItemResource157"></asp:ListItem>
                                                                                    <asp:ListItem Value="19"  Text="19 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="20"  Text="20 Hours" meta:resourcekey="ListItemResource158"></asp:ListItem>
                                                                                    <asp:ListItem Value="21"  Text="21 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="22"  Text="22 Hours" meta:resourcekey="ListItemResource159"></asp:ListItem>
                                                                                    <asp:ListItem Value="23"  Text="23 Hours" ></asp:ListItem>
                                                                                    <asp:ListItem Value="24"  Text="24 Hours" meta:resourcekey="ListItemResource160"></asp:ListItem>
                                                                                </asp:DropDownList>
                                                                                
                                                                 
                                                                 </td>
                                                           </tr>
                                                           
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblMDTSleepTime" runat="server" Text="MDT Sleep Time:" meta:resourcekey="lblMDTSleepTimeResource1"></asp:Label></td>
                                                              <td align="left">
                                                                   <asp:DropDownList
                                                                                    ID="cboMDTSleepTime" runat="server" CssClass="RegularText" meta:resourcekey="cboMDTSleepTimeResource1" >
                                                                                    <asp:ListItem Value="0"  Text="Do not turn off MDT" meta:resourcekey="ListItemResource161"></asp:ListItem>
                                                                                    <asp:ListItem Value="5"  Text="5 Minutes" meta:resourcekey="ListItemResource162"></asp:ListItem>
                                                                                    <asp:ListItem Value="15"  Text="15 Minutes" meta:resourcekey="ListItemResource163"></asp:ListItem>
                                                                                    <asp:ListItem Value="30"  Text="30 Minutes" meta:resourcekey="ListItemResource164"></asp:ListItem>
                                                                                    <asp:ListItem Value="45"  Text="45 Minutes" meta:resourcekey="ListItemResource165"></asp:ListItem>
                                                                                    <asp:ListItem Value="60"  Text="1 Hour" meta:resourcekey="ListItemResource166"></asp:ListItem>
                                                                                    <asp:ListItem Value="120"  Text="2 Hours" meta:resourcekey="ListItemResource167"></asp:ListItem>
                                                                                    <asp:ListItem Value="180"  Text="3 Hours" meta:resourcekey="ListItemResource168"></asp:ListItem>
                                                                                    <asp:ListItem Value="240"  Text="4 Hours" meta:resourcekey="ListItemResource169"></asp:ListItem>
                                                                                    
                                                                                </asp:DropDownList>
                                                                                
                                                                
                                                                </td>
                                                           </tr>
                                                           
                                                           
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblModemTurnOffTime" runat="server" Text="Modem turn off time:" meta:resourcekey="lblModemTurnOffTimeResource1"></asp:Label></td>
                                                              <td align="left">
                                                              
                                                               <asp:DropDownList
                                                                                    ID="cboModemTurnOffTime" runat="server" CssClass="RegularText" meta:resourcekey="cboModemTurnOffTimeResource1" >
                                                                                    <asp:ListItem Value="0"  Text="Do not turn off Modem" meta:resourcekey="ListItemResource170"></asp:ListItem>
                                                                                    <asp:ListItem Value="1"  Text="1 Hour" meta:resourcekey="ListItemResource171"></asp:ListItem>
                                                                                    <asp:ListItem Value="2"  Text="2 Hours" meta:resourcekey="ListItemResource172"></asp:ListItem>
                                                                                    <asp:ListItem Value="3"  Text="3 Hours" meta:resourcekey="ListItemResource173"></asp:ListItem>
                                                                                    <asp:ListItem Value="5"  Text="5 Hours" meta:resourcekey="ListItemResource174"></asp:ListItem>
                                                                                    <asp:ListItem Value="8"  Text="8 Hours" meta:resourcekey="ListItemResource175"></asp:ListItem>
                                                                                    <asp:ListItem Value="10"  Text="10 Hours" meta:resourcekey="ListItemResource176"></asp:ListItem>
                                                                                    <asp:ListItem Value="12"  Text="12 Hours" meta:resourcekey="ListItemResource177"></asp:ListItem>
                                                                                    <asp:ListItem Value="14"  Text="14 Hours" meta:resourcekey="ListItemResource178"></asp:ListItem>
                                                                                    <asp:ListItem Value="16"  Text="16 Hours" meta:resourcekey="ListItemResource179"></asp:ListItem>
                                                                                    <asp:ListItem Value="18"  Text="18 Hours" meta:resourcekey="ListItemResource180"></asp:ListItem>
                                                                                    <asp:ListItem Value="20"  Text="20 Hours" meta:resourcekey="ListItemResource181"></asp:ListItem>
                                                                                    <asp:ListItem Value="22"  Text="22 Hours" meta:resourcekey="ListItemResource182"></asp:ListItem>
                                                                                    <asp:ListItem Value="24"  Text="24 Hours" meta:resourcekey="ListItemResource183"></asp:ListItem>
                                                                                </asp:DropDownList>
                                                                
                                                                
                                                                </td>
                                                           </tr>
                                                           
                                                           
                                                           <tr>
                                                              <td class="formtext" style="width: 152px" align="left" colspan="1">
                                                                 <asp:Label ID="lblPeriodicWakeupTime" runat="server" Text="Periodic wakeup time:" meta:resourcekey="lblPeriodicWakeupTimeResource1"></asp:Label></td>
                                                              <td align="left">
                                                              
                                                              
                                                               <asp:DropDownList
                                                                                    ID="cboPeriodicWakeupTime" runat="server" CssClass="RegularText" meta:resourcekey="cboPeriodicWakeupTimeResource1" >
                                                                                    <asp:ListItem Value="0"  Text="Do not wakeup" meta:resourcekey="ListItemResource184"></asp:ListItem>
                                                                                    <asp:ListItem Value="1"  Text="1 Hour" meta:resourcekey="ListItemResource185"></asp:ListItem>
                                                                                    <asp:ListItem Value="5"  Text="5 Hours" meta:resourcekey="ListItemResource186"></asp:ListItem>
                                                                                    <asp:ListItem Value="10"  Text="10 Hours" meta:resourcekey="ListItemResource187"></asp:ListItem>
                                                                                    <asp:ListItem Value="24"  Text="1 Day" meta:resourcekey="ListItemResource188"></asp:ListItem>
                                                                                    <asp:ListItem Value="48"  Text="2 Days" meta:resourcekey="ListItemResource189"></asp:ListItem>
                                                                                    <asp:ListItem Value="72"  Text="3 Days" meta:resourcekey="ListItemResource190"></asp:ListItem>
                                                                                    <asp:ListItem Value="120"  Text="5 Days" meta:resourcekey="ListItemResource191"></asp:ListItem>
                                                                                    <asp:ListItem Value="168"  Text="7 Days" meta:resourcekey="ListItemResource192"></asp:ListItem>
                                                                                </asp:DropDownList>
                                                              
                                                              
                                                                </td>
                                                           </tr>
                                                           
                                                            
                                                        </table>
                                                        
                                                        <table id="tblExtendedReportInterval" style="width: 384px" cellspacing="0" cellpadding="0"
                                                            width="384" border="0" runat="server">
                                                          <tr>
                                                             <td class="formtext" style="width: 152px" colspan="1">
                                                                <asp:Label ID="lblLabelExtReportInterval" runat="server" CssClass="formtext" 
                                                                   Text="Report Interval" meta:resourcekey="lblLabelExtReportIntervalResource1"></asp:Label>&nbsp;</td>
                                                             <td class=formtext style="width: 232px" >
                                                                <asp:TextBox ID="txtExtRptInterval" runat="server" meta:resourcekey="txtExtRptIntervalResource1"></asp:TextBox>
                                                                (seconds)</td>
                                                          </tr>
                                                          <tr>
                                                             <td class="formtext" colspan="1" style="width: 152px">
                                                                <asp:Label ID="lblExtNumOfReports" runat="server" CssClass="formtext" 
                                                                   Text="Number of Reports" meta:resourcekey="lblExtNumOfReportsResource1"></asp:Label></td>
                                                             <td style="width: 232px">
                                                                <asp:TextBox ID="txtExtNumOfReport" runat="server" meta:resourcekey="txtExtNumOfReportResource1"></asp:TextBox></td>
                                                          </tr>
                                                       </table><table id="tblWriteSMC" style="width: 384px" cellspacing="0" cellpadding="0"
                                                            width="384" border="0" runat="server">
                                                           <tr>
                                                               <td class="formtext" style="width: 152px; height: 24px;" colspan="1">
                                                                   <asp:Label ID="lblSMC_Code1" runat="server" CssClass="formtext" Text="Code 1: (0-255)"></asp:Label></td>
                                                               <td class=formtext style="width: 232px; height: 24px;" >
                                                                   <asp:TextBox ID="txtSMC_Code1" runat="server" ></asp:TextBox>
                                                               </td>
                                                           </tr>
                                                           <tr>
                                                               <td class="formtext" colspan="1" style="width: 152px; height: 24px;">
                                                                   <asp:Label ID="lblSMC_Code2" runat="server" CssClass="formtext" Text="Code 2: (0-255)"></asp:Label></td>
                                                               <td style="width: 232px; height: 24px;">
                                                                   <asp:TextBox ID="txtSMC_Code2" runat="server" ></asp:TextBox></td>
                                                           </tr>
                                                           <tr>
                                                               <td class="formtext" colspan="1" style="width: 152px; height: 18px">
                                                                   <asp:Label ID="lblSMC_Code3" runat="server" CssClass="formtext" Text="Code 3: (0-255)"></asp:Label></td>
                                                               <td style="width: 232px; height: 18px">
                                                                   <asp:TextBox ID="txtSMC_Code3" runat="server" ></asp:TextBox></td>
                                                           </tr>
                                                           <tr>
                                                               <td class="formtext" colspan="1" style="width: 152px">
                                                                   <asp:Label ID="lblSMC_Code4" runat="server" CssClass="formtext" Text="Code 4: (0-255)"></asp:Label></td>
                                                               <td style="width: 232px">
                                                                   <asp:TextBox ID="txtSMC_Code4" runat="server" ></asp:TextBox></td>
                                                           </tr>
                                                           <tr>
                                                               <td class="formtext" colspan="1" style="width: 152px">
                                                                   <asp:Label ID="lblSMC_Code5" runat="server" CssClass="formtext" Text="Code 5: (0-255)"></asp:Label></td>
                                                               <td style="width: 232px">
                                                                   <asp:TextBox ID="txtSMC_Code5" runat="server" ></asp:TextBox></td>
                                                           </tr>
                                                       </table>
                                                       
                                                      
                                                       <asp:TextBox ID="lblMessage" runat="server" CssClass="errortext" ReadOnly="True"
                                                          TextMode="MultiLine" Visible="False" Width="380px" meta:resourcekey="lblMessageResource2"></asp:TextBox>
                                                       
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="width: 412px" align="center">
                                                        <table id="tblSetTrace" style="width: 43px; height: 133px" cellspacing="0" cellpadding="0"
                                                            border="0" runat="server" designtimedragdrop="1402">
                                                            <tr>
                                                                <td style="height: 133px" align="center">
                                                                    <div id="divSetTrace" title="Box Setup:" style="border-right: gray thin outset;
                                                                        border-top: gray thin outset; overflow: auto; border-left: gray thin outset;
                                                                        width: 390px; border-bottom: gray thin outset; height: 318px" align="center">
                                                                        <table id="Table5" style="width: 363px; height: 211px" cellspacing="0" cellpadding="0"
                                                                            width="363" border="0">
                                                                            <tr>
                                                                                <td style="height: 20px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="height: 20px">
                                                                                    <table class="formtext" id="Table4" style="width: 100%; height: 60px" cellspacing="0"
                                                                                        cellpadding="0" width="100%" border="0">
                                                                                        <tr>
                                                                                            <td style="height: 26px">
                                                                                                <asp:Label ID="lblTracePeriodTitle" runat="server" meta:resourcekey="lblTracePeriodTitleResource1" Text="Trace Period:"></asp:Label></td>
                                                                                            <td style="height: 26px">
                                                                                                <asp:DropDownList ID="cboTracePeriod" runat="server" CssClass="RegularText" Width="99px"
                                                                                                    AutoPostBack="True" OnSelectedIndexChanged="cboTracePeriod_SelectedIndexChanged"
                                                                                                    meta:resourcekey="cboTracePeriodResource1">
                                                                                                    <asp:ListItem Value="0" meta:resourcekey="ListItemResource69" Text="Disabled"></asp:ListItem>
                                                                                                    <asp:ListItem Value="60" meta:resourcekey="ListItemResource70" Text="1 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="120" meta:resourcekey="ListItemResource71" Text="2 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="300" Selected="True" meta:resourcekey="ListItemResource72" Text="5 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="600" meta:resourcekey="ListItemResource73" Text="10 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="900" meta:resourcekey="ListItemResource74" Text="15 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="1800" meta:resourcekey="ListItemResource75" Text="30 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="3600" meta:resourcekey="ListItemResource76" Text="1 hour"></asp:ListItem>
                                                                                                    <asp:ListItem Value="43200" meta:resourcekey="ListItemResource77" Text="12 hours"></asp:ListItem>
                                                                                                    <asp:ListItem Value="64800" meta:resourcekey="ListItemResource78" Text="18 hours"></asp:ListItem>
                                                                                                </asp:DropDownList></td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="height: 20px">
                                                                                                <asp:Label ID="lblTraceIntervalTitle" runat="server" meta:resourcekey="lblTraceIntervalTitleResource1" Text="Trace Interval:"></asp:Label></td>
                                                                                            <td style="height: 20px">
                                                                                                <asp:DropDownList ID="cboTraceInterval" runat="server" CssClass="RegularText" Width="99px"
                                                                                                    meta:resourcekey="cboTraceIntervalResource1">
                                                                                                    <asp:ListItem Value="0" meta:resourcekey="ListItemResource79" Text="Disabled"></asp:ListItem>
                                                                                                    <asp:ListItem Value="15" Selected="True" meta:resourcekey="ListItemResource80" Text="15 sec"></asp:ListItem>
                                                                                                    <asp:ListItem Value="30" meta:resourcekey="ListItemResource81" Text="30 sec"></asp:ListItem>
                                                                                                    <asp:ListItem Value="120" meta:resourcekey="ListItemResource82" Text="2 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="300" meta:resourcekey="ListItemResource83" Text="5 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="600" meta:resourcekey="ListItemResource84" Text="10 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="900" meta:resourcekey="ListItemResource85" Text="15 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="1800" meta:resourcekey="ListItemResource86" Text="30 min"></asp:ListItem>
                                                                                                    <asp:ListItem Value="3600" meta:resourcekey="ListItemResource87" Text="1 hour"></asp:ListItem>
                                                                                                    <asp:ListItem Value="43200" meta:resourcekey="ListItemResource88" Text="12 hours"></asp:ListItem>
                                                                                                    <asp:ListItem Value="57600" meta:resourcekey="ListItemResource89" Text="16 hours"></asp:ListItem>
                                                                                                </asp:DropDownList></td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="center">
                                                                                    <asp:DataGrid ID="dgTraceSensors" runat="server" Width="333px" AutoGenerateColumns="False"
                                                                                        BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" DataKeyField="SensorId"
                                                                                        PageSize="4" meta:resourcekey="dgTraceSensorsResource1">
                                                                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                        <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                                        <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black"
                                                                                            BackColor="#DEDFDE"></ItemStyle>
                                                                                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                        <Columns>
                                                                                            <asp:BoundColumn Visible="False" DataField="SensorId" HeaderText='<%$ Resources:dgTraceSensors_SensorId %>'>
                                                                                            </asp:BoundColumn>
                                                                                            <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgTraceSensors_Sensors %>'>
                                                                                            </asp:BoundColumn>
                                                                                            <asp:TemplateColumn HeaderText='<%$ Resources:dgTraceSensors_Trace %>'>
                                                                                                <ItemStyle Wrap="False"></ItemStyle>
                                                                                                <ItemTemplate>
                                                                                                    <asp:Label ID="lblTrace" Text='<%# DataBinder.Eval(Container.DataItem,"TraceStateName") %>'
                                                                                                        runat="server" meta:resourcekey="lblTraceResource1"></asp:Label>
                                                                                                </ItemTemplate>
                                                                                                <EditItemTemplate>
                                                                                                    <asp:DropDownList ID="cboTrace" DataSource='<%# dsTrace %>' DataValueField="TraceStateId"
                                                                                                        DataTextField="TraceStateName" SelectedIndex='<%# GetTrace(Convert.ToInt16(DataBinder.Eval(Container.DataItem,"TraceStateId"))) %>'
                                                                                                        runat="server" meta:resourcekey="cboTraceResource1">
                                                                                                    </asp:DropDownList>
                                                                                                </EditItemTemplate>
                                                                                            </asp:TemplateColumn>
                                                                                            <asp:EditCommandColumn UpdateText="&lt;img src=../images/ok.gif border=0&gt;" CancelText="&lt;img src=../images/cancel.gif border=0&gt;"
                                                                                                EditText="&lt;img src=../images/edit.gif border=0&gt;" meta:resourcekey="EditCommandColumnResource1">
                                                                                            </asp:EditCommandColumn>
                                                                                        </Columns>
                                                                                        <PagerStyle Mode="NumericPages"></PagerStyle>
                                                                                    </asp:DataGrid></td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblVoiceMsg" style="width: 380px; height: 40px" cellspacing="0" cellpadding="0"
                                                            width="380" border="0" runat="server">
                                                            <tr>
                                                                <td style="width: 59px">
                                                                    <asp:Label ID="Label1" runat="server" CssClass="formtext" Width="52px" meta:resourcekey="Label1Resource1" Text="Voice Message:"></asp:Label></td>
                                                                <td align="left">
                                                                    <asp:TextBox ID="txtVoiceMsg" runat="server" CssClass="formtext" Width="333px" TextMode="MultiLine"
                                                                        meta:resourcekey="txtVoiceMsgResource1"></asp:TextBox></td>
                                                            </tr>
                                                        </table><table class="formtext" id="tblDeleteGeoZones" cellspacing="0" cellpadding="0" width="100%"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td style="width: 115px; height: 19px;">
                                                                    <asp:Label ID="lblDeleteGeoZone" runat="server" Text="Delete GeoZone:"></asp:Label></td>
                                                                <td style="height: 19px">
                                                                    <asp:DropDownList ID="cboAssGeoZones" runat="server" CssClass="RegularText" Width="220px" DataTextField="GeozoneName" DataValueField="GeozoneId"
                                                                        >
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        <table class="formtext" id="tblDeviceTest" cellspacing="0" cellpadding="0" width="100%"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td style="width: 115px">
                                                                    <asp:Label ID="lblDeviceTest" runat="server" Text="Device Test:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboDeviceTest" runat="server" CssClass="RegularText" Width="160px"  >
                                                                        <asp:ListItem Selected="True" Value="0">No Test</asp:ListItem>
                                                                        <asp:ListItem Value="1">Flash Test</asp:ListItem>
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        <table class="formtext" id="tblReportInterval" cellspacing="0" cellpadding="0" width="100%"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td style="width: 115px">
                                                                    <asp:Label ID="lblReportingFrequencyTitle" runat="server" meta:resourcekey="lblReportingFrequencyTitleResource1" Text="Reporting Frequency:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboReportingFreq" runat="server" CssClass="RegularText" Width="160px"
                                                                        meta:resourcekey="cboReportingFreqResource1">
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        <table class="formtext" id="tblSpeedThreshold" cellspacing="0" cellpadding="0" width="100%"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td style="width: 115px">
                                                                    <asp:Label ID="lblSpeedThresholdTitle" runat="server" meta:resourcekey="lblSpeedThresholdTitleResource1" Text="Speed Threshold:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboSpeedThreshold" runat="server" CssClass="RegularText" Width="160px"
                                                                        meta:resourcekey="cboSpeedThresholdResource1">
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        <table class="formtext" id="tblGeoFence" cellspacing="0" cellpadding="0" width="100%"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td style="width: 115px">
                                                                    <asp:Label ID="lblGeoFenceTitle" runat="server" meta:resourcekey="lblGeoFenceTitleResource1" Text="GeoFence:"></asp:Label></td>
                                                                <td>
                                                                    <asp:DropDownList ID="cboGeoFence" runat="server" CssClass="RegularText" Width="160px"
                                                                        meta:resourcekey="cboGeoFenceResource1">
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                        </table>
                                                        
                                                        <table class="formtext" id="tblAddGeoZone" cellspacing="0" cellpadding="0" width="100%"
                                                            border="0" runat="server">
                                                           <tr>
                                                              <td style="width: 115px">
                                                                 <asp:Label ID="lblAddGeoZone" runat="server" 
                                                                    Text="Add GeoZone:"></asp:Label></td>
                                                              <td>
                                                                 <asp:DropDownList ID="cboUnAssignedGeoZones" runat="server" CssClass="RegularText" Width="220px" DataTextField="GeozoneName" DataValueField="GeozoneId"
                                                                        >
                                                                 </asp:DropDownList></td>
                                                           </tr>
                                                           <tr>
                                                              <td style="width: 115px">
                                                                  <asp:Label ID="lblGeoZoneSpeed" runat="server" Text="Speed"></asp:Label>
                                                               &nbsp;(<asp:Label ID="lblGeoZoneSpeedUnit" runat="server" CssClass="formtext" 
                                                                      meta:resourcekey="lblUnitResource1"></asp:Label>):</td>
                                                              <td>
                                                                  <asp:TextBox ID="txtGeoZoneSpeed" runat="server" CssClass="formtext" 
                                                                      Width="48px">0</asp:TextBox>
                                                               </td>
                                                           </tr>
                                                        </table>

                                                        <table class="formtext" id="tblSetTemp" cellspacing="0" cellpadding="0" width="100%" border="0" runat="server" >  
                                                            <tr>
                                                              <td style="width: 145px">
                                                                <asp:Label ID="lblZone" runat="server" Text="Select Zone:"></asp:Label>  
                                                              </td>
                                                              <td>
                                                                 
                                                                      <asp:DropDownList ID="ddlTemp" runat="server" ></asp:DropDownList>
                                                               </td>
                                                           </tr>
                                                           <tr>
                                                              <td style="width: 145px">
                                                                  Select Temperature:</td>
                                                              <%--<td>
                                                                  <asp:TextBox ID="txtTemp" runat="server" CssClass="formtext" 
                                                                      Width="48px"  Text="0"/>                                                                  
                                                               </td>--%>                                                              
                                                               
                                                               <td style="height: 45px;">
                                                                   <div id="tempratureType"  visible="false"  runat="server">  
                                                                       <asp:TextBox ID="txtbxcelsius" runat="server" CssClass="formtext" Width="48px" onkeydown="return isNumeric(event.keyCode);"    onkeyup="ConvertTemprature('C',event.keyCode)" Text="0" onpaste = "return false;"  /> 
                                                                       <asp:Label ID="lblcelcius" runat="server" Text="Celsius" style="margin-right:20px" ></asp:Label> 
                                                                       
                                                                        <asp:TextBox ID="txtbxfahrenheit" runat="server" CssClass="formtext" Width="48px" onkeydown="return isNumeric(event.keyCode);"    onkeyup="ConvertTemprature('F',event.keyCode)" Text="0" onpaste = "return false;"  /> 
                                                                       <asp:Label ID="lblfahrenheit" runat="server" Text="Fahrenheit"  ></asp:Label>
                                                                   </div>
                                                               </td>
                                                           </tr> 

                                                            <tr>                                                               
                                                            <td colspan="2" align ="center">
                                                                <asp:label ID="lblErrorMessage" runat="server" Visible="true" style="color:red;font-size:small;margin-right:65px" Text="" ></asp:label> 
                                                                </td>
                                                            </tr> 
                                                        </table>                                                       

                                                        <table class="formtext" id="tblChangeOperatingMode" cellspacing="0" cellpadding="0" width="100%"
                                                            border="0" runat="server">                                                           
                                                           <tr>
                                                              <td style="width: 145px">
                                                                  Select Operating Mode:</td>
                                                              <td>
                                                                  <asp:DropDownList ID="cboOperationMode" runat="server" CssClass="RegularText" Width="220px" >
                                                                    <asp:ListItem Text="Continuous Run Mode" Value="2"></asp:ListItem>
                                                                    <asp:ListItem Text="Cycle Sentry Mode" Value="4"></asp:ListItem>
                                                                    <asp:ListItem Text="Sleep Mode" Value="6"></asp:ListItem>
                                                                 </asp:DropDownList>
                                                               </td>
                                                           </tr>
                                                        </table>

                                                        <table id="tblSetSensors" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                            <tr>
                                                                <td style="height: 133px" align="center">
                                                                    <div id="divSetSensors" title="Box Setup:" style="border-right: gray thin outset;
                                                                        border-top: gray thin outset; overflow: auto; border-left: gray thin outset;
                                                                        width: 390px; border-bottom: gray thin outset; height: 318px" align="center">
                                                                        <table id="tblSetSensorsGeneral" style="width: 369px; height: 190px" cellspacing="0"
                                                                            cellpadding="0" width="369" border="0">
                                                                            <tr>
                                                                                <td style="height: 20px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="center">
                                                                                    <asp:Button ID="cmdUnselectAllSensors" runat="server" CssClass="combutton" Text="Deselect All"
                                                                                        Width="92px" OnClick="cmdUnselectAllSensors_Click" meta:resourcekey="cmdUnselectAllSensorsResource1">
                                                                                    </asp:Button>&nbsp;
                                                                                    <asp:Button ID="cmdSetAllSensors" runat="server" CssClass="combutton" Text="Select All"
                                                                                        Width="92px" OnClick="cmdSetAllSensors_Click" meta:resourcekey="cmdSetAllSensorsResource1">
                                                                                    </asp:Button></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="center">
                                                                                    <asp:DataGrid ID="dgSetSensors" runat="server" Width="331px" AutoGenerateColumns="False"
                                                                                        BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" DataKeyField="SensorId"
                                                                                        PageSize="4" meta:resourcekey="dgSetSensorsResource1">
                                                                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                        <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                                        <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black"
                                                                                            BackColor="#DEDFDE"></ItemStyle>
                                                                                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                        <Columns>
                                                                                            <asp:BoundColumn Visible="False" DataField="SensorId" HeaderText='<%$ Resources:dgSetSensors_SensorId %>'>
                                                                                            </asp:BoundColumn>
                                                                                            <asp:TemplateColumn HeaderText='<%$ Resources:dgSetSensors_Enabled %>'>
                                                                                                <ItemTemplate>
                                                                                                    <asp:CheckBox ID="chkSetSensorsCheckBox" Checked='<%# DataBinder.Eval(Container.DataItem, "chkSet") %>'
                                                                                                        runat="server" meta:resourcekey="chkSetSensorsCheckBoxResource1" />
                                                                                                </ItemTemplate>
                                                                                            </asp:TemplateColumn>
                                                                                            <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgSetSensors_Sensors %>'>
                                                                                            </asp:BoundColumn>
                                                                                        </Columns>
                                                                                        <PagerStyle Mode="NumericPages"></PagerStyle>
                                                                                    </asp:DataGrid></td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr height="25">
                                                    <td style="width: 412px" align="center" height="10">
                                                        <table id="tblBoxSetup" style="width: 391px; height: 323px" cellspacing="0" cellpadding="0"
                                                            border="0" runat="server">
                                                            <tr>
                                                                <td>
                                                                    <div id="divsetup" title="Box Setup:" style="border-right: gray thin outset; border-top: gray thin outset;
                                                                        overflow: auto; border-left: gray thin outset; width: 390px; border-bottom: gray thin outset;
                                                                        height: 315px" align="center">
                                                                        <table id="tblSet" style="width: 359px" cellspacing="0" cellpadding="0" border="0"
                                                                            runat="server">
                                                                            <tr>
                                                                                <td class="tableheading" style="width: 71px; height: 3px">
                                                                                    <asp:Label ID="lblBoxSetupTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblBoxSetupTitleResource1" Text="Box Setup:"></asp:Label></td>
                                                                                <td class="formtext" style="height: 20px">
                                                                                </td>
                                                                                <td>
                                                                                </td>
                                                                                <td>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 71px; height: 18px">
                                                                                    <asp:Label ID="lblReportingFrequencyTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblReportingFrequencyTitle2Resource1" Text="Reporting Frequency:"></asp:Label></td>
                                                                                <td class="formtext" style="height: 18px">
                                                                                    <asp:DropDownList ID="cboFreguency" runat="server" CssClass="RegularText" Width="110px"
                                                                                        meta:resourcekey="cboFreguencyResource1">
                                                                                    </asp:DropDownList></td>
                                                                                <td class="formtext" style="height: 18px">
                                                                                    <asp:Label ID="lblSpeedThresholdTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblSpeedThresholdTitle2Resource1" Text="Speed Threshold:"></asp:Label></td>
                                                                                <td style="height: 18px">
                                                                                    <asp:DropDownList ID="cboSpeed" runat="server" CssClass="RegularText" Width="99px"
                                                                                        meta:resourcekey="cboSpeedResource1">
                                                                                    </asp:DropDownList></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 71px; height: 19px">
                                                                                    <asp:Label ID="lblGeoFenceTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblGeoFenceTitle2Resource1" Text="GeoFence:"></asp:Label></td>
                                                                                <td class="formtext" style="height: 3px">
                                                                                    <asp:DropDownList ID="cboGeo" runat="server" CssClass="RegularText" Width="110px"
                                                                                        Height="25px" meta:resourcekey="cboGeoResource1">
                                                                                    </asp:DropDownList></td>
                                                                                <td class="formtext">
                                                                                    <asp:Label ID="lblCommMode2" runat="server" CssClass="formtext" meta:resourcekey="lblCommMode2Resource1" Text="Communication Mode:"></asp:Label></td>
                                                                                <td>
                                                                                    <asp:DropDownList ID="cboCommMode" runat="server" CssClass="RegularText" Width="99px"
                                                                                        Height="25px" DataTextField="CommModeName" DataValueField="CommModeName" meta:resourcekey="cboCommModeResource1">
                                                                                    </asp:DropDownList></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 71px; height: 23px">
                                                                                    <asp:Label ID="lblTracePeriodTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblTracePeriodTitle2Resource1" Text="Trace Period:"></asp:Label></td>
                                                                                <td class="formtext" style="height: 23px">
                                                                                    <asp:DropDownList ID="cboTracePeriodSetup" runat="server" CssClass="RegularText"
                                                                                        Width="110px" AutoPostBack="True" OnSelectedIndexChanged="cboTracePeriodSetup_SelectedIndexChanged"
                                                                                        meta:resourcekey="cboTracePeriodSetupResource1">
                                                                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource90" Text="Disabled"></asp:ListItem>
                                                                                        <asp:ListItem Value="60" meta:resourcekey="ListItemResource91" Text="1 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="120" meta:resourcekey="ListItemResource92" Text="2 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="300" Selected="True" meta:resourcekey="ListItemResource93" Text="5 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="600" meta:resourcekey="ListItemResource94" Text="10 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="900" meta:resourcekey="ListItemResource95" Text="15 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="1800" meta:resourcekey="ListItemResource96" Text="30 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="3600" meta:resourcekey="ListItemResource97" Text="1 hour"></asp:ListItem>
                                                                                        <asp:ListItem Value="43200" meta:resourcekey="ListItemResource98" Text="12 hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="64800" meta:resourcekey="ListItemResource99" Text="18 hours"></asp:ListItem>
                                                                                    </asp:DropDownList></td>
                                                                                <td class="formtext" style="height: 23px">
                                                                                    <asp:Label ID="lblTraceIntervalTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblTraceIntervalTitle2Resource1" Text="Trace Interval:"></asp:Label></td>
                                                                                <td style="height: 23px">
                                                                                    <asp:DropDownList ID="cboTraceIntervalSetup" runat="server" CssClass="RegularText"
                                                                                        Width="99px" meta:resourcekey="cboTraceIntervalSetupResource1">
                                                                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource100" Text="Disabled"></asp:ListItem>
                                                                                        <asp:ListItem Value="15" Selected="True" meta:resourcekey="ListItemResource101" Text="15 sec"></asp:ListItem>
                                                                                        <asp:ListItem Value="30" meta:resourcekey="ListItemResource102" Text="30 sec"></asp:ListItem>
                                                                                        <asp:ListItem Value="60" meta:resourcekey="ListItemResource103" Text="1 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="120" meta:resourcekey="ListItemResource104" Text="2 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="300" meta:resourcekey="ListItemResource105" Text="5 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="600" meta:resourcekey="ListItemResource106" Text="10 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="900" meta:resourcekey="ListItemResource107" Text="15 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="1800" meta:resourcekey="ListItemResource108" Text="30 min"></asp:ListItem>
                                                                                        <asp:ListItem Value="3600" meta:resourcekey="ListItemResource109" Text="1 hour"></asp:ListItem>
                                                                                        <asp:ListItem Value="57600" meta:resourcekey="ListItemResource110" Text="16 hours"></asp:ListItem>
                                                                                    </asp:DropDownList></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 71px; height: 21px" align="left">
                                                                                    <u><strong></strong></u>
                                                                                </td>
                                                                                <td style="height: 21px" align="left">
                                                                                </td>
                                                                                <td align="right">
                                                                                </td>
                                                                                <td align="right">
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <table id="tblRapSet" cellspacing="0" cellpadding="0" width="300" border="0" runat="server">
                                                                            <tr>
                                                                                <td class="formtext" style="font-weight: bold; height: 19px">
                                                                                    <asp:Label ID="lblBoxSetupTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupTitle2Resource1" Text="Box Setup:"></asp:Label></td>
                                                                                <td style="height: 19px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="height: 19px">
                                                                                </td>
                                                                                <td style="height: 5px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="height: 19px">
                                                                                    <asp:Label ID="lblGPSFrequencyTitle" runat="server" CssClass="formtext" meta:resourcekey="lblGPSFrequencyTitleResource1" Text="GPS Frequency:"></asp:Label></td>
                                                                                <td style="height: 19px">
                                                                                    <asp:DropDownList ID="cboGPSFrequency" runat="server" CssClass="RegularText" Width="110px"
                                                                                        meta:resourcekey="cboGPSFrequencyResource1">
                                                                                    </asp:DropDownList></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="height: 12px">
                                                                                    <asp:Label ID="lblGPSFrequencyStationaryTitle" runat="server" CssClass="formtext"
                                                                                        meta:resourcekey="lblGPSFrequencyStationaryTitleResource1" Text="GPS Frequency Stationary:"></asp:Label></td>
                                                                                <td style="height: 12px">
                                                                                    <asp:DropDownList ID="cboGPSFreqStat" runat="server" CssClass="RegularText" Width="110px"
                                                                                        meta:resourcekey="cboGPSFreqStatResource1">
                                                                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource111" Text="Disable"></asp:ListItem>
                                                                                        <asp:ListItem Value="5" meta:resourcekey="ListItemResource112" Text="5 Min"></asp:ListItem>
                                                                                        <asp:ListItem Value="15" meta:resourcekey="ListItemResource113" Text="15 Min"></asp:ListItem>
                                                                                        <asp:ListItem Value="30" meta:resourcekey="ListItemResource114" Text="30 Min"></asp:ListItem>
                                                                                        <asp:ListItem Value="60" meta:resourcekey="ListItemResource115" Text="1 Hour"></asp:ListItem>
                                                                                        <asp:ListItem Value="120" meta:resourcekey="ListItemResource116" Text="2 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="180" meta:resourcekey="ListItemResource117" Text="3 Hours"></asp:ListItem>
                                                                                        <asp:ListItem Value="240" meta:resourcekey="ListItemResource118" Text="4 Hours"></asp:ListItem>
                                                                                    </asp:DropDownList></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext">
                                                                                    <asp:Label ID="lblDistanceIntervalTitle" runat="server" CssClass="formtext" meta:resourcekey="lblDistanceIntervalTitleResource1" Text="Distance Interval:"></asp:Label></td>
                                                                                <td>
                                                                                    <asp:DropDownList ID="cboDistInterval" runat="server" CssClass="RegularText" Width="110px"
                                                                                        meta:resourcekey="cboDistIntervalResource1">
                                                                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource119" Text="Disable"></asp:ListItem>
                                                                                        <asp:ListItem Value="500" meta:resourcekey="ListItemResource120" Text="500 m"></asp:ListItem>
                                                                                        <asp:ListItem Value="1000" meta:resourcekey="ListItemResource121" Text="1 km"></asp:ListItem>
                                                                                        <asp:ListItem Value="3000" meta:resourcekey="ListItemResource122" Text="3 km"></asp:ListItem>
                                                                                        <asp:ListItem Value="5000" meta:resourcekey="ListItemResource123" Text="5 km"></asp:ListItem>
                                                                                        <asp:ListItem Value="10000" meta:resourcekey="ListItemResource124" Text="10 km"></asp:ListItem>
                                                                                        <asp:ListItem Value="15000" meta:resourcekey="ListItemResource125" Text="15 km"></asp:ListItem>
                                                                                        <asp:ListItem Value="20000" meta:resourcekey="ListItemResource126" Text="20 km"></asp:ListItem>
                                                                                        <asp:ListItem Value="25000" meta:resourcekey="ListItemResource127" Text="25 km"></asp:ListItem>
                                                                                    </asp:DropDownList></td>
                                                                            </tr>
                                                                        </table>
                                                                       <table runat=server class="formtext" id="tblViolations" width="359px"  >
                                                                          <tr>
                                                                             <td style="width: 100px">
                                                                                <asp:Label ID="lblsetupHarshAcceleration" runat="server" Text="Harsh Acceleration  (0-100):" Width="214px" meta:resourcekey="lblsetupHarshAccelerationResource1"></asp:Label></td>
                                                                             <td style="width: 100px">
                                                                                <asp:TextBox ID="txtsetupHarshAcceleration" runat="server" Width="94px" meta:resourcekey="txtsetupHarshAccelerationResource1"></asp:TextBox></td>
                                                                          </tr>
                                                                          <tr>
                                                                             <td style="width: 100px; height: 26px;">
                                                                                <asp:Label ID="lblsetupHarshBraking" runat="server" Text="Harsh Braking  (0-100):" Width="185px" meta:resourcekey="lblsetupHarshBrakingResource1"></asp:Label></td>
                                                                             <td style="width: 100px; height: 26px;">
                                                                                <asp:TextBox ID="txtsetupHarshBraking" runat="server" Width="94px" meta:resourcekey="txtsetupHarshBrakingResource1"></asp:TextBox></td>
                                                                          </tr>
                                                                          <tr>
                                                                             <td style="width: 100px">
                                                                                <asp:Label ID="lblsetupExtremeAcceleration" runat="server" Text="Extreme Acceleration  (0-100):" Width="186px" meta:resourcekey="lblsetupExtremeAccelerationResource1"></asp:Label></td>
                                                                             <td style="width: 100px">
                                                                                <asp:TextBox ID="txtsetupExtremeAcceleration" runat="server" Width="94px" meta:resourcekey="txtsetupExtremeAccelerationResource1"></asp:TextBox></td>
                                                                          </tr>
                                                                          <tr>
                                                                             <td style="width: 100px">
                                                                                <asp:Label ID="lblsetupExtremeBraking" runat="server" Text="Extreme Braking   (0-100):   " Width="162px" meta:resourcekey="lblsetupExtremeBrakingResource1"></asp:Label></td>
                                                                             <td style="width: 100px">
                                                                                <asp:TextBox ID="txtsetupExtremeBraking" runat="server" CssClass="formtext" Width="94px" meta:resourcekey="txtsetupExtremeBrakingResource1"></asp:TextBox></td>
                                                                          </tr>
                                                                       </table>
                                                                        <table id="Table10" cellspacing="0" cellpadding="0" width="300" border="0">
                                                                            <tr>
                                                                                <td height="25">
                                                                                    <asp:Button ID="cmdUnselect" runat="server" CssClass="combutton" Text="Deselect All"
                                                                                        Width="92px" OnClick="cmdUnselect_Click" meta:resourcekey="cmdUnselectResource1">
                                                                                    </asp:Button></td>
                                                                                <td height="25">
                                                                                    <asp:Button ID="cmdSelectAll" runat="server" CssClass="combutton" Text="Select All"
                                                                                        Width="92px" OnClick="cmdSelectAll_Click" meta:resourcekey="cmdSelectAllResource1">
                                                                                    </asp:Button></td>
                                                                            </tr>
                                                                        </table>
                                                                        <asp:DataGrid ID="dgBoxSetupSensors" runat="server" Width="358px" AutoGenerateColumns="False"
                                                                            DataKeyField="SensorId" PageSize="4" meta:resourcekey="dgBoxSetupSensorsResource1">
                                                                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                            <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                            <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black"
                                                                                BackColor="#DEDFDE"></ItemStyle>
                                                                            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                            <Columns>
                                                                                <asp:BoundColumn Visible="False" DataField="SensorId" HeaderText='<%$ Resources:dgBoxSetupSensors_SensorId %>'>
                                                                                </asp:BoundColumn>
                                                                                <asp:TemplateColumn HeaderText='<%$ Resources:dgBoxSetupSensors_Enabled %>'>
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkCheckBox" Checked='<%# DataBinder.Eval(Container.DataItem, "chkSet") %>'
                                                                                            runat="server" meta:resourcekey="chkCheckBoxResource1" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateColumn>
                                                                                <asp:BoundColumn ReadOnly="True" DataField="SensorName" HeaderText='<%$ Resources:dgBoxSetupSensors_Sensors %>'>
                                                                                </asp:BoundColumn>
                                                                                <asp:TemplateColumn HeaderText='<%$ Resources:dgBoxSetupSensors_Trace %>'>
                                                                                    <ItemStyle Wrap="False"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblTraceSetup" Text='<%# DataBinder.Eval(Container.DataItem,"TraceStateName") %>'
                                                                                            runat="server" meta:resourcekey="lblTraceSetupResource1"></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <EditItemTemplate>
                                                                                        <asp:DropDownList ID="cboTraceSetup" DataSource='<%# dsTrace %>' DataValueField="TraceStateId"
                                                                                            DataTextField="TraceStateName" SelectedIndex='<%# GetTrace(Convert.ToInt16(DataBinder.Eval(Container.DataItem,"TraceStateId"))) %>'
                                                                                            runat="server" meta:resourcekey="cboTraceSetupResource1">
                                                                                        </asp:DropDownList>
                                                                                    </EditItemTemplate>
                                                                                </asp:TemplateColumn>
                                                                                <asp:EditCommandColumn UpdateText="&lt;img src=../images/ok.gif border=0&gt;" CancelText="&lt;img src=../images/cancel.gif border=0&gt;"
                                                                                    EditText="&lt;img src=../images/edit.gif border=0&gt;" meta:resourcekey="EditCommandColumnResource2">
                                                                                </asp:EditCommandColumn>
                                                                            </Columns>
                                                                            <PagerStyle Mode="NumericPages"></PagerStyle>
                                                                        </asp:DataGrid></div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblBoxSetupInfo" style="border-top-width: thin; border-left-width: thin;
                                                            border-left-color: gray; border-bottom-width: thin; border-bottom-color: gray;
                                                            border-top-color: gray; border-right-width: thin; border-right-color: gray"
                                                            cellspacing="0" cellpadding="0" width="300" border="0" runat="server">
                                                            <tr>
                                                                <td>
                                                                    <div id="DivBoxSetupInfo" title="Box Setup:" style="border-right: gray thin outset;
                                                                        border-top: gray thin outset; overflow: auto; border-left: gray thin outset;
                                                                        width: 390px; border-bottom: gray thin outset; height: 318px" align="center">
                                                                        <table id="tblGetBoxSet" style="width: 369px; height: 115px" cellspacing="0" cellpadding="0"
                                                                            border="0" runat="server">
                                                                            <tr>
                                                                                <td class="tableheading" style="width: 208px; height: 3px">
                                                                                </td>
                                                                                <td style="height: 20px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="tableheading" style="width: 208px; height: 3px">
                                                                                    <asp:Label ID="lblBoxSetupInfoTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblBoxSetupInfoTitleResource1" Text="Box Setup Information:"></asp:Label></td>
                                                                                <td style="height: 3px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblGPSFrequencyTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblGPSFrequencyTitle2Resource1" Text="GPS Frequency:"></asp:Label>
                                                                                </td>
                                                                                <td style="height: 19px">
                                                                                    <asp:Label ID="lblBoxSetupGPSFreg" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupGPSFregResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblSpeedThresholdTitle3" runat="server" CssClass="formtext" meta:resourcekey="lblSpeedThresholdTitle3Resource1" Text="Speed Threshold:"></asp:Label></td>
                                                                                <td style="height: 3px">
                                                                                    <asp:Label ID="lblBoxSetupSpeed" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupSpeedResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblGeoFenceTitle3" runat="server" CssClass="formtext" meta:resourcekey="lblGeoFenceTitle3Resource1" Text="GeoFence:"></asp:Label></td>
                                                                                <td>
                                                                                    <asp:Label ID="lblBoxSetupGeo" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupGeoResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px" align="left">
                                                                                    <asp:Label ID="lblTracePeriodTitle3" runat="server" CssClass="formtext" meta:resourcekey="lblTracePeriodTitle3Resource1" Text="Trace Period:"></asp:Label></td>
                                                                                <td style="width: 208px; height: 19px" align="left">
                                                                                    <asp:Label ID="lblTracePeriod" runat="server" CssClass="formtext" meta:resourcekey="lblTracePeriodResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px" align="left">
                                                                                    <asp:Label ID="lblTraceIntervalTitle3" runat="server" CssClass="formtext" meta:resourcekey="lblTraceIntervalTitle3Resource1" Text="Trace Interval:"></asp:Label></td>
                                                                                <td style="width: 208px; height: 19px" align="left">
                                                                                    <asp:Label ID="lblTraceInterval" runat="server" CssClass="formtext" meta:resourcekey="lblTraceIntervalResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left" class="formtext" style="width: 208px">
                                                                                    <asp:Label ID="lblIdleTitle" runat="server" CssClass="formtext" meta:resourcekey="lblIdleTitleResource1" Text="Idle:"></asp:Label></td>
                                                                                <td align="left" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblIdle" runat="server" CssClass="formtext" meta:resourcekey="lblIdleResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px" align="left">
                                                                                    <asp:Label ID="lblCommModeTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblCommModeTitle2Resource1" Text="Communication Mode:"></asp:Label></td>
                                                                                <td style="width: 208px; height: 19px" align="left">
                                                                                    <asp:Label ID="lblCommMode" runat="server" CssClass="formtext" meta:resourcekey="lblCommModeResource1"></asp:Label></td>
                                                                            </tr>
                                                                        </table>
                                                                        <p>
                                                                            <table id="tblGetBoxSetRAP" style="width: 369px" cellspacing="0" cellpadding="0"
                                                                                width="300" border="0" runat="server">
                                                                                <tr>
                                                                                    <td class="formtext" style="font-weight: bold; height: 16px">
                                                                                        <asp:Label ID="lblBoxSetupInfoTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupInfoTitle2Resource1" Text="Box Setup Information:"></asp:Label></td>
                                                                                    <td style="height: 16px">
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td class="formtext" style="font-weight: bold; height: 16px">
                                                                                    </td>
                                                                                    <td style="height: 5px">
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td class="formtext" style="height: 13px">
                                                                                        <asp:Label ID="lblGPSFrequencyTitle3" runat="server" CssClass="formtext" meta:resourcekey="lblGPSFrequencyTitle3Resource1" Text="GPS Frequency:"></asp:Label>
                                                                                    </td>
                                                                                    <td style="height: 13px">
                                                                                        <asp:Label ID="lblBoxSetupGPSFreqRAP" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupGPSFreqRAPResource1"></asp:Label></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td class="formtext">
                                                                                        <asp:Label ID="lblGPSFrequencyStationaryTitle2" runat="server" CssClass="formtext"
                                                                                            meta:resourcekey="lblGPSFrequencyStationaryTitle2Resource1" Text="GPS Frequency Stationary:"></asp:Label></td>
                                                                                    <td>
                                                                                        <asp:Label ID="lblBoxSetupGPSFregStatRAP" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupGPSFregStatRAPResource1"></asp:Label></td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td class="formtext">
                                                                                        <asp:Label ID="lblDistanceIntervalTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblDistanceIntervalTitle2Resource1" Text="Distance Interval (m):"></asp:Label></td>
                                                                                    <td>
                                                                                        <asp:Label ID="lblBoxSetupDistIntRAP" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupDistIntRAPResource1"></asp:Label></td>
                                                                                </tr>
                                                                            </table>
                                                                        </p>
                                                                        <p><table id="tblBoxSetupViolations" style="width: 369px" cellspacing="0" cellpadding="0"
                                                                                width="300" border="0" runat="server">
                                                                           
                                                                           <tr>
                                                                              <td class="formtext">
                                                                                 <asp:Label ID="lblHarshAccelerationLabel" runat="server" CssClass="formtext" 
                                                                                    Text="Harsh Acceleration:" meta:resourcekey="lblHarshAccelerationLabelResource1"></asp:Label>
                                                                              </td>
                                                                              <td align="left">
                                                                                 <asp:Label ID="lblHarshAccelerationText" runat="server" CssClass="formtext" meta:resourcekey="lblHarshAccelerationTextResource1" ></asp:Label></td>
                                                                           </tr>
                                                                           <tr>
                                                                              <td class="formtext">
                                                                                 <asp:Label ID="lblHarshBrakingLabel" runat="server" CssClass="formtext" 
                                                                                    Text="Harsh Braking :" meta:resourcekey="lblHarshBrakingLabelResource1"></asp:Label></td>
                                                                              <td style="height: 13px" align="left">
                                                                                 <asp:Label ID="lblHarshBrakingText" runat="server" CssClass="formtext" meta:resourcekey="lblHarshBrakingTextResource1" ></asp:Label></td>
                                                                           </tr>
                                                                           <tr>
                                                                              <td class="formtext">
                                                                                 <asp:Label ID="lblExtremeAccelerationLabel" runat="server" CssClass="formtext" 
                                                                                    Text="Extreme Acceleration:" meta:resourcekey="lblExtremeAccelerationLabelResource1"></asp:Label></td>
                                                                              <td style="height: 13px" align="left">
                                                                                 <asp:Label ID="lblExtremeAccelerationText" runat="server" CssClass="formtext" meta:resourcekey="lblExtremeAccelerationTextResource1"></asp:Label></td>
                                                                           </tr>
                                                                           <tr>
                                                                              <td class="formtext">
                                                                                 <asp:Label ID="lblExtremeBrakingLabel" runat="server" CssClass="formtext" 
                                                                                    Text="Extreme Braking:" meta:resourcekey="lblExtremeBrakingLabelResource1"></asp:Label></td>
                                                                              <td align="left">
                                                                                 <asp:Label ID="lblExtremeBrakingText" runat="server" CssClass="formtext" meta:resourcekey="lblExtremeBrakingTextResource1"></asp:Label></td>
                                                                           </tr>
                                                                        </table>
                                                                           &nbsp;
                                                                            <asp:DataGrid ID="dgBoxSetupSensorsInfo" runat="server" Width="344px" AutoGenerateColumns="False"
                                                                                DataKeyField="SensorId" PageSize="4" meta:resourcekey="dgBoxSetupSensorsInfoResource1">
                                                                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                                <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black"
                                                                                    BackColor="#DEDFDE"></ItemStyle>
                                                                                <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                                <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                <Columns>
                                                                                    <asp:BoundColumn Visible="False" DataField="SensorId" HeaderText='<%$ Resources:dgBoxSetupSensorsInfo_SensorId %>'>
                                                                                    </asp:BoundColumn>
                                                                                    <asp:TemplateColumn Visible="False" HeaderText='<%$ Resources:dgBoxSetupSensorsInfo_Set %>'>
                                                                                        <ItemTemplate>
                                                                                            <asp:CheckBox ID="chkCheckBoxSetupAction" Checked='<%# DataBinder.Eval(Container.DataItem, "SensorStatus") %>'
                                                                                                runat="server" meta:resourcekey="chkCheckBoxSetupActionResource1" />
                                                                                        </ItemTemplate>
                                                                                    </asp:TemplateColumn>
                                                                                    <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgBoxSetupSensorsInfo_Sensors %>'>
                                                                                    </asp:BoundColumn>
                                                                                    <asp:BoundColumn DataField="SensorAction" HeaderText='<%$ Resources:dgBoxSetupSensorsInfo_SensorState %>'>
                                                                                    </asp:BoundColumn>
                                                                                </Columns>
                                                                                <PagerStyle Mode="NumericPages"></PagerStyle>
                                                                            </asp:DataGrid></p>
                                                                        <p>
                                                                            <asp:DataGrid ID="dgTraceBoxSetup" runat="server" Width="346px" AutoGenerateColumns="False"
                                                                                PageSize="4" meta:resourcekey="dgTraceBoxSetupResource1">
                                                                                <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                                <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black"
                                                                                    BackColor="#DEDFDE"></ItemStyle>
                                                                                <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                                <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                <Columns>
                                                                                    <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgTraceBoxSetup_Sensor %>'>
                                                                                    </asp:BoundColumn>
                                                                                    <asp:BoundColumn DataField="TraceState" HeaderText='<%$ Resources:dgTraceBoxSetup_TraceState %>'>
                                                                                    </asp:BoundColumn>
                                                                                </Columns>
                                                                                <PagerStyle Mode="NumericPages"></PagerStyle>
                                                                            </asp:DataGrid></p>
                                                                        <p>
                                                                            <asp:Button ID="cmdCloseSetupInfo" runat="server" CssClass="combutton" Text="Close Setup Info"
                                                                                Width="120px" OnClick="cmdCloseSetupInfo_Click" meta:resourcekey="cmdCloseSetupInfoResource1">
                                                                            </asp:Button></p>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblBoxStatusInfo" cellspacing="0" cellpadding="0" width="300" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td>
                                                                    <div id="divBoxStatusInfo" title="Box Setup:" 
                                                                        style="border: thin outset gray; overflow: auto; width: 390px; height: 393px" 
                                                                        align="center">
                                                                        <table id="Table3" style="width: 359px; height: 192px" cellspacing="0" cellpadding="0"
                                                                            border="0" runat="server">
                                                                            <tr>
                                                                                <td class="tableheading" style="width: 208px; height: 3px">
                                                                                </td>
                                                                                <td style="height: 20px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="tableheading" style="width: 208px; height: 3px">
                                                                                    <asp:Label ID="lblBoxStatusInfoTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblBoxStatusInfoTitleResource1" Text="Box Status Information:"></asp:Label></td>
                                                                                <td style="height: 3px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblArmedTitle" runat="server" CssClass="formtext" meta:resourcekey="lblArmedTitleResource1" Text="Armed:"></asp:Label></td>
                                                                                <td style="height: 3px">
                                                                                    <asp:Label ID="lblBoxStatusArmed" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusArmedResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblMainBatteryTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMainBatteryTitleResource1" Text="Main Battery:"></asp:Label></td>
                                                                                <td style="height: 3px">
                                                                                    <asp:Label ID="lblBoxStatusMainBattery" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusMainBatteryResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblBoxStatusBackupBatteryLabel" runat="server" meta:resourcekey="lblBoxStatusBackupBatteryLabelResource1" Text="Backup Battery:"></asp:Label></td>
                                                                                <td style="height: 3px">
                                                                                    <asp:Label ID="lblBoxStatusBackupBattery" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusBackupBatteryResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblSNTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSNTitleResource1" Text="S.N:"></asp:Label></td>
                                                                                <td style="height: 4px">
                                                                                    <asp:Label ID="lblBoxStatusSN" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusSNResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 5px">
                                                                                    <asp:Label ID="lblFirmwareVersionTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFirmwareVersionTitleResource1" Text="Firmware version:"></asp:Label></td>
                                                                                <td style="height: 19px">
                                                                                    <asp:Label ID="lblBoxStatusFirmware" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusFirmwareResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblMemoryUsageTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMemoryUsageTitleResource1" Text="Memory Usage:"></asp:Label></td>
                                                                                <td style="height: 19px">
                                                                                    <asp:Label ID="lblBoxStatusWaypoint" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusWaypointResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblMDTMessages" runat="server" Text="MDT messages in memory:"></asp:Label></td>
                                                                                <td style="height: 19px">
                                                                                    <asp:Label ID="lblMDTMessagesValue" runat="server" CssClass="formtext"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblSIMESNTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSIMESNTitleResource1" Text="SIM/ESN:"></asp:Label></td>
                                                                                <td style="height: 19px">
                                                                                    <asp:Label ID="lblStatusSIM" runat="server" CssClass="formtext" meta:resourcekey="lblStatusSIMResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblCellTitle" runat="server" CssClass="formtext" meta:resourcekey="lblCellTitleResource1" Text="Cell:"></asp:Label></td>
                                                                                <td style="height: 19px">
                                                                                    <asp:Label ID="lblStatusCell" runat="server" CssClass="formtext" meta:resourcekey="lblStatusCellResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 19px">
                                                                                    <asp:Label ID="lblPRLLabel" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblPRLLabelResource1" Text="PRL:"></asp:Label></td>
                                                                                <td style="height: 19px">
                                                                                    <asp:Label ID="lblPRL" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblPRLResource1"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px" align="left">
                                                                                </td>
                                                                                <td align="right">
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <asp:DataGrid ID="dgBoxStatusInfo" runat="server" Width="321px" AutoGenerateColumns="False"
                                                                            DataKeyField="SensorId" PageSize="4" meta:resourcekey="dgBoxStatusInfoResource1">
                                                                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                            <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                            <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black"
                                                                                BackColor="#DEDFDE"></ItemStyle>
                                                                            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                            <Columns>
                                                                                <asp:BoundColumn Visible="False" DataField="SensorId" HeaderText='<%$ Resources:dgBoxStatusInfo_SensorId %>'>
                                                                                </asp:BoundColumn>
                                                                                <asp:TemplateColumn Visible="False" HeaderText='<%$ Resources:dgBoxStatusInfo_Set %>'>
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox ID="chkCheckBoxAction" Enabled="False" Checked='<%# DataBinder.Eval(Container.DataItem, "SensorStatus") %>'
                                                                                            runat="server" meta:resourcekey="chkCheckBoxActionResource1" />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateColumn>
                                                                                <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgBoxStatusInfo_Sensors %>'>
                                                                                </asp:BoundColumn>
                                                                                <asp:BoundColumn DataField="SensorAction" HeaderText='<%$ Resources:dgBoxStatusInfo_SensorStatus %>'>
                                                                                </asp:BoundColumn>
                                                                            </Columns>
                                                                            <PagerStyle Mode="NumericPages"></PagerStyle>
                                                                        </asp:DataGrid><asp:Button ID="cmdCloseStatusInfo" runat="server" CssClass="combutton"
                                                                            Text="Close Status Info" Width="123px" OnClick="cmdCloseStatusInfo_Click" meta:resourcekey="cmdCloseStatusInfoResource1">
                                                                        </asp:Button></div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <table id="tblBoxGeozones" style="width: 391px; height: 226px" cellspacing="0" cellpadding="0"
                                                            width="391" border="0" runat="server">
                                                            <tr>
                                                                <td>
                                                                    <div id="Div1" title="Box Setup:" style="border-right: gray thin outset; border-top: gray thin outset;
                                                                        overflow: auto; border-left: gray thin outset; width: 390px; border-bottom: gray thin outset;
                                                                        height: 240px" align="center">
                                                                        <table id="Table9" style="width: 359px; height: 41px" cellspacing="0" cellpadding="0"
                                                                            border="0" runat="server">
                                                                            <tr>
                                                                                <td class="tableheading" style="width: 208px; height: 3px">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 3px">
                                                                                    <asp:Label ID="lblAssignedGeozonesTitle" runat="server" CssClass="formtext" meta:resourcekey="lblAssignedGeozonesTitleResource1" Text="Assigned GeoZones:"></asp:Label></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px" align="left">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px" align="center">
                                                                                    <asp:DataGrid ID="dgGeoZone" runat="server" Width="321px" AutoGenerateColumns="False"
                                                                                        DataKeyField="GeoZoneId" PageSize="4" meta:resourcekey="dgGeoZoneResource1">
                                                                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                        <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                                        <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black"
                                                                                            BackColor="#DEDFDE"></ItemStyle>
                                                                                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                        <Columns>
                                                                                            <asp:BoundColumn DataField="Geozonename" HeaderText='<%$ Resources:dgGeoZone_GeoZone %>'>
                                                                                            </asp:BoundColumn>
                                                                                        </Columns>
                                                                                        <PagerStyle Mode="NumericPages"></PagerStyle>
                                                                                    </asp:DataGrid></td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="width: 208px; height: 20px" align="left">
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formtext" style="height: 20px" align="center">
                                                                                    <asp:Button ID="cmdCloseGeoZoneInfo" runat="server" CssClass="combutton" Text="Close Geozone Info"
                                                                                        Width="138px" OnClick="cmdCloseGeoZoneInfo_Click" meta:resourcekey="cmdCloseGeoZoneInfoResource1">
                                                                                    </asp:Button></td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                      
                                                    <td style="width: 412px; height: 30px" align="center">
                                                        <asp:Button ID="cmdRefresh" runat="server" CssClass="combutton" Text="Refresh" Width="97px"
                                                            OnClick="cmdRefresh_Click" meta:resourcekey="cmdRefreshResource1"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;
                                                        <asp:Button runat="server" CssClass="combutton" ID="cmdClose" Width="97px" OnClientClick="WinClose()"
                                                            Text="Close" meta:resourcekey="cmdCloseResource1" /><%--<input class="combutton" id="cmdClose" style="width: 97px" onclick="top.close()"
                                                                type="button" value="Close" name="cmdClose">--%></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        &nbsp;
    </form>
</body>
</html>

