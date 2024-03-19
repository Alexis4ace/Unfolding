using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unfolding.Utils;
using Unfolding.Game.Gameplay;

namespace Unfolding.UI 
{
    namespace Game
    {
        /// <summary>
        /// Script attached to Settings Panel inside Game's view of Unfolding game.
        /// </summary>
        /// <remarks>
        /// Settings panel provide an interface for a tabbed panel, and so this script implements base functionnalities of it.
        /// </remarks>
        public sealed class SettingsPanel : UI.Utils.AnimatedRadioGroup
        {
            // CLASS'S ATTRIBUTES
            public override string TAG { get => "Game - Settings Panel"; }

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            public static SettingsPanel Current { get; private set; }

            // SCENE'S OBJECTS
            /// <summary>
            /// List of objects to store pages of tabbed panel
            /// </summary>
            private List<GameObject> _pages = new List<GameObject>(); 

            [Header("Audio page")]
            [Space(5.0f)]

            [Tooltip("Should be the slider linked to music volume.")]
            [SerializeField] private Slider _musicVolume;

            [Tooltip("Should be the slider linked to effects volume.")]
            [SerializeField] private Slider _effectsVolume;

            /// <summary>
            /// Audio source for music.
            /// </summary>
            [Tooltip("Audio source for music.")] public AudioSource MusicSource;

            /// <summary>
            /// Audio source for effects.
            /// </summary>
            [Tooltip("Audio source for effects.")] public AudioSource EffectsSource;

            [Header("Camera page")]
            [Space(5.0f)]

            [Tooltip("Should be the slider linked to camera's sensitivity.")]
            [SerializeField] private Slider _cameraSensitivity;

            [Tooltip("Should be the slider linked to camera's zoom sensitivity.")]
            [SerializeField] private Slider _zoomSensitivity;

            [Tooltip("Should be the slider linked to camera's field of view.")]
            [SerializeField] private Slider _fov;

            /// <summary>
            /// Awake method, first method called in script life's cycle.
            /// We do not call parent method here because radio buttons here are not directly in this's childrens.
            /// We use this method also for initialize only instance of this script (Singleton pattern).
            /// </summary>
            protected override void Awake() 
            {
                SettingsPanel.Current = this;
                this.transform.Find("Tabs").GetComponentsInChildren<Button>(RadioButtons);
            }

            /// <summary>
            /// Start method, called before first frame.
            /// </summary>
            protected override void Start()
            {
                /* 
                    First, we need to store each pages in our _pages list.
                    So, how are we going to do it programatically ?
                    We can find pages objects with their name, as they all end with keyword "params".
                */

                // We use Skip method to not keeping first element that GetComponentsInChildren returns, as this one is the parent's one
                List<Transform> pagesTransform = new List<Transform>(this.transform.Find("Pages").GetComponentsInChildren<Transform>().Skip(1));

                foreach (Transform pageTransform in pagesTransform.Where( transform => transform.name.EndsWith(" Params") ))
                {
                    _pages.Add(pageTransform.gameObject);
                } 

                /*
                    NOTE: we must research pages's objects by seeking for their transform component, because
                    searching for game objects themselves throws an error. 
                */
                
                _pages.ForEach( page => page.SetActive(false) ); // All pages are disabled by default 

                base.Start(); // Calling parent method, but AFTER retrieving _pages

                /*
                    Now that listeners on radio buttons had been setted, we can call listener for first button to activate first page by default.
                    Why are we doing this ? Because it may be note so clear for user how to open a page inside SettingsPanel.
                    Setting a page active by default may help user to understand how it works.
                */
                RadioButtons[0].onClick.Invoke();

                // Calling method that will configure sliders from audio's page
                this.ConfigureVolumeSliders();

                // Calling method that will configure sliders from camera's page
                this.ConfigureCameraSliders();
            }

            /// <summary>
            /// We have overrided this method here to add some features and link radio buttons to pages,
            /// in order to create the functionnal core of a tabbed panel.
            /// </summary>
            /// <param name="clickedButtonName"></param>
            public override void UpdateRadioGroup(in string clickedButtonName)
            {
                // Before calling parent's method we need to disable current displayed page (if there is one)
                if (CurrentSelected is not null) _pages.Find( page => UI.Utils.Helper.GetFirstWord(page.name).Equals(CurrentSelected) ).SetActive(false);

                base.UpdateRadioGroup(clickedButtonName); // Calling parent method to initialise radio buttons functionnalities

                // Now that _current attribute have been setted, we can display new selected page (if there is one)
                if (CurrentSelected is not null) _pages.Find( page => UI.Utils.Helper.GetFirstWord(page.name).Equals(CurrentSelected) ).SetActive(true);
            }

