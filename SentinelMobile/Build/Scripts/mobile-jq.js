var selectedFeature = null;
var followingFeature = null;
var historyFeature = null;
var allVehicles = [];
var vehicleFeatures = [];
var allFollows = [];
var mapcontentheight = 0;
var findingClosestVehicles = false;
var searchNumVehicles = 10;
var searchRadius = 5;
var showHistorySettingPopup = false;

var GoogleAddressService;

var searchLat = 0;
var searchLon = 0;

var vehicleInfoForEmail = '';
var boxChangeInfoForEmail = '';
var setFuelConfigurationForEmail = '';
var setSeatbeltOdometerStep = 1;

// fix height of content
function fixContentHeight() {

    /*if (navigator.userAgent.match(/Android/i)) {
        window.scrollTo(0, 1);
    }*/
    if ($.mobile.activePage.attr('id') != 'mappage')
        return;
    var footer = $("div[data-role='footer']:visible"),
        header = $("div[data-role='header']:visible"),
        //content = $("div[data-role='content']:visible:visible"),
        content = $("div[id='mapdiv']:visible:visible"),
        viewHeight = $(window).height(),
        //contentHeight = viewHeight - footer.outerHeight() - header.outerHeight();
        //contentHeight = footer.position().top - (header.position().top + header.outerHeight());
        //contentHeight = footer.position().top - 44;
        contentHeight = viewHeight - header.outerHeight();

    if (window.map && window.map instanceof OpenLayers.Map && mapcontentheight == contentHeight) {
        return;
    }
    mapcontentheight = contentHeight;

    //contentHeight = viewHeight - footer.outerHeight();

    /*if ((content.outerHeight() + header.outerHeight() + footer.outerHeight()) !== viewHeight) {
        contentHeight -= (content.outerHeight() - content.height() + 1);
        content.height(contentHeight);
    }*/
    content.height(contentHeight);

    var panelcontent = $("div[id='panelcontent']:visible:visible"),
        panelheader = $("div[id='panelheader']:visible:visible"),
        panelcontentHeight = viewHeight - panelheader.outerHeight();

    if ((panelcontent.outerHeight() + panelheader.outerHeight()) !== viewHeight) {
        panelcontentHeight -= (panelcontent.outerHeight() - panelcontent.height() + 1);
        panelcontent.height(panelcontentHeight);
    }

    if (window.map && window.map instanceof OpenLayers.Map) {
        map.updateSize();
    } else {
        // initialize map
        init(function (feature) {
            selectedFeature = feature;

            var s = '';

            s += '<b>' + feature.attributes.DisplayDateTime + '</b><br />';
            s += feature.attributes.Location + '';
            s += '<div style="margin-top:5px;">' + ResourceSpeed + ': ' + feature.attributes.Speed;
            if (feature.layer.name == ResourceVehicles && feature.attributes.Status != '') {
                s += ' (' + ResourceStatus + ': ' + feature.attributes.Status + ')';
            }
            s += "</div>";
            //if (feature.layer.name == ResourceVehicles) {
            s += ResourceDirection + ': ' + feature.attributes.MyHeading + '<br />';
            //}

            $('#popup #vehicleinfo').empty().append(s);
            $('#popupHeader h1').html(feature.attributes.Description);

            //var showConfig = false;

            if (feature.layer.name == ResourceVehicles) {
                $('#followBtn').show();
                $('#historyBtn').show();

                $('a#followBtn').attr("rel", feature.attributes.BoxId);

                if (feature.attributes.BoxId == FollowingBoxId) {
                    $('a#followBtn').find('.ui-btn-text').html(ResourceUnfollow);
                    //showConfig = true;
                }
                else {
                    $('a#followBtn').find('.ui-btn-text').html(ResourceFollow);
                }
            }
            else {
                $('#followBtn').hide();
                $('#historyBtn').hide();
            }

            $('#popup').popup();
            $('#popup').popup("open");

            /*if (showConfig)
                $('#configBtn').removeClass('ui-btn-hidden');
            else
                $('#configBtn').addClass('ui-btn-hidden');
            */
            selectControl.unselect(feature);
        });
        initLayerList();
    }

}

// one-time initialisation of button handlers 

$("#plus").live('click', function () {
    map.zoomIn();
});

$("#minus").live('click', function () {
    map.zoomOut();
});

$("#locate").live('click', function () {
    var control = map.getControlsBy("id", "locate-control")[0];
    if (control.active) {
        control.getCurrentLocation();
    } else {
        control.activate();
    }
});

//fix the content height AFTER jQuery Mobile has rendered the map page
$('#mappage').live('pageshow', function () {
    //if (navigator.userAgent.match(/Android/i)) {
    //    window.scrollTo(0, -1);
    //}
    //$('#mappage').css('padding-top', '0');
    fixContentHeight();
    $(window).bind("orientationchange resize pageshow", fixContentHeight);
    if (setBaseLayer) {
        baseLayer.map.setBaseLayer(baseLayer);
        setBaseLayer = false;
    }
});

//$(window).bind("orientationchange resize pageshow", fixContentHeight);

$('#vehicleList').live('pageshow', function () {
    $('#vehicleList').page();
    $('#mappage').page();
    $('#searchpage').page();
    $('#followpage').page();
    $('#vehicleInfoPage').page();
    $('#historypage').page();
    $('#sendEmailPage').page();

    loadVehicleByFleet(CurrentFleetId);

    $('#vehicleListSearch').bind('change', function (e) {
        // Prevent form send
        e.preventDefault();

        searchVehicleList(CurrentFleetId);
    });

    $('#vehicleList').die('pageshow', arguments.callee);
});

$('#fleetPage').live('pageshow', function () {
    if (!FleetPageIni && FleetType == 'flat')
        loadFleets(CurrentFleetId);

    $('#fleetSearch').bind('change', function (e) {
        FleetCurrentSearchString = $('#fleetSearch').val();
        FleetSearchMode = 1;
        loadFleets(CurrentFleetId, 1);

    });


    $('#fleetPage').die('pageshow', arguments.callee);
});

$('#popup').live('pageshow', function (event, ui) {
    var li = "";

    for (var attr in selectedFeature.attributes) {
        li += "<li><div style='width:25%;float:left'>" + attr + "</div><div style='width:75%;float:right'>"
        + selectedFeature.attributes[attr] + "</div></li>";
    }
    $("ul#details-list").empty().append(li).listview("refresh");
});

$('#searchpage').live('pageshow', function (event, ui) {
    $('#searchpage').page();
    $('#query').bind('change', function (e) {
        // Prevent form send
        e.preventDefault();

        getLonLatOfAddressAndFindClosestVehicles();
    });
    GoogleAddressService = new IniGoogleAutoComplete();

    // only listen to the first event triggered
    $('#searchpage').die('pageshow', arguments.callee);
});

$('#historypage').live('pageshow', function () {
    if (showHistorySettingPopup) {
        showHistorySettingPopup = false;

        $('#historyCriteria').popup();
        $('#historyCriteria').popup("open");
    }
});

$('#sendEmailPage').live('pageshow', function () {
    $("#company").prop("readonly", true);
    $('#00Ng000000177hz').val(FollowingVehicleName);
    $("#00Ng000000177hz").prop("readonly", true);
    $('#00Ng00000018Oah').val(FollowingBoxId);
    $("#00Ng00000018Oah").prop("readonly", true);
    $('#00Ng00000016f5w').val(FollowingVehicleOdometer);
    $("#00Ng00000016f5w").prop("readonly", true);
    $('#00Ng00000016f6B').val(FollowingVehicleFuelType).selectmenu('refresh', true);
    $('#00Ng00000016f66').val(FollowingVehicleEngineDisplacement);
    $("#00Ng00000016f66").prop("readonly", true);
    $('#00Ng0000001L0XQ').val(FollowingVehicleOdometerUnits).selectmenu('refresh', true);

});

//var addressTryTimes = 0;

function getLonLatOfAddressAndFindClosestVehicles() {
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ "address": $('#query').val() }, function (results, status) {
        //addressTryTimes++;
        if (status == google.maps.GeocoderStatus.OK) {
            searchLat = results[0].geometry.location.lat();
            searchLon = results[0].geometry.location.lng();
            findClosestVehicles();
        }
    });
}

function findClosestVehicles() {

    if (findingClosestVehicles)
        return;

    /*if (searchLon == 0 && searchLat == 0 && addressTryTimes == 0) {
        
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ "address": $('#query').val() }, function (results, status) {
            addressTryTimes++;
            if (status == google.maps.GeocoderStatus.OK) {
                searchLat = results[0].geometry.location.lat();
                searchLon = results[0].geometry.location.lng();
                findClosestVehicles();
            }
        });

        return;
    }

    addressTryTimes = 0;*/

    if (searchLon == 0 && searchLat == 0)
        return;

    findingClosestVehicles = true;

    $('#search_results').empty();
    /*if ($('#query')[0].value === '') {
        findingClosestVehicles = false;
        return;
    }*/
    $.mobile.showPageLoadingMsg();

    //var searchUrl = rootPath + 'Home/_GetAddressLatLon/0?s=' + encodeURIComponent($('#query')[0].value);

    //$.getJSON(searchUrl, function (data) {

    try {
        //if (data.lat != 0 && data.lon != 0) {
        //alert(searchLon + ',' + searchLat);
        $.ajax({
            url: rootPath + 'Home/_FindMyVehiclesByPosition/' + searchLon + ',' + searchLat + ',' + CurrentFleetId + '?searchNumVehicles=' + searchNumVehicles + '&searchRadius=' + searchRadius,
            success: function (result) {
                if (result.indexOf("<!DOCTYPE html>") != -1) {
                    $.mobile.hidePageLoadingMsg();
                    location.replace(rootPath + 'Account/Login');
                    return;
                }
                //alert(result);
                $('#search_results').append(result).listview('refresh');
                $.mobile.hidePageLoadingMsg();
                findingClosestVehicles = false;
            },
            error: function (msg) {
                $.mobile.hidePageLoadingMsg();
                findingClosestVehicles = false;
            }
        });

        //}
        //else {
        //    $.mobile.hidePageLoadingMsg();
        //    findingClosestVehicles = false;
        //}
    }
    catch (e) {
        $.mobile.hidePageLoadingMsg();
        findingClosestVehicles = false;
    }


    //});
}


function initLayerList() {
    $('#layerspage').page();
    $('<li>', {
        "data-role": "list-divider",
        text: ResourceBaseLayers
    })
        .appendTo('#layerslist');
    var baseLayers = map.getLayersBy("isBaseLayer", true);
    $.each(baseLayers, function () {
        addLayerToList(this);
    });

    $('<li>', {
        "data-role": "list-divider",
        text: ResourceOverlayLayers
    })
        .appendTo('#layerslist');
    var overlayLayers = map.getLayersBy("isBaseLayer", false);
    $.each(overlayLayers, function () {
        if (this.displayInLayerSwitcher)
            addLayerToList(this);
    });
    $('#layerslist').listview('refresh');

    map.events.register("addlayer", this, function (e) {
        addLayerToList(e.layer);
    });
}

function addLayerToList(layer) {
    var item = $('<li>', {
        "data-icon": "check",
        "class": layer.visibility ? "checked" : ""
    })
        .append($('<a />', {
            text: layer.name
        })
            .click(function () {
                $.mobile.changePage('#mappage');
                if (layer.isBaseLayer) {
                    //layer.map.setBaseLayer(layer);
                    //if (layer.name.indexOf("Google") != -1)
                    //{
                    //    //alert("fix it");
                    //    //fixContentHeight();
                    //}
                    setBaseLayer = true;
                    baseLayer = layer;
                } else {
                    layer.setVisibility(!layer.getVisibility());
                }
            })
        )
        .appendTo('#layerslist');
    layer.events.on({
        'visibilitychanged': function () {
            $(item).toggleClass('checked');
        }
    });
}

