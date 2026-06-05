using System.Threading.Tasks;
using UnityEngine;

#if DELIVERY_RUSH_UGS
using Unity.Services.Authentication;
using Unity.Services.Core;
#endif  

namespace DeliveryRushExam.UGS
{
    public class UgsInitializer : MonoBehaviour
    {
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private bool verboseLogs = true;

        public bool IsReady { get; private set; }

        private async void Start()
        {
            if (initializeOnStart)
            {
                await InitializeAsync();
            }
        }

        public async Task InitializeAsync()
        {
#if DELIVERY_RUSH_UGS
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            IsReady = true;

            if (verboseLogs)
            {
                Debug.Log("UGS ready. PlayerId: " + AuthenticationService.Instance.PlayerId);
            }
#else
            IsReady = false;
            if (verboseLogs)
            {
                Debug.Log("UGS initializer present. Install UGS packages and define DELIVERY_RUSH_UGS to enable it.");
            }

            await Task.Yield();
#endif
        }
    }
}
