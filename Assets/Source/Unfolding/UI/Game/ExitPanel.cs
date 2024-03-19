using UnityEngine;
using UnityEngine.UI;
using Unfolding.Utils;
using Unfolding.Game.Gameplay;
using Unfolding.UI.Menu;
using UnityEngine.SceneManagement;

namespace Unfolding.UI
{
    namespace Game
    {
        /// <summary>
        /// This class manages exit panel from game UI view.
        /// </summary>
        public class ExitPanel : MonoBehaviour, ITaggable
        {
            // CLASS'S ATTRIBUTES
            public string TAG => "Game - Exit Panel";
            
            /// <summary>
            /// Start method, called before first frame.
            /// </summary>
            private void Start()
            {
                // Getting back confirmation and cancel button from exit panel
                Button validateExit = this.GetComponentsInChildren<Button>()[0];
                Button cancelExit = this.GetComponentsInChildren<Button>()[1];

                // Binding confirmation button with action of switching displays and unselect exit button from radio group
                validateExit.onClick.AddListener(() =>
                {
                    /* GameOptionsManager.Current.UpdateRadioGroup("Exit");
                    CamerasManager.Current.SwitchCameras(); */
                    Utils.Helper.UpdateSettings();
                    
                    /* PolyCubeManager.Current.DestroyGame();
                    LoadGamePanel.Current.LoadGameFiles(); */

                    Debug.Log("Quitting game...");
                    SceneManager.LoadScene("MenuScene");
                });

                // Binding cancel button with action of hide exit panel and unselect exit button from radio group (confirmation button do it as well)
                cancelExit.onClick.AddListener(() =>
                {
                    GameOptionsManager.Current.UpdateRadioGroup("Exit");
                });

                /* NOTE: there is two ways to cancel exit action ; clicking on "no" button inside exit panel, 
                or clicking again on exit button in game options on top-right of the screen. */
            }
        }
    }
}