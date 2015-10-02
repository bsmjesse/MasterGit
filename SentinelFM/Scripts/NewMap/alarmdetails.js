Ext.require([
'Ext.form.*',
'Ext.data.*'
]);

Ext.onReady(function () {
    var required = '<span style="color:red;font-weight:bold" data-qtip="Required">*</span>';
    Ext.define('Alarm',
   {
       extend: 'Ext.data.Model',
       fields: [
      'AlarmId',
       //      {
       //         name : 'TimeCreated', type : 'date'
       //         // , dateFormat : 'c'
       //      }
      'TimeCreated',
      'AlarmLevel',
      'vehicleDescription',
      'AlarmDescription',
      'AlarmState',
      'AlarmSeverity',
      'VehicleId',
      'StreetAddress',
      'ExtraNotes',
      'Latitude',
      'Longitude',
      'Speed',
      'Heading',
      'BoxId'
      ],
       idProperty: 'AlarmId'
   }
   );


    Ext.define('example.fielderror',
   {
       extend: 'Ext.data.Model',
       fields: ['id', 'msg']
   }
   );

    var alarmPanel = Ext.create('Ext.form.Panel',
   {
       // renderTo : 'form-ct',
       renderTo: Ext.getBody(),
       // frame : true,
       title: 'Alarm details',
       width: 525,
       // bodyPadding : 5,
       waitMsgTarget: true,
       defaultType: 'textfield',
       fieldDefaults:
      {
          labelAlign: 'left',
          readOnly: true,
          labelWidth: 100,
          msgTarget: 'side',
          anchor: '100%'
      }
      ,

       // configure how to read the XML data
       reader: Ext.create('Ext.data.reader.Xml',
      {
          // type : 'xml',
          model: 'Alarm',
          // root : 'Alarm',
          record: 'AlarmInfo'
      }
      ),

       // configure how to read the XML errors
       errorReader: Ext.create('Ext.data.reader.Xml',
      {
          model: 'example.fielderror',
          record: 'field'
          // , successProperty : '@success'
      }
      ),

       items: [
      {
          fieldLabel: 'Alarm Number',
          // emptyText : 'First Name',
          name: 'AlarmId'
      }
      ,
      {
          fieldLabel: 'Alarm Description',
          // emptyText : 'Last Name',
          name: 'AlarmDescription'
      }
      ,
      {
          fieldLabel: 'Date Created',
          name: 'TimeCreated'
      }
      ,
      {
          fieldLabel: 'Alarm State',
          name: 'AlarmState'
      }
      ,
      {
          fieldLabel: 'Alarm Severity',
          name: 'AlarmLevel'
      }
      ,
      {
          fieldLabel: 'Vehicle Id',
          name: 'VehicleId'
      }
      ,
      {
          fieldLabel: 'Vehicle Info',
          name: 'vehicleDescription'
      }
      ,
      {
          fieldLabel: 'Street Address',
          name: 'StreetAddress'
      }
      ,
      {
          fieldLabel: 'Notes',
          name: 'ExtraNotes',
          afterLabelTextTpl: required,
          readOnly: false,
          allowBlank: false
      },
      {
          fieldLabel: 'lat',
          name: 'Latitude'
         , hidden: true
      },
      {
          fieldLabel: 'BoxId',
          name: 'BoxId'
         , hidden: true
      }
      ,
      {
          fieldLabel: 'lon',
          name: 'Longitude'
         , hidden: true
      },
      {
          fieldLabel: 'MyHeading',
          name: 'Heading'
         , hidden: true
      },
      {
          fieldLabel: 'Speed',
          name: 'Speed'
         , hidden: true
      },
      {
          fieldLabel: 'htmlExtraInfo',
          value: ''
         , hidden: true
      }
      ],
       buttons: [
      {
          text: 'Accept/Close Alarm',
          formBind: true,
          handler: function () {
              var form = this.up('form').getForm();
              
              if (form.isValid()) {
                  var alarmId = Ext.getCmp('AlarmId');                                 
                  $.ajax({
                      type: 'GET',
                      url: '../Map/AlarmDetails.aspx',
                      data: 'methodType=acceptAlarm&ExtraNotes=' + encodeURIComponent(form.getValues().ExtraNotes),
                      contentType: "application/json; charset=utf-8",
                      dataType: "json",
                      async: false,
                      success: function (msg) {
                          if (msg.success) {
                              window.close();
                              window.opener.document.location.reload();
                          }
                          else {

                            Ext.Msg.show(
                            {
                                title: 'Failure'
                                , msg: msg.msg
                                , modal: true
                                , icon: Ext.Msg.INFO
                                , buttons: Ext.Msg.OK
                            });
                          }
                          
                      },
                      error: function (msg) {
                         Ext.Msg.show(
                         {
                             title: 'Failure ajax'
                            , msg: 'Please provide ExtraNotes for accepting this Alarm.'
                            , modal: true
                            , icon: Ext.Msg.INFO
                            , buttons: Ext.Msg.OK
                         });
                      }
                  });
              }
              else {
                  Ext.Msg.show(
               {
                   title: 'Failure'
                  , msg: 'Please provide ExtraNotes for accepting this Alarm.'
                  , modal: true
                  , icon: Ext.Msg.INFO
                  , buttons: Ext.Msg.OK
               }
               );
              }
          }
      }
      ,
      {
          text: 'Map It',
          // formBind : true,
          handler: function () {
              try {
                  var winparent = window.opener;
                  var mapAlarms = new Array();
                  if (winparent == null) {
                      winparent = window.parent;
                  }
                  if (winparent != null) {
                      var form = this.up('form').getForm();
                      mapAlarms[0] = form.getFieldValues();
                      //winparent.MapAlarm(mapAlarms);
                      winparent.findit(mapAlarms[0].BoxId, true);
                  }
                  window.close();
              }
              catch (err) {
                  // console.log('Error: ' + err.message);
              }
          }
      }
      ,
      {
          text: 'Close',
          handler: function () {
              window.close();
          }
      }
       /* ,
       {
       text : 'Submit',
       disabled : true,
       formBind : true,
       handler : function(){
       this.up('form').getForm().submit({
       url : 'xml-form-errors.xml',
       submitEmptyText : false,
       waitMsg : 'Saving Data...'
       });
       }
       } */]
   }
   );

    alarmPanel.getForm().load(
   {
       url: '../Map/AlarmDetails.aspx',
       params:
      {
          methodType: 'loadData'
      }
      ,
       failure: function (form, action) {
           Ext.Msg.alert("Alarm loading failed", action.result.errorMessage);
       }
   }
   );


}
);
