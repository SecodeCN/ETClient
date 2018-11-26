namespace Secode.Network.Client
{
    // 分发数值监听
    [Event(EventIdType.TestHotfixSubscribMonoEvent)]
    public class TestHotfixSubscribMonoEvent_LogString : AEvent<string>
    {
        public override void Run(string info)
        {
            Log.Info(info);
        }
    }
}
