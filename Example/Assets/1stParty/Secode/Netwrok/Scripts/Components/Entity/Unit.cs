using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Secode.Network.Client
{
    public class Unit : Network.Unit
    {
        /// <summary>
        /// 头部
        /// </summary>
        public Transform Head;

        /// <summary>
        /// 左手手柄
        /// </summary>
        public Transform LeftHand;

        /// <summary>
        /// 右手手柄
        /// </summary>
        public Transform RightHand;

        /// <summary>
        /// 对象
        /// </summary>
        public GameObject GameObject
        {
            get
            {
                return Head.gameObject;
            }
            set
            {
                Head = value.transform;
            }
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return Head.position;
            }
            set
            {
                Head.position = value;
            }
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return Head.rotation;
            }
            set
            {
                Head.rotation = value;
            }
        }

        /// <summary>
        /// 配置角色信息
        /// </summary>
        /// <param name="info"></param>
        public bool SetPlayerInfo(PlayerInfo info)
        {
            PlayerInfo = info;
            return Apply();
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <returns></returns>
        public PlayerInfo GetPlayerInfo()
        {
            var info = new PlayerInfo();
            if (Head != null)
            {
                info.Head = new TransformInfoNoScale();
                info.Head.Position = Head.position.ToInfo();
                info.Head.Rotation = Head.rotation.ToInfo();
            }
            if (LeftHand != null)
            {
                info.LeftHand = new TransformInfoNoScale();
                info.LeftHand.Position = LeftHand.position.ToInfo();
                info.LeftHand.Rotation = LeftHand.rotation.ToInfo();
            }
            if (RightHand != null)
            {
                info.RightHand = new TransformInfoNoScale();
                info.RightHand.Position = RightHand.position.ToInfo();
                info.RightHand.Rotation = RightHand.rotation.ToInfo();
            }
            return info;
        }

        /// <summary>
        /// 应用角色信息数据
        /// </summary>
        /// <returns></returns>
        public bool Apply()
        {
            this.GetComponent<VRMoveComponent>().Apply(PlayerInfo);
            //if (Head != null)
            //{
            //    Head.position = PlayerInfo.Head.Position.ToValue();
            //    Head.rotation = PlayerInfo.Head.Rotation.ToValue();
            //}
            //if (LeftHand != null)
            //{
            //    LeftHand.position = PlayerInfo.LeftHand.Position.ToValue();
            //    LeftHand.rotation = PlayerInfo.LeftHand.Rotation.ToValue();
            //}
            //if (RightHand != null)
            //{
            //    RightHand.position = PlayerInfo.RightHand.Position.ToValue();
            //    RightHand.rotation = PlayerInfo.RightHand.Rotation.ToValue();
            //}
            return true;
        }

        /// <summary>
        /// 是否移动了
        /// </summary>
        public bool IsMoved
        {
            get
            {
                if (PlayerInfo == null) PlayerInfo = GetPlayerInfo();
                if (Head != null && IsTransforMoved(Head, PlayerInfo.Head)) return true;
                if (LeftHand != null && IsTransforMoved(LeftHand, PlayerInfo.LeftHand)) return true;
                if (RightHand != null && IsTransforMoved(RightHand, PlayerInfo.RightHand)) return true;
                return false;
            }
        }

        /// <summary>
        /// 是否移动了
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool IsTransforMoved(Transform transform, TransformInfo info)
        {
            var nsinfo = new TransformInfoNoScale();
            nsinfo.Position = info.Position;
            nsinfo.Rotation = info.Rotation;
            return IsTransforMoved(transform, nsinfo);
        }

        /// <summary>
        /// 是否移动了
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool IsTransforMoved(Transform transform, TransformInfoNoScale info)
        {
            if (transform == null) return false;
            var pos = transform.position;
            if (Mathf.Abs(Vector3.Distance(pos, info.Position.ToValue())) > 0.01) return true;
            var rot = transform.eulerAngles;
            var inforot = info.Rotation.ToValue().eulerAngles;
            if (Mathf.Abs(Vector3.Distance(rot, inforot)) > 5) return true;
            return false;
        }
    }
}