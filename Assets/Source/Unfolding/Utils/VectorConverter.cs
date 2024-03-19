using UnityEngine;

namespace Unfolding.Utils
{
    /// <summary>
    /// Utilitary class to convert between C# built-in Vector3 <c>(System.Numerics.Vector3)</c> and UnityEngine Vector3 <c>(UnityEngine.Vector3)</c>
    /// </summary>
    public static class VectorConverter
    {
        /// <summary>
        /// Convert <c>UnityEngine.Vector3</c> to <c>System.Numerics.Vector3</c>
        /// </summary>
        /// <param name="vec">The UnityEngine Vector3</param>
        /// <returns>The built-in converted vector</returns>
        public static System.Numerics.Vector3 ToSystemVector(in UnityEngine.Vector3 vec)
        {
            return new System.Numerics.Vector3(vec.x, vec.y, vec.z);
        }

        /// <summary>
        /// Convert <c>System.Numerics.Vector3</c> to <c>UnityEngine.Vector3</c>
        /// </summary>
        /// <param name="vec">The built-in Vector3</param>
        /// <returns>The UnityEngine Vector3 converted vector</returns>
        public static UnityEngine.Vector3 ToUnityEngineVector(in System.Numerics.Vector3 vec)
        {
            return new UnityEngine.Vector3(vec.X, vec.Y, vec.Z);
        }
    }
}
