<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="frmCNInvoice.aspx.cs" Inherits="_Default" Debug="true" Theme="Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Manage Landmarks</title>
    <link rel="stylesheet" type="text/css" href="GlobalStyle.css" />
</head>
<body>
    <form id="form1" runat="server" enableviewstate="true">
    <div>
        <table cellpadding="5" style="font-size: small; width: 703px; font-family: Arial; border-width: medium; border-color:Gray">
            <tr>
                <td style="width: 60px">
                </td>
                <td style="width: 409px">
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 60px">
                </td>
                <td style="width: 409px">
                    <asp:Label ID="Label2" runat="server" Text="Upload Invoice XLS file" 
                        CssClass="heading" Width="264px" Font-Names="Arial" />
                    <br />
                    <asp:Label ID="lblMessage" runat ="server" >
                    
                    </asp:Label> 
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 60px">
                </td>
                <td style="width: 409px">
                    <asp:FileUpload ID="FileUpload1" runat="server" Width="528px" ToolTip="Landmarks XLS file" />
                    <asp:RequiredFieldValidator ID="rvFileUpload" runat ="server" ValidationGroup ="gpUpload" ErrorMessage ="Please select a file to upload." ControlToValidate ="FileUpload1"  ></asp:RequiredFieldValidator>
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 60px">
                    &nbsp;</td>
                <td style="width: 409px">
                    &nbsp;</td>
                <td style="width: 65px">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="width: 60px">
                    &nbsp;</td>
                <td style="width: 409px">
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://beta.sentinelfm.com/admin/data.txt" Target="_blank" >View Results</asp:HyperLink>
                </td>
                <td style="width: 65px">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="width: 60px">
                    &nbsp;</td>
                <td style="width: 409px">
                    &nbsp;</td>
                <td style="width: 65px">
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="width: 60px">
                </td>
                <td style="width: 409px">
                    <asp:CheckBox ID="CheckBox2" runat="server" Text="First line contains headers" 
                        Width="186px" CssClass="RegularText" Checked="True" Visible="False" />
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 60px">
                </td>
                <td style="width: 409px">
                    <asp:Button id="cmdSubmit" runat="server" Text="Submit" CssClass="combutton" 
                        OnClick="cmdSubmit_Click" ValidationGroup ="gpUpload" />
                </td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 60px">
                </td>
                <td style="width: 409px">
                    <asp:Label ID="Label1" runat="server" Font-Names="Arial" Font-Size="Small" SkinID ="None"
                        ForeColor="Red" Height="18px" Width="536px" CssClass="errortext" Font-Bold="True"></asp:Label></td>
                <td style="width: 65px">
                </td>
            </tr>
            <tr>
                <td style="width: 60px">
                </td>
            
                <td colspan ="2" >
                    &nbsp;</td>


                   


            </tr>
        </table>
    </div>
    </form>
</body>
</html>
