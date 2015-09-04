var g = jQuery.noConflict();
var pointsCount = 0;
var focusedBox = 'gui-input-source';
var suggestedPoints = {};
var trafficMode = "car";
var avoidType = "";
var ckMessageBody = "";
var alreadyKnownPointsCollection = "";
g(document).ready(function() {
 
    CKEDITOR.replace('emailmessage', {
        htmlEncodeOutput: true
    });
    ckMessageBody = CKEDITOR.instances['emailmessage'];
    OSRM.G.RouteProvider = g('#EngineProvider').val();
    //ToggleTransportMode(RouteProvider);    
    var name = g("#txtSkillName");
    var allFields = g([]).add(name);
    var tips = g(".validateTips");

    if (OSRM.G.RouteProvider == 'OSRM') {
        g('#divPreference').hide();
    }

    g('#gpx_button').on("click", function() {                      
        g("#dialog-form").dialog("open");        
        g('#dialog-form').dialog({ zIndex: 99999 });
        g("#dialog-form").dialog('option', 'title', 'Save Route');
                    
    });

    g('#EngineProvider').change(function () {
        OSRM.G.RouteProvider = g(this).val();
        ClearTransportMode();
        OSRM.GUI.init();
        if (OSRM.G.RouteProvider == 'OSRM') {
            g('#divPreference').hide();
        } else {
            g('#divPreference').show();
        }
        RenderRoute();
    });
    g("#dialog:ui-dialog").dialog("destroy");
   
    function updateTips(t) {
        tips.text(t)
            .addClass("ui-state-highlight");
        setTimeout(function() {
            tips.removeClass("ui-state-highlight", 1500);
        }, 500);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            o.addClass("ui-state-error");
            updateTips("Length of " + n + " must be between " +
                min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }
    g("#dialog-form").dialog({
        autoOpen: false,
        height: 600,
        width: 400,
        modal: true,        
        close: function() {
            allFields.val("").removeClass("ui-state-error");
        }
    });
    
    g("#preference-dialog").dialog({
        autoOpen: false,
        height: 500,
        width: 200,
        modal: true,
        buttons: {
            "Save": function () {                
               
                RenderRoute();
                g(this).dialog("close");
            },
            Cancel: function () {
                g(this).dialog("close");
            }
        },        
        close: function () {
            allFields.val("").removeClass("ui-state-error");
        }
    });

    g("#email-dialog").dialog({
        autoOpen: false,
        height: 400,
        width: 450,
        modal: true,
        buttons: {           
            "Send": function () {
                if (ValidateEmail()) {
                    var htmlBody = htmlEncode(ckMessageBody.getData());                    
                    g.ajax({
                        url: "SentinelService.aspx",
                        type: "POST",
                        dataType: "json",
                        data: "to=" + g('#emailaddress').val() + "&subject=" + g('#routeName').val() + "&name=" + g("#emailname").val() + "&body=" + encodeURIComponent(htmlBody) + "&action=email",
                        async:false,
                        success: function (result) {
                            if (result.status == "success") {
                                alert("Your email has been sent out");
                            }
                            else {
                                alert("Your email cannot be sent out");
                            }
                            
                        },
                        error: function (xhr, status, error) {
                            console.log(xhr.responseText);
                        }
                    });
                    g(this).dialog("close");
                }
                
            },
            Cancel: function () {
                g(this).dialog("close");
            }
        },
        close: function () {
            allFields.val("").removeClass("ui-state-error");
        }
    });

    g("#dialog-modal").dialog({
        autoOpen: false,
        height: 500,
        width: 800,
        modal: true,
        buttons: {
            Cancel: function () {                
                g(this).dialog("close");
            }
        },
    });


    InitialSuggestedPoints();
    InitialRoutesList();
    InitialComponents();
    g('#routeView').click(function () {
        g("#dialog-modal").dialog("open");
    });
    g('#routeSave').click(function () {
        /****
        var buttons = {
            "Save": function () {
                var bValid = ValidateForm();
                if (bValid) {
                    g('#txtWayPoints').val(CollectAddress());
                    g('#NavTeqResponse').val(OSRM.G.NavTeqResponse);
                    g('#txtPreference').val(OSRM.G.NavTEQPreferences);
                    g.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "Main.aspx/AcceptFormData",
                        data: "{'funcParam':'" + g('#routeForm').serialize() + "'}",
                        dataType: "json",
                        error: function (xhr, status, error) {
                            alert(xhr.responseText);
                            g("#dialog-form").dialog("close");
                        },
                        success: function (json) {
                            if (g("#chkSpeed").is(':checked')) {
                                if (json.d != "Failed") {
                                    var jsonData = jQuery.parseJSON(json.d);
                                    ChangeMarkers(jsonData);
                                    alert("You have sucessfully saved the speed route segment.");
                                } else {
                                    alert("You cannot save the speed segment.");
                                }
                            } else {
                                if (json.d == "Created") {
                                    InitialRoutesList();
                                    //g('#routesList').dataTable();
                                    alert("You have saved the route.");
                                    g('#cmdRouteBuilder').val("");
                                } else {
                                    alert("You cannot save the route, please contact with administrator or check if there is any assignment for the route");
                                }
                            }

                            g("#dialog-form").dialog("close");
                        }
                    });
                }

            },
            Cancel: function () {
                g(this).dialog("close");
            }
        };
        ****/
        var buttons = {
            "Save": function() {
                var bValid = ValidateForm();
                if (bValid) {
                    g('#txtWayPoints').val(CollectAddress());
                    g('#NavTeqResponse').val(OSRM.G.NavTeqResponse);
                    console.log('NavTeqResp: ' + OSRM.G.NavTeqResponse);
                    g('#txtPreference').val(OSRM.G.NavTEQPreferences);
                    var formData = g('#routeForm').serialize();
                    
                    g.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: "Main.aspx/AcceptFormData",
                        data: "{'funcParam':'" + formData + "'}",
                        dataType: "json",
                        error: function (xhr, status, error) {
                            console.log(formData);
                            alert(xhr.responseText);                            
                            g("#dialog-form").dialog("close");
                        },
                        success: function(json) {
                            if (g("#chkSpeed").is(':checked')) {

                                if (isNaN(json.d) ) {
                                    var jsonData = jQuery.parseJSON(json.d);
                                    InitialDisputePointsList();
                                    ChangeMarkers(jsonData);
                                    alert("You have sucessfully saved the speed route segment.");
                                } else {
                                    if (parseInt(json.d) > 0) {
                                        g('#correctionid').val(json.d);
                                        alert("You have sucessfully saved the speed route segment.");
                                    } else {
                                        alert("You cannot save the speed segment.");
                                    }
                                    
                                }
                            } else {
                                if (parseInt(json.d) > 0) {
                                    InitialRoutesList();
                                    //g('#routesList').dataTable();
                                    g('#LandmarkId').val(json.d);
                                    alert("You have saved the route.");
                                    g('#cmdRouteBuilder').val("");
                                } else {
                                    alert("You cannot save the route, please contact with administrator or check if there is any assignment for the route");
                                }
                            }

                            g("#dialog-form").dialog("close");
                        }
                    });
                }

            }
        };
        if (g('#correctionid').val() != "" && g('#correctionid').val() != null) {
            g("#dialog-form").dialog('option', 'title', 'Save Speed Segment');
            buttons = g.extend(buttons, {
                Delete: function () {
                    var yes = confirm("Do you want to delete the speed segment?");
                    if (yes) {                        
                        g.ajax({
                            url: "Main.aspx/DeleteCorrectRoute",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            type: "POST",
                            data: "{'routeId':'" + g('#correctionid').val() + "'}",
                            success: function (json) {
                                if (json.d == "Failed") {
                                    alert("You cannot delete the segment, please contact with administrator.");
                                } else {
                                    var jsonData = jQuery.parseJSON(json.d);
                                    CleanUpInfo();
                                    ChangeMarkers(jsonData);
                                    g("#dialog-form").dialog("close");
                                    alert("You have successfully deleted the segment");
                                }
                            },
                            error: function (error) {
                                console.log(error);
                            }
                        });
                    }
                }
            });
        } else {
            g("#dialog-form").dialog('option', 'title', 'Save Route');
        }

        buttons = g.extend(buttons, {
            Cancel: function () {
                g(this).dialog("close");
            }
        });
        g("#dialog-form").dialog('option', 'buttons', buttons).dialog("open");
        g('#dialog-form').dialog({ zIndex: 99999 });
        
    });
    g('#routeExport').click(function () {
        if (OSRM.G.RouteProvider == "OSRM") {
            var query_string = '?hl=' + OSRM.Localization.current_language;
            for (var i = 0; i < OSRM.G.markers.route.length; i++)
                query_string += '&loc=' + OSRM.G.markers.route[i].getLat().toFixed(6) + ',' + OSRM.G.markers.route[i].getLng().toFixed(6);
            document.location.href = OSRM.G.active_routing_server_url + query_string + '&output=gpx';
        } else {
            if (OSRM.G.NavTeqResponse != "") {
                //document.location.href = "GenerateGpx.aspx?gpxonly=1&NavTeqResponse=" + encodeURIComponent(OSRM.G.NavTeqResponse);
                g('#NavTeqResponseData').val(OSRM.G.NavTeqResponse);
                g('#gpxform').submit();
            }
        }
    });
    g('#routeEmail').click(function () {
        SendEmail();
    });
    GiveSuggestionAddress('gui-input-source');
    g(document).on('focus', 'input[id^=gui-input]', function (event) {
        /***/
        var currentId = g(this).attr('id');
        if (focusedBox != currentId) {
            focusedBox = currentId;                        
            GiveSuggestionAddress(currentId);           
        }
               
    });

    g('#gui-language-toggle').change(function() {
        OSRM.G.UserLanguage = g('#gui-language-toggle').val();
        g('#styled-select-gui-language-toggle').html(g('#gui-language-toggle option:selected').text());
    });
    RenderWayPoints(addressesCollection);

    g('#chkSpeed').click(function() {
        if (g("#chkSpeed").is(':checked')) {
            g('#correctSpeed').show();
            g("label[for='correctSpeed']").show();
            g('#gblSegment').show();
            g("label[for='gblSegment']").show();
            g('#chkEnableService').hide();
            g("label[for='chkEnableService']").hide();            
        } else {
            g('#correctSpeed').val('');
            g('#correctSpeed').hide();
            g("label[for='correctSpeed']").hide();
            g('#gblSegment').hide();
            g("label[for='gblSegment']").hide();
            g('#chkEnableService').show();
            g("label[for='chkEnableService']").show();
        }
    });
});

