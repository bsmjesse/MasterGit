var enableOptions = false;
var optionsSet = false;

function loader() {
    Form.Element.focus("txtUserName");
}

function iKey(e) {
    if (optionsSet)
        Form.Element.enable($("cmdLogin"));
//    if (e.keyCode) {
//        if (e.keyCode === 13)
//            performAuthentication();
//    }
}

function test(pe) {
    if (pe) pe.stop();
    //$("txtPassword").focus();
    Form.Element.focus("txtPassword");
}

function iBlur(e) {
    var value = $F("txtUserName");
    //Element.setStyle($("passwordContainer"), { display: "inline-block" });
    Form.Element.enable($("txtPassword"));
    new PeriodicalExecuter(function(pe) { test(pe); }, 0.5);
    getUserOptions();
}

function iFocus(e) {
    optionsSet = false;
    Element.setStyle($("optionContainer"), { display: "none" });
    //Element.setStyle($("passwordContainer"), { display: "none" });
    Form.Element.enable($("cmdLogin"));
//    Form.Element.disable($("txtPassword"));
//    $("txtUserName").value = "";
//    $("txtPassword").value = "";
}

function getUserOptionsResponse(transport) {
    eval(transport.responseText);
    if (enableOptions)
        Element.setStyle($("optionContainer"), { display: "inline-block" });
    if ($F("txtPassword").length > 0)
        Form.Element.enable($("cmdLogin"));

    //Form.Element.enable($("cmdLogin"));
};

function selectRadio(e) {
    switch (e.id) {
        case "rblSFM_0":
            $("rblSFM_1").checked = false ;
            $("rblSFM_0").checked = true;
            break;
        case "rblSFM_1":
            $("rblSFM_1").checked = true  ;
            $("rblSFM_0").checked = false ;
            break;
    }
}


function getUserOptions() {
    var params = { "fct": "getUserOptions", "username": $F("txtUserName") };
    new Ajax.Request("auth.aspx", {
        method: "post",
        parameters: params,
        evalJS: true,
        onSuccess: function(transport) { getUserOptionsResponse(transport); }
    });
}



function performAuthentication() {
    var user = $F("txtUserName");
    var pwd = $F("txtPassword");
    //alert("OK, " + user + ":" + pwd);

}