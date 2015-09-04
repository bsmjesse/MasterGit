<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditContact.ascx.cs" Inherits="SentinelFM.Configuration_Contact_EditContact" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Hay" meta:resourcekey="LoadingPanel1Resource1">
</telerik:RadAjaxLoadingPanel>
<asp:Panel ID="pnl" runat="server" HorizontalAlign="Center">
    <table>
        <tr valign="top">
            <td style="width: 100px">
                <asp:Label ID="lblCompany" runat="server" CssClass="RegularText" meta:resourcekey="lblFirstNameResource1"
                    Text="Company Name:"></asp:Label>
            </td>
            <td colspan="1" align="left">
                <asp:TextBox ID="txtCompany" runat="server" Width="230px" meta:resourcekey="txtCompanyResource1"
                    MaxLength="50" CssClass="RegularText"></asp:TextBox>
            </td>
            <td colspan="2">
                <asp:CheckBox ID="chkIsCompany" runat="server" Text="Company/Organization" 
                    meta:resourcekey="chkIsCompanyResource1"  CssClass="RegularText" />
            </td>
        </tr>
        <tr valign="top">
            <td align="left" style="width: 100px">
                <asp:Label ID="lblFirstName" runat="server" CssClass="RegularText" meta:resourcekey="lblFirstNameResource1"
                    Text="First Name:"></asp:Label>
            </td>
            <td align="left">
                <asp:TextBox ID="txtFirstName" runat="server" Width="230px" meta:resourcekey="txtFirstNameResource1"
                    CssClass="RegularText" MaxLength="50"></asp:TextBox>
            </td>
            <td style="width: 100px" align="left">
                <asp:Label ID="lblMiddleName" runat="server" CssClass="RegularText" meta:resourcekey="lblMiddleNameResource1"
                    Text="Middle Name:"></asp:Label>
            </td>
            <td align="left">
                <asp:TextBox ID="txtMiddleName" runat="server" Width="230px" meta:resourcekey="txtMiddleNameResource1"
                    CssClass="RegularText" MaxLength="50"></asp:TextBox>
            </td>
        </tr>
        <tr valign="top">
            <td align="left" style="width: 100px">
                <asp:Label ID="lblLastName" runat="server" CssClass="RegularText" meta:resourcekey="lblLastNameResource1"
                    Text="Last Name:"></asp:Label>
            </td>
            <td align="left">
                <asp:TextBox ID="txtLastName" runat="server" Width="230px" meta:resourcekey="txtLastNameResource1"
                    CssClass="RegularText" MaxLength="50"></asp:TextBox>
            </td>
            <td  style="width: 100px" align="left">
                
            </td>
            <td class="formtext" style="width: 224px; height: 16px">
            </td>
        </tr>
        <tr>
            <td style="width: 100px" align="left">
                <asp:Label ID="lblTimeZone" runat="server"  CssClass="RegularText" Text=" Time Zone:" meta:resourcekey="lblTimeZoneResource1"></asp:Label>
            </td>
            <td  colspan="3" align="left">
                <telerik:RadComboBox ID="cboTimeZone" runat="server" Width="400px" Skin="Hay" 
                    meta:resourcekey="cboTimeZoneResource1" Filter="Contains" MarkFirstMatch="true" 
                    Height="200px">
                </telerik:RadComboBox>

            </td>
        </tr>
        <tr>
            <td style="width: 100px">
            </td>
            <td style="width: 250px">
            </td>
            <td style="width: 100px">
            </td>
            <td style="width: 250px">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <fieldset>
                    <legend style="color: green" runat="server" id="legendID"></legend>
                    <table cellspacing="0" border="0" id="EditContact1_gdContact_ctl00">
                        <tbody>
                            <tr valign="top">
                                <td align="left" style="width: 200px;">
                                    <asp:DropDownList ID="ddlTypeName" runat="server" Width="200px" DataTextField="CommunicationTypeName"
                                        DataValueField="CommunicationTypeId" Font-Bold="true">
                                    </asp:DropDownList>
                                </td>
                                <td align="left" style="width: 400px;">
                                    <asp:TextBox ID="txtTypedata" runat="server" MaxLength="100" Width="390px" BackColor="#dee0c8"></asp:TextBox>
                                </td>
                                <td align="left" style="width: 15px;">
                                    <asp:ImageButton ID="btnDel" runat="server" ImageUrl="~/images/No.gif" ToolTip="Delete"
                                        meta:resourcekey="btnDelResource1" Width="12px" Height="12px" />
                                    <asp:HiddenField ID="hidID" runat="server" />
                                    <asp:HiddenField ID="hidIsUsed" runat="server" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="center">
                <asp:Button ID="btnSave" runat="server" Text="Save" meta:resourcekey="btnDelResource1" CssClass="combutton"
                    ValidationGroup="vgAdd" OnClientClick="javascript:return Save_Click('vgAdd');" />
                <asp:CustomValidator ID="cvContact" runat="server" ClientValidationFunction="CustomValidateContact"
                    ValidationGroup="vgAdd" Text="" meta:resourcekey="cvContact" />
                <asp:Button ID="btnClose" runat="server" meta:resourcekey="btnNoResource1" Text="Close" CssClass="combutton"
                    OnClientClick="javascript:return RefreshandCloseEdit(false);" />
            </td>
        </tr>
        <tr>
            <td colspan="4" align="center">
                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Visible="true" meta:resourcekey="lblMessageResource1"></asp:Label>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hidContactInfoId" runat="server" />
    <asp:HiddenField ID="hidDeletedID" runat="server" />
