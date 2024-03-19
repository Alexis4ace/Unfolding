using UnityEngine;
using Unfolding.Utils;
using Unfolding.Game.Utils;
using System.Reflection;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

namespace Unfolding.Game.Gameplay
{
    /// <summary>
    /// This script is attached to the game's camera in Unfolding game.
    /// It allows it to move around game board in all directions.
    /// </summary>
    public class MovingCamera : MonoBehaviour, ITaggable
    {
        // CLASS'S ATTRIBUTES
        public string TAG => "Game - Moving Camera";

        /// <summary>
        /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
        /// </summary>
        public static MovingCamera Current { get; private set; }

        // OTHERS ATTRIBUTES
        [HideInInspector]
        /// <summary> 
        /// Multiplier value that affects camera moving speed
        /// </summary>
        public float CameraMovingSpeed = 5f; 

        private float _yRotation;

        private float _xRotation;

        private Vector3 _currentRotation = Quaternion.identity.eulerAngles;

        private Vector3 _currentTranslation = Vector3.zero;

        private Vector3 _currentVelocity = Vector3.zero; // Current vector3 which represents camera's movement

        private Vector3 _rotationCenter;

        [HideInInspector]
        /// <summary>
        /// Represents the relative time (in seconds) that camera will take to move at its next position.
        /// A smaller value would increase camera speed, and a bigger one would decrease it.
        /// </summary>
        public float _smoothFactor = 0.01f;

        private Vector3 _minPolycubePos; // Minimal possible position in space of one face of the polycube

        private Vector3 _maxPolycubePos; // Maximal possible position in space of one face of the polycube

        [Header("Texture and targeted object settings")]
        [Space(5.0f)]

        /// <summary>
        /// Should be the default mouse's cursor texture. Can be null.
        /// </summary>
        [Tooltip("Should be the default mouse's cursor texture. Can be null.")]
        public Texture2D BaseMouseTexture;

        /// <summary>
        /// Should be the grab mouse's cursor texture. Can also be null.
        /// </summary>
        [Tooltip("Should be the grab mouse's cursor texture. Can also be null.")]
        public Texture2D GrabMouseTexture;

        [HideInInspector]
        /// <summary>
        /// Multiplier value to increase or decrease zoom speed
        /// </summary>
        public float ZoomSensitivity = 0.2f;

        private float _zoomLevel = 100f; // Zoom value, set to default at 10

        private const float _ZOOM_MIN = 50f; // Min value that zoom can take

        private const float _ZOOM_MAX = 400f; // Max value that zoom can take

        private bool _isGameStarted = false;

        /// <summary>
        /// Awake method, called first in script life's cycle.
        /// Used here to initialize the singleton of this class.
        /// </summary>
        private void Awake() => MovingCamera.Current = this;

