using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unfolding.Utils;

namespace Unfolding.UI
{
    namespace Utils
    {
        /// <summary>
        /// This class can be extend to provide to one script a RadioGroup's functionnal logic and features.
        /// </summary>
        public abstract class RadioGroup : MonoBehaviour
        {
            // SCENE'S OBJECTS
            /// <summary>
            /// List of radio buttons inside the radio group 
            /// </summary>
            protected List<Button> RadioButtons = new List<Button>();

            /// <summary>
            /// Attribute to store current name of selected radio button
            /// </summary>
            protected string CurrentSelected; 

            // OTHER ATTRIBUTES
            [Header("Soundable settings")]
            [Space(5)]

            /// <summary>
            /// AudioClip that will be played whenever one button is pressed
            /// </summary>
            [Tooltip("AudioClip that will be played whenever one button is pressed.")] public AudioClip Sound;

            /// <summary>
            /// Awake method, called first in script life's cycle.
            /// </summary>
            protected virtual void Awake() => this.GetComponentsInChildren<Button>(RadioButtons); // Getting back each button present in radio group

            /// <summary>
            /// Start method, called before first frame.
            /// This method is protected and virtual in case where a sub-classe wants to add new features.
            /// </summary>
            protected virtual void Start()
            {
                // Now we should configure the functionnal logic of a radio group
                for (int ind = 0; ind < RadioButtons.Count; ++ind)
                {
                    int currentButton = ind; // This is needed because by default, lambda captures value by reference (and we do not wants it)
                    RadioButtons[currentButton].onClick.AddListener(delegate { this.UpdateRadioGroup(RadioButtons[currentButton].name); });
                }
            }

            /// <summary>
            /// This method allows to update state of the radio group according to a radio button selection.
            /// This method is public so that other classes than sub-classes can ask to radio group to update.
            /// If clickedButton is already the one which is selected, then we unselect it.
            /// </summary>
            /// <remarks>This function simply assign current selected button to button param, other modifications should be done in sub-classes.</remarks>
            /// <param name="clickedButtonName">Name of the button which just has been selected.</param>
            public virtual void UpdateRadioGroup(in string clickedButtonName) 
            {
                UI.Utils.Helper.PlayEffectSound(Sound); // Playing sound on click
                CurrentSelected = (clickedButtonName.Equals(CurrentSelected)) ? null : clickedButtonName; // Update CurrentSelected variable
            } 
        }

        /// <summary>
        /// This class extends the RadioGroup one and add several features to provide small animations.
        /// </summary>
        public abstract class AnimatedRadioGroup : RadioGroup, ITaggable
        {
            // CLASS'S ATTRIBUTES
            public abstract string TAG { get; }

            [Header("Tabs icons settings")]
            [Space(5)]

            [Tooltip("The default sprites for each button. Must be setted in the same order than radio buttons are declared in hierarchy.")]
            /// <summary>
            /// The default sprites for each button when the are not selected.
            /// </summary>
            [SerializeField] protected List<Sprite> DefaultIcons;

            [Tooltip("The selected sprites for each button. Must be setted in the same order than radio buttons are declared in hierarchy.")]
            /// <summary>
            /// The default sprites for each button when the are selected.
            /// </summary>
            [SerializeField] protected List<Sprite> SelectedIcons;

            /// <summary>
            /// Start method, called before first frame.
            /// We use it here to add new features to radio buttons.
            /// </summary>
            protected override void Start()
            {
                // Before adding animations to radio buttons, we must check if all of the sprites have been setted
                if (DefaultIcons.Count == SelectedIcons.Count && SelectedIcons.Count == RadioButtons.Count)
                {
                    base.Start(); // Calling parent method
                    /*
                    NOTE:
                        why are we calling parent method in a if statement ? Because added features to base radio group are done
                        by overriding UpdateRadioGroup method, and that implies that all sprites have been setted correctly.
                    */
                }
                else
                {
                    Debug.unityLogger.LogWarning(TAG, "One or several sprites for radio buttons's icons are missing.");
                }
            }

            /// <summary>
            /// This method is overrided here to add a mechanism of animated selection to radio buttons (by modifying their sprites on select / deselect).
            /// </summary>
            /// <param name="clickedButtonName">Name of the button which just has been selected.</param>
            public override void UpdateRadioGroup(in string clickedButtonName)
            {
                // We don't call parent's method yet because we first need to update sprite of previously selected radio button !

                // Getting back previously selected button and its index
                Button previous = RadioButtons.Find(radioButton => radioButton.name.Equals(CurrentSelected));
                int previousIndex = RadioButtons.FindIndex(0, RadioButtons.Count, radioButton => radioButton.name.Equals(CurrentSelected));

                if (previous is not null) // If we have found one button, it means that _current was not null before handling with user selection
                {
                    previous.GetComponent<Image>().sprite = DefaultIcons[previousIndex]; // Then we reset its sprite
                }

                base.UpdateRadioGroup(clickedButtonName); // Calling parent method, to update _current attribute

                // Now we have to update _current icon only if it is not null (it can be in case of user clicked on already selected radio button)
                if (CurrentSelected is not null)
                {
                    // Getting index in radio buttons of _current
                    int currentIndex = RadioButtons.FindIndex(radioButton => radioButton.name.Equals(CurrentSelected));

                    // Updating its sprite
                    RadioButtons.Find(radioButton => radioButton.name.Equals(CurrentSelected)).GetComponent<Image>().sprite = SelectedIcons[currentIndex];
                }
            }
        }
    }
}