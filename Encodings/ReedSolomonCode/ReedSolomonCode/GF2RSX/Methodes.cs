 
namespace michele.natale.Numerics;

partial class GF2RS
{
  /// <summary>
  /// Makes a copy of the current GF2.
  /// <para>Updated by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public GF2RS Copy =>
    new(this.Order, this.IDP, this.Value); 

  public byte PolyEval(ReadOnlySpan<byte> poly, byte x)
  {
    var sum = poly[0];
    var logx = this.Log[x];

    for (int i = 1; i < poly.Length; i++)
    {
      if (poly[i] == 0) continue;

      //var coeff = this.Log[poly[i]];
      var coeff = this.Log[poly[i]];
      var power = (coeff + (logx * (byte)i)) % (this.Order - 1);
      sum ^= this.Exp[power];
    }

    return sum;
  }

}
