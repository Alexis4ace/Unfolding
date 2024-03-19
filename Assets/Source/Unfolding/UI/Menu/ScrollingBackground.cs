using UnityEngine;
using UnityEngine.UI;

namespace Unfolding.UI
{
    namespace Menu
    {
        /// <summary> 
        /// This class can be attached to a raw image to make scrolling. 
        /// </summary>
        [RequireComponent(typeof(RawImage))]
        public class ScrollingBackground : MonoBehaviour
        {
            [Header("Scrolling background settings")]
            [Space(5.0f)]

            /// <summary>
            /// Scrolling values on axis x.
            /// </summary>
            [Tooltip("Scrolling values on axis x")]
            public float X;

            /// <summary>
            /// Scrolling values on axis x.
            /// </summary>            
            [Tooltip("Scrolling values on axis y")]
            public float Y;

            /// <summary>
            /// Update method, called once per frame. <br/>
            /// </summary>
            private void Update()
            {
                var backgroundImage = this.GetComponent<RawImage>();
                backgroundImage.uvRect =
                    new Rect(
                        backgroundImage.uvRect.position + new Vector2(X, Y) * Time.deltaTime,
                        backgroundImage.uvRect.size
                    );
            }
        }
    }
}