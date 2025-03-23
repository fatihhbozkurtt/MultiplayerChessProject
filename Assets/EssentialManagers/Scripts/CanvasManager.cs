using DG.Tweening;
using Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EssentialManagers.Scripts
{
    public class CanvasManager : MonoSingleton<CanvasManager>
    {
        #region Base Fields

        public enum PanelType
        {
            MainMenu,
            Game,
            Success,
            Fail,
            Online,
            Host
        }

        [Header("Canvas Groups")] public CanvasGroup mainMenuCanvasGroup;
        public CanvasGroup gameCanvasGroup;
        public CanvasGroup successCanvasGroup;
        public CanvasGroup failCanvasGroup;
        public CanvasGroup onlineCanvasGroup;
        public CanvasGroup hostCanvasGroup;

        [Header("Standard Objects")] public Image screenFader;
        public TextMeshProUGUI levelText;

        CanvasGroup[] canvasArray;

        #endregion

        [Header("References")] public Server Server;
        public Client Client;
        [SerializeField] private TMP_InputField adressInputField;
        [SerializeField] Button localGameButton;
        [SerializeField] Button onlineGameButton;
        [SerializeField] Button onlineHostButton;
        [SerializeField] Button onlineConnectButton;
        [SerializeField] Button onlineBackButton;
        [SerializeField] Button hostBackButton;
        protected override void Awake()
        {
            base.Awake();

            canvasArray = new CanvasGroup[System.Enum.GetNames(typeof(PanelType)).Length];

            canvasArray[(int)PanelType.MainMenu] = mainMenuCanvasGroup;
            canvasArray[(int)PanelType.Game] = gameCanvasGroup;
            canvasArray[(int)PanelType.Success] = successCanvasGroup;
            canvasArray[(int)PanelType.Fail] = failCanvasGroup;
            canvasArray[(int)PanelType.Online] = onlineCanvasGroup;
            canvasArray[(int)PanelType.Host] = hostCanvasGroup;

            foreach (CanvasGroup canvas in canvasArray)
            {
                canvas.gameObject.SetActive(true);
                canvas.alpha = 0;
            }

            FadeInScreen(1f);
            ShowPanel(PanelType.MainMenu);


            // HACK: Workaround for FBSDK
            // FBSDK spawns a persistent EventSystem object. Since Unity 2020.2 there must be only one EventSystem objects at a given time.
            // So we must dispose our own EventSystem object if it exists.
            UnityEngine.EventSystems.EventSystem[] eventSystems =
                FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystems.Length > 1)
            {
                Destroy(GetComponentInChildren<UnityEngine.EventSystems.EventSystem>().gameObject);
                Debug.LogWarning("There are multiple live EventSystem components. Destroying ours.");
            }
        }

        void Start()
        {
            levelText.text = "LEVEL " + GameManager.instance.GetTotalStagePlayed().ToString();

            GameManager.instance.LevelStartedEvent += (() => ShowPanel(PanelType.Game));
            GameManager.instance.LevelSuccessEvent += (() => ShowPanel(PanelType.Success));
            GameManager.instance.LevelFailedEvent += (() => ShowPanel(PanelType.Fail));

            localGameButton.onClick.AddListener(OnLocalGameButtonClicked);
            onlineGameButton.onClick.AddListener(OnOnlineGameButtonClicked);
            onlineHostButton.onClick.AddListener(OnlineHostButtonClicked);
            onlineConnectButton.onClick.AddListener(OnlineConnectButtonClicked);
            onlineBackButton.onClick.AddListener(OnlineBackButtonClicked);
            hostBackButton.onClick.AddListener(OnHostBackButtonClicked);
        }

        public void ShowPanel(PanelType panelId)
        {
            int panelIndex = (int)panelId;

            for (int i = 0; i < canvasArray.Length; i++)
            {
                if (i == panelIndex)
                {
                    FadePanelIn(canvasArray[i]);
                }

                else
                {
                    FadePanelOut(canvasArray[i]);
                }
            }
        }

        #region Custom UI Region

        private void OnLocalGameButtonClicked()
        {
            Server.Initialize(8007);
            Client.Initialize("127.0.0.1", 8007);
        }

        private void OnOnlineGameButtonClicked()
        {
            FadePanelOut(mainMenuCanvasGroup);
            ShowPanel(PanelType.Online);
        }

        private void OnlineBackButtonClicked()
        {
            FadePanelOut(onlineCanvasGroup);
            FadePanelIn(mainMenuCanvasGroup);
        }

        private void OnlineConnectButtonClicked()
        {
            Client.Initialize(adressInputField.text, 8007);
        }

        private void OnlineHostButtonClicked()
        {
            Server.Initialize(8007);
            Client.Initialize("127.0.0.1", 8007);
            FadePanelOut(onlineCanvasGroup);
            ShowPanel(PanelType.Host);
        }

        private void OnHostBackButtonClicked()
        {
            Server.Shutdown();
            Client.Shutdown();
            FadePanelOut(hostCanvasGroup);
            FadePanelIn(onlineCanvasGroup);
        }

        #endregion

        #region ButtonEvents

        public void OnTapRestart()
        {
            FadeOutScreen(GameManager.instance.RestartStage, 1);
        }

        public void OnTapContinue()
        {
            FadeOutScreen(GameManager.instance.NextStage, 1);
        }

        #endregion

        #region FadeInOut

        private void FadePanelOut(CanvasGroup panel)
        {
            panel.DOFade(0, 0.35f);
            panel.blocksRaycasts = false;
        }

        private void FadePanelIn(CanvasGroup panel)
        {
            panel.DOFade(1, 0.35f);
            panel.blocksRaycasts = true;
        }

        public void FadeOutScreen(TweenCallback callback, float duration)
        {
            screenFader.DOFade(1, duration).From(0).OnComplete(callback);
        }

        public void FadeOutScreen(float duration)
        {
            screenFader.DOFade(1, duration).From(0);
        }

        public void FadeInScreen(TweenCallback callback, float duration)
        {
            screenFader.DOFade(0, duration).From(1).OnComplete(callback);
        }

        public void FadeInScreen(float duration)
        {
            screenFader.DOFade(0, duration).From(1);
        }

        #endregion
    }
}