// JavaScript Document
Ext.onReady(function () {

    Ext.tip.QuickTipManager.init();
    
    var drawComponent = Ext.create('Ext.draw.Component', {
        width: 800,
        height: 600,
        renderTo: document.body
    }), surface = drawComponent.surface;

    surface.add([{
        type: 'circle',
        radius: 10,
        fill: '#00C000',
        x: 10,
        y: 10,
        group: 'greencircles'
    }, 
    ]);

       surface.add([{
        type: 'circle',
        radius: 10,
        fill: '#0000FF',
        x: 50,
        y: 10,
        group: 'bluecircles'
    }, 
    ]);
    // Get references to my groups
    bluecircles = surface.getGroup('bluecircles');

    // Get references to my groups
    greencircles = surface.getGroup('greencircles');    

    // Animate the circles across
    greencircles.animate({
        duration: 1000,
        to: {
            translate: {
                 x: 40
            }
        }
    });
        
    // Animate the circles across
    bluecircles.animate({
        duration: 1000,
        to: {
            translate: {
                 x: 20
            }
        }
    });

});
