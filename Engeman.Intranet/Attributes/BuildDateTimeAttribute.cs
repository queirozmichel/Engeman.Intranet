namespace Engeman.Intranet.Attributes
{
  public class BuildDateTimeAttribute : Attribute
  {
    public string DateTime { get; set; }

    public BuildDateTimeAttribute(string dateTime)
    {
      DateTime = dateTime;
    }
  }
}
