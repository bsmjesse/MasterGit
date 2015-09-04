<%@ Page language="c#" Inherits="SentinelFM.frmPreference" CodeFile="frmPreference.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >




<HTML>
	<head runat="server">
		<title>Configuration</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
		<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script> 

        <script type="text/javascript" src="../scripts/jquery.cookie.js"></script>
        <script src="../reports/jqueryFileTree.js?v=20140220" type="text/javascript"></script>
        <script src="../reports/splitter.js" type="text/javascript"></script>
        <script src="../scripts/tablesorter/jquery.tablesorter.min.js" type="text/javascript"></script>
        <script src="../scripts/colResizable-1.3.min.js" type="text/javascript"></script>
        <script src="../scripts/json2.js" type="text/javascript"></script>
        
        <link rel="stylesheet" href="../scripts/tablesorter/themes/report/style.css" type="text/css" />
	    <link href="../reports/jqueryFileTree.css?v=20140220" rel="stylesheet" type="text/css" media="screen" />

        <style type="text/css">
                    
            .LeftPane {
                background: none repeat scroll 0 0 #FFFFFF;
                border-color: #BBBBBB #FFFFFF #FFFFFF #BBBBBB;
                border-style: solid;
                border-width: 1px;
                height: 300px;
                overflow: scroll;
                padding: 5px;
                width: 300px;
            }
        
            #vehicletreeview, #vehicledetails
            {
                padding: 0 5px;
            }
        
            #vehicledetails
            {
                /*width:450px;*/
                margin-right:16px;
            }
        
            /*
             * Splitter container. Set this to the desired width and height
             * of the combined left and right panes. In this example, the
             * height is fixed and the width is the full width of the body,
             * less the margin on the splitter itself.
             */
            #LeftPane {
	            width: 600px;
	            height: 200px;
	            margin: 5px 10px 5px 0;
	            border: 4px solid #bdb;
	            /* No padding allowed */
            }
            /*
             * Left-side element of the splitter. Use pixel units for the
             * min-width and max-width; the splitter plugin parses them to
             * determine the splitter movement limits. Set the width to
             * the desired initial width of the element; the plugin changes
             * the width of this element dynamically.
             */
            #LeftPane {
	            background: #efe;
	            overflow: auto;
	            /* No margin or border allowed */
            }
            
            ul.jqueryFileTree A
            {
                text-align:left;
            }
            
            .colorPicker
            {
                position:absolute;border:2px solid #cccccc;background-color:White;z-index:1001;display:none;width:130px;
                padding: 1px;
                overflow: hidden;
            }
            
            .colorPickerCell 
            {
                height:15px !important;
                width:15px !important;
                float:left;
                margin: 0;
                border: 1px solid white;
            }
            
            .colorPickerCell:hover
            {
                border: 1px solid #333333;
            }
            .style2
            {
                height: 16px;
                width: 222px;
            }
            .style3
            {
                width: 222px;
            }
        </style>

        <script type="text/javascript">
            function isNumber() {
                var evt= document.getElementById("txtVehicleNotReported").Value;
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    return false;
                }
                return true;
            }
        </script>
		
	

	</head>
	<body>
        <script type="text/javascript">           
            var OrganizationHierarchyPath = "";
            
            //var OrganizationHierarchyMultiplePath = "";
            var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
            var RootOrganizationHierarchyNodeCode = '<%=RootOrganizationHierarchyNodeCode %>';
            function inifiletree(inipath) {
                var multipleExpanded = OrganizationHierarchyPath;
                if(inipath != OrganizationHierarchyPath)
                    multipleExpanded = OrganizationHierarchyPath + ";" + inipath;
                $('#vehicletreeview').fileTree({ root: RootOrganizationHierarchyNodeCode, script: '../reports/vehicleListTree.asmx/FetchVehicleList', expanded: inipath, expandSpeed: 200, collapseSpeed: 200
                        , vehicledetails: ''
                        , searchScript: '../reports/vehicleListTree.asmx/SearchOrganizationHierarchy'
                        , scrollSearchResultToBottom: true
                        , LoadVehicleData: false
                        , MutipleUserHierarchyAssignment: MutipleUserHierarchyAssignment
                        , ShowAllFleets: true
                        , SelectedFolders: $('#DefaultOrganizationHierarchyNodeCode').val()
                        , multipleExpanded: multipleExpanded//OrganizationHierarchyPath
                        , scriptForPreferNodecodes: '../reports/vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes'
                    },
                    /*
                    * Call back function when you click left pane tree folder.
                    */
                    function (NodeCode, fleetId, fleetName, fleetPath) {
                        //alert('NodeCode: ' + NodeCode);
                        
                        if(!MutipleUserHierarchyAssignment)
                        {
                            $('#defaultOrganizationHierarchy').html(NodeCode);
                            $('#DefaultOrganizationHierarchyNodeCode').val(NodeCode);
                        }
                        $('#fleetPath').html(fleetPath);
                        
                    },

                    /*
                    * Call back function when you click right pane vehicle list.
                    */
                    function (BoxId) {
                        //alert('BoxId: ' + BoxId);

                    },

                    function (selectedNodecodes, selectedFleetIds, fleetName, selectedFleetPaths)
                    {
                        //$('#defaultOrganizationHierarchy').html(selectedNodecodes + '; ' + selectedFleetIds + ';' + fleetName);
                        //$('#defaultOrganizationHierarchy').html(selectedNodecodes);                        
                        $('#defaultOrganizationHierarchy').html(selectedFleetPaths.replace(/@,@/g, '<div style="border-bottom:1px solid #aaaaaa;height:1px;width:100%;"></div>'));
                        $('#DefaultOrganizationHierarchyNodeCode').val(selectedNodecodes);
                    }

                );  
            }
            $(document).ready(function () {

                $("#MySplitter").splitter({
                    type: "v",
                    outline: true,
                    minLeft: 100, sizeLeft: 480, minRight: 100,
                    resizeToWidth: true,
                    cookie: "sfmsplitter2",
                    accessKey: 'I'
                });

                inifiletree(OrganizationHierarchyPath);

                $('#ohsearchresult').bind('afterShow', function () {
                    alert('scroll me');
                });

            });
            
        </script>
        <script type="text/javascript">
            OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
            
        </script>
		<form id="Configuration" method="post" runat="server">
        <asp:HiddenField ID="DefaultOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="PreferencePageMode" Value="0" runat="server" />
        <asp:HiddenField ID="landmarkstyleLineColor" runat="server" />
        <asp:HiddenField ID="landmarkstyleBackgroundColor" runat = "server" />
        <div id="logo" runat="server" visible="false">
            <a href="../login.aspx"><img src="../images/ProdLogo.gif" border="0" /></a>
        </div>
        <div class="tableDoubleBorder"  style="z-index: 101; left: 12px;position: absolute; top: <%=ToTop%>; height: auto; width: 97%;  background-color: #fffff0">
   
			<TABLE id="Table3" width="100%"  cellSpacing="0" cellPadding="0" border="0">
			
				<TR id="trPreferences" runat="server">
					<TD class="tableheading" style="HEIGHT: 242px" height="242"></TD>
               <td align="left" class="tableheading" height="242" style="height: 242px">
                  &nbsp;</td>
					<TD class="tableheading" style="HEIGHT: 242px" align="left" height="242">
					    <fieldset style="width: 630px;padding: 5px 5px 5px 5px">
					         <legend>
                                   <asp:Label ID="lblUserPreference" Font-Bold=True CssClass="formtext"   runat="server" Text="User Preferences Info:" meta:resourcekey="lblUserPreferenceResource1" ></asp:Label>
                                   <b><%=username %></b>
                                </legend>
                                
                                
						<TABLE id="tblAddPref" style="HEIGHT: 228px" cellSpacing="0" cellPadding="0"
							border="0" runat="server">
                     <tr>
                        <td class="tableheading" style="height: 25px; width: 267px;">
                   </td>
                        <td class="formtext" style="width: 224px; height: 30px">
                           &nbsp;<asp:LinkButton ID="lnkEnglish" runat="server" CausesValidation="False"
                     Font-Underline="True" ForeColor="Black" OnClick="lnkEnglish_Click" Font-Size="X-Small" meta:resourcekey="lnkEnglishResource1" CssClass="formtext" Text="English"></asp:LinkButton>
                  <asp:LinkButton ID="lnkFrench" runat="server" CausesValidation="False"
                     Font-Underline="True" ForeColor="Black" OnClick="lnkFrench_Click" Font-Size="X-Small" meta:resourcekey="lnkFrenchResource1" CssClass="formtext" Text="Français"></asp:LinkButton></td>

                     </tr>
                     <tr>
                        <td class="formtext" style="height: 16px; width: 267px;">
                           <asp:Label ID="lblMapType" runat="server" Text="Map Type:" meta:resourcekey="lblMapTypeResource1"></asp:Label></td>
                        <td class="formtext" style="width: 224px; height: 16px">
                            <asp:DropDownList ID="cboMapType" runat="server" AutoPostBack="True" CssClass="RegularText"
                                DataTextField="Description" DataValueField="MapId" OnSelectedIndexChanged="cboMapType_SelectedIndexChanged"
                                Width="225px" meta:resourcekey="cboMapTypeResource1">
                            </asp:DropDownList></td>
                     </tr>
							<TR>
								<TD class="formtext" style="HEIGHT: 16px; width: 267px;">
                           <asp:Label ID="lblTimeZone" runat="server" Text=" Time Zone*:" meta:resourcekey="lblTimeZoneResource1"></asp:Label></TD>
								<TD class="formtext" style="WIDTH: 224px; HEIGHT: 16px"><asp:dropdownlist id="cboTimeZone" runat="server" Width="225px" CssClass="RegularText" Enabled="False" meta:resourcekey="cboTimeZoneResource1">
										<asp:ListItem Value="-12" meta:resourcekey="ListItemResource1" Text="GMT-12 Eniwetok,Kwajalein"></asp:ListItem>
										<asp:ListItem Value="-11" meta:resourcekey="ListItemResource2" Text="GMT-11 Midway Island"></asp:ListItem>
										<asp:ListItem Value="-10" meta:resourcekey="ListItemResource3" Text="GMT-10 Hawaii"></asp:ListItem>
										<asp:ListItem Value="-9" meta:resourcekey="ListItemResource4" Text="GMT-9 Alaska"></asp:ListItem>
										<asp:ListItem Value="-8" meta:resourcekey="ListItemResource5" Text="GMT-8 Pacific Time (USA&amp;Canada)"></asp:ListItem>
										<asp:ListItem Value="-7" meta:resourcekey="ListItemResource6" Text="GMT-7 Mountain Time (USA&amp;Canada)"></asp:ListItem>
										<asp:ListItem Value="-6" meta:resourcekey="ListItemResource7" Text="GMT-6 Central Time (USA&amp;Canada)"></asp:ListItem>
										<asp:ListItem Value="-5" meta:resourcekey="ListItemResource8" Text="GMT-5 Eastern Time (USA&amp;Canada)"></asp:ListItem>
										<asp:ListItem Value="-4" meta:resourcekey="ListItemResource9" Text="GMT-4 Atlantic Time (Canada)"></asp:ListItem>
                                        <asp:ListItem Value="-3.50" meta:resourcekey="ListItemResource95" Text="GMT-3:30 NewFoundLand"></asp:ListItem>
										<asp:ListItem Value="-3" meta:resourcekey="ListItemResource10" Text="GMT-3 Brasilia,Greenland"></asp:ListItem>
										<asp:ListItem Value="-2" meta:resourcekey="ListItemResource11" Text="GMT-2 Mid-Atlantic"></asp:ListItem>
										<asp:ListItem Value="-1" meta:resourcekey="ListItemResource12" Text="GMT-1 Azores,Cape Verde Is."></asp:ListItem>
										<asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource13" Text="GMT Dublin,London"></asp:ListItem>
										<asp:ListItem Value="1" meta:resourcekey="ListItemResource14" Text="GMT+1 Amsterdam,Berlin,Bern,Rome"></asp:ListItem>
										<asp:ListItem Value="2" meta:resourcekey="ListItemResource15" Text="GMT+2 Jerusalem,Riga,Tallinn"></asp:ListItem>
										<asp:ListItem Value="3" meta:resourcekey="ListItemResource16" Text="GMT+3 Moscow,St. Petersburg"></asp:ListItem>
										<asp:ListItem Value="4" meta:resourcekey="ListItemResource17" Text="GMT+4 Abu Dhabi,Baku,Tbilisi"></asp:ListItem>
										<asp:ListItem Value="5" meta:resourcekey="ListItemResource18" Text="GMT+5 Islamabad,Karachi"></asp:ListItem>
                                        <%--<asp:ListItem Value="5.50" meta:resourcekey="ListItemResource96" Text="GMT+5:30 New Delhi" Enabled ="false"></asp:ListItem>--%>
										<asp:ListItem Value="6" meta:resourcekey="ListItemResource19" Text="GMT+6 Astana,Dhaka"></asp:ListItem>
										<asp:ListItem Value="7" meta:resourcekey="ListItemResource20" Text="GMT+7 Bangkok,Hanoi,Jakarta"></asp:ListItem>
										<asp:ListItem Value="8" meta:resourcekey="ListItemResource21" Text="GMT+8 Beijing,Hong Kong"></asp:ListItem>
										<asp:ListItem Value="9" meta:resourcekey="ListItemResource22" Text="GMT+9 Osaka,Tokyo,Seoul"></asp:ListItem>
										<asp:ListItem Value="10" meta:resourcekey="ListItemResource23" Text="GMT+10 Sydney,Melbourne"></asp:ListItem>
										<asp:ListItem Value="11" meta:resourcekey="ListItemResource24" Text="GMT+11 Magadan"></asp:ListItem>
										<asp:ListItem Value="12" meta:resourcekey="ListItemResource25" Text="GMT+12 Wellington,Fiji"></asp:ListItem>
										<asp:ListItem Value="13" meta:resourcekey="ListItemResource26" Text="GMT+13 Nuku'alofa"></asp:ListItem>							  
                                        

                                </asp:dropdownlist></TD>
							</TR>
							<TR>
								<TD class="formtext" style="HEIGHT: 16px; width: 267px;"><asp:Label ID="LblDistance" runat="server" Text="Miles / Kilometers:" meta:resourcekey="LblDistanceResource1"></asp:Label></TD>
								<TD><asp:dropdownlist id="cboUnits" runat="server" Width="225px" CssClass="RegularText" Enabled="False" meta:resourcekey="cboUnitsResource1">
										<asp:ListItem Value="0.621371" Selected="True" meta:resourcekey="ListItemResource27" Text="Miles"></asp:ListItem>
										<asp:ListItem Value="1" meta:resourcekey="ListItemResource28" Text="Kilometers"></asp:ListItem>
									</asp:dropdownlist></TD>
							</TR>
                            <tr>
                                <td class="formtext" style="width: 267px; height: 16px">
                                    <asp:Label ID="Label1" runat="server" Text="Litre / Gallons:" 
                                        meta:resourcekey="Label1Resource2"></asp:Label></td>
                                <td>
                                    <asp:dropdownlist id="cboVolumeUnits" runat="server" Width="225px" 
                                        CssClass="RegularText" Enabled="False" 
                                        meta:resourcekey="cboVolumeUnitsResource1">
                                        <asp:ListItem Selected="True" Text="Gallons"
                                            Value="0.26" meta:resourcekey="ListItemResource76"></asp:ListItem>
                                        <asp:ListItem  Text="Litre" Value="1" meta:resourcekey="ListItemResource77"></asp:ListItem>
                                    </asp:dropdownlist></td>
                            </tr>
							<TR height="25">
								<TD class="formtext" style="HEIGHT: 19px; width: 267px;"><asp:Label ID="lblDefaultFleet" runat="server" Text="Default Fleet:" meta:resourcekey="lblDefaultFleetResource1"></asp:Label></TD>
								<TD><asp:dropdownlist id="cboFleet" runat="server" Width="225px" CssClass="RegularText" Enabled="False"
										DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"></asp:dropdownlist></TD>
							</TR>
                            <TR height="25">
								<TD class="formtext" style="HEIGHT: 19px; width: 267px;"><asp:Label ID="lblMapAssets" runat="server" Text="Map Assets (By Default)*:" meta:resourcekey="lblMapAssets" ToolTip="If selected and you are using IE8 browser, may experience slower performance. In this case, we will recommend you use vehicle clustering."></asp:Label></TD>
								<TD><asp:checkbox id="chkMapAssets" runat="server" Enabled="False" meta:resourcekey="chkMapAssets" ></asp:checkbox></TD>
							</TR>
                            <TR height="25">
								<TD class="formtext" style="HEIGHT: 19px; width: 267px;">
                                    <asp:Label ID="lblLoadLandmarkByDefault" runat="server" 
                                        Text="Load Landmark By Default:" meta:resourcekey="lblLoadLandmark"></asp:Label></TD>
								<TD><asp:checkbox id="chkLoadLandmarkByDefault" Checked="true" runat="server" Enabled="False" meta:resourcekey="chkLoadLandmarkByDefault" ></asp:checkbox></TD>
							</TR>

                            <%--Moved From Nested Table to Parent Table--%>
                            <TR height="25">
								        <TD class="formtext" style="HEIGHT: 19px; width: 267px;"><asp:Label ID="lblVehicleClustering" runat="server" Text="Vehicle Clustering:" meta:resourcekey="lblVehicleClustering"></asp:Label></TD>
								        <TD><asp:checkbox id="chkVehicleClustering" runat="server" Enabled="False" meta:resourcekey="chkVehicleClustering" onclick="this.checked?$('#vehicleClusteringOptions').show():$('#vehicleClusteringOptions').hide();"></asp:checkbox></TD>
							        </TR>
                                    <tr id="vehicleClusteringOptions" runat="server">
                                        <td height="25" width="267"></td>
                                        <td class="formtext" style="height:60px;" valign="top">
                                            <asp:Label ID="lblVehicleClusteringDistance" runat="server" Text="Distance:" meta:resourcekey="lblVehicleClusteringDistance"></asp:Label>
                                            <asp:TextBox ID="txtVehicleClusteringDistance" runat="server" Enabled="False" 
                                                Text="20" Width="40px" meta:resourcekey="txtVehicleClusteringDistance"></asp:TextBox> Pixels
                                            <br />
                                            <asp:Label ID="lblVehicleClusteringThreshold" runat="server" Text="Threshold:" meta:resourcekey="lblVehicleClusteringThreshold"></asp:Label>   
                                            <asp:dropdownlist ID="cboVehicleClusteringThreshold" runat="server" 
                                                Enabled="False" meta:resourcekey="cboVehicleClusteringThreshold">
                                                <asp:ListItem Value="2" Text="2" meta:resourcekey="ListItemResource78"></asp:ListItem>
                                                <asp:ListItem Value="3" Text="3" meta:resourcekey="ListItemResource79"></asp:ListItem>
                                                <asp:ListItem Value="4" Text="4" meta:resourcekey="ListItemResource80"></asp:ListItem>
                                                <asp:ListItem Value="5" Selected="True" Text="5" 
                                                    meta:resourcekey="ListItemResource81"></asp:ListItem>
                                                <asp:ListItem Value="6" Text="6" meta:resourcekey="ListItemResource82"></asp:ListItem>
                                                <asp:ListItem Value="7" Text="7" meta:resourcekey="ListItemResource83"></asp:ListItem>
                                                <asp:ListItem Value="8" Text="8" meta:resourcekey="ListItemResource84"></asp:ListItem>
                                                <asp:ListItem Value="9" Text="9" meta:resourcekey="ListItemResource85"></asp:ListItem>
                                                <asp:ListItem Value="10" Text="10" meta:resourcekey="ListItemResource86"></asp:ListItem>
                                            </asp:dropdownlist>
                                        </td>
                                    </tr>
                            <%--End Moved From Nested Table to Parent Table--%>
                            <%--<tr><td colspan="2">                         
                                <table width = "100%" border="0" id="vehicleClusteringPreferences" cellpadding="0" cellspacing="0" runat="server">
                                    <TR height="25">
								        <TD class="formtext" style="HEIGHT: 19px; width: 267px;"><asp:Label ID="lblVehicleClustering" runat="server" Text="Vehicle Clustering:" meta:resourcekey="lblVehicleClustering"></asp:Label></TD>
								        <TD><asp:checkbox id="chkVehicleClustering" runat="server" Enabled="False" meta:resourcekey="chkVehicleClustering" onclick="this.checked?$('#vehicleClusteringOptions').show():$('#vehicleClusteringOptions').hide();"></asp:checkbox></TD>
							        </TR>
                                    <tr id="vehicleClusteringOptions" runat="server">
                                        <td height="25"></td>
                                        <td class="formtext" style="height:60px;" valign="top">
                                            <asp:Label ID="lblVehicleClusteringDistance" runat="server" Text="Distance:" meta:resourcekey="lblVehicleClusteringDistance"></asp:Label>
                                            <asp:TextBox ID="txtVehicleClusteringDistance" runat="server" Enabled="False" 
                                                Text="20" Width="40px" meta:resourcekey="txtVehicleClusteringDistance"></asp:TextBox> Pixels
                                            <br />
                                            <asp:Label ID="lblVehicleClusteringThreshold" runat="server" Text="Threshold:" meta:resourcekey="lblVehicleClusteringThreshold"></asp:Label>
                                            <asp:dropdownlist ID="cboVehicleClusteringThreshold" runat="server" 
                                                Enabled="False" meta:resourcekey="cboVehicleClusteringThreshold">
                                                <asp:ListItem Value="2" Text="2" meta:resourcekey="ListItemResource78"></asp:ListItem>
                                                <asp:ListItem Value="3" Text="3" meta:resourcekey="ListItemResource79"></asp:ListItem>
                                                <asp:ListItem Value="4" Text="4" meta:resourcekey="ListItemResource80"></asp:ListItem>
                                                <asp:ListItem Value="5" Selected="True" Text="5" 
                                                    meta:resourcekey="ListItemResource81"></asp:ListItem>
                                                <asp:ListItem Value="6" Text="6" meta:resourcekey="ListItemResource82"></asp:ListItem>
                                                <asp:ListItem Value="7" Text="7" meta:resourcekey="ListItemResource83"></asp:ListItem>
                                                <asp:ListItem Value="8" Text="8" meta:resourcekey="ListItemResource84"></asp:ListItem>
                                                <asp:ListItem Value="9" Text="9" meta:resourcekey="ListItemResource85"></asp:ListItem>
                                                <asp:ListItem Value="10" Text="10" meta:resourcekey="ListItemResource86"></asp:ListItem>
                                            </asp:dropdownlist>
                                        </td>
                                    </tr>
                                </table>
                            </td></tr>--%>
                                  
                                         
                            
							<TR>
								<TD class="formtext" style=" width: 267px;"> <asp:Label ID="lblScreenRefresh" runat="server" Text="Screen refresh frequency:" meta:resourcekey="lblScreenRefreshResource1"></asp:Label></TD>
								<TD><asp:dropdownlist id="cboRefreshFreq" runat="server" CssClass="RegularText" Enabled="False" meta:resourcekey="cboRefreshFreqResource1">
										<asp:ListItem Value="30000" meta:resourcekey="ListItemResource33" Text="30 sec"></asp:ListItem>
										<asp:ListItem Value="60000" Selected="True" meta:resourcekey="ListItemResource34" Text="60 sec"></asp:ListItem>
										<asp:ListItem Value="90000" meta:resourcekey="ListItemResource35" Text="90 sec"></asp:ListItem>
										<asp:ListItem Value="120000" meta:resourcekey="ListItemResource36" Text="120 sec"></asp:ListItem>
									</asp:dropdownlist></TD>
							</TR>
							<%--<TR>
								<TD class="formtext" style="width: 267px;">
                            <asp:Label ID="lblShowLandmark" runat="server" Text="Show landmark name instead of an address:" meta:resourcekey="lblShowLandmarkResource1"></asp:Label></nobr>
                        </TD>
								<TD class="formtext" style="WIDTH: 224px;" align="left"><asp:checkbox id="chkLandmark" runat="server" Enabled="False" meta:resourcekey="chkLandmarkResource1"></asp:checkbox></TD>
							</TR>--%>
							<TR>
								<TD class="formtext" style=" width: 267px;"><asp:Label ID="lblShowReadMessages" runat="server" Text="Show read messages:" meta:resourcekey="lblShowReadMessagesResource1"></asp:Label></TD>
								<TD style="WIDTH: 224px; " align="left"><asp:checkbox id="chkShowReadMess" runat="server" CssClass="formtext" Enabled="False" meta:resourcekey="chkShowReadMessResource1"></asp:checkbox></TD>
							</TR>
							<TR>
								<TD class="formtext" style=" width: 267px;"><asp:Label ID="lblAssetUtilization" runat="server" Text="Asset Utilization Threshold:" meta:resourcekey="lblAssetUtilizationResource1"></asp:Label></TD>
								<TD class="formtext" style="WIDTH: 224px;" align="left"><FONT class="formtext" face="Arial" size="3">
										<TABLE class="formtext" id="Table1" cellSpacing="1" cellPadding="1" width="300" border="0">
											<TR>
												<TD><FONT face="Arial">
                                        <asp:Label ID="lblDays" runat="server" meta:resourcekey="lblDaysResource1" Text="Days:"></asp:Label></FONT></TD>
												<TD><asp:dropdownlist id="cboExpDays" runat="server" CssClass="RegularText" Enabled="False" meta:resourcekey="cboExpDaysResource1">
														<asp:ListItem Value="0" meta:resourcekey="ListItemResource37" Text="0"></asp:ListItem>
														<asp:ListItem Value="1" meta:resourcekey="ListItemResource38" Text="1"></asp:ListItem>
														<asp:ListItem Value="2" meta:resourcekey="ListItemResource39" Text="2"></asp:ListItem>
														<asp:ListItem Value="3" meta:resourcekey="ListItemResource40" Text="3"></asp:ListItem>
														<asp:ListItem Value="7" meta:resourcekey="ListItemResource41" Text="7"></asp:ListItem>
														<asp:ListItem Value="14" meta:resourcekey="ListItemResource42" Text="14"></asp:ListItem>
														<asp:ListItem Value="21" meta:resourcekey="ListItemResource43" Text="21"></asp:ListItem>
														<asp:ListItem Value="30" meta:resourcekey="ListItemResource44" Text="30"></asp:ListItem>
														<asp:ListItem Value="60" meta:resourcekey="ListItemResource45" Text="60"></asp:ListItem>
													</asp:dropdownlist></TD>
												<TD>
                                        <asp:Label ID="lblHours" runat="server" meta:resourcekey="lblHoursResource1" Text="Hours:"></asp:Label></TD>
												<TD><asp:dropdownlist id="cboExpHours" runat="server" CssClass="RegularText" Enabled="False" meta:resourcekey="cboExpHoursResource1">
														<asp:ListItem Value="0" meta:resourcekey="ListItemResource46" Text="0"></asp:ListItem>
														<asp:ListItem Value="1" meta:resourcekey="ListItemResource47" Text="1"></asp:ListItem>
														<asp:ListItem Value="2" meta:resourcekey="ListItemResource48" Text="2"></asp:ListItem>
                                                        <asp:ListItem Value="3" Text="3" meta:resourcekey="ListItemResource87"></asp:ListItem>
                                                        <asp:ListItem Value="4" Text="4" meta:resourcekey="ListItemResource88"></asp:ListItem>
														<asp:ListItem Value="5" meta:resourcekey="ListItemResource49" Text="5"></asp:ListItem>
														<asp:ListItem Value="10" meta:resourcekey="ListItemResource50" Text="10"></asp:ListItem>
														<asp:ListItem Value="12" meta:resourcekey="ListItemResource51" Text="12"></asp:ListItem>
														<asp:ListItem Value="18" meta:resourcekey="ListItemResource52" Text="18"></asp:ListItem>
													</asp:dropdownlist></TD>
												<TD><FONT class="formtext" face="Arial" size="3">
                                        <asp:Label ID="lblMinutes" runat="server" meta:resourcekey="lblMinutesResource1" Text="Minutes:"></asp:Label>
									</FONT>
								                </TD>
												<TD><asp:dropdownlist id="cboExpMin" runat="server" Width="57px" CssClass="RegularText" Enabled="False" meta:resourcekey="cboExpMinResource1">
														<asp:ListItem Value="0" meta:resourcekey="ListItemResource53" Text="0"></asp:ListItem>
														<asp:ListItem Value="1" meta:resourcekey="ListItemResource54" Text="1"></asp:ListItem>
														<asp:ListItem Value="2" meta:resourcekey="ListItemResource55" Text="2"></asp:ListItem>
														<asp:ListItem Value="5" meta:resourcekey="ListItemResource56" Text="5"></asp:ListItem>
														<asp:ListItem Value="10" meta:resourcekey="ListItemResource57" Text="10"></asp:ListItem>
														<asp:ListItem Value="15" meta:resourcekey="ListItemResource58" Text="15"></asp:ListItem>
														<asp:ListItem Value="20" meta:resourcekey="ListItemResource59" Text="20"></asp:ListItem>
														<asp:ListItem Value="30" meta:resourcekey="ListItemResource60" Text="30"></asp:ListItem>
														<asp:ListItem Value="45" meta:resourcekey="ListItemResource61" Text="45"></asp:ListItem>
													</asp:dropdownlist></TD>
											</TR>
										</TABLE>
									</FONT>
								</TD>
							</TR>
                    <TR>
								<TD class="formtext" style=" width: 267px;"> <asp:Label ID="lblAlarmRefresh" runat="server"
                                Text="Alarm/Messages information refresh frequency:" meta:resourcekey="lblAlarmRefreshResource1"></asp:Label></TD>
								<TD><asp:dropdownlist id="cboAlarmFreq" runat="server" CssClass="RegularText" Enabled="False" meta:resourcekey="cboAlarmFreqResource1">
										<asp:ListItem Value="30000" meta:resourcekey="ListItemResource29" Text="30 sec"></asp:ListItem>
										<asp:ListItem Value="60000" Selected="True" meta:resourcekey="ListItemResource30" Text="60 sec"></asp:ListItem>
										<asp:ListItem Value="90000" meta:resourcekey="ListItemResource31" Text="90 sec"></asp:ListItem>
										<asp:ListItem Value="120000" meta:resourcekey="ListItemResource32" Text="120 sec"></asp:ListItem>
									</asp:dropdownlist></TD>
							</TR>
                     <tr>
                        <td class="formtext" style="width: 267px; ">
                            <asp:Label ID="lblViewAlarmScrolling" runat="server" CssClass="formtext" 
                                Text="Map Screen View:" meta:resourcekey="lblViewAlarmScrollingResource1"></asp:Label>
                            
                         </td>
                        <td align="left" colspan="3">
                            <asp:RadioButtonList ID="optMapView" runat="server" CssClass="formtext" 
                                Enabled="False" RepeatDirection="Horizontal" 
                                meta:resourcekey="optMapViewResource1">
                                <asp:ListItem Selected="True" Value="0" meta:resourcekey="ListItemResource89">Messages</asp:ListItem>
                                <asp:ListItem Value="1" meta:resourcekey="ListItemResource90">Alarms</asp:ListItem>
                                <asp:ListItem Value="2" meta:resourcekey="ListItemResource91">Messages and Alarms</asp:ListItem>
                                <asp:ListItem Value="3" meta:resourcekey="ListItemResource94">None</asp:ListItem>
                            </asp:RadioButtonList>
                         </td>
                     </tr>


                            <tr>
                        <td class="formtext" style="width: 267px; ">
                            <asp:Label ID="lblTemprature" runat="server" CssClass="formtext" 
                                Text="Temperature:" Visible="false"></asp:Label>
                            
                         </td>
                        <td align="left" colspan="3">
                            <asp:RadioButtonList ID="optTemperature" runat="server" CssClass="formtext" 
                                Enabled="False" RepeatDirection="Horizontal" 
                                meta:resourcekey="optTemperatureResource1" Visible="false">
                                <asp:ListItem Selected="True" Value="0" >Fahrenheit</asp:ListItem>
                                <asp:ListItem Value="1" >Celsius </asp:ListItem>                                
                            </asp:RadioButtonList>
                         </td>
                     </tr>
                    
                     <tr>
                        <td class="formtext" style="width: 267px; ">
                           <asp:Label ID="lblShowMapGridFilter" runat="server" Text="Show Map Grid Filter:" meta:resourcekey="lblShowMapGridFilterResource1"></asp:Label></td>
                        <td align="left" >
                           <asp:CheckBox ID="chkShowMapGridFilter" runat="server" CssClass="formtext"
                              Enabled="False" meta:resourcekey="chkShowMapGridFilterResource1"  /></td>
                     </tr>
                     <tr>
                        <td class="formtext" style="width: 267px; height: 16px">
                           <asp:Label ID="lblDateFormat" runat="server" 
                                Text="Date Format:" 
                                 Visible="True"></asp:Label></td>
                        <td align="left" style="height: 16px">
                           <asp:DropDownList ID="ddlDateFormat" runat="server" CssClass="RegularText"
                               Enabled="False" Visible="True">
                           </asp:DropDownList></td>
                         <td class="formtext" style="width: 267px; height: 16px">
                           <asp:Label ID="lblTimeFormat" runat="server" 
                                Text="Time Format:" 
                                 Visible="True"></asp:Label></td>
                        <td align="left" style="height: 16px">
                           <asp:DropDownList ID="ddlTimeFormat" runat="server" CssClass="RegularText"
                               Enabled="False" Visible="True">
                               <asp:ListItem Value="hh:mm:ss tt" Text="12 Hours"></asp:ListItem>
                               <asp:ListItem Value="HH:mm:ss" Text="24 Hours"></asp:ListItem>
                           </asp:DropDownList></td>
                     </tr>
                     <tr>
                        <td style="height: 16px; width: 267px;">
                           <asp:Label ID="lblShowLandmarks" runat="server" Text="Show Landmarks:" CssClass="formtext" meta:resourcekey="lblShowLandmarksResource1" Visible="False"></asp:Label></td>
                        <td align="left" style="width: 365px;">
                           <asp:CheckBox ID="chkShowLandmark" runat="server" AutoPostBack="True" Checked="True"
                              CssClass="formtext" Enabled="False" meta:resourcekey="chkShowLandmarkResource1" Visible="False" /></td>
                     </tr>
							<TR>
								<TD style="width: 267px;">
                           <asp:Label ID="lblShowVehicleName" runat="server" CssClass="formtext" 
                              Text="Display Label on Map by Default:" meta:resourcekey="lblShowVehicleNameResource1" Visible="True"></asp:Label></TD>
								<TD style="WIDTH: 365px; " align="left"><asp:CheckBox ID="chkShowVehicleName" runat="server" Checked="True" CssClass="formtext" Enabled="False" meta:resourcekey="chkShowVehicleNameResource2" Visible="True"  /></TD>
							</TR>
                     <tr>
                        <td style="width: 267px;">
                           <asp:Label ID="lblShowLandmarkName" runat="server" CssClass="formtext" 
                              Text="Show Landmark  names:" meta:resourcekey="lblShowLandmarkNameResource1" Visible="False"></asp:Label></td>
                        <td align="left" style="width: 365px; ">
                           <asp:CheckBox ID="chkShowLandmarkname" runat="server" Checked="True"
                              CssClass="formtext" Enabled="False" meta:resourcekey="chkShowLandmarknameResource2" Visible="False"  /></td>
                     </tr>

                     <tr>
                        <td style="height: 16px; width: 267px;">
                           <asp:Label ID="lblShowRetiredVehicles" runat="server" Text="Show Retired Vehicles:" CssClass="formtext" meta:resourcekey="lblShowRetiredVehiclesResource1" Visible="True"></asp:Label></td>
                        <td align="left" style="width: 365px;">
                           <asp:CheckBox ID="chkShowRetiredVehicles" runat="server" AutoPostBack="True" Checked="True"
                              CssClass="formtext" Enabled="false" meta:resourcekey="chkShowRetiredVehiclesResource1" Visible="true" /></td>
                     </tr>
                     
                      <tr>
                        <td class="formtext" style="width: 267px;">
                           <asp:Label ID="lblMapGridDefaultRows" runat="server" 
                                Text="Map Screen Grid Rows #:" 
                                meta:resourcekey="lblMapGridDefaultRowsResource1" Visible="False"></asp:Label></td>
                        <td align="left" >
                           <asp:DropDownList ID="cboMapGridRows" runat="server" CssClass="RegularText"
                              meta:resourcekey="cboGridPagingResource1" Enabled="False" Visible="False">
                              <asp:ListItem meta:resourcekey="ListItemResource64" Text="1"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemResource65" Text="2"></asp:ListItem>
                              <asp:ListItem Selected="True" meta:resourcekey="ListItemResource66" Text="5"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemResource67" Text="7"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemResource68" Text="10"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemResource69" Text="12"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemResource70" Text="15"></asp:ListItem>
                           </asp:DropDownList></td>
                     </tr>
                     <tr>
                        <td class="formtext" style="width: 267px;">
                          <asp:Label ID="lblHistoryGridDefaultRows" runat="server" 
                                Text="History Screen Grid Rows #:" 
                                meta:resourcekey="lblHistoryGridDefaultRowsResource1" Visible="False"></asp:Label></td>
                        <td align="left">
                           <asp:DropDownList ID="cboHistoryGridRows" runat="server" CssClass="RegularText"
                              meta:resourcekey="cboGridPagingResource1" Enabled="False" Visible="False">
                              <asp:ListItem meta:resourcekey="ListItemResource71" Text="1"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemResource72" Text="2"></asp:ListItem>
                              <asp:ListItem Selected="True" meta:resourcekey="ListItemResource73" Text="5"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemResource74" Text="7"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemResource75" Text="10"></asp:ListItem>
                           </asp:DropDownList></td>
                     </tr>
                     
                     <tr>
                        <td style="width: 267px;" class="formtext">
                        <asp:Label ID="lblAutoDayLight" runat="server"
                                Text="Automatically adjust for daylight savings time:" meta:resourcekey="lblAutoDayLightResource1" Visible="False"></asp:Label></td>
                        <td align="right" style="width: 365px; ">
                        <asp:checkbox id="chkDaylight" runat="server" CssClass="formtext" Enabled="False" meta:resourcekey="chkDaylightResource1" Visible="False"></asp:checkbox></td>
                     </tr>
                     <tr>
                        <td style="width: 267px;" class="formtext">
                        <asp:Label ID="lblDefaultMapView" runat="server"
                                Text="Default Map View:" meta:resourcekey="lblDefaultMapViewResource"></asp:Label></td>
                        <td align="left" style="width: 365px; ">
                        <asp:DropDownList ID="cboDefaultMapView" runat="server" CssClass="RegularText"
                              meta:resourcekey="cboGridPagingResource1" Enabled="False">
                              <asp:ListItem Selected="True" meta:resourcekey="ListItemDefaultMapViewResource64" Value="north" Text="Top"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemDefaultMapViewResource65" Value="south" Text="Bottom"></asp:ListItem>
                              <asp:ListItem meta:resourcekey="ListItemDefaultMapViewResource66" Value="east" Text="Right"></asp:ListItem>                              
                              <asp:ListItem Value="none" Text="None"></asp:ListItem>
                           </asp:DropDownList>
                            <asp:HiddenField ID="OriginalDefaultMapView" runat="server" />
                        </td>
                     </tr>
                     <tr>
                        <td class="formtext" style="width: 267px; ">
                           <asp:Label ID="Label7" runat="server" Text="Remember Last Page:" meta:resourcekey="lblRememberLastPageResource1"></asp:Label></td>
                        <td align="left" >
                           <asp:CheckBox ID="chkRememberLastPage" runat="server" CssClass="formtext" Checked="true"
                              Enabled="False" meta:resourcekey="chkRememberLastPageResource1"  /></td>
                     </tr>
                            <tr>
                        <td class="formtext" style="width: 267px; ">
                           <asp:Label ID="Label5" runat="server" Text="Vehicle Not Reported For X Days:" meta:resourcekey="ddlVehicleNotReportedResource1"></asp:Label></td>
                        <td align="left" class="formtext">
                           
                            <asp:TextBox ID="txtVehicleNotReported" runat="server" Text="3" Width="60px" Enabled="false"></asp:TextBox>
                            <asp:CompareValidator runat="server" Operator="DataTypeCheck" Type="Integer" 
 ControlToValidate="txtVehicleNotReported" ErrorMessage="value must be a whole number;" Text="*"  meta:resourcekey="WholeNumberResource1"/>
                            <asp:RangeValidator runat="server" Type="Integer" 
