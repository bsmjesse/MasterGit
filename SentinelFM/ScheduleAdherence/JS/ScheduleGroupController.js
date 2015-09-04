var ScheduleGroupCtrl = function ($scope, $modal, $http, $filter, saService) {
    $scope.InitController = function () {
        $scope.GroupList = null;
        $scope.Setting = null;
        $scope.GetSetting();
        $scope.Status = "Loading";
    };

    $scope.InitData = function (depot) {
        $scope.InitController();
        saService.SetSchedule(null);
        if (depot != null)
            $scope.GetGroupListByDepotId(depot.StationId);
    };

    $scope.GetSetting = function () {
        var s = saService.GetSASetting();
        if (s != null)
            $scope.Setting = s;
        else
            $http.post('ScheduleData.ashx?Req=GetSetting').success(function (data) {
                if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
                $scope.Setting = data.Object[0];
                if (window.console) console.log('$scope.Setting=' + $scope.Setting.ImportFormat);
            });
    };

    $scope.$on('DepotChanged', function (event, depot) {
        $scope.InitData(depot);
    });

    $scope.GetGroupListByDepotId = function (depotId) {
        $scope.CurGroup = null;
        $http.post('ScheduleData.ashx?Req=GetGroupList&DepotId=' + depotId).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) {
                $scope.Status = "NoData";
                return;
            }
            $scope.Status = "Data";
            angular.forEach(data.Object, function (row) {
                row.IsOpen = false;
                row.Status = "Loading";
                //row.ScheduleBeginDate = new Date(row.ScheduleBeginDate);
                row.ScheduleBeginDate = parseDate(row.ScheduleBeginDate)
                var endDate = new Date(row.ScheduleBeginDate);
                endDate.setDate(row.ScheduleBeginDate.getDate() + row.Duration - 1);
                row.ScheduleEndDate = endDate;

                row.Tag = '';
                for (var day = 0; day < row.Duration; day++) {
                    var date = new Date(row.ScheduleBeginDate);
                    date.setDate(row.ScheduleBeginDate.getDate() + day);
                    row.Tag += $filter('date')(date, 'mediumDate');
                }
                row.ScheduleList = null;
                row.GetScheduleById = function (scheduleId) {
                    var schedule = null;
                    angular.forEach(row.ScheduleList, function (item) {
                        if (item.ScheduleId == scheduleId)
                            schedule = item;
                    });
                    return schedule;
                };
            });
            $scope.GroupList = data.Object;
        });
    };

    $scope.GetGroupById = function (groupId) {
        var group = null;
        if (groupId != 0)
            angular.forEach($scope.GroupList, function (item) {
                if (item.GroupId == groupId)
                    group = item;
            });
        return group;
    };

    $scope.CanEditGroup = function (groupId) {
        return false;
    };
    $scope.CanDeleteGroup = function (groupId) {
        var group = $scope.GetGroupById(groupId);
        var today = new Date();
        return group.ScheduleBeginDate < today;
    };

    $scope.CanImportGroup = function (groupId) {
        //    if (window.console) console.log('CanImportGroup=' + groupId);
        if ($scope.Setting == null || $scope.Setting.ImportFormat == null) return false;
        var group = $scope.GetGroupById(groupId);
        var today = new Date();
        return group.ScheduleBeginDate > today;
    };

    $scope.IsCurrentSchedule = function (scheduleId) {
        var curSchedule = saService.GetCurSchedule();
        if (curSchedule == null) return false;
        return curSchedule.ScheduleId == scheduleId;
    };

    $scope.IsHiddenButton = function (groupId) {
        var group = $scope.GetGroupById(groupId);
        return !group.IsOpen;
    };

    $scope.SelectGroup = function (groupId) {
        var group = $scope.GetGroupById(groupId);

        if (group.ScheduleList == null)
            $http.post('ScheduleData.ashx?Req=GetScheduleList&GroupId=' + groupId).success(function (data) {
                if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) {
                    group.Status = "NoData";
                    return;
                }
                group.Status = "Data";
                angular.forEach(data.Object, function (row) {
                    row.ScheduleBeginDate = parseDate(row.ScheduleBeginDate);
                    //row.ScheduleBeginDate = row.ScheduleBeginDate.getDate();
                });
                group.ScheduleList = data.Object;
            });
        group.IsOpen = !group.IsOpen;
    };
    $scope.GetScheduleStatus = function (groupId) {
        var group = $scope.GetGroupById(groupId);
        if (group == null) return "";
        return group.Status;
    };
    $scope.IsScheduleNoData = function (groupId) {
        return $scope.GetScheduleStatus(groupId) == "NoData";
    };
    $scope.IsScheduleLoading = function (groupId) {
        return $scope.GetScheduleStatus(groupId) == "Loading";
    };
    $scope.HaveScheduleData = function (groupId) {
        return $scope.GetScheduleStatus(groupId) == "Data";
    };
    $scope.IsGroupNoData = function () {
        return $scope.Status == "NoData";
    };
    $scope.IsGroupLoading = function () {
        return $scope.Status == "Loading";
    };
    $scope.HaveGroupData = function () {
        return $scope.Status == "Data";
    };
    $scope.SelectSchedule = function (groupId, scheduleId) {
        var group = $scope.GetGroupById(groupId);
        var schedule = group.GetScheduleById(scheduleId);
        saService.SetSchedule(schedule);
    };

    $scope.CopySchedule = function (groupId, scheduleId) {
        var group = $scope.GetGroupById(groupId);
        var schedule = group.GetScheduleById(scheduleId);
        __ScheduleCopy_ScheduleList = group.ScheduleList;
        saService.SetCopySchedule(schedule);
    };

    $scope.EditGroup = function (groupId) {
        var group = $scope.GetGroupById(groupId);
        saService.SetEditGroup(group);
    };

    $scope.ImportGroup = function (groupId) {
        var group = $scope.GetGroupById(groupId);
        saService.SetImportGroup(group);
    };

    $scope.CopyGroup = function (groupId) {
        var group = $scope.GetGroupById(groupId);
        saService.SetCopyGroup(group);
    };

    $scope.DeleteGroup = function (groupId) {
        var group = $scope.GetGroupById(groupId);
        if (group == null) return;
        if (!saService.ConfirmMsg("Click Ok to delete this group.")) return;
        var url = SAApp_AjaxBaseURL + "?Req=DeleteGroup&GroupId=" + groupId;
        $http.post(url).success(function (data) {
            if ($scope.CurGroup == group) {
                $scope.CurGroup = null;
                saService.SetScheduleGroup(null);
            }
            saService.RefreshGroupList();
        });
    }

    $scope.InitController();
    var _depot = saService.GetCurDepot();
    if (_depot != null)
        $scope.GetGroupListByDepotId(_depot.StationId);
};

