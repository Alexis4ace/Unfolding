using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Unfolding.Kernel
{
        /// <summary>
    /// Graph structure to represent polycube structure, contain methods to create and manipulate 
    /// this type of structure.
    /// </summary>
    public class Graphe
    {
        //-----------------------------------------------------------------------------
        //------------------------------------ATTRIBUTS--------------------------------
        //-----------------------------------------------------------------------------


        private Dictionary<Vector3, Face> _faces;
        private int _numberCubes;
        private int _numberFaces;
        private Vector3 _minPos;
        private Vector3 _maxPos;

        private Stack<Dictionary<Vector3, Face>> _history;

        //-----------------------------------------------------------------------------
        //--------------------------------GET/SET/CONSTRUCTOR--------------------------
        //-----------------------------------------------------------------------------


        /// <summary>
        /// Construct a polucube sructure of n cubes
        /// </summary>
        /// <param name="n">The number of cubes in structure</param>
        public Graphe(int n)
        {
            this._faces = new Dictionary<Vector3, Face>();
            this._numberCubes = n;
            this._numberFaces = 0;
            this._minPos = new Vector3(1, 1, 1);
            this._maxPos = new Vector3(1, 1, 1);

            this._history = new Stack<Dictionary<Vector3, Face>>();

            GenerateGraph(n);

        }

        /// <summary>
        /// Construct a polucube sructure from a Faces Dictionnary and a number of cube
        /// </summary>
        /// <param name="faces">The faces in the polycube structure</param>
        /// <param name="numberCubes">The number of cubes in the structure</param>
        public Graphe(Dictionary<Vector3, Face> faces, int numberCubes)
        {
            Faces = faces;
            NumberCubes = numberCubes;

            this._history = new Stack<Dictionary<Vector3, Face>>();
        }

        /// <summary>
        /// Default constructor, generate an empty structure
        /// </summary>
        public Graphe()
        {
            _faces = new Dictionary<Vector3, Face>();
            NumberCubes = 0;
            this._numberFaces = 0;
            this._minPos = new Vector3(1, 1, 1);
            this._maxPos = new Vector3(1, 1, 1);

            this._history = new Stack<Dictionary<Vector3, Face>>();
        }

        /// <summary>
        /// The faces of the graph (value), indexed by them positions (key)
        /// </summary>
        public Dictionary<Vector3, Face> Faces
        {
            get { return _faces; }
            set { _faces = value; }
        }

        /// <summary>
        /// The number of cubes wich composed the structure at its creation.
        /// </summary>
        public int NumberCubes
        {
            get { return _numberCubes; }
            set { _numberCubes = value; }
        }

        /// <summary>
        /// The number of faces in the structure.
        /// </summary>
        public int NumberFaces
        {
            get { return _numberFaces; }
            set { _numberFaces = value; }
        }

        /// <summary>
        /// The min positions (X, Y, and Z) of the faces in the structure after the generation.
        /// </summary>
        public Vector3 Min
        {
            get { return _minPos; }
        }

        /// <summary>
        ///  The max positions (X, Y, and Z) of the faces in the structure after the generation.
        /// </summary>
        public Vector3 Max
        {
            get { return _maxPos; }
        }



        //-----------------------------------------------------------------------------
        //----------------------------GENERATE-STRUCTURE-------------------------------
        //-----------------------------------------------------------------------------



        /// <summary>
        /// Generate a list of random cube center positions, first position is (1, 1, 1).
        /// Cube center are always in full odd position, like (1, 1, 1) or (5,4,3)
        /// </summary>
        /// <param name="nbCubes"> The number of cubes needed </param>
        /// <returns> The list of n cubes </returns>
        /// <exception cref="ArgumentOutOfRangeException"> If the number of cubes is negative </exception>
        public List<Vector3> GenerateCubes(int nbCubes)
        {
            List<Vector3> list = new List<Vector3>();

            if (nbCubes > 0)
            {
                Vector3 init = new Vector3(1, 1, 1);
                list.Add(init);
                for (int i = 0; i < nbCubes - 1; i++)
                {

                    //random choice of a face
                    Random rand = new Random();
                    int idx = rand.Next(list.Count - 1);
                    //list[rd] is the cube where we will create the new cube

                    while (!AddCubes(list[idx], list))
                    {
                        idx = rand.Next(list.Count - 1);
                    }

                }
                return list;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }



        /// <summary>
        /// AddCubes: this function check if we can add a neighbor on a cube,
        /// if it's possible, we add a cube on a random free position to the cubes centers list.
        /// </summary>
        /// <param name="tmp"> The positions of the cube we wanted to check </param>
        /// <param name="listCubes"> The cube center list </param>
        /// <returns> If the add is succesfull, return true else false </returns>
        /// <exception cref="ArgumentOutOfRangeException"> If the cube is under the plan </exception>
        /// <example>
        /// <code>
        /// //Create a list and add some cube positions
        /// List cubesList = new List();
        /// 
        /// cubesList.Add(new Vector3(1,1,1);
        /// cubesList.Add(new Vector3(-1,1,1);
        /// cubesList.Add(new Vector3(3,1,1);
        /// cubesList.Add(new Vector3(1,3,1);
        /// cubesList.Add(new Vector3(1,1,-1);
        /// cubesList.Add(new Vector3(1,3,3);
        /// 
        /// AddCube(new Vector3(1,1,1), cubesList); //False 
        /// AddCube(new Vector3(1,0,1), cubesList); //Exception
        /// AddCube(new Vector3(5,1,1), cubesList); //True
        /// 
        /// </code>
        /// </example>
        public bool AddCubes(Vector3 tmp, List<Vector3> listCubes)
        {
            if (tmp.Y >= 1)
            {
                bool res = false;
                List<Vector3> goodFaces = new List<Vector3>();

                Vector3 result = new Vector3(tmp.X, tmp.Y, tmp.Z);

                result.Y = result.Y + 2;
                if (!listCubes.Contains(result))
                {
                    goodFaces.Add(new Vector3(result.X, result.Y, result.Z));
                }

                result = tmp;
                result.X = result.X + 2;
                if (!listCubes.Contains(result))
                {
                    goodFaces.Add(new Vector3(result.X, result.Y, result.Z));
                }

                result = tmp;
                result.Y = result.Y - 2;
                if (!listCubes.Contains(result) && result.Y > 0)
                {
                    goodFaces.Add(new Vector3(result.X, result.Y, result.Z));
                }

                result = tmp;
                result.X = result.X - 2;
                if (!listCubes.Contains(result))
                {
                    goodFaces.Add(new Vector3(result.X, result.Y, result.Z));
                }

                result = tmp;
                result.Z = result.Z + 2;
                if (!listCubes.Contains(result))
                {
                    goodFaces.Add(new Vector3(result.X, result.Y, result.Z));
                }

                result = tmp;
                result.Z = result.Z - 2;
                if (!listCubes.Contains(result))
                {
                    goodFaces.Add(new Vector3(result.X, result.Y, result.Z));
                }

                if (goodFaces.Count > 0)
                {
                    Random rand = new Random();
                    int idx = rand.Next(goodFaces.Count);

                    listCubes.Add(goodFaces[idx]);
                    res = true;

                    //test to see if the new cube is an extremum of the structure
                    //for X axe
                    if (goodFaces[idx].X < _minPos.X)
                    {
                        _minPos.X = goodFaces[idx].X;
                    }
                    else
                    {
                        if (goodFaces[idx].X > _maxPos.X)
                        {
                            _maxPos.X = goodFaces[idx].X;
                        }
                    }
                    //for Y axe
                    if (goodFaces[idx].Y < _minPos.Y)
                    {
                        _minPos.Y = goodFaces[idx].Y;
                    }
                    else
                    {
                        if (goodFaces[idx].Y > _maxPos.Y)
                        {
                            _maxPos.Y = goodFaces[idx].Y;
                        }
                    }
                    //for Z axe
                    if (goodFaces[idx].Z < _minPos.Z)
                    {
                        _minPos.Z = goodFaces[idx].Z;
                    }
                    else
                    {
                        if (goodFaces[idx].Z > _maxPos.Z)
                        {
                            _maxPos.Z = goodFaces[idx].Z;
                        }
                    }
                }

                return res;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }



        /// <summary>
        /// Create from a cube center list, a 3D matrix of boolean.
        /// </summary>
        /// <param name="positionList"> The cube center list </param>
        /// <param name="maxPos"> The max coordinates for x, y, z axes </param>
        /// <param name="minPos"> The min coordinates for x, y, z axes </param>
        /// <returns>3D boolean matrix</returns>
        public bool[,,] GenerateMatrix3D(List<Vector3> positionList, Vector3 minPos, Vector3 maxPos)//positionList : liste des position des cubes
        {
            Vector3 posDiff = maxPos - minPos;
            Vector3 tailleCube = posDiff / 2 + new Vector3(1, 1, 1);
            Vector3 tailleMatrix = tailleCube * 2 + new Vector3(1, 1, 1);

            int tX = (int)tailleMatrix.X;
            int tY = (int)tailleMatrix.Y;
            int tZ = (int)tailleMatrix.Z;
            bool[,,] matrix = new bool[tX, tY, tZ];


            int offsetx = minPos.X < 0 ? (int)Math.Abs(minPos.X) + 1 : 0;
            int offsetZ = minPos.Z < 0 ? (int)Math.Abs(minPos.Z) + 1 : 0;

            foreach (Vector3 pos in positionList)
            {
                if (pos.X % 2 == 0 || pos.Y % 2 == 0 || pos.Z % 2 == 0)
                {
                    throw new ArgumentException("Error : the given list is not a list of cubes");
                }
            }

            for (int i = 0; i < tX; i++)
                for (int j = 0; j < tY; j++)
                    for (int k = 0; k < tZ; k++)
                    {
                        matrix[i, j, k] = false;
                    }



            foreach (Vector3 pos in positionList)
            {

                int ip = (int)pos.X + offsetx + 1;
                int jp = (int)pos.Y + 1;
                int kp = (int)pos.Z + offsetZ + 1;

                int im = (int)pos.X + offsetx - 1;
                int jm = (int)pos.Y - 1;
                int km = (int)pos.Z + offsetZ - 1;

                int i = (int)pos.X + offsetx;
                int j = (int)pos.Y;
                int k = (int)pos.Z + offsetZ;


                matrix[ip, j, k] = !matrix[ip, j, k];//face X+

                if (matrix[ip, j, k])
                    _numberFaces++;
                else
                    _numberFaces--;

                if (im >= 0)
                {
                    matrix[im, j, k] = !matrix[im, j, k];//face X-
                    if (matrix[im, j, k])
                        _numberFaces++;
                    else
                        _numberFaces--;
                }

                matrix[i, jp, k] = !matrix[i, jp, k];//face Y+

                if (matrix[i, jp, k])
                    _numberFaces++;
                else
                    _numberFaces--;


                if (jm >= 0)
                {
                    matrix[i, jm, k] = !matrix[i, jm, k];//face Y- 
                    if (matrix[i, jm, k])
                        _numberFaces++;
                    else
                        _numberFaces--;
                }

                matrix[i, j, kp] = !matrix[i, j, kp];//face Z+

                if (matrix[i, j, kp])
                    _numberFaces++;
                else
                    _numberFaces--;

                if (km >= 0)
                {
                    matrix[i, j, km] = !matrix[i, j, km];//face Z-
                    if (matrix[i, j, km])
                        _numberFaces++;
                    else
                        _numberFaces--;
                }
            }

            return matrix;
        }



        /// <summary>
        /// Create a polycube structure (a graph of face), composed by n cube.
        /// </summary>
        /// <param name="nbCubes"> The number of cubes </param>
        public void GenerateGraph(int nbCubes)
        {
            this._numberCubes = nbCubes;
            List<Vector3> cubes = this.GenerateCubes(nbCubes);
            bool[,,] matrix3D = GenerateMatrix3D(cubes, Min, Max);
            PrintMatrix(matrix3D);
            string filePath = "./file.txt";
            File.WriteAllText(filePath, "");
            //PrintMatrixDebugLog(matrix3D);

            //compute matrix size
            Vector3 posDiff = _maxPos - _minPos;
            Vector3 tailleCube = posDiff / 2 + new Vector3(1, 1, 1);
            Vector3 tailleMatrix = tailleCube * 2 + new Vector3(1, 1, 1);

            int tX = (int)tailleMatrix.X;
            int tY = (int)tailleMatrix.Y;
            int tZ = (int)tailleMatrix.Z;

            //compute offset
            int offsetX = _minPos.X < 0 ? (int)Math.Abs(_minPos.X) + 1 : 0;
            Console.WriteLine("OffsetX:" + offsetX);
            int offsetZ = _minPos.Z < 0 ? (int)Math.Abs(_minPos.Z) + 1 : 0;
            Console.WriteLine("OffsetZ:" + offsetZ);

            for (int i = 0; i < tX; ++i)
            {
                for (int j = 0; j < tY; ++j)
                {
                    for (int k = 0; k < tZ; ++k)
                    {
                        //for each face found
                        if (matrix3D[i, j, k])
                        {
                            Vector3 facePosition = new Vector3(i - offsetX, j, k - offsetZ);

                            //create a new face
                            Face face = new Face(facePosition);
                            this._faces.Add(facePosition, face);
                            //Now need to check the orientation
                            face.Orientation = GetOrientationFaceFromPos(facePosition);

                            //Origine face ok! Let's check the neighbors

                            Vector3 neighbors1 = new Vector3();
                            Vector3 neighbors2 = new Vector3();
                            Vector3 neighbors3 = new Vector3();
                            Vector3 neighbors4 = new Vector3();

                            switch (face.Orientation)
                            {
                                case orientation.X:

                                    //face Y+
                                    if (i + 1 < tX && j + 1 < tY && matrix3D[i + 1, j + 1, k])
                                    {
                                        neighbors1 = new Vector3(i + 1 - offsetX, j + 1, k - offsetZ);
                                    }
                                    else if (j + 2 < tY && matrix3D[i, j + 2, k])
                                    {
                                        neighbors1 = new Vector3(i - offsetX, j + 2, k - offsetZ);
                                    }
                                    else if (i - 1 > 0 && j + 1 < tY && matrix3D[i - 1, j + 1, k])
                                    {
                                        neighbors1 = new Vector3(i - offsetX - 1, j + 1, k - offsetZ);
                                    }
                                    else { Console.WriteLine("Error: missing neighbors" + facePosition); }

                                    //face Y-
                                    if (j - 1 >= 0 && i + 1 < tX && matrix3D[i + 1, j - 1, k])
                                    {
                                        neighbors2 = new Vector3(i - offsetX + 1, j - 1, k - offsetZ);
                                    }
                                    else if (i - 1 > 0 && j - 1 >= 0 && matrix3D[i - 1, j - 1, k])
                                    {
                                        neighbors2 = new Vector3(i - offsetX - 1, j - 1, k - offsetZ);
                                    }
                                    else if (j - 2 >= 0 && matrix3D[i, j - 2, k])
                                    {
                                        neighbors2 = new Vector3(i - offsetX, j - 2, k - offsetZ);
                                    }
                                    else { Console.WriteLine("Error: missing neighbors" + facePosition); }

                                    //face Z-
                                    if (i + 1 < tX && k - 1 >= 0 && matrix3D[i + 1, j, k - 1])
                                    {
                                        neighbors3 = new Vector3(i - offsetX + 1, j, k - offsetZ - 1);
                                    }
                                    else if (i - 1 > 0 && k - 1 >= 0 && matrix3D[i - 1, j, k - 1])
                                    {
                                        neighbors3 = new Vector3(i - offsetX - 1, j, k - offsetZ - 1);
                                    }
                                    else if (k - 2 > 0 && matrix3D[i, j, k - 2])
                                    {
                                        neighbors3 = new Vector3(i - offsetX, j, k - offsetZ - 2);
                                    }
                                    else { Console.WriteLine("Error: missing neighbors" + facePosition); }

                                    //face Z+
                                    if (i + 1 < tX && k + 1 < tZ && matrix3D[i + 1, j, k + 1])
                                    {
                                        neighbors4 = new Vector3(i - offsetX + 1, j, k - offsetZ + 1);
                                    }
                                    else if (i - 1 > 0 && k + 1 < tZ && matrix3D[i - 1, j, k + 1])
                                    {
                                        neighbors4 = new Vector3(i - offsetX - 1, j, k - offsetZ + 1);
                                    }
                                    else if (k + 2 < tZ && matrix3D[i, j, k + 2])
                                    {
                                        neighbors4 = new Vector3(i - offsetX, j, k + 2 - offsetZ);
                                    }
                                    else { Console.WriteLine("Error: missing neighbors" + facePosition); }

                                    face.Yp = neighbors1;
                                    face.Yn = neighbors2;
                                    face.Zn = neighbors3;
                                    face.Zp = neighbors4;

                                    break;

                                case orientation.Y:

                                    //Face X-
                                    if ((i - 2) > 0 && matrix3D[i - 2, j, k])
                                    {
                                        neighbors1 = new Vector3(i - 2 - offsetX, j, k - offsetZ);
                                    }
                                    else if (j + 1 < tY && i - 1 >= 0 && matrix3D[i - 1, j + 1, k])
                                    {
                                        neighbors1 = new Vector3(i - 1 - offsetX, j + 1, k - offsetZ);
                                    }
                                    else if (j - 1 > 0 && i - 1 >= 0 && matrix3D[i - 1, j - 1, k])
                                    {
                                        neighbors1 = new Vector3(i - 1 - offsetX, j - 1, k - offsetZ);
                                    }
                                    else { Console.WriteLine("Error: missing neighbors" + facePosition); }

                                    //face X+
                                    if (i + 2 < tX && matrix3D[i + 2, j, k])
                                    {
                                        neighbors2 = new Vector3(i + 2 - offsetX, j, k - offsetZ);
                                    }
                                    else if (j + 1 < tY && i + 1 < tX && matrix3D[i + 1, j + 1, k])
                                    {
                                        neighbors2 = new Vector3(i + 1 - offsetX, j + 1, k - offsetZ);
                                    }
                                    else if (j - 1 > 0 && i + 1 < tX && matrix3D[i + 1, j - 1, k])
                                    {
                                        neighbors2 = new Vector3(i + 1 - offsetX, j - 1, k - offsetZ);
                                    }
                                    else { Console.WriteLine("Error: missing neighbors" + facePosition); }

                                    //face Z-
                                    if (k - 2 >= 0 && matrix3D[i, j, k - 2])
                                    {
                                        neighbors3 = new Vector3(i - offsetX, j, k - 2 - offsetZ);
                                    }
                                    else if (j + 1 < tY && k - 1 >= 0 && matrix3D[i, j + 1, k - 1])
                                    {
                                        neighbors3 = new Vector3(i - offsetX, j + 1, k - 1 - offsetZ);
                                    }
                                    else if (j - 1 > 0 && k - 1 >= 0 && matrix3D[i, j - 1, k - 1])
                                    {
                                        neighbors3 = new Vector3(i - offsetX, j - 1, k - 1 - offsetZ);
                                    }
                                    else { Console.WriteLine("Missing neighbor" + facePosition); }

                                    //face Z+
                                    if (k + 2 < tZ && matrix3D[i, j, k + 2])
                                    {
                                        neighbors4 = new Vector3(i - offsetX, j, k + 2 - offsetZ);
                                    }
                                    else if (j + 1 < tY && k + 1 >= 0 && matrix3D[i, j + 1, k + 1])
                                    {
                                        neighbors4 = new Vector3(i - offsetX, j + 1, k + 1 - offsetZ);
                                    }
                                    else if (j - 1 > 0 && k + 1 >= 0 && matrix3D[i, j - 1, k + 1])
                                    {
                                        neighbors4 = new Vector3(i - offsetX, j - 1, k + 1 - offsetZ);
                                    }
                                    else { Console.WriteLine("Missing neighbor" + facePosition); }


                                    face.Xn = neighbors1;
                                    face.Xp = neighbors2;
                                    face.Zn = neighbors3;
                                    face.Zp = neighbors4;

                                    break;

                                case orientation.Z:

                                    //Face X-
                                    if (i - 2 > 0 && matrix3D[i - 2, j, k])
                                    {
                                        neighbors1 = new Vector3(i - 2 - offsetX, j, k - offsetZ);
                                    }
                                    else if (i - 1 >= 0 && k - 1 > 0 && matrix3D[i - 1, j, k - 1])
                                    {
                                        neighbors1 = new Vector3(i - 1 - offsetX, j, k - 1 - offsetZ);
                                    }
                                    else if (i - 1 >= 0 && k + 1 < tZ && matrix3D[i - 1, j, k + 1])
                                    {
                                        neighbors1 = new Vector3(i - 1 - offsetX, j, k + 1 - offsetZ);
                                    }
                                    else { Console.WriteLine("Missing neighbor" + facePosition); }

                                    //face X+
                                    if (i + 2 < tX && matrix3D[i + 2, j, k])
                                    {
                                        neighbors2 = new Vector3(i + 2 - offsetX, j, k - offsetZ);
                                    }
                                    else if (i + 1 < tX && k - 1 > 0 && matrix3D[i + 1, j, k - 1])
                                    {
                                        neighbors2 = new Vector3(i + 1 - offsetX, j, k - 1 - offsetZ);
                                    }
                                    else if (i + 1 < tX && k + 1 < tZ && matrix3D[i + 1, j, k + 1])
                                    {
                                        neighbors2 = new Vector3(i + 1 - offsetX, j, k + 1 - offsetZ);
                                    }
                                    else { Console.WriteLine("Missing neighbor" + facePosition); }

                                    //face Y--
                                    if (j - 2 > 0 && matrix3D[i, j - 2, k])
                                    {
                                        neighbors3 = new Vector3(i - offsetX, j - 2, k - offsetZ);
                                    }
                                    else if (j - 1 >= 0 && k - 1 > 0 && matrix3D[i, j - 1, k - 1])
                                    {
                                        neighbors3 = new Vector3(i - offsetX, j - 1, k - offsetZ - 1);
                                    }
                                    else if (j - 1 >= 0 && k + 1 < tZ && matrix3D[i, j - 1, k + 1])
                                    {
                                        neighbors3 = new Vector3(i - offsetX, j - 1, k - offsetZ + 1);
                                    }
                                    else { Console.WriteLine("Missing neighbor" + facePosition); }

                                    //face Y+
                                    if (j + 2 < tY && matrix3D[i, j + 2, k])
                                    {
                                        neighbors4 = new Vector3(i - offsetX, j + 2, k - offsetZ);
                                    }
                                    else if (j + 1 < tY && k - 1 > 0 && matrix3D[i, j + 1, k - 1])
                                    {
                                        neighbors4 = new Vector3(i - offsetX, j + 1, k - offsetZ - 1);
                                    }
                                    else if (j + 1 < tY && k + 1 < tZ && matrix3D[i, j + 1, k + 1])
                                    {
                                        neighbors4 = new Vector3(i - offsetX, j + 1, k - offsetZ + 1); ;
                                    }
                                    else { Console.WriteLine("Missing neighbor" + facePosition); }

                                    face.Xn = neighbors1;
                                    face.Xp = neighbors2;
                                    face.Yn = neighbors3;
                                    face.Yp = neighbors4;

                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Try to link 2 faces that are connected
        /// </summary>
        /// <param name="face1">The first face to link</param>
        /// <param name="face2">Second face to link</param>
        public bool Link2Faces(Face face1, Face face2)
        {
            bool isLinked = face1.TryLinkToFaceOrigin(face2);
            if (isLinked)
            {
                face2.TryLinkToFaceOrigin(face1);
            }
            return isLinked;
        }



        /// <summary>
        /// Linking faces list.
        /// </summary>
        /// <param name = "faceList" > the list of faces</param>
        /// <example>
        /// <code>
        /// List faceslist = new List() { new Face(new Vector3(1, 0, 1), orientati.Y), new Face(new Vector3(0, 1, 1), orientati.X), new Face(new Vector3(1, 1, 0), orientati.Z), new Face(new Vector3(1, 2, 1), orientati.Y), new Face(new Vector3(2, 1, 1), orientati.X), new Face(new Vector3(1, 1, 2), orientati.Z) };
        ///
        /// Graphe graphe = new Graphe();
        /// graphe.LinkFacesInList(faceslist);
        /// //Now the faces are linked, and reprensent a cube
        /// </code>
        /// </example>
        public void LinkFacesInList(List<Face> faceList)
        {
            //List<Face> faceList = faces.Select(kv => kv.Value).ToList();
            for (int i = 0; i < faceList.Count; ++i)
            {
                for (int j = i + 1; j < faceList.Count; ++j)
                {
                    // Console.WriteLine(""+ faceList[i]+ "  " + faceList[j]);
                    bool b = Link2Faces(faceList[i], faceList[j]);
                    // Console.WriteLine("this faces (" + faceList[i] + ")  (" + faceList[j]+") "+ (b ? "are linked" : "not linked"));
                }
            }
        }

        //-----------------------------------------------------------------------------
        //----------------------------------UnfoldING----------------------------------
        //-----------------------------------------------------------------------------



        /// <summary>
        /// Used to check if a group a face can be unfolded, we check with it if the
        /// futur positions of each faces is free. In fact it's check if no faces of a
        /// list is in the graph.
        /// </summary>
        /// <param name="facesPos"> The list of faces to check </param>
        /// <returns> True if no faces are in the graph, else false. </returns>
        /// <example>
        /// <code>
        /// //Create an empty graph
        /// Graphe graph = new Graphe();
        /// 
        /// //Create some face and add it to the graph
        /// Face face1 = new Face(new Vector3(1, 1, 0));
        /// Face face2 = new Face(new Vector3(1, 0, 1));
        /// 
        /// graph.Add(face1.Position, face1);
        /// graph.Add(face2.Position, face2);
        /// 
        /// //True case
        /// 
        /// List list1 = new List();
        /// list1.Add(new Vector3(0, 1, 1));
        /// list1.Add(new Vector3(0, 1, 3));
        /// 
        /// graph.PositionsAreFree(list1);
        /// 
        /// //False case
        /// 
        /// List list2 = new List();
        /// list2.Add(new Vector3(1, 1, 0));
        /// list2.Add(new Vector3(0, 1, 3));
        /// 
        /// graph.PositionsAreFree(list2);
        /// </code>
        /// </example>
        public bool PositionsAreFree(List<Vector3> facesPos)
        {
            if (facesPos.Count == 0)
            {
                throw new Exception("Invalid parameters");
            }
            bool isGood = true;

            foreach (Vector3 face in facesPos)
            {
                if (!Face.IsAFace(face))
                {
                    throw new Exception("Error : a position does not correspond to a face");
                }
                else if (this._faces.ContainsKey(face))
                {
                    isGood = false;
                    break;
                }
            }

            return isGood;
        }



        /// <summary>
        /// Return the possible direction for unfolding, for the both side of unfloding, check if the futur positions of the faces to unfold
        /// are free.
        /// </summary>
        /// <param name="positiveRotatePos"> The faces positions after a positive angle rotation</param>
        /// <param name="negativeRotatePos"> The faces positions after a negative angle rotation</param>
        /// <param name="axe"> The edge axe of rotation </param>
        /// <param name="startFacePos"> The face next to the axe of rotation and selected</param>
        /// <param name="destinationFacePos"> The face next to the axe of rotation, not selected</param>
        /// <returns>A tupple of bool, the first is true if the positive side for unfolding is free, the second is true if the negative side is free</returns>
        public Tuple<Tuple<bool, bool>, Tuple<List<Vector3>, List<Vector3>>> GetUnfoldingDirections(List<Vector3> positiveRotatePos, List<Vector3> negativeRotatePos, Vector3 axe, Vector3 startFacePos, Vector3 destinationFacePos)
        {
            bool positiveAngleRotation = PositionsAreFree(positiveRotatePos);
            bool negativeAngleRotation = PositionsAreFree(negativeRotatePos);

            bool positiveTranslation = false;
            bool negativeTranslation = false;

            List<Vector3> positiveAxePositions = new List<Vector3>();
            List<Vector3> negativeAxePositions = new List<Vector3>();

            Face face = new Face();
            orientation axe_ori = GetOrientationEdgeFromPos(axe);

            try
            {
                face = Faces[startFacePos];
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            if (axe_ori == orientation.X)
            {
                if (face.Orientation == orientation.Y)
                {
                    if (face.Zn == destinationFacePos)
                    {
                        if (positiveAngleRotation) { negativeTranslation = true; }
                        if (negativeAngleRotation) { positiveTranslation = true; }
                        positiveAxePositions = negativeRotatePos;
                        negativeAxePositions = positiveRotatePos;
                    }
                    else if (face.Zp == destinationFacePos)
                    {
                        if (positiveAngleRotation) { positiveTranslation = true; }
                        if (negativeAngleRotation) { negativeTranslation = true; }
                        positiveAxePositions = positiveRotatePos;
                        negativeAxePositions = negativeRotatePos;
                    }
                }
                else if (face.Orientation == orientation.Z)
                {
                    if (face.Yn == destinationFacePos)
                    {
                        if (positiveAngleRotation) { positiveTranslation = true; }
                        if (negativeAngleRotation) { negativeTranslation = true; }
                        positiveAxePositions= positiveRotatePos;
                        negativeAxePositions = negativeRotatePos;
                    }
                    else if (face.Yp == destinationFacePos)
                    {
                        if (positiveAngleRotation) { negativeTranslation = true; }
                        if (negativeAngleRotation) { positiveTranslation = true; }
                        positiveAxePositions = negativeRotatePos;
                        negativeAxePositions = positiveRotatePos;
                    }
                }
            }

            else if (axe_ori == orientation.Y)
            {
                if (face.Orientation == orientation.X)
                {
                    if (face.Zn == destinationFacePos)
                    {
                        if (positiveAngleRotation) { positiveTranslation = true; }
                        if (negativeAngleRotation) { negativeTranslation = true; }
                        positiveAxePositions = positiveRotatePos;
                        negativeAxePositions = negativeRotatePos;
                    }
                    else if (face.Zp == destinationFacePos)
                    {
                        if (positiveAngleRotation) { negativeTranslation = true; }
                        if (negativeAngleRotation) { positiveTranslation = true; }
                        positiveAxePositions = negativeRotatePos;
                        negativeAxePositions = positiveRotatePos;
                    }
                }
                else if (face.Orientation == orientation.Z)
                {
                    if (face.Xn == destinationFacePos)
                    {
                        if (positiveAngleRotation) { negativeTranslation = true; }
                        if (negativeAngleRotation) { positiveTranslation = true; }
                        positiveAxePositions = negativeRotatePos;
                        negativeAxePositions = positiveRotatePos;
                    }
                    else if (face.Xp == destinationFacePos)
                    {
                        if (positiveAngleRotation) { positiveTranslation = true; }
                        if (negativeAngleRotation) { negativeTranslation = true; }
                        positiveAxePositions = positiveRotatePos;
                        negativeAxePositions= negativeRotatePos;
                    }
                }
            }

            else
            {
                if (face.Orientation == orientation.X)
                {
                    if (face.Yn == destinationFacePos)
                    {
                        if (positiveAngleRotation) { negativeTranslation = true; }
                        if (negativeAngleRotation) { positiveTranslation = true; }
                        positiveAxePositions = negativeRotatePos;
                        negativeAxePositions = positiveRotatePos;
                    }
                    else if (face.Yp == destinationFacePos)
                    {
                        if (positiveAngleRotation) { positiveTranslation = true; }
                        if (negativeAngleRotation) { negativeTranslation = true; }
                        positiveAxePositions = positiveRotatePos;
                        negativeAxePositions = negativeRotatePos;
                    }
                }
                else if (face.Orientation == orientation.Y)
                {
                    if (face.Xn == destinationFacePos)
                    {
                        if (positiveAngleRotation) { positiveTranslation = true; }
                        if (negativeAngleRotation) { negativeTranslation = true; }
                        positiveAxePositions = positiveRotatePos;
                        negativeAxePositions = negativeRotatePos;
                    }
                    else if (face.Xp == destinationFacePos)
                    {
                        if (positiveAngleRotation) { negativeTranslation = true; }
                        if (negativeAngleRotation) { positiveTranslation = true; }
                        positiveAxePositions = negativeRotatePos;
                        negativeAxePositions = positiveRotatePos;
                    }
                }
            }



            return Tuple.Create(Tuple.Create(positiveTranslation, negativeTranslation), Tuple.Create(positiveAxePositions, negativeAxePositions));
        }



        /// <summary>
        /// Check if a structure will stay on the ground after the 
        /// unfloding process.
        /// </summary>
        /// <param name="oldPosition"> The old positions of the unflod faces </param>
        /// <param name="newPositions"> The new positions of the unflod faces </param>
        /// <returns>True if the structure has at least one face on the floor after unfolding</returns>
        /// <example>
        /// <code>
        /// //Create an empty graph
        /// //Create an empty graph
        /// Graphe graph = new Graphe();
        /// 
        /// //Create some face and add it to the graph
        /// Face face1 = new Face(new Vector3(1, 1, 0));
        /// Face face2 = new Face(new Vector3(1, 0, 1));
        /// 
        /// graph.Add(face1.Position, face1);
        /// graph.Add(face2.Position, face2);
        /// 
        /// //True case
        /// 
        /// List oldPosition = new List();
        /// oldPosition.Add(new Vector3(1, 1, 0));
        /// 
        /// List newPosition = new List(); //Simulate an unfloding of the (1, 1, 0) face
        /// newPosition.Add(new Vector3(1, 0, -1));
        /// 
        /// graph.IsStructureStillOnTheFloor(oldPosition, newPosition);
        /// 
        /// //False case
        /// 
        /// List oldPosition = new List();
        /// oldPosition.Add(new Vector3(1, 0, 1));
        /// 
        /// List newPosition = new List(); //Simulate an unfloding of the (1, 0, 1) face
        /// newPosition.Add(new Vector3(1, -1, 0));
        /// 
        /// graph.IsStructureStillOnTheFloor(oldPosition, newPosition);
        /// 
        /// </code>
        /// </example>
        public bool IsStructureStillOnTheFloor(List<Vector3> oldPosition, List<Vector3> newPositions)
        {
            bool isOnTheFloor = false;

            if (oldPosition.Count != newPositions.Count)
            {
                throw (new Exception("The list have not equal lenght"));
            }
            else
            {
                //Check if a face not unfold connect the structure to the floor
                foreach (Vector3 facePos in Faces.Keys)
                {
                    if (!oldPosition.Contains(facePos) && facePos.Y == 0) { isOnTheFloor = true; break; }
                }

                //Check if a face unfold with a new position connect the structure to the floor
                if (!isOnTheFloor)
                {
                    foreach (Vector3 facePos in newPositions)
                    {
                        if (facePos.Y == 0) { isOnTheFloor = true; break; }
                    }
                }

                return isOnTheFloor;
            }

        }



        /// <summary>
        /// Check if an edge is connected to ONLY ONE face in a list.
        /// Usefull for the unfolding process, indeed an must be connected to
        /// only one face in the list of face to unfold.
        /// </summary>
        /// <param name="faces"> The group of face that should be connected to the edge </param>
        /// <param name="edgePosition"> The edge to check</param>
        /// <returns> True if the edge is connected to the face </returns>
        public bool EdgeIsConnectToOneFace(List<Face> faces, Vector3 edgePosition)
        {
            if (!Face.IsEdge(edgePosition))
            {
                throw new Exception("Edge position invalide");
            }
            else
            {
                int count = 0;

                foreach (Face face in faces)
                {
                    if (Face.IsAFace(face.Position))
                    {
                        if (face.ContainEdge(edgePosition))
                        {
                            count++;
                        }
                    }
                    else
                    {
                        throw new Exception("Face position invalid");
                    }
                }
                return count == 1;
            }
        }



        /// <summary>
        /// Check if the structure is a patern
        /// (win condition)
        /// </summary>
        /// <returns> True if the structure is a pattern </returns>
        public bool IsStructureAPattern()
        {
            bool result = true;

            List<Face> positions = Faces.Values.ToList();

            orientation currentOrientation = positions[0].Orientation;

            for (int i = 1; i < positions.Count; ++i)
            {
                if (currentOrientation != positions[i].Orientation) { result = false; break; }
            }

            return result;
        }



        /// <summary>
        /// Search the face connected to the unfloding axe (an edge position), in a group
        /// of face to unflod.
        /// </summary>
        /// <param name="faces"> A list of faces</param>
        /// <param name="positionEdge"> The position of the edge axe</param>
        /// <returns>The good face or NULL_FACE if it's not found</returns>
        /// <exception cref="Exception">If the edge position is wrong</exception>
        public Vector3 SearchStartFace(List<Face> faces, Vector3 positionEdge)
        {
            if (!Face.IsEdge(positionEdge))
            {
                throw new Exception("Edge position invalide");
            }
            else
            {
                Vector3 facePos = Face.NULL_FACE;

                Vector3 positionFace1 = new Vector3(positionEdge.X, positionEdge.Y, positionEdge.Z);
                Vector3 positionFace2 = new Vector3(positionEdge.X, positionEdge.Y, positionEdge.Z);
                Vector3 positionFace3 = new Vector3(positionEdge.X, positionEdge.Y, positionEdge.Z);
                Vector3 positionFace4 = new Vector3(positionEdge.X, positionEdge.Y, positionEdge.Z);

                Face face;

                orientation o = GetOrientationEdgeFromPos(positionEdge);

                switch (o)
                {
                    case orientation.X:

                        positionFace1.Y++;
                        positionFace2.Y--;
                        positionFace3.Z++;
                        positionFace4.Z--;

                        if (this._faces.ContainsKey(positionFace1))
                        {
                            face = this._faces[positionFace1];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace1;
                            }
                        }

                        if (this._faces.ContainsKey(positionFace2))
                        {
                            face = this._faces[positionFace2];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace2;
                            }
                        }
                        if (this._faces.ContainsKey(positionFace3))
                        {
                            face = this._faces[positionFace3];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace3;
                            }
                        }
                        if (this._faces.ContainsKey(positionFace4))
                        {
                            face = this._faces[positionFace4];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace4;
                            }
                        }

                        break;

                    case orientation.Y:
                        positionFace1.X++;
                        positionFace2.X--;
                        positionFace3.Z++;
                        positionFace4.Z--;

                        if (this._faces.ContainsKey(positionFace1))
                        {
                            face = this._faces[positionFace1];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace1;
                                break;
                            }
                        }

                        if (this._faces.ContainsKey(positionFace2))
                        {
                            face = this._faces[positionFace2];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace2;
                                break;
                            }
                        }

                        if (this._faces.ContainsKey(positionFace3))
                        {
                            face = this._faces[positionFace3];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace3;
                                break;
                            }
                        }

                        if (this._faces.ContainsKey(positionFace4))
                        {
                            face = this._faces[positionFace4];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace4;
                                break;
                            }
                        }

                        break;
                    case orientation.Z:
                        positionFace1.X++;
                        positionFace2.X--;
                        positionFace3.Y++;
                        positionFace4.Y--;

                        if (this._faces.ContainsKey(positionFace1))
                        {
                            face = this._faces[positionFace1];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace1;
                                break;
                            }
                        }

                        if (this._faces.ContainsKey(positionFace2))
                        {
                            face = this._faces[positionFace2];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace2;
                                break;
                            }
                        }

                        if (this._faces.ContainsKey(positionFace3))
                        {
                            face = this._faces[positionFace3];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace3;
                                break;
                            }
                        }

                        if (this._faces.ContainsKey(positionFace4))
                        {
                            face = this._faces[positionFace4];
                            if (faces.Contains(face))
                            {
                                facePos = positionFace4;
                                break;
                            }
                        }

                        break;

                    default:
                        break;
                }

                return facePos;
            }

        }



        /// <summary>
        /// Depth path in graph but only in a faces selection
        /// </summary>
        /// <param name="group"> Face list </param>
        /// <param name="startFace"> The start of the depth Path </param>
        public void DepthPathWithList(List<Face> group, Face startFace)
        {
            Face face;

            startFace.Color = color.ORANGE;

            for (int i = 0; i < 6; i++)
            {
                if (startFace.Neighbors[i] != Face.NULL_FACE)
                {
                    face = _faces[startFace.Neighbors[i]];
                    if (group.Contains(face))
                    {
                        if (face.Color == color.GREEN)
                        {
                            DepthPathWithList(group, face);
                        }
                    }
                }
            }
            startFace.Color = color.RED;
        }



        /// <summary>
        /// AUTO-SELECTION : 
        /// Research from a source face the list of faces connected to it
        /// without pass by selected faces in the graph.
        /// </summary>
        /// <param name="connectedFace"> The list of connected faces</param>
        /// <param name="selectedFaces"> List of selected faces (from the unfolding process) </param>
        /// <param name="startFace"> The start face for the path in the graph </param>
        public void SearchConnectedFaces(List<Face> connectedFace, List<Face> selectedFaces, Face startFace)
        {
            startFace.Color = color.ORANGE;

            if (selectedFaces.Count == 0)
            {
                throw new Exception("Empty selected list");
            }
            else if (!Face.IsAFace(startFace.Position))
            {
                throw new Exception("Invalide face position");
            }

            else if (!selectedFaces.Contains(startFace))
            {
                for (int i = 0; i < 6; ++i)
                {
                    if (startFace.Neighbors[i] != Face.NULL_FACE)
                    {
                        Face currentNeighbor = Faces[startFace.Neighbors[i]];

                        if (currentNeighbor.Color == color.GREEN)
                        {
                            SearchConnectedFaces(connectedFace, selectedFaces, currentNeighbor);
                        }
                    }
                }

                connectedFace.Add(startFace);
            }
        }



        /// <summary>
        /// AUTO-SELECTION : METHOD 2
        /// Get the complete list of the faces that need to be unfold to keep the graph connected 
        /// </summary>
        /// <param name="selectedFaces"> The list of selected faces in the unfloding process </param>
        /// <param name="destination"> The face which should not connected to the faces to unfold exept through the selected faces </param>
        /// <returns></returns>
        public List<Face> GetFacesToUnfold(List<Face> selectedFaces, Face destination)
        {
            List<Face> facesConnected = new List<Face>();
            List<Face> facesUnfold = new List<Face>();

            ResetFacesColor();
            SearchConnectedFaces(facesConnected, selectedFaces, destination);

            foreach (Face face in Faces.Values)
            {
                if (!facesConnected.Contains(face))
                {
                    facesUnfold.Add(face);
                }
            }

            return facesUnfold;
        }



        /// <summary>
        /// Check if all the faces in a list are linked to each other
        /// using a depth path
        /// </summary>
        /// <param name="group"> The faces to check </param>
        /// <param name="startFace"> The start Face for the depth path </param>
        /// <returns>True if the faces are linked</returns>
        public bool AreFacesLinked(List<Face> group, Face startFace)
        {
            if (!Face.IsAFace(startFace.Position))
            {
                throw new Exception("Invalid edge position");
            }
            else
            {
                bool areLinked = true;

                if (group.Count == 0)
                {
                    areLinked = false;
                }
                else
                {
                    foreach (Face face in group)
                    {
                        if (Face.IsAFace(face.Position))
                        {
                            face.Color = color.GREEN;
                        }
                        else
                        {
                            throw new Exception("Invalid Face in List");
                        }
                    }

                    DepthPathWithList(group, startFace);

                    foreach (Face face in group)
                    {
                        areLinked = face.Color == color.RED;
                        if (!areLinked) break;
                    }
                }

                return areLinked;
            }
        }


        /// <summary>
        /// Detache a group of faces of the graph, they are still linked together.
        /// </summary>
        /// <param name="dicoFaces">The group of face to detach</param>
        /// <param name="edge">The edge wich connect this group of face to the rest of the graph</param>
        public void DetachFaces(Dictionary<Vector3, Face> dicoFaces, Vector3 edge)
        {
            if (!Face.IsEdge(edge))
            {
                throw new ArgumentException("Invalid edge: this position is not corresponding to edge");
            }

            List<Face> listfaces = dicoFaces.Select(kv => kv.Value).ToList();

            if (!EdgeIsConnectToOneFace(listfaces, edge))
            {
                throw new ArgumentException("Invalid edge: the edge does not belong to the face list");
            }


            foreach (KeyValuePair<Vector3, Face> couple in dicoFaces)
            {
                Console.WriteLine("===============================================");
                Console.WriteLine("Face  " + couple.Value);
                for (int i = 0; i < couple.Value.Neighbors.Length; ++i)
                {
                    Console.WriteLine("i = " + i);
                    Vector3 neighbor = couple.Value.Neighbors[i];
                    Console.WriteLine("neibor = " + neighbor);
                    Vector3 faceNextToEdge = couple.Value.SearchFaceFromEdge(edge);
                    Console.WriteLine("faceNextToEdge = " + faceNextToEdge);
                    Vector3 facePosColinear = couple.Value.IsExistEdgeColinear(edge);
                    Console.WriteLine("facePosColinear = " + facePosColinear);

                    if (neighbor != Face.NULL_FACE && !dicoFaces.ContainsKey(neighbor) && neighbor != faceNextToEdge && neighbor != facePosColinear)
                    {
                        //Console.WriteLine("Proces au delinkement des deux faces = " + facePosColinear);
                        Face face = this._faces[neighbor];
                        Console.WriteLine("{ Proces au delinkement  ");
                        Console.WriteLine("Premiere face =  " + neighbor);
                        for (int j = 0; j < face.Neighbors.Length; j++)
                        {
                            if (face.Neighbors[j] == couple.Key)
                            {
                                Console.WriteLine("Deuxieme face =  " + face.Neighbors[j]);
                                face.Neighbors[j] = Face.NULL_FACE;
                            }
                        }

                        couple.Value.Neighbors[i] = Face.NULL_FACE;
                        Console.WriteLine("fin delink } ");

                    }
                }
            }
        }



        /// <summary>
        /// Unfold a group of face in the structure 
        /// </summary>
        /// <param name="faces"> Faces to unfold </param>
        /// <param name="positionEdge"> The edge use as rotation axe </param>
        /// <param name="direction"> The rotation direction (true = 90°, false = -90°) </param>
        public void Unfold(List<Face> faces, Vector3 positionEdge, bool direction)
        {
            if (EdgeIsConnectToOneFace(faces, positionEdge))
            {
                if (AreFacesLinked(faces, faces[0]))
                {
                    Vector3 startFacePos = SearchStartFace(faces, positionEdge);
                    Face startFace = this._faces[startFacePos];
                    Vector3 destinationPos = startFace.SearchFaceFromEdge(positionEdge);

                    if (destinationPos != Face.NULL_FACE)
                    {
                        Face destinationFace = this._faces[destinationPos];
                        List<Face> completedFaceList = GetFacesToUnfold(faces, destinationFace);
                        List<Vector3> facePositions = FaceToPosition(completedFaceList);
                        List<Vector3> newFacePositions = MatriceR.RotateGroupOfFace(facePositions, direction, startFacePos, positionEdge);

                        if (PositionsAreFree(newFacePositions) && IsStructureStillOnTheFloor(facePositions, newFacePositions))
                        {
                            this._history.Push(this._faces);

                            Dictionary<Vector3, Face> subDictionarFaces = this._faces.Where(kv => completedFaceList.Contains(kv.Value)).ToDictionary(kv => kv.Key, kv => kv.Value);

                            DetachFaces(subDictionarFaces, positionEdge);
                            ChangeFacePosition(completedFaceList, newFacePositions);
                            ComputeOrientation(completedFaceList);
                            RepositionFaceNeighbors(completedFaceList);

                        }
                        else
                        {
                            throw new Exception("Unfree Positions");
                        }
                    }
                    else
                    {
                        throw new Exception("This edge does not connect the faces to the structure");
                    }

                }
                else
                {
                    throw new Exception("Unlinked faces");
                }
            }
            else
            {
                throw new Exception("Edge connect to the wrong number of faces");
            }

        }




        //-----------------------------------------------------------------------------
        //-----------------------------------TOOLS-------------------------------------
        //-----------------------------------------------------------------------------



        /// <summary>
        /// Compute the orientation of a face from its positions
        /// </summary>
        /// <param name="pos"> The face position </param>
        /// <returns> The orientation of the face </returns>
        /// <exception cref="ArgumentException"> If the position does not correspond to a face </exception>
        public static orientation GetOrientationFaceFromPos(Vector3 pos)
        {
            bool XisPair = pos.X % 2 == 0;
            bool YisPair = pos.Y % 2 == 0;
            bool ZisPair = pos.Z % 2 == 0;

            orientation res;

            if (XisPair && !YisPair && !ZisPair)
            {
                res = orientation.X;
            }
            else
            {
                if (!XisPair && YisPair && !ZisPair)
                {
                    res = orientation.Y;
                }
                else
                {
                    if (!XisPair && !YisPair && ZisPair)
                    {
                        res = orientation.Z;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }

            return res;
        }



        /// <summary>
        /// Compute the orientation of an edge from its positions
        /// </summary>
        /// <param name="pos"> The edge position </param>
        /// <returns> The orientation of the edge</returns>
        /// <exception cref="ArgumentException"> If the position does not correspond to a face </exception>
        public static orientation GetOrientationEdgeFromPos(Vector3 pos)
        {

            bool XisPair = pos.X % 2 == 0;
            bool YisPair = pos.Y % 2 == 0;
            bool ZisPair = pos.Z % 2 == 0;

            orientation res;

            if (!XisPair && YisPair && ZisPair)
            {
                res = orientation.X;
            }
            else
            {
                if (XisPair && !YisPair && ZisPair)
                {
                    res = orientation.Y;
                }
                else
                {
                    if (XisPair && YisPair && !ZisPair)
                    {
                        res = orientation.Z;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
            return res;
        }



        /// <summary>
        /// Compute and set orientation for a group of face
        /// </summary>
        /// <param name="faces"> The group of face </param>
        public void ComputeOrientation(List<Face> faces)
        {
            foreach (Face face in faces)
            {
                face.Orientation = GetOrientationFaceFromPos(face.Position);
            }
        }



        /// <summary>
        /// Update the neighbors of a group a face.
        /// </summary>
        /// <param name="faces"> The list to update </param>
        public void RepositionFaceNeighbors(List<Face> faces)
        {
            foreach (Face face in faces)
            {
                face.RepositionNeighbors();
            }
        }



        /// <summary>
        /// Convert a face list to a vector3 list of face positions
        /// </summary>
        /// <param name="faces"> The list to convert</param>
        /// <returns> A list of face positions </returns>
        public List<Vector3> FaceToPosition(List<Face> faces)
        {
            List<Vector3> positions = new List<Vector3>();

            foreach (Face face in faces)
            {
                positions.Add(face.Position);
            }

            return positions;
        }



        /// <summary>
        /// Change the positions of a group of faces and update 
        /// the face dictionnary of the graph.
        /// </summary>
        /// <param name="facesToChange"> The faces to move </param>
        /// <param name="newPositions"> The new positions for faces</param>
        /// <exception cref="ArgumentOutOfRangeException"> If their is not the same number of faces and positions </exception>
        public void ChangeFacePosition(List<Face> facesToChange, List<Vector3> newPositions)
        {

            if (facesToChange.Count != newPositions.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                for (int i = 0; i < facesToChange.Count; ++i)
                {
                    Face changedFace = _faces[facesToChange[i].Position];

                    for (int j = 0; j < 6; ++j)
                    {
                        if (changedFace.Neighbors[j] != Face.NULL_FACE)
                        {
                            this._faces[changedFace.Neighbors[j]].ReplaceNeighbor(changedFace.Position, newPositions[i]);
                        }
                    }
                    _faces.Remove(facesToChange[i].Position);

                    changedFace.Position = newPositions[i];
                    _faces.Add(changedFace.Position, changedFace);
                }
            }
        }

        /// <summary>
        /// A function that computes the position with minimum x, y and z and the the maximum x, y and z and respectively stores it in minPos and maxPas
        /// </summary>
        public void ComputeMinMaxPos()
        {
            _minPos = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
            _maxPos = new Vector3(int.MinValue, int.MinValue, int.MinValue);

            foreach (KeyValuePair<Vector3, Face> face in _faces)
            {
                _minPos = new Vector3(
                    Math.Min(face.Value.X, _minPos.X),
                    Math.Min(face.Value.Y, _minPos.Y),
                    Math.Min(face.Value.Z, _minPos.Z)
                );
                _maxPos = new Vector3(
                    Math.Max(face.Value.X, _maxPos.X),
                    Math.Max(face.Value.Y, _maxPos.Y),
                    Math.Max(face.Value.Z, _maxPos.Z)
                );
            }
        }

        /// <summary>
        /// Set the color of all the faces to GREEN,
        /// usefull before a path in the graph
        /// </summary>
        public void ResetFacesColor()
        {
            foreach (Face face in this._faces.Values)
            {
                face.Color = color.GREEN;
            }
        }



        /// <summary>
        /// Lauch the path, and return the edges list
        /// of the structure
        /// </summary>
        /// <returns> The list of edges </returns>
        public List<Vector3> GetEdgesPosition()
        {
            List<Vector3> edges = new List<Vector3>();
            ResetFacesColor();

            Face face = Faces.Values.ToList()[0];

            Console.WriteLine("Start : " + face.Position);

            GetEdgesByPath(edges, face);

            return edges;
        }



        /// <summary>
        /// Path in the graph to get edges of a structure
        /// </summary>
        /// <param name="edges"> The list to stock the edges </param>
        /// <param name="startFace"> The start face for the path </param>
        private void GetEdgesByPath(List<Vector3> edges, Face startFace)
        {
            startFace.Color = color.RED;

            List<Vector3> edgeList = new List<Vector3>(6);

            Vector3 e1 = new Vector3(startFace.Position.X - 1, startFace.Position.Y, startFace.Position.Z);
            Vector3 e2 = new Vector3(startFace.Position.X + 1, startFace.Position.Y, startFace.Position.Z);
            Vector3 e3 = new Vector3(startFace.Position.X, startFace.Position.Y - 1, startFace.Position.Z);
            Vector3 e4 = new Vector3(startFace.Position.X, startFace.Position.Y + 1, startFace.Position.Z);
            Vector3 e5 = new Vector3(startFace.Position.X, startFace.Position.Y, startFace.Position.Z - 1);
            Vector3 e6 = new Vector3(startFace.Position.X, startFace.Position.Y, startFace.Position.Z + 1);

            edgeList.Add(e1);
            edgeList.Add(e2);
            edgeList.Add(e3);
            edgeList.Add(e4);
            edgeList.Add(e5);
            edgeList.Add(e6);

            for (int i = 0; i < 6; ++i)
            {
                if (startFace.Neighbors[i] != Face.NULL_FACE && Faces[startFace.Neighbors[i]].Color != color.RED)
                {
                    if (Faces.ContainsKey(startFace.Neighbors[i]))
                    {
                        if (Faces[startFace.Neighbors[i]].Color != color.RED)
                        {
                            edges.Add(edgeList[i]);
                        }
                    }
                    else
                    {
                        throw new Exception("GetEdgeByPath: add edge, face not in the dictionnary");
                    }
                }
            }

            for (int i = 0; i < 6; ++i)
            {
                if (startFace.Neighbors[i] != Face.NULL_FACE && Faces[startFace.Neighbors[i]].Color != color.RED)
                {
                    if (Faces.ContainsKey(startFace.Neighbors[i]))
                    {
                        if (Faces[startFace.Neighbors[i]].Color != color.RED)
                        {
                            GetEdgesByPath(edges, Faces[startFace.Neighbors[i]]);
                        }
                    }
                    else
                    {
                        throw new Exception("GetEdgeByPath: add edge, face not in the dictionnary");
                    }
                }
            }
        }

        /// <summary>
        /// Utilitary function
        /// </summary>
        /// <returns></returns>
        public List<Kernel.Face> GetFacesList()
        {
            List<Face> faces = new List<Face>();
            foreach (Face f in Faces.Values)
            {
                faces.Add(f);
            }
            return faces;
        }

        /// <summary>
        /// Return the structure to its previous state
        /// </summary>
        public bool Undo()
        {
            if (this._history.Count > 0)
            {
                this._faces = this._history.Pop();
                return true;
            }
            return false;
        }

        public Dictionary<Vector3, Face> CloneFaces()
        {
            Dictionary<Vector3, Face> clone = new Dictionary<Vector3, Face>();
            foreach (KeyValuePair<Vector3, Face> face in Faces)
            {
                clone.Add(new Vector3(face.Key.X, face.Key.Y, face.Key.Z), new Face(face.Value));
            }
            return clone;
        }

        /// <summary>
        /// Save the currents faces in the history stack
        /// </summary>
        public void PushHistory()
        {
            this.PrintFacesDebugLog();
            this._history.Push(this.CloneFaces());
        }

        //-----------------------------------------------------------------------------
        //-----------------------------------PRINT-------------------------------------
        //-----------------------------------------------------------------------------



        /// <summary>
        /// Print a list of cube center(vector3)
        /// </summary>
        /// <param name="cubeList">List of vector3</param>
        public void PrintPositionList(List<Vector3> cubeList)
        {
            Console.WriteLine("______________________________________________________");
            Console.WriteLine("|  indice  |      X      |      Y      |      Z      |");
            Console.WriteLine("|__________|_____________|_____________|_____________|");

            for (int i = 0; i < cubeList.Count; ++i)
            {
                Console.WriteLine("|     " + i + "    |      " + cubeList[i].X + "      |      " + cubeList[i].Y + "      |      " + cubeList[i].Z + "      |");
                Console.WriteLine("|__________|_____________|_____________|_____________|");
            }
        }



        /// <summary>
        /// Print the min and max vector of the graph
        /// </summary>
        public void PrintMinMax()
        {
            Console.WriteLine("___________________________________________");
            Console.WriteLine("|      X      |      Y      |      Z      |");
            Console.WriteLine("___________________________________________");
            Console.WriteLine("|      " + _minPos.X + "      |      " + _minPos.Y + "      |      " + _minPos.Z + "      |");
            Console.WriteLine("___________________________________________");
            Console.WriteLine("|      " + _maxPos.X + "      |      " + _maxPos.Y + "      |      " + _maxPos.Z + "      |");
            Console.WriteLine("___________________________________________");
        }



        /// <summary>
        /// Print a 3D matrix of boolean, in multiple 2D array
        /// </summary>
        /// <param name="matrix">The matrix to diplayed</param>
        public void PrintMatrix(bool[,,] matrix)
        {
            int tx = matrix.GetLength(0);
            int ty = matrix.GetLength(1);
            int tz = matrix.GetLength(2);
            int count = 0;

            for (int j = 0; j < ty; j++)
            {
                Console.WriteLine("This is the display of matrix for j=" + j);
                for (int k = 0; k < tz; k++)
                {
                    Console.Write("________");
                }

                Console.Write("\n");

                for (int i = 0; i < tx; i++)
                {
                    for (int k = 0; k < tz; k++)
                    {
                        string chaine = matrix[i, j, k] ? "| FACE |" : "| NULL |";
                        if (matrix[i, j, k])
                        {
                            count++;
                        }
                        Console.Write(chaine);
                    }

                    Console.Write("\n");
                    for (int k = 0; k < tz; k++)
                    {
                        Console.Write("________");
                    }
                    Console.Write("\n");
                }
            }

            Console.WriteLine("Number of faces is " + count);
        }

        /// <summary>
        /// Print a 3D matrix of boolean, in multiple 2D array
        /// </summary>
        /// <param name="matrix">The matrix to diplayed</param>
        public void PrintMatrixDebugLog(bool[,,] matrix)
        {
            int tx = matrix.GetLength(0);
            int ty = matrix.GetLength(1);
            int tz = matrix.GetLength(2);
            int count = 0;
            string res = "";

            for (int j = 0; j < ty; j++)
            {

                res += ("This is the display of matrix for j=" + j + "\n");
                for (int k = 0; k < tz; k++)
                {
                    res +=("________");
                }

                res +=("\n");

                for (int i = 0; i < tx; i++)
                {
                    for (int k = 0; k < tz; k++)
                    {
                        string chaine = matrix[i, j, k] ? "| FACE |" : "| NULL |";
                        if (matrix[i, j, k])
                        {
                            count++;
                        }
                        res +=(chaine);
                    }

                    res +=("\n");
                    for (int k = 0; k < tz; k++)
                    {
                        res +=("________");
                    }
                    res +=("\n");
                }
            }

            res += ("Number of faces is " + count);

            string filePath = "./file.txt";

            try
                {
                    // Write the content of 'res' to the file
                    res += "\n";
                    File.AppendAllText(filePath, res);
                    res = ("Debug log written to file: " + filePath);
                }
            catch (Exception e)
                {
                    res = ("Error writing to file: " + e.Message);
                }
            
        }


        /// <summary>
        /// Print the faces dictionnary of the graph
        /// </summary>
        public void PrintFaces()
        {
            Console.WriteLine("__________________________________________________________________________________________________________");
            Console.WriteLine("|   position   |      Xn      |      Xp      |      Yn      |      Yp      |      Zn      |      Zp      |");
            Console.WriteLine("|______________|______________|______________|______________|______________|______________|______________|");
            foreach (Face face in this._faces.Values)
            {
                face.Print();
            }
        }

        /// <summary>
        /// Print the faces dictionnary of the graph in Debug Log
        /// </summary>
        public string PrintFacesDebugLog()
        {
            string res = "\n";
            res += "__________________________________________________________________________________________________________\n";
            res += "|   position   |      Xn      |      Xp      |      Yn      |      Yp      |      Zn      |      Zp      |\n";
            res += "|______________|______________|______________|______________|______________|______________|______________|\n";
            foreach (Face face in this._faces.Values)
            {
                res += face.PrintDebugLog();
            }
            
            string filePath = "./file.txt";
            //fuck
            try
                {
                    // Write the content of 'res' to the file
                    res += "\n";
                    File.AppendAllText(filePath, res);
                    res = ("Debug log written to file: " + filePath);
                }
            catch (Exception e)
                {
                    res = ("Error writing to file: " + e.Message);
                }

            return res;
            }
        
    }
}