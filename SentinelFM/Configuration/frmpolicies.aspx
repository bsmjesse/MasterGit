<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmpolicies.aspx.cs" Inherits="SentinelFM.Configuration_frmpolicies" Culture="en-US" UICulture="auto" %>
<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
 	<!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
</head>
<body>
    <form id="form1" runat="server" name="Policies">
    <div>
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" style="z-index: 101; left: 8px; position: absolute; top: 4px" width="300">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="btnPolicies" IsPolicy="true" FleetUrl="frmfleets.aspx"  
                    VehicleUrl="frmfleetvehicles.aspx" />

                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td>
                                <table id="tblForm" border="0" cellpadding="0" cellspacing="0" style="border-right: gray 2px outset;
                                    border-top: gray 2px outset; border-left: gray 2px outset; border-bottom: gray 2px outset;
                                    height: 550px; width:990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="tblDriverAssgn" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
                                                                        width: 190px; position: relative; top: 0px; height: 22px">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Button ID="btnCommodities" runat="server" CausesValidation="False" CommandName="4"
                                                                                    CssClass="selectedbutton" Text="Commodities" Width="112px" /></td>
                                                                            <td>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="border-right: gray 2px outset;
                                                                                    border-top: gray 2px outset; border-left: gray 2px outset; width: 960px;
                                                                                    border-bottom: gray 2px outset;">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 98%; height: 495px">
                                                                                                <tr>
                                                                                                    <td align="center" style="width: 100%;" valign="top">
                                                                                                        <div id="tblViewProducts" runat="server">
                                                                                                            <br />
                                                                                                            <asp:Label ID="lblNoProductsMessage" runat="server" CssClass="errortext" Width="500px" Font-Bold="True"></asp:Label>
                                                                                                            <br />
                                                                                                            <asp:GridView ID="gdvProducts" runat="server" AllowPaging="True"
                                                                                                                BackColor="White" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3"
                                                                                                                CellSpacing="1" GridLines="None" PageSize="8" 
                                                                                                                Width="674px" CaptionAlign="Top" OnPageIndexChanging="gdvProducts_PageIndexChanging" AutoGenerateColumns="False" OnRowDeleting="gdvProducts_RowDeleting" OnRowEditing="gdvProducts_RowEditing">
                                                                                                                <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                                                                                                                <SelectedRowStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                                                                                                                <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right" />
                                                                                                                <AlternatingRowStyle BackColor="Beige" CssClass="gridtext" />
                                                                                                                <RowStyle BackColor="White" CssClass="gridtext" />
                                                                                                                <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                                                                <Columns>
                                                                                                                    <asp:BoundField DataField="ProductID" HeaderText="Product ID" SortExpression="ProductID" />
                                                                                                                    <asp:BoundField DataField="ProductName" HeaderText="Product Name" SortExpression="ProductName" />
                                                                                                                    <asp:BoundField DataField="Upper" HeaderText="Upper Limit" SortExpression="Upper" />
                                                                                                                    <asp:BoundField DataField="Lower" HeaderText="Lower Limit" SortExpression="Lower" />
                                                                                                                    <asp:CommandField ButtonType="Image" DeleteImageUrl="~/images/delete.gif" DeleteText=""
                                                                                                                        EditImageUrl="~/images/Edit.gif" EditText="" ShowCancelButton="False" ShowDeleteButton="True"
                                                                                                                        ShowEditButton="True" />
                                                                                                                </Columns>
                                                                                                            </asp:GridView>
                                                                                                            <br />
                                                                                                            <asp:Button ID="btnAddProduct" runat="server" CommandName="23" CssClass="combutton"
                                                                                                                 Text="Add a Product" Width="127px" OnClick="btnAddProduct_Click" />
                                                                                                        </div>
                                                                                                        <br />
                                                                                                        <table id="tblProductDetails" runat="server" border="0" cellpadding="0" cellspacing="2" class="RegularText">
                                                                                                            <tr style="height: 21px">
                                                                                                                <td align="left" style="width: 172px">
                                                                                                                    <asp:Label ID="lblProductName" runat="server" CssClass="RegularText" Width="168px">Product Name</asp:Label></td>
                                                                                                                <td align="left" style="width: 333px">
                                                                                                                    <asp:TextBox ID="txtProductName" runat="server" Width="320px" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left" style="width: 172px">
                                                                                                                    <asp:Label ID="lblUpper" runat="server" CssClass="RegularText" Width="168px">Upper Temperature</asp:Label></td>
                                                                                                                <td align="left" style="width: 333px">
                                                                                                                    <asp:TextBox ID="txtUpper" runat="server" Width="80px" CssClass="RegularText"></asp:TextBox></td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left" style="width: 172px">
                                                                                                                    <asp:Label ID="lblLower" runat="server" CssClass="RegularText" Width="168px" >Lower Temperature</asp:Label></td>
                                                                                                                <td align="left" valign="middle" style="width: 333px; height: 19px">
                                                                                                                    <asp:TextBox ID="txtLower" runat="server" Width="80px" CssClass="RegularText"></asp:TextBox>
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td align="left" style="width: 172px">
                                                                                                                    </td>
                                                                                                                <td align="left" colspan="3" style="height: 20px">
                                                                                                                    </td>
                                                                                                            </tr>
                                                                                                            <tr style="height: 20px">
                                                                                                                <td style="width: 172px" align="left">
                                                                                                                    <asp:Button ID="btnCancel" runat="server" CssClass="combutton" Text="Cancel" OnClick="btnCancel_Click" />
                                                                                                                </td>
                                                                                                                <td style="width: 333px" align="left">
                                                                                                                    <asp:Button ID="btnSave" runat="server" CssClass="combutton" Text="Save" OnClick="btnSave_Click" />
                                                                                                                </td>
                                                                                                            </tr>
                                                                                                        </table>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                    <td align="center" style="height: 15px">
                                                                                                        <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Height="20px" Width="615px" ></asp:Label>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
