<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmNewDriverMsg.aspx.cs" Inherits="SentinelFM.Messages_frmNewDriverMsg" meta:resourcekey="PageResource1" %>



<%@ Register assembly="ISNet.WebUI.WebCombo" namespace="ISNet.WebUI.WebCombo" tagprefix="ISWebCombo" %>



<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>New Driver Text Message</title>
    
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
    
</head>
<body>
    <form id="form1" runat="server">
  
    <table class="formtext" width="100%">
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
            <asp:Label ID="lblTo" runat=server CssClass="formtext" meta:resourcekey="lblToResource1" >To:</asp:Label> 
                
                
                
             
              
                
                                                            
                                                        
                                                        
                
                        </td>
                        <td width="100%" >
                
                
                
             
              
                
                                                            
                                                        
                                                        
                
                   <ISWebCombo:WebCombo ID="cboDrivers" runat="server"   UseDefaultStyle="True" 
                    DataTextField="FullNameAndEmail" DataValueField="DriverId" 
                    OnInitializeDataSource="cboDrivers_InitializeDataSource" Height="20px"  
                    Width="100%" DataMember="" meta:resourcekey="cboDriversResource1">
                    
                    
                <MultipleSelectionSettings Enabled="True" />
                </ISWebCombo:WebCombo>
                
                
             
              
                
                                                            
                                                        
                                                        
                
                
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtSubject" runat="server" Height="300px" TextMode="MultiLine" 
                    Width="100%" meta:resourcekey="txtSubjectResource1"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align=right >
                <asp:Button ID="cmdCancel" runat="server" Text="Cancel" CssClass="combutton" 
                    OnClientClick="window.close();" meta:resourcekey="cmdCancelResource1"  />
&nbsp; &nbsp;<asp:Button ID="cmdSend" runat="server" Text="Send" onclick="cmdSend_Click" 
                    CssClass="combutton" meta:resourcekey="cmdSendResource1" />
                &nbsp;
              
               
            </td>
        </tr>
        <tr>
            <td align=center >
                <asp:Label ID="lblMessage" runat="server" CssClass="formtext" 
                    meta:resourcekey="lblMessageResource1"></asp:Label>
              
               
            </td>
        </tr>
    </table>
  
    </form>
    </body>
</html>
