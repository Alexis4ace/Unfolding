using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unfolding.Utils
{
    /// <summary>
    /// Enum type for ordering cameras in scene.
    /// </summary>
    public enum Cameras : int
    {
        GameCamera = 0,
        MenuCamera = 1
    }

    /// <summary>
    /// This class allows to manage all cameras of the scene with a bunch of utilitary methods.
    /// </summary>
    public sealed class CamerasManager : MonoBehaviour, ITaggable
    {
        public string TAG => "CamerasManager";

        // CLASS'S ATTRIBUTES

        /// <summary>
        /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
        /// </summary>
        public static CamerasManager Current { get; private set; }

        /// <summary>
        /// This attribut stores the current active cameras
        /// </summary>
        public Cameras CurrentRecordingCamera;

        // SCENE'S OBJECTS
        private Canvas _sceneCanvas;

        /// <summary>
        /// Awake method, called first in script life's cycle.
        /// </summary>
        private void Awake()
        {
            Current = this; // Setting the only instance of this class (Singleton pattern)
            _sceneCanvas = FindObjectsOfType<Canvas>()[0]; // Getting back all scene's canvas
        }

        /// <summary>
        /// Start method, called before first frame.
        /// </summary>
        private void Start()
        {
            Current = this;

            // By default, game display and screenshot camera are inactives and only menu display is enabled
            this.transform.Find("ScreenShot Camera").gameObject.SetActive(false);

            // NEW: Make sure that everything linked to the game is displayed
            this.transform.Find("Menu Camera").gameObject.SetActive(false);
            this.transform.Find("Game Camera").gameObject.SetActive(true);
            _sceneCanvas.worldCamera.gameObject.SetActive(true);
            _sceneCanvas.enabled = true;

            // Setting plane distance to very low value to prevent UI to be sliced by polycube object
            _sceneCanvas.planeDistance = 1f;

            /*
                Why not setting plane distance statically in the inspector ?
                Because it would make canvas really difficult to edit outside of Runtime. That's why we set it at game's launch.
            */

            // CurrentRecordingCamera is then the game's one
            CurrentRecordingCamera = Cameras.GameCamera;
        }

        /// <summary>
        /// This function allows another script/class to access Game Camera's object.
        /// </summary>
        /// <returns>GameObject, representing Game Camera (centered on Game's UI).</returns>
        public GameObject GetGameCamera()
        {
            return _sceneCanvas.worldCamera.gameObject;
        }

        /// <summary>
        /// This function allows another script/class to access Menu Camera's object.
        /// </summary>
        /// <returns>GameObject, representing Menu Camera (centered on Menu's UI).</returns>
        public GameObject GetMenuCamera()
        {
            return _sceneCanvas.worldCamera.gameObject;
        }

        /// <summary>
        /// This function allows another script/class to access ScreenShot Camera's object.
        /// </summary>
        /// <returns>GameObject, representing ScreenShot Camera (which is a top view of the pattern)</returns>
        public GameObject GetScreenShotCamera()
        {
            return this.transform.Find("ScreenShot Camera").gameObject;
        }

        /// <summary>
        /// This method allows another script to switch cameras at runtime (between menu camera and game camera).
        /// </summary>
        public void SwitchCameras()
        {
            // Switching state of game display
            bool currentGameState = _sceneCanvas
                .worldCamera
                .gameObject
                .activeSelf;

            _sceneCanvas.worldCamera.gameObject.SetActive(
                !currentGameState
            );

            _sceneCanvas.enabled = !currentGameState;

            // Switching state of menu display
            bool currentMenuState = _sceneCanvas
                .worldCamera
                .gameObject
                .activeSelf;

            _sceneCanvas.worldCamera.gameObject.SetActive(
                !currentMenuState
            );

            _sceneCanvas.enabled = !currentMenuState;

            // Switching state of CurrentRecordingCamera
            CurrentRecordingCamera = CurrentRecordingCamera.Equals(Cameras.MenuCamera)
                ? Cameras.GameCamera
                : Cameras.MenuCamera;
        }

        /// <summary>
        ///	This function allows to take a screenshot from GameCamera when needed.
        /// In our case we use it at the end of the game, when player has succeed.
        /// </summary>
        /// <param name="picturePath">The path where the png will be saved.</param>
        /// <returns>N/D</returns>
        public IEnumerator TakeScreenShot(string picturePath)
        {
            // This function should be called only from game's view, so we just have to disable game's camera first
            this.GetGameCamera().SetActive(false);

            // And then we can activate ScreenShotCamera to take a screenshot of the board
            this.GetScreenShotCamera().SetActive(true);

            // It is a good practice before taking a screenshot to wait until end of frame 
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(picturePath, 2);
            yield return new WaitUntil(() => System.IO.File.Exists(picturePath) is true); // Delaying until screenshot have been saved

            // And now, we juste have to revert cameras arrangment
            this.GetScreenShotCamera().SetActive(false);
            this.GetGameCamera().SetActive(true);
        }
    }
}
