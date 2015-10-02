<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmEmergency.aspx.cs" Inherits="SentinelFM.Configuration_Contact_frmEmergency" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
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
    <div>
        <table>
            <tr>
                <td align="left">
                    <nobr>
                   <asp:Label ID="lblDriver" runat="server" Text="Driver:" meta:resourcekey="lblDriverResource1" ></asp:Label>
                   <asp:Label ID="lblDriverName" runat="server" Text="" meta:resourcekey="lblDriverNameResource1" ></asp:Label>
                </nobr>
                </td>
                <td align="right" witdh="50px">
                    <asp:Button ID="btnClose" runat="server" Text="Close" meta:resourcekey="btnCloseResource1" OnClientClick="javascript:return returnToParent();" CssClass="combutton"/>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <fieldset>
                        <legend style="color: green" runat="server" id="legendPhones"></legend>
                        <telerik:RadGrid ID="gdPhones" runat="server" AutoGenerateColumns="False" GridLines="Both"
                            Width="700px" AllowSorting="false" ShowHeader="true" AllowPaging="false" BorderStyle="None"
                            meta:resourcekey="gdPhonesResource1" OnItemDataBound="gdPhones_ItemDataBound">
                            <MasterTableView BorderStyle="None" DataKeyNames="ContactCommunicationID" ClientDataKeyNames="ContactCommunicationID">
                                <Columns>
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
                                    <telerik:GridBoundColumn DataField="CommunicationData" HeaderText="Emergency Phone"
                                        meta:resourcekey="gdPhonesTelResource1">
                                        <ItemStyle Width="320" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn Visible="false">
                                        <ItemTemplate>
                                            <asp:Button ID="btnDetail" runat="server" Text="Detail" CommandName="Detail" meta:resourcekey="gdEmailsDetailResource1" CssClass="combutton" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="60px" />
                                    </telerik:GridTemplateColumn>
                                </Columns>
                                <ItemStyle HorizontalAlign="Left" />
                                <AlternatingItemStyle HorizontalAlign="Left" />
                                <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader" ForeColor="White" />
                            </MasterTableView>
                        </telerik:RadGrid>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <fieldset>
                        <legend style="color: green" runat="server" id="legendEmails"></legend>
                        <telerik:RadGrid ID="gdEmails" runat="server" AutoGenerateColumns="False" GridLines="Both"
                            Width="700px" AllowSorting="false" ShowHeader="true" AllowPaging="false" BorderStyle="None"
                            meta:resourcekey="gdEmailsResource1">
                            <MasterTableView BorderStyle="None" DataKeyNames="ContactCommunicationID" ClientDataKeyNames="ContactCommunicationID">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Name" HeaderText="Name" meta:resourcekey="gdEmailsNameResource1">
                                        <ItemStyle Width="250" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="CommunicationData" HeaderText="Email" meta:resourcekey="gdEmailsEmailResource1">
                                        <ItemStyle Width="320" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn Visible="false">
                                        <ItemTemplate>
                                            <asp:Button ID="btnDetail" runat="server" Text="Detail" CommandName="Detail" meta:resourcekey="gdEmailsDetailResource1" CssClass="combutton" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="60px" />
                                    </telerik:GridTemplateColumn>
                                </Columns>
                                <ItemStyle HorizontalAlign="Left" />
                                <AlternatingItemStyle HorizontalAlign="Left" />
                                <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader" ForeColor="White" />
                            </MasterTableView>
                        </telerik:RadGrid>
                    </fieldset>
                </td>
            </tr>
        </table>
    </div>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function returnToParent() {
                var oWnd = GetRadWindow();
                oWnd.close();
                return false;
            }
        </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>
