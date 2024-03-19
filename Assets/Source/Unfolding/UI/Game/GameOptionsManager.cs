using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unfolding.Utils;

namespace Unfolding.UI
{
    namespace Game
    {
        /// <summary>
        /// This class manages a radio group in game UI view which represents in-game options.
        /// </summary>
        public sealed class GameOptionsManager : UI.Utils.RadioGroup, ITaggable
        {
            // CLASS'S ATTRIBUTES
            public string TAG => "Game Options";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>    
            public static GameOptionsManager Current { get; private set; }
            
            /// <summary>
            /// The text that will be displayed as a popup the first time we open the settings panel
            /// </summary>
            private static string s_settingsInfo = "This panel allows you to adjust some settings,\n" +
                                                   "like audio volume camera sensitivity, etc...\n" +
                                                   "You can click on icons located on left of the panel\n" +
                                                   "to switch between settings tabs."; 

            /// <summary>
            /// A boolean which ensure that we only show the popup related to settings once 
            /// </summary>
            private bool _displayedOnce = false; 

            // SCENE'S OBJECTS
            [Header("Options icons settings")]
            [Space(5)]

            /// <summary>
            /// List of sprites of each button when there are not selected
            /// </summary>
            [Tooltip("The non-selected sprites for each button (in the same order than their order in hierarchy).")]
            public List<Sprite> RegularSprites = new List<Sprite>();

            /// <summary>
            /// List of sprites of each button when there are selected
            /// </summary>
            [Tooltip("The selected sprites for each button (in the same order than their order in hierarchy).")]
            public List<Sprite> SelectedSprites = new List<Sprite>();

            /// <summary>
            /// Awake method, called first in script life's cycle.
            /// </summary>
            protected override void Awake()
            {
                base.Awake(); // Calling parent method
                GameOptionsManager.Current = this; // Instanciating only instance of this script (Singleton pattern)
            }

            /// <summary>
            /// Start method, called before first frame.
            /// It's mainly used here for initialization of panel's components.
            /// </summary>
            protected override void Start()
            {
                base.Start(); // Calling parent method 

                // If there is not as many sprites than there is buttons in radio group, then we show a warning
                if (RegularSprites.Count != RadioButtons.Count || SelectedSprites.Count != RadioButtons.Count)
                {
                    Debug.unityLogger.LogWarning(TAG, "At least one regular or selected sprite is missing.");
                }
                else
                {
                    this.UpdateIcons(""); // Calling method that will initialize each icon associated to radio buttons
                }
            }

            /// <summary>
            /// This method allows to update state of the radio group according to a radio button selection.
            /// </summary>
            /// <remarks>Assignation to current selected radio button is done in parent class.</remarks>
            /// <param name="clickedButtonName">Name of the button which just has been selected.</param>
            public override void UpdateRadioGroup(in string clickedButtonName)
            {
                base.UpdateRadioGroup(clickedButtonName); // Calling parent method

                // At this point, button _current should have been settled
                // We juste need now to update sprites for radio buttons and do animations if it's necessary
                if (CurrentSelected is null)
                {
                    this.UpdateIcons(""); // Update icon (here, each icon will be set at its regular state)
                    AnimatorsManager.Current.PlayAnimation(clickedButtonName);
                }
                else
                {
                    this.UpdateIcons(clickedButtonName); // Update icon (here, only selected button will have its icon set at its selected state)
                    AnimatorsManager.Current.PlayAnimation(clickedButtonName);

                    // Adding additional proccessing for setting panel
                    if (clickedButtonName is "Settings" && _displayedOnce is false) 
                    {
                        PopupsManager.Current.DisplayPopup("", 10.0f, GameOptionsManager.s_settingsInfo);
                        _displayedOnce = true;
                    }
                }

                /*
                    NOTE: we don't precise to PlayAnimation method which action of animator should be played,
                    as this parameter is already managed in AnimatorsManager class.
                */
            }

            /// <summary>
            /// This method allows to update all icons of radio buttons inside the radio group.
            /// </summary>
            /// <param name="clickedButtonName">Name of the button just selected, "" if there is none.</param>
            private void UpdateIcons(in string clickedButtonName)
            {
                for (int i = 0; i < RadioButtons.Count; ++i) // Travelling all accross radio group
                {
                    // Update each icon according to name of clicked button
                    RadioButtons[i].GetComponent<Image>().sprite = RadioButtons[i].name.Equals(clickedButtonName) ? SelectedSprites[i] : RegularSprites[i];
                } 
            }
        }
    }
}