using System;

namespace JSONTokenizer.Handlers
{
    public class WhiteSpaceTokenizer :Tokenizable
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
                "space", "");
            InputCondition[] i = {IsSpace, HorizentlTab, carriageReutrn, IsLineFeed};
            foreach (var conditon in i)
            {
                token.Value += t.input.loop(conditon);
            }

            return token;
        }
    }
}