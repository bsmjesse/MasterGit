/**
 * @class Ext.ux.Exporter.ExportMenu
 * @extends Ext.Component
 * @author Nige White, with modifications from Ed Spencer, with modifications from iwiznia.
 * Specialised Button class that allows downloading of data via data: urls.
 * Internally, this is just a link.
 * Pass it either an Ext.Component subclass with a 'store' property, or just a store or nothing and it will try to grab the first parent of this button that is a grid or tree panel:
 * new Ext.ux.Exporter.Button({component: someGrid});
 * new Ext.ux.Exporter.Button({store: someStore});
 * @cfg {Ext.Component} component The component the store is bound to
 * @cfg {Ext.data.Store} store The store to export (alternatively, pass a component with a getStore method)
 */
Ext.define("Ext.ux.exporter.ExportMenu", {
    extend: "Ext.menu.Menu",
    alias: "widget.exportermenu",
    html: '<p></p>',
    config: {
        swfPath: '/flash/downloadify.swf',
        downloadImage: '/images/ext_reports/download.png',
        width: 62,
        height: 22,
        downloadName: "download"
    },

    constructor: function(config) {
      config = config || {};

      this.initConfig();
      Ext.ux.exporter.ExportMenu.superclass.constructor.call(this, config);

      var self = this;
      this.on("afterrender", function() { // We wait for the combo to be rendered, so we can look up to grab the component containing it
          self.setComponent(self.store || self.component || self.up("gridpanel") || self.up("treepanel"), config);
      });
    },
	
	items: [{
        text: 'regular item 1'
    },{
        text: 'regular item 2'
    },{
        text: 'regular item 3'
    }]
    /*setComponent: function(component, config) {
        this.component = component;
        this.store = !component.is ? component : component.getStore(); // only components or stores, if it doesn't respond to is method, it's a store
        this.setMenu(config);
    },

    setMenu: function(config) {
        var self = this;
        
		
		Ext.create('Ext.Button',
		   {
			   text: 'Export To Excel2007',
			   id: 'exportbutton',
			   tooltip: 'Export',
			   iconCls: 'map',
			   cls: 'cmbfonts',
			   textAlign: 'left',
			   handler: function () {
				   try {

					   var component = vehiclegrid;
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
		   );
    }*/
});