using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unfolding.Utils;
using System.Linq;

namespace Unfolding.UI
{
    namespace Menu
    {
        /// <summary>
        /// This class is attached to settings panel inside Unfolding game's menu and allows this panel to work correctly.
        /// </summary>
        public sealed class SettingsPanel : BasePanel
        {
            // CLASS ATTRIBUTES
            public override string TAG => "Menu - Settings Panel";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            public static SettingsPanel Current { get; private set; }

            // SCENE'S OBJECTS
            [Header("Panel objects")]
            [Space(5)]

            [Tooltip("Should be the two sliders affected to music and effects volume.")] 
            [SerializeField] private List<Slider> _volumeSliders = new List<Slider>();

            [Tooltip("Should be the two icons associated to volumes sliders.")] 
            [SerializeField] private List<Image> _slidersIcons = new List<Image>();

            [SerializeField] private TMP_Dropdown _windowOptions;

            [Header("Resolution settings")]
            [Space(5)]

            [Tooltip("Dropdown for selecting screen resolutions.")]
            [SerializeField] private TMP_Dropdown _resolutionDropdown;

            // OTHERS ATTRIBUTES

            [Header("Musics settings")]
            [Space(5)]

            /* NOTE: there is already an header for this section inside parent class. */
            /// <summary>
            /// Audio source for music.
            /// </summary>
            [Tooltip("Audio source for music.")] public AudioSource MusicSource;

            /// <summary>
            /// Audio source for effects.
            /// </summary>
            [Tooltip("Audio source for effects.")] public AudioSource EffectsSource;

            [Header("Image settings")]
            [Space(5)]

            /// <summary>
            /// The sprite associated to minimal value for sound.
            /// </summary>
            [Tooltip("The sprite associated to minimal value for sound.")] public Sprite SpriteSoundMute;

            /// <summary>
            /// The sprite associated with mid values for sound.
            /// </summary>
            [Tooltip("The sprite associated with mid values for sound.")] public Sprite SpriteSoundLow;

            /// <summary>
            /// The sprite associated to maximal value for sound.
            /// </summary>
            [Tooltip("The sprite associated to maximal value for sound.")] public Sprite SpriteSoundHigh;

            /// <summary>
            /// Awake function, called first in script life's cycle. <br/>
            /// We use it here to initialize the only instance of this script (Singleton pattern).
            /// </summary>
            private void Awake() => SettingsPanel.Current = this;

            /// <summary>
            /// Start method, called before first frame.<br/>
            /// It's mainly used here for initialization of panel's components.
            /// </summary>
            protected override void Start()
            {
                base.Start(); // Calling parent method to initialize back button

                /*
                    Calling method that will configure sliders which manages volumes of music and effects.
                    It will configure as well icons associated to volume values.
                */
                this.ConfigureVolumeSliders();

                // Configure the resolution dropdown
                ConfigureResolutionDropdown();

                // Now we can also configure the dropdown that controls window mode
                _windowOptions.ClearOptions(); // Starting by cleaning options
                _windowOptions.options.Add(new TMP_Dropdown.OptionData("Windowed"));
                _windowOptions.options.Add(new TMP_Dropdown.OptionData("FullScreen"));
                _windowOptions.RefreshShownValue(); // Refresh dropdown as we have added some new options

                // Setting default window mode (fullscren) and looking in PlayerPrefs to see if user set a specific value during his last session
                _windowOptions.value = PlayerPrefs.HasKey(PlayerPrefsTags.WindowMode) ? PlayerPrefs.GetInt(PlayerPrefsTags.WindowMode) : 1;

                _windowOptions.onValueChanged.AddListener( (newValue) =>
                {
                    // Adding a listener to check for new value setted
                    Screen.fullScreen = newValue.Equals(0) ? false : true;

                    // Updating the fullscreen value in PlayerPrefs
                    PlayerPrefs.SetInt(PlayerPrefsTags.WindowMode, newValue);
                });
            }

