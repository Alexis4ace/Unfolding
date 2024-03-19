using System;
using System.Numerics;
using System.Collections.Generic;

namespace Unfolding.Kernel
{
        /// <summary>
    /// Represent rotation _matrix for faces.
    /// </summary>
    public class MatriceR
    {

        //-----------------------------------------------------------------------------
        //------------------------------------ATTRIBUTS--------------------------------
        //-----------------------------------------------------------------------------

        //this param indicate if the angle rotation is 90° or -90°
        private bool _direction;

        private Vector3 _axe;

        private int[,] _matrix;

        //Dictionary associate each couple of (direction, axe) to a rotation matrix
        private static Dictionary<(bool, Vector3), MatriceR> VectorDirecMatriceR = new Dictionary<(bool, Vector3), MatriceR>()
        {
            { (true  , new Vector3(1, 0, 0)), new MatriceR(true , new Vector3(1, 0, 0)) },
            { (false , new Vector3(1, 0, 0)), new MatriceR(false, new Vector3(1, 0, 0)) },
            { (true  , new Vector3(0, 1, 0)), new MatriceR(true , new Vector3(0, 1, 0)) },
            { (false , new Vector3(0, 1, 0)), new MatriceR(false, new Vector3(0, 1, 0)) },
            { (true  , new Vector3(0, 0, 1)), new MatriceR(true , new Vector3(0, 0, 1)) },
            { (false , new Vector3(0, 0, 1)), new MatriceR(false, new Vector3(0, 0, 1)) }

        };



        //Dictionary associate each couple of (difference, orientation) to a position of face at origin
        private static Dictionary<(Vector3, orientation), Vector3> difference_faceOrigin = new Dictionary<(Vector3, orientation), Vector3>()
        {
            //Orientation X
            { (new Vector3(0, 0, 1), orientation.X), new Vector3(0,1,1) }, { (new Vector3(0, 1, 0), orientation.X), new Vector3(0,1,1) },
            { (new Vector3(0, 0, -1), orientation.X), new Vector3(0,1,-1) }, { (new Vector3(0, 1, -2), orientation.X), new Vector3(0,1, -1) },
            { (new Vector3(0, -2, 1), orientation.X), new Vector3(0,-1, 1) }, { (new Vector3(0, -1, 0), orientation.X), new Vector3(0,-1, 1) },

            //Orientation Y
            { (new Vector3(0, 0, 1), orientation.Y), new Vector3(1,0,1) }, { (new Vector3(1, 0, 0), orientation.Y), new Vector3(1,0,1) },
            { (new Vector3(-2, 0, 1), orientation.Y), new Vector3(-1,0,1) }, { (new Vector3(-1, 0, 0), orientation.Y), new Vector3(-1,0,1) },
            { (new Vector3(0, 0, -1), orientation.Y), new Vector3(1,0,-1) }, { (new Vector3(1, 0, -2), orientation.Y), new Vector3(1,0,-1) },

            //Orientation Z
            { (new Vector3(0, 1, 0), orientation.Z), new Vector3(1,1,0) }, { (new Vector3(1, 0, 0), orientation.Z), new Vector3(1,1,0) },
            { (new Vector3(-2, 1, 0), orientation.Z), new Vector3(-1,1,0) }, { (new Vector3(-1, 0, 0), orientation.Z), new Vector3(-1,1,0)},
            { (new Vector3(0, -1, 0), orientation.Z), new Vector3(1,-1,0) }, { (new Vector3(1, -2, 0) , orientation.Z), new Vector3(1,-1,0) }

        };

        //Dictionary associate each orientation to a unitary vector : (i , j or k) 
        private static Dictionary<orientation, Vector3> orientation_vector = new Dictionary<orientation, Vector3>()
        {
            { orientation.X, new Vector3(1, 0, 0)},
            { orientation.Y, new Vector3(0, 1, 0)},
            { orientation.Z, new Vector3(0, 0, 1) }
        };


