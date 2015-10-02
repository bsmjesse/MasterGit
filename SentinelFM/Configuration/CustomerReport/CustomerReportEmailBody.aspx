<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerReportEmailBody.aspx.cs" Inherits="SentinelFM.Configuration_CustomerReport_CustomerReportEmailBody" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="CustomerReportMenu.ascx" TagName="ctlEquipmentMenu" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<style type="text/css">    

    /* The following CSS needs to be copied to the page to produce textbox-like RadEditor */

    

    .reLeftVerticalSide, 

    .reRightVerticalSide, 

    .reToolZone,

    .reToolCell

    {

        background: white !important;

    }

    

    .reToolCell

    {

        display: none\9 !important; /* for all versions of IE in order to prevent border bottom disappearing */

    }

    

    .reContentCell

    {

        border-width: 0 !important;

    }

    

    .formInput

    {

       border: solid 1px black;

    }

    

    .RadEditor

    {

        filter: chroma(color=c2dcf0);

    }

    

    .reWrapper_corner,

    .reWrapper_center 

    {

        display: none !important; /* for FF */            

    }

    

    td.reWrapper_corner,

    td.reWrapper_center 

    {

        display: block\9 !important; /* for all versions of IE */            

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
            <telerik:AjaxSetting AjaxControlID="btnSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="tblEmail" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

 <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" width="300">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" selectedcontrol="btnCustomReport" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td>
                                <table id="tblForm" border="0" cellpadding="0" cellspacing="0" class="table" style="height: 550px;
                                    width: 990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table2" border="0" cellpadding="0" cellspacing="0" style="left: 10px;
                                                margin-top: 5px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <uc2:ctlEquipmentMenu ID="ctlEquipmentMenu1" runat="server" selectedcontrol="cmpEmailBody" />
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760"
                                                                        style="padding-bottom: 5px">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;"
                                                                                    class="tableDoubleBorder">
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table1" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 950px;
                                                                                                margin-top: 5px; height: 495px">
                                                                                                <tr>
                                                                                                    <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                                                                       <table width="100%" id="tblEmail" runat="server" >
    <tr>
        <td colspan="2"  align="center">
<telerik:RadEditor  ToolsWidth="330px" Width="90%" Height="400px" MaxLength="2000" 
         EditModes="Design" ID="txtEmails"     runat="server" ToolbarMode="ShowOnFocus"
         ToolsFile="~/Configuration/CustomerReport/BasicTools.xml">
         </telerik:RadEditor>

            <!-- <asp:TextBox ID="txtEmails1" runat="server" Width="90%" Height="400px" MaxLength="2000" TextMode="MultiLine" ></asp:TextBox> -->
            <asp:RequiredFieldValidator ID="rvtxtEmails"  runat="server" ControlToValidate="txtEmails"
 ValidationGroup="AddEmail" Display="Dynamic" Text="*" ErrorMessage="Email is required."
                 CssClass="errortext"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td colspan="2"  align="center">
            <nobr>
          <asp:Button ID="btnSave"  CssClass="combutton" runat ="server" Text = "Save" 
                meta:resourcekey="btnSaveEmailResource1" Width="80px"  
                ValidationGroup="AddEmail" onclick="btnSave_Click"  />
           &nbsp;
          
          </nobr>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center">
                <table>
                    <tr>
                        <td align="left">
                            <!--for chrome browser -->
                            <asp:ValidationSummary ID="valSum" runat="server" ValidationGroup="AddEmail" DisplayMode="BulletList" />
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
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
