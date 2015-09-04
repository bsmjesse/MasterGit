<%@ Page Language="C#" AutoEventWireup="true" CodeFile="dispute.aspx.cs" Inherits="dispute_speed" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SentinelFM dispute system</title>    
    <script type="text/javascript" src="../ServiceAssignment/Scripts/ui/jquery-1.9.1.min.js"></script>
    <%
        if (IsMobile)
        {
    %>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="http://code.jquery.com/mobile/1.4.0/jquery.mobile-1.4.0.min.css" />    
    <script src="http://code.jquery.com/mobile/1.4.0/jquery.mobile-1.4.0.min.js"></script>
   
    <style type="text/css">
        html { background-color: #333; }
        @media only screen and (min-width: 600px){
            .ui-page {
                float: left;
                height: 400px;
                width: 600px !important;
                margin: 0 auto !important;
                position: relative !important;
            }
        }
    </style>
   <script type="text/javascript">       
       function ChangeCaptcha(val) {
           $('#changeCaptcha').val(1);
           $.ajax({
               type: "POST",
               contentType: "application/json; charset=utf-8",
               url: "dispute.aspx/GenerateCaptcha",
               dataType: "json",
               error: function (xhr, status, error) {
                   alert(xhr.responseText);

               },
               success: function (json) {
                   if (val == 1) {
                       $('#imgCaptcha').attr('src', json.d);
                   } else {
                       $('#imgCaptchaM').attr('src', json.d);
                   }
               }
           });
       }

       function SubmitForm() {
           if (ValidateForm()) {
               $.ajax({
                   type: "POST",
                   contentType: "application/json; charset=utf-8",
                   url: "dispute.aspx/SubmitTicket",
                   data: "{'infractionid':'" + violationId + "', 'q':'" + vLatlon + "', 'actualspeed':'" + $('#actualspeed').val() + "', 'notes':'" + $('#notes').val() + "', 'txtCaptcha':'" + $('#txtCaptchaM').val() + "', 'name':'" + $('#name').val() + "', 'metric':'" + $('#metric').val() + "', 'errorspeed':'" + $('#errorspeed').val() + "', 'yourspeed':'" + yourspeed + "', 'objects':'" + myobjects + "', 'objectid':'" + objectId + "'}",
                   dataType: "json",
                   error: function (xhr, status, error) {
                       alert(xhr.responseText);

                   },
                   success: function (json) {
                       var data = $.parseJSON(json.d);
                       if (data.status == "success") {
                           //alert("You have saved the dispute.");
                           //marker.closePopup();
                           document.location.href = 'dispute.aspx?submitted=true';

                       }
                       else if (data.status == "reload") {
                           window.location.reload();
                       } else {
                           alert(data.reason);
                       }

                   }
               });
           }
       }

       function IsValidateEmail(email) {
           var filter = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
           return filter.test(email);
       }

       function ValidateForm() {
           if ($('#name').val() == "" || $('#name').val() == null) {
               alert("Please input name");
               return false;
           }

           if ($('#email').val() != "" && $('#email').val() != null) {
               if (!IsValidateEmail($('#email').val())) {
                   alert("Email format is not correct");
                   return false;
               }
           }

           if ($('#actualspeed').val() == "" || $('#actualspeed').val() == null) {
               alert("Please input actual speed");
               return false;
           }

           if (isNaN($('#actualspeed').val())) {
               alert("Please input only numberic in speed field");
               return false;
           }

           if ($('#notes').val().length > 100) {
               alert("The notes length cannot be larger than 100 characters");
               return false;
           }
           if ($('#imgCaptchaM').html() != undefined) {
               if ($('#txtCaptchaM').val() == "" || $('#txtCaptcha').val() == null) {
                   alert("Please input captcha");
                   return false;
               }
           }
           
           return true;
       }

   </script>  
    <%
        }
        else
        {
   %>
     <link rel="stylesheet" href="js/leaflet-0.7.1/leaflet.css" />
    <link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />
    <!--[if lte IE 8]>
     <link rel="stylesheet" href="http://cdn.leafletjs.com/leaflet-0.6.4/leaflet.ie.css" />
    <![endif]-->
    
    <script src="js/leaflet-0.7.1/leaflet.js"></script>  
    <script src="http://maps.google.com/maps/api/js?v=3.2&sensor=false"></script>
    <script src="http://matchingnotes.com/javascripts/leaflet-google.js"></script>
    <script type="text/javascript" src="js/dispute.js"></script>
    <style type="text/css">
        #map {
            height: 1000px;                        
        }
       
        .mnuMenu_5 {
            color: #FFFF00;
        }
        .mnuMenu_3 {
            color: #FFFFFF;
            font-family: Verdana;
            font-size: 12px;
            font-weight: 600;
        }
        .mnuMenu_1 {
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
        }
        .leaflet-map-pane {
            z-index: 2 !important;
        }

        .leaflet-google-layer {
            z-index: 1 !important;
        }
        
    </style>
    
    <%
        }
    %>
   
    <script type="text/javascript">
        var errorSpeed = "<%=ErrorSpeed%>";
        var violationId = "<%=ViolationId%>";
        var vLatlon = "<%= VlatLon %>";
        var myobjects = "<%=Objects%>";
        var objectId = "<%=ObjectId%>";
        var yourspeed = "<%=YourSpeed%>";
        var address = "<%=Address%>";
    </script>
