<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportMasterExtended.aspx.cs"
    Inherits="SentinelFM.Reports_frmReportMasterExtended" Culture="en-US" meta:resourcekey="PageResource1"
    UICulture="auto" %>

<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>
<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
    TagPrefix="ISWebInput" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    <style type="text/css">
        .formtext
        {
            margin-right: 0px;
        }
        .style1
        {
            margin-right: 0px;
            width: 256px;
        }
        .style2
        {
            height: 26px;
        }
        .style3
        {
            width: 100px;
            height: 26px;
        }
    </style>
    </head>
<body>
    <form id="frmReportMaster" method="post" runat="server">
        <div  style=" z-index: 101;left: 12px;position: absolute; top: 4px; height: 97%; width: 98%; background-color: white">
            <table id="Table4" width="100%" border="0">
                <tr>
                    <td height="25px">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td style="height: 80%; width: 90%;" align="center">
                        <table id="Table1" class="tableDoubleBorder"  style="width: 751px;height: 350px;" cellspacing="0" cellpadding="0" border="0">
                            
                            <tr>
                                <td colspan="5" style="height: 295px">
                                    <table class="formtext">
                                        <tr>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 100px">
                                                <asp:Label ID="lblReportTitle" runat="server" CssClass="heading" meta:resourcekey="lblReportTitleResource1"
                                                    Text="Report:"></asp:Label></td>
                                            <td  COLSPAN=3>
                                    <asp:DropDownList ID="cboReports" runat="server" AutoPostBack="True" CssClass="RegularText"
                                        DataTextField="GuiName" DataValueField="GuiId" OnSelectedIndexChanged="cboReports_SelectedIndexChanged"
                                        meta:resourcekey="cboReportsResource1" Width="100%">
                                    </asp:DropDownList></td>
                                            
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">
                                                &nbsp;
                                            </td>
                                            <td colspan="4" align="left">
                                                <table cellpadding="2" id="tblSpeedViolation" runat="server">
                                                    <tr>
                                                        <td >
                                                            <asp:Label ID="lblSpeedViolation" runat="server" Text="Speed:" 
                                                                CssClass="formtext"></asp:Label></td>
                                                        <td >
                                                            <asp:DropDownList ID="cboViolationSpeed" runat="server" CssClass="formtext">
                                                                <asp:ListItem Value="1">For Canada</asp:ListItem>
                                                                <asp:ListItem Value="2">For USA</asp:ListItem>
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                </table>
                                                <table id="tblCost" runat="server">
                                                    <tr>
                                                        <td >
                                                            <asp:Label ID="lblCost" runat="server" CssClass="formtext" Text="Cost of Unnecessary Idling ($ per Hr):"></asp:Label></td>
                                                        <td style="width: 20px">
                                                            <asp:TextBox ID="txtCost" runat="server" CssClass="formtext" Width="30px">1</asp:TextBox></td>
                                                    </tr>
                                                </table>



                                                  <table id="tblFuelCost" runat="server">
                                                    <tr>
                                                        <td >
                                                            <asp:Label ID="Label2" runat="server" CssClass="formtext" Text="Fuel Cost ($ per Liter):"></asp:Label></td>
                                                        <td style="width: 20px">
                                                            <asp:TextBox ID="txtFuelCost" runat="server" CssClass="formtext" Width="30px">1</asp:TextBox></td>
                                                    </tr>
                                                </table>
                                                
                                                
                                                <table id="tblFilter" runat="server">
                                                    <tr>
                                                        <td >
                                                            <asp:Label ID="Label1" runat="server" CssClass="formtext" Text="Color:"></asp:Label></td>
                                                        <td style="width: 20px">
                                                            <asp:TextBox ID="txtColorFilter" runat="server" CssClass="formtext"></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                                
                                                
                                                 <table id="tblIgnition" runat="server" border="0" cellpadding="0" cellspacing="0"
                                                   >
                                                    <tr>
                                                        <td class="formtext" colspan="2" align="left">
                                                            <table>
                                                                <tr>
                                                                    <td class="formtext">
                                                                        <asp:Label ID="lblIgnition" runat="server" meta:resourcekey="lblIgnitionResource1"
                                                                            Text="Calculate Trips based on:"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left">
                                                                        <asp:RadioButtonList ID="optEndTrip" runat="server" CssClass="formtext" meta:resourcekey="optEndTripResource1">
                                                                            <asp:ListItem Selected="True" Text="Ignition" Value="3" meta:resourcekey="ListItemResource60"></asp:ListItem>
                                                                            <asp:ListItem Text="Tractor Power" Value="11" meta:resourcekey="ListItemResource61"></asp:ListItem>
                                                                            <asp:ListItem Value="8" meta:resourcekey="ListItemResource62">PTO</asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                
                                                
                                                
                                                 <table id="tblViolationReport" cellspacing="0"  class="formtext" cellpadding="0" border="0" runat="server">
                                                    <tr>
                                                        <td colspan="2" class="style1">
                                                            <table class="formtext" border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <asp:CheckBox ID="chkSpeedViolation" runat="server" Text="Speed Violation" Checked="True"
                                                                            meta:resourcekey="chkSpeedViolationResource1" /></td>
                                                                    <td>
                                                                        &nbsp;&nbsp;&nbsp;
                                                                    </td>
                                                                    <td>
                                                                        <asp:DropDownList ID="DropDownList1" runat="server" CssClass="formtext">
                                                                            <asp:ListItem Value="1">100 kph (62 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="2">105 kph (65 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="3">110 kph (68 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="4">120 kph (75 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="5">130 kph (80 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="6">140 kph (90 mph)</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <asp:CheckBox ID="chkHarshAcceleration" runat="server" Text="Harsh Acceleration"
                                                                Checked="True" meta:resourcekey="chkHarshAccelerationResource1" /><br />
                                                            <asp:CheckBox ID="chkHarshBraking" runat="server" Text="Harsh Braking" Checked="True"
                                                                meta:resourcekey="chkHarshBrakingResource1" /><br />
                                                            <asp:CheckBox ID="chkExtremeAcceleration" runat="server" Text="Extreme Acceleration"
                                                                Checked="True" meta:resourcekey="chkExtremeAccelerationResource1" /><br />
                                                            <asp:CheckBox ID="chkExtremeBraking" runat="server" Text="Extreme Braking" Checked="True"
                                                                meta:resourcekey="chkExtremeBrakingResource1" /><br />
                                                            <asp:CheckBox ID="chkSeatBeltViolation" runat="server" Text="SeatBelt Violation"
                                                                Checked="True" meta:resourcekey="chkSeatBeltViolationResource1" /><br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                </table>



                                                <fieldset runat=server id="tblPoints" style="width: 300px"   class="formtext" >
                                                <table id="Table2" runat="server" class="formtext">
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed120" runat="server" CssClass="formtext" Text="Speed >" meta:resourcekey="lblSpeed120Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed120" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed120Resource1"
                                                                            Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="Label9" runat="server" CssClass="formtext" Text="Acc. Harsh" meta:resourcekey="Label9Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAccHarsh" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccHarshResource1"
                                                                            Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblBrakingExtreme" runat="server" CssClass="formtext" Text="Braking Extreme"
                                                                            meta:resourcekey="lblBrakingExtremeResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtBrakingExtreme" runat="server" CssClass="formtext" Width="30px"
                                                                            meta:resourcekey="txtBrakingExtremeResource1" Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed130" runat="server" Text="Speed >" meta:resourcekey="lblSpeed130Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed130" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed130Resource1"
                                                                            Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblAccExtreme" runat="server" Text="Acc. Extreme" meta:resourcekey="lblAccExtremeResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAccExtreme" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtAccExtremeResource1"
                                                                            Text="20"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSeatBelt" runat="server" Text="Seat Belt" meta:resourcekey="lblSeatBeltResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSeatBelt" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSeatBeltResource1"
                                                                            Text="50"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center" valign="top">
                                                            <table class="formtext">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblSpeed140" runat="server" Text="Speed >" meta:resourcekey="lblSpeed140Resource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSpeed140" runat="server" CssClass="formtext" Width="30px" meta:resourcekey="txtSpeed140Resource1"
                                                                            Text="50"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td align="center" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblBrakingHarsh" runat="server" CssClass="formtext" Text="Braking Harsh"
                                                                            meta:resourcekey="lblBrakingHarshResource1"></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtBrakingHarsh" runat="server" CssClass="formtext" Width="30px"
                                                                            meta:resourcekey="txtBrakingHarshResource1" Text="10"></asp:TextBox></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                                </fieldset>
                                                

                                                
                                                    <table id="tblDriverOptions" runat="server">
                                                        <tr>
                                                            <td >
                                                                <asp:Label ID="lblDriverCaption" runat="server" Text="Driver:" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="ddlDrivers" runat="server" CssClass="RegularText" EnableViewState="true"
                                                                    DataTextField="FullName" DataValueField="DriverId" Width="200px"  >
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>


                                                      <table id="tblIdlingThreshold" runat="server">
                                                        <tr>
                                                            <td >
                                                                <asp:Label ID="Label3" runat="server" Text="Idling Threshold:" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td>
                                                                 <asp:DropDownList ID="cboIdlingThreshold" runat="server" CssClass="formtext" 
                                                                     Width="140px">
                                                                            <asp:ListItem Value="-1">All</asp:ListItem>
                                                                            <asp:ListItem Value="1">More than 1 Hour</asp:ListItem>
                                                                            <asp:ListItem Value="2">More than 2 Hours</asp:ListItem>
                                                                            <asp:ListItem Value="3">More than 3 Hours</asp:ListItem>
                                                                        </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>



                                                    <table id="tblSpeedThreshold" runat="server">
                                                        <tr>
                                                            <td >
                                                                <asp:Label ID="Label4" runat="server" Text="Speed Threshold:" CssClass="formtext"></asp:Label>
                                                            </td>
                                                            <td>
                                                                 <asp:DropDownList ID="cboSpeedThreshold" runat="server" CssClass="formtext" 
                                                                     Width="140px">
                                                                              <asp:ListItem Value="100">100 kph (62 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="105">105 kph (65 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="110">110 kph (68 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="120">120 kph (75 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="125">125 kph (77 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="130">130 kph (80 mph)</asp:ListItem>
                                                                            <asp:ListItem Value="140">140 kph (90 mph)</asp:ListItem>
                                                                        </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                

                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" >
                                                                <asp:Label ID="lblLandmarkCaption" runat="server" 
                                                    Text="Landmark:" CssClass="tableheading"></asp:Label>
                                                            </td>
                                            <td >
                                                                <asp:DropDownList ID="ddlLandmarks" runat="server" 
                                                    CssClass="RegularText" EnableViewState="true"
                                                                    DataTextField="LandmarkName" DataValueField="LandmarkName" Width="258px" 
                                                    >
                                                                </asp:DropDownList>
                                            </td>
                                            <td align="left" style="width: 100px">
                                                </td>
                                            <td align="left" style="width: 100px">
                                               </td>
                                            <td >
                                                </td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="style2" >
                                                                <asp:Label ID="lblGeozoneCaption" runat="server" Text="Geozone:" CssClass="formtext"></asp:Label>
                                                            </td>
                                            <td class="style2" >
                                                                <asp:DropDownList ID="ddlGeozones" runat="server" CssClass="RegularText" EnableViewState="true"
                                                                    DataTextField="GeozoneName" DataValueField="GeozoneNo" Width="258px" >
                                                                </asp:DropDownList>
                                            </td>
                                            <td align="left" class="style3">
                                                </td>
                                            <td align="left" class="style3">
                                                </td>
                                            <td class="style2" >
                                                </td>
                                        </tr>
                                        <tr>
                                            <td align="left" >
                                               </td>
                                            <td >
                                               </td>
                                            <td align="left">
                                               </td>
                                            <td align="left" >
                                              </td>
                                            <td >
                                                </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 100px">
                                                <asp:Label ID="lblFromTitle3" runat="server" CssClass="tableheading" meta:resourcekey="lblFromTitle3Resource1"
                                                    Text="From:"></asp:Label>
                                                <asp:CompareValidator
                                                        ID="valCompareDates" runat="server" CssClass="errortext" ControlToValidate="txtTo"
                                                        ErrorMessage="The From Date should be earlier than the To Date!" Enabled="False"
                                                        Type="Date" Operator="GreaterThanEqual" ControlToCompare="txtFrom" meta:resourcekey="valCompareDatesResource1"
                                                        Text="*"></asp:CompareValidator>
                                                <asp:RequiredFieldValidator ID="valFromDate" runat="server" ControlToValidate="txtFrom"
                                                    ErrorMessage="Please select a From" meta:resourcekey="valFromDateResource1" Text="*"></asp:RequiredFieldValidator></td>
                                            <td style="width: 100px">
                                                <ISWebInput:WebInput ID="txtFrom" runat="server" Width="258px" Height="17px" Wrap="Off">
                                                                <HighLight IsEnabled="True" Type="Phrase" />
                                                                <DateTimeEditor IsEnabled="True" AccessKey="Space">
                                                                </DateTimeEditor>
                                                            </ISWebInput:WebInput>
                                            </td>
                                            <td align="left" style="width: 100px">
                                            </td>
                                            <td align="left" style="width: 100px">
                                                <asp:Label ID="lblToTitle3" runat="server" CssClass="tableheading" meta:resourcekey="lblToTitle3Resource1"
                                                    Text="To:"></asp:Label>
                                                <asp:RequiredFieldValidator ID="valToDate" runat="server" ControlToValidate="txtTo"
                                                    ErrorMessage="Please select a To" meta:resourcekey="valToDateResource1" Text="*"></asp:RequiredFieldValidator></td>
                                            <td style="width: 100px">
                                                <ISWebInput:WebInput ID="txtTo" runat="server" Width="258px" Height="17px" Wrap="Off">
                                                                <HighLight IsEnabled="True" Type="Phrase" />
                                                                <DateTimeEditor IsEnabled="True" AccessKey="Space">
                                                                </DateTimeEditor>
                                                            </ISWebInput:WebInput>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" style="width: 100px">
                                                <asp:Label ID="lblFleet" runat="server" CssClass="tableheading" Width="33px" meta:resourcekey="lblFleetResource1"
                                                    Text="Fleet:"></asp:Label><asp:RangeValidator ID="valFleet" runat="server" ControlToValidate="cboFleet"
                                                        ErrorMessage="Please select a Fleet" MinimumValue="1" MaximumValue="999999999999999"
                                                        meta:resourcekey="valFleetResource1" Text="*" Enabled="False"></asp:RangeValidator></td>
                                            <td style="width: 100px">
                                                <asp:DropDownList ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                    Width="258px" DataTextField="FleetName" DataValueField="FleetId"
                                                    OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1">
                                                </asp:DropDownList></td>
                                            <td align="left" style="width: 100px">
                                            </td>
                                            <td align="left" style="width: 100px">
                                                <asp:Label ID="lblVehicleName" runat="server" CssClass="tableheading" Width="53px"
                                                    Visible="False" meta:resourcekey="lblVehicleNameResource1" Text="Vehicle:"></asp:Label></td>
                                            <td style="width: 100px">
                                                <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" Width="258px"
                                                    DataTextField="Description" DataValueField="VehicleId" Visible="False" 
                                                    meta:resourcekey="cboVehicleResource1">
                                                </asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td align=left   >
                                                                        <asp:CheckBox ID="chkActiveVehicles" runat="server" 
                                                    Text="Active Vehicles Only" Checked="True" CssClass="formtext"/>
                                            </td>
                                        </tr>
                                        
                                        
                                         <tr>
                                            <td style="width: 100px">
                                            </td>
                                            <td colspan="4">
					<TABLE id="tblFleets" cellSpacing="0" cellPadding="0" border="0" runat="server">
						<TR>
							<TD colspan="2" class="formtext" style="WIDTH: 240px">
                         <asp:Label ID="lblAllFleets" runat="server" 
                             Text="All Fleets"></asp:Label></TD>
							
							<TD class="formtext">
                         <asp:Label ID="lblSelectedFleets" runat="server" 
                             Text="Selected fleets"></asp:Label></TD>
						</TR>
						<TR>
							<TD style="WIDTH: 110px" vAlign=top>
								<asp:listbox id="lstUnAss" DataValueField="FleetId" DataTextField="FleetName" CssClass="formtext"
									Width="200px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstUnAssResource1"></asp:listbox></TD>
							<TD style="WIDTH: 130px" align="center" vAlign=top>
								<TABLE id="tblAddRemoveBtns" style="WIDTH: 75px; HEIGHT: 99px" cellSpacing="0" cellPadding="0"
									width="75" border="0" runat="server">
									<TR>
										<TD vAlign=middle>
											<asp:button id="cmdAdd" CssClass="combutton" runat="server" Text="Add->"  onclick="cmdAdd_Click" ></asp:button></TD>
									</TR>
									<TR>
										<TD style="HEIGHT: 20px"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdAddAll" CssClass="combutton" runat="server" Text="Add All->" onclick="cmdAddAll_Click" ></asp:button></TD>
									</TR>
									<TR>
										<TD id="TD1" style="HEIGHT: 20px" runat="server"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdRemove" CssClass="combutton" runat="server" Text="<-Remove"  onclick="cmdRemove_Click" ></asp:button></TD>
									</TR>
									<TR>
										<TD style="HEIGHT: 20px"></TD>
									</TR>
									<TR>
										<TD>
											<asp:button id="cmdRemoveAll" CssClass="combutton" runat="server" Text="<-Remove All"  onclick="cmdRemoveAll_Click" ></asp:button></TD>
									</TR>
								</TABLE>
							</TD>
							<TD vAlign=top>
								<asp:listbox id="lstAss" DataValueField="FleetId" DataTextField="FleetName" CssClass="formtext"
									Width="200px" runat="server" SelectionMode="Multiple" Rows="15" DESIGNTIMEDRAGDROP="46" ></asp:listbox></TD>
						</TR>
					</TABLE>
                                            </td>
                                        </tr>
                                        
                                        
                                         <tr>
                                            <td style="width: 100px">
                                                &nbsp;</td>
                                            <td colspan="4">
					                            &nbsp;</td>
                                        </tr>
                                        
                                        <tr>
                                            <td align="left" style="width: 100px">
                                                <asp:Label ID="lblFormatTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblFormatTitleResource1"
                                                    Text="Format:"></asp:Label></td>
                                            <td style="width: 100px">
                                                <asp:DropDownList ID="cboFormat" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                    Width="258px" meta:resourcekey="cboFormatResource1">
                                                    <asp:ListItem Value="1" Selected="True" meta:resourcekey="ListItemResource57" Text="PDF"></asp:ListItem>
                                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource58" Text="Excel"></asp:ListItem>
                                                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource59" Text="Word"></asp:ListItem>
                                                </asp:DropDownList></td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                                </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                        </tr>
                                       
                                        <tr>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                        </tr>
                                    </table>
                                    <table style="width: 584px">
                                        <tr>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                            <td style="width: 100px">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 100px">
                                                <asp:Button ID="cmdShow" runat="server" CssClass="combutton" meta:resourcekey="cmdShowResource1"
                                                    OnClick="cmdShow_Click" Text="Preview" Width="178px" /></td>
                                            <td style="width: 100px">
                                                <asp:Button ID="cmdSchedule" runat="server" CssClass="combutton" meta:resourcekey="cmdScheduleResource1"
                                                    OnClick="cmdSchedule_Click" Text="Schedule Report" Width="178px" /></td>
                                            <td style="width: 100px">
                                                <asp:Button ID="cmdViewScheduled" runat="server" CausesValidation="False" CssClass="combutton"
                                                    meta:resourcekey="cmdViewScheduledResource1" OnClick="cmdViewScheduled_Click"
                                                    Text="View Schedule Reports" Width="178px" /></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%" height="25" align="center" colspan="5">
                                    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1">
                                    </asp:ValidationSummary>
                                                <busyboxdotnet:BusyBox ID="BusyReport" runat="server" AnchorControl="" meta:resourcekey="BusyReportResource1"
                                                    ShowBusyBox="Custom" SlideEasing="BackBoth" Text="Preparing the Report" CompressScripts="False"
                                                    GZipCompression="False"></busyboxdotnet:BusyBox>
                                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="240px" Visible="False"
                                        meta:resourcekey="lblMessageResource1"></asp:Label></td>
                            </tr>
                            <tr>
                                <td align="left" colspan="5" height="25" style="width: 100%">
                                    <table id="tblDesc" align="left" width="761px" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td class="tableheading" style="width: 6px" height="30">
                                            </td>
                                            <td class="tableheading" align="left" height="30">
                                                <b>
                                                    <asp:Label ID="lblReportDescTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblReportDescTitleResource1"
                                                        Text="Report Description:"></asp:Label></b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 6px">
                                            </td>
                                            <td>
                                                <asp:Label ID="LabelReportDescription" runat="server" CssClass="formtext"></asp:Label>
                                                <!--
                                            <asp:Label ID="lblTripReportDesc" runat="server" CssClass="formtext" meta:resourcekey="lblTripReportDescResource1" Text="This report provides details about vehicle trips in a specified period of time. Each trip is determined by ignition on/off showing trip start, trip end, vehicle position info and any sensors that have been triggered. This report can be customized to include/exclude street address, sensor triggers, scheduled position reports, idling time, stored position and trip summary. It can also be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblHistoryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblHistoryReportDescResource1" Text="This report summarizes all the activities occurred in the system for a particular vehicle during the selected period of time, including IP address updates, sensor triggers, commands, outputs, position updates, scheduled position reports, MDT text messages and Geo Fence violations. This report can be customized to include/exclude sensor triggers, scheduled GPS coordinates and invalid GPS positions. This report can be filtered by date/time and generated for a single vehicle only."></asp:Label>
                                            <asp:Label ID="lblMessageReportDescription" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblMessageReportDescriptionResource1" Text="This report provides list of text messages sent and received by the system in the selected period of time, including From, To, direction of a message, message text and responses. It can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblTripSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblTripSummaryReportDescResource1" Text="This report summarizes each trip determined by ignition on/off showing departure address and time, arrival address and time, distance traveled, trip time, idling time and stop time for each trip. It also totals the number of trips, trip time, idling time, stop time, and distance traveled in the selected period of time. This report can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblAlarmReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblAlarmReportDescResource1" Text="This report summarizes all the security alarms that occurred in the system during the selected period of time. It can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblStopReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblStopReportDescResource1" Text="This report lists all the stops including idling showing arrival time, street address, departure time and stop duration in the selected period of time. It also totals the number of stops and stop time. This report can be filtered by date/time and generated for a single vehicle or an entire fleet."></asp:Label>
                                            <asp:Label ID="lblLandmarkActivityReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblLandmarkActivityReportDescResource1" Text="This report provides  a summary of total time spent by a vehicle at every landmark, if any"></asp:Label>
                                            <asp:Label ID="lblOffHoursReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblOffHoursReportDescResource1" Text="This report list all the vehicles that were used after hours."></asp:Label>                    
                                            <asp:Label ID="lblFleetMaintenanceReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetMaintenanceReportDescResource1" Text="Provides a report on vehicle maintenance including current odometer readings, and whether maintenance has been recently performed, is due or is overdue." ></asp:Label>
                                            <asp:Label ID="lblFleetViolationDetailsReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetViolationDetailsReportDescResource1" Text="Provides a report on the occurrence of various driving violations including:  Speed Violation, Harsh Acceleration, Harsh Braking, Extreme Acceleration, Extreme Braking and Seat Belt Violation.  The User can choose which violations and period of time on which to report.  " ></asp:Label>
                                            <asp:Label ID="lblFleetViolationSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblFleetViolationSummaryReportDescResource1" Text="This report provides a summary of violations that have occurred for a specified period of time by assigning configurable demerit point values for each type of violation including: Speed Violation, Harsh Acceleration, Harsh Braking, Extreme Acceleration, Extreme Braking and Seat Belt Violation.  The type of violations to be reported on and the number of demerit points for different types of violations can be specified by the User in the report screen.  The report multiplies the number of violation occurrences by the assigned demerit point value to generate a total violation demerit score for each vehicle.  The total score is colour coded to indicated the severity of the violation demerit score. " ></asp:Label>
                                            <asp:Label ID="lblIdlingDetailsReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblIdlingDetailsReportDescResource1" Text="This report summarizes the total number of hours that a vehicle ignition is on, the number of hours the vehicle ignition is on and engine is idling (no vehicle movement) and the percentage of time the vehicle is idling (idling time divided by total ignition on time).  The User can choose the period of time on which to report." ></asp:Label>
                                            <asp:Label ID="lblIdlingSummaryReportDesc" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblIdlingSummaryReportDescResource1" Text="A summary report on Engine Idling details created for a fleet of vehicles for a selected period.  The report displays the total number of hours for the entire fleet when vehicle ignition is on, total number of hours when vehicle ignition is on and engine is idling (no vehicle movement), the percentage of time all vehicles are idling (idling time divided by total ignition on time) and the average idling time per vehicle." ></asp:Label>
                                            -->
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 20px">
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left" height="100" style="width: 50%;">
                        <a href="http://www.adobe.com/products/acrobat/readermain.html" target="top">
                            <img height="31" src="../images/get_adobe_reader.gif" align="right" alt="Adobe Reader"
                                border="0" />
                        </a>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
