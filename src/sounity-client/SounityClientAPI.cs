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
    class SounityClientAPI
    {
        private static int idCounter = 1;
        private long serverTime = API.GetGameTimer();
        private Dictionary<string, SounitySound> sounds = new Dictionary<string, SounitySound>();
        public SounityClientAPI(ExportDictionary Exports)
        {
            Exports.Add("CreateSound", new Func<string, string, string>(CreateSound));
            Exports.Add("StartSound", new Action<string>(StartSound));
            Exports.Add("MoveSound", new Action<string, float, float, float>(MoveSound));
            Exports.Add("RotateSound", new Action<string, float, float, float>(RotateSound));
            Exports.Add("StopSound", new Action<string>(StopSound));
            Exports.Add("DisposeSound", new Action<string>(DisposeSound));
            Exports.Add("AttachSound", new Action<string, int>(AttachSound));
            Exports.Add("DetachSound", new Action<string>(DetachSound));
        }

        private SounitySound getSoundInstance(string identifier)
        {
            if (!sounds.ContainsKey(identifier))
            {
                throw new Exception($"Unknown identifier '{identifier}'");
            }

            return sounds[identifier];
        }

        public string CreateSound(string source, string options_json = "{}")
        {
            var identifier = $"client_{idCounter++}";

            sounds[identifier] = new SounitySound(identifier, source, JsonConvert.DeserializeObject<Dictionary<string, object>>(options_json), true);
            return identifier;
        }

        public string CreateSound(string identifier, string source, string options_json = "{}")
        {
            if(sounds.ContainsKey(identifier))
                throw new Exception($"Sound with given identifier [{identifier}] already exists!");

            sounds[identifier] = new SounitySound(identifier, source, JsonConvert.DeserializeObject<Dictionary<string, object>>(options_json), false);

            return identifier;
        }

        public void setServerTime(long serverTime)
        {
            this.serverTime = serverTime;
        }

        public void StartSound(string identifier)
        {
            getSoundInstance(identifier).Start();
        }

        public void StartSound(string identifier, long sT)
        {
            var startTime = Math.Max(0, serverTime - sT);
            getSoundInstance(identifier).Start(startTime);
        }

        public void MoveSound(string identifier, float posX, float posY, float posZ)
        {
            getSoundInstance(identifier).Move(posX, posY, posZ);
        }

        public void RotateSound(string identifier, float rotX, float rotY, float rotZ)
        {
            getSoundInstance(identifier).Rotate(rotX, rotY, rotZ);
            
        }

        public void StopSound(string identifier)
        {
            getSoundInstance(identifier).Stop();
        }

        public void AttachSound(string identifier, int netId)
        {
            getSoundInstance(identifier).Attach(netId);
        }

        public void DetachSound(string identifier)
        {
            getSoundInstance(identifier).Detach();
        }

        public void DisposeSound(string identifier)
        {
            getSoundInstance(identifier).Dispose();
            sounds.Remove(identifier);
        }

        internal void Tick()
        {
            Vector3 Position = API.GetGameplayCamCoords();
            Vector3 Rotation = API.GetFinalRenderedCamRot(0);

            int musicVolume = API.GetProfileSetting(306);
            int sfxVolume = API.GetProfileSetting(300);

            float waterHeight = 0;
            API.GetWaterHeightNoWaves(Position.X, Position.Y, Position.Z, ref waterHeight);

            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "update",
                posX = Position.X,
                posY = Position.Y,
                posZ = Position.Z,
                rotX = Rotation.X,
                rotY = Rotation.Y,
                rotZ = Rotation.Z,
                underwater = Position.Z < waterHeight,
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
    }
}
