<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewMessageWithFormat.aspx.cs" Inherits="SentinelFM.NewMessageWithFormat" UICulture="auto" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html;charset=windows-1252"/> 
	<title><%= PageTitle%></title> 
	<script src="../Assets/Scripts/prototype.js"  type="text/javascript"></script>
    <script src="../Assets/Scripts/EmailSystem/geoNavValidation.js" type="text/javascript"></script>
    <link href="../Assets/Styles/EmailSystem/NewMessage.css" rel="stylesheet" type="text/css" />
   
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
	<script type="text/javascript">
	    var userID;
	    var userName;
	    var isPostBack = false;
	    var defaultMessageFormatId;
	    var TotalMessagesToSend = 0;
	    var asynchronousCalls;
	    var allVehiclesId;
	    var wDim;
	    var isValid = true;
	  
	    document.observe('dom:loaded', function () {
	        var vpDim = document.viewport.getDimensions();
	        Position.GetWindowSize = function (w) {
	            var width, height;
	            w = w ? w : window;
	            this.width = w.innerWidth || (w.document.documentElement.clientWidth || w.document.body.clientWidth);
	            this.height = w.innerHeight || (w.document.documentElement.clientHeight || w.document.body.clientHeight);
	            return this;
	        }
	        wDim = Position.GetWindowSize();
	    }); 

	    if (Prototype.Browser.IE) {
	        new PeriodicalExecuter(callback, 1);
	        function callback(pe) {
	            if (pe) pe.stop();
	            OnLoad();

	        };
	    } else {
	        window.onload = function () {
	            OnLoad();
	        }
	    }
	    //*******************************************************************************************************************************
	    //***************************************************** Get fields format start *************************************************
	    //*******************************************************************************************************************************
	    function OnLoad() {
	        document.getElementById("inpFrom").style.width = (wDim.width - 170).toString() + "px";
	        document.getElementById("rcbToGroup").style.width = (wDim.width - 164).toString() + "px";
	        document.getElementById("rcbRecipient").style.width = (wDim.width - 164).toString() + "px";
	        document.getElementById("rcbFormat").style.width = (wDim.width - 164).toString() + "px";
	        document.getElementById("messageFormatDiv").style.height = (wDim.height - 210).toString() + "px";
	        document.getElementById("messageFormatDiv").style.width = (wDim.width - 40).toString() + "px";
	        document.getElementById("composeActions").style.left = (wDim.width / 2 - 75).toString() + "px";

	        $("ButtonSend").value = '<%= Button_Send%>';
	        $("ButtonCancel").value = '<%= Button_Cancel%>';

	        loadFormTemplateComplete();
	    }

	    function loadFormTemplateComplete() {
            ConfigureDialog();
	        GetGroupList();
	        GetMessageFormatsList();
	    };
	    //*******************************************************************************************************************************
	    //***************************************************** Get fields format end ***************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //***************************************** Get group select information start **************************************************
	    //*******************************************************************************************************************************
	    function GetGroupList() {
	        EmailSystem.GetGroupList(userID, loadFormInformationComplete, loadFormInformationCompleteError);
	    }

	    function loadFormInformationComplete(transport) {
           
	        var combo = $find("<%= rcbToGroup.ClientID %>");

	        if (null != combo) {
	            var comboItems = combo.get_items();
	            var length = comboItems.get_count();

	            for (var i = 0; i < length; i++) {
	                var item = comboItems.getItem(0);
	                combo.trackChanges();
	                comboItems.remove(item);
	                combo.commitChanges();
	            }

	            var mySplitResult = transport.split("|");

	            for (i = 0; i < mySplitResult.length - 1; ) {
	                var comboItem = new Telerik.Web.UI.RadComboBoxItem();
	                comboItem.set_value(mySplitResult[i]); var tmp_allVehiclesId = mySplitResult[i]; i++;
	                comboItem.set_text(mySplitResult[i]);

	                if ("All Vehicles" == mySplitResult[i])
	                    allVehiclesId = tmp_allVehiclesId;
	                i++;

	                combo.get_items().add(comboItem);
	                combo.commitChanges();
	            }
	            var comboItems = combo.get_items();
	            var item = comboItems.getItem(0);
	            item.select();
	            isPostBack = false;
	        }
	    };

	    function loadFormInformationCompleteError(transport) {
	        alert('<%= ErrorMessage%>');
	    }
	    //*******************************************************************************************************************************
	    //**************************************** Get group select information end *****************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //*****************************************    Get Formats information start   **************************************************
	    //*******************************************************************************************************************************
	    function GetMessageFormatsList() { 
	        EmailSystem.GetMessageFormatsList(userID,GetMessageFormatsComplete, GetMessageFormatsError);
	    }

	    function GetMessageFormatsComplete(transport) {
	        var itemNumber = 0;
	        var itemNumberReal = -1;
	        var combo = $find("<%= rcbFormat.ClientID %>");
	        

	        if (null != combo) {
	            var mySplitResult = transport.split("|");

	            for (i = 0; i < mySplitResult.length - 1; ) {
	                var comboItem = new Telerik.Web.UI.RadComboBoxItem();
	                comboItem.set_value(mySplitResult[i]);
	                if (parseInt(defaultMessageFormatId) == parseInt(mySplitResult[i])) {
	                    itemNumberReal = itemNumber;
	                }
	                i++;

	                comboItem.set_text(mySplitResult[i]);
	                i++;

	                itemNumber++;
	                combo.get_items().add(comboItem);
	                combo.commitChanges();
	            }

	            if (-1 != itemNumberReal) {
	                var comboItems = combo.get_items();
	                var item = comboItems.getItem(itemNumberReal);
	                item.select();
	                EmailSystem.View(defaultMessageFormatId, loadMessageFormat);
	            }
	        }
	    };

	    function GetMessageFormatsError(transport) {
	        alert('<%= ErrorMessage%>');
	    }
	    //*******************************************************************************************************************************
	    //*****************************************    Get Formats information end     **************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //***************************************** Get Message format start ************************************************************
	    //*******************************************************************************************************************************
	    function onSelectedIndexChanged(sender, eventArgs) {
	        document.getElementById("vmessage").style.display = "none";
	        if (undefined != sender) {
	            defaultMessageFormatId = eventArgs.get_item().get_value();
	            EmailSystem.View(defaultMessageFormatId, loadMessageFormat);
	        }
	    }

	    function loadMessageFormat(inHTML) {
	        document.getElementById("messageFormatDiv").style.height = (wDim.height - 215).toString() + "px";
	        $('BodyDiv').update(inHTML);
	    }
	    //*******************************************************************************************************************************
	    //***************************************** Get Message format end **************************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //***************************************** Get recipient select information start **********************************************
	    //*******************************************************************************************************************************
	    function ToGroupChanged() {
	        var combo = $find("<%= rcbToGroup.ClientID %>");
	        var selectedItem = combo.get_selectedItem();
	        var combo = $find("<%= rcbRecipient.ClientID %>");

	        if (null != selectedItem) {
	            if (-1 == selectedItem.get_value()) {

	                if (null != combo) {
	                    var comboItems = combo.get_items();
	                    var item = comboItems.getItem(0);
	                    //First Load
                        if(undefined != item)
	                        item.select();
	                }
	            } else {
	                var comboRecipient = $find("<%= rcbToGroup.ClientID %>");
	                var selectedItem = comboRecipient.get_selectedItem();

	                EmailSystem.GetRecipientList(userID, selectedItem.get_value(), loadRecipientInformationComplete);
	            }
	        }
	    }

	    function loadRecipientInformationComplete(transport) {
	        var combo = $find("<%= rcbRecipient.ClientID %>");

	        if (null != combo) {
	            var comboItems = combo.get_items();
	            var length = comboItems.get_count();

	            for (var i = 0; i < length; i++) {
	                var item = comboItems.getItem(0);
	                combo.trackChanges();
	                comboItems.remove(item);
	                combo.commitChanges();
	            }

	            var mySplitResult = transport.split("|");

	            var index = 0;

	            for (i = 0; i < mySplitResult.length - 1; ) {
	                var comboItem = new Telerik.Web.UI.RadComboBoxItem();
	                comboItem.set_value(mySplitResult[i]); i++;
	                comboItem.set_text(mySplitResult[i]); i++;
	                combo.get_items().add(comboItem);
	                combo.commitChanges();
	            }

	            var comboItems = combo.get_items();
	            var item = comboItems.getItem(0);
	            item.select();
	        }
	    };
	    //*******************************************************************************************************************************
	    //***************************************** Get recipient select information end ************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //********************************************** Get arguments start ************************************************************
	    //*******************************************************************************************************************************
	    function GetRadWindow() {
	        var oWindow = null;
	        if (window.radWindow) oWindow = window.radWindow;
	        else if (window.frameElement.radWindow)
	            oWindow = window.frameElement.radWindow;
	        return oWindow;
	    }

	    function ConfigureDialog() {
	        //Get a reference to the radWindow wrapper     
	        var oWindow = GetRadWindow();
	        //Obtain the argument
	        var oArg = oWindow.argument;
	        //Use the argument
	        
	        $("inpFrom").value = oArg.UserName;
	        userID = oArg.UserID;
	        defaultMessageFormatId = oArg.defaultMessageFormatId;
	    }
	    //*******************************************************************************************************************************
	    //********************************************** Get arguments end **************************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //********************************************** Send message start *************************************************************
	    //*******************************************************************************************************************************
        var objectsValidated;
        var objectToValidate;

        function sendMessage() {
            objectsValidated = 0;
            objectToValidate = 0;
            var isInputFields = false;
            isPageValid = true;
            var objectId = 1;
	        var e = Form.getElements($('form1'));
	        for (var a = 0; a < e.length; a++)
	            if (e[a].id.startsWith('input_')) {
                    objectToValidate++;
                }

	        $find("<%= RadAjaxLoadingPanel1.ClientID %>").show("<%= Panel1.ClientID %>");
	        for (var a = 0; a < e.length; a++)
	            if (e[a].id.startsWith('input_')) {
	                isInputFields = true;
	                var t_title = e[a].title.split(" ");
	                var isRequired = t_title[1].split(":");
                    var isRequiredBool = false;
	                var controlType = t_title[2].split(":");
	                
	                if ("true" == isRequired[1])
	                    isRequiredBool = true;
                    else
                        isRequiredBool = false;

                    if (!$F(e[a].id).blank() || isRequiredBool) {
	                    validate(objectId, controlType[1], isRequired [1], $F(e[a].id));
	                } else {
	                    UpdateObjectCount();
	                }
	                objectId++;
	            }

	            //No input fields
                if (!isInputFields)
	                sendMessageValidated(); 
	    }

        function UpdateObjectCount() {
            objectsValidated++;
            if (objectsValidated == objectToValidate) {
                if (!isPageValid) {
                    $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
                    document.getElementById("vmessage").style.display = "none";
                    alert('<%= Warning_PageIsNotValid%>');
                    return;
                } else {
                    sendMessageValidated();
                }
            }
        }

	    function UpdateFlag() {
	       isPageValid = false;
	    }

	    var SendMessagesTimer;
	    var isPageValid = true;

	    function sendMessageValidated() {
	        var e = Form.getElements($('form1'));
	        $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
	        var comboFormat = $find("<%= rcbFormat.ClientID %>");
	        var comboRecipient = $find("<%= rcbRecipient.ClientID %>");
	        var comboToGroup = $find("<%= rcbToGroup.ClientID %>");

	        var comboFormatSelected = comboFormat.get_selectedItem();
	        var comboRecipientSelected = comboRecipient.get_selectedItem();
	        var comboToGroupSelected = comboToGroup.get_selectedItem();

	        SendMessagesTimer = window.setInterval(CheckMessages, 500);

	        if (null == comboFormatSelected || null == comboRecipientSelected) {
	            alert('<%= WarningMessageFormAndRecepient%>');
	            $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
	        } else {
	            if (-1 == comboRecipientSelected.get_value()) {
	                alert('<%= WarningRecepient%>');
	                $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
	            } else {
	                //Collect params
	                var messageStr = "";
	                //var e = Form.getElements($('form1'));
	                for (var a = 0; a < e.length; a++) {
	                    if (e[a].id.startsWith('input_')) {
	                        // params[e[a].id] = $F(e[a].id);
	                        //Remove  '/' character from Date fields
	                        if (e[a].title.startsWith('Date')) {
	                            if (!$F(e[a].id).blank()) {
	                                var tmp_DateVal = $F(e[a].id).split("/");
	                                if (3 == tmp_DateVal.size())
	                                    messageStr = messageStr + tmp_DateVal[0] + tmp_DateVal[1] + tmp_DateVal[2] + "\n";
	                            }
	                            else {
	                                messageStr = messageStr + "" + "\n";
	                            }
	                        }
	                        //Remove  ':' character from Time fields
	                        else if (e[a].title.startsWith('Time')) {
	                            if (!$F(e[a].id).blank()) {
	                                var tmp_DateVal = $F(e[a].id).split(":");
	                                if (2 == tmp_DateVal.size())
	                                    messageStr = messageStr + tmp_DateVal[0] + tmp_DateVal[1] + "\n";
	                            }
	                            else {
	                                messageStr = messageStr + "" + "\n";
	                            }
	                        } else {
	                            messageStr = messageStr + $F(e[a].id) + "\n";
	                        }
	                    }
	                }
	                // All vehicles + All recepients
	                if (comboToGroupSelected.get_value() == allVehiclesId && 0 == comboRecipientSelected.get_value()) {
	                    var items = comboRecipient.get_items();
	                    asynchronousCalls = items.get_count() - 2;
	                    for (var i = 0; i < items.get_count(); i++) {
	                        var item = items.getItem(i);
	                        if (item) {
	                            if (-1 != item.get_value() && 0 != item.get_value()) {
	                                EmailSystem.SendMessage(true, comboFormatSelected.get_value(), userID, comboToGroupSelected.get_value(), item.get_value(), '', messageStr, sendMessageCompleteAllRecepient, sendMessageAllError);
	                            }
	                        }
	                    }
	                } else {
	                    EmailSystem.SendMessage(true, comboFormatSelected.get_value(), userID, comboToGroupSelected.get_value(), comboRecipientSelected.get_value(), '', messageStr, sendMessageComplete, sendMessageError);
	                }
	            }
	        }
	    }

	    function CheckMessages() {
	        if (TotalMessagesToSend == asynchronousCalls && ErrorMessageFlag) {
	            $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
	            alert('<%= Warning_MessagesNotSent%>');
	            window.clearInterval(SendMessagesTimer);
	            cancel();
	        }
	    }

	    function sendMessageCompleteAllRecepient(val) {
	        TotalMessagesToSend++;
	        if (TotalMessagesToSend == asynchronousCalls && !ErrorMessageFlag) {
	            window.clearInterval(SendMessagesTimer);
	            $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");

	            alert('<%= Warning_MessagesSent%>');
	            //create the argument that will be returned to the parent page
	            var oArg = new Object();
	            oArg.isSent = true;
	            //get a reference to the current RadWindow
	            var oWnd = GetRadWindow();
	            //Close the RadWindow and send the argument to the parent page
	            oWnd.close(oArg);
	            return false;
	        }
	    }

	    function sendMessageComplete(val) {
	        $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
	        window.clearInterval(SendMessagesTimer);
	        alert('<%= Warning_MessageSent%>');
	        //create the argument that will be returned to the parent page
	        var oArg = new Object();
	        oArg.isSent = true;
	        //get a reference to the current RadWindow
	        var oWnd = GetRadWindow();
	        //Close the RadWindow and send the argument to the parent page
	        oWnd.close(oArg);
	        return false;
	    }

	    var ErrorMessageFlag = false;
	    function sendMessageAllError() {
	        ErrorMessageFlag = true;
	        TotalMessagesToSend++;
	    }

	    function sendMessageError(error) {
	        $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
	        window.clearInterval(SendMessagesTimer);
	        alert('<%= Warning_MessageNotSent%>'); // Error message is-' + error.responseText);
	        cancel();
	    } 
	    //*******************************************************************************************************************************
	    //********************************************** Send message end ***************************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //****************************************** Page functionality start ***********************************************************
	    //*******************************************************************************************************************************
	    function cancel() {
	        //create the argument that will be returned to the parent page
	        var oArg = new Object();
	        oArg.isSent = false;
	        oArg.defaultMessageFormatId = defaultMessageFormatId;
	        //get a reference to the current RadWindow
	        var oWnd = GetRadWindow();
	        //Close the RadWindow and send the argument to the parent page
	        oWnd.close(oArg);
	        return false;
	    }

