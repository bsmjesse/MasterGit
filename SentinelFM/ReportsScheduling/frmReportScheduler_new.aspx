<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportScheduler_new.aspx.cs"
    Inherits="SentinelFM.ReportsScheduling_frmReportScheduler_new" Culture="en-US"
    meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    
    <title></title>
    <link href="../Reports/styles.css" rel="stylesheet" type="text/css" />    
   
    
    </head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" 
        Transparency="0" meta:resourcekey="LoadingPanel1Resource1" Skin ="Hay">
    </telerik:RadAjaxLoadingPanel>

    <script type="text/javascript">
        

        function GetRadWindow()
        {
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
            }
            if (val == "-1") {
                oWnd.BrowserWindow.PopupLogin();
            }

            oWnd.close();
            //alert(val);
            //oWnd.close(oArg);
        }
       

    </script>

    <div>
        <table width ="100%">
           <tr style ="height:20px"  >
               <td align="center" valign ="top">
                      <asp:RadioButtonList ID="radReportType" runat="server" CssClass="formtext"
                          RepeatDirection="Horizontal" meta:resourcekey="radReportTypeResource1">
                          <asp:ListItem Value="0" Text="One Time Report" Selected="True" 
                              meta:resourcekey="radReportTypeListItemResource1"></asp:ListItem>
                          <asp:ListItem Value="2" Text="My Report" 
                              meta:resourcekey="radReportTypeListItemResource3"></asp:ListItem>
                          <asp:ListItem Value="1" Text="Scheduling Report" 
                              meta:resourcekey="radReportTypeListItemResource2"></asp:ListItem>
                       </asp:RadioButtonList>
                </td>
            </tr>
            <tr align ="center" >
                <td>
                   <div id="pnlSchedule"   >
                       <table  >
                          <tr >
                             <td align ="center" >
                               <asp:RadioButtonList ID="lstReportType" runat="server" BorderStyle="Solid" CssClass="formtext"
                                     BorderColor="Black" BorderWidth="1px"  
                                    RepeatDirection="Horizontal" meta:resourcekey="lstReportTypeResource1">
                                    <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource1" Text="Once"></asp:ListItem>
                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="Daily"></asp:ListItem>
                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="Weekly"></asp:ListItem>
                                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource4" Text="Monthly"></asp:ListItem>
                                </asp:RadioButtonList>                             
                             </td>
                          </tr>
                          <tr>
                             <td  valign="top" align="center">
                                <table border="0" style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid;
                                    border-left: #ffffff 1px solid; border-bottom: #ffffff 1px solid; display:none"
                                    id="tblWeekly" runat="server">
                                    <tr>
                                        <td align="left" style="width: 100px">
                                            <asp:Label ID="lblWeekly" runat="server" meta:resourcekey="lblWeeklyResource1" Text="Weekly:" CssClass="formtext"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100px; border-right: black 1px solid; border-top: black 1px solid;
                                            border-left: black 1px solid; border-bottom: black 1px solid;" align="left" valign="top">
                                           <table style="width:<%= ContentWidth %>px">
                                                <tr>
                                                    <td style="width: 100%" colspan="3" align="left">
                                                        <asp:Label ID="lblEvery" runat="server" meta:resourcekey="lblEveryResource1" CssClass="formtext"
                                                            Text="Every"></asp:Label>
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
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <table border="0" style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid;
                                    border-left: #ffffff 1px solid;  border-bottom: #ffffff 1px solid;
                                     display:none" id="tblMonthly" runat="server">
                                    <tr>
                                        <td align="left" style="width: 100px">
                                            <asp:Label ID="lblMonthly" runat="server" meta:resourcekey="lblMonthlyResource1" CssClass="formtext"
                                                Text="Monthly:"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 100px; border-right: black 1px solid; border-top: black 1px solid;
                                            border-left: black 1px solid; border-bottom: black 1px solid;" align="left">
                                            <table style="width: <%= ContentWidth %>px">
                                                <tr>
                                                    <td style="width: 100%" colspan="3" align="left">
                                                        &nbsp;<asp:Label ID="lblDay" runat="server" meta:resourcekey="lblDayResource1" Text="Day" CssClass="formtext"></asp:Label>
                                                        <telerik:RadComboBox ID="cboMonthlyDay" runat="server" CssClass="RegularText" meta:resourcekey="cboMonthlyDayResource1"
                                                            MaxHeight="300px" Skin="Hay" Width = "50px" >
                                                        <items>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource12" Text="1"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource13" Text="2"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource14" Text="3"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource15" Text="4"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource16" Text="5"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource17" Text="6"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource18" Text="7"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource19" Text="8"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource20" Text="9"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource21" Text="10"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource22" Text="11"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource23" Text="12"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource24" Text="13"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource25" Text="14"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource26" Text="15"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource27" Text="16"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource28" Text="17"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource29" Text="18"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource30" Text="19"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource31" Text="20"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource32" Text="21"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource33" Text="22"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource34" Text="23"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource35" Text="24"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource36" Text="25"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource37" Text="26"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource38" Text="27"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource39" Text="28"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource40" Text="29"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource41" Text="30"></telerik:RadComboBoxItem>
                                                            <telerik:RadComboBoxItem meta:resourcekey="ListItemResource42" Text="31"></telerik:RadComboBoxItem>
                                                           </items>
                                                        </telerik:RadComboBox>
                                                        <asp:Label ID="lblOfEveryMonth" runat="server" meta:resourcekey="lblOfEveryMonthResource1" CssClass="formtext"
                                                            Text="of every month" Width="136px"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <table border="0" style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid;
                                    border-left: #ffffff 1px solid; border-bottom: #ffffff 1px solid; "
                                    id="Table1" runat="server" >
                                    <tr>
                                        <td align="left" >
                                            <asp:Label ID="lblOccursAt" runat="server" Text="Delivery method:" CssClass="formtext"
                                                meta:resourcekey="lblOccursAtResource1"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right: black 1px solid; border-top: black 1px solid; border-left: black 1px solid;
                                            border-bottom: black 1px solid;" align="left" valign="top">
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
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:RadioButtonList ID="optDeliveryMethod" runat="server" RepeatDirection="Horizontal" CssClass="formtext"
                                                            meta:resourcekey="optDeliveryMethodResource1" OnSelectedIndexChanged="optDeliveryMethod_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Selected="True" Value="0" meta:resourcekey="ListItemResource58" Text="To Email"></asp:ListItem>
                                                            <asp:ListItem Value="1" meta:resourcekey="ListItemResource59" Text="Store to Disk"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <table id="tblEmail" runat="server" >
                                                            <tr>
                                                                <td style="width: 100px">
                                                                    <asp:Label ID="lblEmail" runat="server" meta:resourcekey="lblEmailResource1" Text="Email:" CssClass="formtext"></asp:Label>
                                                                </td>
                                                                <td style="width: 660px;">
                                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="formtext" Width="220px" meta:resourcekey="txtEmailResource1" ></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 276px;">
                                                                    <asp:RequiredFieldValidator ID="reqEmailValidator" runat="server" Display ="None" ValidationGroup ="vgSchedule"  ErrorMessage="Please fill in e-mail address"
                                                                        ControlToValidate="txtEmail" CssClass="errortext" meta:resourcekey="reqEmailValidatorResource1"></asp:RequiredFieldValidator>
                                                                    <%--<asp:CustomValidator ID="cvEmail" Display ="None" ValidationGroup ="vgSchedule" runat="server" CssClass="errortext" 
                                                                        ErrorMessage="" EnableClientScript="true" ControlToValidate="txtEmail" ClientValidationFunction="validate" meta:resourcekey="reqEmailValidatorResource1"/>       --%>
                                                                    <%--<asp:RegularExpressionValidator ID="emailValidator" runat="server" ControlToValidate="txtEmail" Display ="None" ValidationGroup ="vgSchedule"
                                                                        CssClass="errortext" ErrorMessage="Please enter valid e-mail address" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*?(\s*[,;]\s*\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*?)*"
                                                                        meta:resourcekey="emailValidatorResource1" ></asp:RegularExpressionValidator>--%>
                                                                    <asp:CustomValidator ID="CustomValidator1" runat="server"  ClientValidationFunction="validateEmail" EnableClientScript="true" ValidationGroup = "vgSchedule" 
                                                           ErrorMessage="" CssClass="formtext"/>
                                                                        
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>

                                                <tr>

                                                    <td style="width: 100%" colspan="2">
                                                        <div id="EmailSuggestions" runat="server" align="left">
                                                            <ul style=" list-style-type:inherit; "   >
                                                                <li style="color:blue; ">
                                                         <asp:Label ID="lblSuggestionMultipleEmail" runat="server" meta:resourcekey="lblMultipleEmailValidatorResource1" Text="Multiple email should be separated by comma (,) or semicolon (;)." CssClass="formtext"
                                                            Width="220px" ForeColor="Blue"  ></asp:Label>
                                                                   
                                                                    </li>
                                                                <li style="color:blue">
                                                                     <asp:Label ID="lblSuggestionEmailLength" runat="server" meta:resourcekey="lblEmailLengthValidatorResource1" Text=" Email text should not exceed by 8000 characters." CssClass="formtext"
                                                            Width="220px" Visible="True" ForeColor ="blue"></asp:Label>
                                                                    </li>
                                                                </ul>

                                                        </div>
                                                        
                                                        </td>
                                                    </tr>
                                                    <%--<tr>
                                                    <td style="width: 100%" colspan="2">
                                                        <asp:Label ID="lblSuggestionEmailLength" runat="server" meta:resourcekey="errEmailLengthValidatorResource1" Text=" Email text should not exceed by 8000 characters." CssClass="formtext"
                                                            Width="277px" Visible="True" ForeColor ="blue"></asp:Label>
                                                        </td>
                                                    </tr>--%>
                                                <tr>
                                                    <td style="width: 100%" colspan="2">
                                                        <asp:Label ID="lblNote" runat="server" meta:resourcekey="lblNoteResource1" Text="Note: Report will be delivered  within 4 hours after the scheduled time." CssClass="formtext"
                                                            Width="277px" Visible="False"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                <table border="0" style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid;
                                    border-left: #ffffff 1px solid; border-bottom: #ffffff 1px solid; "
                                    id="Table3" runat="server"  >
                                    <tr>
                                        <td align="left" >
                                            <asp:Label ID="lblReportLifeTime" runat="server" meta:resourcekey="lblReportLifeTimeResource1" CssClass="formtext"
                                                Text="Report life time:"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="border-right: black 1px solid; border-top: black 1px solid; border-left: black 1px solid;
                                            border-bottom: black 1px solid; width: <%= ContentWidth %>px; " align="left" valign="top">
                                            <table>
                                                <tr>
                                                    <td style="width: 80px">
                                                        <asp:Label ID="lblStart" runat="server" meta:resourcekey="lblStartResource1" Text="Start:" CssClass="formtext"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 40px">
                                                        <telerik:RadDatePicker ID="txtFrom" runat="server" Width="182px" DateInput-EmptyMessage=""
                                                            MinDate="1900-01-01" MaxDate="3000-01-01" Skin="Hay" 
                                                            meta:resourcekey="txtFromResource1">
                                                            <Calendar>
                                                                <SpecialDays>
                                                                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" 
                                                                        meta:resourcekey="RadCalendarDayResource1" >
