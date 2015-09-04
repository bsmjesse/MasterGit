<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="ServiceAssignment_Main" %>
<%@ Register src="../UserControl/HierarchyTree.ascx" tagname="HierarchyTree" tagprefix="uc4" %>
<%@ Import Namespace="System.Collections.Generic" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Service Assignment</title>
    <link rel="stylesheet" href="Scripts/media/smoothness/jquery-ui.css" />
    <link rel="stylesheet" href="Scripts/media/css/demo_table.css" />  
    <link rel="stylesheet" href="Scripts/media/css/TableTools.css" /> 
    <link rel="stylesheet" href="Scripts/media/css/jquery.ui.timepicker.css" /> 
  <script type="text/javascript" src="Scripts/ui/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Scripts/ui/jquery-migrate-1.1.0.js"></script>
  <script type="text/javascript" src="Scripts/ui/jquery-ui.js"></script>
    <script type="text/javascript" src="Scripts/ckeditor/ckeditor.js"></script>
    <script src="Scripts/media/js/jquery.dataTables.js" type="text/javascript"></script>
    <script type="text/javascript" charset="utf-8" src="Scripts/media/js/ZeroClipboard.js"></script>
	<script type="text/javascript" charset="utf-8" src="Scripts/media/js/TableTools.js"></script>    
    <script type="text/javascript" src="Scripts/editableselect/jquery.eComboBox.min.js"></script>
    <script type="text/javascript" src="Scripts/timepicker/jquery.ui.timepicker.js"></script>
    <script type="text/javascript" src="Scripts/ServiceAssignment.js"></script>
    <style type="text/css">
    body { font-size: 62.5%; }
    label, input { display:inline; }
    input.text { margin-bottom:12px; width:95%; padding: .4em; }
    fieldset { padding:0; border:0; margin-top:25px; }
    h1 { font-size: 1.2em; margin: .6em 0; }
    div#users-contain { width: 350px; margin: 20px 0; }
    div#users-contain table { margin: 1em 0; border-collapse: collapse; width: 100%; }
    div#users-contain table td, div#users-contain table th { border: 1px solid #eee; padding: .6em 10px; text-align: left; }
    .ui-dialog .ui-state-error { padding: .3em;}
    .validateTips { border: 1px solid transparent; padding: 0.3em; }  
    .ui-assignment { padding: .3em;}  
   
    .ui-tooltip {
        width: 210px;
    }
        .include {
            color: green;
            font-weight: bold;
        }
        .exclude {
            color: red;
            font-weight: bold;
        }
        .sample {
            color:green;
        }
    .editableSelect select {
        width: 55px;
        float: left;
    }
    .editableSelect input {
        width: 40px;
        margin-left: -199px;
        margin-top: 1px; 
        border: none; 
        float: left;
    }

    .resizedTextbox {width: 40px;}
     .fixedTextbox {width: 40px;}
    .resizedLabel {width: 25px;}
     .hideTextbox {width: 0px; visibility:hidden}
       .disabledTextbox {width: 0px; disabled:false}
  </style>
    <script type="text/javascript">
        var configuredServices = {};
        var toolTips = {};
        var assignmentWith = "<%=AssignmentWidth%>";
        <% if (ConfiguredServices != null)
       {
           if (ConfiguredServices.Count > 0)
           {
               foreach (Dictionary<string, string> configuredService in ConfiguredServices)
               {
  
               
    %>        
        configuredServices["<%= configuredService["ServiceConfigID"]%>"] = { "RulesApplied": "<%= configuredService["RulesApplied"]%>", "ServiceConfigName": "<%= configuredService["ServiceConfigName"]%>", "ExpiredDate": "<%= configuredService["ExpiredDate"]%>", "RecipientsList": "<%= configuredService["RecipientsList"]%>", "EmailLevel":  "<%= configuredService["EmailLevel"]%>", "Subject": '<%= configuredService["Subject"]%>', "Message": '<%= Server.HtmlDecode(configuredService["Message"]).Replace("\r\n", "")%>', 'IsActive' : '<%=configuredService["IsActive"]%>', 'IsReportable': '<%=configuredService["IsReportable"]%>'};
        
        <%
               }
           }
       } %>
        

        <%
        if (Rules != null)
        {
            if (Rules.Count > 0)
            {
                foreach (Dictionary<string, string> rule in Rules)
                {
        %>
        toolTips["<%=rule["RuleName"]%>"] = {"ToolTip": "<%=rule["ToolTip"]%>", "MustInclude": "<%=rule["MustInclude"]%>", "MustExclude": "<%=rule["MustExclude"]%>", "Sample": "<%=rule["Sample"]%>"};
        <%
                }
            }
        }
        %>
        var TabWidth = "<%= TabWidth %>";
        g(function() {
            g("#tabs").tabs();
            g("#tabs").tabs("option", "active", <%= TabOrder %>);
            
            g('#tabs').tabs({
                activate: function (event, ui){
                    // Do stuff here
                    var selected = ui.newTab.context.id;                    
                    if (selected == 'ui-id-7') {
                        g('#tabs').css("width", TabWidth + "px");
                    } else {                        
                        g('#tabs').css("width", "400px");
                   }
                }
            });
        });

        var $ = jQuery.noConflict();
    </script>
