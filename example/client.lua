Citizen.CreateThread(function()
    while true do
        Citizen.Wait(1)

        -- pressing 'b' will create a sound on server side
        if IsControlJustReleased(0,  29) then
            TriggerServerEvent("example:create-sound")
        end

        -- faster walking/sprinting
        ResetPlayerStamina(PlayerId())
        SetPedMoveRateOverride(PlayerId(),10.0)
        SetRunSprintMultiplierForPlayer(PlayerId(),1.49)
    end
end)