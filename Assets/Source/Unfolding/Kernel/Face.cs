using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Unfolding.Kernel
{
    
    /// <summary>
    /// Orientation of a Face or an edge. The value can X, Y or Z.
    /// For a Face the orientation is the axe of its normal.
    /// For an edge it's the axe of the edge.
    /// </summary>
    public enum orientation { 
        /// <summary>For a face: normal on X axe, for an edge: edge on X axe</summary>
        X,
        /// <summary>For a face: normal on Y axe, for an edge: edge on Y axe</summary>
        Y,
        /// <summary>For a face: normal on Z axe, for an edge: edge on Z axe</summary>
        Z
    }

    /// <summary>
    /// The color is used to tag a face during a path operation for example.
    /// </summary>
    public enum color {
        /// <summary>Used when a treatment on a face is ended</summary>
        RED,
        /// <summary>Used when a treatment on a face is in progress</summary>
        ORANGE,
        /// <summary>Used when a treatment on a face is not started</summary>
        GREEN
    }


    /// <summary>
    /// A Face represent one face of a structure. It's represent by a position, a color,
    /// some neighbors and an orientation. 
    /// </summary>
    public class Face
    {
        //-----------------------------------------------------------------------------
        //------------------------------------ATTRIBUTS--------------------------------
        //-----------------------------------------------------------------------------


        /// <summary>
        /// The definition of the null element of the Face class.
        /// </summary>
        public static Vector3 NULL_FACE = new Vector3(-1, -1, -1);
        
        private color _color;
        private Vector3[] _neighbors;
        private Vector3 _position;
        private orientation orientation;

        private int idColor;

        //-----------------------------------------------------------------------------
        //--------------------------------GET/SET/CONSTRUCTOR--------------------------
        //-----------------------------------------------------------------------------


        /// <summary>
        /// The neighboring Face on the negativ side of the X axe (Only for faces with Y or Z orientation, equal to NULL_FACE for X orientation faces)
        /// </summary>
        /// <example>
        /// <code>
        /// // Face with Z orientation example  
        /// //                      ___________ 
        /// //                     |           | 
        /// //                     |           |
        /// // this neighbor ====> |           |
        /// //                     |           |  
        /// //                     |           |  
        /// // Y                    -----------
        /// // ^
        /// // |
        /// // | 
        /// // |
        /// // -------> X
        /// </code>
        ///  </example>
        public Vector3 Xn
        {
            get { return _neighbors[0]; }
            set { _neighbors[0] = value; }
        }

        /// <summary>
        /// The neighboring Face on the positive side of the X axe (Only for faces with Y or Z orientation, equal to NULL_FACE for X orientation faces)
        /// </summary>
        /// <example>
        /// <code>
        /// // Face with Z orientation example  
        /// //         ___________ 
        /// //        |           | 
        /// //        |           |
        /// //        |           | --===== this neighbor
        /// //        |           |  
        /// //        |           |  
        /// // Y       -----------
        /// // ^
        /// // |
        /// // | 
        /// // |
        /// // -------> X
        /// </code>
        ///  </example>
        public Vector3 Xp
        {
            get { return _neighbors[1]; }
            set { _neighbors[1] = value; }
        }

        /// <summary>
        /// The neighboring Face on the negative side of the Y axe (Only for faces with X or Z orientation, equal to NULL_FACE for Y orientation faces)
        /// </summary>
        /// <example>
        /// <code>
        /// // Face with Z orientation example  
        /// //        ___________ 
        /// //       |           | 
        /// //       |           |
        /// //       |           |
        /// //       |           |  
        /// //       |           |  
        /// //        -----------
        /// //            ^
        /// //            |
        /// //            |
        /// //       this neighbor
        /// // Y
        /// // ^
        /// // |
        /// // | 
        /// // |
        /// // -------> X
        /// </code>
        ///  </example>
        public Vector3 Yn
        {
            get { return _neighbors[2]; }
            set { _neighbors[2] = value; }
        }

        /// <summary>
        /// The neighboring Face on the positive side of the Y axe (Only for faces with X or Z orientation, equal to NULL_FACE for Y orientation faces)
        /// </summary>
        /// <example>
        /// <code>
        /// // Face with Z orientation example  
        /// //
        /// //       this neighbor
        /// //             |
        /// //             |
        /// //             v
        /// //        ___________ 
        /// //       |           | 
        /// //       |           |
        /// //       |           |
        /// //       |           |  
        /// //       |           |  
        /// //        -----------
        /// //
        /// // Y
        /// // ^
        /// // |
        /// // | 
        /// // |
        /// // -------> X
        /// </code>
        ///  </example>
        public Vector3 Yp
        {
            get { return _neighbors[3]; }
            set { _neighbors[3] = value; }
        }

        /// <summary>
        /// The neighboring Face on the negative side of the Z axe (Only for faces with X or Y orientation, equal to NULL_FACE for Z orientation faces)
        /// </summary>
        /// <example>
        /// <code>
        /// // Face with X orientation example  
        /// //                      ___________ 
        /// //                     |           | 
        /// //                     |           |
        /// // this neighbor ====> |           |
        /// //                     |           |  
        /// //                     |           |  
        /// // Y                    -----------
        /// // ^
        /// // |
        /// // | 
        /// // |
        /// // -------> Z
        /// </code>
        ///  </example>
        public Vector3 Zn
        {
            get { return this._neighbors[4]; }
            set { this._neighbors[4] = value; }
        }

        /// <summary>
        /// The neighboring Face on the positive side of the Y axe (Only for faces with X or Y orientation, equal to NULL_FACE for Z orientation faces)
        /// </summary>
        /// <example>
        /// <code>
        /// // Face with X orientation example  
        /// //         ___________ 
        /// //        |           | 
        /// //        |           |
        /// //        |           | --===== this neighbor
        /// //        |           |  
        /// //        |           |  
        /// // Y       -----------
        /// // ^
        /// // |
        /// // | 
        /// // |
        /// // -------> Z
        /// </code>
        ///  </example>
        public Vector3 Zp
        {
            get { return this._neighbors[5]; }
            set { this._neighbors[5] = value; }
        }

        /// <summary>
        /// The neighboring face positions tab of the face (Xn, Xp, Yn, Yp, Zn, Zp)
        /// </summary>
        /// <seealso cref="Xn"/>
        /// <seealso cref="Xp"/>
        /// <seealso cref="Yn"/>
        /// <seealso cref="Yp"/>
        /// <seealso cref="Zn"/>
        /// <seealso cref="Zp"/>
        public Vector3[] Neighbors
        {
            get { return this._neighbors; }
        }

        /// <summary>
        /// The geometric position of the face in a 3D space
        /// </summary>
        public Vector3 Position
        {
            get { return this._position; }
            set { this._position = value; }
        }

        /// <summary>
        /// The X position of the face
        /// </summary>
        /// <seealso cref="Position"/>
        public float X
        {
            get { return this._position.X; }
            set { this._position.X = value; }
        }

        /// <summary>
        /// The Y position of the face
        /// </summary>
        /// <seealso cref="Position"/>
        public float Y
        {
            get { return this._position.Y; }
            set { this._position.Y = value; }
        }

        /// <summary>
        /// The Z position of the face
        /// </summary>
        /// <seealso cref="Position"/>
        public float Z
        {
            get { return this._position.Z; }
            set { this._position.Z = value; }
        }

        /// <summary>
        /// The orientation of the face, correspond to the normal axe of the face
        /// </summary>
        /// <seealso cref="orientation"/>
        public orientation Orientation
        {
            get { return this.orientation; }
            set { this.orientation = value; }

        }

        /// <summary>
        /// The tag color of the face
        /// </summary>
        /// <seealso cref="color"/>
        public color Color
        {
            get { return this._color; }
            set { this._color = value; }
        }

        public int IdColor
        {
            get { return this.idColor; }
            set { this.idColor = value; }
        }

        /// <summary>
        /// Create a face without neighbor and green color
        /// </summary>
        public Face()
        {
            this._neighbors = new Vector3[6];
            this._color = color.GREEN;
            this.idColor = new Random().Next(0, 6);
            for (int i = 0; i < 6; i++)
            {
                _neighbors[i] = NULL_FACE;
            }
        }

        /// <summary>
        /// Create a face without neighbors, green color, at the position given in parameter
        /// </summary>
        /// <param name="pos">The position of the face</param>
        public Face(Vector3 pos)
        {
            this._position = pos;
            this._color = color.GREEN;
            this._neighbors = new Vector3[6];
            this.idColor = new Random().Next(0, 6);
            for (int i = 0; i < 6; i++)
            {
                _neighbors[i] = NULL_FACE;
            }
            this.orientation = Graphe.GetOrientationFaceFromPos(pos);
        }

        /// <summary>
        /// Create a face at a given position with a given orientation, without neighbors and with a green color
        /// </summary>
        /// <param name="pos">The position of the new face</param>
        /// <param name="o">The orientation of the new face</param>
        public Face(Vector3 pos, orientation o)
        {
            this._position = pos;
            this._neighbors = new Vector3[6];
            orientation = o;
            this.idColor = new Random().Next(0, 6);
            for (int i = 0; i < 6; i++)
            {
                _neighbors[i] = NULL_FACE;
            }
            this.orientation = o;

        }

        /// <summary>
        /// Create a face with given neighbors, position and orientation 
        /// </summary>
        /// <param name="f1">Neighbor 1</param>
        /// <param name="f2">Neighbor 2</param>
        /// <param name="f3">Neighbor 3</param>
        /// <param name="f4">Neighbor 4</param>
        /// <param name="pos">The position of the new face</param>
        /// <param name="orientation">The orientation of the new face</param>
        public Face(Vector3 f1, Vector3 f2, Vector3 f3, Vector3 f4, Vector3 pos, orientation orientation)
        {

            this._position = pos;

            this._neighbors = new Vector3[6];
            this.orientation = orientation;
            this._color = color.GREEN;
            this.idColor = new Random().Next(0, 6);
            
            for (int i = 0; i < 6; i++)
            {
                _neighbors[i] = NULL_FACE;
            }

            if (orientation == orientation.X)
            {
                this._neighbors[2] = f1;
                this._neighbors[3] = f2;
                this._neighbors[4] = f3;
                this._neighbors[5] = f4;
            }
            else
            {
                if (orientation == orientation.Y)
                {
                    this._neighbors[0] = f1;
                    this._neighbors[1] = f2;
                    this._neighbors[4] = f3;
                    this._neighbors[5] = f4;
                }
                else
                {
                    this._neighbors[0] = f1;
                    this._neighbors[1] = f2;
                    this._neighbors[2] = f3;
                    this._neighbors[3] = f4;
                }
            }

        }

        /// <summary>
        /// Create a copy of face
        /// </summary>
        /// <param name="face">The original face to copy</param>
        public Face(Face face)
        {
            orientation = face.orientation;
            _color = face.Color;
            _position = new Vector3(face.X, face.Y, face.Z);
            idColor = face.idColor;
            _neighbors = new Vector3[6];

            Xn = new Vector3(face.Xn.X, face.Xn.Y, face.Xn.Z);
            Yn = new Vector3(face.Yn.X, face.Yn.Y, face.Yn.Z);
            Zn = new Vector3(face.Zn.X, face.Zn.Y, face.Zn.Z);
            Xp = new Vector3(face.Xp.X, face.Xp.Y, face.Xp.Z);
            Yp = new Vector3(face.Yp.X, face.Yp.Y, face.Yp.Z);
            Zp = new Vector3(face.Zp.X, face.Zp.Y, face.Zp.Z);
        }

        //-----------------------------------------------------------------------------
        //------------------------------------METHODES---------------------------------
        //-----------------------------------------------------------------------------


        /// <summary>
        /// Check if a face contain an edge (a position),
        /// taking care of the orientation of the face. Compute it with the neighbors existence and the face position
        /// </summary>
        /// <param name="edge">The position of the edge searched</param>
        /// <returns> True if the edge is found </returns>
        /// <example>
        /// <code>
        /// //Create a new face on (1, 1, 0) position, orientation Z
        /// Face face = new Face(new Vector3(1, 1, 0), orientation.Z);
        /// 
        /// //add a neighbor position on Yn
        /// face.Yn = new Vector3(1, 0, 1); 
        /// 
        /// containEdge(new Vector3(1, 0, 0)); //return true
        /// containEdge(new Vector3(1, 2, 0)); //return false, true if a neighbor is add on Yp
        /// containEdge(new Vector3(0, 1, 0)); //return false, true if a neighbor is add on Xn
        /// containEdge(new Vector3(2, 1, 0)); //return false, true if a neighbor is add on Xp
        /// </code>
        /// </example>
        public bool ContainEdge(Vector3 edge)
        {
            if (!IsEdge(edge)) { 
                throw new Exception("Invalid edge: this _position is not corresponding to edge"); 
            }
            else
            {
                Vector3 e1 = new Vector3();
                Vector3 e2 = new Vector3();
                Vector3 e3 = new Vector3();
                Vector3 e4 = new Vector3();

                if (orientation == orientation.X)
                {
                    e1 = new Vector3(_position.X, _position.Y - 1, _position.Z);
                    e2 = new Vector3(_position.X, _position.Y + 1, _position.Z);
                    e3 = new Vector3(_position.X, _position.Y, _position.Z - 1);
                    e4 = new Vector3(_position.X, _position.Y, _position.Z + 1);
                }
                else if (orientation == orientation.Y)
                {
                    e1 = new Vector3(_position.X - 1, _position.Y, _position.Z);
                    e2 = new Vector3(_position.X + 1, _position.Y, _position.Z);
                    e3 = new Vector3(_position.X, _position.Y, _position.Z - 1);
                    e4 = new Vector3(_position.X, _position.Y, _position.Z + 1);
                }
                else if (orientation == orientation.Z)
                {
                    e1 = new Vector3(_position.X - 1, _position.Y, _position.Z);
                    e2 = new Vector3(_position.X + 1, _position.Y, _position.Z);
                    e3 = new Vector3(_position.X, _position.Y - 1, _position.Z);
                    e4 = new Vector3(_position.X, _position.Y + 1, _position.Z);
                }

                return (e1 == edge || e2 == edge || e3 == edge || e4 == edge);
            }

        }



        /// <summary>
        /// Search the neighbors position corresponding to an edge position in a face
        /// </summary>
        /// <param name="edge">The edge wich do the link</param>
        /// <returns>The position of the corresponding neighbor (NULL_FACE if not found)</returns>
        /// <example>
        /// <code>
        /// //Create a new face on (1, 1, 0) position, orientation Z
        /// Face face = new Face(new Vector3(1, 1, 0), orientation.Z);
        /// 
        /// //add a neighbor position on Yn
        /// face.Yn = new Vector3(1, 0, 1); 
        /// 
        /// SearchFaceFromEdge(new Vector3(1, 0, 0)); //return (1, 0, 1)
        /// SearchFaceFromEdge(new Vector3(1, 2, 0)); //return NULL_FACE, no neighbor on Yp
        /// SearchFaceFromEdge(new Vector3(0, 1, 0)); //return NULL_FACE, no neighbor on Xn
        /// SearchFaceFromEdge(new Vector3(2, 1, 0)); //return NULL_FACE, no neighbor on Xp
        /// </code>
        /// </example>
        public Vector3 SearchFaceFromEdge(Vector3 edge)
        {
            if (!IsEdge(edge))
            {
                throw new Exception("Invalid edge: this _position is not corresponding to edge");
            }
            else
            {
                Vector3 res = Face.NULL_FACE;

                Vector3 e1 = new Vector3();
                Vector3 e2 = new Vector3();
                Vector3 e3 = new Vector3();
                Vector3 e4 = new Vector3();

                if (orientation == orientation.X)
                {
                    e1 = new Vector3(_position.X, _position.Y - 1, _position.Z);
                    e2 = new Vector3(_position.X, _position.Y + 1, _position.Z);
                    e3 = new Vector3(_position.X, _position.Y, _position.Z - 1);
                    e4 = new Vector3(_position.X, _position.Y, _position.Z + 1);

                    if (e1 == edge) { res = this.Yn; }
                    else if (e2 == edge) { res = this.Yp; }
                    else if (e3 == edge) { res = this.Zn; }
                    else if (e4 == edge) { res = this.Zp; }
                }
                else if (orientation == orientation.Y)
                {
                    e1 = new Vector3(_position.X - 1, _position.Y, _position.Z);
                    e2 = new Vector3(_position.X + 1, _position.Y, _position.Z);
                    e3 = new Vector3(_position.X, _position.Y, _position.Z - 1);
                    e4 = new Vector3(_position.X, _position.Y, _position.Z + 1);

                    if (e1 == edge) { res = this.Xn; }
                    else if (e2 == edge) { res = this.Xp; }
                    else if (e3 == edge) { res = this.Zn; }
                    else if (e4 == edge) { res = this.Zp; }
                }
                else if (orientation == orientation.Z)
                {
                    e1 = new Vector3(_position.X - 1, _position.Y, _position.Z);
                    e2 = new Vector3(_position.X + 1, _position.Y, _position.Z);
                    e3 = new Vector3(_position.X, _position.Y - 1, _position.Z);
                    e4 = new Vector3(_position.X, _position.Y + 1, _position.Z);

                    if (e1 == edge) { res = this.Xn; }
                    else if (e2 == edge) { res = this.Xp; }
                    else if (e3 == edge) { res = this.Yn; }
                    else if (e4 == edge) { res = this.Yp; }
                }

                return res;
            }

        }



        /// <summary>
        /// Check if one of the edges of the face is colinear to a given edge
        /// </summary>
        /// <param name="edge">The edge to check</param>
        /// <returns>Return the neigbhbors position corresponding to the colinear edge (NULL_FACE if not found)</returns>
        /// <example>
        /// <code>
        /// //Create a new face on (1, 1, 0) position, orientation Z
        /// Face face = new Face(new Vector3(1, 1, 0), orientation.Z);
        /// 
        /// //add a neighbor position on Yn and Yp
        /// face.Yn = new Vector3(1, 0, 1); 
        /// face.Yp = new Vector3(1, 2, 1);
        /// 
        /// IsExistEdgeColinear(new Vector3(3, 0, 0)); //return (1, 0, 1) edge between face and Yn is colinear to (3, 0,0)
        /// IsExistEdgeColinear(new Vector3(1, 4, 0)); //return false, not colinear to one of faces edges
        /// IsExistEdgeColinear(new Vector3(3, 2, 0)); //return (1, 2, 1) edge between face and Yp is colinear to (3, 0,0)
        /// IsExistEdgeColinear(new Vector3(2, 3, 0)); //return false, not colinear to one of faces edges
        /// </code>
        /// </example>
        public Vector3 IsExistEdgeColinear(Vector3 edge)
        {
            Vector3 pos1;
            Vector3 pos2;
            Vector3 pos3;
            Vector3 pos4;

            bool b1;
            bool b2;
            bool b3;
            bool b4;
            if (!Face.IsEdge(edge))
            {
                throw new ArgumentException("Error : the parameter is not an edge");
            }
            else
            {
                if (orientation == orientation.X)
                {
                    pos1 = _position - Vector3.UnitY;
                    pos2 = _position + Vector3.UnitY;
                    pos3 = _position - Vector3.UnitZ;
                    pos4 = _position + Vector3.UnitZ;

                    b1 = MatriceR.IsPointInDroiteEdge(edge, pos1);
                    b2 = MatriceR.IsPointInDroiteEdge(edge, pos2);
                    b3 = MatriceR.IsPointInDroiteEdge(edge, pos3);
                    b4 = MatriceR.IsPointInDroiteEdge(edge, pos4);


                    if (b1)
                    {
                        return _neighbors[2];
                    }
                    if (b2)
                    {
                        return _neighbors[3];
                    }
                    if (b3)
                    {
                        return _neighbors[4];
                    }
                    if (b4)
                    {
                        return _neighbors[5];
                    }
                }
                else if (orientation == orientation.Y)
                {
                    pos1 = _position - Vector3.UnitX;
                    pos2 = _position + Vector3.UnitX;
                    pos3 = _position - Vector3.UnitZ;
                    pos4 = _position + Vector3.UnitZ;

                    b1 = MatriceR.IsPointInDroiteEdge(edge, pos1);
                    b2 = MatriceR.IsPointInDroiteEdge(edge, pos2);
                    b3 = MatriceR.IsPointInDroiteEdge(edge, pos3);
                    b4 = MatriceR.IsPointInDroiteEdge(edge, pos4);

                    if (b1)
                    {
                        return _neighbors[0];
                    }
                    if (b2)
                    {
                        return _neighbors[1];
                    }
                    if (b3)
                    {
                        return _neighbors[4];
                    }
                    if (b4)
                    {
                        return _neighbors[5];
                    }
                }
                else
                {
                    pos1 = _position - Vector3.UnitX;
                    pos2 = _position + Vector3.UnitX;
                    pos3 = _position - Vector3.UnitY;
                    pos4 = _position + Vector3.UnitY;

                    b1 = MatriceR.IsPointInDroiteEdge(edge, pos1);
                    b2 = MatriceR.IsPointInDroiteEdge(edge, pos2);
                    b3 = MatriceR.IsPointInDroiteEdge(edge, pos3);
                    b4 = MatriceR.IsPointInDroiteEdge(edge, pos4);

                    if (b1)
                    {
                        return _neighbors[0];
                    }
                    if (b2)
                    {
                        return _neighbors[1];
                    }
                    if (b3)
                    {
                        return _neighbors[2];
                    }
                    if (b4)
                    {
                        return _neighbors[3];
                    }
                }
            }
            

            return Face.NULL_FACE;
        }



        /// <summary>
        /// Search and replace a neighbor position by another position
        /// (if the neighbor is found)
        /// </summary>
        /// <param name="faceToReplace"> The position to replace </param>
        /// <param name="newFace"> The new position </param>
        /// <returns> True if the neighbors was found </returns>
        ///<example>
        /// <code>
        /// //Create a new face on (1, 1, 0) position, orientation Z
        /// Face face = new Face(new Vector3(1, 1, 0), orientation.Z);
        /// 
        /// //add a neighbor position on Yn and Yp
        /// face.Yn = new Vector3(1, 0, 1); 
        /// face.Yp = new Vector3(1, 2, 1);
        /// 
        /// ReplaceNeighbor(new Vector3(1, 0, 1), new Vector3(1, 0, -1)); //return true, Yn is now (1, 0, -1)
        /// ReplaceNeighbor(new Vector3(1, 2, 1), new Vector3(1, 2, 3)); //return true, Yp is now (1, 2, 3)
        /// ReplaceNeighbor(new Vector3(1, 4, 1), new Vector3(1, 2, 3)); //return false, no neighbor equal to (1, 4, 1)
        /// ReplaceNeighbor(new Vector3(1, 2, 1), new Vector3(1, 3, 3)); //return exception, (1, 3, 3) is not a face position
        /// </code>
        /// </example>
        public bool ReplaceNeighbor(Vector3 faceToReplace, Vector3 newFace)
        {
            bool res = false;

            if(!IsAFace(faceToReplace) || !IsAFace(newFace))
            {
                throw new Exception("Invalide face _position");
            }
            else
            {
                for (int i = 0; i < 6; ++i)
                {
                    if (_neighbors[i] == faceToReplace)
                    {
                        _neighbors[i] = newFace;
                        res = true;
                        break;
                    }
                }

                return res;
            }
        }



        /// <summary>
        /// Update the neighbors of the face depends on
        /// the orientation of it.
        /// </summary>
        ///<example>
        /// <code>
        /// //Create a new face on (1, 1, 0) position, orientation Z
        /// Face face = new Face(new Vector3(1, 1, 0), orientation.Z);
        /// 
        /// //add a neighbor position on Yn and Yp
        /// face.Zn = new Vector3(0, 1, 1); //Impossible in Z orientation(correspond to Xn)
        /// face.Zp = new Vector3(2, 1, 1); //Same (correspond to Xp)
        /// face.Yn = new Vector3(1, 0, 1); 
        /// face.Yp = new Vector3(1, 2, 1);
        /// 
        /// face.RepositionNeighbors();
        /// /**
        /// Zn = NULL_FACE;
        /// Zp = NULL_FACE;
        /// Xn = (0, 1, 1);
        /// Xp = (2, 1, 1);
        /// **/
        /// 
        /// </code>
        /// </example>
        public void RepositionNeighbors()
        {
            Face copy = new Face(this);
            Xn = NULL_FACE;
            Xp = NULL_FACE;
            Yn = NULL_FACE;
            Yp = NULL_FACE;
            Zn = NULL_FACE;
            Zp = NULL_FACE;

            for (int i = 0; i < 6; ++i)
            {
                switch (Orientation)
                {
                    case orientation.X:
                        if (copy.Neighbors[i] != NULL_FACE)
                        {
                            if (copy.Neighbors[i].Y < Y) { Yn = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].Y > Y) { Yp = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].Z < Z) { Zn = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].Z > Z) { Zp = copy.Neighbors[i]; }
                            this.PrintDebugLog2("X1");
                        }
                        break;

                    case orientation.Y:
                        if(copy.Neighbors[i] != NULL_FACE)
                        {
                            if (copy.Neighbors[i].X < X) { Xn = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].X > X) { Xp = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].Z < Z) { Zn = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].Z > Z) { Zp = copy.Neighbors[i]; }
                            this.PrintDebugLog2("Y1");
                        }
                        break;

                    case orientation.Z:
                        if (copy.Neighbors[i] != NULL_FACE)
                        {
                            if (copy.Neighbors[i].X < X) { Xn = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].X > X) { Xp = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].Y < Y) { Yn = copy.Neighbors[i]; }
                            else if (copy.Neighbors[i].Y > Y) { Yp = copy.Neighbors[i]; }
                            this.PrintDebugLog2("Z1");
                        }
                        break ;

                    default:
                        break;
                }
            }

            switch (Orientation)
            {
                case orientation.X:
                    Xn = NULL_FACE;
                    Xp = NULL_FACE;
                    this.PrintDebugLog2("X2");
                    break;
                case orientation.Y:
                    Yn = NULL_FACE;
                    Yp = NULL_FACE;
                    this.PrintDebugLog2("Y2");
                    break;
                case orientation.Z :
                    Zn = NULL_FACE;
                    Zp = NULL_FACE;
                    this.PrintDebugLog2("Z2");
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Try to set a face target as a neibor to the current referenced face
        /// (unidirectional link)
        /// </summary>
        /// <param name="faceTarget"> tThe targeted face </param>
        /// <returns> Setted or not </returns>
        /// <example>
        /// <code>
        /// Face face = new Face(new Vector3(0, 1, 1), orientation.X);
        /// Face faceTarget1 = new Face(1, 0, 1);
        /// bool isSeted = face.TryLinkToFaceOrigin(faceTarget); //b here is true
        /// 
        /// Face face = new Face(0, 1, 1);
        /// Face faceTarget2 = new Face(1, 4, 1);
        /// bool notSeted = face.TryLinkToFaceOrigin(faceTarget2); //b here is false
        /// </code>
        /// </example>
        public bool TryLinkToFaceOrigin(Face faceTarget)
        {
            orientation o = Graphe.GetOrientationFaceFromPos(_position);
            Vector3 diff;
            diff = faceTarget._position - _position;
            bool isLinked = false;
            switch (o)
            {
                case orientation.X:
                    //Y
                    if (diff == -2 * Vector3.UnitY || diff == -Vector3.UnitY + Vector3.UnitX || diff == -Vector3.UnitY - Vector3.UnitX)
                    {
                        if(Yn == Face.NULL_FACE)
                        {

                            Yn = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    if (diff == 2 * Vector3.UnitY || diff == Vector3.UnitY + Vector3.UnitX || diff == Vector3.UnitY - Vector3.UnitX)
                    {
                        if (Yp == Face.NULL_FACE)
                        {
                            Yp = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }

                    //Z
                    if (diff == -2 * Vector3.UnitZ || diff == -Vector3.UnitZ + Vector3.UnitX || diff == -Vector3.UnitZ - Vector3.UnitX)
                    {
                        if (Zn == Face.NULL_FACE)
                        {
                            Zn = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    if (diff == 2 * Vector3.UnitZ || diff == Vector3.UnitZ + Vector3.UnitX || diff == Vector3.UnitZ - Vector3.UnitX)
                    {
                        if (Zp == Face.NULL_FACE)
                        {
                            Zp = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    break;
                case orientation.Y:
                    //X
                    if (diff == -2 * Vector3.UnitX || diff == -Vector3.UnitX + Vector3.UnitY || diff == -Vector3.UnitX - Vector3.UnitY)
                    {
                        if (Xn == Face.NULL_FACE)
                        {
                            Xn = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    if (diff == 2 * Vector3.UnitX || diff == Vector3.UnitX + Vector3.UnitY || diff == Vector3.UnitX - Vector3.UnitY)
                    {
                        if (Xp == Face.NULL_FACE)
                        {
                            Xp = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }

                    //Z
                    if (diff == -2 * Vector3.UnitZ || diff == -Vector3.UnitZ + Vector3.UnitY || diff == -Vector3.UnitZ - Vector3.UnitY)
                    {
                        if (Zn == Face.NULL_FACE)
                        {
                            Zn = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    if (diff == 2 * Vector3.UnitZ || diff == Vector3.UnitZ + Vector3.UnitY || diff == Vector3.UnitZ - Vector3.UnitY)
                    {
                        if (Zp == Face.NULL_FACE)
                        {
                            Zp = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    break;
                default:
                    //X
                    if (diff == -2 * Vector3.UnitX || diff == -Vector3.UnitX + Vector3.UnitZ || diff == -Vector3.UnitX - Vector3.UnitZ)
                    {
                        if (Xn == Face.NULL_FACE)
                        {
                            Xn = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    if (diff == 2 * Vector3.UnitX || diff == Vector3.UnitX + Vector3.UnitZ || diff == Vector3.UnitX - Vector3.UnitZ)
                    {
                        if (Xp == Face.NULL_FACE)
                        {
                            Xp = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }

                    //Y
                    if (diff == -2 * Vector3.UnitY || diff == -Vector3.UnitY + Vector3.UnitZ || diff == -Vector3.UnitY - Vector3.UnitZ)
                    {
                        if (Yn == Face.NULL_FACE)
                        {
                            Yn = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    if (diff == 2 * Vector3.UnitY || diff == Vector3.UnitY + Vector3.UnitZ || diff == Vector3.UnitY - Vector3.UnitZ)
                    {
                        if (Yp == Face.NULL_FACE)
                        {
                            Yp = faceTarget._position;
                            isLinked = true;
                            break;
                        }
                    }
                    break;
            }
            return isLinked;
        }


        /// <summary>
        /// Check if a position correspond to a face position
        /// </summary>
        /// <param name="pos"> The position to test </param>
        /// <returns> True if it's a face position </returns>
        /// <example>
        /// <code>
        /// //True results (only one of X,Y or Z is pair)
        /// IsAFace(new Vector3(1, 1, 0)); 
        /// IsAFace(new Vector3(1, 0, 1));
        /// IsAFace(new Vector3(0, 1, 1));
        /// 
        /// //False results
        /// IsAFace(new Vector3(1, 1, 1));
        /// IsAFace(new Vector3(0, 0, 0));
        /// IsAFace(new Vector3(0, 0, 1));
        /// </code>
        /// </example>
        public static bool IsAFace(Vector3 pos)
        {
            int count = 0;

            if (pos.X % 2 == 0) { count++; }
            if (pos.Y % 2 == 0) { count++; }
            if (pos.Z % 2 == 0) { count++; }

            return count == 1;
        }




        /// <summary>
        /// Check if a position correspond to an edge position
        /// </summary>
        /// <param name="edgeToTest">Position to test</param>
        /// <returns> True if it's an edge position </returns>
        /// <example>
        /// <code>
        /// //True results (only two of X,Y or Z is pair)
        /// IsEdge(new Vector3(1, 0, 0)); 
        /// IsEdge(new Vector3(0, 0, 1));
        /// IsEdge(new Vector3(0, 1, 0));
        /// 
        /// //False results
        /// IsEdge(new Vector3(1, 1, 1));
        /// IsEdge(new Vector3(0, 0, 0));
        /// IsEdge(new Vector3(0, 1, 1));
        /// </code>
        /// </example>
        public static bool IsEdge(Vector3 edgeToTest)
        {
            int cpt = 0;

            if (edgeToTest.X % 2 == 0) { cpt++; }
            if (edgeToTest.Y % 2 == 0) { cpt++; }
            if (edgeToTest.Z % 2 == 0) { cpt++; }

            return cpt == 2;
        }




        //-----------------------------------------------------------------------------
        //-------------------------------------PRINT-----------------------------------
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Console display of a face
        /// </summary>
        public void Print()
        {
            if (_position.X < 0 && _position.Y < 0 && _position.Z < 0)
            {
                Console.Write("| " + this._position + " |");
            }
            else
            {
                if ((_position.X < 0 && _position.Y < 0) || (_position.X < 0 && _position.Z < 0) || (_position.Y < 0 && _position.Z < 0))
                {
                    Console.Write("|  " + this._position + " |");
                }
                else
                {
                    if (_position.X < 0 || _position.Y < 0 || _position.Z < 0)
                    {
                        Console.Write("|  " + this._position + "  |");
                    }
                    else
                    {
                        Console.Write("|  " + this._position + "   |");
                    }
                }
            }

            if (this.Xn.Y != -1)
            {
                Console.Write("  " + this.Xn + "   |");
            }
            else { Console.Write("     NULL     |"); }

            if (this.Xp.Y != -1)
            {
                Console.Write("  " + this.Xp + "   |");
            }
            else { Console.Write("     NULL     |"); }

            if (this.Yn.Y != -1)
            {
                Console.Write("  " + this.Yn + "   |");
            }
            else { Console.Write("     NULL     |"); }


            if (this.Yp.Y != -1)
            {
                Console.Write("  " + this.Yp + "   |");
            }
            else { Console.Write("     NULL     |"); }

            if (this.Zn.Y != -1)
            {
                Console.Write("  " + this.Zn + "   |");
            }
            else { Console.Write("     NULL     |"); }


            if (this.Zp.Y != -1)
            {
                Console.Write("  " + this.Zp + "   |");
            }
            else { Console.Write("     NULL     |"); }


            Console.Write("\n");
            Console.WriteLine("|______________|______________|______________|______________|______________|______________|______________|");
        }

        /// <summary>
        /// Console display of a face
        /// </summary>
        public string PrintDebugLog()
        {
            string res = "";
            if (_position.X < 0 && _position.Y < 0 && _position.Z < 0)
            {
                res += "| " + this._position + " |";
            }
            else
            {
                if ((_position.X < 0 && _position.Y < 0) || (_position.X < 0 && _position.Z < 0) || (_position.Y < 0 && _position.Z < 0))
                {
                    res += "|  " + this._position + " |";
                }
                else
                {
                    if (_position.X < 0 || _position.Y < 0 || _position.Z < 0)
                    {
                        res += "|  " + this._position + "  |";
                    }
                    else
                    {
                        res += "|  " + this._position + "   |";
                    }
                }
            }

            if (this.Xn.Y != -1)
            {
                res +=("  " + this.Xn + "   |");
            }
            else { res +=("     NULL     |"); }

            if (this.Xp.Y != -1)
            {
                res +=("  " + this.Xp + "   |");
            }
            else { res +=("     NULL     |"); }

            if (this.Yn.Y != -1)
            {
                res +=("  " + this.Yn + "   |");
            }
            else { res +=("     NULL     |"); }


            if (this.Yp.Y != -1)
            {
                res +=("  " + this.Yp + "   |");
            }
            else { res +=("     NULL     |"); }

            if (this.Zn.Y != -1)
            {
                res +=("  " + this.Zn + "   |");
            }
            else { res +=("     NULL     |"); }


            if (this.Zp.Y != -1)
            {
                res +=("  " + this.Zp + "   |");
            }
            else { res +=("     NULL     |"); }


            res += "\n";
            res += "|______________|______________|______________|______________|______________|______________|______________|\n";

            return res;
        }

        public void PrintDebugLog2(string c)
        {
            string res = "\nDetail of a face: Position " + c + "\n";
            res += "__________________________________________________________________________________________________________\n";
            res += "|   position   |      Xn      |      Xp      |      Yn      |      Yp      |      Zn      |      Zp      |\n";
            res += "|______________|______________|______________|______________|______________|______________|______________|\n";
            res+= this.PrintDebugLog();
            string filePath = "./file.txt";

            try
            {
                // Write the content of 'res' to the file
                res += "\n";
                File.AppendAllText(filePath, res);
                //res = ("Debug log written to file: " + filePath);
            }
            catch (Exception e)
            {
                //res = ("Error writing to file: " + e.Message);
            }
        }

        /// <summary>
        /// Convert a face to string (position and orientation)
        /// </summary>
        /// <returns>"Orientation"+"Position"</returns>
        public override string ToString()
        {
            string o = "";
            o += _position.X + " " + _position.Y + " " + _position.Z;
            return o;
        }
    }
}