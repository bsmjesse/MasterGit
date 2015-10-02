<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BulkVehicleStateUpdate.aspx.cs" Inherits="SentinelFM.Widgets_BulkVehicleStateUpdate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css" />
    <link rel="stylesheet" href="//cdn.datatables.net/1.10.7/css/jquery.dataTables.css" />

    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="//maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="//cdn.datatables.net/1.10.7/js/jquery.dataTables.min.js"></script>

    <title>Bulk Vehilce State Update</title>

    <style type="text/css">
        #content {
            position: fixed;
            top: 0;
            bottom: 70px;
            left: 0;
            right: 0;
            overflow:auto;
        }

        table.dataTable tbody th, table.dataTable tbody td, table.dataTable thead th, .dataTables_wrapper {
            font-family: verdana;
            font-size: 11px;
        }

        table.dataTable tbody th, table.dataTable tbody td {
            padding: 4px 5px;
        }

        table.dataTable thead th, table.dataTable thead td {
            padding: 5px 9px;
        }
        
        table.dataTable tbody td.details {
            padding-left:20px;
        }
    </style>
</head>
<body>
    <script type="text/javascript">
        var rootpath = top.location.href.match(/^(http.+\/)[^\/]+$/)[1];
        var savingVehicleOperationalState = false;
        var odatatable;
        var SelectedLanguage = '<%=((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en")%>';
        //var VEHICLE_STATE_DATA = <%=VEHICLE_STATE_DATA%>;

        $(document).ready(function () {

            var columnSort = new Array;
            $('#tblVehicleStates').find('thead tr th').each(function () {
                if ($(this).attr('data-bsortable') == 'true') {
                    columnSort.push({ "bsortable": true });
                } else {
                    columnSort.push({ "bSortable": false });
                }
            });

            odatatable = $('#tblVehicleStates').DataTable({
                "bPaginate": false,
                "bFilter": false,
                "bInfo": false,
                "aoColumns": columnSort,
                //"data": VEHICLE_STATE_DATA.data,
                //columns: [
                //    { data: "VehiceDescription" },
                //    { data: "OperationStateName" },
                //    { data: "OperationalStateNotes" },
                //    { data: "LandmarkDuration" },
                //    { data: null, defaultContent: '', orderable: false }
                //],
                //"order": [[0, "asc"]],
                "language": {
                    "url": SelectedLanguage == 'fr' ? "//cdn.datatables.net/plug-ins/1.10.7/i18n/French.json" : ''
                }
            });
            
        });

        function UpdateVehicleStates() {
            $('#btnUpdateVehicleStates').addClass('disabled');
            
            if (savingVehicleOperationalState)
                return '';

            savingVehicleOperationalState = true;
            var updateHasError = false;
            $('.imgVehicleUpdateStatus').hide();


            var selectedvehicles = $('#SelectedVehicles').val().split(",");
            for (var i = 0; i < selectedvehicles.length; i++) {

                //var f = $("#form" + selectedvehicles[i]).serialize();
                var sdata = "landmarkId=" + $('#LandmarkId_' + selectedvehicles[i]).val();
                sdata += "&OperationalStateServiceConfigId=" + $('#ServiceConfigId_' + selectedvehicles[i]).val();
                sdata += "&OperationalState=" + $('#OperationalState_' + selectedvehicles[i]).val();
                sdata += "&OperationalStateNotes=" + $('#OperationalStateNotes_' + selectedvehicles[i]).val();
                sdata += "&LandmarkDuration=" + $('#LandmarkDuration_' + selectedvehicles[i]).val();
                sdata += "&vehicleId=" + $('#vehicleId_' + selectedvehicles[i]).val();
                sdata += "&boxId=" + $('#boxId_' + selectedvehicles[i]).val();
                sdata += "&chkSendEmailImmediately=" + $('#chkSendEmailImmediately_' + selectedvehicles[i]).val();
                sdata += "&landmarkEventId=" + $('#landmarkEventId_' + selectedvehicles[i]).val();
                sdata += "&landmarkInDatetime=" + $('#landmarkInDatetime_' + selectedvehicles[i]).val();
                sdata += "&originOperationState=" + $('#originOperationState_' + selectedvehicles[i]).val();
                //alert(sdata);

                var VehicleDescription = $('#VehicleDescription_' + selectedvehicles[i]).html();
                $('#msgbar').css('color', 'black').html('Updating ' + VehicleDescription + '...').show();

                $.ajax({
                    type: 'GET',
                    url: rootpath + 'MapNew/NewMapGeozoneLandmark.asmx/SaveVehicleOperationalState',
                    data: sdata,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (msg) {
                        var r = eval('(' + msg.d + ')');
                        if (r.status == 200) {
                            $('#msgbar').css('color', 'green').html('Updating ' + VehicleDescription + '... Succeed').show();
                            $('#imgStatus_' + selectedvehicles[i]).attr("src", rootpath + "images/ok.gif").show();
                            if (parent.currentView == 'dashboard') {
                                parent.updateOperationalState($('#boxId_' + selectedvehicles[i]).val(), $('#OperationalState_' + selectedvehicles[i]).val(), $('#OperationalState_' + selectedvehicles[i] + ' option:selected').text(), $('#OperationalStateNotes_' + selectedvehicles[i]).val());
                            }
                        }
                        else
                        {
                            $('#msgbar').css('color', 'red').html('Updating ' + VehicleDescription + '... Failed').show();
                            $('#imgStatus_' + selectedvehicles[i]).attr("src", rootpath + "images/No.gif").show();
                            updateHasError = true;
                        }
                    },
                    error: function (msg) {                        
                        $('#msgbar').css('color', 'red').html('Updating ' + VehicleDescription + '... Failed').show();
                        $('#imgStatus_' + selectedvehicles[i]).attr("src", rootpath + "images/No.gif").show();
                        updateHasError = true;
                    }
                });
                
            }

            $('#btnUpdateVehicleStates').removeClass('disabled');
            savingVehicleOperationalState = false;
            if (updateHasError) {
                $('#msgbar').css('color', 'red').html('Completed the updating with error.').show();
            }
            else {
                $('#msgbar').css('color', 'green').html('Succefully updated the vehilces.').show();
            }
        }

        function CloseWindow() {
            var win = window.parent.Ext.getCmp('winBulkVehicleStateUpdate');
            if (win) win[win.closeAction]();
        }

        function OnOperationalStateChange(o) {
            var a = $(o).attr('id').split("_");
            if(a.length < 2)
                return;

            var vid = a[1];
            if ($(o).val() == 200) {
                $('#selLandmarkDuration_' + vid).hide();
            }
            else if ($(o).val() == 100 && $('#LandmarkId_' + vid).val() != '0' && $('#ServiceConfigId_' + vid).val() != '0') {
                $('#selLandmarkDuration_' + vid).show();
            }
        }

        function getLandmarkDuration(vehicleId, landmarkId, boxId) {
            var serviceConfigId = $('#ServiceConfigId_' + vehicleId).val();
            $.ajax({
                type: 'GET',
                url: rootpath + 'Vehicles.aspx?QueryType=GetLandmarkDuration',
                data: { "serviceConfigId": serviceConfigId, "vehicleId": vehicleId, "landmarkId": landmarkId, "boxId": boxId },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    if (msg.status == 200) {
                        if (msg.landmarkDuration == -1) {
                            $('#LandmarkDuration_' + vehicleId).val(0);
                            //$('#trOperationalStateMessage').show();
                        }
                        else {
                            $('#LandmarkDuration_' + vehicleId).val(msg.landmarkDuration);
                            //$('#trOperationalStateMessage').hide();
                        }

                        if (msg.ShouldSendEmailImmediately != undefined && msg.ShouldSendEmailImmediately.toLowerCase() == "false") {
                            $('#chkSendEmailImmediately_' + vehicleId).prop('checked', false);
                        }
                        else {
                            $('#chkSendEmailImmediately_' + vehicleId).prop('checked', true);
                        }
                    }

                },
                error: function (msg) {
                    alert('failure');
                }
            });
        }
    </script>
    
    <div>
        <div id="content">
            
                <input type="hidden" id="SelectedVehicles" name="SelectedVehicles" value="<%=SelectedVehicles.Replace(",,",",").Trim(',') %>" />
                <div style="margin:25px;" id="main">
                    <table id="tblVehicleStates" style="width:100%;">
                        <thead> 
                            <tr>
                                <th data-bsortable="true" style="width:80px;">Vehilce</th>
                                <th>Service Name</th>
                                <th>State</th>
                                <th>Notes</th>
                                <th style="width:360px;">Email Reminder Period</th>
                                <th></th>                    
                            </tr>
                        </thead>
                        <%=VEHICLE_STATE_DATA %>
                    </table>        
                </div>
            
        </div>
    
        <div class="navbar navbar-blend navbar-fixed-bottom" style="margin-left:25px;">            
            <button id="btnUpdateVehicleStates" type="button" class="btn btn-primary" onclick="UpdateVehicleStates();">Update</button>
            <button type="button" class="btn btn-default" onclick="CloseWindow();">Close</button>
            <span id="msgbar" style="width:100%; height:20px;margin-bottom:10px;margin-left:10px;display:none"></span>
        </div>
    </div>
</body>
</html>
