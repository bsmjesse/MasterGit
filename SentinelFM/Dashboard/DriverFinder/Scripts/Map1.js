
var map, marker, zoom, popup, vectorLayer, clickVectorLayer, controls, epsg4326, projectTo, formFeature, searchContent, clickedLat, clickedLon;
var features = new Array();
var searchFeatures = new Array();
OpenLayers.Control.Click = OpenLayers.Class(OpenLayers.Control, {
    defaultHandlerOptions: {
        'single': true,
        'double': false,
        'pixelTolerance': 0,
        'stopSingle': false,
        'stopDouble': false
    },

    initialize: function (options) {
        this.handlerOptions = OpenLayers.Util.extend(
                {}, this.defaultHandlerOptions
            );
        OpenLayers.Control.prototype.initialize.apply(
                this, arguments
            );
        this.handler = new OpenLayers.Handler.Click(
                this, {
                    'click': this.trigger
                }, this.handlerOptions
            );
    },

    trigger: function (e) {
        var lonlat = map.getLonLatFromPixel(e.xy).transform(projectTo, epsg4326);

        clickedLat = lonlat.lat;
        clickedLon = lonlat.lon;
       
        if (formFeature != null) {
            destroyPopup(formFeature);
            clickVectorLayer.destroyFeatures(formFeature);
        }
        //SetCenter(lonlat.lat, lonlat.lon, zoom, true);
        SetClickFeature(lonlat.lat, lonlat.lon, true);
        clickVectorLayer.addFeatures(formFeature);
        createPopup(formFeature);

    }

});


function init(xmlstr) {
    
    map = new OpenLayers.Map('map');


    
    var osm = new OpenLayers.Layer.OSM();

    
    map.addLayers([osm]);
    epsg4326 = new OpenLayers.Projection("EPSG:4326"); //WGS 1984 projection
    projectTo = map.getProjectionObject(); //The map projection (Spherical Mercator)
    zoom = 14;
    map.addControl(new OpenLayers.Control.LayerSwitcher());
    SetCenter(43.67746, -79.5850766666667, zoom, true);
    //map.setCenter(new OpenLayers.LonLat(-79.5850766666667, 43.67746).transform(epsg4326, projectTo), zoom);
    
    vectorLayer = new OpenLayers.Layer.Vector("Overlay");
    clickVectorLayer = new OpenLayers.Layer.Vector("ClickOverlay");
     controls = {     

        selector: new OpenLayers.Control.SelectFeature(vectorLayer,
	    {
	        clickout: true,
	        toggle: true,
	        hover: true,
	        callbacks: {
	            'over': createPopup,
	            'out' : destroyPopup
	        }
            
	    })               
   
        
        };		

    map.addControl(controls['selector']);    
    controls['selector'].activate();
       
    var click = new OpenLayers.Control.Click();
    map.addControl(click);
    click.activate();

    map.addLayers([clickVectorLayer, vectorLayer]);
    map.addControl(new OpenLayers.Control.LayerSwitcher());
}


$(document).ready(function () {

    $.ajax({
        url: 'SearchForm.aspx',
        method: 'GET',
        dataType: 'HTML',
        success: function (html) {
            searchContent = html;
        }
    });

    $.ajax({
        url: 'SearchForm.aspx?layout=h',
        method: 'GET',
        dataType: 'HTML',
        success: function (html) {
            $('#searchFormOnPage').html(html);
        }
    });

    $('#DriverInfoTable').dataTable({
        "aoColumns": [{ "sType": "natural" }, { "sType": "natural" }, { "sType": "natural" }, { "sType": "natural" }, { "sType": "natural" }, { "sType": "natural" }, { "sType": "natural" }, null],
        "aaSorting": [[1, "asc"]],
        "sScrollX": "100%",
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bInfo": false
    });
}
);


function RunSearch(from) {
    if (from == "page") {
        if($('#txtAddress').val() == '') {
            alert("Please fill up the address");
            return false;
        }

        $.ajax({
            url: 'GetAddressLatLon.aspx',
            method: 'GET',
            dataType: 'json',
            async: false,
            data: 'address=' + encodeURIComponent($('#txtAddress').val()),
            success: function (json) {
                clickedLat = json.lat;
                clickedLon = json.lon;                

                if (formFeature != null) {
                    destroyPopup(formFeature);
                    clickVectorLayer.destroyFeatures(formFeature);
                }
                
                SetClickFeature(clickedLat, clickedLon, false);
                clickVectorLayer.addFeatures(formFeature);
                createPopup(formFeature);

            }
        });
    }

    CleanTable();
    FindMyVehicles(clickedLat, clickedLon, from);
    SetCenter(clickedLat, clickedLon, zoom, true);
    destroyPopup(formFeature);    
}

