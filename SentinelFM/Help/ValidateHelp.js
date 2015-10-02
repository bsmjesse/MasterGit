    $(document).ready(function () {
        $.ajax({ url: "../Validate.aspx", success: function (result) {
            //alert(result);
            if (result == '0') {
                window.open('../../Login.aspx', '_top', '', '');
            }
        }
        });
    });