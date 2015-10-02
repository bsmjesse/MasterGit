<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmNewGarminMsgs.aspx.cs" Inherits="SentinelFM.Messages_frmNewGarminMsgs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Send Message to Garmin</title>
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <table class="style1">
            <tr>
                <td>
                    &nbsp;</td>
               
                <td>
                    <asp:DropDownList ID="cboMessageType" runat="server" CssClass="formtext" 
                        Visible="False">
                        <asp:ListItem Value="42">Text Message</asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td valign=top  >
                    <asp:Label ID="lblMessageBody" runat="server" CssClass="formtext" 
                        Text="Message:"></asp:Label>
                </td>
               
                <td>
                    <asp:TextBox ID="txtMessageBody" runat="server" CssClass="formtext" 
                        TextMode="MultiLine" Width="80%" Height="63px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;</td>
                
                <td align="center">
                    <asp:Button ID="cmdCancel" runat="server" CssClass="combutton" Text="Close" OnClientClick="top.close()" />
&nbsp;<asp:Button ID="cmdSent" runat="server" CssClass="combutton" Text="Send" 
                        onclick="cmdSent_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;</td>
                
                <td>
                    <asp:Label ID="lblMessage" runat="server" CssClass="formtext"></asp:Label>
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