function FindMyVehicles(lat, lon, from) {

    var skillId = 0, vehicleTypeId = -1;    
    if (from == "page") {
        skillId = $('#skillList_h').val();
        vehicleTypeId = $('#vehicleTypes_h').val();
    } else {
        skillId = $('#skillList').val();
        vehicleTypeId = $('#vehicleTypes').val();
    }
    $.ajax({
        url: 'GetSurroundedVehicles.aspx',
        method: 'GET',
        dataType: 'XML',
        data: 'lat=' + lat + '&lon=' + lon + '&skillId=' + skillId + '&vehicleTypeId=' + vehicleTypeId,
        success: function (data) {
            if (features.length > 0) {
                features = new Array();
            }

            if (searchFeatures.length > 0) {
                searchFeatures = new Array();
            }
            RemoveFeatures();
            $(data).find("VehiclesLastKnownPositionInformation").each(function (index, vehicleInfo) {


                if (!FeatureExists($(vehicleInfo).find("VehicleId").text())) {
                    searchFeatures[$(vehicleInfo).find("VehicleId").text()] = $(vehicleInfo);
                }
                else {
                    var tmpVehicleInfo = searchFeatures[$(vehicleInfo).find("VehicleId").text()];
                    var tmpSkillName = tmpVehicleInfo.find("SkillName").text();
                    var mySkillName = tmpSkillName + "," + $(vehicleInfo).find("SkillName").text();
                    tmpVehicleInfo.find("SkillName").text(mySkillName);
                    searchFeatures[$(vehicleInfo).find("VehicleId").text()] = tmpVehicleInfo;
                }
            });


            for (var vid in searchFeatures) {
                var myVehicleInfo = searchFeatures[vid];
                var feature = GetFeature(myVehicleInfo, myVehicleInfo.find("Latitude").text(), myVehicleInfo.find("Longitude").text());
                features.push(feature);
                AppendToTable(myVehicleInfo);
            }
            vectorLayer.addFeatures(features);
        }
    });
}

function GetFeature(vehicleInfo, lat, lon) {
    var vehicleDescr = GetDescription(vehicleInfo, 'all');
    var feature = new OpenLayers.Feature.Vector(
            new OpenLayers.Geometry.Point(lon, lat).transform(epsg4326, projectTo),
            { content: vehicleDescr, vehicleId: vehicleInfo.find("VehicleId").text() },
            { externalGraphic: (vehicleInfo != null ? 'Content/img/marker.png' : 'Content/img/marker-green.png'), graphicHeight: 25, graphicWidth: 21, graphicXOffset: -12, graphicYOffset: -25 }            
        );
    return feature;
}

function SetClickFeature(lat, lon, dropContent) {
    var content = 'lat:' + lat + '<br>lon:' + lon + '<br/>' + searchContent;
    if (!dropContent) {
        content = 'lat:' + lat + '<br>lon:' + lon;
    }
    formFeature = new OpenLayers.Feature.Vector(
            new OpenLayers.Geometry.Point(lon, lat).transform(epsg4326, projectTo),
            { content: content },
            { externalGraphic: 'Content/img/marker-green.png', graphicHeight: 25, graphicWidth: 21, graphicXOffset: -12, graphicYOffset: -25 }
        );
    
}

function createPopup(feature) {
    feature.popup = new OpenLayers.Popup.FramedCloud("pop",
          feature.geometry.getBounds().getCenterLonLat(),
          null,
          '<div class="markerContent">' + feature.attributes.content + '</div>',
          null,
          true,
          function () {
              // controls['selector'].unselectAll();
              destroyPopup(feature);
          }
      );    
    map.addPopup(feature.popup);
}

function destroyPopup(feature) {
    if (feature.popup == null)
        return false;
    feature.popup.destroy();
    feature.popup = null;
}

function RemoveFeatures() {

    var count = vectorLayer.features.length;
    for (var i = 0; i < count; i++)
        destroyPopup(vectorLayer.features[i]);
        vectorLayer.destroyFeatures(vectorLayer.features[i]);        
}