var GroupInstanceCtrl = function ($scope, $modal, $http, saService) {

    $scope.InitController = function () {
        $scope.Group = { GroupId: 0, ScheduleBeginDate: null, Duration: null, Description: '' };
        $scope.DepotId = 0;
        $scope.minDate = new Date();
        $scope.Status = "Init";
    };

    $scope.InitData = function (group) {
        $scope.InitController();
        $scope.DepotId = saService.GetCurDepot().StationId;
        if (group != null)
            $scope.GetGroupById(group.GroupId);
        else
            $scope.Status = "Loaded";
    };

    $scope.$on('EditGroupChanged', function (event, group) {
        $scope.InitData(group);
    });

    $scope.GetGroupById = function (groupId) {
        $http.post('ScheduleData.ashx?Req=GetGroupById&GroupId=' + groupId).success(function (data) {
            if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
            $scope.Group = data.Object[0];
            $scope.Group.ScheduleBeginDate = new Date($scope.Group.ScheduleBeginDate);
            var endDate = new Date();
            endDate.setDate($scope.Group.ScheduleBeginDate.getDate() + $scope.Group.Duration - 1);
            $scope.Group.ScheduleEndDate = endDate;
            $scope.DepotId = $scope.Group.RSCStationId;
            $scope.Status = "Loaded";
        });
    };

    $scope.IsNewInsance = function () {
        if ($scope.Group.GroupId == 0) return true;
        else return false;
    };

    $scope.ok = function () {
        if ($scope.Group.ScheduleBeginDate == null) {
            saService.AlertMsg("Please select a begin date");
            return;
        }
        if ($scope.Group.Duration == null) {
            saService.AlertMsg("Please input duration.");
            return;
        }
        var duration = parseInt($scope.Group.Duration)
        if (isNaN(duration) || duration <= 0){
            saService.AlertMsg("Duration format is wrong.");
            return;
        }
        var url = SAApp_AjaxBaseURL + '?Req=SaveGroup&DepotId=' + $scope.DepotId + "&GroupId=" + $scope.Group.GroupId + "&BeginDate=" + $scope.Group.ScheduleBeginDate.toDateString() + "&Duration=" + $scope.Group.Duration + "&Description=" + $scope.Group.Description;
        $scope.Status = "Pending";
        $http.post(url).success(function (data) {
            $scope.Status = "Loaded";
            saService.EditGroup_Completed("success");
        })
        .error(function () {
            $scope.Status = "Loaded";
            saService.AlertMsg("Save fail, try again.");
        });
    };

    $scope.cancel = function () {
        saService.EditGroup_Completed("cancel");
    };

    $scope.InitController();
};