        /// <summary>
        /// Update function, called once per frame.
        /// Used here to handle camera's movements on player's actions.
        /// </summary>
        private void Update()
        {
            // We need firts to test if game has started (in other terms, if polycube has been generated)
            if (_isGameStarted)
            {
                // Two floats to store input's mouse axis values
                float mouseX,
                    mouseY;

                // Camera movements are holded here with grab by user on mouse's right click
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    /* Custom cursor texture applied to notify user that he is currently moving the camera. */
                    // UnityEngine.Cursor.SetCursor(_grabMouseTexture, Vector2.zero, CursorMode.Auto);

                    // On grab, cursor is locked at center of the screen and invisible
                    UI.Utils.Helper.ChangeCursorState();

                    // Getting moving values from x and y axis
                    mouseX = Input.GetAxis("Mouse X") * CameraMovingSpeed;
                    mouseY = -Input.GetAxis("Mouse Y") * CameraMovingSpeed;

                    // Updating x and y rotation values
                    _xRotation += mouseY;
                    _yRotation += mouseX;

                    // Clamping x rotation to prevent user going too far upside or downside
                    _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                    // Calculate next camera rotation according to x and y rotation values
                    var nextRotation = new Vector3(_xRotation, _yRotation);

                    // Updating current rotation with smooth damp (this function allows to gradually modify the first given vector)
                    _currentRotation = Vector3.SmoothDamp(
                        _currentRotation,
                        nextRotation,
                        ref _currentVelocity,
                        _smoothFactor
                    );

                    // After calculations we can now update the real value of camera rotation
                    this.transform.eulerAngles = _currentRotation;

                    // And then updating camera's position
                    this.transform.position =
                        _rotationCenter + this.transform.forward * -_zoomLevel;

                    // Unlocking cursor
                    UI.Utils.Helper.ChangeCursorState();
                }
                else if (Input.GetKey(KeyCode.Mouse2)) // Middle click for camera's translation
                {
                    /* Custom cursor texture applied to notify user that he sis currently moving the camera. */
                    // UnityEngine.Cursor.SetCursor(_grabMouseTexture, Vector2.zero, CursorMode.Auto);

                    // On grab, cursor is locked at center of the screen and invisible
                    UI.Utils.Helper.ChangeCursorState();

                    // Getting moving values from x and y axis
                    mouseX = Input.GetAxis("Mouse X") * CameraMovingSpeed;
                    mouseY = Input.GetAxis("Mouse Y") * -CameraMovingSpeed;

                    // Updating current translation (and so camera's position)
                    _currentTranslation -= this.transform.right * mouseX;
                    _currentTranslation += this.transform.up * mouseY;

                    // If camera position should be updated, then we should also update rotation center
                    _rotationCenter =
                        ((_minPolycubePos + _maxPolycubePos) / 2) + _currentTranslation;

                    // And then we can update camera positio
                    this.transform.position = _rotationCenter - transform.forward * _zoomLevel;

                    // Unlocking cursor
                    UI.Utils.Helper.ChangeCursorState();
                }
                else
                {
                    /* Default mouse cursor texture applied to notify user that he is no longer moving the camera. */
                    // UnityEngine.Cursor.SetCursor(_baseMouseTexture, Vector2.zero, CursorMode.Auto);
                    mouseX = mouseY = 0f;
                    _xRotation = _currentRotation.x;
                    _yRotation = _currentRotation.y;
                    _currentVelocity = Vector3.zero;
                }

                // Camera's zoom are handled by mouse's scrollwheel
                if (Input.GetAxis("Mouse ScrollWheel") is not 0)
                {
                    // Updating camera's zoom value
                    _zoomLevel +=
                        (Input.GetAxis("Mouse ScrollWheel") >= 0 ? -1 : 1) * ZoomSensitivity;

                    // Clamping camera's zoom to min and max values
                    float length_rotationCenterMaxVertexPolycube = (
                        _rotationCenter - _maxPolycubePos
                    ).magnitude;

                    _zoomLevel = Mathf.Clamp(
                        _zoomLevel,
                        length_rotationCenterMaxVertexPolycube + _ZOOM_MIN,
                        _ZOOM_MAX
                    );

                    // Updating camera's position according to zoom value
                    this.transform.position = _rotationCenter - transform.forward * _zoomLevel;
                }

                // =====================================================
                // Camera's movements with ZSQD or UP, DOWN, RIGHT, LEFT
                // =====================================================
                // And yes this is ugly as fuck
                int _horizontalFactor = 60;
                int _verticalFactor = 100;
                if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                {
                    if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) {
                        // Getting moving values from x and y axis
                        mouseX = 0 * CameraMovingSpeed;
                        mouseY = _verticalFactor * CameraMovingSpeed;

                        // Updating x and y rotation values
                        _xRotation += mouseY;
                        _yRotation += mouseX;

                        // Clamping x rotation to prevent user going too far upside or downside
                        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                        // Calculate next camera rotation according to x and y rotation values
                        var nextRotation = new Vector3(_xRotation, _yRotation);

                        // Updating current rotation with smooth damp (this function allows to gradually modify the first given vector)
                        _currentRotation = Vector3.SmoothDamp(
                            _currentRotation,
                            nextRotation,
                            ref _currentVelocity,
                            _smoothFactor
                        );

                        // After calculations we can now update the real value of camera rotation
                        this.transform.eulerAngles = _currentRotation;

                        // And then updating camera's position
                        this.transform.position =
                            _rotationCenter + this.transform.forward * -_zoomLevel;
                    } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                        // Getting moving values from x and y axis
                        mouseX = 0 * CameraMovingSpeed;
                        mouseY = -_verticalFactor * CameraMovingSpeed;

                        // Updating x and y rotation values
                        _xRotation += mouseY;
                        _yRotation += mouseX;

                        // Clamping x rotation to prevent user going too far upside or downside
                        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                        // Calculate next camera rotation according to x and y rotation values
                        var nextRotation = new Vector3(_xRotation, _yRotation);

                        // Updating current rotation with smooth damp (this function allows to gradually modify the first given vector)
                        _currentRotation = Vector3.SmoothDamp(
                            _currentRotation,
                            nextRotation,
                            ref _currentVelocity,
                            _smoothFactor
                        );

                        // After calculations we can now update the real value of camera rotation
                        this.transform.eulerAngles = _currentRotation;

                        // And then updating camera's position
                        this.transform.position =
                            _rotationCenter + this.transform.forward * -_zoomLevel;
                    }
                    if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) {
                        // Getting moving values from x and y axis
                        mouseX = _horizontalFactor * CameraMovingSpeed;
                        mouseY = 0 * CameraMovingSpeed;

                        // Updating x and y rotation values
                        _xRotation += mouseY;
                        _yRotation += mouseX;

                        // Clamping x rotation to prevent user going too far upside or downside
                        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                        // Calculate next camera rotation according to x and y rotation values
                        var nextRotation = new Vector3(_xRotation, _yRotation);

                        // Updating current rotation with smooth damp (this function allows to gradually modify the first given vector)
                        _currentRotation = Vector3.SmoothDamp(
                            _currentRotation,
                            nextRotation,
                            ref _currentVelocity,
                            _smoothFactor
                        );

                        // After calculations we can now update the real value of camera rotation
                        this.transform.eulerAngles = _currentRotation;

                        // And then updating camera's position
                        this.transform.position =
                            _rotationCenter + this.transform.forward * -_zoomLevel;
                    } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                        // Getting moving values from x and y axis
                        mouseX = -_horizontalFactor * CameraMovingSpeed;
                        mouseY = 0 * CameraMovingSpeed;

