<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HierarchyTree.ascx.cs" Inherits="SentinelFM.UserControl_HierarchyTree" %>
<link rel="stylesheet" href="<%=RootUrl%>scripts/tablesorter2145/css/theme.blue.css">
<link rel="stylesheet" href="<%=RootUrl%>scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css">
<link href="<%=RootUrl%>Reports/jqueryFileTree.css?v=20140313" rel="stylesheet" type="text/css" media="screen" />

<script src="<%=RootUrl%>scripts/jquery.cookie.js" type="text/javascript"></script>
<script src="<%=RootUrl%>Reports/jqueryFileTree.js?v=20150514" type="text/javascript"></script>
<script src="<%=RootUrl%>Reports/splitter.js" type="text/javascript"></script>
<script src="<%=RootUrl%>scripts/tablesorter2145/js/jquery.tablesorter.js?v=20140113" type="text/javascript"></script>
<script src="<%=RootUrl%>scripts/tablesorter2145/js/jquery.tablesorter.widgets.min.js" type="text/javascript"></script>
<script src="<%=RootUrl%>scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.js?v=20140306" type="text/javascript"></script>
<script src="<%=RootUrl%>scripts/colResizable-1.3.min.js" type="text/javascript"></script>

<link href="<%=RootUrl%>scripts/wdContextMenu/sample-css/page.css" rel="stylesheet" type="text/css" />    
<link href="<%=RootUrl%>scripts/wdContextMenu/css/contextmenu.css" rel="stylesheet" type="text/css" />
<script src="<%=RootUrl%>scripts/wdContextMenu/src/Plugins/jquery.contextmenu.js" type="text/javascript"></script>

