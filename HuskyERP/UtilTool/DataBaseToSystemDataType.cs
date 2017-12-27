using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilTool
{
    public static class SqlTyepConvert
    {
        public static int ConvertToInt(object sqlObj)
        {
            if (sqlObj == DBNull.Value)
                return 0;
            try
            {
                return Convert.ToInt32(sqlObj);
            }
            catch
            {
                return 0;
            }
        }
    }
}
