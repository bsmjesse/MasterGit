var store;
var alarmgrid;
var userDateFormat;
// JavaScript Document
Ext.onReady(function () {

    Ext.tip.QuickTipManager.init();
    //    var pagesize = 2;
    var interval = 5000;   
    var dateformat = userdateformat;
    userDateFormat = userDate;
    var initialData = "";
    var soundPresent = false;
    var TIAsoundPresent = false;
    var DIAsoundPresent = false;
    var newPosition = 10;
    var statusColorString = '#00C000';

    Ext.define('Alarm', {
        extend: 'Ext.data.Model',
        fields: [
            'AlarmId',
             { name: 'TimeCreated', type: 'date', dateFormat: 'c' },
            'AlarmLevel',
            'vehicleDescription',
            'AlarmDescription'
        ]
    });

    // create the Data Store
    store = Ext.create('Ext.data.Store', {
        model: 'Alarm',
        autoLoad: true,
        proxy: {
            // load using HTTP
            type: 'ajax',
            url: 'frmAlarmRotatingServerCall_XML.aspx',
            reader: {
                type: 'xml',
                root: 'Alarm',
                record: 'AllUserAlarmsInfo'
            }
        }
    });


    function renderBody() {
        if (navigator.appName.indexOf("Microsoft") != -1) {
            dateformat = userDate + " " + userTime;
        }
        if (document.all)
            return 'document.body';
        else if (document.getElementById)
            return 'alarms-grid';
        else if (document.layers)
            return 'alarms-grid';
    }

    alarmgrid = Ext.create('Ext.grid.Panel', {
        id: 'alarmgrid',
        enableColumnHide: false,
        enableSorting: false,
        closable: false,
        collapsible: false,
        resizable: false,
        width: 525,
        height: 300,
        title: 'Alarms <a style="margin-left:20px;" href="javascript:void(0)" OnClick="ClearAlarms(-1)">Clear Non-Critical</a> <a style="margin-left:10px;" href="javascript:void(0)" OnClick="ClearAlarms(-2)">Clear All</a><div class="lastalarmchecked" id="lastalarmchecked"></div>',
        store: store,
        renderTo: Ext.getBody(),
        viewConfig: {
            emptyText: 'No alarms to display',
            useMsg: false,
            getRowClass: function (rec, rowIdx, params, store) {
                //devin
                if (rec.get('AlarmDescription').indexOf("CIA") != -1 || rec.get('AlarmDescription').indexOf("Emergency Call:On") != -1) {
                    return 'grid-row-red';
                }
                if (rec.get('AlarmDescription').indexOf("VIA") != -1) {
                    return 'grid-row-yellow';
                }
                if (rec.get('AlarmDescription').indexOf("DIA-") != -1) {
                    return 'grid-row-orange';
                }
            }
        },
        columns: [{
            text: 'Number',
            align: 'left',
            width: 80,
            renderer: function (value) {
                return Ext.String.format('<a href="javascript:void(0)" OnClick="NewWindow({0})">{1}</a>', value, value);
            },
            dataIndex: 'AlarmId',
            sortable: false
        }, {
            text: 'Alarm Time',
            align: 'left',
            width: 120,
            xtype: 'datecolumn',
            format: dateformat,
            dataIndex: 'TimeCreated',
            sortable: false
        }, {
            text: 'Alarm Priority',
            align: 'left',
            width: 80,
            dataIndex: 'AlarmLevel',
            sortable: false
        }, {
            text: 'Alarm Description',
            align: 'left',
            width: 100,
            renderer: function (value) {
                //Devin
                if ((value.indexOf("CIA") != -1 || value.indexOf("Emergency Call:On") != -1) && soundPresent != true) {
                    soundPresent = true;
                    return Ext.String.format('{0} <object><embed src="./sounds/FireAlarm.wav" hidden="true" autostart="True" loop="true" type="audio/wav" pluginspage="http://www.apple.com/quicktime/      download/" /></object>', value);
                    //return Ext.String.format('<audio> <source src="./sounds/FireAlarm.wav" type="audio/wav" preload="preload" loop="loop" autoplay="autoplay"/> <source src="./sounds/FireAlarm.mp3" preload="preload" type="audio/mpeg" loop="loop" autoplay="autoplay" />Your browser does not support the audio element.</audio>');  
                }
                else if (value.indexOf("TIA") != -1 && soundPresent != true && TIAsoundPresent != true) {
                    var hasCIA = false;
                    TIAsoundPresent = true;
                    store.each(function (record, idx) {
                        var desc = record.get('AlarmDescription');
                        if (desc.indexOf("CIA") != -1 || desc.indexOf("Emergency Call:On") != -1) {
                            hasCIA = true;
                            return false;
                        }
                    });
                    if (hasCIA)
                        return value;
                    else {
                        return Ext.String.format('{0} <object><embed src="./sounds/TIAalarm.wav" hidden="true" autostart="True" loop="true" type="audio/wav" pluginspage="http://www.apple.com/quicktime/      download/" /></object>', value);
                    }
                }
                else
                    return value;

            },
            dataIndex: 'AlarmDescription',
            sortable: false
        }, {
            text: 'Vehicle Description',
            align: 'left',
            width: 120,
            dataIndex: 'vehicleDescription',
            sortable: false
        }
        ]
    });



    //    var drawComponent = Ext.create('Ext.draw.Component', {
    //        width: 800,
    //        height: 600,
    //        renderTo: document.body
    //    }), surface = drawComponent.surface;

    //    var sprite = drawComponent.surface.add({
    //        type: 'text',
    //        x: 10,
    //        y: 10
    //    });

    //    surface.add([{
    //        type: 'circle',
    //        radius: 10,
    //        fill: statusColorString,
    //        x: 10,
    //        y: 10,
    //        group: 'circles'
    //    }, 
    //    ]);

    /*
    surface.add([{
    type: 'circle',
    radius: 10,
    fill: '#0000FF',
    x: 50,
    y: 10,
    group: 'bluecircles'
    }, 
    ]);

    
    
    // Get references to my groups
    bluecircles = surface.getGroup('bluecircles');
    */

    // Get references to my groups
    //    circles = surface.getGroup('circles');  
    //   

    var s = function (p) {
        return ('' + p).length < 2 ? '0' + p : '' + p;
    };

    function updateStatus() {
        var currentTime = new Date();
        //        sprite.show(true);
        //        drawComponent.surface.setText(sprite, ' Checked alarms at : ' + currentTime);
        var c;
        if (userDateFormat == 'm/d/Y') {
            c = s(currentTime.getMonth() + 1) + '/' +
            s(currentTime.getDate()) + '/' +
            currentTime.getFullYear() + ' ' +

            s(currentTime.getHours()) + ':' +
            s(currentTime.getMinutes()) + ':' +
            s(currentTime.getSeconds());
        }
        else
        {
            c = s(currentTime.getDate()) + '/' +
                        s(currentTime.getMonth() + 1) + '/' +
                        currentTime.getFullYear() + ' ' +

                        s(currentTime.getHours()) + ':' +
                        s(currentTime.getMinutes()) + ':' +
                        s(currentTime.getSeconds());
        }
        $('#lastalarmchecked').css('background-color', 'green').html(c);
        setTimeout(function () { $('#lastalarmchecked').css('background-color', 'white'); }, 500);

        //    
        //         // Animate the circles across
        //        circles.animate({
        //            duration: 1000,
        //            to: {
        //                translate: {
        //                     x: newPosition + 10
        //                }
        //            }
        //        });

        // Animate the circles across
        //        bluecircles.animate({
        //            duration: 1000,
        //            to: {
        //                translate: {
        //                     x: newPosition + 20
        //                }
        //            }
        //        });    
    }


    /**
    * @private
    * Called internally when a Proxy has completed a load request
    */

    var task = {
        run: function () {

            store.getProxy().extraParams = {
                f: '0'
            };

            if (store.isLoading() != true) {
                var operation = new Ext.data.Operation({
                    action: 'read'
                });
                store.proxy.read(operation, this.onProxyLoad, store);
                //                 console.log('Shehul');
            }
        },
        onProxyLoad: function (operation) {

            var me = this,
                resultSet = operation.getResultSet(),
                records = operation.getRecords(),
                successful = operation.wasSuccessful();

            store.loading = false;
            soundPresent = false;
            TIAsoundPresent = false;
            DIAsoundPresent = false;

            if (operation.response.responseText.replace(/^\s+|\s+$/g, "") == "") {
                updateStatus();
                return;
            }

            if (operation.response.responseText == '<Alarm>sessiontimeout</Alarm>') {
                setTimeout(function () { redirectToLogin(); }, 5000);
                $('#sessiontimeout').show();

                return;
            }



            if (resultSet) {

                store.totalCount = resultSet.total;
                //                }

                //                if (successful) {
                //                    store.loadRecords(records, operation);
                //                }

                if (initialData == "") {
                    initialData = operation.response.responseText;
                    //                         console.info('Data have not changed'); 
                    //                         if(newPosition>10)
                    //                         {
                    //                             newPosition=-30;
                    //                         }
                    //                         else
                    //                         {
                    //                             newPosition=10;
                    //                         }
                    //                         statusColorString='#00C000'
                    updateStatus();
                    //store.fireEvent('load', store, records, successful);
                    //                        console.info('Initial load verify');
                }
                else {
                    if (initialData.length != operation.response.responseText.length) {
                        store.load(
                            {
                                params:
                                 {
                                     f: 1
                                 }
                            }
                        );
                        //                          console.info('Data changed');
                        initialData = operation.response.responseText;
                    }
                    else {
                        //                          console.info('Data have not changed'); 
                        //                          if(newPosition>20)
                        //                          {
                        //                             newPosition=-20;
                        //                          }
                        //                          else
                        //                          {
                        //                             newPosition=30;
                        //                          }                          
                        //                          statusColorString='#FF0000'
                        updateStatus();
                    }
                }
            }
        },
        interval: interval //5 second
    }

    var runner = new Ext.util.TaskRunner();

    runner.start(task);

});