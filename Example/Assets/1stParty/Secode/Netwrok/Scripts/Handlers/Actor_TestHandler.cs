﻿namespace Secode.Network.Client
{
    [MessageHandler]
    public class Actor_TestHandler : AMHandler<Actor_Test>
    {
        protected override void Run(Session session, Actor_Test message)
        {
            Log.Debug(message.Info);
        }
    }
}