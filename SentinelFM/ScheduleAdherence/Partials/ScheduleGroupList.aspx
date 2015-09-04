<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ScheduleGroupList.aspx.cs" Inherits="ScheduleAdherence_Partials_ScheduleGroupList" %>
<div ng-controller="ScheduleGroupCtrl">
<div>
    <!-- Search Group content-->
    Search:<input ng-model="filterForGroup.Description" />
    <span class="buttonbar">
        <a href="" ng-click="EditGroup(0)" class="imgbutton">
            <span class="imgbuttonicon imgbuttonnew">New</span>
        </a> 
    </span>
</div>
<br />
<div>
    <!--Group body-->
    <dl class="exlist"   
        ng-repeat="group in GroupList | filter:filterForGroup:strict | orderBy:'-ScheduleBeginDate'">
        <dt class="exlist-head">
			<img ng-src='{{group.IsOpen | ListOpenIcon}}' ng-click="SelectGroup({{group.GroupId}})" alt="expand" style="cursor:hand" />
            <span class="exlist-head-title">From {{group.ScheduleBeginDate| date:'mediumDate'}} To {{group.ScheduleEndDate|date:'mediumDate'}}</span>
            <span class="buttonbar" ng-class="{exlist_hidden:IsHiddenButton({{group.GroupId}})}">
                <a href="" ng-hide="CanEditGroup({{group.GroupId}})" ng-click="EditGroup({{group.GroupId}})" class="imgbutton">
                    <span class="imgbuttonicon imgbuttonedit">Edit</span>
                </a> 
                <a href="" ng-hide="CanDeleteGroup({{group.GroupId}})" ng-click="DeleteGroup({{group.GroupId}})" class="imgbutton">
                    <span class="imgbuttonicon imgbuttondelete">Delete</span>
                </a> 
            </span>
        </dt>
        <dd class="exlist-body" ng-class="{exlist_hidden:IsHiddenButton({{group.GroupId}})}">
            {{group.Description}}
            <dl class="exlist exlist_Schedule" ng-class="{exlist_current:IsCurrentSchedule({{schedule.ScheduleId}})}" 
                ng-click="SelectSchedule({{group.GroupId}}, {{schedule.ScheduleId}})"
                ng-repeat="schedule in group.ScheduleList | orderBy:'ScheduleBeginDate'">
                <dt class="exlist-head">
                    <span class="exlist-head-title">{{schedule.ScheduleBeginDate| date:'mediumDate'}}</span>
                    <span class="buttonbar">
                        <a href="" class="imgbutton">
                            <span class="imgbuttonicon imgbuttonedit">Copy</span>
                        </a> 
                    </span>
                </dt>
            </dl>
        </dd>
    </dl>
</div>
</div>