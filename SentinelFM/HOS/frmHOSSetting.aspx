<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHOSSetting.aspx.cs" Inherits="SentinelFM.HOS_frmHOSSetting" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Configuration/Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>

<%@ Register Src="HosTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />


</head>
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
        <div style="text-align: center">

            <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="300">
                <tr align="left">
                    <td>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs2" runat="server" SelectedControl="btnHOS" />
                    </td>
                </tr>
                <tr align="left">
                    <td>
                        <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="990">
                            <tr>
                                <td>
                                    <uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdSetting" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table id="Table3" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width: 990px;">
                                        <tr>
                                            <td class="configTabBackground">
                                                <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                    <tr>
                                                        <td>
                                                            <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                            <tr>
                                                                                <td>
                                                                                    <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;" class="tableDoubleBorder">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <table id="Table5" align="center" border="0" cellpadding="0" cellspacing="0"
                                                                                                    style="width: 950px; height: 495px">
                                                                                                    <tr>
                                                                                                        <td align="center" style="width: 100%; border-color: black" valign="top" border="1" cellpadding="0" cellspacing="0">
                                                                                                            <fieldset style="width: 940px; padding: 5px 5px 5px 5px">
                                                                                                                <legend>
                                                                                                                    <asp:Label ID="lblHOSSetting" Font-Bold="True" CssClass="formtext" runat="server" Text="HOS Settings:" meta:resourcekey="lblHOSSettingResource1"></asp:Label>

                                                                                                                </legend>
                                                                                                                <table style="border-color: lightgray;">
                                                                                                                    <tr align="left">
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblScanBoardCodeRequired" runat="server" CssClass="formtextGreen" Text="Scan Barcode Required:" meta:resourcekey="lblScanBoardCodeRequiredResource1"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:RadioButtonList ID="radScanBoardCode" runat="server" CssClass="formtext"
                                                                                                                                Enabled="true" RepeatDirection="Horizontal"
                                                                                                                                meta:resourcekey="radScanBoardCodeResource1">
																<asp:ListItem Value="0" meta:resourcekey="radScanBoardCodeResource1Resource2" selected = 'yes'>No</asp:ListItem>
                                                                                                                                <asp:ListItem Value="1" meta:resourcekey="radScanBoardCodeResource1Resource1">Yes</asp:ListItem>
                                                                                                                                
                                                                                                                            </asp:RadioButtonList>
                                                                                                                        </td>

                                                                                                                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblInspectionDays" runat="server" CssClass="formtextGreen" Text="Inspection History Day:" meta:resourcekey="lblInspectionDaysResource1"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:TextBox ID="txtInspectionDay" runat="server" Width="30px" CssClass="formtext"></asp:TextBox>
<asp:RangeValidator ID="rvtxtInspectionDay" 

ControlToValidate="txtInspectionDay"

MinimumValue="1"

MaximumValue="10"

Type="Integer"

EnableClientScript="true"
    CssClass="errortext"
Text="Value must be from 1 to 10."

runat="server" meta:resourcekey="rvtxtInspectionDayResource1" />
                                                                                                                        </td>


                                                                                                                    </tr>
                                                                                                                    <tr  align="left">
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblInspectionAmount" runat="server" CssClass="formtextGreen" Text="Inspection History Amount:" meta:resourcekey="lblInspectionAmountResource1"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:TextBox ID="txtInspectionAmount" runat="server" Width="30px" CssClass="formtext"></asp:TextBox>
<asp:RangeValidator ID="rvtxtInspectionAmount" 

ControlToValidate="txtInspectionAmount"

MinimumValue="1"
    CssClass="errortext"
MaximumValue="20"

Type="Integer"

EnableClientScript="true"

Text="Value must be from 1 to 20!"

runat="server" meta:resourcekey="rvtxtInspectionAmountResource1" />

                                                                                                                        </td>

                                                                                                                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblImageLimit" runat="server" CssClass="formtextGreen" Text="Image Limit:" meta:resourcekey="lblImageLimitResource1"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:TextBox ID="txtImageLimit" runat="server" Width="30px" CssClass="formtext"></asp:TextBox>
                                                                                                                            <asp:RangeValidator ID="rvtxtImageLimit" 

ControlToValidate="txtImageLimit"

MinimumValue="1"
    CssClass="errortext"
MaximumValue="5"

Type="Integer"

EnableClientScript="true"

Text="Value must be from 1 to 5!"

runat="server" meta:resourcekey="txtImageLimitResource1" />
                                                                                                                        </td>


                                                                                                                    </tr>

                                                                                                                    <tr  align="left">
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblQuestionLevel" runat="server" CssClass="formtextGreen" Text="Question Set Level:" meta:resourcekey="lblQuestionLevelResource1"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:TextBox ID="txtQuestionLevel" runat="server" Width="30px" CssClass="formtext"></asp:TextBox>
 <asp:RangeValidator ID="rvtxtQuestionLevel" 

ControlToValidate="txtQuestionLevel"

MinimumValue="1"
    CssClass="errortext"
MaximumValue="10"

Type="Integer"

EnableClientScript="true"

Text="Value must be from 1 to 10!"

