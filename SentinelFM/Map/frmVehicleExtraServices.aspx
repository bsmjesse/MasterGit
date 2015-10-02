<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmVehicleExtraServices.aspx.cs" Inherits="SentinelFM.frmVehicleExtraServices"  %>

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
                                    
                                    <td><asp:Button ID="cmdServices" Text="Services" runat="server" CausesValidation="False" CssClass="selectedbutton"
                                    /></td>
                                    
                                    <td><asp:Button ID="cmdUnitInfo" Text="Unit Info" runat="server" 
                                    CausesValidation="False" CssClass="confbutton" CommandName="53" 
                                    onclick="cmdUnitInfo_Click"/></td>
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
                            
                            
                            <div style="padding : 4px; width : 500px; height : 500px; overflow : auto; ">

                                <table id="tblForm" style="border-right: gray 2px solid; border-top: gray 2px solid;
                                    border-left: gray 2px solid; width: 490px; border-bottom: gray 2px solid;
                                    height: 265px" border="0">
                                    <tr>
                                        <td align="center" class="configTabBackground" valign=middle width="95%"  >
                                        
                                            <table cellpadding="2" class="formtext">
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label1" runat="server" Text="Problem Description:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField1" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label2" runat="server" Text="Contact:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField2" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label3" runat="server" Text="Work Order:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField3" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label4" runat="server" Text="Service Results:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField4" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label5" runat="server" Text="Notes:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField5" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="lblField7" runat="server" Text="Invoice Info"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField6" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="lblField8" runat="server" Text="Invoice Notes:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField7" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label6" runat="server" Text="In Service date:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField8" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label7" runat="server" Text="Field 9:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField9" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                              <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label8" runat="server" Text="Field 10:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField10" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label9" runat="server" Text="Field 11:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField11" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                
                                                
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label10" runat="server" Text="Field 12:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField12" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                
                                                
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label11" runat="server" Text="Field 13:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField13" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                
                                                
                                                <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label12" runat="server" Text="Field 14:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField14" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                
                                                  <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label13" runat="server" Text="Field 15:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField15" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                
                                                
                                                  <tr>
                                                    <td class="style1">
                                                        <asp:Label ID="Label14" runat="server" Text="Field 16:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtField16" runat="server" TextMode="MultiLine" Width="200px"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                
                                                
                                                
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                                
                                </div>
                                
                                <table width="100%">
                                    <tr>
                                                   
                                                    <td colspan="2" align=center style="height:40px "     >
                                                                                                                
                                                        <asp:Button ID="cmdClose" runat="server" CssClass="combutton" Text="Close" OnClientClick="WinClose()"  />
                                                        &nbsp;&nbsp;&nbsp;<asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save" 
                                                            onclick="cmdSave_Click" />
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
    
    </div>
    </form>
</body>
</html>
