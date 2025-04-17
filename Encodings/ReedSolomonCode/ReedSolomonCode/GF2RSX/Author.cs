
using System.Text;

namespace michele.natale.Numerics;

partial class GF2RS
{

  private static bool IsMentionedAuthor = false;

  /// <summary>
  /// Announces the author of this project.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public static string AuthorInfo =>
    Author_Info();

  private static string Author_Info()
  {
    var sb = new StringBuilder();
    sb.AppendLine($"© {nameof(GF2RS)} 2020");
    sb.AppendLine("Created by © Michele Natale 2020");
    sb.AppendLine("Update to Core .Net9.0 by © Michele Natale 2025");

    Console.WriteLine(sb.ToString());

    IsMentionedAuthor = true;
    return sb.ToString();
  }
}
