<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleGeozones.aspx.cs" Inherits="SentinelFM.GeoZone_Landmarks_frmVehicleGeozones" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assigned to Vehicles</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
                                                                    <asp:DataGrid ID="dgVehicles" runat="server" Width="100%" GridLines="None" CellPadding="3"
                                                                        BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White" PageSize="13"
                                                                        AllowPaging="True" DataKeyField="VehicleId" AutoGenerateColumns="False" BorderStyle="Ridge">
                                                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                        <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                        <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                        <Columns>
                                                                            <asp:BoundColumn DataField="Description" HeaderText="Vehicle">
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                          
                                                                        </Columns>
                                                                        <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                            Mode="NumericPages"></PagerStyle>
                                                                    </asp:DataGrid>
    
    </div>
    </form>
</body>
</html>