</head>
<body>
    <form id="ServiceConfigForm" name="ServiceConfigForm" runat="server" defaultbutton="btnDisableEnter">
        
        <div id="tabs" style="width: <%= TabWidth %>px;">
        <ul>
            <li><a href="#tabs-1">Services</a></li>
            <li><a href="#tabs-2">Configured Services</a></li>
            <li><a href="#tabs-3">Assignment</a></li>
        </ul>
        <div id="tabs-1">
            <table>
                <tr>
                    <td>
                       <asp:ListBox ID="ServicesList" runat="server" Width="200" Height="300" AutoPostBack="True"
                    OnSelectedIndexChanged="ServicesList_SelectedIndexChanged"></asp:ListBox>
                    </td>
                </tr>                
            </table>
        </div>
        <div id="tabs-2">
            <table>
                <tr>
                    <td>
                        <asp:ListBox ID="RulesList" runat="server" Width="200" Height="300"></asp:ListBox>
                        
                    </td>
                    <td>
                        <table>
                            <tr>
                        <td>
                            <asp:Button ID="btnAssignVehciles" runat="server" Text="Assign to Vehicles" OnClick="AssignToVehicles" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnAssignFleets" runat="server" Text="Assign to Fleets" OnClick="AssignToFleets" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnAssignLandmarks" runat="server" Text="Assign to Landmarks" OnClick="AssignToLandmarks" />
                        </td>
                    </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                         <input type="button" id="btnCreateRule" name="btnCreateRule" value="Create a rule" />
                         <input type="button" id="btnEditRule" name="btnEditRule" value="View or edit a rule" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="tabs-3">
            <table>
                
                <tr>            
            <td align="center"  style="font-size: 18px;">
            <asp:Label ID="ObjectListLabel" runat="server" Text="Objects List"></asp:Label>
            </td>
            <td></td>
            <asp:PlaceHolder ID="IncludesListTitlePlaceholder" runat="server" Visible="False">
                <td align="center"  style="font-size: 18px;"><asp:Label ID="IncludesListLabel" runat="server" Text="Includes List"></asp:Label></td>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="FleetVehiclesTitlePlaceholder" runat="server" Visible="False">
                <td></td>
                <td align="center"  style="font-size: 18px;">Fleet Vehicles</td>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="ExcludesListTitlePlaceholder" runat="server" Visible="False">
                <td></td>
                <td align="center"  style="font-size: 18px;">Excludes List</td>
            </asp:PlaceHolder>
            </tr>

                <tr>
                    
                    <td>
                        <div>
                    <% if (!Convert.ToBoolean(ViewState["HierarchyView"]))
                       {
                           
                    %>
                             <asp:ListBox ID="ObjectsList" runat="server" Width="200" Height="300" SelectionMode="Multiple"></asp:ListBox>                            
                    <%
                        }
                       else
                       {
                   %>
                            <uc4:HierarchyTree ID="HierarchyTree" GetRootHierarchyBy="userpreference" PreSelectHierarchy="true" OrganizationHierarchyPath="existingselection" HierarchyEditMode="false" Width="100%" LoadVehicleData="false" runat="server" />
                   <%         
                       }
                    %>
                   
                        </div>
                        <div>
                   <%
                       if (!Convert.ToBoolean(ViewState["HierarchyView"]))
                       {
                   %>
                            <asp:Button ID="btnHierarchySwitcher" runat="server" Text="Hierarchy View" OnClick="SwitchToHierarchy" />
                    <%
                       }
                       else
                       {
                    %>
                             <asp:Button ID="Button1" runat="server" Text="List View" OnClick="SwitchToList" />
                    <%               
                       }
                    %>
                        </div>
                    </td>
                    
                
                
                <td>
                    
                    <table>                    
                        <asp:PlaceHolder ID="showVehicles" runat="server" Visible="False">
                    
                        <tr>
                            <td>                                
                                <asp:Button ID="btnShowVehicles" runat="server" Text="Choose Vehicles--->" OnClick="AddVehiclesAction" />
                            </td>
                        </tr>
                    
                        <tr>
                            <td>
                                <asp:Button ID="btnRemoveVehicles" runat="server" Text="<---Exclude Vehicles" OnClick="DeleteIncludedVehiclesAction" />
                            </td>
                        </tr>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="showFleets" runat="server" Visible="False">
                        <tr>
                            <td>
                                <asp:Button ID="btnIncludeFleets" runat="server" Text="Choose Fleets-->" OnClick="AddFleetsAction" />
                            </td>
                        </tr>                   
                            <tr>
                                <td>
                                     <asp:Button ID="btnExcludeFleets" runat="server" Text="<--Remove Fleets" OnClick="RemoveFleetsAction" />
                                </td>
                                </td>
                            </tr>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="landmarksPlaceholder" runat="server" Visible="False">
                        <tr>
                            <td>
                               <asp:Button ID="btnIncludeLandmarks" runat="server" Text="Choose Landmarks-->" OnClick="AddLandmarksAction" />
                            </td>
                        </tr>                   
                            <tr>
                                <td>
                                    <asp:Button ID="btnExcludeLandmarks" runat="server" Text="<--Remove Landmarks" OnClick="RemoveLandmarksAction" />
                                </td>
                            </tr>
                        </asp:PlaceHolder>

                        </table>
                    </td>

                <asp:PlaceHolder ID="showIncludeList" runat="server" Visible="False">
                <td>                    
                    <asp:ListBox ID="IncludeList" runat="server" Width="150" Height="300" SelectionMode="Multiple"></asp:ListBox>
                </td>
                </asp:PlaceHolder>
                
                <asp:PlaceHolder ID="showFleetsVehiclePlaceholder" runat="server" Visible="False">
                    <td>
                        <table>
                            <tr>
                                <td>
                                     <asp:Button ID="btnShowFleetsVehicles" runat="server" Text="Show Vehicles-->" OnClick="ShowFleetsVehiclesAction" />                                    
                                </td>
                            </tr>                            

                        </table>
                    </td>
                    <td>                                        
                        <asp:ListBox ID="FleetsVehicleList" runat="server" Width="150" Height="300"></asp:ListBox>
                    </td>
                </asp:PlaceHolder>

            <asp:PlaceHolder ID="showExclude" Visible="False" runat="server">
                <td valign="middle">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="btnExclude" runat="server" Text="Exclude Vehicles-->" OnClick="ExcludeAction" />            
                            </td>
                        </tr>
                        
                        <tr>
                            <td>
                                <asp:Button ID="btnRemoveExclude" runat="server" Text="<--Remove Excluded Vehicles" OnClick="RemoveExcludeAction" />            
                            </td>
                        </tr>
                    </table>
                    
                </td>
                <td>
                    <asp:ListBox ID="ExcludeList" runat="server" Width="150" Height="300"></asp:ListBox>
                </td>
            </asp:PlaceHolder>
              </tr>  

            </table>
                
        </div>
        </div>
     <asp:Button ID="btnSaveConfig" runat="server" Text="Save Configured Service" OnClick="SaveConfigAction" /> 
      <input type="button" id="btnViewAssignments" name="btnViewAssignments" value="Assignments"/>

    <table>
        
        
    </table>
    
    <div id="dialog-modal" title="Service Configuration" style="display: none;">        
            <table>
                <tr>
                    <td align="left" width="30%">
                        Configuration Name:                                            
                    </td>
                    <td>
                        <asp:TextBox ID="ruleName" name="ruleName" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" width="30%">
                        Active Service:                                            
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" name="chkActive" runat="server" value="1" Checked="false"></asp:CheckBox>
                    </td>
                </tr>
		<tr>
                    <td align="left" width="30%">
                        Reportable Service:                                            
                    </td>
                    <td>
                        <asp:CheckBox ID="chkReportable" name="chkReportable" runat="server" value="1" Checked="True"></asp:CheckBox>
                    </td>
                </tr>
                <!--
                <tr>
                    <td align="left" width="30%">
                        Expired Date:                                            
                    </td>
                    <td>
                        <asp:TextBox ID="expiredDate" name="expiredDate" runat="server"></asp:TextBox>
                        <script type="text/javascript">
                            function onlyFuturedays(date) {
                                var today = new Date();
                                today.setDate(today.getDate() - 1);
                                return [(date > today), ''];
                            }
                            
                            g(document).ready(function() {
                                g('#expiredDate').datepicker({
                                    beforeShowDay: onlyFuturedays,                                    
                                });

                              
                            });                            
                        </script>
                    </td>
                </tr>
                    -->
                <tr>
                    <td colspan="2">
                        <table id="tblServiceConfiguration">
                            <tr id="configurationTr_0">
                                <td align="center" id="baserulestd_0">
                                    <asp:DropDownList ID="BaseRules_0" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <div id="OperatorPlaceHolder_0">
                                        <select id="operator_0" name="operator_0">
                                            <option value=""></option>
                                            <option value="&gt;">&gt;</option>
                                            <option value="&gt;=">&gt;=</option>
                                            <option value="&lt;">&lt;</option>
                                            <option value="&lt;=">&lt;=</option>
                                            <option value="=">=</option>
                                        </select>
                                    </div>
                                </td>
                                <td>
                                    <div id="ValuePlaceHolder_0">
                                        <asp:TextBox ID="Valuebox_0" runat="server"></asp:TextBox>                                        
                                    </div>                                   
                                </td>
                                 <td>
                                    <div id="ValuePlaceHolder1_0">
                                        <asp:Label ID="Valuebox1_0" runat="server" onkeypress="javascript:return false;" ></asp:Label>                                        
                                    </div>                                   
                                </td>
                                 <td>
                                   <div id="ValuePlaceHolder2_0">
                                        <asp:TextBox ID="Valuebox2_0" runat="server"></asp:TextBox>                                        
                                    </div>                                 
                                </td>
                                 <td>
                                    <div id="ValuePlaceHolder3_0">
                                        <asp:Label ID="Valuebox3_0" runat="server"></asp:Label>                                        
                                    </div>                                   
                                </td>
                                <td>
                                    <a href="#" onclick="DeleteValue(0);">Delete</a>                                    
                                </td>
                            </tr>
                        </table>
                    </td>
                    </tr>
                    <tr>
                    <td colspan="2" align="center">
                        <asp:Button ID="btnAddRule" runat="server" Text="Add" OnClientClick="Javascript:return false;" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="divDescription" style="display:inline-block;min-width: 100%; background-color: #d3d3d3;">

                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="left" colspan="2">
                        <textarea id="expression" name="expression" rows="2" cols="50" readonly="readonly"></textarea>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 10px;">&nbsp;</td>
                </tr>
                <tr>
                    <td align="left" colspan="2">
                         Emails List: (Use ';' to separate multiple email address)
                    </td>
                </tr>
                <tr>                    
                    <td colspan="2" align="left">
                        <asp:TextBox ID="txtEmailsList" name="txtEmailsList" runat="server" Width="350"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="left" width="420">
                        <fieldset>
                            <legend>Email Level Options</legend>
                            Vehicle:<input type="checkbox" value="Vehicle;" id="VehicleLevelEmail" name="VehicleLevelEmail" />
                            Landmark:<input type="checkbox" value="Landmark;" id="LandmarkLevelEmail" name="LandmarkLevelEmail" />                            
                            Fleet Critical:<input type="checkbox" value="FleetCritical;" id="FleetCriticalEmail" name="FleetCriticalEmail" />
                            Fleet Warning:<input type="checkbox" value="FleetWarning;" id="FleetWarningEmail" name="FleetWarningEmail" /><br />
                            Fleet Notify:<input type="checkbox" value="FleetNotify;" id="FleetNotifyEmail" name="FleetNotifyEmail" />
                            Fleet All:<input type="checkbox" value="FleetAll;" id="FleetAllEmail" name="FleetAllEmail" />
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 10px;">&nbsp;</td>
                </tr>
                <tr>
                    <td align="left" colspan="2">
                        Subject:
                        <div align="right">Subject placeholder:
                            <select id="sltSubjectPlaceholder">
                                <option value="">Select subject placeholder</option>
                                <option value="[ServiceName]">Service name</option>
                                <option value="[LATITUDE]">Latitude</option>
                                <option value="[LONGITUDE]">Longitude</option>
                                <option value="[ADDRESS]">Address</option>
                                <option value="[StDate]">Event start time</option>
                                <option value="[EVENT_TIME]">Event time</option>
                                <option value="[LICENSE_PLATE]">License plate</option>
                                <option value="[LANDMARK_ID]">Landmark ID</option>
                                <option value="[LANDMARK_NAME]">Landmark name</option>
                                <option value="[FLEET_ID]">Fleet ID</option>
                                <option value="[FLEETS_NAME]">Fleets name</option>
                                <option value="[USER_NAME]">User name</option>
                                <option value="[BOX_ID]">Box id</option>
                                <option value="[SIGNAL_SPEED]">Speed</option>
                                <option value="[Road_SPEED]">Road speed</option>
                                <option value="[VehicleDescription]">Vehicle name</option>
                                <option value="[DRIVER_ID]">Driver ID</option>
                                <option value="[DRIVER_NAME]">Driver name</option>
                                <option value="[GOOGLE_LINK]">GoogleMap point</option>
                                <option value="[DISPUTE_LINK]">Dispute link</option>
                                <option value="[SUPERVISOR]">Driver supervisor</option>  
                                <option value="[CLT_VALUE]">CLT value</option>                                
                                <option value="[EOP_VALUE]">EOP value</option>                                
                                <option value="[EOT_VALUE]">EOT value</option>                                
                                <option value="[STA_VALUE]">STA value</option>                                
                                <option value="[FLIP_VALUE]">FLIP value</option>                                
                                <option value="[FLIS_VALUE]">FLIS value</option>                                
                                <option value="[RPM_VALUE]">RPM value</option>                                
                                <option value="[TVD_VALUE]">TVD value</option>                                
                                <option value="[DTC_VALUE]">DTC value</option>  
                                <option value="[VEHICLE_YEAR]">Vehicle year</option>  
                                <option value="[VEHICLE_MODEL]">Vehicle model</option>  
                                <option value="[VEHICLE_MAKE]">Vehicle make</option>  
                                <option value="[VEHICLE_VIN]">Vehicle VIN</option> 
                                <option value="[VEHICLE_OPERATIONAL_STATE]">Vehicle Operational State</option> 
                                <option value="[CUSTOM_FIELD1]">Custom field1</option>  
                                <option value="[CUSTOM_FIELD2]">Custom field2</option>  
                                <option value="[CUSTOM_FIELD3]">Custom field3</option>  
                                <option value="[LSD]">Speed category</option>
                                <option value="[LSD_RULESETTINGS]">LSD rule setting</option>
                                <option value="[KEYFOB_ID]">KeyFobId</option>
                                <option value="[LSD_RULESETTINGS]">LSD rule setting</option>
                                <option value="[CONTACT_NAME]">Contact Name</option>
                                <option value="[CONTACT_PHONE]">Contact Phone</option>
                                <option value="[PROBLEM_DESCRIPTION]">Problem Description</option>
                                <option value="[S4]">Max speed</option> 
				                <option value="[MAIN_BATTERY]">Main Battery</option>
                                <option value="[ASSIGNED_FLEET]">Fleet Node Code</option>
				                <option value="[FORMATTED_DATETIME]">Formatted Datetime</option>
						<option value="[SERVICE_DURATION]">Event Duration</option>
                            </select>
                        </div>
                    </td>
                </tr>                
                <tr>                    
                    <td colspan="2" align="left">
                        <asp:TextBox ID="txtSubject" name="txtSubject" runat="server" Width="350"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="height: 10px;">&nbsp;</td>
                </tr>
                <tr>
                    <td align="left" colspan="2">
                         Message Body:
                        <div align="right">Message placeholder:
                            <select id="sltInsertPlaceholder">
                                <option value="">Select message placeholder</option>
                                <option value="[ServiceName]">Service name</option>
                                <option value="[LATITUDE]">Latitude</option>
                                <option value="[LONGITUDE]">Longitude</option>
                                <option value="[ADDRESS]">Address</option>
                                <option value="[StDate]">Event start time</option>
                                <option value="[EVENT_TIME]">Event time</option>
                                <option value="[LICENSE_PLATE]">License plate</option>
                                <option value="[LANDMARK_ID]">Landmark ID</option>
                                <option value="[LANDMARK_NAME]">Landmark name</option>
                                <option value="[FLEET_ID]">Fleet ID</option>
                                <option value="[FLEETS_NAME]">Fleets name</option>
                                <option value="[USER_NAME]">User name</option>
                                <option value="[BOX_ID]">Box id</option>
                                <option value="[SIGNAL_SPEED]">Speed</option>
                                <option value="[Road_SPEED]">Road speed</option>
                                <option value="[VehicleDescription]">Vehicle name</option>
                                <option value="[DRIVER_ID]">Driver ID</option>
                                <option value="[DRIVER_NAME]">Driver name</option>
                                <option value="[GOOGLE_LINK]">GoogleMap point</option>
                                <option value="[DISPUTE_LINK]">Dispute link</option>
                                <option value="[SUPERVISOR]">Driver supervisor</option>   
                                <option value="[CLT_VALUE]">CLT value</option>                                
                                <option value="[EOP_VALUE]">EOP value</option>                                
                                <option value="[EOT_VALUE]">EOT value</option>                                
                                <option value="[STA_VALUE]">STA value</option>                                
                                <option value="[FLIP_VALUE]">FLIP value</option>                                
                                <option value="[FLIS_VALUE]">FLIS value</option>                                
                                <option value="[RPM_VALUE]">RPM value</option>                                
                                <option value="[TVD_VALUE]">TVD value</option>  
                                <option value="[DTC_VALUE]">DTC value</option>   
                                <option value="[VEHICLE_YEAR]">Vehicle year</option>  
                                <option value="[VEHICLE_MODEL]">Vehicle model</option>  
                                <option value="[VEHICLE_MAKE]">Vehicle make</option>  
                                <option value="[VEHICLE_VIN]">Vehicle VIN</option> 
                                <option value="[VEHICLE_OPERATIONAL_STATE]">Vehicle Operational State</option> 
                                <option value="[CUSTOM_FIELD1]">Custom field1</option>  
                                <option value="[CUSTOM_FIELD2]">Custom field2</option>  
                                <option value="[CUSTOM_FIELD3]">Custom field3</option>  
                                <option value="[LSD]">Speed Category</option>   
                                <option value="[LSD_RULESETTINGS]">LSD rule setting</option>
                                <option value="[KEYFOB_ID]">KeyFobId</option>
                                <option value="[LSD_RULESETTINGS]">LSD rule setting</option>
                                <option value="[CONTACT_NAME]">Contact Name</option>
                                <option value="[CONTACT_PHONE]">Contact Phone</option>
                                <option value="[PROBLEM_DESCRIPTION]">Problem Description</option>  
                                <option value="[S4]">Max speed</option> 
				                <option value="[MAIN_BATTERY]">Main Battery</option>
                                <option value="[ASSIGNED_FLEET]">Fleet Node Code</option>
				                <option value="[FORMATTED_DATETIME]">Formatted Datetime</option>                  
						<option value="[SERVICE_DURATION]">Event Duration</option>
                            </select>
                        </div>
                    </td>
                </tr>
                <tr>                   
                    <td colspan="2" align="left">
                        <textarea id="txtMessageBody" name="txtMessageBody" rows="3" cols="55"></textarea>
                    </td>
                </tr>
            </table>
    </div>
    <input type="hidden" id="ServiceConfigId" name="ServiceConfigId" value="<%=ServiceConfigId %>" />
    <input type="hidden" id="ServiceId" name="ServiceId" value="<%=ServiceId %>" />
    <input type="hidden" id="action" name="action" value="" />
    <input type="hidden" id="ServiceCreatedDate" name="ServiceCreatedDate" value="" />
    <% if (Rules != null)
       {
           if (Rules.Count > 0)
           {
               foreach (Dictionary<string, string> rule in Rules)
               {
  
               
    %>
                    <input type="hidden" value="<%= rule["Operators"] %>|<%= rule["Results"] %>" id="hiddenRule_<%=rule["RuleName"] %>" name="hiddenRule_<%=rule["RuleName"] %>" />
    <%
               }
           }
       } %>
       <input type="hidden" id="deleteService" name="deleteService" value="" />         
        
        
        <div id="dialog-assignments" title="Assignments List" style="display: none;">
        <div id="dynamic-assignment">
            Select Category:
            <select id="criteria" name="criteria">
                <option id="Fleet">Fleet</option>
                <option id="Vehicle">Vehicle</option>
                <option id="Landmark">Landmark</option>
            </select>
            <table cellpadding="0" cellspacing="0" border="0" class="display" id="AssignmentsList">
      <thead>
		<tr>
			<th width="20%">Name</th>	
            <th width="10%">Service Name</th>	
            <th width="5%">Assigned From</th>	
            <th width="30%">Expression</th>	
			<th width="10%">Created At</th>
            <th width="10%">Expired At</th>
            <th width="10%">Created By</th>  
            <th width="5%">Report</th>            
		</tr>
	</thead>
	<tbody>
		<tr>
			<td colspan="6" class="dataTables_empty">Loading data from server</td>
		</tr>
	</tbody>
	<tfoot>
		<tr>
			<th width="20%">Name</th>	
            <th width="10%">Service Name</th>
            <th width="5%">Assigned From</th>		
            <th width="30%">Expression</th>	
			<th width="10%">Created At</th>
            <th width="10%">Expired At</th>
            <th width="10%">Created By</th>
            <th width="5%">Report</th>               
		</tr>
	</tfoot>

  </table>
            </div>
            </div>  
        
        
        <div id="divLandmarks" title="Landmark helper" style="display: none;">
    <table cellpadding="0" cellspacing="0" border="0" class="display" id="tblLandmarks">
      <thead>
		<tr>
			<th width="10%">Id</th>	
            <th width="80%">Landmark Name</th>	                    
            <th width="10%"></th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td colspan="6" class="dataTables_empty">Loading data from server</td>
		</tr>
	</tbody>
	<tfoot>
		<tr>
			<th width="10%">Id</th>	
            <th width="80%">Landmark Name</th>	                    
            <th width="10%"></th>                   
		</tr>
	</tfoot>

  </table>
        </div>   
        
        <div id="divDTCHelper" title="DTC code helper" style="display: none">
            <table cellpadding="0" cellspacing="0" border="0" class="display" id="tblDtcTable">
                <thead>
		            <tr>
			            <th width="10%">DTC Code</th>	
                        <th width="80%">DTC Text</th>	                    
                        <th width="10%"></th>
		            </tr>
	            </thead>
	            <tbody>
		            <tr>
			            <td colspan="6" class="dataTables_empty">Loading data from server</td>
		            </tr>
	            </tbody>
            </table>
        </div>
        
      
        <div id="divFC" title="Speed Category helper" style="display: none;">
    <table cellpadding="0" cellspacing="0" border="1" class="display" id="tblFchelper">
      <thead>
		<tr>
			<th width="30%"></th>	
            <th width="14%">FC1</th>	                    
            <th width="14%">FC2</th>
            <th width="14%">FC3</th>
            <th width="14%">FC4</th>
            <th width="14%">FC5</th>
		</tr>
	</thead>
	<tbody>
		
	</tbody>
	

  </table>
        </div> 
        
        
        <div id="dialog-report" title="Exceptions list" style="display: none;">        
           <div id="configNameholder"></div>
            <table cellpadding="0" cellspacing="0" border="0" class="display" id="tblReport">
      <thead>
		<tr>
			<th width="10%">Name</th>	
            <th width="10%">Start date</th>	
            <th width="10%">End date</th>	
            <th width="10%">Duration</th>	
			<th width="10%">Fuel</th>
            <th width="10%">Distance</th>
            <th width="10%">Speed</th>
            <th width="5%">LandmarkId</th> 
            <th width="15%">Notes</th>           
		</tr>
	</thead>
	<tbody>
		<tr>
			<td colspan="6" class="dataTables_empty">Loading data from server</td>
		</tr>
	</tbody>
	<tfoot>
		<tr>
			<th width="15%">Name</th>	
            <th width="10%">Start date</th>	
            <th width="10%">End date</th>	
            <th width="10%">Duration</th>	
			<th width="10%">Fuel</th>
            <th width="10%">Distance</th>
            <th width="5%">Speed</th>
            <th width="5%">LandmarkId</th> 
            <th width="15%">Notes</th>            
		</tr>
	</tfoot>

  </table>
            </div>  
                       
<asp:Button ID="btnDisableEnter" runat="server" Text="" OnClientClick="return false;" style="display:none;"/>
</form>
    

          

</body>
</html>
