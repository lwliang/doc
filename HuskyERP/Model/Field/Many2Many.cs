using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilTool;

namespace Model.Field
{
    public class Many2Many : FieldBase<IList<ModelBase>>
    {
        private string _tableName;
        public string RelationTableName
        {
            get
            {
                if (!string.IsNullOrEmpty(_tableName))
                    return _tableName;
                return ModelHelp.ModelRelationTableName(Model.ModelName, _comodelName); ;
            }
            protected set
            {
                _tableName = value;
            }
        }
        public string _columnName1;
        public string ColumnName1
        {
            get
            {
                if (!string.IsNullOrEmpty(_columnName1))
                    return _columnName1;
                return ModelHelp.ModelNameToTableName(_comodelName) + "_id";
            }
            protected set
            {
                _columnName1 = value;
            }
        }
        private string _columnName;
        public override string ColumnName
        {
            get
            {
                if (!string.IsNullOrEmpty(_columnName))
                    return _columnName;
                return ModelHelp.ModelNameToTableName(Model.ModelName) + "_id";
            }
            protected set { _columnName = value; }
        }
        private string _comodelName;
        public Type ModelType
        {
            get
            {
                return DataBase.DataBaseManager.Instance.ModelManager.ModelDic[_comodelName];
            }
        }
        public Many2Many(ModelBase model,
            string fieldName, string comodelName) : base()
        {
            Model = model;
            FieldName = fieldName;
            _comodelName = comodelName;
        }

        private IList<ModelBase> _value;
        public override IList<ModelBase> Value
        {
            get
            {
                if (_value == null)
                {
                    _value = new List<ModelBase>();
                }
                return _value;
            }
        }
    }
}
