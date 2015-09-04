<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmFleetVehicles.aspx.cs" Inherits="SentinelFM.Map_frmFleetVehicles" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Fleet - Vehicle Assigment</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <table>
          <tr>
             <td style="width: 100px">
                <asp:Label ID="lblFleet" runat="server" Text="Fleet" meta:resourcekey="lblFleetResource1"></asp:Label></td>
             <td style="width: 100px">
       <asp:DropDownList ID="cboFleet" runat="server"  BackColor="WhiteSmoke"
          CssClass="RegularText" DataTextField="FleetName" DataValueField="FleetId" 
           Width="317px" meta:resourcekey="cboFleetResource1">
       </asp:DropDownList></td>
          </tr>
          <tr>
             <td style="width: 100px">
             </td>
             <td style="height: 10px" align="center">
                
                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" meta:resourcekey="lblMessageResource1"
                   Visible="False"></asp:Label></td>
          </tr>
          <tr>
             <td style="width: 100px">
             </td>
             <td style="width: 100px; height: 10px">
                <table>
                   <tr>
                      <td style="width: 100px">
                         <asp:Button ID="cmdCancel" OnClientClick="window.close()"  runat="server" Text="Close" CssClass="Commands" meta:resourcekey="cmdCancelResource1" /></td>
                      <td style="width: 100px">
                      </td>
                      <td style="width: 100px">
                         <asp:Button ID="cmdSave" runat="server" Text="Save" CssClass="Commands" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1" /></td>
                   </tr>
                </table>
             </td>
          </tr>
       </table>
    </div>
    </form>
</body>
</html>
