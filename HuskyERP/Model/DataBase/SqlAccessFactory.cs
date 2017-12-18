namespace Model.DataBase
{
    public enum SqlType
    {
        MsSql = 1,
        Oracel,
        MySql
    }

    public class SqlAccessFactory
    {
        public static ISqlAccess CreateSqlAccess(SqlType sqlType, string connStr)
        {
            switch (sqlType)
            {
                case SqlType.MsSql:
                    return new SqlAccess(connStr);
                default:
                    return new SqlAccess(connStr);
            }
        }
    }
}
