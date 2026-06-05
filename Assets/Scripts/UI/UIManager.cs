using System.Collections.Generic;
using DeliveryRushExam.Core;
using DeliveryRushExam.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeliveryRushExam.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ScoreManager scoreManager;

        [Header("HUD")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text ordersCountText;

        [Header("Orders")]
        [SerializeField] private RectTransform ordersContainer;
        [SerializeField] private OrderButtonView orderButtonPrefab;

        [Header("Popups")]
        [SerializeField] private RectTransform popupsContainer;
        [SerializeField] private ScorePopupView scorePopupPrefab;

        [Header("Panels")]
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject resultsPanel;
        [SerializeField] private TMP_Text resultsText;

        private readonly List<OrderButtonView> orderViews = new List<OrderButtonView>();

        private int lastTimerValue = -1;

       // Referencia a la Queue
        private readonly Queue<ScorePopupView> popupQueue = new Queue<ScorePopupView>();

        private void Awake()
        {
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }

            if (orderManager == null)
            {
                orderManager = FindFirstObjectByType<OrderManager>();
            }

            if (scoreManager == null)
            {
                scoreManager = FindFirstObjectByType<ScoreManager>();
            }
        }

        private void OnEnable()
        {
            orderManager.OrdersChanged += RefreshOrderList;
            scoreManager.OrderScored += ShowScorePopup;
            // Suscribirse al evento
            scoreManager.ScoreChanged += UpdateScoreUI;
        }

        private void OnDisable()
        {
            if (orderManager != null)
            {
                orderManager.OrdersChanged -= RefreshOrderList;
            }

            if (scoreManager != null)
            {
                scoreManager.OrderScored -= ShowScorePopup;
            }
            // Suscribirse al evento
            if (scoreManager != null)
            {
                scoreManager.ScoreChanged -= UpdateScoreUI;
            }
        }

        private void Update()
        {
            if (scoreManager == null || gameManager == null)
            {
                return;
            }

            // Primero obtengo el segundo actual redondeado

            int currentSecond = Mathf.CeilToInt(gameManager.RemainingTime);

            // Segundo actualizo la UI cada segundo y no en cada instancia de Update
            // Ahora 

            if (currentSecond != lastTimerValue)
            {
                timerText.text = "Time: " + currentSecond;
                lastTimerValue = currentSecond;

                for (int i = 0; i < orderViews.Count; i++)
                {
                orderViews[i].Refresh();
                }
            }

            // Cargar el Score en cada frame es innecesario, es mejor realizarlo unicamente cuando hay un cambio de score, time, order o coins

            /* scoreText.text = "Score: " + scoreManager.Score;
            coinsText.text = "Coins: " + scoreManager.Coins;
            timerText.text = "Time: " + Mathf.CeilToInt(gameManager.RemainingTime);
            ordersCountText.text = "Orders: " + orderManager.ActiveOrders.Count; */

            // Cargar el Canva en cada frame sobrecarga el uso de la CPU

            /*Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null && ordersContainer != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(ordersContainer);
            }*/
        }

        public void ShowGameplay()
        {
            gameplayPanel.SetActive(true);
            resultsPanel.SetActive(false);
            RefreshOrderList();
        }

        public void ShowResults(int score, int coins, int completedOrders, PlayerProgressData progressData)
        {
            gameplayPanel.SetActive(false);
            resultsPanel.SetActive(true);

            resultsText.text =
                "Delivery Rush Results\n" +
                "Score: " + score + "\n" +
                "Coins earned: " + coins + "\n" +
                "Completed orders: " + completedOrders + "\n" +
                "Best score: " + progressData.bestScore + "\n" +
                "Total coins: " + progressData.totalCoins;
        }

        private void RefreshOrderList()
        {
            // Es Redundante buscar al OrderManager en este metodo ya que el OrderManager es buscado en Awake

            //OrderManager runtimeOrderManager = FindFirstObjectByType<OrderManager>();

            /*if (runtimeOrderManager != null)
            {
                orderManager = runtimeOrderManager;
            }*/

            /*for (int i = 0; i < orderViews.Count; i++)
            {
                Destroy(orderViews[i].gameObject);
            }*/
            
            /*for (int i = 0; i < orders.Count; i++)
            {
                OrderButtonView view = Instantiate(orderButtonPrefab, ordersContainer);
                view.gameObject.SetActive(true);
                view.Setup(orders[i], orderManager.CompleteOrder);
                orderViews.Add(view);
            }*/

            IReadOnlyList<OrderData> orders = orderManager.ActiveOrders;

            // Agregar Nuevas Ordenes

            while (orderViews.Count < orders.Count)
            {
                OrderButtonView newView = Instantiate(orderButtonPrefab, ordersContainer);
                orderViews.Add(newView);
            }

            // En vez de Eliminar las ordenes ahora solo las desactivo gastando mucho menos memoria
            // Realizo Object Pooling Pre-instanciando los objetos del canva necesarios

            for (int i = 0; i < orderViews.Count; i++)
            {
                if (i < orders.Count)
                {
                    orderViews[i].gameObject.SetActive(true);
                    orderViews[i].Setup(orders[i], orderManager.CompleteOrder);
                }
                else
                {
                    orderViews[i].gameObject.SetActive(false);
                }
            }
            ordersCountText.text = "Orders: " + orders.Count;

            // Actualizar el Canva unicamente cuado se agregue o quite una orden es mas eficiente
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null && ordersContainer != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(ordersContainer);
            }
        }

        private void ShowScorePopup(OrderData order)
        {
            ScorePopupView popup;

            // Buscar en la fila si hay alguno en queue
            if (popupQueue.Count > 0)
            {
                popup = popupQueue.Dequeue();
            }
            else
            {
                popup = Instantiate(scorePopupPrefab, popupsContainer);
            }
            popup.transform.localPosition = new Vector3(Random.Range(-90f, 90f), Random.Range(-25f, 35f), 0f);
            popup.Setup("+" + order.rewardPoints + " points", ReturnPopupToQueue);
            popup.gameObject.SetActive(true);

            /*
            ScorePopupView popup = Instantiate(scorePopupPrefab, popupsContainer);
            popup.gameObject.SetActive(true);
            popup.transform.localPosition = new Vector3(Random.Range(-90f, 90f), Random.Range(-25f, 35f), 0f);
            popup.Setup("+" + order.rewardPoints + " points");*/
        }

        // Método para actualizar el Score
        private void UpdateScoreUI(int score, int coins, int completedOrders)
        {
            scoreText.text = "Score: " + score;
            coinsText.text = "Coins: " + coins;
        }


        // Metodo para apagar el Popup

        private void ReturnPopupToQueue(ScorePopupView popup)
        {
            popup.gameObject.SetActive(false); // Lo ocultamos
            popupQueue.Enqueue(popup);         // Lo metemos al final de la fila
        }
    }
}
