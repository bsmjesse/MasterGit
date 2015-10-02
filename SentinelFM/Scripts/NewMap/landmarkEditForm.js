var allserviceslistContent = "-1";
var appliedserviceslistContent = "-1";
var originalRecepients;
var originalSubject;
var newserviceid = 0;

function showeditpolygonform() {
    try {
        $('#polygoninfo').hide();
        $('#polygoneditform').show();
        polygonPopup.setSize(new OpenLayers.Size(350, polygonPopupHeight));        
    }
    catch (err) { }
}
function editformmoreoptions() {
    $('.editformmainoptions').hide();
    $('.editformextendedattributes').hide();
    $('.editformmoreoptions').show();
    //$('.editformassigntofleet').hide();
}
function editformmainoptions() {
    $('.editformmoreoptions').hide();
    $('.editformextendedattributes').hide();
    $('.editformmainoptions').show();
    //$('.editformassigntofleet').hide();
}
function editformextendedattributes() {
    $('.editformmoreoptions').hide();
    $('.editformextendedattributes').show();
    $('.editformmainoptions').hide();
    //$('.editformassigntofleet').hide();

    originalRecepients = $('#txtServiceRecepients').val();
    originalSubject = $('#txtSubject').val();

    polygonPopup.setSize(new OpenLayers.Size(350, polygonPopupHeight + 65));
}

function editformassigntofleet(landmarkId) {
    var url = "Widgets/FleetAssignment.aspx?objectName=landmark&objectId=" + landmarkId;
    var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
    parent.openPopupWindow("Assign to fleet", urlToLoad, 520, 320);
}

function applyServices() {
    allserviceslistContent = "-1";
    appliedserviceslistContent = "-1";

    $('.editformmoreoptions').hide();
    $('.editformextendedattributes').hide();
    $('.editformmainoptions').show();

    resetServiceRuleContent();
}

function cancelServices() {
    if(allserviceslistContent != "-1")
        $('#AllServicesList').html(allserviceslistContent);

    if (appliedserviceslistContent != "-1")
        $('#AppliedServiceList').html(appliedserviceslistContent);

    $('#txtServiceRecepients').val(originalRecepients);
    $('#txtSubject').val(originalSubject);

    if (originalRecepients == $('#txtServiceRecepients').attr('PrompText'))
        $('#txtServiceRecepients').addClass('formtextprompt');

    if (originalSubject == $('#txtSubject').attr('PrompText'))
        $('#txtSubject').addClass('formtextprompt');

    allserviceslistContent = "-1";
    appliedserviceslistContent = "-1";
    
    $('.editformmoreoptions').hide();
    $('.editformextendedattributes').hide();
    $('.editformmainoptions').show();

    resetServiceRuleContent();
}

function resetServiceRuleContent() {
    $('#SerivceRule').html($('#helptip').val());
    $('#txtServiceRecepients').removeClass('validateError');
    polygonPopup.setSize(new OpenLayers.Size(350, polygonPopupHeight));  
}

function onServiceClick(o) {
    $('#SerivceRule').html($(o).attr('rel'));
    highlightservice(o);
}

function onServiceDblclick(o) {
    if (allserviceslistContent == "-1") {
        allserviceslistContent = $('#AllServicesList').html();
    }
    if (appliedserviceslistContent == "-1") {
        appliedserviceslistContent = $('#AppliedServiceList').html();
    }
    if ($(o).parent().parent().hasClass('allservices')) {
        $('#AppliedServiceList').find('ul').append($(o));
    }
    else if ($(o).parent().parent().hasClass('appliedservices')) {
        $('#AllServicesList').find('ul').append($(o));
    }
    highlightservice(o);
}

function highlightservice(o) {
    $('#lblSubject').html($(o).attr('subjects'));
    $('#lblRecepients').html($(o).attr('recepients'));

    $('.selectedservice').removeClass('selectedservice');
    $('.selectedavailableservice').removeClass('selectedavailableservice');
    if ($(o).closest('div').hasClass('appliedservices')) {
        $(o).addClass('selectedservice');
    }
    else {
        $(o).addClass('selectedavailableservice');
    }   
}

function updateServices() {
    var ids = [];
    //var idsnew = [];
    $('#AppliedServiceList li').each(function (idx, li) {
        /*if ($(li).attr('id') != '' && $(li).attr('id') != '-1')
        ids.push($(li).attr('id'));
        else {
        idsnew.push($(li).attr('name') + '|**|' + $(li).attr('rel'));
        }*/
        var s = $(li).attr('id') + '|**|' + $(li).attr('name') + '|**|' + $(li).attr('rel') + '|**|' + $(li).attr('recepients') + '|**|' + escape($(li).attr('subjects'));
        ids.push(s);
    });
    $('#updatedAppliedServices').val(ids.join('==++=='));
    /*$('#newAppliedServices').val(idsnew.join('==++=='));
    if ($('#txtServiceRecepients').val() != $('#txtServiceRecepients').attr('PrompText') && $('#txtServiceRecepients').val().trim() != '') {
        $('#serviceEmailRecepients').val($('#txtServiceRecepients').val());
    }
    else {
        $('#serviceEmailRecepients').val('');
    }
    if ($('#txtSubject').val() != $('#txtSubject').attr('PrompText') && $('#txtSubject').val().trim() != '') {
        $('#serviceEmailSubject').val($('#txtSubject').val());
    }
    else {
        $('#serviceEmailSubject').val('');
    }*/
}

function createNewServices() {
    $('#applyservices').hide();
    $('#createNewServices').show();
}

function cancelCreateNewServices() {
    resetNewServiceForm();
    $('#applyservices').show();
    $('#createNewServices').hide();
}

