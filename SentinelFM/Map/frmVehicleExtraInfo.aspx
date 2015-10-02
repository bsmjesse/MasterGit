<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleExtraInfo.aspx.cs" Inherits="SentinelFM.frmVehicleExtraInfo"  %>

<%@ Register assembly="ISNet.WebUI.WebGrid" namespace="ISNet.WebUI.WebGrid" tagprefix="ISWebGrid" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <style type="text/css">
        .style1
        {
            width: 137px;
        }
        .formtext
        {
            height: 340px;
            width: 400px;
        }
    </style>
    <script type="text/javascript" src="../Scripts/Telerik_AddIn.js" ></script>
    </head>
<body>
    <form id="frmVehicleAttr" runat="server">
    <div>
        <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
            left: 15px; width: 423px; position: absolute; top: 11px; height: 137px" width="423">
            <tr>
                <td style="height: 11px">
                    <table id="tblButtons" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdSensorCommands" runat="server" CausesValidation="False" CssClass="confbutton"
                                     
                                    Text="Commands" OnClick="cmdSensorCommands_Click" 
                                    /></td>
                            <td>
                                <asp:Button ID="cmdVehicleInfo" runat="server" CausesValidation="False" CssClass="confbutton"
                                     Text="Vehicle Info" OnClick="cmdVehicleInfo_Click" 
                                     /></td>
                            <td><asp:Button ID="cmdSettings" Text="Settings" runat="server" 
                                    CausesValidation="False" CssClass="confbutton" onclick="cmdSettings_Click"
                                    /></td>
                                    
                                    <td><asp:Button ID="cmdServices" Text="Services" runat="server" 
                                            CausesValidation="False" CssClass="confbutton" onclick="cmdServices_Click"
                                    /></td>
                                    
                                    <td><asp:Button ID="cmdUnitInfo" Text="Unit Info" runat="server" CausesValidation="False" CssClass="selectedbutton"
                                    /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                     <table id="tblBody" style="width: 513px; height: 284px" cellspacing="0" cellpadding="0"
                         align="left" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" style="border-right: gray 2px solid; border-top: gray 2px solid;
                                    border-left: gray 2px solid; width: 490px; border-bottom: gray 2px solid;
                                    height: 265px" border="0">
                                    <tr>
                                        <td align="center" class="configTabBackground" valign=middle width="95%"  >
                                        
                                            <table cellpadding="2" class="formtext">
                                                <tr>
                                                    <td class="style1">
                                                        <ISWebGrid:WebGrid ID="dgBoxInfo" runat="server" Height="250px" 
                                                            UseDefaultStyle="True"  Width="480px" 
                                                            oninitializedatasource="dgBoxInfo_InitializeDataSource">
                                                            <roottable>
                                                                <Columns>
                                                                    <ISWebGrid:WebGridColumn Caption="Date" DataMember="Timestamp" Name="Date" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="Message" DataMember="BoxMsgInTypeName" Name="BoxMsgInTypeName" Width="100px"/>
                                                                    <ISWebGrid:WebGridColumn Caption="Data" DataMember="CustomProp"  Width="270px"/>
                                                                </Columns>
                                                            </roottable>
                                                            <layoutsettings allowexport="Yes" allowfilter="Default" autofitcolumns="True"/>
                                                        </ISWebGrid:WebGrid>
                                                   </td>
                                                </tr>
                                                <tr>
                                                   
                                                    <td colspan="2" align=center style="height:40px "     >
                                                                                                                
                                                        <asp:Button ID="cmdClose" runat="server" CssClass="combutton" Text="Close" OnClientClick="WinClose()"  />
                                                    </td>
                                                </tr>
                                                
                                                
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                                        <asp:Label ID="lblVehicleId" runat="server" Visible="False"></asp:Label>
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
