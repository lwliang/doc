using Model.Field;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        /// 表名
        /// </summary>
        private string _tableName;
        public int Id { get; protected set; }
        public string TableName
        {
            get => !string.IsNullOrEmpty(_tableName) ?
                _tableName :
                ModelHelp.ModelNameToTableName(ModelName);
            set => _tableName = value;
        }
        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }
        public bool IsAbstract { get; protected set; }
        public bool IsTransient { get; protected set; }

        public virtual IList<Object> GetColumnsField()
        {
            var type = this.GetType();

            var propertys = type.GetProperties();

            IList<Object> columns = new List<Object>();

            foreach (var column in propertys)
            {
                if (column.PropertyType.Name == typeof(IntegerField).Name
                    || column.PropertyType.Name == typeof(StringField).Name
                    || column.PropertyType.Name == typeof(DecimalField).Name
                    || column.PropertyType.Name == typeof(One2Many).Name
                    || column.PropertyType.Name == typeof(Many2Many).Name
                    || column.PropertyType.Name == typeof(Many2One).Name)
                {
                    columns.Add(column.GetValue(this));
                }
            }
            return columns;
        }
        public virtual IList<Object> GetModifColumnsField()
        {
            var type = this.GetType();

            var propertys = type.GetProperties();

            IList<Object> columns = new List<Object>();

            foreach (var column in propertys)
            {
                if (column.PropertyType.Name == typeof(IntegerField).Name
                    || column.PropertyType.Name == typeof(StringField).Name
                    || column.PropertyType.Name == typeof(DecimalField).Name
                    || column.PropertyType.Name == typeof(One2Many).Name
                    || column.PropertyType.Name == typeof(Many2Many).Name
                    || column.PropertyType.Name == typeof(Many2One).Name)
                {
                    dynamic value = column.GetValue(this);
                    if (!value.IsModify) continue;
                    columns.Add(column.GetValue(this));
                }
            }
            return columns;
        }
        public void Save()
        {
            foreach (dynamic col in GetColumnsField())
            {
                col.Save();
            }
        }
    }
}
