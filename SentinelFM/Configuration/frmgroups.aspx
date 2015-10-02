<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmgroups.aspx.cs" Culture="en-US"
    Inherits="SentinelFM.Configuration_frmgroups" meta:resourcekey="PageResource1"
    UICulture="auto" %>

<%@ Register Src="Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
    <title>Groups</title>
    <script language="javascript" type="text/javascript">
        function controlsWindow(GId) {
            var mypage = 'frmGroupControls.aspx?GId=' + GId
            var myname = 'GroupControls';
            var w = 960;
            var h = screen.height - 250;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
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
    <style type="text/css">
        .style1
        {
            width: 118px;
            height: 23px;
        }
        .style2
        {
            width: 5px;
            height: 23px;
        }
        .style3
        {
            width: 183px;
            height: 23px;
        }
    </style>
</head>
<body>
    <form id="frmVehicleInfo" method="post" runat="server">
    <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px"
        cellspacing="0" cellpadding="0" width="300px" border="0">
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
                                        <table id="tblSubCommands" style="left: 10px; position: relative; top: 0px" cellspacing="0"
                                            cellpadding="0" border="0">
                                            <tr>
                                                <td>
                                                    <table id="Table5" style="z-index: 101; width: 190px; position: relative; top: 0px;
                                                        height: 22px" cellspacing="0" cellpadding="0" border="0">
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="cmdUserInfo" runat="server" CommandName="17" CausesValidation="False"
                                                                    CssClass="confbutton" Text="User Info" Width="112px" OnClick="cmdUserInfo_Click"
                                                                    meta:resourcekey="cmdUserInfoResource1"></asp:Button>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="cmdUserGroups" runat="server" CommandName="21" CausesValidation="False"
                                                                    CssClass="confbutton" Text="User-Groups Assignment" Width="173px" Height="22px"
                                                                    OnClick="cmdUserGroups_Click" meta:resourcekey="cmdUserGroupsResource1"></asp:Button>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="cmdGroups" runat="server" Text="Groups" CssClass="selectedbutton"
                                                                    CausesValidation="False" CommandName="71" Width="112px" meta:resourcekey="cmdGroupsResource1">
                                                                </asp:Button>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="cmdGroupConfiguration" runat="server" Text="Group Configuration" CssClass="confbutton" OnClick="cmdGroupConfiguration_Click"
                                                                    CausesValidation="False" CommandName="79" Width="200px" meta:resourcekey="cmdGroupConfigurationResource1">
                                                                </asp:Button>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="cmdControls" runat="server" Text="Controls" CssClass="confbutton" OnClick="cmdControls_Click"
                                                                    CausesValidation="False" CommandName="70" Width="112px" meta:resourcekey="cmdControlsResource1">
                                                                </asp:Button>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="cmdServices" runat="server" Text="Services" CssClass="confbutton" OnClick="cmdServices_Click"
                                                                    CausesValidation="False" CommandName="90" Width="112px" meta:resourcekey="cmdServicesResource1"></asp:Button>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="cmdUserDashBoards" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                    Visible="False" Text="User-DashBoards" Width="173px" Height="22px" OnClick="cmdUserDashBoards_Click">
                                                                </asp:Button>
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
                                                                        <td class="configTabBackground">
                                                                            <table id="Table1" style="height: 444px" cellspacing="0" cellpadding="0" align="center"
                                                                                border="0">
                                                                                <tr>
                                                                                    <td style="width: 100%; height: 360px" align="center" valign="top">
                                                                                        <table id="tblSearch" runat="server" style="border-right: gray 1px solid; border-top: gray 1px solid;
                                                                                            border-left: gray 1px solid; border-bottom: gray 1px solid; width: 328px;" class="formtext">
                                                                                            <tr>
                                                                                                <td class="style1">
                                                                                                </td>
                                                                                                <td class="style2">
                                                                                                </td>
                                                                                                <td class="style3">
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="width: 60px" align="right">
                                                                                                    <asp:Label ID="lblGroup" runat="server" Text="Group:" meta:resourcekey="lblGroupResource1"></asp:Label>
                                                                                                </td>
                                                                                                <td style="width: 5px">
                                                                                                </td>
                                                                                                <td>
                                                                                                    <asp:TextBox ID="txtSearchParam" runat="server" CssClass="formtext" Width="163px"
                                                                                                        meta:resourcekey="txtSearchParamResource1" onkeypress="return clickButton(event,'cmdSearch')"></asp:TextBox>
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td align="center" colspan="3" style="height: 22px">
                                                                                                    <asp:Button ID="cmdClear" runat="server" CssClass="combutton" OnClick="cmdClear_Click"
                                                                                                        Text="Clear" meta:resourcekey="cmdClearResource1" CausesValidation="False" />
                                                                                                    &nbsp;&nbsp;<asp:Button ID="cmdSearch" runat="server" CssClass="combutton" OnClick="cmdSearch_Click"
                                                                                                        Text="Search" meta:resourcekey="cmdSearchResource1" CausesValidation="False" />
                                                                                                </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                                <td style="height: 7px">
                                                                                                </td>
                                                                                                <td style="width: 5px; height: 7px">
                                                                                                </td>
                                                                                                <td style="height: 7px">
                                                                                                </td>
                                                                                            </tr>
                                                                                        </table>
                                                                                        <br />
                                                                                        <asp:DataGrid ID="dgGroups" runat="server" PageSize="12" AllowPaging="True" DataKeyField="UserGroupId"
                                                                                            AutoGenerateColumns="False" CellPadding="3" BorderColor="White" BorderStyle="Ridge"
                                                                                            BorderWidth="2px" BackColor="White" GridLines="None" CellSpacing="1" 
                                                                                            OnSelectedIndexChanged="dgGroups_SelectedIndexChanged"
                                                                                            OnItemCreated="dgGroups_ItemCreated"
                                                                                            OnDeleteCommand="dgGroups_DeleteCommand"
                                                                                            meta:resourcekey="dgGroupsResource1">
                                                                                            <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                                            <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                                            <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                            <Columns>
                                                                                                <asp:BoundColumn Visible="false" DataField="UserGroupId" ReadOnly="true" HeaderText="UserGroupId">
                                                                                                </asp:BoundColumn>
                                                                                                <asp:BoundColumn Visible="false" DataField="AllowEdit" ReadOnly="true" HeaderText="AllowEdit">
                                                                                                </asp:BoundColumn>
                                                                                                <asp:BoundColumn DataField="UserGroupName" HeaderText='<%$ Resources:dgGroups_Group %>'>
                                                                                                </asp:BoundColumn>
                                                                                                <asp:BoundColumn DataField="SecurityLevelName" HeaderText='<%$ Resources:dgGroups_SecurityLevel %>'>
                                                                                                </asp:BoundColumn>
                                                                                                <asp:ButtonColumn Text="&lt;img src=../images/edit.gif border=0&gt;" HeaderText=''
                                                                                                    CommandName="Select"></asp:ButtonColumn>
                                                                                                <asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" HeaderText=''
                                                                                                    CommandName="Delete"></asp:ButtonColumn>
                                                                                            </Columns>
                                                                                            <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                                                Mode="NumericPages"></PagerStyle>
                                                                                        </asp:DataGrid>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="height: 10px" align="center">
                                                                                        <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="270px" Height="8px"
                                                                                            Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td style="height: 25px" align="center">
                                                                                        <asp:Button ID="cmdCreateDefaultGroups" runat="server" CausesValidation="False" CssClass="combutton" ToolTip="Click to creaate default Usergroups"
                                                                                            Text="Add Default Groups" Width="140px" meta:resourcekey="cmdCreateDefaultGroupsResource1" OnClick="cmdCreateDefaultGroups_Click"></asp:Button>
                                                                                        <asp:Button ID="cmdAddGroup" runat="server" CausesValidation="False" CssClass="combutton" ToolTip="Click to add new Usergroup"
                                                                                            Text="Add Group" meta:resourcekey="cmAddGroupResource1" OnClientClick="controlsWindow(0);"></asp:Button>
                                                                                        
                                                                                    </td>
                                                                                </tr>
                                                                                <tr style="text-align: center">
                                                                                    <td>
                                                                                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext"
                                                                                            meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
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
    </form>
</body>
</html>
