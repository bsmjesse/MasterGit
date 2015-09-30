﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="JasperReports_test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="themes/default/easyui.css">
	<link rel="stylesheet" type="text/css" href="themes/icon.css">
	<link rel="stylesheet" type="text/css" href="css/portal.css">

	<style type="text/css">
		body{
			overflow: hidden;
		}
		.title{
			font-size:16px;
			font-weight:bold;
			padding:20px 10px;
			background:#eee;
			overflow:hidden;
			border-bottom:1px solid #ccc;
		}
		.t-list{
			padding:5px;
		}
		button{
			width: 130px;
		}
		.panel-title{
			font-family: sans-serif;
		}
	</style>

	    <script type='text/javascript' src="js/jquery-2.1.4.min.js"></script>
    <script type='text/javascript' src="js/jquery-easyui-1.4.3/jquery.easyui.min.js"></script>
    <script type='text/javascript' src="js/underscore-min.js"></script>
	<script type='text/javascript' src="http://localhost:8080/jasperserver-pro/client/visualize.js"></script>
    <script type="text/javascript">
        var token = 'AA168147784910AE8B28E53427651021';
        //Wrapping and creating the portal. Setting a few parameters around how it will look
        $(function () {
            $('#pp').portal({
                border: false,
                fit: false
            });
        });

        //Here is the JRSClient object that will be used to access repo information and to render reports
        var JRSClient;

        //Log into JRS via visualize.js and create a session object that will be used though out the application
        visualize({
            auth: {
                token: "bsm-" + token,
                loginFn: function (properties, request) {
                    // Use a customLogin function to authenticate 
                    // It must be on the same domain: 'request' works only with JRS instance
                    //alert("Sending custom login request to 'http://your-bi-server/'");
                    return request({
                        url: "http://localhost:8080/jasperserver-pro/flow.html?_flowId=homeFlow&token=bsm-" + token
                    });
                }


            }
        }, function (v) {
            JRSClient = v;
            init();
        }, function () {
            alert("Unexpected error!");
        });

        //Getting a list of all the reports the logged in user can access then run a function to display them 
        function init() {
            JRSClient.resourcesSearch({
                // server: serverUrl,
                folderUri: "/public/Dashboard_Reports",
                recursive: false,
                success: listRepository,
                types: ["reportUnit"],
                error: function (err) {
                    alert(err);
                }
            });
        }

        //Using this function we are adding buttons to the screen to add and remove reports to the dashboard
        //Custom attributes are used to hold information about the status of the report's display and URI inside of JRS
        function listRepository(results) {
            $.each(results, function (index) {
                $("#cp").append("<button status='off' reportURI='" + this.uri + "' onclick=addReport(btnId='btn_" + index + "') title='" + this.label + "' id='btn_" + index + "'>" + reportNameTrim(this.label) + "</button>");
            });
        }

        //Report name trim helper
        function reportNameTrim(name) {
            if (name.length > 20) {
                var x = name.substring(0, 17) + '...';
                return x;
            } else {
                return name;
            }
        }

        //Adding report to the dashboard
        var colIndex = 0;
        function addReport(btnId) {

            var panelCnt = $('#pp').portal('getPanels');

            var reportStatus = $("#" + btnId).attr('status');

            var reportURI = $("#" + btnId).attr('reportURI');

            var reportName = $("#" + btnId).attr('title');

            if (reportStatus == 'off') {

                if (panelCnt.length == 0) {
                    colIndex = 0
                } else if (colIndex > 2) {
                    colIndex = 0
                };

                var p = $('<div />').appendTo('body');

                p.attr('id', 'pannel_' + btnId);

                p.panel({
                    title: reportName,
                    content: '<div id="report_' + btnId + '" style="padding:5px;">Loading your report...</div>',
                    height: 430,
                    closable: false,
                    collapsible: true,
                    noheader: false,
                    tools: [{
                        iconCls: 'icon-pdf',
                        handler: function () { exportReport(btnId); }
                    }, {
                        iconCls: 'icon-reload',
                        handler: function () { refreshReport(btnId); }
                    }, {
                        iconCls: 'icon-filter',
                        handler: function () {
                            $('#inputControls').html('');
                            $('#sendButtonBox').hide();
                            $('#dlg').attr('reportId', btnId);
                            $('#dlg').dialog('open');
                            var ic = JRSClient.inputControls({
                                resource: reportURI,
                                success: function (controls) {
                                    controls.forEach(buildControl);
                                }
                            });
                        }
                    }]
                });

                $('#pp').portal('add', {
                    panel: p,
                    columnIndex: colIndex
                });

                colIndex = colIndex + 1;

                renderReport(reportURI, '#report_' + btnId, JRSClient);

                $("#" + btnId).attr('status', 'on');

                $("#" + btnId).css("background-color", "darkgrey");

            } else {

                removeReport(btnId);

            }
        }

        function buildControl(control) {
            switch (control.type) {
                case "singleValueNumber":
                    $("<div class='panel-title' style='padding-bottom:5px;padding-top:5px'/>")
						.html(control.label)
						.appendTo("#inputControls");

                    $("<input value='' />")
     					.attr("id", control.id)
     					.attr("name", "")
     					.attr("type", "singleValueNumber")
     					.appendTo("#inputControls");

                    $('#' + control.id).numberbox({
                        min: 0,
                    });

                    $('#sendButtonBox').show();
                    break;
                case "multiSelectCheckbox":
                    $("<div class='panel-title' style='padding-bottom:5px;padding-top:5px'/>")
						.html(control.label)
						.appendTo("#inputControls");

                    $("<input value='' />")
     					.attr("id", control.id)
     					.attr("name", "")
     					.attr("type", "multiSelect")
     					.appendTo("#inputControls");

                    $('#' + control.id).combobox({
                        data: control.state.options,
                        valueField: 'value',
                        textField: 'label',
                        multiple: true,
                        onLoadSuccess: function () {
                            $('#sendButtonBox').show();
                        },
                    });
                    break;
                case "singleSelect":
                    $("<div class='panel-title' style='padding-bottom:5px;padding-top:5px'/>")
						.html(control.label)
						.appendTo("#inputControls");

                    $("<input value='' />")
     					.attr("id", control.id)
     					.attr("name", "")
     					.attr("type", "singleSelect")
     					.appendTo("#inputControls");

                    $('#' + control.id).combobox({
                        data: control.state.options,
                        valueField: 'value',
                        textField: 'label',
                        multiple: false,
                        onLoadSuccess: function () {
                            $('#sendButtonBox').show();
                        },
                    });
                    break;
                case "multiSelect":
                    $("<div class='panel-title' style='padding-bottom:5px;padding-top:5px'/>")
						.html(control.label)
						.appendTo("#inputControls");

                    $("<input value='' />")
     					.attr("id", control.id)
     					.attr("name", "")
     					.attr("type", "multiSelect")
     					.appendTo("#inputControls");

                    $('#' + control.id).combobox({
                        data: control.state.options,
                        valueField: 'value',
                        textField: 'label',
                        multiple: true,
                        onLoadSuccess: function () {
                            $('#sendButtonBox').show();
                        },
                    });
                    break;
                case "singleValueText":
                    $("<div class='panel-title' style='padding-bottom:5px;padding-top:5px'/>")
						.html(control.label)
						.appendTo("#inputControls");

                    $("<input value='' />")
     					.attr("id", control.id)
     					.attr("name", "")
     					.attr("type", "singleValueText")
     					.appendTo("#inputControls");
                    $('#sendButtonBox').show();
                    break;
                default:

            }
        }

        function filterReport(id) {
            //alert('Filters Coming Soon!');
            //Get list of all input controls
            var data = {};
            $.each($('#inputControls > :input'), function (index, value) {

                switch ($('#' + value.id).attr("type")) {
                    case "singleSelect":
                        data[value.id] = $('#' + value.id).combobox('getValues');
                        break;
                    case "multiSelect":
                        data[value.id] = $('#' + value.id).combobox('getValues');
                        break;
                    case "singleValueText":
                        var v = [];
                        v.push($('#' + value.id).val());
                        data[value.id] = v;
                        break;
                    case "singleValueNumber":
                        var v = [];
                        v.push($('#' + value.id).numberbox('getValue'));
                        data[value.id] = v;
                        break;
                }
            })

            $.each(reportsList,
				function (index, value) {
				    if (value.container() == ("#report_" + id)) {
				        value
                        .params(data)
                        .run();
				    }
				}
			);
        }

        function refreshReport(id) {
            $.each(reportsList,
				function (index, value) {
				    if (value.container() == ("#report_" + id)) {
				        value.refresh();
				    }
				}
			);
        }

        function exportReport(id) {
            $.each(reportsList,
				function (index, value) {
				    if (value.container() == ("#report_" + id)) {
				        value
						.export({
						    outputFormat: "pdf"
						})
						.done(function (link) {
						    window.open(link.href); // open new window to download report
						})
						.fail(function (err) {
						    alert(err.message);
						});
				    }
				}
			);
        }



        //Used to remove report panels from the dashboard
        function removeReport(id) {

            reportsList = jQuery.grep(reportsList, function (report, i) {
                return (report.container() !== ("#report_" + id));
            });

            $('#pp').portal('remove', $('#pannel_' + id));

            $("#" + btnId).attr('status', 'off');

            $("#" + btnId).css("background-color", "buttonface");

            colIndex = 0;

            $('#dlg').dialog('close');
        }

        var reportsList = new Array();
        //When a pannel is added to the dashboard this function is called to use visualize.js to create the report within that pannel
        function renderReport(uri, container, v) {
            var report = new v.report({
                resource: uri,
                container: container,
                events: {
                    reportCompleted: function (status) {
                    }
                },
                error: function (err) {
                    alert(err.message);
                }
            });
            reportsList.push(report);
        }

	</script>


</head>
<body>
    <form id="form1" runat="server">
    <div id="cp" data-options="region:'west',title:'Report Options',split:true,minWidth:150" style="width:150px;padding-left: 5px;padding-top: 5px;"></div>

    <div data-options="region:'center',title:'Dashboard'" style="padding:5px;background:#eee;">
    	<div id="pp" style="width:1500px;height:auto;">
	    	<div style="width:33%"></div>
	    	<div style="width:33%"></div>
	    	<div style="width:33%"></div>
		</div>
    </div>

    <div id="dlg" reportId="" class="easyui-dialog" title="Report Input Controls" data-options="closed:true,resizable:true" style="width:260px;height:350px;padding:10px">
    	<div id="inputControls"></div>
    	<div id="sendButtonBox" style='padding-bottom:5px;padding-top:20px'>
    		<a id="btnFilter" href="javascript:void(0)" class="easyui-linkbutton" onclick="filterReport($('#dlg').attr('reportId'));" >SetValue</a>
    	</div>	
    </div>
    </form>
   
</html>