namespace Secode.Network.Client
{
    [ObjectSystem]
    public class UnitSyncComponentAwakeSystem : AwakeSystem<UnitSyncComponent, Unit>
    {
        public override void Awake(UnitSyncComponent self, Unit unit)
        {
            self.Awake(unit);
        }
    }

    [ObjectSystem]
    public class UnitSyncComponentUpdateSystem : UpdateSystem<UnitSyncComponent>
    {
        public override void Update(UnitSyncComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 单位同步组件
    /// </summary>
    public class UnitSyncComponent : Component
    {
        public Unit ControlUnit;

        public void Awake(Unit unit)
        {
            ControlUnit = unit;
        }

        public void Update()
        {
            if (ControlUnit.IsMoved)
            {
                var info = new Frame_PlayerMove();
                info.PlayerInfo = ControlUnit.GetPlayerInfo();
                ControlUnit.PlayerInfo = info.PlayerInfo;
                SessionComponent.Instance.Session.Send(info);
            }
        }
    }
}
