<%@ Page Language="C#" AutoEventWireup="true" CodeFile="portal.aspx.cs" Inherits="SentinelFM.DashBoard_portal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Portal Layout Sample</title>

    <link rel="stylesheet" type="text/css" href="extjs/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="classes/portal.css" />

    <script type="text/javascript" src="extjs/ext-core.js"></script>

    <!-- shared example code -->
    <script type="text/javascript" src="shared/examples.js"></script>
    <script type="text/javascript" src="classes/classes.js"></script>

    <script type="text/javascript" src="classes/portals/<%=portalURL %>"></script>
    
    <script type="text/javascript">
        Ext.Loader.setPath('Ext.app', 'classes');

        Ext.require([
            'Ext.layout.container.*',
            'Ext.resizer.Splitter',
            'Ext.fx.target.Element',
            'Ext.fx.target.Component',
            'Ext.window.Window',
            'Ext.app.Portlet',
            'Ext.app.PortalColumn',
            'Ext.app.PortalPanel',
            'Ext.app.Portlet',
            'Ext.app.PortalDropZone',
            'Ext.app.GridPortlet',
            'Ext.app.ChartPortlet'
        ]);

        Ext.onReady(function () {
            Ext.create('Ext.app.Portal');
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
