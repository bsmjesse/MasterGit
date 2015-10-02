var SARouteCtrl = function ($scope, $modal, $http, saService) {

    $scope.InitController = function () {
        $scope.SASchedule = null;
        $scope.RouteList = null;
        $scope.Status = "Init";
    };

    $scope.InitData = function (schedule) {
        $scope.InitController();
        $scope.SASchedule = schedule;
        saService.SetRoute(null);
        if (schedule != null) {
            $scope.GetRouteListByScheduleId(schedule.ScheduleId);
        }
    };

    $scope.$on('SAScheduleChanged', function (event, schedule) {
        $scope.InitData(schedule);
    });

    $scope.GetRouteListByScheduleId = function (scheduleId) {
        $scope.Status = "Loading";
        $http.post('ScheduleData.ashx?Req=GetRouteList&ScheduleId=' + scheduleId).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) {
                $scope.Status = "NoData";
                return;
            }
            $scope.Status = "Data";
            var baseDate = $scope.SASchedule.ScheduleBeginDate;
            angular.forEach(data.Object, function (row) {
                row.RSCDepartureTime = null;
                if (row.RSCDepartureTime_sec != null) {
                    row.RSCDepartureTime = new Date(baseDate);
                    row.RSCDepartureTime.setSeconds(row.RSCDepartureTime_sec);
                }
                row.RSCArrivalTime = null;
                if (row.RSCArrivalTime_sec != null) {
                    row.RSCArrivalTime = new Date(baseDate);
                    row.RSCArrivalTime.setSeconds(row.RSCArrivalTime_sec);
                }
                row.VehicleName = null;
                row.Tag = row.Name;
                var vehicle = saService.GetVehicleById(row.VehicleId);
                if (vehicle != null) {
                    row.VehicleName = vehicle.Description;
                    row.Tag += " " + vehicle.Description;
                }
            });
            $scope.RouteList = data.Object;
        });
    }

    $scope.GetRouteById = function (routeId) {
        var route = null;
        if (routeId != 0)
            angular.forEach($scope.RouteList, function (item) {
                if (item.RouteId == routeId)
                    route = item;
            });
        return route;
    };
    $scope.CanNewRoute = function () {
        var schedule = saService.GetCurSchedule();
        if (schedule == null) return false;
        var today = new Date();
        return schedule.ScheduleBeginDate > today;
    };
    $scope.CanEditRoute = function (routeId) {
        var route = typeof routeId !== 'undefined' ? $scope.GetRouteById(routeId) : saService.GetCurRoute();
        if (route == null) return false;
        var today = new Date();
        return route.RSCDepartureTime > today;
    };
    $scope.CanDeleteRoute = function (routeId) {
        var route = typeof routeId !== 'undefined' ? $scope.GetRouteById(routeId) : saService.GetCurRoute();
        if (route == null) return false;
        var today = new Date();
        return route.RSCDepartureTime > today;
    };

    $scope.IsRouteInit = function () {
        return $scope.Status == "Init";
    };
    $scope.IsRouteNoData = function () {
        return $scope.Status == "NoData";
    };
    $scope.IsRouteLoading = function () {
        return $scope.Status == "Loading";
    };
    $scope.HaveRouteData = function () {
        return $scope.Status == "Data";
    };

    $scope.IsCurrentRoute = function (routeId) {
        var route = saService.GetCurRoute();
        if (route == null) return false;
        return route.RouteId == routeId;
    };

    $scope.IsUncompletedRoute = function (routeId) {
        var route = $scope.GetRouteById(routeId);
        if (route == null) return false;
        return route.RSCDepartureTime == null || route.VehicleName == null || route.RSCArrivalTime == null;
    };

    $scope.SelectRoute = function (routeId) {
        var route = $scope.GetRouteById(routeId);
        saService.SetRoute(route);
    };

    $scope.EditRoute = function (routeId) {
        var route = typeof routeId !== 'undefined' ? $scope.GetRouteById(routeId) : saService.GetCurRoute();
        saService.SetEditRoute(route);
    };

    $scope.DeleteRoute = function (routeId) {
        var route = typeof routeId !== 'undefined' ? $scope.GetRouteById(routeId) : saService.GetCurRoute();
        if (route == null) return;
        if (!saService.ConfirmMsg("Click Ok to delete this route.")) return;
        var url = SAApp_AjaxBaseURL + "?Req=DeleteRoute&RouteId=" + route.RouteId;
        $http.post(url).success(function (data) {
            if (saService.GetCurRoute() == route) {
                saService.SetRoute(null);
            }
            saService.RefreshRouteList();
        });
    }

    $scope.InitController();
};

