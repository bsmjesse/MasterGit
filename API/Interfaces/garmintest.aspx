<%@ Page Language="C#" AutoEventWireup="true" CodeFile="garmintest.aspx.cs" Inherits="test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">


        $(document).ready(function () {
            $.ajax({
                type: 'POST',
                dataType: 'xml',
                url: '../dac/XmlDocService.asmx/GetXml',
                data: {

                },
                success: function (data) {
                    var xml = $.parseJSON($(data).children().first().text());
                    $.ajax({
                        type: 'POST',
                        url: 'garmin.aspx',
                        dataType: 'json',
                        data: { 'Token': "8A272419-61F1-4B9E-BAF1-FB828EF5EC80", 'xml': escape(xml) },
                        success: function (data) {
                            var result = $.parseJSON($(data).children().first().text());
                            if (result == null) {
                                result = data.mid;
                            }
                            $('#dresp').html(result);
                        },
                        error: function (request, status, error) {
                            alert(error);
                        }
                    });
                },
                error: function (request, status, error) {
                    alert(error);
                }
            });

            /*
             <add key="HASH482" value="49E8D7EB-5877-4C2E-9702-CBD4FE8197A8:482:BSMTraining:3921"/>
    <add key="HASH500" value="aOYHmb7ka8YTEqD6I3ZOLK:500:Tother:0"/>
   
   */
        });
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id='dresp'>
    </div>
    </form>
</body>
</html>
