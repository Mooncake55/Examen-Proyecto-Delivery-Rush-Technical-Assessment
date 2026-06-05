using DeliveryRushExam.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

namespace DeliveryRushExam.Save
{
    public class UgsCloudSaveService : ISaveService
    {
        private const string ProgressKey = "delivery_rush_progress";

        public async Task<PlayerProgressData> LoadAsync()
        {
            /*Debug.LogWarning("UGS Cloud Save is not enabled. Add UGS packages and implement it.");
            await Task.Yield();*/

            try
            {
                // Pedir las Keys de la Nuve
                var keys = new HashSet<string> {
                    "playerName", "bestScore", "totalCoins",
                    "completedOrders", "unlockedLevel", "lastSaveDate"
                };

                var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
                PlayerProgressData progress = new PlayerProgressData();

                if (savedData.TryGetValue("playerName", out var nameItem)) progress.playerName = nameItem.Value.GetAsString();
                if (savedData.TryGetValue("bestScore", out var scoreItem)) progress.bestScore = scoreItem.Value.GetAs<int>();
                if (savedData.TryGetValue("totalCoins", out var coinsItem)) progress.totalCoins = coinsItem.Value.GetAs<int>();
                if (savedData.TryGetValue("completedOrders", out var ordersItem)) progress.completedOrders = ordersItem.Value.GetAs<int>();
                if (savedData.TryGetValue("unlockedLevel", out var levelItem)) progress.unlockedLevel = levelItem.Value.GetAs<int>();
                if (savedData.TryGetValue("lastSaveDate", out var dateItem)) progress.lastSaveDate = dateItem.Value.GetAsString();

                return progress;
            }
            catch (Exception e)
            {
                Debug.LogError("Error cargando desde UGS: " + e.Message);
                return new PlayerProgressData();
            }
        }

        public async Task SaveAsync(PlayerProgressData progressData)
        {
            try
            {
                // Actualizamos la fecha
                progressData.TouchSaveDate();

                // ✅ Guardamos EXACTAMENTE los 6 datos exigidos en el examen
                var dataToSave = new Dictionary<string, object>
                {
                    { "playerName", progressData.playerName },
                    { "bestScore", progressData.bestScore },
                    { "totalCoins", progressData.totalCoins },
                    { "completedOrders", progressData.completedOrders },
                    { "unlockedLevel", progressData.unlockedLevel },
                    { "lastSaveDate", progressData.lastSaveDate }
                };

                await CloudSaveService.Instance.Data.Player.SaveAsync(dataToSave);
                Debug.Log("¡Los 6 datos requeridos fueron guardados en UGS con éxito!");
            }
            catch (Exception e)
            {
                Debug.LogError("Error guardando en UGS: " + e.Message);
            }

            /*Debug.LogWarning("UGS Cloud Save is not enabled. Add UGS packages and implement it.");
            await Task.Yield();*/
        }
    }
}
