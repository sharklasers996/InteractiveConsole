using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InteractiveConsole
{
    public class TypeProvider : ITypeProvider
    {
        public List<Type> GetTypes()
        {
            var types = Assembly.GetEntryAssembly().GetTypes().ToList();
            types.AddRange(Assembly.GetCallingAssembly().GetTypes());

            return types;
        }
    }
}