using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups;

/// <summary>
/// Abstraction needed for Mocking or providing a custom implementation of injection into the Player Loop
/// </summary>
public interface IUnityPlayerLoopHelper
{
    /// <summary>
    /// Attaches all system groups to the Unity Player Loop.
    /// </summary>
    void AppendWorldToCurrentPlayerLoop(
        InitializationSystemGroup initializationSystemGroup,
        SimulationSystemGroup simulationSystemGroup,
        PresentationSystemGroup presentationSystemGroup,
        PostRenderingSystemGroup postRenderingSystemGroup,
        PhysicsSystemGroup physicsSystemGroup,
        PostPhysicsSystemGroup postPhysicsSystemGroup);

    /// <summary>
    /// Removes the given system group from the Unity Player Loop.
    /// </summary>
    /// <param name="systemGroup"></param>
    void RemoveFromCurrentPlayerLoop(SystemGroup systemGroup);
}