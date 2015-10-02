<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPage.master" AutoEventWireup="true" CodeFile="frmMaintenanceGroup.aspx.cs" Inherits="SentinelFM.Maintenance_frmMaintenanceGroup"  Theme="TelerikControl" meta:resourcekey="PageResource1" %>
<%@ MasterType VirtualPath="~/MasterPage/MasterPage.master" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="Sentinel" Namespace="SentinelFM.Controls" Assembly="SentinelFM.Controls" %>
<%@ Register src="ListMessage.ascx" tagname="ListMessage" tagprefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" 
        meta:resourcekey="RadAjaxManager1Resource1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdMCC" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="gdMCC">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="gdMCC" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="padding-top:5px" >
                                                    <Sentinel:Grid AutoGenerateColumns="False" ID="gdMCC" IsAutoResize="True" 
                                                        AllowSorting="True" Width="600px"
                                                        AllowPaging="True" runat="server" Skin="Hay" GridLines="None"  OnInsertCommand="gdMCC_OnInsertCommand"
                                                        OnNeedDataSource="gdMCC_NeedDataSource" OnUpdateCommand="gdMCC_UpdateCommand"
                                                        OnDeleteCommand="gdMCC_DeleteCommand" OnItemCommand="gdMCC_ItemCommand" PageSize="20" 
                                                        AllowFilteringByColumn="True" FilterItemStyle-HorizontalAlign="Left" 
                                                        OnItemDataBound="gdMCC_ItemDataBound" allText="All" 
                                                        ClearAllFiltersText="Clear All Filters" ExportText="Export" 
                                                        IsShowExportIcon="True" IsShowFilterIcon="True" 
                                                        meta:resourcekey="gdMCCResource1">
                                                        <GroupingSettings CaseSensitive="false" />
                                                        <MasterTableView DataKeyNames="MccId" ClientDataKeyNames="MccId" CommandItemDisplay="Top"
                                                            >
<CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>

<RowIndicatorColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</RowIndicatorColumn>

<ExpandCollapseColumn>
<HeaderStyle Width="20px"></HeaderStyle>
</ExpandCollapseColumn>
                                                            <Columns>
                                                                <telerik:GridTemplateColumn HeaderText="Group Name" UniqueName="MccName" SortExpression="MccName"
                                                                    meta:resourcekey="GridTemplateColumnMccNameResource1" AllowFiltering="true" DataField="MccName">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblMccName" CssClass="formtext" runat="server" Text='<%# Bind("MccName") %>'
                                                                            meta:resourcekey="lblMccNameResource1"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:TextBox ID="txtMccName" runat="server" MaxLength="50" Width="300px" meta:resourcekey="txtMccNameResource1"
                                                                            Text='<%# Bind("MccName") %>'></asp:TextBox>
                                                                        <span style="color: Red">*</span><br />
                                                                        <asp:RequiredFieldValidator ID="valReqtxtMccName" runat="server" ValidationGroup="valMccAdd"
                                                                            ControlToValidate="txtMccName" meta:resourcekey="valReqtxtMccNameResource1" Text="Please enter MCC name"
                                                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                                                    </EditItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridEditCommandColumn ButtonType="ImageButton" UniqueName="EditCommandColumn"
                                                                    meta:resourcekey="GridMccEditCommandColumnResource1" EditImageUrl="../images/edit.gif">
                                                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                                    <HeaderStyle HorizontalAlign="Center" Width="30px" />

                                                                </telerik:GridEditCommandColumn>
                                                                <telerik:GridButtonColumn ConfirmText="Delete this PM Service?" ConfirmDialogType="Classic"
                                                                    ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                    UniqueName="DeleteColumn" meta:resourcekey="GridMccButtonColumnDeleteResource1"
                                                                    ImageUrl="../images/delete.gif">
                                                                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                                                                    <HeaderStyle HorizontalAlign="Center" Width="30px" />

                                                                </telerik:GridButtonColumn>
                                                                <telerik:GridTemplateColumn Visible="false" 
                                                                    meta:resourcekey="GridTemplateColumnResource1" >
                                                                   <ItemTemplate>
                                                                      <asp:Label ID="lblEmpty" runat="server" Text="&nbsp;" 
                                                                           meta:resourcekey="lblEmptyResource1" ></asp:Label>
                                                                   </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn  AllowFiltering = "false" 
                                                                    meta:resourcekey="GridTemplateColumnResource2" >
                                                                    <ItemTemplate >
                                                                       <asp:Button id="btnDelete" Width="0px" Height="0px"  runat="server" 
                                                                            style="display:none; border:0px;" CommandName="Delete" 
                                                                            meta:resourcekey="btnDeleteResource1"  />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="0px" />
                                                                    <HeaderStyle Width="0px"   />
                                                                </telerik:GridTemplateColumn>

                                                            </Columns>
                                                            <EditFormSettings ColumnNumber="2">
                                                                <FormTableItemStyle HorizontalAlign="Left" VerticalAlign="Top"></FormTableItemStyle>
                                                                <FormTableStyle CellSpacing="0" CellPadding="2" Font-Bold="true" HorizontalAlign="Left" />
                                                                <FormTableAlternatingItemStyle Wrap="False" HorizontalAlign="Left" VerticalAlign="Top">
                                                                </FormTableAlternatingItemStyle>
                                                                <EditColumn ButtonType="PushButton" InsertText="Save" UpdateText="Save">
                                                                </EditColumn>
                                                                <FormMainTableStyle />
                                                            </EditFormSettings>
                                                            <HeaderStyle CssClass="RadGridtblHeader" />
                                                            <CommandItemTemplate>
                                                                <table id="tblCustomerCommand" runat="server" width="100%">
                                                                    <tr runat="server">
                                                                        <td align="left" runat="server">
                                                                            <asp:Button ID="btnAdd" Text='<%$ Resources:btnAddMccResource1 %>'  CommandName="InitInsert" runat="server"
                                                                                CssClass="combutton" Width="180px" meta:resourcekey="btnAddMccResource1" />
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </CommandItemTemplate>
                                                        </MasterTableView>
                                                        <ValidationSettings CommandsToValidate="PerformInsert,Update" ValidationGroup="valMccAdd" />
                                                        <ClientSettings>
                                                                            <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="false"  />
                    <Resizing AllowColumnResize="True" EnableRealTimeResize="True" ResizeGridOnColumnResize="false">
                    </Resizing>

                                                        </ClientSettings>

