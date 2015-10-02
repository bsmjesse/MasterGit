<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmhistorycrt.aspx.cs" Inherits="SentinelFM.History_New_frmhistorycrt" Theme="TelerikControl"  %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
<head id="Head1" runat="server">
<script language="JavaScript" src="../../Scripts/slider.js"></script>

    <title>History Criteria</title>
    <style type="text/css">
        .pageView
        {
            border: 1px solid #898c95;
            height:110px;
        }
        .tabAlignCenter
        {
            text-align:center;
        }
    </style>
    <script language="javascript">
			<!--
        function LoadFrames(menu, main) {
            if (menu != "")
                parent.menu.window.location = menu;
            if (main != "")
                parent.main.window.location = main;
        }

        function LoadHistoryGridData(vehiclePlate) {
            try {
                parent.frmHis.GenerateGridsDate(vehiclePlate);
            }
            catch (err) {
                eval("setTimeout(\"LoadHistoryGridData('" + vehiclePlate + "')\", 500);");
            }
        }

        //for Disable loading data when redirecting from map
        function SetSendCommandEvent(vehiclePlate) {
            try {
                parent.frmHis.SetSendCommandEvent(vehiclePlate);
            }
            catch (err) {
                eval("setTimeout(\"SetSendCommandEvent('" + vehiclePlate + "')\", 500);");
            }

        }

        function StreetLookup() {
            parent.window.location = "../../Reports/frmReportMasterExtendedExt.aspx";
            return false;
        }
        function MapOptions() {
            var mypage = '../frmhistoryMapsoluteLegend.aspx'
            var myname = '';
            var w = 230;
            var h = 200;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }


        function FullScreenGrid() {
            var mypage = '../../Map/frmBigDetailsFrame.htm'
            var myname = 'Data';
            var w = 900;
            var h = 700;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,resizable=1,toolbar=0,menubar=0,scrollbars=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }




        function FullScreenHistoryGrid() {
            var mypage = '../frmFullScreenHistGrid.aspx'
            var myname = 'Data';
            var w = 900;
            var h = 700;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,resizable=1,toolbar=0,menubar=0,scrollbars=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }

        function SensorsInfo(LicensePlate) {
            var mypage = '../../Map/frmSensorMain.aspx?LicensePlate=' + LicensePlate
            var myname = 'Sensors';
            var w = 460;
            var h = 720;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }




        var browserTypes = { "MSIE8": 0, "MSIE7": 1, "MSIE6": 2, "Gecko": 3, "WebKit": 4 };
        var browserType = browserTypes.MSIE;



        var rex = /AppleWebKit\/\d{1,3}\.\d{1,3}/
        var match = navigator.userAgent.match(rex);
        if (match)
            browserType = browserTypes.WebKit;

        rex = /Gecko\/\d*\s/
        match = navigator.userAgent.match(rex);
        if (match)
            browserType = browserTypes.Gecko;


        rex = /MSIE\s*\d{1,2}\.\d{1,3};/
        match = navigator.userAgent.match(rex);
        if (match && match[0]) {
            if (parseInt(match[0].split(' ')[1]) === 8) browserType = browserTypes.MSIE8;
            if (parseInt(match[0].split(' ')[1]) === 7) browserType = browserTypes.MSIE7;
            if (parseInt(match[0].split(' ')[1]) === 6) browserType = browserTypes.MSIE6;
        }



        function CalendarView(hideFields) {

            if (hideFields) {
                document.getElementById("cboHoursTo").style.display = "none";
                document.getElementById("webtab").style.display = "none";
                document.getElementById("cboHistoryType").style.display = "none";
                document.getElementById("cboFleet").style.display = "none";
                document.getElementById("cboVehicle").style.display = "none";
                if (document.getElementById("cboCommMode") != null)
                    document.getElementById("cboCommMode").style.display = "none";

            }
            else {
                document.getElementById("cboHoursTo").style.display = "inline-block";
                document.getElementById("webtab").style.display = "inline-block";
                document.getElementById("cboHistoryType").style.display = "inline-block";
                document.getElementById("cboFleet").style.display = "inline-block";
                document.getElementById("cboVehicle").style.display = "inline-block";
                if (document.getElementById("cboCommMode") != null)
                    document.getElementById("cboCommMode").style.display = "inline-block";
            }

        };

        function calendarClick(hideFields) {

            switch (browserType) {
                /*
                case browserTypes.Gecko:
                break;
                case browserTypes.WebKit:
                Element.setStyle($("obcall"), { left: 87 + "px", top: -22 + "px" });
                break;
                case browserTypes.MSIE7:
                Element.setStyle($("obcall"), { left: 87 + "px", top: -21 + "px" });
                break;
                */ 
                case browserTypes.MSIE6:
                    CalendarView(hideFields);
                    break;
            }


        }
        
				
			//-->
    </script>
   
    </head>