<script language="javascript" type="text/javascript">

    var OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
    var MultipleUserHierarchyAssignment = <%=MultipleUserHierarchyAssignment.ToString().ToLower() %>;
    var PreferOrganizationHierarchyNodeCode = "<%=PreferOrganizationHierarchyNodeCode %>";
    var PreSelectHierarchy = <%=PreSelectHierarchy.ToString().ToLower() %>;
    var RootHierarchyNodeCodes = "<%=RootHierarchyNodeCodes %>";
    
    var HierarchyTree_rightWidth = 336;

    var RootUrl = "<%=RootUrl %>";

    var ht_selectedHierarchyNodeCode = "";
    
    function inifiletree(inipath, appendpath) {
            if(appendpath == undefined)
            {
                appendpath = false;
            }
            
            var selectedFolders = '';
            if(MultipleUserHierarchyAssignment && (PreSelectHierarchy || appendpath))
            {
                selectedFolders = $('#<%=OrganizationHierarchyNodeCode.ClientID %>').val();
                if(appendpath)
                {
                    var x_ps = inipath.split('/');
                    selectedFolders = selectedFolders + ',' + x_ps[x_ps.length-1];
                }
            }
            $('#vehicletreeview').fileTree({ 
                                                root: RootHierarchyNodeCodes
                                                , script: '<%=RootUrl%>Reports/vehicleListTree.asmx/FetchVehicleList'
                                                , scriptForPreferNodecodes: '<%=RootUrl%>Reports/vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes'
                                                , scriptForFetchVehicleByPage: '<%=RootUrl%>Reports/vehicleListTree.asmx/FetchVehicleListFilterByPage'
                                                , searchScript: '<%=RootUrl%>Reports/vehicleListTree.asmx/SearchOrganizationHierarchy'
                                                , expanded: inipath
                                                , expandSpeed: 200
                                                , collapseSpeed: 200
                                                , vehicledetails: '<%=this.LoadVehicleData?"vehicledetails":"" %>'//'vehicledetails'
                                                , highlightVehicleSelection: <%=OrganizationHierarchySelectVehicle.ToString().ToLower() %>
                                                , MultipleUserHierarchyAssignment: MultipleUserHierarchyAssignment
                                                , PreferOrganizationHierarchyNodeCode: PreferOrganizationHierarchyNodeCode                                                
                                                , FullTreeView: false 
                                                , VehicleListPagingBarId: 'vehiclelistPageBar'                                                
                                                , VehiclePageSize: <%=VehiclePageSize %>
                                                , SelectedFolders: selectedFolders
                                                , SelectedFleetIds: (MultipleUserHierarchyAssignment && appendpath) ? $('#OrganizationHierarchyFleetId').val() : ''
                                                , RootUrl: RootUrl
                                                , EnableContextMenu: true
                                                , ManagerColumn: <%=this.ManagerColumn.ToString().ToLower() %>
                                                , LoadVehicleData: <%=this.LoadVehicleData.ToString().ToLower() %>
                                            },
            /*
            * Call back function when you click left pane tree folder.
            */
                        function (NodeCode, FleetId, fleetName, fleetPath) {
                            ht_selectedHierarchyNodeCode = NodeCode;
                            if(!MultipleUserHierarchyAssignment)
                            {
                                $('#<%=OrganizationHierarchyNodeCode.ClientID %>').val(NodeCode);
                                $('#<%=OrganizationHierarchyFleetId.ClientID %>').val(FleetId);
                                $('#<%=OrganizationHierarchyFleetName.ClientID %>').val(fleetName);
                                $('#<%=OrganizationHierarchyBoxId.ClientID %>').val('');                                
                            }
                            $('#<%=OrganizationHierarchyVehicleDescription.ClientID %>').val('');
                            $('#fleetPath').html(fleetPath);
                        },

            /*
            * Call back function when you click right pane vehicle list.
            */
                        function (BoxId, vehicleDescription) {
                            $('#<%=OrganizationHierarchyBoxId.ClientID %>').val(BoxId);
                            $('#<%=OrganizationHierarchyVehicleDescription.ClientID %>').val(vehicleDescription);
                        },

                        function (selectedNodecodes, selectedFleetIds, fleetName, selectedFleetPaths)
                        {   
                            $('#<%=OrganizationHierarchyNodeCode.ClientID %>').val(selectedNodecodes);
                            $('#<%=OrganizationHierarchyFleetId.ClientID %>').val(selectedFleetIds);
                            $('#<%=OrganizationHierarchyFleetName.ClientID %>').val(fleetName);
                            <%if(HierarchyCheckCallBack != "") { %>
                                <%=HierarchyCheckCallBack %>(selectedNodecodes, selectedFleetIds, fleetName, selectedFleetPaths);
                            <% } %>
                        }
                    );
        }
        $(document).ready(function () {
            //try {
                var sizeLeft = $('#tblHierarchyTree').width() - HierarchyTree_rightWidth;
                <%if(LoadVehicleData) { %>
                $("#MySplitter").splitter({
                    type: "v",
                    outline: true,
                    minLeft: 100, sizeLeft: sizeLeft/*385*/, minRight: 100,
                    resizeToWidth: true,
                    //cookie: "vsplitter13",
                    accessKey: 'I'
                });
                <%} %>
                inifiletree(OrganizationHierarchyPath);

                /*$('#vehiclelisttbl').dataTable({
                "bPaginate": false,
                "bLengthChange": false,
                "bFilter": false,
                "bInfo": false
                });*/
                //$('#vehiclelisttbl').tablesorter();
                //$('#vehiclelisttbl').colResizable({ headerOnly: true });
            //}
            //catch (ex) { }
        });

        <%if (this.HierarchyEditMode)
                                  { %>
             
         var msgNoHierarchySelected = '<%=msgNoHierarchySelected %>';
         var msgConfirmDelete = '<%=msgConfirmDelete %>';
         var msgNodeCodeRequired = '<%=msgNodeCodeRequired %>';
         var msgNodeNameRequired = '<%=msgNodeNameRequired %>';
         var msgAddNodeCodeSuccess = '<%=msgAddNodeCodeSuccess %>';
         var msgAddNodeCodeFail = '<%=msgAddNodeCodeFail %>';
         var msgDeleteNodeCodeSuccess = '<%=msgDeleteNodeCodeSuccess %>';
         var msgDeleteNodeCodeFail = '<%=msgDeleteNodeCodeFail %>';
         
         function addhierarchy() {
             

             if (ht_selectedHierarchyNodeCode == '') {
                 //alert('Please select a hierarchy to be added to.');
                 //alert(msgNoHierarchySelectedForAdd);
                 $('#<%=lblMessage.ClientID %>').html(msgNoHierarchySelected).css('color','red');
                 return;
             }

             var id = '<%=btnAddHierarchy.ClientID %>';
             $('#divAddHierarchy').css('left', $('#' + id).offset().left - 30).css('top', $('#' + id).offset().top - 10).show();
         }

         function deletehierarchy() {
             if (ht_selectedHierarchyNodeCode == '') {
                 $('#<%=lblMessage.ClientID %>').html(msgNoHierarchySelected).css('color','red');
                 return;
             }
             if (!confirm(msgConfirmDelete)) {
                 return;
             }
             $.ajax({
                 type: 'POST',
                 url: '<%=RootUrl%>Reports/vehicleListTree.asmx/DeleteOrganizationHierarchyNode',
                 data: JSON.stringify({ nodeCode: ht_selectedHierarchyNodeCode }),
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 async: false,
                 success: function (msg) {

                     var r = eval('(' + msg.d + ')');
                     if (r.status == 200) {
                         
                         cancelAddhierarchy();

                         inifiletree(r.resultList);
                         $('#<%=lblMessage.ClientID %>').html(msgDeleteNodeCodeSuccess).css('color','green');
                     }
                     else {
                         $('#<%=lblMessage.ClientID %>').html(msgDeleteNodeCodeFail).css('color','red');
                     }
                 },
                 error: function (msg) {
                     $('#<%=lblMessage.ClientID %>').html(msgDeleteNodeCodeFail).css('color','red');
                 }
             });
         }

         function addhierarchyAction() {
             if ($('#txtNewHierarchyNodeCode').val().trim() == '') {
                 $('#<%=lblMessage.ClientID %>').html(msgNodeCodeRequired).css('color','red');
                 return;
             }
             if ($('#txtNewHierarchyNodeName').val().trim() == '') {
                 $('#<%=lblMessage.ClientID %>').html(msgNodeNameRequired).css('color','red');
                 return;
             }

             $.ajax({
                 type: 'POST',
                 url: '<%=RootUrl%>Reports/vehicleListTree.asmx/AddOrganizationHierarchyNode',
                 data: JSON.stringify({ nodeCode: $('#txtNewHierarchyNodeCode').val(), nodeName: $('#txtNewHierarchyNodeName').val(), parentNodeCode: ht_selectedHierarchyNodeCode }),
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 async: false,
                 success: function (msg) {

                     var r = eval('(' + msg.d + ')');
                     if (r.status == 200) {
                         
                         cancelAddhierarchy();
                         
                         inifiletree(r.resultList);
                         $('#<%=lblMessage.ClientID %>').html(msgAddNodeCodeSuccess).css('color','green');
                     }
                     else {
                        if(r.resultList != '')
                            $('#<%=lblMessage.ClientID %>').html(r.resultList).css('color','red');
                        else
                            $('#<%=lblMessage.ClientID %>').html(msgAddNodeCodeFail).css('color','red');
                     }
                 },
                 error: function (msg) {
                     $('#<%=lblMessage.ClientID %>').html(msgAddNodeCodeFail).css('color','red');
                 }
             });
         }

         function cancelAddhierarchy() {
             $('#txtNewHierarchyNodeCode').val('');
             $('#txtNewHierarchyNodeName').val('');
             $('#txtNewHierarchyNodeDescription').val('');
             $('#divAddHierarchy').hide();
             $('#<%=lblMessage.ClientID %>').html('');
         }
        <% } %>
