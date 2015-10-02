<%@ Page Language="c#" Inherits="SentinelFM.frmReports" CodeFile="frmReports.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Reports</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="frmTripReport" method="post" runat="server">
        <table id="Table4" style="border-right: gray 2px outset; border-top: gray 2px outset;
            left: 8px; border-left: gray 2px outset; width: 1000px; border-bottom: gray 2px outset;
            position: absolute; top: 4px; height: 575px; background-color: #fffff0" cellspacing="0"
            cellpadding="0" width="1000" border="0">
            <tr>
                <td style="height: 300px" align="center">
                    &nbsp;&nbsp;
                    <table id="Table1" style="border-right: gray 4px double; border-top: gray 4px double;
                        border-left: gray 4px double; width: 751px; border-bottom: gray 4px double;
                        height: 350px" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td class="heading" style="width: 28px" height="20">
                            </td>
                            <td class="heading" style="width: 50px" height="20">
                            </td>
                            <td class="formtext" style="width: 312px" height="20">
                            </td>
                            <td style="width: 48px" height="20">
                            </td>
                            <td style="width: 300px" height="20">
                            </td>
                        </tr>
                        <tr>
                            <td class="heading" style="width: 28px; height: 18px">
                            </td>
                            <td class="heading" style="width: 50px; height: 18px">
                                &nbsp;<asp:Label ID="lblReportTitle" runat="server" CssClass="heading" meta:resourcekey="lblReportTitleResource1" Text="Report:"></asp:Label></td>
                            <td class="formtext" style="width: 312px; height: 25px">
                                <asp:DropDownList ID="cboReports" runat="server" AutoPostBack="True" CssClass="RegularText"
                                    Width="249px" DataTextField="GuiName" DataValueField="GuiId" OnSelectedIndexChanged="cboReports_SelectedIndexChanged" meta:resourcekey="cboReportsResource1">
                                </asp:DropDownList></td>
                            <td style="width: 48px">
                            </td>
                            <td style="width: 300px; height: 18px">
                            </td>
                        </tr>
                        <tr style="width: 300px; height: 81px">
                            <td class="formtext" style="width: 28px;">
                            </td>
                            <td class="formtext" style="width: 50px; height: 10px">
                            </td>
                            <td class="formtext" style="width: 312px;">
                                <table id="tblException" cellspacing="0" cellpadding="0" border="0" runat="server">
                                    <tr>
                                        <td height="3">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formtext">
                                            <table id="Table3" cellspacing="0" cellpadding="0" width="300" border="0">
                                                <tr>
                                                    <td>
                                                        <asp:CheckBox ID="chkDoor" runat="server" CssClass="formtext" Width="203px" Text="No"
                                                            Visible="False" meta:resourcekey="chkDoorResource1"></asp:CheckBox>
                                                        <table id="Table5" style="border-right: gray 1px inset; border-top: gray 1px inset;
                                                            border-left: gray 1px inset; width: 256px; border-bottom: gray 1px inset" cellspacing="0"
                                                            cellpadding="0" width="256" border="0">
                                                            <tr>
                                                                <td class="formtext" align="left" height="4">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="formtext" align="left">
                                                                    &nbsp;<asp:Label ID="Label3" runat="server" CssClass="formtext" meta:resourcekey="Label3Resource1" Text="No opening/closing within:"></asp:Label><asp:DropDownList
                                                                        ID="cboDoorPeriod" runat="server" CssClass="RegularText" Width="70px" Height="14px" meta:resourcekey="cboDoorPeriodResource1">
                                                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource1" Text="1 Hour"></asp:ListItem>
                                                                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource2" Text="2 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource3" Text="3 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="4" meta:resourcekey="ListItemResource4" Text="4 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="5" meta:resourcekey="ListItemResource5" Text="5 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="6" meta:resourcekey="ListItemResource6" Text="6 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="7" meta:resourcekey="ListItemResource7" Text="7 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="8" meta:resourcekey="ListItemResource8" Text="8 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="9" meta:resourcekey="ListItemResource9" Text="9 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="18" meta:resourcekey="ListItemResource10" Text="18 Hours"></asp:ListItem>
                                                                        <asp:ListItem Value="24" meta:resourcekey="ListItemResource11" Text="1 Day"></asp:ListItem>
                                                                        <asp:ListItem Value="48" meta:resourcekey="ListItemResource12" Text="2 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="72" meta:resourcekey="ListItemResource13" Text="3 Days"></asp:ListItem>
                                                                        <asp:ListItem Value="168" meta:resourcekey="ListItemResource14" Text="Week"></asp:ListItem>
                                                                        <asp:ListItem Value="730" meta:resourcekey="ListItemResource15" Text="1 Month"></asp:ListItem>
                                                                        <asp:ListItem Value="1460" meta:resourcekey="ListItemResource16" Text="2 Month"></asp:ListItem>
                                                                    </asp:DropDownList></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkDriverDoorExc" runat="server" CssClass="formtext" Text="Driver Door" meta:resourcekey="chkDriverDoorExcResource1">
                                                                    </asp:CheckBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkPassengerDoorExc" runat="server" CssClass="formtext" Text="Passenger Door" meta:resourcekey="chkPassengerDoorExcResource1">
                                                                    </asp:CheckBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkSideHopperDoorExc" runat="server" CssClass="formtext" Text="Side Hopper Door" meta:resourcekey="chkSideHopperDoorExcResource1">
                                                                    </asp:CheckBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left">
                                                                    <asp:CheckBox ID="chkRearHopperDoorExc" runat="server" CssClass="formtext" Text="Rear Hopper Door" meta:resourcekey="chkRearHopperDoorExcResource1">
                                                                    </asp:CheckBox></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" height="6">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <table id="Table2" cellspacing="0" cellpadding="0" border="0">
                                                <tr>
                                                    <td style="width: 79px; height: 20px">
                                                        <asp:CheckBox ID="chkSOSMode" runat="server" CssClass="formtext" Width="114px" Text="More than" meta:resourcekey="chkSOSModeResource1">
                                                        </asp:CheckBox></td>
                                                    <td style="height: 20px">
                                                        <asp:DropDownList ID="cboSOSLimit" runat="server" CssClass="RegularText" Width="55px"
                                                            Height="14px" meta:resourcekey="cboSOSLimitResource1">
                                                            <asp:ListItem Value="3" meta:resourcekey="ListItemResource17" Text="3"></asp:ListItem>
                                                            <asp:ListItem Value="6" meta:resourcekey="ListItemResource18" Text="6"></asp:ListItem>
                                                            <asp:ListItem Value="9" meta:resourcekey="ListItemResource19" Text="9"></asp:ListItem>
                                                            <asp:ListItem Value="18" meta:resourcekey="ListItemResource20" Text="18"></asp:ListItem>
                                                            <asp:ListItem Value="24" meta:resourcekey="ListItemResource21" Text="24"></asp:ListItem>
                                                            <asp:ListItem Value="48" meta:resourcekey="ListItemResource22" Text="48"></asp:ListItem>
                                                            <asp:ListItem Value="72" meta:resourcekey="ListItemResource23" Text="72"></asp:ListItem>
                                                        </asp:DropDownList></td>
                                                    <td class="formtext" style="height: 20px">
                                                    </td>
                                                    <td class="formtext" style="width: 4px; height: 20px">
                                                    </td>
                                                    <td class="formtext" style="height: 20px">
                                                        <asp:Label ID="lblSOSModesTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSOSModesTitleResource1" Text="SOS modes"></asp:Label></td>
                                                    <td class="formtext" style="height: 20px">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 10px">
                                            <asp:CheckBox ID="chkTAR" runat="server" CssClass="formtext" Width="165px" Text="Any TAR mode events" meta:resourcekey="chkTARResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkImmobilization" runat="server" CssClass="formtext" Width="211px"
                                                Text="Vehicle immobilization events" meta:resourcekey="chkImmobilizationResource1"></asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td style="height: 18px">
                                            <asp:CheckBox ID="chkDriverDoor" runat="server" CssClass="formtext" Width="292px"
                                                Text="15 seconds driver/passenger door violation" meta:resourcekey="chkDriverDoorResource1"></asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td style="height: 18px">
                                            <asp:CheckBox ID="chkLeash" runat="server" CssClass="formtext" Width="270px" Text="50% of Leash Event" meta:resourcekey="chkLeashResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                </table>
                                <table id="tblHistoryOptions" cellspacing="0" cellpadding="0" width="275" border="0"
                                    runat="server">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkHistIncludeCoordinate" runat="server" CssClass="formtext" Text="Include Coordinates"
                                                Checked="True" meta:resourcekey="chkHistIncludeCoordinateResource1"></asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkHistIncludeSensors" runat="server" CssClass="formtext" Width="196px"
                                                Text="Include Sensors" Checked="True" meta:resourcekey="chkHistIncludeSensorsResource1"></asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkHistIncludeInvalidGPS" runat="server" AutoPostBack="True" CssClass="formtext"
                                                Width="280px" Text="Include Invalid GPS for coordinate messages" Checked="True" meta:resourcekey="chkHistIncludeInvalidGPSResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkHistIncludePositions" runat="server" CssClass="formtext" Width="261px"
                                                Text="Include Position" Checked="True" Visible="False" meta:resourcekey="chkHistIncludePositionsResource1"></asp:CheckBox></td>
                                    </tr>
                                </table>
                                <table id="tblOptions1" cellspacing="0" cellpadding="0" width="277" border="0" runat="server">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkIncludeStreetAddress" runat="server" CssClass="formtext" Text="Include Street Address"
                                                Checked="True" meta:resourcekey="chkIncludeStreetAddressResource1"></asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkIncludeSensors" runat="server" CssClass="formtext" Width="198px"
                                                Text="Include Sensors" Checked="True" meta:resourcekey="chkIncludeSensorsResource1"></asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkIncludePosition" runat="server" CssClass="formtext" Width="211px"
                                                Text="Include Position" Checked="True" meta:resourcekey="chkIncludePositionResource1"></asp:CheckBox></td>
                                    </tr>
                                </table>
                                <asp:CheckBox ID="chkShowStorePosition" runat="server" CssClass="formtext" Width="257px"
                                    Text="Show Stored Position" meta:resourcekey="chkShowStorePositionResource1"></asp:CheckBox>
                                <table id="tblOptions2" cellspacing="0" cellpadding="0" width="300" border="0" runat="server">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkIncludeIdleTime" runat="server" CssClass="formtext" Text="Include Idle Time"
                                                Checked="True" meta:resourcekey="chkIncludeIdleTimeResource1"></asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkIncludeSummary" runat="server" CssClass="formtext" Text="Include Summary"
                                                Checked="True" meta:resourcekey="chkIncludeSummaryResource1"></asp:CheckBox></td>
                                    </tr>
                                </table>
                                <table id="tblStopReport" cellspacing="0" cellpadding="0" width="300" border="0"
                                    runat="server">
                                    <tr>
                                        <td class="formtext" style="width: 105px; height: 30px;">
                                            &nbsp;<asp:Label ID="lblStopDurationTitle" runat="server" CssClass="formtext" meta:resourcekey="lblStopDurationTitleResource1" Text="Stop Duration:"></asp:Label></td>
                                        <td style="height: 30px">
                                            <asp:DropDownList ID="cboStopSequence" runat="server" CssClass="formtext" Width="144px" meta:resourcekey="cboStopSequenceResource1">
                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource24" Text="Not Filtered"></asp:ListItem>
                                                <asp:ListItem Value="300" meta:resourcekey="ListItemResource25" Text="Longer than 5 Min"></asp:ListItem>
                                                <asp:ListItem Value="600" meta:resourcekey="ListItemResource26" Text="Longer than 10 Min"></asp:ListItem>
                                                <asp:ListItem Value="900" meta:resourcekey="ListItemResource27" Text="Longer than 15 Min"></asp:ListItem>
                                                <asp:ListItem Value="1800" meta:resourcekey="ListItemResource28" Text="Longer than 30 Min"></asp:ListItem>
                                                <asp:ListItem Value="2700" meta:resourcekey="ListItemResource29" Text="Longer than 45 Min"></asp:ListItem>
                                                <asp:ListItem Value="3600" meta:resourcekey="ListItemResource30" Text="Longer than 1 Hour"></asp:ListItem>
                                                <asp:ListItem Value="7200" meta:resourcekey="ListItemResource31" Text="Longer than 2 Hours"></asp:ListItem>
                                                <asp:ListItem Value="43200" meta:resourcekey="ListItemResource32" Text="Longer than 12 Hours"></asp:ListItem>
                                                <asp:ListItem Value="86400" meta:resourcekey="ListItemResource33" Text="Longer than 24 Hours"></asp:ListItem>
                                                <asp:ListItem Value="172800" meta:resourcekey="ListItemResource34" Text="Longer than 48 Hours"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="formtext" style="width: 105px; height: 30px">
                                            <asp:RadioButtonList ID="optStopFilter" runat="server" CssClass="formtext" Width="169px" meta:resourcekey="optStopFilterResource1">
                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource35" Text="Show Stops Only"></asp:ListItem>
                                                <asp:ListItem Value="1" meta:resourcekey="ListItemResource36" Text="Show Idlings Only"></asp:ListItem>
                                                <asp:ListItem Selected="True" Value="2" meta:resourcekey="ListItemResource37" Text="Show Stops and Idlings"></asp:ListItem>
                                            </asp:RadioButtonList></td>
                                    </tr>
                                </table>
                                <table id="tblViolationReport" cellspacing="0" cellpadding="0" width="300" border="0"
                                    runat="server">
                                    <tr>
                                        <td colspan="2" class="formtext">
                                            <asp:CheckBox ID="chkSpeedViolation" runat="server" Text="Speed Violation" Checked="True" meta:resourcekey="chkSpeedViolationResource1" /><br />
                                            <asp:CheckBox ID="chkHarshAcceleration" runat="server" Text="Harsh Acceleration"
                                                Checked="True" meta:resourcekey="chkHarshAccelerationResource1" /><br />
                                            <asp:CheckBox ID="chkHarshBraking" runat="server" Text="Harsh Braking" Checked="True" meta:resourcekey="chkHarshBrakingResource1" /><br />
                                            <asp:CheckBox ID="chkExtremeAcceleration" runat="server" Text="Extreme Acceleration"
                                                Checked="True" meta:resourcekey="chkExtremeAccelerationResource1" /><br />
                                            <asp:CheckBox ID="chkExtremeBraking" runat="server" Text="Extreme Braking" Checked="True" meta:resourcekey="chkExtremeBrakingResource1" /><br />
                                            <asp:CheckBox ID="chkSeatBeltViolation" runat="server" Text="SeatBelt Violation"
                                                Checked="True" meta:resourcekey="chkSeatBeltViolationResource1" /><br />
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 48px;">
                            </td>
                            <td style="width: 300px;" valign=top >
                                <table class="formtext" id="tblOffHours" style="width: 258px" cellspacing="0" cellpadding="0"
                                    width="258" border="0" runat="server">
                                    <tr>
                                        <td style="width: 103px" height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 103px; height: 10px">
                                            <asp:Label ID="lblMonToFriTitle" runat="server" meta:resourcekey="lblMonToFriTitleResource1" Text="Monday-Friday:"></asp:Label></td>
                                        <td style="height: 10px">
                                            <asp:Label ID="lblFromTitle" runat="server" meta:resourcekey="lblFromTitleResource1" Text="From:"></asp:Label></td>
                                        <td style="height: 10px">
                                            <asp:DropDownList ID="cboFromDayH" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboFromDayHResource1">
                                            </asp:DropDownList></td>
                                        <td style="height: 10px">
                                            :</td>
                                        <td style="height: 10px">
                                            <asp:DropDownList ID="cboFromDayM" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboFromDayMResource1">
                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource38" Text="00"></asp:ListItem>
                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource39" Text="15"></asp:ListItem>
                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource40" Text="30"></asp:ListItem>
                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource41" Text="45"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 103px; height: 18px">
                                        </td>
                                        <td style="height: 18px">
                                            <asp:Label ID="lblToTitle" runat="server" meta:resourcekey="lblToTitleResource1" Text="To:"></asp:Label></td>
                                        <td style="height: 18px">
                                            <asp:DropDownList ID="cboToDayH" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboToDayHResource1">
                                            </asp:DropDownList></td>
                                        <td style="height: 18px">
                                            :</td>
                                        <td style="height: 18px">
                                            <asp:DropDownList ID="cboToDayM" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboToDayMResource1">
                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource42" Text="00"></asp:ListItem>
                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource43" Text="15"></asp:ListItem>
                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource44" Text="30"></asp:ListItem>
                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource45" Text="45"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 103px" height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <asp:CheckBox ID="chkWeekend" runat="server" AutoPostBack="True" Width="184px" Text="Not operated on weekends"
                                                TextAlign="Left" OnCheckedChanged="chkWeekend_CheckedChanged" meta:resourcekey="chkWeekendResource1"></asp:CheckBox></td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 103px" height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                        <td height="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 103px">
                                            <asp:Label ID="lblWeekendTitle" runat="server" meta:resourcekey="lblWeekendTitleResource1" Text="Weekend:"></asp:Label></td>
                                        <td>
                                            <asp:Label ID="lblFromTitle2" runat="server" meta:resourcekey="lblFromTitle2Resource1" Text="From:"></asp:Label></td>
                                        <td>
                                            <asp:DropDownList ID="cboWeekEndFromH" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboWeekEndFromHResource1">
                                            </asp:DropDownList></td>
                                        <td>
                                            :</td>
                                        <td>
                                            <asp:DropDownList ID="cboWeekEndFromM" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboWeekEndFromMResource1">
                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource46" Text="00"></asp:ListItem>
                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource47" Text="15"></asp:ListItem>
                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource48" Text="30"></asp:ListItem>
                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource49" Text="45"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 103px">
                                        </td>
                                        <td>
                                            <asp:Label ID="lblToTitle2" runat="server" meta:resourcekey="lblToTitle2Resource1" Text="To:"></asp:Label></td>
                                        <td>
                                            <asp:DropDownList ID="cboWeekEndToH" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboWeekEndToHResource1">
                                            </asp:DropDownList></td>
                                        <td>
                                            :</td>
                                        <td>
                                            <asp:DropDownList ID="cboWeekEndToM" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboWeekEndToMResource1">
                                                <asp:ListItem Value="00" meta:resourcekey="ListItemResource50" Text="00"></asp:ListItem>
                                                <asp:ListItem Value="15" meta:resourcekey="ListItemResource51" Text="15"></asp:ListItem>
                                                <asp:ListItem Value="30" meta:resourcekey="ListItemResource52" Text="30"></asp:ListItem>
                                                <asp:ListItem Value="45" meta:resourcekey="ListItemResource53" Text="45"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                </table>
                                <table id="tblException1" cellspacing="0" cellpadding="0" width="300" border="0"
                                    runat="server">
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkExcBattery" runat="server" CssClass="formtext" Text="Main battery and backup battery" meta:resourcekey="chkExcBatteryResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkExcTamper" runat="server" CssClass="formtext" Text="Tamper events" meta:resourcekey="chkExcTamperResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkExcPanic" runat="server" CssClass="formtext" Text="Any panic events" meta:resourcekey="chkExcPanicResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkExcKeypad" runat="server" CssClass="formtext" Text="3 keypad/card attempts incorrect" meta:resourcekey="chkExcKeypadResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkExcGPS" runat="server" CssClass="formtext" Text="Alt GPS antenna" meta:resourcekey="chkExcGPSResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkExcAVL" runat="server" CssClass="formtext" Text="Controller Status" meta:resourcekey="chkExcAVLResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:CheckBox ID="chkExcLeash" runat="server" CssClass="formtext" Text="Leash broken" meta:resourcekey="chkExcLeashResource1">
                                            </asp:CheckBox></td>
                                    </tr>
                                    <tr>
                                        <td style="height: 18px">
                                            <asp:CheckBox ID="chkIncCurTARMode" runat="server" CssClass="formtext" Text="Include current TAR mode status" meta:resourcekey="chkIncCurTARModeResource1" /></td>
                                    </tr>
                                </table>
                                            <table class="formtext" id="tblPoints" runat="server" visible="false">
                                                <tr>
                                                    <td style="width: 87px">
                                                       
                                                        <table>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="lblSpeed120" runat="server" Text="Speed >120" CssClass="formtext" meta:resourcekey="lblSpeed120Resource1"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:TextBox ID="txtSpeed120" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed120Resource1" Text="10"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 110px">
                                                       
                                                        <table>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="Label9" runat="server" Text="Acc. Harsh" CssClass="formtext" meta:resourcekey="Label9Resource1"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:TextBox ID="txtAccHarsh" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccHarshResource1" Text="10"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 100px">
                                                       
                                                        <table>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="lblBrakingExtreme" runat="server" Text="Braking Extreme" CssClass="formtext" Width="98px" meta:resourcekey="lblBrakingExtremeResource1"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:TextBox ID="txtBrakingExtreme" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtBrakingExtremeResource1" Text="20"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 87px">
                                                        
                                                        <table class="formtext">
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="lblSpeed130" runat="server" Text="Speed >130" meta:resourcekey="lblSpeed130Resource1"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:TextBox ID="txtSpeed130" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed130Resource1" Text="20"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 110px">
                                                        
                                                        <table class="formtext">
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="lblAccExtreme" runat="server" Text="Acc. Extreme" meta:resourcekey="lblAccExtremeResource1"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:TextBox ID="txtAccExtreme" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccExtremeResource1" Text="20"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 100px">
                                                        
                                                        <table class="formtext">
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="lblSeatBelt" runat="server" Text="Seat Belt" meta:resourcekey="lblSeatBeltResource1"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:TextBox ID="txtSeatBelt" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSeatBeltResource1" Text="50"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 87px;" >
                                                        
                                                        <table class="formtext">
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="lblSpeed140" runat="server" Text="Speed >140" meta:resourcekey="lblSpeed140Resource1"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:TextBox ID="txtSpeed140" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed140Resource1" Text="50"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 110px;">
                                                       
                                                        <table>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="lblBrakingHarsh" runat="server" CssClass="formtext" Text="Braking Harsh" meta:resourcekey="lblBrakingHarshResource1"></asp:Label></td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100px">
                                                                   
                                                                    <asp:TextBox ID="txtBrakingHarsh" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtBrakingHarshResource1" Text="10"></asp:TextBox></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td style="width: 100px;">
                                                    </td>
                                                </tr>
                                            </table>
                            <table id="tblIgnition" cellspacing="0" cellpadding="0" width="300" border="0"
                                    runat="server" visible="true">
                                   <tr>
                                      <td colspan="2" class="formtext">
                                       <table >
                                            <tr>
                                               <td class="formtext" style="width: 151px; height: 20px" >
                                                  <asp:Label ID="lblIgnition" runat="server" meta:resourcekey="lblIgnitionResource1"
                                                     Text="Calculate Trips based on:"></asp:Label></td>
                                            </tr>
                                            <tr>
                                               <td style="width: 151px" align="center">
                                      
                                               <asp:RadioButtonList ID="optEndTrip" runat="server" CssClass="formtext" meta:resourcekey="optEndTripResource1">
                                                  <asp:ListItem Selected="True" Value="3" meta:resourcekey="ListItemResource62" Text="Ignition"></asp:ListItem>
                                                  <asp:ListItem Value="8" meta:resourcekey="ListItemResource63" Text="PTO"></asp:ListItem>
                                               </asp:RadioButtonList>
                                             </td>
                                            </tr>
                                         </table>
                                         
                                     
                                      </td>
                                   </tr>
                                </table>
                            </td>
                        </tr>
                        <tr height="20">
                            <td class="RegularText" style="width: 28px">
                            </td>
                            <td colspan="2" valign="top">
                                <table id="tblFleetMaintenance" runat="server" class="formtext" border="0" cellpadding="0"
                                    cellspacing="0" width="100%">
                                    <tr>
                                        <td class="tableheading" style="width: 2px">
                                        </td>
                                        <td class="tableheading">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 2px; height: 19px">
                                            <asp:Label ID="Label6" runat="server" CssClass="tableheading" meta:resourcekey="Label6Resource1" Text="Fleet:"></asp:Label></td>
                                        <td align="left" style="height: 19px">
                                            <asp:DropDownList ID="cboMaintenanceFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                Width="249px" DataTextField="FleetName" DataValueField="FleetId" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboMaintenanceFleetResource1">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 2px; height: 13px">
                                        </td>
                                        <td style="height: 13px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 2px; height: 19px">
                                            <asp:Label ID="Label7" runat="server" CssClass="tableheading" meta:resourcekey="Label7Resource1" Text="Format:"></asp:Label></td>
                                        <td align="left" style="height: 19px">
                                            <asp:DropDownList ID="cboFleetReportFormat" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                Width="249px" meta:resourcekey="cboFleetReportFormatResource1">
                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource54" Text="Crystal"></asp:ListItem>
                                                <asp:ListItem Selected="True" Value="1" meta:resourcekey="ListItemResource55" Text="PDF"></asp:ListItem>
                                                <asp:ListItem Value="2" meta:resourcekey="ListItemResource56" Text="Excel"></asp:ListItem>
                                                <asp:ListItem Value="3" meta:resourcekey="ListItemResource57" Text="Word"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="width: 2px; height: 14px">
                                        </td>
                                        <td align="left" style="height: 14px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" style="width: 2px; height: 25px">
                                        </td>
                                        <td align="right" style="width: 100px;">
                                            <asp:Button ID="cmdPreviewFleetMaintenanceReport" runat="server" CausesValidation="False"
                                                CssClass="combutton" OnClick="cmdPreviewFleetMaintenanceReport_Click" Text="Preview" meta:resourcekey="cmdPreviewFleetMaintenanceReportResource1" />
                                            &nbsp;&nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 48px">
                            </td>
                            <td style="width: 300px">
                                &nbsp; &nbsp; &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="5">
                                <table width="100%" id="tblGeneralCriteria" runat="server">
                                    <tr>
                                        <td align="left" class="tableheading" style="width: 28px">
                                        </td>
                                        <td align="left" class="tableheading" style="width: 52px">
                                        </td>
                                        <td style="width: 312px">
                                            <asp:Label ID="Label1" runat="server" CssClass="formtext" Width="172px" Font-Bold="True" meta:resourcekey="Label1Resource1" Text="Day"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label
                                                ID="Label2" runat="server" CssClass="formtext" Width="44px" Font-Bold="True" meta:resourcekey="Label2Resource1" Text="Time"></asp:Label></td>
                                        <td align="left" class="tableheading" style="width: 32px">
                                        </td>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" CssClass="formtext" Width="175px" Font-Bold="True" meta:resourcekey="Label4Resource1" Text="Day"></asp:Label>
                                            <asp:Label ID="Label5" runat="server" CssClass="formtext" Width="52px" Font-Bold="True" meta:resourcekey="Label5Resource1" Text="Time"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="tableheading" style="width: 28px" align="left">
                                        </td>
                                        <td class="tableheading" style="width: 52px" align="left">
                                            <asp:Label ID="lblFromTitle3" runat="server" CssClass="tableheading" meta:resourcekey="lblFromTitle3Resource1" Text="From:"></asp:Label>
                                            <asp:RequiredFieldValidator ID="valFromDate" runat="server" ControlToValidate="txtFrom"
                                                ErrorMessage="Please select a From" meta:resourcekey="valFromDateResource1" Text="*"></asp:RequiredFieldValidator><asp:CompareValidator
                                                    ID="valCompareDates" runat="server" CssClass="errortext" ControlToValidate="txtTo"
                                                    ErrorMessage="The From Date should be earlier than the To Date!" Enabled="False"
                                                    Type="Date" Operator="GreaterThanEqual" ControlToCompare="txtFrom" meta:resourcekey="valCompareDatesResource1" Text="*"></asp:CompareValidator></td>
                                        <td style="width: 312px">
                                            <asp:TextBox ID="txtFrom" runat="server" CssClass="RegularText" Width="172px" ReadOnly="True" meta:resourcekey="txtFromResource1"></asp:TextBox><a
                                                onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtFrom','cal','width=220,height=200,left=270,top=380')"
                                                href="javascript:;"><img src="../images/SmallCalendar.gif" border="0"></a>&nbsp;
                                            <asp:DropDownList ID="cboHoursFrom" runat="server" CssClass="RegularText" Width="60px"
                                                Height="14px" meta:resourcekey="cboHoursFromResource1">
                                            </asp:DropDownList></td>
                                        <td class="tableheading" style="width: 32px" align="left">
                                            <asp:Label ID="lblToTitle3" runat="server" CssClass="tableheading" meta:resourcekey="lblToTitle3Resource1" Text="To:"></asp:Label>
                                            <asp:RequiredFieldValidator ID="valToDate" runat="server" ControlToValidate="txtTo"
                                                ErrorMessage="Please select a To" meta:resourcekey="valToDateResource1" Text="*"></asp:RequiredFieldValidator></td>
                                        <td>
                                            <asp:TextBox ID="txtTo" runat="server" CssClass="RegularText" Width="173px" ReadOnly="True" meta:resourcekey="txtToResource1"></asp:TextBox><a
                                                onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtTo','cal','width=220,height=200,left=570,top=380')"
                                                href="javascript:;"><img src="../images/SmallCalendar.gif" border="0"></a>&nbsp;
                                            <asp:DropDownList ID="cboHoursTo" runat="server" CssClass="RegularText" Width="59px"
                                                Height="14px" meta:resourcekey="cboHoursToResource1">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 28px">
                                        </td>
                                        <td style="width: 52px">
                                        </td>
                                        <td style="width: 312px">
                                        </td>
                                        <td style="width: 32px">
                                        </td>
                                        <td style="width: 300px; height: 25px">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tableheading" style="width: 28px; height: 14px">
                                        </td>
                                        <td class="tableheading" style="width: 52px; height: 14px">
                                            <asp:Label ID="lblFleet" runat="server" CssClass="tableheading" Width="33px" meta:resourcekey="lblFleetResource1" Text="Fleet:"></asp:Label><asp:RangeValidator
                                                ID="valFleet" runat="server" ControlToValidate="cboFleet" ErrorMessage="Please select a Fleet"
                                                MinimumValue="1" MaximumValue="999999999999999" meta:resourcekey="valFleetResource1" Text="*"></asp:RangeValidator></td>
                                        <td style="width: 312px; height: 14px">
                                            <asp:DropDownList ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                Width="258px" DataTextField="FleetName" DataValueField="FleetId" DESIGNTIMEDRAGDROP="79"
                                                OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1">
                                            </asp:DropDownList></td>
                                        <td class="formtext" style="width: 32px">
                                            <asp:Label ID="lblVehicleName" runat="server" CssClass="tableheading" Width="53px"
                                                Visible="False" meta:resourcekey="lblVehicleNameResource1" Text=" Vehicle:"></asp:Label></td>
                                        <td style="width: 300px">
                                            <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" Width="252px"
                                                DataTextField="Description" DataValueField="LicensePlate" Visible="False" DESIGNTIMEDRAGDROP="79" meta:resourcekey="cboVehicleResource1">
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td class="formtext" style="width: 28px; height: 25px">
                                        </td>
                                        <td class="formtext" style="width: 52px; height: 25px">
                                        </td>
                                        <td style="width: 312px; height: 25px">
                                        </td>
                                        <td style="width: 32px; height: 25px;">
                                        </td>
                                        <td style="width: 300px; height: 25px" align="left">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tableheading" style="width: 28px; height: 9px">
                                        </td>
                                        <td class="tableheading" style="width: 52px; height: 9px">
                                            <asp:Label ID="lblFormatTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblFormatTitleResource1" Text="Format:"></asp:Label></td>
                                        <td style="width: 312px; height: 9px">
                                            <asp:DropDownList ID="cboFormat" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                Width="258px" meta:resourcekey="cboFormatResource1">
                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource58" Text="Crystal"></asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True" meta:resourcekey="ListItemResource59" Text="PDF"></asp:ListItem>
                                                <asp:ListItem Value="2" meta:resourcekey="ListItemResource60" Text="Excel"></asp:ListItem>
                                                <asp:ListItem Value="3" meta:resourcekey="ListItemResource61" Text="Word"></asp:ListItem>
                                            </asp:DropDownList></td>
                                        <td style="width: 32px">
                                        </td>
                                        <td style="width: 300px; height: 9px" align="left">
                                            <asp:Button ID="cmdShow" runat="server" CssClass="combutton" Text="Preview" OnClick="cmdShow_Click" meta:resourcekey="cmdShowResource1">
                                            </asp:Button>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="tableheading" style="width: 28px; height: 20px">
                                        </td>
                                        <td class="tableheading" style="width: 52px; height: 20px">
                                        </td>
                                        <td style="width: 312px; height: 20px">
                                        </td>
                                        <td style="width: 32px; height: 20px;">
                                        </td>
                                        <td style="width: 300px; height: 20px" align="left">
                                            <asp:Label ID="lblReportFormat" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblReportFormatResource1"></asp:Label></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr height="25">
                            <td style="width: 100%" align="center" colspan="5">
                                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1"></asp:ValidationSummary>
                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="240px" Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="left" >
                    <table id="tblDesc" bordercolor="gray" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td class="tableheading" style="width: 6px" height="15">
                            </td>
                            <td class="tableheading" align="left" height="15">
                                <b>
                                    <asp:Label ID="lblReportDescTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblReportDescTitleResource1" Text="Report Description:"></asp:Label></b>
                            </td>
                        </tr>
                        <tr>
                            <td >
                            </td>
                            <td >
                                <asp:Label ID="lblTripReportDesc" runat="server" CssClass="formtext" meta:resourcekey="lblTripReportDescResource1" Text="This report provides details about vehicle trips in a specified period of time. Each trip is determined by ignition on/off showing trip start, trip end, vehicle position info and any sensors that have been triggered. This report can be customized to include/exclude street address, sensor triggers, scheduled position reports, idling time, stored position and trip summary. It can also be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label><asp:Label
                                    ID="lblHistoryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblHistoryReportDescResource1" Text="This report summarizes all the activities occurred in the system for a particular vehicle during the selected period of time, including IP address updates, sensor triggers, commands, outputs, position updates, scheduled position reports, MDT text messages and Geo Fence violations. This report can be customized to include/exclude sensor triggers, scheduled GPS coordinates and invalid GPS positions. This report can be filtered by date/time and generated for a single vehicle only."></asp:Label><asp:Label
                                        ID="lblMessageReportDescription" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblMessageReportDescriptionResource1" Text="This report provides list of text messages sent and received by the system in the selected period of time, including From, To, direction of a message, message text and responses. It can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label><asp:Label
                                            ID="lblTripSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblTripSummaryReportDescResource1" Text="This report summarizes each trip determined by ignition on/off showing departure address and time, arrival address and time, distance traveled, trip time, idling time and stop time for each trip. It also totals the number of trips, trip time, idling time, stop time, and distance traveled in the selected period of time. This report can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label><asp:Label
                                                ID="lblAlarmReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblAlarmReportDescResource1" Text="This report summarizes all the security alarms that occurred in the system during the selected period of time. It can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label><asp:Label
                                                    ID="lblStopReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblStopReportDescResource1" Text="This report lists all the stops including idling showing arrival time, street address, departure time and stop duration in the selected period of time. It also totals the number of stops and stop time. This report can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                                     <asp:Label ID="lblLandmarkActivityReportDesc" runat="server" CssClass="formtext"
                                    Visible="False" meta:resourcekey="lblLandmarkActivityReportDescResource1" Text="This report provides  a summary of total time spent by a vehicle at every landmark, if any"></asp:Label>
                                    
                                    <asp:Label ID="lblOffHoursReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblOffHoursReportDescResource1" Text="This report list all the vehicles that were used after hours."></asp:Label>
                                <asp:Label ID="lblFleetMaintenanceReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetMaintenanceReportDescResource1" Text="Provides a report on vehicle maintenance including current odometer readings, and whether maintenance has been recently performed, is due or is overdue." ></asp:Label>
                                <asp:Label ID="lblFleetViolationDetailsReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetViolationDetailsReportDescResource1" Text="Provides a report on the occurrence of various driving violations including:  Speed Violation, Harsh Acceleration, Harsh Braking, Extreme Acceleration, Extreme Braking and Seat Belt Violation.  The User can choose which violations and period of time on which to report.  A report can be generated for a single vehicle or for an entire fleet." ></asp:Label>
                                <asp:Label ID="lblFleetViolationSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetViolationSummaryReportDescResource1" Text="This report provides a summary of violations that have occurred for a specified period of time by assigning configurable demerit point values for each type of violation including: Speed Violation, Harsh Acceleration, Harsh Braking, Extreme Acceleration, Extreme Braking and Seat Belt Violation.  The type of violations to be reported on and the number of demerit points for different types of violations can be specified by the User in the report screen.  The report multiplies the number of violation occurrences by the assigned demerit point value to generate a total violation demerit score for each vehicle.  The total score is colour coded to indicated the severity of the violation demerit score.  A report can be generated for a single vehicle or for an entire fleet." ></asp:Label>
                                <asp:Label ID="lblIdlingDetailsReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblIdlingDetailsReportDescResource1" Text="This report summarizes the total number of hours that a vehicle ignition is on, the number of hours the vehicle ignition is on and engine is idling (no vehicle movement) and the percentage of time the vehicle is idling (idling time divided by total ignition on time).  The User can choose the period of time on which to report.  A report can be generated for a single vehicle or for an entire fleet." ></asp:Label>
                                <asp:Label ID="lblIdlingSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblIdlingSummaryReportDescResource1" Text="A summary report on Engine Idling details created for a fleet of vehicles for a selected period.  The report displays the total number of hours for the entire fleet when vehicle ignition is on, total number of hours when vehicle ignition is on and engine is idling (no vehicle movement), the percentage of time all vehicles are idling (idling time divided by total ignition on time) and the average idling time per vehicle." ></asp:Label>
                                
                                                    
                                        </td>
                        </tr>
                        
                    </table>
                    <a href="http://www.adobe.com/products/acrobat/readermain.html" target="top">
                        <img height="31" src="../images/get_adobe_reader.gif" align="right" border="0"></a>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
