<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmDynamicInspections.aspx.cs" Inherits="HOS_frmDynamicInspections" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Configuration/Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="HosTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery.cookie.js"></script>
    <link rel="stylesheet" href="../scripts/tablesorter2145/css/theme.blue.css">
    <link href="../reports/jqueryFileTree.css" rel="stylesheet" type="text/css" media="screen" />
    <style>
        .GridColumnTitle {
            font-weight: bold;
            text-decoration: underline;
        }

        .style1 {
            width: 45px;
        }


        ul.jqueryFileTree A {
            text-align: left;
        }

        UL.jqueryFileTree A.current {
                background: #0A246A;
                color: White;
        }

        ul.jqueryFileTree a:target {
                background-color: #f00;
        }

        .imgPlus {
            content: url("../Reports/Images/plusicon.png");
        }

        .imgMinus {
            content: url("../Reports/Images/minusicon.png");
        }
    </style>
    <script src="../reports/jqueryFileTree.js?v=20140113" type="text/javascript"></script>
    <script src="../reports/splitter.js?v=2013120601" type="text/javascript"></script>

    <script src="../scripts/tablesorter2145/js/jquery.tablesorter.js"></script>
    <script src="../scripts/tablesorter2145/js/jquery.tablesorter.widgets.min.js"></script>
    <!-- Tablesorter pager: required -->
    <link rel="stylesheet" href="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.css">
    <script src="../scripts/tablesorter2145/addons/pager/jquery.tablesorter.pager.js"></script>

    <script src="../scripts/colResizable-1.3.min.js" type="text/javascript"></script>
    <script src="../scripts/json2.js" type="text/javascript"></script>  
  

