using System;
using System.Threading.Tasks;
using DeliveryRushExam.Data;
using UnityEngine;

namespace DeliveryRushExam.Save
{
    public class SaveManager : MonoBehaviour
    {
        public PlayerProgressData CurrentProgress { get; private set; } = new PlayerProgressData();

        public event Action<PlayerProgressData> ProgressLoaded;

        //private LocalSaveService localSaveService;

        // Implementamos la Interfaz
        private ISaveService saveService;

        private async void Awake()
        {
            //localSaveService = new LocalSaveService();

            // Utiliza el Locator para usar el Servicio
            saveService = ServiceLocator.Get<ISaveService>();
            await LoadProgressAsync();
        }

        public async Task LoadProgressAsync()
        {
            //CurrentProgress = await localSaveService.LoadAsync();

            // Utiliza la interfaz
            CurrentProgress = await saveService.LoadAsync();

            //  INYECCIÓN DE TESTING: Imprimimos los datos cargados en la consola
            string loadedJson = JsonUtility.ToJson(CurrentProgress, true); // El 'true' lo formatea para que sea fácil de leer
            Debug.Log("<color=cyan>--- DATOS CARGADOS DEL JUGADOR ---</color>\n" + loadedJson);

            ProgressLoaded?.Invoke(CurrentProgress);
        }

        public async Task SaveMatchResultAsync(int score, int coins, int completedOrders)
        {
            CurrentProgress.bestScore = Mathf.Max(CurrentProgress.bestScore, score);
            CurrentProgress.totalCoins += coins;
            CurrentProgress.completedOrders += completedOrders;

            // Nivel simple para tener un dato extra persistido.
            CurrentProgress.unlockedLevel = Mathf.Max(CurrentProgress.unlockedLevel, 1 + CurrentProgress.completedOrders / 10);

            //await localSaveService.SaveAsync(CurrentProgress);

            // Utiliza la Interfaz
            await saveService.SaveAsync(CurrentProgress);
        }
    }
}
