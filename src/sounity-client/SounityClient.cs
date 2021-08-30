using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SounityClient
{
    class SounityClient : BaseScript
    {
        protected SounityClientAPI sounityClientAPI;

        public SounityClient()
        {
            var EVENT_PREFIX = "Sounity";
            EventHandlers[$"{EVENT_PREFIX}:ServerTime"] += new Action<long>(onServerTime);
            EventHandlers[$"{EVENT_PREFIX}:CreateSound"] += new Action<string, string, string>(onCreateSound);
            EventHandlers[$"{EVENT_PREFIX}:StartSound"] += new Action<string, long>(onPlaySound);
            EventHandlers[$"{EVENT_PREFIX}:StopSound"] += new Action<string>(onStopSound);
            EventHandlers[$"{EVENT_PREFIX}:MoveSound"] += new Action<string, float, float, float>(onMoveSound);
            EventHandlers[$"{EVENT_PREFIX}:RotateSound"] += new Action<string, float, float, float>(onRotateSound);
            EventHandlers[$"{EVENT_PREFIX}:AttachSound"] += new Action<string, int>(onAttachSound);
            EventHandlers[$"{EVENT_PREFIX}:DetachSound"] += new Action<string>(onDetachSound);
            EventHandlers[$"{EVENT_PREFIX}:DisposeSound"] += new Action<string>(onDisposeSound);
            EventHandlers[$"{EVENT_PREFIX}:AddFilter"] += new Action<string, string>(onAddFilter);
            EventHandlers[$"{EVENT_PREFIX}:AddFilters"] += new Action<string, string>(onAddFilters);
            EventHandlers[$"{EVENT_PREFIX}:RemoveFilter"] += new Action<string, string>(onRemoveFilter);
            EventHandlers[$"{EVENT_PREFIX}:AddListenerFilter"] += new Action<string>(onAddListenerFilter);
            EventHandlers[$"{EVENT_PREFIX}:RemoveListenerFilter"] += new Action<string>(onRemoveListenerFilter);


            API.RegisterNuiCallbackType("sounity:ready");
            EventHandlers["__cfx_nui:sounity:ready"] += new Action<IDictionary<string, object>, CallbackDelegate>((data, cb) =>
            {
                TriggerEvent("Sounity:Ready");
                sounityClientAPI.CreateFilter("underwater", "biquad", JsonConvert.SerializeObject(new {
                    Q = 1,
                    frequency = 100,
                    type = "lowpass"
                }));
                cb(new { success = true });
            });

            API.RegisterNuiCallbackType("sounity:get-defaults");
            EventHandlers["__cfx_nui:sounity:get-defaults"] += new Action<IDictionary<string, object>, CallbackDelegate>((data, cb) =>
            {
                var config = Sounity.Config.GetInstance();

             

                cb(JsonConvert.SerializeObject(new {
                    volume = config.Get("volume", 1f),
                    outputType = config.Get("outputType", "sfx"),
                    loop = config.Get("loop", false),

                    posX = config.Get("posX", 0f),
                    posY = config.Get("posY", 0f),
                    posZ = config.Get("posZ", 0f),
                    rotX = config.Get("rotX", 0f),
                    rotY = config.Get("rotY", 0f),
                    rotZ = config.Get("rotZ", 0f),

                    panningModel = config.Get("panningModel", "HRTF"),
                    distanceModel = config.Get("distanceModel", "inverse"),
                    maxDistance = config.Get("maxDistance", 500f),
                    refDistance = config.Get("refDistance", 3f),
                    rolloffFactor = config.Get("rolloffFactor", 1f),
                    coneInnerAngle = config.Get("coneInnerAngle", 360f),
                    coneOuterAngle = config.Get("coneOuterAngle", 0f),
                    coneOuterGain = config.Get("coneOuterGain", 0f),
                }));
            });

            sounityClientAPI = new SounityClientAPI(Exports);
            Tick += BrowserUpdaterTick;
        }

        private void onCreateSound(string identifier, string source, string json_options)
        {
            sounityClientAPI.CreateSound(identifier, source, json_options);
        }


        private void onPlaySound(string identifier, long startTime)
        {
            if (startTime != null) sounityClientAPI.StartSound(identifier, startTime);
            else sounityClientAPI.StartSound(identifier);
        }

        private void onStopSound(string identifier)
        {
            sounityClientAPI.StopSound(identifier);
        }

        private void onMoveSound(string identifier, float posX, float posY, float posZ)
        {
            sounityClientAPI.MoveSound(identifier, posX, posY, posZ);
        }

        private void onRotateSound(string identifier, float rotX, float rotY, float rotZ)
        {
            sounityClientAPI.RotateSound(identifier, rotX, rotY, rotZ);
        }

        private void onAttachSound(string identifier, int netId)
        {
            sounityClientAPI.AttachSound(identifier, netId);
        }

        private void onDetachSound(string identifier)
        {
            sounityClientAPI.DetachSound(identifier);
        }

        private void onDisposeSound(string identifier)
        {
            
            sounityClientAPI.DisposeSound(identifier);
        }

        private void onServerTime(long serverTime)
        {
            sounityClientAPI.setServerTime(serverTime);
        }

        private void onAddFilter(string identifier, string filterName)
        {
            sounityClientAPI.AddSoundFilter(identifier, filterName);
        }

        private void onAddFilters(string identifier, string filterNames_json)
        {
            List<string> filterNames = JsonConvert.DeserializeObject<List<string>>(filterNames_json);

            foreach(var filterName in filterNames)
                onAddFilter(identifier, filterName);
        }

        private void onRemoveFilter(string identifier, string filterName)
        {
            sounityClientAPI.RemoveSoundFilter(identifier, filterName);
        } 
        private void onAddListenerFilter(string filterName)
        {
            sounityClientAPI.AddListenerFilter(filterName);
        }  
        private void onRemoveListenerFilter(string filterName)
        {
            sounityClientAPI.RemoveListenerFilter(filterName);
        }

        private async Task BrowserUpdaterTick()
        {
            sounityClientAPI.Tick();
        }
    }
}
