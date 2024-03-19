using UnityEngine;
using TMPro;
using Unfolding.Utils;

namespace Unfolding.UI
{
    namespace Utils
    {
        /// <summary>
        /// This class assembles some utilitary functions that are used in several game's scripts.
        /// </summary>
        public static class Helper 
        {
            // CLASS'S ATTRIBUTES
            private const string _TAG = "UI - Helper";

            // OTHER ATTRIBUTES
            /// <summary>
            /// Default volume for scene's audio sources
            /// </summary>
            public const float DefaultVolume = 0.1f;

            /// <summary>
            /// This function allows to play a short sound from an .aif, .wav, .mp3 or .ogg file.
            /// </summary>
            /// <param name="clipToPlay">Sound function will play.</param>
            public static void PlayEffectSound(in AudioClip clipToPlay)
            {
                // AudioSource affected to effects sounds is attached to Effects object (son of Audio object in hierarchy)
                var source = GameObject.Find("Effects").GetComponent<AudioSource>();

                if (Time.unscaledTime > 1.0f) // Test if game have been running since one second to prevent some sounds to be played too early...
                {
                    if (source is not null) 
                        source.PlayOneShot(clipToPlay);
                    else
                        Debug.unityLogger.LogWarning(_TAG, "No AudioSource affected to effects sounds found.");
                }
            }

            /// <summary>
            /// This method allows to exit game wherever it's called.
            /// </summary>
            public static void QuitApplication() => Application.Quit();

            /// <summary>
            /// This method allows to increase or decrease an integer input by the given value.
            /// </summary>
            /// <param name="input">The input to modify the text.</param>
            /// <param name="increase">Boolean, true to increase, false either.</param>
            /// <param name="value">Integer, the value to add or to substract.</param>
            public static void ModifyInput(in TMP_InputField input, in bool increase, in int value)
            {
                if (input.contentType is TMP_InputField.ContentType.IntegerNumber)
                {
                    string currentVal = (input.text is "" or "-") ? "0" : input.text;

                    input.text = 
                        increase ? (int.Parse(currentVal) + value).ToString()
                        : (int.Parse(currentVal) - value).ToString();
                }

                else Debug.unityLogger.Log(_TAG, "Input's content is not of type integer number, so it cannot be increased or decreased.");
            }

            /// <summary>
            /// This method allows to get from a given string first word of its sentence.
            /// </summary>
            /// <param name="sentence">String from where to extract the first word.</param>
            /// <returns>Given sentence if there is only one word, either the first word of given sentence.</returns>
            public static string GetFirstWord(in string sentence) 
            {
                if (sentence.LastIndexOf(" ") is not -1)
                    return sentence.Substring(0, sentence.LastIndexOf(" ")); 
                else
                    return sentence;
            }

            /// <summary>
            /// This method is useful to update settings when cameras are switching.
            /// As there is two SettingsPanel in the scene (one for game's view, one for menu's view), it's required
            /// that changes in one are persistents for the other.
            /// </summary>
            public static void UpdateSettings()
            {
                if (CamerasManager.Current.CurrentRecordingCamera is Cameras.MenuCamera)
                {
                    Menu.SettingsPanel.Current.ConfigureVolumeSliders();
                }
                else 
                {
                    Game.SettingsPanel.Current.ConfigureVolumeSliders();
                }
            }

            /// <summary>
            /// This method allows to update text displayed at the top of game's view to notify user of how many cubes he has in his game.
            /// </summary>
            public static void UpdateNumberOfCubes()
            {
                GameObject.Find("Game Info").GetComponent<TMP_Text>().text = 
                    "Number of cubes in the structure : " + PlayerPrefs.GetInt(PlayerPrefsTags.NumberOfCubes);
            }

            /// <summary>
            /// This method allows to reset some keys saved in PlayerPrefs related to previous games's data.
            /// </summary>
            public static void ResetGameData()
            {
                PlayerPrefs.SetInt(PlayerPrefsTags.NumberOfCubes, default);
                PlayerPrefs.SetString(PlayerPrefsTags.ListOfFaces, default);
            }

            /// <summary>
            /// This function allows to lock or unlock cursor depending on its current state.
            /// </summary>
            public static void ChangeCursorState()
            {
                Cursor.lockState = Cursor.lockState is CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
            }
        }
    }
}