# Arch.SystemGroups

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg?style=for-the-badge)](https://opensource.org/licenses/Apache-2.0)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)


The project provides extensions for the [Arch.System](https://github.com/genaray/Arch.Extended) tailored to the Unity Player Loop.
It is inspired by [Unity Entities' System Groups](https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/systems-update-order.html)


## Installation

### Prerequisites
- Unity 2022.3+ is required
- The project targets netstandard2.0/2.1

### SourceGenerator Installation

The SourceGenerator is required for automatic code generation. Choose one of the installation methods below:

#### Option 1: From GitHub Releases (Recommended for Production)

**For Unity Package Manager (UPM):**

Add the SourceGenerator package URL to your Unity project's `manifest.json`:

```json
{
  "dependencies": {
    "com.arch.systemgroups.sourcegenerator": "https://github.com/mikhail-dcl/Arch.SystemGroups/releases/download/v1.0.0/com.arch.systemgroups.sourcegenerator-1.0.0.tgz"
  }
}
```

Replace `v1.0.0` with the desired release version.

**Manual Installation:**

1. Download `com.arch.systemgroups.sourcegenerator-{version}.tgz` from [Releases](https://github.com/mikhail-dcl/Arch.SystemGroups/releases)
2. In Unity, go to **Window → Package Manager**
3. Click **+ → Add package from tarball...**
4. Select the downloaded `.tgz` file

#### Option 2: From Pull Request Artifacts (For Testing)

1. Navigate to the desired PR on GitHub
2. Find the PR comment with the SourceGenerator artifact link
3. Download and extract the artifact (single extraction)
4. You'll get:
   - `Arch.SystemGroups.SourceGenerator.dll`
   - `Arch.SystemGroups.SourceGenerator.dll.meta` (pre-configured as RoslynAnalyzer)
5. Copy both files to your Unity project (e.g., `Assets/Plugins/RoslynAnalyzers/`)
6. Unity will automatically detect it as a Roslyn Analyzer

#### Option 3: Manual Build

1. Clone this repository
2. Build the project in Release configuration:
   ```bash
   dotnet build Arch.SystemGroups.SourceGenerator/Arch.SystemGroups.SourceGenerator.csproj --configuration Release
   ```
3. Copy `Arch.SystemGroups.SourceGenerator.dll` from `bin/Release/netstandard2.0/` to your Unity project
4. In Unity, select the DLL and configure it as a RoslynAnalyzer:
   - Exclude all platforms in the Inspector
   - Add the label `RoslynAnalyzer` (see [Unity docs](https://docs.unity3d.com/Manual/roslyn-analyzers.html))

### Runtime Library Installation

- Copy `Arch.SystemGroups.dll` into the `Plugins` directory of your Unity project, or
- Install via Unity Package Manager using the main package: `com.arch.systemgroups`

## Update In Group

Use the `UpdateInGroup` attribute on the member systems to specify which systems need to be updated in a given group.
When the attribute is specified for class that implements `Arch.System.ISystem<float>` a partial class for the given system is generated.
It contains methods to add a system to the `World Builder` with respect to its update order.

> Note: Only systems that implement `Arch.System.ISystem<float>` are supported as the `float` denotes Delta Time from the Unity Player Loop.

`UpdateInGroup` accepts as a constructor argument a type of the system group or a custom group.

### System Groups

By default `Arch` Groups and Systems contain only `BeforeUpdate`, `AfterUpdate` and `Update` methods.
It's quite limiting and not aligned well with the [Unity Player Loop](https://docs.unity3d.com/ScriptReference/LowLevel.PlayerLoopSystem.html).

System groups extend this capability and provide a predefined set of groups that are bound to a specific moment in the Unity Player Loop.
Every custom group and system in order to get updated must be a child of one of the system groups directly or transitively.

#### Initialization System Group

Updates at the end of the Initialization phase of the player loop. `Time.deltaTime` is passed as an argument.

```csharp
[UpdateInGroup(typeof(InitializationSystemGroup))]
```

#### Simulation System Group

Updates at the end of the Update phase of the player loop. `Time.deltaTime` is passed as an argument.

You would normally use this group as an alternative to the `Update` method of the `MonoBehaviour` class.

```csharp
[UpdateInGroup(typeof(SimulationSystemGroup))]
```

#### Presentation System Group

Updates at the end of the PreLateUpdate phase of the player loop. `Time.deltaTime` is passed as an argument.

You would normally use this group as an alternative to the `LateUpdate` method of the `MonoBehaviour` class.

```csharp
[UpdateInGroup(typeof(PresentationSystemGroup))]
```

#### Post Rendering System Group

Updates at the end of the PostLateUpdate phase of the player loop (after Rendering). `Time.deltaTime` is passed as an argument.

```csharp
[UpdateInGroup(typeof(PostRenderingSystemGroup))]
```

#### Physics System Group

Updates at the beginning of the FixedUpdate phase of the player loop before all fixed updates. `Time.fixedDeltaTime` is passed as an argument.

You would normally use this group as an alternative to the `FixedUpdate` method of the `MonoBehaviour` class (e.g. to assign `Velocity` to the objects that move in the current frame).

```csharp
[UpdateInGroup(typeof(PhysicsSystemGroup))]
```

#### Post Physics System Group

Updates at the end of the FixedUpdate phase of the player loop.

```csharp
[UpdateInGroup(typeof(PostPhysicsSystemGroup))]
```

### Custom Groups
Groups are created automatically on [systems creation](#how-to-instantiate-systems). The method `TryCreateGroup<T>(ref ArchSystemsWorldBuilder<T> worldBuilder)` will be generated but you can just ignore it as it should be called from other generated code only.
#### Default Behaviour
To create a custom group declare an empty `partial` class and annotate it with `UpdateInGroup` attribute. The logic needed for ordering and assigning systems to the group will be autogenerated.

```csharp
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class CustomGroup1
{
    
}
```

#### Custom Behaviour
In some cases it can be useful to provide custom behavior for groups. For example, you might want to create a group that runs at a reduced frequency.

To do so instead of creating an empty `partial` class, create a class that inherits from `Arch.SystemGroups.CustomGroupBase` and annotate it with `UpdateInGroup` attribute.
If the only constructor it has is an empty one then this group will be instantiated automatically.

```csharp
/// <summary>
/// Skips every other update
/// </summary>
public class ThrottleGroupBase : CustomGroupBase<float>
{
    private bool open;
    
    public override void Dispose()
    {
        DisposeInternal();
    }

    public override void Initialize()
    {
        InitializeInternal();
    }

    public override void BeforeUpdate(in float t)
    {
        // Before Update is always called first in the same frame
        open = !open;
        
        if (open)
            BeforeUpdateInternal(in t);
    }

    public override void Update(in float t)
    {
        if (open)
            UpdateInternal(in t);
    }

    public override void AfterUpdate(in float t)
    {
        if (open)
            AfterUpdateInternal(in t);
    }
}
```

You may want to customize groups behaviour even further by providing a custom constructor.
In this case the instantiated group should be passed manually by calling `InjectCustomGroup` before injecting any other systems or groups dependent on it.

```csharp
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class ParametrisedThrottleGroup : ThrottleGroupBase
{
    public ParametrisedThrottleGroup(int framesToSkip) : base(framesToSkip)
    {
    }
}

_worldBuilder.InjectCustomGroup(new ParametrisedThrottleGroup(framesToSkip));
// then inject all systems
```
> Note: If a system is injected before the custom group it is included into directly or transitively an exception of type `GroupNotFoundException` will be thrown.

> Note: If a group does not belong to a `System Group` then it is detached from the Player Loop and its system won't be updated

## Update Order

Systems update order is controlled by `UpdateAfter` and `UpdateBefore` attributes.
- Both systems and groups can be annotated with these attributes.
- Only systems and groups annotated by `UpdateInGroup` are taken into consideration
- `UpdateAfter` and `UpdateBefore` can't contain an open generic type (e.g. `typeof(CustomSystem<>)`). If you have such need, create a custom group and annotate the system with `UpdateInGroup` attribute.
- It is possible to have several of them
- it is possible to place redundant attributes, they will be properly resolved/ignored
- As an argument attributes accept the system or group type
- **Depth first search** is used to sort systems; [System Groups](#system-groups) act as root nodes.
- Sorting happens only once on [Systems Instantiation](#how-to-instantiate-systems)


## How to instantiate systems
In order to bind systems to the player loop, distribute them in groups and sort accordingly, one must use auto-generated API. 
The API is generated for non-abstract generic and non-generic systems that implement `Arch.System.ISystem<float>`.

1. Instantiate `ArchSystemsWorldBuilder` with a desired type of `World`. With `Arch` you are most probably using `Arch.Core.World`

    ```csharp
    var worldBuilder = new ArchSystemsWorldBuilder<World>(World.Create());
    ```

   The system must have a constructor with a first argument of the World type.

    ```csharp
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(CustomGroup1))]
    public partial class CustomSystem1 : BaseSystem<World, float>
    {
        public CustomSystem1(World world) : base(world)
        {
        }
    }
    ```

2. Add systems to the builder. There are multiple ways of doing so:
   - Use a static `Factory Method` `InjectToWorld` of the system and pass `worldBuilder` as `ref`     
      ```csharp
      CustomSystem1.InjectToWorld(ref worldBuilder);
      ```
     If the system has arguments pass the corresponding arguments as well
      ```csharp
        [UpdateInGroup(typeof(InitializationSystemGroup))]
        public partial class CustomSystemWithParameters1 : BaseSystem<TestWorld, float>
        {
            private readonly string _param1;
            private readonly int _param2;
     
            public CustomSystemWithParameters1(TestWorld world, string param1, int param2) : base(world)
            {
                 _param1 = param1;
                 _param2 = param2;
            }
        }
      ```

      ```csharp
      CustomSystemWithParameters1.InjectToWorld(ref worldBuilder, "test", 1);
      ```
   - Invoke Extensions. For every system an extension method is generated `Add{SystemName}({Arguments})`. If you rename the system you will have to modify the code accordingly manually.
      ```csharp
      worldBuilder.AddCustomSystemWithParameters1("test", 1)
      ```
   - Bulk creation. If you have many systems sharing the same constructor's signature using a bulk instantiation may be particularly beneficial
      
     Instead of writing something like
      ```csharp
     worldBuilder
            .AddSystemCGroupAA()
            .AddSystemCGroupAB()
            .AddSystemAGroupAA()
            .AddSystemAGroupAB()
            .AddSystemBGroupAA()
            .AddSystemBGroupAB()

            .AddSystemDGroupBA()
            .AddSystemCGroupBA()
            .AddSystemCGroupBAA()
            .AddSystemBGroupBA()
            .AddSystemBGroupBB()
            .AddSystemBGroupBAA()
            .AddSystemAGroupBA()
            .AddSystemAGroupBB()
            .AddSystemAGroupBAA()
            .AddSystemAGroupBAB();
      ```
     you can simply write an equivalent that will inject all systems (here all the systems are without any arguments) at once: 
      ```csharp
      worldBuilder.AddAllSystems();
      ```
     For every arguments set a separate extension is generated so you can chain them like this:
     ```csharp
      worldBuilder
          .AddAllSystems(new CustomClass1())
          .AddAllSystems("test", 1)
          .AddAllSystems(1.0, (f, i) => { })
     ```

     > For generic systems such extensions are not generated. You will have to use the `Factory Method` or `AddSystem` extension
3. Add as many systems as needed
4. Call `var groupWorld = worldBuilder.Finish()` to create all groups and systems, and sort them
5. When the world creation is finalized, system groups are added to the `Aggregate` which in turn is attached to the Player Loop.
   
   By default it is assumed that the order in which system groups within the same player loop stage are executed is irrelevant.
   However, it might be beneficial to customize it: e.g. in case there is a reliance on the execution order or the worlds have different priority.
   
   It's achieved by passing an implementation of `ISystemGroupAggregate<T>.IFactory` along the data unique for the given world to the `Finish<TAggregationData>(ISystemGroupAggregate<TAggregationData>.IFactory aggregateFactory, TAggregationData aggregationData)` method. 
 
   You can take a look at `OrderedSystemGroupAggregate<T>` as a reference. it uses `T` and `IComparer<T>` to sort system groups being added to and removed from the aggregate according to data passed on worlds creation.
    
6. Call `groupWorld.Initialize()` to recursively initialize systems, it will be called in accordance to `Update Order`
7. From this point all your systems are attached to the Unity Player Loop
8. Once the `World` should be disposed, call `groupWorld.Dispose()` to detach systems from the Player Loop

## Centralized Throttling
In order to minimize CPU impact it might be beneficial to introduce throttling on the [System Groups](#system-groups) level unlike the possibility to do it in `CustomGroups` the logic of which is executed individually per group.
There are two contracts for that: `IUpdateBasedSystemGroupThrottler` is responsible for systems that are executed with Unity Player Loop Update frequency, and `IFixedUpdateBasedSystemGroupThrottler` for systems that are executed with Unity Player Loop FixedUpdate frequency. 

Just provide them as arguments to the `ArchSystemsWorldBuilder` constructor and they will be executed only once for each [Root System Groups](#system-groups).
if `ShouldThrottle` returns `true` then the whole graph of the system group is executed in a throttling mode.

Within the same dependency tree systems and groups may have a finely controlled possibility to throttle. It's achieved by annotating a class by the `ThrottlingEnabled` attribute. Thus, it is possible to tell systems in the same group to follow throttling (that is returned by `ShouldThrottle`) or ignore it.
If the group is annotated with this attribute its children will throttle no matter whether `ThrottlingEnabled` is specified for them or not. 

## Exceptions Handling
Similar to every other native callback (`Update`, `LateUpdate`, `Awake`, etc) Unity invokes `Player Loop` delegates as isolated calls. 
Thus, if an exception occurs it does not break the whole loop but the current step only. In terms of system groups it means that the whole root group will skip an execution frame starting from the exception onwards. 
It might be not exactly what a user expects when they introduce a systems pipeline.

In order to customize this behaviour it is possible to provide an implementation of `ISystemGroupExceptionHandler` to the `ArchSystemsWorldBuilder` constructor so it can tell the whole world what to do if an exception occurs: `Keep running`, `Suspend` or `Dispose`.

## Metadata
As the source generator in the project already operates with attributes it exposes such information to the user so it is possible get attributes data without reflection.
For every system and group a custom class inherited from `AttributesInfoBase` is generated.

It is possible to access it in two ways:
1. Use a strongly-typed static `Metadata` field. The instance is created lazily upon the first retrieval. So if it is not used no memory overhead will present.
2. If a system inherits from `PlayerLoopSystem<TWorld>` the overriden method `protected abstract AttributesInfoBase GetMetadataInternal();` will be generated providing the access to the `AttributesInfo` class instance.
From this point `T GetAttribute<T>()` and `IReadOnlyList<T> GetAttributes<T>()` methods are available. They don't rely on reflection either so the performance is similar to a simple `switch`/`if` expression.
