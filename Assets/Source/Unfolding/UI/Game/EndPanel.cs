using UnityEngine;
using UnityEngine.UI;
using Unfolding.Utils;
using Unfolding.Game.Gameplay;
using UnityEngine.SceneManagement;

namespace Unfolding.UI
{
    namespace Game
    {
        /// <summary>
        /// This class manages end panel from game UI view.
        /// </summary>
        public class EndPanel : MonoBehaviour, ITaggable
        {
            // CLASS'S ATTRIBUTES
            public string TAG => "Game - End Panel";

            /// <summary>
            /// Start method, called before first frame.
            /// </summary>
            private void Start()
            {
                // Main menu and replay buttons from end panel
                Button screenshot = this.GetComponentsInChildren<Button>()[0];
                Button saveToLeaderboard = this.GetComponentsInChildren<Button>()[1];
                Button replay = this.GetComponentsInChildren<Button>()[2];
                Button mainMenu = this.GetComponentsInChildren<Button>()[3];

                // Binding screenshot button
                screenshot.onClick.AddListener(() =>
                {
                    GameObject.Find("PolyCube").GetComponent<PolyCubeManager>().TakeScreenShot();
                });

                // Binding leaderboard button
                saveToLeaderboard.onClick.AddListener(() =>
                {
                    Debug.Log("Saving score to the leaderboard...");
                    /*TODO*/
                });

                // Binding replay button
                replay.onClick.AddListener(() =>
                {
                    Debug.Log("Replaying game...");
                    SceneManager.LoadScene("GameScene");
                });

                // Binding main menu button
                mainMenu.onClick.AddListener(() =>
                {
                    Utils.Helper.UpdateSettings();

                    Debug.Log("Quitting game...");
                    SceneManager.LoadScene("MenuScene");
                });
            }
        }
    }
}