<FilterItemStyle HorizontalAlign="Left"></FilterItemStyle>

<FilterMenu CssClass="FiltMenuCss" EnableTheming="True"></FilterMenu>
                                                    </Sentinel:Grid>
                                                </div>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="True" 
        ShowOnTopWhenMaximized="False" Behavior="Default" InitialBehavior="None" 
        meta:resourcekey="RadWindowManager1Resource1">
        <Windows>
            <telerik:RadWindow ID="RadWindowContentTemplate" Title="Confirm" VisibleStatusbar="false" ShowContentDuringLoad ="true"
                VisibleTitlebar="false" Width="400px" Height="530px" runat="server" Skin="Hay" 
                Modal="true" meta:resourcekey="RadWindowContentTemplateResource1">
                <ContentTemplate>
                    <uc3:ListMessage ID="ListMessage" runat="server" />
                </ContentTemplate>
            </telerik:RadWindow>
        </Windows>
        </telerik:RadWindowManager>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
       <script type="text/javascript" >

           function OpenConfirm(groups, vehicles, isDelete, ctl) {
               $find('<%= RadWindowContentTemplate.ClientID%>').show();
               ListMessage_SetGroupsVehicle(groups, vehicles, isDelete, ctl);
               var minusHeight = 0;
               if (groups == null) minusHeight = minusHeight + 90;
               if (vehicles == null) minusHeight = minusHeight + 330;
               $find('<%= RadWindowContentTemplate.ClientID%>').SetHeight(530 - minusHeight);

           }

           function CLoseConfirm() {
               $find('<%= RadWindowContentTemplate.ClientID%>').close();
               return false;
           }

           function ClickDeleteEvent(id, ctl) {
               if (confirm('<%= deleteGroup%>')) {
                   try {
                       $find("<%= LoadingPanel1.ClientID %>").show("<%= gdMCC.ClientID %>");
                       var postData = "{'MccId':" + id + "}";
                       $telerik.$.ajax({
                           type: "POST",
                           contentType: "application/json; charset=utf-8",
                           url: "frmMaintenanceGroup.aspx/GetVehiclesandGroup",
                           data: postData,
                           dataType: "json",
                           success: function (data) {
                               $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMCC.ClientID %>");
                               if (data.d != '-1' && data.d != "0") {
                                   if (data.d == '') $telerik.$("#" + ctl).click();
                                   else {
                                       var dat = eval(data.d);
                                       var groups = null;
                                       var vehicles = null;
                                       if (dat != '') vehicles = eval(dat);
                                       if (vehicles == null && groups == null) $telerik.$("#" + ctl).click();
                                       else OpenConfirm(groups, vehicles, true, ctl);
                                   }
                               }

                               if (data.d == '-1') {
                                   top.document.all('TopFrame').cols = '0,*';
                                   window.open('../Login.aspx', '_top')
                               }
                               if (data.d == '0') {
                                   alert("<%= Error_Load%>");
                               }
                           },
                           error: function (request, status, error) {
                               $find("<%= LoadingPanel1.ClientID %>").hide("<%= gdMCC.ClientID %>");
                               alert("<%= Error_Load%>");
                               //alert(request.responseText);
                               return false;
                           }

                       });
                   }
                   catch (ex) { }
               }
               return false;
           }          
       </script>
    </telerik:RadCodeBlock>
</asp:Content>

