namespace Stub
{
    using System;
    using System.Text;

    public class RandomStringGenerator
    {
        private static readonly string LOWERCASE = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string UPPERCASE = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string NUMBERS = "0123456789";

        private readonly Random _random;

        public RandomStringGenerator()
        {
            _random = new Random();
        }

        public RandomStringGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public string NextString(int length)
        {
            return NextString(length, useLowerCase: true, useUpperCase: true, useNumbers: true);
        }

        public string NextString(int length, bool useLowerCase, bool useUpperCase, bool useNumbers)
        {
            var characterPool = new StringBuilder();

            if (useLowerCase) characterPool.Append(LOWERCASE);
            if (useUpperCase) characterPool.Append(UPPERCASE);
            if (useNumbers) characterPool.Append(NUMBERS);

            if (characterPool.Length == 0)
                throw new ArgumentException("At least one character set must be enabled.");

            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = characterPool[_random.Next(characterPool.Length)];
            }

            return new string(result);
        }
    }
}
