using System;

namespace JSONTokenizer.Handlers
{
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
                "string", v);
        }
    }
}