<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Directions.aspx.cs" Inherits="Routing_Directions" %>

<!DOCTYPE html>
<html> 
	<head>
		<meta http-equiv="X-UA-Compatible" content="IE=7; IE=EmulateIE9; IE=10" />
		<!--<base href="https://api.maps.nlp.nokia.com/enterprise/en/playground/examples/api-for-js/routing/map-with-draggable-route.html" />-->
		<meta http-equiv="content-type" content="text/html; charset=UTF-8"/>
		<title>Nokia Enterprise Maps API for Web and REST Example: Add draggable waypoints to a route</title>
		<meta name="description" content="Changing an existing route by dragging its start and end markers"/>
		<meta name="keywords" content="draggableroute, services"/>
		<!-- For scaling content for mobile devices, setting the viewport to the width of the device-->
		<meta name=viewport content="initial-scale=1.0, maximum-scale=1.0, user-scalable=no"/>
		<!-- Styling for example container (NoteContainer & Logger)  -->
		<link rel="stylesheet" type="text/css" href="https://api.maps.nlp.nokia.com/enterprise/en/playground/examples/templates/js/exampleHelpers.css"/>
		<!-- By default we add ?with=all to load every package available, it's better to change this parameter to your use case. Options ?with=maps|positioning|places|placesdata|directions|datarendering|all -->
		<script type="text/javascript" charset="UTF-8" src="https://api.maps.nlp.nokia.com/2.5.0/jsl.js?with=all"></script>
		<!-- JavaScript for example container (NoteContainer & Logger)  -->
		<script type="text/javascript" charset="UTF-8" src="https://api.maps.nlp.nokia.com/enterprise/en/playground/examples/templates/js/exampleHelpers.js"></script>
        
		<style type="text/css">
			html {
				overflow:hidden;
			}
			
			body {
				margin: 0;
				padding: 0;
				overflow: hidden;
				width: 100%;
				height: 100%;
				position: absolute;
			}
			
			#mapContainer {
				width: 100%;
				height: 100%;
				left: 0;
				top: 0;
				position: absolute;
			}
		</style>
	</head>
	<body>
		<div id="mapContainer"></div>
		<div id="uiContainer"></div>
		<script type="text/javascript" id="exampleJsSource" src="js/Directions.js"></script>
	</body>
</html>