function mapVehicle(boxId, trytimes) {

    attributes = eval("(" + $('#vehicle_0_' + boxId).attr('data-attr') + ")");
    //alert(attributes.LicensePlate);

    allVehicles = [];
    allVehicles.push(attributes);
    mapSelectedVehicle(0);
}

function mapSelectedVehicle(trytimes) {
    if (trytimes == undefined) trytimes = 0;
    if (map == undefined) {
        if (trytimes > 5) return;
        setTimeout(function () { mapSelectedVehicle(trytimes++); }, 100);
        return;
    }

    if (sprintersLayer && map) {
        sprintersLayer.removeAllFeatures();
    }

    if (allVehicles.length == 0) return;

    vehicleFeatures = [];
    var bounds = new OpenLayers.Bounds();
    var newLoc;
    var mapFollows = false;

    for (i = 0; i < allVehicles.length; i++) {
        //if (followingFeature != null && followingFeature.BoxId == allVehicles[i].BoxId) {
        if (HistoryBoxId == allVehicles[i].BoxId || FollowingBoxId == allVehicles[i].BoxId) {
            mapFollows = true;
        }
        var _v = allVehicles[i];
        newLoc = transformCoords(_v.lon, _v.lat);
        var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
        var marker = new OpenLayers.Feature.Vector(point);
        bounds.extend(newLoc);

        marker.attributes = _v;

        vehicleFeatures.push(marker);
    }
    sprintersLayer.addFeatures(vehicleFeatures);

    if (!mapFollows) {
        allFollows = [];
        followLayer.removeAllFeatures();
    }

    if (allVehicles.length > 1) {
        map.zoomToExtent(bounds, true);
        map.setCenter(bounds.getCenterLonLat());
        var currentZoom = map.getZoom();
        map.zoomTo(currentZoom - 1);
    }
    else
        map.setCenter(newLoc, 14);
}

function transformCoords(lon, lat) {
    return new OpenLayers.LonLat(lon, lat)
   .transform(
   new OpenLayers.Projection("EPSG:4326"),
   map.getProjectionObject()
   //new OpenLayers.Projection("EPSG:900913")
   );
}

//$(".checkBoxLeft").live('click', function () {
//    if ($(this).find('input[type="checkbox"]').is(':checked')) {
//        $(this).removeClass('checkBoxLeftChecked').addClass('checkBoxLeftNotChecked');
//        $(this).find('input[type="checkbox"]').attr('checked', false);
//    } else {
//        $(this).removeClass('checkBoxLeftNotChecked').addClass('checkBoxLeftChecked');
//        $(this).find('input[type="checkbox"]').attr('checked', true);
//    }
//});

$(".checkBoxToggle").live('click', function () {
    if ($(this).find('input[type="checkbox"]').is(':checked')) {
        $(this).removeClass('checkBoxLeftChecked').addClass('checkBoxLeftNotChecked');
        $(this).find('input[type="checkbox"]').attr('checked', false);
    } else {
        $(this).removeClass('checkBoxLeftNotChecked').addClass('checkBoxLeftChecked');
        $(this).find('input[type="checkbox"]').attr('checked', true);
    }
});

$(".singleCheckBoxLeft").live('click', function () {
    $('.singleCheckBoxLeft').removeClass('checkBoxLeftChecked');
    $(this).addClass('checkBoxLeftChecked');
});

/*function pushToallVehicles(o, vehicle) {
    var index = vehicleInArray(vehicle, allVehicles);
    if (!$(o).find('input[type="checkbox"]').is(':checked')) {
        if(index==-1)
            allVehicles.push(vehicle);
    }
    else {        
        if(index > -1)
            allVehicles.splice(index, 1);
    }
    ///alert(allVehicles.length);
}*/

function pushToallVehicles(o, boxId) {
    var vehicle = eval("(" + $('#vehicle_0_' + boxId).attr('data-attr') + ")");
    var index = vehicleInArray(vehicle, allVehicles);
    if (!$(o).find('input[type="checkbox"]').is(':checked')) {
        if (index == -1)
            allVehicles.push(vehicle);
    }
    else {
        if (index > -1)
            allVehicles.splice(index, 1);
    }
    ///alert(allVehicles.length);
}

function vehicleInArray(v, a) {
    for (i = 0; i < a.length; i++) {
        if (v.BoxId == a[i].BoxId)
            return i;
    }
    return -1;
}

function loadVehicleByFleet(fleetId, pageIndex, changePageToVehicleList, keepSelects) {
    if (VehicleListPageLazyLoading)
        return;
    if (changePageToVehicleList == undefined) changePageToVehicleList = false;
    if (keepSelects == undefined) keepSelects = false;
    if (pageIndex == undefined) pageIndex = 1;
    if (pageIndex == 1) {
        VehicleListPageLoadingFinished = false;
        CurrentFleetId = fleetId;
        currentVehicleListPageIndex = 2;
    }
    /*if (isSearch == undefined) isSearch = 0;
    if (searchString == undefined || searchString == '') {
        isSearch = 0;
        searchString = 'na';
    }*/

    VehicleListPageLazyLoading = true;
    if (pageIndex == 1)
        $.mobile.showPageLoadingMsg();

    var checkedVehicleIds = ';';
    if (keepSelects || pageIndex != 1) {
        for (i = 0; i < allVehicles.length; i++) {
            var _v = allVehicles[i];
            checkedVehicleIds += _v.VehicleId + ';';
        }
    }
    if (CurrentSearchString == '') {
        SearchMode = 0;
    }

    $.ajax({
        url: loadVehicleByFleetPath + fleetId + "," + pageIndex + "," + SearchMode + "," + (CurrentSearchString == '' ? 'na' : CurrentSearchString) + "," + checkedVehicleIds,
        success: function (result) {

            VehicleListPageLazyLoading = false;
            if (result.indexOf("<!DOCTYPE html>") != -1) {
                location.replace(rootPath + 'Account/Login');
                return;
            }
            if (pageIndex == 1) {
                $('#ulVehicleList').html(result).listview('refresh');
                if (!keepSelects)
                    allVehicles = [];
                if (changePageToVehicleList) {
                    $.mobile.changePage('#vehicleList');
                }
            }
            else {
                $('#ulVehicleList').append(result).listview('refresh');
            }
            if (result.indexOf("checkBoxLeft") == -1) VehicleListPageLoadingFinished = true;

            $.mobile.hidePageLoadingMsg();
        },
        error: function (msg) {
            $('#listviewmsg').html('It has some problem loading the vehilces. Please try again later.');
            VehicleListPageLazyLoading = false;
            $.mobile.hidePageLoadingMsg();
        }
    });


}

function loadFleets(fleetId, pageIndex) {
    if (FleetListPageLazyLoading)
        return;

    if (pageIndex == undefined) pageIndex = 1;
    if (pageIndex == 1) {
        FleetListPageLoadingFinished = false;
        currentFleetListPageIndex = 2;
    }
    /*if (isSearch == undefined) isSearch = 0;
    if (searchString == undefined || searchString == '') {
        isSearch = 0;
        searchString = 'na';
    }*/

    FleetListPageLazyLoading = true;
    if (pageIndex == 1)
        $.mobile.showPageLoadingMsg();

    if (FleetCurrentSearchString == '')
        FleetSearchMode = 0;

    if (FleetType == 'hierarchy' && FleetCurrentSearchString == '') {
        $('#ulFleetPage').html('');
        $.mobile.hidePageLoadingMsg();
        FleetListPageLazyLoading = false;
        return;
    }

    $.ajax({
        url: loadFleetPath + fleetId + "," + pageIndex + "," + FleetSearchMode + "," + (FleetCurrentSearchString == '' ? 'na' : FleetCurrentSearchString),
        success: function (result) {
            FleetListPageLazyLoading = false;
            if (result.indexOf("<!DOCTYPE html>") != -1) {
                $.mobile.hidePageLoadingMsg();
                return;
            }
            if (pageIndex == 1) {
                $('#ulFleetPage').html(result).listview('refresh');
            }
            else {
                $('#ulFleetPage').append(result).listview('refresh');
            }
            FleetPageIni = true;
            if (result.indexOf("checkBoxLeft") == -1) FleetListPageLoadingFinished = true;

            $.mobile.hidePageLoadingMsg();
        },
        error: function (msg) {
            $('#listviewmsg').html('It has some problem loading the fleets. Please try again later.');
            FleetListPageLazyLoading = false;
            $.mobile.hidePageLoadingMsg();
        }
    });


}

function markFleetChecked(o) {
    $('.singleCheckBoxLeft').removeClass('checkBoxLeftChecked');
    $(o).parent().find('div.singleCheckBoxLeft').addClass('checkBoxLeftChecked');
}

function markVehicleChecked(o) {
    $('.checkBoxLeft').removeClass('checkBoxLeftChecked');
    $(o).parent().find('div.checkBoxLeft').addClass('checkBoxLeftChecked');
}

function isAtBottom() {

    var totalHeight, currentScroll, visibleHeight;

    if (document.documentElement.scrollTop)
    { currentScroll = document.documentElement.scrollTop; }
    else
    { currentScroll = document.body.scrollTop; }

    totalHeight = document.body.offsetHeight;
    visibleHeight = document.documentElement.clientHeight;

    return (totalHeight <= currentScroll + visibleHeight)
}

function searchVehicleList(fleetId) {
    //alert($('#vehicleListSearch').val());
    CurrentSearchString = $('#vehicleListSearch').val();
    SearchMode = 1;
    loadVehicleByFleet(fleetId, 1);
}

function setSearchCriteria() {
    searchNumVehicles = $('#selSearchNumVehicles').val();
    searchRadius = $('#selSearchRadius').val();
}

function follow(o) {
    //boxId = $(o).attr('rel');
    //followingFeature = selectedFeature;
    followingFeature = jQuery.extend(true, {}, selectedFeature);
    boxId = followingFeature.attributes.BoxId;
    FollowingVehicleId = 0;
    FollowingVehicleName = "";
    vehicleInfoForEmail = "";
    setFuelConfigurationForEmail = "";
    boxChangeInfoForEmail = '';
    clearConfig();
    //alert(followingFeature.attributes.LicensePlate);
    //$('#liHistory').hide();

    $('ul#follow_results').empty();
    followLayer.removeAllFeatures();
    allFollows = [];
    clearInterval(FollowIntervalId);

    $('ul#history_results').empty();
    HistoryBoxId = "";
    HistoryVehicleId = 0;

    emailsubject = "";
    emailcontent = "";

    if (boxId == FollowingBoxId) {
        FollowingBoxId = "";
        $('#liFollow').hide();
        $(o).find('.ui-btn-text').html(ResourceFollow);
    }
    else {
        FollowingBoxId = boxId;
        FollowingVehicleId = followingFeature.attributes.VehicleId;
        FollowingVehicleName = followingFeature.attributes.Description;
        $('#followPageTitle').html(ResourceFollow + ': ' + followingFeature.attributes.Description);

        $('#liFollow a .ui-btn-text').html(ResourceFollow);
        $('#liFollow a').attr('onclick', '');
        $('#liFollow a').unbind('click');
        $('#liFollow a').bind('click', function () {
            $.mobile.changePage('#followpage');
        });
        $('#liFollow').show();
        $(o).find('.ui-btn-text').html(ResourceUnfollow);

        //emailsubject = followingFeature.attributes.Description;
        emailsubject = FollowingBoxId;
        //$("#emailfollowpage").attr('href', "mailto:" + emailrecipients + "?subject=" + encodeURIComponent(emailsubject) + "&body=" + emailcontent);

        FollowIntervalId = setInterval(function () {
            getLastestVehicleInfo(FollowingBoxId);
        }, FollowInterval);
    }

    $('#popup').popup('close');
}