function InitialComponents() {
    g('#styled-select-gui-language-toggle').html(OSRM.G.UserLanguage);
    g('#gui-language-toggle').val(OSRM.G.UserLanguage);
}

function InitialSuggestedPoints()
{
    suggestedPoints["gui-input-source"] = "";
    suggestedPoints["gui-input-target"] = "";
}

function InitialRoutesList() {
    g('#routesList').dataTable({
             "bProcessing": true,
			 "bServerSide": true,
             "bDestroy": true,
			 "sAjaxSource": "GetRoutes.aspx",
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
            { "bSortable": false }

        ],
        "sPaginationType": "full_numbers"
    });       
}


function addRoutes() {
    customizedUrl = '?hl=en&loc=43.712740,-79.502210&loc=43.711409,-79.465442';
    OSRM.parseParameters();
}

function AddMorePoints(streetAddress) {
    var newAddress = '<div id="input-target_' + pointsCount + '" class="input-marker">' +
	'<div class="left"><span id="gui-search-target-label_' + pointsCount + '" class="input-label">To:</span></div>' +
	'<div class="center input-box"><input id="gui-input-target_' + pointsCount + '" class="input-box" type="text" maxlength="200" value="' + streetAddress + '" title="Enter destination" size="38" /></div>' +
	'<div class="left"><div id="gui-delete-target_' + pointsCount + '" class="iconic-button cancel-marker input-delete" onClick="cleanAddress(\'gui-input-target_' + pointsCount + '\')"></div></div>' +
	'<div class="right"><a class="button" id="gui-search-target">Show</a></div>' +
	'</div> ';
    g('#input-source-group').append(newAddress);
    if (streetAddress != '' && streetAddress != null) {
        g('#gui-delete-target_' + pointsCount).css("visibility", "visible");
    }
    suggestedPoints["gui-input-target_"] = "";
    pointsCount++;
    AdjustPosition();
}

