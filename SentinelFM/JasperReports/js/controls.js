var BSM = BSM || {};


BSM.Controls = function (el) {

    function _dateControl(elementId, startDate, maxDate) {
        var dateRange = moment().subtract(14, 'days').format("MM/DD/YYYY") + ' - ' + moment().format("MM/DD/YYYY") ;
        $('#StartDate').val(dateRange);
        //$('#' + elementId).datepicker('setDate', startDate);
    };

    function _multipleSelectControl(elementId, dataFeed, defaultData) {
        if ($('#' + elementId).length === 1) {

            var _data = dataFeed;
            var _html = '<br /><select id="' + elementId + '" multiple="multiple">';
            if (_data.length === 1) // only one group
            {
                for (var i = 0; i < _data[0].length; i++) {
                    _html += '<option value="' + _data[0][i].id + '">' + _data[0][i].title + '</option>';
                }
            }
            else if (_data.length > 1) {
                for (var i = 0; i < _data.length; i++) {
                    _html += '<optgroup label="' + _data[i][0].groupName + '">';
                    for (var j = 0; j < _data[i][0].data.length; j++) {
                        _html += '<option value="' + _data[i][0].data[j].id + '">' + _data[i][0].data[j].title + '</option>';
                    }
                    _html += '</optgroup>';
                }

            }
            _html += '</select> <p id="eventResult" class="hide">Here is the result of event.</p>';
            var _parent = $('#' + elementId).parent();
            $('#' + elementId).remove();
            $(_parent).append(_html);
            var $eventResult = $('#eventResult');
            jQuery('#' + elementId).multipleSelect({
                filter: true,
                width: '100%',
                onOpen: function () {
                    console.log('Select opened!');
                },
                onClose: function () {
                    console.log('Select closed!');
                },
                onCheckAll: function () {
                    console.log('Check all clicked!');
                },
                onUncheckAll: function () {
                    console.log('Uncheck all clicked!');
                },
                onFocus: function () {
                    console.log('focus!');
                },
                onBlur: function () {
                    console.log('blur!');
                },
                onOptgroupClick: function (view) {
                    var values = $.map(view.children, function (child) {
                        return child.value;
                    }).join(', ');
                    BSM.fleets_selected = values;
                    $eventResult.text('Optgroup ' + view.label + ' ' +
                        (view.checked ? 'checked' : 'unchecked') + ': ' + values);
                },
                onClick: function (view) {
                    $eventResult.text(view.label + '(' + view.value + ') ' +
                        (view.checked ? 'checked' : 'unchecked'));
                }
            });

            if (defaultData != undefined && defaultData != '') {
                jQuery('#' + elementId).val(defaultData);
                //jQuery('#' + elementId).multipleSelect('setSelects', [1]);
                jQuery('#' + elementId).multipleSelect('refresh');
            }            

        };

    }

    function _singleSelectControl(elementId, dataFeed, defaultData) {
        if ($('#' + elementId).length === 1) {

            var _data = dataFeed;
            var _html = '<br /><select style="width:250px;" id="' + elementId + '">';

            for (var i = 0; i < _data[0].length; i++) {
                _html += '<option value="' + _data[0][i].id + '">' + _data[0][i].title + '</option>';
            }

            _html += '</select>';

            var _parent = $('#' + elementId).parent();
            $('#' + elementId).remove();
            $(_parent).append(_html);
            jQuery('#' + elementId).multipleSelect({
                filter: true,
                single: true
            });

            if (defaultData != undefined && defaultData != '') {
                jQuery('#' + elementId).val(defaultData);
                jQuery('#' + elementId).multipleSelect('refresh');
            }

        };

    }

    function _scanControls() {

        if ($('#StartDate').length === 1) {
            //_dateControl('StartDate', new Date(+new Date - 12096e5)); // 2 weeks
            _dateControl('StartDate', new Date(2015, 6, 1));
        }

        if ($('#StartDate_2').length === 1) {
            _dateControl('StartDate_2', new Date(+new Date - 12096e5)); // 2 weeks
        }

        if ($('#EndDate').length === 1) {
            //_dateControl('EndDate', new Date());
            _dateControl('EndDate', new Date(2015, 6, 31));
        }

        if ($('#EndDate_2').length === 1) {
            _dateControl('EndDate_2', new Date());
        }

        //_daysOfWeekControl();
        _multipleSelectControl('Select_Days_of_Week', getDaysOfWeek(), ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]);
        _multipleSelectControl('Fleet_Selector', getFleetData(), ["124552"]);
        _multipleSelectControl('Driver_Select', getDriver());
        _multipleSelectControl('Driver_Select_2', getDriver());
        _singleSelectControl('Infraction_Category', getInfractionCategory(), "Violation");
        _multipleSelectControl('Infractions_List', getInfractionList());
    }

        function _onToggleLoadDateRangePicker() {
            var $j = jQuery.noConflict();
            $('#StartDate').daterangepicker({
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                },
                startDate: moment([2015, 6, 1]),
                endDate: moment([2015, 6, 31])
            }); //Success
            $('#EndDate').remove();
            $('label[for=StartDate]').text('Start and End Date');
        }

    function getFleetData(){
        //return <%=FLEET_DATA%>;
        return [[{ id: 4670, title: "Pilot Fleet" }, { id: 4733, title: "Chicago Fleet" }, { id: 4734, title: "Ft. Worth" }]];
    }

    function getDaysOfWeek() {
        return [[{ groupName: 'Weekday', data: [{ id: "Monday", title: "Monday" }, { id: "Tuesday", title: "Tuesday" }, { id: "Wednesday", title: "Wednesday" }, { id: "Thursday", title: "Thursday" }, { id: "Friday", title: "Friday" }] }], [{ groupName: 'Weekend', data: [{ id: "Saturday", title: "Saturday" }, { id: "Sunday", title: "Sunday" }] }]];
    }

    function getDriver() {
        //return [[{ id: 1, title: "Franklin" }, { id: 2, title: "Tom" }, { id: 3, title: "John" }, { id: 4, title: "Angela" }, { id: 5, title: "William" }, { id: 6, title: "Georage" }]];
        return BSM.drivers;
    }

    function getInfractionCategory() {
        return [[{ id: "Alarm", title: "Alarm" }, { id: "Diagnostic", title: "Diagnostic" }, { id: "Diagnostic:(Custom)", title: "Diagnostic:(Custom)" }, { id: "DTC", title: "DTC" }, { id: "Violation", title: "Violation" }, { id: "Violation:(Custom)", title: "Violation:(Custom)" }]];
    }

    function getInfractionList() {
        return [[{ id: 3, title: "Speed" }, { id: 35, title: "Speeding" }, { id: 37, title: "ServiceRequired" }, { id: 55, title: "HopperDoorsTamper" }, { id: 58, title: "HarshBraking" }, { id: 59, title: "ExtremeBraking" }, { id: 60, title: "HarshAcceleration" }, { id: 61, title: "ExtrAcceleration" }, { id: 62, title: "SeatBelt" }, { id: 108, title: "ReverseExcessDistance" }, { id: 111, title: "HighRailSpeed" }, { id: 113, title: "ReverseHyRailExcessSpeed" }, { id: 119, title: "Harsh Drive" }, { id: 503, title: "Speed Event" }, { id: 506, title: "HarshBraking" }, { id: 507, title: "ExtremeBraking" }, { id: 508, title: "HarshAcceleration" }, { id: 509, title: "ExtrAcceleration" }, { id: 515, title: "Speed Bucket" }, { id: 521, title: "Speed In Landmark" }, { id: 529, title: "Idle in Landmark" }, { id: 531, title: "Seatbelt" }, { id: 558, title: "Driver class" }]];
    }


    return {
        scanControls: _scanControls,

        onToggle: _onToggleLoadDateRangePicker,

        removeLeft: function () {
            _.each($('.dashlet').parent(), function (data) {
                if ($(data).data('componentid') === "BSM_ADHOC_View_TEMPLATE_Report") {
                    $(data).remove();
                }
            });
        },

        humanize: function () {
            _.each($('label'), function (label) {
                $(label).text(S($(label).text).humanize());
                
            });
        },

        applyDateRangePicker: function(){
            $('#StartDate').daterangepicker();
        },

        isValidDate: function (d) {
            if (Object.prototype.toString.call(d) === "[object Date]") {
                // it is a date
                if (isNaN(d.getTime())) {  // d.valueOf() could also work
                    // date is not valid
                }
                else {
                    console.log('valid date');// date is valid
                }
            }
            else {
                // not a date
                console.log('not a valid date');
            }
        }
    }
}();