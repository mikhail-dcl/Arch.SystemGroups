using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups;

public interface IUnityPlayerLoopHelper
{
    void AppendWorldToCurrentPlayerLoop(
        InitializationSystemGroup initializationSystemGroup,
        SimulationSystemGroup simulationSystemGroup,
        PresentationSystemGroup presentationSystemGroup,
        PostRenderingSystemGroup postRenderingSystemGroup,
        PhysicsSystemGroup physicsSystemGroup,
        PostPhysicsSystemGroup postPhysicsSystemGroup);

    void RemoveFromCurrentPlayerLoop(SystemGroup systemGroup);
}