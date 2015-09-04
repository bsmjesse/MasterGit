<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EmailSystem.aspx.cs" Inherits="SentinelFM.EmailSystem" UICulture="auto" meta:resourcekey="PageResource1" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
     <meta http-equiv="Content-Type" content="text/html;charset=windows-1252"/> 
    <link href="../StandartSkin/TabStrip.Standart.css" rel="stylesheet" type="text/css" />
    <link href="../GlobalStyle.css" type="text/css" rel="stylesheet"/>
    <script type="text/javascript" src="../Assets/Scripts/jquery-1.4.2.min.js"></script>
    <script src="../Assets/Scripts/Utils.js" type="text/javascript"></script>
    <style type ="text/css" >
        body { margin:0px; overflow :auto; font-family:Tahoma ; font-size :x-small}
    </style>
</head>
<body>
    <span style="padding: 2px; z-index: 9999999; font-size: 9pt; font-weight: bolder;background-color: red; color: white; border: 1px outset white; display: none;position: absolute; left: 0px; top: 0px;" id="vmessage">No Errors </span>
    <form id="form1" runat="server">
        <telerik:RadScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/EmailSystem.asmx"/>
            </Services>
        </telerik:RadScriptManager>
        <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
	    <script language="JavaScript" type="text/javascript">
	        /* <![CDATA[  */ 
	        var toolbarInbox;
	        var toolbarOutbox;
	        var gridInbox;
	        var gridOutbox;
	        var tabs;
	        var selectedRowId = -1;
	        var selectedTo;
	        var senderName;
	        var senderId;
	        var oWnd;
	        var isInbox = true;
	        var msg;
	        var currentSortingField;
	        var messageFormatId;
            var defaultMessageFormatId = 0;
            var windowDim = getWindowSize();
            
	        function pageLoad() { 
	            gridInbox  = $find("<%=Inbox_Grid.ClientID %>");
	            gridOutbox = $find("<%=Outbox_Grid.ClientID %>"); 
	            toolbarInbox = $find("<%= RadToolBar1.ClientID %>");
	            toolbarOutbox = $find("<%= RadToolBar2.ClientID %>");
	            tabs = $find("<%= RadTabStrip1.ClientID %>");

	            if (gridInbox.get_masterTableView().get_selectedItems().length == 0) {
	                toolbarInbox.findButtonByCommandName("reply").disable();
                    toolbarInbox.findButtonByCommandName("Forward").disable();
                }
	            
	            if (gridOutbox.get_masterTableView().get_selectedItems().length == 0) {
	                toolbarOutbox.findButtonByCommandName("reply").disable();
                    toolbarOutbox.findButtonByCommandName("Forward").disable();
                }
	                
	            
	            SetInboxFilterDefaultTime(); 
	            SetOutboxFilterDefaultTime(); 
	              
	            var realScreenHeight = windowDim.Y - 380;  //parseInt($("LogoTable").offsetHeight) + parseInt($("MenuDiv").offsetHeight);
	            var realScreenHeight = realScreenHeight.toString() + "px";
            
	            $("#GridDiv").css("width",windowDim.X + "px");
	            $("#GridDiv").css("height",windowDim.Y + "px");
	             
	            $("#Inbox_MessagePane").width((windowDim.X-15) + "px");
	            $("#Outbox_MessagePane").width((windowDim.X-15) + "px");
	            
	            $("#Inbox_MessagePane").height(realScreenHeight);
	            $("#Outbox_MessagePane").height(realScreenHeight);

                if (!$.browser.msie) { 
                    $("#Inbox_MessagePane").css("border-top","1px solid gray"); 
                    $("#Outbox_MessagePane").css("border-top","1px solid gray"); 
                }

                //Update No data string
               
                $("#InboxNoDataDiv").html("<%= Notification_NoData%>"); 
                $("#OutboxNoDataDiv").html("<%= Notification_NoData%>");
                

                EmailSystem.IsEnableForms(<%= sn.UserID %>,IsEnableForms, IsEnableFormsError);
	            EmailSystem.GetRowsListFiltered_Inbox("ReceivedTimestamp", Telerik.Web.UI.GridSortOrder.Descending,<%= sn.UserID %> ,inboxDateFromYear,inboxDateFromMonth,inboxDateFromDay,inboxDateToYear,inboxDateToMonth,inboxDateToDay,inboxTimeHourFrom,inboxTimeMinuteFrom,inboxTimeHourTo,inboxTimeMinuteTo, updateGrid_inbox, updateGridError);
	        }
	        
