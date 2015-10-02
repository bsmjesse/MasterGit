<%@ Page Language="c#" Inherits="SentinelFM.frmHistoryCrt" CodeFile="frmHistoryCrt.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>

<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
   TagPrefix="ISWebInput" %>

<%@ Register Assembly="ISNet.WebUI.WebCombo" Namespace="ISNet.WebUI.WebCombo" TagPrefix="ISWebCombo" %>





<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
<script language="JavaScript" src="../Scripts/slider.js"></script>

    <title>History Criteria</title>

    <script language="javascript">
			<!--
			function LoadFrames(menu,main)
			{	
				if (menu !="")
				parent.menu.window.location=menu;
				if (main !="")
				parent.main.window.location=main;
			}
			
			
				function MapOptions() { 
					var mypage='frmhistoryMapsoluteLegend.aspx'
					var myname='';
					var w=230;
					var h=200;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
				function FullScreenGrid() { 
					var mypage='../Map/frmBigDetailsFrame.htm'
					var myname='Data';
					var w=900;
					var h=700;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,resizable=1,toolbar=0,menubar=0,scrollbars=1' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}		
				
				
				
				
				function FullScreenHistoryGrid() { 
					var mypage='frmFullScreenHistGrid.aspx'
					var myname='Data';
					var w=900;
					var h=700;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,resizable=1,toolbar=0,menubar=0,scrollbars=1' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}		
				
				function SensorsInfo(LicensePlate) { 
					var mypage='../Map/frmSensorMain.aspx?LicensePlate='+LicensePlate
					var myname='Sensors';
					var w=460;
					var h=720;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
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
            
           if (hideFields) 
             {
                 document.getElementById("cboHoursTo").style.display = "none";
                 document.getElementById("webtab").style.display = "none";
                 document.getElementById("cboHistoryType").style.display = "none";
                 document.getElementById("cboFleet").style.display = "none";
                 document.getElementById("cboVehicle").style.display = "none";
                 if  (document.getElementById("cboCommMode")!=null) 
                    document.getElementById("cboCommMode").style.display = "none";
                 
            }
            else
            {
                document.getElementById("cboHoursTo").style.display = "inline-block";
                document.getElementById("webtab").style.display = "inline-block";
                document.getElementById("cboHistoryType").style.display = "inline-block";
                document.getElementById("cboFleet").style.display = "inline-block";
                document.getElementById("cboVehicle").style.display = "inline-block";
                 if  (document.getElementById("cboCommMode")!=null) 
                    document.getElementById("cboCommMode").style.display = "inline-block";
            }

        };
        
        function calendarClick(hideFields)
        {

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
    <form id="frmHistoryCriteria" method="post" runat="server">
       <table border=0 style="width: 250px;">
          <tr>
             <td>
                    <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1" Text="From :"></asp:Label></td>
              <td ><ISWebInput:WebInput ID="txtFrom" runat="server" Width="120px"  Height="17px"  
                      Wrap="Off" meta:resourcekey="txtFromResource2" MaxDate="12/31/9998 23:59:59" 
                      MinDate="1753-01-01"  >

<CultureInfo CultureName="en-US"></CultureInfo>

<DisplayFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</DisplayFormat>
                    
                      <ClientSideEvents OnAfterValueChanged="calendarClick(false);" 
                      OnBlur="calendarClick(false);">
                   
                     <DropDownButtonEditorEvents OnClick="calendarClick(true)" />
                 </ClientSideEvents>
                 
                 
                <HighLight IsEnabled="True" Type="Phrase" />
<EditFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</EditFormat>

                <DateTimeEditor IsEnabled="True" AccessKey="Space" >
                
                </DateTimeEditor>
             </ISWebInput:WebInput>
             </td>
             <td><asp:DropDownList ID="cboHoursFrom" runat="server"  CssClass="RegularText" meta:resourcekey="cboHoursFromResource1"  >
             </asp:DropDownList></td>
          </tr>
          <tr>
             <td class="style1" >
                    <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1" Text="To:"></asp:Label></td>
             <td ><ISWebInput:WebInput ID="txtTo" runat="server" Width="120px" Height="17px"  
                     Wrap="Off" meta:resourcekey="txtToResource2" MaxDate="12/31/9998 23:59:59" 
                     MinDate="1753-01-01"  >

<CultureInfo CultureName="en-US"></CultureInfo>

<DisplayFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</DisplayFormat>

                   <ClientSideEvents OnAfterValueChanged="calendarClick(false);" 
                      OnBlur="calendarClick(false);">
                   
                     <DropDownButtonEditorEvents OnClick="calendarClick(true)" />
                 </ClientSideEvents>
                 
                 

                <HighLight IsEnabled="True" Type="Phrase" />
<EditFormat>
<ErrorWindowInfo IsEnabled="False"></ErrorWindowInfo>
</EditFormat>

                <DateTimeEditor IsEnabled="True" AccessKey="Space">
                </DateTimeEditor>
             </ISWebInput:WebInput>
             </td>
             <td ><asp:DropDownList ID="cboHoursTo" runat="server"  CssClass="RegularText" meta:resourcekey="cboHoursToResource1"  >
             </asp:DropDownList></td>
          </tr>
          <tr>
             <td class="style1">
                    <asp:Label ID="lblHistoryTypeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblHistoryTypeTitleResource1" Text="Type:"></asp:Label></td>
             <td colspan="2">
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
                    </td>
             <td colspan="2" >
                    <asp:DropDownList ID="cboFleet" runat="server" CssClass="formtext" AutoPostBack="True"
                        DataTextField="FleetName" DataValueField="FleetId" Width="100%" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1">
                    </asp:DropDownList></td>
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
             <td colspan="2">
                    <asp:DropDownList ID="cboVehicle" runat="server" CssClass="formtext" AutoPostBack="True"
                        DataTextField="Description" DataValueField="VehicleId" Width="100%"
                        OnSelectedIndexChanged="cboVehicle_SelectedIndexChanged" 
                        meta:resourcekey="cboVehicleResource1">
                    </asp:DropDownList></td>
          </tr>
          <tr>
             <td class="style1">
                    <asp:Label ID="lblCommMode"  runat="server" Font-Bold="False" Text="Comm:"
                        Visible="False" meta:resourcekey="lblCommModeResource1" CssClass="formtext"></asp:Label></td>
             <td colspan="2">
                    <asp:DropDownList ID="cboCommMode" runat="server" CssClass="formtext" AutoPostBack="True"
                        Width="100%" OnSelectedIndexChanged="cboHistoryType_SelectedIndexChanged" DataTextField="CommModeName"
                        DataValueField="DclId" Visible="False" meta:resourcekey="cboCommModeResource1">
                    </asp:DropDownList></td>
          </tr>
          <tr>
             <td colspan="3">
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
             <td colspan="3" valign=top>
                <div id="webtab"  style="display:inline-block;">
                 <ISWebDesktop:WebTab ID="WebHistoryTab" style="z-index: 5"  runat="server" Height="143px" 
                     Width="100%" meta:resourcekey="WebHistoryTabResource1" ActiveTabIndex="0">
                     <FrameStyle Overflow="Hidden"  OverflowX="Hidden" OverflowY="Hidden">
                     </FrameStyle>
                     <ContainerStyle BackColor="WhiteSmoke" BorderColor="Navy" BorderStyle="Solid" BorderWidth="1px"
                         Height="100%" Overflow="Auto" OverflowX="Auto" OverflowY="Auto" Width="100%">
                         <Padding Bottom="1px" Left="1px" Right="1px" Top="1px" />
                     </ContainerStyle>
                     <RoundCornerSettings FillerBorderColor="255, 199, 60" TopBorderColor="230, 139, 44" />
                     <DisabledStyle ForeColor="Gray">
                     </DisabledStyle>
                     <TabPages>
                         <ISWebDesktop:WebTabItem Name="Messages" Text='<%$ Resources:WebHistoryTab_Messages %>'>
                             <PageTemplate>
                                 <table width="95%">
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
                             </PageTemplate>
                         </ISWebDesktop:WebTabItem>
                         <ISWebDesktop:WebTabItem Name="Location" Text='<%$ Resources:WebHistoryTab_Location %>'>
                             <PageTemplate>
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
                             </PageTemplate>
                         </ISWebDesktop:WebTabItem>
                         <ISWebDesktop:WebTabItem Name="Trip" Text='<%$ Resources:WebHistoryTab_Trip %>'>
                             <PageTemplate>
                                
                                <table id="tblIgnition" runat="server" border="0" cellpadding="0" 
                                     cellspacing="0">
                                                    <tr runat="server">
                                                        <td class="formtext" colspan="2" align="left" runat="server">
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
                                                
                             </PageTemplate>
                         </ISWebDesktop:WebTabItem>
                     </TabPages>
                     <TabItemStyle>
                         <Normal BackColor="Gainsboro" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px"
                             Cursor="Hand" Font-Bold="False" Font-Italic="False" Font-Names="Tahoma" Font-Size="8pt"
                             Font-Underline="False" Height="100%" Width="100%">
                             <Padding Bottom="0px" Left="10px" Right="10px" Top="2px" />
                         </Normal>
                         <Over BackColor="WhiteSmoke" BaseStyle="Normal">
                         </Over>
                         <Active BackColor="White" BackColor2="WhiteSmoke" BaseStyle="Normal" BorderColor="Navy"
                             BorderStyle="Solid" BorderWidth="1px" GradientType="Vertical">
                         </Active>
                     </TabItemStyle>
                 </ISWebDesktop:WebTab>
                 </div>
             </td>
          </tr>
          
          <tr>
             <td align="center" colspan="3" >

                <asp:Button ID="cmdShowHistory" runat="server" CssClass="combutton" meta:resourcekey="cmdShowHistoryResource1"
                   OnClick="cmdShowHistory_Click" Text="View" />
                   
                   </td>
          </tr>
        
          
          <tr>
             <td align="center" colspan="3" rowspan="1">
                <asp:ValidationSummary ID="valSummary" runat="server" CssClass="errortext" meta:resourcekey="valSummaryResource1"
                   Width="100%" Font-Size="XX-Small" />
                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" meta:resourcekey="lblMessageResource1"
                   Visible="False" Width="100%" Font-Size="XX-Small"></asp:Label></td>
          </tr>
          
          <tr>
             <td align="center" colspan="3" >
                               <asp:HyperLink ID="lnkMapLegends"  NavigateUrl="javascript:MapOptions()"  runat="server" Font-Bold="False" CssClass="formtext" Font-Underline="True" Visible="False" meta:resourcekey="lnkMapLegendsResource1" Text="Map Legends"></asp:HyperLink>
                              
                </td>
             
          </tr>
       </table>
  
    </form>
   </body>
</html>
