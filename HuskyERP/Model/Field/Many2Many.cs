using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Field
{
    public class Many2Many : FieldBase<IList<ModelBase>>
    {
        private string _tableName;
        public string TableName
        {
            get
            {
                if (!string.IsNullOrEmpty(_tableName))
                    return _tableName;
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            protected set
            {
                _columnName1 = value;
            }
        }
        public string _columnName2;
        public string ColumnName2
        {
            get
            {
                if (!string.IsNullOrEmpty(_columnName2))
                    return _columnName2;
                throw new NotImplementedException();
            }
            protected set
            {
                _columnName2 = value;
            }
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
            string fieldName, string comodelName)
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
