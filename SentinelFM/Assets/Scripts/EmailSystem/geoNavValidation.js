function validationResult(transport) { //alert(" transport.responseText-" + transport.responseText);
    Element.update($('vmessage'), transport.responseText);
    Element.update($('vmessage'), Element.firstDescendant($('vmessage')).innerHTML);
    if ($('vmessage').innerHTML === 'OK') {
        Element.setStyle($('vmessage'), { display: 'none' });
        UpdateObjectCount();
    }
    else {
        var pos = Position.cumulativeOffset($('input_' + transport.request.parameters['id']));
        Element.setStyle($('vmessage'), { display: 'inline', left: pos.left + 'px', top: (pos.top + 23) + 'px' });
        UpdateFlag(false);
        UpdateObjectCount();
    }
}

function validate(id, controlType, isRequired, value) {
    //alert("id-" + id + " controlType-" + controlType + " isRequired->" + isRequired + " value->" + value);
    //alert("value.blank()-" + value.blank() + " isRequired->" + isRequired);
    var path = location.pathname.split("/");
    if (!value.blank() || isRequired) {
        new Ajax.Request("/Services/Validation.asmx/GeoNavInputValidation", {
            method: 'get',
            onComplete: function(transport) { validationResult(transport); },
            parameters: { 'id': id, 'controlType': controlType, 'isRequired': isRequired, 'value': value, 'message': '' },
            onFailure: function(transport) { Element.update($('vmessage'), "failed"); }
        });
    }
}