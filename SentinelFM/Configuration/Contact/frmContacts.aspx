<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmContacts.aspx.cs" Inherits="SentinelFM.Configuration_Contact_frmContacts" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="ctlContactMenu.ascx" TagName="ctlContactMenu" TagPrefix="uc2" %>
<%@ Register Src="EditContact.ascx" TagName="EditContact" TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .communicationsTable
        {
            padding: 1px;
        }
        .emergencyStyle
        {
           font-weight:bold;
           color:Red;
        }
    </style>
</head>
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"  OnAjaxRequest="RadAjaxManager1_AjaxRequest" >
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="gdVehicleContact">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdVehicleContact" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdVehicleContact" LoadingPanelID="LoadingPanel1" />
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
                                                                    <uc2:ctlContactMenu ID="ctlContactMenu" runat="server" selectedcontrol="cmdContacts" />
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760"
                                                                        style="padding-bottom: 5px">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;"
                                                                                    class="tableDoubleBorder">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 950px;
                                                                                                margin-top: 2px; height: 495px">
                                                                                                <tr valign="top">
                                                                                                    <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                                                                        <asp:Panel ID="pnl" runat="server">
                                                                                                            <telerik:RadGrid ID="gdVehicleContact" Width="900px" runat="server" AutoGenerateColumns="False"
                                                                                                                AllowFilteringByColumn="true" GridLines="None" ShowGroupPanel="false" Skin="Hay"
                                                                                                                OnNeedDataSource="gdVehicleContact_NeedDataSource" OnItemDataBound="gdVehicleContact_ItemDataBound"
                                                                                                                AllowSorting="true" AllowPaging="true" PageSize="10"  >
                                                                                                                <GroupingSettings CaseSensitive="false" />   
                                                                                                                <MasterTableView DataKeyNames="ContactInfoId" ClientDataKeyNames="ContactInfoId" CommandItemDisplay="Top"
                                                                                                                    GroupLoadMode="Server" >
                                                                                                                    <Columns>
                                                                                                                        <telerik:GridTemplateColumn DataField="Company" SortExpression="Company" HeaderText="Company" UniqueName="Company" meta:resourcekey="CompanyResource1">
                                                                                                                            <ItemTemplate>
                                                                                                                                <asp:Label ID="lblCompany" runat="server" Text="<%# Bind('Company') %>"></asp:Label>
                                                                                                                                &nbsp;
                                                                                                                            </ItemTemplate>
                                                                                                                            <ItemStyle Width="100px" VerticalAlign="Top" />
                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                        <telerik:GridTemplateColumn DataField="FirstName" SortExpression="FirstName" HeaderText="First Name" UniqueName="FirstName" meta:resourcekey="firstNameResource1">
                                                                                                                            <ItemTemplate>
                                                                                                                                <asp:Label ID="lblFirstName" runat="server" Text="<%# Bind('FirstName') %>"></asp:Label>&nbsp;
                                                                                                                            </ItemTemplate>
                                                                                                                            <ItemStyle Width="100px" VerticalAlign="Top" />
                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                        <telerik:GridTemplateColumn DataField="MiddleName" SortExpression="MiddleName" HeaderText="Middle Name" UniqueName="MiddleName" meta:resourcekey="middleNameResource1">
                                                                                                                            <ItemTemplate>
                                                                                                                                <asp:Label ID="lblMiddleName" runat="server" Text="<%# Bind('MiddleName') %>"></asp:Label>&nbsp;
                                                                                                                            </ItemTemplate>
                                                                                                                            <ItemStyle Width="100px" VerticalAlign="Top" />
                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                        <telerik:GridTemplateColumn DataField="LastName" SortExpression="LastName" HeaderText="Last Name" UniqueName="LastName" meta:resourcekey="lastNameResource1">
                                                                                                                            <ItemTemplate>
                                                                                                                                <asp:Label ID="lblLastName" runat="server" Text="<%# Bind('LastName') %>"></asp:Label>&nbsp;
                                                                                                                            </ItemTemplate>
                                                                                                                            <ItemStyle Width="100px" VerticalAlign="Top" />
                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                        <telerik:GridTemplateColumn AllowFiltering="false" SortExpression="TimeZone"  HeaderText="Time Zone" UniqueName="TimeZone" meta:resourcekey="TimeZoneResource1">
                                                                                                                            <ItemTemplate>
                                                                                                                                <asp:Label ID="lblTimeZone" runat="server"></asp:Label>
                                                                                                                            </ItemTemplate>
                                                                                                                            <ItemStyle Width="100px" VerticalAlign="Top" />
                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                        <telerik:GridTemplateColumn AllowFiltering="false" HeaderText="Contacts">
                                                                                                                            <ItemTemplate>
                                                                                                                                <telerik:RadGrid ID="gdContact" runat="server" AutoGenerateColumns="False" GridLines="None"
                                                                                                                                    Skin="Simple" Visible="false" OnItemDataBound="gdContact_ItemDataBound">
                                                                                                                                    <MasterTableView DataKeyNames="ContactCommunicationID" GroupLoadMode="Server" ShowHeader="false">
                                                                                                                                        <Columns>
                                                                                                                                            <telerik:GridTemplateColumn meta:resourcekey="communicationTypeNameResource1">
                                                                                                                                                <ItemTemplate>
                                                                                                                                                    <asp:Label ID="lblTypeName" runat="server" Text="<%# Bind('CommunicationTypeName') %>"></asp:Label>
                                                                                                                                                </ItemTemplate>
                                                                                                                                                <ItemStyle Width="30%" />
                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                            <telerik:GridTemplateColumn meta:resourcekey="CommunicationDataResource1">
                                                                                                                                                <ItemTemplate>
                                                                                                                                                    <asp:Label ID="lblTypeData" runat="server" Text="<%# Bind('CommunicationData') %>"></asp:Label>
                                                                                                                                                </ItemTemplate>
                                                                                                                                                <ItemStyle Width="80%" />
                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                        </Columns>
                                                                                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                                                                                        <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                                                                        <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                                                                                                                    </MasterTableView>
                                                                                                                                </telerik:RadGrid>
                                                                                                                                <asp:Literal ID="ltlNone" runat="server" Text="&nbsp;"></asp:Literal>
                                                                                                                            </ItemTemplate>
                                                                                                                            <ItemStyle Width="500px" CssClass="communicationsTable" />
                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                        <telerik:GridTemplateColumn AllowFiltering="false">
                                                                                                                            <ItemTemplate>
                                                                                                                                <asp:ImageButton ID="imgEdit" runat="server" ImageUrl="../../images/edit.gif" ToolTip="Modify" meta:resourcekey="imgEditResource2" />
                                                                                                                                <asp:HiddenField ID="hidTimeZone" runat="server" Value="<%# Bind('TimeZone') %>" />
                                                                                                                                <asp:HiddenField ID="hidIsCompany" runat="server" Value="<%# Bind('IsCompany') %>" />
                                                                                                                            </ItemTemplate>
                                                                                                                            <ItemStyle HorizontalAlign="Center" />
                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                        <telerik:GridTemplateColumn AllowFiltering="false">
                                                                                                                            <ItemTemplate>
                                                                                                                                <asp:ImageButton ID="imgDelete" runat="server" ImageUrl="../../images/delete.gif" ToolTip="Delete" meta:resourcekey="imgDeleteResource2" CommandName="Delete" /> 
                                                                                                                            </ItemTemplate>
                                                                                                                            <ItemStyle HorizontalAlign="Center" />
                                                                                                                        </telerik:GridTemplateColumn>
                                                                                                                    </Columns>
                                                                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" />
                                                                                                                    <AlternatingItemStyle HorizontalAlign="Left" VerticalAlign="Top" />
                                                                                                                    <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader" />
                                                                                                                    <CommandItemTemplate>
                                                                                                                        <table width="100%">
                                                                                                                            <tr>
                                                                                                                                <td align="left">
                                                                                                                                    <asp:Button ID="btAddNew" runat="server" Text="Add New Contact" OnClientClick="return ShowInsertForm();" CssClass="combutton" Width="130px" />
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
                                                                                                                <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader" />
                                                                                                                <ClientSettings>
                                                                                                                    <ClientEvents OnGridCreated="GridCreated" />
                                                                                                                </ClientSettings>
                                                                                                            </telerik:RadGrid>
                                                                                                            <asp:HiddenField ID="hidFilter" runat="server" />
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
                SetEditValues("", "", "", "", "0", null, "", "", true);
                $find('<%= RadWindowContentTemplate.ClientID%>').show();
                return false;
            }

            function OpenEdit(rowIndex) {
                var trs = $telerik.$("#<%=gdVehicleContact.ClientID%>>table>tbody>tr");
                var timeZone = "";
                var firstName = "";
                var lastName = "";
                var middleName = "";
                var company = "";
                var contactInfoId = "";
                var isCompany = "";
                var dataItem = $find("<%= gdVehicleContact.MasterTableView.ClientID %>").get_dataItems()[rowIndex];
                contactInfoId = dataItem.getDataKeyValue("ContactInfoId");
                if (rowIndex >= 0) {
                    timeZone = $telerik.$(trs[rowIndex]).find("input:hidden[id$='hidTimeZone']").val();
                    firstName = $telerik.$(trs[rowIndex]).find("span[id$='lblFirstName']").html();
                    lastName = $telerik.$(trs[rowIndex]).find("span[id$='lblLastName']").html();
                    middleName = $telerik.$(trs[rowIndex]).find("span[id$='lblMiddleName']").html();
                    company = $telerik.$(trs[rowIndex]).find("span[id$='lblCompany']").html();
                    isCompany = $telerik.$(trs[rowIndex]).find("input:hidden[id$='hidIsCompany']").val();
                }


                var postData = "{'ContactId':'" + contactInfoId + "'}";

                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnl.ClientID %>");
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmContacts.aspx/GetCommunications",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                            SetEditValues(company, firstName, middleName, lastName, timeZone, data.d, contactInfoId, isCompany, true);
                            $find('<%= RadWindowContentTemplate.ClientID%>').show();
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                            alert("<%= errorLoad%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });
                
//                var conatctsType = new Array();
//                var conatctsData = new Array();
//                $telerik.$(trs[rowIndex]).find("table[id*='gdContact']>tbody>tr").each(function (index) {
//                    var typeName = $telerik.$(this).find("span[id$='lblTypeName']").html();
//                    var typeData = $telerik.$(this).find("span[id$='lblTypeData']").html();
//                    conatctsType[index] = typeName;
//                    conatctsData[index] = typeData;
//                })
//                return false;
            }

            function CLoseEdit() {
                $find('<%= RadWindowContentTemplate.ClientID%>').close();
                return false;
            }

            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdVehicleContact.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=gdVehicleContact.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdVehicleContact.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
            }
            function refreshVehicleContactsGrid(arg) {
                if (arg) {
                    $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("Rebind");
                }
            }

            function confirmDelete(rowIndex) {
                var trs = $telerik.$("#<%=gdVehicleContact.ClientID%>>table>tbody>tr");
                var dataItem = $find("<%= gdVehicleContact.MasterTableView.ClientID %>").get_dataItems()[rowIndex];
                var contactInfoId = dataItem.getDataKeyValue("ContactInfoId");
                var postData = "{'ContactInfoId':'" + contactInfoId + "'}";
                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnl.ClientID %>");
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmContacts.aspx/IsContactInUsed",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                            var msg = "<%= contactInfo_del %>";
                            if (data.d == 'true') {
                                msg = "<%= contactInfo_del_used %>";
                            }

                            if (confirm(msg)) {
                                $find("<%= RadAjaxManager1.ClientID %>").ajaxRequest("Delete?" + contactInfoId);
                            }
                            else return false;
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                            alert("<%= errorLoad%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                        alert("<%= errorLoad%>");
                        alert(request.responseText);
                        return false;
                    }

                });
                return false;
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true"
        EnableAriaSupport="true">
        <Windows>
            <telerik:RadWindow ID="RadWindowContentTemplate" Title="Modify" VisibleStatusbar="false"
                VisibleTitlebar="false" Width="750px" Height="500px" runat="server" ReloadOnShow="true"
                ShowContentDuringLoad="false" Skin="Hay" Modal="true" meta:resourcekey="RadWindowContentTemplateResource1">
                <ContentTemplate>
                    <uc3:EditContact ID="EditContact1" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
    </telerik:RadWindowManager>
    </form>
</body>
</html>
