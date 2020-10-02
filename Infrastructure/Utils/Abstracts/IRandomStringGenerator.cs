namespace Infrastructure.Utils.Abstracts
{
    /// <summary>
    /// For computing unique slugs
    /// This interface was created because "real" pseudo random string generators
    /// call into underlying heavy APIs: BCryptGenRandom on Windows, OpenSSL on other platforms and that might impact unit testing.
    /// </summary>
    public interface IRandomStringGenerator
    {
        string Get(uint length); 
    }
}
