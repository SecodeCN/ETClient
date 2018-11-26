using UnityEngine;
using Secode.Network;

namespace Secode.Network.Client
{
    [MessageHandler]
    public class Actor_CreateUnitsHandler : AMHandler<Actor_CreateUnits>
    {
        protected override void Run(Session session, Actor_CreateUnits message)
        {
            ClientUnitComponent unitComponent = Game.Scene.GetComponent<ClientUnitComponent>();
            foreach (UnitInfo unitInfo in message.Units)
            {
                if (unitComponent.Contain(unitInfo.UnitId)) continue;
                Unit unit = ComponentFactory.CreateWithId<Unit>(unitInfo.UnitId);
                unitComponent.Add(unit);
                unit.AddComponent<VRMoveComponent>();

                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = unit.Id.ToString();
                unit.GameObject = go;
            }
        }
    }
}