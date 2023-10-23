using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.DisconnectedDependencies;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class DDGroup1
{
    
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(DDGroup1))]
public partial class DDGroup2
{
    
}
