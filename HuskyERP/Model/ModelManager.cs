using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

        public RealModel CreateInstance(string modelName)
        {
            if (!ModelDic.ContainsKey(modelName)) return null;
            return Activator.CreateInstance(ModelDic[modelName]) as RealModel;
        }

        public void UpLoad(ModelBase modelBase)
        {
            ModelDic.Remove(modelBase.ModelName);
        }

        public ModelBase Search(string modelName, int id)
        {
            var modelObj = CreateInstance(modelName);
            if (modelObj == null)
                throw new ArgumentException("获取实体表不存在");
            using (var sqlAccess =
               SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                   DataBaseManager.Instance.ConnectString))
            {
                var sql = CreateSelectSql(modelObj, id, false);
                var dt = sqlAccess.ExecuteDataTable(sql);

                if (dt.Rows.Count == 0) return null;
                var firstRow = dt.Rows[0];
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName == "Id")
                    {
                        SetProperty(modelObj, "Id", firstRow[col]);
                    }
                    else
                    {
                        var colPro = modelObj.GetType().GetProperty(col.ColumnName);
                        if (colPro.PropertyType == typeof(Many2One) ||
                            colPro.PropertyType == typeof(Many2Many) ||
                            colPro.PropertyType == typeof(One2Many))
                        {
                            if (firstRow[col] == DBNull.Value) continue;
                            if (colPro.PropertyType == typeof(Many2One))
                            {
                                dynamic m2o = colPro.GetValue(modelObj);

                            }
                            else if (colPro.PropertyType == typeof(One2Many))
                            {

                            }
                        }
                        else
                            SetColumnProperty(modelObj, col.ColumnName, firstRow[col]);
                    }
                }
            }
            return modelObj;
        }

        private void SetColumnProperty(object obj, string propertyName, object value)
        {
            var colPro = obj.GetType().GetProperty(propertyName);
            dynamic colObj = colPro.GetValue(obj);
            if (value == DBNull.Value || value == null)
            {
                SetProperty(colObj, "Value", colObj.Default);
            }
            else
                SetProperty(colObj, "Value", value);
        }

        private void SetProperty(object obj, string propertyName, object value)
        {
            var pro = obj.GetType().GetProperty(propertyName);

            pro.SetValue(obj, value);
        }

        private string CreateSelectSql(ModelBase model, int id, bool isAddm2o)
        {
            var cols = GetSelectColumn(model, isAddm2o);

            var sql = string.Format($"SELECT Id,{cols} " +
                $"FROM {model.TableName} " +
                $"WHERE Id = {id} ");
            return sql;
        }

        private string GetSelectColumn(ModelBase model, bool isAddm2o, string prefix = "")
        {
            var cols = new StringBuilder();
            foreach (var col in model.GetColumnsField())
            {
                dynamic sField = col;

                if (!sField.IsStore) continue;
                var columnName = string.Empty;
                if (sField is Many2Many ||
                    sField is Many2One ||
                    sField is One2Many)
                {
                    if (!isAddm2o) continue;
                    if (sField is Many2Many)
                    {
                    }
                    else
                        columnName = string.Format($"{sField.ColumnName} AS {sField.FieldName}");
                }
                else
                    columnName = string.Format($"{sField.ColumnName} AS {sField.FieldName}");

                if (string.IsNullOrEmpty(columnName)) continue;
                if (!string.IsNullOrEmpty(prefix))
                    columnName = string.Format($"{prefix}.{columnName}");
                if (cols.Length == 0)
                    cols.Append(columnName);
                else
                    cols.Append("," + columnName);
            }
            return cols.ToString();
        }


        public bool Insert(ModelBase model)
        {
            using (var sqlAccess =
               SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                   DataBaseManager.Instance.ConnectString))
            {
                var sql = CreateInsertSql(model);
                return sqlAccess.ExecuteNonQuery(sql) > 0;
            }
        }

        private string CreateInsertSql(ModelBase model)
        {
            var sql = $"INSERT INTO {model.TableName}({{0}}) VALUES({{1}})";
            var fields = new StringBuilder();
            var values = new StringBuilder();
            foreach (dynamic col in model.GetColumnsField())
            {
                if (col is Many2Many ||
                    col is Many2One ||
                    col is One2Many)
                    continue;
                if (fields.Length == 0)
                    fields.Append(col.ColumnName);
                else
                    fields.Append("," + col.ColumnName);
                if (values.Length == 0)
                    values.Append(col.ColumnValue);
                else
                    values.Append("," + col.ColumnValue);
            }

            return string.Format(sql, fields.ToString(), values.ToString());
        }

        public bool Update(ModelBase model)
        {
            if (model == null || model.Id <= 0) return false;
            using (var sqlAccess =
             SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                 DataBaseManager.Instance.ConnectString))
            {
                var sql = CreateUpdateSql(model);
                return sqlAccess.ExecuteNonQuery(sql) > 0;
            }
        }

        private string CreateUpdateSql(ModelBase model)
        {
            var cols = model.GetModifColumnsField();
            if (cols.Count == 0) return string.Empty;

            var sql = $"UPDATE {model.TableName} SET {{0}} WHERE Id = {model.Id}";
            var updates = new StringBuilder();
            foreach (dynamic col in cols)
            {
                if (col is Many2Many ||
                    col is Many2One ||
                    col is One2Many)
                    continue;
                if (updates.Length == 0)
                    updates.Append($"{col.ColumnName} = {col.ColumnValue}");
                else
                    updates.Append($",{col.ColumnName} = {col.ColumnValue}");
            }
            return string.Format(sql, updates.ToString());
        }

        public bool Delete(string modelName, int id)
        {
            var modelObj = CreateInstance(modelName);
            using (var sqlAccess =
              SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                  DataBaseManager.Instance.ConnectString))
            {
                var delSql = CreateDeleteSql(modelObj, id);
                return sqlAccess.ExecuteNonQuery(delSql) > 0;
            }
        }
        private string CreateDeleteSql(ModelBase model, int id)
        {
            return string.Format($"DELETE FROM {model.TableName} WHERE Id = {id}");
        }

        public bool UpgradeDataBase(string dataBaseName)
        {
            if (!IsExistDataBase(dataBaseName))
                CreateDataBase(dataBaseName);
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
                        if (!sfield.IsStore) continue;
                        if (sfield is Many2Many ||
                            sfield is Many2One)
                        {
                            if (sfield is Many2Many)
                            {
                                sql = IsExistTable(model.TableName);
                                if (SqlTyepConvert.ConvertToInt(sqlAccess.ExecuteScalar(sql)) == 0)
                                {//不存在
                                    sql = CreateRelationTable(sfield.RelationTableName,
                                        sfield.ColumnName, sfield.ColumnName1);
                                }
                                else
                                    sql = string.Empty;
                            }
                            else
                            {
                                sql = CreateColumn(model.TableName, sfield.ColumnName,
                                typeof(int), 0, 0);
                            }
                        }
                        else
                        {
                            sql = CreateColumn(model.TableName, sfield.ColumnName,
                                sfield.Type, sfield.Size, sfield.Precision);
                        }
                        if (!string.IsNullOrEmpty(sql))
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

        private string CreateRelationTable(string tableName, string column1, string column2)
        {
            return string.Format($"CREATE TABLE {tableName}({column1} INT,{column2} INT)");
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
                        if (!sfield.IsStore) continue;
                        if (sfield is Many2Many ||
                            sfield is Many2One)
                        {
                            if (sfield is Many2Many)
                            {
                                sql = IsExistTable(model.TableName);
                                if (SqlTyepConvert.ConvertToInt(sqlAccess.ExecuteScalar(sql)) == 0)
                                {//不存在
                                    sql = CreateRelationTable(sfield.RelationTableName,
                                        sfield.ColumnName, sfield.ColumnName1);
                                }
                                else
                                    sql = string.Empty;
                            }
                            else
                            {
                                sql = IsExistColumn(model.TableName, sfield.FieldName);
                                if (SqlTyepConvert.ConvertToInt(sqlAccess.ExecuteScalar(sql)) == 0)
                                    sql = CreateColumn(model.TableName, sfield.ColumnName,
                                                            typeof(int), 0, 0);
                                else
                                    sql = string.Empty;
                            }
                            if (!string.IsNullOrEmpty(sql))
                                sqlAccess.ExecuteNonQuery(sql);
                        }
                        else
                        {
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
            return string.Format($"CREATE TABLE {tableName}(Id INT IDENTITY(1,1) PRIMARY KEY)");
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
