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

        public virtual bool IsModify
        {
            get
            {
                if (OldValue == null && Value != null) return true;
                if (OldValue == null || Value == null) return false;
                if (OldValue != null && Value == null) return true;
                return !OldValue.Equals(Value);
            }
        }
        public T OldValue { get; protected set; }

        public T Default { get; protected set; }
        private T _value;
        public virtual T Value
        {
            get { return _value; }
            set { OldValue = _value; _value = value; }
        }

        public bool IsStore { get; protected set; }

        public Type Type => typeof(T);

        public ModelBase Model { get; protected set; }

        public string FieldName { get; protected set; }
        public virtual string ColumnValue
        {
            get
            {
                if (Value == null) return "";
                return Value.ToString();
            }
        }
        private string _columnName;
        /// <summary>
        /// 数据库字段名称
        /// </summary>
        public virtual string ColumnName
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

        public virtual void Save()
        {
            OldValue = Value;
        }
    }
}
