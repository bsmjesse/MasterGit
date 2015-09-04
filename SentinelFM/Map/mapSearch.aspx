<%@ Page Language="c#" Inherits="SentinelFM.Map.mapSearch" CodeFile="mapSearch.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>Vehicle Search</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
</head>
<body>
    <form id="mapSearchForm" method="post" runat="server">
        <table id="tblSearch" style="z-index: 101; left: 6px; width: 810px; position: absolute;
            top: 5px; height: 126px" cellspacing="0" cellpadding="0" border="0">
            <tr>
                <td style="width: 473px;" valign="top">
                    <table >
                        <tr>
                            <td align="left"  >
                    <asp:Label ID="lblSearchForVehicleBy" runat="server" CssClass="formtext" meta:resourcekey="lblSearchForVehicleByResource1" Text="Search for vehicle by:" ></asp:Label>
                    
                    
                    </td>
                            <td align="left">
                    <asp:DropDownList ID="cboSearchType" runat="server" AutoPostBack="True" CssClass="RegularText"
                        OnSelectedIndexChanged="cboSingleSearchType_SelectedIndexChanged" meta:resourcekey="cboSearchTypeResource1">
                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource1" Text="Description" Enabled="false"></asp:ListItem>
                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="License Plate" Enabled="false"></asp:ListItem>
                        <asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="Landmark" 
                            Enabled="False"></asp:ListItem>
                        <asp:ListItem Value="3" meta:resourcekey="ListItemResource4" Text="Unit ID" Enabled="false"></asp:ListItem>
                        <asp:ListItem Value="4" meta:resourcekey="ListItemResource5" Text="Street Address" Selected="True"></asp:ListItem>
                      
                    </asp:DropDownList></td>
                            <td align="left">
                    <asp:TextBox ID="txtSearchParam" runat="server" CssClass="formtext" 
                        meta:resourcekey="txtSearchParamResource1"></asp:TextBox></td>
                        </tr>
                    </table>
                    <table style="width: 638px" >
                        <tr>
                            <td style="width: 115px" valign="top">
                                <table id="tblAddress" runat="server" class="formtext" style="width: 616px">
                                    <tr>
                                        <td style="width: 175px" valign="top">
                                            <asp:Label ID="lblFleetTitle" runat="server" meta:resourcekey="lblFleetTitleResource1" Text="Fleet:"></asp:Label>&nbsp;</td>
                                        <td style="width: 100px">
                                            <asp:DropDownList ID="cboFleetAdrSrch" runat="server" CssClass="RegularText" DataValueField="FleetId"
                                                DataTextField="FleetName" OnSelectedIndexChanged="cboAdvanceFleet_SelectedIndexChanged"
                                                Width="173px" meta:resourcekey="cboFleetAdrSrchResource1">
                                            </asp:DropDownList></td>
                                        <td style="width: 100px" valign="top">
                                            <asp:Label ID="lblWithin1" runat="server" meta:resourcekey="lblWithin1Resource1" Text="within"></asp:Label></td>
                                        <td style="width: 308px" valign="top">
                                            <asp:DropDownList ID="cboDistanceAdrSrc" runat="server" CssClass="RegularText" Width="173px"
                                                meta:resourcekey="cboDistanceAdrSrcResource1">
                                                <asp:ListItem Value="1000" meta:resourcekey="ListItemResource6" Text="1 km/0.6 mi"></asp:ListItem>
                                                <asp:ListItem Value="5000" meta:resourcekey="ListItemResource7" Text="5 km/3.1 mi"></asp:ListItem>
                                                <asp:ListItem Value="10000" meta:resourcekey="ListItemResource8" Text="10 km/6.2 mi"></asp:ListItem>
                                                <asp:ListItem Value="50000" meta:resourcekey="ListItemResource9" Text="50 km/31 mi"></asp:ListItem>
                                                <asp:ListItem Value="100000" meta:resourcekey="ListItemResource10" Text="100 km/62 mi"></asp:ListItem>
                                                <asp:ListItem Value="250000" meta:resourcekey="ListItemResource11" Text="250 km/155 mi"></asp:ListItem>
                                                <asp:ListItem Value="500000" meta:resourcekey="ListItemResource12" Text="500 km/311 mi"></asp:ListItem>
                                            </asp:DropDownList>&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td style="width: 175px" valign="top">
                                            <asp:Label ID="Label3" runat="server" Text="From Address:" Width="90px" meta:resourcekey="Label3Resource1"></asp:Label></td>
                                        <td height="10" style="width: 100px">
                                        </td>
                                        <td style="width: 100px; " valign="top">
                                        </td>
                                        <td style="width: 308px" valign="top">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 175px; " valign="top">
                                            <asp:Label ID="lblStreetTitle" runat="server" meta:resourcekey="lblStreetTitleResource1" Text="Street:"></asp:Label></td>
                                        <td style="width: 100px; ">
                                            <asp:TextBox ID="txtStreet" runat="server" CssClass="formtext" TextMode="MultiLine"
                                                Width="173px" meta:resourcekey="txtStreetResource1"></asp:TextBox></td>
                                        <td style="width: 100px; " valign="top">
                                            <asp:Label ID="lblCityTitle" runat="server" meta:resourcekey="lblCityTitleResource1" Text="City:"></asp:Label></td>
                                        <td style="width: 308px; " valign="top">
                                            <asp:TextBox ID="txtCity" runat="server" CssClass="formtext" Width="173px" meta:resourcekey="txtCityResource1"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 175px; " valign="top">
                                            <asp:Label ID="lblStateTitle" runat="server" meta:resourcekey="lblStateTitleResource1" Text="Province/State:"></asp:Label></td>
                                        <td valign="top">
                                            <asp:TextBox ID="txtState" runat="server" CssClass="formtext" Width="173px" meta:resourcekey="txtStateResource1"></asp:TextBox></td>
                                        <td style="width: 100px; " valign="top">
                                            <asp:Label ID="lblCountryTitle" runat="server" meta:resourcekey="lblCountryTitleResource1" Text="Country:"></asp:Label>
                                        </td>
                                        <td valign="top" style="width: 308px">
                                            <asp:DropDownList ID="cboCountry" runat="server" CssClass="formtext" Width="173px"
                                                meta:resourcekey="cboCountryResource1">
                                                <asp:ListItem Value="Canada" meta:resourcekey="ListItemResource14" Text="Canada" Selected="True"></asp:ListItem>
                                                <asp:ListItem Value="USA" meta:resourcekey="ListItemResource13" Text="USA"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 175px; " valign="top">
                                            </td>
                                        <td style="width: 100px; ">
                                        </td>
                                        <td valign="top">
                                        </td>
                                        <td valign="top" align="right" style="width: 308px">
                                            <asp:TextBox ID="txtX" runat="server" CssClass="formtext" name="txtX" Visible="False"
                                                Width="173px" meta:resourcekey="txtXResource1"></asp:TextBox>
                                            <asp:TextBox ID="txtY" runat="server" CssClass="formtext" name="txtY" Visible="False"
                                                Width="173px" meta:resourcekey="txtYResource1"></asp:TextBox>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            <table style="width: 618px" >
                       <tr>
                          <td style="width:  110%" valign="top">
                             <table id="tblIntersection" runat="server" style="width:  100%;" class="formtext" >
                                <tr>
                                   <td style="width: 905px" valign="top">
                                      <asp:Label ID="lblIntrFleet" runat="server"  Text="Fleet:" meta:resourcekey="lblIntrFleetResource1"></asp:Label>&nbsp;</td>
                                   <td style="width: 100px">
                                      <asp:DropDownList ID="cboIntrFleet" runat="server" CssClass="RegularText" DataValueField="FleetId"
                                                DataTextField="FleetName" OnSelectedIndexChanged="cboAdvanceFleet_SelectedIndexChanged"
                                                Width="173px" meta:resourcekey="cboIntrFleetResource1" >
                                      </asp:DropDownList></td>
                                   <td style="width: 361px" valign="top">
                                      <asp:Label ID="lblIntrWithin" runat="server"  Text="within" meta:resourcekey="lblIntrWithinResource1"></asp:Label></td>
                                   <td style="width: 308px" valign="top">
                                      <asp:DropDownList ID="cboIntrDistance" runat="server" CssClass="RegularText" Width="173px"
                                                Height="14px" meta:resourcekey="cboIntrDistanceResource1" >
                                         <asp:ListItem meta:resourcekey="ListItemResource6" Value="1000" Text="1 km/0.6 mi"></asp:ListItem>
                                         <asp:ListItem meta:resourcekey="ListItemResource7" Value="5000" Text="5 km/3.1 mi"></asp:ListItem>
                                         <asp:ListItem meta:resourcekey="ListItemResource8" Value="10000" Text="10 km/6.2 mi"></asp:ListItem>
                                         <asp:ListItem meta:resourcekey="ListItemResource9" Value="50000" Text="50 km/31 mi"></asp:ListItem>
                                         <asp:ListItem meta:resourcekey="ListItemResource10" Value="100000" Text="100 km/62 mi"></asp:ListItem>
                                         <asp:ListItem meta:resourcekey="ListItemResource11" Value="250000" Text="250 km/155 mi"></asp:ListItem>
                                         <asp:ListItem meta:resourcekey="ListItemResource12" Value="500000" Text="500 km/311 mi"></asp:ListItem>
                                      </asp:DropDownList>&nbsp;</td>
                                </tr>
                                <tr>
                                   <td  colspan="2">
                                      <asp:Label ID="lblIntrTitle" runat="server"  Text="From intersection:"
                                         Width="100%" meta:resourcekey="lblIntrTitleResource1"></asp:Label></td>
                                   <td style="width: 361px; " valign="top">
                                   </td>
                                   <td style="width: 308px" valign="top">
                                   </td>
                                </tr>
                                <tr>
                                   <td valign="top" style="width: 905px;"  >
                                    
                                               <asp:Label ID="lblIntrStreet1" runat="server"  Text="Street 1:" meta:resourcekey="lblIntrStreet1Resource1"></asp:Label>&nbsp;
                                         
                                      
                                   </td>
                                   
                                   <td valign="top" >
                                         
                                               <asp:TextBox ID="txtIntrStreet1" runat="server" CssClass="formtext" 
                                                  Width="173px" meta:resourcekey="txtIntrStreet1Resource1"></asp:TextBox></td>
                                   <td style="width: 361px; " valign="top">
                                               <asp:Label ID="lblIntrStreet2" runat="server"  Text="Street 2:" meta:resourcekey="lblIntrStreet2Resource1"></asp:Label></td>
                                   <td style="width: 308px; " valign="top">
                                               <asp:TextBox ID="txtIntrStreet2" runat="server" CssClass="formtext" 
                                                  Width="173px" meta:resourcekey="txtIntrStreet2Resource1"></asp:TextBox></td>
                                </tr>
                                <tr>
                                   <td style="width: 905px; " valign="top">
                                      <asp:Label ID="lblIntrCity" runat="server" meta:resourcekey="lblCityTitleResource1" Text="City:"></asp:Label></td>
                                   <td valign="top">
                                      <asp:TextBox ID="txtIntrCity" runat="server" CssClass="formtext" meta:resourcekey="txtCityResource1"
                                         Width="173px"></asp:TextBox></td>
                                   <td style="width: 361px;" valign="top">
                                       &nbsp;<asp:Label ID="Label4" runat="server" meta:resourcekey="lblStateTitleResource1"
                                           Text="Province/State:"></asp:Label></td>
                                   <td valign="top" style="width: 308px">
                                       <asp:TextBox ID="txtIntrState" runat="server" CssClass="formtext" meta:resourcekey="txtStateResource1"
                                           Width="173px"></asp:TextBox></td>
                                </tr>
                                <tr>
                                   <td style="width: 905px;" valign="top">
                                      <asp:Label ID="lblIntrCountry" runat="server" meta:resourcekey="lblCountryTitleResource1" Text="Country:"></asp:Label></td>
                                   <td style="width: 100px;">
                                      <asp:DropDownList ID="cbolblIntrCountry" runat="server" CssClass="formtext" Width="173px"
                                                meta:resourcekey="cboCountryResource1">
                                         <asp:ListItem meta:resourcekey="ListItemResource14" Value="Canada" Text="Canada" Selected="True"></asp:ListItem>
                                         <asp:ListItem meta:resourcekey="ListItemResource13" Value="USA" Text="USA"></asp:ListItem>
                                      </asp:DropDownList></td>
                                   <td valign="top" style="width: 361px; ">
                                   </td>
                                   <td valign="top" align="right" style="width: 308px;">
                                       &nbsp; &nbsp;
                                   </td>
                                </tr>
                             </table>
                                            <asp:Button ID="cmdFindAddress" runat="server" 
                                  CssClass="combutton" Text="Find By Location"
                                                Width="121px" OnClick="cmdFindAddress_Click" 
                                   /></td>
                       </tr>
                    </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 115px;" valign="top">
                                <div style="overflow: auto; width: 382px;">
                                    <asp:DataGrid ID="dgAddress" runat="server" AutoGenerateColumns="False" BackColor="White"
                                        BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3" CellSpacing="1"
                                        GridLines="None" OnSelectedIndexChanged="dgAddress_SelectedIndexChanged" Width="341px"
                                        meta:resourcekey="dgAddressResource1" OnItemCommand="dgAddress_ItemCommand">
                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                        <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                        <HeaderStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" Font-Bold="True"
                            ForeColor="White" BackColor="gray"></HeaderStyle>
                                        <Columns>
                                            <asp:BoundColumn DataField="Address" HeaderText='<%$ Resources:dgAddress_Address %>'>
                                            </asp:BoundColumn>
                                            <asp:BoundColumn DataField="Latitude" HeaderText='<%$ Resources:dgAddress_Latitude %>'
                                                Visible="False"></asp:BoundColumn>
                                            <asp:BoundColumn DataField="Longitude" HeaderText='<%$ Resources:dgAddress_Longitude %>'
                                                Visible="False"></asp:BoundColumn>
                                            <asp:ButtonColumn CommandName="Select" Text="Select" meta:resourcekey="ButtonColumnResource1">
                                            </asp:ButtonColumn>
                                            <asp:ButtonColumn CommandName="Map" Text="Map"  >
                                            </asp:ButtonColumn>
                                        </Columns>
                                        <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
                                    </asp:DataGrid></div>
                            </td>
                        </tr>
                    </table>
                    <table id="tblLandmark" class="formtext" style="width: 433px;" cellspacing="2"
                        cellpadding="2" border="0" runat="server">
                        <tr>
                            <td align="left" class="formtext" colspan="4" style="width: 471px; ">
                                <asp:Label ID="Label2" runat="server" Text="In fleet:" meta:resourcekey="Label2Resource1"></asp:Label>
                                <asp:DropDownList ID="cboAdvanceFleet" runat="server" AutoPostBack="True" CssClass="RegularText"
                                    DataValueField="FleetId" DataTextField="FleetName" OnSelectedIndexChanged="cboAdvanceFleet_SelectedIndexChanged"
                                    Width="185px" meta:resourcekey="cboAdvanceFleetResource1">
                                </asp:DropDownList>
                                <asp:Label ID="lblWithin2" runat="server" meta:resourcekey="lblWithin2Resource1" Text="within"></asp:Label>
                                <asp:DropDownList ID="cboAdvanceDist" runat="server" CssClass="RegularText" Width="99px"
                                    Height="14px" meta:resourcekey="cboAdvanceDistResource1">
                                    <asp:ListItem Value="1000" meta:resourcekey="ListItemResource15" Text="1 km/0.6 mi"></asp:ListItem>
                                    <asp:ListItem Value="5000" meta:resourcekey="ListItemResource16" Text="5 km/3.1 mi"></asp:ListItem>
                                    <asp:ListItem Value="10000" meta:resourcekey="ListItemResource17" Text="10 km/6.2 mi"></asp:ListItem>
                                    <asp:ListItem Value="50000" meta:resourcekey="ListItemResource18" Text="50 km/31 mi"></asp:ListItem>
                                    <asp:ListItem Value="100000" meta:resourcekey="ListItemResource19" Text="100 km/62 mi"></asp:ListItem>
                                    <asp:ListItem Value="250000" meta:resourcekey="ListItemResource20" Text="250 km/155 mi"></asp:ListItem>
                                    <asp:ListItem Value="500000" meta:resourcekey="ListItemResource21" Text="500 km/311 mi"></asp:ListItem>
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td align="left" class="formtext" style=" width: 471px;" colspan="4">
                                <asp:Label ID="Label1" runat="server" Text="from Landmark " meta:resourcekey="Label1Resource1"></asp:Label>
                                <asp:DropDownList ID="cboAdvanceLandmarks" runat="server" CssClass="RegularText"
                                    DataValueField="LandmarkName" DataTextField="LandmarkName" Width="373px"
                                    meta:resourcekey="cboAdvanceLandmarksResource1">
                                </asp:DropDownList></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 473px; height: 22px" align="left">
                    &nbsp;<asp:Button
                            ID="cmdClose" runat="server" CssClass="combutton" Width="121px" Text="Close"
                            OnClick="cmdClose_Click" meta:resourcekey="cmdCloseResource1"></asp:Button>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="cmdSearch" runat="server" CssClass="combutton" Width="121px" Text="Search Vehicles"
                        OnClick="cmdSearch_Click" meta:resourcekey="cmdSearchResource1"></asp:Button></td>
            </tr>
            <tr>
                <td style="width: 473px; height: 22px" align="left">
                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="395px" Height="9px"
                        Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label></td>
            </tr>
            <tr>
                <td align="left" valign="top" style="height: 147px">
                                                                                                    <asp:Button ID="cmdSelectAll" runat="server" CssClass="combutton" Text="Select All"
                                                                                        Width="92px" onclick="cmdSelectAll_Click" Visible="False" >
                                                                                    </asp:Button>
                                                                                    &nbsp;
                                                                                    <asp:Button ID="cmdUnselectAll" runat="server" CssClass="combutton" Text="Deselect All"
                                                                                        Width="92px"  Visible="False" onclick="cmdUnselectAll_Click" >
                                                                                    </asp:Button>
                                                                                    
                    <asp:DataGrid ID="dgMultiplyVehicle" runat="server" Width="100%" AllowSorting="True"
                        AllowPaging="True" AutoGenerateColumns="False" BorderColor="#CCCCCC" BorderStyle="None"
                        BorderWidth="2px" BackColor="White" CellPadding="1" PageSize="8" OnPageIndexChanged="dgMultiplyVehicle_PageIndexChanged"
                        meta:resourcekey="dgMultiplyVehicleResource1">
                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                        <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                        <HeaderStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" Font-Bold="True"
                            ForeColor="White" BackColor="gray"></HeaderStyle>
                        <Columns>
                            <asp:BoundColumn Visible="False" DataField="VehicleId" HeaderText='<%$ Resources:dgMultiplyVehicle_VehicleId %>'>
                            </asp:BoundColumn>
                            <asp:TemplateColumn HeaderText='<%$ Resources:dgMultiplyVehicle_Show %>'>
                                <HeaderStyle Width="20px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                
                                 <ItemTemplate>
                                        <asp:CheckBox ID="chkCheckBox"  Checked='<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "chkBoxShow")) %>'
                                            runat="server"
                                             />
                                    </ItemTemplate>
                                    
                                
                            </asp:TemplateColumn>
                            <asp:BoundColumn DataField="Description" HeaderText='<%$ Resources:dgMultiplyVehicle_Description %>'>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="VehicleStatus" HeaderText='<%$ Resources:dgMultiplyVehicle_Status %>'>
                            </asp:BoundColumn>
                           <asp:BoundColumn  DataField="StreetAddress" HeaderText='<%$ Resources:dgMultiplyVehicle_StreetAddress %>'>
                           </asp:BoundColumn>
                           <asp:BoundColumn DataField="DistanceFrom"  HeaderText="" >
                           </asp:BoundColumn>
                        </Columns>
                        <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                            Mode="NumericPages"></PagerStyle>
                    </asp:DataGrid></td>
            </tr>
            <tr>
                <td class="formtext" valign="top" align="left">
                    <asp:DataGrid ID="dgSingleVehicle" runat="server" Width="341px" AllowSorting="True"
                        AllowPaging="True" AutoGenerateColumns="False" BorderColor="#CCCCCC" BorderStyle="None"
                        BorderWidth="2px" BackColor="White" CellPadding="1" DataKeyField="VehicleId"
                        PageSize="8" OnSelectedIndexChanged="dgSingleVehicle_SelectedIndexChanged" OnPageIndexChanged="dgSingleVehicle_PageIndexChanged"
                        meta:resourcekey="dgSingleVehicleResource1">
                       <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                        <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                        <HeaderStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" Font-Bold="True"
                            ForeColor="White" BackColor="gray"></HeaderStyle>
                        <Columns>
                            <asp:BoundColumn DataField="Description" HeaderText='<%$ Resources:dgSingleVehicle_Description %>'>
                                <HeaderStyle Width="400px"></HeaderStyle>
                            </asp:BoundColumn>
                            <asp:BoundColumn DataField="VehicleStatus" HeaderText='<%$ Resources:dgSingleVehicle_Status %>'>
                                <HeaderStyle Width="170px"></HeaderStyle>
                            </asp:BoundColumn>
                            <asp:ButtonColumn Text="Select" CommandName="Select" meta:resourcekey="ButtonColumnResource2">
                            </asp:ButtonColumn>
                        </Columns>
                        <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                            Mode="NumericPages"></PagerStyle>
                    </asp:DataGrid></td>
            </tr>
            <tr>
                <td align="left" style="width: 473px;">
                    <asp:Button ID="cmdGoTo" runat="server" CssClass="combutton" Width="121px"
                        Text="Map It" Visible="False" OnClick="cmdGoTo_Click" 
                        meta:resourcekey="cmdGoToResource1">
                    </asp:Button></td>
            </tr>
        </table>
    </form>
</body>
</html>
