using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;
using NSubstitute;

namespace Arch.SystemGroups.Tests.ExceptionsHandling;

public class ExceptionsHandlingTests
{
    private ThrowingSystem1 _throwingSystem1;

    private ISystemGroupExceptionHandler _exceptionHandler;
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    private IUnityPlayerLoopHelper _loopHelper;
    private SystemGroupWorld _world;
    private SystemGroup _systemGroup;

    [SetUp]
    public void SetUp()
    {
        _exceptionHandler = Substitute.For<ISystemGroupExceptionHandler>();
        
        _worldBuilder =
            new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(),
                _loopHelper = Substitute.For<IUnityPlayerLoopHelper>(), exceptionHandler: _exceptionHandler);
        
        _throwingSystem1 = ThrowingSystem1.InjectToWorld(ref _worldBuilder);
        _world = _worldBuilder.Finish();

        _systemGroup = _world.SystemGroups.OfType<SimulationSystemGroup>().First();
        _systemGroup.Initialize();
    }

    [Test]
    public void KeepsSystemsRunning()
    {
        _exceptionHandler.Handle(Arg.Any<Exception>(), Arg.Any<Type>())
            .Returns(_ => ISystemGroupExceptionHandler.Action.Continue);
        
        Assert.That(_systemGroup.Update, Throws.Nothing);
    }

    [Test]
    public void Rethrows()
    {
        _exceptionHandler.Handle(Arg.Any<Exception>(), Arg.Any<Type>())
            .Returns(c => throw c.Arg<Exception>());
        
        Assert.That(_systemGroup.Update, Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void Suspends()
    {
        _exceptionHandler.Handle(Arg.Any<Exception>(), Arg.Any<Type>())
            .Returns(_ => ISystemGroupExceptionHandler.Action.Suspend);
        
        _systemGroup.Update();
        Assert.That(_systemGroup.CurrentState, Is.EqualTo(SystemGroup.State.Suspended));
    }
    
    [Test]
    public void Disposes()
    {
        _exceptionHandler.Handle(Arg.Any<Exception>(), Arg.Any<Type>())
            .Returns(_ => ISystemGroupExceptionHandler.Action.Dispose);
        
        _systemGroup.Update();
        Assert.That(_systemGroup.CurrentState, Is.EqualTo(SystemGroup.State.Disposed));
    }
}