</asp:Panel>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
        function SetEditValues(company, firstName, middleName, lastName, timezone, typeData, contactInfoId, isCompany, isFirst) {

            if (isFirst) {
                $telerik.$("#<%= lblMessage.ClientID%>").text("");
                $telerik.$("span[title='errorEmailSpan']").remove();
            }
            $telerik.$("#<%= hidDeletedID.ClientID%>").val("");
            $telerik.$("#<%= txtCompany.ClientID %>").val(company);
            $telerik.$("#<%= txtFirstName.ClientID %>").val(firstName);
            $telerik.$("#<%= txtLastName.ClientID %>").val(lastName);
            $telerik.$("#<%= txtMiddleName.ClientID %>").val(middleName);
            var cboTimeZone = $find("<%= cboTimeZone.ClientID %>");
            var timezoneItem = cboTimeZone.findItemByValue(timezone);
            if (timezoneItem) timezoneItem.select();
            else {
                timezoneItem = cboTimeZone.findItemByValue("<%= cboTimeZoneIninId  %>");
                if (timezoneItem) timezoneItem.select();
            }
            $telerik.$("#<%= hidContactInfoId.ClientID %>").val(contactInfoId);
            var trs = $telerik.$("#EditContact1_gdContact_ctl00>tbody>tr");
            for (var index = trs.length - 1; index > 0; index--) {
                $telerik.$(trs[index]).remove();
            }
            ResetLastEmptyRow(trs[0]);
            if (typeData != null) {
                var commus = eval(typeData);
                if (commus != null && commus.length > 0) {
                    for (var index = 0; index < commus.length; index++) { //Set contact information
                        var cloneLine = $telerik.$(trs[0]).clone(false);
                        //Disable if it is used and it is email and emergencyPhone
                        if (commus[index][3] == "1" && (commus[index][1] == "<%= emailId%>" ||
                                                 commus[index][1] == "<%= emergencyPhoneId%>")) {
                            $telerik.$(cloneLine).find("[name$='ddlTypeName']").bind("click", function () {
                                if ($telerik.$(this).val() == "<%= emailId%>") alert("<%= changeInfo_email %>");
                                else alert("<%= changeInfo_tel %>");

                                return false;
                            });
                        }

                         $telerik.$(cloneLine).find("[name$='ddlTypeName'] option").filter(
                             function () {
                                 return $telerik.$(this).val() == commus[index][1];
                             }).attr('selected', true);

                             if (commus[index][1] == "<%= emergencyPhoneId%>")
                                 $telerik.$(cloneLine).find("[name$='txtTypedata']").addClass("emergencyStyle");
                             else
                                 $telerik.$(cloneLine).find("[name$='txtTypedata']").removeClass("emergencyStyle");
                             $telerik.$($telerik.$(cloneLine).find("[name$='txtTypedata']")[0]).val(commus[index][2]);
                             $telerik.$($telerik.$(cloneLine).find("input:hidden[name$='hidID']")[0]).val(commus[index][0]);
                             $telerik.$($telerik.$(cloneLine).find("input:hidden[name$='hidIsUsed']")[0]).val(commus[index][3]);
                        $telerik.$(cloneLine).insertBefore($telerik.$(trs[0]));
                    }
                }
            }
            if (isCompany.toString().toLowerCase() == "true") {
                $telerik.$("#<%= chkIsCompany.ClientID%>").attr('checked', true);
            }
            else $telerik.$("#<%= chkIsCompany.ClientID%>").attr('checked', false);
            CheckIsCompany();
            SetupEvents();
        }

        function ResetLastEmptyRow(row) {
            $telerik.$(row).find("span[title='errorEmailSpan']").remove();
            $telerik.$(row).find("[name$='ddlTypeName']")[0].selectedIndex = 0;
            $telerik.$($telerik.$(row).find("[name$='txtTypedata']")[0]).val("");
            $telerik.$(row).find("[name$='ddlTypeName']").unbind("click");
            $telerik.$($telerik.$(row).find("input:hidden[name$='hidID']")[0]).val("");
            $telerik.$($telerik.$(row).find("input:hidden[name$='hidIsUsed']")[0]).val("");
            $telerik.$($telerik.$(row).find("[name$='txtTypedata']")[0]).removeClass("emergencyStyle");

        }

        function CustomValidateContact(sender, args) {

            args.IsValid = true;
            $telerik.$("span[title='errorEmailSpan']").remove();
            var company = "";
            var firstName = "";
            //var middleName = "";
            var lastName = "";

            firstName = $telerik.$.trim($telerik.$("#<%= txtFirstName.ClientID %>").val());
            lastName = $telerik.$.trim($telerik.$("#<%= txtLastName.ClientID %>").val());
            //middleName = $telerik.$.trim($telerik.$("#<%= txtMiddleName.ClientID %>").val());
            company = $telerik.$.trim($telerik.$("#<%= txtCompany.ClientID %>").val());

            if ($telerik.$("#<%= chkIsCompany.ClientID%>").attr('checked') && company == "") {
                $telerik.$("#<%= txtCompany.ClientID %>").parent().append("<span title='errorEmailSpan'> <font color='red'> <br /><%= companynameRequired %></font></span>");
                args.IsValid = false;
            }
            if ($telerik.$("#<%= chkIsCompany.ClientID%>").attr('checked') == false && firstName == "" && lastName == "") {
                $telerik.$("#<%= txtFirstName.ClientID %>").parent().append("<span title='errorEmailSpan'> <font color='red'><br /> <%= nameRequired %></font></span>");
                args.IsValid = false;
            }

            $telerik.$("#EditContact1_gdContact_ctl00>tbody>tr").each(function () {
                //Check emal
                if (parseInt($telerik.$(this).find("[name$='ddlTypeName']").val()) > 0) {
                    var selectedText = $telerik.$(this).find("[name$='ddlTypeName']").find('option').filter(':selected').text();
                    var inputText = $telerik.$.trim($telerik.$(this).find("[name$='txtTypedata']").val());
                    if (selectedText.toLowerCase() == "email" && inputText != '') { //validate email address
                        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
                        if (reg.test(inputText) == false) {
                            $telerik.$(this).find("[name$='txtTypedata']").parent().append("<span title='errorEmailSpan'> <font color='red'> <br /><%= errorEmail %></font></span>");
                            args.IsValid = false;
                        }
                    }
                }
                else {
                    var inputText = $telerik.$.trim($telerik.$(this).find("[name$='txtTypedata']").val());

                    if (inputText != "") {
                        $telerik.$(this).find("[name$='txtTypedata']").parent().append("<span title='errorEmailSpan'> <font color='red'> <br /><%= errorSelectType %></font></span>");
                        args.IsValid = false;
                    }
                }
            });
        }

        function AddNewRow() {
            var trs = $telerik.$("#EditContact1_gdContact_ctl00>tbody>tr");
            if (trs.length > 0) { //Add new row if it is last row
                $telerik.$("#EditContact1_gdContact_ctl00>tbody:last").append($telerik.$(trs[trs.length - 1]).clone(false));
                trs = $telerik.$("#EditContact1_gdContact_ctl00>tbody>tr");
                SetupEvents();
                ResetLastEmptyRow(trs[trs.length - 1]);
            }
        }

        function SelectedIndxChanged(event) {
            var trs = $telerik.$("#EditContact1_gdContact_ctl00>tbody>tr");
            if ($telerik.$(trs[event.data.rowIndex]).find("[name$='ddlTypeName']").val() == '<%= emergencyPhoneId %>') {
                $telerik.$(trs[event.data.rowIndex]).find("[name$='txtTypedata']").addClass("emergencyStyle");
            }
            else $telerik.$(trs[event.data.rowIndex]).find("[name$='txtTypedata']").removeClass("emergencyStyle");
            if (trs.length - 1 == event.data.rowIndex) {
                AddNewRow();
            }
            CheckSelectEmergency(trs);
        }

        //Check if Emergency Phone has been selected, if yes then disable
        function CheckSelectEmergency() {
            var emergency = $telerik.$("#EditContact1_gdContact_ctl00>tbody").find("[name$='ddlTypeName']").find("option:selected[value='<%= emergencyPhoneId %>']");
            if (emergency.length > 0) {
                $telerik.$("#EditContact1_gdContact_ctl00>tbody").find("[name$='ddlTypeName']").each(function () {
                    if ($telerik.$(this).val() != '<%= emergencyPhoneId %>') {
                        $telerik.$(this).find("option[value='<%= emergencyPhoneId %>']").attr("disabled", "disabled");
                    }
                })
            }
            else $telerik.$("#EditContact1_gdContact_ctl00>tbody").find("[name$='ddlTypeName']").find("option[value='<%= emergencyPhoneId %>']").attr("disabled", "");
        }

        function DeleRow(event) {

            var trs = $telerik.$("#EditContact1_gdContact_ctl00>tbody>tr");
            if (trs.length - 1 > event.data.rowIndex) {
                var hidID = $telerik.$($telerik.$(trs[event.data.rowIndex]).find("input:hidden[name$='hidID']")[0]).val();
                hidID = $telerik.$.trim(hidID);
                var isUsed = $telerik.$($telerik.$(trs[event.data.rowIndex]).find("input:hidden[name$='hidIsUsed']")[0]).val();
                if (hidID != '') {
                    if ($telerik.$.trim(isUsed) == '1') {
                        var typeCtl = $telerik.$(trs[event.data.rowIndex]).find("[name$='ddlTypeName']");
                        var answer;
                        if ($telerik.$(typeCtl).val() == "<%= emailId%>") answer = confirm("<%= deleteInfo_email %>");
                        else answer = confirm("<%= deleteInfo_tel %>");
                        if (!answer) return false
                    }
                    var deletedId = $telerik.$("#<%= hidDeletedID.ClientID%>").val();
                    if ($telerik.$.trim(deletedId) == '' ) deletedId = hidID
                    else deletedId = deletedId + "," + hidID
                    $telerik.$("#<%= hidDeletedID.ClientID%>").val(deletedId);
                }
                $telerik.$(trs[event.data.rowIndex]).remove();
                SetupEvents();
            }
            return false;
        }

        function SetupEvents() {
            var trs = $telerik.$("#EditContact1_gdContact_ctl00>tbody>tr");
            if (trs.length > 0) {
                trs.each(function (index) {
                    $telerik.$(this).find("[name$='ddlTypeName']").unbind('change');
                    $telerik.$(this).find("[name$='txtTypedata']").unbind('change');
                    $telerik.$(this).find("[name$='btnDel']").unbind("click");
                    if (index == trs.length - 1) {
                        $telerik.$(this).find("[name$='txtTypedata']").change(AddNewRow);
                        $telerik.$(this).find("[name$='btnDel']").hide();
                    }
                    else {
                        $telerik.$(this).find("[name$='btnDel']").show();
                    }
                    $telerik.$(this).find("[name$='btnDel']").bind("click", { rowIndex: index }, DeleRow);
                    $telerik.$(this).find("[name$='ddlTypeName']").bind("change", { rowIndex: index }, SelectedIndxChanged);
                })
            }
            CheckSelectEmergency();
        }

        function CheckIsCompany() {
            if ($telerik.$("#<%= chkIsCompany.ClientID%>").attr('checked')) {
                $telerik.$("#<%= txtCompany.ClientID%>").attr("disabled", "");
                $telerik.$("#<%= txtFirstName.ClientID%>").attr("disabled", "disabled");
                $telerik.$("#<%= txtLastName.ClientID%>").attr("disabled", "disabled");
                $telerik.$("#<%= txtMiddleName.ClientID%>").attr("disabled", "disabled");
            }
            else {
                $telerik.$("#<%= txtCompany.ClientID%>").attr("disabled", "disabled");
                $telerik.$("#<%= txtFirstName.ClientID%>").attr("disabled", "");
                $telerik.$("#<%= txtLastName.ClientID%>").attr("disabled", "");
                $telerik.$("#<%= txtMiddleName.ClientID%>").attr("disabled", "");
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
            var company = "";
            var firstName = "";
            var middleName = "";
            var lastName = "";
            var timeZone = "";
            var contacts = "";
            var contactInfoId = ""
            var isCompany = false;
            var combo = $find("<%= cboTimeZone.ClientID %>");
            if (combo.get_selectedIndex() >= 0) {
                if (combo.get_selectedItem().get_value()) {
                    timeZone = combo.get_selectedItem().get_value();
                }
            }
            firstName = $telerik.$.trim($telerik.$("#<%= txtFirstName.ClientID %>").val());
            lastName = $telerik.$.trim($telerik.$("#<%= txtLastName.ClientID %>").val());
            middleName = $telerik.$.trim($telerik.$("#<%= txtMiddleName.ClientID %>").val());
            company = $telerik.$.trim($telerik.$("#<%= txtCompany.ClientID %>").val());
            contactInfoId = $telerik.$.trim($telerik.$("#<%= hidContactInfoId.ClientID %>").val());
            if ($telerik.$("#<%= chkIsCompany.ClientID%>").attr('checked')) isCompany = true;
            var trs = $telerik.$("#EditContact1_gdContact_ctl00>tbody>tr");
            var c = "\\" + "'";
            var isSelected = false;
            if (trs.length > 0) {
                trs.each(function (index) {
                    var typeCtl = $telerik.$(this).find("[name$='ddlTypeName']");
                    var txtCtl = $telerik.$(this).find("[name$='txtTypedata']");
                    var hidID = $telerik.$(this).find("input:hidden[name$='hidID']");
                    if (parseInt(typeCtl.val()) > 0 && $telerik.$.trim(txtCtl.val()) != '') {
                        if (typeCtl.val() == '<%= emergencyPhoneId %>') {
                            isSelected = true;
                        }
                        if (contactInfoId == "") {
                            if (contacts == '') {
                                contacts = "{'TypeId':'" + typeCtl.val() +
                                 "', 'TypeData':'" + $telerik.$.trim(txtCtl.val()).replace("'", c) + "'}";
                            }
                            else {
                                contacts = contacts + ",{'TypeId':'" + typeCtl.val() +
                                 "', 'TypeData':'" + $telerik.$.trim(txtCtl.val()).replace("'", c) + "'}";

                            }
                        }
                        else {
                            if (contacts == '') {
                                contacts = "{'Id':'" + $telerik.$.trim(hidID.val()) +
                                 "', 'TypeId':'" + typeCtl.val() +
                                 "', 'TypeData':'" + $telerik.$.trim(txtCtl.val()).replace("'", c) + "'}";
                            }
                            else {
                                contacts = contacts + ",{'Id':'" + $telerik.$.trim(hidID.val()) +
                                 "', 'TypeId':'" + typeCtl.val() +
                                 "', 'TypeData':'" + $telerik.$.trim(txtCtl.val()).replace("'", c) + "'}";

                            }

                        }
                    }
                });
            }
            if (!isSelected)
                if (!confirm("<%= saveMessage%>")) return;
            contacts = "[" + contacts + "]";
            var postData = "";
            var ajxUrl = "";
            if (contactInfoId != "") {
                var deletedId = $telerik.$("#<%= hidDeletedID.ClientID%>").val();
                postData = "{'ContactInfoId':'" + contactInfoId +
                              "', 'isCompany':'" + isCompany +
                              "', 'Company':'" + company.replace("'", c) +
                              "', 'FirstName':'" + firstName.replace("'", c) +
                              "', 'MiddleName':'" + middleName.replace("'", c) +
                              "', 'LastName':'" + lastName.replace("'", c) +
                              "', 'TimeZone':'" + timeZone +
                              "', 'DeletedIds':'" + deletedId + 
                              "', 'Contacts':" + contacts + "}";
                ajxUrl = "frmContacts.aspx/ContactInfo_Update";
            }
            else {
                postData = "{'isCompany':'" + isCompany +
                              "', 'Company':'" + company.replace("'", c) +
                              "', 'FirstName':'" + firstName.replace("'", c) +
                              "', 'MiddleName':'" + middleName.replace("'", c) +
                              "', 'LastName':'" + lastName.replace("'", c) +
                              "', 'TimeZone':'" + timeZone +
                              "', 'Contacts':" + contacts + "}";
                ajxUrl = "frmContacts.aspx/ContactInfo_Add";
            }
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

                        if (contactInfoId == "") {
                            $telerik.$("#<%= lblMessage.ClientID%>").text("<%= succeedsave %>");
                            $telerik.$("#<%= lblMessage.ClientID%>").css("color", "green");

                            SetEditValues("", "", "", "", 0, null, "", isCompany, false);
                        }
                        else {
                            alert("<%= succeedsave %>");
                            RefreshandCloseEdit(true);
                        }
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

        function RefreshandCloseEdit(isSave) {
            CLoseEdit();
            var contactInfoId = $telerik.$.trim($telerik.$("#<%= hidContactInfoId.ClientID %>").val());
            //refresh grid after add new 
            //if (contactInfoId == "") refreshVehicleContactsGrid("Rebind");
            if (isSave)
                refreshVehicleContactsGrid("Rebind");
            else {
                if (contactInfoId == "") refreshVehicleContactsGrid("Rebind");
            }
            return false;
        }
    </script>
</telerik:RadCodeBlock>
