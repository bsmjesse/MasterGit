function LiveChartCtrl($scope, $rootScope, $http, $timeout, $log, saService) {
    $scope.InitController = function () {
        $scope.ReasonList = null;
        $scope.StartDate = null;
        $scope.EndDate = null;
        $scope.StartOpened = false;
        $scope.EndOpened = false;

        $scope.VehicleList = [];
        $scope.StationList = [];
        $scope.DepotList = [];
        $scope.ViewList = [];
        $scope.InitSearchDate();
        $scope.OrginalDataList = null;
        $scope.Search = { RouteName: null, Vehicle: null, Station: null, VehicleName: '', StationName: '' }
        $scope.BuffSetting = { DepotLateArrival: 60, DeportLateDeparture: 30, DeportEarlyArrival: 60, DeportEarlyDeparture: 30,
            StationLateArrival: 60, StationLateDeparture: 60, StationEarlyArrival: 60, StationEarlyDeparture: 60
        }
    };

    $scope.$on('GetVehicles', function (event, vehicles) {
        $scope.VehicleList = [];
        angular.forEach(vehicles, function (row) {
            $scope.VehicleList.push(row);
        });
    });

    $scope.$on('GetStations', function (event, stations) {
        $scope.StationList = [];
        angular.forEach(stations, function (row) {
            $scope.StationList.push(row);
        });
    });

    $scope.$on('GetDepots', function (event, depots) {
        $scope.DepotList = [];
        angular.forEach(depots, function (row) {
            $scope.DepotList.push(row);
        });
    });

    $scope.$watch('StartDate', function () {
        saService.SetStartDate($scope.StartDate);
    });

    $scope.$watch('EndDate', function () {
        saService.SetEndDate($scope.EndDate);
    });

    $scope.$watch('Search.RouteName', function () {
        saService.SetRouteName($scope.Search.RouteName);
    });

    $scope.$watch('Search.Vehicle', function () {
        saService.SetVehicle($scope.Search.Vehicle);
    });

    $scope.$watch('Search.SetStation', function () {
        saService.SetVehicle($scope.Search.Station);
    });

    $scope.VehicleCompare = function (v) {
        if ($scope.Search.VehicleName == '') return true;
        return v.Description.toLowerCase().indexOf($scope.Search.VehicleName) >= 0;
    };

    $scope.StationCompare = function (s) {
        if ($scope.Search.StationName == '') return true;
        return s.Name.toLowerCase().indexOf($scope.Search.StationName) >= 0;
    };

    $scope.InitSearchDate = function () {
        var today = new Date();
        var startDate = new Date(today);
        startDate.setDate(today.getDate() - 30);
        $scope.StartDate = startDate;
        $scope.EndDate = new Date(today);
    };

    $scope.SelectSearchStation = function (station) {
        $scope.SearchStation.StationName = station.Name;
    };

    $scope.OpenStartDate = function () {
        $timeout(function () {
            $scope.StartOpened = true;
        });
    };

    $scope.OpenEndDate = function () {
        $timeout(function () {
            $scope.EndOpened = true;
        });
    };

    $scope.CanSearch = function () {
        if ($scope.StartDate == null || $scope.EndDate == null || $scope.StartDate > $scope.EndDate) return false;
        return true;
    };

    function InitView() {
        return {Title:'', URL:'', ViewId:'-1', active:true}
    };

    function GetViewByViewId(viewId) {
        var ret = null;
        angular.forEach($scope.ViewList, function (view) {
            if (view.ViewId == viewId)
                ret = view;
        });
        return ret;
    };

    $scope.AddView = function (summaryType, itemType, depotId) {
        var date = new Date();
        var tt = date.getTime();

        var view = InitView();
        var viewParam = null;
        if (summaryType == 1) {
            switch (itemType) {
                case 1: //Station Arrival
                    view.Title = "Station Arrival";
                    break;
                case 2: //Station Departure
                    view.Title = "Station Departure";
                    break;
                case 3: //Depot Arrival
                    view.Title = "Depot Arrival";
                    break;
                case 4: //Depot Departure
                    view.Title = "Depot Departure";
                    break;
            }
            if (depotId == 0)
                view.Title += ":All";
            else {
                var depot = saService.GetDepotById(depotId);
                view.Title += ":" + depot.Name;
            }
            view.URL = "Partials/DepotSummary.htm?tt=" + tt;
            view.ViewId = summaryType + "_" + itemType + "_" + depotId;
            viewParam = { DepotId: depotId };
        }
        else if (summaryType = 2) { //Summary by Station
            view.Title = "Summary By Station";
            view.URL = "Partials/StationSummary.htm?tt=" + tt;
            view.ViewId = summaryType + "_";
        }
        else {
            view.Title = "Summary By Vehicle";
            view.URL = "Partials/VehicleSummary.htm?tt=" + tt;
            view.ViewId = summaryType + "_";
        }

        var activeView = GetViewByViewId(view.ViewId);
        if (activeView != null)
            activeView.active = true;
        else {
            saService.ViewData(viewParam);
            $scope.ViewList.push(view);
        }
    };

    $scope.View = function () {
        saService.ViewData();
    };

    $scope.InitController();
};

