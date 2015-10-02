<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BoxSetup.ascx.cs" Inherits="SentinelFM.Map_CommandsControls_BoxSetup" %>
<table id="tblBoxSetup" runat="server" border="0" cellpadding="0" cellspacing="0"
    style="width: 391px; height: 323px">
    <tr>
        <td>
            <div id="divsetup" align="center" style="border-right: gray thin outset; border-top: gray thin outset;
                overflow: auto; border-left: gray thin outset; width: 390px; border-bottom: gray thin outset;
                height: 315px" title="Box Setup:">
                <table id="tblSet" runat="server" border="0" cellpadding="0" cellspacing="0" style="width: 359px">
                    <tr>
                        <td class="formtext" colspan="2" style="height: 20px" align="left">
                            <asp:Label ID="lblBoxSetupTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblBoxSetupTitleResource1"
                                Text="Box Setup:"></asp:Label></td>
                        <td style="height: 20px">
                        </td>
                        <td style="height: 20px">
                        </td>
                    </tr>
                    <tr>
                        <td class="formtext" style="width: 71px; height: 18px">
                            <asp:Label ID="lblReportingFrequencyTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblReportingFrequencyTitle2Resource1"
                                Text="Reporting Frequency:"></asp:Label></td>
                        <td class="formtext" style="height: 18px">
                            <asp:DropDownList ID="cboFreguency" runat="server" CssClass="RegularText" meta:resourcekey="cboFreguencyResource1"
                                Width="110px">
                            </asp:DropDownList></td>
                        <td class="formtext" style="height: 18px">
                            <asp:Label ID="lblSpeedThresholdTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblSpeedThresholdTitle2Resource1"
                                Text="Speed Threshold:"></asp:Label></td>
                        <td style="height: 18px">
                            <asp:DropDownList ID="cboSpeed" runat="server" CssClass="RegularText" meta:resourcekey="cboSpeedResource1"
                                Width="99px">
                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td class="formtext" style="width: 71px; height: 19px">
                            <asp:Label ID="lblGeoFenceTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblGeoFenceTitle2Resource1"
                                Text="GeoFence:"></asp:Label></td>
                        <td class="formtext" style="height: 3px">
                            <asp:DropDownList ID="cboGeo" runat="server" CssClass="RegularText" Height="25px"
                                meta:resourcekey="cboGeoResource1" Width="110px">
                            </asp:DropDownList></td>
                        <td class="formtext">
                            <asp:Label ID="lblCommMode2" runat="server" CssClass="formtext" meta:resourcekey="lblCommMode2Resource1"
                                Text="Communication Mode:"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="cboCommMode" runat="server" CssClass="RegularText" DataTextField="CommModeName"
                                DataValueField="CommModeName" Height="25px" meta:resourcekey="cboCommModeResource1"
                                Width="99px">
                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td class="formtext" style="width: 71px; height: 23px">
                            <asp:Label ID="lblTracePeriodTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblTracePeriodTitle2Resource1"
                                Text="Trace Period:"></asp:Label></td>
                        <td class="formtext" style="height: 23px">
                            <asp:DropDownList ID="cboTracePeriodSetup" runat="server" AutoPostBack="True" CssClass="RegularText"
                                meta:resourcekey="cboTracePeriodSetupResource1" OnSelectedIndexChanged="cboTracePeriodSetup_SelectedIndexChanged"
                                Width="110px">
                                <asp:ListItem meta:resourcekey="ListItemResource90" Text="Disabled" Value="0"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource91" Text="1 min" Value="60"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource92" Text="2 min" Value="120"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource93" Selected="True" Text="5 min"
                                    Value="300"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource94" Text="10 min" Value="600"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource95" Text="15 min" Value="900"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource96" Text="30 min" Value="1800"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource97" Text="1 hour" Value="3600"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource98" Text="12 hours" Value="43200"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource99" Text="18 hours" Value="64800"></asp:ListItem>
                            </asp:DropDownList></td>
                        <td class="formtext" style="height: 23px">
                            <asp:Label ID="lblTraceIntervalTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblTraceIntervalTitle2Resource1"
                                Text="Trace Interval:"></asp:Label></td>
                        <td style="height: 23px">
                            <asp:DropDownList ID="cboTraceIntervalSetup" runat="server" CssClass="RegularText"
                                meta:resourcekey="cboTraceIntervalSetupResource1" Width="99px">
                                <asp:ListItem meta:resourcekey="ListItemResource100" Text="Disabled" Value="0"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource101" Selected="True" Text="15 sec"
                                    Value="15"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource102" Text="30 sec" Value="30"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource103" Text="1 min" Value="60"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource104" Text="2 min" Value="120"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource105" Text="5 min" Value="300"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource106" Text="10 min" Value="600"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource107" Text="15 min" Value="900"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource108" Text="30 min" Value="1800"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource109" Text="1 hour" Value="3600"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource110" Text="16 hours" Value="57600"></asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td align="left" class="formtext" style="width: 71px; height: 21px">
                            <u><strong></strong></u>
                        </td>
                        <td align="left" style="height: 21px">
                        </td>
                        <td align="right">
                        </td>
                        <td align="right">
                        </td>
                    </tr>
                </table>
                <table id="tblRapSet" runat="server" border="0" cellpadding="0" cellspacing="0" width="300" visible="false">
                    <tr>
                        <td class="formtext" style="font-weight: bold; height: 19px">
                            <asp:Label ID="lblBoxSetupTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupTitle2Resource1"
                                Text="Box Setup:"></asp:Label></td>
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
                            <asp:Label ID="lblGPSFrequencyTitle" runat="server" CssClass="formtext" meta:resourcekey="lblGPSFrequencyTitleResource1"
                                Text="GPS Frequency:"></asp:Label></td>
                        <td style="height: 19px">
                            <asp:DropDownList ID="cboGPSFrequency" runat="server" CssClass="RegularText" meta:resourcekey="cboGPSFrequencyResource1"
                                Width="110px">
                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td class="formtext" style="height: 12px">
                            <asp:Label ID="lblGPSFrequencyStationaryTitle" runat="server" CssClass="formtext"
                                meta:resourcekey="lblGPSFrequencyStationaryTitleResource1" Text="GPS Frequency Stationary:"></asp:Label></td>
                        <td style="height: 12px">
                            <asp:DropDownList ID="cboGPSFreqStat" runat="server" CssClass="RegularText" meta:resourcekey="cboGPSFreqStatResource1"
                                Width="110px">
                                <asp:ListItem meta:resourcekey="ListItemResource111" Text="Disable" Value="0"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource112" Text="5 Min" Value="5"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource113" Text="15 Min" Value="15"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource114" Text="30 Min" Value="30"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource115" Text="1 Hour" Value="60"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource116" Text="2 Hours" Value="120"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource117" Text="3 Hours" Value="180"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource118" Text="4 Hours" Value="240"></asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                    <tr>
                        <td class="formtext">
                            <asp:Label ID="lblDistanceIntervalTitle" runat="server" CssClass="formtext" meta:resourcekey="lblDistanceIntervalTitleResource1"
                                Text="Distance Interval:"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="cboDistInterval" runat="server" CssClass="RegularText" meta:resourcekey="cboDistIntervalResource1"
                                Width="110px">
                                <asp:ListItem meta:resourcekey="ListItemResource119" Text="Disable" Value="0"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource120" Text="500 m" Value="500"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource121" Text="1 km" Value="1000"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource122" Text="3 km" Value="3000"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource123" Text="5 km" Value="5000"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource124" Text="10 km" Value="10000"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource125" Text="15 km" Value="15000"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource126" Text="20 km" Value="20000"></asp:ListItem>
                                <asp:ListItem meta:resourcekey="ListItemResource127" Text="25 km" Value="25000"></asp:ListItem>
                            </asp:DropDownList></td>
                    </tr>
                </table>
                <table id="tblViolations" runat="server"   class="formtext" width="359">
                    <tr>
                        <td style="width: 214px" align=left  >
                            <asp:Label ID="lblsetupHarshAcceleration" runat="server" meta:resourcekey="lblsetupHarshAccelerationResource1"
                                Text="Harsh Acceleration  (0-100):" Width="214px"></asp:Label></td>
                        <td style="width: 100px">
                            <asp:TextBox ID="txtsetupHarshAcceleration" runat="server" meta:resourcekey="txtsetupHarshAccelerationResource1"
                                Width="94px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td style="width: 214px; height: 26px" align=left>
                            <asp:Label ID="lblsetupHarshBraking" runat="server" meta:resourcekey="lblsetupHarshBrakingResource1"
                                Text="Harsh Braking  (0-100):" Width="214px"></asp:Label></td>
                        <td style="width: 100px; height: 26px">
                            <asp:TextBox ID="txtsetupHarshBraking" runat="server" meta:resourcekey="txtsetupHarshBrakingResource1"
                                Width="94px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td style="width: 214px" align=left>
                            <asp:Label ID="lblsetupExtremeAcceleration" runat="server" meta:resourcekey="lblsetupExtremeAccelerationResource1"
                                Text="Extreme Acceleration  (0-100):" Width="214px"></asp:Label></td>
                        <td style="width: 100px">
                            <asp:TextBox ID="txtsetupExtremeAcceleration" runat="server" meta:resourcekey="txtsetupExtremeAccelerationResource1"
                                Width="94px"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td style="width: 214px" align=left>
                            <asp:Label ID="lblsetupExtremeBraking" runat="server" meta:resourcekey="lblsetupExtremeBrakingResource1"
                                Text="Extreme Braking   (0-100):   " Width="214px"></asp:Label></td>
                        <td style="width: 100px" align=left>
                            <asp:TextBox ID="txtsetupExtremeBraking" runat="server" CssClass="formtext" meta:resourcekey="txtsetupExtremeBrakingResource1"
                                Width="94px"></asp:TextBox></td>
                    </tr>
                </table>
                <table id="Table10" border="0" cellpadding="0" cellspacing="0" width="300">
                    <tr>
                        <td height="25">
                            <asp:Button ID="cmdUnselect" runat="server" CssClass="combutton" meta:resourcekey="cmdUnselectResource1"
                                OnClick="cmdUnselect_Click" Text="Deselect All" Width="92px" /></td>
                        <td height="25">
                            <asp:Button ID="cmdSelectAll" runat="server" CssClass="combutton" meta:resourcekey="cmdSelectAllResource1"
                                OnClick="cmdSelectAll_Click" Text="Select All" Width="92px" /></td>
                    </tr>
                </table>
                <asp:DataGrid ID="dgBoxSetupSensors" runat="server" AutoGenerateColumns="False" DataKeyField="SensorId"
                    meta:resourcekey="dgBoxSetupSensorsResource1" PageSize="4" Width="358px" OnCancelCommand="dgBoxSetupSensors_CancelCommand" OnEditCommand="dgBoxSetupSensors_EditCommand" OnUpdateCommand="dgBoxSetupSensors_UpdateCommand">
                    <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                    <AlternatingItemStyle BackColor="WhiteSmoke" />
                    <ItemStyle BackColor="#DEDFDE" Font-Names="Arial,Helvetica,sans-serif" Font-Size="11px"
                        ForeColor="Black" />
                    <HeaderStyle CssClass="DataHeaderStyle" />
                    <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                    <Columns>
                        <asp:BoundColumn DataField="SensorId" HeaderText='<%$ Resources:dgBoxSetupSensors_SensorId %>'
                            Visible="False"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText='<%$ Resources:dgBoxSetupSensors_Enabled %>'>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkCheckBox" runat="server" Checked='<%# DataBinder.Eval(Container.DataItem, "chkSet") %>'
                                    meta:resourcekey="chkCheckBoxResource1" />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgBoxSetupSensors_Sensors %>'
                            ReadOnly="True"></asp:BoundColumn>
                        <asp:TemplateColumn HeaderText='<%$ Resources:dgBoxSetupSensors_Trace %>'>
                            <ItemStyle Wrap="False" />
                            <ItemTemplate>
                                <asp:Label ID="lblTraceSetup" runat="server" meta:resourcekey="lblTraceSetupResource1"
                                    Text='<%# DataBinder.Eval(Container.DataItem,"TraceStateName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="cboTraceSetup" runat="server" DataSource='<%# dsTrace %>' DataTextField="TraceStateName"
                                    DataValueField="TraceStateId" meta:resourcekey="cboTraceSetupResource1" SelectedIndex='<%# GetTrace(Convert.ToInt16(DataBinder.Eval(Container.DataItem,"TraceStateId"))) %>'>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateColumn>
                        <asp:EditCommandColumn CancelText="&lt;img src=../images/cancel.gif border=0&gt;"
                            EditText="&lt;img src=../images/edit.gif border=0&gt;" meta:resourcekey="EditCommandColumnResource2"
                            UpdateText="&lt;img src=../images/ok.gif border=0&gt;"></asp:EditCommandColumn>
                    </Columns>
                    <PagerStyle Mode="NumericPages" />
                </asp:DataGrid></div>
        </td>
    </tr>
</table>
