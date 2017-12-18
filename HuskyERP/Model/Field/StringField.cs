using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Field
{
    public class StringField : FieldBase<string>
    {
        public StringField(ModelBase model, string fieldName, int size, bool isStore = true)
        {
            Model = model;
            FieldName = fieldName;
            IsStore = isStore;
            Size = size;
        }
    }
}
