using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unfolding.Utils;
using Unfolding.Kernel;
using System.Linq;

namespace Unfolding.Game.IO
{
    /// <summary>
    /// A static class that manage Read and write of the polycube from file
    /// </summary>
    public static class IOFile
    {
        /// <value>
        /// <c>ABSOLUTPATH</c> is the path where is stored every saved file in the game
        /// <remarks> In Windows, it is stored at %appdata%\LocalLow\DefaultCompany\CubeGame\</remarks>
        /// <remarks> In Linux, it is stored at ~/.config/unity3d/DefaultCompany/CubeGame</remarks>
        /// </value>
        public static string ABSOLUTPATH =
            Application.persistentDataPath
            + (System.Environment.OSVersion.Platform == PlatformID.Win32NT ? "\\" : "/");

        
        /// <summary>
        /// This methods reads a file in the correct folder and convert it into a graph
        /// </summary>
        /// <param name="path">The name of the file</param>
        /// <returns>The graph described in the file</returns>
        public static Graphe ReadFile(string path)
        {
            Dictionary<System.Numerics.Vector3, Kernel.Face> faces =
                new Dictionary<System.Numerics.Vector3, Kernel.Face>();

            path = ABSOLUTPATH + path + ".cub";
            StreamReader reader = new StreamReader(path);

            // Getting the number of cubes
            int nbCubes = int.Parse(reader.ReadLine().Split(' ')[0]);
            PlayerPrefs.SetInt(PlayerPrefsTags.NumberOfCubes, nbCubes);

            string line = "";
            // Getting all the faces
            while ((line = reader.ReadLine()) != "*")
            {
                //"*" is the break point between Faces and Edges
                if (line == "*")
                    break;
                else if (line == "")
                    break;
                else
                {
                    string[] result = line.Split(' ');
                    System.Numerics.Vector3 vtmp = new System.Numerics.Vector3(
                        float.Parse(result[0]),
                        float.Parse(result[1]),
                        float.Parse(result[2])
                    );
                    System.Numerics.Vector3 vneighbor1 = new System.Numerics.Vector3(
                        float.Parse(result[3]),
                        float.Parse(result[4]),
                        float.Parse(result[5])
                    );
                    System.Numerics.Vector3 vneighbor2 = new System.Numerics.Vector3(
                       float.Parse(result[6]),
                       float.Parse(result[7]),
                       float.Parse(result[8])
                   );
                    System.Numerics.Vector3 vneighbor3 = new System.Numerics.Vector3(
                       float.Parse(result[9]),
                       float.Parse(result[10]),
                       float.Parse(result[11])
                   );
                    System.Numerics.Vector3 vneighbor4 = new System.Numerics.Vector3(
                       float.Parse(result[12]),
                       float.Parse(result[13]),
                       float.Parse(result[14])
                   );

                    faces.Add(vtmp, new Face(vneighbor1, vneighbor2, vneighbor3, vneighbor4 , vtmp , getOrientation(result[15])  )   );
                }
            }

            //load the timer and launch it
            if ((line = reader.ReadLine()) != null)
            {
                PlayerPrefs.SetFloat(PlayerPrefsTags.Time, float.Parse(line));
            }

            Graphe g = new Graphe(faces, nbCubes);
            return g;
        }

        /// <summary>
        /// This methods creates a file and stores a graph in it
        /// </summary>
        /// <param name="filename">The name of the file created</param>
        /// <param name="g">The graph that we want to serialize</param>
        public static void WriteFile(string filename, Graphe g)
        {
            string result = "";
            // Write the number of cubes
            result += g.NumberCubes + "\n";

            // Write all faces
            foreach (Kernel.Face v in g.Faces.Values)
            {
                result += v + " ";
                
                if( v.Orientation == orientation.X) {
                    result += WriteVector(v.Yn);
                    result += WriteVector(v.Yp);
                    result += WriteVector(v.Zn);
                    result += WriteVector(v.Zp);
                }
                else if( v.Orientation == orientation.Y)
                {
                    result += WriteVector(v.Xn);
                    result += WriteVector(v.Xp);
                    result += WriteVector(v.Zn);
                    result += WriteVector(v.Zp);
                }
                else
                {
                    result += WriteVector(v.Xn);
                    result += WriteVector(v.Xp);
                    result += WriteVector(v.Yn);
                    result += WriteVector(v.Yp);
                }

                result += v.Orientation + " \n";
            }

            result += "*" + "\n";
            result += Unfolding.UI.Game.TimerManager.Current.getTimer();

            //absolutePath find the folder of the output build game
            filename = ABSOLUTPATH + filename + ".cub";
            File.WriteAllText(filename, result);
        }

        //write a 3 number of vector
        public static string WriteVector(System.Numerics.Vector3 vec)
        {
            System.Numerics.Vector3 NULL_FACE = new System.Numerics.Vector3(-1, -1, -1);
            string line = "";

            line += vec.X + " " + vec.Y + " " + vec.Z + " ";

            return line;
        }

         //conversion a string to orientation
        public static orientation getOrientation(string str)
        {
            if (str.CompareTo("X") == 0)
                return orientation.X;
            else if (str.CompareTo("Y") == 0)
                return orientation.Y;
            else
                return orientation.Z;
        }

        // delete a file
        public static void DeleteFile(String filename)
        {
            File.Delete(ABSOLUTPATH+filename + ".cub");
        }

        public static void setPath(string newPath)
        {
            ABSOLUTPATH = Path.GetDirectoryName(newPath)+"/";
        }
    }
}
