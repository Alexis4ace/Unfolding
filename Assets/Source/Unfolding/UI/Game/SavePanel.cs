using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unfolding.Utils;
using Unfolding.Game.Gameplay;
using UnityEditor;
using System.IO;

using SFB;

namespace Unfolding.UI
{
    namespace Game
    {
        /// <summary>
        /// This class manages save panel from game UI view.
        /// </summary>
        public class SavePanel : MonoBehaviour, ITaggable
        {
            // CLASS'S ATTRIBUTES
            public string TAG => "Game - Save Panel";

            /// <summary>
            /// A string variable which contains the message when a polycube is saved
            /// </summary>
            private static string s_savingSucceeded = "Game has been successfully saved."; 

            // SCENE'S OBJECTS
            [Header("Save controllers")]
            [Space(4.0f)]

            [Tooltip("The input associated to save's name given by user.")] [SerializeField]
            /// <summary>
            /// The input associated to save's name given by user
            /// </summary>
            private TMP_InputField _saveName;

            [Tooltip("The button associated with user's confirmation action.")] [SerializeField]
            /// <summary>
            /// The button associated with user's confirmation action
            /// </summary>
            private Button _validate;

            [Tooltip("The button associated to set a save path")]
            [SerializeField]
            /// <summary>
            /// The button associated to set a save path
            /// </summary>
            private Button _path;
            
            /// <summary>
            /// Start method, called before first frame.
            /// </summary>
            private void Start()
            {
                _path = this.transform.Find("SaveAs").GetComponent<Button>();
                //if click on save path
                _path.onClick.AddListener(() =>
                {
                    StandaloneFileBrowser.SaveFilePanelAsync("", "", _saveName.text, "cub", path =>
                    {
                        if (path.Length != 0)
                        {
                            Unfolding.Game.IO.IOFile.setPath(path);
                            Unfolding.Game.IO.IOFile.WriteFile(Path.GetFileNameWithoutExtension(path), PolyCubeManager.Current.PolyCubeGraph);
                            PopupsManager.Current.DisplayPopup("Save Popup", 5f, SavePanel.s_savingSucceeded);
                            GameOptionsManager.Current.UpdateRadioGroup("Save");
                        }
                    });

                });

                // If all variables had been assigned in the inspector
                if (this.IsOneAttributeMissing() is false)
                {
                    // Then we can configure them
                    _saveName.contentType = TMP_InputField.ContentType.Alphanumeric;
                    _saveName.onValueChanged.AddListener(newValue =>
                    {
                        _validate.interactable = (newValue != "");
                    });

                    _validate.interactable = false;
                    _validate.onClick.AddListener(() => 
                    {
                        Unfolding.Game.IO.IOFile.WriteFile(_saveName.text, PolyCubeManager.Current.PolyCubeGraph);
                        GameOptionsManager.Current.UpdateRadioGroup("Save"); // Updating game options radio group (which will hide Save Panel)
                        PopupsManager.Current.DisplayPopup("Save Popup", 5f, SavePanel.s_savingSucceeded); // Display saving confirmation for a certain time
                    });
                }
                else
                {
                    // Otherwise we throw a debug message to notify a warning
                    Debug.unityLogger.LogWarning(TAG, "One or more serialized variable of this class had not been assigned in the inspector");
                }
            }

            /// <summary>
            /// This method allows to know at a specific time if one or more serialized variables
            /// had not been assigned in the inspector.
            /// </summary>
            /// <returns>Boolean, true if one of the variables is missing, false either.</returns>
            private bool IsOneAttributeMissing()
            {
                return (
                    null == _saveName == _validate 
                );
                
                /*
                    NOTE: why not using is keyword for test up here ?
                    Because test if "x is null" will SYSTEMATICALLY return false if x had not been
                    setted EXPLICITELY to null in the code.
                */
            }
        }
    }
}