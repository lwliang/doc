using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Field;

namespace HuskyTest
{
    [TestClass]
    public class ModelTest
    {
        [TestMethod]
        public void ModelInstance()
        {
            var model = new TestModel();
            var columns = model.GetColumnsField();
            Assert.AreEqual(columns.Count, 1);
        }
    }

    public class TestModel : RealModel
    {
        public IntegerField No { get; protected set; }
        public StringField Name { get; protected set; }
        public DecimalField Price { get; protected set; }
        public TestModel() : base()
        {
            ModelName = "test.user";
            No = FieldFactory.CreateIntegerField(this, nameof(No));
            Name = FieldFactory.CreateStringField(this, nameof(Name), 100);
            Price = FieldFactory.CreateDecimalField(this, nameof(Price), 10, 2);
        }
    }
}
