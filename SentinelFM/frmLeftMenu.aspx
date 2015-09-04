<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmLeftMenu.aspx.cs" Inherits="frmLeftMenu" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <table>
          <tr>
             <td style="width: 100px" valign="top">
       <asp:TreeView ID="TreeView1" runat="server" ImageSet="Msdn" NodeIndent="10">
          <ParentNodeStyle Font-Bold="False" />
          <HoverNodeStyle BackColor="#CCCCCC" BorderColor="#888888" BorderStyle="Solid" Font-Underline="True" />
          <SelectedNodeStyle BackColor="White" BorderColor="#888888" BorderStyle="Solid" BorderWidth="1px"
             Font-Underline="False" HorizontalPadding="3px" VerticalPadding="1px" />
          <Nodes>
             <asp:TreeNode Text="Home" Value="Home"></asp:TreeNode>
             <asp:TreeNode Text="Map" Value="Map">
                <asp:TreeNode Text="Vehicles (12)" Value="Vehicles"></asp:TreeNode>
                <asp:TreeNode Text="Alarms (10)" Value="Alarms"></asp:TreeNode>
                <asp:TreeNode Text="Messages (20)" Value="Messages"></asp:TreeNode>
                <asp:TreeNode Text="Drivers (5)" Value="Drivers"></asp:TreeNode>
             </asp:TreeNode>
             <asp:TreeNode Text="History" Value="History"></asp:TreeNode>
             <asp:TreeNode Text="Maintenance" Value="Maintenance"></asp:TreeNode>
             <asp:TreeNode Text="Messages" Value="Messages"></asp:TreeNode>
             <asp:TreeNode Text="GeoPoints" Value="GeoPoints">
                <asp:TreeNode Text="Landmarks" Value="Landmarks"></asp:TreeNode>
                <asp:TreeNode Text="Geo Zones" Value="Geo Zones"></asp:TreeNode>
             </asp:TreeNode>
          </Nodes>
          <NodeStyle Font-Names="Verdana" Font-Size="10pt" ForeColor="Black" HorizontalPadding="5px"
             NodeSpacing="1px" VerticalPadding="2px" />
       </asp:TreeView>
                &nbsp;
             </td>
             <td style="width: 100px" valign="top">
                <img src="images/MapPointStartPageMap.png" /></td>
             
             
          </tr>
          <tr>
             <td style="width: 100px">
             </td>
             <td style="width: 100px">
             </td>
             <td style="width: 100px">
             </td>
          </tr>
          <tr>
             <td style="width: 100px">
             </td>
             <td style="width: 100px">
             </td>
             <td style="width: 100px">
             </td>
          </tr>
       </table>
       
       <br />
       <br />
       &nbsp;<br /><br /><br />
		
		
		<br /><br /><br />

    </div>
    </form>
</body>
</html>
