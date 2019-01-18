using System;

namespace osuTK.Input
{
    /// <summary>
    /// Defines the event data for <see cref="NativeWindow"/> events.
    /// </summary>
    public class FileDropEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the names of the files dropped.
        /// </summary>
        /// <value>The names of the files.</value>
        public string[] FileNames
        {
            get;
            internal set;
        }
    }
}
