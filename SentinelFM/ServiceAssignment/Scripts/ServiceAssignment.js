var g = jQuery.noConflict();
var configCount = 0;
var ruleOptions = null;
var ckMessageBody = null;
var selectedLandmarkIdBox = null;
var SC = { 'sc1': '130,80', 'sc2': '130,80', 'sc3': '100,64', 'sc4': '90,54', 'sc5': '70,40', 'sc6': '50,30', 'sc7': '30,20', 'sc8': '11,6' };
var SCdesc = { 1: '[>130 km/h / >80 MPH]', 2: '[101-130 km/h / 65-80 MPH]', 3: '[91-100 km/h / 55-64 MPH]', 4: '[71-90km/h / 41-54 MPH]', 5: '[51-70 km/h / 31-40 MPH]', 6: '[31-50 km/h / 21-30 MPH]', 7: '[11-30 km/h / 6-20 MPH]', 8: '[<11 km/h / <6 MPH]' };

g(document).ready(function () {
    CKEDITOR.replace('txtMessageBody', {
        htmlEncodeOutput: true
    });
    ckMessageBody = CKEDITOR.instances['txtMessageBody'];
    g('#btnSaveConfig').click(function() {
        DeleteAllValue();
    });    
    CreateFcTable();
    $("#sltInsertPlaceholder").html($('#sltInsertPlaceholder option').sort(function (x, y) {        
        return $(x).text() < $(y).text() ? -1 : 1;

    }));

    $("#sltInsertPlaceholder").get(0).selectedIndex = 0;

    $("#sltSubjectPlaceholder").html($('#sltSubjectPlaceholder option').sort(function (x, y) {
        return $(x).text() < $(y).text() ? -1 : 1;

    }));

    $("#sltSubjectPlaceholder").get(0).selectedIndex = 0;
    g('#btnCreateRule').on("click", function() {                
        DeleteAllValue();
        g('#btnAddRule').click();
        g('#deleteService').val('');
        var myButtons = {
            "Create new rule": function() {
                CreateExpression();
                if (ValidateForm()) {
                    g('#action').val("Save");
                    g('#ServiceConfigForm').submit();                    
                }
                
            },
            Cancel: function() {
                DeleteAllValue();
                g(this).dialog("close");
            }
        };        
        g("#dialog-modal").dialog('option', 'buttons', myButtons).dialog("open");        
    });

    g('#btnEditRule').on("click", function () {
       
        DeleteAllValue();               
        g('#deleteService').val('');
        g('#divDescription').html("");
        var myButtons = {
            /****/
            "Save rule": function() {
                CreateExpression();
                if (ValidateForm()) {
                    g('#action').val("Save");
                    g('#ServiceConfigForm').submit();
                }
            },
            /****/
            Delete: function() {
                var yes = confirm("Do you want to delete the configured service?");
                if (yes) {
                    g('#action').val("Save");
                    g('#deleteService').val(1);
                    g('#ServiceConfigForm').submit();
                }
            },
            Cancel: function() {
                DeleteAllValue();
                g(this).dialog("close");
            }
        };        
        g("#dialog-modal").dialog('option', 'buttons', myButtons).dialog("open");
        
        RendOutService();
       
    });
    g("#dialog:ui-dialog").dialog("destroy");
    g("#dialog-modal").dialog({
        autoOpen: false,
        position: ['center',20],
        height: 710,
        width: assignmentWith,
        modal: true,
        appendTo: "form"
    });

    g('#btnViewAssignments').click(function () {
        //g("#dialog-assignments").dialog("open");
        /****/
        var serviceName = "";
        var serviceVal = g('#ServicesList').val();
        switch(serviceVal) {
            case "1":
                serviceName = "landmark";
                break;
            case "2":
                serviceName = "speed";
                break;
            case "3":
                serviceName = "sensor";
                break;
            case "4":
                serviceName = "route";
                break;
        }
        if (serviceName == "") {
            alert("Please select a service");
            return;
        }
        var strWindowFeatures = "location=yes,scrollbars=yes,status=yes";
        var URL = "AssignmentForm.aspx?service=" + serviceName;
        var win = window.open(URL, "_blank", strWindowFeatures);
        /***/
    });
    g("#dialog-assignments").dialog({
        autoOpen: false,
        height: 350,
        width: 1100,
        modal: true,
        appendTo: "form"
    });
    
    g("#divLandmarks").dialog({
        autoOpen: false,
        position: ['right',30],
        height: 480,
        width: 500,
        modal: true,
        appendTo: "form",
        buttons: {            
            Cancel: function () {
                g(this).dialog("close");
            }
        }
    });

    g("#divDTCHelper").dialog({
        autoOpen: false,
        position: ['right', 30],
        height: 480,
        width: 500,
        modal: true,
        appendTo: "form",
        buttons: {
            Cancel: function () {
                g(this).dialog("close");
            }
        }
    });

    g("#divFC").dialog({
        autoOpen: false,
        position: ['right', 30],
        height: 350,
        width: 830,
        modal: true,
        appendTo: "form",
        buttons: {
            "Ok": function () {
                var filterString = null;
                g("#tblFchelper tr").each(function () {
                    g(this).find("td").each(function () {
                        if (g(this).find("input").length > 0) {
                            var val = g(this).find("select").val();
                            var id = g(this).find("select").attr("id");
                            var myval = val;
                            if (val == 1) {
                                var ids = id.split('_');
                                myval = SC[ids[1]];
                            }
                            if (val != "-1") {
                                if (filterString == null) {
                                    filterString = id + '-' + myval;
                                } else {
                                    filterString += '|' + id + '-' + myval;
                                }
                            }

                        }
                    });
                });
                selectedLandmarkIdBox.val(filterString);
                g(this).dialog("close");
            },
            Cancel: function () {
                g(this).dialog("close");
            }
        }
    });
    
    g("#dialog-report").dialog({
        autoOpen: false,
        height: 480,
        width: 1200,
        modal: true,
        appendTo: "form",
        buttons: {
            Cancel: function () {
                g(this).dialog("close");
            }
        }
    });

    g('#btnAddRule').on("click", function() {
        var lastId = GetLastConfigurationTr();
        if (lastId > 0) {
            var myval = g('#Valuebox_' + lastId).val();
            if (myval == null || myval == '') {
                return;
            }
        }
        
        CreateExpression();
        configCount = GetLastConfigurationTr() + 1;
        var getLastRule = g('#BaseRules_' + lastId).val();
        var getNextRule = GetMustIncludeRule(getLastRule);
        var subexpression = '';
        if (getNextRule != null && getNextRule != "") {
            subexpression = getNextRule + " = " + " ";
        }        
        AddNewConfig(subexpression);
    });

    g(document).on("change", 'select[id^=BaseRules_]', function (event) {        
        var id = parseInt(this.id.replace("BaseRules_", ""));
         g('#BaseRules_' + id + ' option:selected').each(function () {              
             var ruleName = g(this).val();             
             var settingStr = g('#hiddenRule_' + ruleName).val();             
             var settingArray = settingStr.split('|');
             var operators = settingArray[0];
             var operatorsArray = operators.split(',');
             g('#OperatorPlaceHolder_' + id).html(GetOperatorDropDownWithParams(operatorsArray, ''));             
              if (settingArray[1] == null | settingArray[1] == '') {
                  if (ruleName != "LandmarkIn" && ruleName != "LandmarkOut" && ruleName != "SpeedIn" && ruleName != "FCcheck" && ruleName != "Speed" && ruleName != "OverSpeed" && ruleName != "PROP-S4" && ruleName != "SpeedOr"
                      && ruleName != "ScheduleStart" && ruleName != "ScheduleEnd" && ruleName != "ScheduleSend" && ruleName != "PROP-DTC" && ruleName != "PROP-MID" && ruleName != "PROP-PID" && ruleName != "PROP-FMI") {
                      g('#ValuePlaceHolder_' + id).html(GetDefaultResultBox('', id));
                      g('#ValuePlaceHolder1_' + id).html(HideDefaultResultBox1(''));
                      g('#ValuePlaceHolder2_' + id).html(HideDefaultResultBox2(''));
                      g('#ValuePlaceHolder3_' + id).html(HideDefaultResultBox3(''));
                  }
                  else if (ruleName == "Speed" || ruleName == "OverSpeed" || ruleName == "PROP-S4" || ruleName == "SpeedOr") {
                      g('#ValuePlaceHolder_' + id).html(GetDefaultSpeedBox(''));
                      g('#ValuePlaceHolder1_' + id).html(GetLabelBox1(''));
                      g('#ValuePlaceHolder2_' + id).html(GetLabelBox2(''));
                      g('#ValuePlaceHolder3_' + id).html(GetLabelBox3(''));
                  }
                  else if (ruleName == "FCcheck") {
                      g('#ValuePlaceHolder_' + id).html(GetDefaultResultBoxWithPopup('', 'divFCHelperPop', id));
                      g('#ValuePlaceHolder1_' + id).html(HideDefaultResultBox1(''));
                      g('#ValuePlaceHolder2_' + id).html(HideDefaultResultBox2(''));
                      g('#ValuePlaceHolder3_' + id).html(HideDefaultResultBox3(''));
                  }
                  else if (ruleName == "ScheduleStart" || ruleName == "ScheduleEnd" || ruleName == "ScheduleSend") {
                      g('#ValuePlaceHolder_' + id).html(GetDefaultResultBoxWithPopup('', 'divTimePickerHelperPop'));                     
                      g('#ValuePlaceHolder1_' + id).html(HideDefaultResultBox1(''));
                      g('#ValuePlaceHolder2_' + id).html(HideDefaultResultBox2(''));
                      g('#ValuePlaceHolder3_' + id).html(HideDefaultResultBox3(''));
                  }
                  else if (ruleName == "PROP-DTC" || ruleName == "PROP-PID" || ruleName == "PROP-FMI" || ruleName == "PROP-MID") {
                      g('#ValuePlaceHolder_' + id).html(GetDefaultResultBoxWithPopup('', 'divDTCHelperPop', id));
                      g('#ValuePlaceHolder1_' + id).html(HideDefaultResultBox1(''));
                      g('#ValuePlaceHolder2_' + id).html(HideDefaultResultBox2(''));
                      g('#ValuePlaceHolder3_' + id).html(HideDefaultResultBox3(''));
                  }
                  else {
                      g('#ValuePlaceHolder_' + id).html(GetDefaultResultBoxWithPopup('', 'divLandmarksPop'));
                      
                      g('#ValuePlaceHolder1_' + id).html(HideDefaultResultBox1(''));
                      g('#ValuePlaceHolder2_' + id).html(HideDefaultResultBox2(''));
                      g('#ValuePlaceHolder3_' + id).html(HideDefaultResultBox3(''));
                  }
                 
             } else {
                 var resultSettingArray = settingArray[1].split(';');
                 g('#ValuePlaceHolder_' + id).html(GetResultsDropDownWithParams(resultSettingArray, ''));
                 g('#ValuePlaceHolder1_' + id).html(HideDefaultResultBox1(''));
                 g('#ValuePlaceHolder2_' + id).html(HideDefaultResultBox2(''));
                 g('#ValuePlaceHolder3_' + id).html(HideDefaultResultBox3(''));
              }
              DisplayDescription(ruleName);
         });
    });

    g(document).on("click", 'input[id^=Valuebox_]', function (event) {       
        var id = parseInt(this.id.replace("Valuebox_", ""));
        ShowToolTip(id);
    });
    
    g(document).on("click", 'select[id^=Valuebox_]', function (event) {
        var id = parseInt(this.id.replace("Valuebox_", ""));
        ShowToolTip(id);
    });
    
    var newRulesDropDownOptions = g('#BaseRules_0').clone();
    if (ruleOptions == null) {
        ruleOptions = newRulesDropDownOptions;
    }
    InitialAssignmentsList();

    g('#criteria').change(function() {
        InitialAssignmentsList();
    });

    g('#sltInsertPlaceholder').change(function () {
        if (g('#sltInsertPlaceholder').val() == "" || g('#sltInsertPlaceholder').val() == null) {
            return;
        }
        ckMessageBody.insertText(g('#sltInsertPlaceholder').val());
    });
    
    g('#sltSubjectPlaceholder').change(function () {
        if (g('#sltSubjectPlaceholder').val() == "" || g('#sltSubjectPlaceholder').val() == null) {
            return;
        }
        insertAtCaret("txtSubject", g('#sltSubjectPlaceholder').val());
    });
    
 //   g(".divLandmarksPop").popupDiv("#divLandmarks");
    g(document).on("click", ".divLandmarksPop", function() {
        //g(this.id).popupDiv("#divLandmarks", '#' + this.id);
        var myid = '#' + this.id;
        selectedLandmarkIdBox = g(myid);
        g('#divLandmarks').dialog('open');       
        InitialLandmarks();       
    });

    g(document).on("click", ".divDTCHelperPop", function () {
        selectedLandmarkIdBox = g('#' + this.id);
        var values = 1;                
        var ids = this.id.split('_');
        var myid = '#BaseRules_' + ids[1];
        var myvalue = g(myid).val();
        console.log(myvalue);
        if (myvalue == 'PROP-PID')
        {
            values = 2;
        } else if (myvalue == 'PROP-MID')
        {
            values = 3;
        }
        else if (myvalue == 'PROP-FMI') {
            values = 4;
        }        
        g('#divDTCHelper').dialog('open');        
        InitialDtcHelper(values);
    });

    g(document).on("click", ".divFCHelperPop", function () {
        var myid = '#' + this.id;
        ResetFcHelper();
        selectedLandmarkIdBox = g(myid);
        if (selectedLandmarkIdBox.val().length > 0) {
            RenderoutFcHelper(selectedLandmarkIdBox.val());
        }
        //CreateFcTable(selectedLandmarkIdBox.val());
        g('.editableSelect').toggleClass("editableSelect");
        g('.editableInput').toggleClass("editableInput");
        g('#divFC').dialog('open');

    });

    g(document).on("focus", ".divTimePickerHelperPop", function () {
        var myid = '#' + this.id;
        g(myid).timepicker();
    });

    g('#FleetAllEmail').click(function() {
        if (g(this).is(':checked')) {
            ClickOnFleetAll();
        } else {
            g('#FleetCriticalEmail').removeAttr('disabled');
            g('#FleetWarningEmail').removeAttr('disabled');
            g('#FleetNotifyEmail').removeAttr('disabled');
        }
    });
    //g('#ruleName').popupDiv("#divLandmarks");
});

