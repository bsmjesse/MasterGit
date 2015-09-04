<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmGroupControls.aspx.cs" Culture="en-US" UICulture="auto" 
    Inherits="SentinelFM.Configuration_frmGroupControls" meta:resourcekey="PageResource1" %>

<%@ Register Src="../UserControl/HierarchyTree.ascx" TagName="HierarchyTree" TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Group Settings</title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="../scripts/jquery.cookie.js"></script>
    <script src="../reports/jqueryFileTree.js?v=20130606" type="text/javascript"></script>
    <script src="../reports/splitter.js" type="text/javascript"></script>
    <script src="../scripts/tablesorter/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script src="../scripts/colResizable-1.3.min.js" type="text/javascript"></script>
    <script src="../scripts/json2.js" type="text/javascript"></script>
    <link rel="stylesheet" href="../scripts/tablesorter/themes/report/style.css" type="text/css" />
    <script language="javascript" type="text/javascript">
        var pWin
        function setParent() {
            pWin = top.window.opener;
        }

        function OnHierarchyChecked(selectedNodecodes, selectedFleetIds, fleetName, selectedFleetPaths) {
            $('#SelectedHierarchyPath').html(selectedFleetPaths.replace(/@,@/g, '<div style="border-bottom:1px solid #aaaaaa;height:1px;width:100%;margin-bottom:5px;"></div>'));
        }
    </script>
