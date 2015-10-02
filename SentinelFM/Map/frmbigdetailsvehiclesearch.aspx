<%@ Page Language="c#" Inherits="SentinelFM.Map.frmBigDetailsVehicleSearch" CodeFile="frmBigDetailsVehicleSearch.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Vehicle Search</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="frmMapVehicleSearch" method="post" runat="server">
        <table id="tblSearch" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
            left: 6px; width: 785px; position: absolute; top: 5px; height: 126px">
            <tr>
                <td class="formtext" style="width: 133px; height: 57px" valign="top">
                    <asp:Label ID="lblSearchTypeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSearchTypeTitleResource1">Search for vehicle by:</asp:Label></td>
                <td style="width: 109px; height: 57px" valign="top">
                    <asp:DropDownList ID="cboSearchType" runat="server" AutoPostBack="True" CssClass="RegularText"
                        OnSelectedIndexChanged="cboSingleSearchType_SelectedIndexChanged" meta:resourcekey="cboSearchTypeResource1">
                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource1">Description</asp:ListItem>
                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2">License Plate</asp:ListItem>
                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource3">Landmark</asp:ListItem>
                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource4">Unit ID</asp:ListItem>
                        <asp:ListItem Value="4" meta:resourcekey="ListItemResource5">Street Address</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;&nbsp;</td>
                <td style="width: 473px" valign="top">
                    <asp:TextBox ID="txtSearchParam" runat="server" CssClass="formtext" Width="425px"
                        meta:resourcekey="txtSearchParamResource1"></asp:TextBox>
                    <table id="tblAddress" runat="server">
                        <tr>
                            <td style="width: 115px" valign="top">
                                <table class="formtext" style="width: 519px">
                                    <tr>
                                        <td style="width: 138px" valign="top">
                                            <asp:Label ID="lblFleetTitle" runat="server" meta:resourcekey="lblFleetTitleResource1">Fleet:</asp:Label>&nbsp;</td>
                                        <td style="width: 100px">
                                            <asp:DropDownList ID="cboFleetAdrSrch" runat="server" CssClass="RegularText" DataTextField="FleetName"
                                                DataValueField="FleetId" OnSelectedIndexChanged="cboAdvanceFleet_SelectedIndexChanged"
                                                Width="173px" meta:resourcekey="cboFleetAdrSrchResource1">
                                            </asp:DropDownList></td>
                                        <td style="width: 100px" valign="top">
                                            <asp:Label ID="lblWithinTitle1" runat="server" meta:resourcekey="lblWithinTitle1Resource1">within</asp:Label></td>
                                        <td style="width: 308px" valign="top">
                                            <asp:DropDownList ID="cboDistanceAdrSrc" runat="server" CssClass="RegularText" Height="14px"
                                                Width="173px" meta:resourcekey="cboDistanceAdrSrcResource1">
                                                <asp:ListItem Value="1000" meta:resourcekey="ListItemResource6">1 km/0.6 mi</asp:ListItem>
                                                <asp:ListItem Value="5000" meta:resourcekey="ListItemResource7">5 km/3.1 mi</asp:ListItem>
                                                <asp:ListItem Value="10000" meta:resourcekey="ListItemResource8">10 km/6.2 mi</asp:ListItem>
                                                <asp:ListItem Value="50000" meta:resourcekey="ListItemResource9">50 km/31 mi</asp:ListItem>
                                                <asp:ListItem Value="100000" meta:resourcekey="ListItemResource10">100 km/62 mi</asp:ListItem>
                                                <asp:ListItem Value="250000" meta:resourcekey="ListItemResource11">250 km/155 mi</asp:ListItem>
                                                <asp:ListItem Value="500000" meta:resourcekey="ListItemResource12">500 km/311 mi</asp:ListItem>
                                            </asp:DropDownList>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 138px" valign="top">
                                            <asp:Label ID="Label3" runat="server" Text="From Address:" Width="90px" meta:resourcekey="Label3Resource1"></asp:Label></td>
                                        <td height="10" style="width: 100px">
                                        </td>
                                        <td style="width: 100px; height: 26px" valign="top">
                                        </td>
                                        <td style="width: 308px" valign="top">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 138px; height: 36px" valign="top">
                                            <asp:Label ID="lblStreetTitle" runat="server" meta:resourcekey="lblStreetTitleResource1">Street:</asp:Label></td>
                                        <td style="width: 100px; height: 36px">
                                            <asp:TextBox ID="txtStreet" runat="server" CssClass="formtext" TextMode="MultiLine"
                                                Width="173px" meta:resourcekey="txtStreetResource1"></asp:TextBox></td>
                                        <td style="width: 100px; height: 36px" valign="top">
                                            <asp:Label ID="lblCityTitle" runat="server" meta:resourcekey="lblCityTitleResource1">City:</asp:Label></td>
                                        <td style="width: 308px; height: 36px" valign="top">
                                            <asp:TextBox ID="txtCity" runat="server" CssClass="formtext" Width="173px" meta:resourcekey="txtCityResource1"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 138px; height: 36px" valign="top">
                                            <asp:Label ID="lblStateTitle" runat="server" meta:resourcekey="lblStateTitleResource1">State:</asp:Label></td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtState" runat="server" CssClass="formtext" Width="173px" meta:resourcekey="txtStateResource1"></asp:TextBox></td>
                                        <td style="width: 100px; height: 36px" valign="top">
                                            <asp:Label ID="lblCountryTitle" runat="server" meta:resourcekey="lblCountryTitleResource1">Country:</asp:Label>
                                        </td>
                                        <td style="width: 308px" valign="top">
                                            <asp:DropDownList ID="cboCountry" runat="server" CssClass="formtext" Width="173px"
                                                meta:resourcekey="cboCountryResource1">
                                                <asp:ListItem Selected="True" Value="USA" meta:resourcekey="ListItemResource13">USA</asp:ListItem>
                                                <asp:ListItem Value="Canada" meta:resourcekey="ListItemResource14">Canada</asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 138px; height: 36px" valign="top">
                                            <asp:Button ID="cmdFindAddress" runat="server" CssClass="combutton" OnClick="cmdFindAddress_Click"
                                                Text="Find Address" Width="121px" meta:resourcekey="cmdFindAddressResource1" /></td>
                                        <td style="width: 100px; height: 36px">
                                        </td>
                                        <td valign="top">
                                        </td>
                                        <td align="right" style="width: 308px" valign="top">
                                            <asp:TextBox ID="txtX" runat="server" CssClass="formtext" name="txtX" Visible="False"
                                                Width="173px" meta:resourcekey="txtXResource1"></asp:TextBox>
                                            <asp:TextBox ID="txtY" runat="server" CssClass="formtext" name="txtY" Visible="False"
                                                Width="173px" meta:resourcekey="txtYResource1"></asp:TextBox>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 115px" valign="top">
                                <div style="overflow: auto; width: 382px; height: 98px">
                                    <asp:DataGrid ID="dgAddress" runat="server" AutoGenerateColumns="False" BackColor="White"
                                        BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1"
                                        GridLines="None" OnSelectedIndexChanged="dgAddress_SelectedIndexChanged" Width="341px"
                                        meta:resourcekey="dgAddressResource1">
                                        <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                                        <AlternatingItemStyle BackColor="WhiteSmoke" />
                                        <ItemStyle BackColor="#DEDFDE" Font-Size="11px" ForeColor="Black" />
                                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                        <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                                        <Columns>
                                            <asp:BoundColumn DataField="Address" HeaderText='<%$ Resources:dgAddress_Address %>'>
                                            </asp:BoundColumn>
                                            <asp:BoundColumn DataField="Latitude" HeaderText='<%$ Resources:dgAddress_Latitude %>'
                                                Visible="False"></asp:BoundColumn>
                                            <asp:BoundColumn DataField="Longitude" HeaderText='<%$ Resources:dgAddress_Longitude %>'
                                                Visible="False"></asp:BoundColumn>
                                            <asp:ButtonColumn CommandName="Select" Text="Select" meta:resourcekey="ButtonColumnResource1">
                                            </asp:ButtonColumn>
                                        </Columns>
                                        <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
                                    </asp:DataGrid></div>
                            </td>
                        </tr>
                    </table>
                    <table id="tblLandmark" runat="server" border="0" cellpadding="0" cellspacing="0"
                        style="width: 535px">
                        <tr>
                            <td align="left" class="formtext" style="width: 49px; height: 19px">
                                <asp:Label ID="Label2" runat="server" Text=" in fleet:" meta:resourcekey="Label2Resource1"></asp:Label></td>
                            <td style="width: 58px; height: 19px">
                                <asp:DropDownList ID="cboAdvanceFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                    DataTextField="FleetName" DataValueField="FleetId" OnSelectedIndexChanged="cboAdvanceFleet_SelectedIndexChanged"
                                    meta:resourcekey="cboAdvanceFleetResource1">
                                </asp:DropDownList></td>
                            <td align="left" class="formtext" style="width: 36px; height: 19px">
                                &nbsp;<asp:Label ID="lblWithinTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblWithinTitle2Resource1">within</asp:Label></td>
                            <td style="width: 56px; height: 19px">
                                <asp:DropDownList ID="cboAdvanceDist" runat="server" CssClass="RegularText" Height="14px"
                                    Width="107px" meta:resourcekey="cboAdvanceDistResource1">
                                    <asp:ListItem Value="1000" meta:resourcekey="ListItemResource15">1 km/0.6 mi</asp:ListItem>
                                    <asp:ListItem Value="5000" meta:resourcekey="ListItemResource16">5 km/3.1 mi</asp:ListItem>
                                    <asp:ListItem Value="10000" meta:resourcekey="ListItemResource17">10 km/6.2 mi</asp:ListItem>
                                    <asp:ListItem Value="50000" meta:resourcekey="ListItemResource18">50 km/31 mi</asp:ListItem>
                                    <asp:ListItem Value="100000" meta:resourcekey="ListItemResource19">100 km/62 mi</asp:ListItem>
                                    <asp:ListItem Value="250000" meta:resourcekey="ListItemResource20">250 km/155 mi</asp:ListItem>
                                    <asp:ListItem Value="500000" meta:resourcekey="ListItemResource21">500 km/311 mi</asp:ListItem>
                                </asp:DropDownList></td>
                            <td align="left" class="formtext" style="width: 94px; height: 19px">
                                &nbsp;<asp:Label ID="Label1" runat="server" Text="from Landmark " meta:resourcekey="Label1Resource1"></asp:Label></td>
                            <td style="width: 78px; height: 19px">
                                <asp:DropDownList ID="cboAdvanceLandmarks" runat="server" CssClass="RegularText"
                                    DataTextField="LandmarkName" DataValueField="LandmarkName" Height="14px" Width="120px"
                                    meta:resourcekey="cboAdvanceLandmarksResource1">
                                </asp:DropDownList></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="formtext" style="width: 133px">
                </td>
                <td style="width: 109px">
                </td>
                <td align="left" style="width: 473px; height: 22px">
                    <asp:Button ID="cmdSearch" runat="server" CssClass="combutton" OnClick="cmdSearch_Click"
                        Text="Search Vehicles" Width="121px" meta:resourcekey="cmdSearchResource1" />&nbsp;<asp:Button
                            ID="cmdClose" runat="server" CssClass="combutton" OnClick="cmdClose_Click" Text="Close"
                            Width="121px" meta:resourcekey="cmdCloseResource1" />
                    &nbsp; &nbsp; &nbsp;</td>
            </tr>
            <tr>
                <td class="formtext" style="width: 133px">
                </td>
                <td style="width: 109px">
                </td>
                <td align="left" style="width: 473px; height: 22px">
                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Height="9px" Visible="False"
                        Width="395px" meta:resourcekey="lblMessageResource1"></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 133px">
                </td>
                <td style="width: 109px">
                </td>
                <td align="left" style="height: 147px" valign="top">
                    <asp:DataGrid ID="dgMultiplyVehicle" runat="server" AllowPaging="True" AllowSorting="True"
                        AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None"
                        BorderWidth="2px" CellPadding="1" OnPageIndexChanged="dgMultiplyVehicle_PageIndexChanged"
                        PageSize="8" Width="341px" meta:resourcekey="dgMultiplyVehicleResource1">
                        <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                        <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                        <AlternatingItemStyle BackColor="Beige" CssClass="gridtext" />
                        <ItemStyle BackColor="White" CssClass="gridtext" />
                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                        <Columns>
                            <asp:BoundColumn DataField="VehicleId" HeaderText='<%$ Resources:dgMultiplyVehicle_VehicleId %>'
                                Visible="False"></asp:BoundColumn>
                            <asp:TemplateColumn HeaderText='<%$ Resources:dgMultiplyVehicle_Show %>'>
                                <HeaderStyle Width="20px" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkCheckBox" runat="server" meta:resourcekey="chkCheckBoxResource1" />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="Description" HeaderText='<%$ Resources:dgMultiplyVehicle_Description %>'>
                                <HeaderStyle Width="400px" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="VehicleStatus" HeaderText='<%$ Resources:dgMultiplyVehicle_Status %>'>
                                <HeaderStyle Width="160px" />
                            </asp:BoundColumn>
                        </Columns>
                        <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right"
                            Mode="NumericPages" />
                    </asp:DataGrid></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 133px">
                </td>
                <td style="width: 109px">
                </td>
                <td align="left" class="formtext" valign="top">
                    <asp:DataGrid ID="dgSingleVehicle" runat="server" AllowPaging="True" AllowSorting="True"
                        AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None"
                        BorderWidth="2px" CellPadding="1" DataKeyField="VehicleId" OnPageIndexChanged="dgSingleVehicle_PageIndexChanged"
                        OnSelectedIndexChanged="dgSingleVehicle_SelectedIndexChanged" PageSize="8" Width="341px"
                        meta:resourcekey="dgSingleVehicleResource1">
                        <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                        <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                        <AlternatingItemStyle BackColor="Beige" CssClass="gridtext" />
                        <ItemStyle BackColor="White" CssClass="gridtext" />
                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                        <Columns>
                            <asp:BoundColumn DataField="Description" HeaderText='<%$ Resources:dgSingleVehicle_Description %>'>
                                <HeaderStyle Width="400px" />
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="VehicleStatus" HeaderText='<%$ Resources:dgSingleVehicle_Status %>'>
                                <HeaderStyle Width="170px" />
                            </asp:BoundColumn>
                            <asp:ButtonColumn CommandName="Select" Text="Select" meta:resourcekey="ButtonColumnResource2">
                            </asp:ButtonColumn>
                        </Columns>
                        <PagerStyle BackColor="#C6C3C6" Font-Size="11px" ForeColor="Black" HorizontalAlign="Right"
                            Mode="NumericPages" />
                    </asp:DataGrid></td>
            </tr>
            <tr>
                <td class="formtext" style="width: 133px">
                </td>
                <td style="width: 109px">
                </td>
                <td align="left" style="width: 473px; height: 22px">
                    <asp:Button ID="cmdGoTo" runat="server" CssClass="combutton" Height="19px" OnClick="cmdGoTo_Click"
                        Text="Map It" Visible="False" Width="121px" meta:resourcekey="cmdGoToResource1" /></td>
            </tr>
        </table>
        &nbsp;
    </form>
</body>
</html>
