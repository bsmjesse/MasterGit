var BSM = BSM || {};

BSM.Controls = function (el) {

    function _daysOfWeekControl() {
        if ($('#Select_Days_of_Week').length === 1) {
            var _parent = $('#Select_Days_of_Week').parent();
            $(_parent).append('<select id="Select_Days_of_Week" multiple="multiple"><option value="1">Monday</option><option value="2">Tuesday</option><option value="3">Wednesday</option><option value="4">Thursday</option><option value="5">Friday</option><option value="4">Saturday</option><option value="12">Sunday</option></select>');
            $('#Select_Days_of_Week').remove();
        };
    };

    function _dateControl(elementId, startDate, maxDate) {
        startDate = startDate || Date.now();
        $('#' + elementId).datepicker();
        $('#' + elementId).datepicker('setDate', startDate);
    };

    function _multipleSelectControl(elementId, dataFeed) {
        if ($('#' + elementId).length === 1) {

            var _data = window[dataFeed]();
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

        };

    }

    function _singleSelectControl(elementId, dataFeed) {
        if ($('#' + elementId).length === 1) {

            var _data = window[dataFeed]();
            var _html = '<br /><select style="width:250px;" id="' + elementId + '">';

            for (var i = 0; i < _data[0].length; i++) {
                _html += '<option value="' + _data[0][i].id + '">' + _data[0][i].title + '</option>';
            }

            _html += '</select>';

            var _parent = $('#' + elementId).parent();
            $('#' + elementId).remove();
            $(_parent).append(_html);
            $('#' + elementId).multipleSelect({
                filter: true,
                single: true
            });

        };

    }

    function _scanControls() {

        if ($('#StartDate').length === 1) {
            _dateControl('StartDate', new Date(+new Date - 12096e5)); // 2 weeks
        }

        if ($('#StartDate_2').length === 1) {
            _dateControl('StartDate_2', new Date(+new Date - 12096e5)); // 2 weeks
        }

        if ($('#EndDate').length === 1) {
            _dateControl('EndDate', new Date());
        }

        if ($('#EndDate_2').length === 1) {
            _dateControl('EndDate_2', new Date());
        }

        //_daysOfWeekControl();
        _multipleSelectControl('Select_Days_of_Week', 'getDaysOfWeek');
        _multipleSelectControl('Fleet_Selector', 'getFleetData');
        _multipleSelectControl('Driver_Select', 'getDriver');
        _multipleSelectControl('Driver_Select_2', 'getDriver');
        _singleSelectControl('Infraction_Category', 'getInfractionCategory');
    }


    return {
        scanControls: _scanControls,

        removeLeft: function () {
            _.each($('.dashlet').parent(), function (data) {
                if (data.getAttribute('componentid') === "BSM_ADHOC_View_TEMPLATE_Report") {
                    $(data).remove();
                }
            });
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