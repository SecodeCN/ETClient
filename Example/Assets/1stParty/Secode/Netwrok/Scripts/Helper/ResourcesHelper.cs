using UnityEngine;

namespace Secode.Network.Client
{
    public static class ResourcesHelper
    {
        public static UnityEngine.Object Load(string path)
        {
            return Resources.Load(path);
        }
    }
}