function showhistory(o) {
    //boxId = $(o).attr('rel');
    historyFeature = selectedFeature;

    // Unfollow
    FollowingVehicleId = 0;
    FollowingVehicleName = "";
    vehicleInfoForEmail = "";
    setFuelConfigurationForEmail = "";
    FollowingBoxId = '';
    clearConfig();

    $('ul#follow_results').empty();
    followLayer.removeAllFeatures();
    allFollows = [];
    clearInterval(FollowIntervalId);

    HistoryVehicleId = historyFeature.attributes.VehicleId;
    HistoryBoxId = historyFeature.attributes.BoxId;

    showHistorySettingPopup = true;

    $('#liFollow a .ui-btn-text').html(ResourceHistory);
    $('#liFollow a').attr('onclick', '');
    $('#liFollow a').unbind('click');
    $('#liFollow a').bind('click', function () {
        $.mobile.changePage('#historypage');
    });
    $('#liFollow').show();

    $('#popup').popup('close');
    $('#historyPageTitle').html(ResourceHistory + ': ' + historyFeature.attributes.Description);
    $.mobile.changePage('#historypage');
}

function config(o) {

    ConfigBoxId = selectedFeature.attributes.BoxId;
    ConfigVehicleId = selectedFeature.attributes.VehicleId;

    vehicleInfo(o);

    //FollowingLicensePlate = followingFeature.attributes.LicensePlate;
    //FollowingVehicleId = 0;

    //alert(FollowingLicensePlate);
    if (EnableConfigPage && ConfigLicensePlate != selectedFeature.attributes.LicensePlate) {

        ConfigLicensePlate = selectedFeature.attributes.LicensePlate;
        clearConfig();

        $('#updatePositionConfig').hide();
        $('#getBoxStatusConfig').hide();
        $('#SetFuelConfigurationConfig').hide();
        $('#configOdometerConfig').hide();
        $('#swapBtn').hide();

        $.mobile.showPageLoadingMsg();

        var url = rootPath + 'Home/_getVehicleCommands/' + ConfigBoxId + ',' + ConfigLicensePlate;

        $.getJSON(url, function (data) {

            try {
                if (data.hasCommands == 1) {
                    if (data.setOdometer == 'true')
                        $('#configOdometerConfig').show();
                    if (data.updatePosition == 'true')
                        $('#updatePositionConfig').show();
                    if (data.setFuelConfiguration == 'true')
                        $('#SetFuelConfigurationConfig').show();
                    if (data.getboxstatus == 'true')
                        $('#getBoxStatusConfig').show();
                }

            }
            catch (e) {
            }

            $.mobile.hidePageLoadingMsg();

        });
    }

    $('#configPageTitle').html(selectedFeature.attributes.Description);
    //$('#configVehicleName').val(selectedFeature.attributes.Description);
    $.mobile.changePage('#vehicleInfoPage');

    $('#popup').popup('close');
}

var unassignedBoxIdOptions = '';

function vehicleInfo(o) {
    //boxId = $(o).attr('rel');
    var vehicleInfoBoxId = selectedFeature.attributes.BoxId;
    var vehicleInfoVehicleId = selectedFeature.attributes.VehicleId;
    var vehicleInfoLicensePlate = selectedFeature.attributes.LicensePlate;

    if ($('#selVehiceInfoVehicleType').children('option').length <= 1) {
        $('#selVehiceInfoBox')
            .find('option')
            .remove()
            .end()
            .append('<option value="' + ConfigBoxId + '">' + ConfigBoxId + '</option>')
            .val(ConfigBoxId)
            .selectmenu("refresh", true)
        ;

        var url = rootPath + 'Home/_getVehicleInfoComboList/';

        $.getJSON(url, function (data) {

            try {
                if (data.hasVehicleVehicelType == 1) {
                    //alert(data.vehicletypeoptions);                    
                    $('#selVehiceInfoVehicleType').html(data.vehicletypeoptions);
                }
                if (data.hasMake == 1) {
                    $('#selVehiceInfoMake').html(data.makeoptions);
                }
                if (data.hasProvince == 1) {
                    $('#vehicleInfoProvince').html(data.provinceoptions);
                }
                if (data.hasUnassignedBoxId == 1) {
                    unassignedBoxIdOptions = data.unassignedBoxIdOptions;
                    $('#selVehiceInfoBox')
                        .find('option')
                        .end()
                        .append(unassignedBoxIdOptions);
                }

            }
            catch (e) {
            }

            loadVehicleInfo(vehicleInfoLicensePlate);
        });
    }
    else {
        $('#selVehiceInfoBox')
            .find('option')
            .remove()
            .end()
            .append('<option value="' + ConfigBoxId + '">' + ConfigBoxId + '</option>')
            .append(unassignedBoxIdOptions)
            .val(ConfigBoxId)
            .selectmenu("refresh", true)
        ;

        loadVehicleInfo(vehicleInfoLicensePlate);
    }

    $('#popup').popup('close');
}

var makeModelId = -1;

function loadVehicleInfo(licencePlate) {
    var url = rootPath + 'Home/_getVehicleInfoByLicensePlate/' + licencePlate;
    $.mobile.showPageLoadingMsg();
    $.getJSON(url, function (data) {

        try {
            if (data.hasVehicleInfo == 1) {
                $('#vehicleInfoVIN').val(data.VIN);
                $('#vehicleInfoLicensePlate').val(data.LicensePlate);

                $("select#selVehiceInfoVehicleType option")
                    .each(function () { this.selected = (this.text == data.VehicleTypeName); });
                $("select#selVehiceInfoVehicleType").selectmenu("refresh", true);

                $("select#selVehiceInfoMake option")
                    .each(function () { this.selected = (this.text == data.MakeName); });
                $("select#selVehiceInfoMake").selectmenu("refresh", true);

                $("select#vehicleInfoProvince option")
                    .each(function () { this.selected = (this.text == data.StateProvince); });
                $("select#vehicleInfoProvince").selectmenu("refresh", true);

                $('#vehicleInfoYear').val(data.ModelYear);
                $('#vehicleInfoColor').val(data.Color);

                var patt = /[bB][oO][xX][ ]*[(1-9)]+/;
                if (patt.test(data.Description))
                    $('#swapBtn').show();
                else
                    $('#swapBtn').hide();

                // set up hidden fields
                $('#vehicleInfoOldLicense').val(data.LicensePlate);
                $('#vehicleInfoCost').val(data.Cost);
                $('#vehicleInfoDescription').val(data.Description);
                $('#vehicleInfoVehicleId').val(data.VehicleId);
                $('#vehicleInfoBoxId').val(data.BoxId);
                $('#vehicleInfoIconTypeId').val(data.IconTypeId);
                $('#vehicleInfoEmail').val(data.Email);
                $('#vehicleInfoPhone').val(data.Phone);
                $('#vehicleInfoTimeZone').val(data.TimeZone);
                $('#vehicleInfoAutoAdjustDayLightSaving').val(data.AutoAdjustDayLightSaving);
                $('#vehicleInfoFuelType').val(data.FuelType);
                $('#vehicleInfoNotify').val(data.Notify);
                $('#vehicleInfoWarning').val(data.Warning);
                $('#vehicleInfoCritical').val(data.Critical);
                $('#vehicleInfoMaintenance').val(data.Maintenance);
                $('#vehicleInfoServiceConfigID').val(data.ServiceConfigID);

                makeModelId = data.MakeModelId;

                loadModelInfo(true);
            }

        }
        catch (e) {
        }

        $.mobile.hidePageLoadingMsg();

    });
}

function loadModelInfo(setSeatbeltOdometer) {
    if (setSeatbeltOdometer == undefined) {
        setSeatbeltOdometer = false;
    }

    var urlModel = rootPath + 'Home/_getModelByMakeId/' + $("#selVehiceInfoMake").val();

    $.mobile.showPageLoadingMsg();
    $.getJSON(urlModel, function (data) {

        try {
            if (data.hasModelList == 1) {
                $('#selVehiceInfoModel').html(data.modeloptions);

                $("select#selVehiceInfoModel").val(makeModelId);
                $("select#selVehiceInfoModel").selectmenu("refresh", true);

                if (setSeatbeltOdometer) {
                    checkSetSeatBeltOdometer();
                }
            }

        }
        catch (e) {
        }

        $.mobile.hidePageLoadingMsg();

    });
}

function checkSetSeatBeltOdometer() {
    var url = rootPath + 'Home/_checkSetSeatBeltOdometer/' + encodeURIComponent($("#selVehiceInfoMake option:selected").text()) + "," + encodeURIComponent($("#selVehiceInfoModel option:selected").text()) + "," + encodeURIComponent($('#vehicleInfoYear').val());

    $.getJSON(url, function (data) {

        try {
            if (data.found == "1") {
                $('#vehicleInfoSetSeatbeltOdometerBtn').removeClass('ui-disabled');
            }

        }
        catch (e) { alert(e); }

    });
}

function disableSetSeatbeltOdometerBtn() {
    $('#vehicleInfoSetSeatbeltOdometerBtn').addClass('ui-disabled');
}