<ItemStyle CssClass="rcToday"></ItemStyle>
                                                                    </telerik:RadCalendarDay>
                                                                </SpecialDays>
                                                                
                                                            </Calendar>

<DateInput DisplayDateFormat="MM/dd/yyyy" DateFormat="MM/dd/yyyy" LabelCssClass=""></DateInput>

<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
                                                        </telerik:RadDatePicker>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tblEndDuration" runat="server" style="display:none">
                                                <tr>
                                                    <td style="width: 80px">
                                                        <asp:Label ID="lblEnd" runat="server" meta:resourcekey="lblEndResource1" Text="End:" CssClass="formtext"></asp:Label>
                                                    </td>
                                                    <td style="width: 40px">
                                                        <telerik:RadDatePicker ID="txtTo" runat="server" Width="182px" DateInput-EmptyMessage=""
                                                            MinDate="1900-01-01" MaxDate="3000-01-01" Skin="Hay" 
                                                            meta:resourcekey="txtToResource1">
                                                            <Calendar>
                                                                <SpecialDays>
                                                                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" 
                                                                        meta:resourcekey="RadCalendarDayResource2" >
<ItemStyle CssClass="rcToday"></ItemStyle>
                                                                    </telerik:RadCalendarDay>
                                                                </SpecialDays>
                                                            </Calendar>

