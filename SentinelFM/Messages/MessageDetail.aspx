<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MessageDetail.aspx.cs" Inherits="SentinelFM.Messaging_Admin_MessageDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Message Detail</title>
    <link href="css/main.css" rel="stylesheet" type="text/css" />

    <script src="../Scripts/prototype.js" type="text/javascript"></script>

    <script type="text/javascript">

        function loader() {
            window.resizeTo($('MDDiv').scrollWidth + 7, $('MDDiv').scrollHeight + 83);
        };
    
    </script>

    <style type="text/css">
        body
        {
            font-family: Tahoma;
            font-size: x-small;
            margin: 0px;
        }
        .MDDiv
        {
            padding: 10px;
            width: 410px;
            height: 100px;
            border: 1px solid silver;
        }
    </style>
</head>
<body onload="loader();">
    <form id="MDForm" runat="server">
    <div id="MDDiv">
        <div class="MDDiv">
            <asp:Label ID="MDHeader" runat="server" CssClass="Shdr" Text="Message Details" />
            <br />
            <br />
            <asp:Label ID="MDTimestampLabel" CssClass="Scap" runat="server" Text="Timestamp"
                meta:resourcekey="MDTimestampLabel" />
            <br />
            <asp:Label ID="MDTimestampValue" CssClass="Swval" runat="server" Text="Mon, Jan 01, 2000 00:00 AM" />
            <br />
            <asp:Label ID="MDFromLabel" CssClass="Scap" runat="server" Text="From" meta:resourcekey="MDFromLabel" />
            <br />
            <asp:Label ID="MDFromValue" CssClass="Swval" runat="server" Text="Unknown" />
            <asp:Label ID="MDToDiv" runat="server" Style="display: block">
                <br />
                <asp:Label ID="MDToLabel" CssClass="Scap" runat="server" Text="To" meta:resourcekey="MDToLabel" />
                <br />
                <asp:Label ID="MDToValue" CssClass="Swval" runat="server" Text="Unknown" />
            </asp:Label>
            <br />
            <asp:Label ID="MDBodyLabel" CssClass="Scap" runat="server" Text="Body" meta:resourcekey="MDBodyLabel" />
            <asp:Label ID="MDBodyValue" CssClass="Swval" runat="server" Text="Empty" Style="height: 80px;" />
            <asp:Label ID="MDLocationDiv" runat="server" Style="display: block;">
                <br />
                <asp:Label ID="MDLocationLabel" CssClass="Scap" runat="server" Text="Location" meta:resourcekey="MDLocationLabel" />
                <br />
                <asp:Label ID="MDLandmarkValue" CssClass="Swval" runat="server" Text="Unknown" Style="display: none;" />
                <asp:Label ID="MDAddressValue" CssClass="Swval" runat="server" Text="Unknown" Style="display: none;" />
                <asp:Label ID="MDLatLonValue" CssClass="Swval" runat="server" Text="0.0000000000, 0.0000000000" Style="display: none;" />
            </asp:Label>
        </div>
        <asp:Label ID="MDStatusDiv" runat="server" class="MDDiv" Style="display: block; background-color: #f3fff3;
            border-top-style: none;">
            <asp:Label ID="MDStatusLabel" CssClass="Scap" runat="server" Text="Current Status"
                meta:resourcekey="MDStatusLabel" />
            <br />
            <asp:Label ID="MDStatusValue" CssClass="Swval" runat="server" Text="Message Queued" />
            <br />
            <asp:Label ID="MDStatusTimeLabel" CssClass="Scap" runat="server" Text="Status Timestamp"
                meta:resourcekey="MDStatusTimeLabel" />
            <br />
            <asp:Label ID="MDStatusTimeValue" CssClass="Swval" runat="server" Text="Mon, Jan 01, 2000 00:00 AM" />
        </asp:Label>
    </div>
    </form>
</body>
</html>
