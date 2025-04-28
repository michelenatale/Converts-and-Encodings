

using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace michele.natale.ChannelCodings;

using Numerics;

/// <summary>
/// Provides tools and methods for decoding when dealing with Reed Solomon Code.
/// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
/// </summary>
public class RSDecoding
{
  #region Declarations

  /// <summary>
  /// Specifies the minimum exponent for the field size.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MIN_EXP = 4;

  /// <summary>
  /// Specifies the maximum exponent for the field size.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_EXP = 8;

  /// <summary>
  /// Specifies the minimum memory size for ecc - Error Correcting Code.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MIN_ECC_SIZE = 4;

  /// <summary>
  /// Specifies the minimum memory size for ecc - Error Correcting Code.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_ECC_SIZE = (1 << MAX_EXP) >> 1; //128

  /// <summary>
  /// Specifies the maximum message data length.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_DATA_SIZE = 1 << 20; //1'048'576 as byte

  /// <summary>
  /// Specifies the minimum fieldsize.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MIN_FIELD_SIZE = 1 << MIN_EXP;//16

  /// <summary>
  /// Specifies the maximum fieldsize.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_FIELD_SIZE = 1 << MAX_EXP;//256

  /// <summary>
  /// Specifies the minimum number of errors.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MIN_ERR_SIZE = 0;

  /// <summary>
  /// Specifies the maximum number of errors.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_ERR_SIZE = (1 << MAX_EXP) >> 2;//64

  /// <summary>
  /// Indicates the current ecc - Error Correcting Code size.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public int EccSize { get; private set; } = -1;

  /// <summary>
  /// Indicates the current FieldSize.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public int FieldSize { get; private set; } = -1;

  /// <summary>
  /// Indicates the current Galoise. Field.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public GF2RS GField { get; private set; } = null!;

  #endregion Declarations

  #region C-Tor 
  /// <summary>
  /// C-Tor 
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="fieldsize">Desired FieldSize</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code</param>
  public RSDecoding(int fieldsize, int eccsize)
  {
    if (!RSAuthorInfo.IsMentionedAuthor)
      _ = RSAuthorInfo.AuthorInfo;

    AssertRSDecoding(fieldsize);
    var idp = GF2RS.ToIDPs[GF2RS.ToExponent((ushort)fieldsize)].First();
    this.GField = new GF2RS((ushort)fieldsize, idp);
    this.FieldSize = GField.Order;
    this.EccSize = eccsize;
  }

  /// <summary>
  /// C-Tor 
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="field">Desiered Galois Field.</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  public RSDecoding(GF2RS field, int eccsize, bool check_idp = false)
  {
    if (!RSAuthorInfo.IsMentionedAuthor)
      _ = RSAuthorInfo.AuthorInfo;

    if (check_idp)
      CheckIdp(field.Order, field.IDP);

    this.GField = field;
    this.EccSize = eccsize;
    this.FieldSize = GField.Order;
  }

  /// <summary>
  /// C-Tor 
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="fieldsize">Desiered fieldsize.</param>
  /// <param name="idp">Desired IDP - Irreducible Polynomial.</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  public RSDecoding(int fieldsize, int idp, int eccsize, bool check_idp = false)
  {
    if (!RSAuthorInfo.IsMentionedAuthor)
      _ = RSAuthorInfo.AuthorInfo;

    AssertFieldSize(fieldsize, idp);
    if (check_idp) CheckIdp(fieldsize, idp);
    this.GField = new GF2RS((ushort)fieldsize, (ushort)idp);
    this.FieldSize = GField.Order;
    this.EccSize = eccsize;
  }
  #endregion C-Tor

  #region Static Decoding EntryPoints 
  /// <summary>
  /// Calculates the Reed Solomon Code Decoding based on the desired parameters.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="encode">Desired RSC-Encoding</param>
  /// <param name="fieldsize">Desired fieldsize</param> 
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param> 
  /// <returns></returns>
  public static T[] DecodingRS<T>(
    ReadOnlySpan<byte> encode, int fieldsize, int eccsize)
      where T : INumber<T>
  {
    AssertDecodingRS(encode, fieldsize, eccsize);

    var rss = fieldsize - 1;
    var cnt = encode.Length / rss;

    var dbs = rss - eccsize;
    var result = new byte[cnt * dbs];
    var rsdec = new RSDecoding(fieldsize, eccsize);
    for (var i = 0; i < cnt; i++)
      Array.Copy(rsdec.Decoding(encode.Slice(i * rss, rss), out _), 0, result, i * dbs, dbs);

    var length = BitConverter.ToInt32(result, result.Length - 4);

    return [.. FromBytes<T>(result.AsSpan(0, length).ToArray())];
  }

