Ext.onReady(function () {
    
    var doc;
    if (window.ActiveXObject) {         //IE
        var doc = new ActiveXObject("Microsoft.XMLDOM");
        doc.async = "false";
        doc.loadXML(VedioList);
    } else {                             //Mozilla
        var doc = new DOMParser().parseFromString(VedioList, "text/xml");
    }

    Ext.define('Video', {
        extend: 'Ext.data.Model',
        fields: [
            'FileName'
        , { name: 'UnitId', type: 'int' }
        , { name: 'FileSize', type: 'int' }
        , 'Channel'
        , { name: 'RecordingDateTime', type: 'date', dateFormat: 'c' }            
        ]
    });
    //alert(VedioList);
    // create the Data Store
    store = Ext.create('Ext.data.Store', {
        model: 'Video',
        autoLoad: true,
        data: doc,
        proxy: {
            // load using HTTP
            type: 'memory',
            //url: 'VideoViewer.aspx?st=videolist',
            reader: {
                type: 'xml',
                root: 'Video',
                record: 'VideoInfo'
            }
        }
    });

    videogrid = Ext.create('Ext.grid.Panel', {
        id: 'videogrid',
        enableColumnHide: false,
        enableSorting: false,
        closable: false,
        collapsible: false,
        resizable: false,
        width: 525,
        height: '100%',
        title: 'Videos',
        store: store,
        //renderTo: 'videos-grid',
        viewConfig: {
            emptyText: 'No videos to display',
            useMsg: false
        },
        columns: [
        /*{
            text: 'Video',
            align: 'left',
            width: 280,
            //renderer: function (value) {
            //    return Ext.String.format('<a href="javascript:void(0)" OnClick="NewWindow({0})">{1}</a>', value, value);
            //},
            dataIndex: 'FileName',
            sortable: false
        }
        , */{
            text: 'UnitId',
            align: 'left',
            width: 80,
            dataIndex: 'UnitId',
            sortable: false
        }
        , {
            text: 'DateTime',
            align: 'left',
            width: 140,
            xtype: 'datecolumn',
            format: userdateformat,
            dataIndex: 'RecordingDateTime',
            sortable: false
        }
        , {
            text: 'Channel',
            align: 'left',
            width: 100,
            dataIndex: 'Channel',
            sortable: false
        }
        , {
            text: 'Filesize',
            align: 'right',
            width: 80,
            dataIndex: 'FileSize',
            sortable: false
        }
        , {
            text: '',
            align: 'left',
            width: 80,
            dataIndex: 'FileName',
            renderer: function (value) {

                //return Ext.String.format('{0} <object><embed src="./sounds/FireAlarm.wav" hidden="true" autostart="True" loop="true" type="audio/wav" pluginspage="http://www.apple.com/quicktime/      download/" /></object>', value);
                return Ext.String.format("<a href='javascript:void(0)' onclick=\"DownloadVideo('{0}')\">Download</a>", encodeURIComponent(value));
                //return "<a href='javascript:void(0)' onclick='DownloadVideo(\"" + encodeURIComponent() + "\")'>Download</a>";

            }
        }
        ]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: videogrid,
        renderTo: 'videos-grid',
    });
});