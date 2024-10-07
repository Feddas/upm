using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Feddas
{
    /// <summary>
    /// Swaps SetActive state of gameobjects depending on which TState enum is selected as the currentVisual.
    /// </summary>
    /// <typeparam name="TStates"> Enum of available visual states. </typeparam>
    public class VisualStateSwap<TStates> : MonoBehaviour
        where TStates : Enum
    {
        [Tooltip("What set of visuals are being shown right now.")]
        [SerializeField]
        private TStates currentVisual;

        [Tooltip("Reference to Gameobjects to be set to active when their state is selected.")]
        [SerializeField]
        private List<VisualState<TStates>> visualStates;

        /// <summary> Backing field for a property. </summary>
        private Dictionary<TStates, VisualState<TStates>> backingVisualStates;

        /// <summary> The current visual state of this gameobject. </summary>
        public TStates CurrentVisual
        {
            get
            {
                return currentVisual;
            }
            set
            {
                if (currentVisual.Equals(value))
                {
                    return;
                }

                currentVisual = value;
                OnValidate();
            }
        }

        /// <summary> Indexes VisualStates by TStates enum.
        /// Having this property instatiate its backing field allows OnValidate() to work outside of Unity's runmode. </summary>
        private Dictionary<TStates, VisualState<TStates>> VisualStatesDictionary
        {
            get
            {
                if (backingVisualStates == null)
                {
                    Reset();
                }

                return backingVisualStates;
            }
        }

        private void OnValidate()
        {
            // null check & handle using this code from a unit test
            if (visualStates == null)
            {
                Debug.LogException(new ArgumentException("visualStates was not serialized correctly in the inspector. Using empty states."));
                visualStates = new List<VisualState<TStates>>();
            }

            // Make sure list follows dictionary restraints.
            var duplicates = visualStates.GroupBy(v => v.ActiveOn).SelectMany(g => g.Skip(1));
            foreach (var duplicate in duplicates)
            {
                visualStates.Remove(duplicate);
                Debug.LogWarning($"{this.name}s' VisualStates must be unique. A \"{duplicate.ActiveOn}\" state was removed to enforce this.");
            }

            // Add any missing states. If visualStates is not fully populated, it's hard to add that state in the Unity Editor later due to the duplicates restriction above.
            var requiredTypes = Enum.GetValues(typeof(TStates));
            if (visualStates.Count < requiredTypes.Length)
            {
                foreach (var missingType in requiredTypes.Cast<TStates>().Except(visualStates.Select(v => v.ActiveOn)))
                {
                    visualStates.Add(new VisualState<TStates>(missingType));

                    // null check & handle using this code from a unit test
                    if (this != null)
                    {
                        Debug.Log($"{this.name}s' VisualStates should contain all possible states. A \"{missingType}\" state was added to enforce this.");
                    }
                }
            }

            // convert to dictionary
            backingVisualStates = visualStates.ToDictionary(d => d.ActiveOn, d => d);
            UpdateActiveVisuals(backingVisualStates, CurrentVisual);
        }

        /// <summary>
        /// Activates one <paramref name="toState"/> out of the dictionary of <paramref name="fromStates"/>.
        /// Enumerates through the gameobjects in all states to turn on the one's matching toState.
        /// Turns off all other gameobjects not in toState.
        /// </summary>
        /// <param name="fromStates"> gameobjects from all visual states. </param>
        /// <param name="toState"> visual state to be turned on. </param>
        private void UpdateActiveVisuals(Dictionary<TStates, VisualState<TStates>> fromStates, TStates toState)
        {
            // turn on game objects
            var toBeTurnedOn = fromStates[toState].ActiveGameObjects;
            foreach (var toBeOn in toBeTurnedOn)
            {
                toBeOn.SetActive(true);
            }

            // turn off game objects
            var toBeTurnedOff = fromStates
                .Where(s => s.Key.Equals(toState) == false)
                .SelectMany(s => s.Value.ActiveGameObjects)
                .Where(s => s != null && toBeTurnedOn.Contains(s) == false); // leave items on that are in both toBeTurnedOn & toBeTurnedOff
            foreach (var toBeOff in toBeTurnedOff)
            {
                toBeOff.SetActive(false);
            }
        }

        private void Reset()
        {
            backingVisualStates = Enum
                .GetValues(typeof(TStates))
                .Cast<TStates>()
                .ToDictionary(d => d, d => new VisualState<TStates>(d));
            visualStates = backingVisualStates.Select(d => d.Value).ToList();
        }
    }
}