var GroupCopyCtrl = function ($scope, $modal, $http, saService) {

    $scope.InitController = function () {
        $scope.OrgGroup = null;
        $scope.ScheduleBeginDate = null;
        $scope.minDate = new Date();
        $scope.Status = "Init";
    };

    $scope.InitData = function (group) {
        $scope.InitController();
        if (group != null)
            $scope.OrgGroup = angular.copy(group);
        $scope.Status = "Loaded";
    };

    $scope.$on('CopyGroupChanged', function (event, group) {
        $scope.InitData(group);
    });

    $scope.GetScheduleEndDate = function () {
        if ($scope.OrgGroup == null || $scope.ScheduleBeginDate == null) return null;
        var scheduleEndDate = new Date($scope.ScheduleBeginDate);
        scheduleEndDate.setDate($scope.ScheduleBeginDate.getDate() + $scope.OrgGroup.Duration - 1)
        return scheduleEndDate;
    };

    $scope.ok = function () {
        if ($scope.OrgGroup == null) {
            return;
        }
        if ($scope.ScheduleBeginDate == null) {
            saService.AlertMsg("Please select a begin date");
            return;
        }
        var url = SAApp_AjaxBaseURL + "?Req=CopyGroup&GroupId=" + $scope.OrgGroup.GroupId + "&BeginDate=" + $scope.ScheduleBeginDate.toDateString();
        $scope.Status = "Pending";
        $http.post(url).success(function (data) {
            $scope.Status = "Loaded";
            saService.CopyGroup_Completed("success");
        })
        .error(function () {
            $scope.Status = "Loaded";
            saService.AlertMsg("Save fail, try again.");
        });
    };

    $scope.cancel = function () {
        saService.CopyGroup_Completed("cancel");
    };

    $scope.InitController();
};