function ClickOnFleetAll() {
    g('#FleetCriticalEmail').attr('checked', 'checked');
    g('#FleetWarningEmail').attr('checked', 'checked');
    g('#FleetNotifyEmail').attr('checked', 'checked');

    g('#FleetCriticalEmail').attr('disabled', 'disabled');
    g('#FleetWarningEmail').attr('disabled', 'disabled');
    g('#FleetNotifyEmail').attr('disabled', 'disabled');
}

function ShowToolTip(posId) {
    var ruleName = g('#BaseRules_' + posId).val();
    if (toolTips.length < 1) return;
    if (toolTips[ruleName].ToolTip != null && toolTips[ruleName].ToolTip != "") {
        g('#Valuebox_' + posId).attr("title", toolTips[ruleName].ToolTip);
        var tooltip = g('#Valuebox_' + posId).tooltip();
        tooltip.tooltip("open");
    }
}

function GetDefaultrDropDown() {
    var dropDown = '<select ID="operator_' + configCount + '" name="operator_' + configCount + '">' +
        '<option value="&gt;">&gt;</option>' +
        '<option value="&gt;=">&gt;=</option>' +
        '<option value="&lt;">&lt;</option>' +
        '<option value="&lt;=">&lt;=</option>' +
        '<option value="=">=</option>' +
        '</select>';
    return dropDown;
}

