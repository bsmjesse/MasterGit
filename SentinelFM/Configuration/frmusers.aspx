<%@ Page Language="c#" Inherits="SentinelFM.frmUsers" CodeFile="frmUsers.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Src="Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="../UserControl/HierarchyTree.ascx" TagName="HierarchyTree" TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD html 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmUsers</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->



    <script language="javascript">
        
        
<!-- 	
    function ResetPassword(UserId) {
        var mypage = 'frmResetPassword.aspx?UserId=' + UserId
        var myname = 'Resetpassword';
        var w = 510;
        var h = 250;
        var winl = (screen.width - w) / 2;
        var wint = (screen.height - h) / 2;
        winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,toolbar=0,menubar=0,'
        win = window.open(mypage, myname, winprops)
        if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
    }

    function preferenceWindow(userIdToUpdate, username) {
        var mypage = 'frmPreference.aspx?userIdToUpdate=' + userIdToUpdate + '&username=' + username
        var myname = '';
        var w = 700;
        var h = 700;
        var winl = (screen.width - w) / 2;
        var wint = (screen.height - h) / 2;
        winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,toolbar=0,menubar=0,scrollbars=yes,'
        win = window.open(mypage, myname, winprops)
        if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
    }

    function OnHierarchyChecked(selectedNodecodes, selectedFleetIds, fleetName, selectedFleetPaths) {
        $('#SelectedHierarchyPath').html(selectedFleetPaths.replace(/@,@/g, '<div style="border-bottom:1px solid #aaaaaa;height:1px;width:100%;margin-bottom:5px;"></div>'));
    }
    //-->	
    </script>

    <style type="text/css">
        .style1 {
            width: 118px;
            height: 23px;
        }

        .style2 {
            width: 5px;
            height: 23px;
        }

        .style3 {
            width: 183px;
            height: 23px;
        }
    </style>

