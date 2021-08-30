using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace SounityClient
{
    class SounityClientAPI: Sounity.BaseSounityAPI<SounitySound>
    {
        private long serverTime = API.GetGameTimer();
        private bool underwater = false;

        public SounityClientAPI(ExportDictionary Exports) : base(Exports, "client")
        {
            Exports.Add("CreateFilter", new Action<string, string, string>(CreateFilter));
            Exports.Add("AddListenerFilter", new Action<string>(AddListenerFilter));
            Exports.Add("RemoveListenerFilter", new Action<string>(RemoveListenerFilter));
        }

        public void setServerTime(long serverTime)
        {
            this.serverTime = serverTime;
        }

        public void StartSound(string identifier, long sT)
        {
            var startTime = Math.Max(0, serverTime - sT);

            getSoundInstance(identifier).Start(startTime);
        }

        internal void Tick()
        {
            Vector3 Position = API.GetGameplayCamCoords();
            Vector3 Rotation = API.GetFinalRenderedCamRot(0);

            int musicVolume = API.GetProfileSetting(306);
            int sfxVolume = API.GetProfileSetting(300);

            float waterHeight = 0;
            API.GetWaterHeightNoWaves(Position.X, Position.Y, Position.Z, ref waterHeight);

            if(Position.Z < waterHeight && underwater == false)
            {
                AddListenerFilter("underwater");
                underwater = true;
            } else if (Position.Z >= waterHeight && underwater == true)
            {
                RemoveListenerFilter("underwater");
                underwater = false;
            }

            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "update",
                posX = Position.X,
                posY = Position.Y,
                posZ = Position.Z,
                rotX = Rotation.X,
                rotY = Rotation.Y,
                rotZ = Rotation.Z,
                musicVolume,
                sfxVolume,
            }));

            foreach(var sound in sounds.Values)
            {
                if (!sound.isAttached()) continue;

                var netId = sound.GetAttachTo();

                if (!API.NetworkDoesEntityExistWithNetworkId(netId))
                {
                    sound.Detach();
                    continue;
                }

                var entId = API.NetworkGetEntityFromNetworkId(netId);
                var position = API.GetEntityCoords(entId, false); 
                var rotation = API.GetEntityRotation(entId, 2);

                sound.Move(position);
                sound.Rotate(rotation);
            }
        }
        public void CreateFilter(string filterName, string filterType, string options_json)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "createFilter",
                filterName,
                filterType,
                options = JsonConvert.DeserializeObject(options_json)
            }));
        }

        public void AddListenerFilter(string filterName)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "addListenerFilter",
                filterName,
            }));
        }

        public void RemoveListenerFilter(string filterName)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "removeListenerFilter",
                filterName,
            }));
        }
    }
}
