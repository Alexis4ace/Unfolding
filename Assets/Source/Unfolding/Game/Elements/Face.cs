using UnityEngine;
using Unfolding.Utils;
using Unfolding.Game.Gameplay;

namespace Unfolding.Game.Elements
{    
    /// <summary>
    /// A class that handles a face as a 3D Object in Unity
    /// </summary>
    public class Face : MonoBehaviour
    {
        /// <summary>
        /// Face unfolding speed
        /// </summary>
        [Tooltip("Unfolding speed")]
        public static float s_unfoldingSpeed = 100f;

        /// <summary>
        /// Possible colors of a face
        /// </summary>
        [Tooltip("Possible colors of a face")]
        public Color[] m_colors =
        {
            Color.yellow,
            Color.blue,
            Color.red,
            Color.green,
            Color.white,
            Color.Lerp(Color.yellow, Color.red, 0.5f),
            Color.black
        };

        /// <summary>
        /// Game camera, needed for raycasting
        /// </summary>
        private Camera m_camera;

        /// <summary>
        /// Represent if the face is selected
        /// </summary>
        /// <remarks> This variable is public because it must be accessible from the PolyCubeManager </remarks>
        public bool m_isSelected;

        /// <summary>
        /// Attributes needed for the unfolding process
        /// If set to true, begin the linear interpolation between oldPos/Ori and newPos/Ori
        /// </summary>
        private bool isUnfolding;

        /// <summary>
        /// Former position of the face
        /// </summary>
        private Vector3 oldPos;

        /// <summary>
        /// Former Unfolding.Kernel.orientation of the face
        /// </summary>
        private Vector3 oldOri;

        /// <summary>
        /// New position of the face
        /// </summary>
        private Vector3 nextPos;

        /// <summary>
        /// New Unfolding.Kernel.orientation of the face
        /// </summary>
        private Vector3 nextOri;

        /// <summary>
        /// Represent the progress of the unfolding (scale from 0 to 1)
        /// </summary>
        private float unfoldingProgress;

        /// <summary>
        /// Twin of the face in the kernel
        /// </summary>
        private Kernel.Face _attachedFace;

        /// <summary>
        /// The position of the face divided by the scale it, to make the link between unity representation and the kernel
        /// </summary>
        /// <remarks> This variable is public because it needs to be accessible from outside the class </remarks>
        public System.Numerics.Vector3 normalizedPosition;

        /// <summary>
        /// Define the getter and setter of attachedFace
        /// </summary>
        public Kernel.Face AttachedFace
        {
            get { return _attachedFace; }
            set { _attachedFace = value; }
        }

        /// <summary>
        /// Start method, executed at the start of the game
        /// Get current camera and generate color of the face
        /// </summary>
        void Start()
        {
            this.m_isSelected = false;
            this.m_camera = CamerasManager.Current.GetGameCamera().GetComponent<Camera>();
            this.GetComponent<Renderer>().material.color = m_colors[AttachedFace.IdColor];

            normalizedPosition = VectorConverter.ToSystemVector(
                this.transform.position / PolyCubeManager.Current.FaceScale
            );
        }

        /// <summary>
        ///	Executed at each frame.
        ///	Detect if the face is hovered and/or clicked.
        ///	Process of the unfolding is done here due to multiple frames animations
        /// </summary>
        void Update()
        {
            DetectHovering();
            DetectClickOnObject();
            if (m_isSelected)
            {
                this.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            }

            if (isUnfolding)
            {
                this.transform.localPosition = Vector3.Lerp(
                    this.oldPos,
                    this.nextPos,
                    unfoldingProgress
                );
                this.transform.eulerAngles = Vector3.Lerp(
                    this.oldOri,
                    this.nextOri,
                    unfoldingProgress
                );

                this.unfoldingProgress += 1f / s_unfoldingSpeed;
                if (this.unfoldingProgress == 1f)
                {
                    this.transform.localPosition = this.nextPos;
                    this.transform.localRotation = Quaternion.Euler(this.nextOri);
                    this.isUnfolding = false;
                }
            }
        }

        /// <summary>
        /// Detect if the cursor is hovering the face.
        /// Cast a ray from the camera in the direction of the cursor on the screen and then check if the ray hit the face.
        /// If so, the emission map of the face material is enabled to show the highlighting.
        /// </summary>
        void DetectHovering()
        {
            Ray m_ray = m_camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit m_hit;
            if (Physics.Raycast(m_ray, out m_hit))
            {
                if (m_hit.transform == transform)
                {
                    GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                }
                else
                {
                    GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                }
            }
            else
            {
                GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            }
        }

        /// <summary>
        /// Detect if the face is clicked.
        /// If the left click is pressed, a ray is casted in the direction of the cursor from the camera.
        /// If the ray hit the face, the face is selected and added to the list of selected face in the <c>PolyCubeManager</c>
        /// </summary>
        void DetectClickOnObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray m_ray = m_camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit m_hit;
                if (Physics.Raycast(m_ray, out m_hit))
                {
                    if (m_hit.transform == transform)
                    {
                        m_isSelected = !m_isSelected; //switch the value of selection of a face
                        if (m_isSelected)
                        {
                            //Every added face in faceSelected are normalized
                            PolyCubeManager.Current.FacesSelected.Add(this.gameObject);
                        }
                        else
                        {
                            PolyCubeManager.Current.FacesSelected.Remove(this.gameObject);
                        }
                        //                        PolyCubeManager.Current.ToStringListOfFaces();
                    }
                }
            }
        }

        /// <summary>
        /// Unfold the face instantly
        /// </summary>
        /// <param name="newpos">the new position of the face</param>
        /// <param name="newori">the new Unfolding.Kernel.orientation of the face</param>
        public void StaticUnfolding(Vector3 newpos, Unfolding.Kernel.orientation newori)
        {
            string ori = "";
            //Compute rotation
            transform.rotation = Quaternion.identity;
            switch (newori)
            {
                case Unfolding.Kernel.orientation.X:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    ori = "X";
                    break;
                case Unfolding.Kernel.orientation.Y:
                    transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                    ori = "Y";
                    break;
                case Unfolding.Kernel.orientation.Z:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    ori = "Z";
                    break;
            }
            transform.position = newpos * PolyCubeManager.Current.FaceScale;
            this.gameObject.name =
                "Face_" + ori + Unfolding.Utils.VectorConverter.ToSystemVector(newpos);
        }

        /// <summary>
        /// Unfold the face with an animation. Initiate values for the unfolding.
        /// The unfolding process (linear interpolation) takes place in the <c>Update</c> lifecycle function
        /// </summary>
        /// <param name="newpos">the new position of the face</param>
        /// <param name="newori">the new Unfolding.Kernel.orientation of the face</param>
        public void DynamicUnfolding(Vector3 newpos, Unfolding.Kernel.orientation newori)
        {
            // Get old values of position and rotation for the face to prevent interferencs during unfolding process
            Debug.Log("New pos " + newpos + ", new ori " + newori);
            this.oldPos = this.transform.localPosition;
            this.oldOri = this.transform.localRotation.eulerAngles;

            // Store futures values for the face
            this.nextPos = newpos * PolyCubeManager.Current.FaceScale;

            // TODO: Fix this, strange unfolding
            this.nextOri =
                (
                    newori == Unfolding.Kernel.orientation.X
                        ? Vector3.up
                        : newori == Unfolding.Kernel.orientation.Y
                            ? Vector3.right
                            : Vector3.forward
                ) * 90f;

            // Activate unfolding in the Update function
            this.unfoldingProgress = 0f;
            this.isUnfolding = true;
        }
    }
}
