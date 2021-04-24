using System;
using System.Collections.Generic;
using System.Dynamic;

namespace JSONTokenizer
{
    public delegate bool InputCondition(Input input);
    public class Input
    {
        private readonly string input;
        private readonly int length;
        private int position;
        private int lineNumber;
        public int Length
        {
            get
            {
                return this.length;
            }
        }
        public int Position
        {
            get
            {
                return this.position;
            }
        }
        public int NextPosition
        {
            get
            {
                return this.position + 1;
            }
        }
        public int LineNumber
        {
            get
            {
                return this.lineNumber;
            }
        }
        public char Character
        {
            get
            {
                if (this.position > -1) return this.input[this.position];
                else return '\0';
            }
        }
        public Input(string input)
        {
            this.input = input;
            this.length = input.Length;
            this.position = -1;
            this.lineNumber = 1;
        }
        public bool hasMore(int numOfSteps = 1)
        {
            if (numOfSteps <= 0) throw new Exception("Invalid number of steps");
            return (this.position + numOfSteps) < this.length;
        }
        public bool hasLess(int numOfSteps = 1)
        {
            if (numOfSteps <= 0) throw new Exception("Invalid number of steps");
            return (this.position - numOfSteps) > -1;
        }
        public Input step(int numOfSteps = 1)
        {
            if (this.hasMore(numOfSteps))
                this.position += numOfSteps;
            else
            {
                throw new Exception("There is no more step");
            }
            return this;
        }
        public Input back(int numOfSteps = 1)
        {
            if (this.hasLess(numOfSteps))
                this.position -= numOfSteps;
            else
            {
                throw new Exception("There is no more step");
            }
            return this;
        }
        public Input reset() { return this; }
        public char peek(int numOfSteps = 1)
        {
            if (this.hasMore(numOfSteps)) return this.input[this.Position + numOfSteps];
            return '\0';
        }

        public char peekBack(int numOfSteps = 1)
        {
            if (this.hasLess(numOfSteps))
            {
                return this.input[this.Position + 1 - numOfSteps];
            }

            return '\0';
        }
        public string loop(InputCondition condition)
        {
            string buffer = "";
            while (this.hasMore() && condition(this))
                buffer += this.step().Character;
            return buffer;
        }
    }
    public class Token
    {
        public int Position { set; get; }
        public int LineNumber { set; get; }
        public string Type { set; get; }
        public string Value { set; get; }
        public Token(int position, int lineNumber, string type, string value)
        {
            this.Position = position;
            this.LineNumber = lineNumber;
            this.Type = type;
            this.Value = value;
        }
    }
    public abstract class Tokenizable
    {
        public abstract bool tokenizable(Tokenizer tokenizer);
        public abstract Token tokenize(Tokenizer tokenizer);
    }
    public class Tokenizer
    {
        public Input input;
        public Tokenizable[] handlers;
        public Tokenizer(string source, Tokenizable[] handlers)
        {
            this.input = new Input(source);
            this.handlers = handlers;
        }
        public Tokenizer(Input source, Tokenizable[] handlers)
        {
            this.input = source;
            this.handlers = handlers;
        }
        public Token tokenize()
        {

            foreach (var handler in this.handlers)
                if (handler.tokenizable(this)) return handler.tokenize(this);
            return null;
        }
    }
    public class KeywordsTokenizer : Tokenizable
    {
        private List<string> keywords;

        public KeywordsTokenizer(List<string> keywords)
        {
            this.keywords = keywords;
        }

        public override bool tokenizable(Tokenizer t)
        {
            return isLetter(t.input);
        }

        static bool isLetter(Input input)
        {
            char currentCharacter = input.peek();
            return Char.IsLetter(currentCharacter);
        }

        public override Token tokenize(Tokenizer t)
        {
            string value = t.input.loop(isLetter);
            string type;
            if (value == "null")
                type = "null";
            else
            {
                type = "boolean";
            }

            if (!this.keywords.Contains(value))
                throw new Exception("Unexpected token");
            return new Token(t.input.Position, t.input.LineNumber,
                type, value);
        }
    }

