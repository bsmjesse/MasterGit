<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SendMessage.aspx.cs" Inherits="SentinelFM.Widgets_SendMessage" %>
<%if(ACTION == "GetForm") { %>
<div id="sendmsgWait" style="display:none">
    <table style="z-index: 101; left: 220px; width:100%;   top: 128px">
        <tr>
            <td class="RegularText" style="height: 15px" align="center">
                <span class="RegularText">Please wait...</span>
             </td>
        </tr>
        <tr>
            <td class="formtext" style="height: 14px" align="center">
                <span class="formtext">Sending Message...</span></td>
        </tr>
        <tr>
            <td style="height: 22px" align="center">
                <img id="sendmessageWaitImage" width="105px" height="5px" /></td>
        </tr>
        <tr>
            <td style="height: 22px" align="center">
                <input type="button" value="Cancel" onclick="cancelSendMessage();" />
            </td>
        </tr>
    </table>
</div>
<div id="sendmsgMain">
    <form id="sendmessageForm">
        <input type="hidden" id="BoxId" name="BoxId" value="<%=BOXID %>" />
        <input type="hidden" id="Action" name="Action" value="SendMessage" />
        <table>
        <tr><td><span style="font-weight:bold;">Message</span> (<span id="messageCharactersLeft">115</span> characters left)</td>
        <td></td>
        </tr>
        <tr>
        <td style="height: 54px; vertical-align: top;">
            <textarea id="txtMessage" class="textField" style="font-family:monospace;font-size:10pt;height:65px;width:269px;" cols="20" rows="5" name="txtMessage" onkeypress="RecalculateTextRemaining();" onkeyup="RecalculateTextRemaining(this);"></textarea>
        </td>
        <td style="height: 54px; vertical-align: bottom; padding-left: 10px;">
            <input id="cmdSend" class="kd-button" type="button" value="Send" name="cmdSend" onclick="sendMessage(this);" />
        </td></tr>
        <tr>
            <td colspan="2">
                <span id="Span1" class="formtext">* Message text cannot exceed 115 characters.</span>                
            </td>
        </tr>
        <tr><td colspan="2">
             <span id="Span2" class="formtext">* Maximum 4 lines allowed.</span>
        </td></tr>
        <tr><td colspan="2">
            <span id="commandStatus" style="display:none"></span>
        </td></tr>
        </table>
    </form>
</div>

<script type="text/javascript">
    $('#sendmessageWaitImage').attr('src', rootpath + 'images/prgBar.gif');
    var sendingMessage = false;
    function sendMessage(b) {
        var f = $(b).closest("form").serialize();
        
        if ($('#txtMessage').val() == '') {
            $('#commandStatus').css('color', 'red').html('Message cannot be empty.').show();
            return;
        }

        $.ajax({
            type: 'GET',
            url: rootpath + 'Widgets/SendMessage.aspx',
            data: $(b).closest("form").serialize(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg) {
                if (msg.status == 200) {
                    $('#sendmsgMain').hide();
                    $('#sendmsgWait').show();
                    //document.getElementById('sendMessageTimer').src = rootpath + "Widgets/SendMessageTimer.aspx";
                    sendingMessage = true;
                    checkMessageSendingStatus();
                }
                else if (msg.status == 201) {
                    //$('#sendmsgMain').hide();
                    $('#sendmsgWait').hide();
                    $('#commandStatus').css('color', 'green').html(msg.msg).show();
                }
                else {
                    $('#commandStatus').css('color', 'red').html(msg.msg).show();
                }

            },
            error: function (msg) {
                alert('failure');
            }
        });
    }

    function cancelSendMessage() {
        $.ajax({
            type: 'GET',
            url: rootpath + 'Widgets/SendMessage.aspx?ACTION=CancelSendMessage',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg) {
                if (msg.status == 200) {
                    $('#sendmsgMain').show();
                    $('#sendmsgWait').hide();
                    $('#commandStatus').css('color', 'red').html('Message Sending is Cancelled').show();
                    sendingMessage = false;
                }

            },
            error: function (msg) {
                alert('failure');
            }
        });
        
    }

    function RecalculateTextRemaining() {
        var nMaxMessageLength = 115;// 230
        
        var oMessage = window.document.getElementById("txtMessage");
        var BoxValue = 115;// 230;
        var sMes = oMessage.value;
        if (oMessage == null)
            return;

        var innertext = $('#txtMessage').val();
        
        if ((sMes.substring(innertext.length - 1, innertext.length) == "<") || (sMes.substring(innertext.length - 1, innertext.length) == ">") || (sMes.substring(innertext.length - 1, innertext.length) == "~") || (sMes.substring(innertext.length - 1, innertext.length) == ";")) {
            oMessage.value = sMes.substring(0, innertext.length - 1)
            return;
        }
        
        BoxValue = nMaxMessageLength - innertext.length;
        
//        if (parseInt(BoxValue) < 112) {            
//            var s = oMessage.value;
//            oMessage.value = s.substring(0, 120);
//            innertext = $('#txtMessage').val();
        //        }
        if (parseInt(BoxValue) < 0) {
            var s = oMessage.value;
            oMessage.value = s.substring(0, nMaxMessageLength);
            innertext = $('#txtMessage').val();
        }

        $('#messageCharactersLeft').html(nMaxMessageLength - innertext.length);

    }

    function checkMessageSendingStatus() {
        if (!sendingMessage)
            return;
        $.ajax({
            type: 'GET',
            url: rootpath + 'Widgets/SendMessage.aspx?ACTION=CheckStatus',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (msg) {
                if (msg.status == 200) {
                    $('#sendmsgMain').show();
                    $('#sendmsgWait').hide();
                    $('#txtMessage').val('');
                    $('#commandStatus').css('color', 'green').html(msg.msg).show();
                    sendingMessage = false;
                }
                else if (msg.status == 300 && sendingMessage) {
                    window.setTimeout('checkMessageSendingStatus()', 1000)
                }

            },
            error: function (msg) {
                alert('failure');
            }
        });
    }
    
</script>
<% } else { %>
    {"status":<%=STATUS %>, "msg":"<%=MSG %>"}
<% } %>