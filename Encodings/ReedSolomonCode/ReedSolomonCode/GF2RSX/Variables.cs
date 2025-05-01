
namespace michele.natale.Numerics;

partial class GF2RS
{

  private byte MValue;

  /// <summary>
  /// Smallest permitted exponent for the order calculation in a GF2.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MIN_EXP = 4;

  /// <summary>
  /// Largest permitted exponent for the order (fieldsize) calculation in a GF2.
  /// <para>Note: For performance reasons, the maximum exponent is 15 (32'769). The list can be extended if required.</para>
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_EXP = 8; // (int)GF2RS.ToIDPs.Keys.Max();

  /// <summary>
  /// Current GF2 value that represents a decimal number.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public byte Value
  {
    get => this.MValue;
    internal set => this.MValue = ExtMod(value, this.Order);
  }

  /// <summary>
  /// The generator for GF2, which is always 2.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const byte Generator = 0x2; //Basic

  /// <summary>
  /// The current Exp-List for GF2.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  internal readonly byte[] Exp = [];

  /// <summary>
  /// The current Log-List for GF2.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  internal readonly byte[] Log = [];

  /// <summary>
  /// The current irreducible polynomial of this GF2.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// <para>Sample see here <see href="https://www.wolframalpha.com/input?i=gf%28256%29">wolframalpha</see></para>
  /// </summary>
  public ushort IDP { get; private set; } = 0;

  /// <summary>
  /// The current order (2^x) of this GF2.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public ushort Order { get; private set; } = 0;

  /// <summary>
  /// The current Exponent of this GF2.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public int Exponent { get; private set; } = -1;
}
