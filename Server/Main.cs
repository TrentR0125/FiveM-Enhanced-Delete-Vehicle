using System.Numerics;

using CitizenFX.FiveM.Server;
using CitizenFX.FiveM.Server.Entities;
using CitizenFX.FiveM.Shared.Script;
using CitizenFX.FiveM.Shared.Serialization;

namespace DeleteVehicle.Server
{
    public class Main : IScript
    {
        public async void Initialize()
        {
            API.Log.Info("Delete vehicle initialized");
        }

        [OnCommand("deletevehicle")]
        internal async void OnDeleteVehicle([FromSource] Player player)
        {
            API.Log.Info($"Player {player.Name} is deleting a vehicle");

            try
            {
                Ped? ped = player.Ped;

                if (ped == null)
                {
                    API.Log.Warn("Player ped is null");

                    return;
                }

                int vehicleInt = Native.GetVehiclePedIsIn(ped.Handle, false);
                Vehicle? vehicle = vehicleInt != 0 ? new Vehicle(vehicleInt) : null;

                if (vehicle is not null && Native.DoesEntityExist(vehicle.Handle))
                {
                    DeleteVehicle(player, vehicle);

                    return;
                }

                // If the ped is not in a vehicle, try getting the closest vehicle to them
                vehicle = GetClosestVehicle(ped);

                if (vehicle is null || !Native.DoesEntityExist(vehicle.Handle))
                {
                    API.Log.Warn("No vehicle found near player");

                    return;
                }

                DeleteVehicle(player, vehicle);
            }
            catch
            {
                //
            }
        }

        internal void DeleteVehicle(Player player, Vehicle vehicle)
        {
            try
            {
                if (!Native.DoesEntityExist(vehicle.Handle))
                {
                    API.Log.Warn("Vehicle does not exist");

                    return;
                }

                API.Vehicles.Remove(vehicle);

                API.Log.Info($"{player.Name} ({player.Handle}) deleted vehicle {vehicle.Model} ({vehicle.NumberPlateText})");
            }
            catch
            {
                //
            }
        }

        internal Vehicle? GetClosestVehicle(Ped ped)
        {
            Vector3 pedPos = ped.Position;

            byte[] vehicleHandles = Native.GetAllVehicles();

            Dictionary<int, float> vehicles = new();

            foreach (int handle in vehicleHandles)
            {
                float distance = Vector3.DistanceSquared(Native.GetEntityCoords(handle), pedPos);

                if (distance <= 5.0f)
                {
                    vehicles.Add(handle, distance);
                }
            }

            vehicles.OrderBy(i => i.Value);

            return vehicles.Count == 0 ? null : new Vehicle(vehicles.First().Key);
        }
    }
}
