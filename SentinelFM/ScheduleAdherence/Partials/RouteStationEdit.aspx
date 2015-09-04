<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RouteStationEdit.aspx.cs" Inherits="ScheduleAdherence_Partials_RouteStationEdit" %>

<div ng-controller="RouteStationInstanceCtrl">
    <div class="modal-body">
         <table width="100%">
            <tr class="InstanceRow">
                <td>Station:</td>
                <td><select ng-model="Station" ng-options="s.Name for s in Stations" required></select></td>
            </tr>
            <tr class="InstanceRow">
                <td>Depot Arrival:</td>
                <td>
                  +<select ng-model="ArrivalDayInterval" ng-options="d for d in DayIntervals"></select>Days
                  <span ng-model="RouteStation.ArrivalTime" required>
                    <timepicker show-meridian="ismeridian"></timepicker>
                  </span>
                </td>
            </tr>
            <tr class="InstanceRow">
                <td>Departure:</td>
                <td>
                   +<select ng-model="DepDayInterval" ng-options="d for d in DayIntervals"></select>Days
                  <span ng-model="RouteStation.DepartureTime" required>
                    <timepicker show-meridian="ismeridian"></timepicker>
                  </span>
                </td>
                </td>
            </tr>
            <tr class="InstanceRow">
                <td>Delivery Type:</td>
                <td><select ng-model="DeliveryType" ng-options="s.Name for s in DeliveryTypes" required></select></td>
            </tr>
            <tr class="InstanceRow">
                <td>Description:</td>
                <td><input type="text" ng-model="RouteStation.Description" ng-required="true"/>
            </tr>
         </table>
    </div>        
    <div class="modal-footer">
        <button class="btn btn-primary" ng-click="ok()">OK</button>
        <button class="btn btn-warning" ng-click="cancel()">Cancel</button>
    </div>
</div>