<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewDynamicForm.aspx.cs"
    Inherits="ViewDynamicForm" Theme="TelerikControl" %>

<%@ Register Src="HOSViewTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" AsyncPostBackTimeout="300">
        <Scripts>
            <asp:scriptreference assembly="Telerik.Web.UI" name="Telerik.Web.UI.Common.Core.js" />
            <asp:scriptreference assembly="Telerik.Web.UI" name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        EnableAJAX="false">
    </telerik:RadAjaxManager>
    <div>
        <div style="text-align: left">
            <table id="Table1" border="0" cellpadding="0" cellspacing="0">
                <tr align="left">
                    <td>
                        <asp:panel id="pnl" runat="server">
                            <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdDynamicForms" />
                                    </td>
                                </tr>
                                <tr valign="top">
                                    <td align="center">
                                        <table id="Table3" border="0" cellpadding="3" cellspacing="3" style="border-color: gray;
                                            border-style: outset; border-width: 2px; width: 800px">
                                            <tr>
                                                <td align="center">
                                                    <table style="border-width: 1px; border-color: Gray; border-style: solid; width: 550px;"
                                                        cellspacing="0">
                                                        <tr>
                                                            <td align="left">
                                                                <asp:label id="lblFromTitle" runat="server" cssclass="formtext" meta:resourcekey="lblFromTitleResource1"
                                                                    text="From:"></asp:label>
                                                            </td>
                                                            <td align="left">
                                                                <telerik:RadDatePicker ID="txtFrom" runat="server" Width="100px" Height="17px" Calendar-ShowRowHeaders="false"
                                                                    Calendar-Width="170px" meta:resourcekey="txtFromResource2" MaxDate="9998-12-31"
                                                                    MinDate="1753-01-01" OnLoad="txtFrom_Load">
                                                                    <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ShowRowHeaders="False"
                                                                        ViewSelectorText="x" Width="170px">
                                                                    </Calendar>
                                                                    <DateInput Height="17px" LabelCssClass="">
                                                                    </DateInput>
                                                                    <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
                                                                </telerik:RadDatePicker>
                                                            </td>
                                                            <td align="left">
                                                                <asp:label id="lblToTitle" runat="server" cssclass="formtext" meta:resourcekey="lblToTitleResource1"
                                                                    text="To:"></asp:label>
                                                            </td>
                                                            <td align="left">
                                                                <telerik:RadDatePicker ID="txtTo" runat="server" Width="100px" Height="17px" Calendar-ShowRowHeaders="false"
                                                                    Calendar-Width="170px" meta:resourcekey="txtFromResource2" MaxDate="9998-12-31"
                                                                    MinDate="1753-01-01">
                                                                    <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ShowRowHeaders="False"
                                                                        ViewSelectorText="x" Width="170px">
                                                                    </Calendar>
                                                                    <DateInput Height="17px" LabelCssClass="">
                                                                    </DateInput>
                                                                    <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
                                                                </telerik:RadDatePicker>
                                                            </td>
                                                            <td>
                                                                <asp:button id="cmdViewAllData" runat="server" text="View" onclick="cmdViewAllData_Click"
                                                                    height="29px" style="margin-top: 2px" width="167px" meta:resourcekey="cmdViewAllDataResource1" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                             <tr align="center">
                                                <td colspan="5">
                                                    <asp:placeholder id="PlaceHolder1" runat="server"></asp:placeholder>
                                                </td>
                                            </tr>

                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:panel>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
