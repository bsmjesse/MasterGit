<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmAlarms.aspx.cs" Inherits="SentinelFM.Messages_frmAlarms"  Culture="en-US"  UICulture="auto" meta:resourcekey="PageResource2" %>
<% if (QueryType != "export")
   {  %>
<%--<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>--%>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik"%>

<%--<%@ Register Assembly="ISNet.WebUI.WebInput" Namespace="ISNet.WebUI.WebControls"
   TagPrefix="ISWebInput" %>

<%@ Register Assembly="BusyBoxDotNet" Namespace="BusyBoxDotNet" TagPrefix="busyboxdotnet" %>

<%@ Register assembly="ISNet.WebUI.WebCombo" namespace="ISNet.WebUI.WebCombo" tagprefix="ISWebCombo" %>

<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>--%>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>frmMessages</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <link rel="stylesheet" href="//cdn.datatables.net/1.10.7/css/jquery.dataTables.css" />
    <link rel="stylesheet" href="//code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
    <link rel="stylesheet" href="../Scripts/jquery-plugin/dropit/dropit.css" type="text/css">    
    
    <script type="text/javascript" src="//code.jquery.com/jquery-1.11.1.min.js"></script>
    <script type="text/javascript" src="//cdn.datatables.net/1.10.7/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="//code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-plugin/dropit/dropit.js"></script>   

    <script language="javascript">
		<!--

			function ScrollColor()
			{
				with(document.body.style)
				{
				scrollbarDarkShadowColor="003366";
				scrollbar3dLightColor="gray";
				scrollbarArrowColor="gray";
				scrollbarBaseColor="FFFFFF";
				scrollbarFaceColor="FFFFFF";
				scrollbarHighlightColor="gray";
				scrollbarShadowColor="black";
				scrollbarTrackColor="whitesmoke";
				}
			}
			
			
		function MessageInfoWindow(MsgKey) { 
					var mypage='frmMessageInfo.aspx?MsgKey='+MsgKey
					var myname='';
					var w=335;
					var h=360;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
				function NewDriverMsg() { 
    				
                    var combo = ISGetObject("cboDrivers");
                    var driverId=combo.Value;
					var mypage='frmNewDriverMsg.aspx?driverId='+driverId;
					var myname='';
					var w=735;
					var h=480;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
					function NewLocationsWindow() { 
					var mypage='frmNewLocation.aspx'
					var myname='';
					var w=735;
					var h=360;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
				
					function MsgDetails(mt,id,mode) { 
					var mypage='MessageDetail.aspx?mt='+mt+'&id='+id+'&mode='+mode;
					var myname='';
					var w=400;
					var h=600;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=yes,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
		var $j = jQuery.noConflict();

		function exportGrid(format) {
		    var selectedAlarms = '';
		    var filteredRows = odatatable._('tr', {"filter":"applied"});
		    
		    for (var i = 0; i < filteredRows.length; i++)
		    {
		        selectedAlarms += (selectedAlarms == '' ? '' : ',') + filteredRows[i][8];
		    }
		    if(selectedAlarms == '')
		        selectedAlarms = '-1';
		    $j('#alarmIdsForExport').val(selectedAlarms);

		    $('#exportformat').val(format);
		    $("#exportForm").submit();
		}

		//-->
    </script>

    <style type="text/css">
        .style1
        {
            width: 83px;
        }
        
        .WG5-Row img
        {
            left: 0 !important;
        }
        
        .WG5-Row span
        {
            display: inline-block !important;
        }

        #tblAlarmsList span {
            display:none; 
        }

        table.dataTable tbody tr.odd {
            background-color: #f1efe2;
        }
        table.dataTable tbody tr.selected {
            background-color: #b0bed9 !important;
        }

        .menu ul { display: none; } /* Hide before plugin loads */
        .menu ul.dropit-submenu {
            background-color: #fff;
            border: 1px solid #b2b2b2;
            padding: 6px 0;
            margin: 3px 0 0 1px;
            -webkit-border-radius: 3px;
               -moz-border-radius: 3px;
                    border-radius: 3px;
            -webkit-box-shadow: 0px 1px 3px rgba(0,0,0,0.15);
               -moz-box-shadow: 0px 1px 3px rgba(0,0,0,0.15);
                    box-shadow: 0px 1px 3px rgba(0,0,0,0.15);
        }          
        .menu ul.dropit-submenu a {
            display: block;
            font-size: 12px;
            line-height: 25px;
            color: black;
            padding: 0 18px;
            text-align: left;
            text-decoration: none;
            font-family: verdana;
            /*width: 100%;*/
        }
        .menu ul.dropit-submenu a:hover {
            background: #248fc1;
            color: #fff;
            text-decoration: none;
        }
        
        table.dataTable tbody th, table.dataTable tbody td, table.dataTable thead th, .dataTables_wrapper {
            font-family: verdana;
            font-size: 11px;
        }

        table.dataTable tbody th, table.dataTable tbody td {
            padding: 4px 5px;
        }

        table.dataTable thead th, table.dataTable thead td {
            padding: 5px 9px;
        }
        
        table.dataTable tbody td.details {
            padding-left:20px;
        }
    </style>

</head>
<body  >
    <script type="text/javascript">
        var odatatable;
        var ALARM_DATA = <%=ALARM_DATA%>;
        var SelectedLanguage = '<%=((sn.SelectedLanguage != null && sn.SelectedLanguage.Length > 0) ? sn.SelectedLanguage.Substring(0, 2) : "en")%>';
        $j(document).ready(function () {
            odatatable = $j('#tblAlarmsList').dataTable({
                "data": ALARM_DATA.data,
                "order": [[0, "desc"]],
                "language": {
                    "url": SelectedLanguage == 'fr' ? "//cdn.datatables.net/plug-ins/1.10.7/i18n/French.json" : ''
                }
            });

            $j('#tblAlarmsList tbody').on('click', 'tr', function (e) {
                
                $j(this).toggleClass('selected');

                var selectedAlarms = '';
                for (var i = 0; i < odatatable.api().rows('.selected').data().length; i++)
                {
                    selectedAlarms += (selectedAlarms == '' ? '' : '|') + odatatable.api().rows('.selected').data()[i][8];
                }
                $j('#SelectedAlarmIds').val(selectedAlarms);
            });

            $j('.menu').dropit();
        });
    </script>
    <form id="frmMessagesForm" method="post" runat="server">
        <input type="hidden" id="SelectedAlarmIds" name="SelectedAlarmIds" value="" runat ="server" />
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            <script type="text/javascript">

                function onTabSelected(sender, args) {
                    if (args.get_tab().get_value() == "2") {
                        RefreshPage_ViewReport();
                    }
                    if (args.get_tab().get_value() == "1") rebindGrid(false);
                    if (args.get_tab().get_value() == "3") frmScheduleReportListToolBar();
                }

                var hideButtonID = '';
                function OpenCreateWindow(validationGroup, hideButton) {
                    hideButtonID = ''

                    return false;
                }

                //Parameter
                //val: 1 means one time report, 2 means schedule report, 3 means my report
                function PopupSubmit(val) {
                    if (hideButtonID != '') {
                        $telerik.$('#' + hideButtonID).click();
                        hideButtonID = '';
                    }
                }
                function PopupLogin() {
                    if (hideButtonID != '') {

                        hideButtonID = '';
                    }
                }


                //Custom Client Validate 
                function CustomValidateDate(sender, args) {
                    args.IsValid = true;

                    try {
                        var timePickerfrom = $find("<%= cboHoursFrom.ClientID %>");
                        var timeViewfrom = timePickerfrom.get_timeView();
                        var fromTime = timeViewfrom.getTime();
                        var datePickerfrom = $find("<%= txtFrom.ClientID %>");
                        var fromDate = datePickerfrom.get_selectedDate();
                        fromDate.setHours(fromTime.getHours());
                        fromDate.setMinutes(fromTime.getMinutes());


                        var timePickerto = $find("<%= cboHoursTo.ClientID %>");
                        var timeViewto = timePickerto.get_timeView();
                        var toTime = timeViewto.getTime();
                        var datePickerto = $find("<%= txtTo.ClientID %>");
                        var toDate = datePickerto.get_selectedDate();
                        toDate.setHours(toTime.getHours());
                        toDate.setMinutes(toTime.getMinutes());
                        if (fromDate >= toDate) {
                            args.IsValid = false;
                            return;
                        }
                    } catch (err) { }

                    return;
                }

                //************************************************************************************************************
                //                                    Refresh repository page
                //************************************************************************************************************
                var reportIntervalID = setInterval(reflashRepository, 20000);
                function reflashRepository() {

                }
                function refreshAfterSubmit() {

                    pageLoad_repository();
                }

            </script>
        </telerik:RadCodeBlock>
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <asp:ScriptReference Path="~/scripts/jquery.cookie.js" />
                <asp:ScriptReference Path="~/Reports/jqueryFileTree.js" />
                <asp:ScriptReference Path="~/Reports/splitter.js" />
                <asp:ScriptReference Path="~/scripts/tablesorter/jquery.tablesorter.min.js" />
                <asp:ScriptReference Path="~/scripts/colResizable-1.3.min.js" />
                <asp:ScriptReference Path="~/scripts/json2.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Transparency="0" Skin="Hay">
        </telerik:RadAjaxLoadingPanel>
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1">
        </telerik:RadAjaxManager>

        <fieldset>
            <table class="formtext" style="width: 720px" >
                <tr>
                    <td>
                        <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1" Text="From :"></asp:Label>
                    </td>
                    <td style="width: 100px">
                        <telerik:RadDatePicker ID="txtFrom" runat="server" Width="100px" DateInput-EmptyMessage=""
                                    MinDate="01/01/1900" MaxDate="01/01/3000" Skin="Hay"  Culture="en-US" OnLoad="txtFrom_Load" >
                                    <Calendar>
                                        <SpecialDays>
                                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                        </SpecialDays>
                                    </Calendar>
                                    <DateInput LabelCssClass="" />
                        </telerik:RadDatePicker>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="valFromDate" runat="server" ControlToValidate="txtFrom"
                            ValidationGroup="vgSubmit" ErrorMessage="Please select a From" meta:resourcekey="valFromDateResource1" Text="*">
                        </asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 100px">
                        <telerik:RadTimePicker ID="cboHoursFrom" runat="server"  valign="top" meta:resourcekey="cboHoursFromResource1" Skin="Hay" />
                    </td>

                    <td class="style1" >
                        <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1" Text="To:"></asp:Label>
                    </td>
                    <td style="width: 100px">
                        <telerik:RadDatePicker ID="txtTo" runat="server" Width="100px" DateInput-EmptyMessage=""
                                    Skin="Hay" MinDate="01/01/1900" MaxDate="01/01/3000"  Culture="en-US">
                                    <Calendar>
                                        <SpecialDays>
                                            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-CssClass="rcToday" />
                                        </SpecialDays>
                                    </Calendar>
                                    <DateInput LabelCssClass="" />
                                </telerik:RadDatePicker>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator ID="valToDate" runat="server" ControlToValidate="txtTo"
                                        ValidationGroup="vgSubmit" ErrorMessage="Please select a To" meta:resourcekey="valToDateResource1"
                                        Text="*"></asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 100px" valign="top">
                        <telerik:RadTimePicker ID="cboHoursTo" runat="server" valign="top" meta:resourcekey="cboHoursToResource1" Skin="Hay" />
                    </td>
                                                                                
              </tr>

              <tr>
                <td align="left" valign=top colspan="4" >
                    <%--<busyboxdotnet:busybox id="BusyReport" runat="server" anchorcontrol="" compressscripts="False" gzipcompression="False" meta:resourcekey="BusyReportResource1" slideeasing="BackIn" text=""></busyboxdotnet:busybox>--%>
                    <asp:ValidationSummary ID="valSummary" runat="server" CssClass="formtext" meta:resourcekey="valSummaryResource1"> </asp:ValidationSummary>
                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="138px" Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label>
                </td>
             </tr>
  
          <tr>
              <td colspan="8">
                  <asp:CompareValidator ID="valCompareDates"
                runat="server" CssClass="errortext" ControlToValidate="txtTo" ErrorMessage="The From Date should be earlier than the To Date!" Enabled ="false" 
                Type="Date" Operator="GreaterThanEqual" ControlToCompare="txtFrom" meta:resourcekey="valCompareDatesResource1" 
                ValidationGroup="vgSubmit" Text="*"></asp:CompareValidator>
              </td>
          </tr>

            </table>
        </fieldset> 
                    
                    
                     <asp:MultiView ID="MultiviewMessages" runat="server" ActiveViewIndex="0">
                         
                           <asp:View ID="Alarms" runat="server">
                           
                            <table ID="tblAlarms" runat="server" cellpadding="2" cellspacing="2" 
                                      style="width: 328px">
                                      <tr runat="server">
                                          <td align="center" colspan="1" runat="server">
                                              <asp:Button ID="cmdViewAlarms" runat="server" CommandName="25" 
                                                  CssClass="combutton" meta:resourcekey="cmdViewAlarmsResource1" 
                                                  OnClick="cmdViewAlarms_Click" Text="View Alarms" Width="136px" />
                                          </td>
                                          <td align="center" colspan="1" runat="server">
                                              <asp:Button ID="cmdAccept" runat="server" CommandName="25" CssClass="combutton" 
                                                  meta:resourcekey="cmdAcceptResource1" OnClick="cmdAccept_Click" 
                                                  Text="Accept Alarms" Width="136px" />
                                          </td>
                                          <td align="center" colspan="1" runat="server">
                                              <asp:Button ID="cmdCloseAlarms" runat="server" CommandName="25" 
                                                  CssClass="combutton" meta:resourcekey="cmdCloseAlarmsResource1" 
                                                  OnClick="cmdCloseAlarms_Click" Text="Close Alarms" Width="136px" />
                                          </td>
                                          <td align="center" colspan="1" runat="server">
                                              <div style="position: relative; z-index: 3000">
                                                    <ul class="menu">
                                                        <li>
                                                            <a href="#" style="text-decoration:none;">
                                                            <asp:Button ID="Button1" runat="server" CausesValidation="False" 
                                                                CommandName="25" CssClass="combutton" meta:resourcekey="cmdExportMessageResource1" 
                                                                OnClientClick="return false;"  Text="Export" Width="136px" />
                                                            </a>
                                                            <ul>
                                                                <li><a href="javascript:void(0)" onclick="exportGrid('csv');">CSV</a></li>
                                                                <li><a href="javascript:void(0)" onclick="exportGrid('excel2003');">Excel 2003</a></li>
                                                                <li><a href="javascript:void(0)" onclick="exportGrid('excel2007');">Excel 2007</a></li>
                                                                <li><a href="javascript:void(0)" onclick="exportGrid('pdf');">PDF</a></li>
                                                            </ul>
                                                        </li>
                                                    </ul>
                                                </div>
                                          </td>
                                      </tr>
                                  </table>
                                  
                                <div style="height:450px;width:100%;vertical-align:top;padding-top:15px;">
                                    <table id="tblAlarmsList" style="width:100%;">
                                        <thead> 
                                            <tr>
                                                <th style="width:10px;"><%=(string)base.GetLocalResourceObject("dgAlarms_Date")%></th>
                                                <th style="width:10px;"><%=(string)base.GetLocalResourceObject("dgAlarms_Address")%></th>
                                                <th style="width:10px;"><%=(string)base.GetLocalResourceObject("dgAlarms_vehicleDescription")%></th>
                                                <th style="width:10px;"><%=(string)base.GetLocalResourceObject("dgAlarms_AlarmDescription")%></th>
                                                <th style="width:10px;"><%=(string)base.GetLocalResourceObject("dgAlarms_AlarmLevel")%></th>
                                                <th style="width:10px;"><%=(string)base.GetLocalResourceObject("dgAlarms_AlarmState")%></th>
                                                <th style="width:10px;"><%=(string)base.GetLocalResourceObject("dgAlarms_UserName")%></th>
                                                <th style="width:10px;"><%=(string)base.GetLocalResourceObject("dgAlarms_Notes")%></th>                                                                                                                                     
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                                  
                                  <%--<ISWebGrid:WebGrid ID="dgAlarms" runat="server" UseDefaultStyle="True" 
                    Width="100%" OnInitializeDataSource="dgAlarms_InitializeDataSource" 
                    Height="450px" ViewStateStorage="Session" 
                    OnInitializeLayout="dgAlarms_InitializeLayout" 
                    meta:resourcekey="dgAlarmsResource2" ><RootTable DataKeyField="AlarmId"><Columns><ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                                Caption="chkBox" ColumnType="CheckBox" DataMember="chkBox" EditType="NoEdit" IsRowChecker="True"
                                Name="Select Row" ShowInSelectColumns="No" Width="25px"></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_Date %>' DataMember="AlarmDate" DataType="System.DateTime"
                                Name="AlarmDate" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_Address %>'  DataMember="StreetAddress" DataType="System.String"
                                Name="StreetAddress" ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_vehicleDescription %>' DataMember="vehicleDescription" Name="vehicleDescription"
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_AlarmDescription %>' DataMember="AlarmDescription" Name="AlarmDescription"
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_AlarmLevel %>' DataMember="AlarmLevel" Name="AlarmLevel"
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_AlarmState %>' DataMember="AlarmState" Name="AlarmState"
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_UserName %>' DataMember="UserName" Name="UserName" 
                                ></ISWebGrid:WebGridColumn><ISWebGrid:WebGridColumn Caption='<%$ Resources:dgAlarms_Notes %>' DataMember="Notes" Name="Notes"
                                ></ISWebGrid:WebGridColumn></Columns></RootTable>
                                      <LayoutSettings  AllowColumnMove="Yes" AllowContextMenu="False" AllowFilter="Yes" RowChangedAction=OnTheFlyPostback  RowHeightDefault=25px      
                      AllowSorting="Yes"    AutoFilterSuggestion="True" AutoFitColumns="True"
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default" ResetNewRowValuesOnError="False" RowHeaders="Default"  PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" DisplayDetailsOnUnhandledError="False"><FreezePaneSettings AbsoluteScrolling="True" /></LayoutSettings></ISWebGrid:WebGrid>--%>
                      
                      
                                 
                        </asp:View>
                     
                    </asp:MultiView>  
                    
           
              
                    
                          

    
    </form>

    <form id="exportForm" name="exportForm" action="frmAlarms.aspx" method="post" target="frmExport" style="display:none;">
        <input type="hidden" id="QueryType" name="QueryType" value="export" />
        <input type="hidden" id="exportformat" name="exportformat" value="csv" />
        <input type="hidden" id="alarmIdsForExport" name="alarmIdsForExport" value="" />
    </form>
     <iframe id="frmExport" name="frmExport" width="0" height="0"></iframe> 
</body>
</html>
<% } %>