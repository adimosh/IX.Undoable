using IX.StandardExtensions;
using System;
using System.Reflection;

namespace IX.Undoable.Aides
{
    internal static class ItemCloningAide
    {
        internal static Func<TItem, TItem> GenerateCloningFunction<TItem>()
        {
            TypeInfo ti = typeof(TItem).GetTypeInfo();

            // IDeepCloneable is preferred, since it is the best at separating state across cloning
            if (typeof(IDeepCloneable<TItem>).GetTypeInfo().IsAssignableFrom(ti))
            {
                return (item) => ((IDeepCloneable<TItem>)item).DeepClone();
            }

            // IShallowCloneable is then used, since this achieves the same effect as a value type but in a controlled manner
            if (typeof(IShallowCloneable<TItem>).GetTypeInfo().IsAssignableFrom(ti))
            {
                return (item) => ((IShallowCloneable<TItem>)item).ShallowClone();
            }

            // Value types are next, since they will basically clone themselves
            if (ti.IsValueType)
            {
                return (item) => item;
            }

            // Two methods are ignored: ICloneable and the cloning constructor.
            // ICloneable is not recommended by MSDN itself
            // The cloning constructor is too widely misunderstood and misused ti be reliable

            // The next is to return nothing, since we don't know how to clone this stuff
            return null;
        }
    }
}