RegisterNetEvent("example:create-sound")
AddEventHandler("example:create-sound", function() 
    local ped = GetPlayerPed(source);
    local playerCoords = GetEntityCoords(ped);

    local soundId = exports.sounity:CreateSound("https://www.soundhelix.com/examples/mp3/SoundHelix-Song-4.mp3", json.encode({
        posX = playerCoords.x,
        posY = playerCoords.y,
        posZ = playerCoords.z,
    }));

    exports.sounity:StartSound(soundId);

    local veh = GetVehiclePedIsIn(ped, false);
    if veh then
        local networkId = NetworkGetNetworkIdFromEntity(veh)
        exports.sounity:AttachSound(soundId, networkId)
    end
end)