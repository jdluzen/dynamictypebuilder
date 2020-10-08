using System;

namespace DZen.DynamicTypeBuilder
{
    public interface IInterfaceBuilder
    {
        Type Implement<T>(string name = default);
    }
}
