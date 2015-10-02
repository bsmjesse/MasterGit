<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmSuperOrganizationMenu.aspx.cs" Inherits="SentinelFM.frmSuperOrganizationMenu" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Organization List</title>
    <link href="GlobalStyle.css" type="text/css" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
    <table style="width: 100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td align="center">
                <asp:Label ID="lblOrganizationList" runat="server" CssClass="formtext" Text="Organizations List" Font-Bold="True" Font-Size="Small" meta:resourcekey="lblOrganizationListResource1"></asp:Label>
                &nbsp; &nbsp;&nbsp; &nbsp;<asp:Button ID="cmdLogout" runat="server" CssClass="combutton"
                    meta:resourcekey="cmdClearResource1" OnClick="cmdLogout_Click" Text="Logout"
                    Width="70px" /></td>
        </tr>
        <tr>
            <td style="height:50px;" >
            </td>
        </tr>
        <tr>
            <td align=center >
                    
        <asp:DataGrid ID="dgOrganizations" runat="server" AutoGenerateColumns="False"
            BorderColor="#E0E0E0" BorderStyle="Solid" BorderWidth="1px" CellPadding="4" ForeColor="#333333"
            GridLines="None" Width="500px" OnSelectedIndexChanged="dgOrganizations_SelectedIndexChanged" DataKeyField="OrganizationId" meta:resourcekey="dgOrganizationsResource1">
            <PagerStyle BackColor="#284775" Font-Size="11px" ForeColor="White" HorizontalAlign="Center" />
            <AlternatingItemStyle BackColor="White" CssClass="gridtext" ForeColor="#284775" />
            <ItemStyle BackColor="#F7F6F3" CssClass="gridtext" ForeColor="#333333" />
            <Columns>
                <asp:BoundColumn DataField="OrganizationName" HeaderText="Name" ItemStyle-HorizontalAlign="Left" meta:resourcekey="ButtonColumnNameResource1"></asp:BoundColumn>
                <asp:ButtonColumn CommandName="Select" Text="Login"  meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
            </Columns>
            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditItemStyle BackColor="#999999" />
            <SelectedItemStyle BackColor="WhiteSmoke" Font-Bold="True" ForeColor="#333333" />
        </asp:DataGrid>
        


            </td>
        </tr> 
    </table> 
    </form>
</body>
</html>
