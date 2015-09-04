function SARouteStationCtrl($scope, $http, $location, saService) {
    $scope.InitController = function () {
        $scope.SARoute = null;
        $scope.StationList = null;
        $scope.Status = "Init";
    };

    $scope.InitData = function (route) {
        $scope.InitController();
        saService.SetRouteStation(null);
        $scope.SASchedule = saService.GetCurSchedule();
        $scope.SARoute = route;
        if (route != null)
            $scope.GetStationListByRouteId(route.RouteId);
    };

    $scope.$on('SARouteChanged', function (event, route) {
        $scope.InitData(route);
    });

    $scope.GetStationListByRouteId = function (routeId) {
        $scope.Status = "Loading";
        $http.post('ScheduleData.ashx?Req=GetRouteStationList&RouteId=' + routeId).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) {
                $scope.Status = "NoData";
                return;
            }
            $scope.Status = "Data";
            var baseDate = $scope.SASchedule.ScheduleBeginDate;
            angular.forEach(data.Object, function (row) {
                row.DepartureTime = null;
                row.Tag = '';
                row.DepartureTime = null;
                if (row.DepartureSchedule_sec != null) {
                    row.DepartureTime = new Date(baseDate);
                    row.DepartureTime.setSeconds(row.DepartureSchedule_sec);
                }
                row.ArrivalTime = null;
                if (row.ArrivalSchedule_sec != null) {
                    row.ArrivalTime = new Date(baseDate);
                    row.ArrivalTime.setSeconds(row.ArrivalSchedule_sec);
                }
                row.StationName = null;
                var station = saService.GetStationById(row.StationId);
                if (station != null) {
                    row.StationName = station.Name;
                    row.StationNumber = station.StationNumber;
                    row.Tag += station.Name + ' ' + station.StationNumber;
                }
                row.DeliveryType = null;
                var delivery = saService.GetDeliveryTypeById(row.DeliveryTypeId);
                if (delivery != null)
                    row.DeliveryType = delivery.Name;
            });
            $scope.StationList = data.Object;
        });
    };
    $scope.GetRStationById = function (routeStationId) {
        var routeStation = null;
        if (routeStationId != 0)
            angular.forEach($scope.StationList, function (item) {
                if (item.RouteStationId == routeStationId)
                    routeStation = item;
            });
            return routeStation;
    };
    $scope.CanNewStation = function () {
        var route = saService.GetCurRoute();
        if (route == null) return false;
        var today = new Date();
        return route.RSCDepartureTime > today;
    };
    $scope.CanEditStation = function (routeStationId) {
        var routeStation = typeof routeStationId !== 'undefined' ? $scope.GetRStationById(routeStationId) : saService.GetCurRouteStation();
        if (routeStation == null) return true;
        var today = new Date();
        return routeStation.ArrivalTime < today;
    };
    $scope.CanDeleteStation = function (routeStationId) {
        var routeStation = typeof routeStationId !== 'undefined' ? $scope.GetRStationById(routeStationId) : saService.GetCurRouteStation();
        if (routeStation == null) return true;
        var today = new Date();
        return routeStation.ArrivalTime < today;
    };

    $scope.IsStationInit = function () {
        return $scope.Status == "Init";
    };
    $scope.IsStationNoData = function () {
        return $scope.Status == "NoData";
    };
    $scope.IsStationLoading = function () {
        return $scope.Status == "Loading";
    };
    $scope.HaveStationData = function () {
        return $scope.Status == "Data";
    };

    $scope.IsCurrentStation = function (routeStationId) {
        var station = saService.GetCurRouteStation();
        if (station == null) return false;
        return station.RouteStationId == routeStationId;
    };

    $scope.IsUncompletedStation = function (routeStationId) {
        var station = saService.GetCurRouteStation();
        if (station == null) return false;
        return station.DepartureTime == null || station.StationName == null || station.ArrivalTime == null;
    };

    $scope.SelectRouteStation = function (routeStationId) {
        var routeStation = $scope.GetRStationById(routeStationId);
        saService.SetRouteStation(routeStation);
    };

    $scope.EditRouteStation = function (routeStationId) {
        var routeStation = typeof routeStationId !== 'undefined' ? $scope.GetRStationById(routeStationId) : saService.GetCurRouteStation();
        saService.SetEditRouteStation(routeStation);
    };

    $scope.DeleteRouteStation = function (routeStationId) {
        var routeStation = typeof routeStationId !== 'undefined' ? $scope.GetRStationById(routeStationId) : saService.GetCurRouteStation();
        if (routeStation == null) return;
        if (!saService.ConfirmMsg("Click Ok to delete this station.")) return;
        var url = SAApp_AjaxBaseURL + "?Req=DeleteRouteStation&RouteStationId=" + routeStation.RouteStationId;
        $http.post(url).success(function (data) {
            if (saService.GetCurRouteStation() == routeStation) {
                saService.SetRouteStation(null);
            }
            saService.RefreshStationList();
        });
    }
    $scope.InitController();
};


