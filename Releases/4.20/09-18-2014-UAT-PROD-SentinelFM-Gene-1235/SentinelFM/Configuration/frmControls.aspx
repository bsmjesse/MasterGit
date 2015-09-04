<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmControls.aspx.cs"
    Culture="en-US" UICulture="auto" Inherits="SentinelFM.Configuration_frmControls" meta:resourcekey="PageResource1" %>

<%@ Register Src="Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Controls</title>
    <script language="javascript" type="text/javascript">
        function controlsWindow(CId) {
            var ControlsPage = 'frmControls.aspx?CId=' + CId
            window.location.href = ControlsPage;
        }
    </script>
    <style type="text/css">
        .TextboxStyle {
            width: 250px;
        }

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
    <form id="frmVehicleInfo" method="post" runat="server">
        <table id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellspacing="0" cellpadding="0" width="300" border="0">
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
                                                        <table id="Table5" style="z-index: 101; width: 190px; position: relative; top: 0px; height: 22px"
                                                            cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdUserInfo" runat="server" CommandName="17" CausesValidation="False"
                                                                        CssClass="confbutton" Text="User Info" Width="112px" OnClick="cmdUserInfo_Click" meta:resourcekey="cmdUserInfoResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdUserGroups" runat="server" CommandName="21" CausesValidation="False"
                                                                        CssClass="confbutton" Text="User-Groups Assignment" Width="173px" Height="22px"
                                                                        OnClick="cmdUserGroups_Click" meta:resourcekey="cmdUserGroupsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdGroups" runat="server" Text="Groups" CssClass="confbutton"
                                                                        CausesValidation="False" CommandName="71" Width="112px" OnClick="cmdGroups_Click" meta:resourcekey="cmdGroupsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdGroupConfiguration" runat="server" Text="Group Configuration" CssClass="confbutton" OnClick="cmdGroupConfiguration_Click"
                                                                        CausesValidation="False" CommandName="79" Width="200px" meta:resourcekey="cmdGroupConfigurationResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdControls" runat="server" Text="Controls" CssClass="selectedbutton" OnClick="cmdControls_Click"
                                                                        CausesValidation="False" CommandName="70" Width="112px" meta:resourcekey="cmdControlsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdServices" runat="server" Text="Services" CssClass="confbutton" OnClick="cmdServices_Click"
                                                                        CausesValidation="False" CommandName="90" Width="112px" meta:resourcekey="cmdServicesResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdUserDashBoards" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                        Visible="False" Text="User-DashBoards" Width="173px" Height="22px" OnClick="cmdUserDashBoards_Click" meta:resourcekey="cmdUserDashBoardsResource1"></asp:Button>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <asp:PlaceHolder ID="plhControls" runat="server">
                                                    <tr>
                                                        <td>
                                                            <table id="Table6" cellspacing="0" cellpadding="0" width="617" align="center" border="0">
                                                                <tr>
                                                                    <td>
                                                                        <table id="Table7" class="table" height="500px" width="960px"
                                                                            border="0">
                                                                            <tr>
                                                                                <td class="configTabBackground" style="width: 100%; vertical-align:top" align="center" valign="top">
                                                                                    <table id="Table1" cellspacing="0" cellpadding="0" align="center" border="0">
                                                                                        <tr>
                                                                                            <td style="height: 5px"></td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="width: 100%;" align="center" valign="top">
                                                                                                <table id="tblSearch" runat="server" style="border-right: gray 1px solid; border-top: gray 1px solid; border-left: gray 1px solid; border-bottom: gray 1px solid; width: 328px;"
                                                                                                    class="formtext">
                                                                                                    <tr>
                                                                                                        <td style="height: 20px" colspan="3"></td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td style="width: 60px; text-align: right">
                                                                                                            <asp:Label ID="lblControl" runat="server" Text="Control:"></asp:Label>
                                                                                                        </td>
                                                                                                        <td style="width: 5px"></td>
                                                                                                        <td>
                                                                                                            <asp:TextBox ID="txtSearchParam" runat="server" CssClass="formtext" Width="163px"></asp:TextBox>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td colspan="3" style="height: 22px; text-align: center">
                                                                                                            <asp:Button ID="cmdClear" runat="server" CssClass="combutton" OnClick="cmdClear_Click"
                                                                                                                Text="Clear" CausesValidation="False" />
                                                                                                            &nbsp;&nbsp;<asp:Button ID="cmdSearch" runat="server" CssClass="combutton" OnClick="cmdSearch_Click"
                                                                                                                Text="Search" CausesValidation="False" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                    <tr>
                                                                                                        <td style="height: 5px" colspan="3">&nbsp;</td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="height: 10px"></td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="width: 100%; vertical-align:top" align="center" valign="top">
                                                                                                <asp:DataGrid ID="dgControls" runat="server" PageSize="12" AllowPaging="True" DataKeyField="ControlId"
                                                                                                    AutoGenerateColumns="False" CellPadding="3" BorderColor="White" BorderStyle="Ridge"
                                                                                                    BorderWidth="2px" BackColor="White" GridLines="None" CellSpacing="1" OnSelectedIndexChanged="dgControls_SelectedIndexChanged"
                                                                                                    meta:resourcekey="dgControlsResource1">
                                                                                                    <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                                                    <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                                                    <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                                                    <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                                    <Columns>
                                                                                                        <asp:BoundColumn DataField="ControlId" HeaderText="CommandName"></asp:BoundColumn>
                                                                                                        <asp:BoundColumn DataField="ControlDescription" HeaderText='Control Name'></asp:BoundColumn>
                                                                                                        <asp:BoundColumn DataField="FormName" HeaderText='Form Name'></asp:BoundColumn>
                                                                                                        <asp:BoundColumn DataField="ControlIsActive" HeaderText='Is Active'></asp:BoundColumn>
                                                                                                        <asp:ButtonColumn Text="&lt;img src=../images/edit.gif border=0&gt;" HeaderText='Edit'
                                                                                                            CommandName="Select" meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
                                                                                                    </Columns>
                                                                                                    <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                                                        Mode="NumericPages"></PagerStyle>
                                                                                                </asp:DataGrid>
                                                                                            </td>
                                                                                        </tr>

                                                                                        <tr>
                                                                                            <td style="height: 25px; text-align: center">
                                                                                                <asp:Button ID="cmAddControl" runat="server" CausesValidation="False" CssClass="combutton" ToolTip="Click to add new control"
                                                                                                    Text="Add Control" OnClick="cmAddControl_Click" meta:resourcekey="cmAddControlResource1"></asp:Button>
                                                                                            </td>
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
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plhControlForm" runat="server" Visible="False">
                                                    <tr>
                                                        <td>
                                                            <table id="Table2" cellspacing="0" cellpadding="0" width="617" align="center" border="0">
                                                                <tr>
                                                                    <td>
                                                                        <table id="Table3" class="table" height="500px" width="960px" border="0">
                                                                            <tr>
                                                                                <td class="configTabBackground" valign="top">
                                                                                    <table id="Table4" style="width: 600px; height: 20px" cellspacing="0" cellpadding="2"
                                                                                        align="center" border="0">
                                                                                        <tr>
                                                                                            <td style="height: 20px" colspan="2"></td>
                                                                                        </tr>
                                                                                        <asp:PlaceHolder ID="plhMsg" runat="server" Visible="False">
                                                                                            <tr>
                                                                                                <td class="formtext" colspan="2" style="color: #B22222; text-align: center">
                                                                                                    <asp:Label ID="lblMsg" runat="server" Text=""
                                                                                                        meta:resourcekey="lblMsgResource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </asp:PlaceHolder>
                                                                                        <asp:PlaceHolder ID="plhCommandNumber" runat="server" Visible="False">
                                                                                            <tr>
                                                                                                <td class="formtext" width="40%" style="text-align: right">
                                                                                                    <asp:Label ID="lblCommandNumber" runat="server" Text="CommandName:" meta:resourcekey="lblCommandNumberResource1"></asp:Label>&nbsp;
                                                                                                </td>
                                                                                                <td class="formtext">
                                                                                                    <asp:Label ID="lblCommandNumberValue" runat="server" Text="" CssClass="formtext" meta:resourcekey="lblCommandNumberValueResource1"></asp:Label>
                                                                                                </td>
                                                                                            </tr>
                                                                                        </asp:PlaceHolder>
                                                                                        <asp:Panel ID="pnlControlNames" runat="server" meta:resourcekey="pnlControlNamesResource1" />
                                                                                         <tr>
                                                                                            <td class="formtext" width="40%" style="text-align: right">
                                                                                                <asp:Label ID="lblParentControl" runat="server" Text="Parent Control:" meta:resourcekey="lblParentControlResource1"></asp:Label>&nbsp;
                                                                                            </td>
                                                                                            <td class="formtext">
                                                                                                <asp:DropDownList ID="cboParentControl" runat="server" CssClass="formtext"
                                                                                                    DataValueField="ControlID" DataTextField="ControlName" meta:resourcekey="cboParentControlResource1">
                                                                                                </asp:DropDownList>
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td class="formtext" width="40%" style="text-align: right">
                                                                                                <asp:Label ID="lblForm" runat="server" Text="Form:" meta:resourcekey="lblFormResource1"></asp:Label>&nbsp;
                                                                                            </td>
                                                                                            <td class="formtext">
                                                                                                <asp:DropDownList ID="cboForm" runat="server" CssClass="formtext"
                                                                                                    DataTextField="FormName" DataValueField="FormID" meta:resourcekey="cboFormResource1">
                                                                                                </asp:DropDownList>
                                                                                                &nbsp;<asp:RequiredFieldValidator ID="rfvForm" runat="server" ControlToValidate="cboForm"
                                                                                                    Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick" meta:resourcekey="rfvFormResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td class="formtext" width="40%" style="text-align: right">
                                                                                                <asp:Label ID="lblControlID" runat="server" Text="ASP.NET Control ID:" meta:resourcekey="lblControlIDResource1"></asp:Label>&nbsp;
                                                                                            </td>
                                                                                            <td class="formtext">
                                                                                                <asp:TextBox ID="txtControlID" runat="server" Width="250px" CssClass="formtext" MaxLength="50" meta:resourcekey="txtControlIDResource1"></asp:TextBox>
                                                                                                &nbsp;<asp:RequiredFieldValidator ID="rfvControlID" runat="server" ControlToValidate="txtControlID"
                                                                                                    Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick" meta:resourcekey="rfvControlIDResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td class="formtext" width="40%" style="text-align: right">
                                                                                                <asp:Label ID="lblDescription" runat="server" Text="Description:" meta:resourcekey="lblDescriptionResource1"></asp:Label>&nbsp;
                                                                                            </td>
                                                                                            <td class="formtext">
                                                                                                <asp:TextBox ID="txtDescription" runat="server" Width="250px" CssClass="formtext" MaxLength="255" meta:resourcekey="txtDescriptionResource1"></asp:TextBox>
                                                                                                &nbsp;<asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                                                                                                    Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick" meta:resourcekey="rfvDescriptionResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td class="formtext" width="40%" style="text-align: right">
                                                                                                <asp:Label ID="lblURL" runat="server" Text="URL:" meta:resourcekey="lblURLResource1"></asp:Label>&nbsp;
                                                                                            </td>
                                                                                            <td class="formtext">
                                                                                                <asp:TextBox ID="txtURL" runat="server" Width="250px" CssClass="formtext" MaxLength="300" meta:resourcekey="txtURLResource1"></asp:TextBox>
                                                                                                &nbsp;<asp:RequiredFieldValidator ID="rfvURL" runat="server" ControlToValidate="txtURL"
                                                                                                    Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick" meta:resourcekey="rfvURLResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td class="formtext" width="40%" style="text-align: right">
                                                                                                <asp:Label ID="lblIsActive" runat="server" Text="Is Active:" meta:resourcekey="lblIsActiveResource1"></asp:Label>&nbsp;
                                                                                            </td>

                                                                                            <td class="formtext">
                                                                                                <asp:CheckBox ID="chkIsActive" runat="server" meta:resourcekey="chkIsActiveResource1" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align="center" colspan="2">
                                                                                                <asp:Button ID="cmdCancel" runat="server" CausesValidation="False" CssClass="combutton" ToolTip="Click to cancel"
                                                                                                    Text="Cancel" OnClick="cmdCancel_Click" meta:resourcekey="cmdCancelResource1"></asp:Button>
                                                                                                &nbsp;
                                                                                                            <asp:Button ID="cmdUpdate" runat="server" CssClass="combutton" Text="Update"
                                                                                                                OnClick="cmdUpdate_Click" meta:resourcekey="cmdUpdateResource1" />
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
    </form>
</body>
</html>
