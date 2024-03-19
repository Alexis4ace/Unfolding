using System.Collections.Generic;
using UnityEngine;
using Unfolding.Utils;
using Unfolding.Kernel;
using System.IO;
using System;

namespace Unfolding.Game.IO
{
    /// <summary>
    /// A static class that manage Read and write of the polycube from PlayerPrefs
    /// </summary>
    public static class IOPlayerPrefs
    {
        /// <summary>
        /// This method reads a graph from PlayerPrefs and stores it
        /// </summary>
        /// <remarks>This method calls two methods that reads the list of faces and the list of edges : 
        /// <see cref="ReadPlayerPrefsFaces"/>
        /// <returns>The graph read from PlayerPrefs</returns>
        public static Graphe ReadPlayerPrefs()
        {
            int nbCubes = PlayerPrefs.GetInt(PlayerPrefsTags.NumberOfCubes);
            return new Graphe(ReadPlayerPrefsFaces(), nbCubes);
        }

        public static string ABSOLUTPATH =
            Application.persistentDataPath
            + (System.Environment.OSVersion.Platform == PlatformID.Win32NT ? "\\" : "/");

        /// <summary>
        /// An internal method that returns the list of faces in PlayerPrefs
        /// </summary>
        /// <returns> The list of faces </returns>
        internal static Dictionary<System.Numerics.Vector3, Kernel.Face> ReadPlayerPrefsFaces()
        {
            Dictionary<System.Numerics.Vector3, Kernel.Face> faces = new Dictionary<System.Numerics.Vector3, Kernel.Face>();

            string[] result = PlayerPrefs.GetString(PlayerPrefsTags.ListOfFaces).Split(' ');

            for (int i = 0; i < result.Length - 1; i += 16)
            {
                System.Numerics.Vector3 vtmp = new System.Numerics.Vector3(
                        float.Parse(result[i]),
                        float.Parse(result[i + 1]),
                        float.Parse(result[i + 2])
                    );
                System.Numerics.Vector3 vneighbor1 = new System.Numerics.Vector3(
                    float.Parse(result[i + 3]),
                    float.Parse(result[i + 4]),
                    float.Parse(result[i + 5])
                );
                System.Numerics.Vector3 vneighbor2 = new System.Numerics.Vector3(
                   float.Parse(result[i + 6]),
                   float.Parse(result[i + 7]),
                   float.Parse(result[i + 8])
               );

                System.Numerics.Vector3 vneighbor3 = new System.Numerics.Vector3(
                   float.Parse(result[i + 9]),
                   float.Parse(result[i + 10]),
                   float.Parse(result[i + 11])
               );

                System.Numerics.Vector3 vneighbor4 = new System.Numerics.Vector3(
                   float.Parse(result[i + 12]),
                   float.Parse(result[i + 13]),
                   float.Parse(result[i + 14])
               );

                faces.Add(vtmp, new Face(vneighbor1, vneighbor2, vneighbor3, vneighbor4, vtmp, IOFile.getOrientation(result[i+15])));
            }


            return faces;
        }

        /// <summary>
        /// This method writes a graph in PlayerPrefs
        /// </summary>
        /// <remarks>This method calls two methods that writes the list of faces and the list of edges in PlayerPrefs : 
        /// <see cref="WritePlayerPrefsFaces(Dictionary{System.Numerics.Vector3, Face})"/>
        /// <param name="g">The graph that will be stored in PlayerPrefs</param>
        public static void WritePlayerPrefs(Graphe g)
        {
            PlayerPrefs.SetInt(PlayerPrefsTags.NumberOfCubes, g.NumberCubes);
            WritePlayerPrefsFaces(g.Faces);
        }

        /// <summary>
        /// An internal method that writes a list of faces in PlayerPrefs
        /// </summary>
        /// <param name="faces"> The list of faces </param>
        internal static void WritePlayerPrefsFaces(Dictionary<System.Numerics.Vector3, Kernel.Face> faces)
        {
            string result = "";
            foreach (Kernel.Face v in faces.Values)
            {
                result += v + " ";
                if ( v.Orientation == orientation.X) {
                    result += IOFile.WriteVector(v.Yn);
                    result += IOFile.WriteVector(v.Yp);
                    result += IOFile.WriteVector(v.Zn);
                    result += IOFile.WriteVector(v.Zp);
                }
                else if( v.Orientation == orientation.Y)
                {
                    result += IOFile.WriteVector(v.Xn);
                    result += IOFile.WriteVector(v.Xp);
                    result += IOFile.WriteVector(v.Zn);
                    result += IOFile.WriteVector(v.Zp);
                }
                else
                {
                    result += IOFile.WriteVector(v.Xn);
                    result += IOFile.WriteVector(v.Xp);
                    result += IOFile.WriteVector(v.Yn);
                    result += IOFile.WriteVector(v.Yp);
                }


                result += v.Orientation + " \n";
            }

            PlayerPrefs.SetString(PlayerPrefsTags.ListOfFaces, result);
        }

        private static string WriteVector(System.Numerics.Vector3 vec)
        {
            System.Numerics.Vector3 NULL_FACE = new System.Numerics.Vector3(-1, -1, -1);
            string line = "";

            line += vec.X + " " + vec.Y + " " + vec.Z + " ";

            return line;
        }

        private static orientation getOrientation(string str)
        {
            if (str.CompareTo("X") == 0)
                return orientation.X;
            else if (str.CompareTo("Y") == 0)
                return orientation.Y;
            else
                return orientation.Z;
        }
    }

}