            /// <summary>
            /// This function allows to configure sliders that manages music volume and effects volume.
            /// This method is public for the same reasons than its homonym in Menu.SettingsPanel script.
            /// </summary>
            public void ConfigureVolumeSliders()
            {
                // First, we need to check if both sliders have been assigned in the inspector
                if (_musicVolume is not null && _effectsVolume is not null)
                {
                    // Before adding listeners to our sliders, we should set their initial value
                    MusicSource.volume = 
                        PlayerPrefs.HasKey(PlayerPrefsTags.MusicVolume) ? PlayerPrefs.GetFloat(PlayerPrefsTags.MusicVolume) 
                        : UI.Utils.Helper.DefaultVolume;
                    _musicVolume.value = MusicSource.volume;

                    EffectsSource.volume = 
                        PlayerPrefs.HasKey(PlayerPrefsTags.EffectsVolume) ? PlayerPrefs.GetFloat(PlayerPrefsTags.EffectsVolume) 
                        : UI.Utils.Helper.DefaultVolume;
                    _effectsVolume.value = EffectsSource.volume;

                    // Now we can add some listeners to our sliders
                    _musicVolume.onValueChanged.AddListener( newValue => 
                    {
                        MusicSource.volume = newValue;
                        _musicVolume.value = newValue;
                        PlayerPrefs.SetFloat(PlayerPrefsTags.MusicVolume, newValue); 
                    });

                    _effectsVolume.onValueChanged.AddListener( newValue => 
                    {
                        EffectsSource.volume = newValue;
                        _effectsVolume.value = newValue;
                        PlayerPrefs.SetFloat(PlayerPrefsTags.EffectsVolume, newValue); 
                    });
                }
                else
                {
                    Debug.unityLogger.LogWarning(TAG, "One or several volume sliders are missing. Check this out in Unity's inspector.");
                }
            }

            /// <summary>
            /// This method allows to configure sliders linked to camera's settings.
            /// </summary>
            private void ConfigureCameraSliders()
            {
                // First, we need to check if sliders had been setted in the inspector
                if (_cameraSensitivity is not null && _zoomSensitivity is not null && _fov is not null)
                {
                    // We are going to modify camera's movements and zoom speed with singleton instance of class MovingCamera

                    /*
                        First, we should initialize lower and upper bounds of our sliders, and add listeners to them.
                        All listeners added to the sliders will update the value that their are linked to, 
                        and then set the modified value in player prefs (to keep same settings between games). 
                    */

                    _cameraSensitivity.minValue = 0.5f;
                    _cameraSensitivity.maxValue = 10f;

                    _cameraSensitivity.onValueChanged.AddListener(newValue => 
                    {
                        MovingCamera.Current.CameraMovingSpeed = newValue;
                        PlayerPrefs.SetFloat(PlayerPrefsTags.CameraSensitivity, newValue);
                    });

                    _zoomSensitivity.minValue = 5f;
                    _zoomSensitivity.maxValue = 20f;

                    _zoomSensitivity.onValueChanged.AddListener(newValue =>
                    {
                        MovingCamera.Current.ZoomSensitivity = newValue;
                        PlayerPrefs.SetFloat(PlayerPrefsTags.ZoomSensitivity, newValue);
                    });

                    _fov.minValue = 80f;
                    _fov.maxValue = 140f;

                    _fov.onValueChanged.AddListener(newValue => 
                    {
                        CamerasManager.Current.GetGameCamera().GetComponent<Camera>().fieldOfView = newValue;
                        PlayerPrefs.SetFloat(PlayerPrefsTags.FieldOfView, newValue);
                    });

                    // And then, we can initialize sliders's values
                    _cameraSensitivity.value = 
                        PlayerPrefs.HasKey(PlayerPrefsTags.CameraSensitivity) ? PlayerPrefs.GetFloat(PlayerPrefsTags.CameraSensitivity) 
                                                                              : MovingCamera.Current.CameraMovingSpeed;

                    _zoomSensitivity.value = 
                        PlayerPrefs.HasKey(PlayerPrefsTags.ZoomSensitivity) ? PlayerPrefs.GetFloat(PlayerPrefsTags.ZoomSensitivity) 
                                                                              : MovingCamera.Current.ZoomSensitivity;
                    
                    _fov.value = 
                        PlayerPrefs.HasKey(PlayerPrefsTags.FieldOfView) ? PlayerPrefs.GetFloat(PlayerPrefsTags.FieldOfView) 
                                                                              : CamerasManager.Current.GetGameCamera().GetComponent<Camera>().fieldOfView;
                }
                else
                {
                    Debug.unityLogger.LogWarning(TAG, "One or several camera sliders are missing. Check this out in Unity's inspector.");
                }
            }
        }
    }
}