function getLastestVehicleInfo(boxId) {

    if (LoadFollowingData)
        return;

    LoadFollowingData = true;

    var url = rootPath + 'Home/GetVehicleLastKnown/' + FollowingVehicleId + '?startDateTime=' + encodeURIComponent(followingFeature.attributes.OriginDateTime);
    //var url = rootPath + 'Home/GetVehicleLastKnown/31522?startDateTime=' + encodeURIComponent('05/14/2013 10:11:49 AM');

    $.getJSON(url, function (data) {

        LoadFollowingData = false;

        try {
            if (data.success == 0 && data.Msg == "sessiontimeout") {
                location.replace(rootPath + 'Account/Login');
                return;
            }

            if (data.success == 1 && data.hasData == 1) {

                for (i = data.data.length - 1; i >= 0; i--) {
                    var odometer = '';
                    var fuel = '';
                    var rpm = '';
                    var customprop = '';

                    odometer = getValueByKey('odometer', data.data[i].CustomProp);
                    fuel = getValueByKey('fuel', data.data[i].CustomProp);
                    rpm = getValueByKey('rpm', data.data[i].CustomProp);

                    if (odometer != '')
                        customprop += ResourceOdometer + '=' + odometer + '; ';
                    if (rpm != '')
                        customprop += ResourceRPM + '=' + rpm + '; ';
                    if (fuel != '')
                        customprop += ResourceFuel + '=' + fuel + '; ';

                    var validgps = data.data[i].ValidGps;
                    if (validgps == 0)
                        customprop += ResourceGpsValid + "; ";
                    else if (validgps != -1)
                        customprop += ResourceGpsInvalid + "; ";

                    if (data.data[i].MsgDetails != '')
                        customprop = '; ' + customprop;

                    var li = "<li><h2>" + data.data[i].displayDateTime + "</h2>" +
                        "<p>" + ResourceSpeed + ": " + data.data[i].Speed + "</p>" +
                        "<p>" + ResourceMessage + ": " + data.data[i].MsgDetails + " " + customprop + "</p>" +
                        "<p>" + data.data[i].Address + "</p>" +
                        "</li>";

                    //var emailli = encodeURIComponent(data.data[i].displayDateTime) + "%0D" +
                    var emailli = data.data[i].displayDateTime + "\n" +
                            ResourceSpeed + ": " + data.data[i].Speed + "\n" +
                            ResourceMessage + ": " + data.data[i].MsgDetails + " " + customprop + "\n" +
                            data.data[i].Address + "\n\n";

                    followingFeature.attributes.OriginDateTime = data.data[i].originDateTime;
                    followingFeature.attributes.DisplayDateTime = data.data[i].displayDateTime;

                    $("ul#follow_results").prepend(li);

                    if ($('ul#follow_results').children().length > FollowMaxRecords) {
                        $('ul#follow_results').children().slice(FollowMaxRecords, $('ul#follow_results').children().length).remove();
                    }

                    $("ul#follow_results").listview("refresh");
                    //alert(followingFeature.attributes.LicensePlate);

                    mapFollow({
                        BoxId: followingFeature.attributes.BoxId,
                        VehicleId: followingFeature.attributes.VehicleId,
                        Description: followingFeature.attributes.Description,
                        OriginDateTime: followingFeature.attributes.OriginDateTime,
                        DisplayDateTime: followingFeature.attributes.DisplayDateTime,
                        LicensePlate: followingFeature.attributes.LicensePlate,
                        Speed: data.data[i].Speed,
                        Status: data.data[i].Status,
                        Location: data.data[i].Address,
                        //Description: '', 
                        MyHeading: data.data[i].MyHeading,
                        MyHeadingIcon: data.data[i].MyHeadingIcon,
                        lon: data.data[i].Lon,
                        lat: data.data[i].Lat,
                        //icon: data.data[i].icon
                        icon: "/mobile/content/images/" + data.data[i].icon,
                        MsgDetails: data.data[i].MsgDetails
                    });

                    //emailsubject = followingFeature.attributes.Description;
                    emailsubject = followingFeature.attributes.BoxId;
                    emailcontent = emailli + emailcontent;
                    //alert(emailcontent);
                    //$("#emailfollowpage").attr('href', "mailto:" + emailrecipients + "?subject=" + encodeURIComponent(emailsubject) + "&body=" + emailcontent);                    
                }
            }

        }
        catch (e) {
        }

    });
}

function saveVehicleInfo() {

    $('#vehicleInfoMsg').css('color', 'green').html('');
    $('#vehicleInfoLicensePlateMsg').css('color', 'green').html('');

    newDescription = $('#vehicleInfoDescription').val();
    if (newDescription == '') {
        $('#vehicleInfoDescription').addClass('textboxError');
        $('#vehicleInfoDescription').focus();
        return;
    }

    var regex = new RegExp("^[a-zA-Z0-9]+$");

    var vin = $('#vehicleInfoVIN').val();
    if (vin.length != 17) {
        $('#vehicleInfoVIN').addClass('textboxError');
        $('#vehicleInfoVIN').focus();
        $('#vehicleInfoVINMsg').css('color', 'red').html(ResourceVIN17Characters);
        return;
    }
    if (!regex.test(vin)) {
        $('#vehicleInfoVIN').addClass('textboxError');
        $('#vehicleInfoVIN').focus();
        $('#vehicleInfoVINMsg').css('color', 'red').html('Alphanumeric Only');
        return;
    }

    var newlp = $('#vehicleInfoLicensePlate').val();
    if (newlp == '') {
        $('#vehicleInfoLicensePlate').addClass('textboxError');
        $('#vehicleInfoLicensePlate').focus();
        return;
    }
    if (!regex.test(newlp)) {
        $('#vehicleInfoLicensePlate').addClass('textboxError');
        $('#vehicleInfoLicensePlate').focus();
        $('#vehicleInfoLicensePlateMsg').css('color', 'red').html('Alphanumeric Only');
        return;
    }

    $('#vehicleInfoDescription').removeClass('textboxError');
    $('#vehicleInfoVIN').removeClass('textboxError');
    $('#vehicleInfoVINMsg').css('color', 'green').html('');
    $('#vehicleInfoLicensePlate').removeClass('textboxError');

    vehicleInfoForEmail = '';

    $('#vehicleInfoMsg').css('color', 'green').html('').show();

    $.ajax({
        type: 'POST',
        url: rootPath + 'Home/_saveVehicleInfo/',
        //data: $('#formVehicleInfo').serialize(),
        data: JSON.stringify($('#formVehicleInfo').serializeObject()),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data) {
            if (data.success == "1") {
                $('#vehicleInfoOldLicense').val($('#vehicleInfoLicensePlate').val());
                $('#vehicleInfoMsg').css('color', 'green').html(ResourceSuccessfullySaved).show();

                //$('#vehicleInfoMsg').delay(1000).html('');
                vehicleInfoForEmail = ResourceVin + ": " + $('#vehicleInfoVIN').val() + "\n" +
                                      ResourceLicensePlace + ": " + $('#vehicleInfoLicensePlate').val() + "\n" +
                                      ResourceVehicleType + ": " + $("#selVehiceInfoVehicleType option:selected").text() + "\n" +
                                      ResourceMake + ": " + $("#selVehiceInfoMake option:selected").text() + "\n" +
                                      ResourceModel + ": " + $("#selVehiceInfoModel option:selected").text() + "\n" +
                                      ResourceYear + ": " + $('#vehicleInfoYear').val() + "\n" +
                                      ResourceColor + ": " + $('#vehicleInfoColor').val() + "\n" +
                                      ResourceStateProvince + ": " + $('#vehicleInfoProvince').val() + "\n";
                checkSetSeatBeltOdometer();

                var oldDescription = selectedFeature.attributes.Description;
                if (oldDescription != newDescription) {
                    $('#vehicleList #listviewwrapper ul li#livehiclelist_' + selectedFeature.attributes.VehicleId + ' h2').html(newDescription + ' ');
                    var s1 = "', Description: '" + oldDescription + "',";
                    var s2 = "', Description: '" + newDescription + "',";
                    var h = $('#vehicleList #listviewwrapper ul li#livehiclelist_' + selectedFeature.attributes.VehicleId).html();
                    h = h.replace(new RegExp(s1, 'g'), s2);
                    $('#vehicleList #listviewwrapper ul li#livehiclelist_' + selectedFeature.attributes.VehicleId).html(h);

                    selectedFeature.attributes.Description = newDescription;
                    $('#configPageTitle').html(newDescription);
                    $('#followPageTitle').html(ResourceFollow + ': ' + newDescription);
                    FollowingVehicleName = newDescription;
                }

                boxChangeInfoForEmail = '';

                // Box assignment changed
                if ($('#vehicleInfoBoxId').val() != $('#selVehiceInfoBox').val()) {
                    boxChangeInfoForEmail = ResourceOldBoxId + ": " + $('#vehicleInfoBoxId').val() + "\n";
                    boxChangeInfoForEmail += ResourceNewBoxId + ": " + $('#selVehiceInfoBox').val() + "\n";
                    var s1 = "{ BoxId: " + $('#vehicleInfoBoxId').val() + ",";
                    var s2 = "{ BoxId: " + $('#selVehiceInfoBox').val() + ",";
                    var h = $('#vehicleList #listviewwrapper ul li#livehiclelist_' + selectedFeature.attributes.VehicleId).html();
                    h = h.replace(new RegExp(s1, 'g'), s2);

                    $('#vehicleList #listviewwrapper ul li#livehiclelist_' + selectedFeature.attributes.VehicleId).html(h);

                    unassignedBoxIdOptions = '<option value="' + $('#vehicleInfoBoxId').val() + '">' + $('#vehicleInfoBoxId').val() + '</option>' + unassignedBoxIdOptions;
                    s1 = '<option value="' + $('#selVehiceInfoBox').val() + '">' + $('#selVehiceInfoBox').val() + '</option>';
                    s2 = '';
                    unassignedBoxIdOptions = unassignedBoxIdOptions.replace(new RegExp(s1, 'g'), s2);

                    selectedFeature.attributes.BoxId = $('#selVehiceInfoBox').val();
                    $('#vehicleInfoBoxId').val(selectedFeature.attributes.BoxId);
                    ConfigBoxId = selectedFeature.attributes.BoxId;
                }

                /// follow the vehicle automatically
                if (ConfigBoxId != FollowingBoxId) {
                    setFuelConfigurationForEmail = "";

                    followingFeature = selectedFeature;

                    $('ul#follow_results').empty();
                    followLayer.removeAllFeatures();
                    allFollows = [];
                    clearInterval(FollowIntervalId);

                    $('ul#history_results').empty();
                    HistoryBoxId = "";
                    HistoryVehicleId = 0;

                    emailsubject = "";
                    emailcontent = "";

                    FollowingBoxId = ConfigBoxId;
                    FollowingVehicleId = followingFeature.attributes.VehicleId;
                    FollowingVehicleName = followingFeature.attributes.Description;

                    $('#followPageTitle').html(ResourceFollow + ': ' + followingFeature.attributes.Description);

                    $('#liFollow a .ui-btn-text').html(ResourceFollow);
                    $('#liFollow a').attr('onclick', '');
                    $('#liFollow a').unbind('click');
                    $('#liFollow a').bind('click', function () {
                        $.mobile.changePage('#followpage');
                    });
                    $('#liFollow').show();
                    //$(o).find('.ui-btn-text').html(ResourceUnfollow);

                    //emailsubject = followingFeature.attributes.Description;
                    emailsubject = FollowingBoxId;
                    //$("#emailfollowpage").attr('href', "mailto:" + emailrecipients + "?subject=" + encodeURIComponent(emailsubject) + "&body=" + emailcontent);

                    FollowIntervalId = setInterval(function () {
                        getLastestVehicleInfo(FollowingBoxId);
                    }, FollowInterval);
                }

            }
            else {
                $('#vehicleInfoMsg').css('color', 'red').html(ResourceFailedToSave).show();
            }

        },
        error: function (msg) {
            $('#vehicleInfoMsg').css('color', 'red').html(ResourceFailedToSave).show();
        }
    });
}

function swapVehicle() {
    $('#swapConfirm').popup();
    $('#swapConfirm').popup('open');
}

function swapVehicleSubmit() {
    $.ajax({
        type: 'POST',
        url: rootPath + 'Home/_swapVehicle/' + $('#vehicleInfoVehicleId').val(),
        //data: $('#formVehicleInfo').serialize(),
        //data: JSON.stringify($('#formVehicleInfo').serializeObject()),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data) {
            if (data.result > 0) {
                $('#vehicleInfoNameMsg').css('color', 'green').html(ResourceSuccessfullySwapVehicle).show().delay(1000).fadeOut(1000);
                unassignedBoxIdOptions = '<option value="' + $('#vehicleInfoBoxId').val() + '">' + $('#vehicleInfoBoxId').val() + '</option>' + unassignedBoxIdOptions;
                window.setTimeout(function () { removeVehicleAfterSwap(); }, 1500);
            }
            else {
                $('#vehicleInfoNameMsg').css('color', 'red').html(ResourceFailedToSwapVehicle).show().delay(1000).fadeOut(1000);
            }
        },
        error: function (msg) {
            $('#vehicleInfoNameMsg').css('color', 'red').html(ResourceFailedToSwapVehicle).show().delay(1000).fadeOut(1000);
        }
    });
}

