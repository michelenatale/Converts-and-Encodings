

using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace BaseConverterTest;

using michele.natale.Converts;
using static michele.natale.Converters.Services;

public class Program
{
  public static void Main()
  {
    UnitTest.Start();

    TestBaseConverter();
    TestBaseConverterStress1();
    TestBaseConverterStress2();

    TestBase2();
    TestBase8();
    TestBase16();

    TestBaseConverterText1();
    TestBaseConverterText2();

    TestBaseConverterAlphabet256();

    Console.WriteLine();
    Console.WriteLine("FINISH");
    Console.ReadLine();
  }

  private static void TestBaseConverter()
  {

    Console.WriteLine($"{nameof(TestBaseConverter)}: ");
    Console.WriteLine("******************\n");

    Console.WriteLine($"{nameof(BaseConverter)}.Test: ");
    Console.WriteLine($"{nameof(BaseConverterBigInteger)}.Test: ");

    var rand = Random.Shared;
    var sw = Stopwatch.StartNew();

    var (startbase, targetbase) = RngBases();

    var rng = RngBytes(8, true);
    if (rng[^1] == 0) rng[^1] = (byte)rand.Next();
    if (rng[^1] == 0) rng[^1]++;

    var bi = new BigInteger(rng, true, false); //base 10
    var bibytes = TrimFirst(bi.ToByteArray(true, false)); //base 256

    //Notes: For this to work, the byte array of the
    //BigInteger must be entered into the converter
    //as a little-endian.
    var bytes = BaseConverter.Converter(
      bi.ToByteArray().Reverse().ToArray(), 256, 10); //base 10
    var bytes2 = bi.ToString().Select(x => (byte)(x - 48)).ToArray(); //base 10

    var sbase1 = BaseConverter.ToBaseX(bi, startbase);
    var sbase2 = BaseConverter.ToBaseX(bytes, startbase);
    var decipher1 = BaseConverter.FromBaseX(sbase1, startbase);

    var tbase1 = BaseConverter.Converter(sbase2, startbase, targetbase);
    var tbase2 = BaseConverter.ToBaseX(bytes, targetbase);
    var rbytes1 = BaseConverter.Converter(tbase1, targetbase, startbase);

    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 

    var sbase3 = BaseConverterBigInteger.ToBaseX(bi, startbase);
    var sbase4 = BaseConverterBigInteger.ToBaseX(bi.ToString().Select(x => (byte)(x - 48)).ToArray(), startbase);
    var decipher2 = BaseConverterBigInteger.FromBaseX(sbase3, startbase);

    var tbase3 = BaseConverterBigInteger.Converter(sbase3, startbase, targetbase);
    var tbase4 = BaseConverterBigInteger.ToBaseX(bytes, targetbase);
    var rbytes2 = BaseConverterBigInteger.Converter(tbase3, targetbase, startbase);

    Console.WriteLine($"t = {sw.ElapsedMilliseconds} ms\n");
  }

  private static void TestBaseConverterStress1()
  {
    Console.Write($"{nameof(TestBaseConverterStress1)}: ");

    var rand = Random.Shared;
    var sw = Stopwatch.StartNew();
    var (startbase, targetbase) = RngBases();

    var sz = 10 * 1024;
    var rng = RngBytes(sz, true);
    if (rng[^1] == 0) rng[^1] = (byte)rand.Next();
    if (rng[^1] == 0) rng[^1]++;

    var bi = new BigInteger(rng, true, false); //base 10
    var bibytes = TrimFirst(bi.ToByteArray(true, false)); //base 256

    //Notes: For this to work, the byte array of the
    //BigInteger must be entered into the converter
    //as a little-endian.
    var bytes = BaseConverter.Converter(
      bi.ToByteArray().Reverse().ToArray(), 256, 10); //base 10 
    var bytes2 = bi.ToString().Select(x => (byte)(x - 48)).ToArray(); //base 10

    var sbase1 = BaseConverter.ToBaseX(bi, startbase);
    var sbase2 = BaseConverter.ToBaseX(bytes, startbase);
    var decipher1 = BaseConverter.FromBaseX(sbase1, startbase);

    var tbase1 = BaseConverter.Converter(sbase2, startbase, targetbase);
    var tbase2 = BaseConverter.ToBaseX(bytes, targetbase);
    var rbytes1 = BaseConverter.Converter(tbase1, targetbase, startbase);

    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 

    var sbase3 = BaseConverterBigInteger.ToBaseX(bi, startbase);//base 10
    var sbase4 = BaseConverterBigInteger.ToBaseX(bi.ToString().Select(x => (byte)(x - 48)).ToArray(), startbase);
    var decipher2 = BaseConverterBigInteger.FromBaseX(sbase3, startbase);

    var tbase3 = BaseConverterBigInteger.Converter(sbase3, startbase, targetbase);
    var tbase4 = BaseConverterBigInteger.ToBaseX(bytes, targetbase);
    var rbytes2 = BaseConverterBigInteger.Converter(tbase3, targetbase, startbase);

    sw.Stop();

    Console.WriteLine($"startbase = {startbase}; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms; td = {sw.ElapsedMilliseconds / 2} ms\n");
  }

