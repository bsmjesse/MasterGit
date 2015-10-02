var enableOptions = false;
var optionsSet = false;

function loader() {
    Form.Element.focus("usernameValue");
}

function iKey(e) {
    if (optionsSet)
        Form.Element.enable($("loginButton"));
    if (e.keyCode) {
        if (e.keyCode === 13)
            performAuthentication();
    }
}

function test(pe) {
    if (pe) pe.stop();
    //$("passwordValue").focus();
    Form.Element.focus("passwordValue");
}

function iBlur(e) {
    var value = $F("usernameValue");
    //Element.setStyle($("passwordContainer"), { display: "inline-block" });
    Form.Element.enable($("passwordValue"));
    new PeriodicalExecuter(function(pe) { test(pe); }, 0.5);
    getUserOptions();
}

function iFocus(e) {
    optionsSet = false;
    Element.setStyle($("optionContainer"), { display: "none" });
    //Element.setStyle($("passwordContainer"), { display: "none" });
    Form.Element.enable($("loginButton"));
    Form.Element.disable($("passwordValue"));
    $("usernameValue").value = "";
    $("passwordValue").value = "";
}

function getUserOptionsResponse(transport) {
    eval(transport.responseText);
    if (enableOptions)
        Element.setStyle($("optionContainer"), { display: "inline-block" });
    if ($F("passwordValue").length > 0)
        Form.Element.enable($("loginButton"));

    //Form.Element.enable($("loginButton"));
};

function selectRadio(e) {
    switch (e.id) {
        case "standardCheck":
            $("liteCheck").checked = "";
            $("standardCheck").checked = "checked";
            break;
        case "liteCheck":
            $("standardCheck").checked = "";
            $("liteCheck").checked = "checked";
            break;
    }
}


function getUserOptions() {
    var params = { "fct": "getUserOptions", "username": $F("usernameValue") };
    new Ajax.Request("auth.aspx", {
        method: "post",
        parameters: params,
        evalJS: true,
        onSuccess: function(transport) { getUserOptionsResponse(transport); }
    });
}



function performAuthentication() {
    var user = $F("usernameValue");
    var pwd = $F("passwordValue");
    alert("OK, " + user + ":" + pwd);

}