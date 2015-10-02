<%@ Page Language="c#" Inherits="SentinelFM.Messages.frmNewMessage" CodeFile="frmNewMessage.aspx.cs"
    Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>New Message</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->

    <script language="javascript">
		<!--
		var LineLength=0;
		var LineCount=0;
		
		

		function textRemaining()	{
					var nMaxMessageLength = 230
					var oSizeBox = window.document.getElementById("sizebox") ;
					var oMessage = window.document.getElementById("txtMessage") ;
					var oResponseList = window.document.getElementById("lslResponse") ;
					var oResponse = window.document.getElementById("txtResponse") ;
					var BoxValue=230;
					
					
					if (oMessage==null)
						return;
					
					
					var	sMes=oMessage.value;
					
					if ((sMes.substring(oMessage.innerText.length-1,oMessage.innerText.length)=="<") || (sMes.substring(oMessage.innerText.length-1,oMessage.innerText.length)==">") || (sMes.substring(oMessage.innerText.length-1,oMessage.innerText.length)=="~") || (sMes.substring(oMessage.innerText.length-1,oMessage.innerText.length)==";"))
					{
						oMessage.value=sMes.substring(0,oMessage.innerText.length-1)
						return;
					}
			
					
					if (oResponseList!=null)
					{
						BoxValue= nMaxMessageLength - oMessage.innerText.length-oResponseList.innerText.length+oResponseList.length;
					}
					else
					{
						BoxValue= nMaxMessageLength - oMessage.innerText.length;
					}
						
					
					if ( parseInt(BoxValue) < 110 )	
						{	
							//oSizeBox.style.color = "red";
							var	s=oMessage.value;
							oMessage.value=s.substring(0,120)
						}
					else	{	oSizeBox.style.color = "black";	}
					
					if (oMessage.innerText.length!=0)
					{
						oSizeBox.value = nMaxMessageLength - oMessage.innerText.length;	
					}
					else
					{
						oSizeBox.value = nMaxMessageLength;
					}
					
					if (oResponseList.innerText.length!=0)
					{
						oSizeBox.value = oSizeBox.value - (oResponseList.innerText.length)+oResponseList.length;	
					}
					
					
					
				}
				
				
				function RecalculateTextRemaining()	{
					var nMaxMessageLength = 230
					var oSizeBox = window.document.getElementById("sizebox") ;
					var oMessage = window.document.getElementById("txtMessage") ;
					var oResponseList = window.document.getElementById("lslResponse") ;
					var oResponse = window.document.getElementById("txtResponse") ;
					var BoxValue=230;
					var	sMes=oMessage.value;
					
					if (oMessage==null)
						return;
					
					
					if ((sMes.substring(oMessage.innerText.length-1,oMessage.innerText.length)=="<") || (sMes.substring(oMessage.innerText.length-1,oMessage.innerText.length)==">") || (sMes.substring(oMessage.innerText.length-1,oMessage.innerText.length)=="~") || (sMes.substring(oMessage.innerText.length-1,oMessage.innerText.length)==";"))
					{
						oMessage.value=sMes.substring(0,oMessage.innerText.length-1)
						return;
					}
			
					
					if (oResponseList!=null)
					{
						BoxValue= nMaxMessageLength - oMessage.innerText.length-oResponseList.innerText.length+oResponseList.length;
					}
					else
					{
						BoxValue= nMaxMessageLength - oMessage.innerText.length;
					}
						
					
					if ( parseInt(BoxValue) < 112 )	
						{	
							//oSizeBox.style.color = "red";
							var	s=oMessage.value;
							oMessage.value=s.substring(0,120)
						}
					else	
						{	
							oSizeBox.style.color = "black";	
							oSizeBox.value =110;
						}
					
					if (oMessage.innerText.length!=0)
					{
						oSizeBox.value = nMaxMessageLength - oMessage.innerText.length;	
					}
					else
					{
						oSizeBox.value = nMaxMessageLength;
					}
					
					if (oResponseList.innerText.length!=0)
					{
						oSizeBox.value = oSizeBox.value - (oResponseList.innerText.length)+oResponseList.length;	
					}
					
					
					
				}
				
				function ResponseRemaining()	{
					var nMaxMessageLength = 230
					var oSizeBox = window.document.getElementById("sizebox") ;
					var oMessage = window.document.getElementById("txtMessage") ;
					var oResponseList = window.document.getElementById("lslResponse") ;
					var oResponse = window.document.getElementById("txtResponse") ;
					var BoxValue=230;
					
					
										
					var	sMes=oResponse.value;
					if ((sMes.substring(oResponse.value.length-1,oResponse.value.length)=="<") || (sMes.substring(oResponse.value.length-1,oResponse.value.length)==">") || (sMes.substring(oResponse.value.length-1,oResponse.value.length)=="~") || (sMes.substring(oResponse.value.length-1,oResponse.value.length)==";"))
					{
						oResponse.value=sMes.substring(0,oResponse.value.length-1)
						return;
					}
					
					if (oResponseList.innerText.length!=0)
						BoxValue= nMaxMessageLength - oMessage.innerText.length- oResponseList.innerText.length+oResponseList.length-oResponse.value.length;
					 else
						BoxValue= nMaxMessageLength - oMessage.innerText.length;
						
					if ( parseInt(BoxValue) < 0 )	
						{	
							oSizeBox.style.color = "red";
							var	s=oResponse.value;
							oResponse.value=s.substring(0,oResponse.value.length-1)
							return;
						}
					else	{	oSizeBox.style.color = "black";	}
					
					if (oMessage.innerText.length!=0)
						oSizeBox.value = nMaxMessageLength - oMessage.innerText.length;	
					else
						oSizeBox.value = nMaxMessageLength

					
					if (oResponseList.innerText.length!=0)
						oSizeBox.value = oSizeBox.value - (oResponseList.innerText.length)+oResponseList.length;	

					
					if (oResponse.value.length!=0)
						oSizeBox.value = oSizeBox.value - oResponse.value.length;	
					
					if (oResponse.value.length>40) 
					{
						var	s=oResponse.value;
						oResponse.value=s.substring(0,oResponse.value.length-1);
					}
				}
				

		//-->
    </script>

    <style type="text/css">
        .style1
        {
            width: 109px;
        }
        .style2
        {
            width: 90px;
        }
    </style>