function AppendToTable(vehicleInfo) {
    $('#DriverInfoTable').dataTable().fnAddData([
					'<a href="javascript:PopFeature(\'' + vehicleInfo.find("VehicleId").text() + '\', \'all\')">' + vehicleInfo.find("VehicleId").text() + '</a>',
					'<a href="javascript:PopFeature(\'' + vehicleInfo.find("VehicleId").text() + '\', \'all\')">' + vehicleInfo.find("Description").text() + '</a>',
					'<a href="javascript:PopFeature(\'' + vehicleInfo.find("VehicleId").text() + '\', \'all\')">' + vehicleInfo.find("VehicleTypeName").text() + '</a>',
					'<a href="javascript:PopFeature(\'' + vehicleInfo.find("VehicleId").text() + '\', \'all\')">' + vehicleInfo.find("SkillName").text() + '</a>',
                    '<a href="javascript:PopFeature(\'' + vehicleInfo.find("VehicleId").text() + '\', \'all\')">' + vehicleInfo.find("FirstName").text() + " " + vehicleInfo.find("LastName").text() + '</a>',
           '<a href="javascript:PopFeature(\'' + vehicleInfo.find("VehicleId").text() + '\', \'all\')">' + vehicleInfo.find("StreetAddress").text() + '</a>',
            '<a href="javascript:PopFeature(\'' + vehicleInfo.find("VehicleId").text() + '\', \'all\')">' + vehicleInfo.find("GPSDistance").text() + '</a>',
            '<a href="#" onclick="OpenHistoryWindow(\'' + vehicleInfo.find("VehicleId").text()  + '\')">' + "History" + '</a>'
                ]);

}

function CleanTable() {
    $('#DriverInfoTable').dataTable().fnClearTable();
}

function SetCenter(lat, lon, myzoom, transfer) {
    var latlon;
    if (transfer) {
        latlon = new OpenLayers.LonLat(lon, lat).transform(epsg4326, projectTo);
    } else {
        latlon = new OpenLayers.LonLat(lon, lat);
    }
    map.setCenter(latlon, myzoom);
}

function FeatureExists(vehicleId) {
    return (searchFeatures[vehicleId] != undefined ? true : false);
}

function PopFeature(vehicleId, field) {   

    var count = vectorLayer.features.length;
    for (var i = 0; i < count; i++) {
        destroyPopup(vectorLayer.features[i]);
        var vId = vectorLayer.features[i].attributes.vehicleId;
        if (vehicleId == vId) {
            var vehicleInfo = searchFeatures[vehicleId];
            vectorLayer.features[i].attributes.content = GetDescription(vehicleInfo, field);
            createPopup(vectorLayer.features[i]);
            var myLatLonSquare = vectorLayer.features[i].geometry;
            var featureLon = Math.round(myLatLonSquare.getVertices()[0].x);
            var featureLat = Math.round(myLatLonSquare.getVertices()[0].y);
            SetCenter(featureLat, featureLon, zoom, false);
        }
    }
}

$('#txtAddress').live("keydown", function (e) {
    var code = (e.keyCode ? e.keyCode : e.which);
    if (code == 13) { //Enter keycode
        RunSearch("page");
        return false;
    }
});

function GetDescription(vehicleInfo, field) {
    var descr = null;
    var descrVehicleId = "Vehicle Id: " + vehicleInfo.find("VehicleId").text();
    var descrDescription = "Description: " + vehicleInfo.find("Description").text();
    var descrVehicleType = "Vehicle Type: " + vehicleInfo.find("VehicleTypeName").text();
    var descrSkillName = "Vehicle Type: " + vehicleInfo.find("SkillName").text();
    var descrDriverName = "Driver Name: " + vehicleInfo.find("FirstName").text() + " " + vehicleInfo.find("LastName").text();
    var descrStreetAddress = "Address: " + vehicleInfo.find("StreetAddress").text();
    var descrGPSDistance = "Distance: " + vehicleInfo.find("GPSDistance").text();
    
    switch (field) {
        case 'VehicleId':
            descr = descrVehicleId;
            break;
        case 'Description':
            descr = descrDescription;
            break;
        case 'VehicleTypeName':
            descr = descrVehicleType;
            break;
        case 'SkillName':
            descr = descrSkillName;
            break;
        case 'DriverName':
            descr = descrDriverName;
            break;
        case 'StreetAddress':
            descr = descrStreetAddress;
            break;
        case 'GPSDistance':
            descr = descrGPSDistance;
            break;
        default:
            descr = descrVehicleId + "<br/>" + descrDescription + "<br/>" + descrVehicleType + "<br/>" + descrSkillName + "<br/>" + descrDriverName + "<br/>" + descrStreetAddress + "<br/>" + descrGPSDistance;
    }

    return descr;

}


