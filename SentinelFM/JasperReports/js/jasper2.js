var BSM = BSM || {};

BSM.jasper = function() {

    function _buildReportInput() {
         BSM.VisualizeClient.inputControls({
            resource: BSM.currentUrl,
            success: function (controls) {
                controls.forEach(buildControl);
            },
            error: function (err) {
                alert(err);
            }
        });
    }

    function _createReport(uri, v) {
        $("#divContainer").html("");
        BSM.report = BSM.VisualizeClient.report({
            resource: uri,
            error: function (err) {
                alert(err.message);
            },
            container: "#divContainer"
        });
    }

    function _createView(uri, v) {
        $("#divContainer").html("");
        BSM.report = BSM.VisualizeClient.report({
            resource: uri,
            error: function (err) {
                alert(err.message);
            },
            container: "#divContainer"
        });
    }

    function _createDashboard(uri, v) {
        $("#divContainer").html("");
        BSM.dashboard = BSM.VisualizeClient.dashboard({
            resource: uri,
            error: function (err) {
                alert(err.message);
            },
            success: function () {
                _buildDashboardControl();
               
            },
            container: "#divContainer"
        });
    }

    function renderResults(uriName, results, controlId) {
        $('#' + controlId).append("<option>" + uriName + "</option>");
        if (results == null) return;
        for (var i = 0; i < results.length; i++) {
            $('#' + controlId).append("<option value=\"" + results[i].uri + "\" >- - - " + results[i].label + "</option>");
        }

    }

    function _buildControl(control) {

        function _buildOptions(options) {
            if (options == undefined || options.length < 1) return '';
            var template = "<option>{value}</option>";
            return options.reduce(function (memo, option) {
                return memo + template.replace("{value}", option.value);
            }, "");
        }

        if (control.state.options == undefined) return;
        console.log(control);
        BSM.controlId = control.id;
        var templateControl = _getInputControlTemplate(control);
        var template = "<label>{label}</label>" + templateControl + "</select><br>";
        var content = template.replace("{label}", control.label)
            .replace("{options}", buildOptions(control.state.options));
        if ($.trim($("productFamilySelector").html()) == '') {
            $("#productFamilySelector").append($(content));
        }


    }

    function _buildDashboardControl() {
        var params = BSM.dashboard.data().parameters;
        console.log(params);
        BSM.dashboardParams = new Array();
        for (var i = params.length - 1; i >= 0; i--) {
            if (params[i].id.indexOf(":") > -1) continue;
            if (params[i].id === "EndDate") continue;
            var _class = getClassById(params[i].id);
            var $el = $('<div class="form-group ' + _class + '"><label for="' + params[i].id + '">' + S(params[i].id).humanize() + '</label> <input type="text" id="' + params[i].id + '" class="form-control" /></div>"');
            BSM.dashboardParams.push(params[i].id);
            $("#productFamilySelector").prepend($el);
        }
        BSM.Controls.scanControls();
        BSM.Controls.removeLeft();
        $('#Title').parent().hide();
        $('#Vehicle_Selector').parent().hide();
    }

    function getClassById(id) {
        var _class = "";
        if (id === 'StartDate' || id === 'StartDate_2') {
            _class = 'bsm_date';
        }
        else if (id === 'Select_Days_of_Week' || id === 'Fleet_Selector' || id === 'Driver_Select' || id === 'Driver_Select_2') {
            _class = 'bsm_multipleselect';
        }
        else if (id === 'Infraction_Category') {
            _class = 'bsm_singleselect';
        }
        return _class;
    }


    function _buildExportControl(name, options) {

        function buildOptions(options) {
            var template = "<option>{value}</option>";
            return options.reduce(function (memo, option) {
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

    function _renderInputControls(data) {
        var productFamilyInputControl = _.findWhere(data, {id: "Product_Family"});
        var select = $("#productFamilySelector");
        _.each(productFamilyInputControl.state.options, function (option) {
            select.append("<option " + (option.selected ? "selected" : "")
                + " value='" + option.value + "'>" + option.label + "</option>");
        });
    }

    function _getInputControlTemplate(control) {
        var result = null;
        switch (control.type) {
            case 'singleSelect':
                result = "<select id=\"" + control.id + "\" class=\"input_control\" name=\"" + control.id + "\">{options}</select>";
                break;
            case 'multiSelect':
                result = "<select id=\"" + control.id + "\" class=\"input_control\" name=\"" + control.id + "\" multiple>{options}</select>";

                break;
            case 'singleSelectRadio':
                for (var i = 0; i < control.state.options.length; i++) {
                    result += "<input type=\"radio\" id=\"" + control.id + "\" class=\"input_control\" name=\"" + control.id + "\" value=\"" + control.state.options[i].value + "\" />" + control.state.options[i].label + "<br />";
                }
                break;

        }
        return result;

    }

    function _splitDateRangeStringToMoment(dateRange){
        var ranges = dateRange.split('-');
        BSM.dashboardData['StartDate'] = [moment(ranges[0]).format()];
        BSM.dashboardData['EndDate'] = [moment(ranges[1]).format()];
    }

    function _runDashboardWithAllParams() {
        if (BSM.dashboard == null) return;
        BSM.dashboardParams.forEach(function (inputId) {
            var id = $('#' + inputId).attr('id');
            var dashboardData = [];
            var inputControlValue = $.trim($('#' + inputId).val());
            if ( id === 'StartDate'){
                _splitDateRangeStringToMoment(inputControlValue);
            }
            else {
                if (inputControlValue != null && inputControlValue != "") {
                    var vals = inputControlValue.split(",");
                    for (var i = 0; i < vals.length; i++) {
                        var _v = vals[i];
                        if ($('#' + inputId).parent().hasClass('bsm_date')) {
                            _v = moment(_v).format();
                        }
                        dashboardData.push($.trim(_v));
                    }

                    BSM.dashboardData[id] = dashboardData;
                }
                else {
                    delete BSM.dashboardData[id];
                }
            }
        });


        console.log(BSM.dashboardData);
        BSM.dashboard.params(BSM.dashboardData)
            .run();
    }

    function _createReportInView(uri, viewSelect) {
        var uriTemplate = BSM.SERVER_URL + '/flow.html?_flowId=viewReportFlow&standAlone=true&reportUnit=[addressuri]&decorate=no';
        if (viewSelect != null) {
            uriTemplate = BSM.SERVER_URL + '/flow.html?_flowId=' + viewSelect + '&standAlone=true&resource=[addressuri]&decorate=no&viewReport=true';
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

    function _createDashboardInView(uri) {
        var uriTemplate = BSM.SERVER_URL + '/flow.html?_flowId=dashboardRuntimeFlow&dashboardResource=[addressuri]&decorate=no';
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

    function _renderBootstrapResults(results, controlId) {
        if (results == null) return;
        for (var i = 0; i < results.length; i++) {
            var linkStr = '<tr><td><a href="#" onclick="BSM.jasper.displayBootstrapReport(\'' + results[i].uri + '\', \'' + results[i].label + '\')">' + results[i].label + '</a></td></tr>';
            $('#' + controlId).find("tbody").append(linkStr);
        }
    }

    function _displayBootstrapReport(currentUrl, myLabel) {
        if (currentUrl != "" && currentUrl != null) {
            $('#filter-title').html(myLabel);
            $("#productFamilySelector").html("");
            if (currentUrl.indexOf("Dashboards") > -1) {
                BSM.report = null;
                _createDashboard(currentUrl);
                $('#divReportView').hide();
                $('#divContainer').show();
                $('#btnFilter').show();

            }
            else if (currentUrl.indexOf("Standard_Views") > -1) {
                _createReportInView(currentUrl, "adhocFlow");
                $('#divReportView').show();
                $('#divContainer').hide();
                $('#btnFilter').hide();
            }
            else {
                _createReportInView(currentUrl, null);
                $('#divReportView').show();
                $('#divContainer').hide();
                $('#btnFilter').hide();
            }


        }
    }

    return {
        runDashboardWithAllParams: _runDashboardWithAllParams,
        buildExportControl: _buildExportControl,
        createReportInView: _createReportInView,
        renderBootstrapResults: _renderBootstrapResults,
        displayBootstrapReport: _displayBootstrapReport,
        buildDashboardControl: _buildDashboardControl,
        buildControl: _buildControl,
        buildReportInput: _buildReportInput,
        createReport: _createReport,
        createView: _createView,
        createDashboard: _createDashboard
    }
}();