function CreateFcTable() {
    for (var r = 1; r <= 8; r++) {
        var trstr = "<tr>";
        for (var c = 0; c <= 5; c++) {
            if (c == 0) {
                trstr += "<td><strong>SC" + r + " " + SCdesc[r] + "</strong></td>";
            } else {

                trstr += '<td><select style="width:85px;" id="fc' + c + '_sc' + r + '">' +
                        '<option value="0">Use Posted</option>' +
                        '<option value="1">Use Max value</option>' +
                        '<option value="-1">Reject</option>' +
                        '</select></td>';
            }
        }
        trstr += "</tr>";
        g('#tblFchelper tbody').append(trstr);

        //g(trstr).toggleClass("editableSelect", true).appendTo('#tblFchelper tbody');        
    }

    g('#tblFchelper').addClass("editableSelect");
}

function RenderoutFcHelper(filterString) {
    if (filterString.trim() == "") {
        return;
    }
    var elements = filterString.split('|');
    var valueStor = {};
    g.each(elements, function (i, val) {
        var myvals = val.split('-');
        valueStor[myvals[0]] = myvals[1];
    });
    g("#tblFchelper tr").each(function () {
        g(this).find("td").each(function () {
            if (g(this).find("select").length > 0) {
                var myid = g(this).find("select").attr("id");
                if (valueStor[myid] != undefined) {
                    if (valueStor[myid] == "0") {
                        g('#' + myid).val(valueStor[myid]);
                    } else {
                        g('#' + myid).val(1);
                    }
                }
            }
        });
    });
}

