using System.Collections.Generic;

namespace JSONTokenizer
{
    public abstract class JSONValue
    {
    }

    public class JString : JSONValue
    {
        private string value;

        public JString(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }
    }

    public class JNumber : JSONValue
    {
        private double value;

        public JNumber(double value)
        {
            this.value = value;
        }
    }

    public class JBoolean : JSONValue
    {
        private bool value;
    }

    public class JArray : JSONValue
    {
        public List<JSONValue> values;
    }

    public class JKeyValue
    {
        public string key;
        public JSONValue value;

        public JKeyValue()
        {

        }

        public JKeyValue(string key, JSONValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public class JObject : JSONValue
    {
        public List<JKeyValue> values;
    }
}