using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace SounityServer
{
    class SounityServer : BaseScript
    {
        protected SounityServerAPI sounityServerAPI;
        int debug = -1;
        public SounityServer()
        {
            sounityServerAPI = new SounityServerAPI(Exports);
            Tick += OnTick;
        }

        private async Task OnTick()
        {
            sounityServerAPI.Tick();
            TriggerClientEvent("Sounity:ServerTime", API.GetGameTimer());
        }
    }
}