<body leftmargin="0" topmargin=0 rightmargin=0 bottommargin=0   >
<%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
  {%>
  
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>     

    <script language="javascript">
		<!--
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = '<%=DefaultOrganizationHierarchyFleetId %>';
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        
        var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
       
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
            if (MutipleUserHierarchyAssignment) {
                mypage = mypage + "&m=1&f=0";
            }
			var myname='OrganizationHierarchy';
			var w=740;
			var h=440;
			var winl = (screen.width - w) / 2; 
			var wint = (screen.height - h) / 2; 
			winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
			win = window.open(mypage, myname, winprops) 
			if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }  
            return false;
        }

        function OrganizationHierarchyNodeSelected(nodecode, fleetId, fleetName)
        {
            var myVal = document.getElementById('<%=valVehicle.ClientID %>');
            ValidatorEnable(myVal, false); 

            
            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
            $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
            $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
        }
      
			//-->
    </script>
<%} %>
 

    <form id="frmHistoryCriteria" method="post" runat="server">
    <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />
    
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
 <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
         EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboFleet">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlALl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboHistoryType">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlALl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboVehicle">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlALl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cmdShowHistory">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlALl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <asp:Panel ID="pnlALl" runat="server" >
       <table cellspacing="0"  border="0" style="width: 255px;" cellpadding="0"  >
          <tr>
             <td>
                    <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1" Text="From :"></asp:Label></td>
              <td ><telerik:RadDatePicker  ID="txtFrom" runat="server" Width="100px"  Height="17px"  Calendar-ShowRowHeaders="false" Calendar-Width="170px"
                       meta:resourcekey="txtFromResource2"  MaxDate="12/31/9998 23:59:59" 
                      MinDate="1753-01-01" >
             </telerik:RadDatePicker >
             </td>
             <td><asp:DropDownList ID="cboHoursFrom" runat="server"  CssClass="RegularText" meta:resourcekey="cboHoursFromResource1"  >
             </asp:DropDownList>
             </td><td>
             <asp:DropDownList ID="cboMinutesFrom" runat="server"  CssClass="RegularText"  >
             </asp:DropDownList>             
             </td>
          </tr>
          <tr>
             <td class="style1" >
                    <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1" Text="To:"></asp:Label></td>
             <td ><telerik:RadDatePicker ID="txtTo" runat="server" Width="100px"  Height="17px"  Calendar-ShowRowHeaders="false" Calendar-Width="170px" 
                      meta:resourcekey="txtToResource2" MaxDate="12/31/9998 23:59:59" 
                     MinDate="1753-01-01"   >

             </telerik:RadDatePicker>
             </td>
             <td ><asp:DropDownList ID="cboHoursTo" runat="server"  CssClass="RegularText" meta:resourcekey="cboHoursToResource1"  >
             </asp:DropDownList>
             </td><td>
             <asp:DropDownList ID="cboMinutesTo" runat="server"  CssClass="RegularText"  >
             </asp:DropDownList>
             </td>
          </tr>
          <tr>
             <td class="style1">
                    <asp:Label ID="lblHistoryTypeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblHistoryTypeTitleResource1" Text="Type:"></asp:Label></td>
             <td colspan="3">
                    <asp:DropDownList ID="cboHistoryType" runat="server" CssClass="formtext" AutoPostBack="True"
                        Width="100%" OnSelectedIndexChanged="cboHistoryType_SelectedIndexChanged" meta:resourcekey="cboHistoryTypeResource1">
                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource1" Text="Vehicle Path"></asp:ListItem>
                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="Stop and Idle Sequence "></asp:ListItem>
                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="Stop Sequence"></asp:ListItem>
                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource4" Text="Idle Sequence"></asp:ListItem>
                        <asp:ListItem Value="4" meta:resourcekey="ListItemResource24">Trip Report</asp:ListItem>
                    </asp:DropDownList></td>
          </tr>
          <tr>
             <td class="style1">
                    <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1" Text="Fleet:"></asp:Label>                    
                    <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" Text=" Hierarchy Node:" meta:resourcekey="lblOhTitle" Visible="false"  /></td>
             <td colspan="3" >
                    <asp:DropDownList ID="cboFleet" runat="server" CssClass="formtext" AutoPostBack="True"
                        DataTextField="FleetName" DataValueField="FleetId" Width="100%" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1">
                    </asp:DropDownList>
                    <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" Text="" CssClass="combutton" Visible="false" OnClientClick="return onOrganizationHierarchyNodeCodeClick();" />
             </td>
          </tr>
          <tr>
             <td class="style1" >
                    <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" 
                        meta:resourcekey="lblVehicleNameResource1" Text="Vehicle:"></asp:Label><asp:RangeValidator
                        ID="valVehicle" runat="server" CssClass="formtext" ControlToValidate="cboVehicle"
                        ErrorMessage="Please select a Vehicle" MinimumValue="0"
                        MaximumValue="999999999999999" meta:resourcekey="valVehicleResource1" 
                        Text="*"></asp:RangeValidator>
                 </td>
             <td colspan="3">
                    <asp:DropDownList ID="cboVehicle" runat="server" CssClass="formtext" AutoPostBack="True"
                        DataTextField="Description" DataValueField="VehicleId" Width="100%"
                        OnSelectedIndexChanged="cboVehicle_SelectedIndexChanged" 
                        meta:resourcekey="cboVehicleResource1">
                    </asp:DropDownList>                    
             </td>
          </tr>
          <tr>
             <td class="style1">
                    <asp:Label ID="lblCommMode"  runat="server" Font-Bold="False" Text="Comm:"
                        Visible="False" meta:resourcekey="lblCommModeResource1" CssClass="formtext"></asp:Label></td>
             <td colspan="3">
                    <asp:DropDownList ID="cboCommMode" runat="server" CssClass="formtext" AutoPostBack="True"
                        Width="100%" OnSelectedIndexChanged="cboHistoryType_SelectedIndexChanged" DataTextField="CommModeName"
                        DataValueField="DclId" Visible="False" meta:resourcekey="cboCommModeResource1">
                    </asp:DropDownList></td>
          </tr>
          <tr>
             <td colspan="4">
                    <table id="tblStopReport" cellspacing="0" cellpadding="0"
                         border="0" runat="server">
                        <tr>
                            <td class="formtext" >
                                <asp:Label ID="lblStopDurationTitle" runat="server" CssClass="formtext" meta:resourcekey="lblStopDurationTitleResource1" Font-Bold="False" Text="Stop Duration:" Visible="False"></asp:Label></td>
                        </tr>
                        <tr>
                            <td  >
                                <asp:DropDownList ID="cboStopSequence" runat="server" Width="100%" CssClass="formtext" meta:resourcekey="cboStopSequenceResource1" Visible="False">
                                    <asp:ListItem Value="0" meta:resourcekey="ListItemResource5" Text="Not Filtered"></asp:ListItem>
                                    <asp:ListItem Value="300" meta:resourcekey="ListItemResource6" Text="Longer than 5 Min"></asp:ListItem>
                                    <asp:ListItem Value="600" meta:resourcekey="ListItemResource7" Text="Longer than 10 Min"></asp:ListItem>
                                    <asp:ListItem Value="900" meta:resourcekey="ListItemResource8" Text="Longer than 15 Min"></asp:ListItem>
                                    <asp:ListItem Value="1800" meta:resourcekey="ListItemResource9" Text="Longer than 30 Min"></asp:ListItem>
                                    <asp:ListItem Value="2700" meta:resourcekey="ListItemResource10" Text="Longer than 45 Min"></asp:ListItem>
                                    <asp:ListItem Value="3600" meta:resourcekey="ListItemResource11" Text="Longer than 1 Hour"></asp:ListItem>
                                    <asp:ListItem Value="7200" meta:resourcekey="ListItemResource12" Text="Longer than 2 Hours"></asp:ListItem>
                                </asp:DropDownList></td>
                        </tr>
                        
                        
                        
                       
                        
                    </table>
             </td>
          </tr>
          <tr>
             <td colspan="4" valign=top>
                <div id="webtab"  style="display:inline-block; Height:143px">
                 <telerik:RadTabStrip ID="WebHistoryTab" style="z-index: 5"  runat="server"   MultiPageID="RadMultiPage1" 
                     Width="100%" meta:resourcekey="WebHistoryTabResource1" SelectedIndex="0"  Skin="Hay"   >
                     <Tabs>
                        <telerik:RadTab runat="server"  Value="Messages" Text='<%$ Resources:WebHistoryTab_Messages %>' PageViewID="Messages"  cs    >
                        </telerik:RadTab>
                        <telerik:RadTab  runat="server" Value="Location" Text='<%$ Resources:WebHistoryTab_Location %>' PageViewID="Location" >
                        </telerik:RadTab>
                        <telerik:RadTab  runat="server" Value="Trip" Text='<%$ Resources:WebHistoryTab_Trip %>' PageViewID="Trip"  >
                        </telerik:RadTab>
                     </Tabs>
                 </telerik:RadTabStrip>
                 <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" CssClass="pageView" BackColor="Gainsboro" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" >
                    <telerik:RadPageView ID = "Messages" runat="server"  >
