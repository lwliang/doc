using System;

namespace Model.Field
{
    public abstract class FieldBase<T>
    {
        protected FieldBase()
        {
            Default = default(T);
            Value = default(T);
            IsStore = true;
            Model = null;
            FieldName = string.Empty;
            Size = 100;
            Precision = 2;
        }

        public T Default { get; protected set; }

        public virtual T Value { get; set; }

        public bool IsStore { get; protected set; }

        public Type Type => typeof(T);

        public ModelBase Model { get; protected set; }

        public string FieldName { get; protected set; }
        private string _columnName;
        /// <summary>
        /// 数据库字段名称
        /// </summary>
        public string ColumnName
        {
            get
            {
                if (!string.IsNullOrEmpty(_columnName))
                    return _columnName;
                return FieldName;
            }
            protected set { _columnName = value; }
        }

        public int Size { get; protected set; }

        public int Precision { get; protected set; }
    }
}
