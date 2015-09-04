<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmLoginFaileds.aspx.cs" Inherits="frmLoginFaileds" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SentinelFM Login Manager</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="border-right: gray 4px double;
            border-top: gray 4px double; border-left: gray 4px double; width: 712px; border-bottom: gray 4px double;
            height: 163px">
            <tr>
                <td align="center" style="width: 570px; height: 149px" valign="top">
                    &nbsp;<table>
                        <tr>
                            <td style="width: 115px">
                                <asp:DataGrid ID="dgData" runat="server" AutoGenerateColumns="False" BackColor="White"
                                    BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black"
                                    OnSelectedIndexChanged="dgData_SelectedIndexChanged" DataKeyField="IPAddress" Width="507px">
                                    <FooterStyle BackColor="White" ForeColor="#009933" />
                                    <SelectedItemStyle BackColor="SteelBlue" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Right" Mode="NumericPages" />
                                    <AlternatingItemStyle BackColor="WhiteSmoke" />
                                    <ItemStyle BackColor="White" Font-Size="11px" ForeColor="Black" HorizontalAlign="Center"
                                        Wrap="False" />
                                    <HeaderStyle BackColor="AliceBlue" Font-Bold="True" Font-Size="11px" ForeColor="Black"
                                        HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
                                    <Columns>
                                        <asp:BoundColumn DataField="UserName" HeaderText="UserName"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="IPAddress" HeaderText="IPAddress" ReadOnly="True"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="NumTrials" HeaderText="Trials #" ReadOnly="True"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="Cycle" HeaderText="Cycle"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="Status" HeaderText="Status"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="LoginDate" HeaderText="Date"></asp:BoundColumn>
                                        <asp:ButtonColumn CommandName="Select" Text="Unlock"></asp:ButtonColumn>
                                    </Columns>
                                </asp:DataGrid>
                                <asp:Label ID="lblLogins" runat="server" Text="No Failed Logins"></asp:Label></td>
                        </tr>
                        <tr>
                            <td style="width: 115px" align="center">
                                <asp:Button ID="cmdView" runat="server" CssClass="combutton" OnClick="cmdView_Click"
                                    Text="Refresh" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
