using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SounityClient
{
    class SounitySound: Sounity.ISounitySound
    {
        private string identifier;
        private string source;
        private Dictionary<string, object> options;
        private bool underwater = false;

        public SounitySound(string identifier, string source, Dictionary<string, object> options)
        {
            this.identifier = identifier;
            this.source = source;
            this.options = options;

            var initialPosition = getPosition();
            float waterHeight = 0;
            API.GetWaterHeightNoWaves(initialPosition.X, initialPosition.Y, initialPosition.Z, ref waterHeight);

            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "createSound",
                identifier,
                source,
                options,
            }));

            if(initialPosition.Z < waterHeight)
            {
                AddFilter("underwater");
                underwater = true;
            }
        }

        public void Start()
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "startSound",
                identifier,
            }));
        }

        public void Start(long startTime)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "startSound",
                identifier,
                startTime,
            }));
        }

        public void Move(Vector3 pos)
        {
            Move(pos.X, pos.Y, pos.Z);
        }

        public void Move(float posX, float posY, float posZ)
        {
            options["posX"] = posX;
            options["posY"] = posY;
            options["posZ"] = posZ;

            float waterHeight = 0;
            API.GetWaterHeightNoWaves(posX, posY, posZ, ref waterHeight);

            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "moveSound",
                identifier,
                posX,
                posY,
                posZ,
            }));

            bool isPlayerDiving = API.IsPedSwimmingUnderWater(API.PlayerPedId());

            if (isPlayerDiving && underwater == false)
            {
                AddFilter("underwater");
                underwater = true;
            } else if (isPlayerDiving && underwater == true)
            {
                RemoveFilter("underwater");
                underwater = false;
            }
        }

        public void Rotate(Vector3 rot)
        {
            Rotate(rot.X, rot.Y, rot.Z);
        }

        public void Rotate(float rotX, float rotY, float rotZ)
        {
            options["rotX"] = rotX;
            options["rotY"] = rotY;
            options["rotZ"] = rotZ;

            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "rotateSound",
                identifier,
                rotX,
                rotY,
                rotZ
            }));
        }

        public void Attach(int netId)
        {
            options["attachTo"] = netId;
        }

        public void Detach()
        {
            options.Remove("attachTo");
        }

        public void Stop()
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "stopSound",
                identifier,
            }));
        }

        public void Dispose()
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "disposeSound",
                identifier,
            }));
        }

        public bool isAttached()
        {
            return options.ContainsKey("attachTo");
        }

        public int GetAttachTo()
        {
            if (!isAttached()) return 0;
            return Convert.ToInt32(options["attachTo"]);
        }

        public Vector3 getPosition()
        {
            var attachedNetId = GetAttachTo();

            if (attachedNetId != 0)
            {
                return API.GetEntityCoords(API.NetworkGetEntityFromNetworkId(attachedNetId), false);
            }

            float posX = 0, posY = 0, posZ = 0;

            if (options.ContainsKey("posX"))      
                posX = Convert.ToSingle(options["posX"]);
            if (options.ContainsKey("posY"))
                posY = Convert.ToSingle(options["posY"]);
            if (options.ContainsKey("posZ"))
                posZ = Convert.ToSingle(options["posZ"]);

            return new Vector3(posX, posY, posZ);
        }

        public void AddFilter(string filterName)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "addSoundFilter",
                identifier,
                filterName
            }));
        }

        public void RemoveFilter(string filterName)
        {
            API.SendNuiMessage(JsonConvert.SerializeObject(new
            {
                type = "removeSoundFilter",
                identifier,
                filterName
            }));
        }
    }
}
