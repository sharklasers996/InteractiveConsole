using System;
using System.Collections.Generic;

namespace InteractiveConsole
{
    public interface ITypeProvider
    {
        List<Type> GetTypes();
    }
}