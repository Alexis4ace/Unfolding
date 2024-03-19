using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unfolding.Game.Gameplay;
using UnityEditor;
using Unfolding.Utils;
using UnityEngine.SceneManagement;

using SFB;

namespace Unfolding.UI
{
    namespace Menu
    {
        /// <summary>
        /// This class is attached to load game panel inside Unfolding game's menu and allows this panel to work correctly.
        /// </summary>
        public sealed class LoadGamePanel : BasePanel
        {
            // CLASS'S ATTRIBUTES
            public override string TAG => "Menu - Load Game Panel";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            public static LoadGamePanel Current { get; private set; }

            private static uint s_numberOfSaves; // Thsi variable is useful for naming buttons inside list of saves (scrollview)

            // SCENE'S OBJECTS
            [Header("Loading Manager")]
            [Space(5.0f)]
            [Tooltip("Should be the parent element of buttons which represent saves.")]
            [SerializeField]
            private Transform _scrollViewContent;

            [Tooltip(
                "A button Prefab that will be display in the scrollViewContent. Be careful with the size of the button !"
            )]
            [SerializeField]
            private GameObject _buttonPrefab;

            private Button _loadButton;
            private Button _deleteButton;
            private Button _loadFromButton;


            // OTHER ATTRIBUTES
            /// <summary>
            /// String to store filename of selected save.
            /// </summary>
            [HideInInspector]
            public string SelectedSaveFile;

            private GameObject _polycube;

            /// <summary>
            /// Awake function, called first in script life's cycle. <br/>
            /// We use it here to initialize the only instance of this script (Singleton pattern).
            /// </summary>
            private void Awake() => LoadGamePanel.Current = this;

            /// <summary>
            /// Start method, called before first frame.<br/>
            /// It's mainly used here for initialization of panel's components.
            /// </summary>
            protected override void Start()
            {
                base.Start(); // Calling parent method to initialize back button

                _polycube = GameObject.Find("PolyCube");
                // LOADFROM   LOAD  DELETE  BACK 
                Button[] list_button = this.transform.Find("Buttons").GetComponent<GridLayoutGroup>().GetComponentsInChildren<Button>();
                
                _loadFromButton = list_button[0];
                 _loadFromButton.interactable = true;

                _loadFromButton.onClick.AddListener(() =>
                 {
                     StandaloneFileBrowser.OpenFolderPanelAsync("", "", false, (paths) =>
                     {
                         // Cancel
                         if (paths.Length == 0)
                         {
                             return;
                         }

                         string path = paths[0] + "/"; // TODO: OS dependant path separator


                         if (path.Length != 0)
                         {
                             Unfolding.Game.IO.IOFile.ABSOLUTPATH = path;
                             LoadGameFiles();
                             _loadButton.interactable = false;
                             _deleteButton.interactable = false;
                         }
                     });
                 });

                // // Binding dlete button with action of confirm user's save selection
                _deleteButton = list_button[2];
                _deleteButton.interactable = false;

                _deleteButton.onClick.AddListener(() => // delete button delete a save file on click
                {

                     //delete the save files and load the saves menu

                     Unfolding.Game.IO.IOFile.DeleteFile(SelectedSaveFile);
                    LoadGameFiles();

                     // make the load and delete button unclickable unless you select a new save
                     _loadButton.interactable = false;
                    _deleteButton.interactable = false;


                });

                // Binding load button with action of confirm user's save selection
                _loadButton = list_button[1];
                _loadButton.interactable = false; // Load button is disabled by default

                _loadButton.onClick.AddListener(() => // Load button switch cameras on click when a selection is confirmed (and then reset LoadGamePanel)
                {
                    Unfolding.Game.IO.IOPlayerPrefs.WritePlayerPrefs(
                        Unfolding.Game.IO.IOFile.ReadFile(SelectedSaveFile)
                    );

                    SceneManager.LoadScene("GameScene");

                    list_button[3].onClick.Invoke(); /* Invoke all listeners linked to onClick
                                                                                                 of back button to hide LoadPanel */
                });

                /*
                    But we also need to add a second listener to back button !
                    Because go back to main menu should reset selected save's filename, buttons's state and make load button uninteractable.
                */
                list_button[3].onClick.AddListener(() =>
                 {
                        _loadButton.interactable = false;
                        _deleteButton.interactable = false;
                        SelectedSaveFile = "";

                        // Unhighlight all buttons
                        foreach (
                            Utils.HighlightingButton highlightingButton in this.GetComponentsInChildren<Utils.HighlightingButton>()
                        )
                        {
                            highlightingButton.ResetButton();
                        }
                 });

                LoadGameFiles();
            }