runat="server" meta:resourcekey="rvtxtQuestionLevelResource1" />
                                                                                                                        </td>

                                                                                                                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblViolationThreshold" runat="server" CssClass="formtextGreen" Text="Pre Alert Violation Threshold:" meta:resourcekey="lblViolationThresholdResource1"></asp:Label> 
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:TextBox ID="txtViolationThreshold" runat="server" Width="30px" CssClass="formtext" text="0"></asp:TextBox> Minutes
                                                                                                                            <asp:RangeValidator ID="rvtxtViolationThreshold" 

ControlToValidate="txtViolationThreshold"

MinimumValue="0"
    CssClass="errortext"
MaximumValue="600"

Type="Integer"

EnableClientScript="true"

Text="Value must be from 0 to 600!"

runat="server" meta:resourcekey="rvtxtViolationThresholdResource1" />
                                                                                                                        </td>


                                                                                                                    </tr>

                                                                                                                    <tr  align="left">
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblScreenLock" runat="server" CssClass="formtextGreen" Text="Lock Screen While Driving:" meta:resourcekey="lblScreenLockResource1"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:RadioButtonList ID="radScreenLock" runat="server" CssClass="formtext"
                                                                                                                                Enabled="true" RepeatDirection="Horizontal"
                                                                                                                                meta:resourcekey="radScreenLockResource1">
                                                                                                                                <asp:ListItem Value="0" selected = 'yes' meta:resourcekey="radScreenLockResource1Resource2">No</asp:ListItem>
                                                                                                                                <asp:ListItem Value="1" meta:resourcekey="radScreenLockResource1Resource1">Yes</asp:ListItem>                                                                                                                                
                                                                                                                            </asp:RadioButtonList>


                                                                                                                        </td>

                                                                                                                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblInputOdometer" runat="server" CssClass="formtextGreen" Text="Allow Input Odometer:" meta:resourcekey="lblInputOdometerResource1"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:RadioButtonList ID="radInputOdometer" runat="server" CssClass="formtext"
                                                                                                                                Enabled="true" RepeatDirection="Horizontal"
                                                                                                                                meta:resourcekey="radInputOdometerResource1">
																<asp:ListItem Value="0" selected = 'yes' meta:resourcekey="radInputOdometerResource1Resource1">No</asp:ListItem>
                                                                                                                                <asp:ListItem Value="1" meta:resourcekey="radInputOdometerResource1Resource2">Yes</asp:ListItem>
                                                                                                                                
                                                                                                                            </asp:RadioButtonList>

                                                                                                                        </td>


                                                                                                                    </tr>

                                                                                                                    <tr  align="left">
                                                                                                                        <td>
                                                                                                                            <asp:Label ID="lblDefaultScreen" runat="server" CssClass="formtextGreen" Text="Default Screen:" meta:resourcekey="lblDefaultScreenResource1"></asp:Label>
                                                                                                                        </td>
                                                                                                                        <td>
                                                                                                                            <asp:RadioButtonList ID="radDefault" runat="server" CssClass="formtext"
                                                                                                                                Enabled="true" RepeatDirection="Horizontal"
                                                                                                                                meta:resourcekey="radInputOdometerResource1">
                                                                                                                                <asp:ListItem Value="0" selected = 'yes' meta:resourcekey="radInputOdometerResource1">Inspection</asp:ListItem>
                                                                                                                                <asp:ListItem Value="1"  meta:resourcekey="radInputOdometerResource2">HOS</asp:ListItem>
                                                                                                                            </asp:RadioButtonList>


                                                                                                                        </td>

                                                                                                                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                        </td>
                                                                                                                        <td> <asp:Label ID="lblPendingManualLog" runat="server" CssClass="formtextGreen" Text="Pending Manual Log Email:" meta:resourcekey="lblPendingManualLogResource1"></asp:Label></td>
                                                                                                                        <td><asp:TextBox ID="txtPendingManualLog" runat="server" Width="30px" CssClass="formtext" text='24'></asp:TextBox> Hours
                                                                                                                                                                                                                                                      <asp:RangeValidator ID="rvtxtPendingManualLog" 

ControlToValidate="txtPendingManualLog"

MinimumValue="0.5"
    CssClass="errortext"
MaximumValue="240"

Type="Double"

EnableClientScript="true"

Text="Value must be from 0.5 to 240!"

runat="server" meta:resourcekey="rvtxtPendingManualLogResource1" />


                                                                                                                        </td>
                                                                                                                    </tr>

                                                                                                                    <tr  >
                                                                                                                        <td colspan="10" align="center">
                                                                                                                            <asp:Label ID="lblError" runat="server" CssClass="errortext"></asp:Label>
                                                                                                                        </td>
                                                                                                                    </tr>
                                                                                                                    <tr>
                                                                                                                        <td colspan="10" align="center">
                                                                                                                            <asp:Button ID="cmdSave" runat="server" Width="114px" CssClass="combutton" Text="Save"  onclientclick="javascript:return RemoveMessage();"
                                                                                                                                CausesValidation="True" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1"></asp:Button>
                                                                                                                        </td>
                                                                                                                    </tr>

                                                                                                                </table>
                                                                                                            </fieldset>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>







        </div>
    </form>
</body>
</html>
    <script type="text/javascript">
      function RemoveMessage()
      {
        return true;
      }    
    </script>
