var ReeferMaintenancePageStore;
var ReeferMaintenanceGrid;
var ReeferMaintenancePager;

function IniReeferMaintenance() {
    var dateformat = 'd/m/Y h:i a';
    Ext.define('ReeferMaintenanceModel',
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
      ]
   });

        ReeferMaintenancePageStore = Ext.create('Ext.data.Store',
       {
           //buffered: true,
           pageSize: HistoryPagesize,
           storeId: 'ReeferMaintenancePageStore',
           model: 'ReeferMaintenanceModel',
           autoLoad: false,
       
           proxy: {
               type: 'memory',
               reader: {
                   type: 'xml',
                   root: 'MsgInHistory',
                   record: 'VehicleStatusHistory',
                   totalProperty: 'totalCount'
               }
           }
       });

    ReeferMaintenancePager = new Ext.PagingToolbar(
    {
       store: ReeferMaintenancePageStore,
       displayInfo: true,
       displayMsg: 'Displaying histories {0} - {1} of {2}',
       emptyMsg: "No histories to display",
       listeners: {
           beforechange: function (b, page, o) {
               
               try {
                   if (historyPagerDoc != '') {
                       historyPagerDoc = '';
                       return;
                   }
                   
                   historygrid.getView().emptyText = 'loading...';

                   //var form = btnSubmit.up('form').getForm();
                   var form = historyForm.getForm();
                   if (form.isValid()) {
                       form.submit({
                           url: './historynew/historyservices.aspx?fromsession=1&st=gethistoryrecords&start=' + (page - 1) * HistoryPagesize + '&limit=' + HistoryPagesize,
                           success: function (form, action) {
                               historygrid.getView().emptyText = 'No Maintenance to display';
                               var d = action.result.data;
                               d = d.replace(/\u003c/g, "<").replace("\u003e", ">");

                               if (action.result.iconTypeName != "") IconTypeName = action.result.iconTypeName;
                               var doc;
                               if (window.ActiveXObject) {         //IE
                                   var doc = new ActiveXObject("Microsoft.XMLDOM");
                                   doc.async = "false";
                                   doc.loadXML(d);
                               } else {                             //Mozilla
                                   var doc = new DOMParser().parseFromString(d, "text/xml");
                               }
                               historyPageStore.loadRawData(doc);
                               historyForm.hide();
                           },
                           failure: function (form, action) {
                               historygrid.getView().emptyText = 'No History to display';
                               Ext.Msg.alert('Failed', 'some error');
                           }
                       });
                   }
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

   ReeferMaintenanceGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'ReeferMaintenanceGrid',
       enableColumnHide: true,
       title: 'Maintenance',
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
       store: ReeferMaintenancePageStore,
       collapsible: true,
       animCollapse: false,
       split: true,
       //features: [filters],
       //stateId: 'stateVGrid',
       stateful: false, // state should be preserved

       columns: [
       
            {
                id: 'reeferMaintenanceDescription',
                //stateId: 'stDescription',
                text: 'Reefer #',
                align: 'left',
                width: 150,
                /*renderer: function (value, p, record) {
                    return Ext.String.format('<a href="javascript:void(0);" rel="bootstrapvehiclegridpopover" data-mouseover="false" data-popup="false" class="withajaxpopover" data-content="loading..." data-html="true" data-poload="./MapNew/frmGetVehicleInfo.aspx?LicensePlate={0}" data-title="{1}" data-placement="right" data-container="body" OnClick="SensorInfoWindow(\'{0}\')">{1}</a>', Ext.String.escape(record.data['LicensePlate']), value);
                },*/
                dataIndex: 'Description',
                filterable: false,
                sortable: false
            }
            ,
       
            {
                id: 'reeferMaintenanceMessageTime',
                text: 'MessageTime',
                align: 'left',
                width: 120,
                xtype: 'datecolumn',
                format: dateformat,
                dataIndex: 'OriginDateTime',
                filterable: true,
                sortable: true,
                tdCls: 'x-date-time'
            }
            ,
            {
                id: 'reeferMaintenancePMAlert',
                //stateId: 'stAddress',
                text: 'PM Alert',
                align: 'left',
                width: 80,
                dataIndex: 'StreetAddress',
                filterable: true,
                sortable: false
            }
            ,
            {
                id: 'reeferMaintenanceCountDown',
                //stateId: 'stAddress',
                text: 'Count Down',
                align: 'left',
                width: 80,
                dataIndex: 'StreetAddress',
                filterable: true,
                sortable: false
            }
            ,
            {
                id: 'reeferMaintenancePMHours',
                //stateId: 'stAddress',
                text: 'PM Hours',
                align: 'left',
                width: 55,
                dataIndex: 'StreetAddress',
                filterable: true,
                sortable: false
            }
            ,
            {
                id: 'reeferMaintenanceEngineHours',
                //stateId: 'stAddress',
                text: 'Engine Hours',
                align: 'left',
                width: 80,
                dataIndex: 'StreetAddress',
                filterable: true,
                sortable: false
            }
            ,
            {
                id: 'reeferMaintenanceBatteryVolt',
                //stateId: 'stAddress',
                text: 'Battery Volt',
                align: 'left',
                width: 80,
                dataIndex: 'Speed',
                filterable: true,
                sortable: false
            }
            ,
            {
                id: 'reeferMaintenanceFuel',
                //stateId: 'stAddress',
                text: 'Fuel',
                align: 'left',
                width: 70,
                dataIndex: 'Speed',
                filterable: true,
                sortable: false
            }
            ,
            {
                id: 'reeferMaintenanceControlSensor',
                //stateId: 'stAddress',
                text: 'Control Sensor',
                align: 'left',
                width: 100,
                dataIndex: 'Speed',
                filterable: true,
                sortable: false
            }
        ]      
        //,
        //bbar: ReeferMaintenancePager
    });

    tabs.add(ReeferMaintenanceGrid); 
}