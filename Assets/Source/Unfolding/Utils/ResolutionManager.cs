using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class manages the resolutions of the game.
/// </summary>
public class ResolutionManager : MonoBehaviour
{
    // Singleton instance
    public static ResolutionManager Instance { get; private set; }

    // Store the selected resolution index
    private int selectedResolutionIndex;

    // Reference to the CanvasScaler component
    [SerializeField] private CanvasScaler canvasScaler;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mark the GameObject as "Don't Destroy On Load"
        }
        else
        {
            // If an instance already exists, destroy this GameObject
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the selected resolution index and apply the selected resolution.
    /// </summary>
    /// <param name="index">The index of the selected resolution.</param>
    public void SetSelectedResolutionIndex(int index)
    {
        selectedResolutionIndex = index;
        ApplySelectedResolution();
    }

    /// <summary>
    /// Apply the selected resolution to the screen.
    /// </summary>
    private void ApplySelectedResolution()
    {
        Resolution[] resolutions = Screen.resolutions;

        // Check if the selected index is within bounds
        if (selectedResolutionIndex >= 0 && selectedResolutionIndex < resolutions.Length)
        {
            // Retrieve the selected resolution
            Resolution selectedResolution = resolutions[selectedResolutionIndex];

            // Set the screen resolution using the selected resolution's width, height, and fullscreen mode
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
        }
    }

    /// <summary>
    /// Get the index of the currently selected resolution.
    /// </summary>
    /// <returns>The index of the selected resolution.</returns>
    public int GetSelectedResolutionIndex()
    {
        return selectedResolutionIndex;
    }
}
