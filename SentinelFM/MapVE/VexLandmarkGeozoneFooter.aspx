<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VexLandmarkGeozoneFooter.aspx.cs" Inherits="SentinelFM.MapVE_VexLandmarkGeozoneFooter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>VE Footer</title>
    <link type="text/css" rel="Stylesheet" href="ve.css" />
       <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script src="scripts/util.js" type="text/javascript"></script>
    <script type="text/javascript" src="http://yui.yahooapis.com/2.6.0/build/yahoo/yahoo-min.js"></script>

    <script type="text/javascript" src="http://yui.yahooapis.com/2.6.0/build/event/event-min.js"></script>

    <script language="javascript" type="text/javascript">
    
        function initializeFooter()
        {
        }
        
        function prepFooter(frame) {
	}
                
               
        function closeDrawing() {
             parent.window.close();
        }   
       
                             
        function showInsetMap() {
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.toggleInsetMapVisibility(); 
        }     
        
        function insetMapVisibilityChangedHandler() {
            var fb = getFrameElement("framebody", parent);
            var value = fb.mapscreen.insetMapEnabled;
            document.getElementById("ShowMiniMap").className = ("buttonselected_" + value);
        }    

        

    </script>

</head>
<body class="footerdiv" onload="initializeFooter();">
    <table cellpadding="2" cellspacing="0" width="100%">
        <tr>
            <td id="footercellleft" class="footer">
                <nobr>
    <img alt="trans" id="trans1" src="images/trans.gif" width="10px" />
               </nobr>
            </td>
            <td style="text-align: right;">
                <nobr><INPUT id="CloseDraw" class="button" onclick="closeDrawing();" type=button  value='<%=CloseCap %>' /> 
<INPUT id="ShowMiniMap" class="button" onclick="showInsetMap();" type=button value='<%=InsetMapCap %>' /> 
<INPUT id="Help" class="button" onclick="window.open('help.htm','','status=0,location=0,menubar=0,width=400,height=400,scrollbars=1,resizable=1');" type=button value='<%=HelpCap %>' /> 
      </nobr>
            </td>
        </tr>
    </table>
</body>
</html>
