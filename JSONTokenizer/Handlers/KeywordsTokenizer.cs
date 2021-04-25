using System;
using System.Collections.Generic;

namespace JSONTokenizer.Handlers
{
    public class KeywordsTokenizer : Tokenizable
    {
        private List<string> keywords;

        public KeywordsTokenizer()
        {
            this.keywords = new List<string> {"true", "false", "null"};
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
}