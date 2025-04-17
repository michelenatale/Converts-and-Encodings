
using System.Text;


namespace michele.natale.ChannelCodings;

/// <summary>
/// Provides information about the author.
/// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
/// </summary>
public class RSAuthorInfo
{

  public static bool IsMentionedAuthor { get; private set; } = false;

  /// <summary>
  /// Announces the author of this project.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public static string AuthorInfo =>
    Author_Info();

  private static string Author_Info()
  {
    var sb = new StringBuilder();
    sb.AppendLine($"© ReedSolomonCode 2025");
    sb.AppendLine("Created by © Michele Natale 2025");

    Console.WriteLine(sb.ToString());

    IsMentionedAuthor = true;
    return sb.ToString();
  }
}
