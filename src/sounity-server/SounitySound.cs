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
    class SounitySound: Sounity.ISounitySound
    {
        private string identifier;
        private IDictionary<string, object> options;
        private bool isPlaying = false;
        private long startTime = 0;
        private string source = "";
        public List<Player> playersInRange = new List<Player>();

        public SounitySound(string identifier, string source, Dictionary<string, object> options = null)
        {
            this.identifier = identifier;
            this.source = source;

            if (options == null)
                options = new Dictionary<string, object>();

            this.options = options;

            NotifyPlayers("CreateSound", identifier, source, getOptionJSON());
        }

        private void NotifyPlayers(string EventName, params object[] args)
        {
            foreach (var player in playersInRange)
                player.TriggerEvent($"Sounity:{EventName}", identifier, args);
        }

        public void Attach(int netId)
        {

            options["attachTo"] = netId;

            NotifyPlayers("AttachSound", netId);
        }

        public void Detach()
        {
            options.Remove("attachTo");

            NotifyPlayers("DetachSound");
        }
        public void Start()
        {
            startTime = API.GetGameTimer();
            isPlaying = true;

            NotifyPlayers("StartSound");
        }


        public void Move(float posX, float posY, float posZ)
        {
            options["posX"] = posX;
            options["posY"] = posY;
            options["posZ"] = posZ;

            Detach();
            NotifyPlayers("MoveSound", posX, posY, posZ);
        }

        public void Rotate(float rotX, float rotY, float rotZ)
        {
            options["rotX"] = rotX;
            options["rotY"] = rotY;
            options["rotZ"] = rotZ;

            Detach();
            NotifyPlayers("RotateSound", rotX, rotY, rotZ);
        }

        public void Dispose()
        {
            NotifyPlayers("DisposeSound");
        }

        public void Stop()
        {
            isPlaying = false;
            NotifyPlayers("DisposeSound");
        }

        private string getOptionJSON()
        {
            return JsonConvert.SerializeObject(options);
        }

        internal void PlayerInRange(Player player)
        {
            playersInRange.Add(player);

            player.TriggerEvent("Sounity:CreateSound", identifier, source, getOptionJSON());

            if (isPlaying)
            {
                player.TriggerEvent("Sounity:StartSound", identifier, startTime);
            }
        }

        internal void PlayerOutOfRange(Player player)
        {
            playersInRange.Remove(player);

            player.TriggerEvent("Sounity:DisposeSound", identifier);
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
                return API.GetEntityCoords(API.NetworkGetEntityFromNetworkId(attachedNetId));
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

        public string getIdentifer()
        {
            return identifier;
        }
    }
}
