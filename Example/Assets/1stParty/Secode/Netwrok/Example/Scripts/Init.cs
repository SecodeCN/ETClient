using System;
using UnityEngine;
using UnityEngine.UI;
using Secode.Network.Client;
namespace Secode.Network.Client
{
    public class Init : MonoBehaviour
    {
        public const string ServerIP = "192.168.16.252:10002";

        [Header("登录配置")]

        public InputField UserIDInput;
        public InputField PasswordInput;

        [Header("Gate配置")]

        public Transform GatesContent;
        public GameObject GatesContentDemo;
        public GameObject NewGatePanel;
        public InputField NewGateNameInput;

        [Header("面板配置")]

        public Transform LoginPanel;
        public Transform GatesPanel;

        [Header("集合配置")]

        public GameObject UI;

        [Header("角色配置")]

        public GameObject MyPlayer;
        public GameObject PlayerDemo;

        private void Start()
        {
            ClientController.Default.Init();
            ClientController.ServerIP = ServerIP;

            Log.Info(ClientController.ServerIP);

            UI.gameObject.SetActive(true);

            LoginPanel.gameObject.SetActive(true);
            GatesPanel.gameObject.SetActive(false);

            var LoginButton = LoginPanel.Find("Login").GetComponent<Button>();
            LoginButton.onClick.Add(Login);

            GatesContentDemo.gameObject.SetActive(false);
            NewGatePanel.SetActive(false);
            var NewButton = GatesPanel.Find("New").GetComponent<Button>();
            NewButton.onClick.Add(ShowNewGatePanel);
            var ExitButton = GatesPanel.Find("Exit").GetComponent<Button>();
            ExitButton.onClick.Add(Exit);
            var NewGateOKButton = NewGatePanel.transform.Find("OK").GetComponent<Button>();
            NewGateOKButton.onClick.Add(NewGate);

            ClientController.CreateUnitAction = CreateUnit;
        }

        public async void Login()
        {
            var ans = await ClientController.Login(UserIDInput.text, PasswordInput.text);

            if (!ans) return;

            LoginPanel.gameObject.SetActive(false);
            GatesPanel.gameObject.SetActive(true);

            LoadGatesPanel();
        }

        public void LoadGatesPanel()
        {
            var list = ClientController.Gates;
            while (GatesContent.childCount > 1)
            {
                if (GatesContent.GetChild(0).gameObject == GatesContentDemo)
                {
                    GatesContentDemo.transform.SetAsLastSibling();
                }
                else
                {
                    DestroyImmediate(GatesContent.GetChild(0).gameObject);
                }
            }
            foreach (var key in list)
            {
                var go = Instantiate(GatesContentDemo);
                go.name = key;
                go.transform.SetParent(GatesContent);
                go.transform.GetComponentInChildren<Text>().text = key;
                go.transform.SetAsFirstSibling();
                go.SetActive(true);
            }
        }

        public void ShowNewGatePanel()
        {
            NewGatePanel.SetActive(true);
        }

        public async void NewGate()
        {
            var ans = await ClientController.NewGate(NewGateNameInput.text);

            if (ans)
            {
                NewGatePanel.SetActive(false);
                LoadGatesPanel();
            }
            else
            {
                LoginPanel.gameObject.SetActive(true);
                GatesPanel.gameObject.SetActive(false);
            }
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private async void EnterMap()
        {
            var ans = await ClientController.EnterMap();

            if (ans)
            {
                ClientController.MyUnit.GameObject = MyPlayer;
                UI.SetActive(false);
            }
            else
            {
                LoginPanel.gameObject.SetActive(true);
                GatesPanel.gameObject.SetActive(false);
            }
        }

        public void LoadGate(GameObject go)
        {
            LoadGate(go.name);
        }

        public async void LoadGate(string name)
        {
            var ans = await ClientController.Link(name);

            GatesPanel.gameObject.SetActive(false);

            if (ans)
            {
                EnterMap();
            }
            else
            {
                LoginPanel.gameObject.SetActive(true);
            }
        }

        public void CreateUnit(Unit unit)
        {
            var go = Instantiate(PlayerDemo);
            go.transform.position = new Vector3(0, 1.2f, 0);
            go.name = unit.Id.ToString();
            unit.GameObject = go;

            unit.AddComponent<VRMoveComponent>();
        }
    }

}
