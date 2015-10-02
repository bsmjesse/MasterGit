<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmEquipment.aspx.cs" Inherits="SentinelFM.Configuration_frmequipment"
    meta:resourcekey="PageResource1" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="ctlEquipmentMenu.ascx" TagName="ctlEquipmentMenu" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
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
            <telerik:AjaxSetting AjaxControlID="gdEquipment">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdEquipment" LoadingPanelID="LoadingPanel1" />
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
                                                                    <uc2:ctlEquipmentMenu ID="ctlEquipmentMenu1" runat="server" selectedcontrol="cmpEquipment" />
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
                                                                                                        <telerik:RadGrid AutoGenerateColumns="False" ID="gdEquipment" PageSize="25" AllowAutomaticDeletes="false"
                                                                                                            AllowAutomaticInserts="false" AllowSorting="true" AllowAutomaticUpdates="false"
                                                                                                            AllowPaging="True" runat="server" Skin="Hay" GridLines="None" meta:resourcekey="gdEpuipmentResource1"
                                                                                                            Width="700px" OnInsertCommand="gdEquipment_OnInsertCommand" OnNeedDataSource="gdEquipment_NeedDataSource"
                                                                                                            OnUpdateCommand="gdEquipment_UpdateCommand" OnDeleteCommand="gdEquipment_DeleteCommand"
                                                                                                            OnItemDataBound="gdEquipment_ItemDataBound" OnItemCommand="gdEquipment_ItemCommand"
                                                                                                            AllowFilteringByColumn="true" FilterItemStyle-HorizontalAlign="Left">
                                                                                                            <GroupingSettings CaseSensitive="false" />   
                                                                                                            <MasterTableView DataKeyNames="EquipmentId" ClientDataKeyNames="EquipmentId" CommandItemDisplay="Top"
                                                                                                                ItemStyle-Width="300px">
                                                                                                                <Columns>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Description" UniqueName="Description" SortExpression="Description"
                                                                                                                        meta:resourcekey="GridTemplateColumnDescriptionResource1" AllowFiltering="true"
                                                                                                                        DataField="Description">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblDescription" CssClass="formtext" runat="server" Text='<%# Bind("Description") %>'
                                                                                                                                meta:resourcekey="lblDescriptionResource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <EditItemTemplate>
                                                                                                                            <asp:TextBox ID="txtDescription" runat="server" MaxLength="50" Width="300px" meta:resourcekey="txtDescriptionResource1"
                                                                                                                                Text='<%# Bind("Description") %>'></asp:TextBox>
                                                                                                                            <span style="color: Red">*</span><br />
                                                                                                                            <asp:RequiredFieldValidator ID="valReqtxtDescription" runat="server" ValidationGroup="valGPAdd"
                                                                                                                                ControlToValidate="txtDescription" meta:resourcekey="valReqtxtDescriptionResource1"
                                                                                                                                Text="Please enter description" Display="Dynamic"></asp:RequiredFieldValidator>
                                                                                                                        </EditItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Left" Width="350px" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Equipment Type" UniqueName="EquipmentType"
                                                                                                                        SortExpression="TypeName" meta:resourcekey="GridTemplateColumnTypeNameResource1"
                                                                                                                        AllowFiltering="true" DataField="TypeName">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Label ID="lblTypeName" CssClass="formtext" runat="server" Text='<%# Bind("TypeName") %>'
                                                                                                                                meta:resourcekey="lblTypeNameResource1">
                                                                                                                            </asp:Label>
                                                                                                                        </ItemTemplate>
                                                                                                                        <EditItemTemplate>
                                                                                                                            <telerik:RadComboBox ID="combTypeName" runat="server" DataTextField="TypeName" DataValueField="EquipmentTypeId"
                                                                                                                                Width="200px" meta:resourcekey="combTypeNameResource1">
                                                                                                                            </telerik:RadComboBox>
                                                                                                                        </EditItemTemplate>
                                                                                                                        <ItemStyle HorizontalAlign="Left" Width="200px" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn"
                                                                                                                        meta:resourcekey="GridEditCommandColumnResource1" EditImageUrl="../../images/edit.gif">
                                                                                                                    </telerik:GridEditCommandColumn>
                                                                                                                    <telerik:GridButtonColumn ConfirmText="Delete this equipment?" ConfirmDialogType="Classic"
                                                                                                                        ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                                                                        UniqueName="DeleteColumn" meta:resourcekey="GridButtonColumnDeleteResource1"
                                                                                                                        ImageUrl="../../images/delete.gif">
                                                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                                                    </telerik:GridButtonColumn>
                                                                                                                </Columns>
                                                                                                                <EditFormSettings ColumnNumber="2">
                                                                                                                    <FormTableItemStyle HorizontalAlign="Left"></FormTableItemStyle>
                                                                                                                    <FormTableStyle CellSpacing="0" CellPadding="2" Font-Bold="true" HorizontalAlign="Left" />
                                                                                                                    <FormTableAlternatingItemStyle Wrap="False" HorizontalAlign="Left"></FormTableAlternatingItemStyle>
                                                                                                                    <EditColumn ButtonType="PushButton" InsertText="Save" UpdateText="Save"  >
                                                                                                                    </EditColumn>
                                                                                                                    <FormMainTableStyle />
                                                                                                                </EditFormSettings>
                                                                                                                <HeaderStyle CssClass="RadGridtblHeader" />
                                                                                                                <CommandItemTemplate>
                                                                                                                    <table width="100%">
                                                                                                                        <tr>
                                                                                                                            <td align="left">
                                                                                                                                <asp:Button ID="btnAdd" Text="Add New Equipment" CommandName="InitInsert" runat="server" CssClass="combutton" Width= "130px"
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
                    $find('<%=gdEquipment.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=gdEquipment.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdEquipment.ClientID %>').get_masterTableView().hideFilterItem();
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
