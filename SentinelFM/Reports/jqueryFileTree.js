// jQuery File Tree Plugin
//
// Version 1.01
//
// Cory S.N. LaViska
// A Beautiful Site (http://abeautifulsite.net/)
// 24 March 2008
//
// Visit http://abeautifulsite.net/notebook.php?article=58 for more information
//
// Usage: $('.fileTreeDemo').fileTree( options, callback )
//
// Options:  root           - root folder to display; default = /
//           script         - location of the serverside AJAX file to use; default = jqueryFileTree.php
//           folderEvent    - event to trigger expand/collapse; default = click
//           expandSpeed    - default = 500 (ms); use -1 for no animation
//           collapseSpeed  - default = 500 (ms); use -1 for no animation
//           expandEasing   - easing function to use on expand (optional)
//           collapseEasing - easing function to use on collapse (optional)
//           multiFolder    - whether or not to limit the browser to one subfolder at a time
//           loadMessage    - Message to display while initial tree loads (can be HTML)
//
// History:
//
// 1.10 - updated by BSM Wireless for organization hierarchy and changed from one column to two seperated columns: folder column and vehicle list column. (28 Sep 2012)
// 1.01 - updated to work with foreign characters in directory/file names (12 April 2008)
// 1.00 - released (24 March 2008)
//
// TERMS OF USE
// 
// This plugin is dual-licensed under the GNU General Public License and the MIT License and
// is copyright 2008 A Beautiful Site, LLC.
//

var JSON = JSON || {};
// implement JSON.stringify serialization
JSON.stringify = JSON.stringify || function (obj) {
    var t = typeof (obj);
    if (t != "object" || obj === null) {
        // simple data type
        if (t == "string")
            obj = '"' + obj + '"';
        return String(obj);
    } else {
        // recurse array or object
        var n, v, json = [], arr = (obj && obj.constructor == Array);
        for (n in obj) {
            v = obj[n];
            t = typeof (v);
            if (t == "string")
                v = '"' + v + '"';
            else if (t == "object" && v !== null)
                v = JSON.stringify(v);
            json.push((arr ? "" : '"' + n + '":') + String(v));
        }
        return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
    }
};

