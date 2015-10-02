<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FleetVehicleOrganizationHierarchy.ascx.cs" Inherits="SentinelFM.UserControl_FleetVehicleOrganizationHierarchy" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
  {%>
  
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script> 
    <script type="text/javascript" src="<%=RootUrl%>scripts/jquery.cookie.js"></script>
    <script src="<%=RootUrl%>reports/jqueryFileTree.js?v=20150514" type="text/javascript"></script>
    <script src="<%=RootUrl%>reports/splitter.js" type="text/javascript"></script>
    <script src="<%=RootUrl%>scripts/tablesorter2145/js/jquery.tablesorter.js?v=20140113" type="text/javascript"></script>
    <script src="<%=RootUrl%>scripts/tablesorter2145/js/jquery.tablesorter.widgets.min.js" type="text/javascript"></script>
    <script src="<%=RootUrl%>scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.js" type="text/javascript"></script>
    <script src="<%=RootUrl%>scripts/colResizable-1.3.min.js" type="text/javascript"></script>
    <script src="<%=RootUrl%>scripts/json2.js" type="text/javascript"></script>
    <link rel="stylesheet" href="<%=RootUrl%>scripts/tablesorter2145/css/theme.blue.css" type="text/css" />
    <link rel="stylesheet" href="<%=RootUrl%>scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css" type="text/css" />
	<link href="<%=RootUrl%>reports/jqueryFileTree.css" rel="stylesheet" type="text/css" media="screen" />

    <style type="text/css">
        .style1
        {
            width: 45px;
        }
        
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
        #MySplitter {
	        height: 300px;
	        margin: 5px 40px 5px 0;
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
        
        #LeftPane {
	        width: 600px;
	        height: 200px;
	        margin: 5px 10px 5px 0;
	        border: 4px solid #bdb;
	        /* No padding allowed */
        }
        
        #RightPane {
	        background: #f8fff8;
	        overflow: auto;
	        /* No margin or border allowed */
        }
        /* 
         * Splitter bar style; the .active class is added when the
         * mouse is over the splitter or the splitter is focused
         * via the keyboard taborder or an accessKey. 
         */
        #MySplitter .vsplitbar {
	        width: 6px;
	        background: #aca url(images/vgrabber.gif) no-repeat center;
        }
        #MySplitter .vsplitbar.active {
	        background: #da8 url(images/vgrabber.gif) no-repeat center;
	        opacity: 0.7;
        }
        
        ul.jqueryFileTree A
        {
            text-align:left;
        }
        
        #organizationHierarchyTree
        {
            position:absolute;
            width:700px;
            height: 400px;
            left: 20px;
            top: 60px;
            z-index: 200;
            background-color:White;
            border: 2px solid #cccccc;
            padding: 10px;
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

        <script language="javascript">
			<!--
            OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
            var DefaultOrganizationHierarchyFleetId = '<%=DefaultOrganizationHierarchyFleetId %>';
             var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
             var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
            var vehicletreeviewIni = false;
            var selectedOrganizationHierarchyNodeCode = '';
            var RootOrganizationHierarchyNodeCode = '<%=RootOrganizationHierarchyNodeCode %>';

            var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
            
            function inifiletree(inipath) {
                $('#vehicletreeview').fileTree({ root: RootOrganizationHierarchyNodeCode, script: '<%=RootUrl%>reports/vehicleListTree.asmx/FetchVehicleList', expanded: inipath,                    
                    expandSpeed: 200, collapseSpeed: 200, vehicledetails: 'vehicledetails',
                    highlightVehicleSelection: false,
                    searchScript: '<%=RootUrl%>reports/vehicleListTree.asmx/SearchOrganizationHierarchy'
                    , LoadVehicleData: false
                    , MutipleUserHierarchyAssignment: MutipleUserHierarchyAssignment
                    , PreferOrganizationHierarchyNodeCode: RootOrganizationHierarchyNodeCode
                    , scriptForPreferNodecodes: '../reports/vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes'
                    , SelectedFolders: $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val()
                },
                /*
                * Call back function when you click left pane tree folder.
                */
                        function (NodeCode, fleetId) {
                            //$('#OrganizationHierarchyNodeCode').val(NodeCode);
                            //$('#OrganizationHierarchyBoxId').val('');
                            selectedOrganizationHierarchyNodeCode = NodeCode;
                            TempSelectedOrganizationHierarchyFleetId = fleetId;
                        },

                /*
                * Call back function when you click right pane vehicle list.
                */
                        function (BoxId) {
                            //alert('BoxId: ' + BoxId);
                            //$('#OrganizationHierarchyBoxId').val(BoxId);
                        },

                        function (selectedNodecodes, selectedFleetIds, fleetName)
                        {
                            //$('#defaultOrganizationHierarchy').html(selectedNodecodes);
                            //$('#DefaultOrganizationHierarchyNodeCode').val(selectedNodecodes);
                            selectedOrganizationHierarchyNodeCode = selectedNodecodes;
                            TempSelectedOrganizationHierarchyFleetId = selectedFleetIds;
                            
                        }
                    );
            }

            function applyOrganizationHierarchy() {
                $('#organizationHierarchyTree').hide();
                DefaultOrganizationHierarchyFleetId = TempSelectedOrganizationHierarchyFleetId;
                $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(selectedOrganizationHierarchyNodeCode);
                $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(DefaultOrganizationHierarchyFleetId);
                $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();

                
            }

            function cancelOrganizationHierarchy() {
                $('#organizationHierarchyTree').hide();
                TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
            }

            function onOrganizationHierarchyNodeCodeClick()
            {
                $('#organizationHierarchyTree').show();
                
                //if(!vehicletreeviewIni){
                if($('#<%=vehicletreeviewIni.ClientID %>').val()=='false'){
                    //vehicletreeviewIni = true;
                    $('#<%=vehicletreeviewIni.ClientID %>').val('true')
                    $("#MySplitter").splitter({
                        type: "v",
                        outline: true,
                        minLeft: 100, sizeLeft: 280, minRight: 100,
                        resizeToWidth: true,
                        cookie: "vsplitter",
                        accessKey: 'I'
                    });
                
                    //inifiletree(OrganizationHierarchyPath);
                    inifiletree($('#<%=hidOrganizationHierarchyPath.ClientID %>').val());

                    $('#vehiclelisttbl').tablesorter();
                    $('#vehiclelisttbl').colResizable({ headerOnly: true });
                }         
                return false;
            }
            
			//-->
    </script>
<%} %>

<asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
<asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
<asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />
<asp:HiddenField ID="vehicletreeviewIni" Value="false" runat="server" />

<table >
    <tr >
        
        <td align="left" >
            <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1"
                Text="Fleet:" />
            <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" 
                Text=" Hierarchy Node:" Visible="False" 
                meta:resourcekey="lblOhTitleResource1"  />
        </td>
        <td align="left" runat="server" visible="true">
            <telerik:RadComboBox ID="cboFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                OnSelectedIndexChanged="cboFleet_SelectedIndexChanged" Width="257px" 
                Filter="Contains" MarkFirstMatch="True" 
                ChangeTextOnKeyBoardNavigation="False" Skin="Hay"
                MaxHeight="400px" CausesValidation = "False" Visible="true"
            />
            <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                CssClass="combutton" Visible="False" 
                OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />            
        </td>
        <td align="left" >
            <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" meta:resourcekey="lblVehicleNameResource1"
                Text=" Vehicle:"  />
        </td>
        <td align="left">
            <telerik:RadComboBox ID="cboVehicle" runat="server" CssClass="RegularText" DataTextField="Description"
                DataValueField="VehicleId" meta:resourcekey="cboVehicleResource1" Width="256px" 
                Filter="Contains" MarkFirstMatch="True" 
                ChangeTextOnKeyBoardNavigation="False" Skin="Hay"
                MaxHeight="400px" CausesValidation = "False"
            />
        </td>
    </tr>
</table>

<asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" style="display:none;"
    onclick="hidOrganizationHierarchyPostBack_Click" 
    meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />


<%if (sn.User.LoadVehiclesBasedOn == "hierarchy")
  {%>
<div id="organizationHierarchyTree" style="display:none;">
    <div id="ohsearchbar" class="formtext"><asp:Label ID="lblHierarchy" runat="server" 
            CssClass="tableheading" Text="Search Organization Hierarchy: " 
            meta:resourcekey="lblHierarchyResource1"></asp:Label>
        <input type="text" id="ohsearchbox" class="ohsearch" />
        <a href="javascript:void(0);" onclick="onsearchbtnclicked('reports/vehicleListTree.asmx/SearchOrganizationHierarchy');"><img src="../images/searchicon.png" border="0" /></a>
        <asp:Label ID="Label10" runat="server" style="color:#666666;" 
            Text="(Type in at least 3 characters to search)" 
            meta:resourcekey="Label10Resource1"></asp:Label>
    </div>
    <div id="ohsearchresult">
        <div id="ohsearchresulttitle">
        <asp:Label ID="lblSearchResult" runat="server" CssClass="tableheading" 
                Text=" Search Result:" meta:resourcekey="lblSearchResultResource1"></asp:Label>

           <a href="javascript:void(0)" onclick="$('#ohsearchresultlist ul').html('');$('#ohsearchresult').hide();$('#ohsearchbox').val('');">  
            <asp:Label ID="lblClose" runat="server" CssClass="tableheading" Text="Close" 
                meta:resourcekey="lblCloseResource1"></asp:Label></a>
        </div>
        <div id="ohsearchresultlist">
            <ul></ul>
        </div>
    </div>
    
                                                   
        <div id="LeftPane">
		    <div id="vehicletreeview" class="demo"></div>		                                                
        </div>     
        
    
    <div>
        <input type="button" class="kd-button" onclick="applyOrganizationHierarchy();" id="Button1" value="OK" />
        <input type="button" class="kd-button" onclick="cancelOrganizationHierarchy();" id="Button2" value="Cancel" />
    </div>
</div> 
<%} %>