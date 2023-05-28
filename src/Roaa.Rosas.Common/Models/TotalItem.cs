namespace Roaa.Rosas.Common.Models.Totals
{
    public class TotalItem<TKey, TValue>
    {
        public TotalItem(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; set; }
        public TValue Value { get; set; }
    }

    public class TotalItem : TotalItem<string, int>
    {
        public TotalItem(string key, int value)
            : base(key, value)
        {
        }
    }


    public class TotalGroupingDocument<TKey, TValue> : TotalItem<TKey, TValue>
    {
        public TotalGroupingDocument(TKey key, TValue value)
            : base(key, value)
        {
        }
    }

}
