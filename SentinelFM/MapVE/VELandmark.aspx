<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VELandmark.aspx.cs" Inherits="SentinelFM.VELandmark" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SentinelFM Virtual Earth</title>

    <script src="scripts/util.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">

        function initializeFrames() {
            var fb = getFrameElement("framebody");
            fb.initializeMain('Landmark');
            var ff = getFrameElement("framefooter");
            ff.prepFooter(fb);
        }
    
    </script>

</head>
<frameset>
    <frameset onload="initializeFrames();" frameborder="0" rows="6px, *, 60px">
        <frame id="frameheader" src="VeHeader.aspx" scrolling="no">
        <frame id="framebody" src="ve.aspx" scrolling="no">
        <frame id="framefooter" src="VexLandmarkFooter.aspx" scrolling="no">
    </frameset>
</frameset>
</html>
