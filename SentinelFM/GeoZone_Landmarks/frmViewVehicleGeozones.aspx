<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmViewVehicleGeozones.aspx.cs" Inherits="SentinelFM.frmViewVehicleGeozones" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assigned to Vehicles</title>
</head>
<body>
    <form id="form1" runat="server" >
     <div style="text-align: center">
    <asp:Label ID="lblMessage" runat="server" meta:resourcekey="lblMessageResource1"></asp:Label>
    </div>
    
    <div>
    
        <asp:DataGrid ID="dgVehicles" runat="server" Width="100%" GridLines="None" CellPadding="3"
            BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White" 
            PageSize="13" DataKeyField="VehicleId" AutoGenerateColumns="False" 
            BorderStyle="Ridge" meta:resourcekey="dgVehiclesResource1">
            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
            <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
            <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
            <Columns>
                <asp:BoundColumn DataField="BoxId" HeaderText="BoxId">
                    <HeaderStyle Wrap="False"></HeaderStyle>
                </asp:BoundColumn>
                                                                          
                                                                          
                <asp:BoundColumn DataField="Description" HeaderText="Vehicle">
                    <HeaderStyle Wrap="False"></HeaderStyle>
                </asp:BoundColumn>
                                                                            
                                                                            
                    <asp:BoundColumn DataField="LastCommunicatedDateTime" HeaderText="Last Communicated">
                    <HeaderStyle Wrap="False"></HeaderStyle>
                </asp:BoundColumn>
            </Columns>
                                                                       
        </asp:DataGrid>
    
    
    </div>
   
    
    </form>
</body>
</html>
