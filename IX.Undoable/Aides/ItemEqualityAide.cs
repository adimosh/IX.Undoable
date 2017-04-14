using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace IX.Undoable.Aides
{
    internal static class ItemEqualityAide
    {
        internal static Func<TItem, TItem, bool> GenerateEqualityFunction<TItem>()
        {
            TypeInfo ti = typeof(TItem).GetTypeInfo();

            // IEquatable is the de facto implementation to check whether two instances are state-wise equal
            if (typeof(IEquatable<TItem>).GetTypeInfo().IsAssignableFrom(ti))
            {
                return (left, right) => ((IEquatable<TItem>)left).Equals(right);
            }

            return (left, right) => left.Equals(right);
        }
    }
}