//	        window.onresize = SetResize;
//          function SetResize() {
//              windowDim = getWindowSize();
//              $("#Inbox_Grid").css("width",windowDim.X + "px");
//            }
            
            //Disable validation
            function validate(id, controlType, isRequired, value) {
                return false;
            }
            //************************************************************************************************************
	        //*****************************************  Enable Forms - Start ********************************************
	        //************************************************************************************************************
            function IsEnableForms(result) {
                if (result) {
                    toolbarInbox.findButtonByCommandName("NewFormMessageSeparator").show();
                    toolbarInbox.findButtonByCommandName("NewFormMessage").show();
                    toolbarOutbox.findButtonByCommandName("NewFormMessageSeparator").show();
                    toolbarOutbox.findButtonByCommandName("NewFormMessage").show();
                } 
                else {
                    toolbarInbox.findButtonByCommandName("NewFormMessageSeparator").hide();
                    toolbarInbox.findButtonByCommandName("NewFormMessage").hide();
                    toolbarOutbox.findButtonByCommandName("NewFormMessageSeparator").hide();
                    toolbarOutbox.findButtonByCommandName("NewFormMessage").hide();
                }
            }
            
            function IsEnableFormsError(result) {
                alert('<%= ErrorMessage%>');
            }
            //************************************************************************************************************
	        //*****************************************  Enable Forms - End **********************************************
	        //************************************************************************************************************
	        //************************************************************************************************************
	        //                                     Data Binding - Start
	        //************************************************************************************************************
	        function updateGridError(result) {
	            alert('<%= ErrorMessage%>');
	        }

	        function updateGrid_inbox(result) {
                // Check for session
	            if (null == result) {
	                 form1.submit();
	            }
	            var tableView = $find("<%= Inbox_Grid.ClientID %>").get_masterTableView();
	            for(var i=0 ;i< result.length ;i++) {
	                var splittedStr = result[i].MessageStatus.split("|");  
	                result[i].MessageStatus = splittedStr[0];
	            }
                $('#Inbox_MessagePane').html("");
	            tableView.set_dataSource(result);
	            tableView.dataBind();
	        }

	        function updateGrid_outbox(result) {
	            if (null == result) {
	                 form1.submit();
	            }
	            var tableView = $find("<%= Outbox_Grid.ClientID %>").get_masterTableView();
   	            for(var i=0 ;i< result.length ;i++) {
	                var splittedStr = result[i].MessageStatus.split("|");  
	                result[i].MessageStatus = splittedStr[0];
	            }
                $('#Outbox_MessagePane').html("");
	            tableView.set_dataSource(result);
	            tableView.dataBind();
	        }

	        function onTabSelecting(sender, args) {
	            if ("Inbox" == args.get_tab().get_text()) {
                    $("#OutboxFilterDiv").css("visibility","hidden");
	                $("#InboxFilterDiv").css("visibility","visible");
	                EmailSystem.GetRowsListFiltered_Inbox("ReceivedTimestamp", Telerik.Web.UI.GridSortOrder.Descending,<%= sn.UserID %> ,inboxDateFromYear,inboxDateFromMonth,inboxDateFromDay,inboxDateToYear,inboxDateToMonth,inboxDateToDay,inboxTimeHourFrom,inboxTimeMinuteFrom,inboxTimeHourTo,inboxTimeMinuteTo, updateGrid_inbox, updateGridError);
	            } else {
	                 $("#OutboxFilterDiv").css("visibility","visible");
	                 $("#InboxFilterDiv").css("visibility","hidden");
	                 EmailSystem.GetRowsListFiltered_Outbox("ReceivedTimestamp", Telerik.Web.UI.GridSortOrder.Descending,<%= sn.UserID %> ,outboxDateFromYear,outboxDateFromMonth,outboxDateFromDay,outboxDateToYear,outboxDateToMonth,outboxDateToDay,outboxTimeHourFrom,outboxTimeMinuteFrom,outboxTimeHourTo,outboxTimeMinuteTo, updateGrid_outbox, updateGridError);
	            }
	        }
	        //************************************************************************************************************
	        //                                     Data Binding - End
	        //************************************************************************************************************
            //************************************************************************************************************
	        //                                  Working with Windows - Start
	        //************************************************************************************************************
             function onButtonClicked(sender, args) {
	            var commandName = args.get_item().get_commandName();
	            var oWnd;
	            if (commandName == "reply") {
	                oWnd = window.radopen(null, "Edit");  	                 
	            } else if (commandName == "NewFormMessage") {
	                oWnd = window.radopen(null, "NewMessageWithFormat");
	            } else if (commandName == "NewMessage") {
	                oWnd = window.radopen(null, "NewMessage");
                } else if (commandName == "Forward") { 
                    var testBody ;
                    if (isInbox) testBody = $("#Inbox_MessagePane").html();
                    if (!isInbox) testBody = $("#Outbox_MessagePane").html();
                    
                    if (0 == testBody.length)
                        alert('<%= ErrorMessageToForward%>');
                    else 
	                    oWnd = window.radopen(null, "Forward");
	            }
	            
                if (commandName == "NewFormMessage" || commandName == "Forward") {
                    oWnd.set_width(windowDim.X * 0.60 );
                    oWnd.set_height(windowDim.Y * 0.8 );
                    oWnd.moveTo(windowDim.X - windowDim.X*0.75 , 20);
                } else {
                    oWnd.set_width(windowDim.X * 0.35 );
                    oWnd.set_height(windowDim.Y * 0.6 );
                    oWnd.moveTo(windowDim.X - windowDim.X*0.65 , 20);
                }
            }

            /* Called when a window is being shown. Good for setting an argument to the window*/
	        function OnClientShow(radWindow) {
	            //Create a new Object to be used as an argument to the radWindow  
	            var arg = new Object();
                arg.Body = "";

	            if (-1 != selectedRowId) {
	                arg.RowId = selectedRowId;
	                arg.To = selectedTo;
	                arg.From = senderName;
	                arg.SenderId = senderId;
                    arg.isInbox =  isInbox;
                    arg.MessageFormatId = messageFormatId;
                    
                    if (isInbox) arg.Body = $("#Inbox_MessagePane").html();
                    if (!isInbox) arg.Body = $("#Outbox_MessagePane").html();
	            }

	            arg.UserName = userName;
	            arg.UserID = userID;
                arg.defaultMessageFormatId = defaultMessageFormatId;
	            //Using an Object as a argument is convenient as it allows setting many properties.  
	            //Set the argument object to the radWindow        
	            radWindow.argument = arg;
	        }
	        
	        //Get Argument if isSent is True or False
	        function OnClientClose(oWnd, args) {
                //get the transferred arguments
                var arg = args.get_argument();
                if (arg) {
                    defaultMessageFormatId = arg.defaultMessageFormatId;
                    if (arg.isSent) {  
                        var tabs = $find("<%= RadTabStrip1.ClientID %>");                     
                        var tab = tabs.findTabByText("Outbox");
                        tab.set_selected(true);
                        $("#OutboxFilterDiv").css("visibility","visible");
	                    $("#InboxFilterDiv").css("visibility","hidden");

 	                    EmailSystem.GetRowsListFiltered_Outbox("ReceivedTimestamp", Telerik.Web.UI.GridSortOrder.Descending,<%= sn.UserID %> ,outboxDateFromYear,outboxDateFromMonth,outboxDateFromDay,outboxDateToYear,outboxDateToMonth,outboxDateToDay,outboxTimeHourFrom,outboxTimeMinuteFrom,outboxTimeHourTo,outboxTimeMinuteTo, updateGrid_outbox, updateGridError);
   	                }
                }
            }
            //************************************************************************************************************
	        //******************************   Working with Windows - End ************************************************
	        //************************************************************************************************************
            //************************************************************************************************************
	        //****************************** Select Row / Show Format / Show Message - Start  ****************************
	        //************************************************************************************************************
            function onKeyPress(sender, args) {
	            if (args.get_keyCode() == 13) {
	                args.get_domEvent().stopPropagation();
	                args.get_domEvent().preventDefault();
	                return;
	            }
	        }

            function Inbox_onGridRowSelected(sender, args) {
	            //alert("Row: " + args.get_itemIndexHierarchical() + " selected.");
	            selectedRowId = args.getDataKeyValue("MessageId");
	            selectedTo = args.getDataKeyValue("Recipient"); ;
	            senderName = args.getDataKeyValue("Sender");
	            senderId =  args.getDataKeyValue("SenderId")
	            messageFormatId = args.getDataKeyValue("MessageFormatTypeId");
	            toolbarInbox.findButtonByCommandName("reply").enable();
                toolbarInbox.findButtonByCommandName("Forward").enable();
	            isInbox = true;
	            loadFormTemplate();
	        }

            function Outbox_onGridRowSelected(sender, args) {
	            selectedRowId = args.getDataKeyValue("MessageId");
	            selectedTo = args.getDataKeyValue("Recipient"); ;
	            senderName = args.getDataKeyValue("Sender");
	            messageFormatId = args.getDataKeyValue("MessageFormatTypeId");
                toolbarOutbox.findButtonByCommandName("Forward").enable();
	            isInbox = false;
	            loadFormTemplate();
	        }

            function loadFormTemplate() {
	            EmailSystem.View(messageFormatId,MessageFormat,MessageFormatError);
//	          $.ajax({
//		            type: "Post",
//		            url: "../Services/EmailSystem.asmx/View",
//		            data: "{'fid':'" + messageFormatId + "'}",
//		            contentType: "application/json; charset=utf-8",
//		            dataType: "json",
//		            success: function(transport) {
//		                loadFormTemplateComplete(transport.d);
//		            },
//		            error: function(e) {
//		                alert("Error");
//		            }
//		        });
	        }

            function MessageFormat(transport) {
	            if ( null != transport) {
	                if (isInbox) {
                        var changedStr = transport.replace(/<input/g, "<input readonly='readonly'");
                        changedStr = changedStr.replace(/<textarea/g, "<textarea readonly='readonly'");
                        changedStr = changedStr.replace(/input_/g, "input_inbox_");  
	                    $('#Inbox_MessagePane').html(changedStr);
	                   
	                    var tableView = $find("<%= Inbox_Grid.ClientID %>").get_masterTableView();
   	                    tableView.dataBind();
	                } else if (!isInbox) {
                        var changedStr = transport.replace(/<input/g, "<input readonly='readonly'");
                        changedStr = changedStr.replace(/<textarea/g, "<textarea readonly='readonly'");
                        changedStr = changedStr.replace(/input_/g, "input_outbox_"); 
                        $('#Outbox_MessagePane').html(changedStr); 

                        var tableView = $find("<%= Outbox_Grid.ClientID %>").get_masterTableView();
   	                    tableView.dataBind();
	                }
	                EmailSystem.MessageView(userID,selectedRowId,ShowMessage,ShowMessageError);
	            }
	        }

            function MessageFormatError(transport) {
	            alert('<%= ErrorMessage%>');
	        }

            function ShowMessage(result) { 
                var currentObjectIndex = 0;
	            var mySplitResult = result.split("\r\n");
               
                // One end of the line
                if (2 == mySplitResult.length) {
                    mySplitResult = result.split("\n");
                }
	            if (isInbox) { 
	                for (i = 1; i <= mySplitResult.length; i++) {  
	                    $('#input_inbox_' + i).val(mySplitResult[i-1]);
                          if (null == document.getElementById('input_inbox_' + i)) {
                            //Text area (Unformated message)
                            $('#input_inbox_' + currentObjectIndex).val($('#input_inbox_' + currentObjectIndex).val() +  "\n" + mySplitResult[i-1]);
                        } else {
                            //Multiple textboxes (formated message)
                            $('#input_inbox_' + i).val(mySplitResult[i-1]);
                            currentObjectIndex++;
                        }
                    }
	            } else if (!isInbox) { 
	                for (i = 1; i <= mySplitResult.length; i++) {
                        if (null == document.getElementById('input_outbox_' + i)) {
                            //Text area (Unformated message)
                            $('#input_outbox_' + currentObjectIndex).val($('#input_outbox_' + currentObjectIndex).val() + "\n" + mySplitResult[i-1]);
                        } else {
                            //Multiple textboxes (formated message)
                            $('#input_outbox_' + i).val(mySplitResult[i-1]);
                            currentObjectIndex++;
                        }
                    }
	            }
	        }

            function ShowMessageError(result) {
	           alert('<%= ErrorMessage%>');
	        }
            //************************************************************************************************************
	        //******************************* Select Row / Show Format / Show Message - End  *****************************
	        //************************************************************************************************************
	        //************************************************************************************************************
	        //***********************************************  Sorting   *************************************************
	        //************************************************************************************************************
	        //*********************************************    Inbox    **************************************************
	        var currentSortingOrderInbox = Telerik.Web.UI.GridSortOrder.Descending;
	        function OrderGrid_Inbox(sender, args) {
	            args.set_cancel(true);
	            Order_Inbox();
	        }

	        function Order_Inbox() {
	            var tableView = $find("<%= Inbox_Grid.ClientID %>").get_masterTableView();
	            var sortExpressions = tableView.get_sortExpressions();
	            var sortField = "";
            
	            if ( currentSortingOrderInbox == Telerik.Web.UI.GridSortOrder.Descending ) {
	                currentSortingOrderInbox  = Telerik.Web.UI.GridSortOrder.Ascending;
	            } else {
	                currentSortingOrderInbox  = Telerik.Web.UI.GridSortOrder.Descending;
	            }
	                
	            if (sortExpressions.get_count() > 0) {
	                sortField = sortExpressions.getItem(0).get_fieldName();
	                currentSortingField = sortField;
	            } else {
	                sortField = currentSortingField;
	            }
	            EmailSystem.GetRowsListFiltered_Inbox("ReceivedTimestamp", currentSortingOrderInbox,<%= sn.UserID %> ,inboxDateFromYear,inboxDateFromMonth,inboxDateFromDay,inboxDateToYear,inboxDateToMonth,inboxDateToDay,inboxTimeHourFrom,inboxTimeMinuteFrom,inboxTimeHourTo,inboxTimeMinuteTo, updateGrid_inbox, updateGridError);
	        }
	        //*******************************    Inbox    **********************************
	        //*******************************    Outbox   **********************************
	        var currentSortingOrderOutbox = Telerik.Web.UI.GridSortOrder.Descending;
	        function OrderGrid_Outbox(sender, args) {
	            args.set_cancel(true);
	            Order_Outbox();
	        }

	        function Order_Outbox() {
	            var tableView = $find("<%= Outbox_Grid.ClientID %>").get_masterTableView();
	            var sortExpressions = tableView.get_sortExpressions();
	            var sortField = "";
	            
	             if ( currentSortingOrderOutbox == Telerik.Web.UI.GridSortOrder.Descending ) {
	                currentSortingOrderOutbox  = Telerik.Web.UI.GridSortOrder.Ascending;

	            } else {
	                currentSortingOrderOutbox  = Telerik.Web.UI.GridSortOrder.Descending;
	            }
	            
	            if (sortExpressions.get_count() > 0) {
	                sortField = sortExpressions.getItem(0).get_fieldName();
	                currentSortingField = sortField;
	                sortOrder = sortExpressions.getItem(0).get_sortOrder();
	            } else {
	                sortField = currentSortingField;
	            }
	             EmailSystem.GetRowsListFiltered_Outbox("ReceivedTimestamp",currentSortingOrderOutbox,<%= sn.UserID %> ,inboxDateFromYear,inboxDateFromMonth,inboxDateFromDay,inboxDateToYear,inboxDateToMonth,inboxDateToDay,inboxTimeHourFrom,inboxTimeMinuteFrom,inboxTimeHourTo,inboxTimeMinuteTo, updateGrid_outbox, updateGridError);
	        }
	        //*******************************    Outbox   **********************************
	        //******************************************************************************
	        //******************************  Sorting   ************************************
	        //******************************************************************************
	        //******************************************************************************
	        //**************************  Date and Time Filter Start ***********************
	        //******************************************************************************
	        //*******************************    INBOX   ***********************************
	        var inboxDateFromYear = "", inboxDateFromMonth = "", inboxDateFromDay = "", inboxDateToYear = "", inboxDateToMonth = "", inboxDateToDay = "", inboxTimeHourFrom = "", inboxTimeMinuteFrom = "", inboxTimeHourTo = "", inboxTimeMinuteTo = ""; 
	        
	        function InboxOnDateFromSelected(sender, e) {
                if (e.get_newDate() != null) {
                    inboxDateFromYear = e.get_newDate().getFullYear();
                    inboxDateFromMonth = e.get_newDate().getMonth() + 1;
                    inboxDateFromDay = e.get_newDate().getDate();
                }
            }
            function InboxOnDateToSelected(sender, e) {
                if (e.get_newDate() != null) {
                    inboxDateToYear = e.get_newDate().getFullYear();
                    inboxDateToMonth = e.get_newDate().getMonth() + 1;
                    inboxDateToDay = e.get_newDate().getDate();
                }
            }
            function InboxClientTimeSelectedFrom(sender, e)
            {
                inboxTimeHourFrom = e.get_newTime().getHours();
                inboxTimeMinuteFrom = 0;
            }
            function InboxClientTimeSelectedTo(sender, e)
            {
                inboxTimeHourTo = e.get_newTime().getHours();
                inboxTimeMinuteTo = 0;
            }
            
            function ApplyInboxFilter() {
                //Check if empty
                if (0 == inboxDateFromYear.length || 0 == inboxDateFromMonth.length || 0 == inboxDateFromDay.length || 0 == inboxDateToYear.length || 0 == inboxDateToMonth.length || 0 == inboxDateToDay.length ||
                    0 == inboxTimeHourFrom.length || 0 == inboxTimeMinuteFrom.length || 0 == inboxTimeHourTo.length || 0 == inboxTimeMinuteTo.length){
                    alert('<%= Warning_DateAndTime%>');
                } else { 
                    //Check range
                    var dpInboxFrom = $find("<%= dpInboxFrom.ClientID %>");
                    var dpInboxTo = $find("<%= dpInboxTo.ClientID %>");
                    var dateInboxFrom = dpInboxFrom.get_selectedDate();
                    var dateInboxTo = dpInboxTo.get_selectedDate();
                    //Date.parse(fromDate) > Date.parse(toDate)
                    if(Date.parse(dateInboxTo) < Date.parse(dateInboxFrom)) {
                        alert('<%= Warning_DateFromToDate%>');
                        return false;
                    } else if (Date.parse(dateInboxTo) ==  Date.parse(dateInboxFrom)) {
                        if ( inboxTimeHourTo <= inboxTimeHourFrom) {
                            alert('<%= Warning_DateFromToDate%>');
                            return false;
                        }
                    }              
                    EmailSystem.GetRowsListFiltered_Inbox("ReceivedTimestamp", Telerik.Web.UI.GridSortOrder.Descending,<%= sn.UserID %> ,inboxDateFromYear,inboxDateFromMonth,inboxDateFromDay,inboxDateToYear,inboxDateToMonth,inboxDateToDay,inboxTimeHourFrom,inboxTimeMinuteFrom,inboxTimeHourTo,inboxTimeMinuteTo, updateGrid_inbox, updateGridError);
                }
            }
                        
            function SetInboxFilterDefaultTime() {
                //Set default time for Date / Time filter   from midnight till midnight
                 var dpInboxFrom = $find("<%= dpInboxFrom.ClientID %>");
                 var dpInboxTo = $find("<%= dpInboxTo.ClientID %>");
                 var tpInboxFrom = $find("<%= tpInboxFrom.ClientID %>");
                 var tpInboxTo = $find("<%= tpInboxTo.ClientID %>");   
                 var dateInboxFrom = new Date()
                 var dateInboxTo = new Date();

                 dateInboxFrom.setDate(dateInboxFrom.getDate());
                 dpInboxFrom.set_selectedDate(dateInboxFrom)
                 tpInboxFrom.get_timeView().setTime(0,0,0);

                 dateInboxTo.setDate(dateInboxTo.getDate() + 1)
                 dpInboxTo.set_selectedDate(dateInboxTo)
                 tpInboxTo.get_timeView().setTime(0,0,0)

                 inboxDateFromYear = dateInboxFrom.getFullYear();
                 inboxDateFromMonth = dateInboxFrom.getMonth() + 1;
                 inboxDateFromDay = dateInboxFrom.getDate();
                
                 inboxDateToYear = dateInboxTo.getFullYear();
                 inboxDateToMonth = dateInboxTo.getMonth() + 1;
                 inboxDateToDay = dateInboxTo.getDate();
                  
                 inboxTimeHourFrom = 0;
                 inboxTimeMinuteFrom = 0;
                 inboxTimeHourTo = 0;
                 inboxTimeMinuteTo = 0; 

                  EmailSystem.GetRowsListFiltered_Inbox("ReceivedTimestamp", Telerik.Web.UI.GridSortOrder.Descending,<%= sn.UserID %> ,inboxDateFromYear,inboxDateFromMonth,inboxDateFromDay,inboxDateToYear,inboxDateToMonth,inboxDateToDay,inboxTimeHourFrom,inboxTimeMinuteFrom,inboxTimeHourTo,inboxTimeMinuteTo, updateGrid_inbox, updateGridError);


//                //Set default time for Date / Time filter   9 previous day till now  
//	              var dpInboxFrom = $find("<%= dpInboxFrom.ClientID %>");
//                var dpInboxTo = $find("<%= dpInboxTo.ClientID %>");
//                var tpInboxFrom = $find("<%= tpInboxFrom.ClientID %>");
//                var tpInboxTo = $find("<%= tpInboxTo.ClientID %>");   
//                var dateInboxFrom = new Date();
//                var dateInboxTo = new Date();
//                var curr_hour = dateInboxFrom.getHours();
//                  
//                // From time/date    
//                tpInboxFrom.get_timeView().setTime(9,0,0);
//                if (curr_hour < 9 ) {
//                    dateInboxFrom.setDate(dateInboxFrom.getDate() - 1);
//                    dpInboxFrom.set_selectedDate(dateInboxFrom);
//                } else {
//                    dateInboxFrom.setDate(dateInboxFrom.getDate());
//                    dpInboxFrom.set_selectedDate(dateInboxFrom);
//                }
//                // To time/date    
//                tpInboxTo.get_timeView().setTime(curr_hour + 1,0,0)
//                dateInboxTo.setDate(dateInboxTo.getDate());
//                dpInboxTo.set_selectedDate(dateInboxTo);
//                
//                inboxDateFromYear = dateInboxFrom.getFullYear();
//                inboxDateFromMonth = dateInboxFrom.getMonth() + 1;
//                inboxDateFromDay = dateInboxFrom.getDate();
//                
//                inboxDateToYear = dateInboxTo.getFullYear();
//                inboxDateToMonth = dateInboxTo.getMonth() + 1;
//                inboxDateToDay = dateInboxTo.getDate();
//                
//                inboxTimeHourFrom = 9;
//                inboxTimeMinuteFrom = 0;
//                inboxTimeHourTo = dateInboxTo.getHours() + 1;
//                inboxTimeMinuteTo = 0; 
            }
            //*******************************    INBOX   ***********************************
            //*******************************   OUTBOX   ***********************************
	        var outboxDateFromYear = "", outboxDateFromMonth = "", outboxDateFromDay = "", outboxDateToYear = "", outboxDateToMonth = "", outboxDateToDay = "", outboxTimeHourFrom = "", outboxTimeMinuteFrom = "", outboxTimeHourTo = "", outboxTimeMinuteTo = ""; 
	        
	        function OutboxOnDateFromSelected(sender, e) {
                if (e.get_newDate() != null) {
                    outboxDateFromYear = e.get_newDate().getFullYear();
                    outboxDateFromMonth = e.get_newDate().getMonth() + 1;
                    outboxDateFromDay = e.get_newDate().getDate();
                }
            }
            function OutboxOnDateToSelected(sender, e) {
                if (e.get_newDate() != null) {
                    outboxDateToYear = e.get_newDate().getFullYear();
                    outboxDateToMonth = e.get_newDate().getMonth() + 1;
                    outboxDateToDay = e.get_newDate().getDate();
                }
            }
            function OutboxClientTimeSelectedFrom(sender, e)
            {
                outboxTimeHourFrom = e.get_newTime().getHours();
                outboxTimeMinuteFrom = 0;//e.get_newTime().getMinutes();
            }
            function OutboxClientTimeSelectedTo(sender, e)
            {
                outboxTimeHourTo = e.get_newTime().getHours();
                outboxTimeMinuteTo = 0;//e.get_newTime().getMinutes();
            }
            function ApplyOutboxFilter() {
                if (0 == outboxDateFromYear.length || 0 == outboxDateFromMonth.length || 0 == outboxDateFromDay.length || 0 == outboxDateToYear.length || 0 == outboxDateToMonth.length || 0 == outboxDateToDay.length ||
                    0 == outboxTimeHourFrom.length || 0 == outboxTimeMinuteFrom.length || 0 == outboxTimeHourTo.length || 0 == outboxTimeMinuteTo.length ){
                    alert('<%= Warning_DateAndTime%>');
                } else {
                    //Check range
                    var dpOutboxFrom = $find("<%= dpOutboxFrom.ClientID %>");
                    var dpOutboxTo = $find("<%= dpOutboxTo.ClientID %>");
                    var dateOutboxFrom = dpOutboxFrom.get_selectedDate();
                    var dateOutboxTo = dpOutboxTo.get_selectedDate();
                    
                    if(Date.parse(dateOutboxTo) < Date.parse(dateOutboxFrom)) {
                        alert('<%= Warning_DateFromToDate%>');
                        return false;
                    } else if (Date.parse(dateOutboxTo) ==  Date.parse(dateOutboxFrom)) {
                        if ( outboxTimeHourTo <= outboxTimeHourFrom) {
                            alert('<%= Warning_DateFromToDate%>');
                            return false;
                        }
                    }              
                   EmailSystem.GetRowsListFiltered_Outbox("ReceivedTimestamp", Telerik.Web.UI.GridSortOrder.Descending,<%= sn.UserID %> ,outboxDateFromYear,outboxDateFromMonth,outboxDateFromDay,outboxDateToYear,outboxDateToMonth,outboxDateToDay,outboxTimeHourFrom,outboxTimeMinuteFrom,outboxTimeHourTo,outboxTimeMinuteTo, updateGrid_outbox, updateGridError);
                }
            }
                       
            function SetOutboxFilterDefaultTime() {
                //Set default time for Date / Time filter   from midnight till midnight
                var dpOutboxFrom = $find("<%= dpOutboxFrom.ClientID %>");
                var dpOutboxTo = $find("<%= dpOutboxTo.ClientID %>");
                var tpOutboxFrom = $find("<%= tpOutboxFrom.ClientID %>");
                var tpOutboxTo = $find("<%= tpOutboxTo.ClientID %>");   
                var dateOutboxFrom = new Date();
                var dateOutboxTo = new Date();


                dateOutboxFrom.setDate(dateOutboxFrom.getDate());
                dpOutboxFrom.set_selectedDate(dateOutboxFrom)
                tpOutboxFrom.get_timeView().setTime(0,0,0);

                dateOutboxTo.setDate(dateOutboxTo.getDate() + 1)
                dpOutboxTo.set_selectedDate(dateOutboxTo)
                tpOutboxTo.get_timeView().setTime(0,0,0)

                outboxDateFromYear = dateOutboxFrom.getFullYear();
                outboxDateFromMonth = dateOutboxFrom.getMonth() + 1;
                outboxDateFromDay = dateOutboxFrom.getDate();
                
                outboxDateToYear = dateOutboxTo.getFullYear();
                outboxDateToMonth = dateOutboxTo.getMonth() + 1;
                outboxDateToDay = dateOutboxTo.getDate();
                  
                outboxTimeHourFrom = 0;
                outboxTimeMinuteFrom = 0;
                outboxTimeHourTo = 0;
                outboxTimeMinuteTo = 0; 

                 EmailSystem.GetRowsListFiltered_Outbox("ReceivedTimestamp", Telerik.Web.UI.GridSortOrder.Descending,<%= sn.UserID %> ,outboxDateFromYear,outboxDateFromMonth,outboxDateFromDay,outboxDateToYear,outboxDateToMonth,outboxDateToDay,outboxTimeHourFrom,outboxTimeMinuteFrom,outboxTimeHourTo,outboxTimeMinuteTo, updateGrid_outbox, updateGridError);

//                //Set default time for Date / Time filter    
//	            var dpOutboxFrom = $find("<%= dpOutboxFrom.ClientID %>");
//                var dpOutboxTo = $find("<%= dpOutboxTo.ClientID %>");
//                var tpOutboxFrom = $find("<%= tpOutboxFrom.ClientID %>");
//                var tpOutboxTo = $find("<%= tpOutboxTo.ClientID %>");   
//                var dateOutboxFrom = new Date();
//                var dateOutboxTo = new Date();
//                var curr_hour = dateOutboxFrom.getHours();
//                  
//                // From time/date    
//                tpOutboxFrom.get_timeView().setTime(9,0,0);
//                if (curr_hour < 9 ) {
//                    dateOutboxFrom.setDate(dateOutboxFrom.getDate() - 1);
//                    dpOutboxFrom.set_selectedDate(dateOutboxFrom);
//                } else {
//                    dateOutboxFrom.setDate(dateOutboxFrom.getDate());
//                    dpOutboxFrom.set_selectedDate(dateOutboxFrom);
//                }
//                // To time/date    
//                tpOutboxTo.get_timeView().setTime(curr_hour + 1,0,0)
//                dateOutboxTo.setDate(dateOutboxTo.getDate());
//                dpOutboxTo.set_selectedDate(dateOutboxTo);
//                
//                outboxDateFromYear = dateOutboxFrom.getFullYear();
//                outboxDateFromMonth = dateOutboxFrom.getMonth() + 1;
//                outboxDateFromDay = dateOutboxFrom.getDate();
//                
//                outboxDateToYear = dateOutboxTo.getFullYear();
//                outboxDateToMonth = dateOutboxTo.getMonth() + 1;
//                outboxDateToDay = dateOutboxTo.getDate();
//                
//                outboxTimeHourFrom = 9;
//                outboxTimeMinuteFrom = 0;
//                outboxTimeHourTo = dateOutboxTo.getHours() + 1;
//                outboxTimeMinuteTo = 0;  
            }
            //*******************************   OUTBOX   ***********************************
            //******************************************************************************
	        //**************************  Date and Time Filter End *************************
	        //******************************************************************************
	        function OnRowDataBound(sender, args) { }
	        function onWindowLoad(sender, args) { }
	        /* ]]> */
	    </script>
	    </telerik:RadScriptBlock>
	   
	    <div id="GridDiv">
	    
	    <telerik:RadWindowManager runat="Server" ID="RadWindowManager1" EnableViewState="False" Behavior="Default" InitialBehavior="None" meta:resourcekey="RadWindowManager1Resource1">
		    <Windows>
			    <telerik:RadWindow runat="server" ID="Edit" NavigateUrl="Reply.aspx" OnClientPageLoad="onWindowLoad" OnClientShow="OnClientShow" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="True" Left ="10px" Behaviors="Close" 
                    VisibleStatusbar="false" OnClientClose="OnClientClose" meta:resourcekey="EditResource1">
			    </telerik:RadWindow>
                <telerik:RadWindow runat="server" ID="Forward" NavigateUrl="Forwarding.aspx" OnClientPageLoad="onWindowLoad" OnClientShow="OnClientShow" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="True" Left ="10px" Behaviors="Close" 
                    VisibleStatusbar="false" OnClientClose="OnClientClose">
			    </telerik:RadWindow>
			    <telerik:RadWindow runat="server" ID="NewMessage" NavigateUrl="NewMessage.aspx" OnClientPageLoad="onWindowLoad" OnClientShow="OnClientShow" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="True" Left ="10px" Behaviors="Close" 
                    VisibleStatusbar="false" OnClientClose="OnClientClose" meta:resourcekey="NewMessageResource1">
			    </telerik:RadWindow>
			    <telerik:RadWindow runat="server" ID="NewMessageWithFormat" 
                    NavigateUrl="NewMessageWithFormat.aspx" OnClientPageLoad="onWindowLoad" OnClientShow="OnClientShow" ReloadOnShow="true" ShowContentDuringLoad="false" Modal="True" Left ="10px" Behaviors="Close" 
                    VisibleStatusbar="false" OnClientClose="OnClientClose" meta:resourcekey="NewMessageWithFormatResource1" >
			    </telerik:RadWindow>
		    </Windows>
	    </telerik:RadWindowManager>
	    
	    <telerik:RadTabStrip ID="RadTabStrip1" Skin ="Standart" Height ="30px" CssClass ="TabsCss"  EnableEmbeddedSkins ="False" ReorderTabsOnSelect="True" runat="server" MultiPageID="RadMultiPage1" SelectedIndex="0" OnClientTabSelecting="onTabSelecting"  BackColor ="#B4B894" meta:resourcekey="RadTabStrip1Resource1">
            <Tabs>
                <telerik:RadTab Text="Inbox" Font-Size ="X-Small" Width ="100px" meta:resourcekey="RadTabResource1"/>
                <telerik:RadTab Text="Outbox" Font-Size ="X-Small"  Width ="100px" meta:resourcekey="RadTabResource2"/>
            </Tabs>
        </telerik:RadTabStrip>
       
        <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" Width ="100%" meta:resourcekey="RadMultiPage1Resource1"> 
            <telerik:RadPageView ID="RadPageView1" runat="server" meta:resourcekey="RadPageView1Resource1" Selected="True">
                <telerik:RadSplitter runat="server" ID="RadSplitter1" Width="100%" BorderSize="0" BorderStyle="None" PanesBorderSize="0" Height="100%" Orientation="Horizontal" FullScreenMode="True" meta:resourcekey="RadSplitter1Resource1" SplitBarsSize="">
		            <telerik:RadPane runat="server" ID="RadPane1" Height ="30px" EnableViewState="False" Scrolling ="None" Index="0" meta:resourcekey="RadPane1Resource1">
			            <telerik:RadToolBar Width="100%" runat="server" Skin="Hay" ID="RadToolBar1" OnClientButtonClicked="onButtonClicked" EnableViewState="False" meta:resourcekey="RadToolBar1Resource1">
				            <Items>
				                <telerik:RadToolBarButton Text="New Message" Font-Size ="X-Small" ImageUrl="~/Assets/Images/EmailSystem/mail_unread.png" CommandName="NewMessage" runat="server" meta:resourcekey="RadToolBarButtonResource1" Owner="" />
				                <telerik:RadToolBarButton IsSeparator="True" CommandName ="NewFormMessageSeparator" runat="server" meta:resourcekey="RadToolBarButtonResource2" Owner=""/>
				                <telerik:RadToolBarButton Text="New Form Message" Font-Size ="X-Small" ImageUrl="~/Assets/Images/EmailSystem/mail_unread.png" CommandName="NewFormMessage" runat="server" meta:resourcekey="RadToolBarButtonResource3" Owner="" />
                                <telerik:RadToolBarButton IsSeparator="True" runat="server" meta:resourcekey="RadToolBarButtonResource4" Owner=""/>
					            <telerik:RadToolBarButton Text="Forward" Font-Size ="X-Small" ImageUrl="~/Assets/Images/EmailSystem/mail_forward.png" CommandName="Forward" runat="server" Owner="" />
				                <telerik:RadToolBarButton IsSeparator="True" runat="server" meta:resourcekey="RadToolBarButtonResource4" Owner=""/>
					            <telerik:RadToolBarButton Text="Reply" Font-Size ="X-Small" ImageUrl="~/Assets/Images/EmailSystem/mail_reply.png" CommandName="reply" runat="server" meta:resourcekey="RadToolBarButtonResource5" Owner="" />
				            </Items>
			            </telerik:RadToolBar>
		            </telerik:RadPane>
		            <telerik:RadPane runat="server" ID="RadPane6" Height="300px" Index="1" meta:resourcekey="RadPane6Resource1">
        			    <telerik:RadGrid runat="server" ID="Inbox_Grid" Skin="Hay" AutoGenerateColumns="False" Height="300px" Width="100%" GridLines="None" BorderWidth="0px" AllowSorting="True" Style="outline: none" meta:resourcekey="Inbox_GridResource1" >
    				    <ClientSettings ReorderColumnsOnClient="True" AllowColumnsReorder="True" EnableRowHoverStyle="True">
    				        <Selecting AllowRowSelect="True"/>
                            <ClientEvents OnCommand="OrderGrid_Inbox" OnRowDataBound="OnRowDataBound" OnRowSelected="Inbox_onGridRowSelected"/>
                            <Scrolling AllowScroll="True" UseStaticHeaders="True"/>
                        </ClientSettings>
    				    <MasterTableView TableLayout="Fixed" GroupLoadMode="Client" DataKeyNames="MessageId" ClientDataKeyNames="MessageId,MessageReferenceId,MessageFormatTypeId,ReceivedTimestamp,Subject,FieldValues,Sender,SenderId,Recipient,IsRead,MessageStatus">
			            <NoRecordsTemplate><center><div id="InboxNoDataDiv"></div></center></NoRecordsTemplate>
                            <Columns>
                                <telerik:GridBoundColumn DataField="MessageId" meta:resourcekey="GridBoundColumnResource1" UniqueName="MessageId" Visible="False"/>
                                <telerik:GridBoundColumn DataField="MessageReferenceId" meta:resourcekey="GridBoundColumnResource2" UniqueName="MessageReferenceId" Visible="False"/>
                                <telerik:GridBoundColumn DataField="MessageFormatTypeId" meta:resourcekey="GridBoundColumnResource3" UniqueName="MessageFormatTypeId" Visible="False"/>
                                <telerik:GridBoundColumn DataField="FieldValues" HeaderText="FieldValues" meta:resourcekey="GridBoundColumnResource4" UniqueName="FieldValues" Visible="False"/>
                                <telerik:GridBoundColumn DataField="IsRead" meta:resourcekey="GridBoundColumnResource5" UniqueName="IsRead" Visible="False"/>
                                <telerik:GridBoundColumn DataField="Sender" HeaderText="Sender" meta:resourcekey="GridBoundColumnResource6" UniqueName="Sender"/>
                                <telerik:GridBoundColumn DataField="SenderId" meta:resourcekey="GridBoundColumnResource7" UniqueName="SenderId" Visible="False"/>
                                <telerik:GridBoundColumn DataField="Recipient" HeaderText="Recipient" meta:resourcekey="GridBoundColumnResource8" UniqueName="Recipient"/>
                                <telerik:GridBoundColumn DataField="Subject" HeaderText="Subject" meta:resourcekey="GridBoundColumnResource9" UniqueName="Subject"/>
                                <telerik:GridBoundColumn DataField="ReceivedTimestamp" HeaderText="Date and Time" meta:resourcekey="GridBoundColumnResource10" UniqueName="ReceivedTimestamp"/>
                                <telerik:GridBoundColumn DataField="MessageStatus" HeaderText="Status" meta:resourcekey="GridBoundColumnResource11" UniqueName="MessageStatus"/>
                            </Columns>
				        </MasterTableView>
			            </telerik:RadGrid>
		            </telerik:RadPane>
		            <telerik:RadPane runat="server" ID="RadPane3" Index="2" meta:resourcekey="RadPane3Resource1">
		                <hr id="InboxPagerHr"/>
			            <div id="Inbox_MessagePane" style ="position  :absolute; left:10px; overflow :auto;"></div>
    		        </telerik:RadPane>	
	            </telerik:RadSplitter>
            </telerik:RadPageView>
            <telerik:RadPageView ID="RadPageView2" runat="server" meta:resourcekey="RadPageView2Resource1">
                <telerik:RadSplitter runat="server" ID="RadSplitter2" Width="100%" BorderSize="0" BorderStyle="None" PanesBorderSize="0" Height="100%" Orientation="Horizontal" FullScreenMode="True" meta:resourcekey="RadSplitter2Resource1" SplitBarsSize="">
                    <telerik:RadPane runat="server" ID="RadPane2" Height ="30px" EnableViewState="False" Scrolling ="None" Index="0" meta:resourcekey="RadPane2Resource1">
			            <telerik:RadToolBar Width="100%" runat="server" Skin="Hay" ID="RadToolBar2" OnClientButtonClicked="onButtonClicked" EnableViewState="False" meta:resourcekey="RadToolBar2Resource1">
				            <Items>
				                <telerik:RadToolBarButton Text="New Message" Font-Size ="X-Small" ImageUrl="~/Assets/Images/EmailSystem/mail_unread.png" CommandName="NewMessage" runat="server" meta:resourcekey="RadToolBarButtonResource6" Owner=""/>
				                <telerik:RadToolBarButton IsSeparator="True" CommandName="NewFormMessageSeparator" runat="server" meta:resourcekey="RadToolBarButtonResource7" Owner=""/>
				                <telerik:RadToolBarButton Text="New Form Message" Font-Size ="X-Small" ImageUrl="~/Assets/Images/EmailSystem/mail_unread.png" CommandName="NewFormMessage" runat="server" meta:resourcekey="RadToolBarButtonResource8" Owner=""/>
                                <telerik:RadToolBarButton IsSeparator="True" runat="server" meta:resourcekey="RadToolBarButtonResource4" Owner=""/>
					            <telerik:RadToolBarButton Text="Forward" Font-Size ="X-Small"  ImageUrl="~/Assets/Images/EmailSystem/mail_forward.png" CommandName="Forward" runat="server" Owner="" />
				                <telerik:RadToolBarButton IsSeparator="True" runat="server" meta:resourcekey="RadToolBarButtonResource9" Owner=""/>
					            <telerik:RadToolBarButton Text="Reply" Font-Size ="X-Small" ImageUrl="~/Assets/Images/EmailSystem/mail_reply.png" CommandName="reply" runat="server" meta:resourcekey="RadToolBarButtonResource10" Owner=""/>
				            </Items>
			            </telerik:RadToolBar>
		            </telerik:RadPane>
			        <telerik:RadPane runat="server" ID="RadPane4" Height="300px" Index="1" meta:resourcekey="RadPane4Resource1">
                        <telerik:RadGrid runat="server" ID="Outbox_Grid" Skin="Hay" AutoGenerateColumns="False" Height="300px" Width="100%" GridLines="None" BorderWidth="0px" AllowSorting="True" Style="outline: none" meta:resourcekey="Outbox_GridResource1" >
    				    <ClientSettings ReorderColumnsOnClient="True" AllowColumnsReorder="True" EnableRowHoverStyle="True">
    				        <Selecting AllowRowSelect="True"/>
                            <ClientEvents OnCommand="OrderGrid_Outbox" OnRowDataBound="OnRowDataBound" OnRowSelected="Outbox_onGridRowSelected"/>
                            <Scrolling AllowScroll="True" UseStaticHeaders="True"/>
                         </ClientSettings>
    				    <MasterTableView TableLayout="Fixed" GroupLoadMode="Client" DataKeyNames="MessageId" ClientDataKeyNames="MessageId,MessageReferenceId,MessageFormatTypeId,ReceivedTimestamp,Subject,FieldValues,Sender,SenderId,Recipient,IsRead,MessageStatus">
			            <NoRecordsTemplate><center><div id="OutboxNoDataDiv"></div></center></NoRecordsTemplate>
                            <Columns>
                                <telerik:GridBoundColumn DataField="MessageId" meta:resourcekey="GridBoundColumnResource12" UniqueName="MessageId" Visible="False"/>
                                <telerik:GridBoundColumn DataField="MessageReferenceId" meta:resourcekey="GridBoundColumnResource13" UniqueName="MessageReferenceId" Visible="False"/>
                                <telerik:GridBoundColumn DataField="MessageFormatTypeId" meta:resourcekey="GridBoundColumnResource14" UniqueName="MessageFormatTypeId" Visible="False"/>
                                <telerik:GridBoundColumn DataField="FieldValues" HeaderText="FieldValues" meta:resourcekey="GridBoundColumnResource15" UniqueName="FieldValues" Visible="False"/>
                                <telerik:GridBoundColumn DataField="IsRead" meta:resourcekey="GridBoundColumnResource16" UniqueName="IsRead" Visible="False"/>
                                <telerik:GridBoundColumn DataField="Sender" HeaderText="Sender" meta:resourcekey="GridBoundColumnResource17" UniqueName="Sender"/>
                                <telerik:GridBoundColumn DataField="SenderId" meta:resourcekey="GridBoundColumnResource18" UniqueName="SenderId" Visible="False"/>
                                <telerik:GridBoundColumn DataField="Recipient" HeaderText="Recipient" meta:resourcekey="GridBoundColumnResource19" UniqueName="Recipient"/>
                                <telerik:GridBoundColumn DataField="Subject" HeaderText="Subject" meta:resourcekey="GridBoundColumnResource20" UniqueName="Subject"/>
                                <telerik:GridBoundColumn DataField="ReceivedTimestamp" HeaderText="Date and Time" meta:resourcekey="GridBoundColumnResource21" UniqueName="ReceivedTimestamp"/>
                                <telerik:GridBoundColumn DataField="MessageStatus" HeaderText="Status" meta:resourcekey="GridBoundColumnResource22" UniqueName="MessageStatus"/>
                            </Columns>
				        </MasterTableView>
			            </telerik:RadGrid>
		            </telerik:RadPane>
		            <telerik:RadPane runat="server" ID="RadPane5" Index="2" meta:resourcekey="RadPane5Resource1">
                        <hr id="Hr1"/>
		                <div id="Outbox_MessagePane"  style ="position  :absolute; left:10px; overflow :auto;"></div>
		            </telerik:RadPane>	
                </telerik:RadSplitter>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
        <!-- Inbox Date and Time Pickers -->
        <div id="InboxFilterDiv" style="position :absolute ; top:5px;">
            <div id="Div4" style ="position:absolute; left:250px; top:2px;">
	            <asp:Label ID="Label1" runat ="server" Text ="From:" Font-Size="X-Small" meta:resourcekey="Label1Resource1" />
		    </div>
            <div id="Inbox_DateTimeFilter" style ="position  :absolute; left:285px; top:0px;">
                <telerik:RadDatePicker ID="dpInboxFrom" runat="server" Width="100px" Font-Size="XX-Small" Culture="en-CA" meta:resourcekey="dpInboxFromResource1">
                    <Calendar runat ="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"/>
                    <DateInput runat="server" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelCssClass="" Width=""/>
                    <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""/>
                    <ClientEvents OnDateSelected="InboxOnDateFromSelected"/>
                </telerik:RadDatePicker> 
			</div>
			<div id="Div1" style ="position  :absolute; left:385px; top:0px;">
			    <telerik:RadTimePicker runat ="server" ID="tpInboxFrom" Width ="90px" Font-Size="XX-Small" Culture="en-CA" meta:resourcekey="tpInboxFromResource1">
                    <Calendar runat="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"/>
                    <DatePopupButton CssClass="" ImageUrl="" HoverImageUrl=""/>
                    <TimeView ID="TimeView1" runat="server" OnClientTimeSelected="InboxClientTimeSelectedFrom"/>
                    <TimePopupButton CssClass="" ImageUrl="" HoverImageUrl=""/>
                    <DateInput runat ="server" Width="" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelCssClass=""/>
			    </telerik:RadTimePicker>
			</div>
    		<div id="Div5" style ="position  :absolute; left:480px; top:2px;">
			    <asp:Label ID="Label4" runat ="server" Text ="To:" Font-Size="X-Small" meta:resourcekey="Label4Resource1"/>
			</div>
    		<div id="Div2" style ="position  :absolute; left:500px; top:0px;">
			    <telerik:RadDatePicker ID="dpInboxTo" runat="server" Width="100px" Font-Size="XX-Small" Culture="en-CA" meta:resourcekey="dpInboxToResource1">
                    <Calendar runat ="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"/>
                    <DateInput runat ="server" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelCssClass="" Width=""/>
                    <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""/>
			        <ClientEvents OnDateSelected="InboxOnDateToSelected"/> 
			    </telerik:RadDatePicker>
			</div>
    		<div id="Div3" style ="position  :absolute; left:600px; top:0px;">
			    <telerik:RadTimePicker runat ="server" ID="tpInboxTo" Width ="90px" Font-Size="XX-Small" Culture="en-CA" meta:resourcekey="tpInboxToResource1">
                    <Calendar runat ="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"/>
                    <DatePopupButton CssClass="" ImageUrl="" HoverImageUrl=""/>
                    <TimeView ID="TimeView2" runat="server" OnClientTimeSelected="InboxClientTimeSelectedTo"/>
                    <TimePopupButton CssClass="" ImageUrl="" HoverImageUrl=""/>
                    <DateInput runat ="server" Width="" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelCssClass=""/>
			    </telerik:RadTimePicker>
			</div>
    		<div id="Div6" style ="position  :absolute; left:695px; top:1px;">
			    <asp:Button ID="Button1" runat="server" Text ="Apply" OnClientClick ="ApplyInboxFilter(); return false" Font-Size ="XX-Small"  Width ="60px" meta:resourcekey="Button1Resource1"/> 
            </div>
            <div id="Div7" style ="position  :absolute; left:760px; top:1px;">
	            <asp:Button ID="Button4" runat="server" Text ="Clear" OnClientClick ="SetInboxFilterDefaultTime(); return false" Font-Size ="XX-Small" Width ="60px" meta:resourcekey="Button4Resource1"/> 
		    </div>
        </div>
       
        <!-- Outbox Date and Time Pickers -->
        <div id="OutboxFilterDiv" style="position :absolute ; top:5px; visibility:hidden">
            <div id="Div8" style ="position:absolute; left:250px; top:2px;">
			    <asp:Label ID="Label2" runat ="server" Text ="From:" Font-Size="X-Small" meta:resourcekey="Label2Resource1"/>
			</div>
    		<div id="Div9" style ="position  :absolute; left:285px; top:0px;">
			    <telerik:RadDatePicker ID="dpOutboxFrom" runat="server" Width="100px" Font-Size="XX-Small" Culture="en-CA" meta:resourcekey="dpOutboxFromResource1">
                    <Calendar runat ="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"/>
                    <DateInput runat ="server" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelCssClass="" Width=""/>
                    <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""/>
			        <ClientEvents OnDateSelected="OutboxOnDateFromSelected"/>
               </telerik:RadDatePicker> 
            </div>
            <div id="Div10" style ="position  :absolute; left:385px; top:0px;">
               <telerik:RadTimePicker runat ="server" ID="tpOutboxFrom" Width ="90px" Font-Size="XX-Small" Culture="en-CA" meta:resourcekey="tpOutboxFromResource1">
                    <Calendar runat ="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"/>
                    <DatePopupButton CssClass="" ImageUrl="" HoverImageUrl=""/>
	                <TimeView ID="TimeView3" runat="server" OnClientTimeSelected="OutboxClientTimeSelectedFrom"/>
                    <TimePopupButton CssClass="" ImageUrl="" HoverImageUrl=""/>
                    <DateInput runat ="server" Width="" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelCssClass=""/>
               </telerik:RadTimePicker>
            </div>
            <div id="Div11" style ="position  :absolute; left:480px; top:2px;">
                <asp:Label ID="Label3" runat ="server" Text ="To:" Font-Size="X-Small" meta:resourcekey="Label3Resource1"/>
            </div>  
            <div id="Div12" style ="position  :absolute; left:500px; top:0px;">
                <telerik:RadDatePicker ID="dpOutboxTo" runat="server" Width="100px" Font-Size="XX-Small" Culture="en-CA" meta:resourcekey="dpOutboxToResource1">
                    <Calendar runat ="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"/>
                    <DateInput runat ="server" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelCssClass="" Width=""/>
                    <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""/>
                    <ClientEvents OnDateSelected="OutboxOnDateToSelected"/> 
                </telerik:RadDatePicker>
            </div>
            <div id="Div13" style ="position  :absolute; left:600px; top:0px;">
                <telerik:RadTimePicker runat ="server" ID="tpOutboxTo" Width ="90px" Font-Size="XX-Small" Culture="en-CA" meta:resourcekey="tpOutboxToResource1">
                    <Calendar runat ="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"/>
                    <DatePopupButton CssClass="" ImageUrl="" HoverImageUrl=""/>
                    <TimeView ID="TimeView4" runat="server" OnClientTimeSelected="OutboxClientTimeSelectedTo"/>
                    <TimePopupButton CssClass="" ImageUrl="" HoverImageUrl=""/>
                    <DateInput runat ="server" Width="" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" LabelCssClass=""/>
                </telerik:RadTimePicker>
            </div>
            <div id="Div14" style ="position  :absolute; left:695px; top:1px;">
                <asp:Button ID="Button2" runat="server" Text ="Apply" OnClientClick ="ApplyOutboxFilter(); return false" Font-Size ="XX-Small" Width ="60px" meta:resourcekey="Button2Resource1"/> 
            </div>
            <div id="Div15" style ="position  :absolute; left:760px; top:1px;">
                <asp:Button ID="Button3" runat="server" Text ="Clear" OnClientClick ="SetOutboxFilterDefaultTime(); return false" Font-Size ="XX-Small" Width ="60px" meta:resourcekey="Button3Resource1"/> 
            </div>
        </div>
    </div>
    </form>
</body>
</html>
