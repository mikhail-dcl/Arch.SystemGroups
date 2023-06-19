using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.ThrottleGroup;

[UpdateInGroup(typeof(PostRenderingSystemGroup))]
public partial class ThrottlePostRenderingGroup : ThrottleGroupBase
{
    
}