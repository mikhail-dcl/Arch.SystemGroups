using Arch.SystemGroups.Tests.TestSetup1;
using NSubstitute;

namespace Arch.SystemGroups.Tests.DisconnectedDependencies;

public class DisconnectedDependenciesTests
{
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    
    [SetUp]
    public void SetUp()
    {
        _worldBuilder = new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(), Substitute.For<IPlayerLoop>());
    }
    
    [Test]
    public void ThrowsOnSystems()
    {
        DDSystem1.InjectToWorld(ref _worldBuilder);
        DDSystem2.InjectToWorld(ref _worldBuilder);
        
        Assert.Throws<DisconnectedDependenciesFoundException>(() => _worldBuilder.Finish());
    }

    [Test]
    public void ThrowsOnGroups()
    {
        DDSystem1Gr1.InjectToWorld(ref _worldBuilder);
        DDSystem1Gr2.InjectToWorld(ref _worldBuilder);
        
        Assert.Throws<DisconnectedDependenciesFoundException>(() => _worldBuilder.Finish());
    }
}