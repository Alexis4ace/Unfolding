using Unfolding.Utils;
using UnityEngine;

namespace Unfolding.Game.Elements
{
    /// <summary>
    /// This class handles the instanciation and the rendering of the floor
    /// </summary>
    public class Floor : MonoBehaviour, ITaggable
    {
        public string TAG => "Floor";
        
        /// <summary>
        /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
        /// </summary>
        public static Floor Current { get; private set; }

        [Header("Material reference")]
        [SerializeField] private Material floorMat;

        /// <summary>
        /// Awake method, called first in script life's cycle.
        /// </summary>
        private void Awake()
        {
            Floor.Current = this;    
        }

        /// <summary>
        /// Start method, called before first frame.
        /// It's mainly used here to initialize the scale of the localScale of the floor.
        /// In addition, we disable the MeshRenderer of the floor so that it is not displayed
        /// </summary>
        private void Start()
        {
            this.transform.localScale = new Vector3(100, 100, 100);
        }

        /// <summary>
        /// This function render the board by enabling the component MeshRenderer of the GameObject Floor
        /// </summary>
        public void RenderBoard()
        {
            Current.GetComponent<MeshRenderer>().enabled = true;
        }

        /// <summary>
        /// This function hide the board by disabling the component MeshRenderer of the GameObject Floor
        /// </summary>
        public void HideBoard()
        {
            Current.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
