<%@ Page Language="c#" Inherits="SentinelFM.History.frmHistDetails" CodeFile="frmHistDetails.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>History Details</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0"/>
    <meta name="CODE_LANGUAGE" content="C#"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
</head>

<script type="text/javascript" >
    function popUpGooglemap() {
        var lat = document.getElementById("<%= lblLatitude.ClientID%>").innerHTML;
        var lon = document.getElementById("<%= lblLongitude.ClientID%>").innerHTML;
        var street = document.getElementById("<%= lblStreetAddress.ClientID%>").innerHTML;
        var mypage = "http://maps.google.com/maps?q=" + lat + ",+" + lon + "&iwloc=A&hl=en";
        var myname = 'map';
        var w = 1024;
        var h = 768;
        var winl = (screen.width - w) / 2;
        var wint = (screen.height - h) / 2;
        winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=yes,'
        win = window.open(mypage, myname, winprops)
        if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
    }

    </script>

<body  scroll="auto">
    <form id="frmHistDetailsForm" method="post" runat="server">
        <table id="Table1" cellspacing="0" cellpadding="0" border="0" style="width: 529px;
            border-right: black 2px solid; border-top: black 2px solid; border-left: black 2px solid;
            border-bottom: black 2px solid;" class="configTabBackground">
            <tr>
                <td align="center" style="width: 529px">
                    <table id="tblGeneralInfo" cellspacing="0" cellpadding="0" border="0" style="height:98%">
                        <tr>
                            <td class="formtext" height="10"></td>
                            <td height="10"></td>
                            <td height="10"></td>
                            <td height="10" style="width: 127px"></td>
                            <td height="10"></td>
                        </tr>
                        <tr>
                            <td class="formtext">
                                &nbsp;
                                <asp:Label ID="lblOriginatedDateTitle" runat="server" CssClass="formtext" meta:resourcekey="lblOriginatedDateTitleResource1" Text="Originated Date:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblOriginatedDate" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblOriginatedDateResource1"/>
                            </td>
                            <td></td>
                            <td style="width: 127px">
                                <asp:Label ID="lblBoxIdTitle" runat="server" CssClass="formtext" meta:resourcekey="lblBoxIdTitleResource1" Text="BoxId:"/>
                            </td>
                            <td >
                                <asp:Label ID="lblBoxId" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblBoxIdResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td height="10" style="width: 20px"></td>
                            <td height="10" style="width: 20px"></td>
                            <td height="10" style="width: 20px"></td>
                            <td height="10" style="width: 127px"></td>
                            <td height="10" style="width: 20px"></td>
                        </tr>
                        <tr>
                            <td class="formtext">
                                &nbsp;
                                <asp:Label ID="lblReceivedDateTitle" runat="server" CssClass="formtext" meta:resourcekey="lblReceivedDateTitleResource1" Text="Received Date:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblReceivedDate" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblReceivedDateResource1"/>
                            </td>
                            <td></td>
                            <td style="width: 127px">
                                <asp:Label ID="lblHeadingTitle" runat="server" CssClass="formtext" meta:resourcekey="lblHeadingTitleResource1" Text="Heading:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblHeading" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblHeadingResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext"  height="10"></td>
                            <td style=" height: 10px;"></td>
                            <td style=" height: 10px"></td>
                            <td style=" height: 10px; width: 127px;"></td>
                            <td style=" height: 10px"></td>
                        </tr>
                        <tr>
                            <td class="formtext">
                                &nbsp;
                                <asp:Label ID="lblMessageTypeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMessageTypeTitleResource1" Text="Message Type:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblMessageType" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblMessageTypeResource1"/>
                            </td>
                            <td></td>
                            <td style="width: 127px">
                                <asp:Label ID="lblSpeedTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSpeedTitleResource1" Text="Speed:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblSpeed" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblSpeedResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext" height="10"></td>
                            <td height="10"></td>
                            <td height="10"></td>
                            <td height="10" style="width: 127px"></td>
                            <td height="10"></td>
                        </tr>
                        <tr>
                            <td class="formtext"  valign="top">
                                &nbsp;
                                <asp:Label ID="lblMessageTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMessageTitleResource1" Text="Message:"/>
                            </td>
                            <td colspan="4">
                                <asp:TextBox ID="lblMessage" runat="server" BackColor="WhiteSmoke" TextMode="MultiLine" CssClass="formtext" Height="106px" Width="99%" ReadOnly="True" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblMessageResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td height="10" style="width: 150px;"></td>
                            <td height="10" style="height: 5px;"></td>
                            <td height="10" style=" height: 5px"></td>
                            <td height="10" style=" height: 5px; width: 127px;"></td>
                            <td height="10" style=" height: 5px"></td>
                        </tr>
                        <tr>
                            <td class="formtext">
                                &nbsp;
                                <asp:Label ID="lblStreetAddressTitle" runat="server" CssClass="formtext" meta:resourcekey="lblStreetAddressTitleResource1" Text="Street Address:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblStreetAddress" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblStreetAddressResource1"/>
                            </td>
                            <td></td>
                            <td style="width: 127px">
                                <asp:Label ID="lblArmedTitle" runat="server" CssClass="formtext" meta:resourcekey="lblArmedTitleResource1" Text="Armed:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblArmed" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblArmedResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext"  height="10"></td>
                            <td height="10"></td>
                            <td height="10">
                                &nbsp; &nbsp; &nbsp;&nbsp;
                            </td>
                            <td height="10" style="width: 127px"></td>
                            <td height="10"></td>
                        </tr>
                        <tr>
                            <td class="formtext">
                                &nbsp;
                                <asp:Label ID="lblLatitudeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblLatitudeTitleResource1" Text="Latitude:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblLatitude" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblLatitudeResource1"/>
                            </td>
                            <td></td>
                            <td style="width: 127px">
                                <asp:Label ID="lblValidGPSTitle" runat="server" CssClass="formtext" meta:resourcekey="lblValidGPSTitleResource1" Text="Valid GPS:"/>
                            </td>
                            <td>
                                <asp:Label ID="lblValidGPS" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblValidGPSResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext"  height="10"></td>
                            <td height="10"></td>
                            <td height="10"></td>
                            <td height="10" style="width: 127px"></td>
                            <td height="10"></td>
                        </tr>
                        <tr>
                            <td height="10" class="formtext">
                                &nbsp;
                                <asp:Label ID="lblLongitudeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblLongitudeTitleResource1" Text="Longitude:"/>
                            </td>
                            <td height="10">
                                <asp:Label ID="lblLongitude" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblLongitudeResource1"/>
                            </td>
                            <td height="10"></td>
                            <td height="10" style="width: 127px">
                                <asp:Label ID="lblProtocolTypeTitle" runat="server" CssClass="formtext" meta:resourcekey="lblProtocolTypeTitleResource1" Text="Protocol Type:"/>
                            </td>
                            <td height="10">
                                <asp:Label ID="lblProtocolType" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblProtocolTypeResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 150px;" height="10"></td>
                            <td height="10"><a id="googlemap" href="javascript:popUpGooglemap();"  style="font-size: 11px;font-family: verdana;">Google Map</a></td>
                            <td height="10"></td>
                            <td height="10" style="width: 127px"></td>
                            <td height="10"></td>
                        </tr>
                        <tr>
                            <td style="width: 150px;" height="10"></td>
                            <td height="10"></td>
                            <td height="10"></td>
                            <td height="10" style="width: 127px"></td>
                            <td height="10"></td>
                        </tr>
                        <tr>
                            <td class="formtext" style="width: 150px;" height="10">&nbsp;</td>
                            <td height="10">&nbsp;</td>
                            <td height="10">&nbsp;</td>
                            <td height="10" style="width: 127px">&nbsp;</td>
                            <td height="10">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="formtext" style="width: 150px; height: 10px">
                                &nbsp;
                                <asp:Label ID="lblOdometerTitle" runat="server" CssClass="formtext" meta:resourcekey="lblOdometerTitleResource1" Text="Odometer"/>
                                <asp:Label ID="lblUnit" runat="server" CssClass="formtext" meta:resourcekey="lblUnitResource1"/>:
                            </td>
                            <td style="height: 10px; ">
                                <asp:Label ID="lblOdometer" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblOdometerResource1"/>
                            </td>
                            <td style=" height: 10px"></td>
                            <td style=" height: 10px; width: 127px;">
                                <asp:Label ID="lblReasonForIPUpdateTitle" runat="server" CssClass="formtext" meta:resourcekey="lblReasonForIPUpdateTitleResource1" Text="Reason for IP Update:"/>
                            </td>
                            <td style=" height: 10px">
                                <asp:Label ID="lblReasonForIPUpdate" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblReasonForIPUpdateResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext" style="width: 10px;" height="10"></td>
                            <td style=" height: 10px;"></td>
                            <td style=" height: 10px"></td>
                            <td style=" height: 10px; width: 127px;"></td>
                            <td style=" height: 10px"></td>
                        </tr>
                        <tr>
                            <td class="formtext" height="10">
                                &nbsp;
                                <asp:Label ID="lblUserTitle" runat="server" CssClass="formtext" meta:resourcekey="lblUserTitleResource1" Text="User:"/>
                            </td>
                            <td height="10">
                                <asp:Label ID="lblUser" runat="server" CssClass="RegularText" Font-Bold="False" Font-Italic="False" meta:resourcekey="lblUserResource1"/>
                            </td>
                            <td height="10"></td>
                            <td height="10" style="width: 127px">
                                <asp:Label ID="lblFuelConsumption" runat="server" CssClass="formtext" Text="Fuel Consumption:" Width="128px" meta:resourcekey="lblFuelConsumptionResource1"/>
                            </td>
                            <td height="10">
                                <asp:Label ID="lblFuelConsuptionValue" runat="server" CssClass="formtext" meta:resourcekey="lblFuelConsuptionValueResource1"/>
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext" style="height: 10px"></td>
                            <td style="height: 10px"></td>
                            <td style="height: 10px"></td>
                            <td style="width: 127px; height: 10px;"></td>
                            <td style="height: 10px"></td>
                        </tr>
                        <tr>
                            <td class="formtext" style="width: 150px; height: 13px;">
                                &nbsp;
                                <asp:Label ID="lblMIL" runat="server" meta:resourcekey="lblMILResource1" Text="MIL:"/>
                            </td>
                            <td style="height: 13px;">
                                <asp:Label ID="lblMILvalue" runat="server" CssClass="formtext" meta:resourcekey="lblMILvalueResource1"/>
                            </td>
                            <td style=" height: 13px"></td>
                            <td style=" height: 13px; width: 127px;">
                                <asp:Label ID="lblProtocolId" runat="server" CssClass="RegularText" Visible="False" meta:resourcekey="lblProtocolIdResource1"/>
                            </td>
                            <td style=" height: 13px">&nbsp;</td>
                        </tr>
                        <tr>
                            <td class="formtext" height="10"></td>
                            <td height="10"></td>
                            <td height="10"></td>
                            <td height="10" style="width: 127px"></td>
                            <td height="10"></td>
                        </tr>
                        <tr>
                            <td class="formtext" style="height: 10px">
                                &nbsp;
                                <asp:Label ID="lblMainBattery" runat="server" CssClass="formtext" Text="Main Battery:" Visible="False"/>
                            </td>
                            <td style="height: 10px">
                                <asp:Label ID="lblMainBatteryValue" runat="server" CssClass="formtext" Visible="False"/>
                            </td>
                             <td style="height: 10px"></td>
                            <td style="width: 127px; height: 10px">
                                <asp:Label ID="lblDriver" runat="server" CssClass="formtext" 
                                    Text="DriverID:" Visible="False"/>
                            </td>
                            <td style="height: 10px">
                                <asp:Label ID="lblDriverID" runat="server" CssClass="formtext" 
                                    meta:resourcekey="lblDriverID"/>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center" style="width: 100%">
                    <div runat="server" ID="tblReefer">
                        <asp:Label runat="server" ID="lblSetup" CssClass="formtext" Font-Bold="True" meta:resourcekey="lblSetupResource1"/>
                        <br />
                        <fieldset style="width: 90%; text-align: left;" visible="false" runat="server" id="fldTemperature">
                            <legend>
                                <asp:Label runat="server" ID="lblTemperature" CssClass="formtext" Font-Bold="True" meta:resourcekey="lblTemperatureResource1"/>
                            </legend>
                            <asp:Repeater runat="server" ID="rptTemperature">
                                <HeaderTemplate>
                                    <table style="border: solid 1px black; margin: 5px 0px 10px 20px;">
                                        <tr class="formtext" style="font-weight:bold; background-color: gray; color: White;">
                                            <th style="width:150px;">Name</th>
                                            <th style="width:60px;">Active</th>
                                            <th style="width:60px;">Lower</th>
                                            <th style="width:60px;">Current</th>
                                            <th style="width:60px;">Upper</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr style="font-family: Arial,Helvetica,sans-serif; font-size:small; color:Black; background-color:#DEDFDE;">
                                        <td class="formtext" style="text-align:left;">
                                            <%# DataBinder.Eval(Container.DataItem, "Header") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Active") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Lower") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Current") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Upper") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr style="font-family: Arial,Helvetica,sans-serif; font-size:small; color:Black; background-color:WhiteSmoke;">
                                        <td class="formtext" style="text-align:left;">
                                            <%# DataBinder.Eval(Container.DataItem, "Header") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Active") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Lower") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Current") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Upper") %>
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                                <FooterTemplate></table></FooterTemplate>
                            </asp:Repeater>
                        </fieldset>
                        <br />
                        <fieldset style="width: 90%; text-align: left;" visible="false" runat="server" id="fldFuel">
                            <legend>
                                <asp:Label runat="server" ID="lblFuel" CssClass="formtext" Font-Bold="True" meta:resourcekey="lblFuelResource1"/>
                            </legend>
                            <asp:Repeater runat="server" ID="rptFuel">
                                <HeaderTemplate>
                                    <table style="border: solid 1px black; margin: 5px 0px 10px 20px;">
                                        <tr class="formtext" style="font-weight:bold; background-color: gray; color: White;">
                                            <th style="width:100px;">Active</th>
                                            <th style="width:100px;">Lower</th>
                                            <th style="width:100px;">Current</th>
                                            <th style="width:100px;">Upper</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr style="font-family: Arial,Helvetica,sans-serif; font-size:small; color:Black; background-color:#DEDFDE;">
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Active") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Lower") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Current") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Upper") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr style="font-family: Arial,Helvetica,sans-serif; font-size:small; color:Black; background-color:WhiteSmoke;">
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Active") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Lower") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Current") %>
                                        </td>
                                        <td class="formtext" style="text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Upper") %>
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                                <FooterTemplate></table></FooterTemplate>
                            </asp:Repeater>
                        </fieldset>
                        <br />
                        <fieldset style="width: 90%; text-align: left;" visible="false" runat="server" id="fldStatus">
                            <legend>
                                <asp:Label runat="server" ID="lblStatus" CssClass="formtext" Font-Bold="True" meta:resourcekey="lblStatusResource1"/>
                            </legend>
                            <asp:Repeater runat="server" ID="rptStatusSensors">
                                <HeaderTemplate>
                                    <table style="border: solid 1px black; margin: 5px 0px 10px 20px">
                                        <tr class="formtext" style="font-weight:bold; background-color: gray; color: White;">
                                            <th style="width:150px;">Sensor</th>
                                            <th style="width:100px;">Active</th>
                                            <th style="width:100px;">Status</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr style="font-family: Arial,Helvetica,sans-serif; font-size:small; color:Black; background-color:#DEDFDE;">
                                        <td class="formtext" style="width:150px; text-align:left;">
                                            <%# DataBinder.Eval(Container.DataItem, "Header") %>
                                        </td>
                                        <td class="formtext" style="width:100px; text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Active") %>
                                        </td>
                                        <td class="formtext" style="width:100px; text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Value") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr style="font-family: Arial,Helvetica,sans-serif; font-size:small; color:Black; background-color:WhiteSmoke;">
                                        <td class="formtext" style="width:150px; text-align:left;">
                                            <%# DataBinder.Eval(Container.DataItem, "Header") %>
                                        </td>
                                        <td class="formtext" style="width:100px; text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Active") %>
                                        </td>
                                        <td class="formtext" style="width:100px; text-align:center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Value") %>
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                                <FooterTemplate></table></FooterTemplate>
                            </asp:Repeater>
                        </fieldset>
                        <br />
                        <fieldset style="width: 90%; text-align: left;" visible="false" runat="server" id="fldCommonSensors">
                            <legend>
                                <asp:Label runat="server" ID="lblReeferCaption" CssClass="formtext" Font-Bold="True" meta:resourcekey="lblReeferCaptionResource1"/>
                            </legend>
                            <asp:Repeater runat="server" ID="rptCommonSensors">
                                <HeaderTemplate>
                                    <table style="border: solid 1px black; margin: 5px 0px 10px 20px;">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr style="font-family: Arial,Helvetica,sans-serif; font-size:small; color:Black; background-color:#DEDFDE;">
                                        <td class="formtext" style="width:150px; text-align: left;">
                                            <%# DataBinder.Eval(Container.DataItem, "Header") %>
                                        </td>
                                        <td class="formtext" style="width:100px; text-align: center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Value") %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <tr style="font-family: Arial,Helvetica,sans-serif; font-size:small; color:Black; background-color:WhiteSmoke;">
                                        <td class="formtext" style="width:150px; text-align: left;">
                                            <%# DataBinder.Eval(Container.DataItem, "Header") %>
                                        </td>
                                        <td class="formtext" style="width:100px; text-align: center;">
                                            <%# DataBinder.Eval(Container.DataItem, "Value") %>
                                        </td>
                                    </tr>
                                </AlternatingItemTemplate>
                                <FooterTemplate></table></FooterTemplate>
                            </asp:Repeater>
                        </fieldset>                
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 20px"></td>
            </tr>
            <tr>
                <td align="center" style="width: 100%">
                    <asp:DataGrid ID="dgSensors" runat="server" BorderWidth="2px" BorderStyle="Ridge"
                        BorderColor="White" Width="292px" AutoGenerateColumns="False" Height="103px" meta:resourcekey="dgSensorsResource1">
                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"/>
                        <AlternatingItemStyle BackColor="WhiteSmoke"/>
                        <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black" BackColor="#DEDFDE"/>
                        <HeaderStyle CssClass="DataHeaderStyle"/>
                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"/>
                        <Columns>
                            <asp:BoundColumn DataField="SensorName" HeaderText="<%$ Resources:dgSensors_Sensor %>"/>
                            <asp:BoundColumn DataField="SensorAction" HeaderText="<%$ Resources:dgSensors_Action %>"/>
                        </Columns>
                        <PagerStyle Mode="NumericPages"/>
                    </asp:DataGrid>
                </td>
            </tr>
            <tr>
                <td style="height: 20px"></td>
            </tr>
            <tr>
                <td height="10" align="center">
                    <table id="tblBoxSetupInfo" style="border-top-width: thin; border-left-width: thin;
                        border-left-color: black; border-bottom-width: thin; border-bottom-color: black;
                        border-top-color: black; height: 216px; border-right-width: thin;
                        border-right-color: black" cellspacing="0" cellpadding="0" width="95%" border="0" runat="server">
                        <tr>
                            <td>
                                <div id="DivBoxSetupInfo" title="Box Setup:" style="border-right: black thin; border-top: black thin;
                                    overflow: auto; border-left: black thin; width: 100%; border-bottom: black thin;
                                    height: 297px" align="center">
                                    <table id="Table2" style="width: 292px; height: 85px" cellspacing="0" cellpadding="0" border="0" runat="server">
                                        <tr>
                                            <td class="tableheading" style="height: 3px" colspan="2">
                                                <asp:Label ID="lblBoxSetupInfoTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblBoxSetupInfoTitleResource1" Text="Box Setup Information:"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext">
                                                <asp:Label ID="lblGPSFreqTitle" runat="server" CssClass="formtext" meta:resourcekey="lblGPSFreqTitleResource1" Text="GPS Frequency:"/>
                                            </td>
                                            <td style="height: 19px">
                                                <asp:Label ID="lblBoxSetupGPSFreg" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupGPSFregResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext">
                                                <asp:Label ID="lblSpeedThresTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSpeedThresTitleResource1" Text="Speed Threshold:"/>
                                            </td>
                                            <td style="height: 3px">
                                                <asp:Label ID="lblBoxSetupSpeed" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupSpeedResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="height: 7px">
                                                <asp:Label ID="lblGeoFenceTitle" runat="server" CssClass="formtext" meta:resourcekey="lblGeoFenceTitleResource1" Text="GeoFence:"/>
                                            </td>
                                            <td style="height: 7px">
                                                <asp:Label ID="lblBoxSetupGeo" runat="server" CssClass="formtext" meta:resourcekey="lblBoxSetupGeoResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="width: 208px" align="left"></td>
                                            <td align="right"></td>
                                        </tr>
                                    </table>
                                    <asp:DataGrid ID="dgBoxSetupInfo" runat="server" AutoGenerateColumns="False" PageSize="4" DataKeyField="SensorId" meta:resourcekey="dgBoxSetupInfoResource1">
                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"/>
                                        <AlternatingItemStyle BackColor="WhiteSmoke"/>
                                        <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black" BackColor="#DEDFDE"/>
                                        <HeaderStyle CssClass="DataHeaderStyle"/>
                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"/>
                                        <Columns>
                                            <asp:BoundColumn Visible="False" DataField="SensorId" HeaderText='<%$ Resources:dgBoxSetupInfo_SensorId %>'/>
                                            <asp:TemplateColumn Visible="False" HeaderText='<%$ Resources:dgBoxSetupInfo_Set %>'>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkCheckBoxAction" Enabled="False" Checked='<%# DataBinder.Eval(Container.DataItem, "SensorStatus") %>' runat="server" meta:resourcekey="chkCheckBoxActionResource1"/>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgBoxSetupInfo_Sensors %>'/>
                                            <asp:BoundColumn DataField="SensorAction" HeaderText='<%$ Resources:dgBoxSetupInfo_SensorStatus %>'/>
                                        </Columns>
                                        <PagerStyle Mode="NumericPages"/>
                                    </asp:DataGrid>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <table id="tblBoxStatusInfo" cellspacing="0" cellpadding="0" border="0" runat="server" width="95%">
                        <tr>
                            <td>
                                <div id="divBoxStatusInfo" title="Box Setup:" style="border-right: black thin;
                                    border-top: black thin; overflow: auto; border-left: black thin; 
                                    border-bottom: black thin; height: 430px" align="center">
                                    <table id="Table3" style=" height: 128px; width: 462px;" cellspacing="0" cellpadding="0" border="0" runat="server">
                                        <tr>
                                            <td class="tableheading" style="height: 3px" colspan="2">
                                                <asp:Label ID="lblBoxStatusInfoTitle" runat="server" CssClass="tableheading" meta:resourcekey="lblBoxStatusInfoTitleResource1" Text="Box Status Information:"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="width: 160px">
                                                <asp:Label ID="lblArmedTitle2" runat="server" CssClass="formtext" meta:resourcekey="lblArmedTitle2Resource1" Text="Armed:"/>
                                            </td>
                                            <td style="height: 3px">
                                                <asp:Label ID="lblBoxStatusArmed" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusArmedResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="width: 160px">
                                                <asp:Label ID="lblMainBatteryTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMainBatteryTitleResource1" Text="Main Battery:"/>
                                            </td>
                                            <td style="height: 3px">
                                                <asp:Label ID="lblBoxStatusMainBattery" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusMainBatteryResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="width: 160px">
                                                <asp:Label ID="lblBoxStatusBackupBatteryLabel" runat="server" meta:resourcekey="lblBoxStatusBackupBatteryLabelResource1" Text="Backup Battery:"/>
                                            </td>
                                            <td style="height: 3px">
                                                <asp:Label ID="lblBoxStatusBackupBattery" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusBackupBatteryResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="width: 160px">
                                                <asp:Label ID="lblSNTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSNTitleResource1" Text="S.N:"/>
                                            </td>
                                            <td style="height: 4px">
                                                <asp:Label ID="lblBoxStatusSN" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusSNResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="width: 160px">
                                                <asp:Label ID="lblFirmwareTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFirmwareTitleResource1" Text="Firmware version:"/>
                                            </td>
                                            <td style="height: 20px">
                                                <asp:Label ID="lblBoxStatusFirmware" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusFirmwareResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="height: 1px; width: 160px;">
                                                <asp:Label ID="lblMemoryUsageTitle" runat="server" CssClass="formtext" meta:resourcekey="lblMemoryUsageTitleResource1" Text="Memory Usage:"/>
                                            </td>
                                            <td style="height: 1px">
                                                <asp:Label ID="lblBoxStatusWaypoint" runat="server" CssClass="formtext" meta:resourcekey="lblBoxStatusWaypointResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="width: 160px; height: 1px">
                                                <asp:Label ID="lblMDTMessages" runat="server" Text="MDT messages in memory:" Width="164px"/>
                                            </td>
                                            <td style="height: 1px">
                                                <asp:Label ID="lblMDTMessagesValue" runat="server" CssClass="formtext"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="height: 1px; width: 160px;">
                                                <asp:Label ID="lblSIMESNTitle" runat="server" CssClass="formtext" meta:resourcekey="lblSIMESNTitleResource1" Text="SIM/ESN:"/>
                                            </td>
                                            <td style="height: 1px">
                                               &nbsp;<asp:TextBox ID="lblStatusSIM" runat="server" Width=300px  TextMode="MultiLine" meta:resourcekey="lblStatusSIMResource2"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="height: 1px; width: 160px;">
                                                <asp:Label ID="lblCellTitle" runat="server" CssClass="formtext" meta:resourcekey="lblCellTitleResource1" Text="Cell:"/>
                                            </td>
                                            <td style="height: 1px">
                                                <asp:Label ID="lblStatusCell" runat="server" CssClass="formtext" meta:resourcekey="lblStatusCellResource1"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="height: 1px; width: 160px;">
                                                <asp:Label ID="lblPRLLabel" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblPRLLabelResource1" Text="PRL:"/>
                                            </td>
                                            <td style="height: 1px">
                                                <asp:Label ID="lblPRL" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblPRLResource1"/>
                                            </td>                                        
                                        </tr>
                                        <tr>
                                            <td class="formtext" style="width: 160px" align="left"></td>
                                            <td align="right"></td>
                                        </tr>
                                    </table>
                                    <asp:DataGrid ID="dgBoxStatusInfo" runat="server"  DataKeyField="SensorId" PageSize="4" AutoGenerateColumns="False" meta:resourcekey="dgBoxStatusInfoResource1">
                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"/>
                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"/>
                                        <AlternatingItemStyle ForeColor="Black" BackColor="WhiteSmoke"/>
                                        <ItemStyle Font-Size="11px" Font-Names="Arial,Helvetica,sans-serif" ForeColor="Black" BackColor="#DEDFDE"/>
                                        <HeaderStyle CssClass="DataHeaderStyle"/>
                                        <Columns>
                                            <asp:BoundColumn Visible="False" DataField="SensorId" HeaderText='<%$ Resources:dgBoxStatusInfo_SensorId %>'/>
                                            <asp:TemplateColumn Visible="False" HeaderText='<%$ Resources:dgBoxStatusInfo_Set %>'>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkCheckBoxAction" Enabled="False" Checked='<%# DataBinder.Eval(Container.DataItem, "SensorStatus") %>' runat="server" meta:resourcekey="chkCheckBoxActionResource2"/>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:BoundColumn DataField="SensorName" HeaderText='<%$ Resources:dgBoxStatusInfo_Sensors %>'/>
                                            <asp:BoundColumn DataField="SensorAction" HeaderText='<%$ Resources:dgBoxStatusInfo_SensorStatus %>'/>
                                        </Columns>
                                        <PagerStyle Mode="NumericPages"/>
                                    </asp:DataGrid>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <fieldset>
                        <legend>
                            <asp:Label ID="lblMaintenanceServicesLegend" runat="server" CssClass="formtext"  Text="Additional Information" meta:resourcekey="lblMaintenanceServicesLegendResource1"/>
                        </legend> 
                        <table id="tblControllerStatus" style="width: 311px; height: 13px" cellspacing="0" cellpadding="0" width="311" border="0" runat="server">
                            <tr>
                                <td class="formtext" style="width: 152px" colspan="1">
                                    &nbsp;
                                    <asp:Label ID="lblControllerVersionTitle" runat="server" CssClass="formtext" meta:resourcekey="lblControllerVersionTitleResource1" Text="Controller Version:"/>
                                </td>
                                <td>
                                    <asp:Label ID="lblControllerVersion" runat="server" CssClass="formtext" meta:resourcekey="lblControllerVersionResource1"/>
                                </td>
                            </tr>
                        </table>
                        <table id="tblBadSensor" style="width: 219px; height: 13px" cellspacing="0" cellpadding="0" border="0" runat="server">
                            <tr>
                                <td class="formtext" style="width: 152px" colspan="1" align="right">
                                    &nbsp;
                                    <asp:Label ID="lblBadSensor" runat="server" CssClass="formtext" Text="Bad Sensor:"/>
                                </td>
                                <td style="width: 118px">
                                    <asp:Label ID="lblBadSensorValue" runat="server" CssClass="formtext"/>
                                </td>
                            </tr>
                        </table>
                        <table id="tblTAR" style="width: 295px;" cellspacing="0" cellpadding="0" width="295" border="0" runat="server">
                            <tr>
                                <td class="formtext" style="width: 110px" colspan="1" valign="top">
                                    <asp:Label ID="lblTARModePeriodTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTARModePeriodTitleResource1" Text="TAR Mode Period:"/>
                                </td>
                                <td valign="top">
                                    <table>
                                        <tr>
                                            <td style="width: 100px">
                                                <asp:DropDownList ID="cboTARPeriod" runat="server" CssClass="RegularText" Width="99px" meta:resourcekey="cboTARPeriodResource1">
                                                    <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource1" Text="Off"></asp:ListItem>
                                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="1 Hour"/>
                                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource3" Text="2 Hours"/>
                                                    <asp:ListItem Value="4" meta:resourcekey="ListItemResource4" Text="4 Hours"/>
                                                    <asp:ListItem Value="6" meta:resourcekey="ListItemResource5" Text="6 Hours"/>
                                                    <asp:ListItem Value="12" meta:resourcekey="ListItemResource6" Text="12 Hours"/>
                                                    <asp:ListItem Value="24" meta:resourcekey="ListItemResource7" Text="1 Day"/>
                                                    <asp:ListItem Value="48" meta:resourcekey="ListItemResource8" Text="2 Days"/>
                                                    <asp:ListItem Value="72" meta:resourcekey="ListItemResource9" Text="3 Days"/>
                                                    <asp:ListItem Value="96" meta:resourcekey="ListItemResource10" Text="4 Days"/>
                                                    <asp:ListItem Value="120" meta:resourcekey="ListItemResource11" Text="5 Days"/>
                                                    <asp:ListItem Value="144" meta:resourcekey="ListItemResource12" Text="6 Days"/>
                                                    <asp:ListItem Value="168" meta:resourcekey="ListItemResource13" Text="7 Days"/>
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 100px">
                                                <asp:Label ID="lblTarMode" runat="server" CssClass="formtext" Width="56px" meta:resourcekey="lblTarModeResource1"/>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <table id="tblVCRDelay" style="width: 291px; height: 19px" cellspacing="0" cellpadding="0" width="291" border="0" runat="server">
                            <tr>
                                <td class="formtext" style="width: 152px" colspan="1">
                                    <asp:Label ID="lblDelayPeriodTitle" runat="server" CssClass="formtext" meta:resourcekey="lblDelayPeriodTitleResource1" Text="Delay Period:"/>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cboVCRDelayPeriod" runat="server" CssClass="RegularText" Width="99px" meta:resourcekey="cboVCRDelayPeriodResource1">
                                        <asp:ListItem Value="-1" Selected="True" meta:resourcekey="ListItemResource14" Text="Disabled"/>
                                        <asp:ListItem Value="0" meta:resourcekey="ListItemResource15" Text="0 min"/>
                                        <asp:ListItem Value="60" meta:resourcekey="ListItemResource16" Text="1 min"/>
                                        <asp:ListItem Value="120" meta:resourcekey="ListItemResource17" Text="2 min"/>
                                        <asp:ListItem Value="180" meta:resourcekey="ListItemResource18" Text="3 min"/>
                                        <asp:ListItem Value="240" meta:resourcekey="ListItemResource19" Text="4 min"/>
                                        <asp:ListItem Value="300" meta:resourcekey="ListItemResource20" Text="5 min"/>
                                        <asp:ListItem Value="600" meta:resourcekey="ListItemResource21" Text="10 min"/>
                                        <asp:ListItem Value="900" meta:resourcekey="ListItemResource22" Text="15 min"/>
                                        <asp:ListItem Value="1200" meta:resourcekey="ListItemResource23" Text="20 min"/>
                                        <asp:ListItem Value="1800" meta:resourcekey="ListItemResource24" Text="30 min"/>
                                        <asp:ListItem Value="3600" meta:resourcekey="ListItemResource25" Text="1 hour"/>
                                        <asp:ListItem Value="7200" meta:resourcekey="ListItemResource26" Text="2 hours"/>
                                        <asp:ListItem Value="10800" meta:resourcekey="ListItemResource27" Text="3 hours"/>
                                        <asp:ListItem Value="14400" meta:resourcekey="ListItemResource28" Text="4 hours"/>
                                        <asp:ListItem Value="18000" meta:resourcekey="ListItemResource29" Text="5 hours"/>
                                        <asp:ListItem Value="21600" meta:resourcekey="ListItemResource30" Text="6 hours"/>
                                        <asp:ListItem Value="25200" meta:resourcekey="ListItemResource31" Text="7 hours"/>
                                        <asp:ListItem Value="28800" meta:resourcekey="ListItemResource32" Text="8 hours"/>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                        <table id="tblDTC" style="width: 391px; height: 19px" cellspacing="0" cellpadding="0" width="291" border="0" runat="server">
                            <tr>
                                <td class="formtext" style="width: 152px" colspan="1">
                                    <asp:Label ID="lblDTCCount" runat="server" meta:resourcekey="lblDTCCountResource1"  Text="DTC in Vehicle:"/>
                                </td>
                                <td>
                                    <asp:Label ID="lblDTCCountinVehicleValue" runat="server" CssClass="formtext" meta:resourcekey="lblDTCCountinVehicleValueResource1"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="formtext" colspan="1" style="width: 152px; height: 16px">
                                    <asp:Label ID="lblDTCinMsg" runat="server" meta:resourcekey="lblDTCinMsgResource1" Text="DTC in Msg:"/>
                                </td>
                                <td style="height: 16px">
                                    <asp:Label ID="lblDTCinMsgValue" runat="server" CssClass="formtext" meta:resourcekey="lblDTCinMsgValueResource1"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="formtext" colspan="1" style="width: 152px; height: 16px">
                                    <asp:Label ID="lblDTC" runat="server" CssClass="formtext" meta:resourcekey="lblDTCResource1" Text="DTC:"/>
                                </td>
                                <td style="height: 16px">
                                    <asp:TextBox ID="lblDTCValue" runat="server" TextMode="MultiLine" Width="100%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formtext" colspan="1" style="width: 152px; height: 16px">
                                    <asp:Label ID="lblDTCSource" runat="server" CssClass="formtext" meta:resourcekey="lblDTCSourceResource1" Text="DTC Source:"/>
                                </td>
                                <td style="height: 16px">
                                    <asp:Label ID="lblDTCSourceValue" runat="server" CssClass="formtext" meta:resourcekey="lblDTCSourceValueResource1"/>
                                </td>
                            </tr>
                        </table>
                        <table id="tblTetheredState" style="width: 291px; height: 19px" cellspacing="0" cellpadding="0" width="291" border="0" runat="server">
                            <tr>
                                <td class="formtext" style="width: 152px" colspan="1">
                                    <asp:Label ID="lblPerTypeCaption" runat="server" Text="Type:"/>
                                </td>
                                <td>
                                    <asp:Label ID="lblPerType" runat="server" CssClass="formtext"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="formtext" colspan="1" style="width: 152px; height: 16px">
                                    <asp:Label ID="lblMdtTypeCaption" runat="server" Text="MDT Type:"/>
                                </td>
                                <td style="height: 16px">
                                    <asp:Label ID="lblMdtType" runat="server" CssClass="formtext"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="formtext" colspan="1" style="width: 152px; height: 16px">
                                    <asp:Label ID="lblMDTverCaption" runat="server" Text="Version:"/>
                                </td>
                                <td style="height: 16px">
                                    <asp:Label ID="lblMDTver" runat="server" CssClass="formtext"/>
                                </td>
                            </tr>
                        </table>
                    </fieldset> 
                </td>
            </tr>
            <tr>
                <td align="right" height="10" style="width: 329px"></td>
            </tr>
            <tr>
                <td align="right" style="width: 329px">
                    <asp:Button runat="server" CssClass="combutton" ID="cmdClose" OnClientClick="window.close()" Text="Close" meta:resourcekey="cmdCloseResource1"/>
                </td>
            </tr>
            <tr>
                <td height="10" style="width: 329px"></td>
            </tr>
        </table>
    </form>
</body>
</html>