  /// <summary>
  /// Calculates the Reed Solomon Code Decoding based on the desired parameters.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="encode">Desired RSC-Encoding</param>
  /// <param name="fieldsize">Desired fieldsize</param>
  /// <param name="idp">Desired IDP - Irreducible Polynomial.</param> 
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param> 
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  /// <returns></returns>
  public static T[] DecodingRS<T>(
    ReadOnlySpan<byte> encode, int fieldsize, int idp, int eccsize, bool check_idp = false)
      where T : INumber<T>
  {
    AssertDecodingRS(encode, fieldsize, idp, eccsize, check_idp);

    var rss = fieldsize - 1;
    var cnt = encode.Length / rss;
    var dbs = rss - eccsize;
    var result = new byte[cnt * dbs];
    var rsdec = new RSDecoding(fieldsize, idp, eccsize);
    for (var i = 0; i < cnt; i++)
      Array.Copy(rsdec.Decoding(encode.Slice(i * rss, rss), out _), 0, result, i * dbs, dbs);

    var length = BitConverter.ToInt32(result, result.Length - 4);

    return [.. FromBytes<T>(result.AsSpan(0, length).ToArray())];
  }

  /// <summary>
  /// Calculates the Reed Solomon Code Decoding based on the desired parameters.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="encode">Desired RSC-Encoding</param>
  /// <param name="field">Desired Galois Field</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param> 
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  /// <returns></returns>
  public static T[] DecodingRS<T>(
    ReadOnlySpan<byte> encode, GF2RS field, int eccsize, bool check_idp = false)
      where T : INumber<T>
  {
    AssertDecodingRS(encode, field, eccsize, check_idp);

    var fieldsize = (int)field.Order;
    var rss = fieldsize - 1;

    var cnt = encode.Length / rss;
    var dbs = fieldsize - eccsize - 1;
    var result = new byte[cnt * dbs];
    var rsdec = new RSDecoding(field, eccsize);
    for (var i = 0; i < cnt; i++)
      Array.Copy(rsdec.Decoding(encode.Slice(i * rss, rss), out _), 0, result, i * dbs, dbs);

    var length = BitConverter.ToInt32(result, result.Length - 4);

    return [.. FromBytes<T>(result.AsSpan(0, length).ToArray())];
  }
  #endregion Static Decoding EntryPoints 


  #region Decoding

  /// <summary>
  /// Calculates the Reed Solomon Code Decoding based on the desired parameters.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="encode">Desired RSC-Encoding</param>
  /// <param name="ecc">Returns the ecc - Error Correcting Code.</param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public byte[] Decoding(ReadOnlySpan<byte> encode, out byte[] ecc)
  {
    if (encode.Length % (this.FieldSize - 1) != 0)
      throw new ArgumentOutOfRangeException(
        nameof(encode), $"{nameof(encode)}.Length != {this.FieldSize} - 1!");

    var databuffersize = encode.Length - this.EccSize;
    ecc = encode[databuffersize..].ToArray();

    var result = encode[..databuffersize].ToArray();
    var syndroms = ToSyndromPoly(result, ecc);
    var lambda = ToLambda(syndroms, this.EccSize - 1, this.EccSize - 1, this.EccSize - 1);

    var omega = ToOmega(lambda, syndroms, this.EccSize - 2);
    var lambdaprime = ToLambdaPrime(lambda, this.EccSize - 2);

    var chiencache = new byte[this.FieldSize - 1];

    for (var i = 0; i < chiencache.Length; i++)
      chiencache[i] = new GF2RS(this.GField.Exp[i], this.GField).Inv_Mul.Value;

    var erroridx = ChienSearch(lambda, chiencache, this.FieldSize - 1);
    this.RepairErrors(result, ecc, erroridx, omega, lambdaprime);

    return result;
  }
  #endregion Decoding

