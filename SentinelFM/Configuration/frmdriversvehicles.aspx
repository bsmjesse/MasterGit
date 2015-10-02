<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmdriversvehicles.aspx.cs" Inherits="SentinelFM.Configuration_frmdriversvehicles" Culture="en-CA" meta:resourcekey="PageResource1" UICulture="en-CA" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Drivers Assignment</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    <link href="Configuration.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table id="tblCommands" cellpadding="0" cellspacing="0" style="z-index: 101; left: 8px; position: absolute; top: 4px; width:300">
                <tr>
                    <td>
                         <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdDriver" FleetUrl = "frmfleets.aspx"  VehicleUrl="frmfleetvehicles.aspx" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                            <tr>
                                <td>
                                    <table id="tblForm" border="0" cellpadding="0" cellspacing="0" style="width:990px; height:550px" class="frame">
                                        <tr>
                                            <td class="configTabBackground">
                                                <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                    <tr>
                                                        <td>
                                                            <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                                <tr style="height:12px"><td></td></tr>
                                                                <tr>
                                                                    <td>
                                                                        <table id="tblDriverAssgn" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
                                                                            width: 190px; position: relative; top: 0px; height: 22px">
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Button ID="cmdDriversInfo" runat="server" CausesValidation="False" CommandName="4"
                                                                                        CssClass="confbutton" Text="Drivers Info" Width="112px" PostBackUrl="~/Configuration/frmdrivers.aspx" meta:resourcekey="cmdDriversInfoResource1" />
                                                                                      </td>
                                                                                <td>
                                                                                    <asp:Button ID="cmdVehiclesDrivers" runat="server" CausesValidation="False" CommandName="5"
                                                                                        CssClass="selectedbutton" Text="Vehicles-Drivers Assignment" Width="186px" meta:resourcekey="cmdVehiclesDriversResource1" />
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                        <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                            <tr>
                                                                                <td style="width: 961px">
                                                                                    <table id="Table7" border="0" cellpadding="0" cellspacing="0" width="960px" class="table" style="border-bottom: gray 2px outset;">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <table id="tbl" border="0" cellpadding="0" cellspacing="0" style="width: 98%; text-align:center">
                                                                                                    <tr>
                                                                                                        <td align="center" style="width: 100%; height: 520px;" valign="top">
                                                                                                            <asp:Panel ID="pnlAssgnType" runat="server" HorizontalAlign="Center" Width="100%" CssClass="height:auto" meta:resourcekey="pnlAssgnTypeResource1">
                                                                                                            <br />
                                                                                                            <asp:DropDownList ID="ddlAssgnType" runat="server" CssClass="RegularText" OnSelectedIndexChanged="ddlAssgnType_SelectedIndexChanged" AutoPostBack="True" meta:resourcekey="ddlAssgnTypeResource1">
                                                                                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource1" Text="Please select assignment type"></asp:ListItem>
                                                                                                                <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="Current assignment"></asp:ListItem>
                                                                                                                <asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="Postfactum assignment"></asp:ListItem>
                                                                                                            </asp:DropDownList>
                                                                                                            <br />
                                                                                                            <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Height="20px" Width="709px" meta:resourcekey="lblMessageResource1"></asp:Label>
                                                                                                            </asp:Panel>
                                                                                                            
                                                                                                            <asp:Panel ID="pnlAssignment" runat="server" Width="100%" Visible="False" CssClass="height:auto" Direction="LeftToRight" HorizontalAlign="Center" meta:resourcekey="pnlAssignmentResource1">
                                                                                                                <br />
                                                                                                                <div id="tblFleetVehicle1" style="width:600px;" runat="server">
                                                                                                                <div id="divFleet" style="float: left;" runat="server">
                                                                                                                    <asp:Label runat="server" ID="lblFleet" CssClass="RegularText" EnableViewState="False" Width="72px" meta:resourcekey="lblFleetResource1" Text="Fleet: "></asp:Label>
                                                                                                                    <asp:DropDownList ID="ddlFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                                                                                        DataTextField="FleetName" DataValueField="FleetId" Width="200px" OnSelectedIndexChanged="ddlFleet_SelectedIndexChanged" meta:resourcekey="ddlFleetResource1"></asp:DropDownList>
                                                                                                                </div>
                                                                                                                <div id="divVehicle" runat="server" style="float: right; position:relative; top: 0px; left: 0px;" visible="False">
                                                                                                                    <asp:Label runat="server" ID="lblVehicle" CssClass="RegularText" Width="72px" EnableViewState="False" meta:resourcekey="lblVehicleResource1" Text="Vehicle: "></asp:Label>
                                                                                                                    <asp:DropDownList ID="ddlVehicle" runat="server" DataValueField="VehicleId" DataTextField="Description" AutoPostBack="True" Width="200px" OnSelectedIndexChanged="ddlVehicle_SelectedIndexChanged" meta:resourcekey="ddlVehicleResource1" CssClass="RegularText">
                                                                                                                    </asp:DropDownList>
                                                                                                                </div>
                                                                                                                </div>
                                                                                                                <table style="width: 600px;" id="tblAssignment" runat="server">                                                                                                                
                                                                                                                    <tr runat="server">
                                                                                                                        <td align="center" style="width: 313px; height: 276px;" runat="server">
                                                                                                                            <asp:Label ID="lblUnUsers" runat="server" Text="Unassigned Drivers" Width="200px" CssClass="RegularText" meta:resourcekey="lblUnUsersResource1" Height="20px"></asp:Label>
                                                                                                                            <asp:ListBox ID="lboUnassigned" runat="server" CssClass="formtext" Rows="5" Width="200px" meta:resourcekey="lboUnassignedResource1" Height="230px" ></asp:ListBox>
                                                                                                                        </td>
                                                                                                                        <td style="width: 204px; text-align:center; vertical-align:middle; height: 276px;" runat="server">
                                                                                                                            <ul id="tblAddRemoveBtns" runat="server" style="list-style:none; position:relative; top: 10px; left:-20px">
                                                                                                                                <li style="width:auto">
                                                                                                                                    <asp:Button ID="cmdAssign" runat="server" CommandName="39" CssClass="combutton"
                                                                                                                                        Text="Add-&gt;" OnClick="cmdAdd_Click" meta:resourcekey="cmdAssignResource1" Width="100px" />
                                                                                                                                </li>
                                                                                                                                <li>
                                                                                                                                </li>
                                                                                                                                <li style="width:auto">
                                                                                                                                    <asp:Button ID="cmdUnassign" runat="server" CommandName="40" CssClass="combutton"
                                                                                                                                        Text="&lt;-Remove" OnClick="cmdRemove_Click" meta:resourcekey="cmdUnassignResource1" Width="100px" />
                                                                                                                                </li>
                                                                                                                            </ul>
                                                                                                                        </td>
                                                                                                                        <td align="center" style="width: 266px; height: 276px;" runat="server">
                                                                                                                            <asp:Label ID="lblAssUsers" runat="server" Text="Assigned Drivers" Width="200px" CssClass="RegularText" meta:resourcekey="lblAssUsersResource1" Height="20px"></asp:Label>
                                                                                                                            <asp:ListBox ID="lboAssigned" runat="server" CssClass="formtext" Rows="5" Width="200px" meta:resourcekey="lboAssignedResource1" Height="230px"></asp:ListBox>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                </table>
                                                                                                            </asp:Panel>
                                                                                                            
                                                                                                            <asp:Panel ID="pnlPostfactum" runat="server" Width="100%" Visible="False" HorizontalAlign="Center" CssClass="height:auto" Direction="LeftToRight" meta:resourcekey="pnlPostfactumResource1">
                                                                                                                <table id="tblAssgnDates" style="width: 750px" cellpadding="2">
                                                                                                                    <tr><td colspan="4"></td></tr>
                                                                                                                    <tr>
                                                                                                                        <td style="text-align:left;">
                                                                                                                            <asp:Label ID="lblStartAssgnDate" runat="server" CssClass="RegularText" Text="Start date:" meta:resourcekey="lblStartAssgnDateResource1" Width="80px"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left;">
                                                                                                                            <asp:TextBox ID="txtStartAssgnDate" runat="server" CssClass="RegularText" Width="126px" meta:resourcekey="txtStartAssgnDateResource1"></asp:TextBox>&nbsp;
                                                                                                                            <a href="#" onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtStartAssgnDate','cal','width=220,height=200,left=270,top=380')">
                                                                                                                                <img alt="Calendar" src="../images/SmallCalendar.gif" style="border-top-width: 0px; border-left-width: 0px; border-bottom-width: 0px;
                                                                                                                                    border-right-width: 0px; text-decoration: underline;" />
                                                                                                                            </a>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left;">
                                                                                                                            <asp:Label ID="lblStartAssgnTime" runat="server" CssClass="RegularText" Text="Start Time:" meta:resourcekey="lblStartAssgnTimeResource1" Width="80px"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left;">
                                                                                                                            <asp:TextBox ID="txtStartAssgnTime" runat="server" CssClass="RegularText" Width="80px" MaxLength="5" meta:resourcekey="txtStartAssgnTimeResource1"></asp:TextBox>&nbsp;
                                                                                                                            <asp:DropDownList ID="ddlStartAM" runat="server" CssClass="RegularText" meta:resourcekey="ddlStartAMResource1">
                                                                                                                                <asp:ListItem meta:resourcekey="ListItemResource4" Text="AM"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourcekey="ListItemResource5" Text="PM"></asp:ListItem>
                                                                                                                            </asp:DropDownList>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td style="text-align:left;">
                                                                                                                            <asp:Label ID="lblEndAssgnDate" runat="server" CssClass="RegularText" Text="End date:" meta:resourcekey="lblEndAssgnDateResource1" Width="80px"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left;"> 
                                                                                                                            <asp:TextBox ID="txtEndAssgnDate" runat="server" CssClass="RegularText" Width="126px" meta:resourcekey="txtEndAssgnDateResource1"></asp:TextBox>&nbsp;
                                                                                                                            <a href="#" onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtEndAssgnDate','cal','width=220,height=200,left=270,top=380')">
                                                                                                                                <img alt="Calendar" src="../images/SmallCalendar.gif"
                                                                                                                                    style="border-top-width: 0px; border-left-width: 0px; border-bottom-width: 0px;
                                                                                                                                    border-right-width: 0px" />
                                                                                                                            </a>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left;">
                                                                                                                            <asp:Label ID="lblEndAssgnTime" runat="server" CssClass="RegularText" Text="End time:" meta:resourcekey="lblEndAssgnTimeResource1" Width="80px"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left;">
                                                                                                                            <asp:TextBox ID="txtEndAssgnTime" runat="server" CssClass="RegularText" Width="80px" MaxLength="5" meta:resourcekey="txtEndAssgnTimeResource1"></asp:TextBox>&nbsp;
                                                                                                                            <asp:DropDownList ID="ddlEndAM" runat="server" CssClass="RegularText" meta:resourcekey="ddlEndAMResource1">
                                                                                                                                <asp:ListItem meta:resourcekey="ListItemResource6" Text="AM"></asp:ListItem>
                                                                                                                                <asp:ListItem meta:resourcekey="ListItemResource7" Text="PM"></asp:ListItem>
                                                                                                                            </asp:DropDownList>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td style="text-align:left">
                                                                                                                            <asp:Label runat="server" ID="lblPostFleet" CssClass="RegularText" EnableViewState="False" Text="Fleet:" meta:resourcekey="lblPostFleetResource1" Width="80px"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left">
                                                                                                                            <asp:DropDownList ID="ddlPostFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                                                                                            DataTextField="FleetName" DataValueField="FleetId" Width="200px" OnSelectedIndexChanged="ddlPostFleet_SelectedIndexChanged" meta:resourcekey="ddlPostFleetResource1"></asp:DropDownList>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left">
                                                                                                                            <asp:Label runat="server" ID="lblPostVehicle" CssClass="RegularText" EnableViewState="False" Text="Vehicle:" meta:resourcekey="lblPostVehicleResource1" Visible="False" Width="80px"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td style="text-align:left">
                                                                                                                            <asp:DropDownList ID="ddlPostVehicle" runat="server" AutoPostBack="True" DataValueField="VehicleId" DataTextField="Description" Width="200px" CssClass="RegularText" meta:resourcekey="ddlPostVehicleResource1" OnSelectedIndexChanged="ddlPostVehicle_SelectedIndexChanged" Visible="False"></asp:DropDownList>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                </table>
                                                                                                   
                                                                                                                <div id="postDriverAssgn" visible="False" runat="server">
                                                                                                                    <ul style="list-style: none;">
                                                                                                                        <li>
                                                                                                                            <asp:Label ID="lblAllDrivers" runat="server" CssClass="RegularText" Text="Drivers" Width="238px" meta:resourcekey="lblAllDriversResource1" Height="20px"></asp:Label>
                                                                                                                        </li>
                                                                                                                        <li>
                                                                                                                            <asp:ListBox CssClass="RegularText" ID="lboAllDrivers" runat="server" Width="243px" Height="50px" meta:resourcekey="lboAllDriversResource1"></asp:ListBox>
                                                                                                                        </li>
                                                                                                                        <li></li>
                                                                                                                        <li>
                                                                                                                            <asp:Button ID="cmdAssignDriver" runat="server" CssClass="combutton" OnClick="cmdAssignDriver_Click" Text="Assign driver" Width="97px" meta:resourcekey="cmdAssignDriverResource1" />
                                                                                                                        </li>
                                                                                                                    </ul>
                                                                                                                </div>
                                                                                                            </asp:Panel>

                                                                                                            <asp:Panel ID="pnlShowHistory" runat="server" Width="100%" CssClass="height:auto" Direction="LeftToRight" HorizontalAlign="Center" Visible="False" meta:resourcekey="pnlShowHistoryResource1">
                                                                                                                <div id="drvHstButtons">
                                                                                                                    <asp:Button ID="cmdShowHistory" runat="server" CssClass="combutton" Text="Show driver history" Width="160px" Height="21px" OnClick="cmdShowHistory_Click" meta:resourcekey="cmdShowHistoryResource1" />
                                                                                                                    &nbsp;
                                                                                                                    <asp:Button ID="cmdShowVehicleHistory" runat="server" CssClass="combutton" Text="Show vehicle history" Width="160px" Height="21px" OnClick="cmdShowVehicleHistory_Click" meta:resourcekey="cmdShowVehicleHistoryResource1" />
                                                                                                                </div>
                                                                                                                <br />
                                                                                                                <div id="historyGrid" style="height:150px;overflow:auto; ">
                                                                                                                    <asp:GridView ID="gdvAssgnHistory" runat="server" Height="108px" Width="709px" AutoGenerateColumns="False" meta:resourcekey="gdvAssgnHistoryResource1" >
                                                                                                                        <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                                                                                                                        <SelectedRowStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                                                                                                                        <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right" />
                                                                                                                        <AlternatingRowStyle BackColor="Beige" CssClass="gridtext" />
                                                                                                                        <RowStyle BackColor="White" CssClass="gridtext" />
                                                                                                                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                                                                        <Columns>
                                                                                                                            <asp:BoundField DataField="FirstName" HeaderText="First Name" NullDisplayText="N/A" meta:resourcekey="BoundFieldResource6" />
                                                                                                                            <asp:BoundField DataField="LastName" HeaderText="Last Name" NullDisplayText="N/A" meta:resourcekey="BoundFieldResource7" />
                                                                                                                            <asp:BoundField DataField="License" HeaderText="License" NullDisplayText="N/A" meta:resourcekey="BoundFieldResource8" />
                                                                                                                            <asp:BoundField DataField="Description" HeaderText="Vehicle" NullDisplayText="N/A" meta:resourcekey="BoundFieldResource9" />
                                                                                                                            <asp:BoundField DataField="AssignedDateTime" HeaderText="Assignment Date" NullDisplayText="N/A" HtmlEncode="False" DataFormatString="{0:yyyy-MM-dd hh:mm}" meta:resourcekey="BoundFieldResource10" />
                                                                                                                            <asp:BoundField DataField="UnassignedDateTime" HeaderText="Unassignment Date" NullDisplayText="N/A" HtmlEncode="False" DataFormatString="{0:yyyy-MM-dd hh:mm}" meta:resourcekey="BoundFieldResource11" />
                                                                                                                        </Columns>
                                                                                                                    </asp:GridView>
                                                                                                                </div>
                                                                                                            </asp:Panel>
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
                                                                <tr style="height:12px"><td></td></tr>
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
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