MinimumValue="0" MaximumValue="1000" ControlToValidate="txtVehicleNotReported" 
ErrorMessage="Value must be a whole number between 0- 1000;" Text="*" meta:resourcekey="WholeNumberRangeResource1"/>
                        </td>
                     </tr>
                     <tr id="trLoadVehiclesBasedOn" runat="server" visible="false">
                        <td style="width: 267px;" class="formtext">
                            <asp:Label ID="Label3" runat="server" Text="Load Vehicles Based On:" 
                                meta:resourcekey="Label3Resource1"></asp:Label>
                        </td>
                        <td align="left" style="width: 365px; ">
                        <asp:DropDownList ID="cboLoadVehiclesBasedOn" runat="server" CssClass="RegularText" 
                                Enabled="False" meta:resourcekey="cboLoadVehiclesBasedOnResource1">
                              <asp:ListItem Selected="True" Value="fleet" Text="Fleet" 
                                  meta:resourcekey="ListItemResource92"></asp:ListItem>
                              <asp:ListItem Value="hierarchy" Text="Organization Hierarchy" 
                                  meta:resourcekey="ListItemResource93"></asp:ListItem>                              
                           </asp:DropDownList></td>
                     </tr>
                     <tr id="trDefaultOrganizationHierarchy" runat="server" visible="false">
                        <td style="width: 267px;" class="formtext" valign="top">
                        <asp:Label ID="Label2" runat="server"
                                Text="Default Organization Hierarchy:" meta:resourcekey="Label2Resource1"></asp:Label></td>
                        <td align="left" class="formtext" style="width: 365px; ">
                            <asp:Label ID="defaultOrganizationHierarchy" runat="server" 
                                meta:resourcekey="defaultOrganizationHierarchyResource1"></asp:Label>
                        </td>                         
                     </tr>
                     <tr id="trDefaultOrganizationHierarchyTree" runat="server" visible="false">
                        <td style="width: 267px;" class="formtext" colspan="2">
                                <div id="ohsearchbar" class="formtext" style="margin-top:10px;"><asp:Label ID="Label8" runat="server" 
                                        CssClass="tableheading" Text="Search: " meta:resourcekey="Label8Resource1"></asp:Label>
                                    <input type="text" id="ohsearchbox" class="ohsearch" />
                                    <a href="javascript:void(0);" onclick="onsearchbtnclicked('../reports/vehicleListTree.asmx/SearchOrganizationHierarchy');"><img src="../images/searchicon.png" border="0" /></a>
                                    <asp:Label ID="Label10" runat="server" style="color:#666666;display:none;" 
                                        Text="(Type in at least 3 characters to search)" 
                                        meta:resourcekey="Label10Resource1"></asp:Label>
                                </div>
                                <div id="ohsearchresult">
                                    <div id="ohsearchresulttitle">
                                        <asp:Label ID="Label19" runat="server" Text="Search Result:" 
                                            meta:resourcekey="Label19Resource1"></asp:Label>
                                         <a href="javascript:void(0)" onclick="$('#ohsearchresultlist ul').html('');$('#ohsearchresult').hide();$('#ohsearchbox').val('');">
                                        <asp:Label ID="Label18" runat="server" Text="Close" 
                                            meta:resourcekey="Label18Resource1"></asp:Label></a>
                                    </div>
                                    <div id="ohsearchresultlist">
                                        <ul></ul>
                                    </div>
                                </div>
                                <div id="LeftPane">
			                        <div id="vehicletreeview" class="demo"></div>		                                                
                                </div>                                
                                <div style="font-family: Verdana,sans-serif; font-size: 12px; margin-bottom: 5px;">
                                    <table border="0">
                                    <tr><td width="30" valign="top"><asp:Label ID="Label9" runat="server" Text="Path:" class="formtext" meta:resourcekey="Label9Resource1"></asp:Label>
                                    </td>
                                    <td id="fleetPath" width="600" height="35" valign="top" class="formtext">
                                    </td></tr>
                                    </table>                                    
                                </div>
                            
                        </td>                         
                     </tr>
                     <tr style="display:none;">
                        <td style="width: 267px;" class="formtext">
                        <asp:Label ID="lblLandmarkStyle" runat="server"
                                Text="Landmark Style:" meta:resourcekey="lblLandmarkStyleResource1"></asp:Label></td>
                        <td align="left" style="width: 365px; ">
                            <div id="landmarkstyle" style="height:20px;width:20px;border:1px solid #cc6633;background-color:#ffcc66;">
                            </div>
                        </td>
                     </tr>
                     <tr>
                        <td style="width: 267px;">
                        </td>
                        <td align="right" style="width: 365px;">
                           <asp:button id="cmdSave" runat="server" Width="114px" CssClass="combutton" Text="Edit Preferences"
										CausesValidation="True" onclick="cmdSave_Click" meta:resourcekey="cmdSaveResource1" ></asp:button></td>
                     </tr>
						</TABLE>
						</fieldset>
					</TD>
					<TD class="tableheading" style="HEIGHT: 242px" height="242"></TD>
				</TR>
				<TR>
					<TD class="tableheading" height="20"></TD>
               <td class="tableheading" height="20">
               </td>
					<TD class="tableheading" height="20"><asp:label id="lblMessage" runat="server" Width="270px" CssClass="errortext" Visible="False" meta:resourcekey="lblMessageResource1"></asp:label></TD>
					<TD class="tableheading" height="20"></TD>
				</TR>
                <TR>
					<TD class="tableheading" style="BORDER-TOP-WIDTH: thin; BORDER-TOP-COLOR: black; HEIGHT: 21px"
						height="21"></TD>
               <td class="tableheading" height="21" style="border-top-width: thin; border-top-color: black;
                  height: 21px">
               </td>
					<TD class="tableheading" style="BORDER-TOP-WIDTH: thin; BORDER-TOP-COLOR: black; HEIGHT: 21px"
						height="21">
                        <asp:Label ID="lblMessageError" runat="server" 
                            Text="Your account has been expired, please change your password" 
                            Visible="False" ForeColor="Red" meta:resourcekey="lblMessageErrorResource1"></asp:Label>
                    </TD>
					<TD class="tableheading" style="BORDER-TOP-WIDTH: thin; BORDER-TOP-COLOR: black; HEIGHT: 21px"
						height="21"></TD>
				</TR>
				<TR>
					<TD></TD>
               <td align="left">
               </td>
					<TD align="left">
					    <fieldset style="width: 630px;padding: 5px 5px 5px 5px" >
					    
					    <legend>
                                   <asp:Label ID="lblChangePassword" runat="server" CssClass="formtext"  Text="Change Password:" meta:resourcekey="lblChangePasswordResource1" Font-Bold="True"></asp:Label>
                                   
                                </legend>
                                
					    
						<TABLE id="tblChangePswMain" cellSpacing="0" cellPadding="0" width="width: 630px" border="0" runat=server>
							<TR>
								<TD style="HEIGHT: 20px" align="left">
                           <table id="tblChangePassword" runat="server" class="formtext" style="width: 630px">
                              <tr>
                                 <td class="style2" >
                                    <asp:Label ID="lblOldPsw" runat="server" Text="Enter your old password:" meta:resourcekey="lblOldPswResource1"></asp:Label>
												<asp:requiredfieldvalidator id="valNewPassword" runat="server" Enabled="False" ControlToValidate="txtNewPassword"
													ErrorMessage="Enter a new Password" meta:resourcekey="valNewPasswordResource1" Text="*"></asp:requiredfieldvalidator>
												<asp:requiredfieldvalidator id="valOldPassword" runat="server" Enabled="False" ControlToValidate="txtOldPassword"
													ErrorMessage="Enter you old password" meta:resourcekey="valOldPasswordResource1" Text="*"></asp:requiredfieldvalidator></td>
                                 <td style="width: 103px">
												<asp:textbox id="txtOldPassword" tabIndex="2" runat="server" Width="140px" CssClass="formtext"
													Enabled="False" TextMode="Password" meta:resourcekey="txtOldPasswordResource1"></asp:textbox></td>
                              </tr>
                              <tr>
                                 <td class="style2" >
                                    <asp:Label ID="lblNewPsw" runat="server" Text="Enter a new password:" meta:resourcekey="lblNewPswResource1"></asp:Label></td>
                                 <td >
												<asp:textbox id="txtNewPassword" tabIndex="2" runat="server" Width="140px" CssClass="formtext"
													Enabled="False" TextMode="Password" meta:resourcekey="txtNewPasswordResource1"></asp:textbox> &nbsp;<span id="strength" runat=server ></span> </td>
                              </tr>
                              <tr>
                                 <td class="style2" >
                                    <asp:Label ID="lblReenterPsw" runat="server" Text="Re-enter the new password:" meta:resourcekey="lblReenterPswResource1" Visible="False"></asp:Label>
												<asp:comparevalidator id="vlComp" runat="server" Enabled="False" ControlToValidate="txtNewPassword" ErrorMessage="Please re-enter the new password"
													ControlToCompare="txtNewPassword1" meta:resourcekey="vlCompResource1" Text="*"></asp:comparevalidator></td>
                                 <td style="width: 103px">
												<asp:textbox id="txtNewPassword1" tabIndex="2" runat="server" Width="140px" CssClass="formtext"
													Enabled="False" TextMode="Password" meta:resourcekey="txtNewPassword1Resource1"></asp:textbox> <input id="txtPasswordStatus" type="hidden" name="txtPasswordStatus"></td>
                              </tr>
                              <tr>
                                 <td class="style3">
                                 </td>
                                 <td align="right">
                                    &nbsp;<asp:button id="cmdSavePsw" runat="server" Width="114px" CssClass="combutton" Text="Edit Password" onclick="cmdSavePsw_Click" meta:resourcekey="cmdSavePswResource1"></asp:button></td>
                              </tr>
                              <tr>
                                 <td colspan="2">
                                 <asp:label id="lblPswMsg" runat="server" CssClass="errortext" Visible="False" meta:resourcekey="lblPswMsgResource1"></asp:label>
                                 </td>
                              </tr>
                              <tr>
                                 <td class="style3">
                                 </td>
                                 <td align="right">
                                    <asp:Button runat=server CssClass=combutton ID=cmdHome style="width:114px; height:19px"  text=Cancel meta:resourcekey="cmdHomeResource1" OnClick="cmdHome_Click" CausesValidation="False" />
                                    <asp:Button runat="server" CssClass=combutton ID="cmdExit" Visible=false   style="width:114px; height:19px"  text=Close OnClientClick="javascript:window.close();"   CausesValidation="False"/>
                                    </td> 
                              </tr>
                           </table>
								</TD>
							</TR>
							<TR>
								<TD align="center">
								    <asp:validationsummary id="ValidationSummary1" runat="server" Width="317px" CssClass="errortext" meta:resourcekey="ValidationSummary1Resource1"></asp:validationsummary>
								</TD>
							</TR>
							</TABLE>
						</fieldset> 
						<TABLE id="Table2" style="WIDTH: 100%" cellSpacing="0" cellPadding="0" border="0" DESIGNTIMEDRAGDROP="87">
							<TR>
								<TD class="formtext" style="HEIGHT: 15px" align="right"><%--<INPUT class="combutton" id="cmdHome" style="WIDTH: 114px; HEIGHT: 19px" onclick='window.open("../frmMain.htm",target="_parent")'
										type="button" value="Cancel">--%>
								</TD>
							</TR>
                            <tr id="pswRules" runat="server" visible="true">
                                <td class="formtext" style="HEIGHT: 15px" align="left">
                                    <ul>
                                        <li>
                                            <asp:Label ID="Label4" runat="server" 
                                                Text="Password must be atleast 7 characters in length" 
                                                meta:resourcekey="Label4Resource1"></asp:Label>
                                        </li>
                                       <%-- <li>
                                            <asp:Label ID="Label5" runat="server" 
                                                Text="" 
                                                meta:resourcekey="Label5Resource1"></asp:Label>
                                        </li>--%>
                                        <li>
                                            <asp:Label ID="Label6" runat="server"   
                                                Text="Password should contain 1 numeral and 1 uppercase letter." 
                                                meta:resourcekey="Label6Resource1"></asp:Label>
                                        </li>
                                    </ul>                                    
                                </td>
                            </tr>
							<TR>
								<TD class="formtext" style="HEIGHT: 15px" align="left">
									<STRONG>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label ID="lblTimeZoneDesc" runat="server"
                               Text="Time zone: * In order to display local time in the system, you must select your time zone. " Font-Bold="False" meta:resourcekey="lblTimeZoneDescResource1"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp; </STRONG>
									<BR>
                            </TD>
							</TR>
						</TABLE>
					</TD>
					<TD>&nbsp;
					</TD>
				</TR>
			</TABLE>
			</div>
		</form>

        <div id="polygonStyleEditor" style="position:absolute;border:1px solid #cccccc;background-color:White;z-index:1000;display:none;">
            <table style="margin:10px;">
                <tr class="formtext">
                    <td width="100">Line Color</td>
                    <td><div id="polygonStyleEditorLineColor" style="width:40px;height:20px;"></div></td>
                </tr>
            </table>
        </div>

        <div id="colorPicker">
            <!--<table border="0">
                <tr>
                    <td>
                        <div id="colorPicker00" class="colorPickerCell" style="background-color:rgb(255,255,255);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker01" class="colorPickerCell" style="background-color:rgb(204,204,204);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker02" class="colorPickerCell" style="background-color:rgb(192,192,192);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker03" class="colorPickerCell" style="background-color:rgb(153,153,153);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker04" class="colorPickerCell" style="background-color:rgb(102,102,102);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker05" class="colorPickerCell" style="background-color:rgb(51,51,51);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker06" class="colorPickerCell" style="background-color:rgb(0,0,0);">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="colorPicker10" class="colorPickerCell" style="background-color:rgb(255,204,204);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker11" class="colorPickerCell" style="background-color:rgb(255,102,102);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker12" class="colorPickerCell" style="background-color:rgb(255,0,0);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker13" class="colorPickerCell" style="background-color:rgb(204,0,0);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker14" class="colorPickerCell" style="background-color:rgb(153,0,0);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker15" class="colorPickerCell" style="background-color:rgb(102,0,0);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker16" class="colorPickerCell" style="background-color:rgb(51,0,0);">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="colorPicker20" class="colorPickerCell" style="background-color:rgb(255, 204, 153);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker21" class="colorPickerCell" style="background-color:rgb(255, 153, 102);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker22" class="colorPickerCell" style="background-color:rgb(255, 153, 0);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker23" class="colorPickerCell" style="background-color:rgb(255, 102, 0);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker24" class="colorPickerCell" style="background-color:rgb(204, 102, 0);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker25" class="colorPickerCell" style="background-color:rgb(153, 51, 0);">
                        </div>
                    </td>
                    <td>
                        <div id="colorPicker26" class="colorPickerCell" style="background-color:rgb(102, 51, 0);">
                        </div>
                    </td>
                </tr>
            </table>
            -->
        </div>
		
		
    <script type="text/javascript">
         function passwordChanged() 
         {
	        var strength = document.getElementById('strength');
	        var strongRegex = new RegExp("^(?=.{8,})(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*\\W).*$", "g");
	        var mediumRegex = new RegExp("^(?=.{7,})(((?=.*[A-Z])(?=.*[a-z]))|((?=.*[A-Z])(?=.*[0-9]))|((?=.*[a-z])(?=.*[0-9]))).*$", "g");
	        var enoughRegex = new RegExp("(?=.{6,}).*", "g");
	        var pwd = document.getElementById("txtNewPassword");
	        var txtPasswordStatus=document.forms[0].elements["txtPasswordStatus"];
	            
	            
	        if (pwd.value.length==0) {
		        strength.innerHTML ='<%=msgPsw_TypePassword%>';
	        } else if (false == enoughRegex.test(pwd.value)) {
		        strength.innerHTML ='<%=msgPsw_MoreCharacters%>';
		            txtPasswordStatus.value="0";
	        } else if (strongRegex.test(pwd.value)) {
		        strength.innerHTML = '<span style="color:green"><%=msgPsw_Strong%></span>';
		        txtPasswordStatus.value="1";
	        } else if (mediumRegex.test(pwd.value)) {
		        strength.innerHTML = '<span style="color:orange"><%=msgPsw_Medium%></span>';
		            txtPasswordStatus.value="1";
	        } else { 
		        strength.innerHTML = '<span style="color:red"><%=msgPsw_Weak%></span>';
		        txtPasswordStatus.value="0";
	        }
		}

		$('#landmarkstyle').click(function () { if ($('#PreferencePageMode').val() == '1' || true) { editpolygonStyle('landmarkstyle'); } });

		function editpolygonStyle(p) {
		    x = $('#' + p).offset().left;
		    y = $('#' + p).offset().top;

		    var o = {
		        left: x + 30,
		        top: y
		    };

		    $('#polygonStyleEditorLineColor').css('background-color', $('#' + p + 'LineColor').val());
            $('#polygonStyleEditor').offset(o).show();
        }

        /*$('#polygonStyleEditorLineColor').click(function () {
            x = $('#polygonStyleEditorLineColor').offset().left;
            y = $('#polygonStyleEditorLineColor').offset().top;

            var o = {
                left: x,
                top: y
            };

            $('#colorPicker').offset(o).show();
        });*/

        //$('#colorPicker').mouseleave(function () { $('#colorPicker').offset({left:0,top:0}).hide(); });

        //$('#polygonStyleEditorLineColor').colorPicker({ colorPickerDiv: 'colorPicker' });
        
    </script>
	</body>
</HTML>
