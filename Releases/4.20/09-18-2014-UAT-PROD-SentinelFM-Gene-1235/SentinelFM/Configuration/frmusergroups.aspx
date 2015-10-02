<%@ Page Language="c#" Inherits="SentinelFM.frmUserGroups" CodeFile="frmUserGroups.aspx.cs" Culture="en-US" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Src="Components/ctlMenuTabs.ascx" TagName="ctlMenuTabs" TagPrefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>frmUserGroups</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <!--<link href="../GlobalStyle.css" type="text/css" rel="stylesheet" />-->
    <script language="javascript">
<!--
    function controlsWindow() {
        var cboGroupsValue = document.getElementById('cboGroups').value;
        if (cboGroupsValue != '-1') {
            var mypage = 'frmGroupControls.aspx?GrId=' + cboGroupsValue
            var myname = 'GroupControls';
            var w = 960;
            var h = screen.height - 200;
            var winl = (screen.width - w) / 2;
            var wint = (screen.height - h) / 2;
            winprops = 'height=' + h + ',width=' + w + ',top=' + wint + ',left=' + winl + 'location=0,status=0,scrollbars=1,toolbar=0,menubar=0,'
            win = window.open(mypage, myname, winprops)
            if (parseInt(navigator.appVersion) >= 4) { win.window.focus(); }
        }
    }
    //-->	
    </script>
