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
var tooltip_Add_a_landmark_Circle = tooltip_Add_a_landmark_Circle || 'Add a landmark Circle';
var tooltip_Draw_a_Polygon = tooltip_Draw_a_Polygon || 'Draw a polygon Geozone/Landmark';
var tooltip_Modify_feature = tooltip_Modify_feature || 'Modify feature';
var tooltip_UserPreference = tooltip_UserPreference || 'Default Map View Setting'; //Devin Added (Salman: FR translation)
var tooltip_PrintMap = tooltip_PrintMap || 'Print Map'; //Salman (Mantis# 3018???)
var tooltip_SearchOption = tooltip_SearchOption || 'Search Options';

var btnMapToolPan;
var btnMapToolDrawCircle;
var btnMapToolDrawPolygon;

Ext.onReady(function () {

    Ext.QuickTips.init();

    var tb = Ext.create('Ext.toolbar.Toolbar');
    tb.suspendLayout = true;
    tb.render('toolbar');

    tb.addCls("toolbar-transparent");
    tb.removeCls("x-toolbar-default");

    // They can also be referenced by id in or components

    btnMapToolPan = Ext.create('Ext.Button',
        {
            id: 'btnMapToolPan',
            icon: 'theme/img/geosilk/pan.png', // icons can also be specified inline
            cls: 'x-btn-icon maptoolbar maptoolbarnonetoggle maptoolbarselected',
            tooltip: tooltip_Pan_Map,
            listeners:
            {
                'click': function () {
                    //Ext.example.msg('Button Click','You clicked the "icon only" button.');
                    document.getElementById("noneToggle").checked = true;
                    toggleControl(document.getElementById("noneToggle"));
                    $('.maptoolbar').removeClass('maptoolbarselected');
                    $('.maptoolbarnonetoggle').addClass('maptoolbarselected');

                },
                scope: this
            }
        });
    tb.add(btnMapToolPan);

    //tb.add({
    //    icon: 'theme/img/geosilk/pan.png', // icons can also be specified inline
    //    cls: 'x-btn-icon maptoolbar maptoolbarnonetoggle maptoolbarselected',
    //    tooltip: tooltip_Pan_Map,
    //    handler: function () {
    //        //Ext.example.msg('Button Click','You clicked the "icon only" button.');
    //        document.getElementById("noneToggle").checked = true;
    //        toggleControl(document.getElementById("noneToggle"));
    //        $('.maptoolbar').removeClass('maptoolbarselected');
    //        $('.maptoolbarnonetoggle').addClass('maptoolbarselected');
    //    }
    //});
    /*tb.add({
    icon: 'theme/img/silk/information.png', // icons can also be specified inline
    cls: 'x-btn-icon maptoolbar maptoolbarselecttoggle',
    tooltip: 'Edit Feature Info ',
    handler: function () {
    //Ext.example.msg('Button Click','You clicked the "icon only" button.');
    document.getElementById("selectToggle").checked = true;
    toggleControl(document.getElementById("selectToggle"));
    $('.maptoolbar').removeClass('maptoolbarselected');
    $('.maptoolbarselecttoggle').addClass('maptoolbarselected');
    }
    });*/

    btnMapToolDrawCircle = Ext.create('Ext.Button',
        {
            id: 'btnMapToolDrawCircle',
            icon: 'theme/img/mm_20_blue.png', // icons can also be specified inline
            cls: 'x-btn-icon maptoolbar maptoolbarcircletoggle',
            tooltip: tooltip_Add_a_landmark_Circle,
            listeners:
            {
                'click': function () {
                    //Ext.example.msg('Button Click','You clicked the "icon only" button.');
                    document.getElementById("circleToggle").checked = true;
                    toggleControl(document.getElementById("circleToggle"));
                    $('.maptoolbar').removeClass('maptoolbarselected');
                    $('.maptoolbarcircletoggle').addClass('maptoolbarselected');
        
                },
                scope: this
            }            
        });
    tb.add(btnMapToolDrawCircle);

    //tb.add({
    //    icon: 'theme/img/mm_20_blue.png', // icons can also be specified inline
    //    cls: 'x-btn-icon maptoolbar maptoolbarcircletoggle',
    //    tooltip: tooltip_Add_a_landmark_Circle,
    //    handler: function () {
    //        //Ext.example.msg('Button Click','You clicked the "icon only" button.');
    //        document.getElementById("circleToggle").checked = true;
    //        toggleControl(document.getElementById("circleToggle"));
    //        $('.maptoolbar').removeClass('maptoolbarselected');
    //        $('.maptoolbarcircletoggle').addClass('maptoolbarselected');
    //    }
    //});

    btnMapToolDrawPolygon = Ext.create('Ext.Button', {
        id: 'btnMapToolDrawPolygon',
        icon: 'theme/img/silk/polygon.png', // icons can also be specified inline
        cls: 'x-btn-icon maptoolbar maptoolbarpolygontoggle',
        tooltip: tooltip_Draw_a_Polygon,
        listeners:
        {
            'click': function () {
                //Ext.example.msg('Button Click','You clicked the "icon only" button.');
                document.getElementById("polygonToggle").checked = true;
                toggleControl(document.getElementById("polygonToggle"));
                $('.maptoolbar').removeClass('maptoolbarselected');
                $('.maptoolbarpolygontoggle').addClass('maptoolbarselected');
            }
        }
    });
    tb.add(btnMapToolDrawPolygon);
    
    tb.add({
        icon: 'theme/img/silk/shape_handles.png', // icons can also be specified inline
        cls: 'x-btn-icon maptoolbar maptoolbarmodifytoggle',
        tooltip: tooltip_Modify_feature,
        handler: function () {
            //Ext.example.msg('Button Click','You clicked the "icon only" button.');
            document.getElementById("modifyToggle").checked = true;
            toggleControl(document.getElementById("modifyToggle"));
            $('.maptoolbar').removeClass('maptoolbarselected');
            $('.maptoolbarmodifytoggle').addClass('maptoolbarselected');
        }
    });

    //Devin Added
    tb.add({
        icon: 'theme/img/silk/map_magnify.png', // icons can also be specified inline
        cls: 'x-btn-icon maptoolbar maptoolbarmodifytoggle',
        tooltip: tooltip_UserPreference,
        handler: function () {
            SettingDefaultMap();
        }
    });
	
    //Salman Added (Dec 30, 2013 for Print)
    tb.add({
        icon: 'theme/img/silk/printer.png', // icons can also be specified inline
        cls: 'x-btn-icon maptoolbar maptoolbarmodifytoggle',
        tooltip: tooltip_PrintMap,
        handler: function () {
            printMap();
        }
    });

    //Peter Added (July 23, 2014 for Search Options)
    tb.add({
        icon: 'theme/img/silk/searchOptions.png', // icons can also be specified inline
        cls: 'x-btn-icon maptoolbar maptoolbarmodifytoggle',
        tooltip: tooltip_SearchOption,
        handler: function () {
            $('#searchAddressOptions').toggle();
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

