 

namespace michele.natale.Numerics;

partial class GF2RS
{
  public GF2RS()
  {
  }

  /// <summary>
  /// C-Tor 
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="order">Desired order</param>
  public GF2RS(ushort order)
  {
    if (!IsMentionedAuthor)
      Author_Info();

    var expo = ToExponent(order);

    var idp = ToIDPs[expo];
    AssertGF2(order, idp.First());

    this.IDP = idp.First();
    this.Order = order;
    this.Exponent = expo;

    var (exp, log) = this.ToExpLog();
    this.Exp = exp; this.Log = log;
    this.Value = 0;
  }

  /// <summary>  /// <summary>
  /// C-Tor 
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="order">Desired order</param>
  /// <param name="idp">Desired irreducible Polynomial</param>
  public GF2RS(ushort order, ushort idp)
  {
    AssertGF2(order, idp);

    this.IDP = idp;
    this.Order = order;
    this.Exponent = ToExponent(order);

    var (exp, log) = this.ToExpLog();
    this.Exp = exp; this.Log = log;
    this.Value = 0;
  }

  /// <summary>
  /// C-Tor 
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="order">Desired order</param>
  /// <param name="idp">Desired irreducible polynomial</param>
  /// <param name="value">Desired value</param>
  public GF2RS(ushort order, ushort idp, byte value)
    : this(order, idp) => this.Value = ExtMod(value, order);

  /// <summary>
  /// C-Tor 
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="value">Desired GF2-Value</param>
  public GF2RS(GF2RS value) : this(value.Order, value.IDP, value.Value) { }

  public GF2RS(byte value, GF2RS reference) : this(reference.Order, reference.IDP, value) { }

  private (byte[] Exp, byte[] Log) ToExpLog()
  {
    var exp = new byte[this.Order];
    var log = new byte[this.Order];

    if (!IsMentionedAuthor)
      Author_Info();

    var value = (ushort)0x01;
    for (uint i = 0; i < this.Order; i++)
    {
      exp[i] = (byte)value;
      value <<= 1;
      if (value >= this.Order)
      {
        value = (byte)(value ^ this.IDP);
        value = (byte)(value & this.Order - 1);
      }
      if (i < this.Order - 1)
        log[exp[i]] = (byte)i;
    }

    return (exp, log);
  }


  /// <summary>
  /// Calculates the exponent from the variable order.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="order"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public static int ToExponent(ushort order)
  {
    if (!ushort.IsPow2(order))
      throw new ArgumentOutOfRangeException(nameof(order),
        $"{nameof(order)} is not a power of two");

    var result = 1;
    while (1u << result++ != order) ;
    result--;

    return result;
  }

  private static void AssertGF2(ushort order, ushort idp)
  {
    if (!ushort.IsPow2(order) || order < 2)
      throw new ArgumentOutOfRangeException(nameof(order),
        $"{nameof(order)} is not a power of two or order < 2");

    if (order > 2 && order >= idp)
      throw new ArgumentOutOfRangeException(nameof(idp),
        $"{nameof(idp)} is not a valid irreducible polynomial!");
  }

  /// <summary>
  /// Calculates the extended modulo calculation, which always results in a positive number.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T">Desired Type</typeparam>
  /// <param name="value">Desired Value</param>
  /// <param name="order">Desired Order</param>
  /// <returns>New Modulo Value</returns>
  public static byte ExtMod(byte value, ushort order) =>
    (byte)(((value % order) + order) % order);

}
