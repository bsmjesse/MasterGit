<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RouteEdit.aspx.cs" Inherits="ScheduleAdherence_Partials_RouteEdit" %>
<div ng-controller="RouteInstanceCtrl">
    <div class="modal-body">
         <table width="100%">
            <tr class="InstanceRow" >
                <td>Route Name:</td>
                <td><input type="text" ng-model="Route.Name" required /></td>
            </tr>
            <tr class="InstanceRow">
                <td>Vehicle:</td>
                <td><select ng-model="Route.Vehicle" ng-change="VehicleChange(v)" ng-options="v.Description for v in Vehicles" required></select></td>
            </tr>
            <tr class="InstanceRow">
                <td>Depot Departure:</td>
                <td>
                  <div ng-model="Route.RSCDepartureTime" style="display:inline-block;" required >
                    <timepicker show-meridian="ismeridian"></timepicker>
                  </div>    
                </td>
            </tr>
            <tr class="InstanceRow">
                <td>Depot Arrival:</td>
                <td>
                  +<select ng-model="Route.ArrivalDayInterval" ng-change="ArrivalDateChange()" ng-options="d for d in DayIntervals"></select>Days
                  <span ng-model="Route.RSCArrivalTime" required>
                    <timepicker show-meridian="ismeridian"></timepicker>
                  </span>
                </td>
            </tr>
            <tr class="InstanceRow">
                <td>Description:</td>
                <td><input type="text" ng-model="Route.Description" ng-required="true"/>
            </tr>
         </table>
    </div>        
    <div class="modal-footer">
        <button class="btn btn-primary" ng-click="ok()">OK</button>
        <button class="btn btn-warning" ng-click="cancel()">Cancel</button>
    </div>
</div>