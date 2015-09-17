var globalV = null;
var currentUrl = null;
var report = null;
var dashboard = null;
var controlId = null;
var dashboardData = {};



$(document).ready(function () {

    visualize(
    {
        server: SERVER_URL,
        auth: {
            token: "bsm-" + token,
            loginFn: function (properties, request) {
     
                return request({
                    url: SERVER_URL + "/flow.html?_flowId=homeFlow&token=bsm-" + token
                });
            }


        }
    },

    function (v) {
        globalV = v;
        $(window).unload(function () {
            v.logout().always(function () { alert("logout"); });
        });

        if (biDemo.length > 0) {
            v.resourcesSearch({
                folderUri: biDemo,
                recursive: true,
                //types: ["reportUnit"],
                //success: renderResults,
                success: function (results) {
                    renderResults("Ad Hoc Reports", results, "selected_resource_dashboard");
                },
                error: function (err) {
                    renderResults("Ad Hoc Reports", null, "selected_resource_dashboard");
                }
            });
        }
        var reportExports = v.report
                    .exportFormats
                    .concat(["json"]);
        $select = buildExportControl("Export to: ", reportExports);
        v.resourcesSearch({
            folderUri: biPublicDashboard,
            recursive: true,
            //types: ["reportUnit"],
            //success: renderResults,
            success: function (results) {
                renderResults("Dashboard", results, "selected_resource_dashboard");
            },
            error: function (err) {
                renderResults("Dashboard", null, "selected_resource_dashboard");
            }
        });
        v.resourcesSearch({
            folderUri: biPublicReports,
            recursive: true,
            //types: ["reportUnit"],
            //success: renderResults,
            success: function (results) {
                renderResults("Report", results, "selected_resource_dashboard");
            },
            error: function (err) {
                renderResults("Report", null, "selected_resource_dashboard");
            }
        });
        v.resourcesSearch({
            folderUri: biPublicAdHoc,
            recursive: true,
            //types: ["reportUnit"],
            //success: renderResults,
            success: function (results) {
                renderResults("View", results, "selected_resource_dashboard");
            },
            error: function (err) {
                renderResults("View", null, "selected_resource_dashboard");
            }
        });
        v.resourcesSearch({
            folderUri: biOrganizationDashboard,
            recursive: true,
            //types: ["reportUnit"],
            //success: renderResults,
            success: function (results) {
                renderResults("Dashboard", results, "selected_resource_private");
            },
            error: function (err) {
                renderResults("Dashboard", null, "selected_resource_private");
            }
        });
        v.resourcesSearch({
            folderUri: biOrganizationReports,
            recursive: true,
            types: ["reportUnit"],
            //success: renderResults,
            success: function (results) {
                renderResults("Report", results, "selected_resource_private");
            },
            error: function (err) {
                renderResults("Report", null, "selected_resource_private");
            }
        });

        $(':disabled').prop('disabled', false);
        //createReport($("#selected_resource").val());




    }, function (err) {
        alert(err.message);
    });
    $("#selected_resource_dashboard").change(function () {
        $("#container").html("");
        dashboardData = {};
        if ($("#selected_resource_dashboard").val() != "") {
            currentUrl = $("#selected_resource_dashboard").val();
            $("#productFamilySelector").html("");
            if (currentUrl.indexOf("Dashboards") > -1)
            {
                report = null;
                createDashboard(currentUrl);                                
                $('#divReportView').hide();
                $('#container').show();

                //iframe
                //createDashboardInView(currentUrl)                
                //$('#divReportView').show();
                //$('#container').hide();
            }
            else if (currentUrl.indexOf("Standard_Views") > -1)
            {
                //createView(currentUrl);
                //buildReportInput();
                //dashboard = null;
                createReportInView(currentUrl, "adhocFlow");
                $('#divReportView').show();
                $('#container').hide();
            }
            else
            {
                //createReport(currentUrl);
                //buildReportInput();
                //dashboard = null;
                createReportInView(currentUrl, null);
                $('#divReportView').show();
                $('#container').hide();
            }
            
            
            
        }

    });

    $("#productFamilySelector").on("change", ".input_control", function () {
        if (report == null) return;
        var data = {};
        var myid = $(this).attr('id');        
        var v = [];        
        var vals = $.trim($(this).val()).split(",");
        for (var i = 0; i < vals.length; i++)
        {
            v.push($.trim(vals[i]));
        }
        
        data[myid] = v;        
        report.params(data)
            .run();

    });
   
   
    $("#pageRange").change(function () {
        if (report == null) return;
        report
            .pages($(this).val())
            .run()
                .fail(function (err) { alert(err); });
    });

    $("#previousPage").click(function () {
        if (report == null) return;
        var currentPage = report.pages() || 1;

        report
            .pages(--currentPage)
            .run()
                .fail(function (err) { alert(err); });
    });

    $("#nextPage").click(function () {
        if (report == null) return;
        var currentPage = report.pages() || 1;

        report
            .pages(++currentPage)
            .run()
                .fail(function (err) { alert(err); });
    });

    $('#expButton').click(function () {
        if (report == null) return;
        console.log($select.val());
        if ($("#pageRange").val() != "" && $("#pageRange").val() != "0")
        {
            report.export({
                //export options here        
                outputFormat: $select.val(),
                //exports all pages if not specified
                pages: $("#pageRange").val()
            }, function (link) {
                var url = link.href ? link.href : link;
                window.location.href = url;
            }, function (error) {
                console.log(error);
            });
        }
        else
        {
            report.export({
                //export options here        
                outputFormat: $select.val(),
            }, function (link) {
                var url = link.href ? link.href : link;
                window.location.href = url;
            }, function (error) {
                console.log(error);
            });
        }
        
    });
});

