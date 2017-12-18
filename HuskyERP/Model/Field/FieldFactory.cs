using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Field
{
    public class FieldFactory
    {
        public static IntegerField CreateIntegerField(
            ModelBase model, string fieldName, bool isStore = true)
        {
            return new IntegerField(model, fieldName, isStore);
        }

        public static StringField CreateStringField(
            ModelBase model, string fieldName, int size, bool isStore = true)
        {
            return new StringField(model, fieldName, size, isStore);
        }

        public static DecimalField CreateDecimalField(
           ModelBase model, string fieldName, int size, int precision, bool isStore = true)
        {
            return new DecimalField(model, fieldName, size, precision, isStore);
        }
    }
}