        //-----------------------------------------------------------------------------
        //--------------------------------GET/SET/CONSTRUCTOR--------------------------
        //-----------------------------------------------------------------------------


        /// <summary>
        /// construct of class MatriceR
        /// </summary>
        /// <param name="directionR">indicate if it's : 90 deg or not</param>
        /// <param name="_axeR">Rotation unit vector</param>
        public MatriceR(bool directionR, Vector3 _axeR)
        {

            this._direction = directionR;
            this._axe = _axeR;

            _matrix = new int[3, 3];

            float s = this._direction ? 1 : -1;

            _matrix[0, 0] = (int)Math.Pow(_axe.X, 2);

            _matrix[1, 1] = (int)Math.Pow(_axe.Y, 2);

            _matrix[2, 2] = (int)Math.Pow(_axe.Z, 2);

            _matrix[0, 1] = (int)((_axe.X) * (_axe.Y) - (_axe.Z) * s);

            _matrix[1, 0] = (int)((_axe.X) * (_axe.Y) + (_axe.Z) * s);

            _matrix[0, 2] = (int)((_axe.X) * (_axe.Z) + (_axe.Y) * s);

            _matrix[2, 0] = (int)((_axe.X) * (_axe.Z) - (_axe.Y) * s);

            _matrix[1, 2] = (int)((_axe.Y) * (_axe.Z) - (_axe.X) * s);

            _matrix[2, 1] = (int)((_axe.Y) * (_axe.Z) + (_axe.X) * s);

        }

        /// <summary>
        /// Get on the matrix
        /// </summary>
        /// <returns> Rotation matrix as an 2-dimensional array </returns>
        public int[,] GetMatrix()
        {
            return _matrix;
        }



        /// <summary>
        /// Overloading product MatriceR, Vector3
        /// </summary>
        /// <param name="matrice_rotation"> 1ère operande Matrice de rotation</param>
        /// <param name="position"> 2ème operande vecteur </param>
        /// <returns> Result product </returns>
        public static Vector3 operator *(MatriceR matrice_rotation, Vector3 position)
        {
            Vector3 dot = new();

            dot.X = matrice_rotation.GetMatrix()[0, 0] * position.X + matrice_rotation.GetMatrix()[0, 1] * position.Y + matrice_rotation.GetMatrix()[0, 2] * position.Z;

            dot.Y = matrice_rotation.GetMatrix()[1, 0] * position.X + matrice_rotation.GetMatrix()[1, 1] * position.Y + matrice_rotation.GetMatrix()[1, 2] * position.Z;

            dot.Z = matrice_rotation.GetMatrix()[2, 0] * position.X + matrice_rotation.GetMatrix()[2, 1] * position.Y + matrice_rotation.GetMatrix()[2, 2] * position.Z;

            return dot;
        }



        //-----------------------------------------------------------------------------
        //-------------------------------------METHODS---------------------------------
        //-----------------------------------------------------------------------------



        /// <summary>
        /// Getiing the translated face to origin from the dictionary
        /// </summary>
        /// <param name="position"> Position of the face we want to translate to origin</param>
        /// <param name="cenreArrete"> Edge of rotation </param>
        /// <returns> A translated face </returns>
        public static Vector3 GetTranslatedFaceToOrigin(Vector3 position, Vector3 cenreArrete)
        {
            Vector3 translated = new Vector3();
            orientation orientation = Graphe.GetOrientationFaceFromPos(position);
            translated = difference_faceOrigin[(position - cenreArrete, orientation)];

            return translated;  

        }



        /// <summary>
        /// Dictionary associate each orientation to a unitary vector : (i , j or k) 
        /// </summary>
        /// <param name="orientationEdge"> edge orientation </param>
        /// <returns> The unitary vector </returns>
        public static Vector3 GetUnitaryVectorFromOrientationEdge(orientation orientationEdge)
        {
            return orientation_vector[orientationEdge];
        }