function buildReportInput() {
    globalV.inputControls({
        resource: currentUrl,
        success: function (controls) {
            controls.forEach(buildControl);
        },
        //success: renderInputControls,
        error: function (err) {
            alert(err);
        }
    });
}
function createReport(uri) {    
    $("#container").html("");
    report = globalV.report({
        resource: uri,
        //success: function(data){
        //    $("#showPagination").css("visibility", "visible");
        //},
        error: function (err) {
            alert(err.message);
        },
        container: "#container"
    });
}

function createView(uri) {
    $("#container").html("");
    report = globalV.report({
        resource: uri,
        //success: function(data){
        //    $("#showPagination").css("visibility", "visible");
        //},
        error: function (err) {
            alert(err.message);
        },
        container: "#container"
    });
}

function createDashboard(uri) {
    $("#container").html("");
    dashboard = globalV.dashboard({
        resource: uri,
        //success: function(data){
        //    $("#showPagination").css("visibility", "visible");
        //},
        error: function (err) {
            alert(err.message);
        },
        success: function() {
            //$("button").prop("disabled", false);
            buildDashboardControl();
            $("div[data-componentid='Filter_Group']").hide();
        },
        container: "#container"
    });
}

function renderResults(uriName, results, controlId) {    
    $('#' + controlId).append("<option>" + uriName + "</option>");
    if (results == null) return;
    for (var i = 0; i < results.length; i++) {       
        $('#' + controlId).append("<option value=\"" + results[i].uri + "\" >- - - " + results[i].label + "</option>");
    }   

}

function buildControl(control) {

    function buildOptions(options) {
        if (options == undefined || options.length < 1) return '';
        var template = "<option>{value}</option>";
        return options.reduce(function(memo, option) {
            return memo + template.replace("{value}", option.value);
        }, "");
    }
    if (control.state.options == undefined) return;
    console.log(control);
    controlId = control.id;
    var templateControl = getInputControlTemplate(control);
    ////var template = "<label>{label}</label><select id=\"" + controlId + "\">{options}</select><br>";
    var template = "<label>{label}</label>" + templateControl + "</select><br>";
    var content = template.replace("{label}", control.label)
            .replace("{options}", buildOptions(control.state.options));
    if ($.trim($("productFamilySelector").html()) == '') {
        $("#productFamilySelector").append($(content));
    }
    
    
}

