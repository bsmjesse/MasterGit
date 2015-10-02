<%@ Page Language="c#" Inherits="SentinelFM.frmSensorInfoNew" CodeFile="frmSensorInfoNew.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Sensors-Command-Vehicle Info</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
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
                                    CausesValidation="False" OnClick="cmdSettings_Click" Visible="False" >
                                </asp:Button></td>
                                
                            <td>
                                <asp:Button ID="cmdReefer" runat="server" CssClass="confbutton" Text="Reefer"
                                    CausesValidation="False" meta:resourcekey="cmdReeferResource1" OnClick="cmdReefer_Click" Visible="False">
                                </asp:Button>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" style="width: 413px; height: 284px" cellspacing="0" cellpadding="0"
                        width="413" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" style="border-right: gray 2px solid; border-top: gray 2px solid;
                                    border-left: gray 2px solid; width: 421px; border-bottom: gray 2px solid;
                                    height: 265px" width="421" border="0">
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
                                                            ID="lblVehicleId" runat="server" CssClass="Regulartext" Visible="False" meta:resourcekey="lblVehicleIdResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td class="formtext" style="width: 412px; height: 2px" align="left">
                                                        <asp:Label ID="lblNotes" runat="server" CssClass="RegularText" meta:resourcekey="lblNotesResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td align="center" style="width: 412px">
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
                                                                <asp:CheckBox ID="chkSendToFleet" runat="server" Text="Send to selected Fleet: " TextAlign="Left" meta:resourcekey="chkSendToFleetResource1" /></td>
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
                                                        
                                                        <asp:PlaceHolder ID="PlaceHolderFields" runat="server"></asp:PlaceHolder>

                                                        
                                                        <table id="tblFuelConfiguration" style="width: 384px" cellspacing="0" cellpadding="0" width="384"
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
                                                      
                                                      
                                                      <table id="tblEEPROMSettings" style="width: 384px" cellspacing="0" cellpadding="0"
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
                                                       <table class="formtext" id="tblDeleteGeoZones" cellspacing="0" cellpadding="0" width="100%"
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
                                                       
                                                       
                                                        <table id="tblBoxStatusInfo" cellspacing="0" cellpadding="0" width="300" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td>
                                                                    <div id="divBoxStatusInfo" title="Box Setup:" style="border-right: gray thin outset;
                                                                        border-top: gray thin outset; overflow: auto; border-left: gray thin outset;
                                                                        width: 390px; border-bottom: gray thin outset; height: 322px" align="center">
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
                                                        <asp:Button runat="server" CssClass="combutton" ID="cmdClose" Width="97px" OnClientClick="top.close()"
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
