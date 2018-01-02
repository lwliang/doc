using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuskyTest
{
    [TestClass]
    public class DataBaseTest
    {
        [TestMethod]
        public void CreateDataBase()
        {
            var database = DataBaseManager.CreateSingleInstace();
            database.Server = ".";
            database.UserName = "sa";
            database.PassWord = "123456";
            database.SqlType = SqlType.MsSql;
            database.DataBaseName = "master";
            database.ModelManager.Register(new TestModel());
            var b = database.Create("liwenliang");

            Assert.AreEqual(b, true);
        }

        [TestMethod]
        public void UpgradeDataBase()
        {
            var database = DataBaseManager.CreateSingleInstace();
            database.Server = ".";
            database.UserName = "sa";
            database.PassWord = "123456";
            database.SqlType = SqlType.MsSql;
            database.DataBaseName = "master";
            database.ModelManager.Register(new TestModel());
            database.ModelManager.Register(new ParentModel());
            var b = database.Upgrade("liwenliang");

            Assert.AreEqual(b, true);
        }
    }
}
