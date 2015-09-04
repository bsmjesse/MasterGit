<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmFuelCategory.aspx.cs" Inherits="SentinelFM.Configuration_FuelCategory" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style>
        .GridColumnTitle
        {
            font-weight: bold;
            text-decoration: underline;
        }
    </style>
</head>
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        Style="text-decoration: underline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="gdMedia">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdMedia" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="text-align: left; width: 900px">
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" selectedcontrol="cmdOrganization" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td>
                                <table id="tblForm" border="0" cellpadding="0" cellspacing="0" class="table" style="height: 550px;
                                    width: 990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px;
                                                margin-top: 5px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table5" style="z-index: 101; width: 190px; position: relative; top: 0px;
                                                                        height: 22px" cellspacing="0" cellpadding="0" border="0">
                                                                        <tr>
<TD><asp:button id="cmdSettings" runat="server" Width="117px" 
                                                                                        CausesValidation="False" CssClass="confbutton"
																						Text="Settings" meta:resourcekey="cmdSettingsResource1"  PostBackUrl = "~\Configuration\frmOrganizationSettings.aspx" ></asp:button></TD>
																				<TD >
																				    <asp:button id="cmdPushSettings" runat="server" 
                                                                                        Width="117px" CausesValidation="False" CssClass="confbutton"
																						Text="Push Settings" PostBackUrl = "~\Configuration\frmOrganizationPushSettings.aspx" CommandName="51" meta:resourcekey="cmdPushSettingsResource1" ></asp:button>
																					</TD>
																				<TD>
																				    <asp:button id="cmdFuel" runat="server" Width="117px" CausesValidation="False" CssClass="confbutton"
																						Text="Fuel Transactions" PostBackUrl = "~\Configuration\frmFuelTranSettings.aspx"  CommandName="50" meta:resourcekey="cmdFuelResource1" />
																						
																					</TD>