</head>
<body onload="textRemaining();">
    <%if (LoadVehiclesBasedOn == "hierarchy")
  {%>
  
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>     

    <script language="javascript">
		<!--
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
        var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
        var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
        var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        
        function onOrganizationHierarchyNodeCodeClick()
        {
            var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
			var myname='OrganizationHierarchy';
			var w=740;
			var h=440;
			var winl = (screen.width - w) / 2; 
			var wint = (screen.height - h) / 2; 
			winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,resizable=1' 
			win = window.open(mypage, myname, winprops) 
			if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }  
            return false;
        }

        function OrganizationHierarchyNodeSelected(nodecode, fleetId)
        {            
            //var myVal = document.getElementById('<%=valMessage.ClientID %>');
            //ValidatorEnable(myVal, false); 

            
            $('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val(nodecode);
            $('#<%=hidOrganizationHierarchyFleetId.ClientID %>').val(fleetId);
            $('#<%=hidOrganizationHierarchyPostBack.ClientID %>').click();
        }
            
			//-->
    </script>
<%} %>
    <form id="frmNewMessageForm" method="post" runat="server">

    <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
    <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

    <%if (LoadVehiclesBasedOn == "hierarchy")
          {%>        
            <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" 
        Text="Button" style="display:none;" AutoPostBack="True" CausesValidation="False"
            OnClick="hidOrganizationHierarchyPostBack_Click" 
        meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />
        <%} %>
    
             <fieldset style="padding: 10px 10px 10px 10px;" >
             <table id="Table2" style="height: 14px;width:100%" cellspacing="0" cellpadding="0"
                        border="0">
                        <tr>
                         
                            <td class="formtext">
                                <asp:Label ID="lblFleetTitle" runat="server" CssClass="formtext" meta:resourcekey="lblFleetTitleResource1"
                                    Text="Fleet:"></asp:Label>
                                <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" Text=" Hierarchy Node:" 
                                           Visible="False" meta:resourcekey="lblOhTitleResource1"  />
                                <asp:RangeValidator ID="valFleet" runat="server" ControlToValidate="cboFleet" ErrorMessage="Please select a Fleet"
                                    MinimumValue="1" MaximumValue="999999999999999" meta:resourcekey="valFleetResource1"
                                    Text="*"></asp:RangeValidator></td>
                            <td>
                                <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText" DataTextField="FleetName"
                                    DataValueField="FleetId" AutoPostBack="True" OnSelectedIndexChanged="cboFleet_SelectedIndexChanged"
                                    meta:resourcekey="cboFleetResource1">
                                </asp:DropDownList>
                                <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" CssClass="combutton" 
                                           Visible="False" OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                           meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                &nbsp;&nbsp;</td>
                            
                            <td>
                                &nbsp;</td>
                            
                            <td>
                                <asp:Label ID="lblVehicleName" runat="server" CssClass="formtext" Visible="False"
                                    meta:resourcekey="lblVehicleNameResource1" Text="Vehicle:"></asp:Label></td>
                            
                            <td>
                                <asp:DropDownList ID="cboVehicle" runat="server" CssClass="RegularText" AutoPostBack="True"
                                    DataValueField="BoxId" DataTextField="Description" Visible="False" DESIGNTIMEDRAGDROP="79"
                                    OnSelectedIndexChanged="cboVehicle_SelectedIndexChanged" meta:resourcekey="cboVehicleResource1">
                                </asp:DropDownList></td>
                            
                        </tr>
                       
                    </table>
              </fieldset>       <br />
      <asp:MultiView ID="MultiviewMessages" runat="server" ActiveViewIndex="0">
                        <asp:View ID="TextMessages" runat="server">
                           
        <fieldset style="padding: 10px 10px 10px 10px;" >
          <table id="tblWait" style="z-index: 101; left: 220px; width:100%;   top: 128px"
            runat="server">
            <tr>
                <td class="RegularText" style="height: 15px" align="center">
                    <asp:Label ID="lblPleaseWaitMsg" runat="server" CssClass="RegularText" meta:resourcekey="lblPleaseWaitMsgResource1"
                        Text="Please wait..."></asp:Label></td>
            </tr>
            <tr>
                <td class="formtext" style="height: 14px" align="center">
                    <asp:Label ID="lblSendingMessageMsg" runat="server" CssClass="formtext" meta:resourcekey="lblSendingMessageMsgResource1"
                        Text="Sending Message..."></asp:Label></td>
            </tr>
            <tr>
                <td style="height: 22px" align="center">
                    <asp:Image ID="imgWait" runat="server" Width="105px" Height="5px" ImageUrl="../images/prgBar.gif"
                        meta:resourcekey="imgWaitResource1"></asp:Image></td>
            </tr>
            <tr>
                <td style="height: 22px" align="center">
                    <asp:Button ID="cmdCancelSend" runat="server" Width="97px" CssClass="combutton" Text="Cancel"
                        OnClick="cmdCancelSend_Click" meta:resourcekey="cmdCancelSendResource1"></asp:Button></td>
            </tr>
        </table>
        
        
                             <table id="tblMain" style="width: 500px;height: 440px" cellspacing="0" cellpadding="0" border="0" runat="server">
            <tr>
                <td class="tableheading" style="height: 11px">
                </td>
                <td style="height: 11px">
                </td>
            </tr>
         
            <tr>
                <td class="tableheading" style="height: 54px; vertical-align: top;">
                    &nbsp;<asp:Label ID="lblMessageTextTitle" runat="server" CssClass="tableheading"
                        meta:resourcekey="lblMessageTextTitleResource1" Text="Message Text :"></asp:Label>
                    :<asp:RequiredFieldValidator ID="valMessage" runat="server" 
                        ControlToValidate="txtMessage" ErrorMessage="Please enter a message:" 
                        meta:resourcekey="valMessageResource1" Text="*"></asp:RequiredFieldValidator>
                </td>
                <td style="height: 54px; vertical-align: top;">
                    <asp:TextBox onkeypress="textRemaining();" ID="txtMessage" onkeyup="RecalculateTextRemaining(this);"
                        runat="server" Width="269px" Height="65px" CssClass="formtext" Font-Size="10pt"
                        Rows="5" Font-Names="monospace" MaxLength="180" TextMode="MultiLine" meta:resourcekey="txtMessageResource1"></asp:TextBox>
                    <table id="Table1" style="height: 11px" cellspacing="0" cellpadding="0"
                        border="0">
                        <tr>
                            <td style="height: 17px">
                                <asp:Label ID="Label1" runat="server" CssClass="formtext" meta:resourcekey="Label1Resource1"
                                    Text="* Message text and response cannot exceed 230 characters."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label3" runat="server" Height="2px" CssClass="formtext" meta:resourcekey="Label3Resource1"
                                    Text="* Maximum 4 lines allowed."></asp:Label></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="tableheading" style="height: 10px">
                    &nbsp;</td>
                <td class="formtext" style="height: 10px">
                </td>
            </tr>
            <tr>
                <td class="tableheading" height="15">
                    &nbsp;</td>
                <td height="15">
                    <asp:TextBox onkeypress="ResponseRemaining();" ID="txtResponse" onkeydown="ResponseRemaining();"
                        onkeyup="ResponseRemaining();" onfocus="ResponseRemaining();" runat="server"
                        Width="238px" CssClass="formtext" MaxLength="40" meta:resourcekey="txtResponseResource1"></asp:TextBox><br /><asp:Button
                            ID="cmdAddResponse" runat="server" Width="178px" CssClass="combutton" Text="Add to Response List"
                            CausesValidation="False" OnClick="cmdAddResponse_Click" meta:resourcekey="cmdAddResponseResource1">
                        </asp:Button></td>
            </tr>
            <tr>
                <td class="formtext" style="height: 19px" height="19">
                    </td>
                <td style="height: 19px; padding-bottom: 20px;" height="19">
                    <asp:Label ID="Label2" runat="server" CssClass="formtext" meta:resourcekey="Label2Resource1"
                        Text="* Each response cannot exceed 40 characters."></asp:Label></td>
            </tr>
            <tr>
                <td class="tableheading" height="15" style="vertical-align: top;">
                    &nbsp;<asp:Label ID="lblResponseListTitle" runat="server" CssClass="tableheading"
                        meta:resourcekey="lblResponseListTitleResource1" Text="Response List :"></asp:Label></td>
                <td height="15" style="vertical-align: top;">
                    <asp:ListBox ID="lslResponse" runat="server" Width="238px" CssClass="formtext" meta:resourcekey="lslResponseResource1">
                    </asp:ListBox><br /><asp:Button ID="cmdRemove" runat="server" Width="178px"
                        CssClass="combutton" Text="Remove from Response List" CausesValidation="False"
                        OnClick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:Button></td>
            </tr>
            <tr>
                <td class="tableheading" height="10">
                </td>
                <td height="15">
                </td>
            </tr>
            <tr>
                <td class="tableheading" height="10">
                    <asp:Label ID="lblCharactersLeftTitle" runat="server" CssClass="formtext" 
                        meta:resourcekey="lblCharactersLeftTitleResource1" Text="Characters Left:"></asp:Label>
                </td>
                <td class="formtext" height="15">
                    &nbsp;&nbsp;&nbsp;<input ID="sizebox" maxlength="2" name="sizebox" 
                        onfocus="this.blur()" readonly size="3" 
                        style="font-size: large; width: 53px; color: black" value="230"></input></td>
            </tr>
            <tr>
                <td class="formtext">
                </td>
                <td align="left">
                    <asp:Label ID="lblCommandStatus" runat="server" CssClass="regulartext" meta:resourcekey="lblCommandStatusResource1"></asp:Label><asp:Label
                        ID="lblMessage" runat="server" Height="8px" CssClass="errortext"
                        Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label><asp:ValidationSummary
                            ID="ValidationSummary1" runat="server" CssClass="errortext" meta:resourcekey="ValidationSummary1Resource1">
                        </asp:ValidationSummary>
                   
                    <table width="100%">
                        <tr>
                            <td style="width: 100%;" align="left">
                                <asp:RadioButtonList ID="optCommMode" runat="server" AutoPostBack="True" CssClass="formtext"
                                    OnSelectedIndexChanged="optCommMode_SelectedIndexChanged" Width="100%" RepeatDirection="Horizontal"
                                    meta:resourcekey="optCommModeResource1">
                                    <asp:ListItem Value="1" Selected="True" meta:resourcekey="ListItemResource1" Text="Primary Mode&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;"></asp:ListItem>
                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource2" Text="Dual Communication Mode"></asp:ListItem>
                                </asp:RadioButtonList>
                                <table class="formtext" id="tblSchedule" runat="server">
                                    <tr>
                                        <td style="height: 21px">
                                            <asp:Label ID="lblKeepTry" runat="server" Text="Keep trying for:" meta:resourcekey="lblKeepTryResource1"></asp:Label></td>
                                        <td style="height: 21px">
                                        </td>
                                        <td style="height: 21px">
                                            <asp:DropDownList ID="cboSchPeriod" runat="server" CssClass="RegularText"
                                                AutoPostBack="True" OnSelectedIndexChanged="cboSchPeriod_SelectedIndexChanged"
                                                meta:resourcekey="cboSchPeriodResource1">
                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource3" Text="No Retry"></asp:ListItem>
                                                <asp:ListItem Value="900" meta:resourcekey="ListItemResource4" Text="15 Min"></asp:ListItem>
                                                <asp:ListItem Value="1800" meta:resourcekey="ListItemResource5" Text="30 Min"></asp:ListItem>
                                                <asp:ListItem Value="3600" meta:resourcekey="ListItemResource6" Text="1 Hour"></asp:ListItem>
                                                <asp:ListItem Value="7200" meta:resourcekey="ListItemResource7" Text="2 Hours"></asp:ListItem>
                                                <asp:ListItem Value="10800">3 Hours</asp:ListItem>
                                                <asp:ListItem Value="21600">6 Hours</asp:ListItem>
                                                <asp:ListItem Value="43200">12 Hours</asp:ListItem>
                                                <asp:ListItem Value="86400">24 Hours</asp:ListItem>
                                                <asp:ListItem Value="172800">48 Hours</asp:ListItem>
                                                <asp:ListItem Value="259200">72 Hours</asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td style="height: 21px">
                                            <asp:Label ID="lblRetryEveryTitle" runat="server" meta:resourcekey="lblRetryEveryTitleResource1"
                                                Text="Retry every:"></asp:Label></td>
                                        <td style="height: 21px">
                                        </td>
                                        <td style="height: 21px">
                                            <asp:DropDownList ID="cboSchInterval" runat="server" CssClass="RegularText"
                                                AutoPostBack="True" OnSelectedIndexChanged="cboSchInterval_SelectedIndexChanged"
                                                meta:resourcekey="cboSchIntervalResource1">
                                                <asp:ListItem Value="0" meta:resourcekey="ListItemResource8" Text="No Retry"></asp:ListItem>
                                                <asp:ListItem Value="60" meta:resourcekey="ListItemResource9" Text="1 min"></asp:ListItem>
                                                <asp:ListItem Value="300" meta:resourcekey="ListItemResource10" Text="5 min"></asp:ListItem>
                                                <asp:ListItem Value="600" meta:resourcekey="ListItemResource11" Text="10 min"></asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="height: 10px"></td>
                <td style="height: 10px"></td>
            </tr>
            <tr>
                <td style="height: 19px;">
                </td>
                <td style="height: 19px; text-align: center;"  colspan="2">
              
                    <asp:Button ID="cmdSend" runat="server" Width="158px" CssClass="combutton"
                        Text="Send Message" OnClick="cmdSend_Click" meta:resourcekey="cmdSendResource1">
                    </asp:Button>
                    &nbsp;&nbsp;&nbsp;
                    <%--<input class="combutton" id="cmdClose" style="width: 158px; height: 19px" onclick="top.close()"
                        type="button" value="Close" name="cmdCancel">--%>
                    <asp:Button runat="server" CssClass="combutton" ID="cmdClose" Style="width: 158px;
                        height: 19px" OnClientClick="top.close()" Text="Close" meta:resourcekey="cmdCloseResource1" /></td>
            </tr>
            <tr>
                <td height="2">
                    &nbsp;
                </td>
                <td>
                </td>
            </tr>
        </table>
        </fieldset> 
                        </asp:View> 
                        
                         <asp:View ID="GarminMessages" runat="server">
                          <fieldset style="padding: 10px 10px 10px 10px;" >
                         <table id="Table3" style="width: 500px;height: 440px" cellspacing="0" cellpadding="0" border="0" runat="server">
            <tr>
                <td class="tableheading" style="height: 11px" valign=top  >
                    <table >
                        <tr>
                            <td class="style2">
                                &nbsp;</td>
                            <td>
                                <asp:DropDownList ID="cboMessageType" runat="server" CssClass="formtext" 
                                    Visible="False">
                                    <asp:ListItem Value="42">Text Message</asp:ListItem>
                                    <asp:ListItem></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" class="style2">
                                <asp:Label ID="lblMessageBody" runat="server" CssClass="formtext"  meta:resourcekey="lblMessageTextTitleResource1"
                                    Text="Message Text :"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtMessageGarmin" runat="server" CssClass="formtext" 
                                    Height="63px" TextMode="MultiLine" Width="269px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="style2">
                                &nbsp;</td>
                            <td align="left">
                                
                                <table ID="Table4" runat="server" class="formtext">
                                    <tr>
                                        <td style="height: 21px">
                                            <asp:Label ID="Label4" runat="server" meta:resourcekey="lblKeepTryResource1" 
                                                Text="Keep trying for:"></asp:Label>
                                        </td>
                                        <td style="height: 21px">
                                        </td>
                                        <td style="height: 21px">
                                            <asp:DropDownList ID="cboSchPeriodGarmin" runat="server" AutoPostBack="True" 
                                                CssClass="RegularText" meta:resourcekey="cboSchPeriodResource1" 
                                                OnSelectedIndexChanged="cboSchPeriod_SelectedIndexChanged">
                                                <asp:ListItem  Text="No Retry" Value="3600"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource4" Text="15 Min" Value="900"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource5" Text="30 Min" Value="1800"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource6" Text="1 Hour" Value="3600"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource7" Text="2 Hours" Value="7200"></asp:ListItem>
                                                <asp:ListItem Value="10800">3 Hours</asp:ListItem>
                                                <asp:ListItem Value="21600">6 Hours</asp:ListItem>
                                                <asp:ListItem Value="43200">12 Hours</asp:ListItem>
                                                <asp:ListItem Value="86400">24 Hours</asp:ListItem>
                                                <asp:ListItem Value="172800">48 Hours</asp:ListItem>
                                                <asp:ListItem Value="259200">72 Hours</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="height: 21px">
                                            <asp:Label ID="Label5" runat="server" 
                                                meta:resourcekey="lblRetryEveryTitleResource1" Text="Retry every:"></asp:Label>
                                        </td>
                                        <td style="height: 21px">
                                        </td>
                                        <td style="height: 21px">
                                            <asp:DropDownList ID="cboSchIntervalGarmin" runat="server" AutoPostBack="True" 
                                                CssClass="RegularText" Enabled="False" 
                                                meta:resourcekey="cboSchIntervalResource1" 
                                                OnSelectedIndexChanged="cboSchInterval_SelectedIndexChanged">
                                                <asp:ListItem meta:resourcekey="ListItemResource8" Text="No Retry" Value="0"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource9" Text="1 min" Value="60"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource10" Selected="True" 
                                                    Text="5 min" Value="300"></asp:ListItem>
                                                <asp:ListItem meta:resourcekey="ListItemResource11" Text="10 min" Value="600"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                              <td  colspan="2" style="text-align: center">
                                <asp:Button ID="cmdSendGarminMsg" runat="server" CssClass="combutton" 
                                    meta:resourcekey="cmdSendResource1" onclick="cmdSendGarminMsg_Click" 
                                    Text="Send Message" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="cmdCancel" runat="server" CssClass="combutton" 
                                    meta:resourcekey="cmdCloseResource1" OnClientClick="top.close()" Text="Close" />
                            </td>
                        </tr>
                        <tr>
                            <td class="style2">
                                &nbsp;</td>
                            <td align="left">
                            
                            
                               <table width="100%">
                        <tr>
                            <td style="width: 100%;" align="left">
                                
                                &nbsp;</td>
                        </tr>
                    </table>
                    
                                </td>
                        </tr>
                        <tr>
                            <td class="style2">
                                &nbsp;</td>
                            <td>
                                <asp:Label ID="lblGarminMessage" runat="server" CssClass="formtext" 
                                    ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                </tr> 
                </table>  
                            </fieldset> 
                         </asp:View> 
       </asp:MultiView> 
    
       
    </form>
</body>
</html>
