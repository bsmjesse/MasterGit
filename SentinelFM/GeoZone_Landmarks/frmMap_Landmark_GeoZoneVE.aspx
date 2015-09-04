<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMap_Landmark_GeoZoneVE.aspx.cs" Inherits="SentinelFM.GeoZone_Landmarks_frmMap_Landmark_GeoZoneVE" Culture="en-US" UICulture="auto" %>
<%@ Register src="Components/ctlGeozoneLandmarksMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<META HTTP-EQUIV="PRAGMA" CONTENT="NO-CACHE">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head runat="server">
    <title>Untitled Page</title>
       <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    </head>  
<body >
        <form id="frmMap_Landmark_Zone" runat="server" method="post">
            <table id="Table1" style="z-index: 102; left: 31px; position: absolute; top: 65px; height: 494px; width: 933px;">
                <tr>
                    <td style="height: 490px" >
                          <!--<iframe src="frmGeozoneLandmarkMapFrame.aspx" height=490px width=900px scrolling=no   ></iframe>-->     
                          <iframe src="../MapVE/VELandmarks_GeoZones.aspx" height=490px width=900px scrolling=no   ></iframe>     
                    
                    </td>
                </tr>
               
            </table>
            <table id="tblCommands" border="0" cellpadding="0" cellspacing="0" style="z-index: 101;
                left: 8px; position: absolute; top: 4px" width="300">
                <tr>
                    <td>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdMap"  />
                    </td>
                </tr>
                <tr>
                    <td style="height: 588px" valign=top  >
                        <table id="tblBody" align="center" border="0" cellpadding="0" cellspacing="0" width="990">
                            <tr>
                                <td>
                                    <table id="tblForm" border="0" class="table" height="550" width="990">
                                        <tr>
                                            <td class="configTabBackground">
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