</head>
<body topmargin="5px" leftmargin="3px">
    <script language="javascript" type="text/javascript">
        var msgNoQuestionSelected = '<%= msgNoQuestionSelected%>';
        var msgErrorInsert = '<%= msgErrorInsert%>';
        var msgNameIsRequired = '<%= msgNameIsRequired%>';
        var msgSaveSuccessfully = '<%= msgSaveSuccessfully%>';
        var msgSelectaQuestion = '<%= msgSelectaQuestion%>';
        var msgMajor = '<%= msgMajor%>';
        var msgMinor = '<%= msgMinor%>';
        var msgSMCS = '<%= msgSMCS%>';
        var msgConfirmDelete = '<%= msgConfirmDelete%>';

        var curSelectedQuestionId = -1;
        var curSelectedInspectionid = "";
        var var_inspectionJson = "";
        var var_scannableImage = "<img src='../images/qrcode.png' id ='camera' style='width:16px; height:16px;margin-top:-10px;margin-right:5px;margin-left:5px' />";
        var var_configLevels = -1;

        function showQuestionSet(var_categoryJson, inspectionJson) {
           
            var_inspectionJson = inspectionJson;
            $('#tblQuestionButtons').show();
            $('#inspectiontreeview').show();         
            
            if (var_categoryJson.length > 0) {
                for (var index = 0; index < var_categoryJson.length ; index++) {
                    var eleId = var_categoryJson[index]["ID"];
                    var var_questionId = var_categoryJson[index]["QuestionID"];
                    var defectLevel = var_categoryJson[index]["Defect"];
                    if (defectLevel.indexOf('Minor') > 0) {
                        var level = 'Minor'
                        msgMinor = "<span style='color:#008000'>" + level + "</span>"
                        var_categoryJson[index]["Defect"] = var_categoryJson[index]["Defect"].replace("Minor", msgMinor);                        
                    }
                    else if (defectLevel.indexOf('Major') > 0) {
                        var level = 'Major'
                        msjMajor = "<span style='color:#FF0000'>" + level + "</span>"
                        var_categoryJson[index]["Defect"] = var_categoryJson[index]["Defect"].replace("Major", msjMajor);
                    }


                    var element = '<div><img id="plus" class="left_part" src="../Reports/Images/minusicon.png" style="display:none" /img><li class="directory right_part" id = "li_' + eleId + '" question="' + var_questionId + '"><a id = "lia_' + eleId + '" href="javascript:click_inspection(' + eleId + ')">' + var_categoryJson[index]["Defect"];

                    if (var_categoryJson[index]["Scannable"] == "1") {
                        element = element + var_scannableImage + "<span id='loc'>" + var_categoryJson[index]["Location"] + "</span>";
                    }

                    element = element + '</a></li></div>'; 
                    $(element).css('margin-left', '55px').appendTo($('#ulInspections'));                  
                    findInsepctions(eleId, true);

                }
            }
            // Binding the click event
            $('ul #plus').click(function (e) {                

                if ($(this).next().children('#div').hasClass('expand'))
                {
                    $(this).next().children('#div').css('display', 'none');
                    $(this).next().children('#div').removeClass();
                    $(this).next().children('#div').addClass('collapse');
                    //change image source
                    $(this).attr("src", "../Reports/Images/plusicon.png");


                }
                else if ($(this).next().children('#div').hasClass('collapse'))
                {
                    $(this).next().children('#div').css('display', 'block');
                    $(this).next().children('#div').removeClass();
                    $(this).next().children('#div').addClass('expand');
                    //change image source
                    $(this).attr("src", "../Reports/Images/minusicon.png");
                    
                }
            });

        }

        function findInsepctions(inspectionId, isCategory) {
           
            if (var_inspectionJson.length > 0) {
                for (var index = 0; index < var_inspectionJson.length ; index++) {
                    var_parentItemID = var_inspectionJson[index]["ParentItemID"];
                    var var_categoryId = var_inspectionJson[index]["CategoryID"];
                    var var_questionId = var_inspectionJson[index]["QuestionID"];

                    if ((!isCategory && var_parentItemID == inspectionId) || (isCategory && var_parentItemID == "" && var_categoryId == inspectionId)) {
                        var inspectiondefectLevel = var_inspectionJson[index]["Defect"]
                        if (inspectiondefectLevel.indexOf('Minor') > 0) {
                            var level = 'Minor'
                            msgMinor = "<span style='color:#008000'>" + level + "</span>"
                            var_inspectionJson[index]["Defect"] = var_inspectionJson[index]["Defect"].replace("Minor", msgMinor);
                        }
                        else if (inspectiondefectLevel.indexOf('Major') > 0) {
                            var level = 'Major'
                            msjMajor = "<span style='color:#FF0000'>" + level + "</span>"
                            var_inspectionJson[index]["Defect"] = var_inspectionJson[index]["Defect"].replace("Major", msjMajor);
                        }

                        var eleId = var_inspectionJson[index]["ID"];
                        var element = '<div id="div" class="expand"><img id="plus" class="left_part" style="display:none" src="../Reports/Images/minusicon.png" /img><li class="directory collapsed right_part" id = "li_' + eleId + '" question="' + var_questionId + '"><a id = "lia_' + eleId + '" href="javascript:click_inspection(' + eleId + ')">' + var_inspectionJson[index]["Defect"];

                        if (var_inspectionJson[index]["Scannable"] == "1") {
                            element = element + var_scannableImage + "<span id='loc'>" + var_inspectionJson[index]["Location"] + "</span>";
                        }

                        element = element + '</a></li></div>';

                        $(element).css('left', $("#li_" + inspectionId).offset().left + 25).appendTo($('#li_' + inspectionId));
                        $('#li_' + inspectionId).parent().children('#plus').last().css('display', 'block');
                        
                        findInsepctions(eleId, false);
                    }
                   
                }
            }
        }


        function checkQuestionSetName() {
            if ($("#<%= txtName.ClientID %>").val() == '') {
                setMessageText(msgNameIsRequired, true);
                return false;
            }
            $('#lblErrorMessage').html('');
            return true;
        }

        function showLoadingImage(btnId, isEnable) {
            if (isEnable == true) {
                $('#divLoading').css('left', $('#' + btnId).offset().left + $('#' + btnId).width() + 20).css('top', $('#' + btnId).offset().top + 2).show();
                $("input[tag='dynamicInspec']").attr("disabled", "enabled");
            }
            else {
                $('#divLoading').hide();
                $("input[tag='dynamicInspec']").attr("disabled", "");
            }
        }

        function hideLoadingImage(btn) {

            $('#divLoading').hide();
        }

        function setMessageText(msg, isError) {
            $('#lblErrorMessage').html(msg);
            if (isError) $('#lblErrorMessage').css('color', 'red');
            else $('#lblErrorMessage').css('color', 'blue');
        }

        function saveName() {
            
            if (!checkQuestionSetName()) return;
            showLoadingImage('<%= btnSaveName.ClientID%>', true);
            var var_name = $("#<%= txtName.ClientID %>").val();
            var var_type = 0;
            var var_groupId = -1;
            if ($('#<%= hidGroupId.ClientID %>').val() != '') var_groupId = $('#<%= hidGroupId.ClientID %>').val();
            var_type = $("#<%= radTypeList.ClientID %> input:checked").val();
            $.ajax({
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                url: "frmDynamicInspections.aspx/AddOrUpdateInspectionGroup",
                data: JSON.stringify({ groupId: var_groupId, name: var_name, type: var_type }),
                dataType: "json",
                success: function (data) {
                    if (data.d != '-1' && data.d != "0") {
                        if ($('#<%= hidGroupId.ClientID %>').val() == '') $('#<%= hidGroupId.ClientID %>').val(data.d);
                        $('#tblQuestionButtons').show();
                        $('#inspectiontreeview ').show();
                        setMessageText(msgSaveSuccessfully, false);
                    }
                    showLoadingImage('<%= btnSaveName.ClientID%>', false);
                    if (data.d == '-1') {
                        //top.document.all('TopFrame').cols = '0,*';
                        window.open('../Login.aspx', '_top')
                    }
                    if (data.d == '0') {
                        setMessageText(msgErrorInsert, true);
                    }
                },
                error: function (request, status, error) {
                    setMessageText(msgErrorInsert, true);
                    showLoadingImage('<%= btnSaveName.ClientID%>', false);
                }
            });
        }


        function click_inspection(ele) {
            
            if (curSelectedInspectionid != '')
                $('#lia_' + curSelectedInspectionid).removeClass("current");
            curSelectedInspectionid = ele;
            $('#lia_' + curSelectedInspectionid).addClass("current");
            cancelQuestionAction();
        }

        function checkIsSelectItem() {
            if (curSelectedInspectionid == "" || curSelectedInspectionid == "0") {
                setMessageText(msgNoQuestionSelected, true);
                return false;
            }
            return true;
        }

        function editQuestionAction() {
            $('#lblErrorMessage').html('');
            if (!checkIsSelectItem()) return;
            if (curSelectedQuestionId < 0) {
                $('#<%= lblErrorMessageSave.ClientID%>').html(msgSelectaQuestion);
                return;
            }

            var var_groupId = "-1";
            var var_categoryid = "-1";
            var var_inspectionitemId = "-1";
            var var_parentItemId = "-1";
            var var_isCategory = false;
            var var_scannable = false;
            var var_location = "";

            if ($('#<%= hidGroupId.ClientID %>').val() != '') var_groupId = $('#<%= hidGroupId.ClientID %>').val();
             if (curSelectedInspectionid == "" || curSelectedInspectionid == "0") return;
             else {
                 //Find parent Item Id and category Id
                 var parentItemId_s = $('#li_' + curSelectedInspectionid).parent().parent().attr("id");
                 var curSelectedInspectionid_1 = 'li_' + curSelectedInspectionid;
                 while ($('#' + curSelectedInspectionid_1).parent().parent().length > 0 && $('#' + curSelectedInspectionid_1).parent().parent().attr("id") != "ulInspections") {
                     curSelectedInspectionid_1 = $('#' + curSelectedInspectionid_1).parent().parent().attr("id");
                 }
                 if (parentItemId_s == "ulInspections") { //parent is category
                     var_isCategory = true;
                     var_categoryid = curSelectedInspectionid.toString();
                 }
                 else {//parent is inspection 
                     var_categoryid = curSelectedInspectionid_1.replace("li_", "");
                     var_inspectionitemId = curSelectedInspectionid.toString();
                     if (curSelectedInspectionid_1 != parentItemId_s) {
                         var_parentItemId = parentItemId_s.replace("li_", "");
                     }
                 }

                 var duplicateQuestion = false;
                 $('#li_' + curSelectedInspectionid).parent().parent().children("li").each(function () {
                    if ($(this).attr("id") != 'li_' + curSelectedInspectionid && $(this).attr("question") == curSelectedQuestionId.toString()) {
                         alert("<%= msgQuestionBeenUsed %>");
                         duplicateQuestion = true;
                         return;
                     }
                     //alert(this.firstChild.text);
                 });

                 if (duplicateQuestion == true) return;
             }

            showLoadingImage('<%= btnaddQuestionAction.ClientID%>', true);

             if ($('#<%= chkScannable.ClientID %>').attr("checked") == true) {
                 var_scannable = true;
                 var_location = $('#<%= txtLocation.ClientID %>').val();
            }


            $.ajax({
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                url: "frmDynamicInspections.aspx/AddOrUpdateLogData_InspectionGroupItem",
                data: JSON.stringify({
                    groupId: var_groupId,
                    categoryid: var_categoryid,
                    inspectionitemId: var_inspectionitemId,
                    parentItemId: var_parentItemId,
                    questionID: curSelectedQuestionId,
                    isCategory: var_isCategory,
                    path: '',
                    scannable: var_scannable,
                    location: var_location
                }),
                dataType: "json",
                success: function (data) {
                    if (data.d != '-1' && data.d != "0") {
                        var retData = JSON.parse(data.d);
                        var str = retData["Defect"] + ' ' + msgSMCS + ':' + retData["SMCS"];
                        if (retData["DefectLevel"] == 0) {
                            msgMinor = "<span style='color:#008000'>" + msgMinor + "</span>"
                            str = str + " " + msgMinor;
                        }
                        else {
                            msgMajor = "<span style='color:#FF0000'>" + msgMajor + "</span>"
                            str = str + " " + msgMajor;
                        }                        
                        var eleId = curSelectedInspectionid;
                        //var element = '<li class="directory collapsed" id = "li_' + eleId + '" question="' + curSelectedQuestionId + '"><a id = "lia_' + eleId + '" href="javascript:click_inspection(' + eleId + ')">' + str;
                        var var_selecteEle = $('#lia_' + curSelectedInspectionid);
                        var element = '<a id = "lia_' + eleId + '" href="javascript:click_inspection(' + eleId + ')">' + str;
                        if (var_scannable) {
                            element = element + var_scannableImage + "<span id='loc'>" + var_location + "</span>";
                            //element = var_scannableImage + "<span id='loc'>" + var_location + "</span>";
                        }
                        element = element + '</a>';
                        //$('#lia_' + curSelectedInspectionid).text(str);
                        //var_selecteEle.empty();
                        var_selecteEle.replaceWith(element);
                        //$(element).appendTo(var_selecteEle);
                        $('#lia_' + curSelectedInspectionid).addClass("current");
                        setMessageText(msgSaveSuccessfully, false);
                    }
                    showLoadingImage('<%= btnaddQuestionAction.ClientID%>', false);
                    if (data.d == '-1') {
                        //top.document.all('TopFrame').cols = '0,*';
                        window.open('../Login.aspx', '_top')
                    }
                    if (data.d == '0') {
                        setMessageText(msgErrorInsert, true);
                    }
                    cancelQuestionAction();
                },
                error: function (request, status, error) {
                    $('#<%= lblErrorMessageSave.ClientID%>').html(msgErrorInsert);
                    showLoadingImage('<%= btnaddQuestionAction.ClientID%>', false);
                    cancelQuestionAction();
                }
            });
        }

        function addQuestionAction() {
            
            $('#lblErrorMessage').html('');
            if (curSelectedQuestionId < 0) {
                $('#<%= lblErrorMessageSave.ClientID%>').html(msgSelectaQuestion);
                return;
            }

            var var_groupId = "-1";
            var var_categoryid = "-1";
            var var_inspectionitemId = "-1";
            var var_parentItemId = "-1";
            var var_isCategory = false;
            var var_path = "";
            var var_path_1 = "";
            var var_scannable = false;
            var var_location = "";

            if ($('#<%= hidGroupId.ClientID %>').val() != '') var_groupId = $('#<%= hidGroupId.ClientID %>').val();

            var parentItemId_question = $('#ulInspections');
            if (curSelectedInspectionid == "" || curSelectedInspectionid == "0") {
                var_isCategory = true;
            }
            else {
                //Find parent Item Id and category Id
                var parentItemId_s = $('#li_' + curSelectedInspectionid).parent().parent().attr("id");
                var curSelectedInspectionid_1 = 'li_' + curSelectedInspectionid;
                parentItemId_question = $('#li_' + curSelectedInspectionid).parent().parent();
                while ($('#' + curSelectedInspectionid_1).parent().parent().length > 0 && $('#' + curSelectedInspectionid_1).parent().parent().attr("id") != "ulInspections") {
                    if (var_path_1 == '')
                        var_path_1 = curSelectedInspectionid_1.replace("li_", "");
                    else
                        var_path_1 = curSelectedInspectionid_1.replace("li_", "") + '/' + var_path_1;

                    curSelectedInspectionid_1 = $('#' + curSelectedInspectionid_1).parent().parent().attr("id");

                }
                if (parentItemId_s == "ulInspections") { //parent is category
                    var_categoryid = curSelectedInspectionid.toString();
                }
                else {//parent is inspection 
                    var_categoryid = curSelectedInspectionid_1.replace("li_", "");
                    var_parentItemId = curSelectedInspectionid.toString();
                    var_path = var_path_1;
                }
            }

            var duplicateQuestion = false;
            parentItemId_question.children("div").children('li').each(function () {

                    if ($(this).attr("id") != 'li_' + curSelectedInspectionid && $(this).attr("question") == curSelectedQuestionId.toString()) {
                        alert("<%= msgQuestionBeenUsed %>");
                        duplicateQuestion = true;
                        return;
                    }
                    //alert(this.firstChild.text);
            });

            if (duplicateQuestion == true) return;

            showLoadingImage('<%= btnaddQuestionAction.ClientID%>', true);

            if ($('#<%= chkScannable.ClientID %>').attr("checked") == true) {
                var_scannable = true;
                var_location = $('#<%= txtLocation.ClientID %>').val();
            }

            $.ajax({
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                url: "frmDynamicInspections.aspx/AddOrUpdateLogData_InspectionGroupItem",
                data: JSON.stringify({
                    groupId: var_groupId,
                    categoryid: var_categoryid,
                    inspectionitemId: var_inspectionitemId,
                    parentItemId: var_parentItemId,
                    questionID: curSelectedQuestionId,
                    isCategory: var_isCategory,
                    path: var_path,
                    scannable: var_scannable,
                    location: var_location
                }),
                dataType: "json",
                success: function (data) {
                    if (data.d != '-1' && data.d != "0") {
                        var retData = JSON.parse(data.d);
                        var str = retData["Defect"] + ' ' + msgSMCS + ':' + retData["SMCS"];
                        if (retData["DefectLevel"] == 0) {
                            msgMinor = "<span style='color:#008000'>" + msgMinor + "</span>"
                            str = str + " " + msgMinor;
                        }
                        else {
                            msgMajor = "<span style='color:#FF0000'>" + msgMajor + "</span>"
                            str = str + " " + msgMajor;
                        }
                        var eleId = retData["Id"];
                        var element = '<div id="div" class="expand"><img id="plus" class="left_part" style="display:none" src="../Reports/Images/minusicon.png" align="middle"/img><li class="directory right_part" id = "li_' + eleId + '" question="' + curSelectedQuestionId + '"><a id = "lia_' + eleId + '" href="javascript:click_inspection(' + eleId + ')">' + str;

                        if (var_scannable) {
                            element = element + var_scannableImage + "<span id='loc'>" + var_location + "</span>";
                        }
                        element = element + '</a></li></div>';

                        if (curSelectedInspectionid == "" || curSelectedInspectionid == "0") {
                            $(element).css('margin-left', '55px').appendTo($('#ulInspections'));
                        }
                        else {
                            $(element).css('left', $("#li_" + curSelectedInspectionid).offset().left + 25).appendTo($('#li_' + curSelectedInspectionid));

                        }
                        if (curSelectedInspectionid != "" && curSelectedInspectionid != "0") {
                        $('#li_' + curSelectedInspectionid).children('div').css('display', 'block');
                        $('#li_' + curSelectedInspectionid).parent().children('img').css('display', 'block');
                        $('#li_' + curSelectedInspectionid).parent().children('img').attr("src", "../Reports/Images/minusicon.png");
                        }

                        setMessageText(msgSaveSuccessfully, false);
                    }
                    showLoadingImage('<%= btnaddQuestionAction.ClientID%>', false);
                    if (data.d == '-1') {
                        //top.document.all('TopFrame').cols = '0,*';
                        window.open('../Login.aspx', '_top')
                    }
                    if (data.d == '0') {
                        setMessageText(msgErrorInsert, true);
                    }
                    cancelQuestionAction();
                },
                error: function (request, status, error) {
                    $('#<%= lblErrorMessageSave.ClientID%>').html(msgErrorInsert);
                    showLoadingImage('<%= btnaddQuestionAction.ClientID%>', false);
                    cancelQuestionAction();
                }
            });

            // Binding the click event & unbinding the previous event
            $('ul #plus').unbind("click").bind("click",function(){            
                               
                if ($(this).next().children('#div').hasClass('expand')) {                    
                    $(this).next().children('#div').css('display', 'none');
                    $(this).next().children('#div').removeClass();
                    $(this).next().children('#div').addClass('collapse');
                    //change image source
                    $(this).attr("src", "../Reports/Images/plusicon.png");  
        }
                else {
                    if ($(this).next().children('#div').hasClass('collapse')) {                       
                        $(this).next().children('#div').css('display', 'block');
                        $(this).next().children('#div').removeClass();
                        $(this).next().children('#div').addClass('expand');
                        //change image source
                        $(this).attr("src", "../Reports/Images/minusicon.png");   
                    }
                }               
            }); 
        }

        function deleteQuestionAction() {
            
            if (confirm(msgConfirmDelete) == false) return;
            $('#lblErrorMessage').html('');
            if (!checkIsSelectItem()) return;
            showLoadingImage('<%= btnEditQuestion.ClientID%>', true);
            var var_groupId = "-1";
            var var_categoryid = "-1";
            var var_inspectionitemId = "-1";
            var var_parentItemId = "-1";
            var var_isCategory = false;
            if ($('#<%= hidGroupId.ClientID %>').val() != '') var_groupId = $('#<%= hidGroupId.ClientID %>').val();
            if (curSelectedInspectionid == "" || curSelectedInspectionid == "0") return;
            else {
                //Find parent Item Id and category Id
                var parentItemId_s = $('#li_' + curSelectedInspectionid).parent().parent().attr("id");
                var curSelectedInspectionid_1 = 'li_' + curSelectedInspectionid;
                while ($('#' + curSelectedInspectionid_1).parent().parent().length > 0 && $('#' + curSelectedInspectionid_1).parent().parent().attr("id") != "ulInspections") {
                    curSelectedInspectionid_1 = $('#' + curSelectedInspectionid_1).parent().parent().attr("id");
                }
                if (parentItemId_s == "ulInspections") { //parent is category
                    var_isCategory = true;
                    var_categoryid = curSelectedInspectionid.toString();
                }
                else {//parent is inspection 
                    var_categoryid = curSelectedInspectionid_1.replace("li_", "");
                    var_inspectionitemId = curSelectedInspectionid.toString();
                }
            }


            $.ajax({
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                url: "frmDynamicInspections.aspx/DeleteLogData_InspectionGroupItem",
                data: JSON.stringify({
                    groupId: var_groupId,
                    categoryid: var_categoryid,
                    inspectionitemId: var_inspectionitemId,
                    isCategory: var_isCategory
                }),
                dataType: "json",
                success: function (data) {
                    if (data.d != '-1' && data.d != "0") {
                        //Clear selection
                        // Checking the parent has multiple child If not do not remove the image 
                        if ($('#li_' + curSelectedInspectionid).parent().parent().children('div').length <= 1) {
                            $('#li_' + curSelectedInspectionid).parent().parent().siblings().css('display', 'none');

                        }
                        $('#li_' + curSelectedInspectionid).parent().remove();
                        click_inspection('0');
                        setMessageText(msgSaveSuccessfully, false);
                    }
                    showLoadingImage('<%= btnEditQuestion.ClientID%>', false);
                    if (data.d == '-1') {
                        //top.document.all('TopFrame').cols = '0,*';
                        window.open('../Login.aspx', '_top')
                    }
                    if (data.d == '0') {
                        setMessageText(msgErrorInsert, true);
                    }
                },
                error: function (request, status, error) {
                    setMessageText(msgErrorInsert, true);
                    showLoadingImage('<%= btnEditQuestion.ClientID%>', false);
                }
            });
        }

        function clearQuestionSelection() {
            var combo = $find("<%= cboQuestions.ClientID %>");
            combo.clearSelection();
            curSelectedQuestionId = -1;
        }
        function OnQuestionSelectedIndexChanged(sender, eventArgs) {
            $('#lblErrorMessage').html('');
            var item = eventArgs.get_item();
            curSelectedQuestionId = parseInt(item.get_value(), 10);
            if (curSelectedQuestionId == NaN) curSelectedQuestionId = -1;
        }
        function OnchkScannableClicked(ele) {
            if ($(ele).attr("checked") == true) {
                $('#<%= txtLocation.ClientID %>').attr("disabled", "");
            } else {
                $('#<%= txtLocation.ClientID %>').attr("disabled", "enabled");
            }
        }

        function cancelQuestionAction() {
            enableButton(true);
            clearQuestionSelection();
            $('#<%= txtLocation.ClientID %>').val('');
            $('#<%= chkScannable.ClientID %>').attr("checked", false);
            $('#<%= txtLocation.ClientID %>').attr("disabled", "enabled");
            $('#divAddQuestion').hide();
        }

        function enableButton(isEnable) {
            if (isEnable == true) {
                $("input[tag='dynamicInspec']").attr("disabled", "");
            }
            else {
                $("input[tag='dynamicInspec']").attr("disabled", "enabled");
            }
        }

        function showQuestionInpute(isAdd) {
            
            $('#<%= lblErrorMessageSave.ClientID%>').html('');
            $("#<%= btnaddQuestionAction.ClientID%>").unbind("click");
            $('#<%= lblErrorMessage.ClientID%>').html('');
            if (!isAdd) {
                if (!checkIsSelectItem()) return;
                var curSelectedInspectionid_1 = $('#li_' + curSelectedInspectionid);
                var var_questionId = curSelectedInspectionid_1.attr("question");
                var combo = $find("<%= cboQuestions.ClientID %>");
                var itm = combo.findItemByValue(var_questionId);
                itm.select();
                if (curSelectedInspectionid_1.find('#camera').length > 0) {
                    $('#<%= chkScannable.ClientID %>').attr("checked", true);
                    $('#<%= txtLocation.ClientID %>').attr("disabled", "");
                    var var_loc = curSelectedInspectionid_1.find('#loc');
                    if (var_loc != null) $('#<%= txtLocation.ClientID %>').val(var_loc.text());

                }
                var parentItemId_s = $('#li_' + curSelectedInspectionid).parent().parent().attr("id");
                if (parentItemId_s == "ulInspections") { //parent is category
                    EnableScannableAndLocation(true);
                }
                else EnableScannableAndLocation(false);
            }
            else {

                //If it is category then show scannable and location
                if (curSelectedInspectionid == "" || curSelectedInspectionid == "0") {
                    EnableScannableAndLocation(true);
                }
                else {
                    var parentItemId_s = $('#li_' + curSelectedInspectionid).parent().parent().attr("id");
                    var curSelectedInspectionid_1 = 'li_' + curSelectedInspectionid;
                    var var_level = 0;

                    try {
                        if (var_configLevels == -1) {
                            var_configLevels = parseInt($('#<%= hidQuestionSetLevel.ClientID %>').val(), 10);
                            if (var_configLevels == 'NaN') var_configLevels = 3;
                        }
                    }
                    catch (e) { }

                    //Check how many levels.
                    while ($('#' + curSelectedInspectionid_1).parent().parent().length > 0 && $('#' + curSelectedInspectionid_1).parent().parent().attr("id") != "ulInspections") {
                        var_level = var_level + 1;
                        curSelectedInspectionid_1 = $('#' + curSelectedInspectionid_1).parent().parent().attr("id");
                    }

                    if (var_level >= var_configLevels - 1) {
                        $('#<%= lblErrorMessage.ClientID%>').html('<%= msgQuestionSetLevel %>');
                        return;
                    }

                    EnableScannableAndLocation(false);
                }
            }
            $('#divAddQuestion').css('left', ($('#<%= pnlQuestionSet.ClientID %>').width() - $('#divAddQuestion').width()) / 2 + $('#<%= pnlQuestionSet.ClientID %>').offset().left).css('top', $('#<%= pnlQuestionSet.ClientID %>').offset().top + 30).show();
            enableButton(false);
            if (isAdd) {
                $("#<%= btnaddQuestionAction.ClientID%>").bind("click", function () {
                    addQuestionAction()

                    return false;
                });
            }
            else {
                $("#<%= btnaddQuestionAction.ClientID%>").bind("click", function () {
                    editQuestionAction()
                    return false;
                });
            }
          }

        function EnableScannableAndLocation(isEnable) {
            if (isEnable) {
                //$("#<%= chkScannable.ClientID %>").attr("disabled", "");
                //$("#<%= txtLocation.ClientID %>").attr("disabled", "");
                $("#<%= chkScannable.ClientID %>").show();
                $("#<%= txtLocation.ClientID %>").show();
                $("#<%= lblLocation.ClientID %>").show();
                $("#<%= chkScannable.ClientID %>").next('label').show();
            }
            else {
                //$("#<%= chkScannable.ClientID %>").attr("disabled", "enabled");
                //$("#<%= txtLocation.ClientID %>").attr("disabled", "enabled");
                $("#<%= chkScannable.ClientID %>").hide();
                $("#<%= txtLocation.ClientID %>").hide();
                $("#<%= lblLocation.ClientID %>").hide();
                $("#<%= chkScannable.ClientID %>").next('label').hide();
            }
        }
    </script>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
        </telerik:RadAjaxLoadingPanel>
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
            Style="text-decoration: underline">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="gdMedia">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="gdMedia" LoadingPanelID="LoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="pnlQuestionSet" LoadingPanelID="LoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="hidGroupId" LoadingPanelID="LoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="cboQuestions" LoadingPanelID="LoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="btnClose">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="gdMedia" LoadingPanelID="LoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="pnlQuestionSet" LoadingPanelID="LoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <div style="text-align: left; width: 900px">
            <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="300">
                <tr align="left">
                    <td>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs2" runat="server" selectedcontrol="btnHOS" />
                    </td>
                </tr>
                <tr align="left">
                    <td>
                        <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="990">
                            <tr>
                                <td>
                                    <uc2:HosTabs ID="HosTabs1" runat="server" selectedcontrol="cmdQuestionSet" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table id="Table3" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width: 1200px;">
                                        <tr>
                                            <td class="configTabBackground">
                                                <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                    <tr>
                                                        <td>
                                                            <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td>
                                                                        <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                            <tr>
                                                                                <td>
                                                                                    <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 1170px; margin-top: 10px; margin-bottom: 10px" class="tableDoubleBorder">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <table id="Table5" align="center" border="0" cellpadding="0" cellspacing="0"
                                                                                                    style="height: 700px">
                                                                                                    <tr valign="top">
                                                                                                        <td>

                                                                                                            <table id="Table8" cellspacing="0" cellpadding="0" width="870" align="center" border="0">
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <table id="Table9" class="table" width="1170px" height="700px"
                                                                                                                            border="0">
                                                                                                                            <tr>
                                                                                                                                <td class="configTabBackground" style="vertical-align: top; width: 100%">
                                                                                                                                    <table style="vertical-align: top; width: 100%">
                                                                                                                                        <tr>
                                                                                                                                            <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                                                                                                                <telerik:RadGrid AutoGenerateColumns="False" ID="gdMedia" AllowAutomaticDeletes="false"
                                                                                                                                                    PageSize="25" AllowAutomaticInserts="false" AllowSorting="true" AllowAutomaticUpdates="false"
                                                                                                                                                    AllowPaging="True" runat="server" Skin="Hay" GridLines="None" meta:resourcekey="gdMediaResource1"
                                                                                                                                                    Width="900px" AllowFilteringByColumn="true" OnNeedDataSource="gdMedia_NeedDataSource" OnDeleteCommand="gdMedia_DeleteCommand"
                                                                                                                                                    OnItemDataBound="gdMedia_ItemDataBound" OnItemCommand="gdMedia_ItemCommand"
                                                                                                                                                    FilterItemStyle-HorizontalAlign="Left">
                                                                                                                                                    <GroupingSettings CaseSensitive="false" />
                                                                                                                                                    <MasterTableView DataKeyNames="GroupId" ClientDataKeyNames="GroupId" CommandItemDisplay="Top">
                                                                                                                                                        <Columns>
                                                                                                                                                            <telerik:GridTemplateColumn HeaderText="ID" UniqueName="GroupId" SortExpression="GroupId"
                                                                                                                                                                meta:resourcekey="GridTemplateColumnGroupIdResource1" AllowFiltering="false"
                                                                                                                                                                DataField="GroupId">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:Label ID="lblGroupId" CssClass="formtext" runat="server" Text='<%# Bind("GroupId") %>'
                                                                                                                                                                        meta:resourcekey="lblGroupIdResource1">
                                                                                                                                                                    </asp:Label>
                                                                                                                                                                </ItemTemplate>
                                                                                                                                                                <ItemStyle HorizontalAlign="Left" Width="50px" />
                                                                                                                                                                <HeaderStyle HorizontalAlign="Left" Font-Bold="true" Width="50px" />
                                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                                            <telerik:GridTemplateColumn HeaderText="Question Form" UniqueName="Name" SortExpression="Name"
                                                                                                                                                                meta:resourcekey="GridTemplateColumnNameResource1" AllowFiltering="true"
                                                                                                                                                                DataField="Name">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:Label ID="lblName" CssClass="formtext" runat="server" Text='<%# Bind("Name") %>'
                                                                                                                                                                        meta:resourcekey="lblNameResource1">
                                                                                                                                                                    </asp:Label>
                                                                                                                                                                </ItemTemplate>

                                                                                                                                                                <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                                                                                                                                            </telerik:GridTemplateColumn>

                                                                                                                                                            <telerik:GridTemplateColumn AllowFiltering="false">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:LinkButton ID="btnEdit" CommandName="Edit" runat="server"><img src="../images/edit.gif" alt="edit group" /></asp:LinkButton>
                                                                                                                                                                  
                                                                                                                                                                </ItemTemplate>
                                                                                                                                                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                                                                                                                            </telerik:GridTemplateColumn> 
                                                                                                                                                            <telerik:GridTemplateColumn AllowFiltering="false">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:LinkButton ID="btnDelete" CommandName="Delete" ConfirmTitle ="Delete" runat="server"><img src="../images/delete.gif" alt="delete group" /></asp:LinkButton>                                                                                                                                                                  
                                                                                                                                                                </ItemTemplate>
                                                                                                                                                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                                                                                                                            </telerik:GridTemplateColumn>  
                                                                                                                                                        </Columns>
                                                                                                                                                        <HeaderStyle CssClass="RadGridtblHeader" />
                                                                                                                                                        <CommandItemTemplate>
                                                                                                                                                            <table width="100%">
                                                                                                                                                                <tr>
                                                                                                                                                                    <td align="left">
                                                                                                                                                                        <asp:Button ID="btnAdd" Text="New Question Form" runat="server" CssClass="combutton" Width="150px"
                                                                                                                                                                            meta:resourcekey="btnAddResource1" OnClick="btnAdd_Click" />
                                                                                                                                                                    </td>
                                                                                                                                                                    <td align="right">
                                                                                                                                                                        <nobr>
                                                                                                                                <asp:ImageButton ID="imgFilter" runat="server" OnClientClick ="javascript:return showFilterItem();" ImageUrl="~/images/filter.gif" />
                                                                                                                                <asp:LinkButton ID="hplFilter" runat="server"  OnClientClick ="javascript:return showFilterItem();" Text="Show Filter" meta:resourcekey="hplFilterResource1" Font-Underline="true"></asp:LinkButton>
                                                                                                                                </nobr>
                                                                                                                                                                    </td>
                                                                                                                                                                </tr>
                                                                                                                                                            </table>
                                                                                                                                                        </CommandItemTemplate>
                                                                                                                                                    </MasterTableView>

                                                                                                                                                    <ClientSettings>
                                                                                                                                                        <ClientEvents OnGridCreated="GridCreated" />
                                                                                                                                                    </ClientSettings>
                                                                                                                                                </telerik:RadGrid>
                                                                                                                                                <asp:Panel ID="pnlQuestionSet" runat="server" Visible="false">
                                                                                                                                                    <table style="vertical-align: top; width: 100%">
                                                                                                                                                        <tr>
                                                                                                                                                            <td>
                                                                                                                                                                <table style="border-width: 1px; border-color: black; border-style: solid; width: 100%">
                                                                                                                                                                    <tr>
                                                                                                                                                                        <td style="width: 150px">

                                                                                                                                                                            <asp:Label ID="lblName" runat="server" CssClass="tableheading"
                                                                                                                                                                                Text="Question Form Name: " meta:resourcekey="lblNameResource1"></asp:Label>
                                                                                                                                                                            
                                                                                                                                                                        </td>
                                                                                                                                                                        <td style="text-align: left">
                                                                                                                                                                            <asp:TextBox ID="txtName" runat="server" Width="400px" meta:resourcekey="txtNameResource1"></asp:TextBox>
                                                                                                                                                                        </td>
                                                                                                                                                                        <td>
                                                                                                                                                                            <asp:Label ID="lblType" runat="server" CssClass="tableheading" Style="display: none"
                                                                                                                                                                                Text="Type: " meta:resourcekey="lblTypeResource1"></asp:Label>
                                                                                                                                                                        </td>
                                                                                                                                                                        <td>
                                                                                                                                                                            <asp:RadioButtonList ID="radTypeList" runat="server" CssClass="formtext" Style="display: none" meta:resourcekey="radTypeListResource1" RepeatDirection="Horizontal">
                                                                                                                                                                                <asp:ListItem Value="0" meta:resourcekey="radTypeListAllResource" Selected="true" Text="Both"></asp:ListItem>
                                                                                                                                                                                <asp:ListItem Value="1" meta:resourcekey="radTypeListPreTripResource" Text="Pre trip"></asp:ListItem>
                                                                                                                                                                                <asp:ListItem Value="2" meta:resourcekey="radTypeListPoatTripResource" Text="Post trip"></asp:ListItem>
                                                                                                                                                                            </asp:RadioButtonList>

                                                                                                                                                                        </td>
                                                                                                                                                                        <td>
                                                                                                                                                                            <asp:Button CssClass="kd-button" Style="width: 130px;"
                                                                                                                                                                                OnClientClick="saveName();return false;" ID="btnSaveName" runat="server"
                                                                                                                                                                                Text="Save Name" meta:resourcekey="btnSaveNameResource1" />
                                                                                                                                                                            

                                                                                                                                                                        </td>
                                                                                                                                                                        <td>
                                                                                                                                                                            <asp:Button CssClass="kd-button" Style="width: 130px;"
                                                                                                                                                                                ID="btnClose" runat="server" OnClick="btnClose_Click"
                                                                                                                                                                                Text="Close" meta:resourcekey="btnCloseResource1" />

                                                                                                                                                                        </td>
                                                                                                                                                                    </tr>
                                                                                                                                                                </table>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td align="center">
                                                                                                                                                                <table id="tblQuestionButtons" style="display: none">
                                                                                                                                                                    <tr>
                                                                                                                                                                        <td>
                                                                                                                                                                            <table>
                                                                                                                                                                                <tr>
                                                                                                                                                                                    <td>
                                                                                                                                                                                        <asp:Button CssClass="kd-button" OnClientClick="showQuestionInpute(true);return false;"
                                                                                                                                                                                            ID="btnAddQuestion" runat="server" Text="Add Question" Style="width: 150px;"
                                                                                                                                                                                            meta:resourcekey="btnAddQuestionResource1" />
                                                                                                                                                                                    </td>
                                                                                                                                                                                    <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                                                                                    </td>

                                                                                                                                                                                    <td>
                                                                                                                                                                                        <asp:Button CssClass="kd-button" Style="width: 150px;"
                                                                                                                                                                                            OnClientClick="showQuestionInpute(false);return false;" ID="btnEditQuestion" runat="server"
                                                                                                                                                                                            Text="Change Question" meta:resourcekey="btnEditQuestionResource1" />
                                                                                                                                                                                    </td>
                                                                                                                                                                                    <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                                                                                    </td>

                                                                                                                                                                                    <td>
                                                                                                                                                                                        <asp:Button CssClass="kd-button" Style="width: 150px;"
                                                                                                                                                                                            OnClientClick="deleteQuestionAction();return false;" ID="btnDeleteQuestion" runat="server"
                                                                                                                                                                                            Text="Delete Question" meta:resourcekey="btnDeleteQuestionResource1" />
                                                                                                                                                                                    </td>
                                                                                                                                                                                </tr>
                                                                                                                                                                            </table>
                                                                                                                                                                        </td>

                                                                                                                                                                    </tr>
                                                                                                                                                                </table>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td style="text-align: center">
                                                                                                                                                                <asp:Label ID="lblErrorMessage" runat="server" CssClass="errortext" Width="615px" meta:resourcekey="lblErrorMessageResource1"></asp:Label>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                        <tr>
                                                                                                                                                            <td>
                                                                                                                                                                <!-- <div id="inspectiontreeview" class="demo"></div> -->                                                                                                                                                                
                                                                                                                                                                <div runat="server" style="width: 100%; height: 600px; overflow: scroll; display: none" id="inspectiontreeview" class="accord">
                                                                                                                                                                    <ul style="margin-left: 10px" id="ulInspections" class="jqueryFileTree">
                                                                                                                                                                        <li class="directory collapsed" id="li_0"><a id="lia_0" href="javascript:click_inspection('0')"><%= msgQuestionSet%></a></li>
                                                                                                                                                                    </ul>
                                                                                                                                                                </div>
                                                                                                                                                            </td>
                                                                                                                                                        </tr>
                                                                                                                                                    </table>
                                                                                                                                                </asp:Panel>
                                                                                                                                            </td>
                                                                                                                                        </tr>
                                                                                                                                    </table>
                                                                                                                                </td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divLoading" class="loadingdiv" border="0" style="display: none; z-index: 1000; position: absolute; width: 20px; height: 20px">
            <image src="../images/loading2.gif"></image>
        </div>

        <div id="divAddQuestion" class="popupdiv" border="0" style="display: none; z-index: 1000; position: absolute">
            <table>
                <tr>
                    <td>
                        <telerik:RadComboBox ID="cboQuestions" runat="server" Height="200px" Width="600px" DropDownWidth="750px"
                            AllowCustomText="true" OnClientSelectedIndexChanged="OnQuestionSelectedIndexChanged"
                            EmptyMessage="Select a Question" HighlightTemplatedItems="true" Filter="Contains" DataTextField="Defect"
                            DataValueField="RowID" AppendDataBoundItems="true"
                            meta:resourcekey="cboQuestionsResource1">
                            <Items>
                                <telerik:RadComboBoxItem Text="Select a Question" Value="-1" />

                            </Items>

                            <HeaderTemplate>
                                <table cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td style="width: 490px;" class="RegularText">
                                            <asp:Label ID="lbQuestion" CssClass="formtext" runat="server" Text='Question'
                                                meta:resourcekey="lbQuestionResource1">
                                            </asp:Label>

                                        </td>
                                        <td style="width: 120px;" class="RegularText">
                                            <asp:Label ID="lblSMCS" CssClass="formtext" runat="server" Text='SMCS'
                                                meta:resourcekey="lblSMCSResource1">
                                            </asp:Label>

                                        </td>
                                        <td style="width: 70px;" class="RegularText">
                                            <asp:Label ID="lblLevel" CssClass="formtext" runat="server" Text='Level'
                                                meta:resourcekey="lblLabourResource1">
                                            </asp:Label>

                                        </td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td style="width: 530px;" class="RegularText">
                                            <asp:Label ID="lblDefect" CssClass="formtext" runat="server" Text='<%# Bind("Defect") %>'
                                                meta:resourcekey="lblDefectResource1">
                                            </asp:Label>
                                        </td>
                                        <td style="width: 130px;" class="RegularText">
                                            <asp:Label ID="lblSMCSCode" CssClass="formtext" runat="server" Text='<%# Bind("SMCSCode") %>'
                                                meta:resourcekey="lblSMCSCodeResource1">
                                            </asp:Label>
                                        </td>
                                        <td style="width: 110px;" class="RegularText">
                                            <asp:Label ID="lbl1Level" runat="server" Text='<%# Bind("Level") %>'
                                                meta:resourcekey="lblDefectLevelResource1"></asp:Label>
                                            </td>
                                        <td>
                                            <asp:HiddenField ID="hidDefectLevel" runat="server" Value='<%# Bind("DefectLevel") %>'
                                                meta:resourcekey="lblDefectLevelResource1"></asp:HiddenField>
                                            <asp:HiddenField ID="hidRowId" runat="server" Value='<%# Bind("RowID") %>'
                                                meta:resourcekey="lblDefectLevelResource1"></asp:HiddenField>
                                            
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </telerik:RadComboBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkScannable" runat="server" Text="Scannable" meta:resourcekey="chkScannableResource1" />
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;&nbsp;
                                                                                                                                                                                        <asp:Label ID="lblLocation" runat="server" Text="Location" meta:resourcekey="lblLocationResource1" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtLocation" runat="server" Text="" Width="200px" Enabled="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="6" align="center">
                        <table>
                            <tr>
                                    <td valign="bottom" colspan="2" align="center">
                                        <asp:Button CssClass="kd-button" OnClientClick="javascript:return false;"
                                            ID="btnaddQuestionAction" runat="server"
                                            Text="Save" Style="width: 75px;" meta:resourcekey="btnaddQuestionSetActionResource2" />
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button CssClass="kd-button"
                                OnClientClick="cancelQuestionAction();return false;" ID="btncancelQuestionAction" runat="server"
                                Text="Cancel" Style="width: 75px;" meta:resourcekey="btncancelQuestionActionResource2" />
                                    </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="6" align="center">
                    <asp:Label ID="lblErrorMessageSave" runat="server" CssClass="errortext" Width="615px" meta:resourcekey="lblErrorMessageResource1"></asp:Label>
                    </td>
                </tr>
                <tr>

                    <td>
                        <br />
                        <br />
                    </td>
                    </tr>
            </table>
        </div>

        <asp:HiddenField ID="hidFilter" runat="server" />
        <asp:HiddenField ID="hidGroupId" runat="server" />
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            <script type="text/javascript">
                function showFilterItem() {
                    if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                        setTimeout("SetGridFilterWidth()", 10);
                        $find('<%=gdMedia.ClientID %>').get_masterTableView().showFilterItem();
                        $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                        $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");

                    }
                    else {
                        $find('<%=gdMedia.ClientID %>').get_masterTableView().hideFilterItem();
                        $telerik.$("#<%= hidFilter.ClientID %>").val('');
                        $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                    }
                    return false;
                }


                function GridCreated() {
                    if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                        $find('<%=gdMedia.ClientID %>').get_masterTableView().hideFilterItem();
                        $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                    }
                    else {
                        $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                    }
                    setTimeout("SetGridFilterWidth()", 10);
                }
                function SetGridFilterWidth() {
                    $telerik.$(".rgFilterBox[type='text']").each(function () {
                        if ($telerik.$(this).css("visibility") != "hidden") {
                            var buttonWidth = 0;
                            if ($telerik.$(this).next("[type='submit']").length > 0) {
                                buttonWidth = $telerik.$(this).next("[type='submit']").width();
                            }
                            if ($telerik.$(this).next("[type='button']").length > 0) {
                                buttonWidth = $telerik.$(this).next("[type='button']").width();
                            }

                            if (buttonWidth > 0) {
                                $telerik.$(this).width($telerik.$(this).parent().width() - buttonWidth - 10);
                            }
                            else {
                                $telerik.$(this).width($telerik.$(this).parent().width() - 50);
                            }
                        }
                    })
                    hasSetWidth = true;
                }

            </script>
        </telerik:RadCodeBlock>
        <asp:HiddenField ID="hidQuestionSetLevel" runat="server" />
    </form>
</body>
                                                                                                                                                                
</html>

