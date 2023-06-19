using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.ThrottleGroup;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class ParametrisedThrottleGroup : ThrottleGroupBase
{
    public ParametrisedThrottleGroup(int framesToSkip) : base(framesToSkip)
    {
    }
}