function GetOperatorDropDownWithParams(operatorsArray, selectedValue) {
    var options = '';
    if (operatorsArray.length > 0) {
        for (var i = 0; i < operatorsArray.length; i++) {
            if (operatorsArray[i] == selectedValue) {
                options += '<option value="' + operatorsArray[i] + '" selected="selected">' + operatorsArray[i] + '</option>';
            } else {
                options += '<option value="' + operatorsArray[i] + '">' + operatorsArray[i] + '</option>';   
            }
        }
    }
    var dropDown = '<select ID="operator_' + configCount + '" name="operator_' + configCount + '">' +
        options +
        '</select>';
    return dropDown;
}

function GetResultsDropDownWithParams(resultsArray, selectedValue) {
    var options = '';
    if (resultsArray.length > 0) {
        for (var i = 0; i < resultsArray.length; i++) {
            var tempSet = resultsArray[i].split(':');
            if (tempSet[0] == selectedValue) {
                options += '<option value="' + tempSet[0] + '"  selected="selected">' + tempSet[1] + '</option>';
            } else {
                options += '<option value="' + tempSet[0] + '">' + tempSet[1] + '</option>';        
            }            
        }
    }
    var dropDown = '<select ID="Valuebox_' + configCount + '" name="Valuebox_' + configCount + '">' +
        options +
        '</select>';
    return dropDown;
}

function GetDefaultResultBox(inputValue) {
    var box = '<input type="Text" name="Valuebox_' + configCount + '" ID="Valuebox_' + configCount + '" value="' + inputValue + '" +  class="fixedTextbox" />';
    return box;
}
function GetDefaultResultBox(inputValue, id) {
    if (id == '' || id == null) {
        id = configCount;
    }
    var box = '<input type="Text" name="Valuebox_' + id + '" ID="Valuebox_' + id + '" value="' + inputValue + '" />';
    return box;
}

function HideDefaultResultBox1(inputValue) {
    var box = '<input type="Label" name="Valuebox1_' + configCount + '" ID="Valuebox1_' + configCount + '" value="' + '' + '" +  class="hideTextbox" />';
    return box;
}
function HideDefaultResultBox2(inputValue) {
    var box = '<input type="Text" name="Valuebox2_' + configCount + '" ID="Valuebox2_' + configCount + '" value="' + '' + '" +  class="hideTextbox" />';
    return box;
}
function HideDefaultResultBox3(inputValue) {
    var box = '<input type="Label" name="Valuebox3_' + configCount + '" ID="Valuebox3_' + configCount + '" value="' + '' + '" +  class="hideTextbox" />';
    return box;
}


function GetDefaultSpeedBox(inputValue) {
    var box = '<input type="Text" name="Valuebox_' + configCount + '" ID="Valuebox_' + configCount + '" value="' + '' + '" +  class="fixedTextbox" + onkeyup="handleKeyPress(event)" />';
    return box;
}
function GetLabelBox2(inputValue) {
    var box = '<input type="Text" name="Valuebox2_' + configCount + '" ID="Valuebox2_' + configCount + '" value="' + '' + '" +  class="resizedTextbox" + onkeyup="handleKeyPress1(event)" />';
    return box;
}
function GetLabelBox1(inputValue) {
    var box = '<input type="Label" name="Valuebox1_' + configCount + '" ID="Valuebox1_' + configCount + '" value="' + 'KPH' + '" +  class="resizedLabel" + onkeypress="javascript:return false;" />';
    return box;
}
function GetLabelBox3(inputValue) {
    var box = '<input type="Label" name="Valuebox3_' + configCount + '" ID="Valuebox3_' + configCount + '" value="' + 'MPH' + '" +  class="resizedLabel" + onkeypress="javascript:return false;"  />';
    return box;
}

function handleKeyPress(e) {
    
    var controlname = e.currentTarget.id.replace("Valuebox", "Valuebox2");
    try {
       // $('#' + controlname).val(parseInt($('#' + e.currentTarget.id).val() * 0.621371));
        $('#' + controlname).val(parseFloat($('#' + e.currentTarget.id).val() * 0.621371).toFixed(2));
    }
    catch (e) {
        $('#' + controlname).val();
    }
    //controlname.value = e.currentTarget.id.value;
}

