<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmWaypoint.aspx.cs" Inherits="SentinelFM.GeoZone_Landmarks_frmWaypoint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px"
            cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <table id="tblButtons" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td>
                                <asp:Button ID="cmdLandmark" runat="server" Text="Landmarks" CssClass="selectedbutton"
                                    CausesValidation="False" OnClick="cmdLandmark_Click"></asp:Button></td>
                            <td>
                                <asp:Button ID="cmdGeoZone" runat="server" Text="GeoZones" CssClass="confbutton"
                                    CausesValidation="False" OnClick="cmdGeoZone_Click" >
                                </asp:Button></td>
                            <td style="width: 7px">
                                <asp:Button ID="cmdVehicleGeoZone" runat="server" Text="Vehicle-GeoZone Assignment"
                                    CssClass="confbutton" CausesValidation="False" CommandName="8" Width="192px"
                                    OnClick="cmdVehicleGeoZone_Click" >
                                </asp:Button></td>
                            <td style="width: 7px">
                                <asp:Button ID="cmdWaypoints" runat="server" Text="Waypoints"
                                    CssClass="confbutton" CausesValidation="False" CommandName="8" Width="192px"
                                   >
                                </asp:Button></td>
                            <td style="width: 7px">
                                <asp:Button ID="cmdMap" runat="server" Text="Map" CssClass="confbutton" CausesValidation="False"
                                    OnClick="cmdMap_Click"></asp:Button></td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td >
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="679" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" height="550px" width="990px" class=table  border="0">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table1" cellspacing="0" cellpadding="0" width="990" align="center" border="0">
                                                <tr>
                                                    <td class="tableheading" align="left" height="5">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                   
                                                                       
                                                                        
                                                        <table id="tblLandmarks" style="width: 990px;" 
                                                            cellpadding="0" width="720" border="0" runat="server">
                                                            <tr>
                                                               <td align="center">
                                                                
                                                                   &nbsp;</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:DataGrid ID="dgWaypoints" runat="server" Width="100%" GridLines="None" CellPadding="3"
                                                                        BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White" PageSize="13"
                                                                        AllowPaging="True" DataKeyField="WaypointId" AutoGenerateColumns="False" BorderStyle="Ridge" >
                                                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                        <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                        <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                        <Columns>
                                                                            <asp:BoundColumn DataField="WaypointName" HeaderText="WaypointName">
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="Type" HeaderText="TypeName">
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn Visible="False" DataField="Latitude" HeaderText="Latitude">
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn Visible="False" DataField="Longitude" HeaderText="Longitude">
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="StreetAddress" HeaderText="StreetAddress">
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="Email" HeaderText="Persistent">
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:ButtonColumn Text="&lt;img src=../images/edit.gif border=0&gt;" CommandName="cmdEdit"
                                                                                ></asp:ButtonColumn>
                                                                            <asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" CommandName="Delete"
                                                                                ></asp:ButtonColumn>
                                                                            <asp:HyperLinkColumn Text="Map It" DataNavigateUrlField="LandmarkId" DataNavigateUrlFormatString="javascript:var w =LandmarkMap('{0}')"
                                                                                >
                                                                                <ItemStyle Wrap="False" ForeColor="Black" Width="50px"></ItemStyle>
                                                                            </asp:HyperLinkColumn>
                                                                            <asp:BoundColumn Visible="False" DataField="LandmarkId" HeaderText="LandmarkId"></asp:BoundColumn>
                                                                        </Columns>
                                                                        <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                            Mode="NumericPages"></PagerStyle>
                                                                    </asp:DataGrid></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" height="15">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="cmdWaypointAdd" runat="server" Text="Add Waypoint" CssClass="combutton"
                                                                        CausesValidation="False" >
                                                                    </asp:Button></td>
                                                            </tr>
                                                        </table>
                                                        <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="270px" Visible="False"
                                                            Height="8px" ></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table id="tblWaypointAdd" style="width: 670px" cellspacing="0" cellpadding="0" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:RadioButtonList ID="lstAddOptions" runat="server" CssClass="formtext" Width="675px"
                                                                        Height="23px" AutoPostBack="True" RepeatDirection="Horizontal" Font-Bold="True"
                                                                        OnSelectedIndexChanged="lstAddOptions_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource1" Text="By Street Address"></asp:ListItem>
                                                                        <asp:ListItem Value="1"  Text="By Coordinates/Map"></asp:ListItem>
                                                                    </asp:RadioButtonList></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" height="15">
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <table id="Table8" cellspacing="0" cellpadding="0" width="670" border="0">
                                                                        <tr>
                                                                            <td class="formtext" style="width: 179px; height: 13px;">
                                                                               </td>
                                                                            <td class="formtext" style="height: 13px">
                                                                            </td>
                                                                            <td class="formtext" style="height: 13px">
                                                                            </td>
                                                                            <td class="formtext" style="height: 13px">
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" style="height: 22px; width: 179px;">
                                                                                <asp:Label ID="lblWaypointName" runat="server" CssClass="formtext" 
                                                                                    meta:resourcekey="lblLandmarkNameTitleResource1" Text="Waypoint Name:"></asp:Label>
                                                                                <asp:RequiredFieldValidator ID="valLandmark" runat="server" ControlToValidate="txtLandmarkName"
                                                                                    ErrorMessage="Please enter a Landmark Name"  Text="*"></asp:RequiredFieldValidator></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:TextBox ID="txtWaypointName" runat="server" CssClass="formtext" Width="173px"
                                                                                    ></asp:TextBox></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:Label ID="lblType" runat="server" CssClass="formtext" 
                                                                                    meta:resourcekey="lblContactNameTitleResource1" Text="Type:"></asp:Label></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:DropDownList ID="cboType" runat="server" CssClass="formtext">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" style="height: 15px; width: 179px;" height="15">
                                                                            </td>
                                                                            <td class="formtext" style="height: 15px" height="15">
                                                                                &nbsp;</td>
                                                                            <td class="formtext" style="height: 15px" height="15">
                                                                            </td>
                                                                            <td class="formtext" style="height: 15px" height="15">
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" height="15" style="width: 179px">
                                                                                <asp:Label ID="lblPersistent" runat="server" Text="Persistent:"></asp:Label>
                                                                            </td>
                                                                            <td class="formtext" height="15">
                                                                                <asp:CheckBox ID="chkPersistent" runat="server" />
                                                                            </td>
                                                                            <td class="formtext" height="15">
                                                                            </td>
                                                                            <td class="formtext" height="15">
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <table id="Table2" cellspacing="0" cellpadding="0" width="670" border="0">
                                                                        <tr>
                                                                            <td class="formtext" align="center" height="200">
                                                                                <table class="formtext" id="tblStreet" style="width: 670px; " cellspacing="0"
                                                                                    cellpadding="0" width="670" border="0" runat="server">
                                                                                    <tr>
                                                                                        <td style="width: 179px; height: 32px">
                                                                                            
                                                                                            <asp:Label ID="lblStreetTitle" runat="server" Text="Street:"></asp:Label>
                                                                                            <asp:RequiredFieldValidator ID="ValAddress" runat="server" CssClass="formtext" ControlToValidate="txtStreet"
                                                                                                ErrorMessage="Please enter a Street Address" Text="*"></asp:RequiredFieldValidator></td>
                                                                                        <td style="width: 195px; height: 32px">
                                                                                            <asp:TextBox ID="txtStreet" runat="server" CssClass="formtext" Width="173px" TextMode="MultiLine"
                                                                                                 ></asp:TextBox></td>
                                                                                        <td style="width: 99px; height: 32px">
                                                                                            <asp:Label ID="lblCityTitle" runat="server" Text="City:"></asp:Label></td>
                                                                                        <td style="height: 32px">
                                                                                            <asp:TextBox ID="txtCity" runat="server" CssClass="formtext" Width="173px"
                                                                                                ></asp:TextBox></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td height="15" style="width: 179px">
                                                                                        </td>
                                                                                        <td height="15" style="width: 195px">
                                                                                        </td>
                                                                                        <td height="15" style="width: 99px">
                                                                                        </td>
                                                                                        <td height="15">
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="width: 179px">
                                                                                            <asp:Label ID="lblStateProvinceTitle" runat="server" Text="State (Prov):"></asp:Label></td>
                                                                                        <td style="width: 195px">
                                                                                            <asp:TextBox ID="txtState" runat="server" CssClass="formtext" Width="173px"
                                                                                                Height="22px"></asp:TextBox></td>
                                                                                        <td style="width: 99px">
                                                                                            <asp:Label ID="lblCountryTitle" runat="server" Text="Country"></asp:Label>:</td>
                                                                                        <td>
                                                                                            <asp:DropDownList ID="cboCountry" runat="server" CssClass="formtext" 
                                                                                                Width="173px" >
                                                                                                <asp:ListItem Value="USA" Selected="True" Text="USA"></asp:ListItem>
                                                                                                <asp:ListItem Value="Canada" Text="Canada"></asp:ListItem>
                                                                                            </asp:DropDownList></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td height="15" style="width: 179px">
                                                                                        </td>
                                                                                        <td height="15" style="width: 195px">
                                                                                        </td>
                                                                                        <td height="15" style="width: 99px">
                                                                                        </td>
                                                                                        <td height="15">
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                                <table id="Table3" style="width: 670px;" cellspacing="0" cellpadding="0"
                                                                                    border="0">
                                                                                    <tr>
                                                                                        <td class="style2"  >
                                                                                            <asp:Label ID="lblX" runat="server" CssClass="formtext" Visible="False"  Text="Latitude (north is positive):"></asp:Label><asp:RequiredFieldValidator
                                                                                                ID="valX" runat="server" CssClass="formtext" ControlToValidate="txtX" ErrorMessage="Please enter Latitude"
                                                                                                Enabled="False" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                                                                                                    ID="valRangLat" runat="server" CssClass="errortext" ControlToValidate="txtX"
                                                                                                    ErrorMessage="Latitude is wrong." MaximumValue="180" MinimumValue="-180" Enabled="False"
                                                                                                    Type="Double"  Text="*"></asp:RangeValidator></td>
                                                                                        <td align="left">
                                                                                            <asp:TextBox ID="txtY" runat="server" CssClass="formtext"  Visible="False"
                                                                                                name="txtY" meta:resourcekey="txtYResource1" Width="173px"></asp:TextBox></td>
                                                                                        <td class="style1" >
                                                                                            <asp:Label ID="lblY" runat="server" CssClass="formtext" Visible="False"  Text="Longitude (west is negative):"></asp:Label><asp:RangeValidator
                                                                                                    ID="valRangeLong" runat="server" CssClass="errortext" ControlToValidate="txtY"
                                                                                                    ErrorMessage="Longitude is wrong." MaximumValue="90" MinimumValue="-90" Enabled="False"
                                                                                                    Type="Double" Text="*"></asp:RangeValidator><asp:RequiredFieldValidator
                                                                                                ID="valY" runat="server" CssClass="formtext" ControlToValidate="txtY" ErrorMessage="Please enter Longitude"
                                                                                                Enabled="False"  Text="*"></asp:RequiredFieldValidator>
                                                                                        </td>
                                                                                        <td >
                                                                                            <asp:TextBox ID="txtX" runat="server" CssClass="formtext"  Visible="False"
                                                                                                name="txtX" meta:resourcekey="txtXResource1" Width="173px"></asp:TextBox>
                                                                                        </td>
                                                                                    </tr>
                                                                                    </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:Label ID="lblAddMessage" runat="server" CssClass="errortext" Width="270px" Visible="False"
                                                                        Height="8px" meta:resourcekey="lblAddMessageResource1"></asp:Label><asp:ValidationSummary
                                                                            ID="ValidationSummary1" runat="server" CssClass="errortext" Width="321px" Height="17px"
                                                                            meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>
                                                                            
                                                                            
                                                                            <table id="tblShowMap" cellspacing="0" cellpadding="0" border="0" 
                                                                        runat="server" align="center" width="100%">
                                                                                                <tr>
                                                                                                    <td align="center" width="100%" >
                                                                                                        <a class="link" onclick="ShowMap()" href="#">
                                                                                                            <asp:Label ID="lblViewMapTitle" runat="server"  Text="View map"></asp:Label></a></td>
                                                                                                </tr>
                                                                                            </table>
                                                                                            
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="height: 105px" align="center">
                                                                    <div style="overflow: auto; width: 349px; height: 100px">
                                                                        <asp:DataGrid ID="dgAddress" runat="server" Width="271px" GridLines="None" CellPadding="3"
                                                                            BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White" AutoGenerateColumns="False"
                                                                            BorderStyle="Ridge"  >
                                                                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                            <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                            <ItemStyle Font-Size="11px" ForeColor="Black" BackColor="#DEDFDE"></ItemStyle>
                                                                            <HeaderStyle CssClass="DataHeaderStyle"  ></HeaderStyle>
                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                            <Columns>
                                                                                <asp:BoundColumn DataField="Address" HeaderText="Street Address"></asp:BoundColumn>
                                                                                <asp:BoundColumn Visible="False" DataField="Latitude" HeaderText="Latitude"></asp:BoundColumn>
                                                                                <asp:BoundColumn Visible="False" DataField="Longitude" HeaderText="Longitude"></asp:BoundColumn>
                                                                                <asp:ButtonColumn Text="Select" CommandName="Select" >
                                                                                </asp:ButtonColumn>
                                                                            </Columns>
                                                                            <PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"></PagerStyle>
                                                                        </asp:DataGrid></div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100%; height: 19px" align="center">
                                                                    <asp:Button ID="cmdSaveLandmark" runat="server" Text="Save" 
                                                                        CssClass="combutton" meta:resourcekey="cmdSaveLandmarkResource1"></asp:Button>&nbsp;&nbsp;
                                                                    <asp:Button ID="cmdCancelLandmark" runat="server" Text="Cancel" CssClass="combutton"
                                                                        CausesValidation="False" >
                                                                    </asp:Button></td>
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
