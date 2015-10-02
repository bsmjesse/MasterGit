var reeferHststore;
var reeferHstPageStore;
var reeferHstgrid;
var reeferHstGridForm;

var reeferHstStopStore;
var reeferHstStopGrid;

var reeferHstTripStore;
var reeferHstTripGrid;

var reeferHstIni = false;
var reeferHstIniVehicleId = '';
var reeferHstVehicleStoreLoaded = false;
var reeferHstFleetId = '';
var reeferHstFleetName = '';

var reeferHstAddressStore;
var reeferHstAddressGrid;
var reeferHstAddressResetField;
var reeferHstAddressFleetId;
var reeferHstAddressFleetName;

var reeferHstDateFrom;
var reeferHstTimeFrom
var reeferHstDateTo;
var reeferHstTimeTo;

var reeferHstOrganizationHierarchy;

var reeferHstVehicleStore;
var reeferHstHiddenFleet;
var fleetWin;

var reeferHstTripsNum = 0;

var AllReeferHstRecords = [];

var reeferHstPagerDoc = '';

var reeferHstPage = './reeferHst/frmhistmain_new.aspx?VehicleId=';
var reeferHstFleetButton;

Ext.define('ReeferHistoryListModel',
   {
       extend: 'Ext.data.Model',
       fields: [
       //'BoxId', 
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
      , 'icon', 'Micro', 'Tether', 'Power', 'ReeferState', 'ModeOfOp', 'Door', 'AFAX', 'Setpt', 'Ret', 'Dis', 'Amb', 'SensorProbe', 'Setpt2', 'Ret2', 'Dis2', 'SensorProbe2', 'Setpt3', 'Ret3', 'Dis3', 'SensorProbe3', 'SpareTemp', 'FuelLevel', 'EngineHours', 'ControllerType', 'RPM', 'BatteryVolt'
      ]
   }
   );

