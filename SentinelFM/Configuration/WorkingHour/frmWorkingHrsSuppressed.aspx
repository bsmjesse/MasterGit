<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmWorkingHrsSuppressed.aspx.cs"
    Inherits="SentinelFM.Configuration_WorkingHour_frmWorkingHrsSuppressed" Theme="TelerikControl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="ctlWorkingHrsl.ascx" TagName="ctlWorkingHrsl" TagPrefix="uc2" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .NoTDBorder
        {
            border-left-width: 0px !important;
        }
    </style>
    <script type="text/javascript" src="../../Scripts/Telerik_AddIn.js"></script>
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
        OnAjaxRequest="RadAjaxManager1_AjaxRequest" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgSuppressedEmail">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls> 
            </telerik:AjaxSetting>           
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:Panel ID="pnl" runat="server">
        <div style="text-align: left; width: 900px">
            <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300">
                <tr>
                    <td>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdSupress" />
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
                                                    <tr align="center">
                                                        <td valign="top">
                                                            <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <uc2:ctlWorkingHrsl ID="ctlWorkingHrsl1" runat="server" selectedcontrol="cmdSupress" />
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
                                                                                                            <table runat="server" id="pnlAll" width="600px">
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <table runat="server" id="pnlCon">
                                                                                                                            <tr valign="top">
                                                                                                                                <td align="left">
                                                                                                                                    <asp:Label ID="lblFleet" runat="server" CssClass="formtext" Width="40px" meta:resourcekey="lblFleetResource1"
                                                                                                                                        Text="Fleet:"></asp:Label>
                                                                                                                                </td>
                                                                                                                                <td style="width: 312px;" align="left">
                                                                                                                                    <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px"
                                                                                                                                        Filter="Contains" MarkFirstMatch="True" DataTextField="FleetName" DataValueField="FleetId"
                                                                                                                                        meta:resourcekey="cboFleetResource1" Skin="Hay" MaxHeight="200px">
                                                                                                                                    </telerik:RadComboBox>
                                                                                                                                    <asp:RequiredFieldValidator ID="rvcboFleet" runat="server" ControlToValidate="cboFleet"
                                                                                                                                        CssClass="errortext" ErrorMessage="" Display="Dynamic" Text="<br/>Please Select a Fleet."
                                                                                                                                        ValidationGroup="Add" meta:resourcekey="rvcboFleetResource1"></asp:RequiredFieldValidator>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    &nbsp;
                                                                                                                                </td>
                                                                                                                                <td class="formtext">
                                                                                                                                    <asp:DropDownList ID="ddlDateTime" runat="server" class="formText" meta:resourcekey="ddlDateTimeResource1">
                                                                                                                                        <asp:ListItem Text="One Week" Value="1" Selected="True"></asp:ListItem>
                                                                                                                                        <asp:ListItem Text="Two Weeks" Value="2"></asp:ListItem>
                                                                                                                                        <asp:ListItem Text="One Month" Value="3"></asp:ListItem>
                                                                                                                                        <asp:ListItem Text="Two Months" Value="4"></asp:ListItem>
                                                                                                                                        <asp:ListItem Text="Three Months" Value="5"></asp:ListItem>
                                                                                                                                        <asp:ListItem Text="Six Months" Value="6"></asp:ListItem>
                                                                                                                                        <asp:ListItem Text="One Year" Value="7"></asp:ListItem>
                                                                                                                                        <asp:ListItem Text="Two Years" Value="8"></asp:ListItem>
                                                                                                                                        <asp:ListItem Text="All" Value="9"></asp:ListItem>
                                                                                                                                    </asp:DropDownList>
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    &nbsp;
                                                                                                                                </td>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="btmOK" runat="Server" Text="View" CssClass="combutton" meta:resourcekey="btmOKResource1"
                                                                                                                                        ValidationGroup="Add" OnClick="btmOK_Click" />
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <Sentinel:Grid ID="dgSuppressedEmail" runat="server" Visible="true" FilterItemStyle-HorizontalAlign="Left"
                                                                                                                            AllowSorting="True" AutoGenerateColumns="False" LoadingPanelID="LoadingPanel1" 
                                                                                                                            AllowFilteringByColumn="true" ExportText="Export" AllowPaging="true" PageSize="20"
                                                                                                                             Height="600px" OnNeedDataSource="dgSuppressedEmail_NeedDataSource" 
                                                                                                                            Skin="Simple" meta:resourcekey="dgSuppressedEmailResource1" Width="850px">
                                                                                                                            <GroupingSettings CaseSensitive="False" />

                                                                                                                            <MasterTableView CommandItemDisplay="Top" Width="850px" ClientDataKeyNames="BoxId" DataKeyNames="BoxId">
                                                                                                                                 <PagerStyle Mode="NextPrevAndNumeric" />
                                                                                                                                <Columns>
                                                                                                                                    <telerik:GridBoundColumn HeaderText="Box ID" DataField="BoxId" UniqueName="BoxId">
                                                                                                                                        <ItemStyle Width="150px" />
                                                                                                                                        <HeaderStyle Width="150px" />
                                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                                    <telerik:GridBoundColumn DataField="OriginDateTime" DataType="System.DateTime" HeaderText="Origin DateTime"
                                                                                                                                        meta:resourcekey="GridBoundColumnResource1" UniqueName="OriginDateTime">
                                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                                    <telerik:GridBoundColumn DataField="DateTimeReceived" DataType="System.DateTime"
                                                                                                                                        HeaderText="Received DateTime" meta:resourcekey="GridBoundColumnResource2" UniqueName="DateTimeReceived">
                                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                                    <telerik:GridBoundColumn DataField="Latitude" HeaderText="Latitude" DataType="System.Double"
                                                                                                                                        meta:resourcekey="GridBoundColumnResource3" UniqueName="Latitude">
                                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                                    <telerik:GridBoundColumn DataField="Longitude" HeaderText="Longitude" DataType="System.Double"
                                                                                                                                        meta:resourcekey="GridBoundColumnResource4" UniqueName="Longitude">
                                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                                    <telerik:GridBoundColumn DataField="StreetAddress" HeaderText="Street Address" meta:resourcekey="GridBoundColumnResource5"
                                                                                                                                        UniqueName="StreetAddress">
                                                                                                                                    </telerik:GridBoundColumn>
                                                                                                                                </Columns>
                                                                                                                                <CommandItemTemplate>
                                                                                                                                    <table id="tblCustomerCommand" runat="server" width="100%">
                                                                                                                                        <tr>
                                                                                                                                            <td align="left">
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </table>
                                                                                                                                </CommandItemTemplate>
                                                                                                                            </MasterTableView>
                                                                                                                            <FilterItemStyle HorizontalAlign="Left" /> 

                            <ClientSettings>
                                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="500px" SaveScrollPosition="false" />
                            </ClientSettings>
                                                                                                                        </Sentinel:Grid>
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
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
   <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
                $telerik.$(window).resize(function () {
                <%= resizeScript %>
            });
    </script>
   </telerik:RadCodeBlock>
    
    </form>
</body>
</html>
