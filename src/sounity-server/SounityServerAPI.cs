using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Sounity;

namespace SounityServer
{
    class SounityServerAPI : BaseSounityAPI<SounitySound>
    {
        private int MAX_RANGE;

        public SounityServerAPI(ExportDictionary Exports): base(Exports, "server")
        {
            MAX_RANGE = Config.GetInstance().Get("stream_max_range", 100);
            Exports.Add("AddListenerFilter", new Action<int, string>(AddListenerFilter));
            Exports.Add("RemoveListenerFilter", new Action<int, string>(RemoveListenerFilter));
        }

        public void AddListenerFilter(int playerId, string filterName)
        {
            (new PlayerList())[playerId].TriggerEvent("Sounity:AddListenerFilter", filterName);
        }

        public void RemoveListenerFilter(int playerId, string filterName)
        {
            (new PlayerList())[playerId].TriggerEvent("Sounity:RemoveListenerFilter", filterName);
        }

        public void Tick()
        {

            foreach (var sound in sounds.Values)
            {
                foreach (var player in new PlayerList())
                {
                    var ped = API.GetPlayerPed(player.Handle);

                    if (ped == 0)
                        continue;

                    var distance = Vector3.Distance(API.GetEntityCoords(ped), sound.getPosition());
                    if (distance <= MAX_RANGE && !sound.playersInRange.Contains(player))
                    {
                        sound.PlayerInRange(player);
                    }
                    else if (distance > MAX_RANGE && sound.playersInRange.Contains(player))
                    {
                        sound.PlayerOutOfRange(player);
                    }
                }
            }
        }
    }
}
