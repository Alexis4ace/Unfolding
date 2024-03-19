using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unfolding.Utils;
using Unfolding.Game.IO;
using Unfolding.Game.Utils;
using Unfolding.Game.Elements;
using Unfolding.UI.Game;


namespace Unfolding.Game.Gameplay
{
    /// <summary>
    /// This class handles generation and functionnalities of the polycube in the game's view.
    /// </summary>
    /// <remarks>This class is the controller of the struture to unfold (game structure).</remarks>
    public class PolyCubeManager : MonoBehaviour, ITaggable
    {
        // CLASS'S ATTRIBUTES
        public string TAG => "Game - Polycube Manager";

        /// <summary>
        /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
        /// </summary>
        public static PolyCubeManager Current { get; private set; }

        /*
            NOTE: why using the Singleton pattern here with Current variable ?
            Because in Unfolding, there can only one PolyCube at a time.
        */

        [Header("Face settings")]
        [Space(5.0f)]
        [Tooltip("The prefab object associated to a face.")]
        [SerializeField]
        private GameObject _facePrefab;

        [Tooltip("The prefab object associated to an edge")]
        [SerializeField]
        private GameObject _edgePrefab;

        /// <summary>
        /// Scale of a face (same value in x, y and z).
        /// </summary>
        [Tooltip("Scale of a face (same value in x, y and z).")]
        public int FaceScale = 20;

        [Tooltip("Material associated on every faces of each cubes in the polycube.")]
        [SerializeField]
        private Material _faceMaterial;

        private int _numberOfCubes;

        /// <summary>
        /// The graph of the current game. It holds the polycube structure created by the kernel.
        /// </summary>
        public Kernel.Graphe PolyCubeGraph { get; private set; }

        /// <summary>
        /// A Dictionnary of every face in the game.
        /// For an entire game session, no face can be removed.
        /// </summary>
        /// <remarks> The key of the dictionnary is the position of the center of a face, because we can't have 2 faces in the same position. </remarks>
        public Dictionary<System.Numerics.Vector3, GameObject> CreatedFacesList {get; private set;}


        /// <summary>
        /// A Dictionnary of every created edge in the game at a given time.
        /// </summary>
        /// <remarks> The key of the dictionnary is the position of the center of an edge, because we can't have 2 edges in the same position. </remarks>
        public Dictionary<System.Numerics.Vector3, GameObject> CreatedEdgesList;


        /// <summary>
        /// A list of selected faces in the game for the unfolding process.
        /// This list is updated everytime the player clicks on a face.
        /// </summary>
        /// <remarks> This variable has to be public so that it can be modify from unity-face script. </remarks>
        public List<GameObject> FacesSelected; // Faces in Unity

        /// <summary>
        /// The selected edge in the game for the unfolding process.
        /// This variable is updated everytime the player clicks on an edge.
        /// </summary>
        /// <remarks> This variable has to be public so that it can be modify from unity-edge script. </remarks>
        public Edge EdgeSelected;

        [Header("Directionnal arrows settings")]
        [Space(5f)]
        [Tooltip("Should be the 3d object representing directionnal arrows.")]
        [SerializeField]
        private GameObject _arrowPrefab;

        private float _arrowDistanceFromFace = 40f; // How far will arrows be generated from the faces

        [Tooltip("Should be the parent from where directionnal arrows will be generated.")]
        /// <summary>
        /// The transform component of the parent gameObject of every created arrow
        /// </summary>
        public Transform DirectionnalArrowsRoot;

        private List<GameObject> _generatedArrows = new List<GameObject>(2); // List to store generated arrows

        /// <summary>
        /// A boolean that tells wether we are in the unfolding process or not.
        /// </summary>
        /// <remarks> This variable is public because it is accessed by unity edge script to abort the unfolding process. </remarks>
        [HideInInspector] public bool IsInUnfoldingProcess = false;

        private bool _hasGameEnded = false; // A boolean to store state of the game