</script>

<asp:HiddenField ID="OrganizationHierarchyNodeCode" runat="server" />
<input type="hidden" name="OrganizationHierarchyFleetId" id="OrganizationHierarchyFleetId" runat="server" />
<input type="hidden" name="OrganizationHierarchyFleetName" id="OrganizationHierarchyFleetName" runat="server" />
<asp:HiddenField ID="OrganizationHierarchyBoxId" runat="server" />
<asp:HiddenField ID="OrganizationHierarchyVehicleDescription" runat="server" />
    <table id="tblHierarchyTree" width="<%=Width %>"><tr>
                        <td colspan="10" align="left">
                            <div id="ohsearchbar" class="formtext"><asp:Label ID="Label8" runat="server" 
                                    CssClass="formtextGreen" Text="Search Cost Center: " 
                                    meta:resourcekey="Label8Resource1"></asp:Label>
                                <input type="text" id="ohsearchbox" class="ohsearch" onkeypress="return event.keyCode != 13;" />
                                <a href="javascript:void(0);" onclick="onsearchbtnclicked('<%=RootUrl%>Reports/vehicleListTree.asmx/SearchOrganizationHierarchy');"><img src="<%=RootUrl%>images/searchicon.png" border="0" /></a>
                                <%if (this.HierarchyEditMode)
                                  { %>
                                  <asp:Button CssClass="ht_kd-button" OnClientClick="addhierarchy();return false;" 
                                                                                            ID="btnAddHierarchy" runat="server" Text="Add Hierarchy" style="width:140px;"
                                                                                            meta:resourcekey="Button1Resource1" />
                                <asp:Button CssClass="ht_kd-button"  style="width:140px;"
                                    OnClientClick="deletehierarchy();return false;" ID="btnDeleteHierarchy" runat="server" 
                                    Text="Delete Hierarchy" meta:resourcekey="Button2Resource1" />
                                <% }
                                  else
                                  { %>
                                <asp:Label ID="Label10" runat="server" style="color:#666666;" 
                                    Text="(Type in at least 3 characters to search)" 
                                    meta:resourcekey="Label10Resource1"></asp:Label>
                                <% } %>
                            </div>
                            <div id="ohsearchresult">
                                <div id="ohsearchresulttitle">
                                    <asp:Label ID="Label1" runat="server" style="color:#666666;" 
                                        Text="Search Result:" meta:resourcekey="Label1Resource1"></asp:Label> <a href="javascript:void(0)" onclick="$('#ohsearchresultlist ul').html('');$('#ohsearchresult').hide();$('#ohsearchbox').val('');">
                                    <asp:Label ID="Label2" runat="server" style="color:#666666;" Text="Close" 
                                        meta:resourcekey="Label2Resource1"></asp:Label></a>
                                </div>
                                <div id="ohsearchresultlist">
                                    <ul></ul>
                                </div>
                            </div>
                            <%if (LoadVehicleData)
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
                                                <th><asp:Label ID="lblVehicle" Text='Vehicle'  runat="server" 
                                                        meta:resourcekey="lblVehicleResource1"  ></asp:Label></th>
                                                <%if (this.ManagerColumn)
                                                  { %>
                                                <th>Manager</th>          
                                                <%} %>                                      
                                                                                                                                                                                                     
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
                                <%} else
                                  { %>

                                  <div id="LeftPaneWithoutVehicle">
		                                <div id="vehicletreeview" class="demo"></div>		                                                
                                    </div> 

                                <%} %>
                                <div style="font-family: Verdana,sans-serif; font-size: 12px; margin-bottom: 5px;">
                                    <table border="0">
                                        <tr>
                                            <td width="30" valign="top"><asp:Label ID="Label400" runat="server" Text="Path:" 
                                                    class="formtext" meta:resourcekey="Label400Resource1"></asp:Label></td>
                                            <td id="fleetPath" width="600" height="35" valign="top" class="formtext" style="word-wrap:break-word;"></td>
                                        </tr>
                                        <%if (this.HierarchyEditMode)
                                          { %>
                                        <tr>
                                            <td align="left" style="height: 15px" colspan="2">
                                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Height="20px" Width="615px" style="margin-top:5px;" meta:resourcekey="lblMessageResource1"></asp:Label>
                                            </td>
                                        </tr>
                                        <% } %>
                                    </table>
                                    <!--<div style="display: inline-block; width: 620px; height: 35px; vertical-align: top; overflow: hidden;" id="Div1"></div>-->
                                </div>
                                                
                        </td>
                        </tr></table>


