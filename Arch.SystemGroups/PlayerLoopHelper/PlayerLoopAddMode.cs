namespace Arch.SystemGroups
{
    /// <summary>
    /// Determines whether the system should be added to the beginning or the end of the step of the player loop
    /// </summary>
    internal enum PlayerLoopAddMode : byte
    {
        /// <summary>
        /// Add the system to the beginning of the step
        /// </summary>
        Prepend,
        
        /// <summary>
        /// Add the system to the end of the step
        /// </summary>
        Append
    }
}