<DateInput DisplayDateFormat="MM/dd/yyyy" DateFormat="MM/dd/yyyy" LabelCssClass=""></DateInput>
<DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
                                                        </telerik:RadDatePicker>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:Label ID="lblDateValidation" runat="server" CssClass="errortext" Width="283px"
                                                meta:resourcekey="lblDateValidationResource1"></asp:Label>
                                            
                                            <asp:CompareValidator Enabled="False" ID="dateValidator" runat="server" 
                                                ControlToCompare="txtTo" ValidationGroup ="vgSchedule"
                                                ControlToValidate="txtFrom" ErrorMessage="Start date must be less or equal end date"
                                                Type="Date" Display ="None"
                                                 Operator="LessThanEqual" CssClass="errortext" 
                                                meta:resourcekey="dateValidatorResource1"></asp:CompareValidator>
                                             <asp:CustomValidator ID="cvDate" runat="server"  ClientValidationFunction="CustomValidateDate" EnableClientScript="true" ValidationGroup = "vgSchedule" 
                                                           Display ="None" ErrorMessage="" CssClass="formtext"
                                                      />

                                        </td>
                                    </tr>
                                </table>
                                 
                              </td>
                          </tr>
                          <tr>
                                <td align="center" >
                                  <table>
                                     <tr>
                                       <td>
                                    <asp:ValidationSummary ID="vs" runat = "server"  ValidationGroup ="vgSchedule" CssClass="formtext"
                                        meta:resourcekey="vsResource1" />

                                       </td>
                                     </tr>
                                  </table>
                                </td>
                           </tr>                      
                           <tr valign ="top">
                
                                <td align="center">
                                      <asp:Button ID="cmdSubmit" runat="server" CssClass="combutton" Text="Submit" OnClick="cmdSubmit_Click" ValidationGroup ="vgSchedule"
                                                   meta:resourcekey="cmdSubmitResource1" />
                                                   &nbsp;
                                      <asp:Button ID="cmdBack" runat="server" CssClass="combutton" Text="Back" OnClick="cmdBack_Click"
                                                   meta:resourcekey="cmdBackResource1" CausesValidation="False" />
                                 </td>
                             </tr>
                           <tr>
                                <td align ="center">
                                    <asp:Label ID="lblMessage" runat="server" CssClass="formtext" meta:resourcekey="lblMessageResource1"
                                        Width="440px"></asp:Label>
                                </td>
                           </tr>

                       </table>
                   </div>
                </td>  
            </tr>

            <tr align ="center" >
                <td>
                   <div id="pnlOneTimeReport" style="display:none"  >
                   <table>
                          <tr>
                                <td align="center" >
                                    <asp:ValidationSummary ID="vsOneTimeReport" runat = "server"  CssClass="formtext"
                                        ValidationGroup ="vgOneTimeReport" meta:resourcekey="vsMyreportResource1" />
                                </td>
                           </tr>                      
                           <tr valign ="top">
                
                                <td align="center">
                                      <asp:Button ID="btnOneTimeReport" runat="server" CssClass="combutton" Text="Submit"  ValidationGroup ="vgOneTimeReport"
                                                   meta:resourcekey="cmdSubmitResource1" OnClick="btnOneTimeReport_Click"  />
                                                   &nbsp;
                                      <asp:Button ID="btnBack" runat="server" CssClass="combutton" Text="Back" OnClick="cmdBack_Click"
                                                   meta:resourcekey="cmdBackResource1" CausesValidation="False" />
                                 </td>
                             </tr>
                  </table>       
                   </div>
                </td>
            </tr>

            <tr align ="center" >
                <td>
                   <div id="pnlMyReport" style="display:none"  >
                   <table>
                          <tr>
                                <td>
                                    <asp:Label id ="lblMyReportName" runat ="server" Text="Name:" meta:resourcekey="lblMyReportNameResource1" CssClass="formtext"></asp:Label> 
                                    <asp:RequiredFieldValidator ID="rvMyReportName" runat="server" ControlToValidate = "txtMyReportName" Display ="None" ValidationGroup ="vgMyReport"  ErrorMessage="Please fill in my report name"
                                                                         CssClass="errortext" meta:resourcekey="rvMyReportNameResource1"   ></asp:RequiredFieldValidator>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMyReportName" runat ="server" Width = "300px" CssClass="formtext" ></asp:TextBox>
                                </td>
                          </tr>
                          <tr>
                                <td>
                                <asp:Label id ="lblMyReportDesc"  runat ="server" Text="Description:" meta:resourcekey="lblMyReportDescResource1" CssClass="formtext"></asp:Label> 
                                </td>
                                <td colspan="10" >
                                <asp:TextBox ID="txtMyReportDesc" runat="server" Width ="100%" CssClass="formtext"></asp:TextBox>
                                </td>
                          </tr>
                          <tr>
                                <td align="center" colspan ="2" >
                                    <asp:ValidationSummary ID="vsmyReport" runat = "server"  
                                        ValidationGroup ="vgMyReport" meta:resourcekey="vsMyreportResource1" CssClass="formtext" />
                                </td>
                           </tr>                      
                           <tr valign ="top">
                
                                <td align="center" colspan ="2">
                                                                      <asp:Button ID="btnMyReportSubmit" runat="server" CssClass="combutton" Text="Submit"  ValidationGroup ="vgMyReport"
                                                   meta:resourcekey="cmdSubmitResource1" OnClick="btnMyReport_Click"  />
                                                   &nbsp;
                                      <asp:Button ID="btnMyReportBack" runat="server" CssClass="combutton" Text="Back" OnClick="cmdBack_Click"
                                                   meta:resourcekey="cmdBackResource1" CausesValidation="False" />
                                 </td>
                             </tr>
                  </table>       
                   </div>
                </td>
            </tr>
        </table>
    </div>
         <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server" >
            <script type="text/javascript">
        //Select report type
        function EnableSchduleControls(ctl, speed)
        {
            ctl = "input[name=" + ctl + "]:checked";
            var value = $telerik.$(ctl).val();
            if ( value == "1")
            {
                $telerik.$('#pnlSchedule').fadeIn(speed);
                $telerik.$('#pnlOneTimeReport').fadeOut(speed);
                $telerik.$('#pnlMyReport').fadeOut(speed);
                
              EnableTypeControls('<%= lstReportType.ClientID %>')
              //GetRadWindow().SetHeight(500);
              GetRadWindow().SetWidth("<%= WindowWidth %>");
            }
          if (value == "0") {
              $telerik.$('#pnlOneTimeReport').fadeIn(speed);
              $telerik.$('#pnlSchedule').fadeOut(speed);
              $telerik.$('#pnlMyReport').fadeOut(speed);
              GetRadWindow().SetHeight(170 - 30);
              GetRadWindow().SetWidth("<%= WindowWidth %>");
          }
          if (value == "2") {
              $telerik.$('#pnlMyReport').fadeIn(speed);
              $telerik.$('#pnlSchedule').fadeOut(speed);
              $telerik.$('#pnlOneTimeReport').fadeOut(speed);
              GetRadWindow().SetHeight(250 - 30);
              GetRadWindow().SetWidth("<%= WindowWidth %>");
          }

          //return true;

        }

        //Select report type in schedule report
        function EnableTypeControls(ctl)
        {
            ctl = "input[name=" + ctl + "]:checked";
            var value = $telerik.$(ctl).val();

            if (value == "0") {
                EnableControls(false, false, false);
                GetRadWindow().SetHeight(430 - 30);
            }
             if (value == "1") {
                 EnableControls(false, false, true);
                 GetRadWindow().SetHeight(460 - 30);
             }
             if (value == "2") {
                 EnableControls(true, false, true);
                 GetRadWindow().SetHeight(500 - 30);
             }
             if (value == "3") {
                 EnableControls(false, true, true);
                 GetRadWindow().SetHeight(500 - 30);
             }
             
        }
        

        function EnableControls(weeklyVisible, monthlyVisible, endEnabled)
        {
          if (weeklyVisible)
          {
            $telerik.$('#<%=tblWeekly.ClientID %>').show();
          }
          else $telerik.$('#<%=tblWeekly.ClientID %>').hide();
          
          if (monthlyVisible)
          {
            $telerik.$('#<%=tblMonthly.ClientID %>').show();
          }
          else $telerik.$('#<%=tblMonthly.ClientID %>').hide();

          if (endEnabled)
          {
            $telerik.$('#<%=tblEndDuration.ClientID %>').show();
          }
          else $telerik.$('#<%=tblEndDuration.ClientID %>').hide();
          return true;  
        }
        EnableSchduleControls('<%= radReportType.ClientID %>', 'fast');

        //Validate all input date
        function CustomValidateDate(sender, args) {
            args.IsValid = true;
            var calendar = $find("<%= txtFrom.ClientID %>");
            if (calendar.get_selectedDate() == null) {
                args.IsValid = false;
                sender.errormessage = "<%= errresFillStartDate %>"
                return; //Start date is required
            }
            else {
                var selected = calendar.get_selectedDate();
                var hours = '<%= Hours %>';
                if (hours != '0') {
                    var hrs = parseInt(hours); //* 24 * 60 * 60 * 1000;
                    selected.setHours(selected.getHours() + hrs); 
                }
                if (selected < new Date()) {
                    args.IsValid = false;
                    sender.errormessage = "<%= errresValidStartDate %>"
                    return; //Start date should be greater than current date
                }
            }

            var ctl = "input[name=" + '<%= lstReportType.ClientID %>' + "]:checked";
            var value = $telerik.$(ctl).val();

            if (value != "0") {
                var calendarTo = $find("<%= txtTo.ClientID %>");
                if (calendarTo.get_selectedDate() == null) {
                    args.IsValid = false;
                    sender.errormessage = "<%= errresFillEndDate %>"
                    return; 
                }

                if (calendarTo.get_selectedDate() != null && calendar.get_selectedDate() != null) {
                    if (calendar.get_selectedDate() > calendarTo.get_selectedDate()) {
                                           args.IsValid = false;
                                           sender.errormessage = "<%= errdateValidatorResource1 %>";
                    }
                }
            }

            return;
        }
                function validate(sender, args) {
                    var list = document.getElementById("optDeliveryMethod"); //Client ID of the radiolist
                    var inputs = list.getElementsByTagName("input");
                    var selected;
                    for (var i = 0; i < inputs.length; i++) {
                        if (inputs[i].checked) {
                            selected = inputs[i];
                            break;
                        }
                    }
                    if (selected) {
                        alert(selected.value);
                    }
                    return;
                }

                function validateEmail(sender, args)
                {                    
                    var email = document.getElementById("txtEmail").value;
                    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                    if (email.length != 0) {
                    if (email.length > 8000)
                    {
                        args.IsValid = false;
                        sender.errormessage = "<%= errEmailLengthValidatorResource1%>";                       
                        return;
                        
                    }

                   
                        if (email.indexOf(';') > -1) {
                            email = email.replace(/;/g, ",");
                        }

                        if (email.indexOf(',') > -1) {

                            if (email != '') {
                                result = email.split(',');

                                for (var i = 0; i < result.length; i++) {
                                    if (result[i] != '') {

                                        if (!re.test(result[i])) {
                                            args.IsValid = false;
                                            sender.errormessage = "<%=errEmailValidatoreWithEmailTextResource1 %>" + " " + result[i] + " " + "<%= errInvalidEmailTextResource1 %>";
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            if (!re.test(email)) {
                                args.IsValid = false;
                                sender.errormessage =  "<%= errValidEmailValidatoreResource1%>";
                                return;
                            }
                        }
                    }
                   
                    
                    
                }

            </script>
         </telerik:RadCodeBlock>
    </form>
</body>
</html>
