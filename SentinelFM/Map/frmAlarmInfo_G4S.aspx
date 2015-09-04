<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmAlarmInfo_G4S.aspx.cs" Inherits="SentinelFM.frmAlarmInfo_G4S"
Culture="en-US" UICulture="auto" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
    <title>Alarm details</title>
        <style type="text/css">
            .accept {
                background: url('../ExtJS/resources/icons/accept.png') no-repeat 0 0 !important;
            }
            .map {
                background: url('../ExtJS/resources/icons/map.png') no-repeat 0 0 !important;
            }
            .cancel {
                background: url('../ExtJS/resources/icons/cancel.png') no-repeat 0 0 !important;
            }
        </style>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <script language="javascript">	<!--
        
        function onblur(event) {
            window.focus();
        }

        function removeURLParameter(url, parameter) {
            //prefer to use l.search if you have a location/link object
            var urlparts= url.split('?');   
            if (urlparts.length>=2) {

                var prefix= encodeURIComponent(parameter)+'=';
                var pars= urlparts[1].split(/[&;]/g);

                //reverse iteration as may be destructive
                for (var i= pars.length; i-- > 0;) {    
                    //idiom for string.startsWith
                    if (pars[i].lastIndexOf(prefix, 0) !== -1) {  
                        pars.splice(i, 1);
                    }
                }

                url= urlparts[0]+'?'+pars.join('&');
                return url;
            } else {
                return url;
            }
        }

        $( document ).ready(function() {
            document.getElementById("form1").action = removeURLParameter(document.URL,"al");
        });
	//-->
    </script>
</head>
<body> 
    <script type="text/javascript">
        var AlarmLandmarkID = "<%=AlarmLandmarkID %>";
        var AlarmDescriptionText = "<%=AlarmDescriptionText %>";
        var SourcePage = "<%=SourcePage %>";
        var AlarmBoxId = <%=AlarmBoxId == "" ? "''" : AlarmBoxId %>;
        var AlarmVehicleDescription = "<%=AlarmVehicleDescription %>";
        var AlarmLon = <%=AlarmLon == "" ? "''" : AlarmLon %>;
        var AlarmLat = <%=AlarmLat == "" ? "''" : AlarmLat %>;

        function MapIt() {
            if (SourcePage == "newmap") {

                var ShowAlarmLandmarkID = 0;
                if (AlarmLandmarkID != "" && AlarmDescriptionText.indexOf("DIA-") == 0) {
                    ShowAlarmLandmarkID = AlarmLandmarkID;
                }

                opener.showAlarm(AlarmVehicleDescription, AlarmLon, AlarmLat, AlarmLandmarkID);

                
                window.close();
                return false;
            }
            else {
                return true;
            }
        }
    </script>   
   
    <form id="form1" runat="server" method="post">
        
            <fieldset>
                
            <legend class="formtext" runat="server" id="alarmlegentTitle">Alarm details</legend>  
                    <table class="formtext" border=0 cellpadding=5 cellspacing=5    >
                        <tr runat="server" id="trAlarmID">
                            <td><asp:Label  ID="AlarmIDLabel" runat="server" Text="Alarm Number:" ReadOnly="true" /></td>
                            <td><asp:Label  ID="AlarmID" runat="server"  ReadOnly="true" /></td>
                        </tr>
                        <tr runat="server" id="trAlarmDescription">
                            <td><asp:Label ID="AlarmDescriptionLabel" runat="server" Text="Alarm Description:"  ReadOnly="true" AnchorHorizontal="100%" /></td>
                            <td> <asp:Label ID="AlarmDescription" runat="server"   ReadOnly="true" AnchorHorizontal="100%" /></td>
                        </tr>
                        <tr runat="server" id="trTimeCreated">
                            <td> <asp:Label ID="TimeCreatedLabel" runat="server" Text="Time Created:" ReadOnly="true" />  </td>
                            <td> <asp:Label ID="TimeCreated" runat="server"  ReadOnly="true" />  </td>
                        </tr>
                        <tr runat="server" id="trAlarmState">
                            <td>  <asp:Label ID="AlarmStateLabel" runat="server" Text="Alarm State:" ReadOnly="true"  />            </td>
                            <td>  <asp:Label ID="AlarmState" runat="server"  ReadOnly="true"  />            </td>
                        </tr>
                        <tr runat="server" id="trAlarmServerity">
                            <td> <asp:Label ID="AlarmSeverityLabel" runat="server" Text="Alarm Severity:" ReadOnly="true"  />   </td>
                            <td> <asp:Label ID="AlarmSeverity" runat="server"  ReadOnly="true"  />   </td>
                        </tr>
                        <tr runat="server" id="trVehicleId">
                            <td>  <asp:Label ID="VehicleIdLabel" runat="server" Text="Vehicle Id:" ReadOnly="true" Visible />      </td>
                            <td>  <asp:Label ID="VehicleId" runat="server"  ReadOnly="true" Visible />      </td>
                        </tr>
                        <tr runat="server" id="trVehicleDescription">
                            <td> <asp:Label ID="VehicleDescriptionLabel" runat="server"  Text="Vehicle Info:" ReadOnly="true" AnchorHorizontal="100%"/>    </td>
                            <td> <asp:Label ID="VehicleDescription" runat="server"  ReadOnly="true" AnchorHorizontal="100%"/>    </td>
                        </tr>
                        <tr runat="server" id="trStreetAddress">
                            <td>  <asp:Label ID="StreetAddressLabel" runat="server" Text="Street Address:" ReadOnly="true" AnchorHorizontal="100%"/>         </td>
                            <td> <asp:Label ID="StreetAddress" runat="server"  ReadOnly="true" AnchorHorizontal="100%"/>         </td>
                        </tr>                            
                        <tr>
                            <td>  <asp:Label ID="ExtraNotesLabel" runat="server" Text="Notes:" ReadOnly="true" AnchorHorizontal="100%"/>         </td>
                            <td>  
                                <asp:TextBox ID="ExtraNotes" runat="server"  AnchorHorizontal="100%" Rows="3"
                                        Height="51px" Width="292px" TextMode="MultiLine" />
                            </td>
                        </tr>
                            
                    </table>
                        
                </fieldset> 
                         
                      
                          
                      <p></p>
                         
                                                                                      
                       
			            <asp:Label ID="Latitude" runat="server" Text="Latitude" AnchorHorizontal="100%" ReadOnly="true" Visible="false"/>            
                        <asp:Label ID="Longitude" runat="server" Text="Longitude" AnchorHorizontal="100%" ReadOnly="true"  Visible="false"/> 
                        <asp:Label ID="Speed" runat="server" Text="Speed" AnchorHorizontal="100%" ReadOnly="true"  Visible="false"/>            
                        <asp:Label ID="Heading" runat="server" Text="Heading" AnchorHorizontal="100%" ReadOnly="true"  Visible="false"/>            

                 <asp:Button ID="cmdAccept" runat="server" Text="Accept Alarm" Width="154px" OnClick="cmdAccept_Click" CssClass="accept" />
                 <asp:Button ID="cmdMapIt" runat="server" Width="154px" Text="Map It" OnClick="cmdMapIt_Click" CssClass="map"/>
                 <asp:Button runat="server" ID="cmdClose" Width="154px" OnClientClick="window.close()" Text="Exit" CssClass="cancel"/>
    </form>
    
</body>
</html>
