using System.Text.Json;

namespace Engeman.Intranet.Extensions
{
  /// <summary>
  /// Métodos de extensão para armazenar e obter qualquer tipo de dado como variável de sessão
  /// </summary>
  public static class SessionExtensions
  {
    public static void Set<T>(this ISession session, string key, T value)
    {
      session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
      var value = session.GetString(key);
      return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
  }
}