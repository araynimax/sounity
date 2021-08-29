using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace SounityServer
{
    class SounityServerAPI
    {
        private int MAX_RANGE;

        private static int idCounter = 1;
        private Dictionary<string, SounitySound> sounds = new Dictionary<string, SounitySound>();

        public SounityServerAPI(ExportDictionary Exports)
        {
            Exports.Add("CreateSound", new Func<string, string, string>(CreateSound));
            Exports.Add("StartSound", new Action<string>(StartSound));
            Exports.Add("MoveSound", new Action<string, float, float, float>(MoveSound));
            Exports.Add("RotateSound", new Action<string, float, float, float>(RotateSound));
            Exports.Add("StopSound", new Action<string>(StopSound));
            Exports.Add("DisposeSound", new Action<string>(DisposeSound));
            Exports.Add("AttachSound", new Action<string, int>(AttachSound));
            Exports.Add("DetachSound", new Action<string, int>(DetachSound));

            MAX_RANGE = Sounity.Config.GetInstance().Get("stream_max_range", 100);
        }
        

        private SounitySound getSoundInstance(string identifier)
        {
            if (!sounds.ContainsKey(identifier))
            {
                throw new Exception($"Unknown identifier '{identifier}'");
            }

            return sounds[identifier];
        }

        public string CreateSound(string source, string options_json = null)
        {
            var identifier = $"server_{idCounter++}";

            if (options_json == null)
                options_json = "{}";

            sounds[identifier] = new SounitySound(identifier, source, JsonConvert.DeserializeObject<Dictionary<string, object>>(options_json));

            return identifier;
        }

        public void StartSound(string identifier)
        {
            getSoundInstance(identifier).Start();
        }

        public void MoveSound(string identifier, float posX, float posY, float posZ)
        {
            getSoundInstance(identifier).Move(posX, posY, posZ);
        }

        public void RotateSound(string identifier, float rotX, float rotY, float rotZ)
        {
            getSoundInstance(identifier).Rotate(rotX, rotY, rotZ);
        }

        private void StopSound(string identifier)
        {
            getSoundInstance(identifier).Stop();
        }

        public void DisposeSound(string identifier)
        {
            getSoundInstance(identifier).Dispose();
        }

        public void AttachSound(string identifier, int entityId)
        {
            getSoundInstance(identifier).Attach(entityId);
        }

        public void DetachSound(string identifier, int entityId)
        {
            getSoundInstance(identifier).Detach();
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
