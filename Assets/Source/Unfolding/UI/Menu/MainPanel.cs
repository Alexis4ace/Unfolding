using UnityEngine;
using UnityEngine.UI;
using Unfolding.Utils;

namespace Unfolding.UI
{
    namespace Menu
    {
        /// <summary>
        /// The script attached to Main Panel inside Unfolding Game's home scene. <br/>
        /// Main panel contains itself two smaller panels : Game Options and Other Options.
        /// </summary>
        public class MainPanel : MonoBehaviour, ITaggable
        {
            // CLASS ATTRIBUTES
            public string TAG => "Menu - Main Panel";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            public static MainPanel Current { get; private set; }

            // SCENE'S OBJECTS
            private Button _newGameButton;
            private Button _loadGameButton;
            private Button _settingsButton;
            private Button _creditsButton;
            private Button _exitButton;

            // OTHERS ATTRIBUTES
            [Header("Audio settings")]
            [Space(5)]

            /// <summary>
            /// Sound played whenever one game options's button is pressed.
            /// </summary>
            [Tooltip("Sound played whenever one game options's button is pressed.")] public AudioClip ButtonsSound = null;

            /// <summary>
            /// Awake function, called first in script life's cycle. <br/>
            /// We use it here to initialize the only instance of this script (Singleton pattern). 
            /// </summary>
            private void Awake() => MainPanel.Current = this;

            /// <summary>
            /// Start method, called before first frame. <br/>
            /// It's mainly use here for initialization of panel's components.
            /// </summary>
            private void Start()
            {
                // Resets PlayerPrefs data
                Utils.Helper.ResetGameData();

                // Initializing components
                _newGameButton = this.GetComponentsInChildren<Button>()[0];
                _loadGameButton = this.GetComponentsInChildren<Button>()[1];
                _settingsButton = this.GetComponentsInChildren<Button>()[2];
                _creditsButton = this.GetComponentsInChildren<Button>()[3];
                _exitButton = this.GetComponentsInChildren<Button>()[4];

                // Adding sound to each button in the main panel (the one that this script is attached at)
                foreach (Button button in this.GetComponentsInChildren<Button>())
                    button.onClick.AddListener(delegate { Utils.Helper.PlayEffectSound(ButtonsSound); });

                // Binding new game button to open new game panel
                _newGameButton.onClick.AddListener(() =>
                {
                    MainPanel.Current.SetInteractibility(false);
                    NewGamePanel.Current.OpenPanel();
                });

                // Binding load game button to open load game panel
                _loadGameButton.onClick.AddListener(() =>
                {
                    MainPanel.Current.SetInteractibility(false);
                    LoadGamePanel.Current.OpenPanel();
                });

                // Binding settings button to open settings panel
                _settingsButton.onClick.AddListener(() =>
                {
                    MainPanel.Current.SetInteractibility(false); // Deactivate main panel's buttons before opening settings panel
                    SettingsPanel.Current.OpenPanel();
                });

                // Binding credits button to open credits panel
                _creditsButton.onClick.AddListener(() =>
                {
                    MainPanel.Current.SetInteractibility(false);
                    CreditsPanel.Current.OpenPanel();
                });

                // Binding exit button to open exit panel 
                _exitButton.onClick.AddListener(() =>
                {
                    MainPanel.Current.SetInteractibility(false);
                    ExitPanel.Current.OpenPanel();
                });
            }

            /// <summary>
            /// This function allows to set interactable attributes of each button in main panel to newState param (boolean).
            /// </summary>
            /// <param name="newState">Boolean, true to set all buttons interactable, false either.</param>
            public void SetInteractibility(in bool newState)
            {
                foreach (Button button in this.GetComponentsInChildren<Button>())
                    button.interactable = newState;
            }
        }
    }
}