</head>
<body onload="setParent()" onunload="reloadParent()">
    <form id="form1" runat="server">
    <script language="javascript" type="text/javascript">
        function reloadParent() {
            var sRefreshParent = '<%=sRefreshParent%>';
            if (sRefreshParent =='true')
            {
                pWin.location.href = 'frmGroups.aspx';
            }
        }

        function SetChildCheckboxes(SelectedCheckbox, ChildCheckboxes)
        {
            var CheckboxID;
            var ChildCheckboxIDs;
            var Selected = SelectedCheckbox.checked;

            if (ChildCheckboxes != '') {
                ChildCheckboxIDs = ChildCheckboxes.split(";");
                for (i = 0; i < ChildCheckboxIDs.length; i++) {
                    CheckboxID = 'chkUserGroupControl' + ChildCheckboxIDs[i];
                    objCheckbox = document.getElementById(CheckboxID);
                    if (objCheckbox != null) {
                        objCheckbox.checked = Selected;
                    }
                }
            }    
        }

        function SetParentCheckboxes(SelectedCheckbox, ParentCheckboxes) {
            var CheckboxID;
            var ParentCheckboxIDs;
            var Selected = SelectedCheckbox.checked;

            if (ParentCheckboxes != '') {
                ParentCheckboxIDs = ParentCheckboxes.split(";");
                for (i = 0; i < ParentCheckboxIDs.length; i++) {
                    CheckboxID = 'chkUserGroupControl' + ParentCheckboxIDs[i];
                    objCheckbox = document.getElementById(CheckboxID);
                    if (objCheckbox != null && Selected == true) {
                        objCheckbox.checked = Selected;
                    }
                }
            }
        }
    </script>
    <div>
        <table style="width: 640px;" cellpadding="3" border="0">
            <tr>
                <td align="center" colspan="3">
                    <asp:Label ID="lblUserGroupName" runat="server" class="formtext" 
                        Font-Bold="True" meta:resourcekey="lblUserGroupNameResource1"></asp:Label>
                </td>
            </tr>
            <asp:PlaceHolder ID="plhMsg" runat="server" Visible="False">
                <tr>
                    <td class="formtext" colspan="3" style="color: #B22222;text-align:center">
                        <asp:Label ID="lblMsg" runat="server" Text="" 
                            meta:resourcekey="lblMsgResource1"></asp:Label>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhGroupName" runat="server" Visible="true">
                <tr>
                    <td class="formtext" width="40%" style="text-align: right">
                        <asp:Label ID="lblGroupName" runat="server" Text="Group Name:" 
                            meta:resourcekey="lblGroupNameResource1"></asp:Label>
                    </td>
                    <td class="formtext" colspan="2">
                        <asp:TextBox ID="txtGroupName" runat="server" MaxLength="255" Width="250px" 
                            class="formtext" meta:resourcekey="txtGroupNameResource1"></asp:TextBox>&nbsp;
                        <asp:RequiredFieldValidator ID="rfvGroupName" runat="server" ControlToValidate="txtGroupName"
                            Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick" 
                            meta:resourcekey="rfvGroupNameResource1" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddNewGroup" runat="server" Visible="False">
                <tr>
                    <td class="formtext" width="40%" style="text-align: right">
                        <asp:Label ID="lblInheritFromGroup" runat="server" Text="Inherit from Group:" 
                            meta:resourcekey="lblInheritFromGroupResource1"></asp:Label>
                    </td>
                    <td class="formtext" colspan="2">
                        <asp:DropDownList ID="cboInheritFromGroup" runat="server" AutoPostBack="True" CssClass="RegularText"
                            DataTextField="UserGroupName" DataValueField="UserGroupId" 
                            OnSelectedIndexChanged="cboInheritFromGroup_SelectedIndexChanged" 
                            meta:resourcekey="cboInheritFromGroupResource1">
                        </asp:DropDownList>
                        &nbsp;
                        <asp:RequiredFieldValidator ID="rfvInheritFromGroup" runat="server" ControlToValidate="cboInheritFromGroup"
                            Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick" 
                            meta:resourcekey="rfvInheritFromGroupResource1" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td valign="top">
                    <fieldset style="width: 280px; padding: 5px 5px 5px 5px">
                        <legend>
                            <asp:Label ID="lblUserGroupControls" runat="server" Text="Controls" class="formtext"
                                Font-Bold="True" meta:resourcekey="lblUserGroupControlsResource1"></asp:Label></legend>
                        <table style="width: 270px;" cellpadding="3" border="0">
                            <asp:PlaceHolder ID="plhUserGroupControls" runat="server" Visible="False">
                            <tr>
                                <td class="formtext" colspan="2" style="white-space:nowrap">
                                    <asp:DropDownList ID="cboUserGroupControls" runat="server" Width="240" DataTextField="ControlDescription" DataValueField="ControlId"></asp:DropDownList>&nbsp;
                                    <asp:ImageButton ID="btnUserGroupControls" runat="server"  CausesValidation="False" ImageAlign="AbsBottom"
                                        ImageUrl="../images/Load.png" onclick="btnUserGroupControls_Click" />
                                </td> 
                            </tr>
                            </asp:PlaceHolder>
                            <asp:Panel ID="pnlUserGroupControlSettings" runat="server" 
                                meta:resourcekey="pnlUserGroupControlSettingsResource1" />
                        </table>
                    </fieldset>
                </td>
                <td valign="top">
                    <fieldset style="width: 280px; padding: 5px 5px 5px 5px">
                        <legend>
                            <asp:Label ID="lblUserGroupReports" runat="server" Text="Reports" class="formtext"
                                Font-Bold="True" meta:resourcekey="lblUserGroupReportsResource1"></asp:Label></legend>
                        <table style="width: 270px;" cellpadding="3" border="0">
                            <asp:PlaceHolder ID="plhUserGroupReports" runat="server" Visible="False">
                            <tr>
                                <td class="formtext" colspan="2" style="white-space:nowrap">
                                    <asp:DropDownList ID="cboUserGroupReports" runat="server" Width="240" DataTextField="ReportName" DataValueField="ReportTypesId"></asp:DropDownList>&nbsp;
                                    <asp:ImageButton ID="btnUserGroupReports" runat="server"  CausesValidation="False" ImageAlign="AbsBottom"
                                        ImageUrl="../images/Load.png" onclick="UserGroupReports_Click" />
                                </td> 
                            </tr>
                            </asp:PlaceHolder>
                            <asp:Panel ID="pnlUserGroupReportSettings" runat="server" 
                                meta:resourcekey="pnlUserGroupReportSettingsResource1" />
                            <!--<tr><td class="formtext" colspan="2"><b><sup>1</sup></b>&nbsp;<asp:Label ID="lblCrystalReport" runat="server" Text="Crystal Report" meta:resourcekey="lblCrystalReportResource1"></asp:Label></td></tr>-->
                            <tr><td class="formtext" colspan="2"><b><sup>*</sup></b>&nbsp;<asp:Label ID="lblExtendedReport" runat="server" Text="Extended Report" meta:resourcekey="lblExtendedReportResource1"></asp:Label></td></tr>
                        </table>
                    </fieldset>
                </td>
                <td valign="top">
                    <fieldset style="width: 280px; padding: 5px 5px 5px 5px">
                        <legend>
                            <asp:Label ID="lblUserGroupCommands" runat="server" Text="Commands" class="formtext"
                                Font-Bold="True" meta:resourcekey="lblUserGroupCommandsResource1"></asp:Label></legend>
                        <table style="width: 270px;" cellpadding="3" border="0">
                            <asp:PlaceHolder ID="plhUserGroupCommands" runat="server" Visible="False">
                            <tr>
                                <td class="formtext" colspan="2" style="white-space:nowrap">
                                    <asp:DropDownList ID="cboUserGroupCommands" runat="server" Width="240" DataTextField="BoxCmdOutType" DataValueField="BoxCmdOutTypeId"></asp:DropDownList>&nbsp;
                                    <asp:ImageButton ID="btnUserGroupCommands" runat="server"  CausesValidation="False" ImageAlign="AbsBottom"
                                        ImageUrl="../images/Load.png" onclick="btnUserGroupCommands_Click" />
                                </td> 
                            </tr>
                            </asp:PlaceHolder>
                            <asp:Panel ID="pnlUserGroupCommandSettings" runat="server" 
                                meta:resourcekey="pnlUserGroupCommandsResource1" />
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr id="DefaultFleet" visible="true" runat="server">
                <td align="center" colspan="4">
                    <table id="Table8" style="width: 447px;" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td align="center">
                                <table id="tblUsers" style="width: 433px;" cellspacing="0" cellpadding="0"
                                    width="433" border="0">
                                    <tr>
                                        <td colspan="2" class="formtext" style="width: 291px">
                                            <asp:Label ID="lblUnassignedFleets" runat="server" meta:resourcekey="lblUnassignedFleetsResource1"
                                                Text="Unassigned Fleets"></asp:Label>
                                        </td>
                                        <td class="formtext">
                                            <asp:Label ID="lblAssignedFleets" runat="server" meta:resourcekey="lblAssignedFleetsResource1"
                                                Text="Assigned Fleets"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 110px">
                                            <asp:ListBox ID="lstUnassignedFleets" runat="server" CssClass="formtext" Width="160px"
                                                DataValueField="FleetId" DataTextField="FleetName" SelectionMode="Multiple"
                                                Rows="10" meta:resourcekey="lstUnassignedFleetsResource1"></asp:ListBox>
                                        </td>
                                        <td style="width: 181px" align="center">
                                            <table id="tblAddRemoveBtns" style="width: 75px; height: 99px" cellspacing="0" cellpadding="0"
                                                width="75" border="0" runat="server">
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="cmdAdd" runat="server" CssClass="combutton" Text="Add->" CausesValidation="false"
                                                            OnClick="cmdAdd_Click" meta:resourcekey="cmdAddResource1"></asp:Button>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 20px"></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="cmdRemove" runat="server" CssClass="combutton" Text="<-Remove" CausesValidation="false"
                                                            OnClick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:Button>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <asp:ListBox ID="lstAssignedFleets" runat="server" CssClass="formtext" Width="160px"
                                                DataValueField="FleetId" DataTextField="FleetName" SelectionMode="Multiple"
                                                Rows="10" meta:resourcekey="lstAssignedFleetsResource1"></asp:ListBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="Label1" runat="server" CssClass="errortext" Width="270px" Height="8px"
                        Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label>
                </td>
            </tr>
            <tr id="organizationHierarchy" visible="false" runat="server">
                <td colspan="3" style="text-align:center">
                    <table border="0" cellpadding="0" cellspacing="5" align="center">
                        <tr>
                            <td width="325">
                                <uc4:HierarchyTree ID="HierarchyTree1" GetRootHierarchyBy="" LoadVehicleData="false" Width="325px" HierarchyCheckCallBack="OnHierarchyChecked" MultipleUserHierarchyAssignment="true" PreSelectHierarchy="true" GotoLastNode="false" OrganizationHierarchyPath="existingselection" runat="server" />
                            </td>
                            <td valign="top" class="formtext">
                                <asp:Label ID="lblAssignedUserHierarchy" runat="server" meta:resourcekey="lblAssignedUserHierarchyResource1"
                                    Text="Assigned Hierarchies"></asp:Label>

                                <div id="SelectedHierarchyPath" style="width: 315px; height: 310px; border: 1px solid #999999; font-size: 12px; padding: 10px; overflow: auto; margin-top: 5px;"></div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="3">
                    <asp:Button ID="cmdUpdate" runat="server" CssClass="combutton" Text="Update"
                        OnClick="cmdUpdate_Click" meta:resourcekey="cmdUpdateResource1" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
