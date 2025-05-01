

namespace michele.natale.Numerics;

partial class GF2RS
{

  ///// <summary>
  ///// Returns the multiplicative inverse of the current value. 
  ///// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  ///// </summary>
  //public GF2RS Inv_Mul =>
  //  this.Value != 0 ? new GF2RS(this.Order, this.IDP, this.Exp[this.Order - 1 - this.Log[this.Value]])
  //  : throw new ArgumentOutOfRangeException(
  //      nameof(this.InvMul), $"{nameof(this.InvMul)}(0) is undefined!");

  /// <summary>
  /// Returns the multiplicative inverse of the value. 
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="value">Desired Value</param>
  /// <returns>Multiplicative inverse of the value</returns>
  public byte InvMul(byte value) =>
    this.Exp[this.Order - this.Log[value] - 1];

  /// <summary>
  /// Calculates the Galois addition.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="left">Desired GF2-Left-Value</param>
  /// <param name="right">Desired GF2-Right-Value</param>
  /// <returns>new GF2-Value</returns>
  public byte Addition(byte left, byte right) =>
    ExtMod((byte)(ExtMod(left, this.Order) ^ ExtMod(right, this.Order)), this.Order);

  /// <summary>
  /// Calculates the Galois subtraction.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="left">Desired GF2-Left-Value</param>
  /// <param name="right">Desired GF2-right-Value</param>
  /// <returns>new GF2-Value</returns>
  public byte Subtract(byte left, byte right) =>
    ExtMod((byte)(ExtMod(left, this.Order) ^ ExtMod(right, this.Order)), this.Order);

  /// <summary>
  /// Calculates the Galois multiplication.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="left">Desired GF2-Left-Value</param>
  /// <param name="right">Desired GF2-Right-Value</param>
  /// <returns>new GF2-Value</returns>
  public byte Multiply(byte left, byte right)
  {
    var result = (byte)0;
    var order = this.Order;

    left = ExtMod(left, order);
    right = ExtMod(right, order);
    if (left != 0 && right != 0)
    {
      var tmp = (this.Log[left] + this.Log[right]) % (order - 1);
      result = this.Exp[tmp];
    }

    return ExtMod(result, order);
  }

  /// <summary>
  /// Calculates the Galois division.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="left">Desired GF2-Left-Value</param>
  /// <param name="right">Desired GF2-Right-Value</param>
  /// <returns>new GF2 Value</returns>
  /// <exception cref="DivideByZeroException"></exception>
  public byte Divide(byte left, byte right)
  {
    left = ExtMod(left, this.Order);
    right = ExtMod(right, this.Order);

    if (right == 0)
      throw new DivideByZeroException(nameof(right));

    var result = (byte)0;
    var order = this.Order;
    if (left != 0)
    {
      var tmp = (order + this.Log[left] - this.Log[right] - 1) % (order - 1);
      result = this.Exp[tmp];
    }

    return ExtMod(result, order);
  }

  /// <summary>
  /// Compares two GF2-values for equality.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="left">Desired GF2-Value</param>
  /// <param name="right">Desired GF2-Value</param>
  /// <returns>True if Equals, ortherwise false.</returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public static bool operator ==(GF2RS left, GF2RS right) =>
    left.Order == right.Order ? left.Value == right.Value
    : throw new ArgumentOutOfRangeException(nameof(left),
      $"'=='-Operation has failed!");

  /// <summary>
  /// Compares two GF-values for inequality.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="left">Desired GF2-Value</param>
  /// <param name="right">Desired GF2-Value</param>
  /// <returns>True if not Equals, ortherwise false.</returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public static bool operator !=(GF2RS left, GF2RS right) =>
    left.Order == right.Order ? left.Value != right.Value
    : throw new ArgumentOutOfRangeException(nameof(left),
      $"'!='-Operation has failed!");

  /// <summary>
  /// Compares the current GF2 value with another one.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="obj">Desired Value</param>
  /// <returns>True if Equals, ortherwise false.</returns>
  public override bool Equals(object? obj)
  {
    if (obj is null) return false;
    if (obj is not GF2RS gf) return false;
    return this.Order == gf.Order && this.Value == gf.Value;
  }

  /// <summary>
  /// Converts the value of this instance into its equivalent string representation.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <returns>
  /// The string representation of the value of this instance, 
  /// consisting of a sequence of digits ranging from 0 to 9, 
  /// without a sign or leading zeroes.
  /// </returns>
  public override string ToString() => this.Value.ToString();

  /// <summary>
  /// Calculates a hash code from the current combination of fields.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <returns>Return the HashCode</returns>
  public override int GetHashCode() =>
    HashCode.Combine(this.Value, this.Order, this.IDP);
}
