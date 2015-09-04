<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Ant.aspx.cs" Inherits="Ant_Ant" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all-gray.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/resources/css/ext-all.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/shared/example.css"/>
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/GridFilters.css" />
    <link rel="stylesheet" type="text/css" href="../sencha/extjs-4.1.0/examples/ux/grid/css/RangeMenu.css" />   

    <style type="text/css">
 
        .x-menu-item img.preview-right, .preview-right {
            background-image: url(../sencha/extjs-4.1.0/examples/feed-viewer/images/preview-right.gif);
        }
        .x-menu-item img.preview-bottom, .preview-bottom {
            background-image: url(../sencha/extjs-4.1.0/examples/feed-viewer/images/preview-bottom.gif);
        }
        .x-menu-item img.preview-hide, .preview-hide {
            background-image: url(../sencha/extjs-4.1.0/examples/feed-viewer/images/preview-hide.gif);
        }

        .x-menu-item img.preview-top, .preview-top {
            background-image: url(../sencha/extjs-4.1.0/examples/feed-viewer/images/preview-top.gif);
        }
        
        .employee-add {
            background-image: url('../shared/icons/fam/user_add.gif') !important;
        }

        .employee-remove {
            background-image: url('../shared/icons/fam/user_delete.gif') !important;
        }
    </style>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script> 
    <script type="text/javascript" src="../sencha/extjs-4.1.0/bootstrap.js"></script>   
    <script type="text/javascript" src="Ant.js"></script> 
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <div id="tabs1">
        <div id="loading" class="x-hide-display">
        </div>
        <div id="panic" class="x-hide-display">
        </div>
        <div id="dump" class="x-hide-display">
        </div>

    </div>
    </div>
    </form>
</body>
</html>