//	    function limitText(limitField, limitCount, limitNum) {
//	        if (limitField.value.length > limitNum) {
//	            limitField.value = limitField.value.substring(0, limitNum);
//	        } else {
//	            limitCount.value = limitNum - limitField.value.length;
//	        }
//	    }
	    //*******************************************************************************************************************************
	    //****************************************** Page functionality end *************************************************************
	    //*******************************************************************************************************************************
	   </script>
    </telerik:RadScriptBlock>
</head>
<body onload ="OnLoad();">
    <span style="padding: 2px; z-index: 9999999; font-size: 9pt; font-weight: bolder; background-color: red; color: white; border: 1px outset white; display: none; position: absolute; left: 0px; top: 0px;" id="vmessage">No Errors </span>
	<form id="form1" runat="server">
	     <telerik:RadScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/EmailSystem.asmx"/>
            </Services>
        </telerik:RadScriptManager>

        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>

	    <telerik:radskinmanager runat="Server" ID="SkinManager1" Skin="Hay" PersistenceKey="Skin"  PersistenceMode="Session"/>
        <telerik:RadXmlHttpPanel runat="server" ID="Panel1">
	        <div style ="background-color:White; height:100%;">
	            <div style ="position :absolute ; left :10px; top:10px">
	                <asp:Label runat ="server" ID="MyLabel" Text="From" Width ="120px" meta:resourcekey="MyLabelResource1"/>
	                <input readonly type ="text" runat="server" id="inpFrom" name ="inpFrom" value="" tabindex ="1"/>  
	            </div>
	            <div style ="position :absolute ; left :10px; top:40px">
	                <asp:Label runat ="server" ID="Label1" Text="To Group" Width ="120px" meta:resourcekey="Label1Resource1"/>
	                <telerik:RadComboBox ID="rcbToGroup" runat="server" OnClientSelectedIndexChanged ="ToGroupChanged" AllowCustomText="false" MarkFirstMatch ="false" EnableLoadOnDemand ="false"  TabIndex="2" meta:resourcekey="rcbToGroupResource1"/>
	            </div>
	            <div style ="position :absolute ; left :10px; top:70px">
	                <asp:Label runat ="server" ID="Label2" Text="To Recipient" Width ="120px" meta:resourcekey="Label2Resource1"/>
	                <telerik:RadComboBox ID="rcbRecipient" runat="server" AllowCustomText="false" MarkFirstMatch ="false" EnableLoadOnDemand ="false"  TabIndex="3" meta:resourcekey="rcbRecipientResource1"/>
	            </div>
	            <div style ="position :absolute ; left :10px; top:100px">
	                <asp:Label ID="Label3" runat ="server" Text ="Message Format" Width ="120px" meta:resourcekey="Label3Resource1"/>
	                <telerik:RadComboBox runat="server"  ID="rcbFormat" AllowCustomText="false" MarkFirstMatch ="false" EnableLoadOnDemand ="false" OnClientSelectedIndexChanged ="onSelectedIndexChanged" TabIndex="4" meta:resourcekey="rcbFormatResource1"/>
	            </div>
                <div id="messageFormatDiv" runat ="server" class="MessageFormatDiv" style ="position :relative; top:130px;left:10px;right:10px;">
                    <div id="BodyDiv"></div>
                </div>
	            <div id="composeActions" style="position :absolute ; height :30px; bottom :0px">
	                <input type ="button"  id="ButtonSend" style="width: 75px" onclick ="return sendMessage();"/>
	                <input type ="button"  id="ButtonCancel" style="width: 75px" onclick ="return cancel();"/>
	            </div>
	        </div>
        </telerik:RadXmlHttpPanel>
	</form>
</body>
</html>
