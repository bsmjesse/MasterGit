/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

GNU General Public License Usage
This file may be used under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation and appearing in the file LICENSE included in the packaging of this file.  Please review the following information to ensure the GNU General Public License version 3.0 requirements will be met: http://www.gnu.org/copyleft/gpl.html.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
/**
 * @class Ext.app.Portal
 * @extends Object
 * A sample portal layout application class.
 */
// TODO: Fill in the content panel -- no AccordionLayout at the moment
// TODO: Fix container drag/scroll support (waiting on Ext.lib.Anim)
// TODO: Fix Ext.Tool scope being set to the panel header
// TODO: Drag/drop does not cause a refresh of scroll overflow when needed
// TODO: Grid portlet throws errors on destroy (grid bug)
// TODO: Z-index issues during drag

Ext.define('Ext.app.Portal', {

    extend: 'Ext.container.Viewport',

    uses: ['Ext.app.PortalPanel', 'Ext.app.PortalColumn', 'Ext.app.GridPortlet', 'Ext.app.ChartPortlet', 'Ext.app.PiePortlet'],

    getTools: function () {
        return [{
            xtype: 'tool',
            type: 'gear',
            handler: function (e, target, panelHeader, tool) {
                var portlet = panelHeader.ownerCt;
                portlet.setLoading('Working...');
                Ext.defer(function () {
                    portlet.setLoading(false);
                }, 2000);
            }
        }];
    },

    initComponent: function () {
        var content = '<div class="portlet-content">' + Ext.example.shortBogusMarkup + '</div>';

        Ext.apply(this, {
            id: 'app-viewport',
            layout: {
                type: 'border',
                padding: '0 0 0 0' // pad the layout from the window edges
            },
            items: [{
                xtype: 'container',
                region: 'center',
                layout: 'border',
                border: false,
                items: [{
                    id: 'app-portal',
                    xtype: 'portalpanel',

                    region: 'center',
                    style: 'margin:0',
                    items: [

                      {
                          id: 'col-1',

                          items: [

                         {
                             id: 'portlet-HeartBeat',
                             title: 'HeartBeat',
                             tools: this.getTools(),
                             html: '<iframe src="heartbeat.aspx" width="100%" height="330px"  marginheight="0" marginwidth="0" frameborder="0"></iframe>',
                             listeners: {
                                 'close': Ext.bind(this.onPortletClose, this)


                             }
                         }
                        ,
                        {
                            id: 'portlet-AHA',
                            title: 'After Hours Alarms',
                            tools: this.getTools(),
                            html: '<iframe src="ahanew.aspx" width="100%" height="330px"  marginheight="0" marginwidth="0" frameborder="0"></iframe>',
                            listeners: {
                                'close': Ext.bind(this.onPortletClose, this)
                            }
                        }

                       ]
                      },


                     {
                         id: 'col-2',
                         items: [

                        {
                            id: 'portlet-Idling',
                            title: 'Idling',
                            tools: this.getTools(),
                            html: '<iframe src="idlingnew.aspx" width="100%" height="330px" marginheight="0" marginwidth="0" frameborder="0"></iframe>',
                            listeners: {
                                'close': Ext.bind(this.onPortletClose, this)
                            }
                        },

                         {
                             id: 'portlet-Violations',
                         title: 'Violations',
                         tools: this.getTools(),
                         html: '<iframe src="violationnew.aspx" width="100%" height="330px"  marginheight="0" marginwidth="0" frameborder="0"></iframe>',
                         listeners: {
                             'close': Ext.bind(this.onPortletClose, this)
                         }
                     }

                       ]
                     }

                      ,
         



                      {
                      id: 'col-3',
                      items: [

                        {
                            id: 'portlet-Maintenance',
                            title: 'Maintenance',
                            tools: this.getTools(),
                            html: '<iframe src="maintenancenew.aspx" width="100%" height="703px"  marginheight="0" marginwidth="0" frameborder="0"></iframe>',
                            
                            listeners: {
                                'close': Ext.bind(this.onPortletClose, this)
                            }
                        }

                       ]
                  }




                    ]
                }]
            }]
        });
        this.callParent(arguments);
    },

    onPortletClose: function (portlet) {
        this.showMsg('"' + portlet.title + '" was removed');
    },

    showMsg: function (msg) {
        var el = Ext.get('app-msg'),
            msgId = Ext.id();

        this.msgId = msgId;
        el.update(msg).show();

        Ext.defer(this.clearMsg, 3000, this, [msgId]);
    },

    clearMsg: function (msgId) {
        if (msgId === this.msgId) {
            Ext.get('app-msg').hide();
        }
    }
});

