using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unfolding.Utils;

namespace Unfolding.UI
{
    namespace Game
    {
        /// <summary>
        /// This class is attached in game's view to the parent object of all popups in the UI.
        /// You should pass by the singleton instance of this class to display one of them.
        /// </summary>
        public sealed class PopupsManager : MonoBehaviour, ITaggable
        {
            // CLASS'S ATTRIBUTES
            public string TAG => "Game - Popups Manager";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            public static PopupsManager Current { get; private set; }

            // SCENE'S OBJECTS
            
            /// <summary>
            /// List of possible popups in the game
            /// </summary>
            private List<GameObject> _popups = new List<GameObject>();

            // OTHERS ATTRIBUTES
            /// <summary>
            /// A small struct used to store informations about a popup, 
            /// before sending it to DisplayPopup coroutine.
            /// </summary>
            private struct PopupInfo
            {
                public string Name { get; } // Property to store name of the popup to display
                public float DisplayTime { get; } // Property to store for how long popup should be displayed
                public string Text { get; } // Text to display on text component of popup

                public PopupInfo(float displayTime, string text, string name = "Default Popup") // Constructor (name is optionnal)
                {
                    this.Name = name;
                    this.DisplayTime = displayTime;
                    this.Text = text;
                }
            }

            /// <summary>
            /// Awake method, called first in script life's cycle.
            /// Used here to initialize singleton instance of this class.
            /// </summary>
            private void Awake() => PopupsManager.Current = this;

            /// <summary>
            /// Start method, called before first frame.
            /// Used here to retrieve game objects in childrens linked to a popup object.
            /// </summary>
            private void Start()
            {
                // Retrieving all of the childrens
                List<Transform> childrens = new List<Transform>(this.GetComponentsInChildren<Transform>());

                // Filtering them to have only popups roots
                childrens = childrens.FindAll(children => children.name.Contains(" Popup"));

                // Adding the game objects associated to roots to popups list
                childrens.ForEach(children => _popups.Add(children.gameObject));

                // Disable all game objects found
                _popups.ForEach(popup => popup.SetActive(false));
            }

            /// <summary>
            /// This function allows another class to display a popup for a specific time when its needed.
            /// It requires several information to create an object called popupinfo which will be used in a coroutine.
            /// </summary>
            /// <param name="popupName">String, name of the popup to display. Set to "" if default popup is wanted.</param>
            /// <param name="displayTime">Float, time in seconds during which popup will be displayed.</param>
            /// <param name="textToDisplay">String, text to display on popup's tmp component.</param>
            public void DisplayPopup(in string popupName, in float displayTime, in string textToDisplay) 
            {
                // Creating a popupinfo according to given parameters
                PopupInfo popupInfo = popupName is "" ? new(displayTime, textToDisplay) : new(displayTime, textToDisplay, popupName);

                // Launch coroutine with popupinfo
                this.StartCoroutine("DisplayPopupCoroutine", popupInfo);
            }

            /// <summary>
            /// This coroutine allows DisplayPopup function to display a popup for a specific time.
            /// </summary>
            /// <param name="popupInfo">PopupInfo struct, containing all required informations to display a popup.</param>
            /// <returns>N/D</returns>
            private IEnumerator DisplayPopupCoroutine(PopupInfo popupInfo)
            {
                // Adding a little delay at the start to not display popup too early
                yield return new WaitForSeconds(1f);

                // Retrieving popup inside popups list
                GameObject popupToDisplay = _popups.Find(popup => popup.name.Equals(popupInfo.Name));

                // Setting text and activating object
                popupToDisplay.GetComponentInChildren<TMP_Text>().text = popupInfo.Text;
                popupToDisplay.SetActive(true);

                // Delaying deactivation of the popup
                yield return new WaitForSeconds(popupInfo.DisplayTime);

                // Disable the popup
                popupToDisplay.SetActive(false);
            }
        }
    }
}