using Arch.System;

namespace Arch.SystemGroups
{
    internal readonly struct ExecutionNode<T>
    {
        public readonly bool ThrottlingEnabled;
        public readonly bool IsGroup;
        public readonly ISystem<T> System;
        public readonly CustomGroupBase<T> CustomGroup;

        public ExecutionNode(ISystem<T> system, bool throttlingEnabled)
        {
            ThrottlingEnabled = throttlingEnabled;
            System = system;
            CustomGroup = null;
            IsGroup = false;
        }

        public ExecutionNode(CustomGroupBase<T> customGroup, bool throttlingEnabled)
        {
            IsGroup = true;
            ThrottlingEnabled = throttlingEnabled;
            CustomGroup = customGroup;
            System = null;
        }
    
        public void Initialize()
        {
            if (IsGroup)
            {
                CustomGroup.Initialize();
            }
            else
            {
                System.Initialize();
            }
        }
    
        public void Dispose()
        {
            if (IsGroup)
            {
                CustomGroup.Dispose();
            }
            else
            {
                System.Dispose();
            }
        }
    
        public void BeforeUpdate(in T t, bool throttle)
        {
            if (throttle && ThrottlingEnabled)
                return;
        
            if (IsGroup)
            {
                CustomGroup.BeforeUpdate(t, throttle);
            }
            else
            {
                System.BeforeUpdate(t);
            }
        }
    
        public void Update(in T t, bool throttle)
        {
            if (throttle && ThrottlingEnabled)
                return;
        
            if (IsGroup)
            {
                CustomGroup.Update(t, throttle);
            }
            else
            {
                System.Update(t);
            }
        }
    
        public void AfterUpdate(in T t, bool throttle)
        {
            if (throttle && ThrottlingEnabled)
                return;
        
            if (IsGroup)
            {
                CustomGroup.AfterUpdate(t, throttle);
            }
            else
            {
                System.AfterUpdate(t);
            }
        }
    }
}