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

        public SqlType SqlType { get; protected set; }
        public string Server { get; protected set; }
        public string UserName { get; protected set; }
        public string PassWord { get; protected set; }
        public string ConnectString { get; protected set; }
        public string DataBaseName { get; protected set; }

        public ModelManager ModelManager => ModelManager.Instance;

        protected DataBaseManager()
        {
            //DataBaseName = dataBaseName;
        }

        public bool Create()
        {
            return ModelManager.Instance.CreateDataBase(DataBaseName);
        }

        public bool Upgrade()
        {
            return ModelManager.Instance.UpgradeDataBase(DataBaseName);
        }
    }
}
