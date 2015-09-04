<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FirmwareMinder.aspx.cs" Inherits="SentinelFM.Admin.FirmwareMinder" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function gridBinding() {
            alert("gridBinding");
        }

        function dropDownSelected() {
            $('#bFetch').attr('disabled', '');
        }
        function dropDownSelecting() {
            $('#dFilter').hide();
            $('#dGrid').hide();
        }


        function GridCreated(sender, eventArgs) {
            $('#dLoad').hide();
            $('#dFilter').show();
            $('#dGrid').show();
        }

        function showLoading() {
            $('#dLoad').show();
        }

    </script>
    <style type="text/css">
        body
        {
            font-family: Tahoma;
            font-size: 10pt;
            color: Black;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadSkinManager ID="RadSkinManager1" runat="server" ShowChooser="False" Skin="Hay">
    </telerik:RadSkinManager>
   
    <div style="text-align: center; font-size: 16pt; font-weight: bold">
        Firmware Minder
        <br />
    </div>
    <div id="dOrg">
        <asp:Label ID="Label1" runat="server" Text="Organization" />
        <br />
        <telerik:RadComboBox ID="ddlOrg" runat="server" Width="320px" DropDownWidth="320px"
            EmptyMessage="Loading..." Height="320px" OnClientSelectedIndexChanged="dropDownSelected"
            OnClientSelectedIndexChanging="dropDownSelecting" MarkFirstMatch="True">
        </telerik:RadComboBox>
        <br />
        <br />
        <asp:Button ID="bFetch" runat="server" Text="Fetch Current Firmware Details" OnClick="bFetch_Click"
            Enabled="false" />
    </div>
    <div id="dLoad" style="display: none; height: 100px; text-align: center;">
        <img src="images/loading2.gif" alt="loading...." />
    </div>
    <div id="dFilter" style="display: none;">
        <br />
        <asp:Label ID="Label3" runat="server" Text="Filter"></asp:Label>
        <asp:RadioButton ID="rbMB" GroupName="FW" runat="server" Text="Mainboard" Checked="true" />
        <asp:RadioButton ID="rbUB" GroupName="FW" runat="server" Text="Upperboard" Checked="false" />
        <asp:RadioButton ID="rbAX" GroupName="FW" runat="server" Text="Both" Checked="false" />
        <br />
        <telerik:RadComboBox ID="ddlFilter" runat="server" Width="150px" DropDownWidth="170px"
            EmptyMessage="Loading...">
            <Items>
                <telerik:RadComboBoxItem Text="No Filter" Selected="True" />
                <telerik:RadComboBoxItem Text="Include Exact Matches" />
                <telerik:RadComboBoxItem Text="Include Partial Matches" />
                <telerik:RadComboBoxItem Text="Exclude Exact Matches" />
                <telerik:RadComboBoxItem Text="Exclude Partial Matches" />
            </Items>
        </telerik:RadComboBox>
        &nbsp;
        <telerik:RadTextBox ID="txtFW" runat="server" Width="80px" EmptyMessage="mainboard" >
        </telerik:RadTextBox>
                <telerik:RadTextBox ID="txtUB" runat="server" Width="80px"  EmptyMessage="upperboard">
        </telerik:RadTextBox>
        <br />
        <br />
        <asp:Button ID="bFilter" runat="server" Text="Filter Current Firmware Details" OnClick="bFilter_Click" />
        <br />
    </div>
    <div id="dGrid" style="display: none;">
        <br />
        <asp:Label ID="Label2" runat="server" Text="Total Units:"></asp:Label>
        <asp:Label ID="lTotalUnits" runat="server" Text="0" Style="color: Red;"></asp:Label>
        <br />
        <asp:Label ID="Label5" runat="server" Text="Reporting Units:"></asp:Label>
        <asp:Label ID="lReportingUnits" runat="server" Text="0" Style="color: Red;"></asp:Label>
        <br />
        <asp:Label ID="Label7" runat="server" Text="Filtered Results:"></asp:Label>
        <asp:Label ID="lFilteredUnits" runat="server" Text="0" Style="color: Red;"></asp:Label>
        <br />
        <br />
        <telerik:RadGrid ID="GridView" runat="server" AllowPaging="False" AllowSorting="True"
            ClientIDMode="AutoID" BorderStyle="Solid" OnPageIndexChanged="GridView_PageIndexChanged"
            OnPageSizeChanged="GridView_PageSizeChanged" OnSortCommand="GridView_SortCommand"
            ShowStatusBar="True">
            <ExportSettings Excel-Format="Html" ExportOnlyData="True"  IgnorePaging="True" Pdf-PageWidth="11" />
            <ClientSettings>
                <ClientEvents OnGridCreated="GridCreated" />
            </ClientSettings>
            <MasterTableView CommandItemSettings-ShowExportToCsvButton="True" CommandItemSettings-ShowExportToExcelButton="True" CommandItemSettings-ShowExportToPdfButton="False" CommandItemSettings-ShowAddNewRecordButton="False" CommandItemSettings-ShowExportToWordButton="True" CommandItemDisplay="Top">
                <CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>
                <RowIndicatorColumn>
                    <HeaderStyle Width="20px"></HeaderStyle>
                </RowIndicatorColumn>
                <ExpandCollapseColumn>
                    <HeaderStyle Width="20px"></HeaderStyle>
                </ExpandCollapseColumn>
            </MasterTableView>
            <HeaderStyle BackColor="#FFC700" />
        </telerik:RadGrid>
    </div>
    </form>
</body>
</html>
