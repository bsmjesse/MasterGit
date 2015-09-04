// JavaScript Document
Ext.define('Ext.ux.AspWebAjaxProxy', {
    extend: 'Ext.data.proxy.Ajax',
    require: 'Ext.data',

    buildRequest: function (operation) {
        var params = Ext.applyIf(operation.params || {}, this.extraParams || {}),
                                request;
        params = Ext.applyIf(params, this.getParams(params, operation));
        if (operation.id && !params.id) {
            params.id = operation.id;
        }

        params = Ext.JSON.encode(params);

        request = Ext.create('Ext.data.Request', {
            params: params,
            action: operation.action,
            records: operation.records,
            operation: operation,
            url: operation.url
        });
        request.url = this.buildUrl(request);
        operation.request = request;
        return request;
    }
});
