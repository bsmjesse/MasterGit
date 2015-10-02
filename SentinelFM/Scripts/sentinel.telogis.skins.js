
/* ************************************************************************* */
// Dock skins

(function () {
    var DockSkin = Telogis.GeoBase.Widgets.DockSkin;
    DockSkin.standard = (DockSkin.vertical = new DockSkin({
        background: "images/skins/dock/vertical.png"
    }));
})();

(function () {
    var DockSkin = Telogis.GeoBase.Widgets.DockSkin;
    DockSkin.horizontal = new DockSkin({
        background: "images/skins/dock/horizontal.png"
    });
})();


/* ************************************************************************* */
// Slider skins

(function () {

    // create a vertical slider skin
    var SliderSkin = Telogis.GeoBase.Widgets.SliderSkin;
    SliderSkin.standard = (SliderSkin.translucentVertical = new SliderSkin({
        folder: "images/skins/slider/translucentVertical",
        layout: SliderSkin.VERTICAL
    }));

})();


/* ************************************************************************* */
// Balloon skins

// describes a simple skin for a balloon with a plain white background, eight-pixel 
// directional tick, and one-pixel black border.

(function () {

    var BalloonSkin = Telogis.GeoBase.MapLayers.BalloonSkin;

    BalloonSkin.standard = (BalloonSkin.directedWhite = new BalloonSkin({

        bodyStyle: {

            backgroundColor: "#ffffff",
            border: "1px solid black",/*
            color: "#000000",
            fontFamily: "sans-serif",
            fontSize: "10px",
            fontWeight: "normal",
            padding: "10px",
            textAlign: "left",
            verticalAlign: "middle",*/
            fontSize: "9px",
            minWidth: "250px"
        },
        folder: "images/skins/balloon/directedwhite"
    }));
})();