function applyCreateNewServices() {
    if ($('#txtServicename').val().trim() == '' || $('#txtServicename').val().trim() == $('#txtServicename').attr('PrompText')) {
        $('#createrulemessage').html('Service name is required.');
        return;
    }
    if ($('.ruleitemnewserviceWrapper .ruleitemnewservice').length == 0) {
        $('#createrulemessage').html('Please add at least one rule for this service.');
        return;
    }

    var rules = '';
    $('.ruleitemnewserviceWrapper .ruleitemnewservice').each(function (idx, item) {
        rules += $(item).html().replace(/&nbsp;/g, ' ').replace(/&gt;/g, '>').replace('&lt;', '<');
    });

    $('.selectedservice').removeClass('selectedservice');
    $('#lblSubject').html('');
    $('#lblRecepients').html('');

    var s = "<li id='-1' rel='" + rules + "' recepients='' subjects='' class='selectedservice' onclick='onServiceClick(this);' name='" + escape($('#txtServicename').val().trim()) + "'>" + $('#txtServicename').val().trim() + " <a href='javascript:void(0)' onclick='removeService(event, this);'>x</a></li>";

    $('#AppliedServiceList').find('ul').append(s);
    $('#SerivceRule').html(rules);

    resetNewServiceForm();
    $('#applyservices').show();
    $('#createNewServices').hide();
}

function resetNewServiceForm() {
    $('#txtServicename').val($('#txtServicename').attr('PrompText')).addClass('formtextprompt');
    //$('#txtServiceRecepients').val($('#txtServiceRecepients').attr('PrompText')).addClass('formtextprompt');
    //$('#txtSubject').val($('#txtSubject').attr('PrompText')).addClass('formtextprompt');
    $('#ruleValue').val('');
    $('#ruleslist').html('');
    $('#createrulemessage').html('');
}

function addNewServices() {
    if ($('#ruleValue').val() == '') {
        $('#createrulemessage').html('Please set a valid value for this rule.');
        return;
    }
    var s = $('#cboRuleName').val() + '&nbsp;' + $('#ruleOperator').val().replace('Over', '&gt;=').replace('Less', '&lt;=') + '&nbsp;';

    var duplicated = false;
    $('.ruleitemnewserviceWrapper .ruleitemnewservice').each(function (idx, item) {
        if ($(item).html().indexOf(s) >= 0) {
            duplicated = true;
        }
    });

    if (duplicated) {
        $('#createrulemessage').html('Please do not set duplicated rules.');
        return;
    }

    $('#createrulemessage').html('');

    s = '<div class="ruleitemnewserviceWrapper" onmouseenter="toggleRuleCloser(this, true);" onmouseleave="toggleRuleCloser(this, false);"><div class="ruleitemnewservice">' + s + $('#ruleValue').val() + ';</div><div class="rulecloser">&nbsp;<span class="btnruleclose" style="display:none;"><a href="javascript:void(0)" onclick="deleteServiceRule(this);">X</a></span></div></div> ';
    $('#ruleslist').append(s);
}

function toggleRuleCloser(o, s) {
    if (s) {
        $(o).find('.btnruleclose').show();
    }
    else {
        $(o).find('.btnruleclose').hide();
    }
}

function removeService(event, o) {
    event.stopPropagation();
    $(o).closest('li').remove();
    if ($('.selectedservice').length != 1) {
        $('#lblSubject').html('');
        $('#lblRecepients').html('');
    }
    return false;
}

function deleteServiceRule(o) {
    $(o).closest('.ruleitemnewserviceWrapper').remove();
}

function focustextbox(o) {
    if ($(o).val() == $(o).attr('PrompText'))
        $(o).val('');
    $(o).removeClass('formtextprompt');
}

function blurtextbox(o) {
    var s = $(o).attr('PrompText');
    if ($(o).val().trim() == '' || $(o).val().trim() == s) {
        $(o).val(s);
        $(o).addClass('formtextprompt');
    }
    else
        $(o).removeClass('formtextprompt');
}

function showSubjectTemplate(o) {
    if($('.selectedservice').length != 1) {
        $('#SerivceRule').html('Please select a service.');
        return;
    }
    $('#txtSubject').val($('#lblSubject').html());
    $('#subjectTemplate').css('left', 10).css('top', 40).show();    
}

function showRecepientsTemplate(o) {
    if($('.selectedservice').length != 1) {
        $('#SerivceRule').html('Please select a service.');
        return;
    }
    $('#txtRecepients').val($('#lblRecepients').html());
    $('#recepientsTemplate').css('left', 10).css('top', 100).show();
    
}

function setSubject(o) {
    $('#txtSubject').val($(o).attr('rel')).removeClass('formtextprompt');    
}

function applyServiceSubject() {
    $('#lblSubject').html($('#txtSubject').val());
    $('li.selectedservice').attr('subjects', $('#txtSubject').val());
    $('#txtSubject').val('');
    $('#subjectTemplate').hide();
}

function cancelServiceSubject() {
    $('#txtSubject').val('');
    $('#subjectTemplate').hide();
}

function applyServiceRecepients() {
    // validate email format
    if ($('#txtRecepients').val().trim() != '') {
        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
        var as = $('#txtRecepients').val().split(/[\s;,]+/)
        for (var i = 0; i < as.length; i++) {
            if (!emailReg.test(as[i])) {
                $('#txtRecepients').addClass('validateError');                
                return;
            }
        }
    }

    $('#lblRecepients').html($('#txtRecepients').val());
    $('li.selectedservice').attr('recepients', $('#txtRecepients').val());
    $('#txtRecepients').val('');
    $('#recepientsTemplate').hide();
}

function cancelServiceRecepients() {
    $('#txtRecepients').val('');
    $('#recepientsTemplate').hide();
}