        /// <summary>
        /// A small struct used for store data between unfolding functions.
        /// </summary>
        private struct UnfoldingData
        {
            // Creating all needed data which will transit between unfolding functions
            public List<System.Numerics.Vector3> FacesPositionsRotatedInPositive { get; }
            public List<System.Numerics.Vector3> FacesPositionsRotatedInNegative { get; }
            public System.Numerics.Vector3 Edge { get; }
            public List<Kernel.Face> CompletedFaceList { get; }
            public List<GameObject> CompletedUnityFaceList { get; }
            public List<System.Numerics.Vector3> OldPositions { get; }

            // Constructor
            public UnfoldingData(
                List<System.Numerics.Vector3> facesPositionsRotatedInPositive,
                List<System.Numerics.Vector3> facesPositionsRotatedInNegative,
                System.Numerics.Vector3 edge,
                List<Kernel.Face> completedFaceList,
                List<GameObject> completedUnityFaceList,
                List<System.Numerics.Vector3> oldPositions
            ) {
                this.FacesPositionsRotatedInPositive = facesPositionsRotatedInPositive;
                this.FacesPositionsRotatedInNegative = facesPositionsRotatedInNegative;
                this.Edge = edge;
                this.CompletedFaceList = completedFaceList;
                this.CompletedUnityFaceList = completedUnityFaceList;
                this.OldPositions = oldPositions;
            }
        }

        /// <summary>
        /// Awake method, first method called in script's life cycle.
        /// </summary>
        private void Awake()
        {
            FacesSelected = new List<GameObject>();
            FacesSelected.Clear();
            PolyCubeManager.Current = this;
            CreatedFacesList = new Dictionary<System.Numerics.Vector3, GameObject>();
            CreatedEdgesList = new Dictionary<System.Numerics.Vector3, GameObject>();
            //UI.Utils.Helper.ResetGameData();
            EdgeSelected = null;
        }

        /// <summary>
        /// A function that initializes the game view and launch the game
        /// </summary>
        public void LaunchGame()
        {
            // Get the polycube stored in PlayerPrefs
            string listFaces = PlayerPrefs.GetString(PlayerPrefsTags.ListOfFaces);
            _numberOfCubes = PlayerPrefs.GetInt(PlayerPrefsTags.NumberOfCubes);

            if (string.IsNullOrEmpty(listFaces))
            {
                Debug.Log("Are we empty ?");
                PolyCubeGraph = new Kernel.Graphe(_numberOfCubes);
                IOPlayerPrefs.WritePlayerPrefs(PolyCubeGraph);
            }
            else
            {
                PolyCubeGraph = IOPlayerPrefs.ReadPlayerPrefs();
            }
            
            //Debug.Log(PolyCubeGraph.PrintFacesDebugLog());
            this.DrawPolyCube();

            // Start the timer (USELESS)
            //TimerManager.Current.StartTimer();

            GameObject.Find("Floor").GetComponent<Floor>().RenderBoard();
            MovingCamera.Current.InitializeCamera();

            // Initializing _hasGameEnded at false systematically
            _hasGameEnded = false;
        }

        /// <summary>
        /// A function that clear the game view before moving into the menu interface
        /// </summary>
        public void DestroyGame()
        {
            PolyCubeGraph = null;
            //UI.Utils.Helper.ResetGameData();

            // Clear all created objects (USELESS NOW)
            foreach (Transform t in GetComponentInChildren<Transform>())
            {
                Destroy(t.gameObject);
            }

            DestroyDirectionnalArrows();
            FacesSelected.Clear();
            CreatedFacesList.Clear();
            CreatedEdgesList.Clear();

            // Stop the timer
            TimerManager.Current.StopTimer();

            // Hide board
            //Floor.Current.GetComponent<Floor>().HideBoard();

            DebugSphere.Current.CleanDebugSphere();
        }

        /// <summary>
        /// A method that manage the behavior at the end of the game
        /// </summary>
        public void EndGame()
        {
            // Desactivate Undo and Unselect Button and Game Options
            GameObject.Find("Unselect Button").SetActive(false); 
            GameObject.Find("Undo Button").SetActive(false);
            GameObject.Find("Game Options").SetActive(false);

            // Stop the timer
            TimerManager.Current.StopTimer();

            // Update _hasGameEnded to disable unfold actions
            _hasGameEnded = true;

            // Display end menu
            GameObject.Find("Panels").GetComponent<AnimatorsManager>().PlayAnimation("End");
        }

