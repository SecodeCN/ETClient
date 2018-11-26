using UnityEngine;
namespace Secode.Network.Client
{
    [MessageHandler]
    public class Frame_PlayerMoveHandler : AMHandler<Frame_PlayerMove>
    {
        protected override void Run(Session session, Frame_PlayerMove message)
        {
            Unit unit = (Unit)Game.Scene.GetComponent<ClientUnitComponent>().Get(message.Id);
            if (unit == null) return;
            if (unit.Id == ClientController.MyUnit.Id) return;
            unit.PlayerInfo = message.PlayerInfo;
            unit.Apply();
        }
    }
}