            /// <summary>
            /// A method that searchs in the correct folder the available game file and list them in load game panel
            /// </summary>
            public void LoadGameFiles()
            {
                // First of all, we need to clear the previous list of load button to make sure it is entirely empty
                foreach (
                    Transform transform in _scrollViewContent.GetComponentInChildren<Transform>()
                )
                {
                    Destroy(transform.gameObject);
                }

                // Find existing saving files in the correct folder and instanciate them used methods below
                string[] savesFolder = Directory.GetFiles(
                    Unfolding.Game.IO.IOFile.ABSOLUTPATH,
                    "*.cub"
                );

                // Initializing number of saves variable
                s_numberOfSaves = 0;

                // Getting back saves names for each file found in saves folder
                foreach (string filepath in savesFolder)
                {
                    // We first need to isolate save's name from rest of the filepath
                    string saveName = filepath.Replace(Unfolding.Game.IO.IOFile.ABSOLUTPATH, "");
                    saveName = saveName.Replace(".cub", "");

                    // Then we can increment number of saves and launch creation of the button for current save
                    ++s_numberOfSaves;
                    this.CreateButton(saveName);
                }

                // Initializing selected save's path
                SelectedSaveFile = "";
            }

            /// <summary>
            /// A method that instantiates a button using the Prefabs and set an onClick listener on it.
            /// </summary>
            /// <param name="filename">The name of the file, which will be the text on the button.</param>
            private void CreateButton(string filename)
            {
                // Instantiate a new button with given prefab template
                GameObject saveButton = Object.Instantiate(_buttonPrefab);

                // Adding parent inside scroll view of LoadGamePanel which represents list of saves files
                saveButton.transform.SetParent(_scrollViewContent);
                saveButton.name = "Save " + s_numberOfSaves;

                /*
                NOTE:
                    saveButton.transform.localPosition is  is really important to be able to fix the position of the button in the scrollviewcontent,
                    especially the z coordinate, adapt the x coordonate with the interface
                */
                saveButton.transform.localPosition = new Vector3(-158, 0, 0);

                // Now we should get back text component from freshly created button to configure it
                saveButton.GetComponentInChildren<TMP_Text>().text = filename;

                // And we should pass filename to button's listener, to handle selection and deselection
                saveButton
                    .GetComponent<Button>()
                    .onClick.AddListener(
                        delegate
                        {
                            this.SelectButton(filename);
                        }
                    );
            }

            /// <summary>
            /// A private method that defines the behaviour of a button as it is clicked.
            /// </summary>
            /// <param name="selectedSave">The name of the file that will be save as selected save's file.</param>
            private void SelectButton(in string selectedSave)
            {
                if (SelectedSaveFile.Equals(selectedSave)) // User has clicked again on same save
                {
                    // Then we unselect selected save file
                    SelectedSaveFile = "";

                    // And we reset load button
                    _loadButton.interactable = false;
                    _deleteButton.interactable = false;
                }
                else // User has clicked on another save than the one which is selected
                {
                    // Then we simply update selected save's file
                    SelectedSaveFile = selectedSave;

                    // We also need to reset current highlighted button
                    foreach (
                        Utils.HighlightingButton highlightingButton in this.GetComponentsInChildren<Utils.HighlightingButton>()
                    )
                    {
                        if (highlightingButton.IsHighlighted)
                        {
                            highlightingButton.ResetButton();
                            break; // Once we have reset previous highlighted button, we can break out of foreach loop
                        }
                    }

                    // And we make load button interractable (if it's not already the case)
                    if (!_loadButton.interactable)
                    {
                        _loadButton.interactable = true;
                        _deleteButton.interactable = true;
                    }
                }
            }
        }
    }
}
