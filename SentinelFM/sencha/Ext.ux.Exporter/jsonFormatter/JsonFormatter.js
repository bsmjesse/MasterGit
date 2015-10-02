/**
 * @class Ext.ux.Exporter.JsonFormatter
 * @extends Ext.ux.Exporter.Formatter
 * Specialised Format class for outputting .json files
 */
Ext.define("Ext.ux.exporter.jsonFormatter.JsonFormatter", {
    extend: "Ext.ux.exporter.Formatter",
    
    separator: ",",
    extension: "json",

    format: function(store, config) {
        this.columns = config.columns || (store.fields ? store.fields.items : store.model.prototype.fields.items);
		if(config.separator != undefined)
			this.separator = config.separator;
		var s = this.getHeaders() + "," + '"Data":' + '[' + this.getRows(store) + ']';
		//alert(s);
        return '{' + s + '}';
    },
    getHeaders: function(store) {
        var columns = [], title;
        Ext.each(this.columns, function(col) {
          var title;
          if (col.text != undefined) {
            title = col.text;
          } else if(col.name) {
            title = col.name.replace(/_/g, " ");
            title = Ext.String.capitalize(title);
          }
		  if(title != '&#160;')
		  {
			  title = '"' + title.toString().replace(/\"/g, '\\"') + '"';
			  columns.push(title);
		  }
        }, this);

        return '"Header":[' + columns.join(this.separator) + ']';
    },
    getRows: function(store) {
        var rows = [];
        store.each(function(record, index) {
          rows.push(this.geCell(record, index));
        }, this);

        return rows.join(",");
    },
    geCell: function(record, index) {
        var cells = [];
		
        Ext.each(this.columns, function(col) {
			
            var name = col.name || col.dataIndex;
            if(name) {
                if (Ext.isFunction(col.renderer)) {
                  var value = col.renderer(record.get(name), null, record);
                } else {
                  var value = record.get(name);
                }
				value = value.toString().replace(/<br\s*\/?>/img, '[br]');
				//if(value.indexOf('Pre')>=0)
				//	alert(value);
				
				var mydiv = document.createElement("div");
				mydiv.innerHTML = value;

				if (document.all) // IE Stuff
				{
					value =  mydiv.innerText;				   
				}   
				else // Mozilla does not work with innerText
				{
					value = mydiv.textContent;
				}          
				
				value = '"' + value.toString().replace(/\"/g, '\\"') + '"';
                cells.push(value);
            }
			else
			{
				//value = '""';
				//cells.push(value);
			}
        });
		
		return '[' + cells.join(this.separator) + ']';
    }
});