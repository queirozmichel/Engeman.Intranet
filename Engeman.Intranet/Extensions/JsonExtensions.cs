using System.Text.Json;

namespace Engeman.Intranet.Extensions
{
  public static class JsonExtensions
  {
    /// <summary>
    /// Deserializa uma string JSON para um objeto e converte os valores inteiros em boleanos
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static T DeserializeAndConvertIntToBool<T>(this string jsonString) where T : class
    {
      JsonSerializerOptions options = new JsonSerializerOptions
      {
        Converters = { new IntToBoolConverter() }
      };

      return JsonSerializer.Deserialize<T>(jsonString, options);
    }

    /// <summary>
    /// Serializa um objeto para uma string JSON e converte os valores boleanos para inteiros
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SerializeAndBoolToIntConverter<T>(this T obj)
    {
      JsonSerializerOptions options = new JsonSerializerOptions
      {
        Converters = { new JsonBoolToIntConverter() },
      };

      return JsonSerializer.Serialize(obj, options);
    }
  }

  /// <summary>
  /// Conversor de int para bool
  /// </summary>
  public class IntToBoolConverter : System.Text.Json.Serialization.JsonConverter<bool>
  {
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      // Lê o valor inteiro
      int intValue = reader.GetInt32();

      // Converte o valor inteiro em bool (0 -> false, qualquer outro valor -> true)
      return intValue != 0;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
      // Escreve o valor bool como um número inteiro (false -> 0, true -> 1)
      writer.WriteNumberValue(value ? 1 : 0);
    }
  }

  /// <summary>
  /// Conversor de bool para int
  /// </summary>
  public class JsonBoolToIntConverter : System.Text.Json.Serialization.JsonConverter<bool>
  {
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
      writer.WriteNumberValue(value ? 1 : 0);
    }
  }
}
