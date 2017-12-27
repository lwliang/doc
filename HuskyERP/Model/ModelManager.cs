using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataBase;
using Model.Field;
using UtilTool;

namespace Model
{
    public class ModelManager
    {
        private static ModelManager _instance = null;
        public static ModelManager Instance => _instance ?? (_instance = new ModelManager());
        private ModelDic _modelDic = null;
        public ModelDic ModelDic => _modelDic ?? (_modelDic = new ModelDic());

        protected ModelManager()
        {

        }

        public void Register(ModelBase modelBase)
        {
            ModelDic[modelBase.ModelName] = modelBase.GetType();
        }
        public void UpLoad(ModelBase modelBase)
        {
            ModelDic.Remove(modelBase.ModelName);
        }

        public IList<T> Search<T>()
        {
            throw new NotImplementedException();
        }
        public bool Insert(ModelBase model)
        {
            throw new NotImplementedException();
        }

        public bool Update(ModelBase model)
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>(int id)
        {
            throw new NotImplementedException();
        }


        public bool UpgradeDataBase(string dataBaseName)
        {
            if (!IsExistDataBase(dataBaseName)) return false;
            DataBaseManager.Instance.DataBaseName = dataBaseName;
            UpgradeAllTables();
            return true;
        }

        public bool IsExistDataBase(string databaseName)
        {
            var sql = string.Format($"SELECT COUNT(*) FROM	master.dbo.sysdatabases WHERE name = '{databaseName}'");
            var sqlAccess =
               SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                   DataBaseManager.Instance.ConnectString);
            var count = SqlTyepConvert.ConvertToInt(sqlAccess.ExecuteScalar(sql));
            return count > 0;
        }

        public bool CreateDataBase(string databaseName)
        {
            var sqlAccess =
                SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                    DataBaseManager.Instance.ConnectString);
            var sql = string.Format($"CREATE DATABASE {databaseName}");
            try
            {
                //sqlAccess.BeginTransaction();
                sqlAccess.ExecuteNonQuery(sql);
                //sqlAccess.Commit();
                DataBaseManager.Instance.DataBaseName = databaseName;
                CreateAllTables();
                return true;
            }
            catch
            {
                //sqlAccess.Rollback();
                throw;
            }
        }

        private void CreateAllTables()
        {
            var sqlAccess =
                SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                    DataBaseManager.Instance.ConnectString);
            foreach (var keypair in ModelDic)
            {
                try
                {
                    var model = UtilTool.ReflectorHelp.CreateInstance(keypair.Value) as ModelBase;
                    if (model == null) continue;
                    var sql = CreateTable(model.TableName);
                    sqlAccess.ExecuteNonQuery(sql);
                    foreach (var column in model.GetColumnsField())
                    {
                        dynamic sfield = column;

                        sql = CreateColumn(model.TableName, sfield.FieldName,
                        sfield.Type, sfield.Size, sfield.Precision);

                        sqlAccess.ExecuteNonQuery(sql);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        public void UpgradeAllTables()
        {
            var sqlAccess =
               SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                   DataBaseManager.Instance.ConnectString);
            foreach (var keypair in ModelDic)
            {
                try
                {
                    var model = UtilTool.ReflectorHelp.CreateInstance(keypair.Value) as ModelBase;
                    if (model == null) continue;
                    var sql = IsExistTable(model.TableName);
                    if (SqlTyepConvert.ConvertToInt(sqlAccess.ExecuteScalar(sql)) == 0)
                    {//不存在
                        sql = CreateTable(model.TableName);
                        sqlAccess.ExecuteNonQuery(sql);
                    }
                    foreach (var column in model.GetColumnsField())
                    {
                        dynamic sfield = column;
                        sql = IsExistColumn(model.TableName, sfield.FieldName);
                        if (SqlTyepConvert.ConvertToInt(sqlAccess.ExecuteScalar(sql)) > 0)
                        {
                            sql = UpgradeColumn(model.TableName, sfield.FieldName,
                            sfield.Type, sfield.Size, sfield.Precision);
                        }
                        else
                        {
                            sql = CreateColumn(model.TableName, sfield.FieldName,
                            sfield.Type, sfield.Size, sfield.Precision);
                        }
                        sqlAccess.ExecuteNonQuery(sql);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public string CreateTable(string tableName)
        {
            return string.Format($"CREATE TABLE {tableName}(Id int)");
        }

        public string IsExistTable(string tableName)
        {
            return string.Format($"SELECT COUNT(*) FROM sys.tables WHERE name = '{tableName}'");
        }

        public string IsExistColumn(string tableName, string columnName)
        {
            return string.Format($"SELECT COUNT(*) FROM sys.columns WHERE name = '{columnName}' " +
                $"AND object_id = OBJECT_ID('{tableName}')");
        }

        public string CreateColumn(string tableName, string columnName, Type type, int size, int pricesion)
        {
            var sql = string.Format($"ALTER TABLE {tableName} " +
                $"ADD {columnName} ");

            var sqltype = "INT";
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    sqltype = "INT";
                    break;
                case TypeCode.String:
                    sqltype = string.Format($"NVARCHAR({size})");
                    break;
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    sqltype = string.Format($"DECIMAL({size},{pricesion})");
                    break;
            }
            sql += sqltype;
            return sql;
        }

        public string UpgradeColumn(string tableName, string columnName, Type type, int size, int pricesion)
        {
            var sql = string.Format($"ALTER TABLE {tableName} " +
                 $"ALTER COLUMN {columnName} ");

            var sqltype = "INT";
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    sqltype = "INT";
                    break;
                case TypeCode.String:
                    sqltype = string.Format($"NVARCHAR({size})");
                    break;
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    sqltype = string.Format($"DECIMAL({size},{pricesion})");
                    break;
            }
            sql += sqltype;
            return sql;
        }
    }
}