function IniReeferReeferHistory() {
    var proxyTimeOut = 120000;
    
    var filters =
   {
       ftype: 'filters',
       local: true,   // defaults to false (remote filtering)
       filters: [
      {
          type: 'string',
          dataIndex: 'BoxId'
      }
      ,
      {
          type: 'string',
          dataIndex: 'Description'
      }
      ,
      {
          type: 'string',
          dataIndex: 'StreetAddress'
      }
      ,
      {
          type: 'string',
          dataIndex: 'VehicleStatus'
      }
      ,
      {
          type: 'int',
          dataIndex: 'CustomSpeed'
      }
      ,
      {
          type: 'date',
          dataIndex: 'OriginDateTime'
      }
      ,
      {
          type: 'boolean',
          dataIndex: 'BoxArmed'
      }
      ]
   }
   ;

    reeferHststore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: HistoryPagesize,
       storeId: 'reeferHststore',
       model: 'ReeferHistoryListModel',

       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  AllReeferHstRecords = records;
                  //setTimeout(function () { mapReeferHistories(records, true, false, false); }, 500);
                  reeferHstgrid.getSelectionModel().selectAll(false);
                  $('#reeferhistoriescount').html(records.length);
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

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

    reeferHstPageStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: HistoryPagesize,
       storeId: 'reeferHstPageStore',
       model: 'ReeferHistoryListModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {

              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

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

    reeferHstStopStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'reeferHstStopStore',
       model: 'HistoryStopModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  //mapReeferHistories(records, true, false, true);
                  reeferHstStopGrid.getSelectionModel().selectAll(false);

                  $('#stopreeferhistoriescount').html(records.length);
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

       proxy: {
           type: 'memory',
           reader: {
               type: 'xml',
               root: 'DsStopReport',
               record: 'StopData'
               //,totalProperty: 'totalCount'
           }
       }
   });

    reeferHstTripStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'reeferHstTripStore',
       model: 'HistoryTripModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {

                  reeferHstTripsNum = records.length;
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

       proxy: {
           type: 'memory',
           reader: {
               type: 'xml',
               root: 'dstTripSummaryPerVehicle',
               record: 'Table'
               //,totalProperty: 'totalCount'
           }
       }
   });

    reeferHstAddressStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'reeferHstAddressStore',
       model: 'HistoryAddressModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  searchingMask.hide();
              }
              catch (err) {
              }
          }
         ,
          scope: this
      },

       proxy:
      {
          type: 'ajax',
          url: 'Vehicles_Reefer.aspx?QueryType=searchHistoryAddress',
          timeout: 600000,
          reader:
         {
             type: 'xml',
             root: 'NewDataSet',
             record: 'Table'//,
             //totalProperty: 'totalCount'
         }
      }
   });

    reeferHstOrganizationHierarchy = Ext.create('Ext.Button',
   {
       text: DefaultOrganizationHierarchyFleetName == '' ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName,
       id: 'reeferHstOrganizationHierarchyButton',
       tooltip: 'Organization Hierarchy',
       cls: 'cmbfonts',
       textAlign: 'left',
       width: 230,
       style: { marginLeft: '1px' },
       handler: function () {
           try {
               var mypage = '../../Widgets/OrganizationHierarchy.aspx?nodecode=' + reeferHstOrganizationHierarchyNodeCode;
               var myname = 'OrganizationHierarchy';
               var w = 740;
               var h = 440;
               var winl = (screen.width - w) / 2;
               var wint = (screen.height - h) / 2;
               winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
               win = window.open(mypage, myname, winprops)
               if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
               return false;
           }
           catch (err) {
           }
       }
   }
   );

    reeferHstFleetButton = Ext.create('Ext.Button',
   {
       text: SelectedFleetName == '' ? SelectedFleetId : SelectedFleetName,
       id: 'reeferHstFleetButton',
       tooltip: 'Select a fleet',
       cls: 'cmbfonts',
       icon: 'preview.png',
       textAlign: 'left',
       width: 230,
       style: { marginLeft: '5px' },
       handler: function () {
           try {
               var url = "./Widgets/fleet.aspx?fleetId=" + reeferHstFleetId + '&f=reeferHstFleetButton';
               var urlToLoad = '<iframe width="100%" height="100%" frameborder="0" scrolling="no" src="' + url + '"></iframe>';
               //if (fleetWin==undefined)
               fleetWin = openWindow('Select a fleet', urlToLoad, 400, 150);
               //else
               //{
               //     fleetWin.show();
               //}  
           }
           catch (err) {
           }
       }
   }
   );

   var reeferHstPager = new Ext.PagingToolbar(
   {
       store: reeferHstPageStore,
       displayInfo: true,
       displayMsg: 'Displaying reefer histories {0} - {1} of {2}',
       emptyMsg: "No reefer history to display",
       listeners: {
           beforechange: function (b, page, o) {

               try {
                   if (reeferHstPagerDoc != '') {
                       reeferHstPagerDoc = '';
                       return;
                   }
                   if (reeferHstVehicles.getValue() == null || reeferHstVehicles.getValue() == '-1') {
                       Ext.Msg.alert('Oops', 'Please select a vehicle...');
                       return;
                   }

                   reeferHstgrid.getView().emptyText = 'loading...';

                   //var form = btnSubmit.up('form').getForm();
                   var form = reeferHstForm.getForm();
                   if (form.isValid()) {
                       form.submit({
                           url: './historynew/historyservices_Reefer.aspx?fromsession=1&st=gethistoryrecords&reeferScreenName=reefer&start=' + (page - 1) * HistoryPagesize + '&limit=' + HistoryPagesize,
                           success: function (form, action) {
                               reeferHstgrid.getView().emptyText = 'No Reefer History to display';
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
                               reeferHstPageStore.loadRawData(doc);
                               reeferHstForm.hide();
                           },
                           failure: function (form, action) {
                               reeferHstgrid.getView().emptyText = 'No Reefer History to display';
                               //Ext.Msg.alert('Failed', action.result.msg);// Ext.Msg.alert('Failed', 'some error');
                               if (action.result && action.result.msg && action.result.msg != '')
                                   Ext.Msg.alert('Failed', action.result.msg);
                               else
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

    var selReeferHstModel = Ext.create('Ext.selection.CheckboxModel',
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
   }
   );

    var selReeferHstStopModel = Ext.create('Ext.selection.CheckboxModel',
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
   }
   );

    /*
    *
    * Code of reefer history grid with form
    *
    */


    /*
    * Here is where we create the Form
    */
    reeferHstDateFrom = Ext.create('Ext.form.field.Date', {
        anchor: '100%',
        labelWidth: 50,
        maxWidth: 190,
        fieldLabel: 'From',
        name: 'historyDateFrom',
        format: userDate,
        value: new Date()
    });
    reeferHstTimeFrom = Ext.create('Ext.form.field.Time', {
        name: 'historyTimeFrom',
        fieldLabel: '',
        labelWidth: 0,
        minValue: '12 AM',
        maxValue: '11:45 PM',
        increment: 15,
        value: '12:00 AM',
        maxWidth: 100,
        margin: '0 0 0 10',
        editable: false
    });
    reeferHstDateTo = Ext.create('Ext.form.field.Date', {
        anchor: '100%',
        labelWidth: 50,
        maxWidth: 190,
        fieldLabel: 'To',
        name: 'historyDateTo',
        format: userDate,
        //value: (new Date()).getDate() + 1
        value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1)
    });
    reeferHstTimeTo = Ext.create('Ext.form.field.Time', {
        name: 'historyTimeTo',
        fieldLabel: '',
        labelWidth: 0,
        minValue: '12 AM',
        maxValue: '11:45 PM',
        increment: 15,
        value: '12:00 AM',
        maxWidth: 100,
        margin: '0 0 0 10',
        editable: false
    });

    

    var btnReeferHstSearch = Ext.create('Ext.Button',
       {
           text: 'Advanced Search',
           id: 'btmReeferHstSearch',
           tooltip: 'Search Reefer History',
           iconCls: 'map',
           cls: 'cmbfonts',
           textAlign: 'left',
           handler: function () {
               try {
                   if (reeferHstForm.isHidden())
                       reeferHstForm.show();
                   else
                       reeferHstForm.hide();

               }
               catch (err) {
               }
           }
       }
       );

    var btnReeferHstMapit = Ext.create('Ext.Button',
       {
           text: 'Map It',
           id: 'btnReeferHstMapit',
           tooltip: 'Map the selected reefer history records',
           iconCls: 'map',
           margin: '0 5',
           cls: 'cmbfonts',
           textAlign: 'left',
           hidden: true,
           handler: function () {
               try {
                   var reeferHsttype = reeferHstType.getValue();
                   if (reeferHsttype == 0) {
                       selections = reeferHstgrid.getSelectionModel().getSelection();
                       mapReeferHistories(selections, true, true, false);
                   }
                   else if (reeferHsttype == 1 || reeferHsttype == 2 || reeferHsttype == 3) {
                       selections = reeferHstStopGrid.getSelectionModel().getSelection();
                       mapReeferHistories(selections, true, true, true);
                   }
                   else if (reeferHsttype == 4) {
                       var selections = [];
                       for (itrip = 0; itrip < reeferHstTripsNum; itrip++) {
                           var innerGrid = Ext.getCmp('reeferHstDetailsGrid' + itrip);
                           if (innerGrid) {
                               var ss = innerGrid.getSelectionModel().getSelection();
                               selections = selections.concat(ss);
                           }
                       }
                       //var innerGrid = Ext.getCmp('mapReeferHistoriesDetailsGrid' + '0');
                       //var selections = innerGrid.getSelectionModel().getSelection();
                       mapReeferHistories(selections, true, true, false);
                   }

               }
               catch (err) {
                   var i = 0;
               }
           }
       }
       );


    var btnReeferHstLegend = Ext.create('Ext.Button',
       {
           text: 'Map Legend',
           id: 'btnReeferHstLegend',
           iconCls: 'map',
           margin: '0 5',
           cls: 'cmbfonts',
           textAlign: 'left',
           hidden: true,
           handler: function () {
               try {
                   if (!winMapLegend) {
                       var legendURL = "./History/frmhistoryMapsoluteLegend.aspx?f=1";
                       var urlToLoad = '<iframe style="background-color:white;" width="100%" height="100%" frameborder="0" scrolling="no" src="' + legendURL + '"></iframe>';
                       winMapLegend = openWindow('Map Legend', urlToLoad, 300, 230);
                   }
                   else {
                       if (winMapLegend.isVisible()) {
                           winMapLegend.hide();
                       } else {
                           winMapLegend.show();
                       }
                   }
               }
               catch (err) {
               }
           }
       }
       );

    var btnReeferHstMapAll = Ext.create('Ext.Button',
       {
           text: 'Map All',
           id: 'btnReeferHstMapAll',
           iconCls: 'map',
           margin: '0 5',
           cls: 'cmbfonts',
           textAlign: 'left',
           hidden: true,
           handler: function () {
               try {
                   if (AllReeferHstRecords.length > 0)
                       mapReeferHistories(AllReeferHstRecords, true, false, false);
               }
               catch (err) {
               }
           }
       }
       );

    var reeferHstType_values = [
            [0, 'Vehicle Path'],
            [1, 'Stop and Idle Sequence'],
            [2, 'Stop Sequence'],
            [3, 'Idle Sequence'],
            [4, 'Trip Report']
        ];

    var reeferHstType_store = new Ext.data.SimpleStore({
        fields: ['number', 'histype'],
        data: reeferHstType_values
    });


    var reeferHstType = new Ext.form.ComboBox({
        name: 'historyType',
        fieldLabel: 'Type',
        hiddenName: 'historyType',
        store: reeferHstType_store,
        displayField: 'histype',
        valueField: 'number',
        value: 0,
        labelWidth: 50,
        width: 300,
        typeAhead: true,
        mode: 'local',
        triggerAction: 'all',
        emptyText: 'Choose number...',
        selectOnFocus: true,
        editable: false,
        margin: '0 0 0 10',
        listeners:
                 {
                     scope: this,
                     'select': function (combo, value) {
                         try {
                             var selectedtype = combo.getValue();
                             if (selectedtype == 0) {
                                 btnReeferHstMapAll.show();
                                 btnReeferHstLegend.hide();
                                 reeferHsttabs.setActiveTab(reeferHstMessageForm);
                             }
                             else if (selectedtype == 1 || selectedtype == 2 || selectedtype == 3) {
                                 btnReeferHstLegend.show();
                                 btnReeferHstMapAll.hide();
                                 reeferHsttabs.setActiveTab(reeferHstMessageForm);
                             }
                             else {
                                 btnReeferHstLegend.hide();
                                 btnReeferHstMapAll.hide();
                                 reeferHsttabs.setActiveTab(reeferHstTripRadios);
                             }
                         }
                         catch (err) {
                         }
                     }
                 }
    });

    reeferHstHiddenFleet = Ext.create('Ext.form.field.Hidden',
            {
                name: 'historyFleet',
                value: LoadVehiclesBasedOn == 'fleet' ? SelectedFleetId : DefaultOrganizationHierarchyFleetId
            }
        );
                    
    reeferHstVehicleStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryVehicleList',
               autoLoad: false,
               storeId: 'reeferHstVehicleStore',
               listeners:
               {
                   'load': function (xstore, records, options) {

                       var u = Ext.create('HistoryVehicleList', {
                           VehicleId: '-1',
                           Description: 'Select a Vehicle'
                       });
                       xstore.insert(0, u);
                       u = Ext.create('HistoryVehicleList', {
                           VehicleId: '0',
                           Description: 'Entire Fleet'
                       });
                       xstore.insert(1, u);

                       if (reeferHstIniVehicleId != '') {
                           reeferHstVehicles.setValue(reeferHstIniVehicleId);
                           reeferHstIniVehicleId = '';
                       }
                       else
                           reeferHstVehicles.setValue('-1');

                       reeferHstVehicleStoreLoaded = true;
                   }
               },
               proxy:
                  {
                      // load using HTTP
                      type: 'ajax',
                      url: './historynew/historyservices_Reefer.aspx',
                      timeout: proxyTimeOut,
                      reader:
                     {
                         type: 'xml',
                         root: 'Fleet',
                         record: 'VehiclesInformation'
                     }
                  }
           }
       );
             
             
    var reeferHstVehicles = Ext.create('Ext.form.ComboBox',
       {
           name: 'historyVehicle',
           store: 'reeferHstVehicleStore',
           displayField: 'Description',
           valueField: 'VehicleId',
           typeAhead: true,
           fieldStyle: 'cmbfonts',
           labelCls: 'cmbLabel',
           queryMode: 'local',
           triggerAction: 'all',
           fieldLabel: ' Vehicle',
           //emptyText: fleetDefaultText,
           emptyText: 'Loading...',
           tooltip: 'Select a vehicles',
           selectOnFocus: true,
           width: 300,
           labelWidth: 50,
           editable: false,
           //margin: '0 0 0 10',
           listeners:
          {
              scope: this,
              'select': function (combo, value) {
                  //alert('selec changed');
                  var selVehicle = combo.getValue();
                  try {
                      reeferHstCommModeStore.load(
                        {
                            params:
                            {
                                vehicleID: selVehicle
                            }
                        }
                    );
                  }
                  catch (err) {
                  }

              },
              'afterrender': function () {
                  //fleetstore.load();
                  //alert('afterrender');

                  if (LoadVehiclesBasedOn == 'fleet') {
                      //alert('fleet:' + historyHiddenFleet.getValue());
                      reeferHstVehicleStore.load(
                        {
                            params:
                                {
                                    fleetID: SelectedFleetId
                                }
                        }
                      );
                  }
                  else {
                      reeferHstVehicleStore.load(
                            {
                                params:
                                {
                                    //fleetID: HistoryOrganizationHierarchyFleetId
                                    fleetId: DefaultOrganizationHierarchyFleetId
                                }
                            }
                        );
                  }
              }
          }
       });
       
    var reeferHstCommModeStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryCommModeList',
               autoLoad: false,
               storeId: 'reeferHstCommModeStore',
               listeners:
               {
                   'load': function (xstore, records, options) {

                       var u = Ext.create('HistoryCommModeList', {
                           DclId: '-1',
                           CommModeName: 'ALL'
                       });
                       xstore.insert(0, u);

                       reeferHstCommModes.setValue('-1');
                   }
               },
               proxy:
                  {
                      // load using HTTP
                      type: 'ajax',
                      url: './historynew/historyservices_Reefer.aspx?st=getcommmode',
                      timeout: proxyTimeOut,
                      reader:
                     {
                         type: 'xml',
                         root: 'Box',
                         record: 'BoxConfigInfo'
                     }
                  }
           }
       );
    var reeferHstCommModes = Ext.create('Ext.form.ComboBox',
       {
           name: 'historyCommMode',
           store: 'reeferHstCommModeStore',
           displayField: 'CommModeName',
           valueField: 'DclId',
           typeAhead: true,
           fieldStyle: 'cmbfonts',
           labelCls: 'cmbLabel',
           queryMode: 'local',
           triggerAction: 'all',
           fieldLabel: ' Comm',
           emptyText: 'Loading...',
           tooltip: 'Select a Comm Mode',
           selectOnFocus: true,
           width: 300,
           labelWidth: 50,
           editable: false,
           margin: '0 0 0 10',
           listeners:
          {
              scope: this,
              'select': function (combo, value) {
                  try {

                  }
                  catch (err) {
                  }

              },
              'afterrender': function () {
                  //fleetstore.load();
                  //alert('afterrender');
                  reeferHstCommModeStore.load();
              }
          }
       }
       );

    var reeferHstMessageCheckBox = Ext.create('Ext.form.field.Checkbox',
           {
               boxLabel: 'Last message only',
               name: 'lastmessageonly',
               inputValue: '1',
               id: 'checkbox1',
               border: 0
           }
       );

    var reeferHstMessageStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryMessageModel',
               autoLoad: false,
               storeId: 'reeferHstMessageStore',
               listeners:
               {
                   'load': function (xstore, records, options) {
                       var u = Ext.create('HistoryMessageModel', {
                           BoxMsgInTypeId: '-1',
                           BoxMsgInTypeName: 'All Messages'
                       });
                       xstore.insert(0, u);

                       reeferHstMessageList.setValue('-1');
                   }
               },
               proxy:
                  {
                      // load using HTTP
                      type: 'ajax',
                      url: './historynew/historyservices_Reefer.aspx?st=GetMessageList',
                      timeout: proxyTimeOut,
                      reader:
                     {
                         type: 'xml',
                         root: 'Box',
                         record: 'BoxMsgTypes'
                     }
                  }
           }
       );

    var reeferHstMessageList = Ext.create('Ext.ux.form.MultiSelect',
            {
                anchor: '100%',
                msgTarget: 'side',
                fieldLabel: '',
                name: 'historyMessageList',
                width: 280,
                allowBlank: true,
                valueField: 'BoxMsgInTypeId',
                displayField: 'BoxMsgInTypeName',
                height: 120,
                emptyText: '',
                // minSelections: 2,
                // maxSelections: 3,

                store: reeferHstMessageStore,
                ddReorder: false,
                listeners:
                  {
                      scope: this,
                      'afterrender': function () {
                          reeferHstMessageStore.load();
                      }
                  }
            }
        );

    var reeferHstMessageForm = Ext.create('Ext.Panel', {
        title: 'Messages',
        labelWidth: 50, // label settings here cascade unless overridden
        frame: true,
        bodyStyle: 'padding:5px 5px 0;',
        width: 550,
        layout: 'column', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: true,
        defaultType: 'textfield',
        items: [reeferHstMessageCheckBox, reeferHstMessageList]
    });

    var reeferHstLocationText = Ext.create('Ext.form.field.Text', {
        name: 'historyLocation',
        labelWidth: 50,
        fieldLabel: 'Address',
        allowBlank: true  // requires a non-empty value
    });

    var reeferHstByLocation = Ext.create('Ext.form.field.Hidden', {
        name: 'historyByLocation',
        value: '0'
    });

    var reeferHstLoactionForm = Ext.create('Ext.Panel', {
        id: 'reeferHstLoactionForm',
        title: 'Location',
        labelWidth: 50, // label settings here cascade unless overridden
        frame: true,
        bodyStyle: 'padding:5px 5px 0;',
        width: 550,
        layout: 'column', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: true,
        defaultType: 'textfield',
        items: [reeferHstLocationText]
    });


    var reeferHstTripRadios = Ext.create('Ext.Panel', {
        title: 'Trip',
        labelWidth: 0,
        border: 0,
        frame: true,
        bodyStyle: 'padding:10;border:0;background-color:transparent;',
        width: 310,
        layout: 'column', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: false,
        defaultType: 'radiofield',
        items: [{ xtype: 'component', html: 'Calculate Trips based on:', cls: '' },
                    {
                        boxLabel: 'Ignition',
                        name: 'historytrip',
                        inputValue: '3',
                        id: 'historytrip1',
                        checked: true
                    }, {
                        boxLabel: 'Tractor Power',
                        name: 'historytrip',
                        inputValue: '11',
                        id: 'historytrip2'
                    }, {
                        boxLabel: 'PTO',
                        name: 'historytrip',
                        inputValue: '8',
                        id: 'historytrip3'
                    }]
    });

    var btnReeferHstSubmit = Ext.create('Ext.Button', {
        text: 'View',
        cls: 'cmbfonts',
        //margin: '10 auto',
        style: { margin: '10px 0 10px 55px' },
        width: 100,
        handler: function () {
            try {
                if (reeferHstVehicles.getValue() == null || reeferHstVehicles.getValue() == '-1') {
                    Ext.Msg.alert('Oops', 'Please select a vehicle...');
                    return;
                }
                //var form = this.up('form').getForm();
                var form = reeferHstForm.getForm();

                var reeferHsttype = reeferHstType.getValue();
                
                if (reeferHsttype == 0) {
                    reeferHstGridForm.remove(reeferHstStopGrid, false);
                    reeferHstGridForm.remove(reeferHstTripGrid, false);
                    reeferHststore.removeAll();
                    reeferHstPageStore.removeAll();
                    reeferHstGridForm.add(reeferHstgrid);
                }
                else if (reeferHsttype == 1 || reeferHsttype == 2 || reeferHsttype == 3) {
                    reeferHstGridForm.remove(reeferHstgrid, false);
                    reeferHstGridForm.remove(reeferHstTripGrid, false);
                    reeferHstStopStore.removeAll();
                    reeferHstGridForm.add(reeferHstStopGrid);
                }
                else if (reeferHsttype == 4) {
                    reeferHstGridForm.remove(reeferHstgrid, false);
                    reeferHstGridForm.remove(reeferHstStopGrid, false);
                    reeferHstTripStore.removeAll();
                    reeferHstGridForm.add(reeferHstTripGrid);
                }
                reeferHstGridForm.doLayout();

                if (form.isValid()) {
                    loadingMask.show();
                    form.submit({
                        url: './historynew/historyservices_Reefer.aspx?st=gethistoryrecords&reeferScreenName=reefer&start=0&limit=' + HistoryPagesize,
                        timeout: 1800,
                        success: function (form, action) {
                            try {
                                var d;

                                d = action.result.data;
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

                                if (reeferHsttype == 0) {
                                    var wholedata = action.result.wholedata;
                                    wholedata = wholedata.replace(/\u003c/g, "<").replace("\u003e", ">");

                                    var docwholedata;
                                    if (window.ActiveXObject) {         //IE
                                        docwholedata = new ActiveXObject("Microsoft.XMLDOM");
                                        docwholedata.async = "false";
                                        docwholedata.loadXML(wholedata);
                                    } else {                             //Mozilla
                                        docwholedata = new DOMParser().parseFromString(wholedata, "text/xml");
                                    }

                                    reeferHststore.loadRawData(docwholedata);
                                    reeferHstPagerDoc = '1';
                                    reeferHstPager.moveFirst();
                                    reeferHstPageStore.loadRawData(doc);
                                }
                                else if (reeferHsttype == 1 || reeferHsttype == 2 || reeferHsttype == 3) {
                                    reeferHstStopStore.loadRawData(doc);
                                }
                                else if (reeferHsttype == 4) {
                                    reeferHstTripStore.loadRawData(doc);

                                    var tripdetails = action.result.tripdata;
                                    tripdetails = tripdetails.replace(/\u003c/g, "<").replace("\u003e", ">");

                                    var doctripdata;
                                    if (window.ActiveXObject) {         //IE
                                        doctripdata = new ActiveXObject("Microsoft.XMLDOM");
                                        doctripdata.async = "false";
                                        doctripdata.loadXML(tripdetails);
                                    } else {                             //Mozilla
                                        doctripdata = new DOMParser().parseFromString(tripdetails, "text/xml");
                                    }

                                    reeferHststore.loadRawData(doctripdata);
                                }

                                //historystore.loadRawData(doc);
                                reeferHstForm.hide();
                                loadingMask.hide();
                            }
                            catch (error) {
                                reeferHstForm.hide();
                                loadingMask.hide();
                            }

                        },
                        failure: function (form, action) {
                            loadingMask.hide();                            
                            //Ext.Msg.alert('Failed', action.result.msg);//Ext.Msg.alert('Failed', 'some error');
                            if (action.result && action.result.msg && action.result.msg != '')
                                Ext.Msg.alert('Failed', action.result.msg);
                            else
                                Ext.Msg.alert('Failed', 'some error');
                        }
                    });
                }
            }
            catch (err) {
            }
        }
    });

    var txtButtonTitle = Ext.create('Ext.draw.Text', {
        text: LoadVehiclesBasedOn == 'fleet' ? 'Fleet:' : 'Hierarchy:',
        width: LoadVehiclesBasedOn == 'fleet' ? 50 : 55,
        margin: '0 0 0 10',
        height: 20
    });

    var reeferHstDateTimeContainer = Ext.create('Ext.Panel', {
        //title: 'Messages',
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:0;border:0;background-color:transparent;',
        width: 300,
        layout: 'column', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: false,
        defaultType: 'textfield',
        items: [
           reeferHstDateFrom,
           reeferHstTimeFrom,
           reeferHstDateTo,
           reeferHstTimeTo, reeferHstVehicles]
    });

    var reeferHstFormFieldContainer = Ext.create('Ext.Panel', {
        //title: 'Messages',
        labelWidth: 0,
        border: 0,
        frame: false,
        bodyStyle: 'padding:0;border:0;background-color:transparent;',
        width: 630,
        layout: 'column', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: false,
        defaultType: 'textfield',
        items: [reeferHstDateTimeContainer, reeferHstType, reeferHstHiddenFleet,
                txtButtonTitle,
                LoadVehiclesBasedOn == 'fleet' ? reeferHstFleetButton : reeferHstOrganizationHierarchy,
                reeferHstCommModes/*, btnSubmit*/]
    });

    var reeferHsttabs = Ext.create('Ext.tab.Panel',
       {
           region: 'center', // a center region is ALWAYS required for border layout
           deferredRender: false,
           titleCollapse: false,
           autoScroll: true,
           border: false,
           width: 300,
           height: 180,

           autoHeight: true,
           collapsible: false,
           animCollapse: true,
           autoDestroy: false,
           hidden: true,
           activeTab: 0,     // first tab initially active
           items: [reeferHstMessageForm, reeferHstLoactionForm, reeferHstTripRadios],
           listeners:
            {
                tabchange: function (tabPanel, newCard, oldCard, eOpts) {
                    if (newCard.id == 'historyLoactionForm') {
                        reeferHstByLocation.setValue('1');
                    }
                    else {
                        reeferHstByLocation.setValue('0');
                    }
                }
            }

       }
       );

    var reeferHstForm = Ext.create('Ext.form.Panel', {
        title: '',
        labelWidth: 50, // label settings here cascade unless overridden
        url: './historynew/historyservices_Reefer.aspx?st=gethistoryrecords&reeferScreenName=reefer',
        frame: true,
        bodyStyle: 'padding:5px 5px 0;',
        width: 630,
        layout: 'column', // arrange fieldsets side by side
        defaults: {
            width: 240,
            labelWidth: 90
        },
        //margin: '10px 0',
        header: false,
        defaultType: 'textfield',
        items: [reeferHstByLocation, reeferHstFormFieldContainer, reeferHsttabs]
    });

    var exportReeferHistoryToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportReeferHistoryToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = reeferHstgrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "csv";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


    var exportReeferHistoryToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportReeferHistoryToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = reeferHstgrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);

               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2003";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }


    var exportReeferHistoryToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportReeferHistoryToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = reeferHstgrid;
               var config = {};
               var formatter = 'json'

               var data = Ext.ux.exporter.Exporter.exportAny(component, formatter, config);
               //var ed = eval('(' + data + ')');
               //alert(ed.Header);
               //alert(data);
               var id, frame, form, hidden, callback;

               frame = Ext.fly('exportframe').dom;
               frame.src = Ext.SSL_SECURE_URL;

               form = Ext.fly('exportform').dom;

               document.getElementById('exportdata').value = encodeURIComponent(data);
               document.getElementById('filename').value = "vehicles";
               document.getElementById('formatter').value = "excel2007";
               //alert('ok');
               form.submit();

           }
           catch (err) {
               alert(err);
           }
       }
   }

   var reeferHistoryExportMenu = Ext.create('Ext.menu.Menu');

   reeferHistoryExportMenu.add(exportReeferHistoryToCsvButton, exportReeferHistoryToExcel2003Button, exportReeferHistoryToExcel2007Button);

    reeferHstGridForm = Ext.create('Ext.Panel', {
        id: 'reeferHstGridForm',
        frame: false,
        border: 0,
        title: 'Reefer History',
        header: false,
        bodyPadding: 0,
        margin: '5px',
        width: 750,
        closable: false,
        autoHeight: true,
        autoScroll: true,
        layout: 'anchor',    // Specifies that the items will now be arranged in columns

        fieldDefaults: {
            labelAlign: 'left',
            msgTarget: 'side'
        },

        items: [btnReeferHstSearch, btnReeferHstMapit, btnReeferHstMapAll, btnReeferHstSubmit,         
        btnReeferHstLegend, reeferHstForm]
           , listeners: {
               'activate': function (grid, eOpts) {

                   if (reeferHstAddressResetField) {
                       reeferHstAddressResetField = false;
                       SelectedFleetId = reeferHstAddressFleetId;
                       if (reeferHstAddressFleetId == reeferHstHiddenFleet.getValue() && reeferHstVehicleStoreLoaded) {

                           reeferHstVehicles.setValue(reeferHstIniVehicleId);
                           reeferHstIniVehicleId = '';
                       }
                       else if (reeferHstVehicleStoreLoaded) {
                           reeferHstVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: reeferHstAddressFleetId
                                    }
                                }
                            );
                       }

                       if (LoadVehiclesBasedOn == 'fleet') {
                           reeferHstFleetButton.setText('All Vehicles');
                           reeferHstHiddenFleet.setValue(reeferHstAddressFleetId);
                       }
                       else {
                           reeferHstOrganizationHierarchy.setText('All Vehicles');
                           reeferHstHiddenFleet.setValue(reeferHstAddressFleetId);
                       }

                       reeferHstForm.show();
                       removeHistoriesOnMap(mapframe);
                       reeferHststore.removeAll();
                       reeferHstPageStore.removeAll();
                       reeferHstStopStore.removeAll();

                       return;
                   }
                   if (reeferHstIni) {
                       reeferHstIni = false;

                       if (LoadVehiclesBasedOn == 'fleet') {
                           //historyFleetId = SelectedFleetId;
                           //historyFleetName = SelectedFleetName;
                           reeferHstFleetButton.setText(SelectedFleetName);
                           reeferHstHiddenFleet.setValue(SelectedFleetId);
                       }
                       else {
                           //HistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
                           //HistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
                           reeferHstOrganizationHierarchy.setText(organizationHierarchy.getText());
                           reeferHstHiddenFleet.setValue(DefaultOrganizationHierarchyFleetId);
                       }


                       reeferHstForm.show();
                       removeHistoriesOnMap(mapframe);
                       reeferHststore.removeAll();
                       reeferHstPageStore.removeAll();
                       reeferHstStopStore.removeAll();
                   }
                   if (reeferHstIniVehicleId != '' && reeferHstVehicleStoreLoaded) {
                       if (LoadVehiclesBasedOn == 'fleet') {
                           if (reeferHstFleetId == SelectedFleetId) {
                               reeferHstVehicles.setValue(reeferHstIniVehicleId);
                               reeferHstIniVehicleId = '';
                           }
                           else {
                               reeferHstFleetId = SelectedFleetId;
                               reeferHstFleetName = SelectedFleetName;
                               reeferHstFleetButton.setText(reeferHstFleetName);
                               reeferHstHiddenFleet.setValue(reeferHstFleetId);

                               reeferHstVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: reeferHstFleetId
                                    }
                                }
                            );
                           }
                       }
                       else {
                           if (ReeferHistoryOrganizationHierarchyFleetId == DefaultOrganizationHierarchyFleetId) {
                               reeferHstVehicles.setValue(historyIniVehicleId);
                               reeferHstIniVehicleId = '';
                           }
                           else {
                               ReeferHistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
                               ReeferHistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
                               reeferHstOrganizationHierarchy.setText(organizationHierarchy.getText());
                               reeferHstHiddenFleet.setValue(ReeferHistoryOrganizationHierarchyFleetId);

                               reeferHstVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: ReeferHistoryOrganizationHierarchyFleetId
                                    }
                                }
                            );
                           }
                       }
                   }
               },
               'close': function (panel, eOpts) {
                   removeHistoriesOnMap(mapframe);
                   reeferHststore.removeAll();
                   reeferHstPageStore.removeAll();
                   reeferHstStopStore.removeAll();
               }
           }
    });

    var reeferHstgridColumns = [];

    reeferHstgridColumns.push({
        header: 'Reefer #', dataIndex: 'Description', align: 'left',
        width: 120,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'BoxId', dataIndex: 'BoxId', align: 'left',
        width: 70,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Date/Time', dataIndex: 'OriginDateTime', align: 'left',
        align: 'left',
        width: 150,
        xtype: 'datecolumn',
        format: userdateformat, //'d/m/Y h:i:s a',
        filterable: true,
        sortable: true,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Micro', dataIndex: 'Micro', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Location', dataIndex: 'StreetAddress', align: 'left',
        align: 'left',
        width: 230,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Tether', dataIndex: 'Tether', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Power', dataIndex: 'Power', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'ReeferState', dataIndex: 'ReeferState', align: 'left',
        align: 'left',
        width: 120,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Mode Of Op.', dataIndex: 'ModeOfOp', align: 'left',
        align: 'left',
        width: 80,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'S-Door', dataIndex: 'Door', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'AFAX', dataIndex: 'AFAX', align: 'left',
        align: 'left',
        width: 80,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Setpt.', dataIndex: 'Setpt', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Ret.', dataIndex: 'Ret', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Dis.', dataIndex: 'Dis', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'SenPro.', dataIndex: 'SensorProbe', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false,
        renderer: function (value, p, record) {
            var returnvalue = '-';
            var v = value;
            if ($.isNumeric(v)) {
                v = v * 1;
                //returnvalue = (v * 1).toFixed(2);
                if (v >= 3276.7 || v <= -3276.8) {
                    returnvalue = '-';
                }
                else {
                    //returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                    if (TemperatureType == "Fahrenheit")
                        returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                    else
                        returnvalue = (v * 10 / 32).toFixed(2);
                }
            }

            return returnvalue;
        }
    });
    reeferHstgridColumns.push({
        header: 'Amb.', dataIndex: 'Amb', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    if (DisplayZone2_3Temperatures == '1') {
        reeferHstgridColumns.push({
            header: 'Setpt2.', dataIndex: 'Setpt2', align: 'left',
            align: 'left',
            width: 60,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        });
        reeferHstgridColumns.push({
            header: 'Ret2.', dataIndex: 'Ret2', align: 'left',
            align: 'left',
            width: 60,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        });
        reeferHstgridColumns.push({
            header: 'Dis2.', dataIndex: 'Dis2', align: 'left',
            align: 'left',
            width: 60,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        });
        reeferHstgridColumns.push({
            header: 'SenPro2.', dataIndex: 'SensorProbe2', align: 'left',
            align: 'left',
            width: 60,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false,
            renderer: function (value, p, record) {
                var returnvalue = '-';
                var v = value;
                if ($.isNumeric(v)) {
                    v = v * 1;
                    //returnvalue = (v * 1).toFixed(2);
                    if (v >= 3276.7 || v <= -3276.8) {
                        returnvalue = '-';
                    }
                    else {
                        //returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                        if (TemperatureType == "Fahrenheit")
                            returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                        else
                            returnvalue = (v * 10 / 32).toFixed(2);
                    }
                }

                return returnvalue;
            }
        });

        reeferHstgridColumns.push({
            header: 'Setpt3.', dataIndex: 'Setpt3', align: 'left',
            align: 'left',
            width: 60,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        });
        reeferHstgridColumns.push({
            header: 'Ret3.', dataIndex: 'Ret3', align: 'left',
            align: 'left',
            width: 60,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        });
        reeferHstgridColumns.push({
            header: 'Dis3.', dataIndex: 'Dis3', align: 'left',
            align: 'left',
            width: 60,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        });
        reeferHstgridColumns.push({
            header: 'SenPro3.', dataIndex: 'SensorProbe3', align: 'left',
            align: 'left',
            width: 60,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false,
            renderer: function (value, p, record) {
                var returnvalue = '-';
                var v = value;
                if ($.isNumeric(v)) {
                    v = v * 1;
                    //returnvalue = (v * 1).toFixed(2);
                    if (v >= 3276.7 || v <= -3276.8) {
                        returnvalue = '-';
                    }
                    else {
                        // returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                        if (TemperatureType == "Fahrenheit")
                            returnvalue = ((v * 10 / 32) * 9 / 5 + 32).toFixed(2);
                        else
                            returnvalue = (v * 10 / 32).toFixed(2);
                    }
                }

                return returnvalue;
            }
        });
    }
    reeferHstgridColumns.push({
        header: 'Fuel Level(%)', dataIndex: 'FuelLevel', align: 'left',
        align: 'left',
        width: 80,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Fuel Level(gal)', dataIndex: 'FuelLevel', align: 'left',
        align: 'left',
        width: 80,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false,
        renderer: function (value, p, record) {
            var returnvalue = value;
            if ($.isNumeric(value)) {
                returnvalue = (value * 450 / 100).toFixed(0); //based on 450 gallon tank size
            }
            return returnvalue;
        }
    });
    reeferHstgridColumns.push({
        header: 'Eng. Hrs', dataIndex: 'EngineHours', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'Controller', dataIndex: 'ControllerType', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'RPM', dataIndex: 'RPM', align: 'left',
        align: 'left',
        width: 50,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    reeferHstgridColumns.push({
        header: 'BatteryV', dataIndex: 'BatteryVolt', align: 'left',
        align: 'left',
        width: 60,
        filterable: true,
        sortable: false,
        // flex : 1,
        hidden: false
    });
    
    reeferHstgrid = Ext.create('Ext.grid.Panel',
   {
       id: 'reeferHstgrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       //maxWidth: window.screen.width - 50,
       //maxHeight: window.screen.height,
       anchor: '-10, -45',
       autoHeight: true,
       stateful: true,
       closable: false,
       enableColumnHide: false,
       enableSorting: false,

       title: '',
       store: reeferHstPageStore,
       columnLines: true,
       stateId: 'stateReeferHistoryGrid',

       enableColumnHide: true,
       stateful: false,

       collapsible: false,
       animCollapse: true,
       split: true,
       features: [filters],
       margin: '0',
       //selModel: selReeferHstModel,

       viewConfig:
      {
          emptyText: 'No Reefer History to display',
          useMsg: false
      }
      ,
       columns: reeferHstgridColumns
       ,
       dockedItems: {
           xtype: 'toolbar',
           dock: 'top',
           items: [
                { icon: 'preview.png',
                    cls: 'x-btn-text-icon',
                    text: 'Export',
                    menu: reeferHistoryExportMenu
                }
           ]
       },
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       bbar: reeferHstPager
   }
   );

    reeferHstStopGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'reeferHstStopGrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width - 50,
       maxHeight: window.screen.height,

       autoHeight: true,
       stateful: true,
       closable: false,
       enableColumnHide: false,
       enableSorting: false,

       title: '',
       store: reeferHstStopStore,
       columnLines: true,
       stateId: 'stateReeferHistoryStopGrid',

       enableColumnHide: true,
       stateful: false,

       collapsible: false,
       animCollapse: true,
       split: true,
       features: [filters],
       margin: '0',
       selModel: selReeferHstStopModel,

       viewConfig:
      {
          emptyText: 'No Reefer History to display',
          useMsg: false
      }
      ,
       columns: [
        { header: 'Arrival', dataIndex: 'ArrivalDateTime', align: 'left',
            xtype: 'datecolumn',
            format: 'd/m/Y h:i:s a',
            width: 150,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Address', dataIndex: 'Location',
            align: 'left',
            width: 220,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        { header: 'Departure', dataIndex: 'DepartureDateTime', align: 'left',
            xtype: 'datecolumn',
            format: 'd/m/Y h:i:s a',
            width: 150,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Duration', dataIndex: 'StopDuration',
            align: 'left',
            width: 80,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Status', dataIndex: 'Remarks',
            align: 'left',
            width: 80,
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        }
      ],
       bbar: ['->', 'Total Reefer Histories: <span id="stopreeferhistoriescount" style="margin-right:20px;">0</span>']
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       //bbar: historyPager
   }
   );

    reeferHstTripGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'reeferHstTripGrid',
       animCollapse: false,
       autoLoad: false,
       autoScroll: true,
       loadMask: true,
       maxWidth: window.screen.width - 50,
       //maxHeight: window.screen.height,

       autoHeight: true,
       stateful: true,
       closable: false,
       enableColumnHide: false,
       enableSorting: false,

       title: '',
       store: reeferHstTripStore,
       columnLines: true,
       stateId: 'stateReeferHistoryTripGrid',

       enableColumnHide: true,
       stateful: false,

       collapsible: false,
       animCollapse: true,
       split: true,
       features: [filters],
       margin: '0',
       selModel: {
           selType: 'cellmodel'
       },
       plugins: [{
           ptype: 'rowexpander',
           rowBodyTpl: [
                '<div id="tripsummary{TripId}">',
                '</div>'
            ]
       }],

       viewConfig:
      {
          emptyText: 'No Reefer History to display',
          useMsg: false
      }
      ,
       columns: [
        { header: 'Description', dataIndex: 'Description', align: 'left',
            width: 150,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'Departure', dataIndex: 'DepartureTime', align: 'left',
            xtype: 'datecolumn',
            format: 'd/m/Y h:i:s a',
            width: 150,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'Arrival', dataIndex: 'ArrivalTime', align: 'left',
            xtype: 'datecolumn',
            format: 'd/m/Y h:i:s a',
            width: 150,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'From', dataIndex: '_From', align: 'left',
            width: 200,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'To', dataIndex: '_To', align: 'left',
            width: 200,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Duration', dataIndex: 'Duration',
            align: 'left',
            width: 80,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Fuel Consumed', dataIndex: 'FuelConsumed',
            align: 'left',
            width: 80,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        }
      ]
       //,bbar: ['->', 'Total Histories: <span id="stophistoriescount" style="margin-right:20px;">0</span>']
       //,bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       //,bbar: historyPager
   }
   );



    reeferHstTripGrid.view.on('expandBody', function (rowNode, record, expandRow, eOpts) {
        displayInnerGrid(record.get('TripId'), record);
    });

    reeferHstTripGrid.view.on('collapsebody', function (rowNode, record, expandRow, eOpts) {
        destroyInnerGrid(record);
    });

    tabs.add(reeferHstGridForm); 

}
  