</head>
<body>
    <form id="frmVehicleInfo" method="post" runat="server">
        <table id="tblCommands" style="Z-INDEX: 101; LEFT: 8px; POSITION: absolute; TOP: 4px" cellspacing="0"
            cellpadding="0" width="300" border="0">
            <tr>
                <td>
                    <uc1:ctlMenuTabs ID="ctlMenuTabs1" runat="server" SelectedControl="cmdUsers" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblBody" cellspacing="0" cellpadding="0" width="990px" align="center" border="0">
                        <tr>
                            <td>
                                <table id="tblForm" width="990px" height="550px" class="table"
                                    border="0">
                                    <tr>
                                        <td class="configTabBackground">
                                            <table id="tblSubCommands" style="LEFT: 10px; POSITION: relative; TOP: 0px" cellspacing="0"
                                                cellpadding="0" border="0">
                                                <tr>
                                                    <td>
                                                        <table id="Table5" style="z-index: 101; width: 190px; position: relative; top: 0px; height: 22px"
                                                            cellspacing="0" cellpadding="0" border="0">
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="cmdUserInfo" runat="server" CommandName="17" CausesValidation="False"
                                                                        CssClass="confbutton" Text="User Info" Width="112px" OnClick="cmdUserInfo_Click"
                                                                        meta:resourcekey="cmdUserInfoResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdUserGroups" runat="server" CommandName="21" CausesValidation="False"
                                                                        CssClass="selectedbutton" Text="User-Groups Assignment" Width="173px" Height="22px"
                                                                        meta:resourcekey="cmdUserGroupsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdGroups" runat="server" Text="Groups" CssClass="confbutton" OnClick="cmdGroups_Click"
                                                                        CausesValidation="False" CommandName="71" Width="112px" meta:resourcekey="cmdGroupsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdGroupConfiguration" runat="server" Text="Group Configuration" CssClass="confbutton" OnClick="cmdGroupConfiguration_Click"
                                                                        CausesValidation="False" CommandName="79" Width="200px" meta:resourcekey="cmdGroupConfigurationResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdControls" runat="server" Text="Controls" CssClass="confbutton" OnClick="cmdControls_Click"
                                                                        CausesValidation="False" CommandName="70" Width="112px" meta:resourcekey="cmdControlsResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdServices" runat="server" Text="Services" CssClass="confbutton" OnClick="cmdServices_Click"
                                                                        CausesValidation="False" CommandName="90" Width="112px" meta:resourcekey="cmdServicesResource1"></asp:Button>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="cmdUserDashBoards" runat="server" CausesValidation="False" CssClass="confbutton"
                                                                        Visible="False" Text="User-DashBoards" Width="173px" Height="22px" OnClick="cmdUserDashBoards_Click"></asp:Button>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <table id="Table6" cellspacing="0" cellpadding="0" width="617" align="center" border="0">
                                                            <tr>
                                                                <td>
                                                                    <table id="Table7" class="table" height="500px" width="960px"
                                                                        border="0">
                                                                        <tr>
                                                                            <td class="configTabBackground" valign="top">
                                                                                <table id="Table3" style="WIDTH: 299px; HEIGHT: 20px" cellspacing="0" cellpadding="0" width="299"
                                                                                    align="center" border="0">
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td class="formtext" style="width: 51px; HEIGHT: 40px"></td>
                                                                                            <td style="width: 208px"></td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td class="formtext" style="WIDTH: 51px">&nbsp;
                                                                                                <table class="formtext">
                                                                                                    <tr>
                                                                                                        <td style="width: 100px">
                                                                                                            <asp:Label ID="lblGroupTitle" runat="server" meta:resourcekey="lblGroupTitleResource1" Text="Group:"></asp:Label></td>
                                                                                                        <td style="white-space: nowrap;">
                                                                                                            <asp:DropDownList ID="cboGroups" runat="server" CssClass="RegularText" Width="234px" AutoPostBack="True"
                                                                                                                DataValueField="UserGroupId" DataTextField="UserGroupName" OnSelectedIndexChanged="cboGroups_SelectedIndexChanged" meta:resourcekey="cboGroupsResource2">
                                                                                                            </asp:DropDownList>
                                                                                                            <asp:ImageButton ID="btnPreview" runat="server" CausesValidation="False" ImageUrl="../images/info.png" ImageAlign="AbsBottom" Visible="false" OnClientClick="controlsWindow();" />
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                            </td>
                                                                                            <td style="WIDTH: 208px"></td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td style="WIDTH: 959px; HEIGHT: 217px" align="center">
                                                                                                <table id="Table8" style="WIDTH: 447px; HEIGHT: 270px" cellspacing="0" cellpadding="0"
                                                                                                    width="447" border="0">
                                                                                                    <tr>
                                                                                                        <td style="HEIGHT: 270px" align="center">
                                                                                                            <table id="tblUsers" style="WIDTH: 433px; HEIGHT: 235px" cellspacing="0" cellpadding="0"
                                                                                                                width="433" border="0" runat="server">
                                                                                                                <tr>             
                                                                                                                    <td valign="top" colspan="4" style="text-align:right;color:Black;text-decoration:underline;">
                                                                                                                        <asp:PlaceHolder ID="imgExcel" runat="server" Visible="true">
                                                                                                                            <a href="#"  onclick="ExportToExcel()" class="RegularText"><%=ExportToExcel%></a>
                                                                                                                        </asp:PlaceHolder>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td colspan="2" class="formtext" style="WIDTH: 291px">
                                                                                                                        <asp:Label ID="lblUnAssUsers" runat="server" meta:resourcekey="lblUnAssUsersResource1"
                                                                                                                            Text="Unassigned users"></asp:Label></td>
                                                                                                                    <%--<TD style="WIDTH: 181px" align="center"></TD>--%>
                                                                                                                    <td class="formtext">
                                                                                                                        <asp:Label ID="lblAssUsers" runat="server" meta:resourcekey="lblAssUsersResource1"
                                                                                                                            Text="Assigned users"></asp:Label></td>
                                                                                                                </tr>
                                                                                                                <tr>
                                                                                                                    <td style="WIDTH: 110px">
                                                                                                                        <asp:ListBox ID="lstUnAss" runat="server" CssClass="formtext" Width="160px" DataValueField="UserId"
                                                                                                                            DataTextField="UserFullName" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstUnAssResource1"></asp:ListBox></td>
                                                                                                                    <td style="WIDTH: 181px" align="center">
                                                                                                                        <table id="tblAddRemoveBtns" style="WIDTH: 75px; HEIGHT: 99px" cellspacing="0" cellpadding="0"
                                                                                                                            width="75" border="0" runat="server">
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="cmdAdd" runat="server" CssClass="combutton" Text="Add->" CommandName="33" OnClick="cmdAdd_Click" meta:resourcekey="cmdAddResource1"></asp:Button></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td style="HEIGHT: 20px"></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="cmdAddAll" runat="server" CssClass="combutton" Text="Add All->" CommandName="33" OnClick="cmdAddAll_Click" meta:resourcekey="cmdAddAllResource1"></asp:Button></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td id="TD1" style="HEIGHT: 20px" runat="server"></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="cmdRemove" runat="server" CssClass="combutton" Text="<-Remove" CommandName="34" OnClick="cmdRemove_Click" meta:resourcekey="cmdRemoveResource1"></asp:Button></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td style="HEIGHT: 20px"></td>
                                                                                                                            </tr>
                                                                                                                            <tr>
                                                                                                                                <td>
                                                                                                                                    <asp:Button ID="cmdRemoveAll" runat="server" CssClass="combutton" Text="<-Remove All" CommandName="34" OnClick="cmdRemoveAll_Click" meta:resourcekey="cmdRemoveAllResource1"></asp:Button></td>
                                                                                                                            </tr>
                                                                                                                        </table>
                                                                                                                    </td>
                                                                                                                    <td>
                                                                                                                        <asp:ListBox ID="lstAss" runat="server" CssClass="formtext" Width="160px" DataValueField="UserId"
                                                                                                                            DataTextField="UserFullName" SelectionMode="Multiple" Rows="15" meta:resourcekey="lstAssResource1"></asp:ListBox></td>
                                                                                                                </tr>
                                                                                                            </table>
                                                                                                        </td>
                                                                                                    </tr>
                                                                                                </table>
                                                                                                <asp:Label ID="lblMessage" runat="server" CssClass="errortext" Width="270px" Height="8px" Visible="False" meta:resourcekey="lblMessageResource1"></asp:Label>
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
      <asp:HiddenField ID="expdata" runat="server" Value='' />
    </form>

    <script type="text/javascript">
        function ExportToExcel() {
            document.getElementById('exportdata').value = document.getElementById('expdata').value;
            document.getElementById('filename').value = "usergroups";
            document.getElementById('formatter').value = "excel2007";
            exportform.submit();
        }
    </script>
    
    <iframe id="exportframe" name="exportframe" style="display: none"></iframe>
    <form id="exportform" method="post" target="exportframe" action="../MapNew/frmExportData.aspx">
        <input type="hidden" id="exportdata" name="exportdata" value='' />
        <input type="hidden" id="filename" name="filename" value='' />
        <input type="hidden" id="formatter" name="formatter" value='' />
    </form>
</body>
</html>