<table width="100%">
                                     <tr>
                                         <td >
                 <asp:CheckBox ID="chkLatestMsg" runat="server" CssClass="formtext" Text="Last message only"
                     TextAlign="Left" meta:resourcekey="chkLatestMsgResource1" /></td>
                                     </tr>
                                     <tr>
                                         <td >
                 <asp:ListBox ID="lstMsgTypes" runat="server" CssClass="formtext" DataTextField="BoxMsgInTypeName"
                     DataValueField="BoxMsgInTypeId" SelectionMode="Multiple" Width="100%" Height="80px" meta:resourcekey="lstMsgTypesResource1"></asp:ListBox></td>
                                     </tr>
                                 </table>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID = "Location" runat="server"  >
<table class="formtext">
                                     <tr>
                                         <td >
                                             <asp:Label ID="lblAddress" runat="server" Text="Address:" 
                                                 meta:resourcekey="lblAddressResource1"></asp:Label></td>
                                         <td >
                                             <asp:TextBox ID="txtAddress" runat="server" Width="170px" 
                                                 meta:resourcekey="txtAddressResource1"></asp:TextBox></td>
                                     </tr>
                                 </table>
                    </telerik:RadPageView>
                    <telerik:RadPageView ID = "Trip" runat="server" >
<table id="tblIgnition" runat="server" border="0" cellpadding="0" 
                                     cellspacing="0">
                                                    <tr id="Tr1" runat="server">
                                                        <td id="Td1" class="formtext" align="left" runat="server">
                                                            <table  class="formtext">
                                                                <tr>
                                                                    <td class="formtext">
                                                                        <asp:Label ID="lblTripCalculation" runat="server" 
                                                                            Text='<%$ Resources:lblTripCalculation %>'></asp:Label></td>
                                                                </tr>
                                                                <tr>
                                                                    <td align="left">
                                                                        <asp:RadioButtonList ID="optEndTrip" runat="server" CssClass="formtext" >
                                                                            <asp:ListItem Selected="True" Text='<%$ Resources:lblTripIgnition %>' Value="3"></asp:ListItem>
                                                                            <asp:ListItem Text='<%$ Resources:lblTripTractorPower %>' Value="11" ></asp:ListItem>
                                                                            <asp:ListItem Value="8" Text='<%$ Resources:lblTripPTO %>' ></asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                    </telerik:RadPageView>
                 </telerik:RadMultiPage>
                 </div>
             </td>
          </tr>
          
          <tr>
             <td align="center" colspan="4" >

                <asp:Button ID="cmdShowHistory" runat="server" CssClass="combutton" meta:resourcekey="cmdShowHistoryResource1"
                   OnClick="cmdShowHistory_Click" Text="View" />
                <asp:Button  ID="cmdStreetLookup" runat="server" CssClass="combutton" meta:resourcekey="cmdStreetLookupResource1"
                OnClientClick="return StreetLookup();" CausesValidation="false"
                    Text="Street Lookup" />
                   </td>
          </tr>
        
          
          <tr>
             <td align="center" colspan="4" rowspan="1">
                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1"
                   Width="100%" Font-Size="XX-Small" />
                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" meta:resourcekey="lblMessageResource1"
                   Visible="False" Width="100%" Font-Size="XX-Small"></asp:Label></td>
          </tr>
          
          <tr>
             <td align="center" colspan="4" >
                                 <asp:HyperLink ID="lnkMapLegends"  NavigateUrl="javascript:MapOptions()"  runat="server" Font-Bold="False" CssClass="formtext" Font-Underline="True" Visible="False" Text='<%$ Resources:lnkMapLegendsResource1 %>'  ></asp:HyperLink>
                              
                </td>
             
          </tr>
       </table>
       <%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
          {%>        
            <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" style="display:none;" AutoPostBack="True"
            OnClick="hidOrganizationHierarchyPostBack_Click" />
        <%} %>
  </asp:Panel>




    </form>
   </body>
</html>
