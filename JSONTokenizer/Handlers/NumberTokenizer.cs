using System;

namespace JSONTokenizer.Handlers
{
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
                InputCondition[] inputConditions = { IsDigit, IsFraction, IsNegative, IsExponent, IsExponentSigned };
                Token token = new Token(t.input.Position, t.input.LineNumber,
                    "number", "");
                int i = 0;
                while (i < t.input.Length)
                {
                    foreach (var condition in inputConditions)
                    {
                        token.Value += t.input.loop(condition);
                    }
                    i++;
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
}