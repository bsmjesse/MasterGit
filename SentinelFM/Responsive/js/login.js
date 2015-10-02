ns = (document.layers) ? true : false
ie = (document.all) ? true : false


function LoadFrames(menu, main) {
    if (menu != "")
        parent.menu.window.location = menu;
    if (main != "")
        parent.main.window.location = main;
}


function Encrypt() {

    document.forms[0].elements["txtHash"].value = hex_md5(hex_md5($("input[id$=txtPassword]").val()) + $("#txtRnd").val());


    document.forms[0].submit();
}

function press(e) {
    if (ie) {
        if (event.keyCode == 13) {
            Encrypt();
        }
    }
}


function VaildateUserNameInput() {
    var iChars = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";

    for (var i = 0; i < document.forms[0].txtUserName.value.length; i++) {
        if (iChars.indexOf(document.forms[0].txtUserName.value.charAt(i)) != -1) {
            alert("Your username has special characters. \nThese are not allowed.\n Please remove them and try again.");
            return false;
        }
    }
}


function VaildatePswInput() {
    var iChars = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";

    for (var i = 0; i < document.forms[0].txtPassword.value.length; i++) {
        if (iChars.indexOf(document.forms[0].txtPassword.value.charAt(i)) != -1) {
            alert("Your password has special characters. \nThese are not allowed.\n Please remove them and try again.");
            return false;
        }
    }
}


function LoadOnTop() {
    if (window != window.top) {
        window.top.location.href = window.location.href;
    }
}



var lcookie = null;
function setupLogin() {

    try {
        lcookie = $.cookie("sfm_login");
    }
    catch (e) { }

    if ((lcookie !== null) && $("rblSFM_" + lcookie) != null)
        $("rblSFM_" + lcookie).checked = true;


};

function prepareLogin() {
    try {
        window.external.AutoCompleteSaveForm(document.forms[0]);
    }
    catch (err) { }
    $('cmdLogin').disabled = true;

    var val = ($("rblSFM_1").checked) ? 1 : 0;
    if ((lcookie === null) || ($("rblSFM_" + lcookie) == null))
        lcookie = $.cookie("sfm_login", val);
    else {
        if ($("rblSFM_" + lcookie).checked === false) {
            $.removeCookie("sfm_login");
            $.cookie("sfm_login", val, false);
            lcookie = $.cookie("sfm_login");
        }
    }

    Encrypt();
};



//Event.observe(window, "resize", setupLogin);
//Event.observe(document, "keydown", function(event) { press(event) });

function prefocuslogin() {
    if (AuthenticationFailed == 0) {
        $('txtUserName').focus();

    }
    else {
        $('txtPassword').focus();
        setTimeout(function () {
            document.getElementById("txtPassword").value = '';
        }, 100)

    }

}

function BannerRedirect() {

    var url1 = '<%=BannerLink1Url1 %>';
    window.location = url1;
};

var bsmLogin = (function () {

}(bsmLogin || {}));