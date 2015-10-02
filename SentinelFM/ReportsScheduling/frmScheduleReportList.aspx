<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmScheduleReportList.aspx.cs"
    Inherits="SentinelFM.ReportsScheduling_frmScheduleReportList" Culture="en-US"
    meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SentinelFM</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="border: gray 2px outset;
                    z-index: 101; left: 8px; position: absolute; top: 4px; height: 97%; background-color: #fffff0; width: 98%">
                    <tr>
                        <td style="text-align:center;">
                            <table style="margin: 20px 20px 20px 20px">
                                <tr>
                                    <td style="text-align: left; width: 100px">
                                        <asp:Label ID="lblScheduledReports" runat="server" Text="Scheduled Reports"  CssClass="formtext" meta:resourcekey="lblScheduledReportsResource1" Font-Bold="True"></asp:Label></td>
                                    <td style="width: 50px">
                                    </td>
                                    <td style="text-align: left; width: 406px">
                                        <asp:Label ID="lblStoredReports" runat="server" Text="Emailed/Stored Reports" CssClass="formtext" meta:resourcekey="lblStoredReportsResource1" Font-Bold="True" Width="184px"></asp:Label>
                                    </td>
                                </tr>
                                <tr style="height: 21px;">
                                    <td style="width: 100px; text-align: left; height: 21px;">
                                        <asp:Label ID="lblNoDataMessage" runat="server" CssClass="formtext" Font-Bold="True" ForeColor="Red"
                                            meta:resourcekey="lblMessageResource1" Visible="False" Width="528px" Text="No Scheduled Reports"></asp:Label></td>
                                    <td style="width: 50px; height: 21px;">
                                    </td>
                                    <td style="height: 21px; text-align: left" align="left">
                                        <asp:Label ID="lblNoFilesMessage" runat="server" CssClass="formtext" Font-Bold="True" ForeColor="Red"
                                            Visible="False" Text="No Stored Files"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="vertical-align:top;">
                                        <asp:DataGrid ID="dgReports" runat="server" BackColor="White" BorderColor="White"
                                            BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1" DataKeyField="ReportID"
                                            GridLines="None" AutoGenerateColumns="False" AllowPaging="True"
                                            OnDeleteCommand="dgReports_DeleteCommand" OnItemCreated="dgReports_ItemCreated"
                                            OnPageIndexChanged="dgReports_PageIndexChanged" meta:resourcekey="dgReportsResource1" 
                                            OnSelectedIndexChanged="dgReports_SelectedIndexChanged"
                                            OnItemDataBound="dgReports_ItemDataBound" PageSize="9">
                                            <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                                            <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right" Mode="NumericPages" />
                                            <AlternatingItemStyle BackColor="Beige" CssClass="gridtext" />
                                            <ItemStyle BackColor="White" CssClass="gridtext" />
                                            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                            <Columns>
                                                <asp:BoundColumn DataField="ReportId" HeaderText='<%$ Resources:dgReports_ReportId %>' ></asp:BoundColumn>
                                                <asp:ButtonColumn CommandName="Select" ButtonType="LinkButton" HeaderText='<%$ Resources:dgReports_Report %>' DataTextField="GuiName" Text="Select"></asp:ButtonColumn>
                                                <asp:BoundColumn DataField="DateFrom" HeaderText='<%$ Resources:dgReports_From %>'></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DateTo" HeaderText='<%$ Resources:dgReports_To %>'></asp:BoundColumn>
                                                <asp:BoundColumn DataField="FleetName" HeaderText='<%$ Resources:dgReports_Fleet %>'></asp:BoundColumn>
                                                <asp:BoundColumn DataField="StartScheduledDate" HeaderText='<%$ Resources:dgReports_ScheduledDate %>'></asp:BoundColumn>
                                                <asp:BoundColumn DataField="EndScheduledDate" HeaderText='<%$ Resources:dgReports_EndScheduled %>'></asp:BoundColumn>
                                                <asp:BoundColumn DataField="Status" HeaderText='<%$ Resources:dgReports_Status %>'></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DeliveryMethodType" HeaderText='<%$ Resources:dgReports_DeliveryMethodType %>'> </asp:BoundColumn>
                                                <asp:ButtonColumn CommandName="Delete" meta:resourceKey="ButtonColumnResource1" Text="&lt;img src=../images/delete.gif border=0&gt;"></asp:ButtonColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </td>
                                    <td style="width: 50px">
                                    </td>
                                    <td style="vertical-align:top; " align="left">
                                        <asp:DataGrid ID="dgStoredreports" runat="server" BackColor="White" BorderColor="White"
                                            BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1" DataKeyField="RowID"
                                            GridLines="None" AutoGenerateColumns="False" AllowPaging="True" meta:resourcekey="dgReportsResource1" 
                                            OnDeleteCommand="dgStoredreports_DeleteCommand"
                                            PageSize="9" OnPageIndexChanged="dgStoredreports_PageIndexChanged" 
                                            onitemdatabound="dgStoredreports_ItemDataBound1">
                                            <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                                            <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right"
                                                Mode="NumericPages" />
                                            <AlternatingItemStyle BackColor="Beige" CssClass="gridtext" />
                                            <ItemStyle BackColor="White" CssClass="gridtext" />
                                            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                            <Columns>
                                                <asp:HyperLinkColumn DataTextField="GuiName" Target=_blank   HeaderText='<%$ Resources:dgStoredreports_URL %>' DataNavigateUrlField="ReportFileName"></asp:HyperLinkColumn>
                                                <asp:BoundColumn DataField="DateCreated" HeaderText='<%$ Resources:dgStoredreports_Date %>'></asp:BoundColumn>
                                                <asp:BoundColumn DataField="DeliveryMethod"  Visible=false ></asp:BoundColumn>
                                                <asp:ButtonColumn CommandName="Delete" Text="&lt;img src=../images/delete.gif border=0&gt;"></asp:ButtonColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </td>
                                </tr>
                                <tr style="height: 21px">
                                    <td style="text-align:right;">
                                        <asp:Button ID="cmdBack" runat="server" CssClass="combutton" meta:resourcekey="cmdBackResource1" OnClick="cmdBack_Click" Text="Back" />
                                    </td>
                                    <td style="width: 50px">
                                    </td>
                                    <td style="width: 406px">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
