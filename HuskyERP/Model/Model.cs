using System;
using System.Collections.Generic;
using UtilTool;

namespace Model
{

    public class RealModel : ModelBase
    {
        public RealModel()
        {
            IsAbstract = false;
            IsTransient = false;
        }
    }

    public class ModelDic : Dictionary<string, Type>
    {

    }

    public class ModelList : List<ModelBase> { }

    public abstract class ModelBase
    {
        private static string _tableName;
        public int Id { get; protected set; }
        public static string TableName
        {
            get => !string.IsNullOrEmpty(_tableName) ?
                _tableName :
                ModelHelp.ModelNameToTableName(ModelName);
            set => _tableName = value;
        }
        public static string ModelName { get; set; }
        public bool IsAbstract { get; protected set; }
        public bool IsTransient { get; protected set; }

    }
}