    public class StringTokenizer : Tokenizable
    {
        public override bool tokenizable(Tokenizer t)
        {
            return t.input.peek() == '\"';
        }
        public bool isEscape(Tokenizer t)
        {
            char ch = t.input.step().Character;
            return (ch == '\"' || ch == '\\' || ch == '\r'
                || ch == '\n' || ch == '\b' || ch == '\t' || ch == '\f');
        }
        public override Token tokenize(Tokenizer t)
        {
            string v = "";
            char c = t.input.peek();
            if (c == '\"')
            {
                v += t.input.peek();
                while (t.input.hasMore())
                {
                    if (c == '\n' || c == '\t' || c == '\"')
                    {
                        if (!isEscape(t))
                        {
                            throw new Exception("Invalid escape character");
                        }
                    }
                    v += t.input.step().Character;
                    c = t.input.peek();
                    if (c == '\"')
                    {
                        v += t.input.step().Character;
                        break;
                    }
                }
            }
            return new Token(t.input.Position, t.input.LineNumber,
          "is string ", v);
        }
    }
    

    public class IsWhiteSpace : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                char currentCharacter = t.input.peek();
                return Char.IsWhiteSpace(currentCharacter);
            }

            static bool IsSpace(Input input)
            {
                char currentCharacter = input.peek();
                return Char.IsWhiteSpace(currentCharacter);
            }

            static bool IsLineFeed(Input input)
            {
                char currentCharacter = input.peek();
                if (currentCharacter.Equals('\n')) return true;
                else return false;
            }

            static bool carriageReutrn(Input input)
            {
                char currentCharacter = input.peek();
                if (currentCharacter.Equals('\r')) return true;
                else return false;
            }

            static bool HorizentlTab(Input input)
            {
                char currentCharacter = input.peek();
                if (currentCharacter.Equals('\t')) return true;
                else return false;
            }

            public override Token tokenize(Tokenizer t)
            {
                Token token = new Token(t.input.Position, t.input.LineNumber,
                    "Space", "");
                InputCondition[] i = { IsSpace, HorizentlTab, carriageReutrn, IsLineFeed };
                foreach (var conditon in i)
                {
                    token.Value += t.input.loop(conditon);
                }

                return token;
            }
        }


      
        public class NumberTokenizer : Tokenizable
        {
            public override bool tokenizable(Tokenizer t)
            {
                char currentCharacter = t.input.peek();
                char nextCharacter = t.input.peek(2);
                return IsOneNineDigit(currentCharacter) ||
                       (currentCharacter == '0' && nextCharacter == '.') ||
                       (currentCharacter == '-' && Char.IsDigit(nextCharacter));
            }

            public override Token tokenize(Tokenizer t)
            {
                InputCondition[] i = { IsDigit, IsFraction, IsNegative, IsExponent, IsExponentSigned };
                Token token = new Token(t.input.Position, t.input.LineNumber,
                    "number", "");
                int k = 0;
                while (k < t.input.Length)
                {
                    foreach (var condition in i)
                    {
                        token.Value += t.input.loop(condition);
                        //Console.WriteLine(token.Value);
                        //if value has more than one . e + - throw exception
                    }
                    k++;
                }

                return token;
            }

            static bool IsOneNineDigit(char character)
            {
                return (character >= '1' && character <= '9');
            }

            static bool IsDigit(Input input)
            {
                return Char.IsDigit(input.peek());
            }

            static bool IsFraction(Input input)
            {
                return input.peek() == '.' && Char.IsDigit(input.peek(2));
            }

            static bool IsNegative(Input input)
            {
                return input.peek() == '-' && Char.IsDigit(input.peek(2));
            }

            static bool IsExponent(Input input)
            {
                char currentCharacter = input.peek();
                char nextCharacter = input.peek(2);
                return (currentCharacter == 'e' || currentCharacter == 'E') &&
                       (Char.IsDigit(nextCharacter) || nextCharacter == '+' ||
                        nextCharacter == '-');
            }

            static bool IsExponentSigned(Input input)
            {
                char currentCharacter = input.peek();
                char nextCharacter = input.peek(2);
                char previousCharacter = input.peekBack();
                return ((currentCharacter == '+' || currentCharacter == '-') &&
                        (previousCharacter == 'e' || previousCharacter == 'E')
                        && Char.IsDigit(input.peek(2)));
            }
        }


        public class JsonCharactersTokenizer : Tokenizable
        {
            private List<Char> jsonCharacters = new List<Char> { ',', ':', '{', '}', '[', ']' };
            public override bool tokenizable(Tokenizer t)
            {
                return this.jsonCharacters.Contains(t.input.peek());
            }

            public override Token tokenize(Tokenizer t)
            {
                char character = t.input.step().Character;
                switch (character)
                {
                    case '{':
                        return new Token(t.input.Position, t.input.LineNumber,
                            "CurlyBracketOpen", "" + character);
                    case '}':
                        return new Token(t.input.Position, t.input.LineNumber,
                            "CurlyBracketClose", "" + character);
                    case '[':
                        return new Token(t.input.Position, t.input.LineNumber,
                            "SquareBracketOpen", "" + character);
                    case ']':
                        return new Token(t.input.Position, t.input.LineNumber,
                            "SquareBracketClose", "" + character);
                    case ',':
                        return new Token(t.input.Position, t.input.LineNumber,
                            "Comma", "" + character);
                    case ':':
                        return new Token(t.input.Position, t.input.LineNumber,
                            "Colon", "" + character);

                }

                throw new Exception($"Unexpected Token at Ln {t.input.LineNumber} Col {t.input.Position + 1}");

            }


        }


      

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
            public List<JSONValue> vals;
        }

        public class JKeyValue
        {
            public string key;
            public JSONValue value;
        }

        public class JObject : JSONValue
        {
            public List<JKeyValue> values;
        }
        class JSON
        {
            public string key;
            public string value;
            public Input input;

            public JSON(Input input)
            {
                this.input = input;
            }


            public JSONValue ParseValue(Token token)
            {

                if (token.Type == "CurlyBracketOpen")
                {
                    JObject o = ParseObject();
                    return o;
                }
                else if (token.Type == "number")
                {
                    return new JNumber(double.Parse(token.Value));
                }
                else if (token.Type == "string")
                {
                    return new JString(token.Value);
                }
                
                return null;
            }

            public JObject ParseObject()
            {
                JKeyValue keyValue = new JKeyValue();
                List<JKeyValue> list = new List<JKeyValue> { };
                JObject obj = new JObject();

                Tokenizer t = new Tokenizer(this.input, new Tokenizable[]
                {
                new JsonCharactersTokenizer(),
                new StringTokenizer(),
                new IsWhiteSpace(),
                new NumberTokenizer(),
                new KeywordsTokenizer(new List<string>(){"true", "false"})
                });
                Token token = t.tokenize();
                if (token.Value == "CurlyBracketOpen")
                {
                    while (t.input.hasMore() && token.Value == "CurlyBracketClose")
                    {
                        token = t.tokenize();
                        if (token.Type == "string")
                        {
                            Console.Write(token.Value);
                            keyValue.key = token.Value;
                        }
                        else throw new Exception("Not a key");

                        list.Add(keyValue);
                        if (token.Type == "CurlyBracketOpen")
                        {
                            JObject o = ParseObject();
                            keyValue.value = o;
                        }
                        else
                        {
                            token = t.tokenize();
                            Console.Write(token.Value);
                            keyValue.value = this.ParseValue(token);
                        }

                        list.Add(keyValue);
                    }
                }


                while (token != null)
                {
                    Console.WriteLine($"value: {token.Value}            type: {token.Type}");
                    token = t.tokenize();
                }

                obj.values = list;

                return obj;
            }

        }

        class Program
        {
            static void Main(string[] args)
            {
                JSON s = new JSON(new Input(@"
            {
                ""keyOne"": true,
                ""KeyTwo"": ""85998"",
                ""KeyThree"" : {""Key"": ""value  ""},
                ""KeyForth"": [903049309, -5678.67, 1.678E4567]
            }"));
                JObject l = s.ParseObject();
                foreach (var value in l.values)
                {
                    Console.WriteLine(value);
                }
              
            }
        }
    }
