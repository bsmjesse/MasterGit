<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmMDTVersion.aspx.cs" Inherits="HOS_frmMDTVersion" %>


<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register src="../Configuration/Components/ctlMenuTabs.ascx" tagname="ctlMenuTabs" tagprefix="uc1" %>

<%@ Register src="HosTabs.ascx" tagname="HosTabs" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../Configuration/Configuration.css" type="text/css" rel="stylesheet" />
</head>
<body topmargin="5px" leftmargin="3px">
    <script language="javascript">
	    <!--
        OrganizationHierarchyPath = "<%=OrganizationHierarchyPath %>";
            var DefaultOrganizationHierarchyFleetId = <%=DefaultOrganizationHierarchyFleetId %>;
            var DefaultOrganizationHierarchyNodeCode = '<%=DefaultOrganizationHierarchyNodeCode %>';
            var TempSelectedOrganizationHierarchyFleetId = DefaultOrganizationHierarchyFleetId;
        
            function onOrganizationHierarchyNodeCodeClick()
            {
                var mypage='../Widgets/OrganizationHierarchy.aspx?nodecode=' + document.getElementById('<%=hidOrganizationHierarchyNodeCode.ClientID %>').value;//$('#<%=hidOrganizationHierarchyNodeCode.ClientID %>').val();
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
            document.getElementById("<%=hidOrganizationHierarchyNodeCode.ClientID %>").value = nodecode;
            document.getElementById("<%=hidOrganizationHierarchyFleetId.ClientID %>").value = fleetId;
            document.getElementById("<%=hidOrganizationHierarchyPostBack.ClientID %>").click();            
        }
            
        //-->
    </script>

    <form id="form1" runat="server">

        <asp:HiddenField ID="hidOrganizationHierarchyNodeCode" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyFleetId" runat="server" />
        <asp:HiddenField ID="hidOrganizationHierarchyPath" runat="server" />

        <asp:Button ID="hidOrganizationHierarchyPostBack" runat="server" Text="Button" 
            style="display:none;" AutoPostBack="True"
                    OnClick="hidOrganizationHierarchyPostBack_Click" 
            meta:resourcekey="hidOrganizationHierarchyPostBackResource1" />

    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" >
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1" >
        <ajaxsettings>

       </ajaxsettings>
    </telerik:RadAjaxManager>

    <div style="text-align:center">

        <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="300" >
            <tr align="left">
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs2" runat="server" SelectedControl="btnHOS"  />
                </td>
            </tr>
            <tr align="left">
                <td  >
                    <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="990">
                        <tr>
                            <td><uc2:HosTabs ID="HosTabs1" runat="server" SelectedControl="cmdMDTVersion" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table id="Table3" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width:990px;">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="Table4" border="0" cellpadding="0" cellspacing="0" style="left: 10px; position: relative; top: 0px">
                                                <tr>
                                                    <td>
                                                        <table id="tblSubCommands" border="0" cellpadding="0" cellspacing="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table6" align="center" border="0" cellpadding="0" cellspacing="0" width="760">
                                                                        <tr>
                                                                            <td>
                                                                                <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 960px;" class="tableDoubleBorder" >
                                                                                    <tr>
                                                                                        <td>
                                                                                            <table id="Table5" align="center" border="0" cellpadding="0" cellspacing="0" 
                                                                                                style="width: 950px; height: 495px">
                                                                                                <tr valign="top">
                                                                                                    <td>
                                                                                                    
																		<TABLE id="Table8" cellSpacing="0" cellPadding="0" width="657" align="center" border="0">
																			<TR>
																				<TD>
																					<TABLE id="Table9" class=table WIDTH="960px" HEIGHT="500px"
																						border="0">
																						<TR>
																							<TD class="configTabBackground" valign=top  >
																								<TABLE id="Table10" style="WIDTH: 900px; HEIGHT: 444px" cellSpacing="0" cellPadding="0"
																									width="900px" align="center" border="0">
																									<TR>
																										<TD class="tableheading" align="left" height="5" style="width: 900px"></TD>
																									</TR>
                                                                                                    <tr id="trFleetSelectOption" visible="false" runat="server">
                                                                                                        <td height="20" align="center" class="configTabBackground">
                                                                                                            <table class="formtext" runat="server" id="optBaseTable"  cellpadding=0 cellspacing=0  >
                                                                                                                <tr>
                                                                                                                    <td> 
                                                                                                                        <asp:Label ID="Label11" runat="server" Text="Based On:" meta:resourcekey="Label11Resource1"></asp:Label> 
                                                                                                                    </td>
                                                                                                                    <td>
                                                                                                                        <asp:RadioButtonList ID="optAssignBased" name="AssignBased" runat="server"  class="formtext" 
                                                                                                                                    RepeatDirection="Horizontal" AutoPostBack="True"
                                                                                                                                    onselectedindexchanged="optAssignBased_SelectedIndexChanged" 
                                                                                                                                    meta:resourcekey="optAssignBasedResource1" >
                                                                                                                            <asp:ListItem id="Radio2" type="radio" name="raFleetSelectOption" value="0" checked
                                                                                                                                runat="server" Selected="True" meta:resourcekey="ListItemResource1" >Fleet</asp:ListItem> 
                                                                                                                            <asp:ListItem id="Radio1" name="raFleetSelectOption" value="1" runat="server" 
                                                                                                                                        meta:resourcekey="ListItemResource2" >Organization Hierarchy Fleet</asp:ListItem>
                                                                                                                        </asp:RadioButtonList>                                                         
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </tr>
																									<TR id="trBasedOnNormalFleet" runat="server">
																										<TD style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: gray; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: gray; BORDER-TOP-COLOR: gray; HEIGHT: 30px; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: gray; width: 900px;"
																											align="center" colSpan="1">
																											<TABLE id="Table11" cellSpacing="0" cellPadding="0" align="center" border="0">
																												<TR>
																													<TD><asp:label id="lblFleet" runat="server" CssClass="formtext" meta:resourcekey="lblFleetResource1" Text="Fleet:"></asp:label></TD>
																													<TD><asp:dropdownlist id="cboFleet" runat="server" CssClass="RegularText" AutoPostBack="True" DataValueField="FleetId"
																															DataTextField="FleetName" onselectedindexchanged="cboFleet_SelectedIndexChanged" meta:resourcekey="cboFleetResource1"></asp:dropdownlist></TD>
																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
                                                                                                    <tr id="trBasedOnHierarchyFleet" visible="false" runat="server">
                                                                                                        <TD style="BORDER-TOP-WIDTH: 1px; BORDER-LEFT-WIDTH: 1px; BORDER-LEFT-COLOR: gray; BORDER-BOTTOM-WIDTH: 1px; BORDER-BOTTOM-COLOR: gray; BORDER-TOP-COLOR: gray; HEIGHT: 30px; BORDER-RIGHT-WIDTH: 1px; BORDER-RIGHT-COLOR: gray; width: 669px;"
																											align="center" colSpan="1">
																											<TABLE id="Table16" cellSpacing="0" cellPadding="0" align="center" border="0">
																												<tr>
                                                                                                                      <td class="style1">
                                                                                                                            <asp:Label ID="lblOhTitle" runat="server" CssClass="formtext" 
                                                                                                                                Text=" Hierarchy Node:" meta:resourcekey="lblOhTitleResource1"  /></td>
                                                                                                                     <td>
                                                                                                                            <asp:Button ID="btnOrganizationHierarchyNodeCode" runat="server" 
                                                                                                                                CssClass="combutton" Width="200px" 
                                                                                                                                OnClientClick="return onOrganizationHierarchyNodeCodeClick();" 
                                                                                                                                meta:resourcekey="btnOrganizationHierarchyNodeCodeResource1" />
                                                                                                                     </td>
                                                                                                                  </tr>
																											</TABLE>
																										</TD>
                                                                                                    </tr>
																									<TR>
																										<TD style="WIDTH: 900px; HEIGHT: 217px" align="center" valign="top">
																											<TABLE id="Table12" style="HEIGHT: 210px"  cellSpacing="0" cellPadding="0" border="0">
																												<TR>
																													<TD valign="top">
                                                                                                                   <telerik:RadGrid  ID="dgEmails" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                                                                                                        GridLines="None" PageSize="40"
                                                                                                                         EnableTheming="True"
                                                                                                                        Width="674px" 
                                                                                                                        AllowFilteringByColumn="true"
                                                                                                                        AllowSorting="True" 
                                                                                                                        FilterItemStyle-HorizontalAlign="Left" Skin="Hay"
                                                                                                                        meta:resourcekey="gdvDriversResource2" onneeddatasource="dgEmails_NeedDataSource" 
                                                                                                                         >
                                                                                                                        <GroupingSettings CaseSensitive="false" />   
                                                                                                                        <MasterTableView  CommandItemDisplay="Top"
                                                                                                                           GroupLoadMode="Server" NoMasterRecordsText=""  meta:resourcekey="MasterTableViewResource1" >                                                                                                                        
                                                                                                                           
                                                                                                                        <Columns>
                                                                                                                            <telerik:GridBoundColumn DataField="BoxId" HeaderText ="BoxId"  meta:resourcekey="BoundFieldResource1"     >
                                                                                                                               
                                                                                                                            </telerik:GridBoundColumn>
                                                                                                                            <telerik:GridBoundColumn DataField="Description" HeaderText ="Vehicle" meta:resourcekey="BoundFieldResource2" />
                                                                                                                            <telerik:GridBoundColumn DataField="Version" HeaderText = "Version" meta:resourcekey="BoundFieldResource3" />
                                                                                                                            <telerik:GridBoundColumn DataField="HOSVehicleId" HeaderText = "PPCID" meta:resourcekey="BoundFieldResource4" />

                                                                                                                        </Columns>
                                                                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                                                                        <AlternatingItemStyle HorizontalAlign="Left" />
                                                                                                                        <HeaderStyle HorizontalAlign="Left" CssClass="RadGridtblHeader"  />
                                                                                                                        <CommandItemTemplate>
                                                                                                                        <table width="100%">
                                                                                                                            <tr>
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
                                                                                                                    <asp:HiddenField ID="hidFilter" runat="server" />


                                                                                                                    
                                                                                                                    </TD>
																												</TR>
																											</TABLE>
																										</TD>
																									</TR>
																								</TABLE>
																							</TD>
																						</TR>
																					</TABLE>
																				</TD>
																			</TR>
																		</TABLE>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </table>
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
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function showFilterItem() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgEmails.ClientID %>').get_masterTableView().showFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                else {
                    $find('<%=dgEmails.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("#<%= hidFilter.ClientID %>").val('');
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                }
                return false;
            }

            function GridCreated() {
                if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=dgEmails.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
            }

        </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>


