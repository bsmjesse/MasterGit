<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RouteList.aspx.cs" Inherits="ScheduleAdherence_Partials_RouteList" %>

<div ng-controller="SARouteCtrl">
<div>
    <!-- Search Route content-->
    Search:<input ng-model="filterForRoute.Name" />
</div>
<br />
<div>
    <div class="buttonbar">
        <a href="" ng-click="EditRoute(0)" class="imgbutton">
            <span class="imgbuttonicon imgbuttonnew">New</span>
        </a> 
        <a href="" ng-hide="CanEditRoute()" ng-click="EditRoute()" class="imgbutton">
            <span class="imgbuttonicon imgbuttonedit">Edit</span>
        </a> 
        <a href="" ng-hide="CanDeleteRoute()" ng-click="DeleteRoute()" class="imgbutton">
            <span class="imgbuttonicon imgbuttondelete">Delete</span>
        </a> 
    </div>
    <!--Route body-->
    <table>
        <tr class="extable">
            <th>Route</th>
            <th>Vehicle</th>
            <th>Departure On</th>
            <th>Arrival On</th>
            <th>Description</th>
            <th></th>
            <th></th>
        </tr>
        <tr class="extable" ng-class="{extable_current:IsCurrentRoute({{route.RouteId}})}" ng-click="SelectRoute({{route.RouteId}})" 
            ng-repeat="route in RouteList | filter:filterForRoute:strict | orderBy:'Name'">
            <td>{{route.Name}}</td>
            <td>{{route.VehicleName}}</td>
            <td>{{route.RSCDepartureTime| date:'medium'}}</td>
            <td>{{route.RSCArrivalTime| date:'medium'}}</td>
            <td>{{route.Description}}</td>
        </tr>
        <thead></thead>
    </table>
</div>
</div>