<%if (this.HierarchyEditMode)
                                  { %>
 <div id="divAddHierarchy" class="popupdiv">        
        <div class="formfield">
            <table border="0" style="margin-left:15px;">
                <tr>
                    <td height="28">
                        <asp:label ID="Label3" runat="server" text="Node Code:" 
                            meta:resourcekey="LabelNodeCode"></asp:label>
                    </td>
                    <td>
                        <input id="txtNewHierarchyNodeCode" type="text" class="ohtextbox" />
                    </td>
                </tr>
                <tr>
                    <td height="28">
                        <asp:label ID="Label4" runat="server" text="Node Name:" 
                            meta:resourcekey="LabelNodeName"></asp:label>
                    </td>
                    <td>
                        <input id="txtNewHierarchyNodeName" type="text" class="ohtextbox" />
                    </td>
                </tr>
                <tr>
                    <td height="40" valign="bottom" colspan="2">
                        <asp:Button CssClass="ht_kd-button" 
                            OnClientClick="addhierarchyAction();return false;" ID="Button1" runat="server" 
                            Text="OK"  style="width:75px;" meta:resourcekey="Button1Resource2" />
                        <asp:Button CssClass="ht_kd-button" 
                            OnClientClick="cancelAddhierarchy();return false;" ID="Button2" runat="server" 
                            Text="Cancel"  style="width:75px;" meta:resourcekey="Button2Resource2" />
                    </td>
                </tr>
            </table>                
        </div>        
    </div>
<%} %>