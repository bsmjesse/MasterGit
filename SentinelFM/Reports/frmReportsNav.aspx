<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmReportsNav.aspx.cs" Inherits="frmReportsNav" meta:resourcekey="PageResource1" %>

<%--<%@ Register Assembly="ISNet.WebUI.WebDesktop" Namespace="ISNet.WebUI.WebDesktop"
    TagPrefix="ISWebDesktop" %>--%>
     
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/css/bootstrap.min.css">
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>  
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js"></script>

    <title>Untitled Page</title>
    <script type="text/javascript">
        function showStandardReport() {
            $('#extendedReport').hide();
            $('#standardReport').show();
        }
        function showExtendReport() {
            $('#standardReport').hide();
            $('#extendedReport').show();
        }

        $(document).ready(function () {
            $(".btn-group > .btn").click(function () {
                $(".btn-group > .btn").removeClass("active");
                $(this).addClass("active");
                if ($(this).attr('id') == 'btnStandardReport') {
                    showStandardReport();
                }
                else {
                    if ($(this).hasClass('disabled'))
                        return;
                    showExtendReport();
                }
            });
        });
    </script>
</head>
<body id="body" runat="server" leftmargin="10" topmargin="10" rightmargin="10" bottommargin="10">
    <form id="form1" runat="server">
        <div>            
            <table cellspacing="0" cellpadding="0" border="0" style="height:100%; width:100%">
                <tbody>
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" border="0" style="table-layout:fixed; width:100%;">
                                <tbody>
                                    <tr>
                                        <td width="100%">
                                            <div style="overflow:hidden;width:100%; height:100%; position:relative">
                                                <table cellspacing="0" cellpadding="0" border="0" style="width:100%;height:100%;" onselectstart="return false" id="tbHeader_WebReportTab">
                                                    <tbody>
                                                        <tr>
                                                            <td valign="top" nowrap="" style="border-bottom: 1px solid Navy; empty-cells: show; padding-top: 4px;padding-bottom:5px;" name="StandardReports" type="Tab">                                                                
                                                                    <div class="btn-group">
                                                                        <button type="button" class="active btn btn-default" id="btnStandardReport"><asp:Label ID="Label1" runat="server" Text="Label" meta:resourcekey="StandardReports"></asp:Label></button>
                                                                        <% if(ExtendedReportEnabled) { %>
                                                                        <button type="button" class="btn btn-default " id="btnExtendedReport">Extended Reports</button>
                                                                        <%} %>
                                                                    </div>                                                                
                                                            </td>                                                            
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                    <tr height="100%">
                        <td valign="top" style="width:100%; height:100%;">
                            <div style="position: relative; border-top: medium none;width: 100%; height: 100%;" class="WebReportTab-ContainerStyle" id="dvTabCnt_WebReportTab">
                                <div style="width: 100%; height: 100%;" id="standardReport">
                                    <iframe frameborder="0" name="StandardReports" style="width: 100%; height: 100%; position: relative;" src="<%=stReportUrl %>" allowtransparency="true"></iframe>
                                </div>
                                <% if(ExtendedReportEnabled) { %>
                                <div style="width: 100%; height: 100%; display: none;" id="extendedReport">
                                    <iframe frameborder="0" name="ExtendedReports" style="width:100%; height:100%; position:relative;" src="/Reports/frmReportMasterExtendedNew.aspx" allowtransparency="true"></iframe>
                                </div>
                                <% } %>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
