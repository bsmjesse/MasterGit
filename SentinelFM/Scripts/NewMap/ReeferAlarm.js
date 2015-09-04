var ReeferAlarmPageStore;
var ReeferAlarmGrid;
var ReeferAlarmPager;

var ReeferAlarmDateFrom;
var ReeferAlarmTimeFrom;
var ReeferAlarmDateTo;
var ReeferAlarmTimeTo;



function IniReeferAlarm() {

    var alarmLoaded = false;

    var ReeferAlarmPagesize = 15;

    var selModel_ReeferAlarm = Ext.create('Ext.selection.CheckboxModel',
    {
        checkOnly: true,
        enableKeyNav: false,
        listeners:
        {
            selectionchange: function (selModel, selections) {
                try {
                    

                }
                catch (err) {
                }
            }
        }
    });

    var dateformat = 'd/m/Y h:i a';
    Ext.define('ReeferAlarmModel',
    {
        extend: 'Ext.data.Model',
        fields: [       
            {
                name: 'BoxId', type: 'int'
            },
        'VehicleId', 'LicensePlate', 'Description', 'DateTimeReceived', 'DclId', 'BoxMsgInTypeId', 'BoxMsgInTypeName', 'BoxProtocolTypeId',
        'BoxProtocolTypeName', 'CommInfo1', 'CommInfo2', 'ValidGps', 'Latitude', 'Longitude', 'Heading', 'SensorMask', 'CustomProp', 'BlobDataSize', 'SequenceNum',
        'StreetAddress',
        //'Speed',
        {
            name: 'Speed', type: 'int',
            convert: function (value, record) {
                    if (value >= 0 || value < 0)
                        return value * 1;
                    else
                        return -1;
                }
        },
        'BoxArmed', 'MsgDirection', 'Acknowledged', 'Scheduled', 'MsgDetails', 'MyDateTime', 'MyHeading', 'dgKey', 'CustomUrl', 'chkBoxShow',
        {
            name: 'OriginDateTime', type: 'date', dateFormat: 'c'
        }
        , 'icon'
        , 'AlarmDetails'
        , 'Shutdown'
      ]
   });

        ReeferAlarmPageStore = Ext.create('Ext.data.Store',
       {
           //buffered: true,
           pageSize: ReeferAlarmPagesize,
           storeId: 'ReeferAlarmPageStore',
           model: 'ReeferAlarmModel',
           autoLoad: false,
           
           proxy: {
               type: 'ajax',
               url: './historynew/historyservices_Reefer.aspx?st=gethistoryreeferAlarm',
               timeout: 600000,
               reader: {
                   type: 'xml',
                   root: 'MsgInHistory',
                   record: 'VehicleStatusHistory',
                   totalProperty: 'totalCount'
               }
           }

           , listeners:
            {
                'load': function (store, records, options) {

                    try {
                        ReeferAlarmGrid.setTitle("Alarm");
                    }
                    catch (err) {
                    }                    
                }
                ,
                scope: this
            }
       });

       ReeferAlarmPager = new Ext.PagingToolbar(
    {
        store: ReeferAlarmPageStore,
        displayInfo: true,
        displayMsg: 'Displaying alarms {0} - {1} of {2}',
        emptyMsg: "No alarms to display",
        listeners: {
            beforechange: function (b, page, o) {
                
                try {
                    if (historyPagerDoc != '') {
                        historyPagerDoc = '';
                        return;
                    }

                    historygrid.getView().emptyText = 'loading...';

                    var fleetId;
                    if (LoadVehiclesBasedOn == 'fleet') {
                        fleetId = DefaultFleetID;
                    }
                    else {
                        fleetId = DefaultOrganizationHierarchyFleetId;
                    }

                    ReeferAlarmPageStore.proxy.extraParams = {
                        historyFleet: fleetId,
                        historyDateFrom: ReeferAlarmDateFrom,
                        historyTimeFrom: ReeferAlarmTimeFrom,
                        historyDateTo: ReeferAlarmDateTo,
                        historyTimeTo: ReeferAlarmTimeTo,
                        start: (page - 1) * ReeferAlarmPagesize,
                        fromsession: 0
                    };
                }
                catch (err) {
                }
            },

            change: function () {
                //loadingMask.hide();
            }
        }

    }
   );

   ReeferAlarmGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'ReeferAlarmGrid',
       enableColumnHide: true,
       title: 'Alarm',
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width - 5,
       maxHeight: window.screen.height,
       enableSorting: true,
       closable: false,
       columnLines: true,
       width: window.screen.width,
       autoHeight: true,
       store: ReeferAlarmPageStore,
       collapsible: true,
       animCollapse: false,
       split: true,
       //features: [filters],
       //stateId: 'stateVGrid',
       stateful: false, // state should be preserved

       columns: [
            {
                id: 'reeferAlarmDescription',
                //stateId: 'stDescription',
                text: 'Reefer #',
                align: 'left',
                width: 100,
                /*renderer: function (value, p, record) {
                return Ext.String.format('<a href="javascript:void(0);" rel="bootstrapvehiclegridpopover" data-mouseover="false" data-popup="false" class="withajaxpopover" data-content="loading..." data-html="true" data-poload="./MapNew/frmGetVehicleInfo.aspx?LicensePlate={0}" data-title="{1}" data-placement="right" data-container="body" OnClick="SensorInfoWindow(\'{0}\')">{1}</a>', Ext.String.escape(record.data['LicensePlate']), value);
                },*/
                dataIndex: 'Description',
                filterable: false,
                sortable: false
            }
            ,
            {
                id: 'reeferAlarmShutdown',
                //stateId: 'stAddress',
                text: 'Shutdown?',
                align: 'left',
                width: 80,
                dataIndex: 'Shutdown',
                filterable: true,
                sortable: false
            }
            ,

            {
                id: 'reeferAlarmReadTime',
                text: 'Alarm Read Time',
                align: 'left',
                width: 120,
                xtype: 'datecolumn',
                format: dateformat,
                dataIndex: 'OriginDateTime',
                filterable: false,
                sortable: false,
                tdCls: 'x-date-time'
            }
            ,
            {
                id: 'reeferAlarmNumbers',
                text: '# of Alarms',
                align: 'left',
                width: 80,
                xtype: 'datecolumn',
                format: dateformat,
                dataIndex: 'ExtraInfo',
                filterable: false,
                sortable: false,
                renderer: function (value, p, record) {
                    return "1";
                }
            }
            ,
            {
                id: 'reeferAlarmDetails',
                //stateId: 'stAddress',
                text: 'Alarm Details',
                align: 'left',
                width: 350,
                dataIndex: 'AlarmDetails',
                filterable: false,
                sortable: false
            }
        ]
        , selModel: selModel_ReeferAlarm
        , listeners: {
            'activate': function (grid, eOpts) {
                if (alarmLoaded)
                    return;

                ReeferAlarmGrid.setTitle("Loading...");

                alarmLoaded = true;
                var historyDateTimeFrom = new Date();
                //historyDateTimeFrom = historyDateTimeFrom.addHours(-ReeferDashboardDefaultTimeRange);
                
                historyDateTimeFrom.setHours(historyDateTimeFrom.getHours() - ReeferDashboardDefaultTimeRange);
                var historyDateTimeTo = new Date();

                ReeferAlarmDateFrom = TwoDigits(historyDateTimeFrom.getMonth() + 1) + "/" + TwoDigits(historyDateTimeFrom.getDate()) + "/" + historyDateTimeFrom.getFullYear().toString();
                ReeferAlarmTimeFrom = TwoDigits(historyDateTimeFrom.getHours()) + ":" + TwoDigits(historyDateTimeFrom.getMinutes());

                ReeferAlarmDateTo = TwoDigits(historyDateTimeTo.getMonth() + 1) + "/" + TwoDigits(historyDateTimeTo.getDate()) + "/" + historyDateTimeTo.getFullYear().toString();
                ReeferAlarmTimeTo = TwoDigits(historyDateTimeTo.getHours()) + ":" + TwoDigits(historyDateTimeTo.getMinutes());

                var fleetId;
                if (LoadVehiclesBasedOn == 'fleet') {
                    fleetId = DefaultFleetID;
                }
                else {
                    fleetId = DefaultOrganizationHierarchyFleetId;
                }
                ReeferAlarmPageStore.load(
                {
                    params:
                    {
                        historyFleet: fleetId,
                        historyDateFrom: ReeferAlarmDateFrom,
                        historyTimeFrom: ReeferAlarmTimeFrom,
                        historyDateTo: ReeferAlarmDateTo,
                        historyTimeTo: ReeferAlarmTimeTo,
                        limit: ReeferAlarmPagesize
                    }
                });
            }
        }
       //,
       , bbar: ReeferAlarmPager
   });

    tabs.add(ReeferAlarmGrid);
}

function TwoDigits(v) {
    if (v < 10)
        return '0' + v;
    else
        return v;
}