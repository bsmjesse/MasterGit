<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerReportEmail.aspx.cs"
    Inherits="SentinelFM.Configuration_CustomerReport_CustomerReportEmail" Theme="TelerikControl" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="CustomerReportMenu.ascx" TagName="ctlEquipmentMenu" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" >
        .WrapTextClass
        {
            word-wrap:break-word;
            width:280px !important;
            display:block !important;
        }        
    </style>
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
        Style="text-decoration: underline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="dgEmail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgEmail" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="text-align: left; width: 900px">
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" selectedcontrol="btnCustomReport" />
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
                                                                    <uc2:ctlEquipmentMenu ID="ctlEquipmentMenu1" runat="server" selectedcontrol="cmpEmail" />
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
                                                                                                        <telerik:RadGrid ID="dgEmail" Width="900px" runat="server" AutoGenerateColumns="False"
                                                                                                            AllowSorting="True" AllowPaging="True" PageSize="15" GridLines="None" ShowGroupPanel="false"
                                                                                                            AllowFilteringByColumn="true" FilterItemStyle-HorizontalAlign="Left" 
                                                                                                            Skin="Simple" onitemdatabound="dgEmail_ItemDataBound" 
                                                                                                            onneeddatasource="dgEmail_NeedDataSource" 
                                                                                                            onitemcommand="dgEmail_ItemCommand">
                                                                                                            <GroupingSettings CaseSensitive="false" />
                                                                                                            <MasterTableView CommandItemDisplay="Top" ClientDataKeyNames="CustomReportEmailId"
                                                                                                                DataKeyNames="CustomReportEmailId">
                                                                                                                <PagerStyle Mode="NextPrevAndNumeric" />
                                                                                                                <Columns>
                                                                                                                    <telerik:GridTemplateColumn HeaderText="Email" UniqueName="Email" SortExpression="Email" Visible="false" >
                                                                                                                       <ItemTemplate>
                                                                                                                             <span class="WrapTextClass" >
                                                                                                                             <%# Eval("Email") %>
                                                                                                                             </span>
                                                                                                                       </ItemTemplate>
                                                                                                                       <ItemStyle Width="300px" CssClass="WrapTextClass"  />
                                                                                                                        <HeaderStyle Width="300px" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridBoundColumn HeaderText="Email" DataField="Email" UniqueName="EmailIE" Visible="true">
                                                                                                                        <ItemStyle Width="300px" CssClass="WrapTextClass" Wrap="true" />
                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                    <telerik:GridTemplateColumn AllowFiltering="false">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Button CommandName="Edit" ID="btnModifyEmail" runat="server" Text="Modify" Width="75px"
                                                                                                                                CssClass="confbutton" />
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle Width="80px" />
                                                                                                                        <HeaderStyle Width="80px" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn AllowFiltering="false">
                                                                                                                        <ItemTemplate>
                                                                                                                            <asp:Button CommandName="DeleteEmail" ID="btnDeleteEmail" runat="server" Text="Delete" Width="75px" OnClientClick="javascript:if (!checkIsDeleted()) return false;"
                                                                                                                                CssClass="confbutton" />
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle Width="80px" />
                                                                                                                        <HeaderStyle Width="80px" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    <telerik:GridTemplateColumn UniqueName="Fleet" AllowFiltering="false">
                                                                                                                        <ItemTemplate>
                                                                                                                            <telerik:RadGrid ID="dgEmailFleet" runat="server" AutoGenerateColumns="False" GridLines="None"
                                                                                                                                Skin="Simple" ShowHeader="false" Width="290px">
                                                                                                                                <MasterTableView GroupLoadMode="Server" ClientDataKeyNames="CustomReportEmailId, FleetId"
                                                                                                                DataKeyNames="CustomReportEmailId, FleetId">
                                                                                                                                    <Columns>
                                                                                                                                        <telerik:GridBoundColumn HeaderText="FleetName" UniqueName="FleetName" SortExpression="FleetName" DataField="FleetName"
                                                                                                                                            meta:resourcekey="dgEmailFleetNameDescriptionResource1" AllowFiltering="true"
                       >
                                                                                                                                            <ItemStyle Width="250px" />
                                                                                                                                            <HeaderStyle Width="250px" />
                                                                                                                                        </telerik:GridBoundColumn>
                                                                                                                                        <telerik:GridTemplateColumn Visible="false">
                                                                                                                                            <ItemTemplate>
                                                                                                                                                <asp:Button CommandName="DeleteFleet" ID="btnDeleteFleet" runat="server" Text="Delete Fleet"
                                                                                                                                                    CssClass="confbutton" />
                                                                                                                                            </ItemTemplate>
                                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                                    </Columns>
                                                                                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                                                                                    <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                                                                    <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />

                                                                                                                                </MasterTableView>
                                                                                                                                <ClientSettings EnableRowHoverStyle="false">
                                                                                                                                </ClientSettings>
                                                                                                                            </telerik:RadGrid>
                                                                                                                        </ItemTemplate>
                                                                                                                        <ItemStyle Width="300px" CssClass="SubRadGridItem" VerticalAlign="Top" />
                                                                                                                        <HeaderStyle Width="300px" />
                                                                                                                    </telerik:GridTemplateColumn>
                                                                                                                    
                                                                                                                </Columns>
                                                                                                                <ItemStyle VerticalAlign="Top" />
                                                                                                                <AlternatingItemStyle VerticalAlign="Top" />
                                                                                                                <CommandItemTemplate>
                                                                                                                    <table id="tblCustomerCommand" runat="server" width="100%">
                                                                                                                        <tr>
                                                                                                                            <td align="left">
                                                                                                                                <asp:Button ID="btnAddEmail" runat="server" Text="Add New Email" CssClass="combutton" CommandName="InitInsert" />
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
                                                                                                                                    <EditFormSettings ColumnNumber="2" UserControlName="AddEmail.ascx" EditFormType="WebUserControl">
                                                                                                                                        <FormTableItemStyle HorizontalAlign="Left" VerticalAlign="Top"></FormTableItemStyle>
                                                                                                                                        <FormTableStyle CellSpacing="0" CellPadding="2" Font-Bold="true" HorizontalAlign="Left" />
                                                                                                                                        <FormTableAlternatingItemStyle Wrap="False" HorizontalAlign="Left" VerticalAlign="Top">
                                                                                                                                        </FormTableAlternatingItemStyle>
                                                                                                                                        <EditColumn ButtonType="PushButton" InsertText="Save" UpdateText="Save">
                                                                                                                                        </EditColumn>
                                                                                                                                        <FormMainTableStyle />
                                                                                                                                    </EditFormSettings>                                                                                                                
                                                                                                            </MasterTableView>
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
            function checkIsDeleted() {
                if (confirm('<%= deleteEmailText %>'))
                    return true;
                else return false;
            }
            function CustomValidateEmail(sender, args) {
                args.IsValid = true;
                var hasEmail = false;
                var inputText = $telerik.$.trim(args.Value);
                var emailArray = inputText.split(';')
                for (var index = 0; index < emailArray.length; index++) {
                    if ($telerik.$.trim(emailArray[index]) != '') {
                        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
                        if (reg.test(emailArray[index]) == false) {
                            args.IsValid = false;
                            return;
                        }
                        else hasEmail = true;
                    }
                }

                if (!hasEmail) args.IsValid = false;
            }

            function CustomValidateFleet(sender, args) {
            args.IsValid = false;
                if ($telerik.$.find("input:checkbox[id$='chkFleet']:checked").length > 0) {
                    args.IsValid = true;
                }
            }


            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgEmail.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=dgEmail.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgEmail.ClientID %>').get_masterTableView().hideFilterItem();
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
