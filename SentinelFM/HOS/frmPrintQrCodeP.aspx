<%@ Page Language="C#" AutoEventWireup="true" CodeFile="frmPrintQrCodeP.aspx.cs" Inherits="HOS_frmPrintQrCodeP" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="styles.css" rel="stylesheet" type="text/css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.5.2/jquery.min.js"></script>
    <script type="text/javascript" src="../Scripts/qrcode.js"></script>
    <script type="text/javascript" src="../Scripts/jquery.qrcode.js"></script>
    <style type="text/css">

    

    @media print
    {
    	.non-printable { display: none; }
    	.printable { display: block; }
        table { page-break-after:auto }
          tr    { page-break-inside:avoid; page-break-after:auto }
          td    { page-break-inside:avoid; page-break-after:auto }
          thead { display:table-header-group }
          tfoot { display:table-footer-group }
    }
    </style>
</head>
<body>

    <form id="form1" runat="server">
    <div>
        <table>
            <tr >
                <td  class="non-printable" align="center" >
                    <a id ="printid" style="font-weight:bold; "  href="#" onclick="javascript:window.print();"  >[Print]</a>
                    <br />
                    <br />
                </td>

            </tr>
            <tr>
                <td class="printable">
                    <asp:PlaceHolder ID="phlBarcode" runat="server"  ></asp:PlaceHolder>
                </td>
            </tr>
        </table>
    </div>
        <div id="qrcodeTable"></div>
        <script>
            //jQuery('#qrcode').qrcode("this plugin is great");
            var barcodeString = '<%= barcodeString%>'.split(',');
            if (barcodeString == '') $('#printid').hide();
            for (var index = 0; index < barcodeString.length; index++) {
                jQuery('#barcode_' + barcodeString[index]).qrcode({
                    text: barcodeString[index]
                });
                
                //$("<span style='text-align: center' >" + barcodeString[index] + "</span>").append('#barcode_' + barcodeString[index]);
            }
        </script>
    </form>
</body>
</html>