  private static void TestBaseConverterStress2()
  {

    Console.WriteLine($"{nameof(TestBaseConverterStress2)}: ");

    var sz = 20 * 1024; //20 KB 

    var (startbase, targetbase) = RngBases();
    var sbytes = RngBaseXNumber(sz, startbase);

    var sw = Stopwatch.StartNew();
    var basex1 = BaseConverter.Converter(sbytes, startbase, targetbase);
    var decipher1 = BaseConverter.Converter(basex1, targetbase, startbase);

    sw.Stop();
    Console.WriteLine($"BaseConverter: startbase = {startbase}; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms");

    sw = Stopwatch.StartNew();
    var basex2 = BaseConverterBigInteger.Converter(sbytes, startbase, targetbase);
    var decipher2 = BaseConverterBigInteger.Converter(basex2, targetbase, startbase);

    sw.Stop();
    Console.WriteLine($"BaseConverterBigInteger: startbase = {startbase}; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms\n");
  }

  private static void TestBase2()
  {
    Console.WriteLine($"{nameof(TestBase2)}: ");

    var basex = 2;
    var number = RngInt<uint>();

    var bits = BaseConverter.ToBaseX(number, basex);
    var bnumber = BaseConverter.FromBaseX(bits, basex);
    Console.WriteLine($"BaseConverter dec: {number}");
    Console.WriteLine($"BaseConverter bits: {string.Join("", bits)}");
    Console.WriteLine($"BaseConverter dec: {string.Join("", bnumber)}");
    Console.WriteLine();

    bits = BaseConverterBigInteger.ToBaseX(number, basex);
    bnumber = BaseConverterBigInteger.FromBaseX(bits, basex);
    Console.WriteLine($"BaseConverterBigInteger dec: {number}");
    Console.WriteLine($"BaseConverterBigInteger bits: {string.Join("", bits)}");
    Console.WriteLine($"BaseConverterBigInteger dec: {string.Join("", bnumber)}");
    Console.WriteLine();
  }

  private static void TestBase8()
  {
    Console.WriteLine($"{nameof(TestBase8)}: ");

    var basex = 8;
    var number = RngInt<uint>();

    var octs = BaseConverter.ToBaseX(number, basex);
    var bnumber = BaseConverter.FromBaseX(octs, basex);
    Console.WriteLine($"BaseConverter dec: {number}");
    Console.WriteLine($"BaseConverter oct: {string.Join("", octs)}");
    Console.WriteLine($"BaseConverter dec: {string.Join("", bnumber)}");
    Console.WriteLine();

    octs = BaseConverterBigInteger.ToBaseX(number, basex);
    bnumber = BaseConverterBigInteger.FromBaseX(octs, basex);
    Console.WriteLine($"BaseConverterBigInteger dec: {number}");
    Console.WriteLine($"BaseConverterBigInteger oct: {string.Join("", octs)}");
    Console.WriteLine($"BaseConverterBigInteger dec: {string.Join("", bnumber)}");
    Console.WriteLine();
  }

  private static void TestBase16()
  {
    Console.WriteLine($"{nameof(TestBase16)}: ");

    var basex = 16;
    var number = RngInt<uint>();
    var subs = "0123456789ABCDEF";

    var hexs = BaseConverter.ToBaseX(number, basex);
    var bnumber = BaseConverter.FromBaseX(hexs, basex);
    Console.WriteLine($"BaseConverter dec: {number}");
    Console.WriteLine($"BaseConverter hex: {string.Join("", hexs.Select(x => subs[x]))}");
    Console.WriteLine($"BaseConverter dec: {string.Join("", bnumber)}");
    Console.WriteLine();

    hexs = BaseConverterBigInteger.ToBaseX(number, basex);
    bnumber = BaseConverterBigInteger.FromBaseX(hexs, basex);
    Console.WriteLine($"BaseConverterBigInteger dec: {number}");
    Console.WriteLine($"BaseConverterBigInteger hex: {string.Join("", hexs.Select(x => subs[x]))}");
    Console.WriteLine($"BaseConverterBigInteger dec: {string.Join("", bnumber)}");
    Console.WriteLine();
  }

  private static void TestBaseConverterText1()
  {
    Console.WriteLine($"{nameof(TestBaseConverterText1)}: ");

    var sz = 1024;
    var rand = Random.Shared;
    var rng_str = RngAlphaText(sz);
    var targetbase = rand.Next(2, 255);//256 is startbase

    var startbase = targetbase;
    var sw = Stopwatch.StartNew();
    var encode1 = BaseConverter.ToUtf8BaseX(rng_str, targetbase);
    var decode1 = BaseConverter.FromUtf8BaseX(encode1, startbase);

    sw.Stop();
    Console.WriteLine($"BaseConverter Text: startbase = text256; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms");

    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 

    sw = Stopwatch.StartNew();
    var encode2 = BaseConverterBigInteger.ToUtf8BaseX(rng_str, targetbase);
    var decode2 = BaseConverterBigInteger.FromUtf8BaseX(encode2, startbase);

    sw.Stop();
    //if (!rng_str.SequenceEqual(decode1)) throw new Exception();
    //if (!decode1.SequenceEqual(decode2)) throw new Exception();

    Console.WriteLine($"BaseConverterBigInteger Text: startbase = text256; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms\n");
  }

