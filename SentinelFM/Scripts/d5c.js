function createCookie(a, b, expires) {
    var c, d, e, space;
    c = "";
    d = document.cookie.length;
    e = readCookie(a);
    if (e === null) e = "";
    space = d - e.length;
    if (space + b.length > 4000) {
        if (!confirm("cache space for " + a + " data has been exhausted,\ndo you wish to clear old values?")) {
            eraseCookie(a);
            return;
        }
    }
    else
        b = b + e;
    if (expires)
        document.cookie = a + "=" + b + "; path=/";
    else
        document.cookie = a + "=" + b + "; " + "; expires=Tues, 31 Dec 2019 23:59:59 GMT; path=/";

};

function readCookie(a) {
    var b, c, d, i;
    b = a + "=";
    c = document.cookie.split(';');
    for (i = 0; i < c.length; i++) {
        var d = c[i];
        if (d.length > 0) {
            while (d.charAt(0) == ' ')
                d = d.substring(1, d.length);
            if (d.indexOf(b) == 0)
                return d.substring(b.length, d.length);
        }
    }
    return null;
};

function eraseCookie(a) { document.cookie = a + "=" + null + "; expires=Tues, 31 Dec 2000 23:59:59 GMT; path=/"; };

