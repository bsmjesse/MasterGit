<%@ Page Language="c#" Inherits="SentinelFM.frmVehicleFleet" CodeFile="frmVehicleFleet.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register TagPrefix="uc1" TagName="ctlFleetVehicles" Src="Components/ctlFleetVehicles.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ctlOrganizationHierarchyFleetVehicles" Src="Components/ctlOrganizationHierarchyFleetVehicles.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmVehicleFleet</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script language="javascript">
			<!--
				function NewWindow() { 
					var mypage='frmAddFleet.aspx'
					var myname='AddFleet';
					var w=300;
					var h=120;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 
			
			

			//-->
    </script>

</head>
<body>
    <form id="frmVehicleFleetForm" method="post" runat="server">
        <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px"
            cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdVehicles" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="990" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" class=table  height="550px"      width="990px" border="0">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" style="left: 10px; position: relative; top: 0px" cellspacing="0"
                                                cellpadding="0" width="300" border="0">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" cellspacing="0" cellpadding="0" width="300" border="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table5" style="z-index: 102; width: 190px; position: relative; top: 0px;
                                                                        height: 22px" cellspacing="0" cellpadding="0" border="0">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Button ID="cmdVehicleInfo" runat="server" Text="Vehicle Info" CssClass="confbutton"
                                                                                    CausesValidation="False" Width="112px" CommandName="4" OnClick="cmdVehicleInfo_Click" meta:resourcekey="cmdVehicleInfoResource1">
                                                                                </asp:Button></td>
                                                                            <td>
                                                                                <asp:Button ID="cmdAlarms" runat="server" Text="Sensors/Messages" CssClass="confbutton"
                                                                                    CausesValidation="False" Width="136px" CommandName="5" OnClick="cmdAlarms_Click" meta:resourcekey="cmdAlarmsResource1">
                                                                                </asp:Button></td>
                                                                            <td>
                                                                                <asp:Button ID="cmdOutputs" runat="server" Text="Outputs" CssClass="confbutton" CausesValidation="False"
                                                                                    CommandName="6" OnClick="cmdOutputs_Click" meta:resourcekey="cmdOutputsResource1"></asp:Button></td>
                                                                            <td>
                                                                                <asp:Button ID="cmdFleetVehicle" runat="server" Text="Fleet-Vehicle Assignment" CssClass="selectedbutton"
                                                                                    CausesValidation="False" Width="168px" CommandName="7" meta:resourcekey="cmdFleetVehicleResource1"></asp:Button></td>
                                                                            <td>
                                                                                <asp:Button ID="btnEquipmentAssignment" runat="server" Width="168px" CausesValidation="False"
                                                                                    CssClass="confbutton" Text="Equipment Assignment" PostBackUrl="~/Configuration/Equipment/frmAssignmentList.aspx"
                                                                                    meta:resourcekey="btnEquipmentAssignmentResource1"></asp:Button>

                                                                            </td>
                                                                            <td>
                                                                                <asp:Button ID="btnFuelCategory" runat="server" Width="168px" Visible= "false" CausesValidation="False" CssClass="confbutton"
                                                                                     Text="Fuel Category" PostBackUrl="~/Configuration/FuelCategory/frmFuelCategory.aspx"
                                                                                    meta:resourcekey="btnFuelCategoryResource1"></asp:Button>

                                                                            </td>

                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <table id="Table6" cellspacing="0" cellpadding="0" width="617" align="center" border="0">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" style="width: 960px;height: 500px" class="tableDoubleBorder"  border="0">
                                                                                    <tr id="trFleetSelectOption" visible="false" runat="server"><td align="center" class="configTabBackground">
                                                                                            <table class="formtext" runat="server" id="optBaseTable"  cellpadding="0" cellspacing="0"  >
                                                                                                <tr>
                                                                                                    <td> 
                                                                                                        <asp:Label ID="Label11" runat="server" Text="Based On:" 
                                                                                                            meta:resourcekey="Label11Resource1"></asp:Label> </td>
                                                                                                    <td>
                                                                                                                <asp:RadioButtonList ID="optAssignBased" name="AssignBased" runat="server"  class="formtext" 
                                                                                                RepeatDirection="Horizontal" AutoPostBack="True"
                                                                                                onselectedindexchanged="optAssignBased_SelectedIndexChanged" 
                                                                                                                    meta:resourcekey="optAssignBasedResource1" >
                                                                                                <asp:ListItem id="Radio2" type="radio" name="raFleetSelectOption" value="0" checked
                                                                                                    runat="server" Selected="True" meta:resourcekey="ListItemResource1" >Fleet</asp:ListItem> 
                                                                                                <asp:ListItem id="Radio1" name="raFleetSelectOption" value="1" runat="server" 
                                                                                                                        meta:resourcekey="ListItemResource2" >Organization Hierarchy Fleet</asp:ListItem> 
                                                                                                

                                                                                            </asp:RadioButtonList> 
                                                        
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr id="trBasedOnNormalFleet" runat="server">
                                                                                        <td align="center" class="configTabBackground">
                                                                                            <uc1:ctlFleetVehicles ID="CtlFleetVehicles1" runat="server"></uc1:ctlFleetVehicles>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr id="trBasedOnHierarchyFleet" visible="false" runat="server">
                                                                                            <TD align="center" class="configTabBackground">
																								<uc2:ctlOrganizationHierarchyFleetVehicles id="CtlFleetVehicles2" runat="server"></uc2:ctlOrganizationHierarchyFleetVehicles></TD>
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
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
