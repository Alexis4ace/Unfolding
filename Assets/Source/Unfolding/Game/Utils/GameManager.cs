using Unfolding.Game.Gameplay;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject _polycube; // Access to polycube

    // Start is called before the first frame update
    void Start()
    {
        // Getting the polycube manager
        _polycube = GameObject.Find("PolyCube");

        // Switching cameras and updating settings
        //Unfolding.Utils.CamerasManager.Current.SwitchCameras();
        Unfolding.UI.Utils.Helper.UpdateSettings();

        // Launching structure generation
        _polycube.GetComponent<PolyCubeManager>().LaunchGame(); // Creating the structure
        Unfolding.UI.Utils.Helper.UpdateNumberOfCubes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}