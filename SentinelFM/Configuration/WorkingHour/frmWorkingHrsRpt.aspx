<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmWorkingHrsRpt.aspx.cs" Inherits="SentinelFM.Configuration_WorkingHour_frmWorkingHrsRpt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register src="ctlWorkingHrsl.ascx" tagname="ctlWorkingHrsl" tagprefix="uc2" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css" >
         .NoTDBorder
         {
             border-left-width:0px !important;
         }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
        Style="text-decoration: underline">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btmOK">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlAll" />
                    <telerik:AjaxUpdatedControl ControlID="pnlCon"  LoadingPanelID="LoadingPanel1"  />
                </UpdatedControls>
               
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="text-align: left; width: 900px">
    <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300" >
        <tr>
            <td>
                <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="btnWorkinghrs" />
            </td>
        </tr>
        <tr>
            <td>
                <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                    <tr>
                        <td>
                            <table id="tblForm" border="0" cellpadding="0" cellspacing="0" class="table" style="height: 500px;
                                    width: 990px;">
                                <tr>
                                    <td class="configTabBackground" >
                                        <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px;
                                                margin-top: 5px; position: relative; top: 0px" >
                                            <tr align="center">
                                                <td valign="top" >
                                                                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <uc2:ctlWorkingHrsl ID="ctlWorkingHrsl1" runat="server" selectedcontrol="cmdReport" />
                                                                                                                                        <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760"
                                                                        style="padding-bottom: 5px">

                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;"
                                                                                    class="tableDoubleBorder">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 950px;
                                                                                                margin-top: 2px; height: 480px">
                                                                                                <tr valign="top">
                                                                                                    <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                    <table runat="server" id="pnlAll" width="600px">
                                                        <tr>
                                                            <td>
                                                                <table runat="server" id="pnlCon" >
                                                                    <tr valign="top">
                                                                        <td align="left">
                                                                            <asp:Label ID="lblFleet" runat="server" CssClass="formtext" Width="40px" meta:resourcekey="lblFleetResource1"
                                                                                Text="Fleet:"></asp:Label>
                                                                        </td>
                                                                        <td style="width: 312px;" align="left">
                                                                            <telerik:RadComboBox ID="cboFleet" runat="server" CssClass="RegularText" Width="258px" Filter="Contains" MarkFirstMatch="True" 
                                                                                DataTextField="FleetName" DataValueField="FleetId" meta:resourcekey="cboFleetResource1"
                                                                                Skin="Hay"  MaxHeight="200px" 
                                                                                >
                                                                            </telerik:RadComboBox>
                                                                            <asp:RequiredFieldValidator ID="rvcboFleet" runat="server" ControlToValidate="cboFleet"
                                                                                CssClass="errortext" ErrorMessage="" Display="Dynamic"
                                                                                Text="<br/>Please Select a Fleet." ValidationGroup="Add" meta:resourcekey="rvcboFleetResource1"></asp:RequiredFieldValidator>
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                        <td class="formtext">
                         <asp:DropDownList ID="ddlDateTime" runat = "server" class="formText"  meta:resourcekey="ddlDateTimeResource1"
                               >
                            <asp:ListItem Text="One Week"     Value = "1" Selected="True" ></asp:ListItem>
                            <asp:ListItem Text="Two Weeks"  Value = "2" ></asp:ListItem>
                            <asp:ListItem Text="One Month"    Value = "3" ></asp:ListItem>
                            <asp:ListItem Text="Two Months" Value = "4" ></asp:ListItem>
                            <asp:ListItem Text="Three Months"  Value = "5" ></asp:ListItem>
                            <asp:ListItem Text="Six Months" Value = "6" ></asp:ListItem>
                            <asp:ListItem Text="One Year"      Value = "7" ></asp:ListItem>
                            <asp:ListItem Text="Two Years"   Value = "8" ></asp:ListItem>
                            <asp:ListItem Text="All"           Value = "9" ></asp:ListItem>
                         </asp:DropDownList>
                                                                        </td>
                                                                        <td>
                                                                            &nbsp;
                                                                        </td>
                                                                        <td>
                                                                           <asp:Button ID="btmOK" runat ="Server" Text ="View"   CssClass="combutton"
                                                                                meta:resourcekey="btmOKResource1" ValidationGroup="Add" onclick="btmOK_Click" /> 
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                           <td>
                                                               <iframe id="iframePdf" runat="server" width="850px" height ="500px" visible="false"  >
                                                               </iframe>
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
            </td>
        </tr>
    </table>
    </div>
    </form>
</body>
</html>
