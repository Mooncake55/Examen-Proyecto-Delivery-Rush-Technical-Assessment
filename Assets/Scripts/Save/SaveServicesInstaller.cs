using UnityEngine;

namespace DeliveryRushExam.Save
{
    public class SaveServicesInstaller : MonoBehaviour
    {
        public enum SaveMode
        {
            Local,
            Cloud
        }

        [SerializeField] private SaveMode saveMode = SaveMode.Local;

        private void Awake()
        {
            // Registro inicial para que el proyecto funcione.
            // El punto de extensión esperado es registrar una abstracción común.
            if (saveMode == SaveMode.Local)
            {
                //ServiceLocator.Register(new LocalSaveService());

                // Registrar la Interfaz
                ServiceLocator.Register<ISaveService>(new LocalSaveService());
                return;
            }

            //ServiceLocator.Register(new UgsCloudSaveService());
            // Registrar la interfaz para Cloud 
            ServiceLocator.Register<ISaveService>(new UgsCloudSaveService());
        }
    }
}