</head>
<body>
   
    <%
        if (IsMobile)
        {
    %>
            <div data-role="page" class="type-interior">
            <div data-role="content" data-theme="c">
		    <div class="content-primary">
    <%                
        }
    %>

    <form id="disputeForm" method="POST" runat="server" data-ajax="false">
    <%
        if (IsMobile)
        {
            if (!Submitted)
            {
    %>                
        <input type="hidden" id="q" name="q" value="<%= VlatLon %>" />        
        <input type="hidden" id="Hidden1" name="objects" value="<%= Objects %>" /> 
        <input type="hidden" id="Hidden2" name="objectid" value="<%= ObjectId %>" /> 
        <input type="hidden" id="changeCaptcha" name="changeCaptcha" value="" />
        <div data-role="fieldcontain">
				<label style="width: 100%;">Speed Dispute Ticket: <span style="color:red;" data-role="none"><strong><%= ViolationId %></strong></span></label>
                
	</div>
        <div data-role="fieldcontain">
				<label for="infractionid">Address:</label>
				<input style="width: 100%;" data-role="none" type="text" name="infractionid" id="infractionid" value="<%= Address %>" readonly />
	</div>
        <div data-role="fieldcontain">
				<label for="yourspeed">Actual Vehicle Speed:</label>
				<input style="width: 70%;" data-role="none" type="text" name="yourspeed" id="yourspeed"  value="<%= YourSpeed %>" readonly />
	</div>
        <div data-role="fieldcontain">
				<label for="errorspeed">Posted Speed Limit in Notification Email:</label>
				<input style="width: 70%;" data-role="none" type="text" name="errorspeed" id="errorspeed" value="<%= ErrorSpeed %>" readonly />
	</div>
        <div data-role="fieldcontain">
				<fieldset data-role="controlgroup" class="ui-grid-a">
			    	<legend>Actual Speed Limit Posted on Road:</legend>
                <div class="ui-block-a">
				    <input style="width: 100px;" data-role="none" type="text" name="actualspeed" id="actualspeed" placeholder="Required" value="" />
                </div>
                <div class="ui-block-b">
                    <select data-role="none" id="Select1" name="metric">
                    <%
                        if (ErrorSpeed != null)
                        {
                            if (ErrorSpeed.Contains("mph"))
                            {
                    %>
                                <option value="2" selected>mph</option>
                    <%
                            }
                            else
                            {
                    %>
                                <option value="1" selected>km/hr</option>
                    <%
                            }
                        }
                        else
                        {
                    %>
                            <option value="1">km/hr</option><option value="2">mph</option>
                    <%        
                        }
                    %>
                    </select>
                </div>
            </fieldset>
	</div>
        <div data-role="fieldcontain">
				<label for="name">Name:</label>
				<input type="text" style="width: 70%;" data-role="none" name="name" id="name" placeholder="Required" value="" />
            
	</div>
        <div data-role="fieldcontain">
				<label for="email">Email:</label>
				<input type="text" style="width: 70%;" data-role="none" name="email" id="email" placeholder="Required" value="" />
            
	</div>
        <div data-role="fieldcontain">
				<label for="notes">Notes:</label>
				<textarea name="notes" id="notes" style="width: 70%;" data-role="none" placeholder="Optional" value="" ></textarea>(Content cannot be over 100 characters)
            
	</div>
  <%
      if (UseCaptcha)
      {


  %>      
        <div data-role="fieldcontain" data-ajax="false">
            <fieldset data-role="controlgroup" class="ui-grid-a">         
            <asp:Panel ID="CaptchaPanelM" runat="Server">
             <div class="ui-block-a">
            <asp:Image ID="imgCaptchaM" runat="server" Height="50px" Width="211px" />           
             </div>
            <div class="ui-block-b">
            <asp:TextBox ID="txtCaptchaM" name="txtCaptchaM" runat="server" Width="150px"></asp:TextBox>
            </div>
            <div class="ui-block-c">            
            <input type="button" value="Captcha" onclick="ChangeCaptcha(2)" />            
            </div>
            </asp:Panel>            
            </fieldset>
           </div> 
 <%
      }            
 %>          
        <button type="button" style="width: 70%;" name="submit" value="submit" data-role="button" data-theme="b" onclick="SubmitForm()">Submit</button>
 <% }
            else
            {
                if (string.IsNullOrEmpty(ErrorReason))
                {


 %>
                    <h4><span style="color: green;">Your dispute has been submitted. Thank you</span></h4>
<%
                }
                else
                {
%>
                    <h4><span style="color: red;">Sorry, you dispute cannot be submitted: [<%=ErrorReason %>]</span></h4> 
<%
                        
                }
            }
        }
        else
        {
 %>
        <div>                        
        <!--content-->
        <div id="map"></div> 
       
       
        </div>
 <%              
        }
        if (UseCaptcha)
        {


 %>   
     <div id="divCaptcha" style="display:none;">            
            <asp:Panel ID="CaptchaPanel" runat="Server">
            <asp:Image ID="imgCaptcha" runat="server" Height="50px" Width="211px" />    
            <br />        
            <asp:TextBox ID="txtCaptcha" runat="server" Width="150px"></asp:TextBox>
                <!--
            <asp:Button ID="btnRegen" runat="server" Text="Captcha" OnClick="btnRegen_Click" CssClass="commands" />
                -->
            <input type="button" value="Captcha" onclick="ChangeCaptcha(1)" />
            </asp:Panel>            
        </div>
        <%
        }
        %>    
    </form>
 <%
        if (IsMobile)
        {
    %>
            </div>
		    </div>
            </div>  
    <%              
        }
    %>
</body>
</html>