function DeleteRouter(landmarkId)
{
    if (!window.confirm("Click OK to delete this route."))
        return;
    g.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "Main.aspx/DeleteRoute",
        data: "{'landmarkId':'" + landmarkId + "'}",
        dataType: "json",
        success: function(json) {
            if (json.d == "Created") {
                InitialRoutesList();
                //g('#routesList').dataTable();
                //alert("You have deleted route.");
            }
        }
    });
}

function AdjustPosition() {
    //var height = g("#input-source-group").height() + 78;
    //g("#main-output").css('top', height);
    var height = g("#main-operation").height() + 78;
    g("#main-operation").css('height', height);
}
function cleanAddress(cleanId) {
    var oldValue = "";    
    if (cleanId == 'source') {
        oldValue = suggestedPoints["gui-input-source"];
        suggestedPoints["gui-input-source"] = "";
        delete suggestedPoints["gui-input-source"];
        g('#gui-input-source').val(' ');
    }
    else if (cleanId == 'target') {
        oldValue = suggestedPoints["gui-input-target"];
        suggestedPoints["gui-input-target"] = "";
        delete suggestedPoints["gui-input-target"];
        g('#gui-input-target').val(' ');
    }        
    else{
        var deleteAddress = g('#' + cleanId).val();
        g('#' + cleanId).val('');
        var getNum = cleanId.split('_');
        if (!isNaN(getNum[1])) {
            oldValue = suggestedPoints[cleanId];
            g('#input-target_' + getNum[1]).remove();                        
            delete suggestedPoints[cleanId];
            //AdjustPosition();
        }
    }
    addressesCollection = "";
    CustomizeRoute('', oldValue);
    if (OSRM.G.SpeedDispute != undefined) {
        OSRM.G.SpeedDispute.CleanDisputeByPoint(oldValue);
    }    
}

