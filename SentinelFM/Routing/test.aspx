<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="RouterBuilder_test" %>

<!DOCTYPE html>
<html> 
	<head>
	    <title>test</title>
        <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
        <link rel="stylesheet" href="gh-buttons.css" />
		<script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
        <script type="text/javascript" src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
        <script type="text/javascript">
            
            (function ($) {
                $.widget("ui.test", {
                    _init: function () {
                        //alert(this.element.html("test test"));
                    }
                });
            }(jQuery));


            $(document).ready(function() {
                $('#baba').test();
            });
        </script>
	</head>
	<body>
		<div id="baba"></div>
 <ul class="button-group">
    <li><a href="#" class="button primary pill">Dashboard</a></li>
    <li><a href="#" class="button pill">Inbox</a></li>
    <li><a href="#" class="button pill">Account</a></li>
    <li><a href="#" class="button pill">Logout</a></li>
</ul>

	</body>
</html>