        public void TakeScreenShot()
        {
            // Get the path to the My Pictures folder
            string picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            // Create a folder if it doesn't exist
            string screenshotFolder = Path.Combine(picturesFolder, "Unfolding Screenshots");
            Directory.CreateDirectory(screenshotFolder);

            // Generate the screenshot file path
            string screenshotPath = Path.Combine(screenshotFolder, "screenshot " + DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)") + ".png");

            // Take the screenshot
            CamerasManager.Current.StartCoroutine("TakeScreenShot", screenshotPath);

            // Display the popup
            string popupMessage = "Congratulations, you've won !\n" +
                                "A screenshot of your pattern has been saved at :\n" +
                                screenshotPath;
            UI.Game.PopupsManager.Current.DisplayPopup("", 10f, popupMessage);
        }

        /// <summary>
        /// This method creates and renders all the faces of the polycube, using <see cref="PolycubeManager.CreateFace"/>
        /// </summary>
        private void DrawPolyCube()
        {
            List<Kernel.Face> faceList = PolyCubeGraph.GetFacesList();
            foreach (GameObject face in this.CreatedFacesList.Values.ToList<GameObject>())
            {
                Destroy(face);
            }
            this.CreatedFacesList.Clear();
            foreach (Kernel.Face v in faceList)
            {
                this.CreateFace(v);
                
            }
            foreach (GameObject edge in this.CreatedEdgesList.Values.ToList<GameObject>())
            {
                Destroy(edge);
            }
            CreatedEdgesList.Clear();
            foreach (System.Numerics.Vector3 edge in PolyCubeGraph.GetEdgesPosition())
            {
                this.CreateEdge(edge);
            }
        }

        /// <summary>
        /// This method creates and renders a face
        /// </summary>
        /// <param name="v">Face to create and render</param>
        private void CreateFace(Kernel.Face v)
        {
            GameObject face = Instantiate(_facePrefab) as GameObject;
            //face.layer = 6; // Setted to 6 because sixth layer is attached to 3D Object
            face.GetComponent<Elements.Face>().AttachedFace = v;
            Quaternion rotation = Quaternion.identity;
            switch (v.Orientation)
            {
                case Unfolding.Kernel.orientation.X:
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    face.name = "Face_X" + v.Position;
                    break;
                case Unfolding.Kernel.orientation.Y:
                    rotation = Quaternion.Euler(90f, 0f, 0f);
                    face.name = "Face_Y" + v.Position;
                    break;
                case Unfolding.Kernel.orientation.Z:
                    rotation = Quaternion.Euler(0f, 0f, 90f);
                    face.name = "Face_Z" + v.Position;
                    break;
            }

            System.Numerics.Vector3 pos = v.Position;
            Vector3 unitypos = VectorConverter.ToUnityEngineVector(pos);
            face.transform.localScale *= FaceScale * 2;
            face.transform.localPosition = (unitypos * FaceScale);
            face.transform.rotation = rotation;
            face.transform.SetParent(this.transform, true);

            CreatedFacesList.Add(v.Position, face);
        }