function GetLocQueryString()
{
    var query_string = '?hl=' + OSRM.Localization.current_language;
    for (var i = 0; i < OSRM.G.markers.route.length; i++)
        query_string += '&loc=' + OSRM.G.markers.route[i].getLat() + ',' + OSRM.G.markers.route[i].getLng();
    return query_string;
}
function BuildCustomizedUrl()
{
    var url = '?hl=en';
    for(key in suggestedPoints)
    {
        if (suggestedPoints[key] == '') continue;
        url += '&loc=' + suggestedPoints[key];        
    }
    return url;
}

function CustomizeRoute(newCoord, deleteCoord) {
    var oLink = GetLocQueryString();    
    if (newCoord != '' && (deleteCoord == '' || deleteCoord == undefined))
        customizedUrl = BuildCustomizedUrl();
    else if (newCoord != '' && deleteCoord != '') {
        if (oLink.indexOf(deleteCoord) > -1) {
            customizedUrl = oLink.replace(deleteCoord, newCoord);
        } else {
            customizedUrl = oLink + '&loc=' + newCoord;
        }
    }        
    else if (deleteCoord != null && deleteCoord != '') {
        customizedUrl = oLink.replace("&loc=" + deleteCoord, "");
    }    
    RenderRoute();
}

function RenderRoute() {    
    var routingMode = g("input:radio[name=routeOption]").val();
    var selectedValue = routingMode + ";" + trafficMode;
    var mySelectedValue = "";
    g("input[name=RouteFeatureType]:checked").each(function () {
        if (mySelectedValue == "") {
            mySelectedValue += g(this).val() + ":-3";
        } else mySelectedValue += "," + g(this).val() + ":-3";
    });
    selectedValue += (mySelectedValue != null && mySelectedValue != "" ? ";" + mySelectedValue : "");
    //selectedValue += "&MetricSystem=" + metricSystem;
    OSRM.G.NavTEQPreferences = selectedValue;
    OSRM.G.route.reset();
    OSRM.G.markers.reset();
    OSRM.parseParameters();
}

function CollectAddress() {    
    var a1 = g('#gui-input-source').val() + '@' + suggestedPoints['gui-input-source'];
    var a2 = g('#gui-input-target').val()+ '@' + suggestedPoints['gui-input-target'];
    var addressSt = a1 + '|' + a2;
    for (var i = 0; i < pointsCount; i++) {
        if (g('#gui-input-target_' + i).length > 0) {
            addressSt += '|' + g('#gui-input-target_' + i).val()+ '@' + suggestedPoints['gui-input-target_' + i];
        }
    }
    return addressSt;
}

