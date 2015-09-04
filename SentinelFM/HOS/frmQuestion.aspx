<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmQuestion.aspx.cs" Inherits="SentinelFM.HOS_frmQuestion" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Src="../Configuration/Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<%@ Register Src="HosTabs.ascx" TagName="HosTabs" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.4/jquery.min.js"></script>
    <script type="text/javascript" src="../scripts/jquery.cookie.js"></script>
    <link rel="stylesheet" href="../scripts/tablesorter2145/css/theme.blue.css">

    <style>
        .popupdiv {
            -moz-border-bottom-colors: none;
            -moz-border-left-colors: none;
            -moz-border-right-colors: none;
            -moz-border-top-colors: none;
            background-color: #FFFFFF;
            border-color: -moz-use-text-color #BDBDBD #BDBDBD;
            border-image: none;
            border: 1px solid #BDBDBD;
            border-style: solid;
            border-width: medium 1px 1px;
            border-radius: 0 0 10px 10px;
            box-shadow: 0 1px 5px 0 rgba(51, 51, 51, 0.3);
            position: relative;
            min-width: 160px;
            visibility: visible;
            margin: 0;
            font-family: Arial,Helvetica,sans-serif;
            font-size: 13px;
        }

        .GridColumnTitle {
            font-weight: bold;
            text-decoration: underline;
        }
    </style>
