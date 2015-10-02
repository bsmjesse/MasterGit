<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmPanicRegistry.aspx.cs" Inherits="SentinelFM.Admin.frmPanicRegistry" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }

.RegularText
{
	font-weight: normal;
	font-size: 11px;
	text-transform: capitalize;
	color: black;
	font-family: Verdana, Arial, sans-serif;
	text-decoration: none;
            text-align: left;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
    <table style="width:100%" >
        <tr>
            <td style="text-align: center">
                                                <asp:dropdownlist id="cboOrganization" runat="server" 
                                                    DataTextField="OrganizationName" DataValueField="OrganizationId"
													AutoPostBack="True" Width="215px" CssClass="RegularText" 
                                                    onselectedindexchanged="cboOrganization_SelectedIndexChanged"></asp:dropdownlist>
            </td>
        </tr>
        <tr>
            <td style="text-align: center">
						&nbsp;</td>
        </tr>
        <tr>
            <td align=middle >
						<asp:datagrid id="dgCommDiag" runat="server" ForeColor="Black" CellPadding="4" BackColor="White"
							BorderWidth="1px" BorderStyle="None" BorderColor="#DEDFDE" Width="626px" AutoGenerateColumns="False">
							<FooterStyle ForeColor="#009933" BackColor="White"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SteelBlue"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
							<ItemStyle Font-Size="11px" Wrap="False" HorizontalAlign="Center" ForeColor="Black" BackColor="White"></ItemStyle>
							<HeaderStyle Font-Size="11px" Font-Bold="True" Wrap="False" HorizontalAlign="Center" ForeColor="Black"
								VerticalAlign="Middle" BackColor="AliceBlue"></HeaderStyle>
							<Columns>
								<asp:BoundColumn DataField="BoxId" HeaderText="BoxId"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" HeaderText="Description"></asp:BoundColumn>
								<asp:BoundColumn DataField="DeviceId" HeaderText="Panic Device Id"></asp:BoundColumn>
								<asp:BoundColumn DataField="RegistryDate" HeaderText="RegistryDate"></asp:BoundColumn>
								<asp:BoundColumn DataField="AccountId" HeaderText="AccountId"></asp:BoundColumn>
								   <asp:ButtonColumn></asp:ButtonColumn>  
								<asp:BoundColumn DataField="CommAddressValue" HeaderText="Box Communication Info Device Id"></asp:BoundColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
        </tr>
        <tr>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <div>
    
    </div>
    </form>
</body>
</html>
