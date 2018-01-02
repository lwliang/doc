using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Field
{
    public class One2Many : FieldBase<IList<ModelBase>>
    {
        private string _comodelName;
        public Type ModelType
        {
            get
            {
                return DataBase.DataBaseManager.Instance.ModelManager.ModelDic[_comodelName];
            }
        }
        private string _inverseName;
        public string InverseName
        {
            get { return InverseName; }
            protected set { _inverseName = value; }
        }
        public One2Many(ModelBase model,
            string fieldName, string comodelName, string inverseName) : base()
        {
            Model = model;
            FieldName = fieldName;
            _comodelName = comodelName;
            InverseName = inverseName;
            IsStore = false;
        }

        private IList<ModelBase> _value;
        public override IList<ModelBase> Value
        {
            get
            {
                if (_value == null)
                    _value = new List<ModelBase>();
                return _value;
            }
        }
    }
}
