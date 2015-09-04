<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHistDataGridExtended.aspx.cs" Inherits="SentinelFM.History_frmHistDataGridExtended" Culture="en-US" meta:resourcekey="PageResource2" UICulture="auto" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <!--<link href="http://www.sentinelfm.com/GlobalStyle.css" type="text/css" rel="stylesheet">-->
     <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    
     <script language="JavaScript">
<!--

         ns = (document.layers)? true:false
	     ie = (document.all)? true:false
	     var agt=navigator.userAgent.toLowerCase();
	     
	     //if (agt.indexOf("firefox") != -1) 
	      //  {
	          
	        //  var grid = ISGetObject("dgHistoryDetails");    
	         // alert(grid); 
	         // grid.LayoutSettings.AutoHeight="false"; 
	          //grid.LayoutSettings.Height=400;
	         
	         
	        //}
			

	function HistoryInfo(dgKey) { 
					var mypage='frmHistDetails.aspx?dgKey='+dgKey
					var myname='';
					var w=580;
					var h=660;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=yes,' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}
				
				
				
					
				function FullScreenGrid() { 
					var mypage='../Map/frmBigDetailsFrame.htm'
					var myname='Data';
					var w=900;
					var h=700;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,resizable=1,toolbar=0,menubar=0,scrollbars=1' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				}		
				
				
				
				function SensorsInfo(LicensePlate) { 
				    
					var mypage='../Map/frmSensorMain.aspx?LicensePlate='+LicensePlate;
					var myname='Sensors';
					var w=460;
					var h=720;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
				} 
				
		 function HistorySearch()
		 {
		            var mypage='frmHistorySearch.aspx'
					var myname='HistorySearch';
					var w=560;
					var h=120;
					var winl = (screen.width - w) / 2; 
					var wint = (screen.height - h) / 2; 
					winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
					win = window.open(mypage, myname, winprops) 
					if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); } 
		 }		
				
		 function ResizeGrid()
         {
            location.href="?clientWidth="+document.body.clientWidth+"&clientHeight="+document.body.clientHeight; 
         }
	



// function onLoad()
//    {

//        var i = document.getElementById("HistoryGrid");
//        var grid = ISGetObject("dgHistoryDetails");
//        var temp=0;
//        
//        if (agt.indexOf("firefox") != -1) 
//         grid.LayoutSettings.AutoHeight=false; 
//  	    
//    }
	
       
        
       


//-->
    </script>
