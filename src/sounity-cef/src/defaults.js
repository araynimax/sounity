const defaults = {};

fetch(`https://${GetParentResourceName()}/sounity:get-defaults`)
    .then(response => response.text())
    .then(text => JSON.parse(JSON.parse(text)))
    .then(json_defaults =>
        Object.keys(json_defaults)
            .forEach(key => defaults[key] = json_defaults[key])
    );

export default defaults;