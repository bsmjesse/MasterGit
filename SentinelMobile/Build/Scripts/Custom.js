$(document).ready(function() {
    $('#myabout').click(function () {
        //alert("test test");
        $('#myaboutul').append('<li data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="c" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li ui-last-child ui-btn-up-c"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a data-panel="main" href="#aboutsv" class="ui-link-inherit">About test</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>');
        $('#myaboutul').append('<li data-corners="false" data-shadow="false" data-iconshadow="true" data-wrapperels="div" data-icon="arrow-r" data-iconpos="right" data-theme="c" class="ui-btn ui-btn-icon-right ui-li-has-arrow ui-li ui-last-child ui-btn-up-c"><div class="ui-btn-inner ui-li"><div class="ui-btn-text"><a data-panel="main" href="#aboutsv" class="ui-link-inherit">About test 2</a></div><span class="ui-icon ui-icon-arrow-r ui-icon-shadow">&nbsp;</span></div></li>');
    });

    $("div.ui-input-search").css("margin-left", "20px");
    $("div.ui-input-search").css("margin-right", "20px");
    $("form.ui-listview-filter").css("margin-bottom", "0px");

});