  #region PackageData
  /// <summary>
  /// Calculates all relevant parameters including the message based on the PackageData.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="packagedata"></param>
  /// <param name="rsinfo"></param>
  /// <returns></returns>
  public static T[] FromPackageData<T>(
    ReadOnlySpan<byte> packagedata, out RsInfo rsinfo)
      where T : INumber<T>
  {
    ReadOnlySpan<byte> tmp = packagedata[0] == 1
      ? packagedata[1..] : Decompress(packagedata[1..]);
    AssertFromMessage(tmp);

    ReadOnlySpan<byte> info = tmp[..6].ToArray();
    var eccsize = (int)BitConverter.ToUInt16(info.Slice(4, 2));
    var idp = (int)BitConverter.ToUInt16(info.Slice(2, 2));
    var fieldsize = (int)BitConverter.ToUInt16(info[..2]);
    rsinfo = ToRsinfo(fieldsize, idp, eccsize, eccsize >> 1, fieldsize - eccsize - 1);

    return DecodingRS<T>(tmp[6..], fieldsize, idp, eccsize, false);
  }

  /// <summary>
  /// Calculates all relevant parameters per Stream including the message based on the PackageData.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="src"></param>
  /// <param name="dest"></param>
  public static void FromPackageData(string src, string dest, out RsInfo rsinfo)
  {
    using var mss = new MemoryStream();
    using var fsin = new FileStream(src, FileMode.Open, FileAccess.Read);
    using var fsout = new FileStream(dest, FileMode.Create, FileAccess.Write);

    var buffer = new byte[1];
    fsin.ReadExactly(buffer);

    var c = buffer.First();
    CheckSetDecompress(fsin, mss, c);

    var pcode = 7;
    mss.Position = 0;
    var flength = mss.Length;
    buffer = new byte[pcode];
    mss.ReadExactly(buffer);
    AssertFromPackageDataStream(buffer, (int)flength - pcode);

    var idp = (int)BitConverter.ToUInt16(buffer.AsSpan(3, 2).ToArray());
    var eccsize = (int)BitConverter.ToUInt16(buffer.AsSpan(5, 2).ToArray());
    var fieldsize = (int)BitConverter.ToUInt16(buffer.AsSpan(1, 2).ToArray());
    var databuffersize = fieldsize - eccsize - 1;

    rsinfo = new RsInfo()
    {
      Idp = idp,
      EccSize = eccsize,
      FieldSize = fieldsize,
      ErrSizePerFs = eccsize >> 1,
      DataBufferSize = fieldsize - eccsize - 1,
    };

    buffer = new byte[4];
    mss.Position = mss.Length - eccsize - 4;
    mss.ReadExactly(buffer);
    var length = BitConverter.ToInt32(buffer);

    var readsize = 0;
    buffer = new byte[fieldsize - 1];
    var rsdec = new RSDecoding(fieldsize, idp, eccsize);

    var cnt = 0;
    mss.Position = pcode;
    while ((readsize = mss.Read(buffer)) > 0)
    {
      cnt++;
      var dec = rsdec.Decoding(buffer, out _);
      var l = dec.Length < databuffersize ? dec.Length : databuffersize;
      fsout.Write(dec.Take(l).ToArray());
    }
    fsout.SetLength(length);
  }
  #endregion PackageData 


  #region Methoden
  /// <summary>
  /// Calculates the next Power Of Two.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="number">Desired Number</param>
  /// <returns></returns>
  public static int NextPowerOfTwo(int number)
  {
    if (int.IsPow2(number)) return number;

    var result = 1;
    while (1 << result++ < number) ;
    result--;

    return 1 << result;
  }

  /// <summary>
  /// Calculates the next FieldSize.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="number">Desired Number</param>
  /// <returns></returns>
  public static int NextFieldSize(int number)
  {
    if (number >= 1 << GF2RS.MAX_EXP) return 1 << GF2RS.MAX_EXP;
    return NextPowerOfTwo(number);
  }

