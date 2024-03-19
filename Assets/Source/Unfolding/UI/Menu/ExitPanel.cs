using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Unfolding.UI
{
    namespace Menu
    {
        /// <summary>
        /// This class manages exit panel from menu UI view.
        /// </summary>
        public sealed class ExitPanel : BasePanel
        {
            // CLASS'S ATTRIBUTES
            public override string TAG => "Menu - Exit Panel";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            public static ExitPanel Current { get; private set; }

            /// <summary>
            /// Awake function, called first in script life's cycle. <br/>
            /// We use it here to initialize the only instance of this script (Singleton pattern). 
            /// </summary>
            private void Awake() => ExitPanel.Current = this;

            /// <summary>
            /// Start method, called before first frame.<br/>
            /// It's mainly used here for initialization of panel's components.
            /// </summary>
            protected override void Start()
            {
                base.Start(); // Calling parent method to initialize back button

                // As back button is already configured in parent's script
                // We just have to bind confirmation button to qui running application
                this.transform.Find("Confirmation Button").GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.unityLogger.Log(TAG, "Quitting application...");
                    Utils.Helper.QuitApplication();
                });
            }
        }
    }
}