                        // Updating x and y rotation values
                        _xRotation += mouseY;
                        _yRotation += mouseX;

                        // Clamping x rotation to prevent user going too far upside or downside
                        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                        // Calculate next camera rotation according to x and y rotation values
                        var nextRotation = new Vector3(_xRotation, _yRotation);

                        // Updating current rotation with smooth damp (this function allows to gradually modify the first given vector)
                        _currentRotation = Vector3.SmoothDamp(
                            _currentRotation,
                            nextRotation,
                            ref _currentVelocity,
                            _smoothFactor
                        );

                        // After calculations we can now update the real value of camera rotation
                        this.transform.eulerAngles = _currentRotation;

                        // And then updating camera's position
                        this.transform.position =
                            _rotationCenter + this.transform.forward * -_zoomLevel;
                    }
                }
            }
        }

        /// <summary>
        /// This function allows another class (in our case, PolyCubeManager) to initialize
        /// transform component of the moving camera (position and rotation).
        /// </summary>
        public void InitializeCamera()
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            _currentRotation = Quaternion.identity.eulerAngles;
            _currentTranslation = Vector3.zero;
            _currentVelocity = Vector3.zero; // Current vector3 which represents camera's movement

            // Setting value of minimal polycube's position
            _minPolycubePos =
                VectorConverter.ToUnityEngineVector(PolyCubeManager.Current.PolyCubeGraph.Min)
                * PolyCubeManager.Current.FaceScale;

            // Setting value of maximal polycube's position
            _maxPolycubePos =
                VectorConverter.ToUnityEngineVector(PolyCubeManager.Current.PolyCubeGraph.Max)
                * PolyCubeManager.Current.FaceScale;

            // From both previous variables, we can deduct coordinates of the rotation center
            _rotationCenter = ((_minPolycubePos + _maxPolycubePos) / 2f) + _currentTranslation;

            // Set correct zoom level according to the size of the polycube
            float length_rotationCenterMaxVertexPolycube = (
                _rotationCenter - _maxPolycubePos
            ).magnitude;

            // Clamp the zoom level between min and max values it can takes
            _zoomLevel = Mathf.Clamp(
                _zoomLevel,
                length_rotationCenterMaxVertexPolycube + _ZOOM_MIN + 8f,
                _ZOOM_MAX
            );

            // Initialize camera position
            this.transform.position = _rotationCenter - transform.forward * _zoomLevel;

            // Reset camera's rotation (and so position)
            this.transform.rotation = Quaternion.identity;
            this.transform.RotateAround(_rotationCenter, new Vector3(1, 0, 0), 45);
            this.transform.RotateAround(_rotationCenter, new Vector3(0, 1, 0), 45);

            // Updating camera's current rotation
            _currentRotation = this.transform.eulerAngles;

            // Updating current x and y rotations
            _xRotation = _currentRotation.x;
            _yRotation = _currentRotation.y;

            // Modifying is game started to notify update function that it
            // can now handle camera's movements.
            _isGameStarted = true;
        }
    }
}