  private void RepairErrors(
    Span<byte> databuffer, Span<byte> ecc, ReadOnlySpan<byte> erridx,
    ReadOnlySpan<byte> omega, ReadOnlySpan<byte> lp)
  {
    var szecc = ecc.Length;
    var length = databuffer.Length + szecc;

    for (var i = 0; i < length; i++)
      if (erridx[i] == 0)
      {
        var x = this.GField.Exp[i];
        var inversex = this.GField.InvMul(x);

        var top = GField.PolyEval(omega, inversex);
        top = GField.Multiply(top, x);
        var bottom = GField.PolyEval(lp, inversex);

        if (i < szecc)
          ecc[i] ^= GField.Divide(top, bottom);
        else databuffer[i - szecc] ^= GField.Divide(top, bottom);
      }
  }

  private byte[] ChienSearch(
    ReadOnlySpan<byte> lambda,
    ReadOnlySpan<byte> chiencache, int erridxsize)
  {
    var result = new byte[erridxsize];

    for (var i = 0; i < erridxsize; i++)
      result[i] = this.GField.PolyEval(lambda, chiencache[i]);

    return result;
  }

  private byte[] ToSyndromPoly(byte[] databuffer, byte[] ecc)
  {
    var eccsize = ecc.Length;
    var result = new byte[eccsize];
    for (var i = 0; i < eccsize; i++)
    {
      var syndrome = byte.MinValue;
      var root = this.GField.Exp[i];

      for (var j = 0; j < databuffer.Length; j++)
        syndrome = this.GField.Multiply((byte)(syndrome ^ databuffer[^(j + 1)]), root);

      for (var j = 0; j < ecc.Length - 1; j++)
        syndrome = this.GField.Multiply((byte)(syndrome ^ ecc[^(j + 1)]), root);

      result[i] = (byte)(syndrome ^ ecc[0]);
    }

    return result;
  }

  private byte[] ToLambda(
    ReadOnlySpan<byte> syndroms,
    int lambdastarsize, int corrpolysize, int lambdasize)
  {
    var szecc = lambdasize + 1;
    var lambdastar = new byte[lambdastarsize];
    var result = new byte[lambdasize]; result[0] = 1;
    var corrpoly = new byte[corrpolysize]; corrpoly[1] = 1;

    int k = 1, l = 0;
    while (k <= szecc)
    {
      var e = syndroms[k - 1];

      for (var i = 1; i <= l; i++)
        e ^= this.GField.Multiply(result[i], syndroms[k - 1 - i]);

      if (e != 0)
      {
        for (var i = 0; i < lambdastar.Length; i++)
          lambdastar[i] = (byte)(result[i] ^ this.GField.Multiply(e, corrpoly[i]));

        if (2 * l < k)
        {
          l = k - l;
          var inve = this.GField.InvMul(e);
          for (var i = 0; i < corrpolysize; i++)
            corrpoly[i] = this.GField.Multiply(result[i], inve);
        }
      }

      for (var i = corrpolysize - 1; i >= 1; i--)
        corrpoly[i] = corrpoly[i - 1];

      corrpoly[0] = 0;

      if (e != 0)
        Array.Copy(lambdastar, result, lambdasize);

      k++;
    }

    return result;
  }

  private static byte[] ToLambdaPrime(
    ReadOnlySpan<byte> lambda, int lambdaprimesize)
  {
    var result = new byte[lambdaprimesize];
    for (var i = 0; i < lambdaprimesize; i++)
      if ((i & 0x1) == 0)
        result[i] = lambda[i + 1];
      else result[i] = 0;

    return result;
  }

  private byte[] ToOmega(
    ReadOnlySpan<byte> lambda,
    ReadOnlySpan<byte> syndroms, int omegasize)
  {
    var result = new byte[omegasize];
    for (var i = 0; i < result.Length; i++)
    {
      result[i] = syndroms[i];
      for (var j = 1; j <= i; j++)
        result[i] ^= this.GField.Multiply(syndroms[i - j], lambda[j]);
    }

    return result;
  }

  private static RsInfo ToRsinfo(
    int fieldsize, int idp, int eccsize,
    int errsizeperfs, int databuffersize)
  {
    return new RsInfo
    {
      Idp = idp,
      EccSize = eccsize,
      FieldSize = fieldsize,
      ErrSizePerFs = errsizeperfs,
      DataBufferSize = databuffersize,
    };
  }
  #endregion Methoden