function handleKeyPress1(e) {

    var controlname = e.currentTarget.id.replace("Valuebox2", "Valuebox");
    try {
        $('#' + controlname).val(parseFloat($('#' + e.currentTarget.id).val() * 1.60934).toFixed(2));
    }
    catch (e) {
        $('#' + controlname).val();
    }
    //controlname.value = e.currentTarget.id.value;
}

function GetDefaultResultBoxWithPopup(inputValue, className) {
    var box = '<input type="Text" name="Valuebox_' + configCount + '" ID="Valuebox_' + configCount + '" value="' + inputValue + '" +  class="resizedTextbox"/>';
    return box;
}
function GetDefaultResultBoxWithPopup(inputValue, className, id) {

    if (id == '' || id == null) {
        id = configCount;
    }
    var box = '<input type="Text" name="Valuebox_' + id + '" ID="Valuebox_' + id + '" value="' + inputValue + '" class="' + className + '" />';

    return box;
}
function GetLastConfigurationTr() {
    if(g('#tblServiceConfiguration tr').length < 1) {
        return 0;
    }
    var lastTrId = g('#tblServiceConfiguration tr:last').attr('id');
    var lastTrIds = lastTrId.split('_');
    var lastCount = parseInt(lastTrIds[1]);
    return lastCount;
}

function AddNewConfig(condition) {    
    var newRulesDropDown = '<select ID="BaseRules_' + configCount + '" name="BaseRules_' + configCount + '">' + ruleOptions.html() + '</select>';
    var newRow = "";
   // newRow = '<tr id="configurationTr_' + configCount + '"><td id="baserulestd_' + configCount + '" >' + newRulesDropDown + '</td><td><div id="OperatorPlaceHolder_' + configCount + '">' + GetDefaultrDropDown() + '</div></td><td><div id="ValuePlaceHolder_' + configCount + '" >' + GetDefaultResultBox('') + '</div></td><td><div id="ValuePlaceHolder1_' + configCount + '" >' + HideDefaultResultBox1('') + '</div></td></td><td></td></td><td><div id="ValuePlaceHolder3_' + configCount + '" >' + HideDefaultResultBox3('') + '</div></td><td><a href="#" onclick="DeleteValue(' + configCount + ');">Delete</a></td></tr>';
    newRow = '<tr id="configurationTr_' + configCount + '"><td id="baserulestd_' + configCount + '" >' + newRulesDropDown + '</td><td><div id="OperatorPlaceHolder_' + configCount + '">' + GetDefaultrDropDown() + '</div></td><td><div id="ValuePlaceHolder_' + configCount + '" >' + GetDefaultResultBox('') + '</div></td><td><div id="ValuePlaceHolder1_' + configCount + '" >' + HideDefaultResultBox1('') + '</div></td></td><td><div id="ValuePlaceHolder2_' + configCount + '" >' + HideDefaultResultBox2('') + '</div></td></td><td><div id="ValuePlaceHolder3_' + configCount + '" >' + HideDefaultResultBox3('') + '</div></td><td><a href="#" onclick="DeleteValue(' + configCount + ');">Delete</a></td></tr>';
    g('#tblServiceConfiguration').append(newRow);    
    if (condition != '' && condition != null) {
        var matches = condition.match(/\[(.*?)\]/);
        var submatch = null;
        if (matches) {
            submatch = matches[1];
            var iStart = condition.indexOf('[');
            condition = condition.substring(0, iStart);
        }               
        var conditions = condition.split(' ');
        var tempRule = conditions[0];
        var tempOperator = conditions[1];
        var tempValue = conditions[2];

        if (tempRule == 'Speed' || tempRule == 'OverSpeed' || tempRule == 'PROP-S4' || tempRule == 'SpeedOr') {
            newRow = '<tr id="configurationTr_' + configCount + '"><td id="baserulestd_' + configCount + '" >' + newRulesDropDown + '</td><td><div id="OperatorPlaceHolder_' + configCount + '">' + GetDefaultrDropDown() + '</div></td><td><div id="ValuePlaceHolder_' + configCount + '" >' + GetDefaultResultBox('') + '</div></td><td><div id="ValuePlaceHolder1_' + configCount + '" >' + HideDefaultResultBox1('') + '</div></td></td><td><div id="ValuePlaceHolder2_' + configCount + '" >' + HideDefaultResultBox2('') + '</div></td></td><td><div id="ValuePlaceHolder3_' + configCount + '" >' + HideDefaultResultBox3('') + '</div></td><td><a href="#" onclick="DeleteValue(' + configCount + ');">Delete</a></td></tr>';
        }
        else
        {
            newRow = '<tr id="configurationTr_' + configCount + '"><td id="baserulestd_' + configCount + '" >' + newRulesDropDown + '</td><td><div id="OperatorPlaceHolder_' + configCount + '">' + GetDefaultrDropDown() + '</div></td><td><div id="ValuePlaceHolder_' + configCount + '" >' + GetDefaultResultBox('') + '</div></td><td><div id="ValuePlaceHolder1_' + configCount + '" >' + HideDefaultResultBox1('') + '</div></td></td><td></td></td><td><div id="ValuePlaceHolder3_' + configCount + '" >' + HideDefaultResultBox3('') + '</div></td><td><a href="#" onclick="DeleteValue(' + configCount + ');">Delete</a></td></tr>';
        }
        g('#BaseRules_' + configCount).val(tempRule).change();
        g('#operator_' + configCount).val(tempOperator);
        if (submatch != null) {
            tempValue += '[' + submatch + ']';
        }
       
        g('#Valuebox_' + configCount).val(tempValue);
        if (tempRule == 'Speed' || tempRule == 'OverSpeed' || tempRule == 'PROP-S4' || tempRule == 'SpeedOr')
        {
            g('#Valuebox2_' + configCount).val((tempValue* 1.60934).toFixed(2));
        }
     
       
        DisplayDescription(tempRule);
    }
    AddHeight();
}


