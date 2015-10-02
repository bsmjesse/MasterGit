<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmInfo.aspx.cs" Inherits="frmInfo"  Theme="Default" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Information</title>
    <link href="GlobalStyle.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:SqlDataSource ID="sqlCompanyList" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
            SelectCommand="SELECT [OrganizationId], [OrganizationName] FROM [View_OrganizationList]">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sqlFwType" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
            SelectCommand="SELECT [FwTypeId], [FwTypeName] FROM [View_FirmwareTypeList]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sqlInfo" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
            SelectCommand="SELECT * FROM [View_CompanyBoxInfo]" EnableCaching="True"
            OnFiltering="sqlInfo_Filtering" 
            FilterExpression = "OrganizationId = {0}" CacheDuration="300" CacheExpirationPolicy="Sliding" >
            <FilterParameters>
                <asp:ControlParameter ControlID="ddlCompanyList" PropertyName="SelectedValue" Name="OrganizationId" Type="Int32" />
            </FilterParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sqlFwList" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString1 %>"
            OnFiltering="sqlFwList_Filtering" FilterExpression = "FwTypeId = {0}" EnableCaching="true"
            SelectCommand="SELECT [FwId], [FwName], [FwTypeId] FROM [View_FirmwareList]">
            <FilterParameters>
                <asp:ControlParameter ControlID="ddlFwType" Name="FwTypeId" PropertyName="SelectedValue" Type="Int16" />
            </FilterParameters>
        </asp:SqlDataSource>
    <div>
        <p class="header">Boxes Report</p>
        <asp:Label ID="lblError" runat="server" CssClass="errortext" EnableViewState="False" Width="1008px" ForeColor="Red"></asp:Label>
    </div>
    <br />
    <div style="margin:10,10,10,10;">
        <asp:panel ID="pnlFilters" runat="server" height="100px" width="992px" BorderStyle="Solid" BorderWidth="1px"> 
            <asp:Label ID="lblCaptionFilters" runat="server" Style="position:relative; left: 10px; top: 3px" Text="Data Filters" Font-Bold="True" Width="96px"></asp:Label>
            <br />
            <table style="width: 984px; margin:10,10,0,10; position: relative; z-index: 101;">
                <tr>
                    <td style="width: 220px">
                        <asp:Label ID="lblCompany" runat="server" Text="Company"></asp:Label>&nbsp;<asp:DropDownList ID="ddlCompanyList" runat="server" AutoPostBack="True" Width="200px" DataSourceID="sqlCompanyList" DataTextField="OrganizationName" DataValueField="OrganizationId" OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td style="width: 220px">
                        <asp:Label ID="lblFwType" runat="server" Text="Firmware type"></asp:Label>&nbsp;
                        <asp:DropDownList ID="ddlFwType" runat="server" AutoPostBack="True" Width="245px" DataSourceID="sqlFwType" DataTextField="FwTypeName" DataValueField="FwTypeId" OnSelectedIndexChanged="ddlFwType_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                    <td style="width: 550px">
                        <asp:Label ID="lblFirmware" runat="server" Text="Firmware name"></asp:Label>&nbsp;
                        <asp:DropDownList ID="ddlFirmware" runat="server" AutoPostBack="False" Width="480px" DataSourceID="sqlFwList" DataTextField="FwName" DataValueField="FwId" OnSelectedIndexChanged="ddlFirmware_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width: 153px">
                        <asp:Label ID="lblBoxPort" runat="server" Text="Box Port"></asp:Label>
                        <br />
                        <asp:TextBox ID="txtPort" runat="server" Width="128px"></asp:TextBox>
                   </td>
                    <td style="width: 178px">
                        <div id="dtFrom" runat="server" style="visibility:hidden">
                            <asp:Label ID="lblLastDateFrom" runat="server" Text="Last comm date from"></asp:Label>&nbsp;
                            <asp:TextBox ID="txtLastDateFrom" runat="server" Width="120px"></asp:TextBox>&nbsp;
                            <a href="#" onclick="window.open('UserControl/frmCalendar.aspx?textbox=txtLastDateFrom','cal','width=220,height=200,left=270,top=380')">
                               <img alt="Calendar" src="images/SmallCalendar.gif" style="border-top-width: 0px; border-left-width: 0px; border-bottom-width: 0px; border-right-width: 0px;" />
                            </a>
                        </div>
                    </td>
                    <td style="width: 245px">
                        <div id="dtTo" runat="server" style="visibility:hidden">
                            <asp:Label ID="lblLastDateTo" runat="server" Text="Last comm date to"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtLastDateTo" runat="server" Width="120px"></asp:TextBox>
                            <a href="#" onclick="window.open('UserControl/frmCalendar.aspx?textbox=txtLastDateTo','cal','width=220,height=200,left=270,top=380')">
                               <img alt="Calendar" src="images/SmallCalendar.gif" style="border-top-width: 0px; border-left-width: 0px; border-bottom-width: 0px; border-right-width: 0px" />
                            </a>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="width: 153px">
                    </td>
                    <td style="width: 178px">
                    </td>
                    <td style="width: 245px">
                    </td>
                </tr>
            </table>
        </asp:panel>
        <br />
        <asp:panel ID="pnlColumns" runat="server" height="120px" width="1008px" BorderStyle="Solid" BorderWidth="1px">
            <asp:Label ID="lblCaptionColumns" runat="server" Font-Bold="True" Style="position: relative; left: 10px; top: 3px" Text="Select fields to display" Width="160px"></asp:Label>
            <br />
            <asp:CheckBoxList ID="CheckBoxList1" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"
                Style="left: 8px; position: relative; top: 8px; z-index: 101;" Width="992px" AutoPostBack="True" OnSelectedIndexChanged="CheckBoxList1_SelectedIndexChanged">
                <asp:ListItem Value="Company" Selected="True">Company</asp:ListItem>
                <asp:ListItem Value="BoxId" Selected="True">Box Id</asp:ListItem>
                <asp:ListItem Value="LastCommunicated" Selected="True">Last Communicated</asp:ListItem>
                <asp:ListItem Value="FirmwareType" Selected="True">Firmware Type</asp:ListItem>
                <asp:ListItem Value="FirmwareName" Selected="True">Firmware Name</asp:ListItem>
                <asp:ListItem Value="HardwareType" Selected="True">Hardware Type</asp:ListItem>
                <asp:ListItem Value="CommunicationMode" Selected="True">Communication Mode</asp:ListItem>
                <asp:ListItem Value="Protocol" Selected="True">Protocol</asp:ListItem>
                <asp:ListItem Value="VehicleDescription" Selected="True">Vehicle Description</asp:ListItem>
                <asp:ListItem Selected="True">Port</asp:ListItem>
                <asp:ListItem Selected="True" Value="PhoneNumber">Phone Number</asp:ListItem>
                <asp:ListItem Selected="True" Value="DeviceID">Device ID</asp:ListItem>
            </asp:CheckBoxList></asp:panel>
        <br />
        <asp:panel ID="pnlButtons" runat="server" Height="32px" Width="992px">
           <asp:Button ID="btnView" runat="server" Text="Load data" OnClick="btnView_Click" style="position: relative; left: 10px; top: 4px;" />
           <asp:Button ID="btnExport" runat="server" Text="Export to Excel" OnClick="btnExport_Click" style="left: 100px; position: relative; top: 4px;" />
        </asp:panel>
        <br />
        <asp:panel ID="pnlResults" runat="server">
            <asp:GridView ID="gdvwData" runat="server" Width="1148px" AllowPaging="True" PageSize="20" AutoGenerateColumns="False" BackColor="White" PagerSettings-Mode="NumericFirstLast" OnPageIndexChanging="gdvwData_PageIndexChanging" DataSourceID="sqlInfo" CaptionAlign="Top" CellPadding="2" AllowSorting="True">
                <Columns>
                    <asp:BoundField DataField="Company" HeaderText="Company" SortExpression="Company" ReadOnly="True" />
                    <asp:BoundField DataField="Box Id" HeaderText="Box Id" SortExpression="Box Id" ReadOnly="True" />
                    <asp:BoundField DataField="Last Communicated" HeaderText="Last Communicated"
                        SortExpression="Last Communicated" NullDisplayText="N/A" ReadOnly="True" />
                    <asp:BoundField DataField="Fw Type" HeaderText="Fw Type" SortExpression="Fw Type" ReadOnly="True" />
                    <asp:BoundField DataField="Firmware" HeaderText="Firmware Name" SortExpression="Firmware" ReadOnly="True" />
                    <asp:BoundField DataField="Hw Type" HeaderText="Hw Type" SortExpression="Hw Type" ReadOnly="True" />
                    <asp:BoundField DataField="Comm Mode" HeaderText="Comm Mode" SortExpression="Comm Mode" ReadOnly="True" />
                    <asp:BoundField DataField="Protocol" HeaderText="Protocol"
                        SortExpression="Protocol" ReadOnly="True" />
                    <asp:BoundField DataField="Vehicle Description" HeaderText="Vehicle Description"
                        SortExpression="Vehicle Description" ReadOnly="True" />
                    <asp:BoundField DataField="Port" HeaderText="Port" NullDisplayText="N/A" ReadOnly="True"
                        SortExpression="Port" />
                    <asp:BoundField DataField="Phone Number" HeaderText="Phone Number" NullDisplayText="N/A"
                        ReadOnly="True" SortExpression="Phone Number" />
                    <asp:BoundField DataField="Device ID" HeaderText="Device ID" NullDisplayText="N/A"
                        ReadOnly="True" SortExpression="Device ID" />
                </Columns>
                <PagerSettings Mode="NumericFirstLast" />
                <RowStyle Wrap="True" />
                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
            </asp:GridView>
        </asp:panel>
    </div>
    </form>
</body>
</html>
