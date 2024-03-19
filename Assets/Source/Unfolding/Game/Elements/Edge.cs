using Unfolding.Game.Gameplay;
using Unfolding.Utils;
using UnityEngine;

namespace Unfolding.Game.Elements
{
    /// <summary>
    /// A class that handles a edge as a 3D Object in Unity
    /// </summary>
    public class Edge : MonoBehaviour
    {
        /// <summary>
        /// Game camera, used to cast rays
        /// </summary>
        private Camera m_camera;

        /// <summary>
        /// Represent if the edge is selected
        /// </summary>
        public bool m_isSelected;

        /// <summary>
        /// Start method, executed at the start of the game
        /// Get current camera and set edge to not selected
        /// </summary>
        private void Start()
        {
            this.m_isSelected = false;
            this.m_camera = CamerasManager.Current.GetGameCamera().GetComponent<Camera>();
        }

        /// <summary>
        ///	Executed at each frame.
        ///	Detect if the edge is hovered and/or clicked.
        /// </summary>
        private void Update()
        {
            DetectHovering();
            DetectClickOnObject();
            if (m_isSelected)
            {
                GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            }
        }

        /// <summary>
        /// Detect if the cursor is hovering the edge.
        /// Cast a ray from the camera in the direction of the cursor on the screen and then check if the ray hit the edge.
        /// If so, the emission map of the edge material is enabled to show the highlighting.
        /// </summary>
        private void DetectHovering()
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
        /// Detect if the edge is clicked.
        /// If the left click is pressed, a ray is casted in the direction of the cursor from the camera.
        /// If the ray hit the edge, the edge is selected in the <c>PolyCubeManager</c>
        /// </summary>
        private void DetectClickOnObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray m_ray = m_camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit m_hit;
                if (Physics.Raycast(m_ray, out m_hit))
                {
                    if (m_hit.transform == transform)
                    {
                        if (this.m_isSelected)
                        {
                            this.m_isSelected = !this.m_isSelected;
                            PolyCubeManager.Current.StopCoroutine("WaitForUserChoice");
                            PolyCubeManager.Current.DestroyDirectionnalArrows();
                            PolyCubeManager.Current.ClearFacesSelected();
                            PolyCubeManager.Current.EdgeSelected = null;
                            PolyCubeManager.Current.IsInUnfoldingProcess = false;
                            Debug.unityLogger.Log("Edge", "Unfolding stopped by edge " + this.transform.position / PolyCubeManager.Current.FaceScale);
                        }
                        else
                        {
                            PolyCubeManager.Current.EdgeSelected = this;
                            this.m_isSelected = !this.m_isSelected;
                        }
                    }
                }
            }
        }
    }
}
