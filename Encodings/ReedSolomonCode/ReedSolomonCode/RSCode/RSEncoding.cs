
using System.Numerics;
using System.IO.Compression;
using System.Runtime.CompilerServices;

namespace michele.natale.ChannelCodings;

using Numerics;

/// <summary>
/// Provides tools and methods for encoding when dealing with Reed Solomon Code.
/// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
/// </summary>
public class RSEncoding
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
  /// Specifies the maximum memory size for ecc - Error Correcting Code.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_ECC_SIZE = (1 << MAX_EXP) >> 1; //128

  /// <summary>
  /// Specifies the maximum message data length.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_DATA_SIZE = 1 << 20; //1'048'576 as byte

  /// <summary>
  /// Specifies the maximum message data length for the stream variant.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public const int MAX_DATA_SIZE_STREAM = MAX_DATA_SIZE << 1; //2'097'152 as byte

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
  /// Indicates the current FieldSize.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public int FieldSize { get; private set; } = -1;

  /// <summary>
  /// Indicates the current Galoise Field.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  public GF2RS GField { get; private set; } = null!;

  #endregion Declarations

  #region C-Tor
  /// <summary>
  /// C-Tor 
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="fieldsize">Desired fieldsize</param>
  public RSEncoding(int fieldsize)
  {
    if (!RSAuthorInfo.IsMentionedAuthor)
      _ = RSAuthorInfo.AuthorInfo;

    AssertRSEncoding(fieldsize);
    var idp = GF2RS.ToIDPs[GF2RS.ToExponent((ushort)fieldsize)].First();
    this.GField = new GF2RS((ushort)fieldsize, idp);
    this.FieldSize = this.GField.Order;
  }

  /// <summary>
  /// C-Tor 
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="field">Desired filesize.</param>
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  public RSEncoding(GF2RS field, bool check_idp = false)
  {
    if (!RSAuthorInfo.IsMentionedAuthor)
      _ = RSAuthorInfo.AuthorInfo;

    if (check_idp)
      CheckIdp(field.Order, field.IDP);

    this.GField = field;
    this.FieldSize = this.GField.Order;
  }

  /// <summary>
  /// C-Tor 
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="fieldsize">Desired fieldsize</param>
  /// <param name="idp">Desired IDP - Irreducible Polynomial.</param>
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  public RSEncoding(int fieldsize, ushort idp, bool check_idp = false)
  {
    if (!RSAuthorInfo.IsMentionedAuthor)
      _ = RSAuthorInfo.AuthorInfo;

    AssertFieldSize(fieldsize, idp);
    if (check_idp) CheckIdp(fieldsize, idp);
    this.GField = new GF2RS((ushort)fieldsize, idp);
    this.FieldSize = this.GField.Order;
  }

  #endregion C-Tor

  #region Static Encoding EntryPoints  
  /// <summary>
  /// Calculates the Reed Solomon Code Encoding based on the desired parameters.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T">Desired Datatype</typeparam>
  /// <param name="message">Desired Message</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>
  /// <param name="rsinfo">Outputs information for the calculation of Reed Solomon Code.</param>
  /// <returns></returns>
  public static byte[] EncodingRS<T>(
    ReadOnlySpan<T> message, int eccsize, out RsInfo rsinfo)
      where T : INumber<T>
  {
    var mdata = ToBytes(message);
    var mlength = mdata.Length;
    var blength = BitConverter.GetBytes(mlength);

    var (fieldsize, databuffersize, errsizeperfs, cnt) =
         ParamOptimizerEcc(mdata, blength, eccsize);

    var idp = GF2RS.ToIDPs[GF2RS.ToExponent((ushort)fieldsize)].First();
    rsinfo = ToRsinfo(fieldsize, idp, eccsize, errsizeperfs, databuffersize);

    AssertEncodingRS(mdata, fieldsize,
      databuffersize, eccsize, errsizeperfs, cnt);

    var rsenc = new RSEncoding(fieldsize, idp);

    return ToEncode(mdata, blength, rsenc, databuffersize, cnt);
  }

  /// <summary>
  /// Calculates the Reed Solomon Code Encoding based on the desired parameters.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T">Desired DataType</typeparam>
  /// <param name="message">Desired Message</param>
  /// <param name="field">Desired Galois Field</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>
  /// <param name="rsinfo">Outputs information for the calculation of Reed Solomon Code.</param>
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  /// <returns></returns>
  public static byte[] EncodingRS<T>(
    ReadOnlySpan<T> message, GF2RS field,
    int eccsize, out RsInfo rsinfo, bool check_idp = false)
      where T : INumber<T>
  {
    var mdata = ToBytes(message);
    var mlength = mdata.Length;
    var blength = BitConverter.GetBytes(mlength);

    var errsizeperfs = eccsize >> 1;
    var fieldsize = (int)field.Order;
    var databuffersize = fieldsize - eccsize - 1;
    var cnt = (mdata.Length + 4) / databuffersize;
    if ((cnt * databuffersize - mdata.Length) < 4) cnt++;

    var idp = (int)field.IDP;
    rsinfo = ToRsinfo(fieldsize, idp,
      eccsize, errsizeperfs, databuffersize);

    AssertEncodingRS(mdata, fieldsize,
      databuffersize, eccsize, errsizeperfs, cnt);

    var rsenc = new RSEncoding(field, check_idp);

    return ToEncode(mdata, blength, rsenc, databuffersize, cnt);
  }

  /// <summary>
  /// Calculates the Reed Solomon Code Encoding based on the desired parameters.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T">Desired DataType</typeparam>
  /// <param name="message">Desired Message</param>
  /// <param name="fieldsize">Desired fieldsize</param>
  /// <param name="idp">Desired IDP - Irreducible Polynomial.</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>
  /// <param name="rsinfo">Outputs information for the calculation of Reed Solomon Code.</param>
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  /// <returns></returns>
  public static byte[] EncodingRS<T>(
    ReadOnlySpan<T> message, int fieldsize, int idp,
    int eccsize, out RsInfo rsinfo, bool check_idp = false)
      where T : INumber<T> =>
        EncodingRS(message, new GF2RS((ushort)fieldsize,
          (ushort)idp), eccsize, out rsinfo, check_idp);

  #endregion Static Encoding EntryPoints 


  #region Encoding
  /// <summary>
  /// Calculates the Reed Solomon Code Encoding based on the desired parameters.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="data">Desired DataBuffer</param>
  /// <returns></returns>
  public byte[] Encoding(ReadOnlySpan<byte> data)
  {
    //data muss schon den datapuffersize haben!!

    AssertEncoding(data, this.FieldSize);
    var eccsize = this.FieldSize - data.Length - 1;

    var length = data.Length;
    var ecc = new byte[eccsize];
    var multiplier = this.ToMultiplier(data.Length);

    byte r, ecc0 = 0;
    for (var i = 0; i < length - 1; i++)
    {
      r = (byte)(ecc0 ^ data[^(i + 1)]);
      for (var j = 0; j < ecc.Length; j++)
        //Multiplication and Subtraction
        ecc[j] ^= this.GField.Multiply(multiplier[j], r);

      ecc0 = ecc.Last();
      for (var j = 0; j < ecc.Length - 1; j++)
        ecc[^(j + 1)] = ecc[^(j + 2)]; //Shift

      ecc[0] = 0;//Set Zero
    }
    r = (byte)(ecc0 ^ data[0]);

    length = ecc.Length;
    for (var j = 0; j < length; j++)
      //Multiplication and Subtraction
      ecc[j] ^= this.GField.Multiply(multiplier[j], r);

    var result = new byte[data.Length + ecc.Length];
    Array.Copy(data.ToArray(), result, data.Length);
    Array.Copy(ecc, 0, result, data.Length, ecc.Length);

    return result;
  }

  private static byte[] ToEncode(
    ReadOnlySpan<byte> mdata, ReadOnlySpan<byte> bytes,
    RSEncoding rsenc, int databuffersize, int cnt)
  {
    var fieldsize = (int)rsenc.GField.Order;

    var rslength = fieldsize - 1;
    var mlength = mdata.Length;
    var blength = bytes.ToArray();
    var result = new byte[cnt * rslength];
    var databuffer = new byte[databuffersize];
    if (mlength + 4 < databuffersize)
    {
      mdata.CopyTo(databuffer);
      BitConverter.GetBytes(mlength)
        .CopyTo(databuffer, databuffer.Length - 4);
      return rsenc.Encoding(databuffer);
    }
    else
    {
      var i = 0;
      for (i = 0; i < cnt - 1; i++)
      {
        var l = mdata.Length - i * databuffersize;
        var len = l < databuffersize ? l : databuffersize;
        if (len == databuffersize)
          Array.Copy(rsenc.Encoding(
          mdata.Slice(i * databuffersize, len)),
            0, result, i * rslength, rslength);
        else
        {
          mdata.Slice(i * databuffersize, len).CopyTo(databuffer);
          Array.Copy(rsenc.Encoding(databuffer),
            0, result, i * rslength, rslength);
          Array.Clear(databuffer);
        }
      }
      var d = mdata.ToArray().Skip((cnt - 1) * databuffersize).ToArray();
      Array.Copy(d, databuffer, d.Length);
      Array.Copy(blength, 0, databuffer, databuffer.Length - 4, 4);
      Array.Copy(rsenc.Encoding(databuffer), 0, result, (cnt - 1) * rslength, rslength);

      return result;
    }
  }

  #endregion Encoding

  #region PackageData

  #region PackageData Message
  /// <summary>
  /// Calculates the PackageData, for easy handling of saving and sending.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T">Desired DataType</typeparam>
  /// <param name="message">Desired Message</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>
  /// <param name="with_compress">Yes, if the data is to be compressed, otherwise false.</param>
  /// <returns></returns>
  public static byte[] ToPackageData<T>(
    ReadOnlySpan<T> message, int eccsize,
    bool with_compress = false)
      where T : INumber<T> => 
        ToPackageData(EncodingRS(message, eccsize,
          out var rsinfo), rsinfo, with_compress);

  /// <summary>
  /// Calculates the PackageData, for easy handling of saving and sending.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T">Desired DataType</typeparam>
  /// <param name="message">Desired Message</param>
  /// <param name="field">Desired Galois Field</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param> 
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  /// <param name="with_compress">Yes, if the data is to be compressed, otherwise false.</param>
  /// <returns></returns>
  public static byte[] ToPackageData<T>(
    ReadOnlySpan<T> message, GF2RS field, int eccsize,
    bool check_idp = false, bool with_compress = false)
      where T : INumber<T> =>
        ToPackageData(EncodingRS(message, field, eccsize,
          out var rsinfo, check_idp), rsinfo, with_compress);

  /// <summary>
  /// Calculates the PackageData, for easy handling of saving and sending.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T">Desired DataType</typeparam>
  /// <param name="message">Desired Message</param>
  /// <param name="fieldsize">Desired fieldsize</param>
  /// <param name="idp">Desired IDP - Irreducible Polynomial.</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param> 
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  /// <param name="with_compress">Yes, if the data is to be compressed, otherwise false.</param>
  /// <returns></returns>
  public static byte[] ToPackageData<T>(
    ReadOnlySpan<T> message, int fieldsize, int idp, int eccsize,
    bool check_idp = false, bool with_compress = false)
      where T : INumber<T> =>
        ToPackageData(EncodingRS(message, fieldsize, idp, eccsize,
          out var rsinfo, check_idp), rsinfo, with_compress);

  /// <summary>
  /// Calculates the PackageData, for easy handling of saving and sending.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="encode">Desired Message</param>
  /// <param name="rsinfo">Desired RSINFO</param>
  /// <param name="with_compress">Yes, if the data is to be compressed, otherwise false.</param>
  /// <returns></returns>
  public static byte[] ToPackageData(
    ReadOnlySpan<byte> encode, RsInfo rsinfo, bool with_compress)
  {
    AssertPackageData(encode, rsinfo);
    var ip = BitConverter.GetBytes((ushort)rsinfo.Idp).ToArray();
    var ec = BitConverter.GetBytes((ushort)rsinfo.EccSize).ToArray();
    var fs = BitConverter.GetBytes((ushort)rsinfo.FieldSize).ToArray();

    var result = new byte[6 + encode.Length];
    Array.Copy(fs, result, 2);
    Array.Copy(ip, 0, result, 2, 2);
    Array.Copy(ec, 0, result, 4, 2);
    encode.CopyTo(result.AsSpan(6));

    if (with_compress)
      return Concat((byte)2, Compress(result));

    return Concat((byte)1, result);
  }
  #endregion PackageData Message
  #region PackageData Stream

  /// <summary>
  /// Calculates the PackageData Stream, for easy handling of saving and sending.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="src">Desired FileSource</param>
  /// <param name="dest">Desired FileDestination</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>  
  /// <param name="with_compress">Yes, if the data is to be compressed, otherwise false.</param>
  public static void ToPackageData(
      string src, string dest,
      int eccsize, bool with_compress = false)
  {
    using var fsin = new FileStream(
      src, FileMode.Open, FileAccess.Read);

    var blength = BitConverter.GetBytes((int)fsin.Length);
    var (fieldsize, databuffersize, errsize, cnt) =
      ParamOptimizerEcc([255], blength, eccsize);

    var rsinfo = new RsInfo()
    {
      EccSize = eccsize,
      FieldSize = fieldsize,
      ErrSizePerFs = errsize,
      DataBufferSize = databuffersize,
      Idp = GF2RS.ToIDPs[ToExponent(fieldsize)].First(),
    };

    ToPackageData(src, dest, rsinfo, cnt, false, with_compress);
  }

  /// <summary>
  /// Calculates the PackageData Stream, for easy handling of saving and sending.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="src">Desired FileSource</param>
  /// <param name="dest">Desired FileDestination</param>
  /// <param name="field">Desired GF2RSX - Galois Field</param> 
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>  
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  /// <param name="with_compress">Yes, if the data is to be compressed, otherwise false.</param>
  public static void ToPackageData(
    string src, string dest, GF2RS field, int eccsize,
    bool check_idp = false, bool with_compress = false)
  {
    using var fsin = new FileStream(
      src, FileMode.Open, FileAccess.Read);

    var blength = BitConverter.GetBytes((int)fsin.Length);
    var (fieldsize, databuffersize, errsize, cnt) =
      ParamOptimizerEcc([(byte)(field.Order - 1)], blength, eccsize);

    var rsinfo = new RsInfo()
    {
      Idp = field.IDP,
      EccSize = eccsize,
      FieldSize = fieldsize,
      ErrSizePerFs = errsize,
      DataBufferSize = databuffersize,
    };

    ToPackageData(src, dest, rsinfo, cnt, check_idp, with_compress);
  }

  /// <summary>
  /// Calculates the PackageData Stream, for easy handling of saving and sending.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="src">Desired FileSource</param>
  /// <param name="dest">Desired FileDestination</param> 
  /// <param name="fieldsize">Desired FieldSize</param>
  /// <param name="idp">Desiered IDP - Irreducible Polynomial</param>
  /// <param name="eccsize">Desired size of ecc - Error Correcting Code.</param>  
  /// <param name="check_idp">Yes, if the IDP - Irreducible Polynomial is to be checked, otherwise false.</param>
  /// <param name="with_compress">Yes, if the data is to be compressed, otherwise false.</param>
  public static void ToPackageData(
    string src, string dest, int fieldsize, int idp, int eccsize,
    bool check_idp = false, bool with_compress = false)
  {
    var field = new GF2RS((ushort)fieldsize, (ushort)idp);
    ToPackageData(src, dest, field, eccsize, check_idp, with_compress);
  }

  private static void ToPackageData(
      string src, string dest, RsInfo rsinfo, int cnt,
      bool check_idp = false, bool with_compress = false)
  {
    AssertPackageData(src, dest, rsinfo, true);

    using var mss = new MemoryStream();
    using var fsin = new FileStream(src, FileMode.Open, FileAccess.Read);
    using var fsout = new FileStream(dest, FileMode.Create, FileAccess.Write);

    AssertFieldSize(fsin, rsinfo.FieldSize);

    var idp = rsinfo.Idp;
    var blength = fsin.Length;
    var eccsize = rsinfo.EccSize;
    var fieldsize = rsinfo.FieldSize;
    var errsize = rsinfo.ErrSizePerFs;
    var databuffersize = rsinfo.DataBufferSize;
    var sz = BitConverter.GetBytes((int)blength);

    var pcode = 6;
    var readsize = 0;
    mss.Write(new byte[pcode]);

    var mlength = fieldsize - 1;
    var buffer = new byte[databuffersize];
    var rsenc = new RSEncoding(fieldsize, (ushort)idp, check_idp);

    var count = 0;
    fsin.Position = 0;
    while ((readsize = fsin.Read(buffer)) > 0)
    {
      if (count == cnt - 1)
        Array.Copy(sz, 0, buffer, buffer.Length - 4, 4);
      var enc = rsenc.Encoding(buffer);
      mss.Write(enc);
      Array.Clear(buffer);
      count++;
    }

    if (count < cnt)
    {
      Array.Clear(buffer);
      Array.Copy(sz, 0, buffer, buffer.Length - 4, 4);
      var enc = rsenc.Encoding(buffer);
      mss.Write(enc);
    }

    var ec = BitConverter.GetBytes((ushort)eccsize);
    var id = BitConverter.GetBytes(rsenc.GField.IDP);
    var fs = BitConverter.GetBytes((ushort)fieldsize);

    mss.Position = 0; fsout.Position = 0;
    mss.Write(fs); mss.Write(id); mss.Write(ec); mss.Position = 0;

    if (with_compress)
    {
      fsout.WriteByte(2);

      using var gzip = new GZipStream(fsout, CompressionMode.Compress);
      mss.CopyTo(gzip);
      return;
    }

    fsout.WriteByte(1);
    mss.CopyTo(fsout);
  }
  #endregion PackageData Stream

  #endregion PackageData


  #region Methods
  private byte[] ToMultiplier(int databuffersize)
  {
    var count = this.FieldSize - databuffersize - 1;
    var result = new byte[] { this.GField.Exp[0], 1 };

    for (var i = 1; i < count; i++)
      result = this.PolyMult(result, [this.GField.Exp[i], 1]);

    return result;
  }

  private byte[] PolyMult(byte[] left, byte[] right)
  {
    var result = new byte[left.Length + right.Length - 1];

    for (var i = 0; i < left.Length; i++)
      for (var j = 0; j < right.Length; j++)
      {
        var coeff = this.GField.Multiply(left[i], right[j]);
        result[i + j] = (byte)(result[i + j] ^ coeff);
      }

    return result;
  }

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
    if (number >= (1 << GF2RS.MAX_EXP) - 1) return 1 << GF2RS.MAX_EXP;
    return NextPowerOfTwo(number);
  }

  private static (int FieldSize, int DataBufferSize, int ErrSize, int Cnt) ParamOptimizerEcc(
    ReadOnlySpan<byte> message, byte[] blength, int eccsize = 4)
  {
    if (eccsize < 4) eccsize = 4;

    var fsmax = MAX_FIELD_SIZE;
    var mlength = BitConverter.ToInt32(blength);
    var datalength = Math.Max(mlength + 4, message.Length + 4); //Length as byte
    var fsmin = eccsize << 1 >= MAX_FIELD_SIZE ? MAX_FIELD_SIZE : NextFieldSize(eccsize << 1);
    if (fsmin < MIN_FIELD_SIZE) fsmin = MIN_FIELD_SIZE;

    var maxnumber = message.ToArray().Max();
    if (maxnumber > fsmin) fsmin = NextFieldSize(maxnumber);
    if (maxnumber == fsmin) fsmin = NextFieldSize(maxnumber + 1);

    maxnumber = blength.ToArray().Max();
    if (maxnumber > fsmin) fsmin = NextFieldSize(maxnumber);
    if (maxnumber == fsmin) fsmin = NextFieldSize(maxnumber + 1);

    var ecc = eccsize;
    var opt = int.MaxValue;
    var mecc = fsmin >> 1; //max
    int dbuffersize = 0, cnt = 0;
    for (var i = fsmin; i <= fsmax; i <<= 1)    // fieldsize
      for (var j = ecc; j <= mecc; j++)         // ecc
      {
        var m = i - j - 1;
        var k = datalength / m;
        if (datalength % m != 0) k++;
        var o = k * (m + j);
        if (o < opt)
        {
          opt = o; fsmin = i;
          ecc = j; dbuffersize = m; cnt = k;
        }
      }

    return (fsmin, dbuffersize, ecc >> 1, cnt);
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

  /// <summary>
  /// Calculates the exponent from the FieldSize.
  /// </summary>
  /// <param name="fieldsize">Desired FieldSize</param>
  /// <returns></returns>
  public static int ToExponent(int fieldsize) =>
    GF2RS.ToExponent((ushort)fieldsize);
  #endregion Methods

  #region Utils
  private static T[] Concat<T>(T left, T[] right)
    where T : INumber<T> => Concat([left], right);

  private static T[] Concat<T>(T[] left, T[] right)
    where T : INumber<T>
  {
    var result = new T[left.Length + right.Length];
    Array.Copy(left, result, left.Length);
    Array.Copy(right, 0, result, left.Length, right.Length);

    return result;
  }

  /// <summary>
  /// Calculates the Byte Array from an Array of T
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <typeparam name="T">Desired DataType</typeparam>
  /// <param name="input">Desired Array of T</param>
  /// <returns>Array of byte</returns>
  public static byte[] ToBytes<T>(ReadOnlySpan<T> input) where T : INumber<T>
  {
    var length = input.Length;
    var tsz = Unsafe.SizeOf<T>();
    var result = new byte[tsz * length];
    Buffer.BlockCopy(input.ToArray(), 0, result, 0, tsz * length);

    return result;
  }

  #endregion Utils

  #region Compress 
  /// <summary>
  /// Compresses the data from a array of byte.
  /// <para>Powered by <see href="https://github.com/michelenatale">© Michele Natale 2025</see></para>  
  /// </summary>
  /// <param name="input">Desired Data from a Array of byte</param>
  /// <returns></returns>
  private static byte[] Compress(ReadOnlySpan<byte> input)
  {
    var data = input.ToArray();
    if (input.Length == 0) return data;

    using var msout = new MemoryStream();
    {
      using var msin = new MemoryStream([.. data]);
      using var gzip = new GZipStream(
        msout, CompressionMode.Compress, false);
      msin.CopyTo(gzip);
    }
    return msout.ToArray();
  }
  #endregion Compress

  #region Asserts

  private static void AssertEncoding(
    ReadOnlySpan<byte> databuffer, int fieldsize)
  {
    foreach (var item in databuffer)
      if (item >= fieldsize)
        throw new ArgumentOutOfRangeException(nameof(fieldsize),
           $"{nameof(item)} in {nameof(databuffer)} >= {nameof(fieldsize)} has failed!");

    if (databuffer.Length > MAX_DATA_SIZE)
      throw new ArgumentOutOfRangeException(nameof(databuffer),
        $"{nameof(databuffer)}.Length as Bytes > {MAX_DATA_SIZE} has failed!\n" +
        $"Please use Stream!");

    if (databuffer.Length == 0)
      throw new ArgumentOutOfRangeException(nameof(databuffer),
        $"{nameof(databuffer)}.Length == 0 has failed!");

    var eccsize = fieldsize - databuffer.Length - 1;

    if (eccsize < 4 || eccsize > fieldsize >> 1)
      throw new ArgumentOutOfRangeException(nameof(databuffer),
          $"{nameof(eccsize)} < 4 or {nameof(eccsize)} > {nameof(fieldsize)} has failed!");

    //if (databuffer.Length + eccsize != fieldsize - 1)
    //  throw new ArgumentOutOfRangeException(
    //    nameof(databuffer), $"{nameof(databuffer)}.Length + {nameof(eccsize)} != {nameof(fieldsize)} - 1!");
  }

  private static void AssertRSEncoding(int fieldsize) =>
    AssertFieldSize(fieldsize);

  private static void AssertEncodingRS(
    ReadOnlySpan<byte> message, int fieldsize,
    int databuffersize, int eccsize, int errsizeperfs, int cnt)
  {

    AssertFieldSize(message, fieldsize);// ispo2, msg-max .. check

    ArgumentOutOfRangeException.ThrowIfLessThan(eccsize, 4, nameof(eccsize));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(eccsize, fieldsize >> 1, nameof(eccsize));

    ArgumentOutOfRangeException.ThrowIfLessThan(errsizeperfs, 0, nameof(errsizeperfs));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(errsizeperfs, eccsize >> 1, nameof(errsizeperfs));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(errsizeperfs, fieldsize >> 2, nameof(errsizeperfs));

    if (databuffersize + eccsize != fieldsize - 1)
      throw new ArgumentOutOfRangeException(nameof(databuffersize),
        $"{nameof(databuffersize)} + {nameof(eccsize)} != {nameof(fieldsize)} - 1 has failed! ");

    if (cnt * databuffersize < message.Length + 4)
      //It must still be possible to write the length of the message to the databuffer.
      throw new ArgumentOutOfRangeException(nameof(databuffersize),
        $"{nameof(cnt)} * {nameof(databuffersize)} < {nameof(message)}.Length + 4 has failed! ");
  }

  private static void CheckIdp(int fieldsize, ushort idp)
  {
    //Fieldsize was checked beforehand to see whether it is a Power Of Two.

    var result = 0;
    foreach (var item in GF2RS.ToIDPs[GF2RS.ToExponent((ushort)fieldsize)])
      if (idp == item) { result++; break; }

    if (result == 0) throw new ArgumentException(
      $"{nameof(idp)} is not valid!", nameof(idp));
  }

  private static void AssertPackageData(
    ReadOnlySpan<byte> data, RsInfo rsinfo)
  {
    if (data.Length > MAX_DATA_SIZE)
      throw new ArgumentOutOfRangeException(nameof(data),
        $"{nameof(data)}.Length > {MAX_DATA_SIZE} has failed!");

    if (data.Length == 0)
      throw new ArgumentOutOfRangeException(nameof(data),
        $"{nameof(data)}.Length == 0 has failed!");

    ArgumentNullException.ThrowIfNull(rsinfo);

    AssertParameters(rsinfo.FieldSize, rsinfo.Idp,
      rsinfo.DataBufferSize, rsinfo.EccSize, rsinfo.ErrSizePerFs);
  }

  private static void AssertPackageData(
    string src, string dest, RsInfo rsinfo, bool delete_dest = true)
  {
    AssertPackageData(src, dest, delete_dest);
    ArgumentNullException.ThrowIfNull(rsinfo);

    AssertParameters(rsinfo.FieldSize, rsinfo.Idp,
      rsinfo.DataBufferSize, rsinfo.EccSize, rsinfo.ErrSizePerFs);
  }

  private static void AssertPackageData(
    string src, string dest, bool delete_dest)
  {
    //For Streams

    if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(dest))
      throw new ArgumentNullException(nameof(src),
        $"{nameof(src)} or {nameof(dest)} has failed!");

    if (delete_dest && File.Exists(dest)) File.Delete(dest);
    if (!File.Exists(src)) throw new FileNotFoundException(src);

    var file_length = 0L;
    using (var fs = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.Read))
      file_length = fs.Length;

    if (file_length > MAX_DATA_SIZE_STREAM)
      throw new ArgumentOutOfRangeException(nameof(src),
        $"{nameof(file_length)} > {MAX_DATA_SIZE_STREAM} has failed!");

    if (file_length == 0)
      throw new ArgumentOutOfRangeException(nameof(src),
        $"{nameof(file_length)} == 0 has failed!");

  }

  private static void AssertParameters(
    int fieldsize, int idp, int databuffersize, int eccsize, int errsizeperfs)
  {
    AssertFieldSize(fieldsize);
    ArgumentOutOfRangeException.ThrowIfLessThan(eccsize, 4, nameof(eccsize));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(eccsize, fieldsize >> 1, nameof(eccsize));

    ArgumentOutOfRangeException.ThrowIfLessThan(errsizeperfs, 0, nameof(errsizeperfs));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(errsizeperfs, eccsize >> 1, nameof(errsizeperfs));
    ArgumentOutOfRangeException.ThrowIfGreaterThan(errsizeperfs, fieldsize >> 2, nameof(errsizeperfs));

    if (databuffersize + eccsize != fieldsize - 1)
      throw new ArgumentOutOfRangeException(nameof(databuffersize),
        $"{nameof(databuffersize)} + {nameof(eccsize)} != {nameof(fieldsize)} - 1 has failed! ");

    if (fieldsize >= idp)
      throw new ArgumentOutOfRangeException(nameof(fieldsize),
        $"{nameof(fieldsize)} >= {nameof(idp)} has failed!");
  }

  private static void AssertFieldSize(ReadOnlySpan<byte> message, int fieldsize)
  {
    AssertFieldSize(fieldsize);

    foreach (var item in message)
      if (item >= fieldsize)
        throw new ArgumentOutOfRangeException(nameof(fieldsize),
           $"{nameof(item)} in {nameof(message)} >= {nameof(fieldsize)} has failed!");
  }

  private static void AssertFieldSize(Stream stream, int fieldsize)
  {
    AssertFieldSize(fieldsize);
    var length = stream.Length;
    while (stream.Position < length)
      if (stream.ReadByte() >= fieldsize)
        throw new ArgumentOutOfRangeException(nameof(fieldsize),
           $"stream_byte in {nameof(stream)} >= {nameof(fieldsize)} has failed!");
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

    if (fieldsize > MAX_FIELD_SIZE)
      throw new ArgumentOutOfRangeException(
        nameof(fieldsize), $"{nameof(fieldsize)} > {MAX_FIELD_SIZE}!");

    if (!int.IsPow2(fieldsize)) throw new ArgumentOutOfRangeException(
      nameof(fieldsize), $"{nameof(fieldsize)} is not a Power of Two!");
  }
  #endregion Asserts
}
