using UnityEngine;
namespace Secode.Network.Client
{
    public static class UnitHelper
    {
        /// <summary>
        /// 消息转为值
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Vector3 ToValue(this Vector3Info info)
        {
            return new Vector3(info.X, info.Y, info.Z);
        }

        /// <summary>
        /// 消息转为值
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Quaternion ToValue(this QuaternionInfo info)
        {
            return new Quaternion(info.X, info.Y, info.Z, info.W);
        }

        /// <summary>
        /// 值转为消息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3Info ToInfo(this Vector3 value)
        {
            return new Vector3Info()
            {
                X = value.x,
                Y = value.y,
                Z = value.z,
            };
        }

        /// <summary>
        /// 值转为消息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static QuaternionInfo ToInfo(this Quaternion value)
        {
            return new QuaternionInfo()
            {
                X = value.x,
                Y = value.y,
                Z = value.z,
                W = value.w,
            };
        }
    }
}