  private static void TestBaseConverterText2()
  {
    Console.WriteLine($"{nameof(TestBaseConverterText2)}: ");

    var sz = 1024;
    var rand = Random.Shared;
    var rng_str = RngAlphaText(sz);
    var (startbase, targetbase) = RngBases();
    var bytes_base_256 = Encoding.UTF8.GetBytes(rng_str);
    if (bytes_base_256[0] == 0) bytes_base_256[0]++;

    var sw = Stopwatch.StartNew();
    var sbytes1 = BaseConverter.Converter(bytes_base_256, 256, startbase);
    var tbase1 = BaseConverter.Converter(sbytes1, startbase, targetbase);
    var decipher_sbytes_1 = BaseConverter.Converter(tbase1, targetbase, startbase);
    var rbytes1 = BaseConverter.Converter(tbase1, targetbase, 256);

    sw.Stop();
    Console.WriteLine($"BaseConverter Text: startbase = {startbase}; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms");

    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 

    sw = Stopwatch.StartNew();
    var sbytes2 = BaseConverterBigInteger.Converter(bytes_base_256, 256, startbase);
    var tbase2 = BaseConverterBigInteger.Converter(sbytes2, startbase, targetbase);
    var decipher_sbytes_2 = BaseConverterBigInteger.Converter(tbase2, targetbase, startbase);
    var rbytes2 = BaseConverterBigInteger.Converter(tbase2, targetbase, 256);

    sw.Stop();
    if (!rbytes1.SequenceEqual(rbytes2)) throw new Exception();
    Console.WriteLine($"BaseConverterBigInteger Text: startbase = {startbase}; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms\n");
  }

  private static void TestBaseConverterAlphabet256()
  {
    //The alphabetical compilation here consists of the German, Greek, Arabic,
    //Tifinagh, Glagolitic and Armenian alphabets. It is only intended as a
    //possibility to ensure appropriate substitutions as text.
    //It does not correspond to any standardization or the like.

    Console.WriteLine($"{nameof(TestBaseConverterAlphabet256)}: ");

    var sz = 1024;
    var rand = Random.Shared;
    var bytes = RngBytes(sz);
    int[] notbases = [2, 8, 16, 64,];
    var (startbase, targetbase) = RngBases();
    var alpha_dict = Alphabet256.Alphabet_256;
    var alpha_dict_r = Alphabet256.Alphabet_256R;

    while (notbases.Contains(startbase) || notbases.Contains(targetbase))
      (startbase, targetbase) = RngBases();

    var sw = Stopwatch.StartNew();
    var sbytes1a = BaseConverter.Converter(bytes, 256, startbase);
    var encode1 = new string([.. BaseConverter.Converter(sbytes1a, startbase, targetbase).Select(x => alpha_dict[x])]);
    var sbytes1b = BaseConverter.Converter(encode1.Select(c => alpha_dict_r[c]).ToArray(), targetbase, startbase);
    if (!sbytes1a.SequenceEqual(sbytes1b)) throw new Exception();

    var decode1 = BaseConverter.Converter(sbytes1b, startbase, 256);
    if (!bytes.SequenceEqual(decode1)) throw new Exception();

    sw.Stop();
    Console.WriteLine($"BaseConverter Text: startbase = {startbase}; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms");

    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 
    // *********** *********** *********** *********** *********** 

    sw = Stopwatch.StartNew();
    var sbytes2a = BaseConverterBigInteger.Converter(bytes, 256, startbase);
    var encode2 = new string([.. BaseConverterBigInteger.Converter(sbytes2a, startbase, targetbase).Select(x => alpha_dict[x])]);
    var sbytes2b = BaseConverterBigInteger.Converter(encode1.Select(c => alpha_dict_r[c]).ToArray(), targetbase, startbase);
    if (!sbytes2a.SequenceEqual(sbytes1b)) throw new Exception();

    var decode2 = BaseConverterBigInteger.Converter(sbytes2b, startbase, 256);
    if (!bytes.SequenceEqual(decode1)) throw new Exception();

    sw.Stop();
    Console.WriteLine($"BaseConverterBigInteger Text: startbase = {startbase}; targetbase = {targetbase}; size = {sz}; t = {sw.ElapsedMilliseconds} ms\n");
  }

  private static (int StartBase, int TargetBase) RngBases()
  {
    int targetbase;
    var rand = Random.Shared;
    var startbase = rand.Next(2, 256);

    while (true)
    {
      targetbase = rand.Next(2, 256);
      if (startbase != targetbase) break;
    }

    return (startbase, targetbase);
  }

  private static string RngAlphaText(int size)
  {
    var rand = Random.Shared;
    var alphatext = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    return new string(rand.GetItems<char>(alphatext, size));
  }
}