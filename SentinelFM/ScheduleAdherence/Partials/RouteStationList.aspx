<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RouteStationList.aspx.cs" Inherits="ScheduleAdherence_Partials_RouteStationList" %>
<div ng-controller="SARouteStationCtrl">
<div>
    <!-- Search Route Station content-->
    Station:<input ng-model="filterForStation.StationName" />
</div>
<br />
<div>
    <div class="buttonbar">
        <a href="" ng-click="EditRouteStation(0)" class="imgbutton">
            <span class="imgbuttonicon imgbuttonnew">New</span>
        </a> 
        <a href="" ng-hide="CanEditStation()" ng-click="EditRouteStation()" class="imgbutton">
            <span class="imgbuttonicon imgbuttonedit">Edit</span>
        </a> 
        <a href="" ng-hide="CanDeleteStation()" ng-click="DeleteRouteStation()" class="imgbutton">
            <span class="imgbuttonicon imgbuttondelete">Delete</span>
        </a> 
    </div>    
    <div ng-show="IsStationInit()">Please select a Route.</div>
    <div ng-show="IsStationNoData()">No Data.</div>
    <div ng-show="IsStationLoading()">Loading....</div>
    <div ng-show="HaveStationData()">
   <!--Station body-->
    <table>
        <tr class="extable">
            <th>Station</th>
            <th>Station Number</th>
            <th>Arrival On</th>
            <th>Departure On</th>
            <th>Delivery Type</th>
            <th>Description</th>
        </tr>
        <tr class="extable" ng-class="{extable_current:IsCurrentStation({{rs.RouteStationId}})}" ng-click="SelectRouteStation({{rs.RouteStationId}})"
            ng-repeat="rs in StationList | filter:filterForRoute:strict | orderBy:'Name'">
            <td>{{rs.StationName}}</td>
            <td>{{rs.DeliveryType}}</td>
            <td>{{rs.ArrivalTime| date:'medium'}}</td>
            <td>{{rs.DepartureTime| date:'medium'}}</td>
            <td>{{rs.DeliveryType}}</td>
            <td>{{rs.Description}}</td>
        </tr>
        <thead></thead>
    </table>
    </div>
</div>
</div>
