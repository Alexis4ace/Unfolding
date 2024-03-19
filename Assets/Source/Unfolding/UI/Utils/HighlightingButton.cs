using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Unfolding.UI
{
    namespace Utils
    {
        /// <summary>
        /// This class allows a button to have its TMP_Text highlighted on click. Require a TMP_Text children.
        /// </summary>
        [RequireComponent(typeof(Button))]
        public class HighlightingButton : MonoBehaviour
        {
            // CLASS'S ATTRIBUTES
            [Header("Clicky button parameters")]
            [Space(5.0f)]

            /// <summary>
            /// A button that tells wether the button is highlighted or not
            /// </summary>
            /// <remarks> It is useful in order to change its color when it is highlighted </remarks>
            [HideInInspector] public bool IsHighlighted;

            [Tooltip("Should be default color of button's text (when not selected).")] 
            [SerializeField] private Color _defaultColor = Color.white;

            [Tooltip("Should be highlighted color of button's text (when selected).")] 
            [SerializeField] private Color _highlightedColor = Color.yellow;

            private bool _hasTMPTextChildren; // A simple bool to store if current object have a tmp text in its childs

            /// <summary>
            /// Awake method, called first in script life's cycle.
            /// </summary>
            private void Awake() 
            {
                _hasTMPTextChildren = this.GetComponentInChildren<TMP_Text>() is null ? false : true; // Verifying if object have a tmp text child
                IsHighlighted = false;
            }

            /// <summary>
            /// Start method, called before first frame.
            /// </summary>
            private void Start()
            {
                if (_hasTMPTextChildren)
                {
                    // Adding a listener to button component
                    this.GetComponent<Button>().onClick.AddListener(() => 
                    {
                        // Modifying text color according to current state (highlighted or not ?)
                        this.GetComponentInChildren<TMP_Text>().color = IsHighlighted ? _defaultColor : _highlightedColor;
                        IsHighlighted = !IsHighlighted;
                    });
                }
                else
                {
                    Debug.unityLogger.LogWarning(this.name, "This object has no tmp text child, and yet it has a clicky button script attached." +
                                                            "You should fix that.");
                }
            }

            /// <summary>
            /// An utilitary method which allows another class to reset button state on demand.
            /// </summary>
            public void ResetButton()
            {
                if (_hasTMPTextChildren)
                {
                    this.GetComponentInChildren<TMP_Text>().color = _defaultColor;
                    IsHighlighted = false;
                }
            }
        }
    }
}