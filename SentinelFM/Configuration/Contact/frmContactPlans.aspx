<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmContactPlans.aspx.cs"
    Inherits="SentinelFM.Configuration_Contact_frmContactPlans" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="ctlContactMenu.ascx" TagName="ctlContactMenu" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboContactPlans">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnDeletePlan">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="gdPhones">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdPhones" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="gdPhones">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdPhones" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="text-align: left; width: 900px">
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" selectedcontrol="btnPanicManagerment" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td>
                                <table id="tblForm" border="0" cellpadding="0" cellspacing="0" class="table" style="height: 500px;
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
                                                                    <uc2:ctlContactMenu ID="ctlContactMenu" runat="server" selectedcontrol="cmdPlans" />
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760"
                                                                        style="padding-bottom: 5px">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;"
                                                                                    class="tableDoubleBorder">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 950px;
                                                                                                margin-top: 2px; height: 480px">
                                                                                                <tr valign="top">
                                                                                                    <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                                                                        <asp:Panel ID="pnl" runat="server">
                                                                                                            <table>
                                                                                                                <tr>
                                                                                                                    <td align="center">
                                                                                                                        <nobr>
                                                                                                                    <telerik:RadComboBox ID="cboContactPlans" runat="server" CssClass="RegularText" Width="258px"
                                                                                                                        DataTextField="ContactPlanName" DataValueField="ContactPlanId " meta:resourcekey="cboContactPlansResource1"
                                                                                                                        Skin="Hay"  AutoPostBack="true"
                                                                                                                        MaxHeight="200px" AppendDataBoundItems="true" 
                                                                                                                        onselectedindexchanged="cboContactPlans_SelectedIndexChanged">
                                                                                                                        <Items>
                                                                                                                           <telerik:RadComboBoxItem Text= "Select a Plan" Value="-1" meta:resourcekey="cboContactPlansItem0Resource1" />
                                                                                                                        </Items>
                                                                                                                    </telerik:RadComboBox>
                                                                                                                    &nbsp
                                                                                                                    <asp:Button ID="btnDeletePlan" runat="server" Text="Delete Plan"  
                                                                                                                        width="120px"  meta:resourcekey="btnDeletePlanResource1"   Visible = "false" 
                                                                                                                        onclick="btnDeletePlan_Click" 
                                                                                                                        OnClientClick="javascript:if (!ConfirmDeletePlan()) return false" CssClass="combutton"/>
                                                                                                                    &nbsp
                                                                                                                    <asp:Button ID="btnAddNew" runat="server" Text="Add New Plan"  width="120px"  
                                                                                                                        meta:resourcekey="btnAddNewResource1" 
                                                                                                                        OnClientClick="javascript:return ShowInsertForm()"  CssClass="combutton"/>
                                                                                                                    </nobr>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <asp:Panel ID="pnlPhones" runat="server" Visible="false">
                                                                                                                            <fieldset>
                                                                                                                                <legend style="color: green" runat="server" id="legendPhones"></legend>
                                                                                                                                <telerik:RadGrid ID="gdPhones" runat="server" AutoGenerateColumns="False" GridLines="Both"
                                                                                                                                    Width="700px" AllowSorting="false" ShowHeader="true" AllowPaging="false" BorderStyle="None"
                                                                                                                                    meta:resourcekey="gdPhonesResource1" OnDeleteCommand="gdPhones_DeleteCommand"
                                                                                                                                    OnItemDataBound="gdPhones_ItemDataBound" 
                                                                                                                                    onitemcommand="gdPhones_ItemCommand">
                                                                                                                                    <MasterTableView BorderStyle="None" DataKeyNames="ContactCommunicationID" ClientDataKeyNames="ContactCommunicationID"
                                                                                                                                        CommandItemDisplay="Top">
                                                                                                                                        <Columns>
                                                                                                                                            <telerik:GridTemplateColumn UniqueName="upDownArrow" Visible="false">
                                                                                                                                                <ItemTemplate>
                                                                                                                                                    <table width="20px" cellpadding="0" cellspacing="0">
                                                                                                                                                        <tr>
                                                                                                                                                            <td>
                                                                                                                                                                <nobr>
                                                                                                                                                                <asp:ImageButton ID="btnUpArrow" runat="server" ImageUrl="~/images/UpArrow.gif" Width="10px" CommandName="UpPriority" visible = "false"
                                                                                                                                                                    Height="15px" />
                                                                                                                                                                <asp:ImageButton ID="btnDownArrow" runat="server" ImageUrl="~/images/DownArrow.gif" CommandName="DownPriority" visible = "false"
                                                                                                                                                                    Height="15px" Width="10px"  />
                                                                                                                                                                </nobr>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </table>
                                                                                                                                                </ItemTemplate>
                                                                                                                                                <ItemStyle Width="25px" HorizontalAlign="Center" />
                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                            <telerik:GridTemplateColumn>
                                                                                                                                                <ItemTemplate>
                                                                                                                                                    <nobr>
                                                                                                                                               <asp:Label runat="server" CssClass="RegularText" ID="lblPriority" Text="Priority"  ForeColor="Blue" meta:resourcekey="lblPriorityResource1" > </asp:Label>
                                                                                                                                               </nobr>
                                                                                                                                                </ItemTemplate>
                                                                                                                                                <ItemStyle Width="50px" />
                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                            <telerik:GridBoundColumn DataField="Name" HeaderText="Name" meta:resourcekey="gdPhonesNameResource1">
                                                                                                                                                <ItemStyle Width="250" />
                                                                                                                                            </telerik:GridBoundColumn>
                                                                                                                                            <telerik:GridBoundColumn DataField="CommunicationData" HeaderText="Telephone" meta:resourcekey="gdPhonesTelResource1">
                                                                                                                                                <ItemStyle Width="320" />
                                                                                                                                            </telerik:GridBoundColumn>
                                                                                                                                            <telerik:GridTemplateColumn Visible="false">
                                                                                                                                                <ItemTemplate>
                                                                                                                                                    <asp:Button ID="btnDetail" runat="server" Text="Detail" CommandName="Detail" meta:resourcekey="gdPhonesDetailResource1" />
                                                                                                                                                </ItemTemplate>
                                                                                                                                                <ItemStyle HorizontalAlign="Center" Width="60px" />
                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                            <telerik:GridButtonColumn ConfirmText="Delete this telephone?" ConfirmDialogType="Classic"
                                                                                                                                                ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                                                                                                UniqueName="DeleteColumn" meta:resourcekey="gdPhonesDelResource1" ImageUrl="../../images/delete.gif">
                                                                                                                                                <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                                                                                                            </telerik:GridButtonColumn>
                                                                                                                                        </Columns>
                                                                                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                                                                                        <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                                                                        <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader" ForeColor="White" />
                                                                                                                                        <CommandItemTemplate>
                                                                                                                                            <table width="100%">
                                                                                                                                                <tr>
                                                                                                                                                    <td align="left">
                                                                                                                                                        <telerik:RadComboBox ID="cboTels" runat="server" Height="200px" Width="420px" DropDownWidth="550px"
                                                                                                                                                            HighlightTemplatedItems="true" Filter="StartsWith" OnClientDropDownOpening="onDropDownOpening">
                                                                                                                                                            <HeaderTemplate>
                                                                                                                                                                <table cellspacing="0" cellpadding="0">
                                                                                                                                                                    <tr>
                                                                                                                                                                        <td style="width: 15px;">
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="width: 230px;" class="RegularText">
                                                                                                                                                                            Name
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="width: 270px;" class="RegularText">
                                                                                                                                                                            Telephone
                                                                                                                                                                        </td>
                                                                                                                                                                    </tr>
                                                                                                                                                                </table>
                                                                                                                                                            </HeaderTemplate>
                                                                                                                                                            <ItemTemplate>
                                                                                                                                                                <table cellspacing="0" cellpadding="0">
                                                                                                                                                                    <tr>
                                                                                                                                                                        <td style="width: 15px;">
                                                                                                                                                                            <asp:CheckBox ID="chkCommunicationData" runat="server" />
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="width: 230px;" class="RegularText">
                                                                                                                                                                            <%# DataBinder.Eval(Container, "Attributes['Name']")%>
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="width: 270px;">
                                                                                                                                                                            <asp:Label runat="server" CssClass="RegularText" ID="lblCommunicationData" Text='<%# DataBinder.Eval(Container, "Attributes[\"CommunicationData\"]")%>'></asp:Label>
                                                                                                                                                                        </td>
                                                                                                                                                                    </tr>
                                                                                                                                                                </table>
                                                                                                                                                            </ItemTemplate>
                                                                                                                                                        </telerik:RadComboBox>
                                                                                                                                                        <asp:Button ID="btnAddTele" runat="server" Width="120px" Text="Add Telephone" meta:resourcekey="btnAddTeleResource1" CssClass="combutton"
                                                                                                                                                            OnClick="btnAddTele_Click"></asp:Button>
                                                                                                                                                    </td>
                                                                                                                                                </tr>
                                                                                                                                            </table>
                                                                                                                                        </CommandItemTemplate>
                                                                                                                                    </MasterTableView>
                                                                                                                                </telerik:RadGrid>
                                                                                                                            </fieldset>
                                                                                                                        </asp:Panel>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <asp:Panel ID="pnlEmails" runat="server" Visible="false">
                                                                                                                            <fieldset>
                                                                                                                                <legend style="color: green" runat="server" id="legendEmails"></legend>
                                                                                                                                <telerik:RadGrid ID="gdEmails" runat="server" AutoGenerateColumns="False" GridLines="Both"
                                                                                                                                    Width="700px" AllowSorting="false" ShowHeader="true" AllowPaging="false" BorderStyle="None"
                                                                                                                                    meta:resourcekey="gdEmailsResource1" OnDeleteCommand="gdEmails_DeleteCommand"
                                                                                                                                    OnItemDataBound="gdEmails_ItemDataBound">
                                                                                                                                    <MasterTableView BorderStyle="None" DataKeyNames="ContactCommunicationID" ClientDataKeyNames="ContactCommunicationID"
                                                                                                                                        CommandItemDisplay="Top">
                                                                                                                                        <Columns>
                                                                                                                                            <telerik:GridBoundColumn DataField="Name" HeaderText="Name" meta:resourcekey="gdEmailsNameResource1">
                                                                                                                                                <ItemStyle Width="250" />
                                                                                                                                            </telerik:GridBoundColumn>
                                                                                                                                            <telerik:GridBoundColumn DataField="CommunicationData" HeaderText="Email" meta:resourcekey="gdEmailsEmailResource1">
                                                                                                                                                <ItemStyle Width="320" />
                                                                                                                                            </telerik:GridBoundColumn>
                                                                                                                                            <telerik:GridTemplateColumn Visible="false">
                                                                                                                                                <ItemTemplate>
                                                                                                                                                    <asp:Button ID="btnDetail" runat="server" Text="Detail" CommandName="Detail" meta:resourcekey="gdEmailsDetailResource1" />
                                                                                                                                                </ItemTemplate>
                                                                                                                                                <ItemStyle HorizontalAlign="Center" Width="60px" />
                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                            <telerik:GridButtonColumn ConfirmText="Delete this email?" ConfirmDialogType="Classic"
                                                                                                                                                ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                                                                                                UniqueName="DeleteColumn" meta:resourcekey="gdEmailsDelResource1" ImageUrl="../../images/delete.gif">
                                                                                                                                                <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                                                                                                            </telerik:GridButtonColumn>
                                                                                                                                        </Columns>
                                                                                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                                                                                        <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                                                                        <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader" ForeColor="White" />
                                                                                                                                        <CommandItemTemplate>
                                                                                                                                            <table width="100%">
                                                                                                                                                <tr>
                                                                                                                                                    <td align="left">
                                                                                                                                                        <telerik:RadComboBox ID="cboEmails" runat="server" Height="200px" Width="420px" DropDownWidth="550px"
                                                                                                                                                            EmptyMessage="Select a Email" HighlightTemplatedItems="true" Filter="StartsWith"
                                                                                                                                                            OnClientDropDownOpening="onDropDownOpening">
                                                                                                                                                            <HeaderTemplate>
                                                                                                                                                                <table cellspacing="0" cellpadding="0">
                                                                                                                                                                    <tr>
                                                                                                                                                                        <td style="width: 15px;">
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="width: 230px;" class="RegularText">
                                                                                                                                                                            Name
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="width: 270px;" class="RegularText">
                                                                                                                                                                            Email
                                                                                                                                                                        </td>
                                                                                                                                                                    </tr>
                                                                                                                                                                </table>
                                                                                                                                                            </HeaderTemplate>
                                                                                                                                                            <ItemTemplate>
                                                                                                                                                                <table cellspacing="0" cellpadding="0">
                                                                                                                                                                    <tr>
                                                                                                                                                                        <td style="width: 15px;">
                                                                                                                                                                            <asp:CheckBox ID="chkCommunicationData" runat="server" />
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="width: 230px;" class="RegularText">
                                                                                                                                                                            <%# DataBinder.Eval(Container, "Attributes['Name']")%>
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="width: 270px;" class="RegularText">
                                                                                                                                                                            <asp:Label runat="server" ID="lblCommunicationData" Text='<%# DataBinder.Eval(Container, "Attributes[\"CommunicationData\"]")%>'></asp:Label>
                                                                                                                                                                        </td>
                                                                                                                                                                    </tr>
                                                                                                                                                                </table>
                                                                                                                                                            </ItemTemplate>
                                                                                                                                                        </telerik:RadComboBox>
                                                                                                                                                        <asp:Button ID="btnAddEmail" Width="120px" runat="server" Text="Add Email" meta:resourcekey="btnAddEmailResource1"
                                                                                                                                                            OnClick="btnAddEmail_Click" CssClass="combutton"></asp:Button>
                                                                                                                                                    </td>
                                                                                                                                                </tr>
                                                                                                                                            </table>
                                                                                                                                        </CommandItemTemplate>
                                                                                                                                    </MasterTableView>
                                                                                                                                </telerik:RadGrid>
                                                                                                                            </fieldset>
                                                                                                                        </asp:Panel>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </asp:Panel>
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
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function ShowInsertForm() {
                window.radopen("frmEditContactPlan.aspx", "AddNewPlan");
                return false;
            }
            function onDropDownOpening(sender) {
                var items = sender.get_items();
                if (items.get_count() > 0) {
                    if (items.getItem(0).get_value() == "-1") {
                        $telerik.$(items.getItem(0).get_element()).find("[id$='chkCommunicationData']").css("visibility", "hidden");
                    }
                }
            }

            function ConfirmDeletePlan() {
                if (confirm('<%= confirmDeletePlan %>')) return true;
                else return false;
            }
            function RebindPlans() {
                $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("RebindPlans");
            }

        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
        EnableAriaSupport="true">
        <Windows>
            <telerik:RadWindow ID="AddNewPlan" runat="server" Title="Add New Plan" Height="530px"
                Width="800px" ReloadOnShow="true" ShowContentDuringLoad="false" Skin="Hay" Modal="true"
                meta:resourcekey="UserListDialogResource1" VisibleStatusbar="false" VisibleTitlebar="false" />
        </Windows>
    </telerik:RadWindowManager>
    </form>
</body>
</html>
