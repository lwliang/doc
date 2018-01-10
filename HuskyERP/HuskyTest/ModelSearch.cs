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
    public class ModelSearch
    {
        [TestMethod]
        public void TestSearch()
        {
            var database = DataBaseManager.CreateSingleInstace();
            database.Server = ".";
            database.UserName = "sa";
            database.PassWord = "123456";
            database.SqlType = SqlType.MsSql;
            database.DataBaseName = "liwenliang";
            database.ModelManager.Register(new TestModel());
            database.ModelManager.Register(new ParentModel());

            var model = database.ModelManager.Search("test.user", 2);
            Assert.AreNotEqual(model, null);
        }
        [TestMethod]
        public void TestDelete()
        {
            var database = DataBaseManager.CreateSingleInstace();
            database.Server = ".";
            database.UserName = "sa";
            database.PassWord = "123456";
            database.SqlType = SqlType.MsSql;
            database.DataBaseName = "liwenliang";
            database.ModelManager.Register(new TestModel());
            database.ModelManager.Register(new ParentModel());

            var model = database.ModelManager.Delete("test.user", 3);
            Assert.AreEqual(model, true);
        }

        [TestMethod]
        public void TestInsert()
        {
            var database = DataBaseManager.CreateSingleInstace();
            database.Server = ".";
            database.UserName = "sa";
            database.PassWord = "123456";
            database.SqlType = SqlType.MsSql;
            database.DataBaseName = "liwenliang";
            database.ModelManager.Register(new TestModel());
            database.ModelManager.Register(new ParentModel());

            var test = new TestModel();
            //test.Name.Value = "ss";
            test.No.Value = 1;
            test.Price.Value = (decimal)2.33;
            var model = database.ModelManager.Insert(test);
            Assert.AreEqual(model, true);
        }
        [TestMethod]
        public void TestUpdate()
        {
            var database = DataBaseManager.CreateSingleInstace();
            database.Server = ".";
            database.UserName = "sa";
            database.PassWord = "123456";
            database.SqlType = SqlType.MsSql;
            database.DataBaseName = "liwenliang";
            database.ModelManager.Register(new TestModel());
            database.ModelManager.Register(new ParentModel());

            dynamic test = database.ModelManager.Search("test.user", 1);
            test.Name.Value = "iloveyou";
            var model = database.ModelManager.Update(test);
            Assert.AreEqual(model, true);
        }
    }
}
