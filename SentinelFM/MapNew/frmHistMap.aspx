<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmHistMap.aspx.cs" Inherits="SentinelFM.MapNew_frmHistMap" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register src="UserControl/Map.ascx" tagname="Map" tagprefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Styles/Site.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/css/Map/sentinel.telogis.css?ver=3.1" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery-1.4.1.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.geobase.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/telogis.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.util.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.dictionary_en.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.skins.js" language="javascript"></script>
    <script type="text/javascript" src="../Scripts/sentinel.telogis.map.js?ver=3.1" language="javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
      <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;  ">
                <tr>
                    <td id="frmVehicleMapfrmVehicleMap" style="height: 337px; width: 100%">
                        <uc1:Map ID="Map1" runat="server" />
                    </td>
                </tr>
      </table>
    
    </div>
    <script type="text/javascript" >
        function RenderHistoryMap() {
                            var loadingLabel = document.createElement("div");
                            $.ajax({

                                type: "POST",
                                url: "frmHistMap.aspx/LoadHistoryData",
                                data: null,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",

                                success: function (data) {
                                    if (data.d != '-1' && data.d != "0") {
                                        if (data.d != '') {
                                            ShowHistoryMap(eval(data.d))

                                            $(loadingLabel).fadeOut("slow", function () {
                                                $(this).remove();
                                            });

                                        }
                                    }
                                    if (data.d == '') {
                                        $(loadingLabel).fadeOut("slow", function () {
                                            $(this).remove();
                                        });
                                    }
                                    if (data.d == '-1') {
                                        window.open('../Login.aspx', '_top')
                                    }
                                    if (data.d == '0') {
                                        $(loadingLabel).fadeOut("slow", function () {
                                            $(this).remove();
                                        });

                                        alert("<%= errorSave%>");
                                    }
                                },

                                error: function (request, status, error) {
                                    $(loadingLabel).fadeOut("slow", function () {
                                        $(this).remove();
                                    });

                                    //alert("<%= errorSave%>");
                                    console.log(request.responseText);
                                    alert(request.responseText);
                                    return false;
                                },

                                beforeSend: function (request, status) {
                                    loadingLabel.innerHTML = 'loading...';
                                    $(loadingLabel).addClass("loading");
                                    myMap.map.frame.appendChild(loadingLabel);
                                    $(loadingLabel).fadeIn("fast");
                                }

                            });
        }
    </script>
    </form>
</body>
</html>