        /// <summary>
        /// This method creates and renders an edge
        /// </summary>
        /// <param name="edgePos">Edge to create and render</param>
        private void CreateEdge(System.Numerics.Vector3 edgePos)
        {
            GameObject edge = Instantiate(_edgePrefab) as GameObject;
            //face.layer = 6; // Setted to 6 because sixth layer is attached to 3D Object
            Quaternion rotation = Quaternion.identity;
            switch (Kernel.Graphe.GetOrientationEdgeFromPos(edgePos))
            {
                case Unfolding.Kernel.orientation.X:
                    rotation = Quaternion.Euler(0f, 0f, 90f);
                    edge.name = "Edge_X" + edgePos;
                    break;
                case Unfolding.Kernel.orientation.Y:
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    edge.name = "Edge_Y" + edgePos;
                    break;
                case Unfolding.Kernel.orientation.Z:
                    rotation = Quaternion.Euler(90f, 0f, 0f);
                    edge.name = "Edge_Z" + edgePos;
                    break;
            }

            Vector3 unitypos = VectorConverter.ToUnityEngineVector(edgePos);
            edge.transform.localScale *= FaceScale * 2;
            edge.transform.localPosition = (unitypos * FaceScale);
            edge.transform.rotation = rotation;
            edge.transform.SetParent(this.transform, true);

            try
            {
                CreatedEdgesList.Add(edgePos, edge);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Update method, called once per frame.
        /// </summary>
        private void Update()
        {
            if (!_hasGameEnded){
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Z))
                {
                    this.Undo();
                }
                else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.U))
                {
                    this.UnselectAll();
                }
                else if (!IsInUnfoldingProcess)
                {
                    if (FacesSelected != null && FacesSelected.Any() && EdgeSelected != null)
                    {
                        IsInUnfoldingProcess = true;
                        System.Numerics.Vector3 facePosition = FacesSelected[0]
                            .GetComponent<Elements.Face>()
                            .AttachedFace.Position;

                        System.Numerics.Vector3 edge = VectorConverter.ToSystemVector(
                            EdgeSelected.transform.localPosition / FaceScale
                        );

                        this.UnityUnfolding(FacesSelected, edge);
                    }
                }
            }
        }

        /// <summary>
        /// The function that manages the unfolding in Unity.
        /// It does not manage all the process of unfolding, since user have to chose several things.
        /// That's why this function have an auxiliary part.
        /// </summary>
        /// <param name="faces">The list of selected faces.</param>
        /// <param name="edge">The edge which is the axis of the unfolding.</param>
        private void UnityUnfolding(List<GameObject> faces, System.Numerics.Vector3 edge)
        {
            // We must create a function searchStartFace
            if (PolyCubeGraph.EdgeIsConnectToOneFace(ListGameObjectToListFace(faces), edge))
            {
                if (
                    PolyCubeGraph.AreFacesLinked(
                        ListGameObjectToListFace(faces),
                        faces[0].GetComponent<Elements.Face>().AttachedFace
                    )
                )
                {
                    System.Numerics.Vector3 startFacePos = PolyCubeGraph.SearchStartFace(
                        ListGameObjectToListFace(faces),
                        edge
                    );

                    Kernel.Face startFace = PolyCubeGraph.Faces[startFacePos];
                    System.Numerics.Vector3 destinationPosition = startFace.SearchFaceFromEdge(
                        edge
                    );

                    if (destinationPosition != Kernel.Face.NULL_FACE)
                    {
                        Kernel.Face destinationFace = PolyCubeGraph.Faces[destinationPosition];

                        // Selected faces + other faces added by the auto unfolding in the Functionnal Core
                        List<Kernel.Face> completedFaceList = PolyCubeGraph.GetFacesToUnfold(
                            ListGameObjectToListFace(faces),
                            destinationFace
                        );

                        // A list that store the previous positions of completedface
                        List<System.Numerics.Vector3> oldPositions =
                            new List<System.Numerics.Vector3>();

                        // Selected faces + other faces added by the auto unfolding in Unity
                        List<GameObject> completedUnityFaceList = new List<GameObject>();

                        foreach (Kernel.Face face in completedFaceList)
                        {
                            GameObject go = CreatedFacesList[face.Position];
                            completedUnityFaceList.Add(go);
                            System.Numerics.Vector3 tmp = new System.Numerics.Vector3(
                                face.X,
                                face.Y,
                                face.Z
                            );
                            oldPositions.Add(tmp);
                        }

                        List<System.Numerics.Vector3> facePosition = PolyCubeGraph.FaceToPosition(
                            completedFaceList
                        );

                        /*
                            Now we are going to generate directionnal arrows which will help the user
                            to chose unfolding direction. Directionnal arrows are generated according to unfolding possibilities,
                            and are destroyed as soon as we don't need them anymore.
                        */

                        // First we need to generate lists of new positions for the face in both negative and positive rotation's directions
                        List<System.Numerics.Vector3> facesPositionsRotatedInPositive =
                            Kernel.MatriceR.RotateGroupOfFace(
                                facePosition,
                                true,
                                startFacePos,
                                edge
                            );

                        List<System.Numerics.Vector3> facesPositionsRotatedInNegative =
                            Kernel.MatriceR.RotateGroupOfFace(
                                facePosition,
                                false,
                                startFacePos,
                                edge
                            );

                        // Now we are going to get if positions in both directions are free
                        Tuple<
                            Tuple<bool, bool>,
                            Tuple<List<System.Numerics.Vector3>, List<System.Numerics.Vector3>>
                        > areNewPositionsFree = PolyCubeGraph.GetUnfoldingDirections(
                            facesPositionsRotatedInPositive,
                            facesPositionsRotatedInNegative,
                            edge,
                            startFacePos,
                            destinationPosition
                        );

                        // And so now, we can generate directionnal arrows
                        this.GenerateDirectionnalArrows(
                            Unfolding.Utils.VectorConverter.ToUnityEngineVector(startFacePos)
                                * FaceScale,
                            faces[0].GetComponent<Elements.Face>().AttachedFace.Orientation,
                            areNewPositionsFree.Item1.Item1,
                            areNewPositionsFree.Item1.Item2
                        );

                        /*
                           NOTE: if several faces are selected, directionnal(s) arrow(s) is (are) generated in sides of
                           only one face.
                       */

                        // Now, we have to create an object that will store unfolding's data for the coroutine
                        UnfoldingData unfoldingData =
                            new(
                                areNewPositionsFree.Item2.Item1,
                                areNewPositionsFree.Item2.Item2,
                                edge,
                                completedFaceList,
                                completedUnityFaceList,
                                oldPositions
                            );

                        // Finally, we have to wait for user to chose a direction by clicking on an arrow. We will use a coroutine for that.
                        this.StartCoroutine("WaitForUserChoice", unfoldingData);
                    }
                }
            }
        }

        /// <summary>
        /// This function allows game to wait until user have clicked on a directionnal arrow.
        /// </summary>
        /// <param name="data">Struct, containing all needed parameters for UnityUnfoldingAux function.</param>
        /// <returns>N/D</returns>
        private IEnumerator WaitForUserChoice(UnfoldingData data)
        {
            if (_generatedArrows.Count is 1) // Only one arrow had been generated
            {
                Debug.unityLogger.Log(TAG, "1 arrow generated");

                yield return new WaitUntil(
                    () => _generatedArrows[0].GetComponent<UnfoldingArrow>().HasBeenClicked is true
                );

                Debug.unityLogger.Log(
                    TAG,
                    "clicked arrow looked at positive on its axis ? : "
                        + _generatedArrows[0].GetComponent<UnfoldingArrow>().LookAtPositive
                );
                // At this point, user should have chose a direction, we can now end the process of unfolding
                if (_generatedArrows[0].GetComponent<UnfoldingArrow>().LookAtPositive)
                    this.UnityUnfoldingAux(
                        data.FacesPositionsRotatedInPositive,
                        data.Edge,
                        data.CompletedFaceList,
                        data.CompletedUnityFaceList
                    );
                else
                    this.UnityUnfoldingAux(
                        data.FacesPositionsRotatedInNegative,
                        data.Edge,
                        data.CompletedFaceList,
                        data.CompletedUnityFaceList
                    );
            }
            else // Two arrows had been generated
            {
                yield return new WaitUntil(
                    () =>
                        _generatedArrows[0].GetComponent<UnfoldingArrow>().HasBeenClicked is true
                        || _generatedArrows[1].GetComponent<UnfoldingArrow>().HasBeenClicked is true
                );

                // We need to retrieve which arrow has been clicked
                var clickedArrow = _generatedArrows.Find(
                    directionnalArrow =>
                        directionnalArrow.GetComponent<UnfoldingArrow>().HasBeenClicked
                );

                Debug.unityLogger.Log(
                    TAG,
                    "clicked arrow looked at positive on its axis ? : "
                        + clickedArrow.GetComponent<UnfoldingArrow>().LookAtPositive
                );
                // At this point, user should have chose a direction, we can now end the process of unfolding

                if (clickedArrow.GetComponent<UnfoldingArrow>().LookAtPositive)
                    this.UnityUnfoldingAux(
                        data.FacesPositionsRotatedInPositive,
                        data.Edge,
                        data.CompletedFaceList,
                        data.CompletedUnityFaceList
                    );
                else
                    this.UnityUnfoldingAux(
                        data.FacesPositionsRotatedInNegative,
                        data.Edge,
                        data.CompletedFaceList,
                        data.CompletedUnityFaceList
                    );
            }

            // Managing the endgame here
            if (PolyCubeGraph.IsStructureAPattern())
            {
                // We call the method that will tell to user that he has won
                this.EndGame();
            }

            ChangeUnityFacePosition(data.CompletedUnityFaceList, data.OldPositions);

            // Notify that we can go back to unfold state again
            IsInUnfoldingProcess = false;
        }

        /// <summary>
        /// This function is the auxiliary method of UnityUnfolding.
        /// It allows when user have chose a direction of unfolding to end the process.
        /// </summary>
        /// <param name="newFacesPositions"> The list of new faces position  </param>
        /// <param name="edge"> The edge using for the unfolding </param>
        /// <param name="completedFaceList"> The list of kernel faces that all faces that has to unfold </param>
        /// <param name="completedUnityFaceList"> The list of gameobject faces that all faces that has to unfold </param>
        private void UnityUnfoldingAux(
            List<System.Numerics.Vector3> newFacesPositions,
            System.Numerics.Vector3 edge,
            List<Kernel.Face> completedFaceList,
            List<GameObject> completedUnityFaceList
        )
        {
            PolyCubeGraph.PushHistory();

            Dictionary<System.Numerics.Vector3, Kernel.Face> subDictionnaryFaces =
                PolyCubeGraph.Faces
                    .Where(kv => completedFaceList.Contains(kv.Value))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

            PolyCubeGraph.DetachFaces(subDictionnaryFaces, edge);

            PolyCubeGraph.ChangeFacePosition(completedFaceList, newFacesPositions);

            PolyCubeGraph.ComputeOrientation(completedFaceList);

            PolyCubeGraph.RepositionFaceNeighbors(completedFaceList);

            // Unity unfolding
            for (int i = 0; i < completedUnityFaceList.Count; i++)
            {
                Vector3 unityPos = VectorConverter.ToUnityEngineVector(
                    completedFaceList[i].Position
                );

                try
                {
                    completedUnityFaceList[i]
                        .GetComponent<Elements.Face>()
                        .StaticUnfolding(unityPos, completedFaceList[i].Orientation);
                }
                catch (Exception e)
                {
                    Debug.unityLogger.LogError(TAG, "Problem : " + e);
                }
            }

            // Clearing all selected faces and edge
            DestroyDirectionnalArrows();
            this.ClearFacesSelected();
            this.UpdateEdgesDisplay();
            this.ClearEdgeSelected();

            //Debug.Log(PolyCubeGraph.PrintFacesDebugLog());
        }

        /// <summary>
        /// Utility function that extract all the faces of the list of selected GameObject and store them in a list of faces
        /// </summary>
        /// <param name="input">The list of select GameObject</param>
        /// <returns>The list of faces</returns>
        public List<Kernel.Face> ListGameObjectToListFace(List<GameObject> input)
        {
            List<Kernel.Face> result = new List<Kernel.Face>();
            foreach (GameObject go in input)
            {
                result.Add(go.GetComponent<Elements.Face>().AttachedFace);
            }
            return result;
        }

        /// <summary>
        /// Update edges to display existing edges and removing old ones
        /// </summary>
        private void UpdateEdgesDisplay()
        {
            foreach (GameObject edge in this.CreatedEdgesList.Values.ToList<GameObject>())
            {
                Destroy(edge);
            }
            CreatedEdgesList.Clear();
            foreach (System.Numerics.Vector3 edge in PolyCubeGraph.GetEdgesPosition())
            {
                this.CreateEdge(edge);
            }
        }

        /// <summary>
        /// A function that clear the FacesSelected variable.
        /// </summary>
        /// <remarks>We need to set all the selected face to false before clearing the list</remarks>
        public void ClearFacesSelected()
        {
            foreach (GameObject g in FacesSelected)
            {
                g.GetComponent<Elements.Face>().m_isSelected = false;
            }
            FacesSelected.Clear();
        }

        /// <summary>
        /// A function that clear the <c>EdgeSelected</c> variable.
        /// </summary>
        public void ClearEdgeSelected()
        {
            if (this.EdgeSelected)
            {
                foreach (GameObject g in CreatedEdgesList.Values)
                {
                    g.GetComponent<Elements.Edge>().m_isSelected = false;
                }
                this.EdgeSelected.m_isSelected = false;
                this.EdgeSelected = null;
            }
        }

        /// <summary>
        /// Utility function that print the list of selected GameObject
        /// </summary>
        public void ToStringListOfFaces()
        {
            foreach (GameObject f in FacesSelected)
            {
                Debug.unityLogger.Log(TAG, f);
            }
        }

        /// <summary>
        /// Update the dictionnary of faces with the new list of faces
        /// </summary>
        /// <param name="facesToChange">List of faces updated </param>
        /// <param name="oldPositions">Old position of the faces in the dictionnary </param>
        /// <exception cref="ArgumentOutOfRangeException"> This exception is raised when both lists haven't the same size </exception>
        public void ChangeUnityFacePosition(
            List<GameObject> facesToChange,
            List<System.Numerics.Vector3> oldPositions
        )
        {
            if (facesToChange.Count != oldPositions.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                foreach (System.Numerics.Vector3 vector in oldPositions)
                {
                    CreatedFacesList.Remove(vector);
                }
                foreach (GameObject go in facesToChange)
                {
                    System.Numerics.Vector3 tmp =
                        go.GetComponent<Elements.Face>().AttachedFace.Position;
                    CreatedFacesList.Add(tmp, go);
                }
            }
        }

        /// <summary>
        /// Undo an action, return the graphe to its previous state
        /// </summary>
        public void Undo()
        {
            if (PolyCubeGraph.Undo())
            {
                this.DrawPolyCube();
            }
        }

        /// <summary>
        /// Clear all selected objects (Faces and Edges)
        /// </summary>
        public void UnselectAll()
        {
            this.StopCoroutine("WaitForUserChoice");
            this.IsInUnfoldingProcess = false;
            this.ClearEdgeSelected();
            this.ClearFacesSelected();
            this.DestroyDirectionnalArrows();
        }

        /// <summary>
        /// This function allows to generate directionnal arrows useful in the unfolding process.
        /// The arrows are generated according to selected faces orientation and the possibilities for
        /// their unfolding (on one given axis, can we unfold in positive and / or negative ?).
        /// </summary>
        /// <param name="oneFacePosition">Vector3, one of the selected faces's position. Arrow will be generated in front / back to it.</param>
        /// <param name="facesOrientation">Faces's orientation, could be X, Y or Z.</param>
        /// <param name="isPositiveRotationPossible">Boolean, can faces be unfold in positive direction on their orientation ?</param>
        /// <param name="isNegativeRotationPossible">Boolean, can faces be unfold in negative direction on their orientation ?</param>
        public void GenerateDirectionnalArrows(
            Vector3 oneFacePosition,
            Kernel.orientation facesOrientation,
            bool isPositiveRotationPossible,
            bool isNegativeRotationPossible
        )
        {
            /*
                First, we are going to create all possible positions for directionnal arrows,
                so that we just have to use the appropriate ones to generate arrows.
                All possibles arrows rotations are already defined in UnfoldingArrow.cs
            */

            _generatedArrows.Clear(); // Clearing arrow lists before adding new ones

            // Arrows are always generated in front of one of the selected face
            Vector3 RightPosition = new Vector3(
                oneFacePosition.x + _arrowDistanceFromFace,
                oneFacePosition.y,
                oneFacePosition.z
            );
            Vector3 LeftPosition = new Vector3(
                oneFacePosition.x - _arrowDistanceFromFace,
                oneFacePosition.y,
                oneFacePosition.z
            );

            Vector3 UpPosition = new Vector3(
                oneFacePosition.x,
                oneFacePosition.y + _arrowDistanceFromFace,
                oneFacePosition.z
            );
            Vector3 DownPosition = new Vector3(
                oneFacePosition.x,
                oneFacePosition.y - _arrowDistanceFromFace,
                oneFacePosition.z
            );

            Vector3 FrontPosition = new Vector3(
                oneFacePosition.x,
                oneFacePosition.y,
                oneFacePosition.z + _arrowDistanceFromFace
            );
            Vector3 BackPosition = new Vector3(
                oneFacePosition.x,
                oneFacePosition.y,
                oneFacePosition.z - _arrowDistanceFromFace
            );

            // Now we are going to check different possibilities for unfold faces on axis
            switch (isPositiveRotationPossible, isNegativeRotationPossible)
            {
                // First case : neither positive and negative sides are free to let the faces unfold
                case (false, false):

                    UI.Game.PopupsManager.Current.DisplayPopup(
                        "",
                        5f,
                        "No unfolding is possible for selected faces and edge."
                    );

                    break;

                // Second case : only positive side is free for unfolding
                case (true, false):

                    if (facesOrientation is Kernel.orientation.X) // Arrow is generated looking right
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                RightPosition,
                                ArrowRotations.Right,
                                DirectionnalArrowsRoot
                            )
                        );
                    }
                    else if (facesOrientation is Kernel.orientation.Y) // Arrow is generated looking up
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                UpPosition,
                                ArrowRotations.Up,
                                DirectionnalArrowsRoot
                            )
                        );
                    }
                    else // Arrow is generated looking front
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                FrontPosition,
                                ArrowRotations.Front,
                                DirectionnalArrowsRoot
                            )
                        );
                    }

                    break;

                // Third case : only negative side is free for unfolding
                case (false, true):

                    if (facesOrientation is Kernel.orientation.X) // Arrow is generated looking left
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                LeftPosition,
                                ArrowRotations.Left,
                                DirectionnalArrowsRoot
                            )
                        );
                    }
                    else if (facesOrientation is Kernel.orientation.Y) // Arrow is generated looking down
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                DownPosition,
                                ArrowRotations.Down,
                                DirectionnalArrowsRoot
                            )
                        );
                    }
                    else // Arrow is generated looking back
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                BackPosition,
                                ArrowRotations.Back,
                                DirectionnalArrowsRoot
                            )
                        );
                    }

                    break;

                // Fourth case : both negative and positive sides are free for unfolding
                case (true, true):

                    if (facesOrientation is Kernel.orientation.X) // Arrows are generated looking right and left
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                RightPosition,
                                ArrowRotations.Right,
                                DirectionnalArrowsRoot
                            )
                        );
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                LeftPosition,
                                ArrowRotations.Left,
                                DirectionnalArrowsRoot
                            )
                        );
                    }
                    else if (facesOrientation is Kernel.orientation.Y) // Arrows are generated looking up and down
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                UpPosition,
                                ArrowRotations.Up,
                                DirectionnalArrowsRoot
                            )
                        );
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                DownPosition,
                                ArrowRotations.Down,
                                DirectionnalArrowsRoot
                            )
                        );
                    }
                    else // Arrows are generated looking front and back
                    {
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                FrontPosition,
                                ArrowRotations.Front,
                                DirectionnalArrowsRoot
                            )
                        );
                        _generatedArrows.Add(
                            UnityEngine.Object.Instantiate(
                                _arrowPrefab,
                                BackPosition,
                                ArrowRotations.Back,
                                DirectionnalArrowsRoot
                            )
                        );
                    }
                    break;
            }
        }

        /// <summary>
        /// A function that destroys directionnal arrows in the game
        /// </summary>
        public void DestroyDirectionnalArrows()
        {
            // Firstly we destroy every existing directionnalArrow in the game
            foreach (GameObject directionnalArrow in _generatedArrows)
                Destroy(directionnalArrow);

            // Finally we clear the list of arrow in the game
            _generatedArrows.Clear();
        }
    }
}