var DepotSummaryCtrl = function ($scope, $http, saService) {
    $scope.InitController = function () {
        $scope.ChartData = [];
        $scope.dataItem = null;
        $scope.Depots = [];
        InitChart();
    };

    $scope.InitData = function () {
        $scope.InitController();
        var viewParam = saService.GetViewParam();
        ViewData(viewParam);
    };

    $scope.$on('ViewData', function (event) {
        // $scope.InitData();
    });

    function ViewData(viewParam) {
        var url = saService.GetDataURL('StArrival'); +
        $http.post(url).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            var db = TAFFY(data.Object);
            if (viewParam.DepotId == 0) {
                $scope.ChartData.data.rows[1].c[1].v = db({ StatusId: 1 }).sum("StatusCount"); // late
                $scope.ChartData.data.rows[2].c[1].v = db({ StatusId: 2 }).sum("StatusCount"); // Early
                $scope.ChartData.data.rows[0].c[1].v = db({ StatusId: 3 }).sum("StatusCount"); // On Time
                $scope.ChartData.data.rows[3].c[1].v = db({ StatusId: 4 }).sum("StatusCount") + db({ StatusId: { isNull: true} }).sum("StatusCount"); // Not yet delivery
            }
            else{
                $scope.ChartData.data.rows[1].c[1].v = db({ DepotId: viewParam.DepotId, StatusId: 1 }).sum("StatusCount"); // late
                $scope.ChartData.data.rows[2].c[1].v = db({ DepotId: viewParam.DepotId, StatusId: 2 }).sum("StatusCount"); // Early
                $scope.ChartData.data.rows[0].c[1].v = db({ DepotId: viewParam.DepotId, StatusId: 3 }).sum("StatusCount"); // On Time
                $scope.ChartData.data.rows[3].c[1].v = db({ DepotId: viewParam.DepotId, StatusId: 4 }).sum("StatusCount") + db({ DepotId: depot.StationId, StatusId: { isNull: true} }).sum("StatusCount"); // Not yet delivery
            };
        });
    };

    function InitChartDataObject() {
        var chartObject = {};
        chartObject.type = "PieChart";
        chartObject.displayed = false;
        chartObject.cssStyle = "height:300px; width:30%;";

        chartObject.data = {
            "cols": [
            { id: "status", label: "Delivery Status", type: "string" },
            { id: "count", label: "Count", type: "number" },
        ],
            "rows": [
            { c: [
                { v: "On Time" },
                { v: 0 }
            ]
            },
            { c: [
                { v: "Late" },
                { v: 2 }
            ]
            },
            { c: [
                { v: "Early" },
                { v: 3 }
            ]
            },
            { c: [
                { v: "Not yet delivery" },
                { v: 4 }
            ]
            }
        ]
        };

        chartObject.options = {
            "title": "",
            "isStacked": "true",
            "fill": 20,
            "displayExactValues": true
        };
        return chartObject;
    };

    function InitChart() {
        var DeliveryStatus = [{ Id: 1, Name: "Late" }, { Id: 2, Name: "Early" }, { Id: 3, Name: "On Time" }, { Id: 4, Name: "Not yet delivery"}];
        var charData = [];

        //dataItem
        $scope.dataItem = InitChartDataObject();

        //All Data
        var allChartObj = InitChartDataObject();
        allChartObj.options.title = "All";
        charData.push(allChartObj);
        $scope.Depots = saService.GetDepotList();
        angular.forEach($scope.Depots, function (depot) {
            var chartObj = InitChartDataObject();
            chartObj.options.title = depot.Name;
            charData.push(chartObj);
        });
        $scope.ChartData = charData;
    }

    $scope.chartReady = function () {
        fixGoogleChartsBarsBootstrap();
    }

    $scope.alertMe = function (str) {
        setTimeout(function () {
            alert("You've selected the alert tab!" + str);
        });
    };

    function fixGoogleChartsBarsBootstrap() {
        // Google charts uses <img height="12px">, which is incompatible with Twitter
        // * bootstrap in responsive mode, which inserts a css rule for: img { height: auto; }.
        // *
        // * The fix is to use inline style width attributes, ie <img style="height: 12px;">.
        // * BUT we can't change the way Google Charts renders its bars. Nor can we change
        // * the Twitter bootstrap CSS and remain future proof.
        // *
        // * Instead, this function can be called after a Google charts render to "fix" the
        // * issue by setting the style attributes dynamically.

        $(".google-visualization-table-table img[width]").each(function (index, img) {
            $(img).css("width", $(img).attr("width")).css("height", $(img).attr("height"));
        });
    };
    $scope.InitController();
};

var StationSummaryCtrl = function ($scope, $http, saService) {

    $scope.InitController = function () {
        $scope.OrgGroup = null;
        $scope.ScheduleBeginDate = null;
        $scope.minDate = new Date();
    };

    $scope.InitData = function (group) {
        $scope.InitController();
    };

    $scope.$on('CopyGroupChanged', function (event, group) {
        $scope.InitData(group);
    });

    $scope.InitController();
};

var VehicleSummaryCtrl = function ($scope, $http, saService) {

    $scope.InitController = function () {
        $scope.OrgGroup = null;
        $scope.ScheduleBeginDate = null;
        $scope.minDate = new Date();
    };

    $scope.InitData = function (group) {
        $scope.InitController();
    };

    $scope.$on('CopyGroupChanged', function (event, group) {
        $scope.InitData(group);
    });

    $scope.InitController();
};
