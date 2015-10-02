<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmNotificationInfo.aspx.cs" Inherits="SentinelFM.Dashboard_frmNotificationInfo" meta:resourcekey="PageResource1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Notification Info</title>
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    <style type="text/css">
        .style1
        {
            width: 288px;
        }
    </style>
    </head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <table class="formtext" width="100%" >
            <tr>
                <td>
                    <asp:Label ID="lblVehicleCaption" runat="server" Font-Bold="True" 
                        Text="Vehicle:" meta:resourcekey="lblVehicleCaptionResource1"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblVehicle" runat="server" 
                        meta:resourcekey="lblVehicleResource1"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblLicensePlateCaption" runat="server" Font-Bold="True" 
                        Text="LicensePlate:" meta:resourcekey="lblLicensePlateCaptionResource1"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblLicensePlate" runat="server" 
                        meta:resourcekey="lblLicensePlateResource1"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                       <asp:Label ID="lblDateTime" runat="server" CssClass="formtext" Text="Date:" 
                        Font-Bold="True" meta:resourcekey="lblDateTimeResource1"></asp:Label>
                </td>
                <td>
                       <asp:Label ID="lblDateTimeValue" runat="server" CssClass="formtext" 
                           meta:resourcekey="lblDateTimeValueResource1"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                       <asp:Label ID="lblAddressCaption" runat="server" 
                           CssClass="formtext" Text="Address:" Font-Bold="True" 
                           meta:resourcekey="lblAddressCaptionResource1"></asp:Label>
                </td>
                <td>
                       <asp:Label ID="lblAddress" runat="server" 
                           meta:resourcekey="lblAddressResource1"></asp:Label>
                </td>
            </tr>
        </table>
    
    </div>
    <asp:MultiView ID="viewNotications"  runat="server">
        <asp:View ID="ViewDTC"  runat="server">
          <table>
          
          
          <tr>
                <td align=left class="style1"  >
                    <asp:Label ID="lblDTCLabel" runat="server" CssClass="formtext" 
                        Text="DTC Information" Font-Bold="True" 
                        meta:resourcekey="lblDTCLabelResource1"></asp:Label>
                </td>
               
            </tr>
            
            
           
                 <tr>
                  <td align="left" class="style1">
                       <asp:Label ID="lblDTCSourceCaption" runat="server" CssClass="formtext" 
                           Text="DTC Source:" meta:resourcekey="lblDTCSourceCaptionResource1"></asp:Label>
                      <asp:Label ID="lblDTCSourceValue" runat="server" CssClass="formtext" 
                           meta:resourcekey="lblDTCSourceValueResource1"></asp:Label>
                  </td>
                 
              </tr>
              
              <tr>
                  <td align="left" class="style1">
                      <asp:Label ID="lblDTCCntCaption" runat="server" CssClass="formtext" 
                          Text="DTC Received:" meta:resourcekey="lblDTCCntCaptionResource1"></asp:Label>
                      <asp:Label ID="lblDTCCnt" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblDTCCntResource1"></asp:Label>
                  </td>
                 
              </tr>
            
              <tr>
                  <td align="left" class="style1">
                      <asp:Label ID="lblDTCInVehicleCaption" runat="server" CssClass="formtext" 
                          Text="DTC In Vehicle:" meta:resourcekey="lblDTCInVehicleCaptionResource1"></asp:Label>
                      <asp:Label ID="lblDTCInVehicle" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblDTCInVehicleResource1"></asp:Label>
                  </td>
              </tr>
            
              <tr>
                  <td align="left" class="style1">
                      <asp:Label ID="lblDTC" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblDTCResource1"></asp:Label>
                  </td>
              </tr>
            
          <!--
            <tr>
                <td><asp:Label runat=server ID="lblDTCSourceLabel" Text="DTC Source:"  CssClass="formtext"  ></asp:Label> </td>
                <td><asp:Label runat=server ID="lblDTCSource" Text=""  CssClass="formtext"  ></asp:Label> </td>
            </tr>
            
             <tr>
                <td><asp:Label runat=server ID="lblDTCCountLabel" Text="DTC Count:"  CssClass="formtext"  ></asp:Label> </td>
                <td><asp:Label runat=server ID="lblDTCCount" Text=""  CssClass="formtext"  ></asp:Label> </td>
            </tr>
            -->
            
               <tr>
                <td class="style1">
                    <asp:Label ID="lblDTCcodeLabel" runat="server" CssClass="formtext" 
                        Text="DTC Code" meta:resourcekey="lblDTCcodeLabelResource1"></asp:Label>
                   </td>
               
            </tr>
              <tr>
                  <td class="style1">
                      <asp:Label ID="lblDTCcode" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblDTCcodeResource1"></asp:Label>
                  </td>
                
              </tr>
              <tr>
                  <td class="style1">
                      <asp:TextBox ID="lblDTCcodeDescription" runat="server" 
                          TextMode="MultiLine" ReadOnly="True" Height="80px" Width="100%" 
                          meta:resourcekey="lblDTCcodeDescriptionResource1"></asp:TextBox>
                  </td>
              </tr>
          </table> 
        </asp:View> 
        
         <asp:View ID="ViewMIL"  runat="server">
          <table>
            <tr>
                <td><asp:Label runat=server ID="lblMILLightLabel" Text="MIL Light:"  
                        CssClass="formtext" Font-Bold="True" 
                        meta:resourcekey="lblMILLightLabelResource1"  ></asp:Label> </td>
                <td><asp:Label runat=server ID="lblMILLigh"  CssClass="formtext" 
                        meta:resourcekey="lblMILLighResource1"  ></asp:Label> </td>
            </tr>
          </table> 
        </asp:View> 


        <asp:View ID="ViewNotifications"  runat="server">
          <table>
          
          
          <tr>
                <td align=left  >
                    <asp:Label ID="lblMaintenance" runat="server" CssClass="formtext" 
                        Text="Maintenance Information" Font-Bold="True" 
                        meta:resourcekey="lblMaintenanceResource1"></asp:Label>
                </td>
               
            </tr>
            
            
           
              <tr>
                  <td>
                  <asp:Label ID="lblBoxIdNotificationCaption" runat="server" CssClass="formtext" 
                          Text="BoxId:" meta:resourcekey="lblBoxIdNotificationCaptionResource1"></asp:Label>
                      <asp:Label ID="lblBoxIdNotification" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblBoxIdNotificationResource1"></asp:Label>
                  </td>
                 
              </tr>
             
                 <tr>
                  <td>
                      <asp:Label ID="lblServiceNotificationCaption" runat="server" 
                          CssClass="formtext" Text="Service:" 
                          meta:resourcekey="lblServiceNotificationCaptionResource1"></asp:Label>
                      <asp:Label ID="lblServiceNotification" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblServiceNotificationResource1"></asp:Label>
                  </td>
                 
              </tr>
              
               <tr>
                  <td>
                      <asp:Label ID="lblStatusNotificationCaption" runat="server" CssClass="formtext" 
                          Text="Status:" meta:resourcekey="lblStatusNotificationCaptionResource1"></asp:Label>
                      <asp:Label ID="lblStatusNotification" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblStatusNotificationResource1"></asp:Label>
                  </td>
                 
              </tr>
              
              
              <tr>
                  <td>
                      <asp:Label ID="lblMessageNotificationCaption" runat="server" 
                          CssClass="formtext" Text="Message:" 
                          meta:resourcekey="lblMessageNotificationCaptionResource1"></asp:Label>
                      <asp:Label ID="lblMessageNotification" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblMessageNotificationResource1"></asp:Label>
                  </td>
                 
              </tr>
              
            
               
              <tr>
                  <td>
                      <asp:Label ID="lblMaintenanceValue" runat="server" CssClass="formtext" 
                          meta:resourcekey="lblMaintenanceValueResource1"></asp:Label>
                  </td>
                 
              </tr>
              </table> 
        </asp:View> 

    </asp:MultiView>
   <table width=99% ><tr><td align=right  ><br /><br /><br /><br />
                                                                <asp:Button runat="server" CssClass="combutton" ID="cmdClose" OnClientClick="top.close()"
                                                            Text="Close" 
           meta:resourcekey="cmdCloseResource1" />
   </td></tr></table>
    </form>
</body>
</html>
