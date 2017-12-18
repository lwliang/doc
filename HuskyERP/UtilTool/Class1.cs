using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTool
{
    public class ModelHelp
    {
        public static string ModelNameToTableName(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
                return string.Empty;
            return modelName.Replace(',', '_');
        }
    }
}
