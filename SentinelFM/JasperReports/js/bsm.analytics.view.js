var AnalyticsView = function() {

    $("#selected_resource_dashboard").change(function () {
        $("#divContainer").html("");
        BSM.dashboardData = {};
        if ($("#selected_resource_dashboard").val() != "") {
            BSM.currentUrl = $("#selected_resource_dashboard").val();
            $("#productFamilySelector").html("");
            if (BSM.currentUrl.indexOf("Dashboards") > -1) {
                BSM.report = null;
                BSM.jasper.createDashboard(currentUrl);
                $('#divReportView').hide();
                $('#divContainer').show();

            }
            else if (BSM.currentUrl.indexOf("Standard_Views") > -1) {
                BSM.jasper.createReportInView(currentUrl, "adhocFlow");
                $('#divReportView').show();
                $('#divContainer').hide();
            }
            else {
                BSM.jasper.createReportInView(currentUrl, null);
                $('#divReportView').show();
                $('#divContainer').hide();
            }


        }

    });

    $("#productFamilySelector").on("change", ".input_control", function () {
        if (report == null) return;
        var data = {};
        var myid = $(this).attr('id');
        var v = [];
        var vals = $.trim($(this).val()).split(",");
        for (var i = 0; i < vals.length; i++) {
            v.push($.trim(vals[i]));
        }

        data[myid] = v;
        BSM.report.params(data)
            .run();

    });


    $("#pageRange").change(function () {
        if (BSM.report == null) return;
        BSM.report
            .pages($(this).val())
            .run()
            .fail(function (err) {
                alert(err);
            });
    });

    $("#previousPage").click(function () {
        if (BSM.report == null) return;
        var currentPage = BSM.report.pages() || 1;

        BSM.report
            .pages(--currentPage)
            .run()
            .fail(function (err) {
                alert(err);
            });
    });

    $("#nextPage").click(function () {
        if (BSM.report == null) return;
        var currentPage = BSM.report.pages() || 1;

        BSM.report
            .pages(++currentPage)
            .run()
            .fail(function (err) {
                alert(err);
            });
    });

    $('#expButton').click(function () {
        if (BSM.report == null) return;
        console.log($select.val());
        if ($("#pageRange").val() != "" && $("#pageRange").val() != "0") {
            BSM.report.export({
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
        else {
            BSM.report.export({
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
}();