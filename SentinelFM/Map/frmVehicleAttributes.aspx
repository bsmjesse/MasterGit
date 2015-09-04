<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleAttributes.aspx.cs" Inherits="SentinelFM.Map_frmVehicleAttributes" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
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
                                    meta:resourcekey="cmdSensorCommandsResource1" /></td>
                            <td>
                                <asp:Button ID="cmdVehicleInfo" runat="server" CausesValidation="False" CssClass="confbutton"
                                     Text="Vehicle Info" OnClick="cmdVehicleInfo_Click" 
                                    meta:resourcekey="cmdVehicleInfoResource1" /></td>
                            <td><asp:Button ID="cmdAttributes" runat="server" CausesValidation="False" CssClass="selectedbutton"
                                     Text="Settings" meta:resourcekey="Button1Resource1" /></td>
                             <td><asp:Button ID="cmdServices" runat="server" CausesValidation="False" CssClass="confbutton"
                                     Text="Services" onclick="cmdServices_Click" CommandName="52" /></td>
                            <td><asp:Button ID="cmdUnitInfo" Text="Unit Info" runat="server" 
                                    CausesValidation="False" CssClass="confbutton" CommandName="53" 
                                    onclick="cmdUnitInfo_Click"/></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" style="width: 413px;
                        height: 284px" width="413">
                        <tr>
                            <td style="height: 281px">
                                <table id="tblForm" border="0" style="border-right: gray 2px solid; border-top: gray 2px solid;
                                    border-left: gray 2px solid; width: 421px; border-bottom: gray 2px solid;
                                    height: 450px" width="421">
                                    <tr>
                                        <td align="center" class="configTabBackground" valign=middle  >
                                        
                                        <fieldset >
                                            <table class="formtext">
                                                <tr>
                                                    <td align="left" colspan="3">
                                                        <asp:Label ID="lblBoxId" runat="server" Visible="False" 
                                                            meta:resourcekey="lblBoxIdResource1"></asp:Label>
                                                        <asp:Label ID="lblVehicleId" runat="server" Visible="False" 
                                                            meta:resourcekey="lblVehicleIdResource1"></asp:Label>
                                                        <asp:CheckBoxList ID="chkAttrList" runat="server" CssClass="formtext" 
                                                            meta:resourcekey="chkAttrListResource1">
                                                        </asp:CheckBoxList></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 100px">
                                                        <asp:Button ID="cmdClose" runat="server" CssClass="combutton" 
                                                            OnClientClick="top.close()" Text="Close" meta:resourcekey="cmdCloseResource1" 
                                                             /></td>
                                                    <td style="width: 100px">
                                                    </td>
                                                    <td style="width: 100px">
                                                        <asp:Button ID="cmdSave" runat="server" CssClass="combutton" meta:resourcekey="cmdSaveResource1"
                                                            OnClick="cmdSave_Click" Text="Save" /></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 100px">
                                                    </td>
                                                    <td style="width: 100px">
                                                    </td>
                                                    <td style="width: 100px">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="3">
                                                        <asp:Label ID="lblMsg" runat="server" meta:resourcekey="lblMsgResource1"></asp:Label></td>
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
    
    </div>
    </form>
</body>
</html>
