using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unfolding.Game.Gameplay;
using Unfolding.Utils;
using UnityEngine.SceneManagement;

namespace Unfolding.UI
{
    namespace Menu 
    {
        /// <summary>
        /// This class is attached to new game panel inside Unfolding game's menu and allows this panel to work correctly.
        /// </summary>
        /// <remarks>
        /// In this script, instead of searching panel's components inside the code, we chose to directly
        /// link objects inside the inspector. This choice is purely arbitrary, we made it only to have both mechanisms exposed.
        /// </remarks>
        public sealed class NewGamePanel : BasePanel
        {
            // CLASS'S ATTRIBUTES
            public override string TAG => "Menu - New Game Panel";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            public static NewGamePanel Current { get; private set; }

            // SCENE'S OBJECTS
            [Header("Panel Objects")]
            [Space(5)]

            [Tooltip("Should be the input which is containing number of cubes wanted by user.")]
            [SerializeField] private TMP_InputField _userChoice;

            [Tooltip("Should be the increase button for user choice's input.")]
            [SerializeField] private Button _increaseButton;

            [Tooltip("Should be the decrease button for user choice's input.")]
            [SerializeField] private Button _decreaseButton;
            
            [Tooltip("Should be the button linked to start game action.")] 
            [SerializeField] private Button _startButton;

            // OTHER ATTRIBUTES
            private const int _DEFAULTNUMBEROFCUBES = 10;

            private const int _MAXNUMBEROFCUBES = 50;

            private GameObject _polycube; // Access to polycube

            /// <summary>
            /// Awake function, called first in script life's cycle. <br/>
            /// We use it here to initialize the only instance of this script (Singleton pattern). 
            /// </summary>
            private void Awake() => NewGamePanel.Current = this;

            /// <summary>
            /// Start method, called before first frame.<br/>
            /// It's mainly used here for initialization of panel's components.
            /// </summary>
            protected override void Start()
            {
                base.Start(); // Calling parent method to initialize back button

                // Getting the polycube manager
                _polycube = GameObject.Find("PolyCube");

                // Before configuring components of this panel, we must check if one more are missing
                if (this.CheckForMissingPanelObject() is not true)
                {
                    // And if not, then we can do our configurations

                    // Configuring user choice input
                    _userChoice.contentType = TMP_InputField.ContentType.IntegerNumber;
                    _userChoice.text = _DEFAULTNUMBEROFCUBES.ToString();

                    // Adding actions on increase and decrease buttons
                    _increaseButton.onClick.AddListener( delegate {Utils.Helper.ModifyInput(_userChoice, true, 0b1);} );
                    _decreaseButton.onClick.AddListener( delegate {Utils.Helper.ModifyInput(_userChoice, false, 0b1);} );

                    // Adding action on input (only to activate / deactivate start button)
                    _userChoice.onValueChanged.AddListener((newValue) => 
                    {
                        if (!newValue.Contains("-") && newValue is not "" && (int.Parse(newValue) is > 0 and <= _MAXNUMBEROFCUBES))
                        {
                            _startButton.interactable = true;
                        }
                        else _startButton.interactable = false;
                    });

                    // Adding action on start button to launch a game
                    _startButton.onClick.AddListener(() => 
                    {
                        PlayerPrefs.SetFloat(PlayerPrefsTags.Time, .0f);
                        PlayerPrefs.SetInt(PlayerPrefsTags.NumberOfCubes, int.Parse(_userChoice.text));

                        this.transform.Find("Back Button").GetComponent<Button>().onClick.Invoke(); /* Invoke listeners linked to back button 
                                                                                                            to close panel */

                        SceneManager.LoadScene("GameScene");
                    });
                }
            }

            /// <summary>
            /// This method allows to check if all necessary objects for panel's activity
            /// have been linked inside the inspector.
            /// </summary>
            /// <returns>Boolean, true if one or more variables are missing.</returns>
            private bool CheckForMissingPanelObject()
            {
                return (
                    _userChoice is null ||
                    _increaseButton is null ||
                    _decreaseButton is null ||
                    _startButton is null
                );
            }
        }
    }
}