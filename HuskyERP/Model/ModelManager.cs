using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataBase;

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
            throw new NotImplementedException();
        }

        public bool IsExistDataBase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public bool CreateDataBase(string databaseName)
        {
            var sqlAccess =
                SqlAccessFactory.CreateSqlAccess(DataBaseManager.Instance.SqlType,
                    DataBaseManager.Instance.ConnectString);
            var sql = string.Format($"CREATE DATABASE {databaseName}");
            try
            {
                sqlAccess.BeginTransaction();
                sqlAccess.ExecuteNonQuery(sql);
                sqlAccess.Commit();
                return true;
            }
            catch
            {
                sqlAccess.Rollback();
                throw;
            }
        }

        private void CreateAllTables()
        {
            foreach (var keypair in ModelDic)
            {
                try
                {
                    var sql = CreateTable(keypair.Value)
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

        }

        public bool IsExistTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public bool IsExistColumn(string columnName)
        {
            throw new NotImplementedException();
        }

        public string CreateColumn(string columnName)
        {
            throw new NotImplementedException();
        }

        public string UpgradeColumn(string tableName, string columnName)
        {
            throw new NotImplementedException();
        }
    }
}
