using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Field
{
    public class IntegerField : FieldBase<int>
    {
        public IntegerField(ModelBase model, string fieldName, bool isStore = true)
        {
            Model = model;
            FieldName = fieldName;
            IsStore = isStore;
        }
    }
}
