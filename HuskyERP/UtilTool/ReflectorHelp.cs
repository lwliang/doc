using System;

namespace UtilTool
{
    public class ReflectorHelp
    {
        public static object  CreateInstance(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            return Activator.CreateInstance(type);
        }
    }
}
