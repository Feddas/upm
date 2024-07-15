using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Feddas
{
    /// <summary> Disables the ability to modify a field inside the Unity inspector.
    /// Copied from https://gist.github.com/LotteMakesStuff/c0a3b404524be57574ffa5f8270268ea
    /// and http://answers.unity.com/answers/801283/view.html </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}
