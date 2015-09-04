<%@ Page Language="C#" CodeFile="Reply.aspx.cs" Inherits="SentinelFM.Reply" AutoEventWireup="true" meta:resourcekey="PageResource1" UICulture="auto"%>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head id="Head1" runat="server">
	<title><%= PageTitle%></title>
	<script src="../Assets/Scripts/prototype.js"  type="text/javascript"></script>
	<link href="../Assets/Styles/EmailSystem/NewMessage.css" rel="stylesheet" type="text/css" />
	<style type ="text/css" >
        .BodyTextCss { vertical-align :top ; }
    </style>
	<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
	<script type="text/javascript">
	    var userID;
	    var userName;
	    var recepientId;
	    var isPostBack = false;
	    var wDim;

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
	    //*******************************************************************************************************************************
	    //***************************************************** Get fields format start *************************************************
	    //*******************************************************************************************************************************
	    function OnLoad() {
	        document.getElementById("inpFrom").style.width = (wDim.width - 180).toString() + "px";
	        document.getElementById("inpGroups").style.width = (wDim.width - 180).toString() + "px";
	        document.getElementById("inpRecepient").style.width = (wDim.width - 180).toString() + "px";
	        document.getElementById("inpSubject").style.width = (wDim.width - 180).toString() + "px";
	        document.getElementById("inpMessage").style.width = (wDim.width - 180).toString() + "px";
	        document.getElementById("inpMessage").style.height = (wDim.height - 210).toString() + "px";
	        document.getElementById("composeActions").style.left = (wDim.width / 2 - 75).toString() + "px";

	        $("ButtonSend").value = '<%= Button_Send%>';
	        $("ButtonCancel").value = '<%= Button_Cancel%>';

	        loadFormTemplateComplete();
	    }

	    function loadFormTemplateComplete() {
	        ConfigureDialog();
	    };
	    //*******************************************************************************************************************************
	    //***************************************************** Get fields format end ***************************************************
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
	    
	        $('inpFrom').value = oArg.UserName;
	        $('inpRecepient').value = oArg.From;
	        userID = oArg.UserID;
	        recepientId = oArg.SenderId;
	        $('inpSubject').focus();
	    }
	    //*******************************************************************************************************************************
	    //********************************************** Get arguments end **************************************************************
	    //*******************************************************************************************************************************
	    //*******************************************************************************************************************************
	    //********************************************** Send message start *************************************************************
	    //*******************************************************************************************************************************
	    function sendMessage() {
	        EmailSystem.SendMessage(false, 0, userID, 0, recepientId, $F("inpSubject"), $F("inpMessage"), sendMessageComplete, sendMessageError);
        }
	    
	    function sendMessageComplete(val) {
	        alert('<%= Warning_MessageSent%>');
	        //create the argument that will be returned to the parent page
	        var oArg = new Object();
	        oArg.isSent = true
	        //get a reference to the current RadWindow
	        var oWnd = GetRadWindow();
	        //Close the RadWindow and send the argument to the parent page
	        oWnd.close(oArg);
	        return false;
	    }

	    function sendMessageError(error) {
	        alert('<%= Warning_MessageNotSent%>'); //Error message is-' + error.responseText);
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
	    //*******************************************************************************************************************************
	    //****************************************** Page functionality end *************************************************************
	    //*******************************************************************************************************************************
	   </script>
    </telerik:RadScriptBlock>
</head>
<body onload ="OnLoad();">
    <span style="padding: 2px; z-index: 9999999; font-size: 9pt; font-weight: bolder;background-color: red; color: white; border: 1px outset white; display: none;position: absolute; left: 0px; top: 0px;" id="vmessage">No Errors </span>
	<form id="form1" runat="server">
	    <telerik:RadScriptManager ID="ScriptManager1" runat="server">
           <Services>
               <asp:ServiceReference Path="~/Services/EmailSystem.asmx"/>
           </Services>
        </telerik:RadScriptManager>
	    <telerik:radskinmanager runat="Server" ID="SkinManager1" Skin="Hay" PersistenceKey="Skin"  PersistenceMode="Session"/>
	    <div style ="background-color:White; height:100%;">
	        <div style ="position :absolute ; left :10px; top:10px">
	            <asp:Label runat ="server" ID="MyLabel" Text="From" Width ="120px" meta:resourcekey="MyLabelResource1"/>
	            <input readonly type ="text" runat="server" id="inpFrom" name ="inpFrom" value="" tabindex ="1"/>  
	        </div>
	        <div style ="position :absolute ; left :10px; top:40px">
	            <asp:Label runat ="server" ID="Label1" Text="To Group" Width ="120px" meta:resourcekey="Label1Resource1"/>
	            <input readonly type ="text" runat="server" id="inpGroups" name ="inpGroups" value="All Vehicles" tabindex ="2"/> 
	        </div>
	        <div style ="position :absolute ; left :10px; top:70px">
	            <asp:Label runat ="server" ID="Label2" Text="To Recipient" Width ="120px" meta:resourcekey="Label2Resource1"/>
	            <input readonly type ="text" runat="server" id="inpRecepient" name ="inpRecepient" tabindex ="3"/> 
	        </div>
	        <div style ="position :absolute ; left :10px; top:100px">
	            <asp:Label runat ="server" ID="Label3" Text="Subject" Width ="120px" meta:resourcekey="Label3Resource1"/>
	            <input type ="text" id="inpSubject" maxlength ="100" tabindex ="4"/>
	        </div>
            <div id="messageFormatDiv" runat ="server" class="MessageFormatDiv" style ="position :relative;left :10px; top:130px">
                <asp:Label runat ="server" ID="Label4" Text="Body" Width ="120px" CssClass="BodyTextCss" meta:resourcekey="Label4Resource1"/>
	            <textarea id="inpMessage"  style ="overflow :auto;"  tabindex ="5" maxlength="4000"></textarea>
	        </div>
	        <div id="composeActions" style="position :absolute ; height :30px; bottom :0px">
	            <input type ="button" id="ButtonSend" style="width: 75px" onclick ="return sendMessage();"/>
	            <input type ="button" id="ButtonCancel" style="width: 75px" onclick ="return cancel();" />
	        </div>
        </div>
	</form>
</body>
</html>