<td>																				    <asp:button id="cmdMapSubscription" runat="server"
                                                                                        Width="117px" CausesValidation="False" CssClass="confbutton"
																						Text="Map Subscription" CommandName="54" PostBackUrl = "~\Configuration\frmmapsubscription.aspx"  meta:resourcekey="cmdMapSubscriptionResource1" ></asp:button>
                                                                                </TD>
                                                                                <TD>
																				    <asp:button id="cmdOverlaySubscription" runat="server"
                                                                                        Width="127px" CausesValidation="False" CssClass="confbutton"
																						Text="Overlay Subscription" CommandName="54" PostBackUrl = "~\Configuration\frmoverlaysubscription.aspx"  meta:resourcekey="cmdOverlaySubscriptionResource1" ></asp:button>
                                                                                </TD>
                                                                            <td>
                                                                                <asp:Button ID="btnFuelCategory" runat="server" Width="168px" CausesValidation="False" CssClass="selectedbutton"
                                                                                     Text="Fuel Category" 
                                                                                    meta:resourcekey="btnFuelCategoryResource1"></asp:Button>

                                                                            </td>

                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td>
                                                                    
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760"
                                                                        style="padding-bottom: 5px">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;"
                                                                                    class="tableDoubleBorder">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 950px;
                                                                                                margin-top: 5px; height: 495px">
                                                                                                <tr>
                                                                                                    <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                                                                        <telerik:RadGrid AutoGenerateColumns="False" ID="gdMedia" AllowAutomaticDeletes="false"
                                                                                                            PageSize="15" AllowAutomaticInserts="false" AllowSorting="true" AllowAutomaticUpdates="false"
                                                                                                            AllowPaging="True" runat="server" Skin="Hay" GridLines="None" meta:resourcekey="gdMediaResource1"
                                                                                                            Width="900px" OnNeedDataSource="gdMedia_NeedDataSource" OnDeleteCommand="gdMedia_DeleteCommand"
                                                                                                            OnItemDataBound="gdMedia_ItemDataBound" OnItemCommand="gdMedia_ItemCommand" AllowFilteringByColumn="true"
                                                                                                            FilterItemStyle-HorizontalAlign="Left">
                                                                                                            <GroupingSettings CaseSensitive="false" /> 
                                                                                                            <MasterTableView DataKeyNames="FuelTypeID" ClientDataKeyNames="FuelTypeID" CommandItemDisplay="Top">
                                                                                                                <Columns>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Fuel Type" UniqueName="FuelType" SortExpression="FuelType"
                                                                                                                        meta:resourcekey="GridTemplateColumnFuelTypeResource1" AllowFiltering="true"
                                                                                                                        DataField="FuelType">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblFuelType" CssClass="formtext" runat="server" Text='<%# Bind("FuelType") %>'
                                                                                                                                meta:resourcekey="lblFuelTypeResource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Left" Width="150px" />
                                                                                                                        <HeaderStyle  HorizontalAlign="Left" Font-Bold="true"/>
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="GHG Category" UniqueName="GHGCategory" SortExpression="GHGCategory"
                                                                                                                        meta:resourcekey="GridTemplateColumnGHGCategoryResource1" AllowFiltering="true" 
                                                                                                                        DataField="GHGCategory">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblGHGCategory" CssClass="formtext" runat="server" Text='<%# Bind("GHGCategory") %>'
                                                                                                                                meta:resourcekey="lblGHGCategoryResource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Left" Width="150px" />
                                                                                                                        <HeaderStyle  HorizontalAlign="Left" Font-Bold="true"/>
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="GHG Category Desc" UniqueName="GHGCategoryDesc" meta:resourcekey="GridTemplateColumnGHGCategoryDescResource1"
                                                                                                                        AllowFiltering="true" DataField="GHGCategoryDesc">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblGHGCategoryDesc" CssClass="formtext" runat="server"
                                                                                                                                Text='<%# Bind("GHGCategoryDesc") %>' meta:resourcekey="lblGHGCategoryDescResource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle  HorizontalAlign="Left" Width="300px" />
                                                                                                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="CO2 Factor" UniqueName="CO2Factor" meta:resourcekey="GridTemplateColumnCO2FactorResource1" DataType="System.Double"
                                                                                                                        DataField="CO2Factor" >
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblCO2Factor" CssClass="formtext" runat="server"
                                                                                                                                Text='<%# Bind("CO2Factor") %>' meta:resourcekey="lblCO2FactorResource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle  HorizontalAlign="Left" Width="100px" />
                                                                                                                        <HeaderStyle  HorizontalAlign="Left" Font-Bold="true"/>
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    
                                                                                                                    <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn"
                                                                                                                        meta:resourcekey="GridEditCommandColumnResource1" EditImageUrl="../../images/edit.gif"
                                                                                                                        >
                                                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridEditCommandColumn>
                                                                                                                    <telerik:GridButtonColumn ConfirmText="Delete this fuel category?"  ConfirmDialogType="Classic"
                                                                                                                        ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                                                                        UniqueName="DeleteColumn" meta:resourcekey="GridButtonColumnDeleteResource1"
                                                                                                                        ImageUrl="../../images/delete.gif">
                                                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridButtonColumn>
                                                                                                                </Columns>
                                                                                                                <EditFormSettings ColumnNumber="2" UserControlName="EditFuelCategory.ascx" EditFormType="WebUserControl">
                                                                                                                    <FormTableItemStyle HorizontalAlign="Left"></FormTableItemStyle>
                                                                                                                    <FormTableStyle CellSpacing="0" CellPadding="2" HorizontalAlign="Left" />
                                                                                                                    <FormTableAlternatingItemStyle Wrap="False" HorizontalAlign="Left"></FormTableAlternatingItemStyle>
                                                                                                                    <EditColumn ButtonType="PushButton" InsertText="Save" UpdateText="Save">
                                                                                                                    </EditColumn>
                                                                                                                    <FormMainTableStyle />
                                                                                                                </EditFormSettings>
                                                                                                                <HeaderStyle CssClass="RadGridtblHeader" />
                                                                                                                <CommandItemTemplate>
                                                                                                                    <table width="100%">
                                                                                                                        <tr>
                                                                                                                            <td align="left">
                                                                                                                                <asp:Button ID="btnAdd" Text="Add New Fuel Category" CommandName="InitInsert" runat="server" CssClass="combutton" Width ="150px"
                                                                                                                                    meta:resourcekey="btnAddResource1" />
                                                                                                                            </td>
                                                                                                                            <td align="right">
                                                                                                                                <nobr>
                                                                                                                                <asp:ImageButton ID="imgFilter" runat="server" OnClientClick ="javascript:return showFilterItem();" ImageUrl="~/images/filter.gif" />
                                                                                                                                <asp:LinkButton ID="hplFilter" runat="server"  OnClientClick ="javascript:return showFilterItem();" Text="Show Filter" meta:resourcekey="hplFilterResource1" Font-Underline="true"></asp:LinkButton>
                                                                                                                                </nobr>
                                                                                                                            </td>
                                                                                                                        </tr>
                                                                                                                    </table>
                                                                                                                </CommandItemTemplate>
                                                                                                            </MasterTableView>
                                                                                                            <ValidationSettings CommandsToValidate="PerformInsert,Update" ValidationGroup="valGPAdd" />
                                                                                                            <ClientSettings>
                                                                                                                <ClientEvents OnGridCreated="GridCreated" />
                                                                                                            </ClientSettings>
                                                                                                        </telerik:RadGrid>
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
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hidFilter" runat="server" />
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdMedia.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=gdMedia.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdMedia.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
            }

        </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>
