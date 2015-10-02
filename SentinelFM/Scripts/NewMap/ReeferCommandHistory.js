var commandHststore;
var commandHstPageStore;
var commandHstgrid;
var commandHstGridForm;

var commandHstStopStore;
var commandHstStopGrid;

var commandHstTripStore;
var commandHstTripGrid;

var commandHstIni = false;
var commandHstIniVehicleId = '';
var commandHstVehicleStoreLoaded = false;
var commandHstFleetId = '';
var commandHstFleetName = '';

var commandHstAddressStore;
var commandHstAddressGrid;
var commandHstAddressResetField;
var commandHstAddressFleetId;
var commandHstAddressFleetName;

var commandHstDateFrom;
var commandHstTimeFrom
var commandHstDateTo;
var commandHstTimeTo;

var commandHstOrganizationHierarchy;

var commandHstVehicleStore;
var commandHstHiddenFleet;
var fleetWin;

var commandHstTripsNum = 0;

var AllCommandHstRecords = [];

var commandHstPagerDoc = '';

var commandHstPage = './commandHst/frmhistmain_new.aspx?VehicleId=';
var commandHstFleetButton;

function IniReeferCommandHistory() {
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

    commandHststore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: HistoryPagesize,
       storeId: 'commandHststore',
       model: 'HistoryListModel',

       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  AllCommandHstRecords = records;
                  //setTimeout(function () { mapCommandHistories(records, true, false, false); }, 500);
                  commandHstgrid.getSelectionModel().selectAll(false);
                  $('#commandhistoriescount').html(records.length);
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

    commandHstPageStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: HistoryPagesize,
       storeId: 'commandHstPageStore',
       model: 'HistoryListModel',
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

    commandHstStopStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'commandHstStopStore',
       model: 'HistoryStopModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {
                  //mapCommandHistories(records, true, false, true);
                  commandHstStopGrid.getSelectionModel().selectAll(false);

                  $('#stopcommandhistoriescount').html(records.length);
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

    commandHstTripStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'commandHstTripStore',
       model: 'HistoryTripModel',
       autoLoad: false,
       listeners:
      {
          'load': function (store, records, options) {
              try {

                  commandHstTripsNum = records.length;
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

    commandHstAddressStore = Ext.create('Ext.data.Store',
   {
       //buffered: true,
       pageSize: 10000,
       storeId: 'commandHstAddressStore',
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

 var exportReeferCommandHistoryToCsvButton =
   {
       text: 'Export To CSV',
       id: 'exportReeferCommandHistoryToCsvButton',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = commandHstgrid;
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


 var exportReeferCommandHistoryToExcel2003Button =
   {
       text: 'Export To Excel2003',
       id: 'exportReeferCommandHistoryToExcel2003Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = commandHstgrid;
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


 var exportReeferCommandHistoryToExcel2007Button =
   {
       text: 'Export To Excel2007',
       id: 'exportReeferCommandHistoryToExcel2007Button',
       tooltip: 'Export',
       iconCls: 'map',
       cls: 'cmbfonts',
       textAlign: 'left',
       handler: function () {
           try {

               var component = commandHstgrid;
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

 var reeferCommandHistoryExportMenu = Ext.create('Ext.menu.Menu');

 reeferCommandHistoryExportMenu.add(exportReeferCommandHistoryToCsvButton, exportReeferCommandHistoryToExcel2003Button, exportReeferCommandHistoryToExcel2007Button);

    commandHstOrganizationHierarchy = Ext.create('Ext.Button',
   {
       text: DefaultOrganizationHierarchyFleetName == '' ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName,
       id: 'commandHstOrganizationHierarchyButton',
       tooltip: 'Organization Hierarchy',
       cls: 'cmbfonts',
       textAlign: 'left',
       width: 230,
       style: { marginLeft: '1px' },
       handler: function () {
           try {
               var mypage = '../../Widgets/OrganizationHierarchy.aspx?nodecode=' + commandHstOrganizationHierarchyNodeCode;
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

    commandHstFleetButton = Ext.create('Ext.Button',
   {
       text: SelectedFleetName == '' ? SelectedFleetId : SelectedFleetName,
       id: 'commandHstFleetButton',
       tooltip: 'Select a fleet',
       cls: 'cmbfonts',
       icon: 'preview.png',
       textAlign: 'left',
       width: 230,
       style: { marginLeft: '5px' },
       handler: function () {
           try {
               var url = "./Widgets/fleet.aspx?fleetId=" + commandHstFleetId + '&f=commandHstFleetButton';
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

    var commandHstPager = new Ext.PagingToolbar(
   {
       store: commandHstPageStore,
       displayInfo: true,
       displayMsg: 'Displaying command histories {0} - {1} of {2}',
       emptyMsg: "No command history to display",
       listeners: {
           beforechange: function (b, page, o) {

               try {
                   if (commandHstPagerDoc != '') {
                       commandHstPagerDoc = '';
                       return;
                   }
                   if (commandHstVehicles.getValue() == null || commandHstVehicles.getValue() == '-1') {
                       Ext.Msg.alert('Oops', 'Please select a vehicle...');
                       return;
                   }

                   commandHstgrid.getView().emptyText = 'loading...';

                   //var form = btnSubmit.up('form').getForm();
                   var form = commandHstForm.getForm();
                   if (form.isValid()) {
                       form.submit({
                           url: './historynew/historyservices_Reefer.aspx?fromsession=1&st=gethistoryrecords&reeferScreenName=command&start=' + (page - 1) * HistoryPagesize + '&limit=' + HistoryPagesize,
                           success: function (form, action) {
                               commandHstgrid.getView().emptyText = 'No Command History to display';
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
                               commandHstPageStore.loadRawData(doc);
                               commandHstForm.hide();
                           },
                           failure: function (form, action) {
                               commandHstgrid.getView().emptyText = 'No Command History to display';
                               //Ext.Msg.alert('Failed', 'some error');
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

    var selCommandHstModel = Ext.create('Ext.selection.CheckboxModel',
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

    var selCommandHstStopModel = Ext.create('Ext.selection.CheckboxModel',
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
    * Code of command history grid with form
    *
    */


    /*
    * Here is where we create the Form
    */
    commandHstDateFrom = Ext.create('Ext.form.field.Date', {
        anchor: '100%',
        labelWidth: 50,
        maxWidth: 190,
        fieldLabel: 'From',
        name: 'historyDateFrom',
        format: userDate,
        value: new Date()
    });
    commandHstTimeFrom = Ext.create('Ext.form.field.Time', {
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
    commandHstDateTo = Ext.create('Ext.form.field.Date', {
        anchor: '100%',
        labelWidth: 50,
        maxWidth: 190,
        fieldLabel: 'To',
        name: 'historyDateTo',
        format: userDate,
        //value: (new Date()).getDate() + 1
        value: new Date((new Date()).getFullYear(), (new Date()).getMonth(), (new Date()).getDate() + 1)
    });
    commandHstTimeTo = Ext.create('Ext.form.field.Time', {
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

    

    var btnCommandHstSearch = Ext.create('Ext.Button',
       {
           text: 'Advanced Search',
           id: 'btmCommandHstSearch',
           tooltip: 'Search Command History',
           iconCls: 'map',
           cls: 'cmbfonts',
           textAlign: 'left',
           handler: function () {
               try {
                   if (commandHstForm.isHidden())
                       commandHstForm.show();
                   else
                       commandHstForm.hide();

               }
               catch (err) {
               }
           }
       }
       );

    var btnCommandHstMapit = Ext.create('Ext.Button',
       {
           text: 'Map It',
           id: 'btnCommandHstMapit',
           tooltip: 'Map the selected command history records',
           iconCls: 'map',
           margin: '0 5',
           cls: 'cmbfonts',
           textAlign: 'left',
           hidden: true,
           handler: function () {
               try {
                   var commandHsttype = commandHstType.getValue();
                   if (commandHsttype == 0) {
                       selections = commandHstgrid.getSelectionModel().getSelection();
                       mapCommandHistories(selections, true, true, false);
                   }
                   else if (commandHsttype == 1 || commandHsttype == 2 || commandHsttype == 3) {
                       selections = commandHstStopGrid.getSelectionModel().getSelection();
                       mapCommandHistories(selections, true, true, true);
                   }
                   else if (commandHsttype == 4) {
                       var selections = [];
                       for (itrip = 0; itrip < commandHstTripsNum; itrip++) {
                           var innerGrid = Ext.getCmp('commandHstDetailsGrid' + itrip);
                           if (innerGrid) {
                               var ss = innerGrid.getSelectionModel().getSelection();
                               selections = selections.concat(ss);
                           }
                       }
                       //var innerGrid = Ext.getCmp('mapCommandHistoriesDetailsGrid' + '0');
                       //var selections = innerGrid.getSelectionModel().getSelection();
                       mapCommandHistories(selections, true, true, false);
                   }

               }
               catch (err) {
                   var i = 0;
               }
           }
       }
       );


    var btnCommandHstLegend = Ext.create('Ext.Button',
       {
           text: 'Map Legend',
           id: 'btnCommandHstLegend',
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

    var btnCommandHstMapAll = Ext.create('Ext.Button',
       {
           text: 'Map All',
           id: 'btnCommandHstMapAll',
           iconCls: 'map',
           margin: '0 5',
           cls: 'cmbfonts',
           textAlign: 'left',
           hidden: true,
           handler: function () {
               try {
                   if (AllCommandHstRecords.length > 0)
                       mapCommandHistories(AllCommandHstRecords, true, false, false);
               }
               catch (err) {
               }
           }
       }
       );

    var commandHstType_values = [
            [0, 'Vehicle Path'],
            [1, 'Stop and Idle Sequence'],
            [2, 'Stop Sequence'],
            [3, 'Idle Sequence'],
            [4, 'Trip Report']
        ];

    var commandHstType_store = new Ext.data.SimpleStore({
        fields: ['number', 'histype'],
        data: commandHstType_values
    });


    var commandHstType = new Ext.form.ComboBox({
        name: 'historyType',
        fieldLabel: 'Type',
        hiddenName: 'historyType',
        store: commandHstType_store,
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
                                 btnCommandHstMapAll.show();
                                 btnCommandHstLegend.hide();
                                 commandHsttabs.setActiveTab(commandHstMessageForm);
                             }
                             else if (selectedtype == 1 || selectedtype == 2 || selectedtype == 3) {
                                 btnCommandHstLegend.show();
                                 btnCommandHstMapAll.hide();
                                 commandHsttabs.setActiveTab(commandHstMessageForm);
                             }
                             else {
                                 btnCommandHstLegend.hide();
                                 btnCommandHstMapAll.hide();
                                 commandHsttabs.setActiveTab(commandHstTripRadios);
                             }
                         }
                         catch (err) {
                         }
                     }
                 }
    });

    commandHstHiddenFleet = Ext.create('Ext.form.field.Hidden',
            {
                name: 'historyFleet',
                value: LoadVehiclesBasedOn == 'fleet' ? SelectedFleetId : DefaultOrganizationHierarchyFleetId
            }
        );
                    
    commandHstVehicleStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryVehicleList',
               autoLoad: false,
               storeId: 'commandHstVehicleStore',
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

                       if (commandHstIniVehicleId != '') {
                           commandHstVehicles.setValue(commandHstIniVehicleId);
                           commandHstIniVehicleId = '';
                       }
                       else
                           commandHstVehicles.setValue('-1');

                       commandHstVehicleStoreLoaded = true;
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
             
             
    var commandHstVehicles = Ext.create('Ext.form.ComboBox',
       {
           name: 'historyVehicle',
           store: 'commandHstVehicleStore',
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
                      commandHstCommModeStore.load(
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
                      commandHstVehicleStore.load(
                        {
                            params:
                                {
                                    fleetID: SelectedFleetId
                                }
                        }
                      );
                  }
                  else {
                      commandHstVehicleStore.load(
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
       
    var commandHstCommModeStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryCommModeList',
               autoLoad: false,
               storeId: 'commandHstCommModeStore',
               listeners:
               {
                   'load': function (xstore, records, options) {

                       var u = Ext.create('HistoryCommModeList', {
                           DclId: '-1',
                           CommModeName: 'ALL'
                       });
                       xstore.insert(0, u);

                       commandHstCommModes.setValue('-1');
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
    var commandHstCommModes = Ext.create('Ext.form.ComboBox',
       {
           name: 'historyCommMode',
           store: 'commandHstCommModeStore',
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
                  commandHstCommModeStore.load();
              }
          }
       }
       );

    var commandHstMessageCheckBox = Ext.create('Ext.form.field.Checkbox',
           {
               boxLabel: 'Last message only',
               name: 'lastmessageonly',
               inputValue: '1',
               id: 'checkbox1',
               border: 0
           }
       );

    var commandHstMessageStore = Ext.create('Ext.data.Store',
           {
               model: 'HistoryMessageModel',
               autoLoad: false,
               storeId: 'commandHstMessageStore',
               listeners:
               {
                   'load': function (xstore, records, options) {
                       var u = Ext.create('HistoryMessageModel', {
                           BoxMsgInTypeId: '-1',
                           BoxMsgInTypeName: 'All Messages'
                       });
                       xstore.insert(0, u);

                       commandHstMessageList.setValue('-1');
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

    var commandHstMessageList = Ext.create('Ext.ux.form.MultiSelect',
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

                store: commandHstMessageStore,
                ddReorder: false,
                listeners:
                  {
                      scope: this,
                      'afterrender': function () {
                          commandHstMessageStore.load();
                      }
                  }
            }
        );

    var commandHstMessageForm = Ext.create('Ext.Panel', {
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
        items: [commandHstMessageCheckBox, commandHstMessageList]
    });

    var commandHstLocationText = Ext.create('Ext.form.field.Text', {
        name: 'historyLocation',
        labelWidth: 50,
        fieldLabel: 'Address',
        allowBlank: true  // requires a non-empty value
    });

    var commandHstByLocation = Ext.create('Ext.form.field.Hidden', {
        name: 'historyByLocation',
        value: '0'
    });

    var commandHstLoactionForm = Ext.create('Ext.Panel', {
        id: 'commandHstLoactionForm',
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
        items: [commandHstLocationText]
    });


    var commandHstTripRadios = Ext.create('Ext.Panel', {
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

    var btnCommandHstSubmit = Ext.create('Ext.Button', {
        text: 'View',
        cls: 'cmbfonts',
        //margin: '10 auto',
        style: { margin: '10px 0 10px 55px' },
        width: 100,
        handler: function () {
            try {
                if (commandHstVehicles.getValue() == null || commandHstVehicles.getValue() == '-1') {
                    Ext.Msg.alert('Oops', 'Please select a vehicle...');
                    return;
                }
                //var form = this.up('form').getForm();
                var form = commandHstForm.getForm();

                var commandHsttype = commandHstType.getValue();
                
                if (commandHsttype == 0) {
                    commandHstGridForm.remove(commandHstStopGrid, false);
                    commandHstGridForm.remove(commandHstTripGrid, false);
                    commandHststore.removeAll();
                    commandHstPageStore.removeAll();
                    commandHstGridForm.add(commandHstgrid);
                }
                else if (commandHsttype == 1 || commandHsttype == 2 || commandHsttype == 3) {
                    commandHstGridForm.remove(commandHstgrid, false);
                    commandHstGridForm.remove(commandHstTripGrid, false);
                    commandHstStopStore.removeAll();
                    commandHstGridForm.add(commandHstStopGrid);
                }
                else if (commandHsttype == 4) {
                    commandHstGridForm.remove(commandHstgrid, false);
                    commandHstGridForm.remove(commandHstStopGrid, false);
                    commandHstTripStore.removeAll();
                    commandHstGridForm.add(commandHstTripGrid);
                }
                commandHstGridForm.doLayout();

                if (form.isValid()) {
                    loadingMask.show();
                    form.submit({
                        url: './historynew/historyservices_Reefer.aspx?st=gethistoryrecords&reeferScreenName=command&start=0&limit=' + HistoryPagesize,
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

                                if (commandHsttype == 0) {
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

                                    commandHststore.loadRawData(docwholedata);
                                    commandHstPagerDoc = '1';
                                    commandHstPager.moveFirst();
                                    commandHstPageStore.loadRawData(doc);
                                }
                                else if (commandHsttype == 1 || commandHsttype == 2 || commandHsttype == 3) {
                                    commandHstStopStore.loadRawData(doc);
                                }
                                else if (commandHsttype == 4) {
                                    commandHstTripStore.loadRawData(doc);

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

                                    commandHststore.loadRawData(doctripdata);
                                }

                                //historystore.loadRawData(doc);
                                commandHstForm.hide();
                                loadingMask.hide();
                            }
                            catch (error) {
                                commandHstForm.hide();
                                loadingMask.hide();
                            }

                        },
                        failure: function (form, action) {
                            loadingMask.hide();
                            //Ext.Msg.alert('Failed', 'some error');
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

    var commandHstDateTimeContainer = Ext.create('Ext.Panel', {
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
           commandHstDateFrom,
           commandHstTimeFrom,
           commandHstDateTo,
           commandHstTimeTo, commandHstVehicles]
    });

    var commandHstFormFieldContainer = Ext.create('Ext.Panel', {
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
        items: [commandHstDateTimeContainer, commandHstType, commandHstHiddenFleet,
                txtButtonTitle,
                LoadVehiclesBasedOn == 'fleet' ? commandHstFleetButton : commandHstOrganizationHierarchy,
                commandHstCommModes/*, btnSubmit*/]
    });

    var commandHsttabs = Ext.create('Ext.tab.Panel',
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
           items: [commandHstMessageForm, commandHstLoactionForm, commandHstTripRadios],
           listeners:
            {
                tabchange: function (tabPanel, newCard, oldCard, eOpts) {
                    if (newCard.id == 'historyLoactionForm') {
                        commandHstByLocation.setValue('1');
                    }
                    else {
                        commandHstByLocation.setValue('0');
                    }
                }
            }

       }
       );

    var commandHstForm = Ext.create('Ext.form.Panel', {
        title: '',
        labelWidth: 50, // label settings here cascade unless overridden
        url: './historynew/historyservices_Reefer.aspx?st=gethistoryrecords&reeferScreenName=command',
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
        items: [commandHstByLocation, commandHstFormFieldContainer, commandHsttabs]        
    });

    commandHstGridForm = Ext.create('Ext.Panel', {
        id: 'commandHstGridForm',
        frame: false,
        border: 0,
        title: 'Command History',
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

        items: [btnCommandHstSearch, btnCommandHstMapit, btnCommandHstMapAll, btnCommandHstSubmit, btnCommandHstLegend, commandHstForm]
           , listeners: {
               'activate': function (grid, eOpts) {

                   if (commandHstAddressResetField) {
                       commandHstAddressResetField = false;
                       SelectedFleetId = commandHstAddressFleetId;
                       if (commandHstAddressFleetId == commandHstHiddenFleet.getValue() && commandHstVehicleStoreLoaded) {

                           commandHstVehicles.setValue(commandHstIniVehicleId);
                           commandHstIniVehicleId = '';
                       }
                       else if (commandHstVehicleStoreLoaded) {
                           commandHstVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: commandHstAddressFleetId
                                    }
                                }
                            );
                       }

                       if (LoadVehiclesBasedOn == 'fleet') {
                           commandHstFleetButton.setText('All Vehicles');
                           commandHstHiddenFleet.setValue(commandHstAddressFleetId);
                       }
                       else {
                           commandHstOrganizationHierarchy.setText('All Vehicles');
                           commandHstHiddenFleet.setValue(commandHstAddressFleetId);
                       }

                       commandHstForm.show();
                       removeHistoriesOnMap(mapframe);
                       commandHststore.removeAll();
                       commandHstPageStore.removeAll();
                       commandHstStopStore.removeAll();

                       return;
                   }
                   if (commandHstIni) {
                       commandHstIni = false;

                       if (LoadVehiclesBasedOn == 'fleet') {
                           //historyFleetId = SelectedFleetId;
                           //historyFleetName = SelectedFleetName;
                           commandHstFleetButton.setText(SelectedFleetName);
                           commandHstHiddenFleet.setValue(SelectedFleetId);
                       }
                       else {
                           //HistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
                           //HistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
                           commandHstOrganizationHierarchy.setText(organizationHierarchy.getText());
                           commandHstHiddenFleet.setValue(DefaultOrganizationHierarchyFleetId);
                       }


                       commandHstForm.show();
                       removeHistoriesOnMap(mapframe);
                       commandHststore.removeAll();
                       commandHstPageStore.removeAll();
                       commandHstStopStore.removeAll();
                   }
                   if (commandHstIniVehicleId != '' && commandHstVehicleStoreLoaded) {
                       if (LoadVehiclesBasedOn == 'fleet') {
                           if (commandHstFleetId == SelectedFleetId) {
                               commandHstVehicles.setValue(commandHstIniVehicleId);
                               commandHstIniVehicleId = '';
                           }
                           else {
                               commandHstFleetId = SelectedFleetId;
                               commandHstFleetName = SelectedFleetName;
                               commandHstFleetButton.setText(commandHstFleetName);
                               commandHstHiddenFleet.setValue(commandHstFleetId);

                               commandHstVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: commandHstFleetId
                                    }
                                }
                            );
                           }
                       }
                       else {
                           if (CommandHistoryOrganizationHierarchyFleetId == DefaultOrganizationHierarchyFleetId) {
                               commandHstVehicles.setValue(historyIniVehicleId);
                               commandHstIniVehicleId = '';
                           }
                           else {
                               CommandHistoryOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
                               CommandHistoryOrganizationHierarchyNodeCode = DefaultOrganizationHierarchyNodeCode;
                               commandHstOrganizationHierarchy.setText(organizationHierarchy.getText());
                               commandHstHiddenFleet.setValue(CommandHistoryOrganizationHierarchyFleetId);

                               commandHstVehicleStore.load(
                                {
                                    params:
                                    {
                                        fleetID: CommandHistoryOrganizationHierarchyFleetId
                                    }
                                }
                            );
                           }
                       }
                   }
               },
               'close': function (panel, eOpts) {
                   removeHistoriesOnMap(mapframe);
                   commandHststore.removeAll();
                   commandHstPageStore.removeAll();
                   commandHstStopStore.removeAll();
               }
           }
    });

    commandHstgrid = Ext.create('Ext.grid.Panel',
   {
       id: 'commandHstgrid',
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
       store: commandHstPageStore,
       columnLines: true,
       stateId: 'stateCommandHistoryGrid',

       enableColumnHide: true,
       stateful: false,

       collapsible: false,
       animCollapse: true,
       split: true,
       features: [filters],
       margin: '0',
       selModel: selCommandHstModel,

       viewConfig:
      {
          emptyText: 'No Command History to display',
          useMsg: false
      }
      ,
       columns: [
        { header: 'Unit ID', dataIndex: 'BoxId', align: 'left',
            width: 70,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        { header: 'Vehicle', dataIndex: 'Description', align: 'left',
            width: 120,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Date/Time', dataIndex: 'OriginDateTime', align: 'left',
            align: 'left',
            width: 150,
            xtype: 'datecolumn',
            format: userdateformat,//'d/m/Y h:i:s a',
            filterable: true,
            sortable: true,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Address', dataIndex: 'StreetAddress', align: 'left',
            align: 'left',
            width: 230,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Speed', dataIndex: 'Speed', align: 'left',
            align: 'left',
            width: 50,
            renderer: function (value, p, record) {
                if (value == -1)
                    return 'N/A';
                else
                    return value;
            },
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Message', dataIndex: 'BoxMsgInTypeName', align: 'left',
            renderer: function (value, p, record, ri) {
                //return '<a href="javascript:var w =HistoryInfo(' + ri + ')">' + value + '</a>';
                return '<a href="' + record.data.CustomUrl + '">' + value + '</a>';
            },
            align: 'left',
            width: 130,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            //header: 'MsgDetails', dataIndex: 'CustomProp', align: 'left',
            header: 'MsgDetails', dataIndex: 'MsgDetails', align: 'left',
            align: 'left',
            width: 130,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        },
        {
            header: 'Ack', dataIndex: 'Acknowledged', align: 'left',
            align: 'left',
            width: 50,
            filterable: true,
            sortable: false,
            // flex : 1,
            hidden: false
        }
      ]
       ,
       dockedItems: {
            xtype: 'toolbar',
            dock: 'top',
            items: [
                { icon: 'preview.png',
                    cls: 'x-btn-text-icon',
                    text: 'Export',
                    menu: reeferCommandHistoryExportMenu
                }
           ]
        },
       /*dockedItems: {
       //xtype: 'toolbar',
       dock: 'top',
       items: [historyGridForm]
       },*/
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       bbar: commandHstPager
   }
   );

    commandHstStopGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'commandHstStopGrid',
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
       store: commandHstStopStore,
       columnLines: true,
       stateId: 'stateCommandHistoryStopGrid',

       enableColumnHide: true,
       stateful: false,

       collapsible: false,
       animCollapse: true,
       split: true,
       features: [filters],
       margin: '0',
       selModel: selCommandHstStopModel,

       viewConfig:
      {
          emptyText: 'No Command History to display',
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
       bbar: ['->', 'Total Command Histories: <span id="stopcommandhistoriescount" style="margin-right:20px;">0</span>']
       //bbar: ['->', 'Total Histories: <span id="historiescount" style="margin-right:20px;">0</span>']
       //bbar: historyPager
   }
   );

    commandHstTripGrid = Ext.create('Ext.grid.Panel',
   {
       id: 'commandHstTripGrid',
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
       store: commandHstTripStore,
       columnLines: true,
       stateId: 'stateCommandHistoryTripGrid',

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
          emptyText: 'No Command History to display',
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



    commandHstTripGrid.view.on('expandBody', function (rowNode, record, expandRow, eOpts) {
        displayInnerGrid(record.get('TripId'), record);
    });

    commandHstTripGrid.view.on('collapsebody', function (rowNode, record, expandRow, eOpts) {
        destroyInnerGrid(record);
    });

    tabs.add(commandHstGridForm); 

}
  

function mapCommandHistories(selections, isInitial, s, stopcommandHst) {
    zoomtomap = true;

    try {
        var mapcommandHstJsonData = new Array();
        if (s) {
            selections = Ext.Array.sort(selections, function (r1, r2) {
                if (stopcommandHst) {
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
                        if (stopcommandHst) {
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


                    mapcommandHstJsonData.push(exirecord.data);
                }

            }
                        );

        }

        if (mapcommandHstJsonData.length > 0) {
            if (isInitial) {
                ShowcommandHstMapFrameData(mapcommandHstJsonData, true, mapframe, zoomtomap);
            }
            else {
                ShowcommandHstMapFrameData(mapcommandHstJsonData, false, mapframe, zoomtomap);
            }
        }
        else {
            removeHistoriesOnMap(mapframe);
        }

    }
    catch (err) {
    }
}


function showCommandHistoryTab(VehicleId, setDateTime, fleetId) {
    commandHstIniVehicleId = VehicleId;
    commandHstIni = true;
    
    
    //tabs.add(historyGridForm);
    tabs.setActiveTab(commandHstGridForm);
}