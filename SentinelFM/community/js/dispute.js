var marker = null;
$(document).ready(function () {
    var latlons = vLatlon.replace(" ", "").split(",");
    var popup = L.popup({
        maxWidth: 320
    });
    popup.setContent(GetForm());
    var map = L.map('map').setView([parseFloat(latlons[0]), parseFloat(latlons[1])], 13);    
    
    /***
    L.tileLayer('http://{s}.tile.cloudmade.com/BC9A493B41014CAABB98F0471D759707/997/256/{z}/{x}/{y}.png', {
        attribution: 'Map data &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, <a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="http://cloudmade.com">CloudMade</a>',
        maxZoom: 18
    }).addTo(map);
    ***/
    var googleLayer = new L.Google('ROADMAP');
    map.addLayer(googleLayer);
    marker = L.marker([parseFloat(latlons[0]), parseFloat(latlons[1])]).addTo(map);      
    marker.bindPopup(popup).openPopup();
});


function GetForm(latlon, speed) {
    var selectedMetric = '<select id="metric" name="metric" style=\"width: 80px\"><option value="1" selected>km/hr</option></select></td>';
    if (errorSpeed.indexOf("mph") > -1) {
        selectedMetric = '<select id="metric" name="metric" style=\"width: 80px\"><option value="2" selected>mph</option></select></td>';
    }
    var formStr = '<div>' +
              '<div class=\"span3\">Speed Dispute Ticket: <strong>' + violationId + '</strong></div>' +
              '<input type="hidden" id="infractionid" name="infractionid" value="' + violationId + '" />' +
              '<input type="hidden" id="q" name="q" value="' + vLatlon + '" />' +
              '<input type="hidden" id="errorspeed" name="errorspeed" value="' + errorSpeed + '" />' +
              '<table width="100%">' +
              '<tr>' +
              '<td><label><strong>Address: </strong></label><br/>' + address + '</td>' +
              '</tr>' +
              '<tr>' + 
              '<td><label><strong>Actual Vehicle Speed:  </strong></label><br/>' + yourspeed + '</td>' +
              '</tr>' +
              '<tr>' +
              '<td ><label><strong>Posted Speed Limit in Notification Email:  </strong></label><br/>' + errorSpeed + '</td>' +
              '</tr>' +
              
              '<tr>' +
              '<td style=\"width: 120px\"><label for="actualspeed"><strong>Actual Speed Limit Posted on Road:</strong></label><br/>' +
              '<input type="text" placeholder="Required" id="actualspeed" name="actualspeed" style=\"width: 100px;\" />' +
              selectedMetric +
              '</tr>' +
              '<tr>' +
              '<td><label for="name"><strong>Name:</strong> </label><br/>' +
              '<input type="text" placeholder="Required" id="name" name="name" style=\"width: 100px;\" /></td>' +
              '</tr>' +
              '<tr>' +
              '<td><label for="email"><strong>Email:</strong> </label><br/>' +
              '<input type="text" placeholder="Optional" id="email" name="email" style=\"width: 250px;\" /></td>' +
              '</tr>' +
              '<tr>' +              
              '<td><label for="notes"><strong>Note:</strong></label><br/>' +
              '<textarea placeholder="Optional" id="notes" name="notes"></textarea><br>(Content cannot be over 100 characters)</td>' +
              '</tr>' +
              '<tr>' + 
              '<td id=\"captchaContainer\" colspan = \"2\" align=\"center\">' + ($('#divCaptcha').html() != undefined ? $('#divCaptcha').html() : '') + '</td>' +
              '</tr>' +
              '<tr>' +
              '<td colspan = \"2\" align=\"center\">' +
              
                '<button type="button" class="btn" onclick="CancelSubmit()">Cancel</button>'+
                '<button type="button" class="btn btn-primary" onclick="SubmitForm()">Submit</button>' +
                '</td>' +
                '</tr>' +
                '</table>' +                       
               '</div>';      
    $('#divCaptcha').remove();
    return formStr;
}

function SubmitForm() {
    if (ValidateForm()) {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "dispute.aspx/SubmitTicket",
            data: "{'infractionid':'" + violationId + "', 'q':'" + vLatlon + "', 'actualspeed':'" + $('#actualspeed').val() + "', 'notes':'" + $('#notes').val() + "', 'txtCaptcha':'" + $('#txtCaptcha').val() + "', 'name':'" + $('#name').val() + "', 'metric':'" + $('#metric').val() + "', 'errorspeed':'" + $('#errorspeed').val() + "', 'yourspeed':'" + yourspeed + "', 'objects':'" + myobjects + "', 'objectid':'" + objectId + "', 'email':'" + $('#email').val() + "'}",
            dataType: "json",
            error: function (xhr, status, error) {
                alert(xhr.responseText);

            },
            success: function (json) {
                var data = $.parseJSON(json.d);
                if (data.status == "success") {                    
                    alert("You have saved the dispute.");
                    marker.closePopup();

                }
                else if (data.status == "reload") {
                    window.location.reload();
                }
                else {
                    alert(data.reason);
                }

            }
        });
    }    
}

function ValidateForm() {
    if ($('#name').val() == "" || $('#name').val() == null) {
        alert("Please input name");
        return false;
    }

    if ($('#email').val() != "" && $('#email').val() != null) {
        if (!IsValidateEmail($('#email').val())) {
            alert("Email format is not correct");
            return false;
        }
    }
    
    if ($('#actualspeed').val() == "" || $('#actualspeed').val() == null) {
        alert("Please input actual speed");
        return false;
    }

    if (isNaN($('#actualspeed').val())) {
        alert("Please input only numberic in speed field");
        return false;
    }

    if ($('#notes').val().length > 100) {
        alert("The notes length cannot be larger than 100 characters");
        return false;
    }
    
    if ($('#divCaptcha').html() != undefined) {
        if ($('#txtCaptcha').val() == "" || $('#txtCaptcha').val() == null) {
            alert("Please input captcha");
            return false;
        }
    }
    
    return true;
}

function IsValidateEmail(email) {
    var filter = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
    return filter.test(email);
}

function CancelSubmit() {
    marker.closePopup();
}

function ChangeCaptcha(val) {
    $('#changeCaptcha').val(1);
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "dispute.aspx/GenerateCaptcha",
        dataType: "json",
        error: function (xhr, status, error) {
            alert(xhr.responseText);

        },
        success: function (json) {
            if (val == 1) {
                $('#imgCaptcha').attr('src', json.d);
            } else {
                $('#imgCaptchaM').attr('src', json.d);
            }
        }
    });
}