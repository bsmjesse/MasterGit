<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ScheduleGroupEdit.aspx.cs" Inherits="ScheduleAdherence_Partials_ScheduleGroupEdit" %>
 <div ng-controller="GroupInstanceCtrl">
    <div class="modal-body">
            <table>
            <tr class="InstanceRow">
                <td>Begin Date:</td>
                <td><input type="text" ng-disabled="1" ng-model="Group.ScheduleBeginDate" /></td>
            </tr>
            <tr class="InstanceRow">
                <td></td>
                <td>
                <div ng-model="Group.ScheduleBeginDate">
                    <datepicker min="minDate" show-weeks="showWeeks"></datepicker>
                </div>
                </td>
            </tr>
            <tr class="InstanceRow">
                <td>Duration:</td>
                <td><input type="text" ng-model="Group.Duration" ng-required="true"/></td>
            </tr>
            </table>
    </div>        
    <div class="modal-footer">
        <button class="btn btn-primary" ng-click="ok()">OK</button>
        <button class="btn btn-warning" ng-click="cancel()">Cancel</button>
    </div>
</div>