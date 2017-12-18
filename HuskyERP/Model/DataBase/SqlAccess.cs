using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DataBase
{

    public interface ISqlAccess
    {
        void ExecuteReader(string sql);
        object ExecuteScalar(string sql);

        int ExecuteNonQuery(string sql);

        DataTable ExecuteDataTable(string sql);
        void BeginTransaction();
        void Rollback();
        void Commit();
    }

    public class SqlAccess : ISqlAccess
    {
        public string ConnectString { get; protected set; }
        public SqlAccess(string connStr)
        {

        }

        public void ExecuteReader(string sql)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string sql)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string sql)
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteDataTable(string sql)
        {
            throw new NotImplementedException();
        }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }
    }
}
