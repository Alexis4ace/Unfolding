using UnityEngine;
using TMPro;
using Unfolding.Utils;

namespace Unfolding.UI.Game
{
    /// <summary>
    /// This class handles the timer in the game's view
    /// </summary>
    public class TimerManager : MonoBehaviour, ITaggable
    {
        public string TAG => "Game - Timer Manager";

        /// <summary>
        /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
        /// </summary>
        public static TimerManager Current { get; private set; }

        // ATTRIBUTS RELATED TO THE TIMER'S DISPLAY

        /// <summary>
        /// Elapsed seconds since the start of the level
        /// </summary>
        public string Seconds { get; private set; }

        /// <summary>
        /// Elapsed minutes since the start of the level
        /// </summary>
        public string Minutes { get; private set; }

        /// <summary>
        /// GameObject that displays the timer in the game, since the game's view is loaded
        /// </summary>
        private TMP_Text TimerDisplay;

        // ATTRIBUTS RELATED TO THE VALUE OF THE FIMER

        /// <summary>
        /// Float value of the timer since the start of the level
        /// </summary>
        private float Timer;

        /// <summary>
        /// Float value of the timer since the start of the level load
        /// </summary>
        private float TimerLoad;

        /// <summary>
        /// Float that represents the start of the timer
        /// </summary>
        private float TimerStart;

        // BOOLEAN THAT CONTROL THE TIMER
        /// <summary>
        /// Boolean that tells wether the timer should start or not
        /// </summary>
        private bool IsTimerCounting;

        /// <summary>
        /// Start is called before the first frame update
        /// Initialize the timer
        /// </summary>
        private void Start()
        {
            Current = this;
            Timer = 0f;
            TimerStart = Time.time;
            TimerLoad = PlayerPrefs.GetFloat(PlayerPrefsTags.Time);

            IsTimerCounting = true;
            Seconds = "00";
            Minutes = "00";
            TimerDisplay = GetComponentInChildren<TMP_Text>();

            DisplayTimerValue();
        }

        /// <summary>
        /// Update is called once per frame
        /// Updates the timer
        /// </summary>
        private void Update()
        {
            if (IsTimerCounting)
            {
                Timer = Time.time - TimerStart + TimerLoad;
                Minutes = Mathf.Floor(Timer / 60).ToString("00");
                Seconds = Mathf.Floor(Timer % 60).ToString("00");
            }

            DisplayTimerValue();
        }

        /// <summary>
        /// A method that starts the timer
        /// </summary>
        public void StartTimer()
        {
            TimerStart = Time.time;
            IsTimerCounting = true;
            TimerLoad = 0;
        }

        /// <summary>
        /// A method that starts the timer when a game is loaded at the time the game was saved
        /// </summary>
        public void StartTimer(float tload)
        {
            TimerStart = Time.time;
            IsTimerCounting = true;
            TimerLoad = tload;
        }

        /// <summary>
        /// A method that pauses or stops the timer
        /// </summary>
        public void StopTimer()
        {
            IsTimerCounting = false;
        }

        /// <summary>
        /// A method that display the timer value if the interface
        /// </summary>
        private void DisplayTimerValue()
        {
            TimerDisplay.text = Minutes + " : " + Seconds;
            //Debug.Log(Minutes + " : " + Seconds);
        }

        /// <summary>
        /// A method that get the timer value if the interface
        /// </summary>
        public float getTimer()
        {
            return this.Timer;
        }
    }
}
