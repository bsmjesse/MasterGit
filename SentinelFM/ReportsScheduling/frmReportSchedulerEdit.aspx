<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportSchedulerEdit.aspx.cs"
    Inherits="SentinelFM.ReportsScheduling_frmReportSchedulerEdit" Culture="en-US"
    meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Report Schedule Editor</title>
    <%--<link href="../Reports/styles.css" rel="stylesheet" type="text/css" />--%>
    <style type="text/css">
    body
    {
        background-color: White;
        border-style: double double double double;
        border-color: Gray Gray Gray Gray;
        border-width: 4px 4px 4px 4px;
        font-family: Calibri, Verdana, Tahoma, Consolas; 
        font-size: 12px;
        color: inherit;
        width: 500px;
        text-align: center;
        }
    input .EntryTextBox
    {
         border-style: solid;
         border-color: #bebebe;
         border-width: 1px;
        }
    td
    {
        text-align: left;
        padding-left: 2px;
        }
    .EntryTable
    {
        border-style: solid;
        border-color: Black;
        border-width: 1px;
        padding-top: 5px;
        padding-bottom: 5px;
        margin-top: 5px;
        margin-bottom: 5px;
        width: 400px;
        }
     .Title
     {
         font-weight: bold;
         height: 30px;
         text-align: left;
         padding-left: 2px;
         padding-top: 12px;
         vertical-align: bottom;
         }
     .Labels
     {
         height: 30px;
         text-align: left;
         padding-left: 2px;
         width: 400px;
         vertical-align: bottom;
         }
    .labeltext
    {
        height: 18px;
        vertical-align: bottom;
        text-align: left;
        }
    .BorderPanel
    {
        border: #CCCCCC 1px solid; 
        text-align: left;
        padding-left: 6px;
        width: 400px;
        border-color:#bcbcbc;
        border-style:solid;
        border-width:1px;
        }
    .BorderTable
    {
        border: #CCCCCC 1px solid; 
        border-color:#bcbcbc;
        border-style:solid;
        border-width:1px;
        width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" >

    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Transparency="0" meta:resourcekey="LoadingPanel1Resource1"></telerik:RadAjaxLoadingPanel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server" >
    <script type="text/javascript">

        //Select report type
        function EnableSchduleControls(index, speed)            //(ctl, speed) 
        {
//            GetRadWindow().SetWidth(iWindowWidth);              //"WindowWidth %>");
//            GetRadWindow().SetHeight(iWindowHight);

            $telerik.$('#pnlSchedule').fadeIn(speed);
            EnableTypeControls('<%= lstReportType.ClientID %>');
            
            var oWnd = GetRadWindow();
            
            oWnd.SetWidth(iWindowWidth);              //"WindowWidth %>");
            oWnd.SetHeight(iWindowHight);
            oWnd.Center();
        }

        //Select report type in schedule report
        function EnableTypeControls(ctl) 
        {
            ctl = "input[name=" + ctl + "]:checked";

            var value = $telerik.$(ctl).val();

            value = (value >= 0 && value <= 3) ? value : ScheculeIndex;       //  defIndex.toString();

            if (value == "0" || value == 0) {
                EnableControls(false, false, false);        //                    GetRadWindow().SetHeight(iWindowHight);         // (530 - 30);
            }
            if (value == "1" || value == 1) {
                EnableControls(false, false, true);         //                    GetRadWindow().SetHeight(iWindowHight + 30);    //(560 - 30);
            }
            if (value == "2" || value == 2) {
                EnableControls(true, false, true);          //                    GetRadWindow().SetHeight(iWindowHight + 70);    //(600 - 30);
            }
            if (value == "3" || value == 3) {
                EnableControls(false, true, true);          //                    GetRadWindow().SetHeight(iWindowHight + 70);    //(600 - 30);
            }
        }

        // Enable Controls
        function EnableControls(weeklyVisible, monthlyVisible, endEnabled) 
        {
            if (weeklyVisible)
                $telerik.$('#<%=tblWeekly.ClientID %>').show();
            else 
                $telerik.$('#<%=tblWeekly.ClientID %>').hide();

            if (monthlyVisible)
                $telerik.$('#<%=tblMonthly.ClientID %>').show();
            else 
                $telerik.$('#<%=tblMonthly.ClientID %>').hide();

            if (endEnabled)
                $telerik.$('#<%=tblEndDuration.ClientID %>').show();
            else
                $telerik.$('#<%=tblEndDuration.ClientID %>').hide();

            return true;
        }

        //Validate all input date
        function CustomValidateDate(sender, args) {

            args.IsValid = true;
        
//            var calendar = $find("<%= txtFrom.ClientID %>");
        
//            if (calendar.get_selectedDate() == null) {
//                args.IsValid = false;
//                sender.errormessage = "<%= errresFillStartDate %>"
//                return; //Start date is required
//            }
//            else {
//                var selected = calendar.get_selectedDate();
//                var hours = '<%= Hours %>';
//                if (hours != '0') {
//                    var hrs = parseInt(hours); //* 24 * 60 * 60 * 1000;
//                    selected.setHours(selected.getHours() + hrs);
//                }
//                if (selected < new Date()) {
//                    args.IsValid = false;
//                    sender.errormessage = "<%= errresValidStartDate %>"
//                    return; //Start date should be greater than current date
//                }
//            }

//            var ctl = "input[name=" + '<%= lstReportType.ClientID %>' + "]:checked";
//            var value = $telerik.$(ctl).val();

//            if (value != "0") {
//                var calendarTo = $find("<%= txtTo.ClientID %>");
//                if (calendarTo.get_selectedDate() == null) {
//                    args.IsValid = false;
//                    sender.errormessage = "<%= errresFillEndDate %>"
//                    return;
//                }

//                if (calendarTo.get_selectedDate() != null && calendar.get_selectedDate() != null) {
//                    if (calendar.get_selectedDate() > calendarTo.get_selectedDate()) {
//                        args.IsValid = false;
//                        sender.errormessage = "<%= errdateValidatorResource1 %>";
//                    }
//                }
//            }

            return;
        }

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        //Return value:
        // val - -1 means no login, 1 means one time report, 2 means schedule report, 3 means my report
        function returnToParent(val)
        {
            //var oArg = new Object();
            //oArg.value = val;
            var oWnd = GetRadWindow();
            if (val == "1" || val == "2" || val == "3") {
                oWnd.BrowserWindow.PopupSubmit(val);
                if (val == 2 && oWnd.BrowserWindow.rebindScheduleGrid != null)
                    oWnd.BrowserWindow.rebindScheduleGrid(false);
            }
            if (val == "-1") {
                oWnd.BrowserWindow.PopupLogin();
            }

            oWnd.close();
            this.window.close();

            //alert(val);
            //oWnd.close(oArg);
        }

        var iWindowHight = 640;
        var iWindowWidth = 540;
        var ScheculeIndex = <%= piSelectedScheduleTypeIndex %>;

        $telerik.$(document).ready(function() { 
            EnableSchduleControls(ScheculeIndex, 'fast');  //= radReportType.ClientID
        });

    </script>
    </telerik:RadCodeBlock>
    <center>
    <div style="text-align: center; width:400px; margin: 6px 6px 6px 6px;">
        <div id="divScheduleInfo">
            <div class="Title">
                Report Schedule Detail
            </div>
            <div id="pnlScheduleDetail" class="BorderPanel">
                <table width="100%" style="padding: 6px 6px 6px 6px;">
                    <tr id="trScheduleID">
                        <td style="width:80px; padding-left:6px;">
                            <asp:label ID="lblSchedule" runat="server" Text="Schedule ID: " CssClass="lblFormat" />
                        </td>
                        <td>
                            <asp:Label ID="txtSchedule" runat="server" Text="" CssClass="formtext" /> 
                        </td>
                    </tr>
                    <tr>
                        <td style="width:80px; padding-left:6px;">
                            <asp:label ID="lblReport" runat="server" Text="Report Name: " CssClass="lblFormat" />
                        </td>
                        <td>
                            <asp:Label ID="txtReportName" runat="server" Text="" CssClass="formtext" /> 
                        </td>
                    </tr>
                    <tr>
                        <td style="width:80px; padding-left:6px;">
                            <asp:Label ID="lblUser" Text="Created by: " CssClass="formtext" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="txtUser" Text="" CssClass="formtext" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width:80px; padding-left:6px;">
                            <asp:Label ID="lblStatue" Text="Status: " CssClass="formtext" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="txtStatus" Text="" CssClass="formtext" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="divFormat">
            <div class="Title">
                Report Format
            </div>
            <div id="pnlFormat" class="BorderPanel">
                <asp:RadioButtonList ID="rblFormat" CssClass="formtext" RepeatDirection="Horizontal" runat="server" Width="80%">
                    <asp:ListItem Value="1" Text="PDF" />
                    <asp:ListItem Value="2" Text="Excel" />
                    <asp:ListItem Value="3" Text="Word" />
                </asp:RadioButtonList>
            </div>
        </div>
        <div id="divSchedule">
            <div class="Title">
                Schedule Type
            </div>
            <div id="pnlSchedule" class="BorderPanel">
                <asp:RadioButtonList ID="lstReportType" CssClass="formtext" RepeatDirection="Horizontal" runat="server" Width="80%">
                    <asp:ListItem Value="0" meta:resourcekey="ListItemResource1" Text="Once"></asp:ListItem>
                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="Daily"></asp:ListItem>
                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="Weekly"></asp:ListItem>
                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource4" Text="Monthly"></asp:ListItem>
                </asp:RadioButtonList>
                <div id="pnlDates"  style="margin-top:6px;">
                <table id="tblWeekly" border="0" width="100%" style="display:none; padding-top:1px;" runat="server">
                    <tr>
                        <td style="width:80px; padding-left:6px;">
                            <asp:Label ID="lblWeekly" runat="server" meta:resourcekey="lblWeekResource1" Text="Weekly:" Width="60px"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" meta:resourcekey="lblWeekResource2" CssClass="formtext" Text="on "></asp:Label>
                            &nbsp;
                            <telerik:RadComboBox ID="cboWeekDay" runat="server" Width ="80px" meta:resourcekey="cboWeekDayResource1" CssClass="formtext"
                                Skin="Hay">
                                <Items>
                                    <telerik:RadComboBoxItem Value="1" meta:resourcekey="ListItemResource5" Text="Mon">
                                    </telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem Value="2" meta:resourcekey="ListItemResource6" Text="Tue">
                                    </telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem Value="3" meta:resourcekey="ListItemResource7" Text="Wed">
                                    </telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem Value="4" meta:resourcekey="ListItemResource8" Text="Thur">
                                    </telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem Value="5" meta:resourcekey="ListItemResource9" Text="Fri">
                                    </telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem Value="6" meta:resourcekey="ListItemResource10" Text="Sat">
                                    </telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem Value="0" meta:resourcekey="ListItemResource11" Text="Sun">
                                    </telerik:RadComboBoxItem>
                                </Items>
                            </telerik:RadComboBox>
                            &nbsp;
                            <asp:Label ID="Label2" runat="server" meta:resourcekey="lblWeekResource3" CssClass="formtext" Text=" of every week"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table id="tblMonthly" border="0" width="100%" style="display:none; padding-top:1px;" runat="server">
                    <tr>
                        <td style="width:80px; padding-left:6px;">
                            <asp:Label ID="lblMonthly" runat="server" meta:resourcekey="lblMonthResource1" CssClass="formtext" Text="Monthly:" Width="60px"></asp:Label>
                        </td>
                        <td align="left">
                            <asp:Label ID="lblDay" runat="server" meta:resourcekey="lblMonthResource2" Text="on the " CssClass="formtext"></asp:Label>
                            &nbsp;
                            <telerik:RadComboBox ID="cboMonthlyDay" runat="server" CssClass="RegularText" meta:resourcekey="cboMonthlyDayResource1" MaxHeight="300px" Skin="Hay" Width = "50px" >
                                <items>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource12" Text="1st"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource13" Text="2nd"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource14" Text="3rd"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource15" Text="4th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource16" Text="5th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource17" Text="6th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource18" Text="7th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource19" Text="8th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource20" Text="9th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource21" Text="10th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource22" Text="11th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource23" Text="12th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource24" Text="13th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource25" Text="14th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource26" Text="15th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource27" Text="16th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource28" Text="17th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource29" Text="18th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource30" Text="19th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource31" Text="20th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource32" Text="21th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource33" Text="22th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource34" Text="23th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource35" Text="24th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource36" Text="25th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource37" Text="26th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource38" Text="27th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource39" Text="28th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource40" Text="29th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource41" Text="30th"></telerik:RadComboBoxItem>
                                    <telerik:RadComboBoxItem meta:resourcekey="ListItemResource42" Text="31th"></telerik:RadComboBoxItem>
                                </items>
                            </telerik:RadComboBox>
                            &nbsp;
                            <asp:Label ID="lblOfEveryMonth" runat="server" meta:resourcekey="lblMonthResource3" CssClass="formtext" Text=" of every month"></asp:Label>
                         </td>
                    </tr>
                </table>
                <table id="tblYearly" border="0" class="BorderTable" style="display:none;" runat="server" >
                    <tr>
                        <td>
                            <table style="width: <%= ContentWidth %>px">
                                <tr>
                                    <td style="width: 7px">
                                        <telerik:RadComboBox ID="cboOccursHour" runat="server" AppendDataBoundItems="True"
                                            CssClass="RegularText" Width="41px" meta:resourcekey="cboOccursHourResource1"
                                            Visible="False" Skin="Hay">
                                            <Items>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource43" Text="0"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource44" Text="1"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource45" Text="2"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource46" Text="3"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource47" Text="4"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource48" Text="5"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource49" Text="6"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource50" Text="7"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource51" Text="8"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource52" Text="9"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource53" Text="10"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource54" Text="11"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource55" Text="12"></telerik:RadComboBoxItem>
                                            </Items>
                                        </telerik:RadComboBox>
                                    </td>
                                    <td style="width: 89px">
                                        <telerik:RadComboBox ID="cboOccursHoursType" runat="server" CssClass="formtext" Skin="Hay"
                                            meta:resourcekey="cboOccursHoursTypeResource1" Visible="False">
                                            <Items>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource56" Text="AM"></telerik:RadComboBoxItem>
                                                <telerik:RadComboBoxItem meta:resourcekey="ListItemResource57" Text="PM"></telerik:RadComboBoxItem>
                                            </Items>
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                </div>                             
            </div>
        </div>
        <div id="divDelivery">
            <div class="Title">
                Delivery method
            </div>
            <div id="pnlDelivery" class="BorderPanel">
            <table style="width: 400px">
                <tr>
                    <td align="left" class="style2">
                        <asp:RadioButtonList ID="optDeliveryMethod" runat="server" 
                            RepeatDirection="Horizontal" CssClass="formtext"
                            meta:resourcekey="optDeliveryMethodResource1" Width="287px" >
                            <asp:ListItem Selected="True" Value="0" meta:resourcekey="ListItemResource58" Text="To Email"></asp:ListItem>
                            <asp:ListItem Value="1" meta:resourcekey="ListItemResource59" Text="Store to Disk"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="style2">
                        <table id="tblEmail" runat="server">
                            <tr>
                                <td>
                                    <asp:Label ID="lblEmail" runat="server" meta:resourcekey="lblEmailResource1" Text="Email:" Width="40px" CssClass="formtext"></asp:Label>
                                </td>
                                <td style="width: 209px">
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="EntryTextBox"  Width="220px" meta:resourcekey="txtEmailResource1"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            </div>
            <div id="pnlDeliveryValidator">
                <asp:Label ID="lblNote" runat="server" meta:resourcekey="lblNoteResource1" Text="Note: Report will be delivered  within 4 hours after the scheduled time." CssClass="formtext"
                            Width="440px" Visible="False"></asp:Label>
                <asp:RequiredFieldValidator ID="reqEmailValidator" runat="server" Display ="None" ValidationGroup ="vgSchedule"  ErrorMessage="Please fill in e-mail address"
                    ControlToValidate="txtEmail" CssClass="errortext" meta:resourcekey="reqEmailValidatorResource1"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="emailValidator" runat="server" ControlToValidate="txtEmail" Display ="None" ValidationGroup ="vgSchedule"
                    CssClass="errortext" ErrorMessage="Please enter valid e-mail address" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                    meta:resourcekey="emailValidatorResource1"></asp:RegularExpressionValidator>

            </div>
        </div>
        <div id="divPeriod">
            <div class="Title">
                Report life time
            </div>
            <div id="pnlPeriod" class="BorderPanel">
                <table id="tblStartDuration" runat="server" width="100%" style="padding-left: 10px;">
                    <tr>
                        <td style="width:80px;padding-left:10px;">
                            <asp:Label ID="lblStart" runat="server" meta:resourcekey="lblStartResource1" Text="Start:" CssClass="formtext" Width="60px"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadDatePicker ID="txtFrom" runat="server" Width="182px" DateInput-EmptyMessage="" Skin="Hay" meta:resourcekey="txtFromResource1">
                                <Calendar ID="Calendar1" runat="server">
                                    <SpecialDays>
                                        <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" meta:resourcekey="RadCalendarDayResource1" >
                                            <ItemStyle CssClass="rcToday"></ItemStyle>
                                        </telerik:RadCalendarDay>
                                    </SpecialDays>
                                </Calendar>
                                <DateInput LabelCssClass="" runat="server"></DateInput>
                                <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
                            </telerik:RadDatePicker>
                        </td>
                    </tr>
                </table>
                <table id="tblEndDuration" runat="server" width="100%" style="display:none; padding-left: 10px;">
                    <tr>
                        <td style="width:80px;padding-left:10px;">
                            <asp:Label ID="lblEnd" runat="server" meta:resourcekey="lblEndResource1" Text="End:" CssClass="formtext" Width="60px" />
                        </td>
                        <td>
                            <telerik:RadDatePicker ID="txtTo" runat="server" Width="182px" DateInput-EmptyMessage="" Skin="Hay" meta:resourcekey="txtToResource1">
                                <Calendar ID="Calendar2" runat="server">
                                    <SpecialDays>
                                        <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" meta:resourcekey="RadCalendarDayResource2" >
                                            <ItemStyle CssClass="rcToday"></ItemStyle>
                                        </telerik:RadCalendarDay>
                                    </SpecialDays>
                                </Calendar>
                                <DateInput LabelCssClass="" runat="server"></DateInput>
                                <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
                            </telerik:RadDatePicker>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="pnlPeriodValidator">
                <asp:Label ID="lblDateValidation" runat="server" CssClass="errortext" Width="283px" meta:resourcekey="lblDateValidationResource1" />
                <br />
                <asp:CompareValidator Enabled="False" ID="dateValidator" runat="server" 
                    ControlToCompare="txtTo" ValidationGroup ="vgSchedule"
                    ControlToValidate="txtFrom" ErrorMessage="Start date must be less or equal end date"
                    Type="Date" Display ="None"
                    Operator="LessThanEqual" CssClass="errortext" 
                    meta:resourcekey="dateValidatorResource1"></asp:CompareValidator>
                <asp:CustomValidator ID="cvDate" runat="server"  ClientValidationFunction="CustomValidateDate" EnableClientScript="true" ValidationGroup = "vgSchedule" Display ="None" ErrorMessage="" CssClass="formtext"/>
                <asp:ValidationSummary ID="vs" runat = "server"  ValidationGroup="vgSchedule" CssClass="formtext" meta:resourcekey="vsResource1" Width="100%" />
            </div>
        </div>
    </div>
    <div id="divCommand" style="height:60px; margin-top: 10px;">
        <div id="pnlCommand" align="center">
            <asp:Button ID="cmdSubmit" Text="Submit" OnClick="cmdSubmit_Click" CssClass="combutton" runat="server" ValidationGroup ="vgSchedule" Width="80px" meta:resourcekey="cmdSubmitResource1" />
            &nbsp;
            <asp:Button ID="cmdBack" Text="Back" OnClick="cmdBack_Click" CssClass="combutton" runat="server" CausesValidation="False" Width="80px" meta:resourcekey="cmdBackResource1" />
        </div>
        <div id="pnlMessage" align="center">
            <asp:Label ID="lblMessage" runat="server" CssClass="formtext" meta:resourcekey="lblMessageResource1" Visible="false" Width="100%"></asp:Label>
        </div>
    </div>
    </center>
    </form>
</body>
</html>
