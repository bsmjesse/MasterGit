<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VexLandmarkFooter.aspx.cs"
    Inherits="SentinelFM.MapVE_VexLandmarkFooter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>VE Footer</title>
    <link type="text/css" rel="Stylesheet" href="ve.css" />
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script src="scripts/interframe.js" type="text/javascript"></script>
    <script src="scripts/util.js" type="text/javascript"></script>

    <script type="text/javascript" src="http://yui.yahooapis.com/2.6.0/build/yahoo/yahoo-min.js"></script>

    <script type="text/javascript" src="http://yui.yahooapis.com/2.6.0/build/event/event-min.js"></script>

    <script language="javascript" type="text/javascript">
    
        function initializeFooter()
        {
        }
        
        function prepFooter(frame) {
/*
    this.onDrawStarted = null;
    this.onDrawEnded = null;
    this.onDrawingChanged = null;
    this.onDrawingSave = null;
    this.onDrawingSubmit = null;
    this.onDrawingClose = null;
    this.onDrawSetPoints = null;

*/
            
            frame.mapscreen.onDrawStarted.subscribe(DrawingStartedHandler);
            frame.mapscreen.onDrawEnded.subscribe(DrawingEndedHandler);
            frame.mapscreen.onDrawChanged.subscribe(DrawingChangedHandler);
            frame.mapscreen.onDrawCleared.subscribe(DrawingClearedHandler);
            frame.mapscreen.onDrawSave.subscribe(DrawingSaveHandler);
            frame.mapscreen.onDrawSubmit.subscribe(DrawingSubmitHandler);
            frame.mapscreen.onDrawClose.subscribe(DrawingCloseHandler);
       }
        

        
        function startDrawing() {
            var fb = getFrameElement("framebody", parent);
           fb.mapscreen.setDrawType("Landmark"); 
            fb.mapscreen.startDraw(); 
        }
        
        function DrawingStartedHandler() {
            var fb = getFrameElement("framebody", parent);
            var enabled = !fb.mapscreen.drawing;
            var value = "disabled";
            if (enabled)
                value = "";
            document.getElementById("StartDraw").disabled = value;        
        }
                
        function finishDrawing() {
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.endDraw(); 
        }
        
        function DrawingEndedHandler() {
            var fb = getFrameElement("framebody", parent);
            var enabled = fb.mapscreen.drawing;
            var value = "disabled";
            if (enabled)
                value = "";
            document.getElementById("FinishDraw").disabled = value;        
            document.getElementById("SaveDraw").disabled = "";        
     }
              
        function saveDrawing() {
            document.getElementById("SaveDraw").disabled = "disabled";
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.saveDraw(); 
        }     
    
        function DrawingSaveHandler() {
            var fb = getFrameElement("framebody", parent);
            var enabled = !fb.mapscreen.drawing;
            var value = "disabled";
            if (enabled)
                value = "";
            document.getElementById("SaveDraw").disabled = value;        
        }        
                
        function clearDrawing() {
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.clearDraw(); 
        }   
        
        function DrawingClearedHandler() {
            document.getElementById("StartDraw").disabled = "";     
            document.getElementById("FinishDraw").disabled = "disabled";     
            document.getElementById("SaveDraw").disabled = "disabled";     
        }
                
               
        function closeDrawing() {
            var fb = getFrameElement("framebody", parent);
            fb.mapscreen.closeDraw(); 
        }   
       
        function DrawingCloseHandler() {
            if(document.getElementById("SaveDraw").disabled == "") {
                var value = confirm("Do you wish to save your working prior to closing?");
                if (value) {
                    var fb = getFrameElement("framebody", parent);
                    fb.mapscreen.saveDraw();
                }
            }
             parent.window.close();
        }        
            
        function DrawingSubmitHandler() {
            //alert(parent.window.frames["framebody"].mapscreen.submitvalue);
            var frame = getFrameElement("framebody", parent);
            //var frame = parent.window.frames["framebody"];
            var points = frame.mapscreen.submitvalue;
            var pointele = frame.document.getElementById("Points");
            pointele.value = points;
            var typeele = frame.document.getElementById("MapType");
            typeele.value = "Landmark";
            frame.document.VEForm.submit();
        }
                    
                    
        function DrawingChangedHandler() {
            var fb = getFrameElement("framebody", parent);
            if (fb.mapscreen.drawing)
                document.getElementById("FinishDraw").disabled = "";        
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
                    <img id="trans1" alt="trans" src="images/trans.gif" width="10" />
                    <input id="StartDraw" class="button" onclick="startDrawing();" type="button" value='<%=startCap %>' />
                    <input id="FinishDraw" class="button" disabled onclick="finishDrawing();" type="button"
                        value='<%=EndCap %>' />
                    <input id="ClearDraw" class="button" onclick="clearDrawing();" type="button" value='<%=ClearCap %>' />
                    <input id="SaveDraw" class="button" disabled onclick="saveDrawing();" type="button"
                        value='<%=SaveCap  %>' />
                    <input id="CloseDraw" class="button" onclick="closeDrawing();" type="button" value='<%=CloseCap %>' />
                </nobr>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">
                <nobr>
                    <input id="ShowMiniMap" class="button" onclick="showInsetMap();" type="button" value='<%=InsetMapCap %>' />
                    <input id="Help" class="button" onclick="window.open('help.htm','','status=0,location=0,menubar=0,width=400,height=400,scrollbars=1,resizable=1');"
                        type="button" value='<%=HelpCap %>' />
                </nobr>
            </td>
        </tr>
    </table>
</body>
</html>
