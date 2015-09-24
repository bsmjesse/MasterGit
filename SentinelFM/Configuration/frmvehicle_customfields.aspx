<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmvehicle_customfields.aspx.cs"
    Inherits="SentinelFM.Configuration_frmvehicle_customfields" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Custom Fields</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <style type="text/css">
        .RegularText {
            margin-left: 42px;
        }

        .style1 {
            width: 470px;
            height: 25px;
        }

        .style2 {
            height: 25px;
        }

        .style3 {
            width: 470px;
            height: 22px;
        }

        .style4 {
            width: 100px;
            height: 22px;
        }

        .style5 {
            width: 1597px;
            height: 25px;
        }

        .style6 {
            width: 1597px;
        }

        .style7 {
            width: 1597px;
            height: 22px;
        }

        .auto-style3 {
            width: 2072px;
        }

        .auto-style4 {
            width: 2002px;
            height: 22px;
        }

        .auto-style5 {
            width: 100%;
            height: 2px;
        }

        .auto-style7 {
            width: 101px;
        }

        .auto-style8 {
            width: 101px;
            height: 22px;
        }

        .auto-style9 {
            width: 200px;
        }

        .auto-style10 {
            width: 160px;
        }

        .auto-style11 {
            width: 1974px;
            height: 38px;
        }

        .auto-style12 {
            width: 101px;
            height: 38px;
        }

        .auto-style13 {
            width: 183px;
            height: 38px;
        }

        .auto-style15 {
            width: 183px;
            height: 22px;
        }

        #Table3 {
            width: 554px;
        }

        .auto-style18 {
            width: 1974px;
        }

        .auto-style20 {
            width: 183px;
        }

        .auto-style21 {
            height: 28px;
            width: 183px;
        }

        .auto-style22 {
            width: 195px;
        }

        .auto-style23 {
            width: 195px;
            height: 22px;
        }

        .auto-style24 {
            width: 195px;
            height: 38px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table border="0" cellpadding="0" cellspacing="0" width="96%">
            <tr>
                <td valign="bottom">
                    <table id="tblButtons" border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdInfo" runat="server" CausesValidation="False" CssClass="confbutton"
                                    OnClick="cmdInfo_Click" Text="Info" Width="104px" meta:resourcekey="cmdInfoResource1" /></td>
                            <td style="width: 105px">
                                <asp:Button ID="cmdCustomFields" runat="server" CausesValidation="False" CssClass="selectedbutton"
                                    Text="Custom Fields" Width="104px" meta:resourcekey="cmdCustomFieldsResource1" /></td>
                            <td style="width: 105px">
                                <asp:Button ID="cmdVehicleStatus" runat="server" CausesValidation="False" CssClass="confbutton" Width="104px" CommandName="114"
                                    Text="Vehicle Status" OnClick="cmdVehicleStatus_Click" meta:resourcekey="cmdVehicleStatusResource1" /></td>
                                <td style="width: 105px">
                                    <asp:Button ID="cmdWorkingHours" runat="server" CausesValidation="False" CssClass="confbutton"
                                        Text="Working Hours" Width="104px" OnClick="cmdWorkingHours_Click" Visible="False" meta:resourcekey="cmdWorkingHoursResource1" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 100px">
                    <table id="Table3" cellpadding="0" cellspacing="0" border="0" class="table" height="365px">
                        <tr>
                            <td align="center" class="configTabBackground" valign="top">
                                <table style="width: 525px" class="formtext" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td colspan="6" style="height: 32px"></td>
                                    </tr>
                                    <tr>
                                        <td align="left" class="auto-style9">
                                            <asp:Label ID="lblField1" Width="100px" runat="server" meta:resourcekey="lblField1Resource1"
                                                Text="Field 1:"></asp:Label></td>
                                        <td align="left" class="auto-style7"></td>
                                        <td class="auto-style22">
                                            <asp:TextBox ID="txtField1" runat="server" meta:resourcekey="txtField1Resource1"></asp:TextBox>
                                            <asp:DropDownList ID="cboField1" runat="server"
                                                DataValueField="UserId" DataTextField="UserName"
                                                CssClass="RegularText" meta:resourcekey="cboField1Resource1">
                                            </asp:DropDownList>
                                        </td>
                                        <%-- </tr>
                                    
                                    <tr>--%>
                                        <td class="auto-style18"></td>
                                        <td align="left" class="auto-style3">
                                            <asp:Label ID="lblField2" Width="100px" runat="server" meta:resourcekey="lblField2Resource1"
                                                Text="Field 2:"></asp:Label></td>
                                        <td align="left" class="auto-style20">
                                            <asp:TextBox ID="txtField2" runat="server" meta:resourcekey="txtField2Resource1"></asp:TextBox>
                                        </td>
                                        <td style="width: 100px; height: 26px;">&nbsp;</td>
                                    </tr>

                                    <tr>
                                        <td align="left" class="auto-style4">
                                            <asp:Label ID="lblField3" runat="server" meta:resourcekey="lblField3Resource1"
                                                Text="Field 3:"></asp:Label></td>
                                        <td align="left" class="auto-style8"></td>
                                        <td class="auto-style23">
                                            <asp:TextBox ID="txtField3" runat="server" meta:resourcekey="txtField3Resource1"></asp:TextBox></td>
                                        <%--</tr>

                                    <tr>--%>
                                        <td align="left" class="auto-style18">&nbsp;</td>
                                        <td align="left" class="auto-style10">
                                            <asp:Label ID="lblField4" runat="server" meta:resourcekey="lblField4Resource1"
                                                Text="Field 4:"></asp:Label>
                                        </td>
                                        <td class="auto-style20">
                                            <asp:TextBox ID="txtField4" runat="server" meta:resourcekey="txtField4Resource1"></asp:TextBox></td>
                                    </tr>

                                    <tr>
                                        <td align="left" class="auto-style9">
                                            <asp:Label ID="lblField5" runat="server" meta:resourcekey="lblField5Resource1"
                                                Text="Field 5:"></asp:Label></td>
                                        <td align="left" class="auto-style7"></td>
                                        <td class="auto-style22">
                                            <asp:TextBox ID="txtField5" runat="server" meta:resourcekey="lblField5Resource1"></asp:TextBox></td>
                                        <%--</tr>

                                    <tr>--%>
                                        <td align="left" class="auto-style18">&nbsp;</td>
                                        <td align="left" class="auto-style10">
                                            <asp:Label ID="lblVehicleWeight" runat="server"
                                                meta:resourcekey="lblVehicleWeight" Text="Weight:"></asp:Label>
                                        </td>
                                        <td style="text-align: left;" class="auto-style20">
                                            <asp:TextBox ID="txtVehicleWeight" Width="45px" runat="server" meta:resourcekey="lblField5Resource1"></asp:TextBox>
                                            <asp:DropDownList ID="cboVehicleWtUnit" Width="70px" runat="server" meta:resourcekey="cboVehicleWtUnitResource1">
                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource1">Select Unit</asp:ListItem>
                                                <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">Kilogram</asp:ListItem>
                                                <asp:ListItem Value="2" meta:resourcekey="ListItemResource3">M/Ton</asp:ListItem>
                                                 <asp:ListItem Value="3" meta:resourcekey="ListItemResource4">LB</asp:ListItem>      
                                            </asp:DropDownList>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td class="auto-style9" align="left">
                                            <asp:Label ID="lblFuelCapacity" runat="server"
                                                meta:resourcekey="lblFuelCapacity" Text="Fuel Capacity:"></asp:Label>
                                        </td>
                                        <td height="10" class="auto-style7"></td>
                                        <td class="auto-style22">
                                            <asp:TextBox ID="txtFuelCapacity" runat="server"
                                                meta:resourcekey="lblField5Resource1"></asp:TextBox>
                                        </td>
                                        <%--</tr>
                                    <tr>--%>
                                        <td align="left" class="auto-style18" valign="top">&nbsp;</td>
                                        <td align="left" valign="top" class="auto-style10">
                                            <asp:Label ID="lblFuelBurnRate" runat="server"
                                                meta:resourcekey="lblFuelBurnRate" Text="Fuel B/Rate:"></asp:Label>
                                        </td>
                                        <td style="text-align: left;" valign="top" class="auto-style21">
                                            <asp:TextBox ID="txtFuelBurnRate" runat="server"
                                                meta:resourcekey="lblField5Resource1"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <%--Salman Aug 08,2014--%>
                                    <tr>
                                        <td colspan="6" style="border-top: 1px solid #000; margin-top: 5px; height: 5px;" class="auto-style5"></td>
                                    </tr>

                                    <%-- PlaceHolder: Logical Separator for 3rd Party Vehicle Additional Information--%>

                                    <asp:PlaceHolder ID="plh3rdPartyVAI" runat="server" Visible="true">
                                        <tr>
                                            <td align="left" class="auto-style9">
                                                <asp:Label ID="lblEquipNumber" runat="server"
                                                    Text="Equip Number:" meta:resourcekey="lblEquipNumberResource1"></asp:Label></td>
                                            <td align="left" class="auto-style7"></td>
                                            <td style="text-align: left;" class="auto-style22">
                                                <asp:TextBox ID="txtEquipNumber" runat="server" meta:resourcekey="txtEquipNumberResource1"></asp:TextBox>
                                            </td>
                                            <%--</tr>

                                    <tr>--%>
                                            <td align="left" class="auto-style9">&nbsp;</td>
                                            <td align="left" class="auto-style7">
                                                <asp:Label ID="lblSAPEquipNumber" runat="server" Text="SAP Equip#:" meta:resourcekey="lblSAPEquipNumberResource1"></asp:Label>
                                            </td>
                                            <td style="width: 100px; text-align: left;">
                                                <asp:TextBox ID="txtSAPEquipNumber" runat="server" meta:resourcekey="txtSAPEquipNumberResource1"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style9">
                                                <asp:Label ID="lblLegacyEquipNumber" runat="server" Text="Legacy Equip#:" meta:resourcekey="lblLegacyEquipNumberResource1"></asp:Label></td>
                                            <td align="left" class="auto-style7"></td>
                                            <td style="text-align: left;" class="auto-style22">
                                                <asp:TextBox ID="txtLegacyEquipNumber" runat="server" meta:resourcekey="txtLegacyEquipNumberResource1"></asp:TextBox>
                                            </td>
                                            <%--</tr>

                                    <tr>--%>
                                            <td align="left" class="auto-style9">&nbsp;</td>
                                            <td align="left" class="auto-style7">
                                                <asp:Label ID="lblObjectType" runat="server" Text="Object Type:" meta:resourcekey="lblObjectTypeResource1"></asp:Label>
                                            </td>
                                            <td style="width: 100px; text-align: left;">
                                                <asp:TextBox ID="txtObjectType" runat="server" meta:resourcekey="txtObjectTypeResource1"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style9">
                                                <asp:Label ID="lblDOTNumber" runat="server" Text="DOT Number:" meta:resourcekey="lblDOTNumberResource1"></asp:Label></td>
                                            <td align="left" class="auto-style7"></td>
                                            <td style="text-align: left;" class="auto-style22">
                                                <asp:TextBox ID="txtDOTNumber" runat="server" meta:resourcekey="txtDOTNumberResource1"></asp:TextBox>
                                            </td>
                                            <%--</tr>

                                    <tr>--%>
                                            <td align="left" class="style2">&nbsp;</td>
                                            <td align="left" class="auto-style7">
                                                <asp:Label ID="lblEquipCategory" runat="server" Text="Equip Categ:" meta:resourcekey="lblEquipCategoryResource1"></asp:Label>
                                            </td>
                                            <td style="width: 100px; text-align: left;">
                                                <asp:TextBox ID="txtEquipCategory" runat="server" meta:resourcekey="txtEquipCategoryResource1"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style11">
                                                <asp:Label ID="lblAcquireDate" runat="server" Text="Acquire Date:" meta:resourcekey="lblAcquireDateResource1"></asp:Label></td>
                                            <td align="left" class="auto-style12"></td>
                                            <td style="text-align: left;" class="auto-style24">
                                                <asp:TextBox ID="txtAcquireDate" Width="77px" runat="server" meta:resourcekey="txtAcquireDateResource1"></asp:TextBox>
                                                <a href="javascript:;" onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtAcquireDate','cal','width=220,height=200,left=567,top=380')">
                                                    <img alt="Calendar" src="../images/SmallCalendar.gif" style="border-width: 0px;" /></a>
                                            </td>

                                            <%-- </tr>

                                    <tr>--%>
                                            <td align="left" class="auto-style11">&nbsp;</td>
                                            <td align="left" class="auto-style12">
                                                <asp:Label ID="lblRetireDate" runat="server" Text="Retire Date:" meta:resourcekey="lblRetireDateResource1"></asp:Label>
                                            </td>
                                            <td style="text-align: left;" class="auto-style13">
                                                <asp:TextBox ID="txtRetireDate" Width="77px" runat="server" meta:resourcekey="txtRetireDateResource1"></asp:TextBox>
                                                <a href="javascript:;" onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtRetireDate','cal','width=220,height=200,left=567,top=380')">
                                                    <img alt="Calendar" src="../images/SmallCalendar.gif" style="border-width: 0px;" /></a>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style9">
                                                <asp:Label ID="lblSoldDate" runat="server" Text="Sold Date:" meta:resourcekey="lblSoldDateResource1"></asp:Label></td>
                                            <td align="left" class="auto-style7"></td>
                                            <td style="text-align: left;" class="auto-style22">
                                                <asp:TextBox ID="txtSoldDate" Width="77px" runat="server" meta:resourcekey="txtSoldDateResource1"></asp:TextBox>
                                                <a href="javascript:;" onclick="window.open('../UserControl/frmCalendar.aspx?textbox=txtSoldDate','cal','width=220,height=200,left=567,top=380')">
                                                    <img alt="Calendar" src="../images/SmallCalendar.gif" style="border-width: 0px;" /></a>
                                            </td>

                                            <%--</tr>

                                    <tr>--%>
                                            <td align="left" class="auto-style9">&nbsp;</td>
                                            <td align="left" class="auto-style7">
                                                <asp:Label ID="lblObjectPrefix" runat="server" Text="Obj Prefix:" meta:resourcekey="lblObjectPrefixResource1"></asp:Label>
                                            </td>
                                            <td style="width: 100px; text-align: left;">
                                                <asp:TextBox ID="txtObjectPrefix" runat="server" meta:resourcekey="txtObjectPrefixResource1"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style9">
                                                <asp:Label ID="lblOwningDistrict" runat="server" Text="Owning Dist:" meta:resourcekey="lblOwningDistrictResource1"></asp:Label></td>
                                            <td align="left" class="auto-style7"></td>
                                            <td style="text-align: left;" class="auto-style22">
                                                <asp:TextBox ID="txtOwningDistrict" runat="server" meta:resourcekey="txtOwningDistrictResource1"></asp:TextBox>
                                            </td>
                                            <%--</tr>

                                    <tr>--%>
                                            <td align="left" class="auto-style9">&nbsp;</td>
                                            <td align="left" class="auto-style7">
                                                <asp:Label ID="lblProjectNumber" runat="server" Text="Project Nbr:" meta:resourcekey="lblProjectNumberResource1"></asp:Label>
                                            </td>
                                            <td style="width: 100px; text-align: left;">
                                                <asp:TextBox ID="txtProjectNumber" runat="server" meta:resourcekey="txtProjectNumberResource1"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style4">
                                                <asp:Label ID="lblTotalCtrReading_1" runat="server" Text="Total Ctr R_1:" meta:resourcekey="lblTotalCtrReading_1Resource1"></asp:Label></td>
                                            <td align="left" class="auto-style8"></td>
                                            <td style="text-align: left;" class="auto-style23">
                                                <asp:TextBox ID="txtTotalCtrReading_1" runat="server" meta:resourcekey="txtTotalCtrReading_1Resource1"></asp:TextBox>
                                            </td>
                                            <%--</tr>

                                    <tr>--%>
                                            <td align="left" class="auto-style4"></td>
                                            <td align="left" class="auto-style8">
                                                <asp:Label ID="lblTotalCtrReading_2" runat="server" Text="Total Ctr R_2:" meta:resourcekey="lblTotalCtrReading_2Resource1"></asp:Label>
                                            </td>
                                            <td style="text-align: left;" class="auto-style15">
                                                <asp:TextBox ID="txtTotalCtrReading_2" runat="server" meta:resourcekey="txtTotalCtrReading_2Resource1"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style9">
                                                <asp:Label ID="lblCtrReadingUom_1" runat="server" Text="Ctr Read U_1:" meta:resourcekey="lblCtrReadingUom_1Resource1"></asp:Label></td>
                                            <td align="left" class="auto-style7"></td>
                                            <td style="text-align: left;" class="auto-style22">
                                                <asp:TextBox ID="txtCtrReadingUom_1" runat="server" meta:resourcekey="txtCtrReadingUom_1Resource1"></asp:TextBox>
                                            </td>
                                            <%--</tr>

                                    <tr>--%>
                                            <td align="left" class="auto-style9">&nbsp;</td>
                                            <td align="left" class="auto-style7">
                                                <asp:Label ID="lblCtrReadingUom_2" runat="server" Text="Ctr Read U_2:" meta:resourcekey="lblCtrReadingUom_2Resource1"></asp:Label>
                                            </td>
                                            <td style="width: 100px; text-align: left;">
                                                <asp:TextBox ID="txtCtrReadingUom_2" runat="server" meta:resourcekey="txtCtrReadingUom_2Resource1"></asp:TextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style9">
                                                <asp:Label ID="lblShortDesc" runat="server" Text="Short Desc:" meta:resourcekey="lblShortDescResource1"></asp:Label></td>
                                            <td align="left" class="auto-style7"></td>
                                            <td style="text-align: left;" class="auto-style22">
                                                <asp:TextBox ID="txtShortDesc" runat="server" meta:resourcekey="txtShortDescResource1"></asp:TextBox>
                                            </td>

                                            <td align="left" class="auto-style9"></td>

                                                <td align="left" class="auto-style7"></td>
                                                <td style="width: 100px; text-align: left;"></td>
                                        </tr>

                                        <tr>
                                            <td align="left" class="auto-style9">&nbsp;</td>
                                            <td align="left" class="auto-style7">&nbsp;</td>
                                            <td style="text-align: left;" class="auto-style22">&nbsp;</td>
                                        </tr>
                                    </asp:PlaceHolder>
                                    <tr>
                                        <td align="center" colspan="6">
 

                                            <asp:Button runat="server" CssClass="combutton" ID="Button1" OnClientClick="window.close()"
                                                Text="Close" meta:resourcekey="Button1Resource1" />
                                        

                                            <asp:Button ID="cmdSave" runat="server" CssClass="combutton" Text="Save" OnClick="cmdSave_Click" meta:resourcekey="cmdSaveResource1"></asp:Button>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="6" style="height: 32px" align="center">
                                            <asp:Label ID="lblVehicleId" runat="server" Visible="False" meta:resourcekey="lblVehicleIdResource1"></asp:Label>
                                            <asp:Label ID="lblLicensePlate" runat="server" Visible="False" meta:resourcekey="lblLicensePlateResource1"></asp:Label>
                                            <asp:Label ID="lblMessage" runat="server" CssClass="errortext" meta:resourcekey="lblMessageResource1"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
