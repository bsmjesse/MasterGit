<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmServices.aspx.cs"
    Culture="en-US" UICulture="auto" Inherits="SentinelFM.Configuration_frmServices" meta:resourcekey="PageResource1" %>

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
                                                                    <asp:Button ID="cmdControls" runat="server" Text="Controls" CssClass="confbutton" OnClick="cmdControls_Click"
                                                                        CausesValidation="False" CommandName="70" Width="112px" meta:resourcekey="cmdControlsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdServices" runat="server" Text="Services" CssClass="selectedbutton" OnClick="cmdServices_Click"
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

                                                <tr>
                                                    <td>
                                                        <table id="Table6" cellspacing="0" cellpadding="0" width="617" align="center" border="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table7" class="table" height="500px" width="960px"
                                                                        border="0">
                                                                        <tr>
                                                                            <td class="configTabBackground" style="width: 100%; vertical-align: top" align="center" valign="top">
                                                                                <table id="Table1" cellspacing="0" cellpadding="2" align="center" border="0">
                                                                                    <tr>
                                                                                        <td colspan="2" style="height: 5px"></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td class="formtext" width="40%" style="text-align: right">
                                                                                            <asp:Label ID="lblOrganization" runat="server" Text="Organization:" meta:resourcekey="lblOrganizationResource1"></asp:Label>&nbsp;
                                                                                        </td>
                                                                                        <td class="formtext">
                                                                                            <asp:DropDownList ID="cboOrganization" runat="server" CssClass="formtext" AutoPostBack="true" CausesValidation="false"
                                                                                                DataValueField="OrganizationId" DataTextField="OrganizationName" meta:resourcekey="cboOrganizationResource1"
                                                                                                OnSelectedIndexChanged="cboOrganization_SelectedIndexChanged">
                                                                                            </asp:DropDownList>
                                                                                            &nbsp;
                                                                                            <asp:RequiredFieldValidator ID="rfvOrganization" runat="server" ControlToValidate="cboOrganization"
                                                                                                Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick"
                                                                                                meta:resourcekey="rfvOrganizationResource1" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td class="formtext" width="40%" style="text-align: right">
                                                                                            <asp:Label ID="lblService" runat="server" Text="Service:" meta:resourcekey="lblServiceResource1"></asp:Label>&nbsp;
                                                                                        </td>
                                                                                        <td class="formtext">
                                                                                            <asp:DropDownList ID="cboService" runat="server" CssClass="formtext" Width="180" AutoPostBack="true" CausesValidation="false"
                                                                                                DataValueField="ServiceID" DataTextField="ServiceName" meta:resourcekey="cboServiceResource1" OnSelectedIndexChanged="cboService_SelectedIndexChanged">
                                                                                            </asp:DropDownList>
                                                                                            &nbsp;
                                                                                            <asp:RequiredFieldValidator ID="rfvService" runat="server" ControlToValidate="cboService"
                                                                                                Display="Dynamic" ErrorMessage="mandatory field" ForeColor="Firebrick"
                                                                                                meta:resourcekey="rfvServiceResource1" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td class="formtext" width="40%" style="text-align: right">
                                                                                            <asp:Label ID="lblBillable" runat="server" Text="Billable:" meta:resourcekey="lblBillableResource1"></asp:Label>&nbsp;
                                                                                        </td>
                                                                                        <td class="formtext">
                                                                                            <asp:CheckBox ID="chkBillable" runat="server" AutoPostBack="true" OnCheckedChanged="chkBillable_CheckedChanged" />
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan="2" style="height: 25px; text-align: center">
                                                                                            <asp:Button ID="cmdEnableFeature" runat="server" CausesValidation="true" CssClass="combutton" ToolTip="Click to add new service"
                                                                                                Text="Enable" OnClick="cmdEnableFeature_Click" Visible="false" meta:resourcekey="cmdEnableFeatureResource1"></asp:Button>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <asp:PlaceHolder ID="plhMsg" runat="server" Visible="False">
                                                                                        <tr>
                                                                                            <td class="formtext" colspan="2" style="color: #B22222; text-align: center">
                                                                                                <asp:Label ID="lblMsg" runat="server" Text=""
                                                                                                    meta:resourcekey="lblMsgResource1"></asp:Label>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </asp:PlaceHolder>
                                                                                    <tr>
                                                                                        <td colspan="2" style="width: 100%; vertical-align: top" align="center" valign="top">
                                                                                            <table id="Table2" style="width: auto;">
                                                                                                <tr>             
                                                                                                    <td valign="top" style="text-align:right;color:Black;text-decoration:underline;">
                                                                                                        <asp:PlaceHolder ID="imgExcel" runat="server" Visible="true">
                                                                                                            <a href="#"  onclick="ExportToExcel()" class="RegularText"><%=ExportToExcel%></a>
                                                                                                        </asp:PlaceHolder>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td>
                                                                                                        <asp:DataGrid ID="dgOrganizationFeatures" runat="server" PageSize="11" AllowPaging="True"
                                                                                                            DataKeyField="OrganizationServiceID"
                                                                                                            AutoGenerateColumns="False" CellPadding="3" BorderColor="White" BorderStyle="Ridge"
                                                                                                            BorderWidth="2px" BackColor="White" GridLines="None" CellSpacing="1"
                                                                                                            OnItemCreated="dgOrganizationFeatures_ItemCreated"
                                                                                                            OnDeleteCommand="dgOrganizationFeatures_DeleteCommand"
                                                                                                            meta:resourcekey="dgOrganizationFeaturesResource1">
                                                                                                            <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                                                            <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                                                            <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                                                            <Columns>
                                                                                                                <asp:BoundColumn DataField="ServiceName" HeaderText='<%$ Resources:dgOrganizationFeatures_ServiceName %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="OrganizationName" HeaderText='<%$ Resources:dgOrganizationFeatures_Organization %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="UserName" HeaderText='<%$ Resources:dgOrganizationFeatures_EnabledByUser %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="ServiceStartDate" HeaderText='<%$ Resources:dgOrganizationFeatures_StartDate %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="ServiceEndDate" HeaderText='<%$ Resources:dgOrganizationFeatures_EndDate %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="IsBillable" HeaderText='<%$ Resources:dgOrganizationFeatures_IsBillable %>'></asp:BoundColumn>
                                                                                                                <asp:BoundColumn DataField="ServiceStatus" HeaderText='<%$ Resources:dgOrganizationFeatures_ServiceStatus %>'></asp:BoundColumn>
                                                                                                                <asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" HeaderText='<%$ Resources:dgOrganizationFeatures_Disable %>'
                                                                                                                    CommandName="Delete"></asp:ButtonColumn>
                                                                                                            </Columns>
                                                                                                            <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                                                                Mode="NumericPages"></PagerStyle>
                                                                                                        </asp:DataGrid>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan="2" style="height: 10px"></td>
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

    <script type="text/javascript">
        function ExportToExcel() {
            document.getElementById('exportdata').value = document.getElementById('expdata').value;
            document.getElementById('filename').value = 'services';
            document.getElementById('formatter').value = 'excel2007';
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