function CreateExpression() {
    var mylastexpression = '';
        g('select[id^=BaseRules_]').each(function() {
            var id = parseInt(this.id.replace("BaseRules_", ""));            
            var ruleName = g('#BaseRules_' + id).val();          
            var operator = g('#operator_' + id).val();
            var currentmyval = g('#Valuebox_' + id).val();
            var currentmyval2 = '';
            if (ruleName == 'Speed' || ruleName == 'OverSpeed' || ruleName == 'PROP-S4' || ruleName == 'SpeedOr')
            {
                //currentmyval2 = g('#Valuebox2_' + id).val();
                if (currentmyval != "" && currentmyval != null) {
                    var tempExp = ruleName + ' ' + operator + ' ' + currentmyval + ' ' + currentmyval2 + ';';
                    mylastexpression += tempExp;
                }
            }
            else {
                if (currentmyval != "" && currentmyval != null) {
                    var tempExp = ruleName + ' ' + operator + ' ' + currentmyval +  ';';
                    mylastexpression += tempExp;
                }
              
            }
            
        });
        g('#expression').text(mylastexpression);
}

function DeleteValue(rowNum) {
    g('#configurationTr_' + rowNum).remove();
    CreateExpression();
    DecreaseHeight();
}

function DeleteAllValue() {
    g('#ServiceConfigId').val('');
    g('#ruleName').val('');
    g('#expiredDate').val('');
    g('#tblServiceConfiguration tr').remove();
    g('#expression').text('');
    g('#txtEmailsList').val('');
    g('#txtSubject').val('');
    //g('#txtMessageBody').text('');
    ckMessageBody.setData('');
    
    g('#VehicleLevelEmail').removeAttr('checked');
    g('#LandmarkLevelEmail').removeAttr('checked');
    g('#FleetCriticalEmail').removeAttr('checked');
    g('#FleetWarningEmail').removeAttr('checked');
    g('#FleetNotifyEmail').removeAttr('checked');
    configCount = 0;
}

function RendOutService() {       
    var selectedConfiguredRules = g('#RulesList').val();
    if (selectedConfiguredRules == undefined || selectedConfiguredRules == "" || selectedConfiguredRules == null) {
        alert("Please select a configured rule");
        return;
    }
    g('#ServiceConfigId').val(selectedConfiguredRules);
    var configuredRuleStr = configuredServices[selectedConfiguredRules].RulesApplied;
    g('#ruleName').val(configuredServices[selectedConfiguredRules].ServiceConfigName);

    if (configuredServices[selectedConfiguredRules].IsActive == 'True')
        g('#chkActive').attr("checked", "checked");
    else
    {
        g('#chkActive').removeAttr('checked');
    }
    g('#chkReportable').prop('checked', configuredServices[selectedConfiguredRules].IsReportable == '1' ? true : false);
    g('#expiredDate').datepicker("setDate", new Date(configuredServices[selectedConfiguredRules].ExpiredDate));
    
    var configuredRules = configuredRuleStr.split(';');
    for (var i = 0; i < configuredRules.length; i++) {
        if (configuredRules[i] == '' || configuredRules[i] == null) {
            continue;
        }
        configCount = GetLastConfigurationTr() + 1;
        AddNewConfig(configuredRules[i]);        
    }
    
    CreateExpression();
    
    g('#txtEmailsList').val(configuredServices[selectedConfiguredRules].RecipientsList);
    g('#txtSubject').val(configuredServices[selectedConfiguredRules].Subject);    
    //g('#txtMessageBody').val(configuredServices[selectedConfiguredRules].Message);
    var message = configuredServices[selectedConfiguredRules].Message;
    ckMessageBody.editable().setData(message);
    SetEmailLevel(configuredServices[selectedConfiguredRules].EmailLevel);
    //alert(ckMessageBody.getData());
}

function SetEmailLevel(emailLevel) {    
    if (emailLevel != "" && emailLevel != null) {
        var emaillevels = emailLevel.split(';');
        g.each(emaillevels, function (i, val) {            
            if (val != null && val != "") {
                var id = null;
                switch (val) {
                    case 'Vehicle':
                        id = "VehicleLevelEmail";
                        break;
                    case 'Landmark':
                        id = "LandmarkLevelEmail";
                        break;
                    case 'FleetCritical':
                        id = "FleetCriticalEmail";
                        break;
                    case 'FleetWarning':
                        id = "FleetWarningEmail";
                        break;
                    case 'FleetNotify':
                        id = "FleetNotifyEmail";
                        break;
                    case 'FleetAll':
                        id = "FleetAllEmail";
                        break;
                }                
                if (id != null) {
                    g('#' + id).attr("checked", "checked");
                    if (id == "FleetAllEmail") {
                        ClickOnFleetAll();
                    }
                }
            }
            
        });
    }
}
function ValidateForm() {
    var all_ok = true;
    var allRules = new Array();
    
    var rulesArray = g('#expression').text().split(';');
    for (key in rulesArray) {
        
    }
    g.each(rulesArray, function (key, val) {
        if (val != "" && val != null) {
            var valsArray = val.split(' ');
            var vName = valsArray[0];
            
            
            if (vName != "LandmarkIn" && vName != "LandmarkOut" && vName != "Sensor") {                
                if (g.inArray(vName, allRules) > -1) {
                    alert(vName + " cannot be duplicated");
                    all_ok = false;
                    return false;
                }               
                else if (vName == "" || vName == null) {
                    alert(vName + " cannot be empty");
                    all_ok = false;
                    return false;
                }

            }
            if (vName != "" && vName != null) {
                allRules.push(vName);
            }
        }
        
    });    

    if (g('#ruleName').val().length < 1) {
        alert("Please input the service name");
        all_ok = false;
    }        
    
    if (g('#expression').text().length < 1) {
        alert("Please select at least one condition");
        all_ok = false;
    }    

    if (!ValidaIncludeAndExclude(allRules)) {
        all_ok = false;
    }

    if (!ValidaIncludeAndExclude(allRules)) {
        alert("Include and Exclude rules are not matched with your expression.");
        all_ok = false;
    }
    return all_ok;
}

