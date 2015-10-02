var saStationApp = angular.module('saStationApp', ['ui.bootstrap'])
.service('saService', function ($rootScope, $http) {
})
.filter('ListOpenIcon', function () {
    return function (input) {
        return input ? "img/minus.gif" : "img/plus.gif";
    };
});

function FireClick(id) {
    if (typeof (event) != "undefined") {
        document.getElementById(id).click();
    }
    else {
        var eClick = document.createEvent("MouseEvents");
        eClick.initEvent('click', true, true);
        document.getElementById(id).dispatchEvent(eClick);
    }
}