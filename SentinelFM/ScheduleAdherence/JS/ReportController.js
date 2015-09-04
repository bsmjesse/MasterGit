saReportApp.controller('ReportCtrl', function ReportCtrl($scope, $http, $timeout, $log, $filter, ngTableParams, saService) {
    //function ReportCtrl($scope, $http, $timeout, $log, saService) {
    $scope.InitController = function () {
        $scope.DepotList = null;
        $scope.StationList = null;
        $scope.VehicleList = null;
        $scope.StatusList = null;
        $scope.DeliveryStatus = null;
        $scope.ReasonList = null;
        $scope.StartDate = null;
        $scope.EndDate = null;
        $scope.StartOpened = false;
        $scope.EndOpened = false;
        $scope.GetStatusList();
        $scope.InitSearchDate();
        $scope.RouteList = [];
        $scope.OrginalDataList = null;
        $scope.Status = "Init";
        $scope.IsAutoSave = true;
        $scope.DefaultSearch = { RouteName: DefaultRoute, VehicleName: DefaultVehicle, StationName: DefaultStation };
        $scope.DefaultVehicle = null;
        $scope.DefaultStation = null;
        $scope.SearchRoute = { Depot: null, RouteName: DefaultRoute, VehicleName: DefaultVehicle, StationName: DefaultStation }
        $scope.tableParams = null;
        $scope.CSVText = null;
    };
    $scope.$on('GetVehicles', function (event, vehicles) {
        $scope.VehicleList = [];
        angular.forEach(vehicles, function (row) {
            $scope.VehicleList.push(row);
        });
        if ($scope.DefaultSearch.VehicleName != "") {
            angular.forEach(vehicles, function (row) {
                if (row.Description == $scope.DefaultSearch.VehicleName)
                    $scope.DefaultVehicle = row;
            });
        }
    });

    $scope.$on('GetStations', function (event, stations) {
        $scope.StationList = [];
        angular.forEach(stations, function (row) {
            $scope.StationList.push(row);
        });
        if ($scope.DefaultSearch.StationName != "") {
            angular.forEach(stations, function (row) {
                if (row.Name == $scope.DefaultSearch.StationName || row.StationNumber == $scope.DefaultSearch.StationName)
                    $scope.DefaultStation = row;
            });
        }
    });

    $scope.$on('GetDepots', function (event, depots) {
        $scope.DepotList = [];
        $scope.SearchRoute.Depot = { StationId: 0, Name: "All" };
        $scope.DepotList.push($scope.SearchRoute.Depot);
        angular.forEach(depots, function (row) {
            $scope.DepotList.push(row);
        });
    });

    $scope.$on('GetReasons', function (event, reasons) {
        $scope.ReasonList = [];
        angular.forEach(reasons, function (row) {
            $scope.ReasonList.push(row);
        });
    });

    $scope.ListCompare = function (item) {
        if ($scope.SearchRoute.Depot.StationId == 0 && $scope.SearchRoute.RouteName == '' && $scope.SearchRoute.VehicleName == '' && $scope.SearchRoute.StationName == '') return true;
        if ($scope.SearchRoute.Depot.StationId != 0 && item.Route.RSCDepot.StationId != $scope.SearchRoute.Depot.StationId) return false;
        var des = $scope.SearchRoute.RouteName.toLowerCase();
        if (des != '' && item.Route.Name.toLowerCase().indexOf(des) < 0) return false;
        des = $scope.SearchRoute.VehicleName.toLowerCase();
        if (des != '' && (item.Route.VehicleName == null || item.Route.VehicleName.toLowerCase().indexOf(des) < 0)) return false;
        des = $scope.SearchRoute.StationName.toLowerCase();
        if (des != '' &&
            (item.Station.StationName.toLowerCase().indexOf(des) < 0 &&
             item.Station.StationNo.toLowerCase().indexOf(des) < 0)) return false;
        return true;
    };

    $scope.VehicleCompare = function (v) {
        if ($scope.SearchRoute.VehicleName == '') return true;
        return v.Description.toLowerCase().indexOf($scope.SearchRoute.VehicleName) >= 0;
    };

    $scope.StationCompare = function (item) {
        if ($scope.SearchRoute.StationName == '') return true;
        des = $scope.SearchRoute.StationName.toLowerCase();
        if (des != '' &&
            (item.Name.toLowerCase().indexOf(des) < 0 &&
             item.StationNumber.toLowerCase().indexOf(des) < 0)) return false;
        return true;
    };

    $scope.GetStatusList = function () {
        $scope.StatusList = [];
        var statusList = saService.GetDeliveryStatusList();
        var allStatus = { Id: 0, Name: 'All' };
        $scope.StatusList.push(allStatus);
        angular.forEach(statusList, function (row) {
            $scope.StatusList.push(row);
        });
        $scope.DeliveryStatus = allStatus;
    };

    $scope.InitSearchDate = function () {
        var today = new Date();
        var dayofweek = today.getDay();
        var startDate = new Date(today);
        startDate.setDate(today.getDate() - dayofweek);
        $scope.StartDate = startDate;
        var endDate = new Date(today);
        endDate.setDate(today.getDate() + 7 - dayofweek - 1);
        $scope.EndDate = endDate;
    };

    $scope.SelectSearchStation = function (station) {
        $scope.SearchStation.StationName = station.Name;
    };

    $scope.NextWeek = function () {
    };
    $scope.LastWeek = function () {
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
        if ($scope.DepotList == null || $scope.StationList == null ||
            $scope.VehicleList == null || $scope.StatusList == null ||
            $scope.ReasonList == null) {
            return false;
        }
        if ($scope.StartDate == null || $scope.EndDate == null || $scope.StartDate > $scope.EndDate) {
            return false;
        }
        return true;
    };

    $scope.CanExport = function () {
        return $scope.Status == 'Data' && (_IsFirfox || _IsChrome);
    };

    $scope.View = function () {
        $scope.Status = "Loading";
        var url = "";
        //        if ($scope.Depot != null && $scope.Depot.StationId != 0)
        //            url += "&DepotId=" + $scope.Depot.StationId;
        if ($scope.DefaultStation != null)
            url += "&StatiionId=" + $scope.DefaultStation.StationId;
        if ($scope.DefaultVehicle != null)
            url += "&VehicleId=" + $scope.DefaultVehicle.VehicleId;
        url += "&StatusId=" + $scope.DeliveryStatus.Id;
        url += "&StartDate=" + $scope.StartDate.toDateString();
        url += "&EndDate=" + $scope.EndDate.toDateString();
        $scope.RouteList = [];
        $http.post('ScheduleData.ashx?Req=GetReport' + url).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) {
                $scope.Status = "NoData";
                return;
            }
            $scope.OrginalDataList = data.Object;
            var db = TAFFY($scope.OrginalDataList);
            var routeIds = db().distinct("RouteId");

            $scope.RouteList = [];
            angular.forEach(routeIds, function (routeId) {
                var orgRoute = db({ RouteId: routeId }).first();
                var route = $scope.GetRouteDataByOrgData(orgRoute);
                var stationList = $scope.GetStationListByOrgData(routeId);
                angular.forEach(stationList, function (s) {
                    $scope.RouteList.push({ Route: route, Station: s, RouteName: route.Name });
                });
            });
            $scope.Status = "Data";
            if ($scope.tableParams == null)
                $scope.tableParams = InitTableParams();
            $scope.tableParams.total($scope.RouteList.length);
            $scope.tableParams.reload();
        });
    };
    $scope.$watch('SearchRoute.Depot', function () {
        if ($scope.tableParams == null) return;
        $scope.tableParams.reload();
    });
    $scope.$watch('SearchRoute.RouteName', function () {
        if ($scope.tableParams == null) return;
        $scope.tableParams.reload();
    });
    $scope.$watch('SearchRoute.VehicleName', function () {
        if ($scope.tableParams == null) return;
        $scope.tableParams.reload();
    });
    $scope.$watch('SearchRoute.StationName', function () {
        if ($scope.tableParams == null) return;
        $scope.tableParams.reload();
    });

    $scope.generateCSV = function () {
        var filteredData = [];
        angular.forEach($scope.RouteList, function (row) {
            if ($scope.ListCompare(row))
                filteredData.push(row);
        });
        var orderedData = $scope.tableParams.sorting() ? $filter('orderBy')(filteredData, $scope.tableParams.orderBy()) : filteredData;
        var data = '"Depot","Route","Vehicle","Station No","Station Name","Depot Depart Sched","Depot Depart Actual","Depot Depart Reason",'+
                    '"Station Arrival Sched","Station Arrival Actual","Station Arrival Reason","Station Depart Sched","Station Depart Actual","Station Depart Reason",'+
                    '"Depot Arrival Sched","Depot Arrival Actual","Depot Arrival Reason","Route Duration","Depot Comments","Station Comments"\n';
        angular.forEach(orderedData, function (item, i) {
            var route = item.Route;
            var station = item.Station;
            var rowData = "";
            if (route.RSCDepot == null)
                rowData += stringify("") + ',';
            else
                rowData += stringify(route.RSCDepot.Name) + ',';
            rowData += stringify(route.Name) + ',';
            rowData += stringify(route.VehicleName) + ',';
            rowData += stringify(station.StationNo) + ',';
            rowData += stringify(station.StationName) + ',';
            rowData += stringify($filter('date')(route.RSCSchedule_Departure, "medium")) + ',';
            rowData += stringify($filter('date')(route.RSCActual_Departure, "medium")) + ',';
            rowData += stringify(route.RSCDeparture_CodeDescription) + ',';
            rowData += stringify($filter('date')(station.Schedule_Arrival, "medium")) + ',';
            rowData += stringify($filter('date')(station.Actual_Arrival, "medium")) + ',';
            rowData += stringify(station.Arrival_CodeDescription) + ',';
            rowData += stringify($filter('date')(station.Schedule_Departure, "medium")) + ',';
            rowData += stringify($filter('date')(station.Actual_Departure, "medium")) + ',';
            rowData += stringify(station.Departure_CodeDescription) + ',';
            rowData += stringify($filter('date')(route.RSCSchedule_Arrival, "medium")) + ',';
            rowData += stringify($filter('date')(route.RSCActual_Arrival, "medium")) + ',';
            rowData += stringify(route.RSCArrival_CodeDescription) + ',';
            if (route.RSCActual_Arrival == null || route.RSCActual_Departure == null)
                rowData += stringify("") + ',';
            else
                rowData += stringify($filter('TimeString')(route.RSCActual_Arrival.getTime() - route.RSCActual_Departure.getTime())) + ','; // Route Duration
            rowData += stringify(route.Description) + ',';
            rowData += stringify(station.Description) + ',';
            rowData = rowData.slice(0, rowData.length - 1); //remove last semicolon
            data += rowData + "\n";
        });
        $scope.CSVText = 'data:text/csv;charset=UTF-8,' + encodeURIComponent(data);
    };

    function InitTableParams() {
        var tablePageCount = 5;
        if ($scope.tableParams != null)
            tablePageCount = $scope.tableParams.count();
        return new ngTableParams({
            page: 1,            // show first page
            count: tablePageCount,           // count per page
            sorting: {
                // initial sorting
            }
        }, {
            $scope: $scope,
            total: $scope.RouteList.length, // length of data
            getData: function ($defer, params) {
                var filteredData = [];
                angular.forEach($scope.RouteList, function (row) {
                    if ($scope.ListCompare(row))
                        filteredData.push(row);
                });
                params.total(filteredData.length);
                var orderedData = params.sorting() ?
                    $filter('orderBy')(filteredData, params.orderBy()) : filteredData;
                $defer.resolve(orderedData.slice((params.page() - 1) * params.count(), params.page() * params.count()));
            }
        });
    };

    $scope.CanSave = function () {
        if ($scope.RouteList == null) return false;
        for (var i = 0; i < $scope.RouteList.length; i++) {
            var item = $scope.RouteList[i];
            if (item.Route.IsDeparture_Changed || item.Route.IsArrival_Changed || item.Route.IsDescription_Changed)
                return true;
            if (item.Station.IsDeparture_Changed || item.Station.IsArrival_Changed || item.Station.IsDescription_Changed)
                return true;
        }
        return false;
    };

    $scope.Save = function () {
        if ($scope.RouteList == null) return;
        for (var i = 0; i < $scope.RouteList.length; i++) {
            var item = $scope.RouteList[i];
            var route = item.Route;
            if (route.IsDeparture_Changed || route.IsArrival_Changed || route.IsDescription_Changed) {
                var departureCode = null; var arrivalCode = null; var description = null;
                if (route.IsDeparture_Changed) departureCode = route.RSCDepart_codeInstance;
                if (route.IsArrival_Changed) arrivalCode = route.RSCArrival_codeInstance;
                if (route.IsDescription_Changed) description = route.Description;
                $scope.UpdateRoute(true, route.RouteId, departureCode, arrivalCode, description);
                route.IsDeparture_Changed = false;
                route.IsArrival_Changed = false;
                route.IsDescription_Changed = false;
            }
            var station = item.Station;
            if (station.IsDeparture_Changed || station.IsArrival_Changed || station.IsDescription_Changed) {
                var departureCode = null; var arrivalCode = null; var description = null;
                if (station.IsDeparture_Changed) departureCode = station.Depart_codeInstance;
                if (station.IsArrival_Changed) arrivalCode = station.Arrival_codeInstance;
                if (station.IsDescription_Changed) description = station.Description;
                $scope.UpdateRSStation(true, route.RouteId, station.RouteStationId, departureCode, arrivalCode, description);
                station.IsDeparture_Changed = false;
                station.IsArrival_Changed = false;
                station.IsDescription_Changed = false;
            }
        }
    };

    function GetRouteById(routeId) {
        var item = null;
        angular.forEach($scope.RouteList, function (i) {
            if (i.Route.RouteId == routeId)
                item = i.Route;
        });
        return item;
    };

    $scope.GetCodeInstanceByCode = function (codeStr) {
        if (angular.isUndefined(codeStr)) return null;
        var item = null;
        angular.forEach($scope.ReasonList, function (code) {
            if (code.ReasonCode == codeStr || code.Description == codeStr)
                item = code;
        });
        return item;
    };

    $scope.SetRSCDepartCode = function (routeId, codeId) {
        var route = GetRouteById(routeId);
        var code = saService.GetReasonCodeById(codeId);
        if (route == null || code == null) return;
        route.RSCDepart_codeInstance = code;
        route.RSCDeparture_CodeDescription = $scope.GetCodeDescription(code);
        if (code != null)
            $scope.UpdateRoute($scope.IsAutoSave, routeId, code, null, null);
    };

    $scope.RSCDepartCodeChanged = function (routeId) {
        var route = GetRouteById(routeId);
        var code = $scope.GetCodeInstanceByCode(route.RSCDeparture_CodeDescription);
        if (code == null) {
            route.RSCDeparture_CodeDescription = $scope.GetCodeDescription(route.RSCDepart_codeInstance);
            return;
        }
        if (route.RSCDepart_codeInstance == code) {
            route.RSCDepart_codeInstance = null;
            return;
        }
        route.RSCDepart_codeInstance = code;
        route.RSCDeparture_CodeDescription = $scope.GetCodeDescription(code);
        if (code != null)
            $scope.UpdateRoute($scope.IsAutoSave, routeId, code, null, null);
    };

    $scope.SetRSCArrivalCode = function (routeId, codeId) {
        var route = GetRouteById(routeId);
        var code = saService.GetReasonCodeById(codeId);
        if (route == null || code == null) return;
        route.RSCArrival_codeInstance = code;
        route.RSCArrival_CodeDescription = $scope.GetCodeDescription(code);
        if (code != null)
            $scope.UpdateRoute($scope.IsAutoSave, routeId, null, code, null);
    };

    $scope.RSCArrivalCodeChanged = function (routeId) {
        var route = GetRouteById(routeId);
        var code = $scope.GetCodeInstanceByCode(route.RSCArrival_CodeDescription);
        if (code == null) {
            route.RSCArrival_CodeDescription = $scope.GetCodeDescription(route.RSCArrival_codeInstance);
            return;
        }
        if (route.RSCArrival_codeInstance == code) {
            route.RSCArrival_codeInstance = null;
            return;
        }
        route.RSCArrival_codeInstance = code;
        route.RSCArrival_CodeDescription = $scope.GetCodeDescription(code);
        if (code != null)
            $scope.UpdateRoute($scope.IsAutoSave, routeId, null, code, null);
    };

    $scope.RSCDescriptionChanged = function (routeId) {
        var route = GetRouteById(routeId);
        if (route.Description != route.OldDescription) {
            $scope.UpdateRoute($scope.IsAutoSave, routeId, null, null, route.Description);
            route.OldDescription = route.Description;
        }
    };

    $scope.GetRouteDataByOrgData = function (orgRoute) {
        var baseDate = parseDate(orgRoute.ScheduleBeginDate);
        var vehicle = saService.GetVehicleById(orgRoute.VehicleId);
        var depot = saService.GetDepotById(orgRoute.RSCStationId);
        var departureTime = null;
        if (orgRoute.RSCDepartureTime_sec != null) {
            departureTime = new Date(baseDate)
            departureTime.setSeconds(orgRoute.RSCDepartureTime_sec);
        }
        var arrivalTime = null;
        if (orgRoute.RSCArrivalTime_sec != null) {
            arrivalTime = new Date(baseDate);
            arrivalTime.setSeconds(orgRoute.RSCArrivalTime_sec);
        }
        var departure_ActualTime = null;
        if (orgRoute.RSCActualDep_sec != null) {
            departure_ActualTime = new Date(baseDate);
            departure_ActualTime.setSeconds(orgRoute.RSCActualDep_sec);
        }
        var arrival_ActualTime = null;
        if (orgRoute.RSCActualArr_sec != null) {
            arrival_ActualTime = new Date(baseDate);
            arrival_ActualTime.setSeconds(orgRoute.RSCActualArr_sec);
        }
        var departure_Code = null;
        if (orgRoute.RSCDepReasonCodeId != null) {
            departure_Code = saService.GetReasonCodeById(orgRoute.RSCDepReasonCodeId);
        }
        var arrival_Code = null;
        if (orgRoute.RSCArrReasonCodeId != null) {
            arrival_Code = saService.GetReasonCodeById(orgRoute.RSCArrReasonCodeId);
        }
        var route = { RouteId: orgRoute.RouteId, BaseDate: baseDate, Name: orgRoute.Name, VehicleName: vehicle == null ? null : vehicle.Description,
            RSCDepot: depot,
            RSCSchedule_Departure: departureTime, RSCActual_Departure: departure_ActualTime,
            RSCSchedule_Arrival: arrivalTime, RSCActual_Arrival: arrival_ActualTime,
            RSCDeparture_CodeDescription: departure_Code == null ? null : $scope.GetCodeDescription(departure_Code), RSCDepart_codeInstance: departure_Code,
            RSCArrival_CodeDescription: arrival_Code == null ? null : $scope.GetCodeDescription(arrival_Code), RSCArrival_codeInstance: arrival_Code,
            RSCDeparture_StatusId: orgRoute.RSCDepStatusId, RSCArrival_StatusId: orgRoute.RSCArrStatusId,
            Description: orgRoute.RouteDescription, OldDescription: orgRoute.RouteDescription,
            IsDeparture_Changed: false, IsArrival_Changed: false, IsDescription_Changed: false
        };
        return route;
    };

    $scope.GetCodeDescription = function (code) {
        if (code == null) return "";
        return code.ReasonCode + " " + code.Description;
    };

    $scope.GetStationListByOrgData = function (routeId) {
        var stationList = [];
        var db = TAFFY($scope.OrginalDataList);
        db({ RouteId: routeId }).each(function (station) {
            var newStation = $scope.GetStationDataByOrgData(station);
            stationList.push(newStation);
        });
        return stationList;
    };

    $scope.GetStationDataByOrgData = function (orgData) {
        var baseDate = parseDate(orgData.ScheduleBeginDate);
        var station = saService.GetStationById(orgData.StationId);

        var departureTime = null;
        if (orgData.DepartureSchedule_sec != null) {
            departureTime = new Date(baseDate)
            departureTime.setSeconds(orgData.DepartureSchedule_sec);
        }
        var arrivalTime = null;
        if (orgData.ArrivalSchedule_sec != null) {
            arrivalTime = new Date(baseDate);
            arrivalTime.setSeconds(orgData.ArrivalSchedule_sec);
        }
        var departure_ActualTime = null;
        if (orgData.DepartureActual_sec != null) {
            departure_ActualTime = new Date(baseDate);
            departure_ActualTime.setSeconds(orgData.DepartureActual_sec);
        }
        var arrival_ActualTime = null;
        if (orgData.ArrivalActual_sec != null) {
            arrival_ActualTime = new Date(baseDate);
            arrival_ActualTime.setSeconds(orgData.ArrivalActual_sec);
        }
        var departure_Code = null;
        if (orgData.DepReasonCodeId != null) {
            departure_Code = saService.GetReasonCodeById(orgData.DepReasonCodeId);
        }
        var arrival_Code = null;
        if (orgData.ArrReasonCodeId != null) {
            arrival_Code = saService.GetReasonCodeById(orgData.ArrReasonCodeId);
        }
        return { RouteStationId: orgData.RouteStationId, BaseDate: baseDate,
            StationName: station == null ? '' : station.Name, StationNo: station == null ? '' : station.StationNumber,
            Schedule_Departure: departureTime, Actual_Departure: departure_ActualTime,
            Schedule_Arrival: arrivalTime, Actual_Arrival: arrival_ActualTime,
            Departure_CodeDescription: departure_Code == null ? null : $scope.GetCodeDescription(departure_Code), Depart_codeInstance: departure_Code,
            Arrival_CodeDescription: arrival_Code == null ? null : $scope.GetCodeDescription(arrival_Code), Arrival_codeInstance: arrival_Code,
            Departure_StatusId: orgData.DepStatusId, Arrival_StatusId: orgData.ArrStatusId,
            Description: orgData.StationDescription, OldDescription: orgData.StationDescription,
            IsDeparture_Changed: false, IsArrival_Changed: false, IsDescription_Changed: false
        };
    };

    function GetSAStationById(routeId, rsStationId) {
        var item = null;
        angular.forEach($scope.RouteList, function (i) {
            if (i.Station.RouteStationId == rsStationId)
                item = i.Station;
        });
        return item;
    };

    $scope.SetDepartCode = function (routeId, rsStationId, codeId) {
        var station = GetSAStationById(routeId, rsStationId);
        var code = saService.GetReasonCodeById(codeId);
        if (station == null || code == null) return;
        station.Depart_codeInstance = code;
        station.Departure_CodeDescription = $scope.GetCodeDescription(code);
        if (code != null)
            $scope.UpdateRSStation($scope.IsAutoSave, routeId, rsStationId, code, null, null);
    };

    $scope.DepartCodeChanged = function (routeId, rsStationId) {
        var station = GetSAStationById(routeId, rsStationId);
        var code = $scope.GetCodeInstanceByCode(station.Departure_CodeDescription);
        if (code == null) {
            station.Departure_CodeDescription = $scope.GetCodeDescription(station.Depart_codeInstance);
            return;
        }
        if (station.Depart_codeInstance == code) {
            station.Depart_codeInstance = null;
            return;
        }
        station.Depart_codeInstance = code;
        station.Departure_CodeDescription = $scope.GetCodeDescription(code);
        if (code != null)
            $scope.UpdateRSStation($scope.IsAutoSave, routeId, rsStationId, code, null, null);
    };

    $scope.SetArrivalCode = function (routeId, rsStationId, codeId) {
        var station = GetSAStationById(routeId, rsStationId);
        var code = saService.GetReasonCodeById(codeId);
        if (station == null || code == null) return;
        station.Arrival_codeInstance = code;
        station.Arrival_CodeDescription = $scope.GetCodeDescription(code);
        if (code != null)
            $scope.UpdateRSStation($scope.IsAutoSave, routeId, rsStationId, null, code, null);
    };

    $scope.ArrivalCodeChanged = function (routeId, rsStationId) {
        var station = GetSAStationById(routeId, rsStationId);
        var code = $scope.GetCodeInstanceByCode(station.Arrival_CodeDescription);
        if (code == null) {
            station.Arrival_CodeDescription = $scope.GetCodeDescription(station.Arrival_codeInstance);
            return;
        }
        if (station.Arrival_codeInstance == code) {
            station.Arrival_codeInstance = null;
            return;
        }
        station.Arrival_codeInstance = code;
        station.Arrival_CodeDescription = $scope.GetCodeDescription(code);
        if (code != null)
            $scope.UpdateRSStation($scope.IsAutoSave, routeId, rsStationId, null, code, null);
    };

    $scope.StationDescriptionChanged = function (routeId, rsStationId) {
        var station = GetSAStationById(routeId, rsStationId);
        if (station.Description != station.OldDescription) {
            $scope.UpdateRSStation($scope.IsAutoSave, routeId, rsStationId, null, null, station.Description);
            station.OldDescription = station.Description;
        }
    };

    $scope.UpdateRoute = function (IsRealSave, routeId, departureCode, arrivalCode, description) {
        if (IsRealSave) {
            var url = "&RouteId=" + routeId;
            if (departureCode != null)
                url += "&DepartureReasonId=" + departureCode.ReasonCodeId;
            if (arrivalCode != null)
                url += "&ArrivalReasonId=" + arrivalCode.ReasonCodeId;
            if (description != null)
                url += "&Description=" + description;
            $http.post('ScheduleData.ashx?Req=UpdateRoute' + url).success(function (data) {
            });
        }
        else {
            var route = GetRouteById(routeId);
            if (departureCode != null)
                route.IsDeparture_Changed = true;
            if (arrivalCode != null)
                route.IsArrival_Changed = true;
            if (description != null)
                route.IsDescription_Changed = true;
        }
    };

    $scope.UpdateRSStation = function (IsRealSave, routeId, rsStationId, departureCode, arrivalCode, description) {
        if (IsRealSave) {
            var url = "&RouteStationId=" + rsStationId;
            if (departureCode != null)
                url += "&DepartureReasonId=" + departureCode.ReasonCodeId;
            if (arrivalCode != null)
                url += "&ArrivalReasonId=" + arrivalCode.ReasonCodeId;
            if (description != null)
                url += "&Description=" + description;
            $http.post('ScheduleData.ashx?Req=UpdateRouteStation' + url).success(function (data) {
            });
        }
        else {
            var station = GetSAStationById(routeId, rsStationId);
            if (departureCode != null)
                station.IsDeparture_Changed = true;
            if (arrivalCode != null)
                station.IsArrival_Changed = true;
            if (description != null)
                station.IsDescription_Changed = true;
        }
    };

    $scope.DepotChange = function () {
    };

    $scope.InitController();
    //};
});

function stringify(str) {
    if (str == null)
        return '""';
    return '"' +
        str.replace(/^\s\s*/, '').replace(/\s*\s$/, '') // trim spaces
            .replace(/"/g, '""') + // replace quotes with double quotes
        '"';
}

var _IsFirfox = navigator.userAgent.indexOf("Firefox") > 0;
var _IsChrome = navigator.userAgent.indexOf("Chrome") > 0;
