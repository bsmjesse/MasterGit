var BSM = BSM || {
    SERVER_URL: "",
    token: null,
    currentUrl: null,
    report: null,
    dashboard: null,
    controlId: null,
    biPublicDashboard: "/public/BSM_Public/Dashboards",
    biPublicReports: "/public/BSM_Public/Standard_Reports",
    biPublicAdHoc: "/public/BSM_Public/Standard_Views",
    biOrganizationDashboard: "/Dashboards",
    biOrganizationReports: "/organizations",
    biDemo: "/public/Samples/Reports",
    dashboardData: {},
    dashboardParams: new Array(),
    drivers: [],
    fleets_selected: [],

    disableEnterKey: function(e){
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = e.which; //firefox      

        return (key != 13);
    },



    ranges: {
        'Today': [moment(), moment()],
        'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
        'Last 7 Days': [moment().subtract(6, 'days'), moment()],
        'Last 30 Days': [moment().subtract(29, 'days'), moment()],
        'This Month': [moment().startOf('month'), moment().endOf('month')],
        'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
    }
};