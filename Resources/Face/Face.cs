using UnityEngine;
using Unfolding.Utils;

namespace Unfolding.Game.Elements
{
    public class Face : MonoBehaviour
    {
        public const float UNFOLD_ANGLE = 2f;
        private static Color orange = Color.Lerp(Color.yellow, Color.red, 0.5f);
        public Color[] m_colors =
        {
            Color.yellow,
            Color.blue,
            Color.red,
            Color.green,
            Color.white,
            orange
        };

        private Camera m_camera;
        private Ray m_ray;
        private RaycastHit m_hit;
        private bool m_isSelected;
        private Vector3 initialPos;
        private bool isUnfolding;

        void Start()
        {
            this.m_isSelected = false;
            this.m_camera = Camera.main;
            this.GetComponent<Renderer>().material.color = m_colors[Random.Range(0, 6)];
            this.initialPos = this.transform.position;
        }

        void Update()
        {
            DetectHovering();
            DetectClickOnObject();
        }

        void DetectHovering()
        {
            m_ray = m_camera.ScreenPointToRay(Input.mousePosition);
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

        void DetectClickOnObject()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Click");
                //m_ray = new Ray(
                //    m_camera.ScreenToWorldPoint(Input.mousePosition),
                //    m_camera.transform.forward
                //);

                m_ray = m_camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(m_ray, out m_hit))
                {
                    if (m_hit.transform == transform)
                    {
                        Debug.Log($"{m_hit.collider.name} Detected", m_hit.collider.gameObject);
                        this.GetComponent<Renderer>().material.color = m_colors[Random.Range(0, 6)];
                    }
                }
            }
        }

        public void Unfold(OrientedAxis axis, OrientedAxis edgeSide, bool rotateDirection)
        {
            this.transform.RotateAround(
                this.initialPos
                    + new Vector3(
                        (
                            edgeSide == OrientedAxis.Xplus
                                ? 0.5f
                                : edgeSide == OrientedAxis.Xminus
                                    ? -0.5f
                                    : 0
                        ),
                        (
                            edgeSide == OrientedAxis.Yplus
                                ? 0.5f
                                : edgeSide == OrientedAxis.Yminus
                                    ? -0.5f
                                    : 0
                        ),
                        (
                            edgeSide == OrientedAxis.Zplus
                                ? 0.5f
                                : edgeSide == OrientedAxis.Zminus
                                    ? -0.5f
                                    : 0
                        )
                    ),
                new Vector3(
                    (
                        axis == OrientedAxis.Xplus
                            ? 1
                            : axis == OrientedAxis.Xminus
                                ? -1
                                : 0
                    ),
                    (
                        axis == OrientedAxis.Yplus
                            ? 1
                            : axis == OrientedAxis.Yminus
                                ? -1
                                : 0
                    ),
                    (
                        axis == OrientedAxis.Zplus
                            ? 1
                            : axis == OrientedAxis.Zminus
                                ? -1
                                : 0
                    )
                ),
                (rotateDirection ? 1f : -1f) * UNFOLD_ANGLE
            );

            if (Vector3.Angle(this.initialPos, this.transform.position) == 90f)
            {
                this.initialPos = this.transform.position;
                this.isUnfolding = false;
            }
        }
    }
}
