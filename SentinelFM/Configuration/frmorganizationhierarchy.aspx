<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmorganizationhierarchy.aspx.cs" Inherits="SentinelFM.Configuration_frmorganizationhierarchy" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register src="Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<%@ Register src="Components/ctlOrganizationSubMenuTabs.ascx" tagname="ctlOrganizationSubMenuTabs" tagprefix="ucSubmenu" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
	<meta content="C#" name="CODE_LANGUAGE" />
	<meta content="JavaScript" name="vs_defaultClientScript" />
	<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />

    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" /> 
    <link rel="stylesheet" type="text/css" href="../Styles/SentinelFM/css/simplePagination.css" /> 
    <script type="text/javascript" src="../sencha/extjs-4.1.0/ext-all.js"></script>   
    
    
    
	<link href="../reports/jqueryFileTree.css" rel="stylesheet" type="text/css" media="screen" />

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
            /*margin-right:16px;*/
            
            border-bottom: 1px solid #CCCCCC;            
            height: 260px;
            margin-bottom: 0;
            margin-right: 0;
            overflow: auto;            
        }
        
        #vehiclelistPageBar
        {
            padding: 5px 0 0 5px;  
            height: 31px; 
            overflow: hidden;                   
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

   
    		       
</head>
<body>
<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery.cookie.js"></script>
    <script src="../reports/jqueryFileTree.js?v=2015070202" type="text/javascript"></script>
    <script src="../reports/splitter.js?v=2013120601" type="text/javascript"></script>
    
    <script src="../scripts/tablesorter2145/js/jquery.tablesorter.js"></script>
	<script src="../scripts/tablesorter2145/js/jquery.tablesorter.widgets.min.js"></script>
    <link rel="stylesheet" href="../scripts/tablesorter2145/css/theme.blue.css">
    <!-- Tablesorter pager: required -->
	<link rel="stylesheet" href="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css">
	<script src="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.js"></script>

    <script src="../scripts/colResizable-1.3.min.js" type="text/javascript"></script>
    <script src="../scripts/json2.js" type="text/javascript"></script>
    

     <script type="text/javascript">
         var OrganizationHierarchyPath = "";
         var selectedHierarchyNodeCode = "";
         var msgNoHierarchySelected = '<%=msgNoHierarchySelected %>';
         var msgConfirmDelete = '<%=msgConfirmDelete %>';
         var msgNodeCodeRequired = '<%=msgNodeCodeRequired %>';
         var msgNodeNameRequired = '<%=msgNodeNameRequired %>';
         var msgAddNodeCodeSuccess = '<%=msgAddNodeCodeSuccess %>';
         var msgAddNodeCodeFail = '<%=msgAddNodeCodeFail %>';
         var msgDeleteNodeCodeSuccess = '<%=msgDeleteNodeCodeSuccess %>';
         var msgDeleteNodeCodeFail = '<%=msgDeleteNodeCodeFail %>';
         var MultipleUserHierarchyAssignment = <%=MultipleUserHierarchyAssignment.ToString().ToLower() %>;
         var RootOrganizationHierarchyNodeCode = '<%=RootOrganizationHierarchyNodeCode %>';

         function inifiletree(inipath) {
             $('#vehicletreeview').fileTree({ root: MultipleUserHierarchyAssignment?RootOrganizationHierarchyNodeCode:'', script: '../reports/vehicleListTree.asmx/FetchVehicleList',
                 expanded: inipath, expandSpeed: 200, collapseSpeed: 200,
                 vehicledetails: 'vehicledetails',
                 highlightVehicleSelection: false,
                 searchScript: '../reports/vehicleListTree.asmx/SearchOrganizationHierarchy'
                 , VehicleListPagingBarId: 'vehiclelistPageBar'
                 , scriptForFetchVehicleByPage: '../reports/vehicleListTree.asmx/FetchVehicleListFilterByPage'
                 , scriptForPreferNodecodes: '../reports/vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes'
                 , VehiclePageSize: <%=VehiclePageSize %>

             },
             /*
             * Call back function when you click left pane tree folder.
             */
                        function (NodeCode) {
                            //$('#OrganizationHierarchyNodeCode').val(NodeCode);
                            //$('#OrganizationHierarchyBoxId').val('');
                            selectedHierarchyNodeCode = NodeCode;
                            $('#lblMessage').html('');
                        },

             /*
             * Call back function when you click right pane vehicle list.
             */
                        function (BoxId) {
                            //alert('BoxId: ' + BoxId);
                            //$('#OrganizationHierarchyBoxId').val(BoxId);                            
                        }
                    );
         }

         $(document).ready(function () {
             $("#MySplitter").splitter({
                 type: "v",
                 outline: true,
                 minLeft: 100, sizeLeft: 359, minRight: 100,
                 resizeToWidth: true,
                 cookie: "vsplitterj",
                 accessKey: 'I'
             });

             inifiletree(OrganizationHierarchyPath);

             /*$('#vehiclelisttbl').dataTable({
             "bPaginate": false,
             "bLengthChange": false,
             "bFilter": false,
             "bInfo": false
             });*/
             $('#vehiclelisttbl').tablesorter();
             $('#vehiclelisttbl').colResizable({ headerOnly: true });

         });

         function addhierarchy() {
             if (selectedHierarchyNodeCode == '') {
                 //alert('Please select a hierarchy to be added to.');
                 //alert(msgNoHierarchySelectedForAdd);
                 $('#lblMessage').html(msgNoHierarchySelected);
                 return;
             }

             var id = 'btnAddHierarchy';
             $('#divAddHierarchy').css('left', $('#' + id).offset().left - 20).css('top', $('#' + id).offset().top + 28).show();
         }

         function deletehierarchy() {
             if (selectedHierarchyNodeCode == '') {
                 $('#lblMessage').html(msgNoHierarchySelected);
                 return;
             }
             if (!confirm(msgConfirmDelete)) {
                 return;
             }
             $.ajax({
                 type: 'POST',
                 url: '../Reports/vehicleListTree.asmx/DeleteOrganizationHierarchyNode',
                 data: JSON.stringify({ nodeCode: selectedHierarchyNodeCode }),
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 async: false,
                 success: function (msg) {

                     var r = eval('(' + msg.d + ')');
                     if (r.status == 200) {
                         
                         cancelAddhierarchy();

                         inifiletree(r.resultList);
                         $('#lblMessage').html(msgDeleteNodeCodeSuccess);
                     }
                     else {
                         $('#lblMessage').html(msgDeleteNodeCodeFail);
                     }
                 },
                 error: function (msg) {
                     $('#lblMessage').html(msgDeleteNodeCodeFail);
                 }
             });
         }

         function addhierarchyAction() {
             if ($('#txtNewHierarchyNodeCode').val().trim() == '') {
                 $('#lblMessage').html(msgNodeCodeRequired);
                 return;
             }
             if ($('#txtNewHierarchyNodeName').val().trim() == '') {
                 $('#lblMessage').html(msgNodeNameRequired);
                 return;
             }

             $.ajax({
                 type: 'POST',
                 url: '../Reports/vehicleListTree.asmx/AddOrganizationHierarchyNode',
                 data: JSON.stringify({ nodeCode: $('#txtNewHierarchyNodeCode').val(), nodeName: $('#txtNewHierarchyNodeName').val(), parentNodeCode: selectedHierarchyNodeCode }),
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 async: false,
                 success: function (msg) {

                     var r = eval('(' + msg.d + ')');
                     if (r.status == 200) {
                         
                         cancelAddhierarchy();
                         
                         inifiletree(r.resultList);
                         $('#lblMessage').html(msgAddNodeCodeSuccess);
                     }
                     else {
                        if(r.resultList != '')
                            $('#lblMessage').html(r.resultList);
                        else
                            $('#lblMessage').html(msgAddNodeCodeFail);
                     }
                 },
                 error: function (msg) {
                     $('#lblMessage').html(msgAddNodeCodeFail);
                 }
             });
         }

         function cancelAddhierarchy() {
             $('#txtNewHierarchyNodeCode').val('');
             $('#txtNewHierarchyNodeName').val('');
             $('#txtNewHierarchyNodeDescription').val('');
             $('#divAddHierarchy').hide();
             $('#lblMessage').html('');
         }
        
    </script>

    <form id="form1" runat="server">
    <table id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellSpacing="0"
				    cellPadding="0" width="300" border="0">
	        <tr>
		        <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdOrganization" />
		        </td>
	        </tr>
            <TR>
					<TD>
						<TABLE id="TABLE2" cellSpacing="0" cellPadding="0" width="990" align="center" border="0">
							<TR>
								<TD>
									<TABLE id="tblForm" class=table width="1060" border="0" style="height:450px;overflow:hidden;">
										<TR>
											<TD class="configTabBackground" valign="top">
												<TABLE id="Table3" style="LEFT: 10px; POSITION: relative; TOP: 10px" cellSpacing="0" cellPadding="0"
													width="300" border="0">
													<TR>
														<TD>
															<TABLE id="tblSubCommands" cellSpacing="0" cellPadding="0" width="300" border="0">
																<TR>
																	<TD>
																		<ucSubmenu:ctlOrganizationSubMenuTabs ID="ctlSubMenuTabs" runat="server" SelectedControl="cmdHierarchy" />
																	</TD>
																</TR>
																<TR>
																	<TD  >
																		<table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="690">
                                                                            <tr>
                                                                                <td>
                                                                                    <div id="ohsearchbar" class="formtext" style="margin-top:20px;">
                                                                                        <asp:Label ID="Label8" runat="server" CssClass="tableheading" 
                                                                                            Text="Search: " meta:resourcekey="Label8Resource1"></asp:Label>
                                                                                        <input type="text" id="ohsearchbox" class="ohsearch" />
                                                                                        <a href="javascript:void(0);" onclick="onsearchbtnclicked('../reports/vehicleListTree.asmx/SearchOrganizationHierarchy');"><img src="../images/searchicon.png" border="0" /></a>
                                                                                        <asp:Label ID="Label10" runat="server" style="color:#666666;display:none;" 
                                                                                            Text="(Type in at least 3 characters to search)" 
                                                                                            meta:resourcekey="Label10Resource1"></asp:Label>

                                                                                        <asp:Button CssClass="kd-button" OnClientClick="addhierarchy();return false;" 
                                                                                            ID="btnAddHierarchy" runat="server" Text="Add Hierarchy" style="width:100px;"
                                                                                            meta:resourcekey="Button1Resource1" />
                                                                                        <asp:Button CssClass="kd-button"  style="width:120px;"
                                                                                            OnClientClick="deletehierarchy();return false;" ID="btnDeleteHierarchy" runat="server" 
                                                                                            Text="Delete Hierarchy" meta:resourcekey="Button2Resource1" />

                                                                                        
                                                                                    </div>
                                                                                    <div id="ohsearchresult" calss="popupdiv">
                                                                                        <div id="ohsearchresulttitle">
                                                                                            <asp:Label ID="Label4" runat="server" Text="Search Result:" 
                                                                                                meta:resourcekey="Label4Resource1"></asp:Label> <a href="javascript:void(0)" onclick="$('#ohsearchresultlist ul').html('');$('#ohsearchresult').hide();$('#ohsearchbox').val('');">
                                                                                                <asp:Label ID="Label5" runat="server" Text="Close" 
                                                                                                meta:resourcekey="Label5Resource1"></asp:Label></a>
                                                                                        </div>
                                                                                        <div id="ohsearchresultlist">
                                                                                            <ul></ul>
                                                                                        </div>
                                                                                    </div>
                                                                                    <div id="MySplitter">
                                                   
                                                                                       <div id="LeftPane">
			                                                                                <div id="vehicletreeview" class="demo"></div>		                                                
                                                                                       </div>     
                                                                                       <div id="RightPane">
                                                                                            <div id="vehicledetails">
                                                                                                <table cellspacing="0" class="vehiclelisttbl tablesorter" id="vehiclelisttbl">
                                                                                                    <thead>
                                                                                                    <tr>
                                                                                                        <th><asp:Label ID="Label3" runat="server" Text="Vehicle" 
                                                                                                                meta:resourcekey="Label3Resource1"></asp:Label></th>
                                                                                                                                                                                                     
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
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td align="left" style="height: 15px">
                                                                                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Height="20px" Width="615px" style="margin-top:20px;" meta:resourcekey="lblMessageResource1"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
																	</TD>
																</TR>
															</TABLE>
														</TD>
													</TR>
												</TABLE>
											</TD>
										</TR>
									</TABLE>
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
    </table>


    <div id="divAddHierarchy" class="popupdiv">        
        <div class="formfield">
            <table border="0" style="margin-left:15px;">
                <tr>
                    <td height="28">
                        <asp:label ID="Label2" runat="server" text="Node Code:" 
                            meta:resourcekey="Label2Resource1"></asp:label>
                    </td>
                    <td>
                        <input id="txtNewHierarchyNodeCode" type="text" class="ohtextbox" />
                    </td>
                </tr>
                <tr>
                    <td height="28">
                        <asp:label ID="Label1" runat="server" text="Node Name:" 
                            meta:resourcekey="Label1Resource1"></asp:label>
                    </td>
                    <td>
                        <input id="txtNewHierarchyNodeName" type="text" class="ohtextbox" />
                    </td>
                </tr>
                <tr>
                    <td height="40" valign="bottom" colspan="2">
                        <asp:Button CssClass="kd-button" 
                            OnClientClick="addhierarchyAction();return false;" ID="Button1" runat="server" 
                            Text="OK"  style="width:75px;" meta:resourcekey="Button1Resource2" />
                        <asp:Button CssClass="kd-button" 
                            OnClientClick="cancelAddhierarchy();return false;" ID="Button2" runat="server" 
                            Text="Cancel"  style="width:75px;" meta:resourcekey="Button2Resource2" />
                    </td>
                </tr>
            </table>                
        </div>        
    </div>


    </form>

    
</body>
</html>

