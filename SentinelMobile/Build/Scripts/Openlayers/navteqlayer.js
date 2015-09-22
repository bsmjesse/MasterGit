OpenLayers.Layer.Navteq = OpenLayers.Class(OpenLayers.Layer.XYZ, {
     name: "Navteq",
	 type: "normal.day",
	 app_id: "v5HljYEynPujgUUkNmny",
	 token: "x14y1MmQaoSerjNQKGsABw",
     attribution: "&copy; 2013 Nokia&nbsp;<a href='http://maps.nokia.com/services/terms' target='_blank' title='Terms of Use' style='color:#333;text-decoration: underline;'>Terms of Use</a>",
     sphericalMercator: true,
     //url: 'https://maps.nlp.nokia.com/maptiler/v2/maptile/newest/satellite.day/${z}/${x}/${y}/256/png8?app_id=v5HljYEynPujgUUkNmny&token=x14y1MmQaoSerjNQKGsABw',
     url: 'https://1.base.maps.api.here.com/maptile/2.1/maptile/newest/normal.day/${z}/${x}/${y}/256/png8?app_id=v5HljYEynPujgUUkNmny&app_code=x14y1MmQaoSerjNQKGsABw',
     normalBaseUrl: 'https://1.base.maps.api.here.com/maptile/2.1/maptile/newest/',
     hybridBaseUrl: 'https://1.aerial.maps.cit.api.here.com/maptile/2.1/maptile/newest/',
	 initialize: function(name, type, app_id, token, options) {
        name = name || this.name;
		type = type || this.type;
		app_id = app_id || this.app_id;
		token = token || this.token;
		//url = 'https://maps.nlp.nokia.com/maptiler/v2/maptile/newest/' + type + '/${z}/${x}/${y}/256/png8?app_id=' + app_id + '&token=' + token;
		//url = 'https://1.base.maps.api.here.com/maptile/2.1/maptile/newest/' + type + '/${z}/${x}/${y}/256/png8?app_id=' + app_id + '&app_code=' + token;
		url = (type == 'hybrid.day' ? this.hybridBaseUrl : this.normalBaseUrl) + type + '/${z}/${x}/${y}/256/png8?app_id=' + app_id + '&app_code=' + token;
        var newArguments = [name, url, {}, options];
        OpenLayers.Layer.XYZ.prototype.initialize.apply(this, newArguments);
    },
     clone: function(obj) {
         if (obj == null) {
             obj = new OpenLayers.Layer.Navteq(
                 this.name, this.url, this.getOptions());
         }
         obj = OpenLayers.Layer.XYZ.prototype.clone.apply(this, [obj]);
         return obj;
     },
     wrapDateLine: true,
	 
	 CLASS_NAME: "OpenLayers.Layer.Navteq"
});