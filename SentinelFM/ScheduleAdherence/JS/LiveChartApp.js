
var saLiveChartApp = angular.module('saLiveChartApp', ['ui.bootstrap', 'googlechart'])
    .service('saService', function ($rootScope, $http) {
        var StartDate = null;
        var EndDate = null;
        var Depots = null;
        var Stations = null;
        var Vehicles = null;
        var ReasonCodes = null;
        var RouteName = null;
        var Vehicle = null;
        var Station = null;
        var ViewParam = null;
        var DeliveryStatus = [{ Id: 1, Name: "Late" }, { Id: 2, Name: "Early" }, { Id: 3, Name: "On Time" }, { Id: 4, Name: "Not yet delivery"}];
        $http.post('ScheduleData.ashx?Req=GetVehicles').success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            Vehicles = data.Object;
            $rootScope.$broadcast('GetVehicles', Vehicles);
        });
        $http.post('ScheduleData.ashx?Req=GetStationList').success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            Stations = data.Object;
            $rootScope.$broadcast('GetStations', Stations);
        });
        $http.post('ScheduleData.ashx?Req=GetDeptList').success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            Depots = data.Object;
            $rootScope.$broadcast('GetDepots', Depots);
        });
        $http.post('ScheduleData.ashx?Req=GetReasonList').success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            ReasonCodes = data.Object;
            $rootScope.$broadcast('GetReasons', ReasonCodes);
        });
        return {
            SetStartDate: function (date) {
                StartDate = date;
            },
            SetEndDate: function (date) {
                EndDate = date;
            },
            SetRouteName: function (routeName) {
                RouteName = routeName;
            },
            GetVehicleList: function () {
                return Vehicles;
            },
            GetVehicleById: function (vehicleId) {
                if (vehicleId == null) return null;
                var vehicle = null;
                angular.forEach(Vehicles, function (item) {
                    if (item.VehicleId == vehicleId)
                        vehicle = item;
                });
                return vehicle;
            },
            SetVehicle: function (_vehicle) {
                Vehicle = _vehicle;
            },
            GetDepotList: function () {
                return Depots;
            },
            GetDepotById: function (depotId) {
                if (depotId == null) return null;
                var depot = null;
                angular.forEach(Depots, function (item) {
                    if (item.StationId == depotId)
                        depot = item;
                });
                return depot;
            },
            GetStationList: function () {
                return Stations;
            },
            GetStationById: function (stationId) {
                if (stationId == null) return null;
                var station = null;
                angular.forEach(Stations, function (item) {
                    if (item.StationId == stationId)
                        station = item;
                });
                return station;
            },
            SetStation: function (station) {
                Station = station;
            },
            GetReasonCodeList: function () {
                return ReasonCodes;
            },
            GetReasonCodeById: function (id) {
                if (id == null) return null;
                var code = null;
                angular.forEach(ReasonCodes, function (item) {
                    if (item.ReasonCodeId == id)
                        code = item;
                });
                return code;
            },
            GetDeliveryStatusList: function () {
                return DeliveryStatus;
            },
            GetDeliveryStatusById: function (id) {
                if (id == null) return null;
                var status = null;
                angular.forEach(DeliveryStatus, function (item) {
                    if (item.Id == id)
                        status = item;
                });
                return status;
            },
            ViewData: function (_viewParam) {
                ViewParam = _viewParam;
            },
            GetViewParam: function () {
                return ViewParam;
            },
            GetDataURL: function (command) {
                if (StartDate == null || EndDate == null) return null;
                var url = "ScheduleData.ashx?Req=GetLiveChartReport&Command=" + command;
                url += "&StartDate=" + StartDate.toDateString();
                url += "&EndDate=" + EndDate.toDateString();
                if (RouteName != null)
                    url += "&RouteName=" + RouteName;
                if (Vehicle != null)
                    url += "&VehicleId=" + Vehicle.VehicleId;
                if (Station != null)
                    url += "&StationId=" + Station.StationId;
                return url;
            },
            AlertMsg: function (msg) {
                alert(msg);
            },
            ConfirmMsg: function (msg) {
                return window.confirm(msg);
            }
        };
    })
    .filter('ListOpenIcon', function () {
        return function (input) {
            return input ? "img/minus.gif" : "img/plus.gif";
        };
    })
    .directive('ngBlur', function () {
        return function (scope, elem, attrs) {
            elem.bind('blur', function () {
                scope.$apply(attrs.ngBlur);
            });
        };
    });

var SAApp_AjaxBaseURL = "ScheduleData.ashx";
function parseDate(input) {
    if (angular.isDate(input)) return input;
    var parts = input.split('T');
    var date = parts[0];
    var dateParts = date.split('-');
    if (parts.length == 1) {
        return new Date(dateParts[0], dateParts[1] - 1, dateParts[2]);
    }
    else {
        var time = parts[1];
        var timeParts = time.split(':');
        return new Date(dateParts[0], dateParts[1] - 1, dateParts[2], timeParts[0], timeParts[1], timeParts[2]);
    }
}