  #region Utils
  private static T[] FromBytes<T>(byte[] bytes)
  {
    var length = bytes.Length;
    var tsz = Unsafe.SizeOf<T>();
    var result = new T[length / tsz];
    Buffer.BlockCopy(bytes, 0, result, 0, length);

    return result;
  }
  #endregion Utils

  #region Decompress
  /// <summary>
  /// Decompresses the data from a array of byte.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="input">Desired Data from a Array of byte</param>
  /// <returns></returns>
  private static byte[] Decompress(ReadOnlySpan<byte> input)
  {
    var data = input.ToArray();
    if (data.Length == 0) return [];

    using var msout = new MemoryStream();
    {
      using var msin = new MemoryStream([.. data]);
      using var gzip = new GZipStream(
        msin, CompressionMode.Decompress, false);
      gzip.CopyTo(msout);
    }
    return msout.ToArray();
  }

  private static void CheckSetDecompress(
    Stream fsin, Stream mss, byte choice)
  {
    var begin = choice == 1 ? 0 : 1;
    fsin.Position = begin;
    mss.Position = 0;
    if (choice == 2)
    {
      using var gzip = new GZipStream(fsin, CompressionMode.Decompress);
      mss.WriteByte(choice);
      gzip.CopyTo(mss);
    }
    else fsin.CopyTo(mss);
    mss.Position = 0;
  }
  #endregion Decompres

  #region Asserts
  private static void AssertDecodingRS(
    ReadOnlySpan<byte> encode, int fieldsize, int eccsize)
  {
    AssertFieldSize(encode, fieldsize);

    var buffersize = fieldsize - eccsize - 1;

    if (eccsize > fieldsize >> 1)
      throw new ArgumentOutOfRangeException(nameof(eccsize),
        $"{nameof(eccsize)} > {nameof(fieldsize)} >> 1 has failed!");

    if (buffersize + eccsize != fieldsize - 1)
      throw new ArgumentOutOfRangeException(nameof(fieldsize),
        $"{nameof(eccsize)} + {nameof(buffersize)} != ({nameof(fieldsize)} - 1) has failed!");
  }

  private static void AssertDecodingRS(
    ReadOnlySpan<byte> encode, int fieldsize, int idp, int eccsize, bool check_idp)
  {
    AssertFieldSize(encode, fieldsize);
    if (check_idp) CheckIdp(fieldsize, idp);
    var buffersize = fieldsize - eccsize - 1;

    if (eccsize > fieldsize >> 1)
      throw new ArgumentOutOfRangeException(nameof(eccsize),
        $"{nameof(eccsize)} > {nameof(fieldsize)} >> 1 has failed!");

    if (buffersize + eccsize != fieldsize - 1)
      throw new ArgumentOutOfRangeException(nameof(fieldsize),
        $"{nameof(eccsize)} + {nameof(buffersize)} != ({nameof(fieldsize)} - 1) has failed!");
  }

  private static void AssertDecodingRS(
    ReadOnlySpan<byte> encode, GF2RS field, int eccsize, bool check_idp)
  {
    var fieldsize = field.Order;
    AssertFieldSize(encode, fieldsize);

    var idp = field.IDP;
    var buffersize = fieldsize - eccsize - 1;
    if (check_idp) CheckIdp(fieldsize, idp);

    if (eccsize > fieldsize >> 1)
      throw new ArgumentOutOfRangeException(nameof(field),
        $"{nameof(eccsize)} > {nameof(fieldsize)} >> 1 has failed!");

    if (buffersize + eccsize != fieldsize - 1)
      throw new ArgumentOutOfRangeException(nameof(field),
        $"{nameof(eccsize)} + {nameof(buffersize)} != ({nameof(fieldsize)} - 1) has failed!");
  }

  private static void AssertRSDecoding(int fieldsize) =>
    AssertFieldSize(fieldsize);

