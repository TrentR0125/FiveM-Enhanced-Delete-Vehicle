using System;
using System.Collections.Generic;
using System.Text;

using CitizenFX.FiveM.Client;
using CitizenFX.FiveM.Server;
using CitizenFX.FiveM.Shared.Script;

namespace DeleteVehicle.Client
{
    public class Main : IScript
    {
        public void Initialize()
        {
            API.Log.Info("Client on");
        }

        [OnEvent("DeleteVehicle:Client:ShowNotification")]
        internal void OnShowNotification(string message, bool flash)
        {
            Native.SetNotificationTextEntry("STRING");
            Native.AddTextComponentString(message);
            Native.DrawNotification(flash, true);
        }
    }
}
