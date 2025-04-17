
using System.Text;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ReedSolomonCodeTest;
 
using System.Security.Cryptography;
using michele.natale.ChannelCodings;

public class Program
{
  public static void Main()
  { 
    TestRSC();

    Console.WriteLine();
    Console.WriteLine("FINISH");
    Console.ReadLine();
  }

  private static void TestRSC()
  {

    TestRSEnDecodeErrors();
    TestRSEnDecodeErrorsULong();

    TestRSPackageData();
    TestRSPackageDataStream();

  }


  private static void TestRSEnDecodeErrors()
  {
    // fsize      ecc        err
    // 16         4-8        0-4
    // 32         4-16       0-8
    // 64         4-32       0-16
    // 128        4-64       0-32
    // .......

    //A very simple procedure.
    //The message and eccsize can simply be entered as they 
    //are in the “EncodingRS” method. ReedSolomonCode 
    //calculates and optimizes everything itself.

    //var msg = "Hallo World";
    var msg = "Hallo World - Reed Solomon Code";

    var message = Encoding.UTF8.GetBytes(msg);
    var msglength = message.Length;
    var eccsize = 4;

    var enc = RSEncoding.EncodingRS<byte>(message, eccsize, out var rsinfo);
    var dec = RSDecoding.DecodingRS<byte>(enc, rsinfo.FieldSize, rsinfo.EccSize);
    var newmessage = Encoding.UTF8.GetString([.. dec]);
    if (!msg.SequenceEqual(newmessage)) throw new Exception();
    var enccopy = enc.ToArray(); 


    //enc with errors
    //***************

    //Set various errors. Even if "SetRngErrors"
    //has no built-in errors, the repair will be done correctly. 
    var rss = rsinfo.FieldSize - 1;
    var cnt = enccopy.Length / rss;
    for (var i = 0; i < cnt; i++)
      SetRngErrors(enccopy.AsSpan(i * rss, rss), rsinfo.FieldSize, rsinfo.EccSize, rsinfo.ErrSizePerFs);

    //RSDecoding instance with fieldsize and ecc-size
    //After Decoding with enc
    var newdec = RSDecoding.DecodingRS<byte>(enc, rsinfo.FieldSize, rsinfo.EccSize);
    var newmsg = Encoding.UTF8.GetString([.. dec]);
    if (!dec.SequenceEqual(newdec)) throw new Exception();
    if (!msg.SequenceEqual(newmsg)) throw new Exception();

    Console.WriteLine($"{nameof(TestRSEnDecodeErrors)}: FINISH");
  }

  private static void TestRSEnDecodeErrorsULong()
  {
    // fsize      ecc        err
    // 16         4-8        0-4
    // 32         4-16       0-8
    // 64         4-32       0-16
    // 128        4-64       0-32
    // .......

    //Here a longer string is taken, which is simultaneously
    //cast into an array of ulong.

    var msg = "Hallo World - Reed Solomon Code";
    msg = MultString(msg, 10);

    var message = Encoding.UTF8.GetBytes(msg).Select(x => (ulong)x).ToArray();
    var msglength = message.Length;

    var enc = RSEncoding.EncodingRS<ulong>(message, 4, out var rsinfo);
    var dec = RSDecoding.DecodingRS<ulong>(enc, rsinfo.FieldSize, rsinfo.EccSize);
    var newmessage = Encoding.UTF8.GetString([.. dec.Select(x => (byte)x)]);
    if (!msg.SequenceEqual(newmessage)) throw new Exception();
    var enccopy = enc.ToArray();

    //enc with errors
    //***************

    //Set various errors. Even if "SetRngErrors"
    //has no built-in errors, the repair will be done correctly. 
    var rss = rsinfo.FieldSize - 1;
    var cnt = enccopy.Length / rss;
    for (var i = 0; i < cnt; i++)
      SetRngErrors(enccopy.AsSpan(i * rss, rss), rsinfo.FieldSize, rsinfo.EccSize, rsinfo.ErrSizePerFs);

    //RSDecoding instance with fieldsize and ecc-size
    //After Decoding with enc  
    var newdec = RSDecoding.DecodingRS<ulong>(enc, rsinfo.FieldSize, rsinfo.EccSize);
    var newmsg = Encoding.UTF8.GetString([.. dec.Select(x => (byte)x)]);
    if (!dec.SequenceEqual(newdec)) throw new Exception();
    if (!msg.SequenceEqual(newmsg)) throw new Exception();

    Console.WriteLine($"{nameof(TestRSEnDecodeErrorsULong)}: FINISH\n");
  }

