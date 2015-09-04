<%@ Page Language="C#" AutoEventWireup="true" CodeFile="landmarkEditForm.aspx.cs" Inherits="SentinelFM.MapNew_landmarkEditForm" meta:resourcekey="PageResource1" %>
<% if (Show_Header)
   {
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Landmark Edit Form</title>
    <link rel="stylesheet" href="../maps/style.css" type="text/css">
    
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script> 
    <script type="text/javascript" src="../scripts/NewMap/landmarkEditForm.js"></script>
    
</head>
<body>
<% } %>
    <div id="polygoninfo">
        <h6 style="margin-left:10px;"><asp:Label ID="lblLandmark" Text="Landmark:" 
                runat="server" meta:resourcekey="lblLandmarkResource1"></asp:Label> 
            <asp:Label ID="LandmarkNameLabel" runat="server" meta:resourcekey="LandmarkNameLabelResource1"></asp:Label>
        </h6>
        <div style="margin:10px 0 0 20px;">
            <a href="javascript:void(0)" onclick="showeditpolygonform();"><asp:Label ID="lblEdit" runat="server" Text="Edit" meta:resourcekey="lblEditResource1"></asp:Label></a>
            <a href="javascript:void(0)" onclick="redrawLandmark(<%=landmarkId.ToString() %>, polygonPopup, '<%=geoassetname.ClientID %>');"><asp:Label ID="lblRedraw" runat="server" Text="Redraw" meta:resourcekey="lblRedrawResource1"></asp:Label></a>
        </div>
    </div>

    <div id="polygoneditform" style="display:none;">
        <h6 style="margin-left:10px;border-bottom:1px solid #cccccc">
            <asp:Label ID="FormTitle" runat="server" meta:resourcekey="FormTitleResource1"></asp:Label>
        </h6>
        <div style='font-size:1em'>
    
        <form id="form1" runat="server" enableviewstate="False">
            <input type='hidden' id='lstAddOptions' name='lstAddOptions' value='0' />
            <input type='hidden' id='oid' name='oid' value='' />
            <input type='hidden' id='isNew' name='isNew' value='0' />
            <input type='hidden' id='pointSets' name='pointSets' value='' />
    
            <input type='hidden' id='txtX' name='txtX' value='' runat="server" />
            <input type='hidden' id='txtY' name='txtY' value='' runat="server" />
            <input type='hidden' id='oldLandmarkName' name='oldLandmarkName' value='' runat="server" />
            <input type='hidden' id='geoassettype' name='geoassettype' value='' runat="server" />
            <input type='hidden' id='geoassetname' name='geoassetname' value='' runat="server" />
            <input type='hidden' id='metaOnly' name='metaOnly' value='1' runat="server" />
            <input type="hidden" id="orginalAppliedServices" name="orginalAppliedServices" value="" runat="server" />
            <input type="hidden" id="updatedAppliedServices" name="updatedAppliedServices" value="" runat="server" />
            <input type="hidden" id="newAppliedServices" name="newAppliedServices" value="" runat="server" />
            <input type="hidden" id="serviceEmailRecepients" name="serviceEmailRecepients" value="" runat="server" />
            <input type="hidden" id="serviceEmailSubject" name="serviceEmailSubject" value="" runat="server" />
            <input type="hidden" id="helptip" name="helptip" value="" runat="server" />
            
            <table class="landmarkfield editformmainoptions" cellspacing="0" cellpadding="0" border="0" style="margin-left:10px;">
                <tr><td style="padding-top:10px;">
                    <table border="0">
                        <tr>
                            <td class="formtext" style="height: 25px; width: 139px;">
                                <asp:Label ID="lblLandmarkNameTitle" Text="Landmark Name (*):" class="formtext" 
                                    runat="server" meta:resourcekey="lblLandmarkNameTitleResource1"></asp:Label>
                                <span id="valLandmark" style="color:Red;visibility:hidden;">*</span></td>
                            <td class="formtext" style="height: 25px">
                                <input name="txtLandmarkName" type="text" id="txtLandmarkName" class="formtext bsmforminput" style="width:173px;" runat="server" value="" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext" style="height: 25px">
                                <asp:Label ID="lblContactNameTitle" Text="Contact Name:" class="formtext" 
                                    runat="server" meta:resourcekey="lblContactNameTitleResource1"></asp:Label>
                            </td>
                            <td class="formtext" style="height: 25px">
                                <input name="txtContactName" type="text" id="txtContactName" class="formtext bsmforminput" style="width:173px;" runat="server" value="" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext" style="height: 25px">
                                <asp:Label ID="lblLandmarkDescriptionTitle" Text="Landmark Description:" 
                                    class="formtext" runat="server" 
                                    meta:resourcekey="lblLandmarkDescriptionTitleResource1"></asp:Label>
                            </td>
                            <td class="formtext">
                                <input name="txtLandmarkDesc" type="text" id="txtLandmarkDesc" class="formtext bsmforminput" style="width:173px;" runat="server" value="" /></td>
                        </tr>
                        <tr>
                            <td class="formtext" style="height: 25px">
                                <asp:Label ID="lblPhoneTitle" Text="Phone :" class="formtext" runat="server" 
                                    meta:resourcekey="lblPhoneTitleResource1"></asp:Label>
                            </td>
                            <td class="formtext">
                                <input name="txtPhone" type="text" id="txtPhone" class="formtext bsmforminput" style="width:173px;" runat="server" value="" />
                            </td>
                        </tr>
                        
                        <tr id="radiusRow" visible="false" runat="server">
                            <td class="formtext" height="25" style="height: 15px;">
                                <asp:Label ID="lblRadiusTitle" Text="Radius" class="formtext" runat="server" 
                                    meta:resourcekey="lblRadiusTitleResource1"></asp:Label>
                                <asp:Label ID="lblUnit" Text="(m):" class="formtext" runat="server" 
                                    meta:resourcekey="lblUnitResource1"></asp:Label>
                                <span id="valRadius" style="color:Red;visibility:hidden;">*</span><span id="valRangeRadius" style="color:Red;visibility:hidden;">*</span>
                                </td>
                            <td class="formtext" height="25">
                                <input name="txtRadius" type="text" id="txtRadius" class="formtext bsmforminput" style="width:173px;" runat="server" value="" />
                            </td>
                        </tr>

                        <tr>
                            <td class="formtext" style="height:40px;">
                                <asp:Label ID="lblCategoryTitle" runat="server" CssClass="formtext" 
                                            meta:resourcekey="lblCategoryTitleResource1" Text="Category"></asp:Label>
                            </td>
                            <td class="formtext" style="height:40px;">
                                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="formtext" Width="173px">
                                    </asp:DropDownList>
                            </td>
                        </tr>

                        <%if (ShowCallTimer)
                          { %>
                        <tr id="trCallTimer" visible = "false">
                            <td style="height:40px;">
                                <asp:Label ID="Label2" runat="server" Text="Timeout:" CssClass="formtext" 
                                    meta:resourcekey="Label2Resource1"></asp:Label>
                            </td>
                            <td style="height:40px;">
                                <asp:DropDownList ID="cboServices" runat="server" CssClass="RegularText"
                                        DataTextField="RulesApplied" DataValueField="ServiceConfigId" 
                                    meta:resourcekey="cboFleetResource6">                                                                                                                                                                                                       
                                    </asp:DropDownList>
                            </td>
                        </tr>
                        <%} %>
        
                        <tr>
                            <td colspan="2" class="formtext" height="25" style="height: 25px;">
                                <asp:RadioButtonList ID="lstPublicPrivate" runat="server" CssClass="formtext" meta:resourcekey="lstPublicPrivateResource1" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0" meta:resourcekey="ListItemResource35" Text="Private"></asp:ListItem>
                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource36" Text="Public"></asp:ListItem>                                                                
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext" height="15" colspan="2"></td>                            
                        </tr>
                        <tr>
                            <td colspan="2" class="formtext" height="15">
                            <a href='javascript:void(0)' onclick='editformmoreoptions();'>
                                <asp:Label ID="lblMoreOptions" runat="server" Text="More Options" 
                                    meta:resourcekey="lblMoreOptionsResource1"></asp:Label></a> &nbsp;
                            <a href='javascript:void(0)' onclick='editformextendedattributes();' runat="server" id="lnkExtendedAttributes">
                                <asp:Label ID="lblExtended" runat="server" Text="Extended." 
                                    meta:resourcekey="lblExtendedResource1"></asp:Label></a>
                            <%if (ShowAssignToFleet)
                              { %>
                            <a href='javascript:void(0)' onclick='editformassigntofleet(<%=landmarkId %>);' id="lnkAssignToFleet">
                                <asp:Label ID="lblAssignToFleet" runat="server" Text="Assign to fleet" 
                                    meta:resourcekey="lblAssignToFleetResource1"></asp:Label></a>
                               
                            <%} %>
                            </td>
                        </tr>
                        <tr>
                            <td class="formtext" height="15" colspan='2'></td>                            
                        </tr>
                    </table>
                </td></tr>
            </table>

            <table class="editformmoreoptions" id="Table4" style="display:none;" cellspacing="0" cellpadding="0" >
                <tr>
                    <td valign="top" style="padding:10px;vertical-align: top;">
                        <table border="0">
                            <tr>
                                <td class="formtext" style="height: 30px">
                                    <asp:Label ID="lblEmailTitle" Text="Email:" class="formtext" runat="server" 
                                        meta:resourcekey="lblEmailTitleResource1"></asp:Label>
                                </td>
                                <td style="height: 30px">
                                    <input name="txtEmail" type="text" id="txtEmail" class="formtext bsmforminput" value="" style="width:173px;" runat="server" />
                                </td>
                            </tr>
                            <tr style="display:none;">
                                <td class="style4" style="height: 30px">
                                    <asp:Label ID="lblPhone" Text="Phone:" class="formtext" runat="server" 
                                        meta:resourcekey="lblPhoneResource1"></asp:Label>
                                    <span id="RegularExpressionValidator1" class="formtext" style="color:Red;visibility:hidden;">*</span>
                                </td>
                                <td style="height: 30px">
                                    <input name="txtPhoneSMS" type="text" id="txtPhoneSMS" disabled="disabled" class="formtext bsmforminput" value="" style="width:173px;" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formtext" style="height: 30px">
                                    <asp:Label ID="lblTimeZoneTitle" Text="Time Zone:" class="formtext" 
                                        runat="server" meta:resourcekey="lblTimeZoneTitleResource1"></asp:Label>
                                </td>
                                <td style="height: 30px">                                    
                                    <asp:DropDownList ID="cboTimeZone" runat="server" CssClass="RegularText" Width="168px"
                                        DataTextField="TimeZoneName" DataValueField="TimeZoneId" meta:resourcekey="cboTimeZoneResource1">
                                    </asp:DropDownList>
                                </td>
                                <td class="style4" style="height: 30px">&nbsp;</td>
                                <td style="height: 30px">&nbsp;</td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height: 30px">
                                    <asp:Label ID="lblMultipleEmails" 
                                        Text="*Multiple email addresses Must be Separated by semicolon or comma" 
                                        class="formtext" runat="server" meta:resourcekey="lblMultipleEmailsResource1"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 30px" colspan="2">
                                    <span class="formtext"><input id="chkDayLight" type="checkbox" name="chkDayLight" runat="server" /><label for="chkDayLight"><asp:Label
                                            ID="lblAAFDDLST" runat="server" 
                                        Text="Automatically adjust for daylight savings time" 
                                        meta:resourcekey="lblAAFDDLSTResource1"></asp:Label></label></span>    
                                </td>
                            </tr>
                            <tr>
                                <td height="5" class="style4" colspan="2"></td>                                
                            </tr>
                            <tr>
                                <td colspan="2" class="formtext" height="15">
                                    <a href='javascript:void(0)' onclick='editformmainoptions();'>
                                        <asp:Label ID="lblBack" runat="server" Text="Back" 
                                        meta:resourcekey="lblBackResource1"></asp:Label></a>
                                </td>
                            </tr>
                            <tr>
                                <td height="5" class="style4">
                                </td>
                                <td height="5">
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>

            <table id="tblextendedattributes" class="editformextendedattributes" style="display:none;" cellspacing="0" cellpadding="0" runat="server" >
                <tr>
                    <td valign="top" style="padding-top:5px;vertical-align: top;">
                        <table border="0" id="applyservices">                            
                            <tr>
                                <td valign="top">
                                    <div id="serviceleftpane">
                                        All services: <a href='javascript:void(0)' onclick='$("#SerivceRule").html("<%=HELPTIP %>");'>?</a>
                                        <div id="AllServicesList" class="allservices serviceslist">
                                            <%=ALL_SERVICES %>
                                        </div>
                                    </div>
                                </td>
                                <td valign="top">
                                    <div id="servicerightpane">
                                        Applied Services: <a href='javascript:void(0)' onclick='$("#SerivceRule").html("<%=HELPTIP %>");'>?</a>
                                        <div id="AppliedServiceList" class="appliedservices serviceslist">
                                            <ul>
                                                <%=APPLIED_SERVICES %>                                                
                                            </ul>
                                        </div>
                                    
                                        <div id="SerivceRule">
                                            <%=HELPTIP %>
                                        </div>
                                    </div>                                    
                                </td>
                            </tr>                            
                            <tr>
                                <td colspan="2" style="height: 28px" valign="top">
                                    <asp:Label ID="lblSubjectTitle" runat="server" Text="Subject: " 
                                        meta:resourcekey="lblSubjectTitleResource1"></asp:Label>
                                    <!--<asp:TextBox ID="lblSubject" runat="server" ReadOnly="true" Enabled="false" Text="" Style="width:270px;border:1px solid #cccccc;overflow:hidden;background-color:White;color:Black;"></asp:TextBox>-->
                                    <div id="lblSubject" onclick="showSubjectTemplate(this);" style="width:270px;border:1px solid #cccccc;overflow:hidden;background-color:White;color:Black;display:inline-block;height:18px;white-space:nowrap;"></div>
                                    
                                    <a href="javascript:void(0)" onclick="showSubjectTemplate(this);">...</a>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="height:28px;">
                                    <asp:Label ID="lblRecepientsTitle" runat="server" Text="Recepients: " 
                                        meta:resourcekey="lblRecepientsTitleResource1"></asp:Label>
                                    <!--<asp:TextBox ID="lblRecepients" runat="server" ReadOnly="true" Enabled="false" Text="" Style="width:250px;border:1px solid #cccccc;overflow:hidden;background-color:White;color:Black;"></asp:TextBox>-->
                                    <div id="lblRecepients" onclick="showRecepientsTemplate(this);" style="width:250px;height:18px;display:inline-block;border:1px solid #cccccc;overflow:hidden;background-color:White;color:Black;"></div>
                                    <a href="javascript:void(0)" onclick="showRecepientsTemplate(this);">...</a>
                                    
                                    <!--<asp:TextBox ID="txtServiceRecepients" runat="server" class="formtext bsmforminput formtextprompt" style="width:177px"
                                        onfocus="focustextbox(this);" onblur="blurtextbox(this);" Text="Recepients" Title="Recepients" PrompText="Recepients"></asp:TextBox>-->

                                    
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <a href='javascript:void(0)' onclick='applyServices();'>
                                        <asp:Label ID="lblApply" runat="server" Text="Apply" 
                                        meta:resourcekey="lblApplyResource1"></asp:Label></a>
                                    <a href='javascript:void(0)' onclick='cancelServices();'>
                                        <asp:Label ID="lblCancel" runat="server" Text="Cancel" 
                                        meta:resourcekey="lblCancelResource1"></asp:Label></a>
                                    <a href='javascript:void(0)' onclick='createNewServices();'>
                                        <asp:Label ID="lblNewService" runat="server" Text="New Service" 
                                        meta:resourcekey="lblNewServiceResource1"></asp:Label></a>
                                </td>
                            </tr>
                        </table>
                        <table id="createNewServices" style="display:none;">
                            <tr>
                                <td colspan="4" style="height: 28px">
                                    <asp:TextBox ID="txtServicename" runat="server" 
                                        class="formtext bsmforminput formtextprompt" onfocus="focustextbox(this);" 
                                        onblur="blurtextbox(this);" Text="Service name (*)" Title="Service name (*)" 
                                        PrompText="Service name (*)" meta:resourcekey="txtServicenameResource1"></asp:TextBox>                                    
                                </td>                               
                            </tr>
                            <tr>                    
                                <td class="formtext" style="height: 28px;width:118px;">
                                    <asp:DropDownList ID="cboRuleName" runat="server" DataTextField="RuleName" 
                                        DataValueField="RuleName" CssClass="RegularText" Width="108px" 
                                        meta:resourcekey="cboRuleNameResource1">                                        
                                    </asp:DropDownList>
                                </td>
                                <td style="height: 30px;width:68px;">                                    
                                    <asp:DropDownList ID="ruleOperator" runat="server" CssClass="RegularText" Width="58px"
                                        meta:resourcekey="cboSpeedOperation">
                                        <asp:ListItem Text="Over" Value="&gt;=" meta:resourcekey="ListItemResource2"></asp:ListItem>
                                        <asp:ListItem Text="Less" Value="&lt;=" meta:resourcekey="ListItemResource4"></asp:ListItem>
                                        <asp:ListItem Text="Equal" Value="=" meta:resourcekey="ListItemResource5"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="ruleValue" runat="server" Width="77px" class="formtext bsmforminput" Title="Value of the rule"
                                        meta:resourcekey="polygonspeedValueResource1"></asp:TextBox>
                                </td>
                                <td>
                                    <input type='button' class='kd-button' onclick='addNewServices();' value='Add' style='margin-left:2px;' />
                                </td>                                
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <div id="ruleslist"></div>
                                </td>
                            </tr> 
                            <tr>
                                <td colspan="4">
                                    <div id="createrulemessage">
                                    </div>
                                </td>
                            </tr>                         
                            <tr>
                                <td colspan="4" class="formtext" height="15">
                                    <a href='javascript:void(0)' onclick='applyCreateNewServices()' style='margin:0 10px;'>
                                        <asp:Label ID="lblOK" runat="server" Text="OK" 
                                        meta:resourcekey="lblOKResource1"></asp:Label></a>
                                    <a href='javascript:void(0)' onclick='cancelCreateNewServices();'>
                                        <asp:Label ID="lblCancel1" runat="server" Text="Cancel" 
                                        meta:resourcekey="lblCancel1Resource1"></asp:Label></a>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" height="5" class="style4"></td>                                
                            </tr>
                        </table>
                        
                    </td>
                </tr>
            </table>

            <table id="tblAssignToFleet" class="editformassigntofleet" style="display:none;" cellspacing="0" cellpadding="0" runat="server" >
                <tr>
                    <td>
                        <div style="height:190px;overflow:auto;width:320">
                            <asp:CheckBoxList ID="CheckBoxFleet" runat="server" Height="170px" 
                                DataTextField="FleetName" DataValueField="FleetId" Width="300px" 
                                meta:resourcekey="CheckBoxFleetResource1">
                            </asp:CheckBoxList>
                        </div>
                    </td>
                </tr>
                
            </table>
    
            <div style='font-size: 1em;margin-top:10px;'>
                <input <% if(Assigned) { %> disabled <% } %> type='button' class='kd-button <% if(Assigned) { %> kd-button-disabled <% } %> ' onclick='deleteThisPolygon(this, polygonPopup, selectedFeature);' value=<%=Res_btnDelete %> />
                <input type='button' class='kd-button' onclick='updateServices();savePolygon(this, polygonPopup, selectedFeature);' value=<%=Res_btnSave %> style='margin-left:20px;' /> 
                <input type='button' class='kd-button' value=<%=Res_btnCancel %> onclick='cancelEditPolygon(polygonPopup, selectedFeature);' />
            </div>
        </form>
                    
        </div>
    </div>

    <div id="subjectTemplate">
        <div id="subjectTemplateTitle">
        <span>Subject</span>
        </div>
        <div id="subjectTemplateMain">
            <input type="text" id="txtSubject" class="formtext bsmforminput" style="width:324px;" />            
        
            <span>Choose From a Subject Template</span>        
            <ul>
                <li rel="[ServiceName] - for [VehicleDescription] at [Stdate]" onclick="setSubject(this);">Template1</li>
                <li rel="Speedy - for [VehicleDescription] at [LandmarkName] at [Stdate]" onclick="setSubject(this);">Speed</li>                
            </ul>
            <div>
                <a href='javascript:void(0)' onclick='applyServiceSubject()' style='margin:0 10px;'>OK</a>
                <a href='javascript:void(0)' onclick='cancelServiceSubject();'>Cancel</a>
            </div>
        </div>
            
    </div>

    <div id="recepientsTemplate">
        <div id="recepientsTemplateTitle">
        <span>Recepients</span>
        </div>
        <div id="recepientsTemplateMain">
            <input type="text" id="txtRecepients" class="formtext bsmforminput" style="width:324px;" />       
            
            <div style="margin-top:40px;">
                <a href='javascript:void(0)' onclick='applyServiceRecepients()' style='margin:0 10px;'>OK</a>
                <a href='javascript:void(0)' onclick='cancelServiceRecepients();'>Cancel</a>
            </div>
        </div>
            
    </div>
    
<% if (Show_Header)
   { %>
   
</body>
</html>
<% } %>