  private static void CheckIdp(int fieldsize, int idp)
  {
    //Fieldsize was checked beforehand to see whether it is a Power Of Two.

    var result = 0;
    foreach (var item in GF2RS.ToIDPs[GF2RS.ToExponent((ushort)fieldsize)])
      if (idp == item) { result++; break; }

    if (result == 0) throw new ArgumentException(
      $"{nameof(idp)} is not valid!", nameof(idp));
  }
  private static void AssertFieldSize(ReadOnlySpan<byte> encode, int fieldsize)
  {
    AssertFieldSize(fieldsize);

    if (encode.Length % (fieldsize - 1) != 0)
      throw new ArgumentOutOfRangeException(nameof(encode),
        $"{nameof(encode)}.Length % ({nameof(fieldsize)} - 1) != 0 has failed!");

    foreach (var item in encode)
      if (item >= fieldsize)
        throw new ArgumentOutOfRangeException(nameof(fieldsize),
           $"{nameof(item)} in {nameof(encode)} >= {nameof(fieldsize)} has failed!");
  }

  private static void AssertFieldSize(int fieldsize, int idp)
  {
    AssertFieldSize(fieldsize);

    if (fieldsize >= idp)
      throw new ArgumentOutOfRangeException(nameof(fieldsize),
        $"{nameof(fieldsize)} >= {nameof(idp)} has failed!");
  }

  private static void AssertFieldSize(int fieldsize)
  {

    if (fieldsize < MIN_FIELD_SIZE)
      throw new ArgumentOutOfRangeException(
        nameof(fieldsize), $"{nameof(fieldsize)} < {MIN_FIELD_SIZE}!");

    if (!int.IsPow2(fieldsize)) throw new ArgumentOutOfRangeException(
      nameof(fieldsize), $"{nameof(fieldsize)} is not a Power of Two!");
  }

  private static void AssertFromMessage(ReadOnlySpan<byte> encode)
  {
    var enc = encode[6..].ToArray();
    ReadOnlySpan<byte> info = encode[..6].ToArray();

    var fieldsize = BitConverter.ToUInt16(info[..2]);
    var idp = BitConverter.ToUInt16(info.Slice(2, 2));
    var eccsize = BitConverter.ToUInt16(info.Slice(4, 2));
    var databuffersize = fieldsize - eccsize - 1;

    AssertFieldSize(enc, fieldsize);

    if (databuffersize + eccsize != fieldsize - 1)
      throw new ArgumentOutOfRangeException(nameof(encode),
        $"{nameof(databuffersize)} + {nameof(eccsize)} != ({nameof(fieldsize)} - 1) has failed!");

    if (eccsize > fieldsize >> 1)
      throw new ArgumentOutOfRangeException(nameof(encode),
        $"{nameof(eccsize)} > ({nameof(fieldsize)} / 2) has failed!");

    if (fieldsize > 2 && idp <= fieldsize)
      throw new ArgumentException(
        $"if fieldsize > 2, idp must be > fieldsize!", nameof(encode));
  }

  private static void AssertFromPackageDataStream(
    ReadOnlySpan<byte> input, int filelength)
  {
    if (input.Length != 7)
      throw new ArgumentOutOfRangeException(nameof(input));

    if (input[0] != 1 && input[0] != 2)
      throw new ArgumentOutOfRangeException(nameof(input));

    var idp = BitConverter.ToUInt16(input.Slice(3, 2));
    var eccsize = BitConverter.ToUInt16(input.Slice(4, 2));
    var fieldsize = BitConverter.ToUInt16(input.Slice(1, 2));
    //var datasize = (int)BitConverter.ToUInt32(input.Slice(1, 4));
    var databuffersize = fieldsize - eccsize - 1;

    AssertRSDecoding(fieldsize);

    if (filelength % (fieldsize - 1) != 0)
      throw new ArgumentOutOfRangeException(nameof(input),
        $"{nameof(filelength)} % ({nameof(fieldsize)} - 1) != 0 has failed!");

    if (databuffersize + eccsize != fieldsize - 1)
      throw new ArgumentException(
        $"{nameof(fieldsize)} has failed! ", nameof(input));

    if (idp <= fieldsize)
      throw new ArgumentOutOfRangeException(
        nameof(input), $"{nameof(idp)} has failde !");

  }
  #endregion Asserts
}