</head>
<body>
    <script language="javascript" type="text/javascript">
        
        function ConfirmDeletePlan() {
            var cboStatusid = document.getElementById("cboStatus");

            var cmbstatusText = cboStatusid.options[cboStatusid.selectedIndex].text;
            
            if(cmbstatusText == 'Delete' || cmbstatusText == 'Effacer' )
            {
                if (confirm("<%= confirmDeletePlan %>")) return true;
                else return false;
              
            }
            return true;
            
        }

        function clickButton(e, buttonid) {
            var evt = e ? e : window.event;
            var bt = document.getElementById(buttonid);
            if (bt) {
                if (evt.keyCode == 13) {
                    bt.click();
                    return false;
                }
            }
        }
    </script>
    <form id="frmVehicleInfo" method="post" runat="server">
        <table id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellspacing="0"
            cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdUsers" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="990" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" class="table" height="550px" width="990px" border="0">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="tblSubCommands" style="LEFT: 10px; POSITION: relative; TOP: 0px" cellspacing="0" cellpadding="0" border="0">
                                                <tr>
                                                    <td>
                                                        <table id="Table5" style="Z-INDEX: 101; width: 190px; POSITION: relative; TOP: 0px; height: 22px"
                                                            cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdUserInfo" runat="server" Text="User Info" CssClass="selectedbutton" CausesValidation="False"
                                                                        CommandName="17" Width="112px" meta:resourcekey="cmdUserInfoResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdUserGroups" runat="server" Text="User-Groups Assignment" CssClass="confbutton"
                                                                        CausesValidation="False" CommandName="21" Width="173px" Height="22px" OnClick="cmdUserGroups_Click" meta:resourcekey="cmdUserGroupsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdGroups" runat="server" Text="Groups" CssClass="confbutton" OnClick="cmdGroups_Click"
                                                                        CausesValidation="False" CommandName="71" Width="112px" meta:resourcekey="cmdGroupsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdGroupConfiguration" runat="server" Text="Group Configuration" CssClass="confbutton" OnClick="cmdGroupConfiguration_Click"
                                                                        CausesValidation="False" CommandName="79" Width="200px" meta:resourcekey="cmdGroupConfigurationResource1"></asp:Button>
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
                                                                    <asp:Button ID="cmdUserDashBoards" runat="server" CausesValidation="False" CssClass="confbutton" Visible="False"
                                                                        Text="User-DashBoards" Width="173px" Height="22px" OnClick="cmdUserDashBoards_Click"></asp:Button>
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
                                                                    <table id="Table7" class="table" height="500px" width="960px"
                                                                        border="0">
                                                                        <tr>
                                                                            <td class="configTabBackground">
                                                                                <table id="Table1" style="height: 444px" cellspacing="0" cellpadding="0" align="center" border="0">
                                                                                    <tr>
                                                                                        <td style="width: 100%; height: 360px" align="center" valign="top">
                                                                                            <table id="tblAddUsers" style="height: 169px" cellspacing="0" cellpadding="0" border="0" runat="server">
                                                                                                <tr style="height: 20px">
                                                                                                    <td></td>
                                                                                                    <td>
                                                                                                        <asp:Label ID="lblUserId" runat="server" Visible="False" meta:resourcekey="lblUserIdResource1" Text="0"></asp:Label>
                                                                                                    </td>
                                                                                                    <td></td>
                                                                                                    <td></td>
                                                                                                </tr>
                                                                                                <tr style="height: 20px">
                                                                                                    <td class="formtext" style="text-align: left; width: 80px">
                                                                                                        <asp:Label ID="lblFirstName" runat="server" meta:resourcekey="lblFirstNameResource1" Text="First Name:"></asp:Label>
                                                                                                    </td>
                                                                                                    <td style="width: 235px">
                                                                                                        <asp:TextBox ID="txtFirstName" runat="server" Width="210px" CssClass="formtext" meta:resourcekey="txtFirstNameResource1"></asp:TextBox>
                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please Enter First Name"
                                                                                                            ControlToValidate="txtFirstName" meta:resourcekey="RequiredFieldValidator1Resource1" Text="*"></asp:RequiredFieldValidator>
                                                                                                    </td>
                                                                                                    <td class="formtext" style="text-align: left; width: 100px;">
                                                                                                        <asp:Label ID="lblLastName" runat="server" meta:resourcekey="lblLastNameResource1" Text="Last Name:"></asp:Label>
                                                                                                    </td>
                                                                                                    <td style="width: 235px">
                                                                                                        <asp:TextBox ID="txtLastName" runat="server" Width="210px" CssClass="formtext" meta:resourcekey="txtLastNameResource1"></asp:TextBox>
                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please Enter Last Name"
                                                                                                            ControlToValidate="txtLastName" meta:resourcekey="RequiredFieldValidator2Resource1" Text="*"></asp:RequiredFieldValidator>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr style="height: 20px">
                                                                                                    <td class="formtext" style="text-align: left; width: 80px;">
                                                                                                        <asp:Label ID="lblUserName" runat="server" meta:resourcekey="lblUserNameResource1" Text="User Name:"></asp:Label>
                                                                                                    </td>
                                                                                                    <td style="width: 235px">
                                                                                                        <asp:TextBox ID="txtUserName" runat="server" Width="210px" CssClass="formtext" meta:resourcekey="txtUserNameResource1"></asp:TextBox>
                                                                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please Enter an User Name"
                                                                                                            ControlToValidate="txtUserName" meta:resourcekey="RequiredFieldValidator3Resource1" Text="*"></asp:RequiredFieldValidator>
                                                                                                    </td>


                                                                                                    <td colspan="2">
                                                                                                        <table id="tblExpire" runat="server" border="0" cellpadding="0" cellspacing="0" class="formtext">
                                                                                                            <tr>
                                                                                                                <td class="formtext" style="text-align: left; width: 105px;">
                                                                                                                    <asp:Label ID="lblExpiredDate" runat="server" meta:resourcekey="lblExpiredDateResource1" Text="Expiration Date:"></asp:Label></td>
                                                                                                                <td style="width: 235px">
                                                                                                                    <asp:TextBox ID="txtExpire" runat="server" CssClass="formtext" Width="191px" Enabled="true" meta:resourcekey="txtExpireResource1" Text="Unlimited"></asp:TextBox>
                                                                                                                    <a onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtExpire','cal','width=245,height=200,left=270,top=380')"
                                                                                                                        href="javascript:;">
                                                                                                                        <img src="../images/SmallCalendar.gif" border="0"></a>&nbsp;
																											<asp:ImageButton ID="cmdCancelExpire" runat="server" ImageUrl="../images/Cancel.gif" ToolTip="Cancel Expire Date"
                                                                                                                CausesValidation="False" meta:resourcekey="cmdCancelExpireResource1"></asp:ImageButton>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </td>

                                                                                                </tr>
                                                                                                <tr style="height: 20px">
                                                                                                    <td class="formtext" style="text-align: left; width: 80px;">
                                                                                                        <asp:Label ID="lblStatus" meta:resourcekey="lblStatus" runat="server" Text="Status:"></asp:Label>
                                                                                                    </td>
                                                                                                    <td style="width: 235px">
                                                                                                        <asp:Label ID="lblUserStatusText" runat="server" CssClass="formtext"></asp:Label>
                                                                                                    </td>
                                                                                                    <td class="formtext" style="text-align: left; width: 100px;">
                                                                                                        <asp:Label ID="lblChangeStatus" meta:resourcekey="lblChangeStatus" runat="server" Text="Change Status:"></asp:Label>
                                                                                                    </td>
                                                                                                    <td style="width: 235px">
                                                                                                        <asp:DropDownList ID="cboStatus" runat="server" CssClass="formtext"
                                                                                                            Width="210px">
                                                                                                        </asp:DropDownList>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td class="formtext" style="text-align: left;" colspan="4">

                                                                                                        <table class="formtext" id="tblPassword" runat="server" border="0" cellpadding="0" cellspacing="0">
                                                                                                            <tr>
                                                                                                                <td>
                                                                                                                    <asp:Label ID="lblPassword" runat="server" meta:resourcekey="lblPasswordResource1" Text="Password:"></asp:Label></td>
                                                                                                                <td>
                                                                                                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="formtext" Width="210px" TextMode="Password" meta:resourcekey="txtPasswordResource1"></asp:TextBox></td>
                                                                                                                <td>
                                                                                                                    <asp:RequiredFieldValidator ID="valPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Please Enter a Password" meta:resourcekey="valPasswordResource1" Text="*"></asp:RequiredFieldValidator></td>
                                                                                                                <td><span id="strength" runat="server"></span>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr>
                                                                                                                <td>
                                                                                                                    <asp:Label ID="lblReenterPassword" runat="server" Text="Re-enter the  password:" meta:resourcekey="lblReenterPasswordResource1"></asp:Label></td>
                                                                                                                <td>
                                                                                                                    <asp:TextBox ID="txtReenterPassword" runat="server" CssClass="formtext" Width="210px" TextMode="Password" meta:resourcekey="txtReenterPasswordResource1"></asp:TextBox>
                                                                                                                </td>
                                                                                                                <td>
                                                                                                                    <asp:CompareValidator ID="valPasswordComp" runat="server" ControlToCompare="txtReenterPassword"
                                                                                                                        ControlToValidate="txtPassword" ErrorMessage="Please re-enter the new password"
                                                                                                                        Text="*" meta:resourcekey="valPasswordCompResource1"></asp:CompareValidator></td>
                                                                                                                <td>
                                                                                                                    <asp:HiddenField ID="txtPasswordStatus" runat="server" />
                                                                                                                    <input id="txtPasswordStatus1" type="hidden" name="txtPasswordStatus1">
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>

                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td colspan="4" style="height: 10px"></td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td align="center" colspan="4">
                                                                                                        <table id="Table8" style="width: 447px;" cellspacing="0" cellpadding="0" border="0">
                                                                                                            <tr>
                                                                                                                <td align="center">
                                                                                                                    <table id="tblUsers" style="width: 433px;" cellspacing="0" cellpadding="0"
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
                                                                                                                                    Rows="10" meta:resourcekey="lstUnassignedGroupsResource1"></asp:ListBox>
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
                                                                                                                                <asp:ListBox ID="lstAssignedGroups" runat="server" CssClass="formtext" Width="160px"
                                                                                                                                    DataValueField="UserGroupId" DataTextField="UserGroupName" SelectionMode="Multiple"
                                                                                                                                    Rows="10" meta:resourcekey="lstAssignedGroupsResource1"></asp:ListBox>
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
                                                                                                <tr>
                                                                                                    <td class="formtext" style="text-align: left">
                                                                                                        <asp:Label ID="lblUserGroup" runat="server" Text="User Group:" Visible="false" meta:resourcekey="lblUserGroupResource1"></asp:Label></td>

                                                                                                    <td class="formtext" style="text-align: left">
                                                                                                        <asp:DropDownList ID="ddlUserGroup" Width="210px" runat="server" CssClass="formtext" DataTextField="UserGroupName" DataValueField="UserGroupId" Visible="false" meta:resourcekey="ddlUserGroupResource1"></asp:DropDownList>
                                                                                                        <asp:CompareValidator ID="CompareUserGroup" runat="server" ControlToValidate="ddlUserGroup" Operator="GreaterThan" ValueToCompare="-1" Text="*" ErrorMessage="User group is not selected" meta:resourcekey="CompareUserGroupResource1"></asp:CompareValidator></td>
                                                                                                    <td class="formtext" style="width: 100px; text-align: left"></td>
                                                                                                    <td style="width: 235px"></td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td align="left" colspan="4">
                                                                                                        <asp:LinkButton ID="lnkResetPassword" runat="server" Font-Names="Tahoma" Font-Size="X-Small" CausesValidation="False" meta:resourcekey="lnkResetPasswordResource1">Reset Password</asp:LinkButton>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr id="organizationHierarchy" visible="false" runat="server">
                                                                                                    <td colspan="4">
                                                                                                        <table border="0" cellpadding="0" cellspacing="5">
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
                                                                                                    <td colspan="4" align="center">
                                                                                                        <asp:Button ID="cmdSaveUser" runat="server" CssClass="combutton" Text="Save" OnClick="cmdSaveUser_Click" meta:resourcekey="cmdSaveUserResource1"></asp:Button>
                                                                                                          &nbsp;&nbsp;
                                                                                                        <asp:Button ID="cmdCancelAddUser" runat="server" CausesValidation="False" CssClass="combutton"
                                                                                                            Text="Cancel" OnClick="cmdCancelAddUser_Click" meta:resourcekey="cmdCancelAddUserResource1"></asp:Button>
                                                                                                      
																											
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>

                                                                                             <table id="tblSearch" runat="server" style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; border-bottom: gray 1px solid; width: 328px;">
                                                                                                <tr>
                                                                                                    <td class="style1"></td>
                                                                                                    <td class="style2"></td>
                                                                                                    <td class="style3"></td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td style="width: 138px">&nbsp;<asp:DropDownList
                                                                                                        ID="cboSearchType" runat="server" AutoPostBack="True" CssClass="RegularText"
                                                                                                        Width="124px" meta:resourcekey="cboSearchTypeResource1"
                                                                                                        OnSelectedIndexChanged="cboSearchType_SelectedIndexChanged">
                                                                                                        <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource1" Text="User Name"></asp:ListItem>
                                                                                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="Last Name"></asp:ListItem>
                                                                                                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource3"></asp:ListItem>

                                                                                                    </asp:DropDownList>
                                                                                                    </td>
                                                                                                    <td style="width: 5px"></td>
                                                                                                    <td style="width: 183px">
                                                                                                        <asp:TextBox ID="txtSearchParam" runat="server" CssClass="formtext"
                                                                                                            Width="163px" meta:resourcekey="txtSearchParamResource1" onkeypress="return clickButton(event,'cmdSearch')"></asp:TextBox>
                                                                                                        <asp:DropDownList ID="cboStatusSearch" runat="server" CssClass="formtext"
                                                                                                            Visible="False" Width="163px">
                                                                                                            <asp:ListItem meta:resourcekey="cboStatusSearc_All" Selected="True" Value="0">All</asp:ListItem>
                                                                                                            <asp:ListItem meta:resourcekey="cboStatusSearc_Active" Value="Active">Active</asp:ListItem>
                                                                                                            <asp:ListItem meta:resourcekey="cboStatusSearc_Deleted" Value="Deleted">Deleted</asp:ListItem>
                                                                                                            <asp:ListItem meta:resourcekey="cboStatusSearc_Reset" Value="Reset Password">Reset Password</asp:ListItem>
                                                                                                            <asp:ListItem meta:resourcekey="cboStatusSearc_Deactivated" Value="Deactivated">Deactivated</asp:ListItem>
                                                                                                        </asp:DropDownList>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td align="left" colspan="3" style="height: 22px">&nbsp;<asp:Button ID="cmdClear" runat="server" CssClass="combutton"
                                                                                                        OnClick="cmdClear_Click" Text="Clear" Width="124px" meta:resourcekey="cmdClearResource1" CausesValidation="False" />
                                                                                                        &nbsp; &nbsp;<asp:Button ID="cmdSearch" runat="server" CssClass="combutton"
                                                                                                            OnClick="cmdSearch_Click" Text="Search" Width="163px" meta:resourcekey="cmdSearchResource1" CausesValidation="False" /></td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td style="width: 118px; height: 7px">                                                                                                       
                                                                                                       
                                                                                                    </td>
                                                                                                    <td align="center" style="width: 5px; height: 7px">
                                                                                                        
                                                                                                    </td>
                                                                                                    <td align="center" style="width: 183px; height: 7px"></td>
                                                                                                </tr>
                                                                                                
                                                                                            </table>
                                                                                            <br />
                                                                                            <table id =" tblDeletedUser">
                                                                                                <tr>
                                                                                                    <td align="left" style="width: 160px; height: 7px">
                                                                                                         <asp:CheckBox ID="chkbxViewDeletedUser"  runat="server" Text="View Deleted Users"  OnCheckedChanged="chkbxViewDeletedUser_CheckChanged"  AutoPostBack="true" Font-Names="Serif" Font-Size="Small"></asp:CheckBox>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            <table id="Table2" style="width:auto;">
                                                                                                <tr>             
                                                                                                    <td valign="top" style="text-align:right;color:Black;text-decoration:underline;">
                                                                                                        <asp:PlaceHolder ID="imgExcel" runat="server" Visible="true">
                                                                                                            <a href="#"  onclick="ExportToExcel()" class="RegularText"><%=ExportToExcel%></a>
                                                                                                        </asp:PlaceHolder>
                                                                                                    </td>
                                                                                                </tr>

                                                                                                <tr>
                                                                                                    <td>
                                                                                                        <asp:DataGrid ID="dgUsers" runat="server" PageSize="11" AllowPaging="True" DataKeyField="UserId"
                                                                                                            AutoGenerateColumns="False" CellPadding="3" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px"
                                                                                                            BackColor="White" GridLines="None" CellSpacing="1" OnSelectedIndexChanged="dgUsers_SelectedIndexChanged" OnDeleteCommand="dgUsers_DeleteCommand" OnItemCreated="dgUsers_ItemCreated" meta:resourcekey="dgUsersResource1">
                                                                                                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                                                            <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                                                            <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                                                            <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                                            <Columns>
                                                                                                                <asp:BoundColumn Visible="False" DataField="PersonId" ReadOnly="True" HeaderText="Person Id"></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="FirstName" HeaderText='<%$ Resources:dgUsers_FirstName %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="LastName" HeaderText='<%$ Resources:dgUsers_LastName %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="UserName" HeaderText='<%$ Resources:dgUsers_UserName %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="ExpiredDate" HeaderText='<%$ Resources:dgUsers_ExpiredDate %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="UserStatus" HeaderText='<%$ Resources:dgUsers_Status %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="UserGroups" HeaderText='<%$ Resources:dgUsers_UserGroups %>' ItemStyle-Width="220"></asp:BoundColumn>
                                                                                                                <asp:ButtonColumn Text="&lt;img src=../images/edit.gif border=0&gt;" CommandName="Select" meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
                                                                                                                <asp:ButtonColumn Text='<%$ Resources:dgUsers_Settings %>' CommandName="Settings"></asp:ButtonColumn>
                                                                                                            </Columns>
                                                                                                            <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6" Mode="NumericPages"></PagerStyle>
                                                                                                        </asp:DataGrid>  
                                                                                                    </td>
                                                                                                    
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="height: 10px" align="center">
                                                                                            <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="270px" Height="8px" Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label> <br /> <br /></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="height: 25px" align="center">
                                                                                            <asp:Button ID="cmdAddUser" runat="server" CommandName="18" CausesValidation="False" CssClass="combutton"
                                                                                                Text="Add User" OnClick="cmdAddUser_Click" meta:resourcekey="cmdAddUserResource1"></asp:Button></td>
                                                                                    </tr>
                                                                                    <tr style="text-align: center">
                                                                                        <td>
                                                                                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext" meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
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
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="expdata" runat="server" Value='' />
    </form>

    <script language="javascript">
        function passwordChanged() {
            var strength = document.getElementById('strength');
            var strongRegex = new RegExp("^(?=.{8,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
            var mediumRegex = new RegExp("^(?=.{7,})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$", "g");
            var enoughRegex = new RegExp("(?=.{6,}).*", "g");
            var pwd = document.getElementById("txtPassword");
            var txtPasswordStatus = document.getElementById("txtPasswordStatus"); //document.forms[0].elements["txtPasswordStatus"];

            if (pwd.value.length == 0) {
                strength.innerHTML = '<%=msgPsw_TypePassword%>';
	            } else if (false == enoughRegex.test(pwd.value)) {
	                strength.innerHTML = '<%=msgPsw_MoreCharacters%>';
	                txtPasswordStatus.value = "0";
	            } else if (strongRegex.test(pwd.value)) {
	                strength.innerHTML = '<span style="color:green"><%=msgPsw_Strong%></span>';
	                txtPasswordStatus.value = "1";
	            } else if (mediumRegex.test(pwd.value)) {
	                strength.innerHTML = '<span style="color:orange"><%=msgPsw_Medium%></span>';
	                txtPasswordStatus.value = "1";
	            } else {
	                strength.innerHTML = '<span style="color:red"><%=msgPsw_Weak%></span>';
	                txtPasswordStatus.value = "0";
	            }
        }

        function ExportToExcel() {
            document.getElementById('exportdata').value = document.getElementById('expdata').value;
            document.getElementById('filename').value = "users";
            document.getElementById('formatter').value = "excel2007";
            exportform.submit();
        }
    </script>

    <iframe id="exportframe" name="exportframe" style="display: none"></iframe>
    <form id="exportform" method="post" target="exportframe" action="../MapNew/frmExportData.aspx">
        <input type="hidden" id="exportdata" name="exportdata" value='' />
        <input type="hidden" id="filename" name="filename" value='' />
        <input type="hidden" id="formatter" name="formatter" value='' />
    </form>
</body>
</html>
