<%@ Page Title="" Language="C#" MasterPageFile="~/ScheduleAdherence/SAMasterPage.master" AutoEventWireup="true" CodeFile="frmStation.aspx.cs" Inherits="ScheduleAdherence_frmStation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">   
          .cssPager span,a { font-size:11px;}    
     </style>
    <script type="text/javascript" src="JS/frmStationList.js"></script>
    <script type="text/javascript">
        var IDHead = "ctl00_ContentPlaceHolder1_";
        function EditStation(stationId) {
            document.getElementById(IDHead + "hdStation").value = stationId;
        }

        function DeleteStation(stationId, name) {
            if (!confirm("Are you sure you want to delete " + name + "?"))
                return false;
            document.getElementById(IDHead + "hdStation").value = stationId;
            return true;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:HiddenField ID="hdStation" runat="server" />
<table width="100%" class="formtext">
    <tr><td><asp:Label ID="lbError" runat="server" CssClass="errortext"/></td></tr>
    <tr><td>
        <asp:GridView ID="gvStation" runat="server" Width="900" AutoGenerateColumns="false"
            onrowcommand="gvStation_RowCommand" onrowdatabound="gvStation_RowDataBound"
            onpageindexchanging="gvStation_PageIndexChanging"
            GridLines="None" CellPadding="3" BackColor="White" BorderWidth="2px" BorderStyle="Ridge"
            CellSpacing="1" BorderColor="White" PageSize="13" AllowPaging="true">
            <EmptyDataTemplate>No Data.</EmptyDataTemplate>
            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
            <SelectedRowStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedRowStyle>
            <AlternatingRowStyle CssClass="gridtext" BackColor="Beige"></AlternatingRowStyle>
            <RowStyle CssClass="gridtext" BackColor="White"></RowStyle>
            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
            <PagerSettings Mode="NumericFirstLast" />
            <PagerStyle CssClass="cssPager" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"></PagerStyle>
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Name" />
                <asp:BoundField DataField="LandMarkName" HeaderText="Landmark Name" />
                <asp:BoundField DataField="TypeName" HeaderText="Type" />
                <asp:BoundField DataField="StationNumber" HeaderText="Station Number" />
                <asp:BoundField DataField="ContractName" HeaderText="Contact" />
                <asp:BoundField DataField="PhoneNumber" HeaderText="Phone Number" />
                <asp:BoundField DataField="FaxNumber" HeaderText="Fax Number" />
                <asp:BoundField DataField="Address" HeaderText="Address" />
                <asp:ButtonField  ButtonType="Image"  ImageUrl="../images/edit.gif" CommandName="cmdEdit" />
                <asp:ButtonField ButtonType="Image" ImageUrl="../images/delete.gif" CommandName="cmdDelete" />
            </Columns>
        </asp:GridView>
    </td></tr>
    <tr><td height="20"></td></tr>
    <tr><td align="center">
        <asp:button id="cmdAddStation" runat="server" Width="127px" CssClass="combutton" Text="Add Station" onclick="cmdAddStation_Click" />
    </td></tr>
</table>
</asp:Content>

