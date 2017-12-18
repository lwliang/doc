using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
namespace HuskyTest
{
    [TestClass]
    public class ModelTest
    {
        [TestMethod]
        public void ModelInstance()
        {

        }
    }

    public class TestModel : RealModel
    {
        public TestModel() : base()
        {
            ModelName = "test.user";
        }
    }
}