</head>
<body topmargin="5px" leftmargin="3px">
    <script language="javascript" type="text/javascript">
        function addQuestion() {
            $('#lblMessage').html('');
            $("#<%= hidCurrentRowId.ClientID %>").val("");
              showQuestionInpute();
          }
          function editQuestion(rowID, lblDefect, lblSMCS, defectLevel) {
              $('#lblMessage').html('');
              $("#<%= hidCurrentRowId.ClientID %>").val(rowID);
              $('#<%= txtDescription.ClientID %>').val($('#' + lblDefect).text());
              var combo = $find("<%= cboSMCS.ClientID %>");
              var combo_itm = combo.findItemByValue($('#' + lblSMCS).text());
              if (combo_itm != null)
                combo_itm.select();
              if (defectLevel == 0) {
                  $("#<%= optDefects.ClientID %>_0").attr('checked', 1);
                  $("#<%= optDefects.ClientID %>_1").attr('checked', 0);
              }
              else {
                  $("#<%= optDefects.ClientID %>_0").attr('checked', 0);
                  $("#<%= optDefects.ClientID %>_1").attr('checked', 1);

              }
              showQuestionInpute();
              return false;
          }

          function showQuestionInpute() {
              $('#divAddQuestion').css('left', ($('#<%= gdMedia.ClientID %>').width() - $('#divAddQuestion').width()) / 2 + $('#<%= gdMedia.ClientID %>').offset().left).css('top', $('#<%= gdMedia.ClientID %>').offset().top + 80).show();
              enableButton(false);
          }
          function cancelQuestionAction() {
              enableButton(true);
              $('#<%= txtDescription.ClientID %>').val('');
              var combo = $find("<%= cboSMCS.ClientID %>");
              combo.clearSelection();
              $('#divAddQuestion').hide();
              $("#<%= optDefects.ClientID %>_0").attr('checked', 1);
              $("#<%= optDefects.ClientID %>_1").attr('checked', 0);
          }

          function AddQuestionAction() {
              var desc = $('#<%= txtDescription.ClientID %>').val();
              if ($.trim(desc) == '') {
                  $('#lblMessage').html('<%= descriptionisrequired %>');
                  return false;
              }
              var combo = $find("<%= cboSMCS.ClientID %>");
              if (combo.get_selectedItem() == null)
              {
                  $('#lblMessage').html('<%= msgSMCSisrequired %>');
                  return false;

              }
              var var_smcsValue = combo.get_selectedItem().get_value();
              if (var_smcsValue == undefined || var_smcsValue == "-1" || var_smcsValue == "")
              {
                  $('#lblMessage').html('<%= msgSMCSisrequired %>');
                  return false;
              }
              return true;
          }

          function enableButton(isEnable) {
              if (isEnable == true) {
                  $("input[tag='dynamicInspec']").attr("disabled", "");
              }
              else {
                  $("input[tag='dynamicInspec']").attr("disabled", "enabled");
              }
          }
    </script>
    <form id="form1" runat="server">
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
        </telerik:RadAjaxLoadingPanel>
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1" meta:resourcekey="RadAjaxManager1Resource1"
            Style="text-decoration: underline">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="gdMedia">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="gdMedia" LoadingPanelID="LoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <div style="text-align: left; width: 900px">
            <table id="Table1" border="0" cellpadding="0" cellspacing="0" width="300">
                <tr align="left">
                    <td>
                        <uc1:ctlMenuTabs ID="ctlMenuTabs2" runat="server" selectedcontrol="btnHOS" />
                    </td>
                </tr>
                <tr align="left">
                    <td>
                        <table id="Table2" align="left" border="0" cellpadding="0" cellspacing="0" width="990">
                            <tr>
                                <td>
                                    <uc2:HosTabs ID="HosTabs1" runat="server" selectedcontrol="cmdQuestion" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table id="Table3" border="0" cellpadding="0" cellspacing="0" class="frame" style="height: 550px; width: 1200px;">
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
                                                                                    <table id="Table7" border="0" cellpadding="0" cellspacing="0" style="width: 1170px; margin-top: 10px; margin-bottom: 10px" class="tableDoubleBorder">
                                                                                        <tr>
                                                                                            <td>
                                                                                                <table id="Table5" align="center" border="0" cellpadding="0" cellspacing="0"
                                                                                                    style="height: 700px">
                                                                                                    <tr valign="top">
                                                                                                        <td>

                                                                                                            <table id="Table8" cellspacing="0" cellpadding="0" width="870" align="center" border="0">
                                                                                                                <tr>
                                                                                                                    <td>
                                                                                                                        <table id="Table9" class="table" width="1170px" height="700px"
                                                                                                                            border="0">
                                                                                                                            <tr>
                                                                                                                                <td class="configTabBackground" style="vertical-align: top; width: 100%">
                                                                                                                                    <table style="vertical-align: top; width: 100%">
                                                                                                                                        <tr>
                                                                                                                                            <td align="center" style="width: 100%; padding-top: 5px; padding-bottom: 5px" valign="top">
                                                                                                                                                <telerik:RadGrid AutoGenerateColumns="False" ID="gdMedia" AllowAutomaticDeletes="false"
                                                                                                                                                    PageSize="25" AllowAutomaticInserts="false" AllowSorting="true" AllowAutomaticUpdates="false"
                                                                                                                                                    AllowPaging="True" runat="server" Skin="Hay" GridLines="None" meta:resourcekey="gdMediaResource1"
                                                                                                                                                    Width="900px" AllowFilteringByColumn="true" OnNeedDataSource="gdMedia_NeedDataSource" OnDeleteCommand="gdMedia_DeleteCommand"
                                                                                                                                                    OnItemDataBound="gdMedia_ItemDataBound" OnItemCommand="gdMedia_ItemCommand"
                                                                                                                                                    FilterItemStyle-HorizontalAlign="Left">
                                                                                                                                                    <GroupingSettings CaseSensitive="false" />
                                                                                                                                                    <MasterTableView DataKeyNames="RowID" ClientDataKeyNames="RowID" CommandItemDisplay="Top">
                                                                                                                                                        <Columns>
                                                                                                                                                            <telerik:GridTemplateColumn HeaderText="ID" UniqueName="RowID" SortExpression="RowID"
                                                                                                                                                                meta:resourcekey="GridTemplateColumnRowIDResource1" AllowFiltering="true"
                                                                                                                                                                DataField="RowID">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:Label ID="lblRowID" CssClass="formtext" runat="server" Text='<%# Bind("RowID") %>'
                                                                                                                                                                        meta:resourcekey="lblRowIDResource1">
                                                                                                                                                                    </asp:Label>
                                                                                                                                                                </ItemTemplate>
                                                                                                                                                                <ItemStyle HorizontalAlign="Left" Width="50px" />
                                                                                                                                                                <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                                            <telerik:GridTemplateColumn HeaderText="Description" UniqueName="Defect" SortExpression="Defect"
                                                                                                                                                                meta:resourcekey="GridTemplateColumnDefectResource1" AllowFiltering="true"
                                                                                                                                                                DataField="Defect">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:Label ID="lblDefect" CssClass="formtext" runat="server" Text='<%# Bind("Defect") %>'
                                                                                                                                                                        meta:resourcekey="lblDefectResource1">
                                                                                                                                                                    </asp:Label>
                                                                                                                                                                </ItemTemplate>

                                                                                                                                                                <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                                            <telerik:GridTemplateColumn HeaderText="SMCS" UniqueName="SMCSCode" SortExpression="SMCSCode"
                                                                                                                                                                meta:resourcekey="GridTemplateColumnSMCSCodeResource1" AllowFiltering="true"
                                                                                                                                                                DataField="SMCSCode">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:Label ID="lblSMCSCode" CssClass="formtext" runat="server" Text='<%# Bind("SMCSCode") %>'
                                                                                                                                                                        meta:resourcekey="lblSMCSCodeResource1">
                                                                                                                                                                    </asp:Label>
                                                                                                                                                                </ItemTemplate>
                                                                                                                                                                <ItemStyle HorizontalAlign="Left" />
                                                                                                                                                                <HeaderStyle HorizontalAlign="Left" Font-Bold="true" Width="100px" />
                                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                                            <telerik:GridTemplateColumn HeaderText="Level" UniqueName="DefectLevelDesc" SortExpression="DefectLevelDesc"
                                                                                                                                                                meta:resourcekey="GridTemplateColumnDefectLevelResource1" AllowFiltering="true"
                                                                                                                                                                DataField="DefectLevelDesc">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:Label ID="lblDefectLevel" CssClass="formtext" runat="server" Text='<%# Bind("DefectLevelDesc") %>'
                                                                                                                                                                        meta:resourcekey="lblDefectLevelResource1">
                                                                                                                                                                    </asp:Label>
                                                                                                                                                                </ItemTemplate>
                                                                                                                                                                <ItemStyle HorizontalAlign="Left" Width="100px" />
                                                                                                                                                                <HeaderStyle HorizontalAlign="Left" Font-Bold="true" />
                                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                                            <telerik:GridTemplateColumn AllowFiltering="false">
                                                                                                                                                                <ItemTemplate>
                                                                                                                                                                    <asp:ImageButton ID="bntEdit" runat="server" ImageUrl="../images/edit.gif" />
                                                                                                                                                                </ItemTemplate>
                                                                                                                                                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                                                                                                                            </telerik:GridTemplateColumn>
                                                                                                                                                            <telerik:GridButtonColumn ConfirmDialogType="Classic"
                                                                                                                                                                ConfirmTitle="Delete" ButtonType="ImageButton" CommandName="Delete" Text="Delete"
                                                                                                                                                                UniqueName="DeleteColumn" meta:resourcekey="GridButtonColumnDeleteResource1"
                                                                                                                                                                ImageUrl="../images/delete.gif">
                                                                                                                                                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                                                                                                                            </telerik:GridButtonColumn>
                                                                                                                                                        </Columns>
                                                                                                                                                        <HeaderStyle CssClass="RadGridtblHeader" />
                                                                                                                                                        <CommandItemTemplate>
                                                                                                                                                            <table width="100%">
                                                                                                                                                                <tr>
                                                                                                                                                                    <td align="left">
                                                                                                                                                                        <asp:Button ID="btnAdd" Text="Add New Question" CommandName="InitInsert" runat="server" CssClass="combutton" Width="150px"
                                                                                                                                                                            meta:resourcekey="btnAddResource1" OnClientClick="javascript:return addQuestion()" />
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
                                                                                                                                                    <ValidationSettings CommandsToValidate="PerformInsert,Update" />
                                                                                                                                                    <ClientSettings>
                                                                                                                                                        <ClientEvents OnGridCreated="GridCreated" />
                                                                                                                                                    </ClientSettings>
                                                                                                                                                </telerik:RadGrid>
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
        <div id="divAddQuestion" class="popupdiv" border="0" style="display: none; z-index: 1000; position: absolute">
            <table>
                <tr>
                    <td height="28">
                        <asp:Label ID="lblDescription" runat="server" Text="Description:"
                            meta:resourcekey="lblDescriptionResource1"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" Height="50px" Style="width: 500px" />
                    </td>
                </tr>
                <tr>
                    <td height="28">
                        <asp:Label ID="lblSMCS" runat="server" Text="SMCS Code:"
                            meta:resourcekey="lblSMCSResource1"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadComboBox ID="cboSMCS" runat="server" Height="200px" Width="500px" DropDownWidth="500px"
                            AllowCustomText="true" 
                            EmptyMessage="Select a SMCS code" HighlightTemplatedItems="true" Filter="Contains" DataTextField="SMCS"
                            DataValueField="SMCS" AppendDataBoundItems="true"
                            meta:resourcekey="cboQuestionsResource1">
                            <Items>
                                <telerik:RadComboBoxItem Text="Select a Question" Value="-1" />
                            </Items>
                            </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:RadioButtonList ID="optDefects" runat="server" CssClass="formtext" meta:resourcekey="optDefectsResource1" RepeatDirection="Horizontal">
                            <asp:ListItem Value="0" meta:resourcekey="optDefectsMinorResource" Selected="true" Text="Minor"></asp:ListItem>
                            <asp:ListItem Value="1" meta:resourcekey="optDefectsMajorResource" Text="Major"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="615px" meta:resourcekey="lblMessageResource1"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td valign="bottom" colspan="2" align="center">
                        <asp:Button CssClass="kd-button" OnClientClick="javascript:return AddQuestionAction();"
                            OnClick="btnaddQuestionAction_Click" ID="btnaddQuestionAction" runat="server"
                            Text="Save" Style="width: 75px;" meta:resourcekey="btnaddQuestionActionResource2" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button CssClass="kd-button"
                                OnClientClick="cancelQuestionAction();return false;" ID="btncancelQuestionAction" runat="server"
                                Text="Cancel" Style="width: 75px;" meta:resourcekey="btncancelQuestionActionResource2" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="hidFilter" runat="server" />
        <asp:HiddenField ID="hidCurrentRowId" runat="server" />
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            <script type="text/javascript">
                function showFilterItem() {
                    if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                        setTimeout("SetGridFilterWidth()", 10);
                        $find('<%=gdMedia.ClientID %>').get_masterTableView().showFilterItem();
                        $telerik.$("#<%= hidFilter.ClientID %>").val('1');
                        $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");

                    }
                    else {
                        $find('<%=gdMedia.ClientID %>').get_masterTableView().hideFilterItem();
                        $telerik.$("#<%= hidFilter.ClientID %>").val('');
                        $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");

                    }
                    return false;
                }


                function GridCreated() {
                    if ($telerik.$("#<%= hidFilter.ClientID %>").val() == '') {
                    $find('<%=gdMedia.ClientID %>').get_masterTableView().hideFilterItem();
                    $telerik.$("a[id$='hplFilter']").text("<%= showFilter %>");
                }
                else {
                    $telerik.$("a[id$='hplFilter']").text("<%= hideFilter %>");
                }
                setTimeout("SetGridFilterWidth()", 10);
            }
            function SetGridFilterWidth() {
                $telerik.$(".rgFilterBox[type='text']").each(function () {
                    if ($telerik.$(this).css("visibility") != "hidden") {
                        var buttonWidth = 0;
                        if ($telerik.$(this).next("[type='submit']").length > 0) {
                            buttonWidth = $telerik.$(this).next("[type='submit']").width();
                        }
                        if ($telerik.$(this).next("[type='button']").length > 0) {
                            buttonWidth = $telerik.$(this).next("[type='button']").width();
                        }

                        if (buttonWidth > 0) {
                            $telerik.$(this).width($telerik.$(this).parent().width() - buttonWidth - 10);
                        }
                        else {
                            $telerik.$(this).width($telerik.$(this).parent().width() - 50);
                        }
                    }
                })
                hasSetWidth = true;
            }

            </script>
        </telerik:RadCodeBlock>

    </form>
</body>
</html>