var RouteStationInstanceCtrl = function ($scope, $modal, $http, saService) {
    var routeStationList = new Array();
    $scope.InitController = function () {
        $scope.RouteStation = { RouteStationId: 0, DepartureTime: new Date(), ArrivalTime: new Date(), Description: '' };
        $scope.Schedule = null;
        $scope.Route = null;
        $scope.Stations = null;
        $scope.Station = null;
        $scope.DepDayInterval = 0;
        $scope.ArrivalDayInterval = 0;
        $scope.DayIntervals = [0, 1, 2, 3, 4, 5, 6];
        $scope.DeliveryTypes = null;
        $scope.DeliveryType = null;
        $scope.Status = "Init";
    };

    $scope.InitData = function (rStation) {
        $scope.InitController();
        $scope.Stations = saService.GetStationList();
        $scope.DeliveryTypes = saService.GetDeliveryTypeList();
        $scope.Route = saService.GetCurRoute();
        var schedule = saService.GetCurSchedule();
        $scope.Schedule = schedule;
        $scope.RouteStation = { RouteStationId: 0, DepartureTime: new Date(schedule.ScheduleBeginDate),
            ArrivalTime: new Date(schedule.ScheduleBeginDate), Description: ''
        };
        GetStationListByRouteId($scope.Route.RouteId);
        if (rStation != null)
            $scope.GetRStationById(rStation.RouteStationId);
        else
            $scope.Status = "Loaded";
    };

    $scope.$on('EditRStationChanged', function (event, rStation) {
        $scope.InitData(rStation);
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

    $scope.GetRStationById = function (rStationId) {
        $http.post('ScheduleData.ashx?Req=GetRouteStationById&RouteStationId=' + rStationId).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            var _rStation = data.Object[0];
            var baseDate = $scope.Schedule.ScheduleBeginDate;
            $scope.Station = saService.GetStationById(_rStation.StationId);
            $scope.DeliveryType = saService.GetDeliveryTypeById(_rStation.DeliveryTypeId);

            $scope.DepDayInterval = Math.floor(_rStation.DepartureSchedule_sec / 86400);
            _rStation.DepartureTime = new Date(baseDate);
            _rStation.DepartureTime.setSeconds(_rStation.DepartureSchedule_sec);

            $scope.ArrivalDayInterval = Math.floor(_rStation.ArrivalSchedule_sec / 86400);
            _rStation.ArrivalTime = new Date(baseDate);
            _rStation.ArrivalTime.setSeconds(_rStation.ArrivalSchedule_sec);

            $scope.RouteStation = _rStation;
            $scope.Status = "Loaded";
        });
    };

    $scope.ok = function () {
        var _rStation = $scope.RouteStation;
        if ($scope.Station == null) {
            saService.AlertMsg("Please select a station.");
            return;
        }
        var departure_seconds = $scope.DepDayInterval * 86400 + Math.floor((_rStation.DepartureTime.getTime() - $scope.Schedule.ScheduleBeginDate.getTime()) / 1000) % 86400;
        var arrival_seconds = $scope.ArrivalDayInterval * 86400 + Math.floor((_rStation.ArrivalTime.getTime() - $scope.Schedule.ScheduleBeginDate.getTime()) / 1000) % 86400;
        if (departure_seconds < arrival_seconds) {
            saService.AlertMsg("Arrival time should be less than Departure time.");
            return;
        }
        if (arrival_seconds < $scope.Route.RSCDepartureTime_sec) {
            saService.AlertMsg("Arrival time should be more than depot departure time.");
            return;
        }
        if (departure_seconds > $scope.Route.RSCArrivalTime_sec) {
            saService.AlertMsg("Departure time should be less than depot arrival time.");
            return;
        }
        if ($scope.DeliveryType == null) {
            saService.AlertMsg("Please select a delivery type.");
            return;
        }
        var hasError = false;
        angular.forEach(routeStationList, function (row) {
            if (row.RouteStationId == _rStation.RouteStationId) return;
            if (hasError) return;
            if (row.ArrivalSchedule_sec < departure_seconds && row.DepartureSchedule_sec > departure_seconds) {
                saService.AlertMsg("Departure time is invalid.");
                hasError = true;
                return;
            }
            if (row.ArrivalSchedule_sec < arrival_seconds && row.DepartureSchedule_sec > arrival_seconds) {
                saService.AlertMsg("Arrival time is invalid.");
                hasError = true;
                return;
            }
            if (arrival_seconds < row.ArrivalSchedule_sec && departure_seconds > row.ArrivalSchedule_sec) {
                saService.AlertMsg("Arrival time or departure time are invalid.");
                hasError = true;
                return;
            }
            if (arrival_seconds < row.DepartureSchedule_sec && departure_seconds > row.DepartureSchedule_sec) {
                saService.AlertMsg("Arrival time or departure time are invalid.");
                hasError = true;
                return;
            }
        });
        if (hasError) return;

        var url = SAApp_AjaxBaseURL;
        var data = 'Req=SaveRouteStation';
        data += "&RouteId=" + $scope.Route.RouteId;
        data += "&RouteStationId=" + _rStation.RouteStationId;
        data += "&StationId=" + $scope.Station.StationId;
        data += "&DevliveryTypeId=" + $scope.DeliveryType.Id;
        data += "&DepartureTime=" + departure_seconds;
        data += "&ArrivalTime=" + arrival_seconds;
        data += "&Description=" + _rStation.Description;
        $scope.Status = "Pending";
        $http.post(url + '?' + data).success(function (data) {
            $scope.Status = "Loaded";
            saService.EditRouteStation_Completed("success");
        })
        .error(function () {
            $scope.Status = "Loaded";
            saService.AlertMsg("Save fail, try again.");
        });
    };

    $scope.cancel = function () {
        saService.EditRouteStation_Completed("cancel");
    };

    $scope.ArrivalDateChange = function () {
        var _station = $scope.RouteStation;
        var arrival_seconds = $scope.ArrivalDayInterval * 86400 + Math.floor((_station.ArrivalTime.getTime() - $scope.Schedule.ScheduleBeginDate.getTime()) / 1000) % 86400;
        var date = new Date($scope.Schedule.ScheduleBeginDate);
        date.setSeconds(arrival_seconds);
        _station.ArrivalTime = date;
    };

    $scope.DeparturelDateChange = function () {
        var _station = $scope.RouteStation;
        var departure_seconds = $scope.DepDayInterval * 86400 + Math.floor((_station.DepartureTime.getTime() - $scope.Schedule.ScheduleBeginDate.getTime()) / 1000) % 86400;
        var date = new Date($scope.Schedule.ScheduleBeginDate);
        date.setSeconds(departure_seconds);
        _station.DepartureTime = date;
    };

    $scope.InitController();
};
