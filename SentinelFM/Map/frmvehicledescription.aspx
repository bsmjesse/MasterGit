<%@ Page Language="c#" Inherits="SentinelFM.frmVehicleDescription" CodeFile="frmVehicleDescription.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Vehicle Information</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script language="javascript">
	<!--
			function onblur(event) 
			{
		window.focus();
		}


	//-->
    </script>

</head>
<body onblur="window.focus()" class="body">
    <form id="frmVehicleDescriptionForm" method="post" runat="server">
        <table id="tblCommands" style="z-index: 101; left: 15px; width: 423px; position: absolute;
            top: 11px; height: 137px" cellspacing="0" cellpadding="0" width="423" border="0">
            <tr>
                <td style="height: 11px">
                    <table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdSensorCommands" runat="server" CssClass="confbutton" CausesValidation="False"
                                    Text="Commands" OnClick="cmdSensorCommands_Click" meta:resourcekey="cmdSensorCommandsResource1">
                                </asp:Button></td>
                            <td>
                                <asp:Button ID="cmdVehicleInfo" runat="server" CssClass="selectedbutton" CausesValidation="False"
                                    Text="Vehicle Info" meta:resourcekey="cmdVehicleInfoResource1">
                                </asp:Button></td>
                                
                                 <td>
                                <asp:Button ID="cmdSettings" runat="server" CssClass="confbutton" Text="Settings"
                                    CausesValidation="False" OnClick="cmdSettings_Click" CommandName="49" meta:resourcekey="cmdSettingsfoResource1"  >
                                </asp:Button></td>
                                
                            <td>
                                <asp:Button ID="cmdReefer" runat="server" CssClass="confbutton" Text="Reefer"
                                    CausesValidation="False" meta:resourcekey="cmdReeferResource1" OnClick="cmdReefer_Click" Visible="False">
                                </asp:Button>
                            </td>
                            
                            <td>
                                <asp:Button ID="cmdServices" runat="server" CssClass="confbutton" Text="Services"
                                    CausesValidation="False"  Visible="True" onclick="cmdServices_Click" CommandName="52">
                                </asp:Button>
                            </td>
                            
                            
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
                                <table id="tblForm" style="border-right: gray 2px solid; border-top: gray 2px solid;
                                    border-left: gray 2px solid; width: 490px; border-bottom: gray 2px solid;
                                    height: 265px" border="0">
                                    <tr>
                                        <td align="center" class="configTabBackground">
                                            <table id="Table1" style="border-top-width: thin; border-left-width: thin; border-left-color: black;
                                                border-bottom-width: thin; border-bottom-color: black; width: 340px; border-top-color: black;
                                                height: 312px; border-right-width: thin; border-right-color: black" cellspacing="0"
                                                cellpadding="0" width="340" border="0">
                                                <tr>
                                                    <td class="BigFormText" style="width: 9px; height: 5px;">
                                                    </td>
                                                    <td class="BigFormText" style="width: 111px; height: 5px;">
                                                    </td>
                                                    <td style="height: 5px">
                                                        <asp:Label ID="lblVehicleId" runat="server" Visible="False" meta:resourcekey="lblVehicleIdResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px; height: 27px">
                                                    </td>
                                                    <td class="formtext" style="width: 111px; height: 27px">
                                                        &nbsp;<asp:Label ID="lblDescriptionTitle" runat="server" CssClass="formtext" meta:resourcekey="lblDescriptionTitleResource1" Text="Description:"></asp:Label>&nbsp;</td>
                                                    <td style="height: 27px">
                                                        <asp:Label ID="lblVehicleInfo" runat="server" CssClass="regulartext" meta:resourcekey="lblVehicleInfoResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px">
                                                    </td>
                                                    <td class="formtext" style="width: 111px">
                                                        &nbsp;<asp:Label ID="lblVinTitle" runat="server" CssClass="formtext" meta:resourcekey="lblVinTitleResource1" Text="VIN Number:"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblVin" runat="server" CssClass="regulartext" meta:resourcekey="lblVinResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px">
                                                    </td>
                                                    <td class="formtext" style="width: 111px">
                                                        &nbsp;<asp:Label ID="lblLicensePlateTitle" runat="server" CssClass="formtext" meta:resourcekey="lblLicensePlateTitleResource1" Text="License Plate:"></asp:Label>&nbsp;&nbsp;</td>
                                                    <td>
                                                        <asp:Label ID="lblPlate" runat="server" CssClass="regulartext" meta:resourcekey="lblPlateResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px">
                                                    </td>
                                                    <td class="formtext" style="width: 111px">
                                                        &nbsp;<asp:Label ID="lblVehicleTypeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblVehicleTypeTitleResource1" Text="Vehicle Type:"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblType" runat="server" CssClass="regulartext" meta:resourcekey="lblTypeResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px">
                                                    </td>
                                                    <td class="formtext" style="width: 111px">
                                                        &nbsp;<asp:Label ID="lblMakeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMakeTitleResource1" Text="Make:"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblMake" runat="server" CssClass="regulartext" meta:resourcekey="lblMakeResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px">
                                                    </td>
                                                    <td class="formtext" style="width: 111px">
                                                        &nbsp;<asp:Label ID="lblModelTitle" runat="server" CssClass="formtext" meta:resourcekey="lblModelTitleResource1" Text="Model:"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblModel" runat="server" CssClass="regulartext" meta:resourcekey="lblModelResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px; height: 25px">
                                                    </td>
                                                    <td class="formtext" style="width: 111px; height: 25px">
                                                        &nbsp;<asp:Label ID="lblYearTitle" runat="server" CssClass="formtext" meta:resourcekey="lblYearTitleResource1" Text="Year:"></asp:Label></td>
                                                    <td style="height: 25px">
                                                        <asp:Label ID="lblYear" runat="server" CssClass="regulartext" meta:resourcekey="lblYearResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px; height: 25px;">
                                                    </td>
                                                    <td class="formtext" style="width: 111px; height: 25px;">
                                                        &nbsp;<asp:Label ID="lblColorTitle" runat="server" CssClass="formtext" meta:resourcekey="lblColorTitleResource1" Text="Color:"></asp:Label></td>
                                                    <td style="height: 25px">
                                                        <asp:Label ID="lblColor" runat="server" CssClass="regulartext" meta:resourcekey="lblColorResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="BigFormText" style="width: 9px; height: 22px">
                                                    </td>
                                                    <td class="formtext" style="width: 111px; height: 22px">
                                                        &nbsp;<asp:Label ID="lblBoxIdTitle" runat="server" CssClass="formtext" meta:resourcekey="lblBoxIdTitleResource1" Text="Unit ID:"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                                    <td style="height: 22px">
                                                        <asp:Label ID="lblBoxId" runat="server" CssClass="regulartext" meta:resourcekey="lblBoxIdResource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="RegularText" style="width: 9px; height: 20px;">
                                                    </td>
                                                    <td class="RegularText" style="width: 111px; height: 20px;">
                                                        &nbsp;<asp:Label ID="lblField1Title" runat="server" CssClass="formtext" meta:resourcekey="lblField1TitleResource1" Text="Field 1:"></asp:Label></td>
                                                    <td style="height: 22px">
                                                        <asp:Label ID="lblField1" runat="server" CssClass="formtext" meta:resourcekey="lblField1Resource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="RegularText" height="20" style="width: 9px">
                                                    </td>
                                                    <td class="RegularText" height="20" style="width: 111px">
                                                        &nbsp;<asp:Label ID="lblField2Title" runat="server" CssClass="formtext" meta:resourcekey="lblField2TitleResource1" Text="Field 2:"></asp:Label></td>
                                                    <td style="height: 22px">
                                                        <asp:Label ID="lblField2" runat="server" CssClass="formtext" meta:resourcekey="lblField2Resource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="RegularText" style="width: 9px; height: 20px">
                                                    </td>
                                                    <td class="RegularText" style="width: 111px; height: 20px">
                                                        &nbsp;<asp:Label ID="lblField3Title" runat="server" CssClass="formtext" meta:resourcekey="lblField3TitleResource1" Text="Field 3:"></asp:Label></td>
                                                    <td style="height: 22px">
                                                        <asp:Label ID="lblField3" runat="server" CssClass="formtext" meta:resourcekey="lblField3Resource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="RegularText" style="width: 9px; height: 22px">
                                                    </td>
                                                    <td class="RegularText" style="width: 111px; height: 22px">
                                                        &nbsp;<asp:Label ID="lblField4Title" runat="server" CssClass="formtext" meta:resourcekey="lblField4TitleResource1" Text="Field 4:"></asp:Label></td>
                                                    <td style="height: 22px">
                                                        <asp:Label ID="lblField4" runat="server" CssClass="formtext" meta:resourcekey="lblField4Resource1"></asp:Label></td>
                                                </tr>
                                                <tr height="25">
                                                    <td class="RegularText" style="width: 9px; height: 20px">
                                                    </td>
                                                    <td class="RegularText" style="width: 111px; height: 20px">
                                                        &nbsp;<asp:Label ID="lblField5Title" runat="server" CssClass="formtext" 
                                                            Text="Field 5:" meta:resourcekey="lblField5Resource1"></asp:Label></td>
                                                    <td style="height: 22px">
                                                        <asp:Label ID="lblField5" runat="server" CssClass="formtext" meta:resourcekey="lblField4Resource1"></asp:Label></td>
                                                </tr>
                                               <tr height="25">
                                                  <td colspan="3">
                                                     &nbsp;<asp:Label ID="lblFleetAssigment" runat="server" CssClass="formtext" Text="Assigned to Fleets:" meta:resourcekey="lblFleetAssigmentResource1"></asp:Label></td>
                                               </tr>
                                              
                                               <tr height="25">
                                                  <td class="RegularText" style="width: 9px">
                                                  </td>
                                                  <td class="RegularText" colspan="2">
                                                     <asp:DataGrid ID="dgFleets" runat="server" AutoGenerateColumns="False" BackColor="White"
                                                        BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" 
                                                        Width="380px" ShowHeader="False" meta:resourcekey="dgFleetsResource1">
                                                        <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                                                        <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                                                        <AlternatingItemStyle BackColor="WhiteSmoke" />
                                                        <ItemStyle BackColor="#DEDFDE" Font-Names="Arial,Helvetica,sans-serif" Font-Size="11px"
                                                           ForeColor="Black" />
                                                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                        <PagerStyle Mode="NumericPages" />
                                                        <Columns>
                                                           <asp:BoundColumn DataField="FleetName" ></asp:BoundColumn>
                                                        </Columns>
                                                     </asp:DataGrid></td>
                                               </tr>
                                                <tr height="25">
                                                    <td class="RegularText" style="width: 9px">
                                                    </td>
                                                    <td class="RegularText" style="width: 111px">
                                                    </td>
                                                    <td align="right">
                                                        <asp:Button runat="server" CssClass="combutton" ID="cmdClose" OnClientClick="top.close()"
                                                            Text="Close" meta:resourcekey="cmdCloseResource1" /><%--<input class="combutton" id="cmdClose" onclick="top.close()" type="button"
                                                                value="Close">--%></td>
                                                </tr>
                                                <tr>
                                                    <td class="RegularText" style="width: 9px" height="5">
                                                    </td>
                                                    <td class="RegularText" style="width: 111px" height="5">
                                                    </td>
                                                    <td align="center" height="5">
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
