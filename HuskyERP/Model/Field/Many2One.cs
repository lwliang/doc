using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Field
{
    public class Many2One : FieldBase<ModelBase>
    {
        private string _comodelName;
        public string CoModelName
        {
            get { return _comodelName; }
        }
        public Type ModelType
        {
            get
            {
                return DataBase.DataBaseManager.Instance.ModelManager.ModelDic[_comodelName];
            }
        }
        public Many2One(ModelBase model,
            string fieldName, string comodelName) : base()
        {
            Model = model;
            FieldName = fieldName;
            _comodelName = comodelName;
        }

        private ModelBase _value;
        public override ModelBase Value
        {
            get
            {
                if (_value == null)
                {

                }
                return _value;
            }
            set { _value = value; }
        }
    }
}
