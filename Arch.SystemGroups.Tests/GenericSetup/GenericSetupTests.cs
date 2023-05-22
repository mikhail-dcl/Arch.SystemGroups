using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;
using Arch.SystemGroups.Tests.TestSetup1.Systems;
using NSubstitute;

namespace Arch.SystemGroups.Tests.GenericSetup;

public class GenericSetupTests
{
    public struct CustomStruct1
    {
    }
    
    public struct CustomEquatableStruct : IEquatable<CustomEquatableStruct>
    {
        public int A;

        public bool Equals(CustomEquatableStruct other)
        {
            return A == other.A;
        }

        public override bool Equals(object? obj)
        {
            return obj is CustomEquatableStruct other && Equals(other);
        }

        public override int GetHashCode()
        {
            return A;
        }
    }
    
    public enum CustomEnum1
    {
        A, B, C, D
    }
    
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    private IUnityPlayerLoopHelper? _loopHelper;
    
    private GenericSystem1<int> _genericSystem1Int;
    private GenericSystem1<string> _genericSystem1String;
    
    private GenericSystem2<CustomStruct1> _genericSystem2CustomStruct1;
    private GenericSystem2<float> _genericSystem2Float;
    private GenericSystem2<double> _genericSystem2Double;
    
    private GenericSystem3<object, CustomEquatableStruct, CustomEnum1> _genericSystem3ObjectCustomStruct1TestEnum;

    [SetUp]
    public void SetUp()
    {
        _worldBuilder = new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(), _loopHelper = Substitute.For<IUnityPlayerLoopHelper>());
        
        _genericSystem1Int = GenericSystem1<int>.InjectToWorld(ref _worldBuilder, 1);
        _genericSystem1String = GenericSystem1<string>.InjectToWorld(ref _worldBuilder, "test");
        
        _genericSystem2CustomStruct1 = GenericSystem2<CustomStruct1>.InjectToWorld(ref _worldBuilder, new CustomStruct1());
        _genericSystem2Float = GenericSystem2<float>.InjectToWorld(ref _worldBuilder, 1.0f);
        _genericSystem2Double = GenericSystem2<double>.InjectToWorld(ref _worldBuilder, 1.0);
        
        _genericSystem3ObjectCustomStruct1TestEnum = GenericSystem3<object, CustomEquatableStruct, CustomEnum1>.InjectToWorld(ref _worldBuilder, new object(), new CustomEquatableStruct {A = 100}, CustomEnum1.B);
    }

    [Test]
    public void BuildsHierarchy()
    {
        var world = _worldBuilder.Finish();
        
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<InitializationSystemGroup, Group1OfGenerics, Group2OfGenerics>(world);

        var init = world.SystemGroups.OfType<InitializationSystemGroup>().First();
        var group1 = init.Systems.OfType<Group1OfGenerics>().First();
        var group2 = init.Systems.OfType<Group2OfGenerics>().First();
        
        CollectionAssert.AreEquivalent(new List<ISystem<float>>
        {
            _genericSystem3ObjectCustomStruct1TestEnum
        }, group1.Systems);
        
        CollectionAssert.AreEquivalent(new List<ISystem<float>>
        {
            _genericSystem1Int,
            _genericSystem2Float,
            _genericSystem2Double,
            _genericSystem1String,
            _genericSystem2CustomStruct1
        }, group2.Systems);
    }
}