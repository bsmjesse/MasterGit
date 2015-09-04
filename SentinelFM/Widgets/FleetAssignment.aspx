<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FleetAssignment.aspx.cs" Inherits="SentinelFM.Widgets_FleetAssignment" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <style type="text/css">
        body 
        {
            background: none repeat scroll 0 0 #FFFFFF;
        }
        .DataHeaderStyle, .DataHeaderStyle td
            {
	            background-color:#009933 !important;
	            font-size:11px !important;
	            font-family: Arial,Helvetica,sans-serif !important;
	            FONT-WEIGHT:bold !important;
	            color:White !important;
	            padding-left: 5px;
                                                                            
            }
        .gridtext
        {
	        font-weight: normal;
	        font-size: 11px;
	        color: #000000;
	        font-family: verdana;
	        text-decoration: none;
        }
            
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
            text-decoration: none;
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
        function closepopup() {
            if (parent.popupWindow == undefined) {
                parent.closeFleetAssignment();
            }
            else {
                parent.popupWindow.close();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="height:190px;overflow:auto;width:320;margin:10px;">
        <asp:CheckBoxList ID="CheckBoxFleet" runat="server" Height="170px" 
            DataTextField="FleetName" DataValueField="FleetId" Width="300px" 
            Visible="False" meta:resourcekey="CheckBoxFleetResource1">
        </asp:CheckBoxList>

        <asp:DataGrid ID="AssignedFleets" runat="server" Width="100%" GridLines="None" CellPadding="3"
                                                                        
            BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White"
                                                                        
            OnDeleteCommand="AssignedFleets_DeleteCommand" DataKeyField="FleetId" 
            AutoGenerateColumns="False" BorderStyle="Ridge" meta:resourcekey="AssignedFleetsResource1"                                                                        
                                                                        >
            <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
            <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
            <Columns>
                <asp:BoundColumn DataField="FleetName" HeaderText="Assigned Fleets">    
                                <HeaderStyle Wrap="False"></HeaderStyle>
                </asp:BoundColumn>
                <asp:ButtonColumn Text="X" CommandName="Delete"
                                                                                meta:resourcekey="ButtonColumnResource2"></asp:ButtonColumn>
                
                
            </Columns>
        </asp:DataGrid>
    </div>
    <asp:Label ID="lblMessage" runat="server" Visible="False" 
        meta:resourcekey="lblMessageResource1"></asp:Label><br />
    <div id="footer" style="padding-left:20px;">
    <%--<asp:Button ID="Button1" runat="server" Text="Save" onclick="Button1_Click" class="kd-button" style="margin-top: 10px;margin-left:20px;" />--%>
    <%--<asp:Label ID="Label1" runat="server" Text="Add a fleet: "></asp:Label>--%>
    <asp:DropDownList ID="ddFleet" DataTextField="FleetName" 
        DataValueField="FleetId" runat="server" AutoPostBack="True"
        onselectedindexchanged="ddFleet_SelectedIndexChanged" 
            meta:resourcekey="ddFleetResource1">
    </asp:DropDownList>
    <%--<asp:Button ID="Button2" runat="server" Text="Close" OnClientClick="closepopup();" class="kd-button" style="margin-top: 10px;margin-left:20px;" />--%>
    <a href="javascript:void(0)" onclick="closepopup();" class="kd-button" style="margin-top: 10px;margin-left:20px;">Close</a>
    </div>
    </form>
</body>
</html>
