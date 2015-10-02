<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmFleetGeozone.aspx.cs" Inherits="SentinelFM.Configuration.frmFleetGeozone" meta:resourcekey="PageResource1" %>

<%@ Register Src="Components/ctlGeozoneLandmarksMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <title></title>
</head>
<body>
    <%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
      {%>

    <script language="javascript">
		<!--
    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

        var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val() + '&loadVehicle=0';
            if(MutipleUserHierarchyAssignment && $('#hidOrganizationHierarchyFleetId').val().indexOf(",") < 0)
            {
                mypage = mypage + "&sl=1";
            }

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
            //var myVal = document.getElementById('');
            //ValidatorEnable(myVal, false); 

            
            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
            $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
            $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
        }
            
        //-->
    </script>
    <%} %>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

        <%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
          {%>
        <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server"
            Text="Button" Style="display: none;" AutoPostBack="True"
            OnClick="hidOrganizationHierarchyPostBack_Click"
            meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />
        <%} %>
        <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px"
            cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdFleetGeozone" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="617" align="center" border="0">
                        <tr>
                            <td>
                                 <table id="tblForm" height="550px" width="990px" class="table" border="0">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" style="left: 10px; position: relative; top: 0px" cellspacing="0"
                                                cellpadding="0" width="300" border="0">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" cellspacing="0" cellpadding="0" width="300" border="0">
                                                            <tr>
                                                                <td></td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                   <table id="Table6" style="width: 885px;" cellspacing="0" cellpadding="0"
                                                                        width="885" align="center" border="0">
                                                                        <tr>
                                                                            <td align="center">
                                                                                <table id="Table7" style="width: 810px;" border="0">
                                                                                    <tr>
                                                                                        <td align="center" class="configTabBackground" style="vertical-align:top;">
                                                                                             <table id="Table1" cellspacing="0" cellpadding="0" width="100%"
                                                                                                align="center" border="0">
                                                                                                <tr>
                                                                                                    <td class="tableheading" align="left" height="5"></td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td class="tableheading" style="border-bottom-width: 1px; border-bottom-color: black; height: 30px; vertical-align:top;"
                                                                                                        align="center">
                                                                                                        <table id="Table3" style="width: 667px; height: 20px" cellspacing="0" cellpadding="0"
                                                                                                            width="667" align="center" border="0">
                                                                                                            <tr>
                                                                                                                <td class="formtext" style="width: 136px" align="right">
                                                                                                                    <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource6"></asp:Label>
                                                                                                                    <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" Text=" Hierarchy Node:"
                                                                                                                        Visible="False" meta:resourcekey="lblOhTitleResource1" />
                                                                                                                </td>
                                                                                                                <td></td>
                                                                                                                <td style="width: 258px">
                                                                                                                    <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText"
                                                                                                                        DataTextField="FleetName" DataValueField="FleetId" AutoPostBack="True" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                                                                                                        meta:resourcekey="cboFleetResource6">
                                                                                                                    </asp:DropDownList>
                                                                                                                    <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" CssClass="combutton"
                                                                                                                        Visible="False" OnClientClick="return onOrganizationHierarchyNodeCodeClick();" Width="200px"
                                                                                                                        meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                                                                                                </td>
                                                                                                                <td style="width: 64px" align="center"></td>
                                                                                                                <td></td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="width: 959px" align="center">
                                                                                            <table id="tblGeoZones" style="width: 663px;" cellspacing="0" cellpadding="0"
                                                                                                width="663" border="0" runat="server">
                                                                                                <tr>
                                                                                                    <td style="height: 309px" align="center">
                                                                                                        <table id="tblVehicles" style="width: 100%" cellspacing="0" cellpadding="0" border="0"
                                                                                                            runat="server">
                                                                                                            <tr>
                                                                                                                <td class="formtext" align="center">
                                                                                                                    <asp:Label ID="lblUnassignedGeozones" runat="server" CssClass="formtext"
                                                                                                                        meta:resourcekey="lblUnassignedGeozonesResource6" Text="Unassigned GeoZones"></asp:Label></td>
                                                                                                                <td style="width: 106px" align="center"></td>
                                                                                                                <td class="formtext" align="center">
                                                                                                                    <asp:Label ID="lblAssignedGeozones" runat="server" CssClass="formtext"
                                                                                                                        meta:resourcekey="lblAssignedGeozonesResource6" Text="Assigned GeoZones"></asp:Label></td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td style="width: 30%" valign="top" align="left" height="257">
                                                                                                                    <asp:ListBox ID="lstUnAss" DataValueField="GeozoneNo" DataTextField="GeozoneName"
                                                                                                                        CssClass="formtext" Width="236px" runat="server" SelectionMode="Multiple" Rows="15"
                                                                                                                        meta:resourcekey="lstUnAssResource1"></asp:ListBox>
                                                                                                                </td>
                                                                                                                <td valign="top" align="center" height="257">
                                                                                                                    <table id="tblAddRemoveBtns" style="height: 99px" cellspacing="0" cellpadding="0"
                                                                                                                        border="0" runat="server">
                                                                                                                        <tr>
                                                                                                                            <td>
                                                                                                                                <asp:Button ID="cmdAdd" runat="server" Text="Add->" CssClass="combutton" OnClick="cmdAdd_Click"
                                                                                                                                    meta:resourcekey="cmdAddResource6"></asp:Button></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px"></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 30px">
                                                                                                                                <asp:Button ID="cmdAddAll" runat="server" Text="Add All->" CssClass="combutton" OnClick="cmdAddAll_Click"
                                                                                                                                    meta:resourcekey="cmdAddAllResource6"></asp:Button></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px"></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px">
                                                                                                                                <asp:Button ID="cmdRemove" runat="server" Text="<-Remove" CssClass="combutton"
                                                                                                                                    meta:resourcekey="cmdRemoveResource6" OnClick="cmdRemove_Click"></asp:Button>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px"></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px">
                                                                                                                                <asp:Button ID="cmdClearAll" runat="server" Text="<-Remove All" CssClass="combutton"
                                                                                                                                    OnClick="cmdClearAll_Click" meta:resourcekey="cmdClearAllResource6"></asp:Button></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 16px"></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px"></td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                </td>
                                                                                                                <td valign="top" align="center" width="30%" height="257">
                                                                                                                    <asp:ListBox ID="lstAss" DataValueField="GeozoneNo" DataTextField="GeozoneName" CssClass="formtext"
                                                                                                                        Width="236px" runat="server" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstAssResource1"></asp:ListBox>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td style="height: 40px" align="center"></td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="270px" Height="8px"
                                                                                    Visible="False" meta:resourcekey="lblMessageResource6"></asp:Label></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td align="left" class="configTabBackground"></td>
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
