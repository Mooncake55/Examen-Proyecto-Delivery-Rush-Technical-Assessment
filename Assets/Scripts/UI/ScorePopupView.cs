using System;
using TMPro;
using UnityEngine;

namespace DeliveryRushExam.UI
{
    public class ScorePopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private float lifetime = 1.1f;
        [SerializeField] private float moveSpeed = 55f;

        private float age;

        // Referancia al Canva Group
        private CanvasGroup canvasGroup;

        //
        private Action<ScorePopupView> onLifeTimeFinished;

        private void Awake()
        {
            // La llamamos una vez en el awake y no en cada frame
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        }
        public void Setup(string message, Action<ScorePopupView> callback)
        {
            age = 0f;
            messageText.text = message;
            onLifeTimeFinished = callback;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private void Update()
        {
            age += Time.deltaTime;
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
           
            // No es necesario llamarlo cada vez en el update
            // CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f - age / lifetime;
            }

            if (age >= lifetime)
            {
                // Desactivamos el objeto para no hacer un gran gasto de memoria
                onLifeTimeFinished?.Invoke(this);

                // Destruir el objeto es muy costoso para la cpu
                //Destroy(gameObject);
            }
        }
    }
}
