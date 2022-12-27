using System.ComponentModel;
using System.Reflection;

namespace Engeman.Intranet.Extensions
{
  static class EnumExtensions
  {
    /// <summary>
    /// Método de extensão utilizado para obter o nome amigável inserido como decorador à cada elemento do enumerador. Ex: [Description("inclusão")]
    /// </summary>
    public static string GetEnumDescription(this Enum e)
    {
      Type eType = e.GetType();
      string eName = Enum.GetName(eType, e);
      if (eName != null)
      {
        FieldInfo fieldInfo = eType.GetField(eName);
        if (fieldInfo != null)
        {
          DescriptionAttribute descriptionAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
          if (descriptionAttribute != null)
          {
            return descriptionAttribute.Description;
          }
        }
      }
      return null;
    }
  }
}
