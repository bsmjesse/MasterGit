var g = jQuery.noConflict();
var driverSkills = new Array();
var selectedKey;
var editaction;
var deleteSkill = 0;
var rowCount = 0;
g(document).ready(function () {
    //jQuery.noConflict();
    g('#DriverSkill').dataTable({
        "aoColumns": [{ "sType": "natural" }, { "sType": "natural" }, null, null],
        "aaSorting": [[1, "asc"]],
        "sScrollX": "100%",
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bInfo": false
    });

    g('#AddNewDriverSkill').click(function () {
        editaction = "sds";
        deleteSkill = 0;
        selectedKey = null;
        DisplayDriversList();
        g("#dialog-form").dialog("open");
        g('#dialog-form').dialog({ zIndex: 99999 });
        g("#dialog-form").dialog('option', 'title', 'Create new driver skill');
        g(".ui-dialog-buttonpane button:contains('Edit an driver skill') span").text("Create an driver skill");
        g(".ui-dialog-buttonpane button:contains('Delete an driver skill') span").text("Create an driver skill");
    });

    g("#dialog:ui-dialog").dialog("destroy");



    g("#dialog-form").dialog({
        autoOpen: false,
        autoHeight: false,
        width: 350,
        modal: true,
        buttons: {
            "Create an driver skill": function () {

                var shouldClose = ProcessSkill();
                if (shouldClose) {
                    g(this).dialog("close");
                }

            },
            Cancel: function () {
                g(this).dialog("close");
            }
        },
        close: function () {
        }
    });

    FillTable();

});


function AppendToTable(driverSkillInfo) {
    var selectedKey = driverSkillInfo.DriverId + '_' + driverSkillInfo.SkillId;
    g('#DriverSkill').dataTable().fnAddData([
					driverSkillInfo.FullName,
                    driverSkillInfo.SkillName,
                    driverSkillInfo.DsDescription,
                    "<a href=\"#\" onclick=\"EditMe('" + selectedKey + "')\">Edit</a>&nbsp;<a href=\"#\" onclick=\"DeleteMe('" + selectedKey + "')\">Delete</a>"
                ]);
    var theNode = g('#DriverSkill').dataTable().fnSettings().aoData[rowCount].nTr;
    theNode.setAttribute('id', 'row_' + driverSkillInfo.DriverId + '_' + driverSkillInfo.SkillId);
    rowCount++;
    //theNode.id = 'row_' + skillInfo.SkillId;

}


function UpdateRow(driverSkillId) {
    var tdArray = new Array();
    g('#row_' + driverSkillId).find('td').each(function () {
        tdArray.push($(this));
    });

    tdArray[1].text(driverSkills[driverSkillId].SkillName);
    tdArray[2].text(driverSkills[driverSkillId].DsDescription);
}

function EditMe(myKey) {
    selectedKey = myKey;
    deleteSkill = 0;
    editaction = "uds";
    DisplayDriverName(driverSkills[selectedKey].FullName)
    g("#skillsList").val(driverSkills[selectedKey].SkillId);
    g("#txtSkillDescription").val(driverSkills[selectedKey].DsDescription);    
    g("#dialog-form").dialog("open");
    g("#dialog-form").dialog('option', 'title', 'Edit driver skill');
    g(".ui-dialog-buttonpane button:contains('Create an driver skill') span").text("Edit an driver skill");
    g(".ui-dialog-buttonpane button:contains('Delete an driver skill') span").text("Edit an driver skill");
}

function DeleteMe(myKey) {
    selectedKey = myKey;
    deleteSkill = 1;
    editaction = "dds";
    DisplayDriverName(driverSkills[selectedKey].FullName)
    g("#skillsList").val(driverSkills[selectedKey].SkillId);
    g("#dialog-form").dialog("open");
    g("#dialog-form").dialog('option', 'title', 'Delete driver\'s skill');
    g(".ui-dialog-buttonpane button:contains('Create an driver skill') span").text("Delete an driver skill");
    g(".ui-dialog-buttonpane button:contains('Edit an driver skill') span").text("Delete an driver skill");
}

