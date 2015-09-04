/*

This file is part of Ext JS 4

Copyright (c) 2011 Sencha Inc

Contact:  http://www.sencha.com/contact

Commercial Usage
Licensees holding valid commercial licenses may use this file in accordance with the Commercial Software License Agreement provided with the Software or, alternatively, in accordance with the terms contained in a written agreement between you and Sencha.

If you are unsure which license is appropriate for your use, please contact the sales department at http://www.sencha.com/contact.

*/
Ext.require(['*']);

// TODO: The "Users" menu containing buttons is completely screwed: ButtonGroup needs work.

var tooltip_Pan_Map = tooltip_Pan_Map || 'Pan Map';
var tooltip_Add_a_landmark_depot = tooltip_Add_a_landmark_depot || 'Add a depot';
var tooltip_Add_a_landmark_station = tooltip_Add_a_landmark_station || 'Add a station';

Ext.onReady(function () {

    Ext.QuickTips.init();

    var tb = Ext.create('Ext.toolbar.Toolbar');
    tb.suspendLayout = true;
    tb.render('toolbar');

    tb.addCls("toolbar-transparent");
    tb.removeCls("x-toolbar-default");

    // They can also be referenced by id in or components
    tb.add({
        icon: 'img/pan.png', // icons can also be specified inline
        cls: 'x-btn-icon maptoolbar maptoolbarnonetoggle maptoolbarselected',
        tooltip: tooltip_Pan_Map,
        handler: function () {
            //Ext.example.msg('Button Click','You clicked the "icon only" button.');
            toggleControl("none");
            $('.maptoolbar').removeClass('maptoolbarselected');
            $('.maptoolbarnonetoggle').addClass('maptoolbarselected');
        }
    });
    tb.add({
        icon: 'img/mm_20_blue.png', // icons can also be specified inline
        cls: 'x-btn-icon maptoolbar maptoolbarcircletoggle',
        tooltip: tooltip_Add_a_landmark_station,
        handler: function () {
            //Ext.example.msg('Button Click','You clicked the "icon only" button.');
            toggleControl("station", true);
            $('.maptoolbar').removeClass('maptoolbarselected');
            $('.maptoolbarcircletoggle').addClass('maptoolbarselected');
        }
    });
    tb.add({
        icon: 'img/mm_20_red.png', // icons can also be specified inline
        cls: 'x-btn-icon maptoolbar maptoolbarpolygontoggle',
        tooltip: tooltip_Add_a_landmark_depot,
        handler: function () {
            //Ext.example.msg('Button Click','You clicked the "icon only" button.');
            toggleControl("depot", true);
            $('.maptoolbar').removeClass('maptoolbarselected');
            $('.maptoolbarpolygontoggle').addClass('maptoolbarselected');
        }
    });
    tb.suspendLayout = false;
    tb.doLayout();

    // functions to display feedback
    function onButtonClick(btn) {
        //Ext.example.msg('Button Click','You clicked the "{0}" button.', btn.text);
    }

    function onItemClick(item) {
        //Ext.example.msg('Menu Click', 'You clicked the "{0}" menu item.', item.text);
    }

    function onItemCheck(item, checked) {
        //Ext.example.msg('Item Check', 'You {1} the "{0}" menu item.', item.text, checked ? 'checked' : 'unchecked');
    }

    function onItemToggle(item, pressed) {
        //Ext.example.msg('Button Toggled', 'Button "{0}" was toggled to {1}.', item.text, pressed);
    }

});

