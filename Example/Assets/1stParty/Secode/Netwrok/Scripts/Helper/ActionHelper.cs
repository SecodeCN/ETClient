using System;
using UnityEngine.UI;

namespace Secode.Network.Client
{
    public static class ActionHelper
    {
        public static void Add(this Button.ButtonClickedEvent buttonClickedEvent, Action action)
        {
            buttonClickedEvent.AddListener(() => { action?.Invoke(); });
        }
    }
}