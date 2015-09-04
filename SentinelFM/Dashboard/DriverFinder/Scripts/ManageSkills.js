
var g = jQuery.noConflict();
var organizationSkills = new Array();
var selectedSkillId;
var editaction;
var deleteSkill = 0;
var rowCount = 0;
g(document).ready(function () {
    //jQuery.noConflict();
    g('#OrganizationSkillsTable').dataTable({
        "aoColumns": [{ "sType": "natural" }, { "sType": "natural" }, null, null],
        "aaSorting": [[1, "asc"]],
        "sScrollX": "100%",
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bInfo": false
    });

    g('#btnAddNewSkill').click(function () {
        editaction = "sos";
        deleteSkill = 0;
        selectedSkillId = null;
        g("#dialog-form").dialog("open");
        g('#dialog-form').dialog({ zIndex: 99999 }); 
        g("#dialog-form").dialog('option', 'title', 'Create new Skill');
        g(".ui-dialog-buttonpane button:contains('Edit an skill') span").text("Create an skill");
        g(".ui-dialog-buttonpane button:contains('Delete an skill') span").text("Create an skill");
    });

    g("#dialog:ui-dialog").dialog("destroy");

    var name = g("#txtSkillName");
    var allFields = g([]).add(name);
    var tips = g(".validateTips");
    function updateTips(t) {
        tips.text(t)
				.addClass("ui-state-highlight");
        setTimeout(function () {
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

    function checkRegexp(o, regexp, n) {
        if (!(regexp.test(o.val()))) {
            o.addClass("ui-state-error");
            updateTips(n);
            return false;
        } else {
            return true;
        }
    }

    g("#dialog-form").dialog({
        autoOpen: false,
        height: 300,
        width: 350,
        modal: true,
        buttons: {
            "Create an skill": function () {
                var bValid = true;
                allFields.removeClass("ui-state-error");

                bValid = bValid && checkLength(name, "skill name", 1, 200);

                if (bValid) {
                    ProcessSkill();
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

    FillTable();

});


function AppendToTable(skillInfo) {
    g('#OrganizationSkillsTable').dataTable().fnAddData([
					skillInfo.SkillName,
                    skillInfo.SkillDescription,
                    "<a href=\"#\" onclick=\"EditMe('" + skillInfo.SkillId + "')\">Edit</a>",
                    "<a href=\"#\" onclick=\"DeleteMe('" + skillInfo.SkillId + "')\">Delete</a>"
                ]);
    var theNode = g('#OrganizationSkillsTable').dataTable().fnSettings().aoData[rowCount].nTr;
    theNode.setAttribute('id', 'row_' + skillInfo.SkillId);
    rowCount++;
    //theNode.id = 'row_' + skillInfo.SkillId;

}


function UpdateRow(skillId) {
    var tdArray = new Array();
    g('#row_' + skillId).find('td').each(function () {
        tdArray.push($(this));
     });    

     tdArray[0].text(organizationSkills[skillId].SkillName);
     tdArray[1].text(organizationSkills[skillId].SkillDescription);
}

function EditMe(skillId) {
    selectedSkillId = skillId;
    deleteSkill = 0;
    editaction = "uos";
    g("#txtSkillName").val(organizationSkills[skillId].SkillName);
    g("#txtSkillDescription").val(organizationSkills[skillId].SkillDescription);
    g("#dialog-form").dialog("open");
    g("#dialog-form").dialog('option', 'title', 'Edit Skill');
    g(".ui-dialog-buttonpane button:contains('Create an skill') span").text("Edit an skill");
    g(".ui-dialog-buttonpane button:contains('Delete an skill') span").text("Edit an skill");
}

function DeleteMe(skillId) {
    selectedSkillId = skillId;
    deleteSkill = 1;
    editaction = "dos";
    g("#txtSkillName").val(organizationSkills[skillId].SkillName);
    g("#txtSkillDescription").val(organizationSkills[skillId].SkillDescription);
    g("#dialog-form").dialog("open");
    g("#dialog-form").dialog('option', 'title', 'Delete Skill');
    g(".ui-dialog-buttonpane button:contains('Create an skill') span").text("Delete an skill");
    g(".ui-dialog-buttonpane button:contains('Edit an skill') span").text("Delete an skill");
}

function ProcessSkill() {
    var skillName = g('#txtSkillName').val();
    var skillDescription = g('#txtSkillDescription').val();
    if (deleteSkill == 1) {
        var r = confirm("Will you delete this skill?");
        if (!r) {
            return false;
        }
    }
    $.ajax({
        url: '../DriverFinder/ManageDriverSkills.aspx?action=' + editaction,
        type: 'POST',
        dataType: 'json',
        data: 'skillname=' + skillName + '&description=' + skillDescription + '&skillid=' + selectedSkillId + '&delete=' + deleteSkill,
        success: function (data) {
            var newId = parseInt(data.result);
            if (newId > 0) {
                //RefreshTable();
                if (editaction == "uos") {
                    organizationSkills[selectedSkillId].SkillName = skillName;
                    organizationSkills[selectedSkillId].SkillDescription = skillDescription;
                    UpdateRow(selectedSkillId);
                }
                else {
                    var newSkillObj = new Object();
                    newSkillObj.SkillId = newId.toString();
                    newSkillObj.SkillName = skillName;
                    newSkillObj.SkillDescription = skillDescription;
                    var newSkill = JSON.stringify(newSkillObj);
                    //alert(newSkill);
                    //AppendToTableParms(newId.toString(), skillName, skillDescription);
                    AppendToTable(newSkillObj);
                    organizationSkills[newSkillObj.SkillId] = newSkillObj;
                }

            }
            else if (editaction == "dos") {
                $('#row_' + selectedSkillId).remove();
            }
            else {
                alert("Cannot save skill");
            }
        }
    });
}

function CleanTable() {
    g('#OrganizationSkillsTable').dataTable().fnClearTable();
}

function RefreshTable() {
    CleanTable();
    FillTable();
}

function FillTable() {
    $.ajax({
        url: '../DriverFinder/ManageDriverSkills.aspx?action=ros',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            g.each(data, function (i) {
                organizationSkills[data[i].SkillId] = data[i];
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