            /// <summary>
            /// This method allows to configure sliders that manages music and effects volumes in the scene.
            /// </summary>
            /// <remarks>
            /// This method should be called in Start method of this script. 
            /// This method is also public because it is needed to be called when scene's cameras are switching.
            /// </remarks>
            public void ConfigureVolumeSliders()
            {
                /*
                    NOTE: as this method can be called in another place than Start method of this class,
                    we have to add another configuration to all components : we need to reset their listeners before adding new ones.
                */

                // We are going to configure each slider affected to volumes (music & effects)
                foreach (Slider slider in _volumeSliders) 
                {
                    // Resetting slider's listeners
                    slider.onValueChanged.RemoveAllListeners();

                    // For both slider that manages sound volume, min and max values are set to 0 and 1
                    slider.minValue = 0.0f;
                    slider.maxValue = 1.0f;

                    float savedValue; // Variable to store saved value in PlayerPrefs whether its for one volume or the other

                    // Getting back icon associated to current slider
                    string associatedImageName = slider.name.Substring(0, slider.name.IndexOf(" ")) + " Image";
                    Image associatedImage = _slidersIcons.Find(item => item.name.Equals(associatedImageName));

                    // Associated icon have a default sprite
                    associatedImage.sprite = SpriteSoundLow;

                    // Adding a listener to slider's onValueChanged param
                    if (slider.name.Equals("Music Slider"))
                    {
                        slider.onValueChanged.AddListener( newValue => 
                        {
                            // Updating source volume 
                            MusicSource.volume = newValue; 

                            // Updating associated icon (according to new slider's value)
                            associatedImage.sprite = 
                                (newValue > 0.5f) ? SpriteSoundHigh : (newValue > 0.0f && newValue < 0.5f) ? SpriteSoundLow : SpriteSoundMute;

                            // Saving value of volume in player prefs so that user will still have this parameter when he will re-launch the game
                            PlayerPrefs.SetFloat(PlayerPrefsTags.MusicVolume, newValue);
                        });

                        /* 
                            Now we should initialize slider value :
                            for that, we will check if there is some saved value in PlayerPrefs ;
                            and if not, then we set slider value to default value (which is 0.1).
                        */
                        if (PlayerPrefs.HasKey(PlayerPrefsTags.MusicVolume))
                        {
                            // Setting slider value according to the one found in PlayerPrefs
                            savedValue = PlayerPrefs.GetFloat(PlayerPrefsTags.MusicVolume);
                            slider.value = savedValue; 

                            // If value found is equal to 0, then listener is not called (we have to call it ourselves)
                            if (savedValue.Equals(0.0f)) 
                                slider.onValueChanged.Invoke(savedValue);
                        }
                        else
                        {
                            slider.value = UI.Utils.Helper.DefaultVolume;
                        }
                    }
                    else
                    {
                        slider.onValueChanged.AddListener( newValue => 
                        {
                            // Updating source volume 
                            EffectsSource.volume = newValue;

                            // Updating associated icon (according to new slider's value)
                            associatedImage.sprite = 
                                (newValue > 0.5f) ? SpriteSoundHigh : (newValue > 0.0f && newValue < 0.5f) ? SpriteSoundLow : SpriteSoundMute;

                            // Saving value of volume in player prefs so that user will still have this parameter when he will re-launch the game
                            PlayerPrefs.SetFloat(PlayerPrefsTags.EffectsVolume, newValue);
                        });

                        /* 
                            Now we should initialize slider value :
                            for that, we will check if there is some saved value in PlayerPrefs ;
                            and if not, then we set slider value to default value (which is 0.1).
                        */
                        if (PlayerPrefs.HasKey(PlayerPrefsTags.EffectsVolume))
                        {
                            // Setting slider value according to the one found in PlayerPrefs
                            savedValue = PlayerPrefs.GetFloat(PlayerPrefsTags.EffectsVolume);
                            slider.value = savedValue; 

                            // If value found is equal to 0, then listener is not called (we have to call it ourselves)
                            if (savedValue.Equals(0.0f)) 
                                slider.onValueChanged.Invoke(savedValue);
                        }
                        else
                        {
                            slider.value = UI.Utils.Helper.DefaultVolume;
                        }
                    }

                    // Resetting icon's listeners
                    associatedImage.GetComponent<Button>().onClick.RemoveAllListeners();

                    /*
                        As associated icons are interractive too, we should add to their button component correct action.
                        Clicking on button component on image can instantly mute / demute associated volume.
                    */
                    associatedImage.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        slider.value = (associatedImage.sprite == SpriteSoundMute) ? UI.Utils.Helper.DefaultVolume : 0.0f;
                    });
                }

                /* 
                NOTE: testing if PlayerPrefs containing both keys is useful essentially for the first launch,
                    because user will probably edit these volume values during his first session.
                */
            }

            /// <summary>
            /// This method allows configuring the resolutions for the game screen using a dropdown menu.
            /// </summary>
            private void ConfigureResolutionDropdown()
            {
                _resolutionDropdown.ClearOptions(); // Clearing existing options

                // Get available screen resolutions and add them to the dropdown
                Resolution[] resolutions = Screen.resolutions;

                // Filter the possible duplicate from the array
                resolutions = resolutions.Distinct().ToArray();

                List<string> resolutionOptions = new List<string>();

                foreach (Resolution resolution in resolutions)
                {
                    // Create a string representation of the resolution (e.g., "1920x1080")
                    string option = $"{resolution.width}x{resolution.height}";
                    resolutionOptions.Add(option);
                }

                // Add the resolution options to the dropdown
                _resolutionDropdown.AddOptions(resolutionOptions);
                _resolutionDropdown.RefreshShownValue(); // Refresh dropdown as new options have been added

                // Set default resolution and check PlayerPrefs for a saved value
                int savedResolutionIndex = PlayerPrefs.GetInt(PlayerPrefsTags.ScreenResolution, -1);
                _resolutionDropdown.value = (savedResolutionIndex != -1 && savedResolutionIndex < resolutions.Length)
                    ? savedResolutionIndex
                    : resolutions.Length - 1; // Set to the highest resolution by default

                // Add a listener to handle changes in the resolution dropdown
                _resolutionDropdown.onValueChanged.AddListener((newValue) =>
                {
                    // Update the selected resolution index in the ResolutionManager
                    ResolutionManager.Instance.SetSelectedResolutionIndex(newValue);

                    // Save the selected resolution index in PlayerPrefs for future sessions
                    PlayerPrefs.SetInt(PlayerPrefsTags.ScreenResolution, newValue);
                });
            }

        }

    }
}