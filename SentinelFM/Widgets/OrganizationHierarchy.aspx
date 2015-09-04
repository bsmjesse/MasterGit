<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OrganizationHierarchy.aspx.cs" Inherits="SentinelFM.Widgets_OrganizationHierarchy" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery.cookie.js"></script>
    <script src="../reports/jqueryFileTree.js?v=2015070202" type="text/javascript"></script>
    <script src="../reports/splitter.js" type="text/javascript"></script>
    <%--<script src="../scripts/tablesorter/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script src="../scripts/tablesorter/jquery.tablesorter.widgets.min.js" type="text/javascript"></script>--%>
    <script src="../scripts/tablesorter2145/js/jquery.tablesorter.min.js"></script>
	<script src="../scripts/tablesorter2145/js/jquery.tablesorter.widgets.min.js"></script>
    <script src="../scripts/colResizable-1.3.min.js" type="text/javascript"></script>
    <script src="../scripts/json2.js" type="text/javascript"></script>
    
    <%--<link rel="stylesheet" href="../scripts/tablesorter/themes/report/style.css" type="text/css" />--%>
    <link rel="stylesheet" href="../scripts/tablesorter2145/css/theme.blue.css">
	<link href="../reports/jqueryFileTree.css?v=20140108" rel="stylesheet" type="text/css" media="screen" />
    
    <!-- Tablesorter pager: required -->
	<link rel="stylesheet" href="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css">
	<script src="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.js"></script>

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
            width: 369px;
        }
        
        #vehicletreeview, #vehicledetails
        {
            padding: 0 5px;
        }
        
        #vehicledetails
        {
            /*width:450px;*/
            <%--margin-right:16px;--%>
            border-bottom: 1px solid #CCCCCC;            
            height: 255px;
            margin-bottom: 0;
            margin-right: 0;
            overflow: auto; 
        }
        
        #vehiclelistPageBar
        {
            padding: 2px 0 0 5px;
            height: 26px;  
            overflow: hidden;                  
        }
        
        /*
         * Splitter container. Set this to the desired width and height
         * of the combined left and right panes. In this example, the
         * height is fixed and the width is the full width of the body,
         * less the margin on the splitter itself.
         */
        #MySplitter {
	        height: 290px;
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
        
        <% if(!LoadVehicles) { %>
        #LeftPane {
	        width: 600px;
	        height: 200px;
	        margin: 5px 10px 5px 0;
	        border: 4px solid #bdb;
	        /* No padding allowed */
        }
        <% } %>
        /*
         * Right-side element of the splitter.
         */
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
            left: 10px;
            top: 10px;
            z-index: 200;
            background-color:White;
            border: 2px solid #cccccc;
            padding: 10px;
        }
        
        /*#vehiclelisttbl
        {
            width: 250px;
        }*/
        
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
            var ResMultipleHierarchy = '<%=ResMultipleHierarchy %>'; //'Multiple Hierarchies';
            
            var MutipleUserHierarchyAssignment = <%=MutipleUserHierarchyAssignment.ToString().ToLower() %>;
            var FullTreeView = <%=FullTreeView.ToString().ToLower() %>;
            var PreferOrganizationHierarchyNodeCode = '<%=PreferOrganizationHierarchyNodeCode %>';
            var SelectedOrganizationHierarchyNodeCode = '<%=SelectedOrganizationHierarchyNodeCode %>';
            var RootOrganizationHierarchyNodeCode = '<%=RootOrganizationHierarchyNodeCode %>';

            OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
            var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
            var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
            var vehicletreeviewIni = false;
            var selectedOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
            var selectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
            var selectedOrganizationHierarchyFleetName = '';
            var LoadVehicles = <%=LoadVehicles.ToString().ToLower() %>;
            
            function inifiletree(inipath) {
                $('#vehicletreeview').fileTree({ root: RootOrganizationHierarchyNodeCode, script: '../reports/vehicleListTree.asmx/FetchVehicleList', expanded: inipath,
                    expandSpeed: 200, collapseSpeed: 200, vehicledetails: 'vehicledetails',
                    highlightVehicleSelection: false,
                    searchScript: '../reports/vehicleListTree.asmx/SearchOrganizationHierarchy'
                    , MutipleUserHierarchyAssignment: MutipleUserHierarchyAssignment
                    , FullTreeView: FullTreeView
                    , PreferOrganizationHierarchyNodeCode: PreferOrganizationHierarchyNodeCode
                    , scriptForPreferNodecodes: '../reports/vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes'
                    //, SelectedOrganizationHierarchyNodeCode: SelectedOrganizationHierarchyNodeCode
                    //, SelectedFolders: SelectedOrganizationHierarchyNodeCode
                    , SelectedFolders: $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val()
                    , VehicleListPagingBarId: 'vehiclelistPageBar'
                    , scriptForFetchVehicleByPage: '../reports/vehicleListTree.asmx/FetchVehicleListFilterByPage'
                    , LoadVehicleData: LoadVehicles
                    , ResMultipleHierarchy: ResMultipleHierarchy
                    , VehiclePageSize: <%=VehiclePageSize %>
                },
                /*
                * Call back function when you click left pane tree folder.
                */
                        function (NodeCode, fleetId, fleetName, fleetPath) {
                            if(!MutipleUserHierarchyAssignment)
                            {
                                selectedOrganizationHierarchyNodeCode = NodeCode;
                                selectedOrganizationHierarchyFleetId = fleetId;
                                selectedOrganizationHierarchyFleetName = fleetName;     
                            }
                            $('#fleetPath').html(fleetPath);                                   
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
                            selectedOrganizationHierarchyFleetId = selectedFleetIds;
                            selectedOrganizationHierarchyFleetName = fleetName;
                            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(selectedNodecodes)
                            
                        }
                    );
            }

            function applyOrganizationHierarchy() {                
                window.opener.OrganizationHierarchyNodeSelected(selectedOrganizationHierarchyNodeCode, selectedOrganizationHierarchyFleetId, selectedOrganizationHierarchyFleetName);
                window.close();
                
            }

            function cancelOrganizationHierarchy() {
                window.close();
            }

            //-->
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('#test').click(function () {
                window.opener.test();
            });



            $("#MySplitter").splitter({
                type: "v",
                outline: true,
                minLeft: 100, sizeLeft: 371, minRight: 100,
                resizeToWidth: true,
                cookie: "sfmsplitter8",
                accessKey: 'I'
            });

            //inifiletree(OrganizationHierarchyPath);
            inifiletree('<%=OrganizationHierarchyPath %>');

            
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="organizationHierarchyTree">
    <div id="ohsearchbar" class="formtext"><asp:Label ID="Label8" runat="server" 
            CssClass="tableheading" Text="Search: " meta:resourcekey="Label8Resource1"></asp:Label>
        <input type="text" id="ohsearchbox" class="ohsearch" />
        <a href="javascript:void(0);" onclick="onsearchbtnclicked('../reports/vehicleListTree.asmx/SearchOrganizationHierarchy');"><img src="../images/searchicon.png" border="0" /></a>
        <asp:Label ID="Label10" runat="server" style="color:#666666;display:none;" 
            Text="(Type in at least 3 characters to search)" 
            meta:resourcekey="Label10Resource1"></asp:Label>
    </div>
    <div id="ohsearchresult">
        <div id="ohsearchresulttitle">
            <asp:Label ID="Label2" runat="server" Text="Search Result:" 
                meta:resourcekey="Label2Resource1"></asp:Label>
             <a href="javascript:void(0)" onclick="$('#ohsearchresultlist ul').html('');$('#ohsearchresult').hide();$('#ohsearchbox').val('');">
            <asp:Label ID="Label3" runat="server" Text="Close" 
                meta:resourcekey="Label3Resource1"></asp:Label></a>
        </div>
        <div id="ohsearchresultlist">
            <ul></ul>
        </div>
    </div>
    <%if (LoadVehicles)
      { %> 
    <div id="MySplitter">
                                                   
        <div id="LeftPane">
		    <div id="vehicletreeview" class="demo"></div>		                                                
        </div>  
          
        <div id="RightPane">
            <div id="vehicledetails">
                <table cellspacing="0" class="vehiclelisttbl tablesorter" id="vehiclelisttbl">
                    <thead>
                    <tr>
                        <th><asp:Label ID="Label1" runat="server" Text="Vehicle" 
                                meta:resourcekey="Label1Resource1"></asp:Label></th>                                                                                                   
                    </tr>
                    </thead>
                    <tbody></tbody>
                    
                </table>
            </div>
            <div id="vehiclelistPageBar">
                <table>		            
		            <tr>
			            <td class="pager">
				            <img src="../scripts/tablesorter2145/addons/pager/icons/first.png" class="first" alt="First" />
				            <img src="../scripts/tablesorter2145/addons/pager/icons/prev.png" class="prev" alt="Prev" />
				            <span class="pagedisplay"></span> <!-- this can be any element, including an input -->
				            <img src="../scripts/tablesorter2145/addons/pager/icons/next.png" class="next" alt="Next" />
				            <img src="../scripts/tablesorter2145/addons/pager/icons/last.png" class="last" alt="Last" />				            
			            </td>
		            </tr>
	            </table>
            </div>
        </div>       
        
    </div> 
    <%}
      else
      { %>

      <div id="LeftPane">
		    <div id="vehicletreeview" class="demo"></div>		                                                
        </div> 

    <%} %>
    <div style="font-family: Verdana,sans-serif; font-size: 12px; margin-bottom: 5px;">
        <table border="0">
        <tr><td width="30" valign="top"><asp:Label ID="Label4" runat="server" Text="Path:" class="formtext" meta:resourcekey="Label4Resource1"></asp:Label>
        </td>
        <td id="fleetPath" width="600" height="35" valign="top">
        </td></tr>
        </table>
        <!--<div style="display: inline-block; width: 620px; height: 35px; vertical-align: top; overflow: hidden;" id="Div1"></div>-->
    </div>
    <div>
        <asp:Button ID="Button3" runat="server" CssClass="kd-button" 
            OnClientClick="applyOrganizationHierarchy();return false;" Text="OK" 
            meta:resourcekey="Button3Resource1" />
        <asp:Button ID="Button4" runat="server" CssClass="kd-button" 
            OnClientClick="cancelOrganizationHierarchy();return false;" Text="Cancel" 
            meta:resourcekey="Button4Resource1" />
        <!--<input type="button" class="kd-button" onclick="applyOrganizationHierarchy();" id="Button1" value="OK" />
        <input type="button" class="kd-button" onclick="cancelOrganizationHierarchy();" id="Button2" value="Cancel" />-->
    </div>
</div>
        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
    </form>
</body>
</html>
