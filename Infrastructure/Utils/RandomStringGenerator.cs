using RandomStringCreator;

namespace Infrastructure.Utils
{
    /// <summary>
    /// For computing unique slugs
    /// </summary>
    class RandomStringGenerator : Abstracts.IRandomStringGenerator
    {
        private readonly StringCreator stringCreator;
        public RandomStringGenerator()
        {
            stringCreator = new StringCreator();
        }

        public string Get(uint length)
        {
            return stringCreator.Get((int)length);
        }
    }
}