  private static void TestRSPackageData()
  {
    // fsize      ecc        err
    // 16         4-8        0-4
    // 32         4-16       0-8
    // 64         4-32       0-16
    // 128        4-64       0-32
    // .......

    //A very simple procedure.
    //The Msg and a desired eccsize can simply be entered. It does
    //not matter which DataType the message represents. You can also
    //choose whether the result should be compressed. MessageData
    //calculates and optimizes everything itself. 


    var rand = Random.Shared;

    //var msg = "Hallo World";
    var msg = "Hallo World - Reed Solomon Code";

    var message = Encoding.UTF8.GetBytes(msg);

    var eccsize = rand.Next(4, 128 >> 1);
    var withcompress = int.IsEvenInteger(rand.Next());


    var enc = RSEncoding.ToPackageData<byte>(message, 4, withcompress);
    var dec = RSDecoding.FromPackageData<byte>(enc, out var rsinfo);

    var newmessage = Encoding.UTF8.GetString([.. dec]);
    if (!msg.SequenceEqual(newmessage)) throw new Exception();


    //And here is another ulong variant.
    //**********************************

    msg = MultString(msg, 30);
    var messageul = Encoding.UTF8.GetBytes(msg).Select(x => (ulong)x).ToArray();

    eccsize = rand.Next(4, 256 >> 1);
    withcompress = int.IsEvenInteger(rand.Next());

    var encul = RSEncoding.ToPackageData<ulong>(messageul, eccsize, withcompress);
    var decul = RSDecoding.FromPackageData<ulong>(encul, out var rsinfoul);

    var newmessageul = Encoding.UTF8.GetString([.. decul.Select(x => (byte)x)]);
    if (!msg.SequenceEqual(newmessageul)) throw new Exception();

    Console.WriteLine($"{nameof(TestRSPackageData)}: FINISH");
  }

  private static void TestRSPackageDataStream()
  {

    var src = "data2.txt";
    //var src = "data1.txt";
    var dest = "rsdata.txt";
    var newsrc = "newdata.txt";

    var rand = Random.Shared;

    if (src == "data2.txt")
      RngFileData(src, rand.Next(100,
        RSEncoding.MAX_DATA_SIZE_STREAM >> 7));


    var maxeccsize = RSEncoding.MAX_ECC_SIZE;

    //eccsize must always be at least 4.
    var eccsize = rand.Next(4, maxeccsize + 1);
    var with_compress = int.IsEvenInteger(rand.Next());

    //ToPackageDataStream
    //Note: You only need to specify the desired number
    //of ecc. PackageMessage calculates and optimizes the
    //rest itself.
    RSEncoding.ToPackageData(src, dest, eccsize, with_compress);
    RSDecoding.FromPackageData(dest, newsrc);
    if (!EqualFiles(src, newsrc)) throw new Exception();

    Console.WriteLine($"{nameof(TestRSPackageDataStream)}: FINISH");

  }

  private static void SetRngErrors(
    Span<byte> enc_msg, int fieldsize,
    int eccsize, int errsize = 4)
  {
    // fsize      ecc        err
    // 16         4-8        0-4
    // 32         4-16       0-8
    // 64         4-32       0-16
    // 128        4-64       0-32
    // .......

    if (errsize < 1) return;

    if (eccsize < 4 || errsize > eccsize)
      throw new ArgumentOutOfRangeException(nameof(eccsize));

    var rand = Random.Shared;
    var msgsize = enc_msg.Length - eccsize;
    var a = Enumerable.Range(0, msgsize).ToArray();
    var b = Enumerable.Range(msgsize, eccsize).ToArray();
    rand.Shuffle(a); rand.Shuffle(b);

    int cnt1 = 0, cnt2 = 0;
    while (cnt1 + cnt2 < errsize)
    {
      if (cnt2 < msgsize) cnt2++;
      if (cnt1 + cnt2 >= errsize) break;
      if (cnt1 < eccsize >> 1) cnt1++;
    }

    for (var i = 0; i < cnt1; i++)
      enc_msg[b[i]] = int.IsOddInteger(rand.Next()) ?
        byte.MinValue : (byte)rand.Next(1, fieldsize);

    for (var i = 0; i < cnt2; i++)
      enc_msg[a[i]] = int.IsOddInteger(rand.Next()) ?
        byte.MinValue : (byte)rand.Next(1, fieldsize);
  }


  private static string MultString(string str, int mul)
  {
    var result = new StringBuilder(mul * str.Length);
    for (var i = 0; i < mul; i++)
      result.Append(str);

    return result.ToString();
  }

  private static bool EqualFiles(string file_left, string file_right)
  {
    using var fsleft = new FileStream(file_left, FileMode.Open);
    using var fsright = new FileStream(file_right, FileMode.Open);
    return SHA512.HashData(fsleft).SequenceEqual(SHA512.HashData(fsright));
  }

  private static void RngFileData(string fielname, int size)
  {
    //var cd = Environment.CurrentDirectory;
    File.WriteAllBytes(fielname, RngInts<byte>(size));
  }

  private static T[] RngInts<T>(int size) where T : INumber<T>
  {
    var rand = Random.Shared;
    var tsz = Unsafe.SizeOf<T>();

    var result = new T[size];
    var bytes = new byte[size * tsz];

    rand.NextBytes(bytes);
    if (bytes.First() == 0) bytes[0]++;
    Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);

    return result;
  }


  private static string RngAlphaChars(int size)
  {
    var rand = Random.Shared;
    var alpha_numeric = "0123456789";
    var alpha_lower = "abcdefghijklmnopqrstuvwxyz";
    var alpha_upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    var alphas = alpha_lower + alpha_upper + alpha_numeric;

    return new string(rand.GetItems<char>(alphas, size));
  }
}
