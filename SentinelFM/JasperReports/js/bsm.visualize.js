var BSM = BSM || {};

BSM.Visualize = function ()
{
    function _runPageSpecific(windowObj, initializeFunction) {
        $(windowObj).load(function () {
            visualize({
                server: BSM.SERVER_URL,
                auth: {
                    token: "bsm-" + BSM.token,
                    loginFn: function (properties, request) {

                        return request({
                            url: BSM.SERVER_URL + "/flow.html?_flowId=homeFlow&token=bsm-" + BSM.token
                        });
                    }


                }
            }, function (v) {

                //'v' it's a client to JRS instance under the config set by visualize.config();
                BSM.VisualizeClient = v;
                initializeFunction();
            });
        });
    }

    function _menuLinks(v) {
        v = BSM.VisualizeClient;
        if (BSM.biDemo.length > 0) {
            v.resourcesSearch({
                folderUri: BSM.biDemo,
                recursive: true,
                success: function (results) {
                    BSM.jasper.renderBootstrapResults(results, "tblDemos");
                },
                error: function (err) {
                }
            });
        }
        var reportExports = v.report
            .exportFormats
            .concat(["json"]);
        $select = BSM.jasper.buildExportControl("Export to: ", reportExports);
        v.resourcesSearch({
            folderUri: BSM.biPublicDashboard,
            recursive: true,
            success: function (results) {
                BSM.jasper.renderBootstrapResults(results, "tblDashboards");
            },
            error: function (err) {
            }
        });
        v.resourcesSearch({
            folderUri: BSM.biPublicReports,
            recursive: true,
            success: function (results) {
                BSM.jasper.renderBootstrapResults(results, "tblReports");
            },
            error: function (err) {
            }
        });
        v.resourcesSearch({
            folderUri: BSM.biPublicAdHoc,
            recursive: true,
            success: function (results) {
                BSM.jasper.renderBootstrapResults(results, "tblViews");
            },
            error: function (err) {
            }
        });

        $(':disabled').prop('disabled', false);

    }

    function _loadMenu(windowObj) {
        _runPageSpecific(windowObj, _menuLinks);
    }


    function _runEverything() {
        visualize(
            {
                server: BSM.SERVER_URL,
                auth: {
                    token: "bsm-" + BSM.token,
                    loginFn: function (properties, request) {

                        return request({
                            url: BSM.SERVER_URL + "/flow.html?_flowId=homeFlow&token=bsm-" + BSM.token
                        });
                    }


                }
            },

            function (v) {
                globalV = v;


                if (biDemo.length > 0) {
                    v.resourcesSearch({
                        folderUri: BSM.biDemo,
                        recursive: true,
                        success: function (results) {
                            BSM.jasper.renderBootstrapResults(results, "tblDemos");
                        },
                        error: function (err) {
                        }
                    });
                }
                var reportExports = v.report
                    .exportFormats
                    .concat(["json"]);
                $select = BSM.jasper.buildExportControl("Export to: ", reportExports);
                v.resourcesSearch({
                    folderUri: BSM.biPublicDashboard,
                    recursive: true,
                    success: function (results) {
                        BSM.jasper.renderBootstrapResults(results, "tblDashboards");
                    },
                    error: function (err) {
                    }
                });
                v.resourcesSearch({
                    folderUri: BSM.biPublicReports,
                    recursive: true,
                    success: function (results) {
                        BSM.jasper.renderBootstrapResults(results, "tblReports");
                    },
                    error: function (err) {
                    }
                });
                v.resourcesSearch({
                    folderUri: BSM.biPublicAdHoc,
                    recursive: true,
                    success: function (results) {
                        BSM.jasper.renderBootstrapResults(results, "tblViews");
                    },
                    error: function (err) {
                    }
                });

                $(':disabled').prop('disabled', false);

            }, function (err) {
                alert(err.message);
            });
    }



    return{
        runEverything: _runEverything,
        runPageSpecific: _runPageSpecific,
        loadMenu: _loadMenu
    }
}();