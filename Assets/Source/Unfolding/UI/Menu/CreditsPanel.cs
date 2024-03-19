using UnityEngine;

namespace Unfolding.UI
{
    namespace Menu
    {
        /// <summary>
        /// This class is attached to credits panel inside Unfolding game's menu and allows this panel to work correctly.
        /// </summary>
        public sealed class CreditsPanel : BasePanel
        {
            // CLASS'S ATTRIBUTES
            public override string TAG => "Menu - Credits Panel";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            [HideInInspector] public static CreditsPanel Current { get; private set; }

            /// <summary>
            /// Awake function, called first in script life's cycle. <br/>
            /// We use it here to initialize the only instance of this script (Singleton pattern). 
            /// </summary>
            private void Awake() => CreditsPanel.Current = this;

            /// <summary>
            /// Start method, called before first frame.<br/>
            /// It's mainly used here for initialization of panel's components.
            /// </summary>
            protected override void Start() => base.Start(); // Calling parent method to initialize back button
        }
    }
}