        /// <summary>
        /// _matrix from the disctionary associated to (directio, _axe)
        /// </summary>
        /// <param name="direction"> L'angle de rotion </param>
        /// <param name="_axe"> _axe de rotation </param>
        /// <returns> Matrice de rotation </returns>
        private static MatriceR GetMatriceRFromDirection_axe(bool direction, Vector3 _axe)
        {
            return VectorDirecMatriceR[(direction, _axe)];
        }



        //-----------------------------------------------------------------------------
        //--------------------------------ROTATION--------------------------
        //-----------------------------------------------------------------------------

        /// <summary>
        /// From a given position, direction and a centre of an edge, 
        /// this function calculate the rotation of the face 
        /// </summary>
        /// <param name="position">face position</param>
        /// <param name="direction">90° of -90°</param>
        /// <param name="centreArrete">center of the edge</param>
        /// <returns> The new rotated position  </returns>
        public static Vector3 RotateSingleFace(Vector3 position, bool direction, Vector3 centreArrete)
        {

            orientation orientation_Arrete = Graphe.GetOrientationEdgeFromPos(centreArrete);

            Vector3 v = GetUnitaryVectorFromOrientationEdge(orientation_Arrete);

            MatriceR matrice = GetMatriceRFromDirection_axe(direction, v);

            Vector3 translatedFaceToOrigin = GetTranslatedFaceToOrigin(position, centreArrete);

            Vector3 rotatedFaceInOrigin = matrice * translatedFaceToOrigin;

            Vector3 rotatedFaceResult = rotatedFaceInOrigin + position - translatedFaceToOrigin;

            return rotatedFaceResult;
        }



        /// <summary>
        /// From a given list of positions, direction and a centre of an edge, 
        /// this function calculate the rotation of the group of faces 
        /// </summary>
        /// <param name="listPosition"> a list of face positions </param>
        /// <param name="direction"> 90° or -90° </param>
        /// <param name="positionClickedFace"> position of the clicked face</param>
        /// <param name="centreArrete">center of the edge</param>
        /// <returns> The new rotated list position </returns>
        public static List<Vector3> RotateGroupOfFace(List<Vector3> listPosition, bool direction, Vector3 positionClickedFace, Vector3 centreArrete)
        {

            List<Vector3> result = new List<Vector3>();

            orientation orientation = Graphe.GetOrientationFaceFromPos(positionClickedFace);

            orientation orientation_Arrete = Graphe.GetOrientationEdgeFromPos(centreArrete);

            Vector3 v = GetUnitaryVectorFromOrientationEdge(orientation_Arrete);

            MatriceR matrice = GetMatriceRFromDirection_axe(direction, v);

            Vector3 translationToOrigin = GetTranslatedFaceToOrigin(positionClickedFace, centreArrete) - positionClickedFace;

            foreach (Vector3 position in listPosition)
            {
                Vector3 translatedFaceToOrigin = position + translationToOrigin;
                Vector3 rotationOfTranslatedFace = matrice * translatedFaceToOrigin;
                Vector3 resultRotationPosition = rotationOfTranslatedFace - translationToOrigin;

                result.Add(resultRotationPosition);
            }

            return result;
        }



        /// <summary>
        /// Check if 2 points are colenear in a direction : X, Y or Z
        /// </summary>
        /// <param name="point">First point</param>
        /// <param name="edge">Second point</param>
        /// <returns> boolean </returns>
        public static bool IsPointInDroiteEdge(Vector3 point, Vector3 edge)
        {
            Vector3 unitaryVector = orientation_vector[Graphe.GetOrientationEdgeFromPos(edge)];
            Vector3 diff = edge - point;
            Vector3 crossProduct = Vector3.Cross(unitaryVector, diff);

            return crossProduct == Vector3.Zero;
        }
    }
}