function ProcessSkill() {
    
    
    var skillName = g('#skillsList option:selected').text();
    var description = g('#txtSkillDescription').val();
    var driverId = g('#DriversList').val();
    var skillId = g('#skillsList').val();
    var oldSkillId = 0;

    if (selectedKey != null && selectedKey != "") {
        var driverSkllsArray = selectedKey.split('_');
        driverId = driverSkllsArray[0];
        oldSkillId = driverSkills[selectedKey].SkillId;
    }

    if (deleteSkill == 1) {
        var r = confirm("Will you delete this driver skill?");
        if (!r) {
            return false;
        }
    }
    $.ajax({
        url: '../DriverFinder/ManageDriverSkills.aspx?action=' + editaction,
        type: 'POST',
        dataType: 'json',
        data: 'driverId=' + driverId + '&skillid=' + skillId + '&delete=' + deleteSkill + '&oldskillid=' + oldSkillId + "&description=" + description,
        success: function (data) {
            if (editaction == "dds") {
                $('#row_' + selectedKey).remove();
                return true;
            }

            if (data == null) {
                return false;
            }
            var newId = data.result;

            if (newId == "NeedLogin") {
                window.location.replace("Login.aspx");
                return false;
            }

            if (newId != null && newId != "") {
                //RefreshTable();
                if (editaction == "uds") {
                    driverSkills[selectedKey].SkillId = skillId;
                    driverSkills[selectedKey].SkillName = skillName;
                    driverSkills[selectedKey].DsDescription = description;
                    UpdateRow(selectedKey);
                }
                else {
                    var newSkillObj = new Object();
                    newSkillObj.DriverId = driverId;
                    newSkillObj.SkillId = skillId;
                    newSkillObj.SkillName = skillName;
                    newSkillObj.FullName = g('#DriversList option:selected').text();
                    newSkillObj.DsDescription = description;
                    AppendToTable(newSkillObj);
                    driverSkills[newId] = newSkillObj;
                }

            }
            else {
                alert("Cannot save skill");
                return false;
            }
        }
    });
    return true;
}

function CleanTable() {
    g('#driverSkillsTable').dataTable().fnClearTable();
}

function RefreshTable() {
    CleanTable();
    FillTable();
}

function DisplayDriverName(driverName) {
    $('#listDrivers').css("display", "none");
    $('#DriverName').css("display", "block");
    $('#DriverName').html("<p><b>" + driverName + "</b></p>");
}

function DisplayDriversList() {
    $('#listDrivers').css("display", "block");
    $('#DriverName').css("display", "none");    
}

function FillTable() {
    $.ajax({
        url: '../DriverFinder/ManageDriverSkills.aspx?action=rds',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            if(data.result != undefined) {
                if (data.result == "NeedLogin") {
                    window.location.replace("Login.aspx");
                    return false;
                }
            }
            g.each(data, function (i) {
                driverSkills[data[i].DriverId + '_' + data[i].SkillId] = data[i];
                AppendToTable(data[i]);
            });
        }
    });
}

g.fn.dataTableExt.oSort['natural-asc'] = function (a, b) {
    var processedA = a.replace(/<(?:.|\n)*?>/gm, '');
    var processedB = b.replace(/<(?:.|\n)*?>/gm, '');

    return naturalSort(processedA, processedB);
};

g.fn.dataTableExt.oSort['natural-desc'] = function (a, b) {
    var processedA = a.replace(/<(?:.|\n)*?>/gm, '');
    var processedB = b.replace(/<(?:.|\n)*?>/gm, '');

    return naturalSort(processedA, processedB) * -1;
};
g.fn.dataTableExt.aTypes.unshift(
    function (sData) {
        var deformatted = sData.replace(/[^\d\-\.\/a-zA-Z]/g, '');
        if (g.isNumeric(deformatted)) {
            return 'formatted-num';
        }
        return null;
    }
);
g.fn.dataTableExt.oSort['formatted-num-asc'] = function (a, b) {
    /* Remove any formatting */
    var x = a.match(/\d/) ? a.replace(/[^\d\-\.]/g, "") : 0;
    var y = b.match(/\d/) ? b.replace(/[^\d\-\.]/g, "") : 0;

    /* Parse and return */
    return parseFloat(x) - parseFloat(y);
};

g.fn.dataTableExt.oSort['formatted-num-desc'] = function (a, b) {
    var x = a.match(/\d/) ? a.replace(/[^\d\-\.]/g, "") : 0;
    var y = b.match(/\d/) ? b.replace(/[^\d\-\.]/g, "") : 0;

    return parseFloat(y) - parseFloat(x);
};

// Natural sort function
function naturalSort(a, b) {
    var re = /(^-?[0-9]+(\.?[0-9]*)[df]?e?[0-9]?$|^0x[0-9a-f]+$|[0-9]+)/gi,
        sre = /(^[ ]*|[ ]*$)/g,
        dre = /(^([\w ]+,?[\w ]+)?[\w ]+,?[\w ]+\d+:\d+(:\d+)?[\w ]?|^\d{1,4}[\/\-]\d{1,4}[\/\-]\d{1,4}|^\w+, \w+ \d+, \d{4})/,
        hre = /^0x[0-9a-f]+$/i,
        ore = /^0/,
        i = function (s) { return naturalSort.insensitive && ('' + s).toLowerCase() || '' + s },
    // convert all to strings strip whitespace
        x = i(a).replace(sre, '') || '',
        y = i(b).replace(sre, '') || '',
    // chunk/tokenize
        xN = x.replace(re, '\0$1\0').replace(/\0$/, '').replace(/^\0/, '').split('\0'),
        yN = y.replace(re, '\0$1\0').replace(/\0$/, '').replace(/^\0/, '').split('\0'),
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