<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmManagingHOS.aspx.cs" Inherits="frmManagingHOS" Theme="TelerikControl" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register Src="../UserControl/FleetOrganizationHierarchy.ascx" TagName="FleetVehicle" TagPrefix="uc1" %>
<%@ Register Src="HOSViewTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">      
        .RegularText
        {}
       
        .style1
        {
            width: 15%;          
            vertical-align:top;
            margin-left:auto;            
        }
        .style2
        {
            width: 85%;         
            vertical-align: top;
            margin-left:auto;
        }
      
         .iframeStyle
        {
            width: 100%;         
            vertical-align: top;
            margin-left:auto;
            height:1000px;
        }
        .style3
        {
            width: 380px;
        }
    </style>
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/Telerik_AddIn.js"></script>
    </head>
    
<body topmargin="5px" leftmargin="3px">
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" AsyncPostBackTimeout = "3600">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>

    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1"
    />
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel2" runat="server" Skin="Hay"   meta:resourcekey="LoadingPanel1Resource2"
     IsSticky="false" BackgroundPosition="Top"  Height="3000"    />
 
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"  OnAjaxRequest="RadAjaxManager1_AjaxRequest" 
         EnableAJAX="false">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls   >
                    <telerik:AjaxUpdatedControl ControlID="pnl" />
                </UpdatedControls>
            </telerik:AjaxSetting>

            <telerik:AjaxSetting AjaxControlID="pnl">
                <UpdatedControls   >
                    <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="LogSheetsGrid">
               <UpdatedControls>
                  <telerik:AjaxUpdatedControl ControlID="lblMessage"  />
                  <telerik:AjaxUpdatedControl ControlID="iframe"  />
                  <telerik:AjaxUpdatedControl ControlID="LogSheetsGrid"  LoadingPanelID= "LoadingPanel2"/>
              </UpdatedControls>
           </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="btnErrorLog">
               <UpdatedControls>
                  <telerik:AjaxUpdatedControl ControlID="pnl" LoadingPanelID="LoadingPanel1" />
              </UpdatedControls>
           </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="dgErrorLog">
               <UpdatedControls>
                  <telerik:AjaxUpdatedControl ControlID="dgErrorLog" LoadingPanelID="LoadingPanel1" />
              </UpdatedControls>
           </telerik:AjaxSetting>

        </AjaxSettings>
    </telerik:RadAjaxManager>
        <div style="text-align: left; height:100%">
        <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="100%"  >
            <tr align="left" >
                <td>
                    <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="100%"  >
                        <tr>
                            <td>
                                <uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdLogs" />
                            </td>
                        </tr>
                        <tr>
                        <td valign="top">
                          <table id="Table3" border="0" cellpadding="3" cellspacing="3" class="frame" style="border-color: #009933;" width="100%" height="800px" >
                          <tr>
                          <td align="center" valign="top" >
     <asp:Panel ID="pnl" runat="server" style="height:100%"  >

           <TABLE id="tblBody" width="100%" align="center" border="0" style="border-collapse:collapse;height:100%" cellspacing="0" runat="server" visible="true">
							<TR>
								<TD>
									<TABLE id="tblForm" width="100%" height="100%" border="0" style="border-collapse:collapse;" cellspacing="0">
										<TR>
											<TD align="center" >
                                                <table border="0" style="border-collapse:collapse;" cellspacing="0">
                                                    <tr>
                                                        <td align="center">
                                                            <table  style="border-width: 1px;border-color:Gray; border-style:solid;width: 550px;" cellspacing="0">
                                                                <tr>
                                                                    <td colspan="3" valign=top class="style3"  align="center"  >
                                                   
                                                            <table border="0" style="border-collapse:collapse; width: 550px; height: 83px;" 
                                                                 cellspacing="0" frame="void">
                                                                  <tr>
                                                                    <td align="left">
                                                                        <asp:Label ID="lblFromTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFromTitleResource1"
                                                                            Text="From:"></asp:Label></td>
                                                                    <td align="left">
                                                                                                                <telerik:RadDatePicker ID="txtFrom" runat="server" Width="100px" Height="17px" Calendar-ShowRowHeaders="false"
                                                                                                                    Calendar-Width="170px" meta:resourcekey="txtFromResource2" MaxDate="9998-12-31"
                                                                                                                    MinDate="1753-01-01" OnLoad="txtFrom_Load">
                                                                                                                    <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ShowRowHeaders="False"
                                                                                                                        ViewSelectorText="x" Width="170px">
                                                                                                                    </Calendar>
                                                                                                                    <DateInput Height="17px" LabelCssClass=""></DateInput>
                                                                                                                    <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
                                                                                                                </telerik:RadDatePicker>                                                                                                                                                                                                                                                                                                                                               
                                                                    </td>
                                                                    <td align="left">
                                                                        <asp:Label ID="lblToTitle" runat="server" CssClass="formtext" meta:resourcekey="lblToTitleResource1"
                                                                            Text="To:"></asp:Label></td>
                                                                    <td align="left">
                                                                                                                <telerik:RadDatePicker ID="txtTo" runat="server" Width="100px" Height="17px" Calendar-ShowRowHeaders="false"
                                                                                                                    Calendar-Width="170px" meta:resourcekey="txtFromResource2" MaxDate="9998-12-31"
                                                                                                                    MinDate="1753-01-01" Calendar-CultureInfo="en-US" >
                                                                                                                    <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ShowRowHeaders="False"
                                                                                                                        ViewSelectorText="x" Width="170px">
                                                                                                                    </Calendar>
                                                                                                                    <DateInput Height="17px" LabelCssClass=""></DateInput>
                                                                                                                    <DatePopupButton ImageUrl="" HoverImageUrl="" CssClass=""></DatePopupButton>
                                                                                                                </telerik:RadDatePicker>                                                                                                               
                                                                       
                                                                    </td>

                                                                    <td align="right" colspan="2">
                                                                     <asp:Button ID="cmdViewAllData" runat="server" 
                                                                            Text="View All Logsheets" OnClick="cmdViewAllData_Click" Height="29px" 
                                                                            style="margin-top: 2px" Width="167px" 
                                                                             />                                                                    
                                                                    </td>
                                                                  </tr>

                                                                  <tr valign="top">
                                                                    <td align="left">
                                                                        <asp:Label ID="lblDriver" runat="server" CssClass="formtext" Text="Driver:" meta:resourcekey="lblDriverResource1"></asp:Label>
                                                                       </td>
                                                                    <td align="left" colspan="3">
                                                                        <asp:DropDownList ID="cboDrivers" runat="server" CssClass="formtext" DataTextField="drivername"
                                                                            DataValueField="driverid" Width="175px" 
                                                                            meta:resourcekey="cboDriversResource1">
                                                                        </asp:DropDownList>
                                                                        <asp:ObjectDataSource ID="HOS_Drivers" runat="server" 
                                                                            OldValuesParameterFormatString="original_{0}" SelectMethod="GetDrivers" 
                                                                            TypeName="HOS_DBTableAdapters.GetDriversTableAdapter">
                                                                            <SelectParameters>
                                                                                <asp:Parameter Name="companyid" Type="Int32" />
                                                                            </SelectParameters>
                                                                        </asp:ObjectDataSource>
                                                                    </td>
                                                                    <td align="right" colspan="2">
                                                                        <asp:Button ID="cmdViewData" runat="server"  
                                                                            Text="View Driver Logsheet" OnClick="cmdViewData_Click" Height="29px" 
                                                                            style="margin-top: 2px" Width="167px" 
                                                                            />

                                                                    </td>
                                                                  </tr>

                                                                  <tr>
                                                                    <td id="FleetColumn" runat="server" class="tableheading" style="width: 52px; height: 14px" align="left">
                                                                    <asp:label id="lblFleet" runat="server" cssclass="tableheading" width="33px" meta:resourcekey="lblFleetResource1"
                                                                     Text ="Fleet:"></asp:label>
                                                                    </td>
                                                                    <td style="width: 312px; height: 14px" align="left" colspan="3">
                                                                    <asp:dropdownlist id="cboFleet" runat="server" cssclass="RegularText"
                                                                    width="258px" datatextfield="FleetName" datavaluefield="FleetId" meta:resourcekey="cboFleetResource1">
                                                                    </asp:dropdownlist>
                                                                    <uc1:FleetVehicle ID="FleetVehicle1" runat="server" />
                                                                    </td>
                                                                    <td style="width: 20px;">                                                                            
                                                                        <asp:CheckBox ID="chkDefaultNode" runat="server" Text="Include Node" Font-Size="Small" />
                                                                    </td>
                                                                    <td align="right" colspan="2">
                                                                                                                                              <asp:Button ID="btnErrorLog" runat="server"  
                                                                            Text="View Sensor Failure"  Height="29px" 
                                                                            style="margin-top: 2px" Width="167px" onclick="btnErrorLog_Click" 
                                                                            />

                                                                   </td>                                                           
                                                                  </tr>
                                                                  <!-- 
                                                                  <tr>
                                                                    <td></td>
                                                                    <td>
                                                                       <a  href="aa" onclick="javascript:return ShowManualPDF()">Manual log sheet</a> 
                                                                    </td>
                                                                  </tr>
                                                                  -->
                                                            </table>                                                                  
                                                           
                                                                    </td>
                                                                </tr>
                                                            </table>                                                           
                                                            <asp:Label ID="lblMessage" runat="server" CssClass="errortext" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                                    </tr>
                                                                                                       
                                                </table>
											</TD>
										</TR>
									</TABLE>
								</TD>
							</TR>                            
						</TABLE>
            <table id = "tbllogsheet" width="100%" border="0" style="border-collapse:collapse;" cellspacing="0" runat="server">
                <tr>
                    <td align="left" class="style1">
                         <asp:Panel runat="server" ID="pnlLogSheetsGrid" style="width: 300px; height: 450px; overflow-y:auto"  >
                                      <asp:GridView ID="LogSheetsGrid" runat="server" AutoGenerateColumns="False" 
                            EnableModelValidation="True"  onrowdatabound="LogSheetsGrid_RowDataBound" Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Id" >
                                            <ItemTemplate >
                                                <asp:LinkButton id="lnkId" runat="server" CommandName="FileDownload" Text = '<%# Eval("refid")%>'
                                                   OnClientClick="return getSelectedRow(this)"  InsertVisible="False"></asp:LinkButton>
                                                
                                                <input type="hidden" id="logSheetFilePath"  value='<%# Eval("FileName")%>' />
                                             </ItemTemplate>
                                         </asp:TemplateField>                               
                                <asp:BoundField DataField="drivername" HeaderText="Driver Name" 
                                    InsertVisible="False" SortExpression="drivername" ReadOnly="True" />
                                                                            <asp:BoundField DataField="date" HeaderText="Date" InsertVisible="False"
                                    SortExpression="date" ReadOnly="True" />
                                <asp:TemplateField HeaderText="File Name" InsertVisible="False" Visible="False">
                                    <ItemTemplate>
                                        <asp:Label id="lblFilePath" runat ="server" text='<%# Eval("FileName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Inspection" >
                                    <ItemTemplate>
                                         <asp:GridView ID="phlInspections" runat="server" ShowHeader="false" AutoGenerateColumns="false"  
                                          onrowdatabound="phlInspections_RowDataBound"
                                         >
                                         <Columns>
                                         <asp:TemplateField HeaderText="Inspection" >
                                             <ItemTemplate >
                                                <asp:LinkButton id="lnkInspection" runat="server" CommandName="Inspection" CommandArgument= '<%# Eval("FileName")%>' Text = '<%# Eval("Inspection")%>'
                                                   OnClientClick='return getSelectedRow(this)' ></asp:LinkButton>
                                                 <input type="hidden" id="InspectionSheetFilePath"  value='<%# Eval("FileName")%>' />
                                                 <input type="hidden" id="InspectionRefId" class="insRefId"  value='<%# Eval("RefId")%>' />
                                                 <input type="hidden" id="InspectionTime" class="insTime" value='<%# Eval("Time")%>' />
                                             </ItemTemplate>
                                         </asp:TemplateField>

                                             
                                          </Columns>  
                                         </asp:GridView>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                            </Columns>
                            <AlternatingRowStyle VerticalAlign="Top" />
                            <RowStyle VerticalAlign="Top" />
                        </asp:GridView>
                        </asp:Panel>
                                        <asp:ObjectDataSource ID="HOS_DS_AllLogSheets" runat="server" 
                                OldValuesParameterFormatString="original_{0}" SelectMethod="GetLogSheets" 
                                TypeName="HOS_DBTableAdapters.GetReportLogSheetTableAdapter">
                                <SelectParameters>
                                    <asp:Parameter Name="companyid" Type="Int32" />
                                    <asp:Parameter Name="start" Type="DateTime" />
                                    <asp:Parameter Name="stop" Type="DateTime" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:ObjectDataSource ID="HOS_DS_DriverLogSheet" runat="server" 
                                OldValuesParameterFormatString="original_{0}" SelectMethod="GetLogSheets" 
                                TypeName="HOS_DBTableAdapters.GetReportLogSheet_ByDriverTableAdapter">
                            <SelectParameters>
                                <asp:Parameter Name="companyid" Type="Int32" />
                                <asp:Parameter Name="start" Type="DateTime" />
                                <asp:Parameter Name="stop" Type="DateTime" />
                                <asp:Parameter Name="driverId" Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td align="left" class="style2">
                        <iframe src="./HOSPDF.aspx" id="iframe" class="iframeStyle" style="visibility:hidden;">
                            <p>Your browser does not support iframes.</p>
                        </iframe>  
                    </td>
                </tr>
            </table>     
            
            
             <table id="tblErrorLog" width="100%" border="0" style="border-collapse:collapse;visibility:hidden;" cellspacing="0" runat="server" visible="true"   >
               <tr>
                  <td align="center">
            <telerik:RadGrid ID="dgErrorLog" runat="server" Visible="true" FilterItemStyle-HorizontalAlign="Left"
                AllowSorting="True" AutoGenerateColumns="False"  LoadingPanelID="LoadingPanel1" 
                AllowFilteringByColumn="true" 
                AllowPaging="true"  PageSize="20"  Width="800px" Height="600px"
                onneeddatasource="dgErrorLog_NeedDataSource" 
                   Skin="Simple" 
                >
                <GroupingSettings CaseSensitive="false" />
                <MasterTableView  CommandItemDisplay="Top"  >
                    <PagerStyle Mode="NextPrevAndNumeric" />
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Box ID" UniqueName="BoxId" DataField="BoxId">
                            <ItemStyle Width="200px"  HorizontalAlign="Left" />
                            <HeaderStyle Width="200px" />

                        </telerik:GridBoundColumn>
                         
                        <telerik:GridBoundColumn HeaderText="Driver" UniqueName="driver" DataField="driver">
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="Error" UniqueName="Errortype" DataField="Errortype">
                            <ItemStyle Width="200px"  HorizontalAlign="Left" />
                            <HeaderStyle Width="200px" />
                        </telerik:GridBoundColumn>

                        <telerik:GridBoundColumn HeaderText="Date Time" DataField="Logtime"
                            UniqueName="Logtime">
                            <ItemStyle Width="200px"  HorizontalAlign="Left" />
                            <HeaderStyle Width="200px" />
                        </telerik:GridBoundColumn>


                    </Columns>
                    <CommandItemTemplate>
                            <table width="100%">
                                <tr>
                                    <td align="left">

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
      </div>
      <asp:HiddenField ID="hidDiv" runat="server" />
      <asp:HiddenField ID="hidFilter" runat="server" />
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgErrorLog.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=dgErrorLog.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgErrorLog.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                }
                function setDateFormat() {                   
                    var picker = $find('<%=txtFrom.ClientID %>');
                    var date = picker.get_selectedDate();
                    date.setDate(date.getDate() + 1);
                    picker.set_selectedDate(date);
                   
            }

        </script>
    </telerik:RadCodeBlock>
    </form>
    <script type="text/javascript">
                  function hidemsg() {
                      try {
                          if (document.getElementById("lblMessage") != undefined) {
                              document.getElementById("lblMessage").style.visibility = 'hidden';
                          }
                      }
                      catch (ex) { }
                  }
                  function ShowManualPDF() {
                      popupWindow = window.open('http://74.200.13.52/ASINew/Reports/HOSManualLogSheet.pdf', 'popUpWindow', 'height=800,width=800,left=100,top=100,resizable=yes,scrollbars=yes,toolbar=yes,menubar=no,location=no,directories=no, status=yes');
                      return false;
                  }

                  function getSelectedRow(lnk)
                  {
                      var fileInput = lnk.parentElement.getElementsByTagName('input');
                      var refId = '';
                      var time = '';
                      if (fileInput.length >= 3) {
                          refId = fileInput[1].value;
                          time = fileInput[2].value;
                      } else {
                          refId = lnk.innerText;
                      }

                      var selectedFilePath = fileInput[0].value;
                      if (selectedFilePath != null)
                      {
                          var path = "./frmManagingExtHOS.aspx?QueryType=DownloadFile&FileName=" + selectedFilePath
                                        + "&RefId=" + refId + "&Time=" + encodeURI(time);
                          
                          document.getElementById('iframe').setAttribute('src', path);
                          document.getElementById("iframe").style.visibility = 'visible';
                          
                          return false;
                      }
                  }
          </script>

</body>
</html>
