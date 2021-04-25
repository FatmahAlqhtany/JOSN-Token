using System;
using System.Collections.Generic;
using JSONTokenizer.Handlers;

namespace JSONTokenizer
{
    class JSON
    {
        public string key;
        public string value;
        public Input input;
        private List<JKeyValue> list;
        private JObject obj;

        private Tokenizer t;
        public JSON(Input input)
        {
            this.input = input;
            this.list = new List<JKeyValue>();
            obj = new JObject();
            this.t = new Tokenizer(input, new Tokenizable[]
            {
                new JsonCharactersTokenizer(),
                new StringTokenizer(),
                new WhiteSpaceTokenizer(),
                new NumberTokenizer(),
                new KeywordsTokenizer()
            });
        }

        
        public JSONValue ParseValue(Token token)
        {
            
             if (token.Type == "CurlyBracketOpen")
             {
                 JObject o = ParseObject();
                 return o;
             }

             if (token.Type == "number")
             {
                 //JNumber number = new JNumber(double.Parse(token.Value));
                 //number.value = double.Parse(token.Value);
                 //return number;
                 return new JNumber(double.Parse(token.Value));
             }

             if (token.Type == "string")
             {
                 return new JString(token.Value);
             }

             if (token.Type == "space")
             {
                 
             }
             return null;
        }

        public JArray ParseJArray()
        {
            JArray arr = new JArray();
            arr.values = new List<JSONValue> { };
            Token token = t.tokenize();
            while (t.input.hasMore())
            {
                token = t.tokenize();
                while (token.Type == "space")
                {
                    token = t.tokenize();
                }
                if (token.Type == "CurlyBracketOpen")
                {
                    JObject obj = ParseObject();
                    arr.values.Add(obj);
                }
                else if (token.Type == "SquareBracketOpen")
                {
                    JArray array = ParseJArray();
                    arr.values.Add(array);
                }
                else 
                {
                    token = t.tokenize();
                    if (token == null){break;}
                    arr.values.Add(this.ParseValue(token));
                }
                token = t.tokenize();
                while (token.Type == "space")
                {
                    token = t.tokenize();
                }
                if (token.Type == "Comma")
                {
                    continue;
                } else if (token.Type == "CurlyBracketClose")
                {
                    break;
                }
             
            }

            return arr;
        }
    
        

        public JObject ParseObject()
        {
            string key ="";
            JSONValue value=null;
            Token token = t.tokenize();
            if (token.Type == "CurlyBracketOpen" || token.Type == "string")
            {
                while (t.input.hasMore())
                {
                    while (token != null && token.Type == "space")
                    {
                        token = t.tokenize();
                    }
                    token = t.tokenize();
                    if (token.Type == "Comma")
                    {
                        token = t.tokenize();
                    }

                    while (token.Type == "space")
                    {
                        token = t.tokenize();
                    }
                    if (token.Type == "string")
                    {
                        //Console.Write(token.Value);
                        key = token.Value;
                    }
                    token = t.tokenize();
                    if (token == null)
                    {
                        break;
                    }
                    while (token != null && token.Type == "space")
                    {
                        token = t.tokenize();
                    }

                    if (token != null && token.Type == "Colon")
                    {
                         token = t.tokenize();
                         while (token.Type == "space")
                         {
                             token = t.tokenize();
                         }
                         if (token.Type == "CurlyBracketOpen")
                         {
                             value = ParseValue(token);
                             list.Add(new JKeyValue(key, value));
                         }
                         else if (token.Type == "SquareBracketOpen")
                         {
                             //value = ParseJArray(token);
                             value = ParseJArray();
                             list.Add(new JKeyValue(key, value));
                         }
                         else
                         {
                             value = ParseValue(token);
                             list.Add(new JKeyValue(key, value));
                         }
                    }
                    while (token.Type == "space")
                    {
                        token = t.tokenize();
                    }

                    if (token.Type == "CurlyBracketClose")
                    {
                        break;
                    }
                }
            }
            while (token != null)
            {
                token = t.tokenize();
            }

            obj.values = list;

            return obj;
        }
       
    }
}