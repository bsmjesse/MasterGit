<%@ Page Language="C#" AutoEventWireup="true" Async="true"  CodeFile="frmDashboardAlarms.aspx.cs" Inherits="SentinelFM.Dashboard_frmDashboardAlarms" %>

<%@ Register Assembly="ISNet.WebUI.WebGrid" Namespace="ISNet.WebUI.WebGrid" TagPrefix="ISWebGrid" %>


<%@ Register assembly="WebChart" namespace="WebChart" tagprefix="Web" %>


<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    
    
    
     <script language="javascript">
			<!--
			 setTimeout('location.reload(true)',120000)
			-->
    </script>
    
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
</head>
<body leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
    <form id="form1" runat="server">
    <div  style="Width:100%;height:100%"  >
    
     <fieldset  style="margin: 0px 0px 5px 0px; padding: 0px 0px 0px 0px">
                          <table cellpadding=1 cellspacing=1 >
                            <tr>
                                <td>
                                    <asp:Label CssClass=formtext  ID="lblLastUpdatedCaption" runat="server" Text="Updated:"></asp:Label>
                                    <asp:Label CssClass=formtext ID="lblLastUpdated" runat="server" Text=""></asp:Label>
                                </td>
                                <td>&nbsp;&nbsp;&nbsp;</td>
                                <td>
                                    <asp:LinkButton CssClass=formtext Font-Bold=true    ID="cmdAcceptAlarm" runat="server" 
                                        onclick="cmdAcceptAlarm_Click">Accept</asp:LinkButton>
                                </td>
                                <td>&nbsp;&nbsp;&nbsp;</td>
                                <td>
                                    <asp:LinkButton CssClass=formtext Font-Bold=true  ID="cmdCloseAlarm" runat="server" 
                                        onclick="cmdCloseAlarm_Click">Close</asp:LinkButton>
                                </td>
                               
                                <td>&nbsp;</td>
                               
                                <td>
                                    &nbsp;</td>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:Label ID="lblMessage" CssClass="formtext" ForeColor=Red    runat="server" Text=""  ></asp:Label>
                                      </td>
                          </tr>
                          </table> 
                            
                          
                            </fieldset> 
                            
        <ISWebGrid:WebGrid ID="dgAlarms" runat="server" UseDefaultStyle="True"
             OnInitializeDataSource="dgAlarms_InitializeDataSource" Height="90%"   Width="100%">
            <RootTable DataKeyField="AlarmId">
                <Columns>
                
                 <ISWebGrid:WebGridColumn AllowGrouping="No" AllowSizing="No" AllowSorting="No" Bound="False"
                            Caption="chkBoxShow" ColumnType="CheckBox" DataMember="chkBoxShow" EditType="NoEdit"
                            IsRowChecker="True" Name="SelectRow" ShowInSelectColumns="No" Width="25px">
                         </ISWebGrid:WebGridColumn>
                         
                    <ISWebGrid:WebGridColumn Caption="Time" DataMember="TimeCreated" Name="TimeCreated"
                        Width="100px">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Vehicle" DataMember="vehicleDescription" Name="vehicleDescription"
                        Width="100px">
                    </ISWebGrid:WebGridColumn>
                    <ISWebGrid:WebGridColumn Caption="Alarm" DataMember="AlarmDescription" Name="AlarmDescription"
                        Width="100px">
                    </ISWebGrid:WebGridColumn>
                    
                     <ISWebGrid:WebGridColumn Caption="Status" DataMember="AlarmState" Name="AlarmState"
                        Width="100px">
                    </ISWebGrid:WebGridColumn>
                    
                    
                     <ISWebGrid:WebGridColumn Caption="Severity" DataMember="AlarmLevel" Name="AlarmLevel"
                        Width="100px">
                    </ISWebGrid:WebGridColumn>
                    
                </Columns>
            </RootTable>
            <LayoutSettings AutoHeight=true   AutoFitColumns=true  PersistRowChecker="True"       >
            </LayoutSettings>
        </ISWebGrid:WebGrid>
        
        
       </div>
    </form>
</body>
</html>
