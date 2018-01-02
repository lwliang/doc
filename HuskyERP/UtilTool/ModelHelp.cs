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
            return modelName.Replace('.', '_');
        }

        public static string ModelRelationTableName(string model1, string model2)
        {
            return string.Format($"{ModelNameToTableName(model1)}_{ModelNameToTableName(model2)}_ref");
        }

    }
}
