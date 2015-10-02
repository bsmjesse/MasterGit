Ext.require([
'*'
]);

Ext.onReady(function()
{
   Ext.QuickTips.init();

   var bd = Ext.getBody();

   var required = '<span style="color:red;font-weight:bold" data-qtip="Required">*</span>';


   var top = Ext.widget(
   {
      xtype : 'form',
      id : 'feedbackform',
      // collapsible : true,
      //frame : true,
      //title : 'BSM Beta map feedback',
      titleCollapse: true,
      //bodyPadding : '5 5 0',
     // width : 600,
      fieldDefaults :
      {
         labelAlign : 'top',
         msgTarget : 'side'
      }
      ,

      items : [
      {
         xtype : 'container',
         anchor : '100%',
         layout : 'hbox',
         items : [
         {
            xtype : 'container',
            flex : 1,
            layout : 'anchor',
            items : [
            {
               xtype : 'textfield',
               fieldLabel : 'First Name',
               afterLabelTextTpl : required,
               name : 'firstname',
               anchor : '95%',
               allowBlank: false 
               //value : ''
            }
            ,
            {
               xtype : 'textfield',
               fieldLabel : 'Company',
               name : 'company',
               anchor : '95%'
               //,value : ''
            }
            ]
         }
         ,
         {
            xtype : 'container',
            flex : 1,
            layout : 'anchor',
            items : [
            {
               xtype : 'textfield',
               fieldLabel : 'Last Name',
               afterLabelTextTpl : required,
               name : 'lastname',
               anchor : '100%',
               allowBlank: false 
               //,value : ''
            }
            ,
            {
               xtype : 'textfield',
               fieldLabel : 'Email',
               afterLabelTextTpl : required,
               name : 'email',
               vtype : 'email',
               anchor : '100%',
               allowBlank: false 
               //,value : ''
            }
            ]
         }
         ]
      }    
 ,/*
      {
         xtype : 'htmleditor',
         name : 'comments',
         id: 'comments',
         fieldLabel : 'Feedback',
         height : 200,
         anchor : '100%'
      }
      ,*/
      {
                    xtype: 'textareafield',
                     name : 'comments',
                    fieldLabel: 'Feedback/Suggetions',
                    labelAlign: 'top',
                    id: 'comments',
                    height : 200,
                    anchor : '100%',
                    //flex: 1,
                    //margins: '0',
                    afterLabelTextTpl: required,
                    allowBlank: false
      }
      ],
      buttons : [
      /* {
      text : 'Save',
      handler : function(){

      }
      }, */
      {
         xtype : 'button',
         formBind : true,
         disabled : true,
         text : 'Submit feedback',
         width : 140,
         handler : function()
         {
            var form = this.up('form').getForm();
            if (form.isValid())
            {
               /* Normally we would submit the form to the server here and handle the response... */
               form.submit(
               {
                  clientValidation : true,
                  url : 'Feedback.aspx', // Just for emailing feedback
                  params :
                  {
                     cmd : 'save'
                  }
                  ,
                  waitMsg : 'Sending...',
                  success : function(form, action)
                  {
                     //console.log(action.response.responseText);
                     Ext.Msg.show(
                     {
                        title : 'Success'
                        , msg : 'Feedback submitted successfully'
                        , modal : true
                        , icon : Ext.Msg.INFO
                        , buttons : Ext.Msg.OK
                     }
                     );
                     
                     //window.close();
                  }
                  ,
                  failure : function(form, action)
                  {
                       this.showError(action.result.error || action.response.responseText);                   
                  }
                  ,
                  showError : function(msg, title)
                  {
                     title = title || 'Error';
                     Ext.Msg.show(
                     {
                        title : title
                        , msg : msg
                        , modal : true
                        , icon : Ext.Msg.ERROR
                        , buttons : Ext.Msg.OK
                     }
                     );
                  }
                  // eo function showError
               }
               );
            }

            // if (form.isValid()) {
            // Ext.Msg.alert('Thank you your feedback has been submitted..');

            // }
         }
      }
      ,
      {
         text : 'Reset',
         handler : function()
         {
            this.up('form').getForm().reset();
         }
      }
      /* ,
      {
      text : 'Cancel',
      handler : function(){

      }
      } */
      ]
      // }]
   }
   );

   top.render(document.body);
}
);
