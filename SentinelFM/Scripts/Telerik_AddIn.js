//devin
function GetRadWindow() {
    w = window;
    if (parent != null) w = parent.window;

    var oWindow = null;
    try {
        if (w.radWindow) oWindow = w.radWindow;
        else if (w.frameElement.radWindow) oWindow = w.frameElement.radWindow;
    }
    catch (err) { }
    return oWindow;
}

function WinClose() {
    var oWnd = GetRadWindow();
    if (oWnd != null) {
        oWnd.close();
    }
    else {
        top.close();
    }
    return false;
}


function showFilterItemScript(hidFilterID, gridID, hplFilter, hideFilterText, showFilterText) {
    if ($telerik.$("#" + hidFilterID).val() == '') {
        $find(gridID).get_masterTableView().showFilterItem();
        $telerik.$("#" + hidFilterID).val('1');
        $telerik.$("a[id$='" + hplFilter + "']").text(hideFilterText);
    }
    else {
        $find(gridID).get_masterTableView().hideFilterItem();
        $telerik.$("#" + hidFilterID).val('');
        $telerik.$("a[id$='" + hplFilter + "']").text(showFilterText);

    }
    return false;
}

function SetFilterWhenCreatedScript(hidFilterID, gridID, hplFilter, hideFilterText, showFilterText) {

    if ($telerik.$("#" + hidFilterID).val() == '') {
        $find(gridID).get_masterTableView().hideFilterItem();
        $telerik.$("a[id$='" + hplFilter + "']").text(showFilterText);
    }
    else {
        $find(gridID).get_masterTableView().showFilterItem();
        $telerik.$("a[id$='" + hplFilter + "']").text(hideFilterText);
    }
}

function ShowToolTipScreen(flagWidth, content, ctl) {
    $telerik.$("div#ui-slider-tooltip-for-screen").remove();
    var oTip = $telerik.$("<div id='ui-slider-tooltip-for-screen' class='ui-slider-tooltip  ui-corner-all'></div>");
    var oPointer = $telerik.$("<div class='ui-tooltip-pointer-down'><div class='ui-tooltip-pointer-down-inner'></div></div>");
    var oTipInfo = $telerik.$("<div >" + content + "</div>").attr("class", "ui-tooltip-font").css("width", flagWidth + "px");
    var oTipCLose = $telerik.$("<div style='text-align:right;color:white !important;' ><span style='cursor: hand !important;color:white !important;'  href='#' onclick='RemoveToolTipScreen(this)' >X</span></div>").attr("class", "ui-tooltip-font-a-x ui-tooltip-font hbg");
    //oToolTip = $telerik.$(oTip).append(oTipCLose).append(oTipInfo).append(oPointer);
    oToolTip = $telerik.$(oTip).append(oTipCLose).append(oTipInfo);


    $telerik.$(oToolTip).fadeIn("slow");
    $telerik.$(ctl).after(oToolTip);

    var position = $telerik.$(ctl).position();
    var x = $telerik.$(ctl).offset().left;
    var y = $telerik.$(ctl).offset().top;
    var oTipTop = eval(position.top - $telerik.$(oTip).outerHeight() - 8);
    if (oTipTop <= 0) oTipTop = 8;
    //else $telerik.$(oToolTip).append(oPointer);
    var oTipLeft = position.left;
    $telerik.$(oToolTip).css("top", oTipTop + "px").css("left", oTipLeft + "px");
}

function RemoveToolTipScreen(ctl) {
    $telerik.$(ctl).parent().parent().remove();
}