function _GetAddressByWayPoint(point)
{
    var str = point.split('@');
    if (str.length > 0)
        return str[0];
    else
        "";
}

function _GetPointByWayPoint(point)
{
    var str = point.split('@');
    if (str.length > 1)
        return str[1];
    else
        "";
}
function RenderWayPoints(wayPointsCollection) {
    if (wayPointsCollection != '' && wayPointsCollection != null) {
        var collection = wayPointsCollection.split('|');
        for (var i = 0; i < collection.length; i++) {
            if (i == 0) {                
                if (collection[i] != "") {
                    g('#gui-input-source').val(_GetAddressByWayPoint(collection[i]));
                    suggestedPoints["gui-input-source"] = _GetPointByWayPoint(collection[i]);                    
                }
                
            }
            else if (i == 1) {
                if (collection[i] != "") {
                    g('#gui-input-target').val(_GetAddressByWayPoint(collection[i]));
                    suggestedPoints["gui-input-target"] = _GetPointByWayPoint(collection[i]);                    
                }                
            } else {
                AddMorePoints(_GetAddressByWayPoint(collection[i]));
                suggestedPoints["gui-input-target_" + (pointsCount - 1)] = _GetPointByWayPoint(collection[i]);                
            }            
        }
    }
}

function ReRenderWayPoints(newCoordsCollection, appendCoord) {
    if (appendCoord == undefined || appendCoord == null) {
        appendCoord = false;
    }
    var addressesCollections = addressesCollection.split('|');
    var alreadyKnownCollections = [];
    for (var i = 0; i < addressesCollections.length; i++){
        var coordInfo = addressesCollections[i].split('@');        
        alreadyKnownCollections[coordInfo[1]] = coordInfo[0];        
    }
    var newCoordsCollections = newCoordsCollection.split(';');    
    var newProcessedAddressCollections = "";
    for (var a = 0; a < newCoordsCollections.length; a++) {
        var address = null;        
        if (alreadyKnownCollections[newCoordsCollections[a]] == null || alreadyKnownCollections[newCoordsCollections[a]] == undefined || alreadyKnownCollections[newCoordsCollections[a]] == "") {
            //console.log('getting new address....' + newCoordsCollections[a]);
            if (newCoordsCollections[a] == "" || newCoordsCollections[a] == null) {
                continue;
            }
            g.ajax({
                url: "SentinelService.aspx?latlng=" + newCoordsCollections[a] + "&language=en&action=geocode",
                dataType: "json",
                type: "GET",
                async: false,
                success: function (data) {
                    address = data.results[0].formatted_address.replace("'", " ");
                    if (appendCoord) {
                        address += "[" + newCoordsCollections[a] + "]";
                    } 
                },
                error: function (error) {                    
                    console.log(error + '@' + newCoordsCollections[a]);
                }
            });
        }
        else {
          //  console.log('not need to get new address' + newCoordsCollections[a]);
            address = alreadyKnownCollections[newCoordsCollections[a]];
        }
        if (newProcessedAddressCollections == "") {
            newProcessedAddressCollections = address + '@' + newCoordsCollections[a];
        }
        else {
            newProcessedAddressCollections += '|' + address + '@' + newCoordsCollections[a];
        }        
    }
    CleanUpAllBoxes();
    addressesCollection = newProcessedAddressCollections;
    //console.log(newProcessedAddressCollections);
    RenderWayPoints(newProcessedAddressCollections);
    customizedUrl = BuildCustomizedUrl();
}

