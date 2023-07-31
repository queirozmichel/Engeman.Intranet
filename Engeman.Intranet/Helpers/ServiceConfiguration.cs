namespace Engeman.Intranet.Helpers
{
  public class ServiceConfiguration
  {
    private readonly IConfiguration _configuration;

    public ServiceConfiguration(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public string GetCryptoSecretKey()
    {
      return _configuration["AppSecrets:CryptoSecretKey"];
    }
  }
}
