<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportScheduler.aspx.cs"
    Inherits="SentinelFM.ReportsScheduling_frmReportScheduler" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>
    
    <%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
   TagPrefix="ISWebInput" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SentinelFM</title> 
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="border: gray 2px outset;
                z-index: 101; left: 8px; position: absolute; top: 4px; height: 97%; background-color: #fffff0; width:98%;">
                <tr>
                    <td style="text-align:center">
                        <table style="border: gray 4px double; left: 282px; width: 480px; height: 347px; margin: 20px 20px 20px 20px">
                            <tr>
                                <td align="center" valign="top" style="width: 467px">
                                    <table class="formtext" style="width: 96%;" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="center">
                                                <asp:Label ID="lblSchedulingReport" runat="server" meta:resourcekey="lblSchedulingReportResource1" Text="Scheduling Report"></asp:Label></td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:RadioButtonList ID="lstReportType" runat="server" BorderStyle="Solid" CssClass="formtext"
                                                    AutoPostBack="True" BorderColor="Black" BorderWidth="1px" OnSelectedIndexChanged="lstReportType_SelectedIndexChanged"
                                                    RepeatDirection="Horizontal" Width="384px" meta:resourcekey="lstReportTypeResource1">
                                                    <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource1" Text="Once"></asp:ListItem>
                                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="Daily"></asp:ListItem>
                                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="Weekly"></asp:ListItem>
                                                    <asp:ListItem Value="3" meta:resourcekey="ListItemResource4" Text="Monthly"></asp:ListItem>
                                                </asp:RadioButtonList></td>
                                        </tr>
                                        <tr>
                                            <td rowspan="2" valign="top" align="center">
                                                <table border="0" style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid;
                                                    border-left: #ffffff 1px solid; border-bottom: #ffffff 1px solid; height: 84px"
                                                    id="tblWeekly" runat="server" width="300">
                                                    <tr>
                                                        <td align="left" style="width: 100px">
                                                            <asp:Label ID="lblWeekly" runat="server" meta:resourcekey="lblWeeklyResource1" Text="Weekly:"></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 100px; border-right: black 1px solid; border-top: black 1px solid;
                                                            border-left: black 1px solid; border-bottom: black 1px solid;" align="left" valign="top">
                                                            &nbsp;<table style="width: 170px">
                                                                <tr>
                                                                    <td style="width: 100%" colspan="3" align="left">
                                                                        &nbsp;<asp:Label ID="lblEvery" runat="server" meta:resourcekey="lblEveryResource1" Text="Every"></asp:Label>
                                                                        <asp:DropDownList ID="cboWeekDay" runat="server" CssClass="RegularText" meta:resourcekey="cboWeekDayResource1">
                                                                            <asp:ListItem Value="1" meta:resourcekey="ListItemResource5" Text="Mon"></asp:ListItem>
                                                                            <asp:ListItem Value="2" meta:resourcekey="ListItemResource6" Text="Tue"></asp:ListItem>
                                                                            <asp:ListItem Value="3" meta:resourcekey="ListItemResource7" Text="Wed"></asp:ListItem>
                                                                            <asp:ListItem Value="4" meta:resourcekey="ListItemResource8" Text="Thur"></asp:ListItem>
                                                                            <asp:ListItem Value="5" meta:resourcekey="ListItemResource9" Text="Fri"></asp:ListItem>
                                                                            <asp:ListItem Value="6" meta:resourcekey="ListItemResource10" Text="Sat"></asp:ListItem>
                                                                            <asp:ListItem Value="0" meta:resourcekey="ListItemResource11" Text="Sun"></asp:ListItem>
                                                                        </asp:DropDownList></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table border="0" style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid;
                                                    border-left: #ffffff 1px solid; width: 300px; border-bottom: #ffffff 1px solid;
                                                    height: 84px" id="tblMonthly" runat="server">
                                                    <tr>
                                                        <td align="left" style="width: 100px">
                                                            <asp:Label ID="lblMonthly" runat="server" meta:resourcekey="lblMonthlyResource1" Text="Monthly:"></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 100px; border-right: black 1px solid; border-top: black 1px solid;
                                                            border-left: black 1px solid; border-bottom: black 1px solid;" align="left">
                                                            <table style="width: 232px">
                                                                <tr>
                                                                    <td style="width: 100%" colspan="3" align="left">
                                                                        &nbsp;<asp:Label ID="lblDay" runat="server" meta:resourcekey="lblDayResource1" Text="Day"></asp:Label>
                                                                        <asp:DropDownList ID="cboMonthlyDay" runat="server" CssClass="RegularText" meta:resourcekey="cboMonthlyDayResource1">
                                                                            <asp:ListItem meta:resourcekey="ListItemResource12" Text="1"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource13" Text="2"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource14" Text="3"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource15" Text="4"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource16" Text="5"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource17" Text="6"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource18" Text="7"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource19" Text="8"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource20" Text="9"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource21" Text="10"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource22" Text="11"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource23" Text="12"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource24" Text="13"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource25" Text="14"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource26" Text="15"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource27" Text="16"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource28" Text="17"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource29" Text="18"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource30" Text="19"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource31" Text="20"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource32" Text="21"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource33" Text="22"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource34" Text="23"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource35" Text="24"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource36" Text="25"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource37" Text="26"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource38" Text="27"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource39" Text="28"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource40" Text="29"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource41" Text="30"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource42" Text="31"></asp:ListItem>
                                                                        </asp:DropDownList>
                                                                        <asp:Label ID="lblOfEveryMonth" runat="server" meta:resourcekey="lblOfEveryMonthResource1" Text="of every month" Width="136px"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table border="0" style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid;
                                                    border-left: #ffffff 1px solid; border-bottom: #ffffff 1px solid; height: 84px"
                                                    id="Table1" runat="server" width="300">
                                                    <tr>
                                                        <td align="left" style="width: 100px">
                                                            <asp:Label ID="lblOccursAt" runat="server" Text="Delivery method:"></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="border-right: black 1px solid; border-top: black 1px solid; border-left: black 1px solid;
                                                            border-bottom: black 1px solid;" align="left" valign="top">
                                                            <table style="width: 272px">
                                                                <tr>
                                                                    <td style="width: 7px">
                                                                        <asp:DropDownList ID="cboOccursHour" runat="server" AppendDataBoundItems="True" CssClass="RegularText"
                                                                            Width="41px" meta:resourcekey="cboOccursHourResource1" Visible="False">
                                                                            <asp:ListItem meta:resourcekey="ListItemResource43" Text="0"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource44" Text="1"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource45" Text="2"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource46" Text="3"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource47" Text="4"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource48" Text="5"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource49" Text="6"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource50" Text="7"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource51" Text="8"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource52" Text="9"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource53" Text="10"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource54" Text="11"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource55" Text="12"></asp:ListItem>
                                                                        </asp:DropDownList></td>
                                                                    <td style="width: 89px">
                                                                        <asp:DropDownList ID="cboOccursHoursType" runat="server" CssClass="formtext" 
                                                                            meta:resourcekey="cboOccursHoursTypeResource1" Visible="False">
                                                                            <asp:ListItem meta:resourcekey="ListItemResource56" Text="AM"></asp:ListItem>
                                                                            <asp:ListItem meta:resourcekey="ListItemResource57" Text="PM"></asp:ListItem>
                                                                        </asp:DropDownList></td>
                                                                </tr>
                                                                <tr>
                                                                   
                                                                    <td colspan="2">
                                                                        <asp:RadioButtonList ID="optDeliveryMethod" runat="server" RepeatDirection="Horizontal" meta:resourcekey="optDeliveryMethodResource1" Width="280px">
                                                                            <asp:ListItem Selected="True" Value="0" meta:resourcekey="ListItemResource58" Text="To Email"></asp:ListItem>
                                                                            <asp:ListItem Value="1" meta:resourcekey="ListItemResource59" Text="Store to Disk"></asp:ListItem>
                                                                        </asp:RadioButtonList></td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="2">
                                                                        <table id="tblEmail" runat="server">
                                                                            <tr>
                                                                                <td style="width: 100px">
                                                                                    <asp:Label ID="lblEmail" runat="server" meta:resourcekey="lblEmailResource1" Text="Email:"></asp:Label>
                                                                                </td>
                                                                                <td style="width: 209px">
                                                                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="formtext" Width="220px" meta:resourcekey="txtEmailResource1"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="width: 276px;" colspan="2">
                                                                                    <asp:RequiredFieldValidator ID="reqEmailValidator" runat="server" ErrorMessage="Please fill in e-mail address" Width="273px" ControlToValidate="txtEmail" CssClass="errortext" meta:resourcekey="reqEmailValidatorResource1"></asp:RequiredFieldValidator>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td style="width: 276px" colspan="2">
                                                                                    <asp:RegularExpressionValidator ID="emailValidator" runat="server" ControlToValidate="txtEmail"
                                                                                        CssClass="errortext" ErrorMessage="Please enter valid e-mail address" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                                                                        Width="271px" meta:resourcekey="emailValidatorResource1"></asp:RegularExpressionValidator>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="width: 100%" colspan="2">
                                                                        <asp:Label ID="lblNote" runat="server" meta:resourcekey="lblNoteResource1" 
                                                                            Text="Note: Report will be delivered  within 4 hours after the scheduled time." 
                                                                            Width="277px" Visible="False"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table border="0" style="border-right: #ffffff 1px solid; border-top: #ffffff 1px solid;
                                                    border-left: #ffffff 1px solid; border-bottom: #ffffff 1px solid; height: 84px"
                                                    id="Table3" runat="server" width="300">
                                                    <tr>
                                                        <td align="left" style="width: 294px">
                                                            <asp:Label ID="lblReportLifeTime" runat="server" meta:resourcekey="lblReportLifeTimeResource1" Text="Report life time:"></asp:Label></td>
                                                    </tr>
                                                    <tr>
                                                        <td style="border-right: black 1px solid; border-top: black 1px solid; border-left: black 1px solid;
                                                            border-bottom: black 1px solid; width: 294px; height: 74px;" align="left" valign="top">
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 80px">
                                                                        <asp:Label ID="lblStart" runat="server" meta:resourcekey="lblStartResource1" Text="Start:"></asp:Label>
                                                                    </td>
                                                                   
                                                                    <td align="left" style="width: 40px">
                                                                     <ISWebInput:WebInput ID="txtFrom" runat="server" Width="180px" EditFormat-Type="DateTime" Height="17px" Wrap="Off">
                                                                        <HighLight IsEnabled="True" Type="Phrase" />
				                                                        <DateTimeEditor IsEnabled="True" AccessKey="Space"></DateTimeEditor>
                                                                    </ISWebInput:WebInput>
                                                                  </td>
                                                                </tr>
                                                            </table>
                                                            <table id="tblEndDuration" runat="server">
                                                                <tr>
                                                                    <td style="width: 80px">
                                                                        <asp:Label ID="lblEnd" runat="server" meta:resourcekey="lblEndResource1" Text="End:"></asp:Label>
                                                                    </td>
                                                                    <td style="width: 40px">
                                                                        <ISWebInput:WebInput ID="txtTo" runat="server" Width="180px" EditFormat-Type="DateTime" Height="17px" Wrap="Off">
                                                                            <HighLight IsEnabled="True" Type="Phrase" />
					                                                        <DateTimeEditor IsEnabled="True" AccessKey="Space"></DateTimeEditor>
                                                                        </ISWebInput:WebInput>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <asp:Label ID="lblDateValidation" runat="server" CssClass="errortext" Width="283px" meta:resourcekey="lblDateValidationResource1"></asp:Label>
                                                            <br />
                                                            <asp:CompareValidator Enabled="false" ID="dateValidator" runat="server" ControlToCompare="txtTo"
                                                                ControlToValidate="txtFrom" ErrorMessage="Start date must be less or equal end date"
                                                                Type="Date" Width="286px" Operator="LessThanEqual" CssClass="errortext" meta:resourcekey="dateValidatorResource1">
                                                            </asp:CompareValidator>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <br />
                                                <asp:Button ID="cmdBack" runat="server" CssClass="combutton" Text="Back" OnClick="cmdBack_Click" meta:resourcekey="cmdBackResource1" CausesValidation="False" />&nbsp;
                                                <asp:Button ID="cmdSubmit" runat="server" CssClass="combutton" Text="Submit" OnClick="cmdSubmit_Click" meta:resourcekey="cmdSubmitResource1" /></td>
                                        </tr>
                                        </table>
                                    <asp:Label ID="lblMessage" runat="server" CssClass="formtext" meta:resourcekey="lblMessageResource1" Width="440px"></asp:Label></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
