using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Secode.Network.Client
{
    public class ClientController : MonoBehaviour
    {
        #region Default

        private static ClientController m_Default;

        /// <summary>
        /// 默认客户端
        /// </summary>
        public static ClientController Default
        {
            get
            {
                if (m_Default != null) return m_Default;
                var go = new GameObject("ClientController");
                DontDestroyOnLoad(go);
                m_Default = go.AddComponent<ClientController>();
                return m_Default;
            }
            set
            {
                m_Default = value;
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            Log.LogAction += delegate (LogType type, object msg)
            {
                if (msg == null) return;
                print(type.ToString() + " : " + msg.ToString());
            };
            Session.DebugFrameMessage = true;
            ClientManager.Init(typeof(ClientController));
        }

        #endregion

        #region 事件

        protected virtual void Update()
        {
            ClientManager.Update();
        }

        protected virtual void LateUpdate()
        {
            ClientManager.LateUpdate();
        }

        protected virtual void OnApplicationQuit()
        {
            ClientManager.OnApplicationQuit();
        }

        #endregion

        #region 拓展参数

        /// <summary>
        /// 服务器IP
        /// </summary>
        public static string ServerIP
        {
            get
            {
                return ClientManager.ServerIP;
            }
            set
            {
                ClientManager.ServerIP = value;
            }
        }

        /// <summary>
        /// 可连接房间名称
        /// </summary>
        public static List<string> Gates
        {
            get
            {
                return ClientManager.Gates;
            }
        }

        /// <summary>
        /// 玩家角色
        /// </summary>
        public static Player MyPlayer
        {
            get
            {
                return ClientManager.MyPlayer;
            }
            set
            {
                ClientManager.MyPlayer = value;
            }
        }

        /// <summary>
        /// 玩家单位
        /// </summary>
        public static Unit MyUnit
        {
            get
            {
                return ClientManager.MyUnit;
            }
            set
            {
                ClientManager.MyUnit = value;
            }
        }

        #endregion

        #region 拓展方法

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> Login(string userid, string password)
        {
            var ans = await ClientManager.Login(userid, password);
            return ans;
        }

        /// <summary>
        /// 连接房间
        /// </summary>
        /// <param name="gatename"></param>
        /// <returns></returns>
        public static async Task<bool> Link(string gatename)
        {
            var ans = await ClientManager.Link(gatename);
            return ans;
        }

        /// <summary>
        /// 新建房间
        /// </summary>
        /// <param name="gatename"></param>
        /// <returns></returns>
        public static async Task<bool> NewGate(string gatename)
        {
            var ans = await ClientManager.NewGate(gatename);
            return ans;
        }

        /// <summary>
        /// 进入地图开始接收帧同步消息
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> EnterMap()
        {
            var ans = await ClientManager.EnterMap();
            MyUnit.AddComponent<VRMoveComponent>();
            MyUnit.AddComponent<UnitSyncComponent, Unit>(MyUnit);
            return ans;
        }

        #endregion
    }

    public class ClientManager
    {
        #region 初始化

        /// <summary>
        /// 客户端是否初始化
        /// </summary>
        public static bool IsInit = false;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init(Type GameType)
        {
            if (IsInit) return;
            IsInit = true;
            try
            {
                #region 多线程汇入主线程处理
                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
                #endregion

                #region 事件系统注入代码库
                Game.EventSystem.Add(DLLType.Model, typeof(Game).Assembly);
                Game.EventSystem.Add(DLLType.Game, GameType.Assembly);
                #endregion

                #region 设置启用的组件库

                #region 联网模块基础组件库
                Game.Scene.AddComponent<NetOuterComponent>(); // 外部网络交互组件
                Game.Scene.AddComponent<ClientPlayerComponent>(); // 玩家列表管理组件
                Game.Scene.AddComponent<ClientUnitComponent>(); // 对象列表管理组件
                #endregion

                #region 接收消息的处理组件
                Game.Scene.AddComponent<OpcodeTypeComponent>(); // 消息类型处理组件
                Game.Scene.AddComponent<MessageDispatherComponent>(); // 普通消息分发组件
                Game.Scene.AddComponent<ClientFrameComponent>(); // 帧同步数据处理组件
                #endregion

                #endregion
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #endregion

        #region 事件

        public static void Update()
        {
            if (!IsInit) return;
            OneThreadSynchronizationContext.Instance.Update();
            Game.EventSystem.Update();
        }

        public static void LateUpdate()
        {
            if (!IsInit) return;
            Game.EventSystem.LateUpdate();
        }

        public static void OnApplicationQuit()
        {
            if (!IsInit) return;
            Game.Close();
        }

        #endregion

        #region 拓展参数

        /// <summary>
        /// 服务器IP
        /// </summary>
        public static string ServerIP = "";

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public static IPEndPoint ServerEndPoint
        {
            get
            {
                return NetworkHelper.ToIPEndPoint(ServerIP);
            }
        }

        /// <summary>
        /// 可连接房间名称
        /// </summary>
        public static List<string> Gates = new List<string>();

        /// <summary>
        /// 房间的连接Session
        /// </summary>
        public static Session GateSession
        {
            get
            {
                if (Game.Scene.GetComponent<SessionComponent>() == null)
                {
                    Game.Scene.AddComponent<SessionComponent>();
                }
                return Game.Scene.GetComponent<SessionComponent>().Session;
            }
            set
            {
                if (Game.Scene.GetComponent<SessionComponent>() == null)
                {
                    Game.Scene.AddComponent<SessionComponent>();
                }
                Game.Scene.GetComponent<SessionComponent>().Session = value;
            }
        }

        /// <summary>
        /// 玩家角色
        /// </summary>
        public static Player MyPlayer
        {
            get
            {
                ClientPlayerComponent com = Game.Scene.GetComponent<ClientPlayerComponent>();
                if (com == null) com = Game.Scene.AddComponent<ClientPlayerComponent>();
                return com.MyPlayer;
            }
            set
            {
                ClientPlayerComponent com = Game.Scene.GetComponent<ClientPlayerComponent>();
                if (com == null) com = Game.Scene.AddComponent<ClientPlayerComponent>();
                com.MyPlayer = value;
            }
        }

        /// <summary>
        /// 玩家单位
        /// </summary>
        public static Unit MyUnit
        {
            get
            {
                ClientUnitComponent com = Game.Scene.GetComponent<ClientUnitComponent>();
                if (com == null) com = Game.Scene.AddComponent<ClientUnitComponent>();
                return (Unit)com.MyUnit;
            }
            set
            {
                ClientUnitComponent com = Game.Scene.GetComponent<ClientUnitComponent>();
                if (com == null) com = Game.Scene.AddComponent<ClientUnitComponent>();
                com.Add(value);
                com.MyUnit = value;
            }
        }

        #endregion

        #region 拓展方法

        #region 登录

        /// <summary>
        /// 房间连接钥匙
        /// </summary>
        private static long LinkKey;

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> Login(string userid, string password)
        {
            try
            {
                Session session = Game.Scene.GetComponent<NetOuterComponent>().Create(ServerEndPoint);

                R2C_Login info = (R2C_Login)await session.Call(new C2R_Login() { UserID = userid, Password = password });

                if (info.Error == 0)
                {
                    LinkKey = info.Key;

                    Gates = new List<string>();
                    foreach (var key in info.Gates)
                    {
                        Gates.Add(key);
                    }
                    return true;
                }
                else
                {
                    Log.Info(info.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return false;
            }
        }

        #endregion

        #region 连接房间

        /// <summary>
        /// 连接房间
        /// </summary>
        /// <param name="gatename"></param>
        /// <returns></returns>
        public static async Task<bool> Link(string gatename)
        {
            try
            {
                if (LinkKey == 0) return false;
                if (string.IsNullOrEmpty(gatename)) return false;
                if (MyPlayer != null) return false;

                GateSession = Game.Scene.GetComponent<NetOuterComponent>().Create(ServerEndPoint);

                G2C_Link info = (G2C_Link)await Game.Scene.GetComponent<SessionComponent>().Session.Call(new C2G_Link() { Key = LinkKey, Gate = gatename });

                if (info.Error == 0)
                {
                    // 创建Player
                    MyPlayer = ComponentFactory.CreateWithId<Player>(info.PlayerId);
                    return true;
                }
                else
                {
                    Log.Info(info.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return false;
            }
        }

        #endregion

        #region 新建房间

        /// <summary>
        /// 新建房间
        /// </summary>
        /// <param name="gatename"></param>
        /// <returns></returns>
        public static async Task<bool> NewGate(string gatename)
        {
            try
            {
                if (LinkKey == 0) return false;
                if (string.IsNullOrEmpty(gatename)) return false;

                var Session = Game.Scene.GetComponent<NetOuterComponent>().Create(ServerEndPoint);

                R2C_NewGate info = (R2C_NewGate)await Session.Call(new C2R_NewGate() { Key = LinkKey, Name = gatename });

                if (info.Error == 0 && info.Gate == gatename)
                {
                    Gates.Add(info.Gate);
                    return true;
                }
                else
                {
                    Log.Info(info.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return false;
            }
        }

        #endregion

        #region 进入地图开始接收帧同步消息

        /// <summary>
        /// 进入地图开始接收帧同步消息
        /// </summary>
        public static async Task<bool> EnterMap()
        {
            if (MyPlayer == null) return false;
            try
            {
                G2C_EnterMap info = (G2C_EnterMap)await SessionComponent.Instance.Session.Call(new C2G_EnterMap());
                if (info.Error == 0)
                {
                    MyPlayer.UnitId = info.UnitId;

                    MyUnit = ComponentFactory.CreateWithId<Unit>(info.UnitId);

                    return true;
                }
                else
                {
                    Log.Info(info.Message);
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return false;
            }
        }

        #endregion

        #endregion
    }
}

