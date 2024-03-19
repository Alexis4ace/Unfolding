using UnityEngine;
using UnityEngine.UI;
using Unfolding.Utils;

namespace Unfolding.UI
{
    namespace Menu
    {
        /// <summary>
        /// This class should be inherited by all panels in Home Scene of Unfolding Game, <br/>
        /// except the main panel.
        /// </summary>
        public abstract class BasePanel : MonoBehaviour, ITaggable
        {
            /* NOTE: each panel inheriting from this class have a back button and an audio clip to play whenever one of its button is pressed.*/
            // CLASS'S ATTRIBUTES
            public abstract string TAG { get; }

            // OTHERS ATTRIBUTES
            [Header("Soundable settings")]
            [Space(5)]

            [Tooltip("Sound played whenever one panel's button is pressed.")]
            /// <summary>
            /// Sound played whenever one panel's button is pressed.
            /// </summary>
            [SerializeField] protected AudioClip Sound;

            /// <summary>
            /// Start method, called before first frame.<br/>
            /// It's mainly used here for initialization of back button object, which each game object with a script inheriting of this class should have.
            /// </summary>
            protected virtual void Start()
            {
                // A simple log message to check if scripts which are inheriting this class have effectively called Start method from this class
                Debug.unityLogger.Log(TAG, "Children panel " + this.name + " has called start !");

                // Adding sound to each button in the panel
                foreach (Button button in this.GetComponentsInChildren<Button>())
                    button.onClick.AddListener(delegate { Utils.Helper.PlayEffectSound(Sound); });

                if (this.name.CompareTo("Load Game Panel") == 0)
                {
                    //list of buttons in load panel : [LOADFROM   LOAD  DELETE  BACK]
                    Button[] list_button = this.transform.Find("Buttons").GetComponent<GridLayoutGroup>().GetComponentsInChildren<Button>();

                     //button delete
                     list_button[3].onClick.AddListener(() =>
                     {
                         this.ClosePanel();
                         MainPanel.Current.SetInteractibility(true);
                     });
                }
                else
                {
                    this.transform.Find("Back Button").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        this.ClosePanel();
                        MainPanel.Current.SetInteractibility(true);
                    });
                }

                // Binding back button to close panel (and open main one)

                this.ClosePanel(); // Panel is not visible by default
            }

            /// <summary>
            /// This method allows a panel to activate itself (showing up in the scene).
            /// </summary>
            public virtual void OpenPanel() => this.gameObject.SetActive(true);

            /// <summary>
            /// This method allows a panel to deactivate itself (hiding in the scene).
            /// </summary>
            public virtual void ClosePanel() => this.gameObject.SetActive(false);
        }
    }
}