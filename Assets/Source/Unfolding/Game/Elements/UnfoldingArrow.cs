using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unfolding.Game
{    
    namespace Elements
    {
        /// <summary>
        /// A small class containing all possible rotations (quaternions) for directionnal arrows.
        /// </summary>
        public static class ArrowRotations
        {
            // Directionnal arrows rotations are quaternions where only y and z values should evolve
            public static Quaternion Down = Quaternion.Euler(0, 0, 0);
            public static Quaternion Up = Quaternion.Euler(0, 0, 180);
            public static Quaternion Right = Quaternion.Euler(0, 0, 90);
            public static Quaternion Left = Quaternion.Euler(0, 0, -90);
            public static Quaternion Back = Quaternion.Euler(0, 90, 90);
            public static Quaternion Front = Quaternion.Euler(0, -90, 90);
        }

        /// <summary>
        /// This script is attached to 3d arrow object prefab in the Unfolding project.
        /// It allows it to be highlighted when cursor pointer is on it.
        /// </summary>
        public class UnfoldingArrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
        {
            // CLASS'S ATTRIBUTES
            /// <summary>
            /// A boolean to store if arrow is looking at the positive side on its axis or not.
            /// </summary>
            [HideInInspector] public bool LookAtPositive { get; private set; }

            /// <summary>
            /// A boolean to store if arrow has been clicked or not
            /// </summary>
            [HideInInspector] public bool HasBeenClicked { get; private set; } 
            
            [Header("Materials settings")]
            [Space(5f)]

            [Tooltip("The material which represents base material for arrows.")]
            [SerializeField] private Material _baseMaterial;

            [Tooltip("The material which represents highlighted material for arrows.")]
            [SerializeField] private Material _highlightedMaterial;

            private Renderer _objectRenderer; // Attribute to store object's mesh renderer

            /// <summary>
            /// Awake method, called first in script life's cycle.
            /// </summary>
            private void Awake() 
            {   
                _objectRenderer = this.GetComponentInChildren<Renderer>();
                _objectRenderer.material = _baseMaterial; // Initializing default material for 3D arrow

                // Initializing look at positive boolean to know wher
                LookAtPositive = 
                    this.transform.eulerAngles.Equals(ArrowRotations.Up.eulerAngles) || 
                    this.transform.eulerAngles.Equals(ArrowRotations.Right.eulerAngles) ||
                    this.transform.eulerAngles.Equals(ArrowRotations.Front.eulerAngles); 
            }

            /// <summary>
            /// This method allows to define some actions when cursor pointer enter arrow object's zone.
            /// </summary>
            /// <param name="eventData">N/D</param>
            public void OnPointerEnter(PointerEventData eventData)
            {
                _objectRenderer.material = _highlightedMaterial;
            }

            /// <summary>
            /// This method allows to define some actions when cursor pointer exit arrow object's zone.
            /// </summary>
            /// <param name="eventData">N/D</param>
            public void OnPointerExit(PointerEventData eventData) 
            {
                _objectRenderer.material = _baseMaterial;
            }

            /// <summary>
            /// This method allows to define some actions when user click on the arrow.
            /// In our case, clicking on a arrow notify PolyCubeManager that user has chose a direction.
            /// </summary>
            /// <param name="eventData">N/D</param>
            public void OnPointerClick(PointerEventData eventData)
            {
                HasBeenClicked = true;
                

            }
        }
    }
}