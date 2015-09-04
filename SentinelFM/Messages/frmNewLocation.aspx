<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmNewLocation.aspx.cs" Inherits="SentinelFM.Messages_frmNewLocation" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Send location to Garmin</title>
   
  
   
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        .style2
        {
            width: 76px;
        }
    </style>
   
  
   
</head>
<body>
    <%if (LoadVehiclesBasedOn == "hierarchy")
  {%>
  
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>     

    <script language="javascript">
		<!--
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
			var myname='OrganizationHierarchy';
			var w=740;
			var h=440;
			var winl = (screen.width - w) / 2; 
			var wint = (screen.height - h) / 2; 
			winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
			win = window.open(mypage, myname, winprops) 
			if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }  
            return false;
        }

        function OrganizationHierarchyNodeSelected(nodecode, fleetId)
        {            
            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
            $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
            $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
        }
            
			//-->
    </script>
<%} %>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />
    <%if (LoadVehiclesBasedOn == "hierarchy")
          {%>        
            <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" 
        Text="Button" style="display:none;" AutoPostBack="True"
            OnClick="hidOrganizationHierarchyPostBack_Click" 
        meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />
        <%} %>

   <fieldset>
    <table>
        <tr>
            <td align=center >
                                                 <fieldset style="padding: 10px 10px 10px 10px;">
                                                     <table id="Table2" border="0" cellpadding="0" cellspacing="0" 
                                                         style="height: 14px;width:100%">
                                                         <tr>
                                                             <td class="formtext">
                                                                 <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" 
                                                                     meta:resourcekey="lblFleetTitleResource1" Text="Fleet:"></asp:Label>
                                                                 <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" Text=" Hierarchy Node:" 
                                                                    Visible="False" meta:resourcekey="lblOhTitleResource1"  />
                                                                 <asp:RangeValidator ID="valFleet" runat="server" ControlToValidate="cboFleet" 
                                                                     ErrorMessage="Please select a Fleet" MaximumValue="999999999999999" 
                                                                     meta:resourcekey="valFleetResource1" MinimumValue="1" Text="*"></asp:RangeValidator>
                                                                 <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" CssClass="combutton" 
                                                                       Visible="False" OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                                                       meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" /> 
                                                             </td>
                                                             <td>
                                                                 <asp:DropDownList ID="cboFleet" runat="server" AutoPostBack="True" 
                                                                     CssClass="RegularText" DataTextField="FleetName" DataValueField="FleetId" 
                                                                     meta:resourcekey="cboFleetResource1" 
                                                                     OnSelectedIndexChanged="cboFleet_SelectedIndexChanged">
                                                                 </asp:DropDownList>
                                                                 &nbsp;&nbsp;</td>
                                                             <td>
                                                                 &nbsp;</td>
                                                             <td>
                                                                 <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" 
                                                                     meta:resourcekey="lblVehicleNameResource1" Text="Vehicle:" Visible="False"></asp:Label>
                                                             </td>
                                                             <td>
                                                                 <asp:DropDownList ID="cboVehicle" runat="server" AutoPostBack="True" 
                                                                     CssClass="RegularText" DataTextField="Description" DataValueField="BoxId" 
                                                                     DESIGNTIMEDRAGDROP="79" meta:resourcekey="cboVehicleResource1" 
                                                                     Visible="False">
                                                                 </asp:DropDownList>
                                                             </td>
                                                         </tr>
                                                     </table>
                                                 </fieldset>
                                                 <br />
                                                 </td>
        </tr>
        <tr>
            <td align=center >
                                                 <asp:RadioButtonList ID="lstAddOptions" runat="server" AutoPostBack="True" 
                                                         CssClass="formtext" Font-Bold="True" Height="23px" 
                                                         
                                                         
                                                         RepeatDirection="Horizontal" Width="470px" 
                                                     onselectedindexchanged="lstAddOptions_SelectedIndexChanged" 
                                                     meta:resourcekey="lstAddOptionsResource1">
                                                     <asp:ListItem  Selected="True" 
                                                             Text="By Street Address" Value="0" 
                                                         meta:resourcekey="ListItemResource1"></asp:ListItem>
                                                     <asp:ListItem Text="By Coordinates" 
                                                             Value="1" meta:resourcekey="ListItemResource2"></asp:ListItem>
                                                     <asp:ListItem Value="2" meta:resourcekey="ListItemResource3">By Landmark</asp:ListItem>
                                                 </asp:RadioButtonList>
                                                 </td>
        </tr>
        <tr>
            <td align=center  >
                                                                 <table ID="tblStreet" runat="server" border="0" cellpadding="0" cellspacing="0" 
                                                                     class="formtext">
                                                                     <tr>
                                                                         <td align="left">
                                                                             <asp:Label ID="lblStreetTitle" runat="server" 
                                                                                  Text="Street:" meta:resourcekey="lblStreetTitleResource1"></asp:Label>
                                                                         </td>
                                                                         <td style="width: 195px; height: 32px">
                                                                             <asp:TextBox ID="txtStreet" runat="server" CssClass="formtext" 
                                                                                 
                                                                                 TextMode="MultiLine" Width="173px" meta:resourcekey="txtStreetResource1"></asp:TextBox>
                                                                         </td>
                                                                         <td style="width: 99px; height: 32px">
                                                                             <asp:Label ID="lblCityTitle" runat="server" 
                                                                                 Text="City:" meta:resourcekey="lblCityTitleResource1"></asp:Label>
                                                                         </td>
                                                                         <td style="height: 32px">
                                                                             <asp:TextBox ID="txtCity" runat="server" CssClass="formtext" 
                                                                                 
                                                                                 Width="173px" meta:resourcekey="txtCityResource1"></asp:TextBox>
                                                                         </td>
                                                                     </tr>
                                                                     <tr>
                                                                         <td class="style4">
                                                                             <asp:Label ID="lblStateProvinceTitle" runat="server" 
                                                                                  Text="State (Prov):" meta:resourcekey="lblStateProvinceTitleResource1"></asp:Label>
                                                                         </td>
                                                                         <td style="width: 195px">
                                                                             <asp:TextBox ID="txtState" runat="server" CssClass="formtext" Height="22px" 
                                                                                 
                                                                                 Width="173px" meta:resourcekey="txtStateResource1"></asp:TextBox>
                                                                         </td>
                                                                         <td style="width: 99px">
                                                                             <asp:Label ID="lblCountryTitle" runat="server" 
                                                                                 Text="Country" meta:resourcekey="lblCountryTitleResource1"></asp:Label>
                                                                             :</td>
                                                                         <td>
                                                                             <asp:DropDownList ID="cboCountry" runat="server" CssClass="formtext" 
                                                                                 
                                                                                  Width="173px" meta:resourcekey="cboCountryResource1">
                                                                                 <asp:ListItem  Selected="True" Text="USA" 
                                                                                     Value="USA" meta:resourcekey="ListItemResource4"></asp:ListItem>
                                                                                 <asp:ListItem Text="Canada" Value="Canada" meta:resourcekey="ListItemResource5"></asp:ListItem>
                                                                             </asp:DropDownList>
                                                                         </td>
                                                                     </tr>
                                                                     <tr>
                                                                         <td class="style4">
                                                                             &nbsp;</td>
                                                                         <td style="width: 195px">
                                                                             &nbsp;</td>
                                                                         <td style="width: 99px">
                                                                             &nbsp;</td>
                                                                         <td>
                                                                             &nbsp;</td>
                                                                     </tr>
                                                                     <tr>
                                                                         <td class="style4" colspan="4" align="center">
                                                     <asp:Button ID="cmdSave" runat="server" CssClass="combutton" 
                                                          OnClick="cmdSaveLandmark_Click" 
                                                         Text="Find Location" Visible="False" meta:resourcekey="cmdSaveResource1" />
                                                                         </td>
                                                                     </tr>
                                                                     <tr>
                                                                         <td class="style4" colspan="4" align="center">
    
                                                     <div style="overflow: auto; width: 349px; ">
                                                         <asp:DataGrid ID="dgAddress" runat="server" AutoGenerateColumns="False" 
                                                             BackColor="White" BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" 
                                                             CellPadding="3" CellSpacing="1" GridLines="None" 
                                                             
                                                             OnSelectedIndexChanged="dgAddress_SelectedIndexChanged" Width="271px" 
                                                             meta:resourcekey="dgAddressResource1">
                                                             <SelectedItemStyle BackColor="SlateGray" Font-Bold="True" ForeColor="White" />
                                                             <AlternatingItemStyle BackColor="WhiteSmoke" />
                                                             <ItemStyle BackColor="#DEDFDE" Font-Size="11px" ForeColor="Black" />
                                                             <HeaderStyle CssClass="DataHeaderStyle" />
                                                             <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />
                                                             <Columns>
                                                                 <asp:BoundColumn DataField="Address" HeaderText="Street Address">
                                                                 </asp:BoundColumn>
                                                                 <asp:BoundColumn DataField="Latitude" HeaderText="Latitude" Visible="False">
                                                                 </asp:BoundColumn>
                                                                 <asp:BoundColumn DataField="Longitude" HeaderText="Longitude" Visible="False">
                                                                 </asp:BoundColumn>
                                                                 <asp:ButtonColumn CommandName="Select"  
                                                                     Text="Select" meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
                                                             </Columns>
                                                             <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />
                                                         </asp:DataGrid>
                                                     </div>
    
                                                                         </td>
                                                                     </tr>
                                                                 </table>
                                                                 </td>
        </tr>
        <tr>
            <td align=left  >
                                                                 <table ID="tblCoordinates" border="0" cellpadding="0" cellspacing="0" 
                                                                     runat="server">
                                                                     <tr>
                                                                         <td >
                                                                             <asp:Label ID="lblX" runat="server" CssClass="formtext" 
                                                                                  Text="Latitude:" meta:resourcekey="lblXResource1"></asp:Label>
                                                                         </td>
                                                                         <td align="left">
                                                                             <asp:TextBox ID="txtLatitude" runat="server" CssClass="formtext" 
                                                                                  name="txtY" Width="150px" meta:resourcekey="txtLatitudeResource1">0</asp:TextBox>
                                                                         </td>
                                                                     
                                                                     </tr>
                                                                     <tr>
                                                                         <td >
                                                                             <asp:Label ID="lblY" runat="server" CssClass="formtext" 
                                                                                  Text="Longitude:" meta:resourcekey="lblYResource1"></asp:Label>
                                                                         </td>
                                                                         <td align="left">
                                                                             <asp:TextBox ID="txtLongitude" runat="server" CssClass="formtext" 
                                                                                 name="txtX" Width="150px" meta:resourcekey="txtLongitudeResource1">0</asp:TextBox>
                                                                         </td>
                                                                       
                                                                     </tr>
                                                                 </table>
                                                                 
                                                                  <table ID="tblLandmarks" border="0" cellpadding="0" cellspacing="0" 
                                                                     runat="server">
                                                                     <tr>
                                                                         <td >
                                                                             <asp:Label ID="lblLandmark" runat="server" CssClass="formtext" 
                                                                                  Text="Landmark:" meta:resourcekey="lblLandmarkResource1"></asp:Label>
                                                                         </td>
                                                                         <td align="left">
                                <asp:DropDownList ID="cboLandmarks" runat="server" CssClass="RegularText"
                                    DataValueField="LandmarkName" DataTextField="LandmarkName" Height="14px" Width="457px"
                                    meta:resourcekey="cboAdvanceLandmarksResource1">
                                </asp:DropDownList>
                                                                         </td>
                                                                     
                                                                     </tr>
                                                                     
                                                                 </table>
                                                             </td>
        </tr>
        <tr>
            <td align=left  >
                                                     <table cellpadding=1 cellspacing=0   >
                                                         <tr>
                                                             <td align="left">
                                                                 <asp:Label ID="lblMessagetext" runat="server" Text="Message:" 
                                                                     CssClass="formtext" meta:resourcekey="lblMessagetextResource1"></asp:Label>
                                                             </td>
                                                             <td align="left">
    
                    <asp:TextBox ID="txtMessageBody" runat="server" CssClass="formtext" 
                        TextMode="MultiLine" Height="40px" Width="460px" meta:resourcekey="txtMessageBodyResource1" ></asp:TextBox>
    
                                                             </td>
                                                         </tr>
                                                     </table>
                                                 </td>
        </tr>
        <tr>
            <td align=center  >
                    &nbsp;&nbsp;
                                                     </td>
        </tr>
        <tr>
            <td align=center  >
    
                                                     &nbsp;</td>
        </tr>
        <tr>
            <td align=center  >
            
        <asp:Label ID="lblMessage" runat="server" CssClass="formtext" 
                    meta:resourcekey="lblMessageResource1"></asp:Label>
    
                                                 </td>
        </tr>
        <tr>
            <td align=center  >
    
                    <asp:Button runat="server" CssClass="combutton" ID="cmdClose"  
                        OnClientClick="top.close()" Text="Close" meta:resourcekey="cmdCloseResource1" /> &nbsp;&nbsp;
    
                                                 <asp:Button ID="cmdSent" runat="server" 
                        CssClass="combutton" Text="Send" 
                        onclick="cmdSent_Click" meta:resourcekey="cmdSentResource1" />
    
                                                 </td>
        </tr>
    </table>
    </fieldset> 
    </form>
</body>
</html>
