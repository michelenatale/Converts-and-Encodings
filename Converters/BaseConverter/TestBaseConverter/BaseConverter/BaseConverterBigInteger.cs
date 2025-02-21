using System.Numerics;
using System.Runtime.CompilerServices;

namespace TestBaseConverter.BaseConverter; 

public class BaseConverterBigInteger
{

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] Converter(
    ReadOnlySpan<byte> bytes, int startbase, int targetbase)
  {
    var base10 = startbase == 10 ? bytes : FromBaseX(bytes, startbase);
    if (targetbase == 10) return base10.ToArray();
    return ToBaseX(base10, targetbase);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] FromBaseX(ReadOnlySpan<byte> bytes, int basex)
  {
    //From Base X to Base 10
    if (basex == 10) return [.. bytes.ToArray()];

    var length = bytes.Length;
    var bi = BigInteger.Zero;
    for (var i = 0; i < length; i++)
      bi += bytes[^(1 + i)] * BigInteger.Pow(basex, i);

    return [.. bi.ToString().Select(x => (byte)(x - 48))];
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] ToBaseX(ReadOnlySpan<byte> bytes_base10, int basex) =>
    ToBaseX(BigInteger.Parse(string.Join("", bytes_base10.ToArray())), basex);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] ToBaseX(BigInteger base10, int basex)
  {
    //From Base 10 to Base X (Reverse !)
    if (basex == 10) return [.. base10.ToString().Select(x => (byte)(x - 48))];

    var tmp = new Stack<byte>();
    while (base10 != 0)
    {
      tmp.Push((byte)(base10 % basex));
      base10 /= basex;
    }

    var result = tmp.ToArray(); 
    return result;
  }
}
