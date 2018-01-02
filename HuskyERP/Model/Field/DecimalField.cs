using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Field
{
    public class DecimalField : FieldBase<decimal>
    {
        public DecimalField(ModelBase model, 
            string fieldName, int size, int precision, bool isStore = true) : base()
        {
            Model = model;
            FieldName = fieldName;
            IsStore = isStore;
            Size = size;
            Precision = precision;
        }
    }
}
