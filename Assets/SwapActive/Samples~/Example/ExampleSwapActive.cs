using System.Collections;
using UnityEngine;

namespace Feddas
{
    [RequireComponent(typeof(CourseContentVisualSwap))]
    public class ExampleSwapActive : MonoBehaviour
    {
        public float SecondsToNextState = 2;
        public UnityEngine.UI.Text label;

        private CourseContentVisualSwap multiStateObj
        {
            get
            {
                return _multiStateObj ??= this.GetComponent<CourseContentVisualSwap>();
            }
        }
        private CourseContentVisualSwap _multiStateObj;

        /// <summary> In what order states will be traversed. </summary>
        private readonly CourseContentVisualStates[] traversal = new CourseContentVisualStates[]
        {
            CourseContentVisualStates.Error,
            CourseContentVisualStates.Pending,
            CourseContentVisualStates.Downloading,
            CourseContentVisualStates.Downloaded,
            CourseContentVisualStates.Installed,
            CourseContentVisualStates.QueuedForRemoval,
        };

        private int traversalIndex = -1; // First iteration of Start will increment -1 to 0

        IEnumerator Start()
        {
            while (true)
            {
                // show next visual;
                traversalIndex = (traversalIndex + 1) % traversal.Length;
                multiStateObj.CurrentVisual = traversal[traversalIndex];
                if (label != null)
                {
                    label.text = "SwapActive set to\n" + multiStateObj.CurrentVisual.ToString();
                }
                yield return new WaitForSeconds(SecondsToNextState);
            }
        }
    }
}
