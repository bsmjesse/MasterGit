var g = jQuery.noConflict();
g(document).ready(function () {
    g('#tblAllRoutes').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "Allroutes.aspx",
        /***
         "fnRowCallback": function (nRow, aData, iDisplayIndex) {
             $.each(aData, function (i) {
                                  
           });            
    },
    ***/

        "bAutoWidth": false,
        "aoColumns": [            
            { "sType": "text" },
            { "sType": "text" },
            { "sType": "text" },
            { "sType": "text" },
            { "bSortable": false },
            { "bSortable": false }
        ],
        "sPaginationType": "full_numbers",
        "fnServerData": function (sSource, aoData, fnCallback) {
            aoData.push({ "name": "jsondata", "value": 1 });
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
            });
        }
    });
});

function ShowAssignmentHistory(serviceConfigId, serviceName) {
    g('#serviceNamePlaceholder').html(serviceName);
    g("#tabs").tabs("option", "active", 1);
    g('#tblAssignmentHistory').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "Allroutes.aspx",
        /***
         "fnRowCallback": function (nRow, aData, iDisplayIndex) {
             $.each(aData, function (i) {
                                  
           });            
    },
    ***/

        "bAutoWidth": false,
        "aoColumns": [            
            { "sType": "text" },
            { "sType": "text" },
            { "sType": "text" },
            { "sType": "text" },            
            { "bSortable": false }
        ],
        "sPaginationType": "full_numbers",
        "fnServerData": function (sSource, aoData, fnCallback) {
            aoData.push({ "name": "jsondata", "value": 1 });
            aoData.push({ "name": "searchCriteria", "value": "Vehicle" });
            aoData.push({ "name": "bInvolveDeleted", "value": 1 });
            aoData.push({ "name": "bServiceConfigId", "value": serviceConfigId });
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
            });
        }
    });
}