namespace Feddas
{
    /// <summary> Which visual states are available to CourseContentButton. </summary>
    public enum CourseContentVisualStates
    {
        /// <summary> State shown when course is up to date. No download will happen. </summary>
        Installed,

        /// <summary> State shown when download is queued. </summary>
        Pending,

        /// <summary> State shown when download is occurring. </summary>
        Downloading,

        /// <summary> State shown when download completed successfully in this session. </summary>
        Downloaded,

        /// <summary> The content for this course is in a bad state or a download failed and could not be resumed. </summary>
        Error,

        /// <summary> Content is currently installed but will be removed after the current session is completed. </summary>
        QueuedForRemoval,

        /// <summary> Status of content is unknown. Occurs when this enum can't be parsed from a string. </summary>
        Null,
    }

    /// <summary> Controls which visuals state is active. </summary>
    public class CourseContentVisualSwap : VisualStateSwap<CourseContentVisualStates>
    {
    }
}
