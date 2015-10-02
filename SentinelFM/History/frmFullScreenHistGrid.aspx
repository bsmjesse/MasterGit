<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmFullScreenHistGrid.aspx.cs" Inherits="SentinelFM.frmFullScreenHistGrid" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Data</title>
    <!--<link href="http://www.sentinelfm.com/GlobalStyle.css" type="text/css" rel="stylesheet">-->
     <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    
     <script language="JavaScript">
<!--

         ns = (document.layers)? true:false
			ie = (document.all)? true:false
			

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
				
				
		 function ResizeGrid()
         {
            location.href="?clientWidth="+document.body.clientWidth+"&clientHeight="+document.body.clientHeight; 
         }
	

//-->
    </script>
</head>
<body id="body" runat="server" leftmargin="5px" topmargin="0" rightmargin="0" bottommargin="0" >

       <form id="frmHistory" runat="server" >
         
          <table width="100%" >
             <tr>
                <td align="left" >
                  <table>
                      <tr>
                         <td >
                            <asp:Label ID="lblVisibleRows" runat="server" CssClass="formtext" Text="Visible Rows" Visible="False"></asp:Label></td>
                         <td>
                            <asp:DropDownList ID="cboRows" runat="server" AutoPostBack="True" CssClass="RegularText"
                               meta:resourcekey="cboGridPagingResource1" Visible="False">
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
                               <asp:ListItem Value="999" meta:resourcekey="ListItemResource1">All</asp:ListItem>
                               <asp:ListItem meta:resourcekey="ListItemResource2">15</asp:ListItem>
                               <asp:ListItem Selected="True" meta:resourcekey="ListItemResource3">25</asp:ListItem>
                               <asp:ListItem meta:resourcekey="ListItemResource4">50</asp:ListItem>
                            </asp:DropDownList>&nbsp;
                         </td>
                         <td  >&nbsp;<asp:Button ID="cmdResizeGrid" runat="server" Text="Resize Grid" OnClientClick="javascript:ResizeGrid()"  CssClass="combutton" meta:resourcekey="cmdResizeGridResource1" Visible="False"/> 
                         </td>
                      </tr>
                   </table>
                </td>
             </tr>
          </table>
           <div id=HistoryGrid >
                                 <ISWebGrid:WebGrid ID="dgHistoryDetails"  Height="610px"  Width="860px"   runat="server"   UseDefaultStyle="True" 
              OnInitializeDataSource="dgHistoryDetails_InitializeDataSource"  meta:resourcekey="dgHistoryDetailsResource2" OnInitializeLayout="dgHistoryDetails_InitializeLayout">
               <RootTable>
                  <Columns>
                     <ISWebGrid:WebGridColumn Caption="dgKey" DataMember="dgKey" DataType="System.Int32"
                        Name="dgKey" Visible="False">
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
                     <ISWebGrid:WebGridColumn Caption='<%$ Resources:dgHistoryDetailsTitle_Ack %>' DataMember="Acknowledged" Name="Ack" Width="60px">
                     </ISWebGrid:WebGridColumn>
                  </Columns> 
               </RootTable>
           
      <LayoutSettings AllowColumnMove="Yes" AllowContextMenu="False"    AllowFilter="Yes" RowChangedAction=None 
                      AllowSorting="Yes"    AutoFilterSuggestion="True" AutoFitColumns="True"
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default"  ResetNewRowValuesOnError="False" RowHeaders="Default"  PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False"  RowHeightDefault=25px>
                      <FreezePaneSettings AbsoluteScrolling="True" />
         
                   </LayoutSettings>
                   
          </ISWebGrid:WebGrid>
     
                                      <ISWebGrid:WebGrid ID="dgStops" runat="server"  Height="610px"  Width="860px"
                                         OnInitializeDataSource="dgStops_InitializeDataSource" 
                                         UseDefaultStyle="True"  OnInitializeLayout="dgStops_InitializeLayout" meta:resourcekey="dgStopsResource2" >
                                         <RootTable>
                                            <Columns>
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
                                               
                                            </Columns>
                                         </RootTable>
                                         <LayoutSettings AllowColumnMove="Yes"     AllowContextMenu="False" AllowFilter="Yes"
                      AllowSorting="Yes"    AutoFilterSuggestion="True" AutoFitColumns="True" RowChangedAction=None 
                      ColumnSetHeaders="Default" HideColumnsWhenGrouped="Default" ResetNewRowValuesOnError="False" RowHeaders="Default"  PersistRowChecker="True" RowLostFocusAction="NeverUpdate" AllowExport="Yes" PagingMode="ClassicPaging" DisplayDetailsOnUnhandledError="False" RowHeightDefault=25px>
                      <FreezePaneSettings AbsoluteScrolling="True"  />
                   </LayoutSettings>
                                      </ISWebGrid:WebGrid>
                                       </div>
                                      
       </form>
       
      
   
    
  
</body>
</html>

