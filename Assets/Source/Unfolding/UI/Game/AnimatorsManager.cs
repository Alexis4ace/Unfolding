using System.Collections.Generic;
using UnityEngine;
using Unfolding.Utils;

namespace Unfolding.UI
{
    namespace Game
    {
        /// <summary>
        /// This class can be attached to an empty game object, which have childrens with animator component.
        /// It will then allow other classes to play animation on demand.
        /// </summary>
        internal sealed class AnimatorsManager : MonoBehaviour, ITaggable
        {
            // CLASS'S ATTRIBUTES
            public string TAG => "Game - Animators Manager";

            /// <summary>
            /// Implementation of the singleton pattern : we have only one instance of this class, which we access by this variable
            /// </summary>
            public static AnimatorsManager Current { get; private set; }

            // SCENE'S OBJECTS
            /// <summary>
            /// List of existing animators in the game
            /// </summary>
            private List<Animator> _animators;

            /// <summary>
            /// Name of the current panel which is visible (only one can be visible at a given time)
            /// </summary>
            [HideInInspector] public string CurrentShowedPanel = ""; 

            // OTHER ATTRIBUTES
            /// <summary>
            /// Static string variable which will launch the "hide" animation
            /// </summary>
            private static string s_hideAction = "Hide";

            /// <summary>
            /// Static string variable which will launch the "show" animation
            /// </summary>
            private static string s_showAction = "Show";

            /// <summary>
            /// Awake method, called first in script life's cycle.
            /// </summary>
            private void Awake()
            {
                AnimatorsManager.Current = this; // Initialize only instance of this class
                _animators = new List<Animator>(this.GetComponentsInChildren<Animator>()); // Getting back all animators in childrens
            }

            /// <summary>
            /// This function allows to command an animator from outside a class to launch a special animation.
            /// </summary>
            /// <param name="buttonName">String, name of the button just clicked. The first word of associated animator's name should be equal to it.</param>
            public void PlayAnimation(string buttonName)
            {
                Animator animator = _animators.Find(anim => Utils.Helper.GetFirstWord(anim.name).Equals(buttonName));

                if (animator is not null) // If there is an animator linked to clicked button
                {
                    // We first need to check if showed panel is equal the one attached to found animator
                    if (CurrentShowedPanel.Equals(animator.gameObject.name))
                    {
                        // If so, we should hide the panel and update _currentShowedPanel
                        animator.SetTrigger(s_hideAction);
                        CurrentShowedPanel = "";
                    }
                    else 
                    {
                        // In this case, we should first hide _currentShowedPanel (if there is one), and then show requested panel
                        if (!CurrentShowedPanel.Equals(""))
                        {
                            // We can get back animator associated to showed panel by comparing first word of their name
                            Animator showedPanelAnimator = _animators.Find (
                                anim => Utils.Helper.GetFirstWord(anim.name).Equals(Utils.Helper.GetFirstWord(CurrentShowedPanel))
                            );

                            showedPanelAnimator.SetTrigger(s_hideAction);
                        }

                        // Now we can show requested panel
                        animator.SetTrigger(s_showAction);
                        CurrentShowedPanel = animator.gameObject.name;

                        /* 
                        NOTE:
                            if you don't understand what we have done it here, there is a quick explanation :
                            animators in the game's view are attached to panels, and panels are named with a keyword  + " panel".
                            So to compare animators, panels and buttons, we systematically need to extract their first word,
                            which is for all, a keyword.
                        */
                    }
                }
            }
        }
    }
}