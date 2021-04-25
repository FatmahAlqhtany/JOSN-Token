using System;
using System.Collections.Generic;

namespace JSONTokenizer.Handlers
{
    public class JsonCharactersTokenizer : Tokenizable
    {
        private List<Char> jsonCharacters = new List<Char> {'{', '}', '[', ']', ',', ':'};

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
            throw new Exception($"Unexpected Token {t.input.LineNumber}");
        }

    }
}