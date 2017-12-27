using System;

namespace Model.DataBase
{
    public class DataBaseManager
    {
        private static DataBaseManager _instance;

        public static DataBaseManager Instance => _instance ?? (_instance = new DataBaseManager());

        public static DataBaseManager CreateSingleInstace()
        {
            _instance = new DataBaseManager();
            return _instance;
        }

        public SqlType SqlType { get; set; }
        public string Server { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string ConnectString
        {
            get
            {
                if (string.IsNullOrEmpty(DataBaseName))
                {
                    return string.Format
                   ($"user id={UserName};password={PassWord};" +
                   //$"initial catalog={DataBaseName};" +
                   $"data source={Server};" +
                   $"connect Timeout=20");
                }
                else
                {
                    return string.Format
                   ($"user id={UserName};password={PassWord};" +
                   $"initial catalog={DataBaseName};" +
                   $"data source={Server};" +
                   $"connect Timeout=20");
                }
            }
        }
        public string DataBaseName { get; set; }

        public ModelManager ModelManager => ModelManager.Instance;

        protected DataBaseManager()
        {
            //DataBaseName = dataBaseName;
        }

        public bool Create(string databaseName)
        {
            return ModelManager.Instance.CreateDataBase(databaseName);
        }

        public bool Upgrade(string databaseName)
        {
            return ModelManager.Instance.UpgradeDataBase(databaseName);
        }
    }
}