var GroupImportCtrl = function ($scope, $modal, $http, $timeout, saService) {
    //Import Status: 1:Init 2:file seleced 3:importing progress 4:Import failed
    $scope.InitController = function () {
        $scope.ImportStatus = 1;
        $scope.Overlap = true;
        $scope.OrgGroup = null;
        $scope.FileName = "No File.";
        $scope.ErrorMessage = "";
        $scope.ProgressMsg = "";
        $scope.ProgressRate = 0;
        $scope.File = null;
    };
    $scope.InitData = function (group) {
        $scope.InitController();
        if (group != null)
            $scope.OrgGroup = angular.copy(group);
    };

    $scope.$on('ImportGroupChanged', function (event, group) {
        $scope.InitData(group);
    });

    $scope.GetScheduleEndDate = function () {
        if ($scope.OrgGroup == null || $scope.ScheduleBeginDate == null) return null;
        var scheduleEndDate = new Date($scope.ScheduleBeginDate);
        scheduleEndDate.setDate($scope.ScheduleBeginDate.getDate() + $scope.OrgGroup.Duration - 1)
        return scheduleEndDate;
    };

    $scope.fileChange = function () {
        $scope.ImportStatus = 2;
    };

    $scope.Upload = function () {
        if ($scope.OrgGroup == null) {
            return;
        }
        if ($scope.File == null) return;
        $scope.ImportStatus = 3;
        $scope.ProgressMsg = "Loading...";
        $scope.ProgressRate = 0;
        saService.SendImportFile($scope.OrgGroup.GroupId, "upload", $scope.Overlap, $scope.File, $scope.Upload_Completed);
        $scope.inquiry();
    };

    $scope.Upload_Completed = function (data) {
        $scope.$apply(function () {
            if (data.Result) {
                $scope.ImportStatus = 1;
                saService.AlertMsg("Upload schedule successfully.");
                saService.SetImportGroup_Completed();
                return;
            }
            $scope.ImportStatus = 4;
            $scope.ErrorMessage = data.Message;
        });
        //$scope.ErrorMessage = "Test2334";
    };

    $scope.Ignore = function () {
        $scope.ErrorMessage = "";
        $scope.ImportStatus = 3;
        saService.SendImportFile($scope.OrgGroup.GroupId, "Ignore", null, null, $scope.Upload_Completed);
        $scope.inquiry();
    };

    $scope.Cancel_Completed = function (data) {
        $scope.$apply(function () {
            $scope.ImportStatus = 1;
        });
    };

    $scope.cancel = function () {
        $scope.ErrorMessage = "";
        $scope.ImportStatus = 3;
        saService.SendImportFile($scope.OrgGroup.GroupId, "cancel", null, null, $scope.Cancel_Completed);
    };

    var inquiry_timer = null;
    $scope.Inquiry_Completed = function (data) {
        $scope.$apply(function () {
            if (data.Result) {
                $scope.ProgressMsg = data.Object.Message;
                $scope.ProgressRate = data.Object.Progress;
                $scope.inquiry();
            }
        });
    };

    $scope.inquiry = function () {
        inquiry_timer = $timeout(function () {
            if ($scope.ImportStatus == 3)
                saService.SendImportFile($scope.OrgGroup.GroupId, "inquiry", null, null, $scope.Inquiry_Completed);
            else {
                $timeout.cancel(inquiry_timer); ;
            }
        }, 500);
    };

    $scope.InitController();
};

var __ScheduleCopy_ScheduleList = [];

var ScheduleCopyCtrl = function ($scope, $modal, $http, saService) {

    $scope.InitController = function () {
        $scope.OrgSchedule = null;
        $scope.DesSchedule = null;
        $scope.ScheduleList = null;
        $scope.Status = "Init";
    };

    $scope.InitData = function (schedule) {
        $scope.InitController();
        $scope.OrgSchedule = schedule;
        $scope.ScheduleList = [];
        if (schedule != null)
            angular.forEach(__ScheduleCopy_ScheduleList, function (row) {
                if (row.ScheduleId != schedule.ScheduleId)
                    $scope.ScheduleList.push(row);
            });
        $scope.Status = "Loaded";
    };

    $scope.$on('CopyScheduleChanged', function (event, group) {
        $scope.InitData(group);
    });

    $scope.ok = function () {
        if ($scope.OrgSchedule == null) {
            return;
        }
        if ($scope.DesSchedule == null) {
            saService.AlertMsg("Please select a date");
            return;
        }
        var url = SAApp_AjaxBaseURL + "?Req=CopySchedule&OrgScheduleId=" + $scope.OrgSchedule.ScheduleId + "&DesScheduleId=" + $scope.DesSchedule.ScheduleId;
        $scope.Status = "Pending";
        $http.post(url).success(function (data) {
            $scope.Status = "Loaded";
            saService.CopySchedule_Completed("success");
        })
        .error(function () {
            $scope.Status = "Loaded";
            saService.AlertMsg("Save fail, try again.");
        });
    };

    $scope.cancel = function () {
        saService.CopySchedule_Completed("cancel");
    };

    $scope.InitController();
};
