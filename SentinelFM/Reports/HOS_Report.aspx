<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HOS_Report.aspx.cs" Inherits="Reports_HOS_Report" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 100px;
            height: 22px;
        }
        .style2
        {
            width: 100%;
        }
        .style3
        {
            width: 38px;
        }
        .RegularText
        {}
    </style>
    </head>
<body>
    <form id="form1" runat="server">
            <div>
                <h1> Hours of Service report </h1>
            </div>
            <TABLE id="tblBody" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
							<TR>
								<TD>
									<TABLE id="tblForm" class=table 
										width="100%" height="100%" border="0">
										<TR>
											<TD class="configTabBackground" align="center" >
                                                <table>
                                                    <tr>
                                                        <td align="center" class="style2">
                                                            <table style="width: 577px">
                                                                <tr>
                                                                    <td colspan="3" style="width: 557px" valign=top  >
                                                         <fieldset >
                                                            <table class="formtext" style="width: 550px; height: 77px;">
                                                                <tr>
                                                                    <td align="left" class="style3">
                                                                        <asp:Label ID="lblDriver" runat="server" CssClass="formtext" Text="Driver:" meta:resourcekey="lblDriverResource1"></asp:Label>
                                                                        <asp:RangeValidator ID="valDriver" runat="server" ControlToValidate="cboDrivers"
                                                                            ErrorMessage="Please select a driver" MaximumValue="99999" MinimumValue="0" meta:resourcekey="valDriverResource1">*</asp:RangeValidator></td>
                                                                    <td align="left" style="width: 206px">
                                                                        <asp:DropDownList ID="cboDrivers" runat="server" CssClass="formtext" DataTextField="drivername"
                                                                            DataValueField="driverid" Width="175px" 
                                                                            meta:resourcekey="cboDriversResource1" Visible="False">
                                                                        </asp:DropDownList>
                                                                        <asp:ObjectDataSource ID="HOS_Drivers" runat="server" 
                                                                            OldValuesParameterFormatString="original_{0}" SelectMethod="GetDrivers" 
                                                                            TypeName="HOS_DBTableAdapters.GetDriversTableAdapter">
                                                                            <SelectParameters>
                                                                                <asp:Parameter Name="companyid" Type="Int32" />
                                                                            </SelectParameters>
                                                                        </asp:ObjectDataSource>
                                                                    </td>
                                                                    <td align="left" colspan="2">
                                                                        &nbsp;</td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left" class="style3">
                                                                        <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1"
                                                                            Text="From:"></asp:Label></td>
                                                                    <td align="left" style="width: 206px" >
                                                                        <table border="0" cellpadding="0" cellspacing="0">
                                                                            <tr>
                                                                                <td class="style1">
                                                                                       <asp:TextBox ID="txtFrom" runat="server" CssClass="RegularText" Width="173px" meta:resourcekey="txtToResource1"></asp:TextBox>
                                                                                </td>
                                                                                <td class="style1"><a id="calFrom"
                                                                                        onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtFrom','cal','width=220,height=200,left=570,top=380')"
                                                                                        href="javascript:;"><img src="../images/SmallCalendar.gif" border="0" id="imgFrom"></a>
                                                                                    </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td align="left" style="width: 19px">
                                                                        <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1"
                                                                            Text="To:"></asp:Label>
                                                                    </td>
                                                                    <td align="left" style="width: 182px">
                                                                        <table border="0" cellpadding="0" cellspacing="0">
                                                                            <tr>
                                                                                <td class="style1">
                                                                                    <asp:TextBox ID="txtTo" runat="server" CssClass="RegularText" Width="173px" meta:resourcekey="txtToResource1"></asp:TextBox>
                                                                                </td>
                                                                                <td class="style1">
                                                                                <a id="calTo"
                                                                                        onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtTo','cal','width=220,height=200,left=570,top=380')"
                                                                                        href="javascript:;"><img src="../images/SmallCalendar.gif" border="0" id="imgTo"></a>
                                                                                        </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                <td align="left" class="style3"></td>
                                                                    <td align="left" style="width: 206px">
                                                                    <asp:Button ID="cmdViewAllData" runat="server" CssClass="combutton" 
                                                                            Text="View All Logsheets" OnClick="cmdViewAllData_Click" 
                                                                            meta:resourcekey="cmdViewDataResource1" />&nbsp;
                                                                        &nbsp;</td>
                                                                    <td align="center" colspan="2">
                                                            <asp:Button ID="cmdViewData" runat="server" CssClass="combutton" 
                                                                            Text="View Driver Logsheet" OnClick="cmdViewData_Click" 
                                                                            meta:resourcekey="cmdViewDataResource1" />&nbsp;
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            </fieldset> 
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                           <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="errortext" meta:resourcekey="ValidationSummary1Resource1" />
                                                            <asp:Label ID="lblMessage" runat="server" CssClass="errortext" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                                    </tr>
                                                                                                       
                                                </table>
											</TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
        <div>
            <asp:GridView ID="LogSheetsGrid" runat="server" AutoGenerateColumns="False" 
                EnableModelValidation="True" onrowcommand="LogSheetsGrid_RowCommand">
                <Columns>
                    <asp:ButtonField CommandName="FileDownload" DataTextField="refid" 
                        HeaderText="Id" InsertVisible="False" />
                    <asp:BoundField DataField="drivername" HeaderText="Driver Name" 
                        InsertVisible="False" SortExpression="drivername" ReadOnly="True" />
                    <asp:BoundField DataField="date" HeaderText="Date" InsertVisible="False" 
                        SortExpression="date" ReadOnly="True" />
                    <asp:TemplateField HeaderText="File Name" InsertVisible="False" Visible="False">
                        <ItemTemplate>
                            <asp:Label id="lblFilePath" runat ="server" text='<%# Eval("FileName")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="HOS_DS_AllLogSheets" runat="server" 
                OldValuesParameterFormatString="original_{0}" SelectMethod="GetLogSheets" 
                TypeName="HOS_DBTableAdapters.GetReportLogSheetTableAdapter">
                <SelectParameters>
                    <asp:Parameter Name="companyid" Type="Int32" />
                    <asp:Parameter Name="start" Type="DateTime" />
                    <asp:Parameter Name="stop" Type="DateTime" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:ObjectDataSource ID="HOS_DS_DriverLogSheet" runat="server" 
                OldValuesParameterFormatString="original_{0}" SelectMethod="GetLogSheets" 
                TypeName="HOS_DBTableAdapters.GetReportLogSheet_ByDriverTableAdapter">
                <SelectParameters>
                    <asp:Parameter Name="companyid" Type="Int32" />
                    <asp:Parameter Name="start" Type="DateTime" />
                    <asp:Parameter Name="stop" Type="DateTime" />
                    <asp:Parameter Name="driverId" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />
        </div>
    </form>
</body>
</html>