function CleanUpAllBoxes() {
    g('#gui-input-source').val("");
    g('#gui-input-target').val("");
    g("input[id^=gui-input-target_]").each(function (key, val) {
        g('#input-target_' + key).remove();
    });
    pointsCount = 0;
}
    function SetPreference() {
        g("#preference-dialog").dialog("open");
    }

    function ClearTransportMode() {
        g('#styled-select-gui-engine-toggle').remove();
        g('#gui-engine-toggle')
            .find('option')
            .remove();
    }

    g(document).on('change', '#gui-engine-toggle', function () {
        var modeValue = g('#gui-engine-toggle').val();
        switch (modeValue) {
            case "0":
                trafficMode = "car";
                break;
            case "1":
                trafficMode = "truck";
                break;
            case "2":
                trafficMode = "pedestrian";
                break;
        }
        g('#styled-select-gui-engine-toggle').html(g('#gui-engine-toggle option:selected').text());
        RenderRoute();
        g('#gui-engine-toggle').val(modeValue);
    });

    g(document).on('change', '#gui-units-toggle', function () {
        OSRM.G.active_routing_metric = parseInt(g('#gui-units-toggle').val());
        g('#styled-select-gui-units-toggle').html(g('#gui-units-toggle option:selected').text());
        RenderRoute();
    });

    function GiveSuggestionAddress(id) {
        OSRM.G.IsDblClicked = false;
        g('#' + id).autocomplete({
            source: function (request, response) {            
                g.ajax({
                    url: "SentinelService.aspx?input=" + request.term + "&language=en&action=autocomplete",
                    dataType: "json",
                    type: "GET",
                    success: function (data) {                    
                        /****/
                        response(g.map(data.predictions, function (item) {
                        
                            return {
                                label: item.description,//item.name + (item.adminName1 ? ", " + item.adminName1 : "") + ", " + item.countryName,
                                value: item.value,
                                reference: item.reference
                            };
                        }));
                        /***/
                    },
                    error: function (error) {
                        console.log("error");
                        console.log(error);
                    }
                });
            },
            focus: function (event, ui) {                
                g(this).val(ui.item.value);
            },
            minLength: 2,
            select: function (event, ui) {                
                if (ui.item.reference.indexOf(',') > -1) {
                    AutocompleteRender(id, ui.item.reference);
                    UpdateWayPointsCollection(ui.item.value, ui.item.reference);
                    g('.input-delete').css("visibility", "visible");
                }
                else {
                    g.ajax({
                        url: "SentinelService.aspx?reference=" + ui.item.reference + "&language=" + OSRM.G.UserLanguage + "&action=getCoord",
                        dataType: "json",
                        type: "GET",
                        success: function (data) {
                            /***/
                            var newCoord = data.result.geometry.location.lat + "," + data.result.geometry.location.lng;
                            AutocompleteRender(id, newCoord);
                            UpdateWayPointsCollection(ui.item.value, newCoord);
                            g('.input-delete').css("visibility", "visible");
                            if (alreadyKnownPointsCollection == "") {
                                alreadyKnownPointsCollection = newCoord;
                            } else {
                                alreadyKnownPointsCollection += ";" + newCoord;
                            }   
                            /***/
                            //RenderRouteByPoints(data.result.geometry.location.lat, data.result.geometry.location.lng, ui.item.value);
                        },
                        error: function (error) {
                            console.log(error);
                        }
                    });
                }
                //console.log(ui.item.reference);
            },
            open: function() {
                g( this ).removeClass( "ui-corner-all" ).addClass( "ui-corner-top" );
            },
            close: function() {
                g( this ).removeClass( "ui-corner-top" ).addClass( "ui-corner-all" );
            }            
        });
        /****
        .data( "autocomplete" )
._renderMenu = function( ul, items ) {
    $.ui.autocomplete.prototype._renderMenu.apply( this, [ul, items] );
    ul.append( '<li><a href="/search/'+ this.term + '">Search: '+ this.term + '</a></li>' );	
}
        //add icon http://stackoverflow.com/questions/3774332/display-an-icon-in-jquery-ui-autocomplete-results
***/        
    }    

    function RenderRouteByPoints(lat, lng, address) {
        var newCoord = lat + "," + lng;
        AutocompleteRender(id, newCoord);
        UpdateWayPointsCollection(address, newCoord);
        g('.input-delete').css("visibility", "visible");
        if (alreadyKnownPointsCollection == "") {
            alreadyKnownPointsCollection = newCoord;
        } else {
            alreadyKnownPointsCollection += ";" + newCoord;
        }
    }
    function AutocompleteRender(focusedBox, newCoord) {
        var oldCoord = suggestedPoints[focusedBox];
        suggestedPoints[focusedBox] = newCoord;        
        CustomizeRoute(suggestedPoints[focusedBox], oldCoord);
    }
    
    function UpdateWayPointsCollection(address, coord) {
        if (addressesCollection == null || addressesCollection == "") {
            addressesCollection = address + '@' + coord;
        } else {
            if (addressesCollection.indexOf(coord) == -1) {
                addressesCollection += '|' + address + '@' + coord;
            }            
        }
        
    }
    function ValidateEmail() {
        if (g('#emailname').val().length < 1) {
            alert("Please input name");
            return false;
        }

        if (g('#emailaddress').val().length < 1) {
            alert("Please input email address");
            return false;
        }
        
        if (!IsValidateEmail(g('#emailaddress').val())) {
            alert("Email address is not in correct format");
            return false;
        }

        if (ckMessageBody.getData().length < 1) {
            alert("Email content cannot be empty");
            return false;
        }

        return true;
    }
    function SendEmail() {
        var bodyInfo = OSRM.Printing.getDescriptions();
        var myBody = '<!DOCTYPE html><html><head> <link rel="stylesheet" href="http://preprod.sentinelfm.com/routing/printing/printing.css" type="text/css"/><head><body><table class="description medium-font">' + bodyInfo.header + bodyInfo.body + '</table></body>';
        ckMessageBody.setData(myBody);
        g('#email-dialog').dialog("open");
    }    

    function IsValidateEmail(email) {
        var filter = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
        return filter.test(email);
    }

    function HideComponents() {
        if (!OSRM.G.IsHgiUser) {
            g('#gui-language-toggle').val(OSRM.G.UserLanguage);
            g('#gui-language-toggle').hide();
            g('#styled-select-gui-language-toggle').css("visibility", "hidden");
            g('#config-handle-wrapper').css("visibility", "hidden");
            g('#mapping-handle-wrapper').css("visibility", "hidden");
            g("div[aria-haspopup='true']").css("visibility", "hidden");
        }
    }

    function htmlEncode(value) {
        //create a in-memory div, set it's inner text(which jQuery automatically encodes)
        //then grab the encoded contents back out.  The div never exists on the page.
        return g('<div/>').text(value).html();
    }

    function ReversePositions() {
        var addressName = Array();       
        g("input[id^=gui-input-]").each(function (key, val) {            
            addressName.push(g(this).val());
        });
        
        var index = 1;
        g("input[id^=gui-input-]").each(function (key, val) {
            g(this).val(addressName[addressName.length - index]);
            index++;
        });
        var tmpVals = Array();       
        for (var key in suggestedPoints) {            
            tmpVals.push(suggestedPoints[key]);
        }
        var i = 1;
        var tmpArray = Array();
        
        for (var key1 in suggestedPoints) {  
            tmpArray[key1] = tmpVals[tmpVals.length - i];
            i++;
        };
        suggestedPoints = tmpArray;
    }
    
    function ValidateForm() {
        if (g('#routeName').val().length < 1) {
            alert("Please input route name");
            return false;
        }
        
        if (g('#routeBuffer').val().length < 1) {
            alert("Please input route buffer");
            return false;
        }
        
        if (g('#txtName').val().length < 1) {
            alert("Please input contact name");
            return false;
        }
        
        if (g('#txtEmail').val().length < 1) {
            alert("Please input email address");
            return false;
        }
        
        if (!IsValidateEmail(g('#txtEmail').val())) {
            alert("Please input correct email format address");
            return false;
        }

        return true;
    }
    
    function BuildAlreadyKnownCollection(wayPointsCollection) {
        if (wayPointsCollection != '' && wayPointsCollection != null) {
            var collection = wayPointsCollection.split('|');
            for (var i = 0; i < collection.length; i++) {
                if (alreadyKnownPointsCollection == null || alreadyKnownPointsCollection == "") {
                    alreadyKnownPointsCollection = _GetPointByWayPoint(collection[i]);
                } else {
                    alreadyKnownPointsCollection += ';' +  _GetPointByWayPoint(collection[i]);
                }
            }
        }
    }
    function CleanupForm() {
        g('#routeName').val('');
        g('#routeBuffer').val('');
        g('#txtName').val('');
        g('#txtEmail').val('');
        g('#txtPhone').val('');
        g('#txtDescription').val('');
        g('#txtEditLink').val('');
        g('#txtWayPoints').val('');
        g('#txtQueueString').val('');
        g('#LandmarkId').val('');
        g('#NavTeqResponse').val('');
        g('#correctionid').val('');
        g('#gpxonly').val('');
    }

  