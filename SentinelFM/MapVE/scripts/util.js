function getFrameElement(name, parent) {
    var b = window;
    if (parent !== undefined)
        b = parent.window;
    var c = b.frames.length;
    for (var a = 0; a < c; a++) {
        var fx = b.frames[a].frameElement;
        if (fx.id === name)
            return fx.contentWindow;
    }

}