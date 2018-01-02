using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DataBase
{

    public interface ISqlAccess : IDisposable
    {
        SqlDataReader ExecuteReader(string sql);
        object ExecuteScalar(string sql);

        int ExecuteNonQuery(string sql);

        DataTable ExecuteDataTable(string sql);
        void BeginTransaction();
        void Rollback();
        void Commit();
    }

    public class SqlAccess : ISqlAccess, IDisposable
    {
        public string ConnectString { get; protected set; }
        private SqlConnection _conn;
        private SqlTransaction _tran;
        public SqlAccess(string connStr)
        {
            ConnectString = connStr;
            _conn = new SqlConnection(connStr);
        }

        public SqlDataReader ExecuteReader(string sql)
        {
            using (var sqlCmd = new SqlCommand())
            {
                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();
                sqlCmd.Connection = _conn;
                if (_tran != null) sqlCmd.Transaction = _tran;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = sql;
                return sqlCmd.ExecuteReader();
            }

        }

        public object ExecuteScalar(string sql)
        {
            using (var sqlCmd = new SqlCommand())
            {
                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();
                sqlCmd.Connection = _conn;
                if (_tran != null) sqlCmd.Transaction = _tran;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = sql;
                return sqlCmd.ExecuteScalar();
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            using (var sqlCmd = new SqlCommand())
            {
                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();
                sqlCmd.Connection = _conn;
                if (_tran != null) sqlCmd.Transaction = _tran;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = sql;
                return sqlCmd.ExecuteNonQuery();
            }
        }

        public DataTable ExecuteDataTable(string sql)
        {
            using (var sqlCmd = new SqlCommand())
            {
                if (_conn.State == ConnectionState.Closed)
                    _conn.Open();
                sqlCmd.Connection = _conn;
                if (_tran != null) sqlCmd.Transaction = _tran;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = sql;
                var SqlDataAdapter = new SqlDataAdapter();
                SqlDataAdapter.SelectCommand = sqlCmd;
                var dt = new DataTable();
                SqlDataAdapter.Fill(dt);
                dt.Dispose();
                return dt;
            }
        }

        public void BeginTransaction()
        {
            if (_conn.State == ConnectionState.Closed)
                _conn.Open();
            _tran = _conn.BeginTransaction();
        }

        public void Rollback()
        {
            if (_tran == null) return;
            _tran.Rollback();
        }

        public void Commit()
        {
            if (_tran == null) return;
            _tran.Commit();
        }

        public void Dispose()
        {
            if (_tran != null) _tran.Dispose();
            if (_conn != null) _conn.Dispose();
        }
    }
}
