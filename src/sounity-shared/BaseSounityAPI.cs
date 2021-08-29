using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sounity
{
    public abstract class BaseSounityAPI<T> where T: ISounitySound
    {
        protected static int idCounter = 1;
        protected string identifierPrefix;
        protected Dictionary<string, T> sounds = new Dictionary<string, T>();

        public BaseSounityAPI(ExportDictionary Exports, string identifierPrefix)
        {
            Exports.Add("CreateSound", new Func<string, string, string>(CreateSound));
            Exports.Add("StartSound", new Action<string>(StartSound));
            Exports.Add("MoveSound", new Action<string, float, float, float>(MoveSound));
            Exports.Add("RotateSound", new Action<string, float, float, float>(RotateSound));
            Exports.Add("StopSound", new Action<string>(StopSound));
            Exports.Add("DisposeSound", new Action<string>(DisposeSound));
            Exports.Add("AttachSound", new Action<string, int>(AttachSound));
            Exports.Add("DetachSound", new Action<string>(DetachSound));

            this.identifierPrefix = identifierPrefix;
        }

        protected T getSoundInstance(string identifier)
        {
            if (!sounds.ContainsKey(identifier))
            {
                throw new Exception($"Unknown identifier '{identifier}'");
            }

            return sounds[identifier];
        }

        public string CreateSound(string identifier, string source, string options_json = null)
        {
            if (options_json == null)
                options_json = "{}";

            ConstructorInfo c = typeof(T).GetConstructor(new[] { typeof(string), typeof(string), typeof(Dictionary<string, object>) });
            if (c == null)
                throw new InvalidOperationException(string.Format("A constructor for type '{0}' was not found.", typeof(T)));

            sounds[identifier] = (T)c.Invoke(new object[] { identifier, source, JsonConvert.DeserializeObject<Dictionary<string, object>>(options_json) });

            return identifier;
        }

        public string CreateSound(string source, string options_json = null)
        {
            var identifier = $"{identifierPrefix}_{idCounter++}";

            return CreateSound(identifier, source, options_json);
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
        public void StopSound(string identifier)
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

        public void DetachSound(string identifier)
        {
            getSoundInstance(identifier).Detach();
        }
    }
}
