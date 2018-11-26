using UnityEngine;

namespace Secode.Network.Client
{
    [ObjectSystem]
    public class VRMoveComponentAwakeSystem : AwakeSystem<VRMoveComponent>
    {
        public override void Awake(VRMoveComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class VRMoveComponentUpdateSystem : UpdateSystem<VRMoveComponent>
    {
        public override void Update(VRMoveComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// VR移动组件
    /// </summary>
    public class VRMoveComponent : Component
    {
        /// <summary>
        /// 设置时数据
        /// </summary>
        public PlayerInfo FromData;

        /// <summary>
        /// 目标数据
        /// </summary>
        public PlayerInfo ToData;

        /// <summary>
        /// 执行时间
        /// </summary>
        public float HandleTime = float.MaxValue;

        /// <summary>
        /// 共计用时
        /// </summary>
        public float TotalHandleTime = 0.1f;

        /// <summary>
        /// 单位对象
        /// </summary>
        public Unit Unit;

        /// <summary>
        /// 是否已经处理完成
        /// </summary>
        public bool IsHandled { get; private set; } = true;

        /// <summary>
        /// 初始化执行
        /// </summary>
        public void Awake()
        {
            Unit = this.GetParent<Unit>();
        }

        /// <summary>
        /// 帧更新执行
        /// </summary>
        public void Update()
        {
            if (IsHandled) return;

            this.HandleTime += Time.deltaTime;

            if (Unit.Head != null)
            {
                HandleMove(Unit.Head, FromData.Head.Position.ToValue(), ToData.Head.Position.ToValue());
                HandleRotate(Unit.Head, FromData.Head.Rotation.ToValue(), ToData.Head.Rotation.ToValue());
            }
            if (Unit.LeftHand != null)
            {
                HandleMove(Unit.LeftHand, FromData.LeftHand.Position.ToValue(), ToData.LeftHand.Position.ToValue());
                HandleRotate(Unit.LeftHand, FromData.LeftHand.Rotation.ToValue(), ToData.LeftHand.Rotation.ToValue());
            }
            if (Unit.RightHand != null)
            {
                HandleMove(Unit.RightHand, FromData.RightHand.Position.ToValue(), ToData.RightHand.Position.ToValue());
                HandleRotate(Unit.RightHand, FromData.RightHand.Rotation.ToValue(), ToData.RightHand.Rotation.ToValue());
            }

            if (this.HandleTime > this.TotalHandleTime) IsHandled = true;
        }

        /// <summary>
        /// 设置目标数据
        /// </summary>
        /// <param name="info"></param>
        public void Apply(PlayerInfo info)
        {
            FromData = Unit.GetPlayerInfo();
            ToData = info;
            HandleTime = 0f;
            IsHandled = false;
        }

        /// <summary>
        /// 移动至
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void HandleMove(Transform transform, Vector3 from, Vector3 to)
        {
            if (this.HandleTime > this.TotalHandleTime)
            {
                transform.position = to;
                return;
            }

            Vector3 v = Vector3.Slerp(from, to, this.HandleTime / this.TotalHandleTime);
            transform.position = v;
        }

        /// <summary>
        /// 旋转至
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void HandleRotate(Transform transform, Quaternion from, Quaternion to)
        {
            if (this.HandleTime > this.TotalHandleTime)
            {
                transform.rotation = to;
                return;
            }

            Quaternion v = Quaternion.Slerp(from, to, this.HandleTime / this.TotalHandleTime);
            transform.rotation = v;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
        }
    }
}