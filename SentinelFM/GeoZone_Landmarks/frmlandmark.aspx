<%@ Page Language="c#" Inherits="SentinelFM.frmLandmark" CodeFile="frmLandmark.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Src="Components/ctlGeozoneLandmarksMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<!doctype html>
<html>
<head id="Head1" runat="server">
    <title>Landmark</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet">-->
    <%--<link rel="stylesheet" href="http://code.jquery.com/ui/1.10.2/themes/smoothness/jquery-ui.css" />--%>
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.2/themes/smoothness/jquery-ui.css" />

    <%--<script type="text/javascript" src="../Scripts/jquery-1.4.1.js" language="javascript"></script>--%>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
    <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.2/jquery-ui.min.js"></script>

    <style type="text/css">
        #tblShowMap {
            width: 243px;
        }

        .style1 {
            width: 94px;
        }

        .style2 {
            width: 178px;
        }

        .style3 {
            height: 21px;
            width: 62px;
        }

        .style4 {
            width: 62px;
        }

        .style5 {
            height: 9px;
            width: 62px;
        }

        .ui-dialog {
            z-index: 1000;
        }

        .ui-widget-header {
            font-size: 11px !important;
        }
    </style>

    <script type="text/javascript">
        function assignLandmarkToFleet(landmarkId) {
            var url = "../Widgets/FleetAssignment.aspx?objectName=landmark&objectId=" + landmarkId;
            $("#dlgFleetAssignment").html("<iframe width='100%' height='100%' frameborder='0' scrolling='no' src='" + url + "' ></iframe>").dialog({ width: 520, height: 350, title: 'Assign to fleet', position: { my: "center center", at: "center center", of: window }, resizable: false });
        }

        function closeFleetAssignment() {
            $("#dlgFleetAssignment").dialog("close");
        }

        function clickButton(e, buttonid) {
            var evt = e ? e : window.event;
            var bt = document.getElementById(buttonid);
            if (bt) {
                if (evt.keyCode == 13) {
                    bt.click();
                    return false;
                }
            }
        }
    </script>

