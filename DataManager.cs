using System;
using System.Numerics;


namespace Model_Depliage
{
    /// <summary>
    /// Tool to save and load polycube structure from extern files
    /// </summary>
    public static class DataManager
    {
        /// <summary>
        /// The Path where read and write file
        /// </summary>
        public static string PATHWRITE = "";

        /// <summary>
        /// The time of the game, we get the timer with unity :  Unfolding.UI.Game.TimerManager.Current.getTimer()
        /// </summary>
        public static short TIME = 00;



        /// <summary>
        /// This methods creates a file and stores a graph in it
        /// </summary>
        /// <param name="filename">The name of the file created</param>
        /// <param name="g">The graph that we want to serialize</param>
        public static void WriteFile(string filename, Graphe_faces g)
        {

            string result = "";
            // Write the number of cubes
            result += g.NumberCubes + "\n";

            // Write all faces
            foreach (Face v in g.Faces.Values)
            {
                result += v + " "; //save face V and for each save these neighbors ( 4 neighbors )  

                if (v.Orientation == orientation.X)
                { // depending on the orientation we save the good neighbors
                    result += WriteVector(v.Yn);
                    result += WriteVector(v.Yp);
                    result += WriteVector(v.Zn);
                    result += WriteVector(v.Zp);
                }
                else if (v.Orientation == orientation.Y)
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


                result += v.Orientation + " \n"; // we save the orientation 
            }

            result += "*" + "\n";
            // write the current time of the game in the file
            result += TIME; 
            //absolutePath find the folder of the output build game
            filename = PATHWRITE + filename + ".cub";
            File.WriteAllText(filename, result);
        }

        /// <summary>
        /// This methods reads a file in the correct folder and convert it into a graph
        /// </summary>
        /// <param name="path">The name of the file</param>
        /// <returns>The graph described in the file</returns>
        public static Graphe_faces ReadFile(string path)
        {

            Dictionary<System.Numerics.Vector3, Face> faces =
                new Dictionary<System.Numerics.Vector3, Face>();

            path = PATHWRITE + path + ".cub";
            StreamReader reader = new StreamReader(path);

            // Getting the number of cubes
            int nbCubes = int.Parse(reader.ReadLine().Split(' ')[0]);

            string line = "";
            // Getting all the faces
            while ((line = reader.ReadLine()) != "*")
            {
                //"*" is the break point 
                if (line == "*")
                    break;
                else if (line == "")
                    break;
                else
                {
                    string[] result = line.Split(' ');   // get the face 
                    System.Numerics.Vector3 vtmp = new System.Numerics.Vector3(
                        float.Parse(result[0]),
                        float.Parse(result[1]),
                        float.Parse(result[2])
                    );
                    System.Numerics.Vector3 vneighbor1 = new System.Numerics.Vector3(   // get 4 neighbor 
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

                    faces.Add(vtmp, new Face(vneighbor1, vneighbor2, vneighbor3, vneighbor4, vtmp, getOrientation(result[15])));  //add a face with 4 neighbors and orientation in the dictionnary 
                }
            }
            //load the timer and launch it
            if ((line = reader.ReadLine()) != null)
                TIME = short.Parse(line);

            Graphe_faces g = new Graphe_faces(faces, nbCubes);
            return g;
        }

        //conversion a string to orientation
        public static orientation getOrientation(string str)
        {
            if (str.CompareTo("X") == 0)
                return orientation.X;
            else if (str.CompareTo("Y") == 0)
                return orientation.Y;
            else if (str.CompareTo("Z") == 0)
                return orientation.Z;
            else
                throw new ArgumentException("Orientation not equals to X , Y or Z ");
        }


        // delete a file 
        public static void DeleteFile(String filename)
        {
            File.Delete(PATHWRITE + filename + ".cub");
        }

        //write a 3 number of vector
        public static string WriteVector(System.Numerics.Vector3 vec)
        {
            System.Numerics.Vector3 NULL_FACE = new System.Numerics.Vector3(-1, -1, -1);
            string line = "";

            line += vec.X + " " + vec.Y + " " + vec.Z + " ";

            return line;
        }

        //set a path for save
        public static void setPath(string newPath)
        {
            PATHWRITE = newPath + "/";
        }
    }
}