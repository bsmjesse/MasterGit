
var saScheduleApp = angular.module('saScheduleApp', ['ui.bootstrap'])

    .service('saService', function ($rootScope, $http) {
        var Depot = null;
        var EditGroup = null;
        var CopyGroup = null;
        var ImportGroup = null;
        var Schedule = null;
        var CopySchedule = null;
        var Vehicles = null;
        var Stations = null;
        var DeliveryTypes = [{ Id: 1, Name: "Reg Sched" }, { Id: 2, Name: "Cust P/U / LTL" }, { Id: 3, Name: "Flyer"}];
        var Route = null;
        var EditRoute = null;
        var RouteStation = null;
        var EditStation = null;
        var Setting = null;
        $http.post('ScheduleData.ashx?Req=GetSetting').success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            Setting = data.Object[0];
            $rootScope.$broadcast('GetSetting', Setting);
        });
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
        return {
            SetDepot: function (_depot, forceFlag) {
                forceFlag = typeof forceFlag !== 'undefined' ? forceFlag : false;
                if (forceFlag || Depot == null || _depot == null || Depot.StationId != _depot.StationId) {
                    Depot = _depot;
                    $rootScope.$broadcast('DepotChanged', Depot);
                }
            },
            GetCurDepot: function () {
                return Depot;
            },
            GetSASetting: function () {
                return Setting;
            },
            SetEditGroup: function (_group, openDialog) {
                openDialog = typeof openDialog !== 'undefined' ? openDialog : true;
                if (EditGroup == null || _group == null || EditGroup.GroupId != _group.GroupId) {
                    EditGroup = _group;
                    $rootScope.$broadcast('EditGroupChanged', EditGroup);
                    if (openDialog) {
                        $("#Dialog_Group").dialog("option", "title", EditGroup == null ? "Add a Schedule Group" : "Edit a Schedule Group");
                        $("#Dialog_Group").dialog("open");
                    }
                }
            },
            EditGroup_Completed: function (result) {
                $("#Dialog_Group").dialog("close");
                if (result == "success")
                    $rootScope.$broadcast('DepotChanged', Depot);
            },
            SetCopyGroup: function (_group, openDialog) {
                openDialog = typeof openDialog !== 'undefined' ? openDialog : true;
                if (CopyGroup == null || _group == null || CopyGroup.GroupId != _group.GroupId) {
                    CopyGroup = _group;
                    $rootScope.$broadcast('CopyGroupChanged', CopyGroup);
                    if (openDialog) {
                        $("#Dialog_GroupCopy").dialog("option", "title", "Copy a Schedule Group");
                        $("#Dialog_GroupCopy").dialog("open");
                    }
                }
            },
            CopyGroup_Completed: function (result) {
                $("#Dialog_GroupCopy").dialog("close");
                if (result == "success")
                    $rootScope.$broadcast('DepotChanged', Depot);
            },
            SetImportGroup: function (_group, openDialog) {
                openDialog = typeof openDialog !== 'undefined' ? openDialog : true;
                if (ImportGroup == null || _group == null || ImportGroup.GroupId != _group.GroupId) {
                    ImportGroup = _group;
                    $rootScope.$broadcast('ImportGroupChanged', ImportGroup);
                    if (openDialog) {
                        $("#Dialog_GroupImport").dialog("option", "title", "Import a Schedule Group");
                        $("#Dialog_GroupImport").dialog("open");
                    }
                }
            },
            SetImportGroup_Completed: function (result) {
                $("#Dialog_GroupImport").dialog("close");
                Schedule = null;
                $rootScope.$broadcast('SAScheduleChanged', Schedule);
            },
            SetSchedule: function (_schedule) {
                if (Schedule == null || _schedule == null || Schedule.ScheduleId != _schedule.ScheduleId) {
                    Schedule = _schedule;
                    $rootScope.$broadcast('SAScheduleChanged', Schedule);
                }
            },
            GetCurSchedule: function () {
                return Schedule;
            },
            SetCopySchedule: function (_schedule, openDialog) {
                openDialog = typeof openDialog !== 'undefined' ? openDialog : true;
                if (CopySchedule == null || _schedule == null || CopySchedule.ScheduleId != _schedule.ScheduleId) {
                    CopySchedule = _schedule;
                    $rootScope.$broadcast('CopyScheduleChanged', CopySchedule);
                    if (openDialog) {
                        $("#Dialog_ScheduleCopy").dialog("option", "title", "Copy a Schedule");
                        $("#Dialog_ScheduleCopy").dialog("open");
                    }
                }
            },
            CopySchedule_Completed: function (result) {
                $("#Dialog_ScheduleCopy").dialog("close");
                if (result == "success")
                    $rootScope.$broadcast('DepotChanged', Depot);
            },
            RefreshGroupList: function () {
                $rootScope.$broadcast('DepotChanged', Depot);
            },
            RefreshRouteList: function () {
                $rootScope.$broadcast('SAScheduleChanged', Schedule);
            },
            RefreshStationList: function () {
                $rootScope.$broadcast('SARouteChanged', Route);
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
            GetDeliveryTypeList: function () {
                return DeliveryTypes;
            },
            GetDeliveryTypeById: function (id) {
                if (id == null) return null;
                var type = null;
                angular.forEach(DeliveryTypes, function (item) {
                    if (item.Id == id)
                        type = item;
                });
                return type;
            },
            SetRoute: function (_route) {
                if (Route == null || _route == null || Route.RouteId != _route.RouteId) {
                    Route = _route;
                    $rootScope.$broadcast('SARouteChanged', Route);
                }
            },
            GetCurRoute: function () {
                return Route;
            },
            SetEditRoute: function (_route, openDialog) {
                openDialog = typeof openDialog !== 'undefined' ? openDialog : true;
                if (EditRoute == null || _route == null || EditRoute.RouteId != _route.RouteId) {
                    EditRoute = _route;
                    $rootScope.$broadcast('EditRouteChanged', EditRoute);
                    if (openDialog) {
                        $("#Dialog_Route").dialog("option", "title", EditRoute == null ? "Add a Route" : "Edit a Route");
                        $("#Dialog_Route").dialog("open");
                    }
                }
            },
            EditRoute_Completed: function (result) {
                $("#Dialog_Route").dialog("close");
                if (result == "success")
                    $rootScope.$broadcast('SAScheduleChanged', Schedule);
            },
            SetRouteStation: function (_station) {
                if (RouteStation == null || _station == null || RouteStation.RouteStationId != _station.RouteStationId) {
                    RouteStation = _station;
                    $rootScope.$broadcast('RStationChanged', RouteStation);
                }
            },
            GetCurRouteStation: function () {
                return RouteStation;
            },
            SetEditRouteStation: function (_station, openDialog) {
                openDialog = typeof openDialog !== 'undefined' ? openDialog : true;
                if (EditStation == null || _station == null || EditStation.RouteStationId != _station.RouteStationId) {
                    EditStation = _station;
                    $rootScope.$broadcast('EditRStationChanged', EditStation);
                    if (openDialog) {
                        $("#Dialog_RStation").dialog("option", "title", EditStation == null ? "Add a Route Station" : "Edit a Route Station");
                        $("#Dialog_RStation").dialog("open");
                    }
                }
            },
            EditRouteStation_Completed: function (result) {
                $("#Dialog_RStation").dialog("close");
                if (result == "success")
                    $rootScope.$broadcast('SARouteChanged', Route);
            },
            AlertMsg: function (msg) {
                alert(msg);
            },
            ConfirmMsg: function (msg) {
                return window.confirm(msg);
            },
            SendImportFile: function (groupId, command, overlap, file, successfun) {
                var data = new FormData();
                if (file != null) {
                    data.append('overlap', overlap);
                    data.append('file', file, file.name);
                }
                data.append('groupId', groupId);
                data.append('command', command);
                $.ajax({
                    url: "ImportSchedule.ashx?",
                    data: data,
                    processData: false,
                    contentType: false,
                    type: 'POST',
                    success: successfun
                });
                /*
                xhr = new XMLHttpRequest();

                // When the request starts.
                xhr.onloadstart = function () {
                console.log('Factory: upload started: ', file.name);
                };

                // When the request has failed.
                xhr.onerror = function (e) {
                };

                // Send to server, where we can then access it with $_FILES['file].
                xhr.open('POST', 'ImportSchedule.ashx?');
                xhr.send(data);
                */
            }
        };
    })
    .filter('ListOpenIcon', function () {
        return function (input) {
            return input ? "img/minus.gif" : "img/plus.gif";
        };
    })
    .filter('TimeString', function () {
        return function (input) {
        };
    })
    .directive('fileChange', function () {
        return function ($scope, elem, attrs) {
            elem.bind('change', function (event) {
                if (event.target.files.length <= 0) return;
                var file = event.target.files[0];
                $scope.File = file;
                $scope.FileName = file.name;
                $scope.$apply(attrs.fileChange);
            });
        };
    })
    .directive('integer', function () {
        var INTEGER_REGEXP = /^\-?\d*$/;
        return {
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                ctrl.$parsers.unshift(function (viewValue) {
                    if (INTEGER_REGEXP.test(viewValue)) {
                        // it is valid
                        ctrl.$setValidity('integer', true);
                        return viewValue;
                    } else {
                        // it is invalid, return undefined (no model update)
                        ctrl.$setValidity('integer', false);
                        return undefined;
                    }
                });
            }
        };
    }); 

    function SAAppCtrl($scope, saService) {
        var date = new Date();
        var tt = date.getTime();
        $scope.DepotURL = "Partials/DepotList.htm?tt="+tt;
        $scope.ScheduleGroupURL = "Partials/ScheduleGroupList.htm?tt=" + tt;
        $scope.GroupEditURL = "Partials/ScheduleGroupEdit.htm?tt=" + tt;
        $scope.GroupCopyURL = "Partials/ScheduleGroupCopy.htm?tt=" + tt;
        $scope.ScheduleCopyURL = "Partials/ScheduleCopy.htm?tt=" + tt;
        $scope.RouteListURL = "Partials/RouteList.htm?tt=" + tt;
        $scope.RouteEditURL = "Partials/RouteEdit.htm?tt=" + tt;
        $scope.RStationEditURL = "Partials/RouteStationEdit.htm?tt=" + tt;
        $scope.RouteStationListURL = "Partials/RouteStationList.htm?tt=" + tt;
        $scope.GroupImportURL = "Partials/GroupImport.htm?tt=" + tt; ;
        $scope.DialogGroupImportLoaded = function () {
            $("#Dialog_GroupImport").dialog({
                autoOpen: false,
                height: 500,
                width: 450,
                modal: true,
                resizable: true,
                close: function () {
                }
            });
            $("#Dialog_GroupImport").on("dialogclose", function (event, ui) {
                saService.SetImportGroup(null, false);
            });
        };
        $scope.DialogGroupLoaded = function () {
        $("#Dialog_Group").dialog({
            autoOpen: false,
            height: 500,
            width: 450,
            modal: true,
            resizable: true,
            close: function () {
            }
        });
        $("#Dialog_Group").on("dialogclose", function (event, ui) {
            saService.SetEditGroup(null, false);
        });
    };
    $scope.DialogGroupCopyLoaded = function () {
        $("#Dialog_GroupCopy").dialog({
            autoOpen: false,
            height: 450,
            width: 450,
            modal: true,
            resizable: true,
            close: function () {
            }
        });
        $("#Dialog_GroupCopy").on("dialogclose", function (event, ui) {
            saService.SetCopyGroup(null, false);
        });
    };
    $scope.DialogScheduleCopyLoaded = function () {
        $("#Dialog_ScheduleCopy").dialog({
            autoOpen: false,
            height: 250,
            width: 400,
            modal: true,
            resizable: true,
            close: function () {
            }
        });
        $("#Dialog_ScheduleCopy").on("dialogclose", function (event, ui) {
            saService.SetCopySchedule(null, false);
        });
    };
    $scope.DialogRouteLoaded = function () {
        $("#Dialog_Route").dialog({
            autoOpen: false,
            height: 550,
            width: 500,
            modal: true,
            resizable: true,
            close: function () {
            }
        });
        $("#Dialog_Route").on("dialogclose", function (event, ui) {
            saService.SetEditRoute(null, false);
        });
    };
    $scope.DialogRStationLoaded = function () {
        $("#Dialog_RStation").dialog({
            autoOpen: false,
            height: 500,
            width: 450,
            modal: true,
            resizable: true,
            close: function () {
            }
        });
        $("#Dialog_RStation").on("dialogclose", function (event, ui) {
            saService.SetEditRouteStation(null, false);
        });
    };
};

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