function OpenHistoryWindow(vehicleId) {
    window.open('/History/frmhistmain_new.aspx?VehicleId=' + vehicleId);
}




jQuery.fn.dataTableExt.oSort['natural-asc'] = function (a, b) {
    var processedA = a.replace(/<(?:.|\n)*?>/gm, '');
    var processedB = b.replace(/<(?:.|\n)*?>/gm, '');
    
    return naturalSort(processedA, processedB);
};

jQuery.fn.dataTableExt.oSort['natural-desc'] = function (a, b) {
    var processedA = a.replace(/<(?:.|\n)*?>/gm, '');
    var processedB = b.replace(/<(?:.|\n)*?>/gm, '');

    return naturalSort(processedA, processedB) * -1;
};
jQuery.fn.dataTableExt.aTypes.unshift(  
    function ( sData )  
    {  
        var deformatted = sData.replace(/[^\d\-\.\/a-zA-Z]/g,'');
        if ( $.isNumeric( deformatted ) ) {
            return 'formatted-num';
        }
        return null;  
    }  
);
jQuery.fn.dataTableExt.oSort['formatted-num-asc'] = function(a,b) {
    /* Remove any formatting */
    var x = a.match(/\d/) ? a.replace( /[^\d\-\.]/g, "" ) : 0;
    var y = b.match(/\d/) ? b.replace( /[^\d\-\.]/g, "" ) : 0;
      
    /* Parse and return */
    return parseFloat(x) - parseFloat(y);
};
  
jQuery.fn.dataTableExt.oSort['formatted-num-desc'] = function(a,b) {
    var x = a.match(/\d/) ? a.replace( /[^\d\-\.]/g, "" ) : 0;
    var y = b.match(/\d/) ? b.replace( /[^\d\-\.]/g, "" ) : 0;
      
    return parseFloat(y) - parseFloat(x);
};

// Natural sort function
function naturalSort (a, b) {
    var re = /(^-?[0-9]+(\.?[0-9]*)[df]?e?[0-9]?$|^0x[0-9a-f]+$|[0-9]+)/gi ,
        sre = /(^[ ]*|[ ]*$)/g ,
        dre = /(^([\w ]+,?[\w ]+)?[\w ]+,?[\w ]+\d+:\d+(:\d+)?[\w ]?|^\d{1,4}[\/\-]\d{1,4}[\/\-]\d{1,4}|^\w+, \w+ \d+, \d{4})/ ,
        hre = /^0x[0-9a-f]+$/i ,
        ore = /^0/ ,
        i = function(s) { return naturalSort.insensitive && ('' + s).toLowerCase() || '' + s },
        // convert all to strings strip whitespace
        x = i(a).replace(sre, '') || '',
        y = i(b).replace(sre, '') || '',
        // chunk/tokenize
        xN = x.replace(re, '\0$1\0').replace( /\0$/ , '').replace( /^\0/ , '').split('\0'),
        yN = y.replace(re, '\0$1\0').replace( /\0$/ , '').replace( /^\0/ , '').split('\0'),
        // numeric, hex or date detection
        xD = parseInt(x.match(hre)) || (xN.length != 1 && x.match(dre) && Date.parse(x)),
        yD = parseInt(y.match(hre)) || xD && y.match(dre) && Date.parse(y) || null,
        oFxNcL, oFyNcL;
    // first try and sort Hex codes or Dates
    if (yD)
        if (xD < yD) return -1;
        else if (xD > yD) return 1;
    // natural sorting through split numeric strings and default strings
    for (var cLoc = 0, numS = Math.max(xN.length, yN.length); cLoc < numS; cLoc++) {
        // find floats not starting with '0', string or 0 if not defined (Clint Priest)
        oFxNcL = !(xN[cLoc] || '').match(ore) && parseFloat(xN[cLoc]) || xN[cLoc] || 0;
        oFyNcL = !(yN[cLoc] || '').match(ore) && parseFloat(yN[cLoc]) || yN[cLoc] || 0;
        // handle numeric vs string comparison - number < string - (Kyle Adams)
        if (isNaN(oFxNcL) !== isNaN(oFyNcL)) {
            return (isNaN(oFxNcL)) ? 1 : -1;
        }
            // rely on string comparison if different types - i.e. '02' < 2 != '02' < '2'
        else if (typeof oFxNcL !== typeof oFyNcL) {
            oFxNcL += '';
            oFyNcL += '';
        }
        if (oFxNcL < oFyNcL) return -1;
        if (oFxNcL > oFyNcL) return 1;
    }
    return 0;
}