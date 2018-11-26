using System;
using UnityEngine;
using UnityEngine.UI;
using Secode.Network.Client;
namespace Secode.Network.Client
{
    public class Init : MonoBehaviour
    {
        public string ServerIP = "127.0.0.1:10002";

        public InputField UserIDInput;
        public InputField PasswordInput;

        public Transform GatesContent;
        public GameObject GatesContentDemo;
        public GameObject NewGatePanel;
        public InputField NewGateNameInput;

        public Transform LoginPanel;
        public Transform GatesPanel;

        private void Start()
        {
            ClientController.Default.Init();
            ClientController.ServerIP = "127.0.0.1:10002";

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
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = ClientController.MyUnit.Id.ToString();
                ClientController.MyUnit.GameObject = go;
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
    }

}
