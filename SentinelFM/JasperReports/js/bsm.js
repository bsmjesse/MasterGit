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
    fleets_selected: []
};