<%@ Page Language="C#" CodeFile="frmReefer.aspx.cs" Inherits="SentinelFM.frmReefer" Culture="en-US" UICulture="auto" %>
<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Reefer</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="formReefer" runat="server" method="post">
        <div id="tblReefer" runat="server" style="position: relative; top:0px;">
            <table id="tblCommands" style="left: 5px; width: 100%; position: relative; top: 0px;" cellspacing="0" cellpadding="0" width="423" border="0">
                <tr>
                    <td style="width: auto">
                        <table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td>
                                    <asp:Button ID="cmdSensorCommands" runat="server" CssClass="confbutton" Text="Commands"
                                        CausesValidation="False" meta:resourcekey="cmdSensorCommandsResource1" OnClick="cmdSensorCommands_Click"></asp:Button></td>
                                <td>
                                    <asp:Button ID="cmdVehicleInfo" runat="server" CssClass="confbutton" Text="Vehicle Info"
                                        CausesValidation="False" meta:resourcekey="cmdVehicleInfoResource1" OnClick="cmdVehicleInfo_Click">
                                    </asp:Button>
                                </td>
                                <td>
                                    <asp:Button ID="cmdReefer" runat="server" CssClass="selectedbutton" Text="Reefer"
                                        CausesValidation="False" meta:resourcekey="cmdReeferResource1">
                                    </asp:Button>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 100%">
                        <table id="tblBody" runat="server" style="position:relative; top: 0px; width: 460px;" cellspacing="0" cellpadding="0">
                            <tr>
                                <td style="height: 100%">
                                    <table cellpadding="0" style="border: gray 2px solid;"> 
                                        <tr>
                                            <td colspan="2">
                                                <fieldset id="pnlReeferTemperature" runat="server" style="border: ridge 1px gray; width: 415px; height: auto; padding: 0px 5px 5px 5px;">
                                                    <legend>
                                                        <asp:Label ID="lblTempCaption" runat="server" CssClass="headinginstructions" Text="Temperature Thresholds (°C)"></asp:Label>
                                                    </legend>
                                                    <table style="width: 400px; border: ridge 1px gray; margin: 5px 0px 5px 0px">
                                                        <tr>
                                                            <td style="width:59px" rowspan="2">
                                                                <asp:Label ID="lblReeferProduct" runat="server" CssClass="formtext" Text="Product Lookup:"></asp:Label>
                                                            </td>
                                                            <td style="width:190px"></td>
                                                            <td style="width:60px; text-align: center;">
                                                                <asp:Label ID="lblCaptionLower" runat="server" CssClass="formtext" Text="Lower"></asp:Label>
                                                            </td>
                                                            <td style="width:60px; text-align: center;">
                                                                <asp:Label ID="lblCaptionUpper" runat="server" CssClass="formtext" Text="Upper"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr style="height: 21px;">
                                                            <td style="width:190px; text-align: left;">
                                                                <asp:DropDownList ID="ddlReeferProducts" AutoPostBack="true" runat="server" Width="180px" CssClass="RegularText" OnSelectedIndexChanged="ddlReeferProducts_SelectedIndexChanged">
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td style="width:60px; text-align: center;">
                                                                <asp:Label ID="lblLower" runat="server" CssClass="formtext" Width="30px"></asp:Label>
                                                            </td>
                                                            <td style="width:60px; text-align: center;">
                                                                <asp:Label ID="lblUpper" runat="server" CssClass="formtext" Width="30px"></asp:Label>
                                                            </td>    
                                                        </tr>
                                                    </table>
                                                    <asp:GridView ID="gvReeferTempSensors" runat="server" Width="400px" AutoGenerateColumns="False" Font-Size="Smaller"
                                                       BackColor="White" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1" GridLines="None" 
                                                       OnRowEditing="gvTempSensors_RowEditing" OnRowUpdating="gvTempSensors_RowUpdating" 
                                                       OnRowCancelingEdit="gvTempSensors_RowCancelingEdit" DataKeyNames="SensorId" OnSelectedIndexChanged="gvTempSensors_SelectedIndexChanged">
                                                       <AlternatingRowStyle BackColor="WhiteSmoke" CssClass="formtext" Height="11px" />
                                                       <RowStyle BackColor="#DEDFDE" CssClass="formtext" Height="11px" />
                                                       <EditRowStyle BackColor="White" CssClass="formtext" Height="11px" />
                                                       <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                       <EmptyDataRowStyle BackColor="White" CssClass="errortext" Font-Bold="True" />
                                                       <Columns>
                                                        <asp:BoundField DataField="SensorId" Visible="False" />
                                                        <asp:BoundField HeaderText="Zone" DataField="SensorName" ItemStyle-Width="150px" />
                                                        <asp:TemplateField HeaderText="Active">
                                                            <ItemStyle HorizontalAlign="center"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:CheckBox AutoPostBack="true" runat="server" ID="chkActive" Checked="false" OnCheckedChanged="chkActive_CheckedChanged" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Lower">
                                                            <ItemStyle Width="50px" HorizontalAlign="center"></ItemStyle>
						                                    <ItemTemplate>
						                                        <asp:TextBox ID="txtReeferLower" Width="30px" Runat="server" Enabled="false" CssClass="formtext"></asp:TextBox>
						                                    </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Current">
                                                            <ItemStyle Width="50px" HorizontalAlign="center"></ItemStyle>
						                                    <ItemTemplate>
						                                        <asp:Label ID="txtReeferCurrent" Width="30px" Runat="server" BorderColor="ButtonShadow" BorderWidth="1px" BorderStyle="Outset"></asp:Label>
						                                    </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Upper">
                                                            <ItemStyle Width="50px" HorizontalAlign="center"></ItemStyle>
						                                    <ItemTemplate>
						                                        <asp:TextBox ID="txtReeferUpper" Width="30px" Runat="server" Enabled="false" CssClass="formtext"></asp:TextBox>
						                                    </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Interval">
                                                            <ItemStyle Width="50px"></ItemStyle>
						                                    <ItemTemplate>
						                                        <asp:DropDownList ID="ddlTempInterval" runat="server" Width="70px" CssClass="RegularText" Enabled="false">
						                                            <asp:ListItem Value="0">Not Set</asp:ListItem>
						                                            <asp:ListItem Value="30">30 Sec</asp:ListItem>
						                                            <asp:ListItem Value="60">1 Min</asp:ListItem>
						                                            <asp:ListItem Value="120">2 Min</asp:ListItem>
                                                                    <asp:ListItem Value="300">5 Min</asp:ListItem>
                                                                    <asp:ListItem Value="600">10 Min</asp:ListItem>
                                                                    <asp:ListItem Value="900">15 Min</asp:ListItem>
                                                                    <asp:ListItem Value="1800">30 Min</asp:ListItem>
                                                                    <asp:ListItem Value="3600">1 Hour</asp:ListItem>
                                                                    <asp:ListItem Value="7200">2 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="14400">4 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="21600">6 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="28800">8 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="36000">10 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="43200">12 Hours</asp:ListItem>
                                                                </asp:DropDownList>
						                                    </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:CommandField ButtonType="Button" SelectText="Paste" ShowSelectButton="True">
                                                            <ControlStyle CssClass="combutton" Width="45px" />
                                                        </asp:CommandField>
                                                       </Columns>
                                                       <EmptyDataTemplate>
                                                           <asp:Label ID="lblEmpty" runat="server" Text="Empty sensor data"></asp:Label>
                                                       </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </fieldset>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <fieldset id="pnlReeferFuel" runat="server" style="border: ridge 1px gray; width: 415px; height: auto; padding: 0px 5px 5px 5px;">
                                                   <legend>
                                                       <asp:Label ID="lblFuelCaption" runat="server" CssClass="headinginstructions" Text="Fuel Sensor"></asp:Label>
                                                   </legend>
                                                   <asp:GridView ID="gvFuelSensors" runat="server" AutoGenerateColumns="False" Style="margin-top: 5px"
                                                       BackColor="White" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1" GridLines="None">
                                                       <AlternatingRowStyle BackColor="WhiteSmoke" CssClass="formtext" Height="11px" />
                                                       <RowStyle BackColor="#DEDFDE" CssClass="formtext" Height="11px" />
                                                       <EditRowStyle BackColor="White" CssClass="formtext" />
                                                       <HeaderStyle BackColor="gray" CssClass="formtext" Font-Bold="True" ForeColor="White" />
                                                       <EmptyDataRowStyle BackColor="White" CssClass="errortext" Font-Bold="True" />
                                                       <Columns>
                                                        <asp:BoundField DataField="SensorId" Visible="False" />
                                                        <asp:BoundField HeaderText="" DataField="SensorName" HeaderStyle-Width="120px" />
                                                        <asp:TemplateField HeaderText="Active" HeaderStyle-Width="40px">
                                                            <ItemStyle HorizontalAlign="center"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:CheckBox AutoPostBack="true" runat="server" ID="chkFuelActive" Checked="false" OnCheckedChanged="chkFuelActive_CheckedChanged" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Lower">
                                                            <ItemStyle Width="50px" HorizontalAlign="center"></ItemStyle>
						                                    <ItemTemplate>
						                                        <asp:TextBox ID="txtFuelLower" Width="30px" Runat="server" Enabled="false" CssClass="formtext"></asp:TextBox>
						                                    </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Current">
                                                            <ItemStyle Width="50px" HorizontalAlign="Center"></ItemStyle>
						                                    <ItemTemplate>
						                                        <asp:Label ID="txtFuelCurrent" Width="30px" Runat="server" BorderColor="ButtonShadow" BorderWidth="1px" BorderStyle="Outset"></asp:Label>
						                                    </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Upper">
                                                            <ItemStyle Width="50px" HorizontalAlign="center"></ItemStyle>
						                                    <ItemTemplate>
						                                        <asp:TextBox ID="txtFuelUpper" Width="30px" Runat="server" Enabled="false" CssClass="formtext"></asp:TextBox>
						                                    </ItemTemplate>
                                                        </asp:TemplateField>
                                                        </Columns>
                                                       <EmptyDataTemplate>
                                                           <asp:Label ID="lblEmpty" runat="server" Text="Empty sensor data"></asp:Label>
                                                       </EmptyDataTemplate>
                                                   </asp:GridView>
                                                   <table class="formtext" style="border: outset 1px #555555; margin-top: 5px;">
                                                        <tr style="background-color: gray; color:White;">
                                                            <th style="width:120px;"></th>
                                                            <th style="width:60px;">Active</th>
                                                            <th style="width:100px;">Fuel Level (%)</th>
                                                            <th style="width: 90px;">Fuel Interval</th>
                                                        </tr>
                                                        <tr style="line-height: small; background-color:#DEDFDE; line-height:11px">
                                                            <td>
                                                                <asp:Label ID="lblFuelLevelCaption" runat="server" Text="Fuel Level Rise/Drop" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td style="text-align: center;">
                                                                <asp:CheckBox ID="chkFuelLevelActive" AutoPostBack="true" Checked="false" runat="server" OnCheckedChanged="chkFuelLevelActive_CheckedChanged" />
                                                            </td>
                                                            <td style="text-align: center;">
                                                                <asp:TextBox ID="txtFuelLevel" Width="40px" Runat="server" Enabled="false" CssClass="formtext"></asp:TextBox>
                                                            </td>
                                                            <td style="text-align: center;">
                                                                <asp:DropDownList ID="ddlFuelInterval" runat="server" Width="70px" CssClass="RegularText" Enabled="false">
						                                            <asp:ListItem Value="0">Not Set</asp:ListItem>
						                                            <asp:ListItem Value="30">30 Sec</asp:ListItem>
						                                            <asp:ListItem Value="60">1 Min</asp:ListItem>
						                                            <asp:ListItem Value="120">2 Min</asp:ListItem>
                                                                    <asp:ListItem Value="300">5 Min</asp:ListItem>
                                                                    <asp:ListItem Value="600">10 Min</asp:ListItem>
                                                                    <asp:ListItem Value="900">15 Min</asp:ListItem>
                                                                    <asp:ListItem Value="1800">30 Min</asp:ListItem>
                                                                    <asp:ListItem Value="3600">1 Hour</asp:ListItem>
                                                                    <asp:ListItem Value="7200">2 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="14400">4 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="21600">6 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="28800">8 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="36000">10 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="43200">12 Hours</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                   </table>
                                                </fieldset>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left; vertical-align: top;">
                                                <fieldset id="pnlReeferStatus" runat="server" style="border: ridge 1px gray; width: 200px; height: auto;">
                                                    <legend>
                                                        <asp:Label ID="lblStatusCaption" runat="server" CssClass="headinginstructions" Text="Status Sensors"></asp:Label>
                                                    </legend>
                                                    <asp:GridView ID="gvStatusSensors" runat="server" Style="margin: 5px 5px 5px 5px" DataMember="StatusSensors" Width="180px"
                                                        BackColor="White" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1" GridLines="None" AutoGenerateColumns="False">
                                                        <AlternatingRowStyle BackColor="WhiteSmoke" CssClass="formtext" Height="11px" />
                                                        <RowStyle BackColor="#DEDFDE" CssClass="formtext" Height="11px" />
                                                        <HeaderStyle BackColor="gray" CssClass="formtext" Font-Bold="True" ForeColor="White" />
                                                        <EmptyDataRowStyle BackColor="White" CssClass="errortext" Font-Bold="True" />
                                                        <Columns>
                                                        <asp:BoundField DataField="SensorId" Visible="False" />
                                                        <asp:BoundField DataField="SensorName" ItemStyle-Width="100px" />
                                                        <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label ID="lblStatusCaption" runat="server" Text="Active" Width="50px" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkActive" runat="server" Checked="false" />
                                                        </ItemTemplate>
                                                        </asp:TemplateField>                    
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            <asp:Label ID="lblEmpty" runat="server" Text="Empty sensor data"></asp:Label>
                                                        </EmptyDataTemplate>
                                                     </asp:GridView>
                                                </fieldset>
                                            </td>
                                            <td style="text-align: left; vertical-align: top; width: 200px;">
                                                <fieldset id="pnlReeferReporting" runat="server" style="border: ridge 1px gray; margin-left: 10px; width: 200px;">
                                                    <legend>
                                                        <asp:Label ID="lblReportingCaption" runat="server" CssClass="headinginstructions" Text="Reporting"></asp:Label>
                                                    </legend>
                                                    <table style="margin: 5px 5px 5px 5px">
                                                        <tr>
                                                            <td style="width: 70px; height: 34px;">
                                                                <asp:Label ID="lblReeferInterval" runat="server" Text="Interval:" Width="60px" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td style="width: 80px; text-align:left; height: 34px;">
                                                                <asp:DropDownList ID="ddlReeferInterval" runat="server" Width="80px" CssClass="RegularText">
                                                                    <asp:ListItem Value="300">5 Min</asp:ListItem>
                                                                    <asp:ListItem Value="600">10 Min</asp:ListItem>
                                                                    <asp:ListItem Value="900">15 Min</asp:ListItem>
                                                                    <asp:ListItem Value="1800">30 Min</asp:ListItem>
                                                                    <asp:ListItem Value="3600">1 Hour</asp:ListItem>
                                                                    <asp:ListItem Value="7200">2 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="14400">4 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="21600">6 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="28800">8 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="36000">10 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="43200">12 Hours</asp:ListItem>
                                                                    <asp:ListItem Value="86400">24 Hours</asp:ListItem>
                                                                </asp:DropDownList>&nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td style="width: 100px">
                                                                <asp:Label ID="lblIncludePosition" runat="server" Text="Include Position" Width="100px" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td style="width: 30px">
                                                                <asp:CheckBox ID="chkReeferIncludePosition" runat="server" Checked="false" />
                                                            </td>
                                                        </tr>
                                                    </table> 
                                                </fieldset>
                                                <br />
                                                <fieldset id="pnlReeferAdditional" style="border: ridge 1px gray; margin-left: 10px; width: 200px;">
                                                    <asp:Label runat="server" ID="lblFWVersionCaption" CssClass="formtext" Text="Firmware Version:" style="margin-left: 10px"></asp:Label>
                                                    <br />
                                                    <asp:Label runat="server" ID="lblFWVersionValue" CssClass="formtext" style="margin-left: 10px"></asp:Label>
                                                    <br />
                                                    <asp:Label runat="server" ID="lblMainBatteryCaption" Text="Main Battery:" CssClass="formtext" Width="90px" style="margin-left: 10px"></asp:Label>
                                                    <asp:Label runat="server" ID="lblMainBatteryValue" CssClass="formtext"></asp:Label>
                                                </fieldset>
                                            </td>
                                        </tr>
                                        <!--tr>
                                            <td colspan="2">
                                                <asp:Label ID="lblReeferReason" runat="server" Text="Reason" Width="70px" CssClass="headinginstructions"></asp:Label>
                                                <asp:TextBox ID="txtReeferReason" runat="server" Width="300px" CssClass="formtext"></asp:TextBox>
                                            </td>
                                        </tr-->
                                        <tr>
                                            <td colspan="2">
                                                <table>
                                                    <tr>
                                                        <td style="width: 120px;">
                                                            <asp:Button ID="btnReeferGet" runat="server" Text="Get" Width="80px" CssClass="combutton" OnClick="btnGet_Click" />
                                                        </td>
                                                        <td style="width: 120px;">
                                                            <asp:Button ID="btnReeferSet" runat="server" Text="Set" Width="80px" CssClass="combutton" OnClick="btnSet_Click" />
                                                        </td>
                                                        <td style="width: 120px;">
                                                            <asp:Button ID="btnReset" runat="server" Text="Reset" Width="80px" CssClass="combutton" OnClick="btnReset_Click" />
                                                        </td>
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
                <tr style="height:20px">
                    <td>
                    </td>
                </tr>
                <tr>
                    <td style="width: 508px">
                        <asp:Label ID="lblMessage" runat="server" style="left: 0px;" CssClass="errortext"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <busyboxdotnet:busybox id="BusySend" runat="server" anchorcontrol="" compressscripts="False"
            gzipcompression="False" slideeasing="BackBoth" text="Sending Command...">
        </busyboxdotnet:busybox>
    </form>
</body>
</html>
