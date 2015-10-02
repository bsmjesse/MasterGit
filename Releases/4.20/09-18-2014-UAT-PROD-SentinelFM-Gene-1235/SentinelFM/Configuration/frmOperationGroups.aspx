<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmOperationGroups.aspx.cs"
    Inherits="SentinelFM.Configuration_frmOperationGroups" meta:resourcekey="PageResource1" %>

<%@ Register Src="Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
    <title>Operation-Groups Assignment</title>
</head>
<body>
    <form id="frmVehicleInfo" method="post" runat="server">
        <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px"
            cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdUsers" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="990px" align="center"
                        border="0">
                        <tr>
                            <td>
                                <table id="tblForm" width="990px" height="550px" class="table" border="0">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="tblSubCommands" style="left: 10px; position: relative; top: 0px" cellspacing="0"
                                                cellpadding="0" border="0">
                                                <tr>
                                                    <td>
                                                        <table id="Table5" style="z-index: 101; width: 190px; position: relative; top: 0px; height: 22px"
                                                            cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdUserInfo" runat="server" CommandName="17" CausesValidation="False"
                                                                        CssClass="confbutton" Text="User Info" Width="112px" OnClick="cmdUserInfo_Click"
                                                                        meta:resourcekey="cmdUserInfoResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdUserGroups" runat="server" CommandName="21" CausesValidation="False"
                                                                        OnClick="cmdUserGroups_Click" CssClass="confbutton" Text="User-Groups Assignment"
                                                                        Width="173px" Height="22px" meta:resourcekey="cmdUserGroupsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdGroups" runat="server" Text="Groups" CssClass="confbutton" OnClick="cmdGroups_Click"
                                                                        CausesValidation="False" CommandName="71" Width="112px" meta:resourcekey="cmdGroupsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdGroupConfiguration" runat="server" Text="Group Configuration"
                                                                        CssClass="selectedbutton" OnClick="cmdGroupConfiguration_Click" CausesValidation="False"
                                                                        CommandName="79" Width="200px" meta:resourcekey="cmdGroupConfigurationResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdControls" runat="server" Text="Controls" CssClass="confbutton" OnClick="cmdControls_Click"
                                                                        CausesValidation="False" CommandName="70" Width="112px" meta:resourcekey="cmdControlsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdServices" runat="server" Text="Services" CssClass="confbutton" OnClick="cmdServices_Click"
                                                                        CausesValidation="False" CommandName="90" Width="112px" meta:resourcekey="cmdServicesResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdUserDashBoards" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                        Visible="False" Text="User-DashBoards" Width="173px" Height="22px"
                                                                        OnClick="cmdUserDashBoards_Click" meta:resourcekey="cmdUserDashBoardsResource1"></asp:Button>
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
                                                                    <table id="Table7" class="table" height="500px" width="960px" border="0">
                                                                        <tr>
                                                                            <td class="configTabBackground" valign="top">
                                                                                <table id="Table3" style="height: 20px" cellspacing="0" cellpadding="0"
                                                                                    width="299" align="center" border="0">
                                                                                    <tr>
                                                                                        <td class="formtext" style="height: 10px"></td>
                                                                                        <td style="width: 208px"></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td class="formtext" style="width: 959px">
                                                                                        <table class="formtext" border="0"  width="100%" cellpadding="2">
                                                                                            <tr>
                                                                                                <td style="width: 140px; white-space:nowrap">
                                                                                                    <asp:Label ID="lblOperationType" runat="server" meta:resourcekey="lblOperationTypeResource1"
                                                                                                        Text="Operation Type:"></asp:Label>
                                                                                                </td>
                                                                                                <td style="white-space: nowrap;">
                                                                                                    <asp:DropDownList ID="cboOperation" runat="server" CssClass="RegularText" Width="234px"
                                                                                                        AutoPostBack="True" DataValueField="OperationType" DataTextField="OperationTypeName"
                                                                                                        OnSelectedIndexChanged="cboOperation_SelectedIndexChanged" meta:resourcekey="cboOperationResource1">
                                                                                                    </asp:DropDownList>
                                                                                                    &nbsp;
                                                                                                    <asp:RequiredFieldValidator ID="rfvOperation" runat="server" ControlToValidate="cboOperation"
                                                                                                        Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick" meta:resourcekey="rfvOperationResource1" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="width: 140px; white-space:nowrap">
                                                                                                    <asp:Label ID="lblControl" runat="server"
                                                                                                        meta:resourcekey="lblControlResource1"></asp:Label>
                                                                                                </td>
                                                                                                <td style="white-space: nowrap;">
                                                                                                    <asp:DropDownList ID="cboControls" runat="server" CssClass="RegularText" Width="234px"
                                                                                                        AutoPostBack="True" DataValueField="ControlID" DataTextField="ControlName" OnSelectedIndexChanged="cboControls_SelectedIndexChanged"
                                                                                                        meta:resourcekey="cboControlsResource1">
                                                                                                    </asp:DropDownList>
                                                                                                    &nbsp;
                                                                                                    <asp:RequiredFieldValidator ID="rfvControls" runat="server" ControlToValidate="cboControls"
                                                                                                        Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick" meta:resourcekey="rfvControlsResource1" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <asp:PlaceHolder ID="plhOrganization" runat="server" Visible="true">
                                                                                            <tr>
                                                                                                <td style="width: 140px; white-space:nowrap">
                                                                                                    <asp:Label ID="lblOrganization" runat="server" meta:resourcekey="lblOrganizationResource1"
                                                                                                        Text="Organization:"></asp:Label>
                                                                                                </td>
                                                                                                <td style="white-space: nowrap;">
                                                                                                    <asp:DropDownList ID="cboOrganization" runat="server" CssClass="RegularText" Width="234px"
                                                                                                        AutoPostBack="True" DataValueField="OrganizationId" DataTextField="OrganizationName"
                                                                                                        OnSelectedIndexChanged="cboOrganization_SelectedIndexChanged" meta:resourcekey="cboOrganizationResource1">
                                                                                                    </asp:DropDownList>
                                                                                                </td>
                                                                                            </tr>
                                                                                            </asp:PlaceHolder>
                                                                                            <tr>
                                                                                                <td style="width: 140px; white-space:nowrap">
                                                                                                    <asp:Label ID="lblMultipleUserGroups" runat="server" meta:resourcekey="lblMultipleUserGroupsResource1"
                                                                                                        Text="Multiple User Groups:"></asp:Label>
                                                                                                </td>
                                                                                                <td style="white-space: nowrap;">
                                                                                                    <asp:CheckBox ID="chkMultipleUserGroups" runat="server" AutoPostBack="true" CausesValidation="false" OnCheckedChanged="chkMultipleUserGroups_CheckedChanged" />
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                        </td>
                                                                                        <td style="width: 208px"></td>
                                                                                    </tr>
                                                                                    <asp:PlaceHolder ID="plhListboxes" runat="server" Visible="true">
                                                                                        <tr>
                                                                                            <td style="width: 959px; height: 217px" align="center">
                                                                                                <table id="Table8" style="width: 447px; height: 270px" cellspacing="0" cellpadding="0"
                                                                                                    width="447" border="0">
                                                                                                    <tr>
                                                                                                        <td style="height: 270px" align="center">
                                                                                                            <table id="tblUsers" style="width: 433px; height: 235px" cellspacing="0" cellpadding="0"
                                                                                                                width="433" border="0">
                                                                                                                <tr>
                                                                                                                    <td colspan="2" class="formtext" style="width: 291px">
                                                                                                                        <asp:Label ID="lblUnassignedUserGroups" runat="server" meta:resourcekey="lblUnassignedUserGroupsResource1"
                                                                                                                            Text="Unassigned User Groups"></asp:Label>
                                                                                                                    </td>
                                                                                                                    <td class="formtext">
                                                                                                                        <asp:Label ID="lblAssignedUserGroups" runat="server" meta:resourcekey="lblAssignedUserGroupsResource1"
                                                                                                                            Text="Assigned User Groups"></asp:Label>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td style="width: 110px">
                                                                                                                        <asp:ListBox ID="lstUnassignedGroups" runat="server" CssClass="formtext" Width="160px"
                                                                                                                            DataValueField="UserGroupId" DataTextField="UserGroupName" SelectionMode="Multiple"
                                                                                                                            Rows="15" meta:resourcekey="lstUnassignedGroupsResource1"></asp:ListBox>
                                                                                                                    </td>
                                                                                                                    <td style="width: 181px" align="center">
                                                                                                                        <table id="tblAddRemoveBtns" style="width: 75px; height: 99px" cellspacing="0" cellpadding="0"
                                                                                                                            width="75" border="0" runat="server">
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="cmdAdd" runat="server" CssClass="combutton" Text="Add->" CommandName="33"
                                                                                                                                        OnClick="cmdAdd_Click" meta:resourcekey="cmdAddResource1"></asp:Button>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td style="height: 20px"></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="cmdAddAll" runat="server" CssClass="combutton" Text="Add All->" CommandName="33"
                                                                                                                                        OnClick="cmdAddAll_Click" meta:resourcekey="cmdAddAllResource1"></asp:Button>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td id="TD1" style="height: 20px" runat="server"></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="cmdRemove" runat="server" CssClass="combutton" Text="<-Remove" CommandName="34"
                                                                                                                                        OnClick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:Button>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td style="height: 20px"></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="cmdRemoveAll" runat="server" CssClass="combutton"
                                                                                                                                        Text="<-Remove All" CommandName="34" OnClick="cmdRemoveAll_Click"
                                                                                                                                        meta:resourcekey="cmdRemoveAllResource1"></asp:Button>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                    <td>
                                                                                                                        <asp:ListBox ID="lstAssignedGroups" runat="server" CssClass="formtext" Width="160px"
                                                                                                                            DataValueField="UserGroupId" DataTextField="UserGroupName" SelectionMode="Multiple"
                                                                                                                            Rows="15" meta:resourcekey="lstAssignedGroupsResource1"></asp:ListBox>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="270px" Height="8px"
                                                                                                    Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </asp:PlaceHolder>
                                                                                    <tr>
                                                                                        <td class="formtext" colspan="2" style="color: #B22222; text-align: center">
                                                                                            <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <asp:PlaceHolder ID="plhMultipleUserGroups" runat="server" Visible="false">
                                                                                        <tr>
                                                                                            <td class="formtext" style="width: 959px">
                                                                                                <table class="formtext" border="0" cellpadding="2" width="100%">
                                                                                                    <tr>
                                                                                                        <td style="width: 140px; white-space:nowrap">
                                                                                                            <asp:Label ID="lblUserGroups" runat="server"
                                                                                                                Text="User Groups:" meta:resourcekey="lblUserGroupsResource1"></asp:Label>
                                                                                                        </td>
                                                                                                        <td style="white-space: nowrap;" class="formtext">
                                                                                                            <asp:DropDownList ID="cboUserGroups" runat="server" CssClass="RegularText" Width="234px">
                                                                                                            </asp:DropDownList>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td style="width: 140px"></td>
                                                                                                        <td style="text-align: center">
                                                                                                            <asp:Button ID="cmdUnasign" runat="server" CssClass="combutton" Text="Unasign" CausesValidation="true"
                                                                                                                OnClick="cmdUnasign_Click" meta:resourcekey="cmdUnasignResource1"></asp:Button>&nbsp;
                                                                                                        <asp:Button ID="cmdAssign" runat="server" CssClass="combutton" Text="Assign" CausesValidation="true"
                                                                                                            OnClick="cmdAssign_Click" meta:resourcekey="cmdAssignResource1"></asp:Button>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            </td>
                                                                                            <td style="width: 208px"></td>
                                                                                        </tr>
                                                                                    </asp:PlaceHolder>
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
