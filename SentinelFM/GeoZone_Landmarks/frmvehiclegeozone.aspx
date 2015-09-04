<%@ Page Language="c#" Inherits="SentinelFM.Configuration.frmVehicleGeoZone" CodeFile="frmVehicleGeoZone.aspx.cs" Culture="en-US" meta:resourcekey="PageResource6" UICulture="auto" %>

<%@ Register Src="Components/ctlGeozoneLandmarksMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmVehicleGeoZone</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
      {%>

    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>

    <script language="javascript">
		<!--
    OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;

        var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
            if (MutipleUserHierarchyAssignment) {
                mypage = mypage + "&m=1&f=0&loadVehicle=0";
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
    <form id="frmVehicleGeozoneForm" method="post" runat="server">

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
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdVehicleGeoZone" />
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
                                                                    <table id="Table6" style="width: 885px; height: 519px" cellspacing="0" cellpadding="0"
                                                                        width="885" align="center" border="0">
                                                                        <tr>
                                                                            <td align="center">
                                                                                <table id="Table7" style="width: 810px; height: 500px" border="0">
                                                                                    <tr>
                                                                                        <td align="center" class="configTabBackground" style="vertical-align:top;">
                                                                                            <table id="Table1" cellspacing="0" cellpadding="0" width="100%"
                                                                                                align="center" border="0">
                                                                                                <tr>
                                                                                                    <td class="tableheading" align="left" height="5"></td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td class="tableheading" style="border-bottom-width:1px; border-bottom-color:black; height:30px; vertical-align:top;"
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
                                                                                                                        DataTextField="FleetName" DataValueField="FleetId" AutoPostBack="True" OnSelectedIndexChanged="cboToFleet_SelectedIndexChanged"
                                                                                                                        meta:resourcekey="cboFleetResource6">
                                                                                                                    </asp:DropDownList>
                                                                                                                    <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" CssClass="combutton"
                                                                                                                        Visible="False" OnClientClick="return onOrganizationHierarchyNodeCodeClick();" Width="200px"
                                                                                                                        meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                                                                                                </td>
                                                                                                                <td style="width: 64px" align="center">
                                                                                                                    <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" Height="10px"
                                                                                                                        Visible="False" meta:resourcekey="lblVehicleNameResource6"
                                                                                                                        Text=" Vehicle:"></asp:Label></td>
                                                                                                                <td>
                                                                                                                    <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText"
                                                                                                                        DataTextField="Description" DataValueField="VehicleId" AutoPostBack="True" Visible="False"
                                                                                                                        DESIGNTIMEDRAGDROP="79" OnSelectedIndexChanged="cboVehicle_SelectedIndexChanged"
                                                                                                                        meta:resourcekey="cboVehicleResource6">
                                                                                                                    </asp:DropDownList></td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="width: 959px; vertical-align:top;" align="center">
                                                                                            <table id="tblGeoZones" style="width: 663px; height: 362px" cellspacing="0" cellpadding="0"
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
                                                                                                                    <table id="tblGeoZoneHeaderUnAss" style="border-right: gray 1px outset; border-top: gray 1px outset; border-left: gray 1px outset; width: 171px; border-bottom: gray 1px outset; height: 259px"
                                                                                                                        bordercolor="ghostwhite" cellspacing="0" cellpadding="1" bgcolor="gray" border="0"
                                                                                                                        runat="server">
                                                                                                                        <tr>
                                                                                                                            <td style="font-weight: bold; font-size: 11px; width: 59px; color: white; font-family: Arial"
                                                                                                                                height="20">
                                                                                                                                <asp:Label ID="lblGeozoneHeaderUnAssGeozone" runat="server"
                                                                                                                                    meta:resourcekey="lblGeozoneHeaderUnAssGeozoneResource6" Text="GeoZone"></asp:Label></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td id="TD1" style="font-weight: bold; font-size: 11px; width: 59px; color: white; font-family: Arial"
                                                                                                                                class="configTabBackground" height="237" runat="server"></td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                    <table id="Table8" cellspacing="0" cellpadding="0" border="0">
                                                                                                                        <tr>
                                                                                                                            <td>
                                                                                                                                <asp:DataGrid ID="dgUnAssGeoZone" runat="server" Width="205px" GridLines="None" BackColor="White"
                                                                                                                                    PageSize="8" AllowSorting="True" AllowPaging="True" AutoGenerateColumns="False"
                                                                                                                                    BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1"
                                                                                                                                    meta:resourcekey="dgUnAssGeoZoneResource6">
                                                                                                                                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                                                                    <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                                                                                    <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                                                                                    <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                                                                                    <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                                                                    <Columns>
                                                                                                                                        <asp:BoundColumn Visible="False" DataField="GeoZoneId" HeaderText='<%$ Resources:dgUnAssGeozone_GeozoneId %>'></asp:BoundColumn>
                                                                                                                                        <asp:TemplateColumn>
                                                                                                                                            <HeaderStyle Width="20px"></HeaderStyle>
                                                                                                                                            <ItemStyle HorizontalAlign="Center" Width="29px"></ItemStyle>
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <asp:CheckBox ID="chkCheckBox" Checked='<%# DataBinder.Eval(Container.DataItem, "chkBox") %>'
                                                                                                                                                    runat="server" meta:resourcekey="chkCheckBoxResource6" />
                                                                                                                                            </ItemTemplate>
                                                                                                                                        </asp:TemplateColumn>
                                                                                                                                        <asp:BoundColumn DataField="GeozoneName" ReadOnly="True" HeaderText='<%$ Resources:dgUnAssGeozone_GeozoneName %>'>
                                                                                                                                            <HeaderStyle Width="300px"></HeaderStyle>
                                                                                                                                            <ItemStyle Width="300px"></ItemStyle>
                                                                                                                                        </asp:BoundColumn>
                                                                                                                                        <asp:BoundColumn Visible="False" DataField="SeverityId" HeaderText='<%$ Resources:dgUnAssGeozone_SeverityId %>'></asp:BoundColumn>
                                                                                                                                    </Columns>
                                                                                                                                    <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                                                                                        Mode="NumericPages"></PagerStyle>
                                                                                                                                </asp:DataGrid></td>
                                                                                                                        </tr>
                                                                                                                    </table>
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
                                                                                                                                <asp:Button ID="cmdClearAll" runat="server" Text="<-Remove All" CssClass="combutton"
                                                                                                                                    OnClick="cmdClearAll_Click" meta:resourcekey="cmdClearAllResource6"></asp:Button></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px"></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px">
                                                                                                                                <asp:Button ID="cmdCheckSync" runat="server" Text="Check Sync" CssClass="combutton" OnClick="cmdCheckSync_Click" meta:resourcekey="cmdCheckSyncResource6"></asp:Button>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 30px">
                                                                                                                                <asp:Button ID="cmdSync" runat="server" Text="Sync" CssClass="combutton"
                                                                                                                                    Enabled="False" Visible="False" OnClick="cmdSync_Click"
                                                                                                                                    meta:resourcekey="cmdSyncResource6"></asp:Button></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 16px"></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 20px"></td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                    <table id="tblWait" style="width: 84px; height: 68px" cellspacing="0" cellpadding="0"
                                                                                                                        runat="server">
                                                                                                                        <tr>
                                                                                                                            <td class="RegularText" style="height: 15px" align="center">
                                                                                                                                <asp:Label ID="lblPleaseWait" runat="server" CssClass="RegularText"
                                                                                                                                    meta:resourcekey="lblPleaseWaitResource6" Text="Please wait..."></asp:Label></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 25px" align="center">
                                                                                                                                <asp:Image ID="imgWait" runat="server" Width="79px" Height="5px" ImageUrl="../images/prgBar.gif"
                                                                                                                                    meta:resourcekey="imgWaitResource6"></asp:Image></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="height: 22px" align="center">
                                                                                                                                <asp:Button ID="cmdCancelSync" runat="server" Text="Cancel" CssClass="combutton"
                                                                                                                                    OnClick="cmdCancelSync_Click" meta:resourcekey="cmdCancelSyncResource6"></asp:Button></td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                </td>
                                                                                                                <td valign="top" align="center" width="30%" height="257">
                                                                                                                    <table id="tblGeoZoneHeader" style="border-right: gray 1px outset; border-top: gray 1px outset; border-left: gray 1px outset; width: 221px; border-bottom: gray 1px outset; height: 220px"
                                                                                                                        bordercolor="ghostwhite" cellspacing="0" cellpadding="1" bgcolor="gray" border="0"
                                                                                                                        runat="server">
                                                                                                                        <tr>
                                                                                                                            <td style="font-weight: bold; font-size: 11px; width: 59px; color: white; font-family: Arial"
                                                                                                                                height="20">
                                                                                                                                <asp:Label ID="lblGeozoneHeaderGeozone" runat="server"
                                                                                                                                    meta:resourcekey="lblGeozoneHeaderGeozoneResource6" Text="GeoZone"></asp:Label></td>
                                                                                                                            <td style="font-weight: bold; font-size: 11px; width: 51px; color: white; font-family: Arial"
                                                                                                                                height="20">
                                                                                                                                <asp:Label ID="lblGeozoneHeaderSeverity" runat="server"
                                                                                                                                    meta:resourcekey="lblGeozoneHeaderSeverityResource6" Text="Severity"></asp:Label></td>
                                                                                                                            <td style="font-weight: bold; font-size: 11px; color: white; font-family: Arial"
                                                                                                                                height="20">
                                                                                                                                <asp:Label ID="lblGeozoneHeaderStatus" runat="server"
                                                                                                                                    meta:resourcekey="lblGeozoneHeaderStatusResource6" Text="Status"></asp:Label></td>
                                                                                                                        </tr>
                                                                                                                        <tr>
                                                                                                                            <td style="font-weight: bold; font-size: 11px; width: 59px; color: white; font-family: Arial"
                                                                                                                                class="configTabBackground" height="237"></td>
                                                                                                                            <td style="font-weight: bold; font-size: 11px; width: 51px; color: white; font-family: Arial"
                                                                                                                                class="configTabBackground" height="237"></td>
                                                                                                                            <td style="font-weight: bold; font-size: 11px; color: white; font-family: Arial"
                                                                                                                                class="configTabBackground" height="237">&nbsp;</td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                    <table id="tblAssGeoZones" cellspacing="0" cellpadding="0" width="50" border="0">
                                                                                                                        <tr>
                                                                                                                            <td>
                                                                                                                                <asp:DataGrid ID="dgAssGeoZone" runat="server" GridLines="None" BackColor="White"
                                                                                                                                    PageSize="7" AllowSorting="True" AllowPaging="True" AutoGenerateColumns="False"
                                                                                                                                    BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1"
                                                                                                                                    DataKeyField="GeoZoneId" meta:resourcekey="dgAssGeoZoneResource6">
                                                                                                                                    <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                                                                    <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                                                                                    <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                                                                                    <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                                                                                    <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                                                                    <Columns>
                                                                                                                                        <asp:BoundColumn Visible="False" DataField="GeoZoneId" HeaderText='<%$ Resources:dgAssGeozone_GeoZoneId %>'></asp:BoundColumn>
                                                                                                                                        <asp:BoundColumn DataField="GeozoneName" ReadOnly="True" HeaderText='<%$ Resources:dgAssGeozone_GeozoneName %>'></asp:BoundColumn>
                                                                                                                                        <asp:TemplateColumn HeaderText='<%$ Resources:dgAssGeozone_Severity %>'>
                                                                                                                                            <ItemStyle Wrap="False"></ItemStyle>
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <asp:Label ID="lblSeverity" Text='<%# DataBinder.Eval(Container.DataItem,"Severity") %>'
                                                                                                                                                    runat="server" meta:resourcekey="lblSeverityResource6"></asp:Label>
                                                                                                                                            </ItemTemplate>
                                                                                                                                            <EditItemTemplate>
                                                                                                                                                <asp:DropDownList ID="cboSeverity" DataSource='<%# dsSeverity %>' DataValueField="SeverityId"
                                                                                                                                                    DataTextField="SeverityName" SelectedIndex='<%# GetSeverity(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"SeverityId"))) %>'
                                                                                                                                                    runat="server" meta:resourcekey="cboSeverityResource6">
                                                                                                                                                </asp:DropDownList>
                                                                                                                                            </EditItemTemplate>
                                                                                                                                        </asp:TemplateColumn>
                                                                                                                                        <asp:BoundColumn DataField="Status" ReadOnly="True" HeaderText='<%$ Resources:dgAssGeozone_Status %>'></asp:BoundColumn>
                                                                                                                                        <asp:BoundColumn DataField="Speed" ReadOnly="True" HeaderText='<%$ Resources:dgAssGeozone_Speed %>'></asp:BoundColumn>
                                                                                                                                        <asp:BoundColumn Visible="False" DataField="Assigned" HeaderText='<%$ Resources:dgAssGeozone_Assigned %>'></asp:BoundColumn>
                                                                                                                                        <asp:EditCommandColumn UpdateText="Update" CancelText="Cancel" EditText="Edit" meta:resourcekey="EditCommandColumnResource6">
                                                                                                                                            <ItemStyle ForeColor="Black"></ItemStyle>
                                                                                                                                        </asp:EditCommandColumn>
                                                                                                                                        <asp:TemplateColumn>
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <asp:LinkButton ID="lnkDelete" runat="server" ForeColor="Black" CommandName="Delete"
                                                                                                                                                    CausesValidation="False" Text="Delete" meta:resourcekey="lnkDeleteResource6"></asp:LinkButton>
                                                                                                                                            </ItemTemplate>
                                                                                                                                        </asp:TemplateColumn>
                                                                                                                                    </Columns>
                                                                                                                                    <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                                                                                        Mode="NumericPages"></PagerStyle>
                                                                                                                                </asp:DataGrid></td>
                                                                                                                        </tr>
                                                                                                                    </table>
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
                                                                            <td align="left" class="configTabBackground">
                                                                                <fieldset>
                                                                                    <asp:Label ID="lblNotes" runat="server" meta:resourcekey="lblNote" runat="server" CssClass="formtext" Height="53px" Width="800px"
                                                                                        Text="When a Geozone is added to (or removed from) the Assigned Geozones table, System will attempt to synchronize it immediately. If the unit is not available, the Geozone will be automatically synced the next time the unit communicates with the server.
                                                                                                 Status Definitions: Sync – Geozone is confirmed synchronized between the unit and the Assigned Geozones table. Add Pending – Geozone is queued for loading on the box, and will automatically add the Geozone when the unit communicates."></asp:Label>
                                                                                </fieldset>
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
