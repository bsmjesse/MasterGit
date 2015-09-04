<%@ Page Language="C#" AutoEventWireup="true" CodeFile="fleet.aspx.cs" Inherits="SentinelFM.Widgets_fleet" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <style type="text/css">
        .kd-button {
            -moz-transition: all 0.218s ease 0s;
            background-color: #F5F5F5;
            background-image: -moz-linear-gradient(center top , #F5F5F5, #F1F1F1);
            /*border: 1px solid rgba(0, 0, 0, 0.1);*/
            border: 1px solid #D8D8D8;
            border-radius: 2px 2px 2px 2px;
    
            color: #444444;
            display: inline-block;
            font-size: 100%;
            font-family: arial,​sans-serif;
            font-weight: bold;
            height: 27px;
            line-height: 27px;
            min-width: 54px;
            padding: 0 8px;
            text-align: center;
        }

        .kd-button:hover {
            -moz-transition: all 0s ease 0s;
            background-color: #F8F8F8;
            background-image: -moz-linear-gradient(center top , #F8F8F8, #F1F1F1);
            border: 1px solid #C6C6C6;
            box-shadow: 0 1px 1px rgba(0, 0, 0, 0.1);
            color: #333333;
            text-decoration: none;
        }

        .kd-button-disabled, .kd-button-disabled:hover {
            -moz-transition: all 0.218s ease 0s !important;
            background-color: #F5F5F5 !important;
            background-image: -moz-linear-gradient(center top , #F5F5F5, #F1F1F1) !important;
            border: 1px solid #D8D8D8 !important;
            border-radius: 2px 2px 2px 2px !important;
            color: #cccccc !important;
            font-weight: normal !important;
            box-shadow: none !important;        
        }        
    </style>

    <script type="text/javascript">
        function applyFleet(c) {
            var fid = $('#' + cboFleetId).val();
            if (fid == "-1" && c) return;
            if (c) {
                var fleetName = $("#" + cboFleetId + " option:selected").text();
                selectedFleetId = fid;
                parent.OnFleetSelect(c, selectedFleetId, fleetName, caller);
            }
            else {
                $('#' + cboFleetId).val(selectedFleetId);
                parent.OnFleetSelect(c);
            }            

        }
    </script>
</head>
<body>
    <script type="text/javascript">
        var cboFleetId = '<%=cboFleet.ClientID %>';
        var selectedFleetId = '<%=FleetId %>';
        var caller = '<%=Caller %>';
    </script>

    <form id="form1" runat="server">
    
    <div>
        <!--Select a fleet:<br />-->
        <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" Width="258px" 
                DataTextField="FleetName" DataValueField="FleetId" Skin="Hay" MaxHeight="300px" 
                meta:resourcekey="cboFleetResource1">
        </asp:DropDownList>
    </div>

    <div style="margin-top:20px;">        
        <asp:Button ID="Button3" runat="server" CssClass="kd-button" 
            OnClientClick="applyFleet(true);return false;" Text="OK" 
            meta:resourcekey="Button3Resource1" />

        <asp:Button ID="Button4" runat="server" CssClass="kd-button" 
            OnClientClick="applyFleet(false);return false;" Text="Cancel" 
            meta:resourcekey="Button4Resource1"  />
    </div>
    </form>
</body>
</html>
