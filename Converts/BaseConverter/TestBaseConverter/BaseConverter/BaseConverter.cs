using System.Numerics;
using System.Runtime.CompilerServices;

namespace michele.natale.Converts;
 

public class BaseConverter
{

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] ToBaseX(BigInteger base10, int basex)
  {
    if (base10.Sign < 0)
      throw new ArgumentException(
        $"{nameof(base10)} is negativ !", nameof(base10));

    //From Base 10 to Base X
    var bytes = base10.ToString().Select(x => (byte)(x - 48)).ToArray();
    if (basex == 10) return bytes;

    return Converter(bytes, 10, basex);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] ToBaseX(ReadOnlySpan<byte> bytes, int basex) =>
    Converter(bytes, 10, basex);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] FromBaseX(ReadOnlySpan<byte> bytes, int basex)
  {
    //From Base X to Base 10
    if (basex == 10) return bytes.ToArray();

    return Converter(bytes, basex, 10);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] Converter(
    ReadOnlySpan<byte> bytes, int startbase, int targetbase)
  {
    if (bytes.Length == 0) return new byte[1];
    var cap = Convert.ToInt32(bytes.Length * Math.Log(startbase) / Math.Log(targetbase)) + 1;
    var result = new Stack<byte>(cap);

    var ext = true;
    var length = bytes.Length;
    var input = bytes.ToArray();
    byte remainder, accumulator;
    while (ext)
    {
      remainder = 0; ext = false;
      for (var i = 0; i < length; i++)
      {
        accumulator = (byte)((startbase * remainder + input[i]) % targetbase);
        input[i] = (byte)((startbase * remainder + input[i]) / targetbase);
        remainder = accumulator;
        if (input[i] > 0) ext = true;
      }
      result.Push(remainder);
    }

    return [.. result];
  }
}
