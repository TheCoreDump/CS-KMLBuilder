using System;
using System.Text;

namespace KmlBuilder
{
    public class IFDEntry
    {
        private Type _tagType;

        public IFDEntry() { }

        public IFDEntry(Type tagType)
        {
            _tagType = tagType;
        }


        public ushort TagId { get; set; }

        public ushort Type { get; set; }

        public uint Count { get; set; }

        public byte[] OriginalValueBytes { get; set; }

        public byte[] ValueBytes { get; set; }

        public object Value { get; set; }

        public string TagToString()
        {
            if (_tagType != null)
            {
                try
                {
                    return Enum.GetName(_tagType, TagId);
                }
                catch { }
            }

            return TagId.ToString("X4");
        }

        protected string ValueBytesToString()
        {
            StringBuilder Result = new StringBuilder();

            foreach (byte b in ValueBytes)
                Result.AppendFormat("{0} ", b.ToString("X2"));

            return Result.ToString();
        }

        protected string GetValueAsString()
        {
            if (Type == 2)
            {
                return string.Format("\"{0}\"", Encoding.UTF8.GetString(ValueBytes));
            }
            else
            {
                return ValueBytesToString();
            }
        }

        public override string ToString()
        {
            return string.Format("TagId: {0}  Type: {1}  Count: {2}  ObjectType: {3}  Value: {4}",
                                    TagToString(),
                                    Type,
                                    Count,
                                    Value == null ? "<<NULL>>" : Value.GetType().ToString(),
                                    GetValueAsString());
        }
    }
}