</head>
<body id="body" leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0" >

       <form id="frmHistory" runat="server" >
         
          <table id="Table1" style="left: 4px; width: 99%; height: 100%;" cellspacing="0" cellpadding="0" border="0">
             <tr>
                <td align="left" >
                  <table>
                      <tr>
                         <td >
                            <asp:Label ID="lblVisibleRows" runat="server" CssClass="formtext" Text="Visible Rows" Visible="False"></asp:Label></td>
                         <td>
                            <asp:DropDownList ID="cboRows" runat="server" AutoPostBack="True" CssClass="RegularText"
                               meta:resourcekey="cboGridPagingResource1" OnSelectedIndexChanged="cboRows_SelectedIndexChanged" Visible="False">
                               <asp:ListItem>1</asp:ListItem>
                               <asp:ListItem>2</asp:ListItem>
                               <asp:ListItem Selected="True">5</asp:ListItem>
                               <asp:ListItem>7</asp:ListItem>
                               <asp:ListItem>10</asp:ListItem>
                            </asp:DropDownList>&nbsp;
                         </td>
                         <td >
                            <asp:Label ID="lblPageSize" runat="server" CssClass="formtext" Text="Items per Page:" meta:resourcekey="lblPageSizeResource1"></asp:Label></td>
                         <td >
                            <asp:DropDownList ID="cboGridPaging" runat="server" AutoPostBack="True" CssClass="RegularText"
                               OnSelectedIndexChanged="cboGridPaging_SelectedIndexChanged" meta:resourcekey="cboGridPagingResource1">
                               <asp:ListItem Value="9999" Selected="True" meta:resourcekey="ListItemResource1">All</asp:ListItem>
                               <asp:ListItem meta:resourcekey="ListItemResource2">15</asp:ListItem>
                               <asp:ListItem  meta:resourcekey="ListItemResource3">25</asp:ListItem>
                               <asp:ListItem meta:resourcekey="ListItemResource4">50</asp:ListItem>
                            </asp:DropDownList>&nbsp;
                         </td>
                         <td  >
                            <asp:Button ID="cmdMapSelected" runat="server" CssClass="combutton" OnClick="cmdMapSelected_Click"
                               Text="Map It" meta:resourcekey="cmdMapIt" Width="150px" />
                         </td>
                         <td>
                            <asp:Button ID="cmdVehicleGrid" runat="server" CssClass="combutton" OnClientClick="javascript:FullScreenGrid()" 
                               Text="Vehicle Grid"  meta:resourcekey="cmdVehicleGrid" Width="150px" /></td>
                         <td>
                            <asp:Button ID="cmdSendCommand" runat="server" CssClass="combutton"     
                               Text="Send Command" meta:resourcekey="cmdSendCommand" Width="150px" /></td>
                          <td>
                              <asp:Button ID="cmdSearch" OnClientClick="javascript:HistorySearch();"  runat="server" CssClass="combutton"     
                               Text="Search" Width="150px" Visible="False" /></td>
                         <td>
                            <asp:Button ID="cmdResizeGrid" runat="server" Text="Resize Grid" OnClientClick="javascript:ResizeGrid()"  CssClass="combutton" meta:resourcekey="cmdResizeGridResource1" Visible="False"/></td>
                      </tr>
                   </table>
                </td>
             </tr>
             
             <tr>
                <td valign=top  >
                
                           <div id="HistoryGrid"   > 
                           
                                  
             
          
                                 <ISWebGrid:WebGrid ID="dgHistoryDetails"  Height="90px"   Width="100%"    runat="server"   UseDefaultStyle="True" 
              OnInitializeDataSource="dgHistoryDetails_InitializeDataSource" OnRowChanged="dgHistoryDetails_RowChanged"   OnButtonClick="dgHistoryDetails_ButtonClick" meta:resourcekey="dgHistoryDetailsResource2" OnInitializeRow="dgHistoryDetails_InitializeRow" OnInitializeLayout="dgHistoryDetails_InitializeLayout">
               <RootTable DataKeyField="dgKey">
               
                  <Columns>
                      <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                            ColumnType="CheckBox"  EditType="NoEdit" DataMember="chkBoxShow"
                            IsRowChecker="True" Name="SelectRow" ShowInSelectColumns="No" Width="25px">
                            
                         </ISWebGrid:WebGridColumn>
                         
                     <ISWebGrid:WebGridColumn Caption="dgKey" DataMember="dgKey" DataType="System.Int32"
                        Name="dgKey" Visible="False">
                        
                     </ISWebGrid:WebGridColumn>
                     
                     
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_BoxId %>' DataMember="BoxId" DataType="System.Int32"
                            Name="BoxId" Width="50px">
                         </ISWebGrid:WebGridColumn>
                         
                          
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Vehicle %>' DataMember="Description" DataType="System.String"
                            Name="Description" >
                         </ISWebGrid:WebGridColumn>
                         
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_DateTime %>' DataMember="OriginDateTime" DataType="System.DateTime"
                        Name="MyDateTime" Width="100px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Address %>' DataMember="StreetAddress" Name="StreetAddress"
                        Width="200px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Speed %>' DataMember="Speed" DataType="System.Int32"
                        Name="Speed" Width="50px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_MsgTypeName %>' ColumnType="Template" DataMember="BoxMsgInTypeName"
                        Name="BoxMsgInTypeName" Width="100px">
                        <CellTemplate>
                         <asp:HyperLink ID="HyperLink_Site" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>' Text='<%# DataBinder.Eval(Container.DataItem, "BoxMsgInTypeName") %>' meta:resourcekey="HyperLink_SiteResource1" ></asp:HyperLink>
                        </CellTemplate>
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_MsgDetails %>' DataMember="MsgDetails" Name="MsgDetails"
                        Width="100px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Ack %>' DataMember="Acknowledged" Name="Ack" Width="40px">
                     </ISWebGrid:WebGridColumn>
                      <%-- <ISWebGrid:WebGridColumn ButtonAutoPostback="True" ButtonPostbackMode="FullPagePostback" Caption= '<%$ Resources:dgHist_MapIt %>'
                                                  ButtonText='<%$ Resources:dgHist_MapIt %>' ColumnType="Button" EditType="NoEdit" Name="MapIt" Width="60px">
                                               </ISWebGrid:WebGridColumn>--%>
                 
                                     <ISWebGrid:WebGridColumn Caption="Latitude"   DataMember="Latitude" Name="Latitude"
                        Visible="false">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption="Longitude" DataMember="Longitude" Name="Longitude"
                        Visible="False">
                     </ISWebGrid:WebGridColumn>
                   
                  </Columns> 
               </RootTable>
           
  <LayoutSettings AutoHeight=true  AutoWidth=true AutoColMinWidth=50   AutoFitColumnsBuffering=false AutoFitColumns=true      ShowRefreshButton=False  RowChangedAction=OnTheFlyPostback 
                      AllowSorting="Yes"  RowHeightDefault=25px AllowFilter=Yes  
                      PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False" />
                     
                
                   
          </ISWebGrid:WebGrid>
     
                                      <ISWebGrid:WebGrid ID="dgStops" runat="server"  Height="90px"  Width="100%"   OnButtonClick="dgStops_ButtonClick"
                                         OnInitializeDataSource="dgStops_InitializeDataSource" OnRowChanged="dgStops_RowChanged"
                                         UseDefaultStyle="True"  OnInitializeLayout="dgStops_InitializeLayout" 
                                     meta:resourcekey="dgStopsResource2" OnInitializeRow="dgStops_InitializeRow" 
                                     >
                                         <RootTable DataKeyField="StopIndex">
                                            <Columns>
                                            
                                                <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                            ColumnType="CheckBox"  EditType="NoEdit" DataMember="chkBoxShow"
                            IsRowChecker="True" Name="SelectRow" ShowInSelectColumns="No" Width="25px">
                         </ISWebGrid:WebGridColumn>
                         
                                               <ISWebGrid:WebGridColumn Caption="StopIndex" DataMember="StopIndex" Name="StopIndex"
                                                  Visible="False" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Arrival %>' DataMember="ArrivalDateTime" DataType="System.DateTime"
                                                  Name="ArrivalDateTime" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Address %>' DataMember="Location" Name="Location"
                                                  Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Departure %>' DataMember="DepartureDateTime" DataType="System.DateTime"
                                                  Name="DepartureDateTime" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Duration %>' DataMember="StopDuration" Name="StopDuration"
                                                  Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgStopsTitle_Status %>' DataMember="Remarks" Name="Remarks" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption="Latitude" DataMember="Latitude" Name="Latitude"
                                                  Visible="False" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption="Longitude" DataMember="Longitude" Name="Longitude"
                                                  Visible="False" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                               <ISWebGrid:WebGridColumn Caption="StopDurationVal" DataMember="StopDurationVal" Name="StopDurationVal"
                                                  Visible="False" Width="100px">
                                               </ISWebGrid:WebGridColumn>
                                              <%--  <ISWebGrid:WebGridColumn ButtonAutoPostback="True" ButtonPostbackMode="FullPagePostback" Caption= '<%$ Resources:dgHist_MapIt %>'
                                                  ButtonText='<%$ Resources:dgHist_MapIt %>' ColumnType="Button" EditType="NoEdit" Name="MapIt" Width="60px">
                                               </ISWebGrid:WebGridColumn>--%>
                                            </Columns>
                                         </RootTable>
                                         <LayoutSettings AutoHeight=true  AutoWidth=true AutoColMinWidth=50   AutoFitColumnsBuffering=false AutoFitColumns=true      ShowRefreshButton=False  RowChangedAction=OnTheFlyPostback 
                      AllowSorting="Yes"  RowHeightDefault=25px AllowFilter=Yes  
                      PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False" />
                                      </ISWebGrid:WebGrid>
                                      
                                      
                                      
                                      
                  <ISWebGrid:WebGrid ID="dgTrips" Height="100%"  Width="100%"  UseDefaultStyle="True"
                                     runat="server" OnInitializeDataSource="dgTrips_InitializeDataSource" oninitializerow="dgTrips_InitializeRow" 
                                      >
                                  <RootTable  Caption="Trip Summary" DataKeyField="TripId" DataMember="Table">
                         
                                  <Columns>
                    
                                <ISWebGrid:WebGridColumn  DataMember="Description"   Name="Description" Caption='<%$ Resources:dgTrips_Description %>'
                                  Width="100px">
                                </ISWebGrid:WebGridColumn>
                                  
                                  <ISWebGrid:WebGridColumn  DataMember="DepartureTime"    Name="DepartureTime" Caption='<%$ Resources:dgTrips_Departure %>'
                                  Width="100px">
                                   </ISWebGrid:WebGridColumn>
                                  <ISWebGrid:WebGridColumn  DataMember="ArrivalTime"    Name="ArrivalTime" Caption='<%$ Resources:dgTrips_Arrival %>' 
                                  Width="100px">
                                   </ISWebGrid:WebGridColumn>
         
                                   <ISWebGrid:WebGridColumn  DataMember="_From"    Name="_From" Caption='<%$ Resources:dgTrips_From %>'
                                  Width="100px">
                                   </ISWebGrid:WebGridColumn>
                                   
                                   <ISWebGrid:WebGridColumn  DataMember="_To"    Name="_To" Caption='<%$ Resources:dgTrips_To %>'
                                  Width="100px">
                                   </ISWebGrid:WebGridColumn>
                                   
                                   <ISWebGrid:WebGridColumn  DataMember="Duration"    Name="Duration" Caption='<%$ Resources:dgTrips_Duration %>' 
                                  Width="100px">
                                   </ISWebGrid:WebGridColumn>
                                   
                                    <ISWebGrid:WebGridColumn  DataMember="FuelConsumed"    Name="FuelConsumed" Caption='<%$ Resources:dgTrips_FuelConsumed %>'
                                  Width="100px">
                                   </ISWebGrid:WebGridColumn>
                   
                  
                          </Columns> 
                          
                          
                                  <ChildTables   >
                       
                            
                                      <ISWebGrid:WebGridTable Caption="Trip Details" DataKeyField="dgKey" DataMember="TripDetails">
                                           
                                          <Columns>
                                              
                                              
                                                    <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No"  AllowSorting="No" Bound="False"
                            ColumnType="CheckBox"  EditType="NoEdit" DataMember="chkBoxShow"
                            IsRowChecker="True" Name="SelectRow" ShowInSelectColumns="No" Width="25px">
                            
                         </ISWebGrid:WebGridColumn>
                         
                     <ISWebGrid:WebGridColumn Caption="dgKey" DataMember="dgKey" DataType="System.Int32"
                        Name="dgKey" Visible="False">
                        
                     </ISWebGrid:WebGridColumn>
                     
                     
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_BoxId %>' DataMember="BoxId" DataType="System.Int32"
                            Name="BoxId" Width="50px">
                         </ISWebGrid:WebGridColumn>
                         
                          
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Vehicle %>' DataMember="Description" DataType="System.String"
                            Name="Description" Width="70px">
                         </ISWebGrid:WebGridColumn>
                         
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_DateTime %>' DataMember="OriginDateTime" DataType="System.DateTime"
                        Name="MyDateTime" Width="100px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Address %>' DataMember="StreetAddress" Name="StreetAddress"
                        Width="200px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Speed %>' DataMember="Speed" DataType="System.Int32"
                        Name="Speed" Width="100px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_MsgTypeName %>' ColumnType="Template" DataMember="BoxMsgInTypeName"
                        Name="BoxMsgInTypeName" Width="100px">
                        <CellTemplate>
                         <asp:HyperLink ID="HyperLink_Site" runat="server" NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "CustomUrl") %>' Text='<%# DataBinder.Eval(Container.DataItem, "BoxMsgInTypeName") %>' meta:resourcekey="HyperLink_SiteResource1" ></asp:HyperLink>
                        </CellTemplate>
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_MsgDetails %>' DataMember="MsgDetails" Name="MsgDetails"
                        Width="100px">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Ack %>' DataMember="Acknowledged" Name="Ack" Width="60px">
                     </ISWebGrid:WebGridColumn>
                        <ISWebGrid:WebGridColumn Caption="Latitude"   DataMember="Latitude" Name="Latitude"
                        Visible="false">
                     </ISWebGrid:WebGridColumn>
                     <ISWebGrid:WebGridColumn Caption="Longitude" DataMember="Longitude" Name="Longitude"
                        Visible="False">
                     </ISWebGrid:WebGridColumn>
                     
                                                                                   
                                          </Columns>
                                         
                                      </ISWebGrid:WebGridTable>
                                  </ChildTables>
                                  
                                 
                       </RootTable>
           
                      
                   
                   
             
             
                   
                      
                   
                   
                     <LayoutSettings  AutoHeight=true  AutoWidth=true AutoColMinWidth=50  Hierarchical="True" AutoFitColumnsBuffering=false AutoFitColumns=true      ShowRefreshButton=False  
                      AllowSorting="Yes"  RowHeightDefault=25px AllowFilter=Yes 
                      PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False" />
                      
                      
                   
          </ISWebGrid:WebGrid>
                               
                     </div>
                </td>
             </tr>
          </table>
      
                                      
          
                                      
       </form>
       
      
   
    
  
</body>
<script type="text/javascript">
    window.onresize();
</script>
    
</html>
