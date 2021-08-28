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


            API.RegisterNuiCallbackType("sounity:ready");
            EventHandlers["__cfx_nui:sounity:ready"] += new Action<IDictionary<string, object>, CallbackDelegate>((data, cb) =>
            {
                TriggerEvent("Sounity:Ready");
                cb(new { success = true});
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
            if (startTime != null) sounityClientAPI.StartSound(identifier, (long) startTime);
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

        private async Task BrowserUpdaterTick()
        {
            sounityClientAPI.Tick();
        }
    }
}
