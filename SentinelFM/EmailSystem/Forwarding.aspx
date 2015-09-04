<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Forwarding.aspx.cs" Inherits="SentinelFM.Forwarding" UICulture="auto" meta:resourcekey="PageResource1"%>

<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head id="Head1" runat="server">
	<title>Forward</title>
	<script src="../Assets/Scripts/prototype.js"  type="text/javascript"></script>
    <link href="../Assets/Styles/EmailSystem/NewMessage.css" rel="stylesheet" type="text/css"/>
  
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
	<script type="text/javascript">
	    var userID;
	    var userName;
	    var isInbox;
	    var messageFormatId;
	    var allVehiclesId;
	    var isPostBack = false;
	    var TotalMessagesToSend = 0;
	    var asynchronousCalls;
	    var wDim;

	    document.observe('dom:loaded', function () {
	        var vpDim = document.viewport.getDimensions();
	        Position.GetWindowSize = function (w) {
	            var width, height;
	            w = w ? w : window;
	            this.width = w.innerWidth || (w.document.documentElement.clientWidth || w.document.body.clientWidth);
	            this.height = w.innerHeight ||(w.document.documentElement.clientHeight ||w.document.body.clientHeight);
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

	    //Disable validation
	    function validate(id, controlType, isRequired, value) {
	        return false;
	    }
	    //*******************************************************************************************************************************
	    //***************************************************** Get fields format start *************************************************
	    //*******************************************************************************************************************************
	    function OnLoad() {
	        document.getElementById("inpFrom").style.width = (wDim.width - 166).toString() + "px";
	        document.getElementById("rcbToGroup").style.width = (wDim.width - 165).toString() + "px";
	        document.getElementById("rcbRecipient").style.width = (wDim.width - 165).toString() + "px";
	        document.getElementById("messageFormatDiv").style.height = (wDim.height - 210).toString() + "px";
	        document.getElementById("messageFormatDiv").style.width = (wDim.width - 40).toString() + "px";
	        document.getElementById("composeActions").style.left = (wDim.width / 2 - 75).toString() + "px";
//	        $("inpFrom").setStyle({
//	            width: (parseInt(wDim) - 180) + "px"
//	        });
//	        $("rcbToGroup").setStyle({
//	            width: (parseInt(windowDim.X) - 175) 
//	        });
//	        $("rcbRecipient").setStyle({
//	            width: (parseInt(windowDim.X) - 175) 
//	        });
//	       
//	        $("messageFormatDiv").setStyle({
//	            height: (wDim.height - 165 - 45),
//	            width: (parseInt(wDim.width) - 40) 
//	        });

//	        $("composeActions").setStyle({
//	            left: (parseInt(windowDim.X) / 2 - 75) 
//	        });

	        $("ButtonSend").value = '<%= Button_Send%>';
	        $("ButtonCancel").value = '<%= Button_Cancel%>';

	        loadFormTemplateComplete();
	    }

	    function loadFormTemplateComplete() {
	        ConfigureDialog();
	        GetFieldsInformation();
	    };
	    //*******************************************************************************************************************************
	    //***************************************************** Get fields format end ***************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //***************************************** Get group select information start **************************************************
	    //*******************************************************************************************************************************
	    function GetFieldsInformation() {
	        EmailSystem.GetGroupList(userID, loadFormInformationComplete);
	    }

	    function loadFormInformationComplete(transport) {
	        var combo = $find("<%= rcbToGroup.ClientID %>");

	        if (null != combo) {
	            var mySplitResult = transport.split("|");
	            var index = 0;

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
	        }
	    };

	    function loadFormInformationError(transport) {
	        alert('<%= ErrorMessage%>');
	    }
	    //*******************************************************************************************************************************
	    //**************************************** Get group select information end *****************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //***************************************** Get recipient select information start **********************************************
	    //*******************************************************************************************************************************
	    function ToGroupChanged() {
	        var combo = $find("<%= rcbToGroup.ClientID %>");
	        var selectedItem = combo.get_selectedItem();
	        var combo = $find("<%= rcbRecipient.ClientID %>");

	        if (isPostBack) {
	            if (-1 == selectedItem.get_value()) {
	               
	                var comboItems = combo.get_items();
	                var item = comboItems.getItem(0);
	                item.select();
	            } else {
	                var comboRecipient = $find("<%= rcbToGroup.ClientID %>");
	                var selectedItem = comboRecipient.get_selectedItem();

	                EmailSystem.GetRecipientList(userID, selectedItem.get_value(), loadRecipientInformationComplete);
	            }
	        }
	        isPostBack = true;
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
	      

            userID = oArg.UserID;
            isInbox = oArg.isInbox;
            messageFormatId = oArg.MessageFormatId;
            $('BodyDiv').update(oArg.Body);
            if (!Prototype.Browser.IE) {
                EmailSystem.MessageView(userID, oArg.RowId, ShowMessage);
            }        
	        $("inpFrom").value = oArg.UserName;
	    }
	    
	    function ShowMessage(result) {
            var mySplitResult = result.split("\n");
	        if (isInbox) {
	            for (i = 1; i <= mySplitResult.length; i++) {
	                $('input_inbox_' + i).value = mySplitResult[i-1];
                }
	        } else if (!isInbox) {
	            for (i = 1; i <= mySplitResult.length; i++) {
	                $('input_outbox_' + i).value = mySplitResult[i-1];
                }
	        }
        }
	    //*******************************************************************************************************************************
	    //********************************************** Get arguments end **************************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //********************************************** Send message start *************************************************************
	    //*******************************************************************************************************************************
	    var SendMessagesTimer;

	    function sendMessage() {
	        $find("<%= RadAjaxLoadingPanel1.ClientID %>").show("<%= Panel1.ClientID %>");

	        var comboRecipient = $find("<%= rcbRecipient.ClientID %>");
	        var comboToGroup = $find("<%= rcbToGroup.ClientID %>");

	        var comboRecipientSelected = comboRecipient.get_selectedItem();
	        var comboToGroupSelected = comboToGroup.get_selectedItem();

	        SendMessagesTimer = window.setInterval(CheckMessages, 500);

	        if (null == comboRecipientSelected) {
	            alert('<%= Warning_Select%>');
	            $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
	        } else {
	            if (-1 == comboRecipientSelected.get_value()) {
	                alert('<%= Warning_Select%>');
	                $find("<%= RadAjaxLoadingPanel1.ClientID %>").hide("<%= Panel1.ClientID %>");
	            } else {
	               //Collect params
	                var messageStr = "";
	                var e = Form.getElements($('form1'));
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
	                        }
	                        //Remove  ':' character from Time fields
	                        else if (e[a].title.startsWith('Time')) {
	                            if (!$F(e[a].id).blank()) {
	                                var tmp_DateVal = $F(e[a].id).split(":");
	                                if (2 == tmp_DateVal.size())
	                                    messageStr = messageStr + tmp_DateVal[0] + tmp_DateVal[1] + "\n";
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
	                                 EmailSystem.SendMessage(true, messageFormatId, userID, comboToGroupSelected.get_value(), item.get_value(), '', messageStr, sendMessageCompleteAllRecepient, sendMessageAllError);
	                            }
	                        }
	                    }
	                } else {
	                    EmailSystem.SendMessage(true, messageFormatId, userID, comboToGroupSelected.get_value(), comboRecipientSelected.get_value(), '', messageStr, sendMessageComplete, sendMessageError);
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
	        oArg.isSent = false
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
<body> 
	<form id="form1" runat="server">
	    <telerik:RadScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/EmailSystem.asmx"/>
            </Services>
        </telerik:RadScriptManager>

        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>

        <telerik:radskinmanager runat="Server" ID="SkinManager1" Skin="Hay" PersistenceKey="Skin"  PersistenceMode="Session"/>
        <telerik:RadXmlHttpPanel runat="server" ID="Panel1">
	        <div id="TopMenuDiv" style ="background-color :White ; height:100%;">
	            <div style ="position :absolute ; left :10px; top:10px">
	                <asp:Label runat ="server" ID="MyLabel" Text="From" Width ="120px" meta:resourcekey="MyLabelResource1"/>
	                <input readonly type ="text" runat="server" id="inpFrom" name ="inpFrom" value="" tabindex ="1"/>  
	            </div>
	            <div style ="position :absolute ; left :10px; top:40px">
	                <asp:Label runat ="server" ID="Label1" Text="To Group" Width ="120px" meta:resourcekey="Label1Resource1"/>
	                <telerik:RadComboBox ID="rcbToGroup" runat="server" AllowCustomText="false" MarkFirstMatch ="false" EnableLoadOnDemand ="false" OnClientSelectedIndexChanged ="ToGroupChanged" TabIndex ="2" meta:resourcekey="rcbToGroupResource1"/>
	            </div>
	            <div style ="position :absolute ; left :10px; top:70px">
	                <asp:Label runat ="server" ID="Label2" Text="To Recipient" Width ="120px" meta:resourcekey="Label2Resource1"/>
	                <telerik:RadComboBox ID="rcbRecipient" runat="server" AllowCustomText="false" MarkFirstMatch ="false" EnableLoadOnDemand ="false" TabIndex ="3" meta:resourcekey="rcbRecipientResource1"/>
	            </div>
                <div id="messageFormatDiv" runat ="server" class="MessageFormatDiv" style ="position :relative;left :10px; top:110px;right:10px">
                    <div id="BodyDiv"></div>
	            </div>
	            <div id="composeActions" style="position :absolute ; height :30px; bottom :0px">
                    <input  type ="button" id="ButtonSend"  style ="width :75px" onclick="return sendMessage();" />
                    <input  type ="button" id="ButtonCancel" style ="width :75px" onclick="return cancel();" />
	            </div>
	        </div> 
         </telerik:RadXmlHttpPanel>
	</form>
</body>
</html>