var RouteInstanceCtrl = function ($scope, $modal, $http, saService) {
    var routeStationList = new Array();
    $scope.InitController = function () {
        $scope.Route = { RouteId: 0, RSCDepartureTime: new Date(), RSCArrivalTime: new Date(), Description: '' };
        $scope.Schedule = null;
        $scope.Vehicles = null;
        $scope.Vehicle = null;
        $scope.DepartDayInterval = 0;
        $scope.ArrivalDayInterval = 2;
        $scope.DayIntervals = [0, 1, 2, 3, 4, 5, 6];
        $scope.Status = "Init";
    };

    $scope.InitData = function (route) {
        $scope.InitController();
        $scope.Vehicles = saService.GetVehicleList();
        var schedule = saService.GetCurSchedule();
        $scope.Route = { RouteId: 0, RSCDepartureTime: new Date(schedule.ScheduleBeginDate), DepartDayInterval: 0,
            RSCArrivalTime: new Date(schedule.ScheduleBeginDate), ArrivalDayInterval: 0, Description: ''
        };
        $scope.Schedule = schedule;
        routeStationList = new Array();
        if (route != null) {
            $scope.GetRouteById(route.RouteId);
            GetStationListByRouteId(route.RouteId);
        }
        else
            $scope.Status = "Loaded";
    };

    $scope.$on('EditRouteChanged', function (event, route) {
        $scope.InitData(route);
    });

    function GetStationListByRouteId(routeId) {
        routeStationList = new Array();
        $http.post('ScheduleData.ashx?Req=GetRouteStationList&RouteId=' + routeId).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) {
                return;
            }
            routeStationList = data.Object;
        });
    };


    $scope.GetRouteById = function (routeId) {
        $http.post('ScheduleData.ashx?Req=GetRouteById&RouteId=' + routeId).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            var _route = data.Object[0];
            _route.RSCDepartureTime = null;
            if (_route.RSCDepartureTime_sec != null) {
                _route.RSCDepartureTime = new Date($scope.Schedule.ScheduleBeginDate);
                _route.RSCDepartureTime.setSeconds(_route.RSCDepartureTime_sec);
            }
            _route.RSCArrivalTime = null;
            if (_route.RSCArrivalTime_sec != null) {
                _route.RSCArrivalTime = new Date($scope.Schedule.ScheduleBeginDate);
                _route.RSCArrivalTime.setSeconds(_route.RSCArrivalTime_sec);
            }
            _route.VehicleName = null;
            var vehicle = saService.GetVehicleById(_route.VehicleId);
            if (vehicle != null)
                _route.VehicleName = vehicle.Description;
            _route.Vehicle = vehicle;
            _route.DepartDayInterval = Math.floor(_route.RSCDepartureTime_sec / 86400);
            _route.ArrivalDayInterval = Math.floor(_route.RSCArrivalTime_sec / 86400);
            $scope.Route = _route;
            $scope.Status = "Loaded";
        });
    };

    $scope.ok = function () {
        var _route = $scope.Route;
        if (_route == null) return;
        if (_route.Name == null) {
            saService.AlertMsg("Please input Name.");
            return;
        }
        if (_route.Vehicle == null) {
            saService.AlertMsg("Please select a vehicle.");
            return;
        }
        var departure_seconds = _route.DepartDayInterval * 86400 + Math.floor((_route.RSCDepartureTime.getTime() - $scope.Schedule.ScheduleBeginDate.getTime()) / 1000) % 86400;
        var arrival_seconds = _route.ArrivalDayInterval * 86400 + Math.floor((_route.RSCArrivalTime.getTime() - $scope.Schedule.ScheduleBeginDate.getTime()) / 1000) % 86400;
        if (departure_seconds > arrival_seconds) {
            saService.AlertMsg("departure time should be less than arrival time.");
            return;
        }
        var hasError = false;
        angular.forEach(routeStationList, function (row) {
            if (hasError) return;
            if (row.ArrivalSchedule_sec < departure_seconds || row.DepartureSchedule_sec < departure_seconds) {
                saService.AlertMsg("Departure time is invalid.");
                hasError = true;
                return;
            }
            if (row.ArrivalSchedule_sec > arrival_seconds || row.DepartureSchedule_sec > arrival_seconds) {
                saService.AlertMsg("Arrival time is invalid.");
                hasError = true;
                return;
            }
        });
        if (hasError) return;
        var url = SAApp_AjaxBaseURL;
        var data = 'Req=SaveRoute';
        data += "&ScheduleId=" + $scope.Schedule.ScheduleId;
        data += "&RouteId=" + _route.RouteId;
        data += "&Name=" + _route.Name;
        data += "&VehicleId=" + _route.Vehicle.VehicleId;
        data += "&DepartureTime=" + departure_seconds;
        data += "&ArrivalTime=" + arrival_seconds;
        data += "&Description=" + _route.Description;
        $scope.Status = "Pending";
        $http.post(url + '?' + data).success(function (data) {
            $scope.Status = "Loaded";
            saService.EditRoute_Completed("success");
        })
        .error(function () {
            $scope.Status = "Loaded";
            saService.AlertMsg("Save fail, try again.");
        });
    };

    $scope.cancel = function () {
        saService.EditRoute_Completed("cancel");
    };

    $scope.VehicleChange = function (k) {
    };

    $scope.DepartureDateChange = function () {
        var _route = $scope.Route;
        var departure_seconds = _route.DepartDayInterval * 86400 + Math.floor((_route.RSCDepartureTime.getTime() - $scope.Schedule.ScheduleBeginDate.getTime()) / 1000) % 86400;
        var date = new Date($scope.Schedule.ScheduleBeginDate);
        date.setSeconds(departure_seconds);
        _route.RSCDepartureTime = date;
    };

    $scope.ArrivalDateChange = function () {
        var _route = $scope.Route;
        var arrival_seconds = _route.ArrivalDayInterval * 86400 + Math.floor((_route.RSCArrivalTime.getTime() - $scope.Schedule.ScheduleBeginDate.getTime()) / 1000) % 86400;
        var date = new Date($scope.Schedule.ScheduleBeginDate);
        date.setSeconds(arrival_seconds);
        _route.RSCArrivalTime = date;
    };

    $scope.InitController();
};