function AddHeight() {
    var height = g("#dialog-modal").height() + 25;
    //console.log(height);
    g("#dialog-modal").css('height', height);
}

function DecreaseHeight() {
    var height = g("#dialog-modal").height() - 25;
    //console.log(height);
    if(height > 0)
        g("#dialog-modal").css('height', height);
}

function InitialAssignmentsList() {
    g('#AssignmentsList').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "AssignmentManager.aspx",
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
            { "sType": "text" },
            { "sType": "text" },
            { "sType": "text" },
            { "bSortable": false }
        ],
        "sPaginationType": "full_numbers",
        "fnServerData": function (sSource, aoData, fnCallback) {
            aoData.push({ "name": "searchCriteria", "value": g('#criteria').val() });
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
                if (json.iTotalRecords > 7) {
                    g('#dialog-assignments').css("height", 480);
                } else {
                    g('#dialog-assignments').css("height", 350);
                }
            });
        }
    });
    
    //g("#dialog-assignments").css('height', 350);
}

function InitialLandmarks() {
    g('#tblLandmarks').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "LandmarkIdHelper.aspx",
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
            { "bSortable": false }
        ],
        "sPaginationType": "full_numbers",
        "fnServerData": function (sSource, aoData, fnCallback) {
            var serviceType = g('#ServicesList').val();
            aoData.push({ "name": "serviceType", "value": serviceType });
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);                
            });
        }
    });

}

function InitialDtcHelper(dtcType) {
    g('#tblDtcTable').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "DtcValueHelper.aspx",
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
            { "bSortable": false }
        ],
        "sPaginationType": "full_numbers",
        "fnServerData": function (sSource, aoData, fnCallback) {            
            aoData.push({ "name": "dtcType", "value": dtcType });
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
            });
        }
    });

}

function insertAtCaret(areaId, text) {
    var txtarea = document.getElementById(areaId);
    var scrollPos = txtarea.scrollTop;
    var strPos = 0;
    var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ?
        "ff" : (document.selection ? "ie" : false));
    if (br == "ie") {
        txtarea.focus();
        var range = document.selection.createRange();
        range.moveStart('character', -txtarea.value.length);
        strPos = range.text.length;
    }
    else if (br == "ff") strPos = txtarea.selectionStart;

    var front = (txtarea.value).substring(0, strPos);
    var back = (txtarea.value).substring(strPos, txtarea.value.length);
    txtarea.value = front + text + back;
    strPos = strPos + text.length;
    if (g.browser.msie) {
        txtarea.focus();
        var range = document.selection.createRange();
        range.moveStart('character', -txtarea.value.length);
        range.moveStart('character', strPos);
        range.moveEnd('character', 0);
        range.select();
    }
    else if (br == "ff") {
        txtarea.selectionStart = strPos;
        txtarea.selectionEnd = strPos;
        txtarea.focus();
    }
    txtarea.scrollTop = scrollPos;
}

function SelectLandmark(landmarkId, landmarkName) {
    selectedLandmarkIdBox.val(landmarkId + '[' + landmarkName + ']');
    g('#divLandmarks').dialog('close');
}

function SelectDtcValue(dtcId, dtcName) {
    selectedLandmarkIdBox.val(dtcId + '[' + dtcName + ']');
    g('#divDTCHelper').dialog('close');
}

function InitializeReport(searchFor, searchField, serviceConfigId, serviceConfigName) {
    var fileName = serviceConfigName + "-" + searchFor + "-" + searchField;
    g('#dialog-report').dialog('open');
    g('#tblReport').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bDestroy": true,
        "sAjaxSource": "ViolationsReport.aspx",       
        "bAutoWidth": false,
        "aoColumns": [
            { "sType": "text" },
            { "sType": "text" },
            { "bSortable": false },
            { "sType": "text" },
            { "bSortable": false },
            { "bSortable": false },            
            { "sType": "text" },
            { "sType": "text" },
            { "bSortable": false }
        ],
        "sPaginationType": "full_numbers",
        "sDom": 'T<"clear">lfrtip',
        "oTableTools": {
            "sSwfPath": "Scripts/media/swf/copy_csv_xls_pdf.swf",
            "aButtons": [
                "copy",
                {
                    "sExtends": "csv",
                    "sTitle": "ExceptionsReport",
                    "sFileName": "ExceptionsReport-" + fileName + ".csv"
                },
                {
                    "sExtends": "xls",
                    "sTitle": "ExceptionsReport",
                    "sFileName": "ExceptionsReport-" + fileName + ".xls"
                },
                {
                    "sExtends": "pdf",
                    "sTitle": "ExceptionsReport",
                    "sFileName": "ExceptionsReport-" + fileName + ".pdf"
                },
                "print"
        ]
        },
       "aLengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]],
        "fnServerData": function (sSource, aoData, fnCallback) {
            aoData.push({ "name": "searchFor", "value": searchFor });
            aoData.push({ "name": "searchField", "value": searchField });
            aoData.push({ "name": "serviceConfigId", "value": serviceConfigId });
            g.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
                if (json.iTotalRecords > 7) {
                    g('#dialog-assignments').css("height", 480);
                } else {
                    g('#dialog-assignments').css("height", 350);
                }
            });
        }
    });
}