if (jQuery) (function ($) {

    $.extend($.fn, {
        fileTree: function (o, p, h, m) {
            // Defaults            
            if (!o) var o = {};
            if (o.root == undefined) o.root = '/';
            if (o.script == undefined) o.script = 'vehicleListTree.asmx/FetchVehicleList';
            if (o.folderEvent == undefined) o.folderEvent = 'click';
            if (o.expandSpeed == undefined) o.expandSpeed = 500;
            if (o.collapseSpeed == undefined) o.collapseSpeed = 500;
            if (o.expandEasing == undefined) o.expandEasing = null;
            if (o.collapseEasing == undefined) o.collapseEasing = null;
            if (o.multiFolder == undefined) o.multiFolder = true;
            if (o.loadMessage == undefined) o.loadMessage = 'Loading...';
            if (o.vehicledetails == undefined) o.vehicledetails = 'vehicledetails';
            if (o.expanded == undefined) o.expanded = null;
            if (o.highlightVehicleSelection == undefined) o.highlightVehicleSelection = true;
            if (o.searchScript == undefined) o.searchScript = 'vehicleListTree.asmx/SearchOrganizationHierarchy';
            if (o.scrollSearchResultToBottom == undefined) o.scrollSearchResultToBottom = false;
            if (o.MutipleUserHierarchyAssignment == undefined) o.MutipleUserHierarchyAssignment = false;
            if (o.MultipleUserHierarchyAssignment == undefined) o.MultipleUserHierarchyAssignment = o.MutipleUserHierarchyAssignment;
            if (o.ShowAllFleets == undefined) o.ShowAllFleets = true;
            if (o.SelectedFolders == undefined) o.SelectedFolders = '';
            if (o.SelectedFleetIds == undefined) o.SelectedFleetIds = '';
            if (o.SelectedFleetName == undefined) o.SelectedFleetName = '';
            if (o.SelectedFleetPath == undefined) o.SelectedFleetPath = '';
            if (o.SelectedFleetShortPath == undefined) o.SelectedFleetShortPath = '';
            if (o.FullTreeView == undefined) o.FullTreeView = true;
            if (o.PreferOrganizationHierarchyNodeCode == undefined) o.PreferOrganizationHierarchyNodeCode = '';
            if (o.scriptForPreferNodecodes == undefined) o.scriptForPreferNodecodes = 'vehicleListTree.asmx/FetchVehicleListByPreferNodeCodes';
            if (o.multipleExpanded == undefined) o.multipleExpanded = null;
            if (o.LoadVehicleData == undefined) o.LoadVehicleData = true;
            if (o.VehicleListPagingBarId == undefined) o.VehicleListPagingBarId = '';
            //if (o.scriptForFetchVehicleByPage == undefined) o.scriptForFetchVehicleByPage = 'vehicleListTree.asmx/FetchVehicleListByPage';
            if (o.scriptForFetchVehicleByPage == undefined) o.scriptForFetchVehicleByPage = 'vehicleListTree.asmx/FetchVehicleListFilterByPage';
            //../reports/vehicleListTree.asmx/FetchVehicleListFilterByPage
            if (o.ResMultipleHierarchy == undefined) o.ResMultipleHierarchy = 'Multiple Hierarchies';
            if (o.VehiclePageSize == undefined) o.VehiclePageSize = 10;
            if (o.RootUrl == undefined) o.RootUrl = "/";
            if (o.EnableContextMenu == undefined) o.EnableContextMenu = false;
            if (o.ManagerColumn == undefined) o.ManagerColumn = false;

            //o.expanded = 'UPC01';
            var expansion_completed = false;
            var vl;

            var originExpended = o.expanded;
            var MultipleInitialPath = false;
            var MultipleInitialPathIndex = 0;

            var currentFleetId = 0;

            if (o.expanded != null)
                o.expanded = ftrim(o.expanded, '/');

            var ShowPreferNodecodes = false;
            if (o.root != '/' && o.root != '')
                ShowPreferNodecodes = true;


            $(this).each(function () {

                function showTree(c, t, x, s) {

                    if (x == undefined) x = true;
                    if (s == undefined) s = true;
                    if (x) {
                        $(c).addClass('wait');
                        $(".jqueryFileTree.start").remove();
                    }

                    /*var ShowPreferNodecodes = false;
                    if ($(c).find('a:first').attr('root') == "1" && !o.FullTreeView && o.MultipleUserHierarchyAssignment) {
                    ShowPreferNodecodes = true;
                    }

                    if (o.root == unescape(t) && o.root != '' && o.root != '/') {
                    ShowPreferNodecodes = true;
                    o.PreferOrganizationHierarchyNodeCode = o.root;
                    }*/

                    $.ajax({
                        type: 'POST',
                        url: ShowPreferNodecodes ? o.scriptForPreferNodecodes : o.script,
                        //data: ShowPreferNodecodes ? JSON.stringify({ nodecode: t, MutipleUserHierarchyAssignment: o.MultipleUserHierarchyAssignment, PreferNodeCodes: o.PreferOrganizationHierarchyNodeCode, LoadVehicleData: o.LoadVehicleData }) : JSON.stringify({ nodecode: t, MutipleUserHierarchyAssignment: o.MultipleUserHierarchyAssignment, LoadVehicleData: o.LoadVehicleData }),
                        data: ShowPreferNodecodes ? JSON.stringify({ nodecode: t, MutipleUserHierarchyAssignment: o.MultipleUserHierarchyAssignment, PreferNodeCodes: o.root, LoadVehicleData: o.LoadVehicleData }) : JSON.stringify({ nodecode: t, MutipleUserHierarchyAssignment: o.MultipleUserHierarchyAssignment, LoadVehicleData: o.LoadVehicleData }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: false,
                        success: function (msg) {
                            ShowPreferNodecodes = false;
                            var r = eval('(' + msg.d + ')');

                            if (r.fleetId && r.fleetId > 0)
                                currentFleetId = r.fleetId;

                            if (x) {
                                $(c).find('.start').html('');
                                $(c).removeClass('wait');
                                var needbind = false;

                                if ($(c).find('UL').length == 0) {
                                    $(c).append(r.folderlist);
                                    needbind = true;
                                }

                                if (o.root == t) {
                                    $(c).find('UL:hidden').show();
                                    $(c).find('a:first').attr('root', 1);
                                    //alert('root');
                                }
                                else {
                                    $(c).children('UL:hidden').slideDown({ duration: o.expandSpeed, easing: o.expandEasing }).children('LI').removeClass('expanded').addClass('collapsed');
                                }

                                if (needbind)
                                    bindTree(c, r.fleetId);

                                if (o.MultipleUserHierarchyAssignment) {
                                    if (needbind) {
                                        $(c).children().find('.chkfleet').bind('click', function () {
                                            o.SelectedFolders = UpdateCollection(o.SelectedFolders, $(this).attr('data-nodecode'), $(this).attr('checked'));
                                            o.SelectedFleetIds = UpdateCollection(o.SelectedFleetIds, $(this).parent().find('a:first').attr('fleetid'), $(this).attr('checked'));
                                            o.SelectedFleetName = o.SelectedFolders.split(',').length > 1 ? o.ResMultipleHierarchy : $('a[rel="' + o.SelectedFolders + '"]').html();
                                            o.SelectedFleetPath = UpdateCollection(o.SelectedFleetPath, $(this).parent().find('a:first').attr('fleetpath'), $(this).attr('checked'), "@,@");

                                            //unselect all the children and make it disable
                                            if ($(this).attr('checked')) {

                                                $(this).parent().children().find('.chkfleet').each(function (index) {
                                                    if ($(this).attr('checked')) {
                                                        o.SelectedFolders = UpdateCollection(o.SelectedFolders, $(this).attr('data-nodecode'), false);
                                                        o.SelectedFleetIds = UpdateCollection(o.SelectedFleetIds, $(this).parent().find('a:first').attr('fleetid'), false);
                                                        o.SelectedFleetName = o.SelectedFolders.split(',').length > 1 ? o.ResMultipleHierarchy : $('a[rel="' + o.SelectedFolders + '"]').html();
                                                        o.SelectedFleetPath = UpdateCollection(o.SelectedFleetPath, $(this).parent().find('a:first').attr('fleetpath'), false, "@,@");
                                                        $(this).attr('checked', false);
                                                    }
                                                });
                                                $(this).parent().children().find('.chkfleet').attr("disabled", true);
                                                $(this).parent().children().find('.chkfleet').attr("checked", true);
                                            }
                                            else {
                                                $(this).parent().children().find('.chkfleet').attr("disabled", false);
                                                $(this).parent().children().find('.chkfleet').attr("checked", false);
                                            }
                                            m(o.SelectedFolders, o.SelectedFleetIds, o.SelectedFleetName, o.SelectedFleetPath);
                                        });
                                    }

                                    // if any parent checked, all the children will be disabled.
                                    var parentChecked = false;
                                    if ($(c).find('input:first').attr('checked'))
                                        parentChecked = true;
                                    if (!parentChecked) {
                                        $(c).parents('li.directory').each(function (index) {
                                            if ($(this).find('input:first').attr('checked')) {
                                                parentChecked = true;
                                                return false;
                                            }
                                        });
                                    }

                                    $(c).children().find('.chkfleet').each(function (index) {
                                        if (parentChecked) {
                                            $(this).attr('checked', true);
                                            $(this).attr("disabled", true);
                                        }
                                        else if (FolderSelected(o.SelectedFolders, $(this).attr('data-nodecode'))) {
                                            $(this).attr('checked', true);
                                            o.SelectedFleetIds = UpdateCollection(o.SelectedFleetIds, $(this).parent().find('a:first').attr('fleetid'), true);
                                            o.SelectedFleetName = o.SelectedFolders.split(',').length > 1 ? o.ResMultipleHierarchy : $('a[rel="' + o.SelectedFolders + '"]').html();
                                            o.SelectedFleetPath = UpdateCollection(o.SelectedFleetPath, $(this).parent().find('a:first').attr('fleetpath'), true, "@,@");
                                        }
                                    });

                                    m(o.SelectedFolders, o.SelectedFleetIds, o.SelectedFleetName, o.SelectedFleetPath);


                                }

                                /*if (o.vehicledetails != '') {
                                $('#' + o.vehicledetails + ' table tr.vehicleitem').remove();
                                $('#' + o.vehicledetails + ' table tbody').append(r.vehiclelist);

                                //$("#vehiclelisttbl").trigger("update");
                                $("#" + o.vehicledetails + " table").trigger("update");

                                bindVehiclelist($('#' + o.vehicledetails + ' table'));

                                }*/

                            }
                            else {
                                /*if (o.vehicledetails != '' && s) {
                                $('#' + o.vehicledetails + ' table tr.vehicleitem').remove();
                                $('#' + o.vehicledetails + ' table tbody').append(r.vehiclelist);

                                //$("#vehiclelisttbl").trigger("update");
                                $("#" + o.vehicledetails + " table").trigger("update");

                                bindVehiclelist($('#' + o.vehicledetails + ' table'));
                                }*/
                            }

                            if (o.VehicleListPagingBarId != '' && r.totalVehicles !== '') {
                                //$('#' + o.VehicleListPagingBarId).html(generatePageBar(r.totalVehicles, r.vehiclePageSize, r.vehicleCurrentPage));
                                //generatePageBar(r.totalVehicles, r.vehiclePageSize, r.vehicleCurrentPage);
                            }

                            /// tablesorter code

                            if (o.vehicledetails != '') {
                                //$('#vehiclelisttbl').tablesorter({
                                $("#" + o.vehicledetails + " table").tablesorter({
                                    theme: 'blue',

                                    headers: {
                                        // disable sorting
                                        0: {
                                            // disable it by setting the property sorter to false
                                            sorter: false
                                        },
                                        1: {
                                            // disable it by setting the property sorter to false
                                            sorter: false
                                        }
                                    },

                                    // hidden filter input/selects will resize the columns, so try to minimize the change
                                    widthFixed: true,

                                    // initialize zebra striping and filter widgets
                                    widgets: ["filter"]
                                })
                                .tablesorterPager({

                                    // **********************************
                                    //  Description of ALL pager options
                                    // **********************************

                                    // target the pager markup - see the HTML block below
                                    //container: $('#' + o.VehicleListPagingBarId), //$(".pager"),
                                    container: $(".pager"),

                                    // use this format: "http:/mydatabase.com?page={page}&size={size}&{sortList:col}"
                                    // where {page} is replaced by the page number (or use {page+1} to get a one-based index),
                                    // {size} is replaced by the number of records to show,
                                    // {sortList:col} adds the sortList to the url into a "col" array, and {filterList:fcol} adds
                                    // the filterList to the url into an "fcol" array.
                                    // So a sortList = [[2,0],[3,0]] becomes "&col[2]=0&col[3]=0" in the url
                                    // and a filterList = [[2,Blue],[3,13]] becomes "&fcol[2]=Blue&fcol[3]=13" in the url

                                    //ajaxUrl: '../reports/vehicleListTree.asmx/FetchVehicleListFilterByPage?fleetid=' + currentFleetId + '&page={page}&{filterList:filter}', //'http://mottie.github.io/tablesorter/docs/assets/City{page}.json?{filterList:filter}&{sortList:column}',
                                    ajaxUrl: o.scriptForFetchVehicleByPage + '?fleetid=' + currentFleetId + '&manager=' + o.ManagerColumn + '&page={page}&{filterList:filter}', //'http://mottie.github.io/tablesorter/docs/assets/City{page}.json?{filterList:filter}&{sortList:column}',

                                    // modify the url after all processing has been applied
                                    customAjaxUrl: function (table, url) {
                                        // manipulate the url string as you desire
                                        // url += '&cPage=' + window.location.pathname;
                                        // trigger my custom event
                                        $(table).trigger('changingUrl', url);
                                        // send the server the current page
                                        return url;
                                    },

                                    // add more ajax settings here
                                    // see http://api.jquery.com/jQuery.ajax/#jQuery-ajax-settings
                                    ajaxObject: {
                                        dataType: 'json'
                                    },

                                    // process ajax so that the following information is returned:
                                    // [ total_rows (number), rows (array of arrays), headers (array; optional) ]
                                    // example:
                                    // [
                                    //   100,  // total rows
                                    //   [
                                    //     [ "row1cell1", "row1cell2", ... "row1cellN" ],
                                    //     [ "row2cell1", "row2cell2", ... "row2cellN" ],
                                    //     ...
                                    //     [ "rowNcell1", "rowNcell2", ... "rowNcellN" ]
                                    //   ],
                                    //   [ "header1", "header2", ... "headerN" ] // optional
                                    // ]
                                    // OR
                                    // return [ total_rows, $rows (jQuery object; optional), headers (array; optional) ]
                                    ajaxProcessing: function (data) {
                                        if (o.vehicledetails != '') {
                                            $('#' + o.vehicledetails + ' table tbody tr.vehicleitem').remove();
                                            $('#' + o.vehicledetails + ' table tbody tr.current').remove();
                                            $('#' + o.vehicledetails + ' table tbody').append(data.vehiclelist);

                                            //$("#vehiclelisttbl").trigger("update");
                                            $("#" + o.vehicledetails + " table").trigger("update");

                                            bindVehiclelist($('#' + o.vehicledetails + ' table'));
                                            highlightVehicleList();
                                        }
                                        return [data.totalVehicles, null];
                                    },

                                    // output string - default is '{page}/{totalPages}'; possible variables: {page}, {totalPages}, {startRow}, {endRow} and {totalRows}
                                    //output: '{startRow} to {endRow} ({totalRows})',
                                    output: '{page}/{totalPages}',

                                    // apply disabled classname to the pager arrows when the rows at either extreme is visible - default is true
                                    updateArrows: true,

                                    // starting page of the pager (zero based index)
                                    page: 0,

                                    // Number of visible rows - default is 10
                                    size: o.VehiclePageSize,

                                    // if true, the table will remain the same height no matter how many records are displayed. The space is made up by an empty
                                    // table row set to a height to compensate; default is false
                                    fixedHeight: false,

                                    // remove rows from the table to speed up the sort of large tables.
                                    // setting this to false, only hides the non-visible rows; needed if you plan to add/remove rows with the pager enabled.
                                    removeRows: false,

                                    // css class names of pager arrows
                                    cssNext: '.next',  // next page arrow
                                    cssPrev: '.prev',  // previous page arrow
                                    cssFirst: '.first', // go to first page arrow
                                    cssLast: '.last',  // go to last page arrow
                                    cssPageDisplay: '.pagedisplay', // location of where the "output" is displayed
                                    cssPageSize: '.pagesize', // page size selector - select dropdown that sets the "size" option
                                    cssErrorRow: 'tablesorter-errorRow', // error information row

                                    // class added to arrows when at the extremes (i.e. prev/first arrows are "disabled" when on the first page)
                                    cssDisabled: 'disabled' // Note there is no period "." in front of this class name

                                });

                                //$('#vehiclelisttbl').colResizable({ headerOnly: true });
                                $("#" + o.vehicledetails + " table").colResizable({ headerOnly: true });
                            }


                            /// end  of tablesorter code
                        },
                        error: function (msg) {
                            $(c).removeClass('wait');
                        }
                    });
                }

                function bindVehiclelist(v) {
                    $(v).find('tbody tr td').bind('click', function () {
                        if (o.highlightVehicleSelection) {
                            //$('.vehiclelisttbl tbody tr').removeClass('current');
                            $('#' + o.vehicledetails + ' table tbody tr').removeClass('current');
                            $(this).parent().addClass('current');
                        }
                        h($(this).parent().attr('rel'), $(this).html());
                    });
                }
                function highlightVehicleList() {
                    $("#vehiclelisttbl tr").click(function () {
                        var row = $(this);

                        $('#vehiclelisttbl tr').removeClass('current');
                        $('#vehiclelisttbl tr').addClass("vehicleitem");

                        var VehicleClass = row.hasClass("vehicleitem");
                        var SelectVehicleClass = row.hasClass("current");

                        if (VehicleClass) {
                            row.removeClass("vehicleitem");
                            row.addClass("current");
                        }
                    });
                }
                function menuAction(target) {
                    if (this.data.alias == "gotohierarchy") {
                        //alert($(target).parent().attr('data-vehilceid'));
                        $.ajax({
                            type: 'POST',
                            url: o.RootUrl + "Reports/vehicleListTree.asmx/OrganizationHierarchyGetHierarchyByVehicleId",
                            data: JSON.stringify({ vehicleId: $(target).parent().attr('data-vehilceid') }),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: false,
                            success: function (msg) {
                                var r = eval('(' + msg.d + ')');
                                //alert(r.fleetPath);
                                inifiletree(r.fleetPath, true);
                            },
                            error: function (msg) {

                            }
                        });
                    }
                    else if (this.data.alias == "vehicleinfo") {
                        //alert($(target).parent().attr('data-licenseplate'));
                        jQueryFileTree_SensorInfoWindow(o.RootUrl, $(target).parent().attr('data-licenseplate'));
                    }
                }
                function applyrule(menu) {
                }
                function BeforeContextMenu() {
                    return true;
                }

                function bindTree(t, fleetId) {
                    $(t).find('LI A').bind(o.folderEvent, function () {
                        if ($(this).parent().hasClass('directory')) {
                            $('UL.jqueryFileTree A').removeClass('current');
                            $(this).addClass('current');
                            if ($(this).parent().hasClass('collapsed')) {
                                // Expand
                                if (!o.multiFolder) {
                                    $(this).parent().parent().find('UL').slideUp({ duration: o.collapseSpeed, easing: o.collapseEasing });
                                    $(this).parent().parent().find('LI.directory').removeClass('expanded').addClass('collapsed');
                                }
                                //$(this).parent().find('UL').remove(); // cleanup    //changed to not to cleanup                            
                                showTree($(this).parent(), escape($(this).attr('rel')), true);
                                $(this).parent().removeClass('collapsed').addClass('expanded');
                            } else {
                                // Collapse
                                $(this).parent().find('UL').slideUp({ duration: o.collapseSpeed, easing: o.collapseEasing });
                                $(this).parent().removeClass('expanded').addClass('collapsed');
                                showTree($(this).parent(), escape($(this).attr('rel')), false);
                            }
                            var fleetPath = '';

                            p($(this).attr('rel'), $(this).attr('fleetId'), $(this).html(), $(this).attr('fleetPath'));
                            //currentFleetId = fleetId;

                        } else {
                            h($(this).attr('rel'), $(this).html());
                        }
                        return false;
                    });
                    // Prevent A from triggering the # on non-click events
                    if (o.folderEvent.toLowerCase != 'click') $(t).find('LI A').bind('click', function () { return false; });
                }

                function generatePageBar(total, pageSize, currentPage) {

                    $('#' + o.VehicleListPagingBarId).pagination({
                        items: total,
                        itemsOnPage: pageSize,
                        displayedPages: 3,
                        cssStyle: 'light-theme',
                        prevText: '',
                        nextText: '',
                        onPageClick: onVehiclePageClick
                    });
                }

                /*function onVehiclePageClick(page, e) {
                $.ajax({
                type: 'POST',
                url: o.scriptForFetchVehicleByPage,
                data: JSON.stringify({ fleetid: currentFleetId, page: page }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (msg) {
                var r = eval('(' + msg.d + ')');

                if (o.vehicledetails != '') {
                $('#' + o.vehicledetails + ' table tr.vehicleitem').remove();
                $('#' + o.vehicledetails + ' table tbody').append(r.vehiclelist);

                //$("#vehiclelisttbl").trigger("update");
                $("#" + o.vehicledetails + " table").trigger("update");

                bindVehiclelist($('#' + o.vehicledetails + ' table'));

                }

                },
                error: function (msg) {

                }
                });
                //////////////////////////////////////////
                }*/

                // Loading message
                $(this).html('<ul class="jqueryFileTree start"><li class="wait">' + o.loadMessage + '<li></ul>');
                // Get the initial file list                
                showTree($(this), escape(o.root), true, false);
            });

            //add search feature.
            $('#ohsearchbox').keyup(function () {
                var s = $(this).val();
                if (s.length > 2) {
                    doDelaySearch(s, { searchScript: o.searchScript, scrollSearchResultToBottom: o.scrollSearchResultToBottom });
                }
                else {
                    $('#ohsearchresult ul').html('');
                    $('#ohsearchresult').hide()
                }
            });

            if (o.multipleExpanded != null & o.multipleExpanded != '' && o.MultipleUserHierarchyAssignment) {
                var listExpanded = o.multipleExpanded.split(";");
                MultipleInitialPath = true;
                setTimeout(function () {
                    autoExpand(listExpanded[0], 0);
                }, 100)
            }
            else if (o.expanded != null && o.expanded != '') {
                setTimeout(function () {
                    autoExpand(o.expanded, 0);
                }, 100)
            }

            function autoExpand(s, si) {
                var ld = s.split("/")[si];
                $("A[rel=" + ld + "]").parent().removeClass('expanded').addClass('collapsed');
                $("A[rel=" + ld + "]").click();

                if (si < s.split("/").length - 1) {
                    setTimeout(function () {
                        autoExpand(s, si + 1);
                    }, 100)
                }
                else {
                    //alert($("A[rel=" + ld + "]").parent().position().top);
                    if ($("A[rel=" + ld + "]").parent().position() != null) {
                        $('#LeftPane').animate({
                            scrollTop: $("A[rel=" + ld + "]").parent().position().top
                        }, 1000);
                    }
                    //alert($("A[rel=" + ld + "]").parent().position().top);

                    if (MultipleInitialPath) {
                        MultipleInitialPathIndex++;
                        var listExpanded = o.multipleExpanded.split(";");
                        if (listExpanded.length > MultipleInitialPathIndex) {
                            setTimeout(function () {
                                autoExpand(listExpanded[MultipleInitialPathIndex], 0);
                            }, 100)
                        }
                    }
                }

            }
        }
    });

})(jQuery);

var searching = false;

/*function autoExpand(s, si) {
    var ld = s.split("/")[si];
    $("A[rel=" + ld + "]").parent().removeClass('expanded').addClass('collapsed');
    $("A[rel=" + ld + "]").click();

    if (si < s.split("/").length - 1) {
        setTimeout(function () {
            autoExpand(s, si + 1);
        }, 100)
    }
    else {
        //alert($("A[rel=" + ld + "]").parent().position().top);
        if ($("A[rel=" + ld + "]").parent().position() != null) {
            $('#LeftPane').animate({
                scrollTop: $("A[rel=" + ld + "]").parent().position().top
            }, 1000);
        }
        //alert($("A[rel=" + ld + "]").parent().position().top);
    }
    
}*/

var delayTimer;
function doDelaySearch(s, o) {
    clearTimeout(delayTimer);
    delayTimer = setTimeout(function () {
        searchoh(s, o);
    }, 1500); 
}

function searchoh(s, o) {

    if (!o) var o = {};
    if (o.searchScript == undefined) o.searchScript = 'vehicleListTree.asmx/SearchOrganizationHierarchy';
    if (o.scrollSearchResultToBottom == undefined) o.scrollSearchResultToBottom = false;
    
    if (searching)
        return;

    searching = true;

    $('#ohsearchresult').show();

    if (o.scrollSearchResultToBottom) {
        //$("html, body").animate({ scrollTop: $(document).height() }, 2000);
    }

    $.ajax({
        type: 'POST',
        url: o.searchScript,
        data: JSON.stringify({ searchString: s }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (msg) {

            var r = eval('(' + msg.d + ')');
            $('#ohsearchresultlist').find('ul').html('').append(r.resultList);

            searching = false;
        },
        error: function (msg) {
            $('#ohsearchresultlist').find('ul').html('').append('error');
            searching = false;
        }
    });


}

function onsearchbtnclicked(u) {
    if (!u) var u = 'vehicleListTree.asmx/SearchOrganizationHierarchy';
    
    var s = $('#ohsearchbox').val();
    if (s.length > 2)
        searchoh(s, { searchScript: u });
}

function gotooh(o, checkpreference, p) {
    if (checkpreference == undefined)
        checkpreference = false;

    var reloadTree = true;
    var hideSearchResult = true;

    if (checkpreference) {
        $.ajax({
            type: 'POST',
            url: p,
            data: JSON.stringify({ nodecode: $(o).parent().attr('data-nodecode') }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg) {

                var r = eval('(' + msg.d + ')');
                if (r.status == 0) {
                    var r = confirm(r.resultList);
                    if (r == true) {
                        var scriptAdd = p.replace('ValidateNodeCodeInUserPreference', 'AddNodeCodeInUserPreference');
                        $.ajax({
                            type: 'POST',
                            url: scriptAdd,
                            data: JSON.stringify({ nodecode: $(o).parent().attr('data-nodecode') }),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: false,
                            success: function (msg) {

                                var r = eval('(' + msg.d + ')');
                                if (r.status == 1) {
                                    $('#ohsearchresultlist').find('ul').html(r.resultList);
                                    hideSearchResult = false;
                                    window.location = window.location.pathname;

                                }
                            },
                            error: function (msg) {

                            }
                        });
                    }
                    else
                        reloadTree = false;

                }
            },
            error: function (msg) {

            }
        });
    }

    if(hideSearchResult)
        $('#ohsearchresult').hide();

    if(reloadTree)
        inifiletree($(o).parent().attr('rel'), true);
}

/*function UpdateSelectedFolders(SelectedFolders, nodecode, checked) {
    var slist = [];
    if(SelectedFolders != '') slist = SelectedFolders.split(',');
    if (checked) {
        slist.push(nodecode);
    }
    else {
        //var index = slist.indexOf(nodecode);
        var index = $.inArray(nodecode, slist)
        if (index > -1) {
            slist.splice(index, 1);
        }
    }
    
    return slist.join(',');
}*/

function UpdateCollection(Collection, item, checked, seperator) {
    if (seperator == undefined) seperator = ",";
    var slist = [];
    if (Collection != '') slist = Collection.split(seperator);
    if (checked && $.inArray(item, slist) == -1) {
        slist.push(item);
    }
    else if( !checked ) {
        var index = $.inArray(item, slist)
        if (index > -1) {
            slist.splice(index, 1);
        }
    }

    return slist.join(seperator);
}

function FolderSelected(folders, nodecode) {
    if (nodecode == '') return false;
    var slist = folders.split(',');
    return $.inArray(nodecode, slist) > -1;
}

function ftrim(str, chr) {
    var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^' + chr + '+|' + chr + '+$', 'g');
    return str.replace(rgxtrim, '');
}

function jQueryFileTree_SensorInfoWindow(root, LicensePlate) {
    var mypage = root + 'Map/frmSensorMain.aspx?LicensePlate=' + LicensePlate
    var myname = 'Sensors';
    var w = 525;
    var h = 720;
    var winl = (screen.width - w) / 2;
    var wint = (screen.height - h) / 2;
    winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1'
    win = window.open(mypage, myname, winprops)
    if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
}