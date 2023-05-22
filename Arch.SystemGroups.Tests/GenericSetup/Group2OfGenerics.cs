using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.GenericSetup;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(Group1OfGenerics))]
public partial class Group2OfGenerics {}