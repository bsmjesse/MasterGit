<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AssignmentForm.aspx.cs" Inherits="ServiceAssignment_AssignmentForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Assignment</title>
    <link rel="stylesheet" href="Scripts/media/smoothness/jquery-ui.css" />
    <link rel="stylesheet" href="Scripts/media/css/demo_table.css" />  
    <link rel="stylesheet" href="Scripts/media/css/TableTools.css" />   
    <link rel="stylesheet" href="Scripts/media/gh/gh-buttons.css" /> 
    <%--<link rel="stylesheet" href="Scripts/bootstrap/css/bootstrap.min.css" />--%>
  <script type="text/javascript" src="Scripts/ui/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Scripts/ui/jquery-migrate-1.1.0.js"></script>
  <script type="text/javascript" src="Scripts/ui/jquery-ui.js"></script>
    <script type="text/javascript" src="Scripts/ckeditor/ckeditor.js"></script>
    <script src="Scripts/media/js/jquery.dataTables.js" type="text/javascript"></script>
    <script type="text/javascript" charset="utf-8" src="Scripts/media/js/ZeroClipboard.js"></script>
	<script type="text/javascript" charset="utf-8" src="Scripts/media/js/TableTools.js"></script>   
    <script src="Scripts/AssignToVehicle.js?v=20150619" type="text/javascript"></script>
    <%--<script src="Scripts/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>--%>
    <script type="text/javascript">
        var vehicleId = "<%=VehicleId%>";
        var serviceId = "<%=ServiceId%>";
        var objectName = "<%=ObjectName%>";
        var serviceName = "<%=ServiceName%>";
        var lookfor = "<%=Service%>";
        var serviceId = "<%=ServiceId%>";
        var hgiUser = "<%=HgiUser%>";
    </script> 
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
        #serviceWrapper {
            width: 800px;
        }
  </style>   
</head>
<body>
    <div class="actions button-container">
    <a href="#" id="btnUnassigned" class="button primary icon add">To Be assigned</a>
    <a href="#" id="btnAssignment" class="button primary icon settings">Create Assignment</a>
    <div class="button-group">
    <a href="#" id="btnFleet" class="button primary icon search">Fleet</a>
    <a href="#" id="btnVehicle" class="button primary icon search">Vehicle</a>
    <a href="#" id="btnLandmark" class="button primary icon search">Landmark</a>
    </div>
        
        <%--<div class="btn-group">
                <a href="#" class="button primary icon arrowdown" data-toggle="dropdown">Export</a>                
                <ul class="dropdown-menu">
                    <li><a href="#" id="exportCSV" onclick="exportData('csv');">CSV</a></li>
                    <li><a href="#" id="exportExcel2003" onclick="exportData('excel2003');">Excel 2003</a></li>
                    <li><a href="#" id="exportExcel2007" onclick="exportData('excel2007');">Excel 2007</a></li>
                    <li><a href="#" id="exportPDF" onclick="exportData('pdf');">PDF</a></li>                 
                </ul>
              </div>--%>
                        
        </div>
    <form id="AssignmentForm" runat="server">
        <table id="serviceWrapper">
            <tr>
                <td align="center">
                    <div align="center" style="font-size:1.87em; font-family: Arial;display: inline;"><%=char.ToUpper(Service[0]) + Service.Substring(1)%> service assignment for <div class="objectholder" style="display: inline;">vehicle</div></div>
                </td>
            </tr>
            <tr>
                <td>
                    <table cellpadding="0" cellspacing="0" border="0" class="display" id="allRoutes">
      <thead>
		<tr>
		    <th width="20%">Service Name</th>	
            <th width="10%" id="objectName">Vehicle Name</th> 
            <th width="10%">Assigned From</th>
            <th width="35%">Expression</th>                        
			<th width="10%">Assignment Date</th>            
            <th width="10%">Assigned By</th>
            <th width="5%"></th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td colspan="4" class="dataTables_empty">Loading data from server</td>
		</tr>
	</tbody>
    <tfoot>
        <th></th>
        <th></th>
        <th></th>        
        <th></th>        
        <th></th>
        <th></th>
    </tfoot>    
  </table>
                </td>
            </tr>
            <tr>
                <td align="center"></td>
            </tr>
        </table>
    
    <div>
               
        
    </div>
        <input type="hidden" name="sId" id="sId" value=""/>
        <input type="hidden" name="delete" id="delete" value=""/>
        
        
        <div id="dialog-assignment" title="Assignment" style="display: none;">
        
        <table>
            <tr>
                <td colspan="3" align="center"><div align="center" style="font-size:1.5em; font-family: Arial; font-weight:700; display: inline;">Create <%=Service%> assignment for <div class="objectholder" style="display: inline;">vehicle</div></div>        </td>
            </tr>
            <tr>
                <td>                                        
                    <input type="text" id="txtInput" placeholder="Please input object name" />
                </td>
                <td>
                    
                </td>
                <td>
                    <input type="text" id="txtRules" placeholder="Please input service name" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:ListBox ID="inputsList" Width="250" Height="300" SelectionMode="Multiple" runat="server"></asp:ListBox>
                    
                </td>
                
                <td>
                    --->
                </td>
                <td>
                    <asp:ListBox ID="servicesList" Width="200" Height="300" SelectionMode="Multiple" runat="server"></asp:ListBox>                    
                </td>
                
            </tr>
        </table>   
        </div> 
        
        
        <div id="dialog-report" title="Exceptions list" style="display: none;">        
           <div id="configNameholder"></div>
            <table cellpadding="0" cellspacing="0" border="0" class="display" id="tblReport">
      <thead>
		<tr>
			<th width="10%">Vehicle Name</th>	
            <th width="19%"><%=StartTimeColumnName %></th>	
            <th width="19%"><%=EndTimeColumnName %></th>	
            <th width="5%">Duration(<%=DurationUnit %>)</th>	
			<th width="1%">Fuel</th>
            <th width="1%">Distance</th>
            <th width="5%">Speed</th>
            <th width="15%">Landmark</th> 
            <th width="15%">StreetAddress</th>           
		</tr>
	</thead>
	<tbody>
		<tr>
			<td colspan="6" class="dataTables_empty">Loading data from server</td>
		</tr>
	</tbody>	

  </table>
            </div>                     
    </form>             

<%--    <form id="exportForm" name="exportForm" action="frmMesssagesExtendedNew.aspx" method="post" target="frmExport" style="display:none;">
        <input type="hidden" id="QueryType" name="QueryType" value="export" />
        <input type="hidden" id="exportType" name="exportType" value="message" />
        <input type="hidden" id="exportformat" name="exportformat" value="csv" />        
    </form>
     <iframe id="frmExport" name="frmExport" width="0" height="0"></iframe>--%>

</body>
</html>

