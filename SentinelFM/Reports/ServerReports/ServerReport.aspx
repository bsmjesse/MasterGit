<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" CodeFile="ServerReport.aspx.cs" Inherits="SentinelFM.ServerReportMain" %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>SSRS Report Demo</title>
    <style>
        
        #Page 
        {
            width:100%;
            text-align: center;
            height:auto;
            border-top: 3px double gray;
         }
        #SideMenu 
        {
            padding-top: 20px; 
            border-right-width: 3px;
            border-right-style: double;
            border-right-color: Gray;
            min-height:1000px;
            min-width: 200px; 
            float:left;     
       }
       
        #MainPanel
        {
            padding-top: 20px; 
            padding-left: 20px;
            width:auto;
            float:left;     
            min-height:1000px;
       }
       .Label
       {
           width: 60px;
           padding-right: 4px;
           }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <center>
   
    <div id="Page">
        <div id="SideMenu">
            <table  cellpadding="0" cellspacing="0" style="background-color: #fcfcfc;">
                <tr>
                    <td align="left">Organization:&nbsp;</td>
                    <td align="left"><asp:Label ID="lblOrganization" runat="server" /></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td align="left">Report:&nbsp;&nbsp;</td>
                    <td align="left" style="background-color:#eeeeee;">
                        <asp:DropDownList ID="ddlReports" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ReportSelectedIndex_Changed" />
                    </td>
                </tr>
                <tr style="visibility:hidden;">
                    <td>Fleet:</td>
                    <td><asp:DropDownList ID="ddlFleet" runat="server"  AutoPostBack="false" /></td>
                </tr>
                <tr style="visibility:hidden;">
                    <td align="left">From:</td>
                    <td><asp:TextBox ID="txtFromDate" runat="server"></asp:TextBox></td>
                </tr>
                <tr style="visibility:hidden;">
                    <td align="left">To:</td>
                    <td><asp:TextBox ID="txtToDate" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td align="left">
                        <asp:Button ID="cmdReport" runat="server" Text="View"  OnClick="ViewReport_Clicked"/>
                        &nbsp;&nbsp;
                        <asp:Button ID="cmdReturn" runat="server" Text="Home"  OnClick="ViewReport_Clicked"/>   
                    </td>
                </tr>
            </table>
        </div>
        <div id="MainPanel">
            <rsweb:ReportViewer ID="ServerReportViewer" runat="server" BorderColor="Beige" Width="100%"></rsweb:ReportViewer>
        </div>
    </div>
    <div style="visibility: hidden;" >
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    </div>
    </center>
    </form>
</body>
</html>
