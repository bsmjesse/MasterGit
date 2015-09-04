<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmEditContactPlan.aspx.cs"
    Inherits="SentinelFM.Configuration_Contact_frmEditContactPlan" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1">
        <Scripts>
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
        </Scripts>
    </telerik:RadScriptManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
    </telerik:RadAjaxLoadingPanel>
    <asp:Panel ID="pnl" runat="server">
        <table width="730px">
            <tr>
                <td>
                    <asp:Label ID="lblContactPlanName" CssClass="RegularText" runat="server" Text="Plan Name:" meta:resourcekey="lblContactPlanNameResource1" ></asp:Label>
                    <asp:TextBox ID="txtContactPlanName" runat="server" MaxLength="50" Width="300px" CssClass="RegularText"
                        meta:resourcekey="txtContactPlanNameResource1"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valReqtxtContactPlanName" runat="server" ControlToValidate="txtContactPlanName"
                        meta:resourcekey="txtContactPlanNameResource1" ValidationGroup="vgAdd" Text="Please enter plan name"
                        Display="Dynamic"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:Panel ID="pnlPhones" runat="server">
                                    <fieldset>
                                        <legend style="color: green" runat="server" id="legendPhones"></legend>
                                        <table cellspacing="0" border="0" id="EditTelephone_gdContact_ctl00">
                                            <tbody>
                                                <tr valign="top" id="EditeTelephone_gdContact_ctl00">
                                                    <td width="80px" valign="middle">
                                                        <nobr>
                                                        <asp:Label runat="server" CssClass="RegularText" ID="lblPriority" Text="Priority 1" ForeColor="Blue" meta:resourcekey="lblPriorityResource1" > </asp:Label>
                                                        </nobr>
                                                    </td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="cboTelephone" runat="server"  Width="300px" meta:resourcekey="cboTelephoneResource1"
                                                            AppendDataBoundItems="true" DataTextField="Name" DataValueField="contactCommunicationID">
                                                            <asp:ListItem Text="Select a Contact" Value="-1" meta:resourcekey="cboTelephoneItem0Resource1">
                                                            </asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" valign="middle" >
                                                        <asp:Label   BackColor="#dee0c8" ID="lblTelephone" runat="server" Width="320px"  meta:resourcekey="lblTelephoneResource1"></asp:Label>
                                                    </td>
                                                    <td align="left" style="width: 15px;">
                                                        <asp:ImageButton ID="btnDelTelephone" runat="server" ImageUrl="~/images/No.gif" ToolTip="Delete"
                                                            meta:resourcekey="TelephoneResource1" Width="12px" Height="12px" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </fieldset>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="pnlEmails" runat="server">
                                    <fieldset>
                                        <legend style="color: green" runat="server" id="legendEmails"></legend>
                                        <table>
                                            <tr>
                                                <td>
                                                    <table width="700px">
                                                        <tr>
                                                            <td align="left">
                                                                <telerik:RadComboBox ID="cboEmails" runat="server" Height="200px" Width="420px" DropDownWidth="650px"
                                                                    EmptyMessage="Select a Email" HighlightTemplatedItems="true" Filter="StartsWith"
                                                                    OnClientDropDownOpening="onDropDownOpening">
                                                                    <HeaderTemplate>
                                                                        <table cellspacing="0" cellpadding="0">
                                                                            <tr >
                                                                                <td style="width: 15px;">
                                                                                </td>
                                                                                <td style="width: 280px;"  class="RegularText">
                                                                                    Name
                                                                                </td>
                                                                                <td style="width: 320px;"  class="RegularText">
                                                                                    Email
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <table cellspacing="0" cellpadding="0">
                                                                            <tr >
                                                                                <td style="width: 15px;">
                                                                                    <asp:CheckBox ID="chkEmail" runat="server" />
                                                                                </td>
                                                                                <td style="width: 280px;"  class="RegularText">
                                                                                    <%# DataBinder.Eval(Container, "Attributes['Name']")%>
                                                                                </td>
                                                                                <td style="width: 320px;"  class="RegularText">
                                                                                    <asp:Label runat="server" ID="lblCommunicationData" Text='<%# DataBinder.Eval(Container, "Attributes[\"CommunicationData\"]")%>'></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ItemTemplate>
                                                                </telerik:RadComboBox>
                                                                <asp:Button ID="btnAddEmail" runat="server" Text="Add Email" meta:resourcekey="btnAddEmailResource1" CssClass="combutton"
                                                                    OnClientClick="javascript:return AddEmail_Click()"></asp:Button>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table cellspacing="0" border="0" id="EditeMail_gdContact_ctl00">
                                                        <tbody>
                                                            <tr valign="top" style="visibility: hidden;" >
                                                                <td align="left">
                                                                    <asp:Label ID="lbleMailName"  runat="server" Width="300px" meta:resourcekey="lbleMailNameResource1"></asp:Label>
                                                                </td>
                                                                <td align="left">
                                                                    <asp:Label ID="lbleMail"  runat="server" Width="350" BackColor="#dee0c8" meta:resourcekey="lbleMailResource1"></asp:Label>
                                                                    <asp:HiddenField ID="hidcontactCommunicationID" runat="server" />
                                                                </td>
                                                                <td align="left" style="width: 15px;">
                                                                    <asp:ImageButton ID="btnDeleMail" runat="server" ImageUrl="~/images/No.gif" ToolTip="Delete"
                                                                        meta:resourcekey="btnDeleMailResource1" Width="12px" Height="12px" />
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button ID="btnSave" runat="server" Text="Save" meta:resourcekey="btnDelResource1"
                        ValidationGroup="vgAdd" OnClientClick="javascript:return Save_Click('vgAdd');" CssClass="combutton" />
                    <asp:Button ID="btnClose" runat="server" meta:resourcekey="btnNoResource1" Text="Close"
                        OnClientClick="javascript:return returnToParent();" CssClass="combutton" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="true" meta:resourcekey="lblMessageResource1"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function returnToParent() {
                var oWnd = GetRadWindow();
                oWnd.BrowserWindow.RebindPlans();
                oWnd.close(); 
                return false;
            }

            //New
            function AddEmail_Click() {
                var cboEmails = $find("<%= cboEmails.ClientID %>");
                var items = cboEmails.get_items();
                for (i = 0; i < items.get_count(); i++) {
                    if ($telerik.$(items.getItem(i).get_element()).find("[id$='chkEmail']:checked").length > 0) {
                        var emailValue = items.getItem(i).get_value();
                        var emailText = items.getItem(i).get_text();
                        var emailContent = $telerik.$(items.getItem(i).get_element()).find("[id$='lblCommunicationData']").html();
                        var valueFilter = "[id$='hidcontactCommunicationID']&&[value='" + emailValue + "']";
                        if ($telerik.$("#EditeMail_gdContact_ctl00>tbody>tr").find(valueFilter).length > 0) {
                            $telerik.$(items.getItem(i).get_element()).find("[id$='chkEmail']:checked").attr('checked', false);
                            continue;
                        }
                        var trs_email = $telerik.$("#EditeMail_gdContact_ctl00>tbody>tr");
                        if ($telerik.$(trs_email[trs_email.length - 1]).css("visibility") == "hidden")
                            $telerik.$(trs_email[trs_email.length - 1]).css("visibility", "visible")
                        else {
                            $telerik.$("#EditeMail_gdContact_ctl00>tbody:last").append($telerik.$(trs_email[trs_email.length - 1]).clone(false));
                        }
                        var trs_email = $telerik.$("#EditeMail_gdContact_ctl00>tbody>tr");
                        $telerik.$($telerik.$(trs_email[trs_email.length - 1]).find("[id$='lbleMailName']")[0]).html(emailText);
                        $telerik.$($telerik.$(trs_email[trs_email.length - 1]).find("[id$='lbleMail']")[0]).html(emailContent);
                        $telerik.$($telerik.$(trs_email[trs_email.length - 1]).find("[id$='hidcontactCommunicationID']")[0]).val(emailValue);
                        $telerik.$(items.getItem(i).get_element()).find("[id$='chkEmail']:checked").attr('checked', false);
                    }
                }
                SetupEvents_eMail();
                return false;
            }

            function SelectedIndexChanged_tel(event) {
                var trs = $telerik.$("#EditTelephone_gdContact_ctl00>tbody>tr");
                var ddl = $telerik.$(trs[event.data.rowIndex]).find("[name$='cboTelephone']");
                if (ddl[0].selectedIndex == 0) {
                    $telerik.$(trs[event.data.rowIndex]).find("[id$='lblTelephone']").html("");
                    return;
                }

                for (var index = 0; index < trs.length; index++) {
                    var ddl_1 = $telerik.$(trs[index]).find("[name$='cboTelephone']");
                    if (ddl_1[0].selectedIndex == ddl[0].selectedIndex && index != event.data.rowIndex) {
                        ddl[0].selectedIndex = selectedTelIndex;
                        alert("<%= selectAlreday %>");
                        return;
                    }
                }


                var ddlID = $telerik.$(ddl[0]).attr("id");
                var communicationID = $telerik.$(ddl[0]).val();

                var postData = "{'CommunicationID':'" + communicationID + "'}";

                $find("<%= LoadingPanel1.ClientID %>").show(ddlID);
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "frmEditContactPlan.aspx/GetCommunicationData",
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d != '-1' && data.d != "0") {
                            RenderData(data.d, trs[event.data.rowIndex])
                            AddNewRow(event);
                            $find("<%= LoadingPanel1.ClientID %>").hide(ddlID);
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $find("<%= LoadingPanel1.ClientID %>").hide(ddlID);
                            alert("<%= errorLoad%>");
                            return false;
                        }

                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide(ddlID);
                        alert("<%= errorLoad%>");
                        //alert(request.responseText);
                        return false;
                    }

                });


            }
            //End
            function RenderData(typeData, tr) {
                if (typeData != null) {
                    var commus = eval(typeData);
                    if (commus != null && commus.length > 0) {
                        for (var index = 0; index < commus.length; index++) { //Set contact information
                            //Add telepohne
                            if (commus[index][1] == "<%= emergencyPhoneId%>") {
                                $telerik.$(tr).find("[id$='lblTelephone']").html(commus[index][2]);
                            }

                            //Add email
                            if (commus[index][1] == "<%= emailId%>") {
                                var valueFilter = "[id$='hidcontactCommunicationID']&&[value='" + commus[index][0] + "']";
                                if ($telerik.$("#EditeMail_gdContact_ctl00>tbody>tr").find(valueFilter).length > 0) continue; ;

                                var trs_email = $telerik.$("#EditeMail_gdContact_ctl00>tbody>tr");
                                if ($telerik.$(trs_email[trs_email.length - 1]).css("visibility") == "hidden")
                                    $telerik.$(trs_email[trs_email.length - 1]).css("visibility", "visible")
                                else {
                                    $telerik.$("#EditeMail_gdContact_ctl00>tbody:last").append($telerik.$(trs_email[trs_email.length - 1]).clone(false));
                                }
                                var trs_email = $telerik.$("#EditeMail_gdContact_ctl00>tbody>tr");
                                $telerik.$($telerik.$(trs_email[trs_email.length - 1]).find("[id$='lbleMailName']")[0]).html($telerik.$(tr).find("[name$='cboTelephone'] option:selected").text());
                                $telerik.$($telerik.$(trs_email[trs_email.length - 1]).find("[id$='lbleMail']")[0]).html(commus[index][2]);
                                $telerik.$($telerik.$(trs_email[trs_email.length - 1]).find("[id$='hidcontactCommunicationID']")[0]).val(commus[index][0]);
                                SetupEvents_eMail();
                            }


                        }
                    }
                }

            }


            function AddNewRow(event) {
                var trs = $telerik.$("#EditTelephone_gdContact_ctl00>tbody>tr");
                if (trs.length - 1 == event.data.rowIndex) { //Add new row if it is last row
                    $telerik.$("#EditTelephone_gdContact_ctl00>tbody:last").append($telerik.$(trs[trs.length - 1]).clone(false));
                    var trs = $telerik.$("#EditTelephone_gdContact_ctl00>tbody>tr");
                    ResetLastEmptyRow(trs[trs.length - 1]);
                }
            }
            function ResetLastEmptyRow(row) {
                $telerik.$(row).find("[name$='cboTelephone']")[0].selectedIndex = 0;
                $telerik.$($telerik.$(row).find("[id$='lblTelephone']")[0]).html("");
                SetupEvents_Telephone();
            }

            function DeleRow_Telephone(event) {
                var trs = $telerik.$("#EditTelephone_gdContact_ctl00>tbody>tr");
                if (trs.length - 1 > event.data.rowIndex) {
                    $telerik.$(trs[event.data.rowIndex]).remove();
                    SetupEvents_Telephone();
                }
                return false;
            }

            function DeleRow_eMail(event) {
                var trs_email = $telerik.$("#EditeMail_gdContact_ctl00>tbody>tr");
                if (trs_email.length == 1) {
                    DeleteLastEmailRow(trs_email[event.data.rowIndex]);
                }
                else {
                    $telerik.$(trs_email[event.data.rowIndex]).remove();
                    SetupEvents_eMail();
                }
                return false;
            }

            function DeleteLastEmailRow(row) {
                $telerik.$(row).css("visibility", "hidden");
                $telerik.$($telerik.$(row).find("[id$='lbleMailName']")[0]).html("");
                $telerik.$($telerik.$(row).find("[id$='lbleMail']")[0]).html("");
                $telerik.$($telerik.$(row).find("[id$='hidcontactCommunicationID']")[0]).val("");

            }

            function SetupEvents_Telephone() {
                var trs = $telerik.$("#EditTelephone_gdContact_ctl00>tbody>tr");
                if (trs.length > 0) {
                    trs.each(function (index) {
                        //New
                        $telerik.$(this).find("[name$='cboTelephone']").unbind("change");
                        //End
                        $telerik.$(this).find("[name$='cboTelephone']").unbind("click");
                        $telerik.$(this).find("[name$='btnDelTelephone']").unbind("click");
                        if (index == trs.length - 1) {
                            $telerik.$(this).find("[name$='btnDelTelephone']").hide();
                        }
                        else {
                            $telerik.$(this).find("[name$='btnDelTelephone']").show();
                        }
                        $telerik.$(this).find("[name$='btnDelTelephone']").bind("click", { rowIndex: index }, DeleRow_Telephone);
                        $telerik.$(this).find("[name$='cboTelephone']").bind("change", { rowIndex: index }, SelectedIndexChanged_tel);
                        //New
                        $telerik.$(this).find("[name$='cboTelephone']").bind("click", { rowIndex: index }, Click_tel);
                        var priorityText = "<%= priorityText %> " + (index + 1).toString();
                        $telerik.$(this).find("[id$='lblPriority']").html(priorityText);
                        //End

                    })
                }
            }
            //New
            var selectedTelIndex = 0;
            function Click_tel(event) {
                var trs = $telerik.$("#EditTelephone_gdContact_ctl00>tbody>tr");
                var ddl = $telerik.$(trs[event.data.rowIndex]).find("[name$='cboTelephone']");
                selectedTelIndex = ddl[0].selectedIndex;
                return true;
            }

            function SetupEvents_eMail() {
                var trs_email = $telerik.$("#EditeMail_gdContact_ctl00>tbody>tr");
                if (trs_email.length > 0) {
                    trs_email.each(function (index) {
                        $telerik.$(this).find("[name$='btnDeleMail']").unbind("click");
                        $telerik.$(this).find("[name$='btnDeleMail']").bind("click", { rowIndex: index }, DeleRow_eMail);
                    })
                }
            }
            //End
            $telerik.$(document).ready(function () {
                SetupEvents_Telephone();

            });

            function onDropDownOpening(sender) {
                var cboEmails = $find("<%= cboEmails.ClientID %>");
                var items = cboEmails.get_items();
                if (items.get_count() > 0) {
                    if (items.getItem(0).get_value() == "-1") {
                        $telerik.$(items.getItem(0).get_element()).find("[id$='chkEmail']").css("visibility", "hidden");
                    }
                }
            }

            function Save_Click(validationGroup) {
                $telerik.$("#<%= lblMessage.ClientID%>").text("");
                if (!Page_ClientValidate(validationGroup)) {
                    Page_BlockSubmit = false;
                    return false; //not valid return false
                }
                CreateSaveParameters();
                return false;
            }

            function CreateSaveParameters() {
                var ContactCommunicationIDs = '';
                $telerik.$("#EditTelephone_gdContact_ctl00>tbody>tr").find("[name$='cboTelephone']").each(function () {
                    if (this.selectedIndex > 0 && $telerik.$(this).val() != '-1' && $telerik.$.trim($telerik.$(this).val()) != '' ) {
                        if (ContactCommunicationIDs == '') ContactCommunicationIDs = $telerik.$.trim($telerik.$(this).val());
                        else ContactCommunicationIDs = ContactCommunicationIDs + "," + $telerik.$.trim($telerik.$(this).val());
                    }
                });

                $telerik.$("#EditeMail_gdContact_ctl00>tbody>tr").find("[id$='hidcontactCommunicationID']").each(function () {
                    if ($telerik.$.trim($telerik.$(this).val()) != '') {
                        if (ContactCommunicationIDs == '') ContactCommunicationIDs = $telerik.$.trim($telerik.$(this).val());
                        else ContactCommunicationIDs = ContactCommunicationIDs + "," + $telerik.$.trim($telerik.$(this).val());
                    }
                });

                var planName = $telerik.$.trim($telerik.$("#<%= txtContactPlanName.ClientID %>").val());

                var postData = "";
                var ajxUrl = "";
                var c = "\\" + "'";
                postData = "{'ContactPlanName':'" + planName.replace("'", c) +
                              "', 'ContactCommunicationIDs':'" + ContactCommunicationIDs + "'}";
                ajxUrl = "frmEditContactPlan.aspx/ContactPlan_Add";
                $find("<%= LoadingPanel1.ClientID %>").show("<%= pnl.ClientID %>");
                $telerik.$.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: ajxUrl,
                    data: postData,
                    dataType: "json",
                    success: function (data) {
                        if (data.d == '1') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");

                            $telerik.$("#<%= lblMessage.ClientID%>").text("<%= succeedsave %>");
                            $telerik.$("#<%= lblMessage.ClientID%>").css("color", "green");
                            ResetFormValues();
                        }

                        if (data.d == '-1') {
                            top.document.all('TopFrame').cols = '0,*';
                            window.open('../../Login.aspx', '_top')
                        }
                        if (data.d == '0') {
                            $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                            $telerik.$("#<%= lblMessage.ClientID%>").text("<%= saveError %>");
                            $telerik.$("#<%= lblMessage.ClientID%>").css("color", "red");
                        }

                    },
                    error: function (request, status, error) {
                        $find("<%= LoadingPanel1.ClientID %>").hide("<%= pnl.ClientID %>");
                        $telerik.$("#<%= lblMessage.ClientID%>").text("<%= saveError %>");
                        $telerik.$("#<%= lblMessage.ClientID%>").css("color", "red");
                        //alert(request.responseText);
                    }

                });
            }

            function ResetFormValues() {
                $telerik.$("#<%= txtContactPlanName.ClientID %>").val("");
                var trs_tel = $telerik.$("#EditTelephone_gdContact_ctl00>tbody>tr");
                for (var index = trs_tel.length - 1; index > 0; index--) {
                    $telerik.$(trs_tel[index]).remove();
                }
                ResetLastEmptyRow(trs_tel[0]);

                var trs_email = $telerik.$("#EditeMail_gdContact_ctl00>tbody>tr");
                for (var index = trs_email.length - 1; index > 0; index--) {
                    $telerik.$(trs_email[index]).remove();
                }
                DeleteLastEmailRow(trs_email[index]);
            }
        </script>
    </telerik:RadCodeBlock>
    </form>
</body>
</html>