jQuery.fn.popupDiv = function (divToPop, myid) {
    var pos = g(myid).offset();
    var h = g(myid).height();
    var w = g(myid).width();
    g(divToPop).css({ left: pos.left + w + 10, top: pos.top + h + 10 });

    g(myid).click(function (e) {
        g(divToPop).css({ left: pos.left + w + 10, top: pos.top + h + 10 });
        if (g(divToPop).css('display') !== 'none') {
            g(divToPop).hide();
        }
        else {
            g(divToPop).show();
        }
    });
};

function ResetFcHelper() {
    g("#tblFchelper tr").each(function () {
        g(this).find("td").each(function () {
            if (g(this).find("select").length > 0) {
                //g(this).find("select").val(-1);   
                //var myid = g(this).find("select").attr("id");
                g(this).find("option").remove();
                g(this).find("select").append('<option value="0">Use Posted</option><option value="1" selected>Use Max value</option><option value="-1" selected>Reject</option>');
                g(this).find("select").eComboBox({
                    'allowNewElements': false,		// default : true
                    'editableElements': false		// default : true
                });
            }
        });
    });
}


function DisplayDescription(ruleName) {
    var mustInclude = toolTips[ruleName].MustInclude;
    var mustExclude = toolTips[ruleName].MustExclude;
    var sample = toolTips[ruleName].Sample;
    if (mustInclude == "" || mustExclude == null) {
        return;
    }
    g('#divDescription').html("");
    g('#divDescription').append("<div class=\"include\"> Must Include:&nbsp;" + mustInclude + "</div>");
    g('#divDescription').append("<div class=\"exclude\"> Must Exclude:&nbsp;" + mustExclude + "</div>");
    g('#divDescription').append("<div class=\"sample\"> Sample:&nbsp;" + sample + "</div>");
}

function GetMustIncludeRule(ruleName) {
    if (ruleName == '' || ruleName == null) {
        return null;
    }
    var mustInclude = toolTips[ruleName].MustInclude;
    if (mustInclude == "" || mustInclude == null) {
        return null;
    }
    var mustIncludesSeg = mustInclude.split(" and ");

    if (mustInclude.indexOf(" or ") > -1) {
        var mustIncludesSeg = mustInclude.split(" or ");
    }
    if (mustIncludesSeg.length > 1) {
        var mustIncludesSubSegs = mustIncludesSeg[1].split(" ");
        return mustIncludesSubSegs[mustIncludesSubSegs.length - 1];
    }
    return mustIncludesSeg[0];
}

function ValidaIncludeAndExclude(allRules) {
    var allPass = true;
    var allRulesStr = allRules.join(" ");
    for (var i = 0; i < allRules.length; i++) {
        var mustInclude = toolTips[allRules[i]].MustInclude;
        var mustExclude = toolTips[allRules[i]].MustExclude;
        if (mustInclude != "" && mustInclude != null) {
            var andOr = parseInclude(mustInclude);
            var andStrs = andOr.And.split(" ");
            var orStr = andOr.Or;
            for (var d = 0; d < andStrs.length; d++) {
                allPass = allPass && allRulesStr.indexOf(andStrs[d]) > -1;
            }
            if (orStr != null && orStr != "") {
                var tmpOr = false;
                for (var a = 0; a < allRules.length; a++) {
                    if (orStr.indexOf(allRules[a]) > -1) {
                        tmpOr = true;
                        break;
                    }
                }
                allPass = allPass && tmpOr;
            }                        
        }

        if (mustExclude != "" && mustExclude != null) {
            var excludeArray = mustExclude.split(",");
            for (var c = 0; c < excludeArray.length; c++) {               
                if (g.inArray(excludeArray[c], allRules) > -1) {
                    allPass = false;
                    break;
                }
            }
        }
    }
    return allPass;
}

function parseInclude(includeString) {
    var andRulesNoSpace = includeString.split(" ");
    var includeAndStr = "";
    var includeOrStr = "";
    for (var i = 0; i < andRulesNoSpace.length; i++) {
        if (andRulesNoSpace[i] == "or") {
            includeOrStr += andRulesNoSpace[i - 1] + " " + andRulesNoSpace[i + 1];
        }
    }

    for (var a = 0; a < andRulesNoSpace.length; a++) {
        if (andRulesNoSpace[a] != "and" && andRulesNoSpace[a] != "or") {
            if (includeOrStr.indexOf(andRulesNoSpace[a]) == -1) {
                if (includeAndStr == "" || includeAndStr == null) {
                    includeAndStr = andRulesNoSpace[a];
                }
                else includeAndStr += " " + andRulesNoSpace[a];
            }
        }
    }

    return { "And": includeAndStr, "Or": includeOrStr };
}

g.widget("ui.dialog", g.ui.dialog, {
    _allowInteraction: function (event) {
        return !!g(event.target).closest(".cke_dialog").length || this._super(event);
    }
});