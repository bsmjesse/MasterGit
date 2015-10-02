<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMedia.aspx.cs" Inherits="SentinelFM.Configuration_Equipment_Media" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="ctlEquipmentMenu.ascx" TagName="ctlEquipmentMenu" TagPrefix="uc2" %>
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
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" selectedcontrol="cmdEquipment" />
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
                                                                    <uc2:ctlEquipmentMenu ID="ctlEquipmentMenu1" runat="server" selectedcontrol="cmpMedia" />
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
                                                                                                            <MasterTableView DataKeyNames="MediaId" ClientDataKeyNames="MediaId" CommandItemDisplay="Top">
                                                                                                                <Columns>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Description" UniqueName="Description" SortExpression="Description"
                                                                                                                        meta:resourcekey="GridTemplateColumnDescriptionResource1" AllowFiltering="true"
                                                                                                                        DataField="Description">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblDescription" CssClass="formtext" runat="server" Text='<%# Bind("Description") %>'
                                                                                                                                meta:resourcekey="lblDescriptionResource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Left" Width="210px" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Media Type" UniqueName="MediaType" SortExpression="TypeName"
                                                                                                                        meta:resourcekey="GridTemplateColumnTypeNameResource1" AllowFiltering="true"
                                                                                                                        DataField="TypeName">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblTypeName" CssClass="formtext" runat="server" Text='<%# Bind("TypeName") %>'
                                                                                                                                meta:resourcekey="lblTypeNameResource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Left" Width="120px" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Factor1" UniqueName="Factor1" meta:resourcekey="GridTemplateColumnFactor1Resource1"
                                                                                                                        AllowFiltering="false">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblFactorName1" CssClass="formtext  GridColumnTitle" runat="server"
                                                                                                                                Text='<%# Bind("FactorName1") %>' meta:resourcekey="lblFactorName1Resource1">
                                                                                                                            </asp:Label>
                                                                                                                            <br />
                                                                                                                            <asp:Label ID="lblFactor1" CssClass="formtext" runat="server" 
                                                                                                                                meta:resourcekey="lblFactor1Resource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Factor2" UniqueName="Factor2" meta:resourcekey="GridTemplateColumnFactor2Resource1"
                                                                                                                        AllowFiltering="false">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblFactorName2" CssClass="formtext GridColumnTitle" runat="server"
                                                                                                                                Text='<%# Bind("FactorName2") %>' meta:resourcekey="lblFactorName2Resource1">
                                                                                                                            </asp:Label>
                                                                                                                            <br />
                                                                                                                            <asp:Label ID="lblFactor2" CssClass="formtext" runat="server" 
                                                                                                                                meta:resourcekey="lblFactor2Resource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Factor3" UniqueName="Factor3" meta:resourcekey="GridTemplateColumnFactor3Resource1"
                                                                                                                        AllowFiltering="false">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblFactorName3" CssClass="formtext GridColumnTitle" runat="server"
                                                                                                                                Text='<%# Bind("FactorName3") %>' meta:resourcekey="lblFactorName3Resource1">
                                                                                                                            </asp:Label>
                                                                                                                            <br />
                                                                                                                            <asp:Label ID="lblFactor3" CssClass="formtext" runat="server" 
                                                                                                                                meta:resourcekey="lblFactor3Resource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Factor4" UniqueName="Factor4" meta:resourcekey="GridTemplateColumnFactor4Resource1"
                                                                                                                        AllowFiltering="false">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblFactorName4" CssClass="formtext GridColumnTitle" runat="server"
                                                                                                                                Text='<%# Bind("FactorName4") %>' meta:resourcekey="lblFactorName4Resource1">
                                                                                                                            </asp:Label>
                                                                                                                            <br />
                                                                                                                            <asp:Label ID="lblFactor4" CssClass="formtext" runat="server" 
                                                                                                                                meta:resourcekey="lblFactor4Resource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Factor5" UniqueName="Factor5" meta:resourcekey="GridTemplateColumnFactor5Resource1"
                                                                                                                        AllowFiltering="false">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblFactorName5" CssClass="formtext GridColumnTitle" runat="server"
                                                                                                                                Text='<%# Bind("FactorName5") %>' meta:resourcekey="lblFactorName5Resource1">
                                                                                                                            </asp:Label>
                                                                                                                            <br />
                                                                                                                            <asp:Label ID="lblFactor5" CssClass="formtext" runat="server" 
                                                                                                                                meta:resourcekey="lblFactor5Resource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                                                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn"
                                                                                                                        meta:resourcekey="GridEditCommandColumnResource1" EditImageUrl="../../images/edit.gif">
                                                                                                                    </telerik:GridEditCommandColumn>
                                                                                                                    <telerik:GridButtonColumn ConfirmText="Delete this media?" ConfirmDialogType="Classic"
                                                                                                                        ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                                                                        UniqueName="DeleteColumn" meta:resourcekey="GridButtonColumnDeleteResource1"
                                                                                                                        ImageUrl="../../images/delete.gif">
                                                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridButtonColumn>
                                                                                                                </Columns>
                                                                                                                <EditFormSettings ColumnNumber="2" UserControlName="EditFactors.ascx" EditFormType="WebUserControl">
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
                                                                                                                                <asp:Button ID="btnAdd" Text="Add New Media" CommandName="InitInsert" runat="server" CssClass="combutton"
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
