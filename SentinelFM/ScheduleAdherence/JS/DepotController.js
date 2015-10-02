function DepotListCtrl($scope, $http, $location, saService) {
    $http.post('ScheduleData.ashx?Req=GetDeptList').success(function (data) {
        if (!data.Result || !angular.isArray(data.Object) || data.Object.length == 0) return;
        $scope.DepotList = data.Object;
        $scope.Depot = data.Object[0];
        $scope.DepotChange();
    });

    $scope.DepotChange = function () {
        saService.SetDepot($scope.Depot);
    };
};