function removeVehicleAfterSwap() {
    //Goto previous page
    $.mobile.changePage('#mappage');

    //Unfollow

    setFuelConfigurationForEmail = "";
    followingFeature = jQuery.extend(true, {}, selectedFeature);
    boxId = "";
    FollowingVehicleId = 0;
    FollowingBoxId = "";
    FollowingVehicleName = "";
    vehicleInfoForEmail = "";
    setFuelConfigurationForEmail = "";
    clearConfig();

    $('ul#follow_results').empty();
    followLayer.removeAllFeatures();
    allFollows = [];
    clearInterval(FollowIntervalId);

    $('ul#history_results').empty();
    HistoryBoxId = "";
    HistoryVehicleId = 0;
    emailsubject = "";
    emailcontent = "";
    $('#popup').popup('close');
    $('#liFollow').hide();

    //Remove Marker
    sprintersLayer.removeAllFeatures();

    //Remove Vehicle
    $('#vehicleList #listviewwrapper ul li#livehiclelist_' + $('#vehicleInfoVehicleId').val()).remove();
}

function sendEmail() {
    $('#company').removeClass('textboxError');          //Organization
    $('#00Ng00000018gf1').removeClass('textboxError');  //Installer Organization
    $('#00Ng00000018grq').removeClass('textboxError');  //Installer Name
    $('#00Ng00000018gew').removeClass('textboxError');  //Installer Email
    $('#00Ng0000001MI3p').removeClass('textboxError');  //Installer Phone
    //$('#00Ng00000018Oah').removeClass('textboxError');  //Box ID
    //$('#00Ng000000177hz').removeClass('textboxError');  //Vehicle Description
    $('#00Ng00000016kAm').removeClass('textboxError');  //ECM
    $('#00Ng00000016f5w').removeClass('textboxError');  //Odometer
    //$('#00Ng0000001L0XQ').removeClass('textboxError');  //Odometer Units
    //$('#00Ng00000016f61').removeClass('textboxError');  //Engine Hours
    //$('#00Ng00000016f6B').removeClass('textboxError');  //Fuel Type
    //$('#00Ng00000016f66').removeClass('textboxError');  //Engine Displacement
    $('#00Ng00000016jwG').removeClass('textboxError');  //Ignition Source
    $('#00Ng00000016jvw').removeClass('textboxError');  //Power Source
    $('00Ng00000016jZR').removeClass('textboxError');   //Box Location
    $('00Ng00000016jtb').removeClass('textboxError');   //GPS Antenna Location
    $('00Ng00000016juy').removeClass('textboxError');   //Cell Antenna Location

    if ($('#company').val() == '') {
        $('#company').addClass('textboxError');
        $('#company').focus();
        $('#OrganizationMsg').css('color', 'red').html(OrganizationNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000018gf1').val() == '') {
        $('#00Ng00000018gf1').addClass('textboxError');
        $('#00Ng00000018gf1').focus();
        $('#InstallerOrganizationMsg').css('color', 'red').html(InstallerOrganizationNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000018grq').val() == '') {
        $('#00Ng00000018grq').addClass('textboxError');
        $('#00Ng00000018grq').focus();
        $('#InstallerMsg').css('color', 'red').html(ResourceInstallerNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000018gew').val() == '') {
        $('#00Ng00000018gew').addClass('textboxError');
        $('#00Ng00000018gew').focus();
        $('#InstallerEmailMsg').css('color', 'red').html(ResourceEmailToNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    //invalid email address
    var sendEmailInstallerEmail = $('#00Ng00000018gew').val();
    if (sendEmailInstallerEmail != '' && !isValidEmailAddress(sendEmailInstallerEmail)) {
        $('#00Ng00000018gew').addClass('textboxError');
        $('#00Ng00000018gew').focus();
        $('#InstallerEmailMsg').css('color', 'red').html(ResourceInvalidEmailAddress).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng0000001MI3p').val() == '') {
        $('#00Ng0000001MI3p').addClass('textboxError');
        $('#00Ng0000001MI3p').focus();
        $('#InstallerPhoneMsg').css('color', 'red').html(InstallerPhoneNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000016kAm').val() == '') {
        $('#00Ng00000016kAm').addClass('textboxError');
        $('#00Ng00000016kAm').focus();
        $('#ECMMsg').css('color', 'red').html(ECMNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    //odometer should be numeric
    VehicleOdometer = $('#00Ng00000016f5w').val();
    if (VehicleOdometer != '' && !$.isNumeric(VehicleOdometer)) {
        $('#00Ng00000016f5w').addClass('textboxError');
        $('#00Ng00000016f5w').focus();
        $('#OdometerMsg').css('color', 'red').html(InvalidOdometer2).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000016jwG').val() == '') {
        $('#00Ng00000016jwG').addClass('textboxError');
        $('#00Ng00000016jwG').focus();
        $('#IgnitionSourceMsg').css('color', 'red').html(IgnitionSourceNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000016jvw').val() == '') {
        $('#00Ng00000016jvw').addClass('textboxError');
        $('#00Ng00000016jvw').focus();
        $('#PowerSourceMsg').css('color', 'red').html(PowerSourceNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000016jZR').val() == '') {
        $('#00Ng00000016jZR').addClass('textboxError');
        $('#00Ng00000016jZR').focus();
        $('#BoxLocationMsg').css('color', 'red').html(BoxLocationNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000016jtb').val() == '') {
        $('#00Ng00000016jtb').addClass('textboxError');
        $('#00Ng00000016jtb').focus();
        $('#GPSAntennaLocationMsg').css('color', 'red').html(GPSAntennaLocationNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    if ($('#00Ng00000016juy').val() == '') {
        $('#00Ng00000016juy').addClass('textboxError');
        $('#00Ng00000016juy').focus();
        $('#CellAntennaLocationMsg').css('color', 'red').html(CellAntennaLocationNotEmpty).show().delay(1000).fadeOut(1000);
        return;
    }

    $("#formSendEmail").submit();
}

function mapFollow(attributes, trytimes) {
    if (trytimes == undefined) trytimes = 0;
    if (map == undefined) {
        if (trytimes > 5) return;
        setTimeout(function () { mapSelectedVehicle(trytimes++); }, 100);
        return;
    }

    if (!$.isNumeric(attributes.lon) || !$.isNumeric(attributes.lat) || attributes.lon == 0 || attributes.lat == 0)
        return;

    for (j = 0; j < allVehicles.length; j++) {
        if (allVehicles[j].BoxId == attributes.BoxId) {
            try {
                // change current position to history
                //historyFeature = allVehicles[j];

                var oldattr = eval("(" + $('#vehicle_0_' + attributes.BoxId).attr('data-attr') + ")");
                oldattr.DisplayDateTime = attributes.DisplayDateTime;
                oldattr.OriginDateTime = attributes.OriginDateTime;
                oldattr.Speed = attributes.Speed;
                oldattr.Status = attributes.Status;
                oldattr.Location = attributes.Location;
                oldattr.MyHeading = attributes.MyHeading;
                oldattr.MyHeadingIcon = attributes.MyHeadingIcon;
                oldattr.lon = attributes.lon;
                oldattr.lat = attributes.lat;

                $('#vehicle_0_' + attributes.BoxId).attr('data-attr', AttributesToString(oldattr));
                $('#vehicle_datetime_0_' + attributes.BoxId).html(oldattr.DisplayDateTime);
                $('#vehicle_speed_0_' + attributes.BoxId).html(oldattr.Speed);
                $('#vehicle_address_0_' + attributes.BoxId).html(oldattr.Location);
                var newLoc;
                //var _v = historyFeature;
                var _v = allVehicles[j];
                //alert('allVehicles[j].displayDateTime:' + _v.DisplayDateTime);
                //alert('displaytime:' + allVehicles[j].displayDateTime);
                newLoc = transformCoords(_v.lon, _v.lat);
                var hpoint = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                var hmarker = new OpenLayers.Feature.Vector(hpoint);
                var vspeed = _v.Speed.split(" ")[0];

                if ($.isNumeric(vspeed) && vspeed > 0) {
                    _v.icon = "/mobile/content/images/followBlue" + _v.MyHeadingIcon + ".png";
                }
                else {
                    _v.icon = "/mobile/content/images/followBrown.png"
                }
                //hmarker.attributes = _v;
                hmarker.attributes = jQuery.extend(true, {}, _v);

                followLayer.addFeatures([hmarker]);

                allFollows.push(hmarker);

                if (allFollows.length > FollowMaxRecords) {
                    followLayer.removeFeatures(allFollows.slice(0, allFollows.length - FollowMaxRecords));
                    allFollows.splice(0, allFollows.length - FollowMaxRecords);
                }

                // update live position                
                _v = attributes;
                newLoc = transformCoords(_v.lon, _v.lat);
                var point = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                var marker = new OpenLayers.Feature.Vector(point);
                marker.attributes = _v;
                sprintersLayer.removeFeatures([vehicleFeatures[j]]);
                sprintersLayer.addFeatures([marker]);

                vehicleFeatures[j] = marker;
                allVehicles[j] = _v;
            }
            catch (err) {

            }
            break;
        }
    }
}


function IniGoogleAutoComplete() {

    //Google auto completion    
    /*var mapOptions = {
        center: new google.maps.LatLng(43.67746, -79.5850766666667),
        zoom: 13,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var cmap = new google.maps.Map(document.getElementById('map_canvas'),
          mapOptions);
    var input = document.getElementById('query');
    $('#map_canvas').css("display", "none");
    autocomplete = new google.maps.places.Autocomplete(input);
    autocomplete.bindTo('bounds', cmap);
    */
    var autocomplete = new google.maps.places.Autocomplete($("#query")[0], {});

    //var infowindow = new google.maps.InfoWindow();    

    /*google.maps.event.addListener(autocomplete, 'place_changed', function () {

        //infowindow.close();
        
        //input.className = '';
        var place = autocomplete.getPlace();
        
        if (!place.geometry) {
            // Inform the user that the place was not found and return.
            //input.className = 'notfound';
            //alert("The address cannot be found");
            searchLat = 0;
            searchLon = 0;
            return;
        }
        var txt = $('#query').val();
        var numb = txt.match(/\d/g);
        if (numb == null) {
            //alert("The address is not normal street address format, the information might not be displayed correctly.");
        }
        searchLat = place.geometry.location.lat();
        searchLon = place.geometry.location.lng();
    });
    */
    //var autocomplete = new google.maps.places.Autocomplete($("#query")[0], {});

    //google.maps.event.addListener(autocomplete, 'place_changed', function () {
    //    var place = autocomplete.getPlace();
    //    //console.log(place.address_components);
    //});

}

/*function configOdometer() {
    $('#configOdometerMsg').html('');
    odometer = $('#configOdometer').val();
    if (!$.isNumeric(odometer)) {
        $('#configOdometer').addClass('textboxError');
        $('#configOdometer').focus();
    }
    else {
        var url = rootPath + 'Home/_sendCommand/' + FollowingBoxId + ',setodometer,' + odometer;
        
        //$('#configOdometer').hide();
        $('#configOdometerBtn').addClass('ui-disabled');
        $('#configOdometerWaiting').show();
        $.getJSON(url, function (data) {
            
            try {
                if (data.Status == 200) {
                    setTimeout(function () {
                        checkCommandStatus(data.BoxId, data.ProtocolTypeId, data.CommModeId, 'configOdometer');
                    }, 1000);
                    
                }
                else if (data.Status == 500) {
                    $('#configOdometerMsg').css('color', 'red').html(data.Msg);
                    //$('#configOdometer').show();
                    $('#configOdometerBtn').removeClass('ui-disabled');
                    $('#configOdometerWaiting').hide();
                }


            }
            catch (e) {
            }
        });
    }
}*/

function sendCommand(commandName) {
    $('#vehicleInfoMsg').css('color', 'green').html('');
    /// follow the vehicle automatically
    if (ConfigBoxId != FollowingBoxId) {
        vehicleInfoForEmail = '';
        boxChangeInfoForEmail = '';
        followingFeature = selectedFeature;

        $('ul#follow_results').empty();
        followLayer.removeAllFeatures();
        allFollows = [];
        clearInterval(FollowIntervalId);

        $('ul#history_results').empty();
        HistoryBoxId = "";
        HistoryVehicleId = 0;

        emailsubject = "";
        emailcontent = "";

        FollowingBoxId = ConfigBoxId;
        FollowingVehicleId = followingFeature.attributes.VehicleId;
        FollowingVehicleName = followingFeature.attributes.Description;

        $('#followPageTitle').html(ResourceFollow + ': ' + followingFeature.attributes.Description);

        $('#liFollow a .ui-btn-text').html(ResourceFollow);
        $('#liFollow a').attr('onclick', '');
        $('#liFollow a').unbind('click');
        $('#liFollow a').bind('click', function () {
            $.mobile.changePage('#followpage');
        });
        $('#liFollow').show();
        //$(o).find('.ui-btn-text').html(ResourceUnfollow);

        //emailsubject = followingFeature.attributes.Description;
        emailsubject = FollowingBoxId;
        //$("#emailfollowpage").attr('href', "mailto:" + emailrecipients + "?subject=" + encodeURIComponent(emailsubject) + "&body=" + emailcontent);

        FollowIntervalId = setInterval(function () {
            getLastestVehicleInfo(FollowingBoxId);
        }, FollowInterval);
    }


    ///////////////////////////////////////


    //$('#' + commandName + 'Msg').html('');
    $('#configContent .configPageMsg').html('');

    var commandValue = ""
    var cmdName = "";
    var validation = true;

    if (commandName == 'configOdometer') {
        odometer = $('#configOdometer').val();
        selMesUnits = $('#selMesUnits').val();
        if (!$.isNumeric(odometer)) {
            $('#configOdometer').addClass('textboxError');
            $('#configOdometer').focus();
            validation = false;
        }

        if (!validation)
            return;

        cmdName = 'setodometer';
        commandValue = Math.round(odometer / selMesUnits);

        FollowingVehicleOdometer = odometer;
        if (selMesUnits == 1)
            FollowingVehicleOdometerUnits = "KM";
        else
            FollowingVehicleOdometerUnits = "MI";
    }
    else if (commandName == 'updatePosition') {
        cmdName = 'updateposition';
        commandValue = 'na';
    }
    else if (commandName == 'getBoxStatus') {
        cmdName = 'getboxstatus';
        commandValue = 'na';
    }
    else if (commandName == 'reeferChangeOperatingMode') {
        cmdName = 'reeferChangeOperatingMode';
        commandValue = $('#reeferOperationModeType').val();
    }
    else if (commandName == 'reeferChangeSetPoint') {
        setPoint = $('#reeferSetPoint').val();
        if (!$.isNumeric(setPoint)) {
            $('#reeferSetPoint').addClass('textboxError');
            $('#reeferSetPoint').focus();
            validation = false;
        }

        if (!validation)
            return;

        cmdName = 'reeferChangeSetPoint';
        commandValue = setPoint;
    }
    else if (commandName == 'reeferClearAlarm') {
        cmdName = 'reeferClearAlarm';
        commandValue = 'na';
    }
    else if (commandName == 'reeferOpenFreshAirDoor') {
        cmdName = 'reeferOpenFreshAirDoor';
        commandValue = 'na';
    }
    else if (commandName == 'reeferCloseFreshAirDoor') {
        cmdName = 'reeferCloseFreshAirDoor';
        commandValue = 'na';
    }
    else if (commandName == 'reeferInitiatePreTrip') {
        cmdName = 'reeferInitiatePreTrip';
        commandValue = 'na';
    }
    else if (commandName == 'reeferInitiateDefrost') {
        cmdName = 'reeferInitiateDefrost';
        commandValue = 'na';
    }
    else if (commandName == 'reeferMultipleAlarmRead') {
        cmdName = 'reeferMultipleAlarmRead';
        commandValue = 'na';
    }
    else if (commandName == 'reeferSoftwareIdentification') {
        cmdName = 'reeferSoftwareIdentification';
        commandValue = 'na';
    }
    else if (commandName == 'reeferTurnPowerOff') {
        cmdName = 'reeferTurnPowerOff';
        commandValue = 'na';
    }
    else if (commandName == 'reeferTurnPowerOn') {
        cmdName = 'reeferTurnPowerOn';
        commandValue = 'na';
    }
    else if (commandName == 'SetFuelConfiguration') {

        var FuelType = $('#selSetFuelConfigurationFuelType').val();
        var SetFuelConfigurationEngineDisplacement = $('#SetFuelConfigurationEngineDisplacement').val();
        var SetFuelConfigurationVolumeEfficency = $('#SetFuelConfigurationVolumeEfficency').val();
        var SetFuelConfigurationAirFuelRatiox10 = $('#SetFuelConfigurationAirFuelRatiox10').val();
        var SetFuelConfigurationDenominator = $('#SetFuelConfigurationDenominator').val();

        if (FuelType == 4) {
            if (!$.isNumeric(SetFuelConfigurationDenominator)) {
                $('#SetFuelConfigurationDenominator').addClass('textboxError');
                $('#SetFuelConfigurationDenominator').focus();
                validation = false;
            }
        }
        else {
            if (!$.isNumeric(SetFuelConfigurationEngineDisplacement)) {
                $('#SetFuelConfigurationEngineDisplacement').addClass('textboxError');
                $('#SetFuelConfigurationEngineDisplacement').focus();
                validation = false;
            }

            if (!$.isNumeric(SetFuelConfigurationVolumeEfficency) || SetFuelConfigurationVolumeEfficency < 0 || SetFuelConfigurationVolumeEfficency > 100) {
                $('#SetFuelConfigurationVolumeEfficency').addClass('textboxError');
                $('#SetFuelConfigurationVolumeEfficency').focus();
                validation = false;
            }

            if (!$.isNumeric(SetFuelConfigurationAirFuelRatiox10)) {
                $('#SetFuelConfigurationAirFuelRatiox10').addClass('textboxError');
                $('#SetFuelConfigurationAirFuelRatiox10').focus();
                validation = false;
            }
        }

        if (!validation)
            return;

        if (SetFuelConfigurationEngineDisplacement.trim() == '') SetFuelConfigurationEngineDisplacement = 0;
        if (SetFuelConfigurationVolumeEfficency.trim() == '') SetFuelConfigurationVolumeEfficency = 0;
        if (SetFuelConfigurationAirFuelRatiox10.trim() == '') SetFuelConfigurationAirFuelRatiox10 = 0;
        if (SetFuelConfigurationDenominator.trim() == '') SetFuelConfigurationDenominator = 0;

        FollowingVehicleEngineDisplacement = SetFuelConfigurationEngineDisplacement;
        if (FuelType == '0' || FuelType == '1' || FuelType == '2')
            FollowingVehicleFuelType = 'Gas';
        else if (FuelType == '4')
            FollowingVehicleFuelType = 'Diesel';
        else
            FollowingVehicleFuelType = '';

        commandValue = FuelType + ';' + SetFuelConfigurationEngineDisplacement + ';' + SetFuelConfigurationVolumeEfficency + ';' + SetFuelConfigurationAirFuelRatiox10 + ';' + SetFuelConfigurationDenominator;
        cmdName = 'setfuelconfiguration';
    }
    else if (commandName == 'setSeatbeltOdometer') {
        commandValue = encodeURIComponent($("#selVehiceInfoMake option:selected").text()) + ";" + encodeURIComponent($("#selVehiceInfoModel option:selected").text()) + ";" + encodeURIComponent($('#vehicleInfoYear').val());
        cmdName = 'setSeatbeltOdometer';
    }

    var url = rootPath + 'Home/_sendCommand/' + ConfigBoxId + ',' + cmdName + ',' + commandValue + ',' + ConfigVehicleId;

    //$('#configOdometer').hide();
    $('.configButton').addClass('ui-disabled');
    $('#' + commandName + 'Waiting').show();
    $.getJSON(url, function (data) {

        try {
            if (data.success == 0 && data.Msg == "sessiontimeout") {
                location.replace(rootPath + 'Account/Login');
                return;
            }
            if (data.Status == 200) {
                setTimeout(function () {
                    checkCommandStatus(data.BoxId, data.ProtocolTypeId, data.CommModeId, commandName, data.VehicleId);
                }, 1000);

            }
            else if (data.Status == 500) {
                $('#' + commandName + 'Msg').css('color', 'red').html(data.Msg);
                //$('#configOdometer').show();
                $('.configButton').removeClass('ui-disabled');
                $('#' + commandName + 'Waiting').hide();
            }


        }
        catch (e) {
        }
    });
}

function sendSetSeatbeltOdometerFollowingCommands(commandName) {
    var commandValue = ""
    var cmdName = "";
    if (commandName == 'setSeatbeltOdometer') {
        commandValue = "na";
        cmdName = 'setSeatbeltOdometerNext1';
    }
    else if (commandName == 'setSeatbeltOdometerNext1') {
        commandValue = "na";
        cmdName = 'setSeatbeltOdometerNext2';
    }

    var url = rootPath + 'Home/_sendCommand/' + ConfigBoxId + ',' + cmdName + ',' + commandValue + ',' + ConfigVehicleId;

    //$('#configOdometer').hide();
    $('.configButton').addClass('ui-disabled');
    //$('#' + commandName + 'Waiting').show();
    $.getJSON(url, function (data) {

        try {
            if (data.success == 0 && data.Msg == "sessiontimeout") {
                location.replace(rootPath + 'Account/Login');
                return;
            }
            if (data.Status == 200) {
                setTimeout(function () {
                    checkCommandStatus(data.BoxId, data.ProtocolTypeId, data.CommModeId, cmdName, data.VehicleId);
                }, 1000);

            }
            else if (data.Status == 500) {
                $('#' + commandName + 'Msg').css('color', 'red').html(data.Msg);
                //$('#configOdometer').show();
                $('.configButton').removeClass('ui-disabled');
                $('#' + commandName + 'Waiting').hide();
            }


        }
        catch (e) {
        }
    });
}


function configVehicleName() {
    newDescription = $('#configVehicleName').val();
    if (newDescription == '') {
        $('#configVehicleName').addClass('textboxError');
        $('#configVehicleName').focus();
        return;
    }

    var url = rootPath + 'Home/_updateVehicleDescription/' + followingFeature.attributes.VehicleId + ',' + encodeURIComponent(newDescription);

    $('#vehicleNameSettingWaiting').show();
    $('#configVehicleNameBtn').addClass('ui-disabled');
    $('#vehicleNameSettingMsg').html('');
    $.getJSON(url, function (data) {
        //alert(url + ', ' + data.Status);
        try {
            $('#configVehicleNameBtn').removeClass('ui-disabled');
            $('#vehicleNameSettingWaiting').hide();

            if (data.result > 0) {
                $('#vehicleNameSettingMsg').css('color', 'green').html(data.Msg);

                var oldDescription = followingFeature.attributes.Description;
                $('#vehicleList #listviewwrapper ul li#livehiclelist_' + followingFeature.attributes.VehicleId + ' h2').html(newDescription + ' ');
                var s1 = "', Description: '" + oldDescription + "',";
                var s2 = "', Description: '" + newDescription + "',";
                var h = $('#vehicleList #listviewwrapper ul li#livehiclelist_' + followingFeature.attributes.VehicleId).html();
                h = h.replace(new RegExp(s1, 'g'), s2);
                $('#vehicleList #listviewwrapper ul li#livehiclelist_' + followingFeature.attributes.VehicleId).html(h);

                followingFeature.attributes.Description = newDescription;
                $('#configPageTitle').html(newDescription);
                $('#followPageTitle').html(ResourceFollow + ': ' + newDescription);
                //emailsubject = newDescription;
            }
            else {
                $('#vehicleNameSettingMsg').css('color', 'red').html(data.Msg);
            }
        }
        catch (e) {
        }
    });
}

function checkCommandStatus(boxId, ProtocolTypeId, CommModeId, commandName, vehicleId) {
    var url = rootPath + 'Home/_checkCommandStatus/' + boxId + ',' + ProtocolTypeId + ',' + CommModeId + ',' + vehicleId;

    $.getJSON(url, function (data) {
        //alert(url + ', ' + data.Status);
        try {
            if (data.Status == 200) {
                if (data.Waiting == 1) {
                    setTimeout(function () {
                        checkCommandStatus(boxId, ProtocolTypeId, CommModeId, commandName, vehicleId);
                    }, 1000);
                }
                else {
                    if (commandName == 'getBoxStatus') {
                        $('#BoxStatusInfo').show();
                        $('#BoxStatusInfoContent').html(data.boxStatus);
                    }
                    else if (commandName == 'updatePosition') {
                        $('#' + commandName + 'Msg').css('color', 'green').html(data.Msg);
                        $('#BoxLastPositionInfo').show();
                        $('#BoxLastPositionContent').html(data.boxPosition);
                    }
                    else if (commandName == 'setSeatbeltOdometer') {
                        $('#setSeatbeltOdometerMsg').css('color', 'green').html(data.Msg + " " + ResourceSendingSecondECMSeatbeltOdometer);
                        sendSetSeatbeltOdometerFollowingCommands(commandName);
                    }
                    else if (commandName == 'setSeatbeltOdometerNext1') {
                        $('#setSeatbeltOdometerMsg').css('color', 'green').html(data.Msg + " " + ResourceSendingThirdECMSeatbeltOdometer);
                        sendSetSeatbeltOdometerFollowingCommands(commandName);
                    }
                    else if (commandName == 'setSeatbeltOdometerNext2') {
                        $('#setSeatbeltOdometerMsg').html(data.Msg);
                    }
                    else {
                        $('#' + commandName + 'Msg').css('color', 'green').html(data.Msg);
                    }
                    $('.configButton').removeClass('ui-disabled');
                    $('#' + commandName + 'Waiting').hide();

                    if (commandName == 'SetFuelConfiguration') {
                        setFuelConfigurationForEmail = ResourceFuelType + ": " + $("#selSetFuelConfigurationFuelType option:selected").text() + "\n" +
                                ResourceEngineDisplacement + ": " + $('#SetFuelConfigurationEngineDisplacement').val() + "\n";
                        //ResourceVolumeEfficency + ": " + $('#SetFuelConfigurationVolumeEfficency').val() + "\n" +
                        //ResourceAirFuelRatiox10 + ": " + $('#SetFuelConfigurationAirFuelRatiox10').val() + "\n";
                    }
                }

            }
            else if (data.Status == 500) {
                $('#' + commandName + 'Msg').css('color', 'red').html(data.Msg);

                if (commandName == 'setSeatbeltOdometerNext1') {
                    $('#setSeatbeltOdometerMsg').css('color', 'red').html(data.Msg);
                }
                else if (commandName == 'setSeatbeltOdometerNext2') {
                    $('#setSeatbeltOdometerMsg').css('color', 'red').html(data.Msg);
                }

                $('.configButton').removeClass('ui-disabled');
                $('#' + commandName + 'Waiting').hide();
            }


        }
        catch (e) {
        }
    });
}

function clearConfig() {
    $('#configOdometerMsg').html('');
    $('#configOdometer').val('');
    $('#configVehicleName').val('');

    $('#configContent .configPageMsg').html('');

    $('#BoxStatusInfoContent').html('');
    $('#BoxStatusInfo').hide();
}

function getValueByKey(key, s) {
    s = s.toLowerCase();
    key = key.toLowerCase() + '=';

    var n = s.indexOf(key);
    var m;

    if (n < 0)
        return '';

    if (n == 0) {
        m = s.indexOf(";");
        return s.substring(n + key.length, m);
    }

    n = s.indexOf(";" + key);
    m = s.indexOf(";", n + 1);
    if (n == -1)
        return '';
    else
        return s.substring(n + key.length + 1, m);

}

function mailFollowPage() {
    //$("#emailfollowpage").attr('href', "mailto:" + emailrecipients + "?subject=" + encodeURIComponent(emailsubject) + "&body=" + emailcontent);
    var emailHeader = encodeURIComponent("Customer: " + OrganizationName + "\n") +
                      encodeURIComponent("Vehicle ID: " + FollowingVehicleName + "\n") +
                      encodeURIComponent("Engine Displacement: \n") +
                      //encodeURIComponent("Bluetooth ID: \n") +
                      encodeURIComponent("Bus Type: ODBII or JBus/6pin/9pin\n") +
                      encodeURIComponent("Box location: \n") +
                      encodeURIComponent("GPS Antenna Location: \n") +
                      encodeURIComponent("Cell Antenna Location: \n") +
                      encodeURIComponent("Power Source: \n") +
                      encodeURIComponent("Ignition Source: \n") +
                      //encodeURIComponent("Power Info: \n") +
                      encodeURIComponent("Other comments: \n\n");

    if (vehicleInfoForEmail != '') {
        emailHeader += vehicleInfoForEmail + encodeURIComponent("\n");
    }

    if (setFuelConfigurationForEmail != '') {
        emailHeader += setFuelConfigurationForEmail + encodeURIComponent("\n");
    }

    var s = "mailto:" + emailrecipients + "?subject=" + encodeURIComponent(emailsubject) + "&body=" + emailHeader + emailcontent;

    document.location.href = s;
}

function searchHistory() {
    //TODO: validate datetime format.

    if (LoadingHistoryData || HistoryVehicleId <= 0)
        return;

    LoadingHistoryData = true;

    var startDatetime = $('#txtFromDate').val() + " " + $('#txtFromTime').val();
    var toDatetime = $('#txtToDate').val() + " " + $('#txtToTime').val();

    var url = rootPath + 'Home/SearchVehicleHistory/' + HistoryVehicleId + '?startDateTime=' + encodeURIComponent(startDatetime) + '&toDateTime=' + encodeURIComponent(toDatetime);
    //var url = rootPath + 'Home/GetVehicleLastKnown/31522?startDateTime=' + encodeURIComponent('05/14/2013 10:11:49 AM');

    /*$("ul#history_results").html('');
    allFollows = [];
    followLayer.removeAllFeatures();*/
    clearHistory();

    $.mobile.showPageLoadingMsg();

    $.getJSON(url, function (data) {
        $('#historyCriteria').popup('close');
        LoadingHistoryData = false;

        try {
            if (data.success == 0 && data.Msg == "sessiontimeout") {
                $.mobile.hidePageLoadingMsg();
                location.replace(rootPath + 'Account/Login');
                return;
            }

            if (data.success == 1 && data.hasData == 1) {

                for (i = data.data.length - 1; i >= 0; i--) {
                    var odometer = '';
                    var fuel = '';
                    var rpm = '';
                    var customprop = '';

                    odometer = getValueByKey('odometer', data.data[i].CustomProp);
                    rpm = getValueByKey('rpm', data.data[i].CustomProp);
                    fuel = getValueByKey('fuel', data.data[i].CustomProp);

                    if (odometer != '')
                        customprop += ResourceOdometer + '=' + odometer + ';';
                    if (rpm != '')
                        customprop += ResourceRPM + '=' + rpm + '; ';
                    if (fuel != '')
                        customprop += ResourceFuel + '=' + fuel + ';';

                    var validgps = data.data[i].ValidGps;
                    if (validgps == 0)
                        customprop += ResourceGpsValid + "; ";
                    else if (validgps != -1)
                        customprop += ResourceGpsInvalid + "; ";

                    if (data.data[i].MsgDetails != '')
                        customprop = '; ' + customprop;

                    var li = "<li><h2>" + data.data[i].displayDateTime + "</h2>" +
                        "<p>" + ResourceSpeed + ": " + data.data[i].Speed + "</p>" +
                        "<p>" + ResourceMessage + ": " + data.data[i].MsgDetails + " " + customprop + "</p>" +
                        "<p>" + data.data[i].Address + "</p>";
                    if (data.data[i].reeferData == "1") {
                        li += "<p>Tether: " + data.data[i].Tether + "; Reefer Power: " + data.data[i].Power;
                        if (data.data[i].Tether != "Off" && data.data[i].Power != "Off") {
                            li += "<p style='white-space: normal;'>Amb:" + data.data[i].Amb +
                                "; Battery(v):" + data.data[i].BatteryVolt +
                                "; Setpt.:" + data.data[i].Setpt +
                                "; Ret.:" + data.data[i].Ret +
                                "; Reefer&nbsp;State:" + data.data[i].reeferState +
                                "; Mode&nbsp;Of&nbsp;Op.:" + data.data[i].ModeOfOp +
                                "; Setpt2.:" + data.data[i].Setpt2 +
                                "; Ret2.:" + data.data[i].Ret2 +
                                "; Reefer&nbsp;State2:" + data.data[i].reeferState2 +
                                "; Mode&nbsp;Of&nbsp;Op2.:&nbsp;" + data.data[i].ModeOfOp2 +
                                "; Setpt3.:&nbsp;" + data.data[i].Setpt3 +
                                "; Ret3.:&nbsp;" + data.data[i].Ret3 +
                                "; Reefer&nbsp;State3:&nbsp;" + data.data[i].reeferState3 +
                                "; Mode&nbsp;Of&nbsp;Op3.:&nbsp;" + data.data[i].ModeOfOp3 +
                                "</p>";
                        }
                        li += "</p>";
                    }

                    li += "</li>";

                    /*var emailli = encodeURIComponent(data.data[i].displayDateTime + "\n") +
                            encodeURIComponent(ResourceSpeed + ": " + data.data[i].Speed + "\n") +
                            encodeURIComponent(ResourceMessage + ": " + data.data[i].MsgDetails + " " + customprop + "\n") +
                            encodeURIComponent(data.data[i].Address + "\n\n");
                     */

                    $("ul#history_results").prepend(li);

                    //if ($('ul#history_results').children().length > FollowMaxRecords) {
                    //    $('ul#history_results').children().slice(FollowMaxRecords, $('ul#follow_results').children().length).remove();
                    //}



                    var _v = {
                        BoxId: historyFeature.attributes.BoxId,
                        VehicleId: historyFeature.attributes.VehicleId,
                        Description: historyFeature.attributes.Description,
                        OriginDateTime: data.data[i].displayDateTime,
                        DisplayDateTime: data.data[i].displayDateTime,
                        Speed: data.data[i].Speed,
                        Status: data.data[i].Status,
                        Location: data.data[i].Address,
                        //Description: '', 
                        MyHeading: data.data[i].MyHeading,
                        MyHeadingIcon: data.data[i].MyHeadingIcon,
                        lon: data.data[i].Lon,
                        lat: data.data[i].Lat,
                        //icon: data.data[i].icon
                        icon: "/mobile/content/images/" + data.data[i].icon,
                        MsgDetails: data.data[i].MsgDetails
                    };

                    var newLoc;


                    newLoc = transformCoords(_v.lon, _v.lat);
                    var hpoint = new OpenLayers.Geometry.Point(newLoc.lon, newLoc.lat);
                    var hmarker = new OpenLayers.Feature.Vector(hpoint);
                    var vspeed = _v.Speed.split(" ")[0];
                    if ($.isNumeric(vspeed) && vspeed > 0) {
                        _v.icon = "/mobile/content/images/followBlue" + _v.MyHeadingIcon + ".png";
                    }
                    else {
                        _v.icon = "/mobile/content/images/followBrown.png"
                    }
                    hmarker.attributes = _v;
                    followLayer.addFeatures([hmarker]);

                    allFollows.push(hmarker);

                    //emailsubject = followingFeature.attributes.Description;
                    //emailcontent = emailli + emailcontent;

                }
                //alert("finished");

                $("ul#history_results").listview("refresh");

            }
            $.mobile.hidePageLoadingMsg();

        }
        catch (e) {
            //alert(e);
            $.mobile.hidePageLoadingMsg();

        }

    });
}

function clearHistory() {
    followLayer.removeAllFeatures();
    allFollows = [];
    $('ul#history_results').empty();
}

function selSetFuelConfigurationFuelTypeChange() {
    var FuelType = $('#selSetFuelConfigurationFuelType').val();
    if (FuelType == 4) {
        $('#SetFuelConfigurationDiv .fuelTypeDiesel').show();
        $('#SetFuelConfigurationDiv .fuelTypeNotDiesel').hide();
    }
    else {
        $('#SetFuelConfigurationDiv .fuelTypeDiesel').hide();
        $('#SetFuelConfigurationDiv .fuelTypeNotDiesel').show();
    }
}

function changeVehicleListTitle(o) {
    $('#vehicleList #vehicleListHeader h2').html($(o).find('h2').html());
}

function AttributesToString(o) {

    var attrstring = '';
    for (var prop in o) {
        attrstring += prop + ": '" + o[prop] + "',";
    }
    if (attrstring.length > 0)
        attrstring = attrstring.substring(0, attrstring.length - 1);

    return "{" + attrstring + "}";
}

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

$(".input_capital").live('keyup', function (e) {
    $(this).val($(this).val().toUpperCase());
});

$(".alphanumeric").live('keypress', function (event) {
    var regex = new RegExp("^[a-zA-Z0-9]+$");
    var code = event.charCode === 0 ? event.which : event.charCode;
    var key = String.fromCharCode(code);
    if (!regex.test(key) && code !== 8 && code != 0) {
        // 8 is ascii code for backspace
        event.preventDefault();
        return false;
    }
    else
        return true;
});

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    return pattern.test(emailAddress);
};

var sensorObject;

function onSaveSensorClick() {

    if (sensorObject == null)
        return;

    if ((sensorObject.SensorId == null) || (sensorObject.SensorId == "") || (sensorObject.SensorId < 0))
        return;

    var sensorNameTextFieldValue = $('#sensorInfoDescription').val();
    sensorObject.SensorName = sensorNameTextFieldValue;
    $('#sensorInfoDescription').val(sensorObject.SensorName);

    saveSensorInfo(sensorObject);
}

function onToggleSensorNameText_Unsused_Click() {

    if (sensorObject == null)
        return;

    if ((sensorObject.SensorId == null) || (sensorObject.SensorId == "") || (sensorObject.SensorId < 0))
        return;

    var sensorOriginalName = $('#sensorInfoSensorOriginalName').val();
    var sensorNameTextFieldValue = $('#sensorInfoDescription').val();
    if ((sensorNameTextFieldValue.lastIndexOf(sensorOriginalName, 0)) == -1) {

        newSensorName = sensorOriginalName;
    }
    else {

        var keyText = 'Unused';
        var currentSensorName = sensorObject.SensorName;
        var newSensorName = currentSensorName;
        var nIndex = currentSensorName.lastIndexOf(keyText, 0);

        switch (nIndex) {
            case 0:
                {
                    newSensorName = currentSensorName.substring(keyText.length, currentSensorName.length);
                    break;
                }

            default:
                {
                    newSensorName = (keyText + ' ' + sensorOriginalName);
                }
        }
    }

    sensorObject.SensorName = newSensorName;
    $('#sensorInfoDescription').val(sensorObject.SensorName);
};

function onSensorClick(sensorObj) {

    if ((sensorObj.SensorId == null) || (sensorObj.SensorId == "") || (sensorObj.SensorId < 0)) {
        return;
    }

    sensorObject = sensorObj;

    $.mobile.showPageLoadingMsg();

    $.mobile.changePage('#editSensorPage');

    $('#editSensorPageTitle').html(sensorObj.SensorName);

    $('#sensorInfoSensorOriginalName').val(sensorObj.SensorName);
    $('#sensorInfoSensorId').val(sensorObj.SensorId);
    $('#sensorIdDescription').val(sensorObj.SensorId);
    $('#sensorInfoDescription').val(sensorObj.SensorName);
    $('#sensorInfoSensorAction').val(sensorObj.SensorAction);
    $('#sensorInfoSensorAlarmLevelOn').val(sensorObj.AlarmLevelOn);
    $('#sensorInfoSensorAlarmLevelOff').val(sensorObj.AlarmLevelOff);

    $.mobile.hidePageLoadingMsg();

}
var UpdatedSelectedSensorlist = [];
var UpdatedUnSelectedSensorlist = [];
var UpdatedSelectedSensorNameList = [];
var UpdatedUnSelectedSensorNameList = [];
function rename(checkbox, id, SensorId, sensorNameControl) {
    var SensorName = sensorNameControl.innerText.replace(SensorId + '', '');
    SensorName = SensorName.replace('"', '');
    if (checkbox.className.indexOf("checkBoxLeftNotChecked") > -1 && checkbox.className.indexOf("disabled='true'") == -1) {
        SensorName = SensorName.trim();
        sensorNameControl.innerText = sensorNameControl.innerText.replace('Unused ', '');
        if (sensorNameControl.innerText.indexOf('unused') > -1 || sensorNameControl.innerText.indexOf('Unused') > -1) {
            sensorNameControl.innerText = sensorNameControl.innerText.replace('Unused', '');
            sensorNameControl.innerText = sensorNameControl.innerText.replace('unused', '');
        }
        if (UpdatedSelectedSensorlist.indexOf(SensorId) == -1) {
            UpdatedSelectedSensorlist.push(SensorId);
            UpdatedSelectedSensorNameList.push(SensorName.replace('Unused ', ''));
        }
        if (UpdatedUnSelectedSensorlist.indexOf(SensorId) > -1) {
            UpdatedUnSelectedSensorlist.splice(UpdatedUnSelectedSensorlist.indexOf(SensorId), 1);
            UpdatedUnSelectedSensorNameList.splice(UpdatedUnSelectedSensorNameList.indexOf(SensorName), 1);
        }
        checkbox.className = "checkBoxLeft checkBoxLeftChecked";
        document.getElementById(id).checked = true;
    }
    else if (checkbox.className.indexOf("disabled='true'") == -1) {
        SensorName = SensorName.trim();
        sensorNameControl.innerText = SensorId + ' ' + 'Unused ' + SensorName;
        if (UpdatedSelectedSensorlist.indexOf(SensorId) > -1) {
            UpdatedSelectedSensorlist.splice(UpdatedSelectedSensorlist.indexOf(SensorId), 1);
            UpdatedSelectedSensorNameList.splice(UpdatedSelectedSensorNameList.indexOf(SensorName), 1);
        }
        if (UpdatedUnSelectedSensorlist.indexOf(SensorId) == -1) {
            UpdatedUnSelectedSensorlist.push(SensorId);
            UpdatedUnSelectedSensorNameList.push('Unused ' + SensorName);
        }
        checkbox.className = "checkBoxLeft checkBoxLeftNotChecked";
        document.getElementById(id).checked = false;
    }

}

function saveSensorInfo() {

    var vehicleDescription = selectedFeature.attributes.Description;
    var vehicleInfoBoxId = selectedFeature.attributes.BoxId;
    var vehicleInfoVehicleId = selectedFeature.attributes.VehicleId;
    var vehicleInfoLicensePlate = selectedFeature.attributes.LicensePlate;

    $.mobile.showPageLoadingMsg();

    var url = rootPath + 'Home/_saveSensorInfo/';

    $.ajax({
        url: url,
        data: { "selected": JSON.stringify(UpdatedSelectedSensorlist), "unselected": JSON.stringify(UpdatedUnSelectedSensorlist), "selectedName": JSON.stringify(UpdatedSelectedSensorNameList), "unselectedName": JSON.stringify(UpdatedUnSelectedSensorNameList), "LicensePlate": vehicleInfoLicensePlate },
        success: function (result) {

            $.mobile.hidePageLoadingMsg();

            if (result == null) {
                $('#sensorlistviewmsg').html('It has some problem loading the sensors. Please try again later.');
            }

            //  Success
            if (result == 0) {
                $.mobile.loading('show', { theme: "d", text: "Sensors saved successfully...", textonly: true, textVisible: true });
                setTimeout(function () {
                    $.mobile.loading('hide');
                    UpdatedSelectedSensorlist = [];
                    UpdatedUnSelectedSensorlist = [];
                    UpdatedSelectedSensorNameList = [];
                    UpdatedUnSelectedSensorNameList = [];
                    $.mobile.changePage('#vehicleInfoPage');
                }, 3000);
            }
            else {
                $('#editsensorviewmsg').html('It has some problem loading the sensors. Please try again later.');
            }
        },
        error: function (msg) {

            $('#editsensorviewmsg').html('It has some problem loading the sensors. Please try again later.');
            $.mobile.hidePageLoadingMsg();
        }
    });

}

function getBoxSensorInfo() {

    var vehicleInfoBoxId = selectedFeature.attributes.BoxId;
    var vehicleInfoVehicleId = selectedFeature.attributes.VehicleId;
    var vehicleInfoLicensePlate = selectedFeature.attributes.LicensePlate;
    UpdatedSelectedSensorlist = [];
    UpdatedUnSelectedSensorlist = [];
    var url = rootPath + 'Home/_getVehicleSensors/' + vehicleInfoBoxId + ',' + vehicleInfoLicensePlate;

    $.ajax({
        url: url,
        success: function (result) {

            if (result == null) {
                $('#sensorlistviewmsg').html('It has some problem loading the sensors. Please try again later.');

                $.mobile.hidePageLoadingMsg();
            }

            if (result.indexOf("<!DOCTYPE html>") != -1) {
                //location.replace(rootPath + 'Account/Login');
                return;
            }

            //$('#ulVehicleSensorsPage').html(result).listview('refresh'); //   Move From
            $('#vehicleSensorsPageTitle').html('Sensors for ' + selectedFeature.attributes.Description);
            $.mobile.changePage('#vehicleSensorList');
            $('#popup').popup('close');

            $.mobile.hidePageLoadingMsg();

            $('#ulVehicleSensorsPage').html(result).listview('refresh');    //  Moved Here
        },
        error: function (msg) {
            $('#sensorlistviewmsg').html('It has some problem loading the sensors. Please try again later.');

            $.mobile.hidePageLoadingMsg();
        }
    });
}