using UnityEngine;

namespace Unfolding.Game.Utils
{
    /// <summary>
    /// This class handles the generation of spheres in the game which are used as a debug dot in the game
    /// </summary>
    public class DebugSphere : MonoBehaviour
    {
        /// <summary>
        /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
        /// </summary>
        public static DebugSphere Current { get; private set; }

        /// <summary>
        /// Awake method, first method called in script's life cycle.
        /// </summary>
        void Awake()
        {
            DebugSphere.Current = this;
        }

        /// <summary>
        /// Generate a sphere on the scene to visualize a specific point in the space
        /// </summary>
        /// <param name="position">The position of the sphere</param>
        /// <param name="scalePos">The scale of the position of the sphere</param>
        /// <param name="scaleSize">The scale of the size of the sphere</param>
        public void GenerateDebug(Vector3 position, float scalePos, float scaleSize = 1)
        {
            GameObject go =
                Instantiate(
                    Resources.Load("Prefabs/Debug Sphere/DebugSpherePrefabs"),
                    DebugSphere.Current.transform
                ) as GameObject;
            go.transform.localPosition = position * scalePos;
            go.transform.localScale *= scaleSize;
        }

        /// <summary>
        /// Destroy every debug sphere in the scene
        /// </summary>
        public void CleanDebugSphere()
        {
            foreach (Transform t in DebugSphere.Current.transform)
            {
                Destroy(t.gameObject);
            }
        }

        /// <summary>
        /// A method that is runned when the class is destroyed
        /// </summary>
        void OnDestroy()
        {
            CleanDebugSphere();
        }
    }
}