function buildDashboardControl()
{
    var params = dashboard.data().parameters;
    console.log(params);
   
    for (var i = params.length - 1; i >= 0; i--) {
        console.log(params[i].id);
        if (params[i].id.indexOf(":") > -1) continue;
        var $el = $("<div>" + params[i].id + ": <input type='text' data-paramId='" + params[i].id + "' id=\"" + params[i].id + "\" /><input type=\"button\" onclick=\"runDashboardWithParams('" + $.trim(params[i].id) + "')\" value=\"Apply\" class=\"dashboardControl\" /></div>");

        $("#productFamilySelector").prepend($el);

        //$el.find("input").val(dashboard.params()[params[i].id]);
    }
}


function buildExportControl(name, options) {

    function buildOptions(options) {
        var template = "<option>{value}</option>";
        return options.reduce(function(memo, option) {
            return memo + template.replace("{value}", option);
        }, "");
    }

    var template = "<label>{label}</label><select>{options}</select><br>",
        content = template.replace("{label}", name)
            .replace("{options}", buildOptions(options));

    var $control = $(content);
    $control.insertBefore($("#expButton"));
    //return select
    return $($control[1]);
}
function renderInputControls(data) {
    var productFamilyInputControl = _.findWhere(data, { id: "Product_Family" });
    var select = $("#productFamilySelector");
    _.each(productFamilyInputControl.state.options, function (option) {
        select.append("<option " + (option.selected ? "selected" : "")
                      + " value='" + option.value + "'>" + option.label + "</option>");
    });
}

function getInputControlTemplate(control) {
    var result = null;
    switch (control.type) {
        case 'singleSelect':
            result = "<select id=\"" + control.id + "\" class=\"input_control\" name=\"" + control.id + "\">{options}</select>";
            break;
        case 'multiSelect':
            result = "<select id=\"" + control.id + "\" class=\"input_control\" name=\"" + control.id + "\" multiple>{options}</select>";            
  
            break;
        case 'singleSelectRadio':
            for (var i = 0; i < control.state.options.length; i++)
            {
                result += "<input type=\"radio\" id=\"" + control.id + "\" class=\"input_control\" name=\"" + control.id + "\" value=\"" + control.state.options[i].value + "\" />" + control.state.options[i].label + "<br />";
            }
            break;
			        
    }
    return result;
    
}

function runDashboardWithParams(inputId)
{
    if (dashboard == null) return;
    var myid = $('#' + inputId).attr('id');
    var v = [];
    var oval = $.trim($('#' + inputId).val());
    if (oval != null && oval != "")
    {
        var vals = oval.split(",");
        for (var i = 0; i < vals.length; i++) {
            v.push($.trim(vals[i]));
        }

        dashboardData[myid] = v;
    }
    else
    {
        delete dashboardData[myid];
    }
   
    console.log(dashboardData);
    dashboard.params(dashboardData)
        .run();
}

function createReportInView(uri, viewSelect)
{
    var uriTemplate = SERVER_URL + '/flow.html?_flowId=viewReportFlow&standAlone=true&reportUnit=[addressuri]&decorate=no';
    if (viewSelect != null)
    {
        uriTemplate = SERVER_URL + '/flow.html?_flowId=' + viewSelect + '&standAlone=true&resource=[addressuri]&decorate=no&viewReport=true';
    }
    var url = uriTemplate.replace("[addressuri]", uri);
    console.log(url);
    var $iframe = $('#reportView');
    if ($iframe.length) {
        $iframe.attr('src', url);
        return false;
    }
    return true;
}

function createDashboardInView(uri) {
    var uriTemplate = SERVER_URL + '/flow.html?_flowId=dashboardRuntimeFlow&dashboardResource=[addressuri]&decorate=no';
    //if (viewSelect != null) {
    //    uriTemplate = SERVER_URL + '/flow.html?_flowId=' + viewSelect + '&standAlone=true&resource=[addressuri]&decorate=no&viewReport=true';
    //}
    var url = uriTemplate.replace("[addressuri]", encodeURIComponent(uri));
    console.log(url);
    var $iframe = $('#reportView');
    if ($iframe.length) {
        $iframe.attr('src', url);
        return false;
    }
    return true;
}