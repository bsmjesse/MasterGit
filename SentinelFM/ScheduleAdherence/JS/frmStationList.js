Ext.onReady(function () {
    Ext.define('User', {
        extend: 'Ext.data.Model',
        fields: ['name', 'email', 'phone', 'date']
    });

    var userStore = Ext.create('Ext.data.Store', {
        model: 'User',
        autoLoad: true,
        pageSize: 2,
        data: [
        { name: 'Lisa', email: 'lisa@simpsons.com', phone: '555-111-1224', date:'2013-03-01' },
        { name: 'Bart', email: 'bart@simpsons.com', phone: '555-222-1234', date: '2013-04-01' },
        { name: 'Homer', email: 'home@simpsons.com', phone: '555-222-1244', date: '2013-05-01' },
        { name: 'Marge', email: 'marge@simpsons.com', phone: '555-222-1254', date: '2013-06-01' }
    ]
    });

    Ext.create('Ext.grid.Panel', {
        renderTo: Ext.getDom('Dev_test'),
        store: userStore,
        width: 600,
        title: 'Application Users',
        selType: 'rowmodel',
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
            clicksToEdit: 1
            }),
            Ext.create('Ext.grid.plugin.RowEditing', {
                clicksToEdit: 1
            })
        ],
        columns: [
        {
            text: 'Name',
            width: 100,
            sortable: false,
            hideable: false,
            dataIndex: 'name'
        },
        {
            text: 'Email Address',
            width: 150,
            dataIndex: 'email',
            renderer: function (value) {
                return Ext.String.format('<a href="mailto:{0}">{0}</a>', value);
            },
            editor: 'textfield'
        },
        {
            text: 'Date',
            width: 150,
            dataIndex: 'date',
            renderer: Ext.util.Format.dateRenderer('m/d/Y'),
            editor: 'datefield'
        },
        {
            text: 'Phone Number',
            flex: 1,
            dataIndex: 'phone',
            editor: 'textfield'
        }
    ],
    dockedItems: [{
        xtype: 'pagingtoolbar',
        store: userStore,   // same store GridPanel is using
        dock: 'bottom',
        displayInfo: true
    }]
    });
});