</head>
<body>
    <form id="frmLandmarkForm" method="post" runat="server">
        <table id="tblCommands" style="z-index: 101; left: 8px; position: absolute; top: 4px"
            cellspacing="0" cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdLandmark" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="679" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" height="550px" width="990px" class="table" border="0">
                                    <tr>
                                        <td class="configTabBackground" style="vertical-align:top;">
                                            <table id="Table1" cellspacing="0" cellpadding="0" width="990" align="center" border="0">
                                                <tr>
                                                    <td class="tableheading" align="left" height="5"></td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table id="tblLandmarks" style="width: 990px;" cellpadding="0" width="720" border="0" runat="server">
                                                            <tr>
                                                                <td align="center">

                                                                    <table id="tblSearch" runat="server" style="width: 782px;">
                                                                        <tr>
                                                                            <td>
                                                                                <asp:DropDownList ID="cboSearchType" runat="server" AutoPostBack="True" CssClass="RegularText" Width="200px" meta:resourcekey="cboSearchTypeResource1">
                                                                                    <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource5">Landmark Name</asp:ListItem>
                                                                                    <asp:ListItem Value="1" meta:resourcekey="ListItemResource6">Description</asp:ListItem>
                                                                                    <asp:ListItem Value="2" meta:resourcekey="ListItemResource7">Address</asp:ListItem>

                                                                                </asp:DropDownList>
                                                                            </td>
                                                                            <td style="width: 183px">
                                                                                <asp:TextBox ID="txtSearchParam" runat="server" CssClass="formtext"
                                                                                    Width="163px" meta:resourcekey="txtSearchParamResource1" onkeypress="return clickButton(event,'cmdSearch')"></asp:TextBox></td>
                                                                            <td style="width: 183px">
                                                                                <asp:Button ID="cmdSearch" runat="server" CssClass="combutton"
                                                                                    OnClick="cmdSearch_Click" Text="Search" Width="163px" meta:resourcekey="cmdSearchResource1" /></td>
                                                                            <td style="width: 183px">
                                                                                <asp:Button ID="cmdClear" runat="server" CssClass="combutton"
                                                                                    OnClick="cmdClear_Click" Text="Clear" Width="124px" meta:resourcekey="cmdClearResource1" /></td>
                                                                            <td style="color:Black;width:200px;white-space:nowrap;">
                                                                                &nbsp;&nbsp;&nbsp;
                                                                                <asp:LinkButton ID="lnkBtnOpenManageCategory" runat="server" style="font-family: verdana;font-size: 11px;font-weight: normal;"
                                                                                    OnClientClick="javascript:window.open('frmManageLandmarkCategory.aspx', '_blank', 'location=no,height=480,width=500,top=200,left=200,scrollbars=yes,status=no');return false;"
                                                                                    Text="Manage Category" meta:resourcekey="lnkBtnOpenManageCategoryResource1"></asp:LinkButton>

                                                                            </td>
                                                                            <td style="color:Black;width:200px;white-space:nowrap;">
	                                                                            &nbsp;&nbsp;&nbsp;
	                                                                            <asp:LinkButton ID="lnkBtnOpenLandmarkImport" runat="server" style="font-family: verdana;font-size: 11px;font-weight: normal;"
		                                                                            OnClientClick="javascript:window.open('frmLandmarkImport.aspx', '_blank', 'location=no,height=480,width=800,top=200,left=100,scrollbars=yes,status=no');return false;"
		                                                                            Text="Landmark Import"></asp:LinkButton>

                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:DataGrid ID="dgLandmarks" runat="server" Width="100%" GridLines="None" CellPadding="3"
                                                                        BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White" PageSize="13"
                                                                        AllowPaging="True" DataKeyField="LandmarkName" AutoGenerateColumns="False" BorderStyle="Ridge"
                                                                        OnDeleteCommand="dgLandmarks_DeleteCommand" OnItemCommand="dgLandmarks_ItemCommand"
                                                                        OnItemCreated="dgLandmarks_ItemCreated"
                                                                        meta:resourcekey="dgLandmarksResource1"
                                                                        OnItemDataBound="dgLandmarks_ItemDataBound1">
                                                                        <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                        <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                        <AlternatingItemStyle CssClass="gridtext" BackColor="Beige"></AlternatingItemStyle>
                                                                        <ItemStyle CssClass="gridtext" BackColor="White"></ItemStyle>
                                                                        <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                        <Columns>
                                                                            <asp:BoundColumn DataField="LandmarkName" HeaderText='<%$ Resources:dgLandmarks_LandmarkName %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="description" HeaderText='<%$ Resources:dgLandmarks_Description %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn Visible="False" DataField="Latitude" HeaderText='<%$ Resources:dgLandmarks_Latitude %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn Visible="False" DataField="Longitude" HeaderText='<%$ Resources:dgLandmarks_Longitude %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="StreetAddress" HeaderText='<%$ Resources:dgLandmarks_StreetAddress %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="CategoryName" HeaderText='<%$ Resources:dgLandmarks_Category %>'>
	                                                                            <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="Email" HeaderText='<%$ Resources:dgLandmarks_Email %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:BoundColumn DataField="contactPhoneNum" HeaderText='<%$ Resources:dgLandmarks_ContactPhoneNum %>'>
                                                                                <HeaderStyle Wrap="False"></HeaderStyle>
                                                                            </asp:BoundColumn>
                                                                            <asp:TemplateColumn HeaderText='<%$ Resources:dgLandmarks_Radius %>'>
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblRadius" runat="server"></asp:Label>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateColumn>
                                                                            <asp:ButtonColumn Text="&lt;img src=../images/edit.gif border=0&gt;" CommandName="cmdEdit"
                                                                                meta:resourcekey="ButtonColumnResource1"></asp:ButtonColumn>
                                                                            <asp:ButtonColumn Text="&lt;img src=../images/delete.gif border=0&gt;" CommandName="Delete"
                                                                                meta:resourcekey="ButtonColumnResource2"></asp:ButtonColumn>
                                                                            <asp:HyperLinkColumn Text="Map It" DataNavigateUrlField="LandmarkId" DataNavigateUrlFormatString="javascript:var w =LandmarkMap('{0}')"
                                                                                meta:resourcekey="HyperLinkColumnResource1">
                                                                                <ItemStyle Wrap="False" ForeColor="Black" Width="50px"></ItemStyle>
                                                                            </asp:HyperLinkColumn>
                                                                            <asp:BoundColumn Visible="False" DataField="LandmarkId" HeaderText="LandmarkId"></asp:BoundColumn>
                                                                            <asp:TemplateColumn HeaderText='lid' Visible="false">
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lid" Text='<%# DataBinder.Eval(Container.DataItem, "lid")%>'></asp:Label>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateColumn>
                                                                            <asp:BoundColumn Visible="false" DataField="CategoryId" HeaderText='CategoryId'></asp:BoundColumn>
                                                                        </Columns>
                                                                        <PagerStyle Font-Size="11px" HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"
                                                                            Mode="NumericPages"></PagerStyle>
                                                                    </asp:DataGrid></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" height="15"></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="cmdLandMarkAdd" runat="server" Text="Add Landmark" CssClass="combutton"
                                                                        CausesValidation="False" OnClick="cmdLandMarkAdd_Click" meta:resourcekey="cmdLandMarkAddResource1"></asp:Button></td>
                                                            </tr>
                                                        </table>
                                                        <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="270px" Visible="False"
                                                            Height="8px" meta:resourcekey="lblMessageResource1"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td align="center">
                                                        <table id="tblLandmarkAdd" style="width: 670px" cellspacing="0" cellpadding="0" border="0"
                                                            runat="server">
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:RadioButtonList ID="lstAddOptions" runat="server" CssClass="formtext" Width="675px"
                                                                        Height="23px" AutoPostBack="True" RepeatDirection="Horizontal" Font-Bold="True"
                                                                        OnSelectedIndexChanged="lstAddOptions_SelectedIndexChanged" meta:resourcekey="lstAddOptionsResource1">
                                                                        <asp:ListItem Value="0" Selected="True" meta:resourcekey="ListItemResource1" Text="By Street Address"></asp:ListItem>
                                                                        <asp:ListItem Value="1" meta:resourcekey="ListItemResource2" Text="By Coordinates/Map"></asp:ListItem>
                                                                    </asp:RadioButtonList></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center" height="15"></td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <table id="Table8" cellspacing="0" cellpadding="0" width="670" border="0">
                                                                        <tr>
                                                                            <td class="formtext" style="width: 179px; height: 13px;"></td>
                                                                            <td class="formtext" style="height: 13px"></td>
                                                                            <td class="formtext" style="height: 13px"></td>
                                                                            <td class="formtext" style="height: 13px"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" style="height: 22px; width: 179px;">
                                                                                <asp:Label ID="lblLandmarkNameTitle" runat="server" CssClass="formtext" meta:resourcekey="lblLandmarkNameTitleResource1" Text="Landmark Name:"></asp:Label>
                                                                                <asp:RequiredFieldValidator ID="valLandmark" runat="server" ControlToValidate="txtLandmarkName"
                                                                                    ErrorMessage="Please enter a Landmark Name" meta:resourcekey="valLandmarkResource1" Text="*"></asp:RequiredFieldValidator></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:TextBox ID="txtLandmarkName" runat="server" CssClass="formtext" Width="173px"
                                                                                    meta:resourcekey="txtLandmarkNameResource1"></asp:TextBox></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:Label ID="lblContactNameTitle" runat="server" CssClass="formtext" meta:resourcekey="lblContactNameTitleResource1" Text="Contact Name:"></asp:Label></td>
                                                                            <td class="formtext" style="height: 22px">
                                                                                <asp:TextBox ID="txtContactName" runat="server" CssClass="formtext" Width="173px"
                                                                                    meta:resourcekey="txtContactNameResource1"></asp:TextBox></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" style="height: 15px; width: 179px;" height="15"></td>
                                                                            <td class="formtext" style="height: 15px" height="15"></td>
                                                                            <td class="formtext" style="height: 15px" height="15"></td>
                                                                            <td class="formtext" style="height: 15px" height="15"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" style="width: 179px">
                                                                                <asp:Label ID="lblLandmarkDescriptionTitle" runat="server" CssClass="formtext" meta:resourcekey="lblLandmarkDescriptionTitleResource1" Text="Landmark Description:"></asp:Label></td>
                                                                            <td class="formtext">
                                                                                <asp:TextBox ID="txtLandmarkDesc" runat="server" CssClass="formtext" Width="173px"
                                                                                    meta:resourcekey="txtLandmarkDescResource1"></asp:TextBox></td>
                                                                            <td class="formtext">
                                                                                <asp:Label ID="lblPhoneTitle" runat="server" CssClass="formtext" meta:resourcekey="lblPhoneTitleResource1" Text="Phone:"></asp:Label></td>
                                                                            <td class="formtext">
                                                                                <asp:TextBox ID="txtPhone" runat="server" CssClass="formtext" Width="173px" meta:resourcekey="txtPhoneResource1"></asp:TextBox></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" height="15" style="width: 179px"></td>
                                                                            <td class="formtext" height="15"></td>
                                                                            <td class="formtext" height="15"></td>
                                                                            <td class="formtext" height="15"></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" height="15" style="width: 179px">
                                                                                <asp:Label ID="lblRadiusTitle" runat="server" CssClass="formtext" meta:resourcekey="lblRadiusTitleResource1" Text="Radius"></asp:Label>
                                                                                (
                                                                                <asp:Label ID="lblUnit" runat="server" meta:resourcekey="lblUnitResource1"></asp:Label>):
                                                                                <asp:RequiredFieldValidator ID="valRadius" runat="server" ControlToValidate="txtRadius"
                                                                                    ErrorMessage="Please enter a Radius." meta:resourcekey="valRadiusResource1" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                                                                                        ID="valRangeRadius" runat="server" ControlToValidate="txtRadius" ErrorMessage="Please enter a correct radius."
                                                                                        MaximumValue="9999999" MinimumValue="-1" meta:resourcekey="valRangeRadiusResource1" Text="*"></asp:RangeValidator></td>
                                                                            <td class="formtext" height="15">
                                                                                <asp:TextBox ID="txtRadius" runat="server" CssClass="formtext" Width="173px" meta:resourcekey="txtRadiusResource1"></asp:TextBox></td>
                                                                            <td class="formtext" height="15">
                                                                                <asp:Label ID="lblCategoryTitle" runat="server" CssClass="formtext" meta:resourcekey="lblCategoryTitleResource1" Text="Category"></asp:Label>
                                                                                <asp:RequiredFieldValidator  ID="valDDLCategory" InitialValue="0" Display="Dynamic" 
                                                                                        runat="server" ControlToValidate="ddlCategory"
                                                                                        Text="*" ErrorMessage="Please select a category" meta:resourcekey="valDDLCategoryResource1"></asp:RequiredFieldValidator>
                                                                            </td>
                                                                            <td class="formtext" height="15">
                                                                                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="formtext" Width="173px">
                                                                                </asp:DropDownList>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td class="formtext" height="15" style="width: 179px"></td>
                                                                            <td class="formtext" height="15"></td>
                                                                            <td class="formtext" height="15"></td>
                                                                            <td class="formtext" height="15"></td>
                                                                        </tr>
                                                                    </table>
                                                                    <table id="Table2" cellspacing="0" cellpadding="0" width="670" border="0">
                                                                        <tr>
                                                                            <td class="formtext" align="center" height="200">
                                                                                <table class="formtext" id="tblStreet" style="width: 670px;" cellspacing="0"
                                                                                    cellpadding="0" width="670" border="0" runat="server">
                                                                                    <tr>
                                                                                        <td style="width: 179px; height: 32px">
                                                                                            <asp:Label ID="lblStreetTitle" runat="server" meta:resourcekey="lblStreetTitleResource1" Text="Street:"></asp:Label>
                                                                                            <asp:RequiredFieldValidator ID="ValAddress" runat="server" CssClass="formtext" ControlToValidate="txtStreet"
                                                                                                ErrorMessage="Please enter a Street Address" meta:resourcekey="ValAddressResource1" Text="*"></asp:RequiredFieldValidator></td>
                                                                                        <td style="width: 195px; height: 32px">
                                                                                            <asp:TextBox ID="txtStreet" runat="server" CssClass="formtext" Width="173px" TextMode="MultiLine"
                                                                                                OnTextChanged="txtStreet_TextChanged" meta:resourcekey="txtStreetResource1"></asp:TextBox></td>
                                                                                        <td style="width: 99px; height: 32px">
                                                                                            <asp:Label ID="lblCityTitle" runat="server" meta:resourcekey="lblCityTitleResource1" Text="City:"></asp:Label></td>
                                                                                        <td style="height: 32px">
                                                                                            <asp:TextBox ID="txtCity" runat="server" CssClass="formtext" Width="173px" OnTextChanged="txtCity_TextChanged"
                                                                                                meta:resourcekey="txtCityResource1"></asp:TextBox></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td height="15" style="width: 179px"></td>
                                                                                        <td height="15" style="width: 195px"></td>
                                                                                        <td height="15" style="width: 99px"></td>
                                                                                        <td height="15"></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td style="width: 179px">
                                                                                            <asp:Label ID="lblStateProvinceTitle" runat="server" meta:resourcekey="lblStateProvinceTitleResource1" Text="State (Prov):"></asp:Label></td>
                                                                                        <td style="width: 195px">
                                                                                            <asp:TextBox ID="txtState" runat="server" CssClass="formtext" Width="173px" OnTextChanged="txtState_TextChanged"
                                                                                                meta:resourcekey="txtStateResource1" Height="22px"></asp:TextBox></td>
                                                                                        <td style="width: 99px">
                                                                                            <asp:Label ID="lblCountryTitle" runat="server" meta:resourcekey="lblCountryTitleResource1" Text="Country"></asp:Label>:</td>
                                                                                        <td>
                                                                                            <asp:DropDownList ID="cboCountry" runat="server" CssClass="formtext" Width="173px"
                                                                                                OnSelectedIndexChanged="cboCountry_SelectedIndexChanged" meta:resourcekey="cboCountryResource1">
                                                                                                <asp:ListItem Value="USA" Selected="True" meta:resourcekey="ListItemResource3" Text="USA"></asp:ListItem>
                                                                                                <asp:ListItem Value="Canada" meta:resourcekey="ListItemResource4" Text="Canada"></asp:ListItem>
                                                                                            </asp:DropDownList></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td height="15" style="width: 179px"></td>
                                                                                        <td height="15" style="width: 195px"></td>
                                                                                        <td height="15" style="width: 99px"></td>
                                                                                        <td height="15"></td>
                                                                                    </tr>
                                                                                </table>
                                                                                <table id="Table3" style="width: 670px;" cellspacing="0" cellpadding="0"
                                                                                    border="0">
                                                                                    <tr>
                                                                                        <td class="style2" width="25%">
                                                                                            <asp:Label ID="lblX" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblXResource1" Text="Latitude (north is positive):"></asp:Label><asp:RequiredFieldValidator
                                                                                                ID="valX" runat="server" CssClass="formtext" ControlToValidate="txtX" ErrorMessage="Please enter Latitude"
                                                                                                Enabled="False" meta:resourcekey="valXResource1" Text="*"></asp:RequiredFieldValidator><asp:RangeValidator
                                                                                                    ID="valRangLat" runat="server" CssClass="errortext" ControlToValidate="txtX"
                                                                                                    ErrorMessage="Latitude is wrong." MaximumValue="180" MinimumValue="-180" Enabled="False"
                                                                                                    Type="Double" meta:resourcekey="valRangLatResource1" Text="*"></asp:RangeValidator></td>
                                                                                        <td align="left" width="25%">
                                                                                            <asp:TextBox ID="txtY" runat="server" CssClass="formtext" Visible="False"
                                                                                                name="txtY" meta:resourcekey="txtYResource1" Width="173px"></asp:TextBox></td>
                                                                                        <td class="style1" width="25%">
                                                                                            <asp:Label ID="lblY" runat="server" CssClass="formtext" Visible="False" meta:resourcekey="lblYResource1" Text="Longitude (west is negative):"></asp:Label><asp:RangeValidator
                                                                                                ID="valRangeLong" runat="server" CssClass="errortext" ControlToValidate="txtY"
                                                                                                ErrorMessage="Longitude is wrong." MaximumValue="90" MinimumValue="-90" Enabled="False"
                                                                                                Type="Double" meta:resourcekey="valRangeLongResource1" Text="*"></asp:RangeValidator><asp:RequiredFieldValidator
                                                                                                    ID="valY" runat="server" CssClass="formtext" ControlToValidate="txtY" ErrorMessage="Please enter Longitude"
                                                                                                    Enabled="False" meta:resourcekey="valYResource1" Text="*"></asp:RequiredFieldValidator>
                                                                                        </td>
                                                                                        <td width="25%">
                                                                                            <asp:TextBox ID="txtX" runat="server" CssClass="formtext" Visible="False"
                                                                                                name="txtX" meta:resourcekey="txtXResource1" Width="173px"></asp:TextBox>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan="2" width="50%">
                                                                                            <table>
                                                                                                <tr id="trTimer" runat="server" visible="false">
                                                                                                    <td style="height: 40px;">
                                                                                                        <asp:Label ID="Label2" runat="server" Text="Timeout:" CssClass="formtext"></asp:Label>
                                                                                                    </td>
                                                                                                    <td style="height: 40px;">
                                                                                                        <asp:DropDownList ID="cboServices" runat="server" CssClass="RegularText"
                                                                                                            DataTextField="RulesApplied" DataValueField="ServiceConfigId" AutoPostBack="False" meta:resourcekey="cboFleetResource6">
                                                                                                        </asp:DropDownList>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                        <td colspan="2">
                                                                                            <table>
                                                                                                <tr id="trAssignment" runat="server" visible="false">
                                                                                                    <td style="height: 40px;">
                                                                                                        <asp:Label ID="Label1" runat="server" Text="Assign to fleet:" CssClass="formtext"></asp:Label>
                                                                                                    </td>
                                                                                                    <td style="height: 40px;">
                                                                                                        <asp:DropDownList ID="cboFleet" runat="server" CssClass="RegularText"
                                                                                                            DataTextField="FleetName" DataValueField="FleetId" AutoPostBack="True" meta:resourcekey="cboFleetResource6">
                                                                                                        </asp:DropDownList>
                                                                                                    </td>
                                                                                                </tr>
                                                                                                <tr id="trAssignmentPopup" runat="server" visible="false" class="formtext">
                                                                                                    <td style="height: 40px;" colspan="2" align="left">
                                                                                                        <a href='javascript:void(0)' onclick='assignLandmarkToFleet(<%=LandmarkId %>);' id="lnkAssignToFleet">Assign to fleet</a>
                                                                                                    </td>
                                                                                                </tr>
                                                                                            </table>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan="4" align="center">
                                                                                            <asp:RadioButtonList ID="LandmarkOptions" runat="server" CssClass="formtext"
                                                                                                Width="250px" BorderWidth="0px" Enabled="false"
                                                                                                RepeatDirection="Horizontal">
                                                                                                <asp:ListItem Value="0" Selected="True" Text="Circle" meta:resourcekey="ListItemType"></asp:ListItem>
                                                                                                <asp:ListItem Value="1"
                                                                                                    Text="Rectangle"></asp:ListItem>
                                                                                                <asp:ListItem Value="2" Text="Polygon"></asp:ListItem>
                                                                                            </asp:RadioButtonList>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan="4" align="center">
                                                                                            <asp:RadioButtonList ID="optLandmarkPublicPrivate" runat="server" CssClass="formtext"
                                                                                                BorderWidth="0px"
                                                                                                RepeatDirection="Horizontal">
                                                                                                <asp:ListItem Value="0" Selected="True" Text="Private"></asp:ListItem>
                                                                                                <asp:ListItem Value="1" Text="Public">
                                                                                                </asp:ListItem>
                                                                                            </asp:RadioButtonList>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan="4" align="center">
                                                                                            <asp:DataGrid ID="dgLandmarkCoordinates" Visible="false"
                                                                                                runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="4"
                                                                                                ForeColor="#333333" GridLines="None" BorderColor="#E0E0E0" BorderStyle="Solid"
                                                                                                BorderWidth="1px" meta:resourcekey="dgGeoZonesCoordinatesResource1">
                                                                                                <PagerStyle Font-Size="11px" BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                                                                                <AlternatingItemStyle CssClass="gridtext" BackColor="White" ForeColor="#284775" />
                                                                                                <ItemStyle CssClass="gridtext" BackColor="#F7F6F3" ForeColor="#333333" />
                                                                                                <Columns>
                                                                                                    <asp:BoundColumn DataField="Latitude" HeaderText="Latitude"></asp:BoundColumn>
                                                                                                    <asp:BoundColumn DataField="Longitude" HeaderText="Longitude"></asp:BoundColumn>
                                                                                                </Columns>
                                                                                                <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                                                                                <EditItemStyle BackColor="#999999" />
                                                                                                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                                                                            </asp:DataGrid></td>

                                                                                    </tr>
                                                                                </table>
                                                                                <table id="Table4" style="border-right: 2px ridge; border-top: 2px ridge; border-left: 2px ridge; border-bottom: 2px ridge; padding-left: 10px; padding-right: 10px; padding-top: 5px; padding-bottom: 5px;"
                                                                                    cellspacing="0" cellpadding="2" width="587" border="0">
                                                                                    <tr>
                                                                                        <td class="formtext">
                                                                                            <asp:Label ID="lblEmailTitle" runat="server" CssClass="formtext" meta:resourcekey="lblEmailTitleResource1" Text="Email:"></asp:Label>
                                                                                            <asp:CustomValidator ID="CustomValidator1" runat="server" ClientValidationFunction="CustomValidateDate" ControlToValidate="txtEmail" OnServerValidate="ValidateEmail"
                                                                                                EnableClientScript="false" ErrorMessage="Please enter a correct email address" Display="None"
                                                                                                Text="*" meta:resourcekey="valEmailResource1" />
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:TextBox ID="txtEmail" runat="server" CssClass="formtext" Width="173px" meta:resourcekey="txtEmailResource1"></asp:TextBox></td>
                                                                                        <td class="style4">
                                                                                            <asp:Label ID="lblPhone" runat="server" CssClass="formtext"
                                                                                                Text="Phone:"> Phone:</asp:Label>
                                                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                                                                ControlToValidate="txtPhoneSMS" CssClass="formtext"
                                                                                                ErrorMessage="Invalid Phone Number:"
                                                                                                ValidationExpression="^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$">*</asp:RegularExpressionValidator>
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:TextBox ID="txtPhoneSMS" runat="server" CssClass="formtext" Width="173px"
                                                                                                Enabled="False"></asp:TextBox></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td class="formtext">
                                                                                            <asp:Label ID="lblTimeZoneTitle" runat="server" CssClass="formtext" meta:resourcekey="lblTimeZoneTitleResource1" Text="Time Zone:"></asp:Label>
                                                                                        </td>
                                                                                        <td>
                                                                                            <asp:DropDownList ID="cboTimeZone" runat="server" CssClass="RegularText" Width="168px"
                                                                                                DataTextField="TimeZoneName" DataValueField="TimeZoneId" meta:resourcekey="cboTimeZoneResource1">
                                                                                            </asp:DropDownList></td>
                                                                                        <td class="style4">&nbsp;</td>
                                                                                        <td>&nbsp;</td>
                                                                                    </tr>
                                                                                     <tr>
                                                                                       
                                                                                        <td colspan="4" style="height: 9px">
                                                                                            <asp:CheckBox ID="chkDayLight" runat="server" CssClass="formtext" Text="Automatically adjust for daylight savings time"
                                                                                                meta:resourcekey="chkDayLightResource1"></asp:CheckBox></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan="4">
                                                                                            <asp:Label
                                                                                                ID="lblMultipleEmails" class="formtext" runat="server" meta:resourcekey="lblMultipleEmails" Text="*Multiple email addresses Must be Separated by semicolon or comma"></asp:Label>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <asp:Label ID="lblAddMessage" runat="server" CssClass="errortext" Width="270px" Visible="False"
                                                                        Height="8px" meta:resourcekey="lblAddMessageResource1"></asp:Label><asp:ValidationSummary
                                                                            ID="ValidationSummary1" runat="server" CssClass="errortext" Width="321px" Height="17px"
                                                                            meta:resourcekey="ValidationSummary1Resource1"></asp:ValidationSummary>


                                                                    <table id="tblShowMap" cellspacing="0" cellpadding="0" border="0"
                                                                        runat="server" align="center" width="100%">
                                                                        <tr>
                                                                            <td align="center" width="100%">
                                                                                <a class="link" onclick="ShowMapCall()" href="#">
                                                                                    <asp:Label ID="lblViewMapTitle" runat="server" meta:resourcekey="lblViewMapTitleResource1" Text="View map"></asp:Label></a></td>
                                                                        </tr>
                                                                    </table>

                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <div style="overflow: auto; width: 349px; height: 70px">
                                                                        <asp:DataGrid ID="dgAddress" runat="server" Width="271px" GridLines="None" CellPadding="3"
                                                                            BackColor="White" BorderWidth="2px" CellSpacing="1" BorderColor="White" AutoGenerateColumns="False"
                                                                            BorderStyle="Ridge" OnSelectedIndexChanged="dgAddress_SelectedIndexChanged" meta:resourcekey="dgAddressResource1">
                                                                            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="SlateGray"></SelectedItemStyle>
                                                                            <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                                            <ItemStyle Font-Size="11px" ForeColor="Black" BackColor="#DEDFDE"></ItemStyle>
                                                                            <HeaderStyle CssClass="DataHeaderStyle"></HeaderStyle>
                                                                            <FooterStyle ForeColor="Black" BackColor="#C6C3C6"></FooterStyle>
                                                                            <Columns>
                                                                                <asp:BoundColumn DataField="Address" HeaderText="Street Address"></asp:BoundColumn>
                                                                                <asp:BoundColumn Visible="False" DataField="Latitude" HeaderText="Latitude"></asp:BoundColumn>
                                                                                <asp:BoundColumn Visible="False" DataField="Longitude" HeaderText="Longitude"></asp:BoundColumn>
                                                                                <asp:ButtonColumn Text="Select" CommandName="Select" meta:resourcekey="ButtonColumnResource3"></asp:ButtonColumn>
                                                                            </Columns>
                                                                            <PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#C6C3C6"></PagerStyle>
                                                                        </asp:DataGrid>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style="width: 100%; height: 19px" align="center">
                                                                    <asp:Button ID="cmdSaveLandmark" runat="server" Text="Save" CssClass="combutton"
                                                                        OnClick="cmdSaveLandmark_Click" meta:resourcekey="cmdSaveLandmarkResource1"></asp:Button>&nbsp;&nbsp;
                                                                    <asp:Button ID="cmdCancelLandmark" runat="server" Text="Cancel" CssClass="combutton"
                                                                        CausesValidation="False" OnClick="cmdCancelLandmark_Click" meta:resourcekey="cmdCancelLandmarkResource1"></asp:Button></td>
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
        <asp:Button ID="btnHidden" runat="server" Width="0px" Height="0px" Style="display: none" OnClick="btnHidden_Click" ValidationGroup="NoGroup" />
        <asp:HiddenField ID="hidAddress" runat="server" />
    </form>


    <script language="javascript">
			<!--
    function ShowMapCall() {
        if ($("#<%= hidAddress.ClientID %>").val() == '') {
                 ShowMap();
                 return;
             }
             var postData = "{'street':'" + escape($("#<%= hidAddress.ClientID %>").val()) +
                               "'}";
             $.ajax({
                 type: "POST",
                 contentType: "application/json; charset=utf-8",
                 url: "frmlandmark.aspx/SetSearchMap",
                 data: postData,
                 dataType: "json",
                 success: function (data) {
                     ShowMap();
                 },
                 error: function (request, status, error) {
                     ShowMap();
                 }

             });
         }

         function ShowMap() {
             var mypage = '<%=(LandmarkMap==""?"../MapNew/frmDrawLandmarkGeozone.aspx?FormName=Landmark&ShowControl=True":LandmarkMap)%>';
             var myname = '';
             var w = 830;
             var h = 680;
             var winl = (screen.width - w) / 2;
             var wint = (screen.height - h) / 2;
             winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,'
             win = window.open(mypage, myname, winprops)
             if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }

         }



         function LandmarkMap(LandmarkId) {

             var mypage = '<%=ViewLandmarkMap%>?LandmarkId=' + LandmarkId
             var myname = 'Landmark';
             var w = 830;
             var h = 680;
             var winl = (screen.width - w) / 2;
             var wint = (screen.height - h) / 2;
             winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=0,toolbar=0,menubar=0,resizable=no'
             win = window.open(mypage, myname, winprops)
             if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
         }

         function CustomValidateDate(sender, args) {
             args.IsValid = true;
             var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
             var address = document.getElementById("<%= txtEmail.ClientID %>").value;
             if (reg.test(address) == false) {
                 args.IsValid = false;
             }
         }
         function ReFreshWindow() {
             document.getElementById("<%= btnHidden.ClientID %>").click();
         }
         //-->
    </script>
    <div style="display: none;" id="dlgFleetAssignment">
    </div>
</body>


</html>