function mapReeferHistories(selections, isInitial, s, stopreeferHst) {
    zoomtomap = true;

    try {
        var mapreeferHstJsonData = new Array();
        if (s) {
            selections = Ext.Array.sort(selections, function (r1, r2) {
                if (stopreeferHst) {
                    if (r1.data.StopIndex < r2.data.StopIndex)
                        return 1;
                    if (r1.data.StopIndex > r2.data.StopIndex)
                        return -1;
                    return 0;
                }
                else {
                    if (r1.data.OriginDateTime < r2.data.OriginDateTime)
                        return 1;
                    if (r1.data.OriginDateTime > r2.data.OriginDateTime)
                        return -1;
                    return 0;
                }

            });
        }
        if (selections.length > 0) {

            Ext.each(selections, function (exirecord) {

                if (isNumber(exirecord.data.Latitude) && isNumber(exirecord.data.Longitude) && (exirecord.data.Latitude != 0 || exirecord.data.Longitude != 0)) {

                    /*var today = new Date();
                    var posExpireDate = new Date();
                    posExpireDate.setTime(today.getTime() - (60 * PositionExpiredTime * 1000));*/

                    if (exirecord.data.icon == undefined || exirecord.data.icon == '') {
                        var newIcon = "";
                        if (stopreeferHst) {
                            if (exirecord.data.Remarks == 'Idling')
                                newIcon = "../Bigicons/Idle.ico";
                            else if (exirecord.data.StopDurationVal / 60 < 15)
                                newIcon = "../Bigicons/Stop_3.ico";
                            else if (exirecord.data.StopDurationVal / 60 < 60)
                                newIcon = "../Bigicons/Stop_15.ico";
                            else
                                newIcon = "../Bigicons/Stop_60.ico";

                            var dthis = new Date(exirecord.data.ArrivalDateTime);
                            var newDthis = Ext.Date.format(dthis, 'd/m/Y h:i:s a');
                            exirecord.data.convertedArrivalDisplayDate = newDthis;
                        }
                        else {
                            if (exirecord.data.Speed != 0) {
                                newIcon = "Green" + IconTypeName + exirecord.data.MyHeading + ".ico";
                            }
                            else {
                                newIcon = "Red" + IconTypeName + ".ico";
                            }
                        }
                        exirecord.data.icon = newIcon;
                    }

                    var dt = new Date(exirecord.data.OriginDateTime);
                    var newDt = Ext.Date.format(dt, 'd/m/Y h:i a');
                    exirecord.data.convertedDisplayDate = newDt;


                    mapreeferHstJsonData.push(exirecord.data);
                }

            }
                        );

        }

        if (mapreeferHstJsonData.length > 0) {
            if (isInitial) {
                ShowreeferHstMapFrameData(mapreeferHstJsonData, true, mapframe, zoomtomap);
            }
            else {
                ShowreeferHstMapFrameData(mapreeferHstJsonData, false, mapframe, zoomtomap);
            }
        }
        else {
            removeHistoriesOnMap(mapframe);
        }

    }
    catch (err) {
    }
}


function showReeferHistoryTab(VehicleId, setDateTime, fleetId) {
    reeferHstIniVehicleId = VehicleId;
    reeferHstIni = true;
    
    
    //tabs.add(historyGridForm);
    tabs.setActiveTab(reeferHstGridForm);
}