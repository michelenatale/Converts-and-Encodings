


using System.Numerics;
using System.Diagnostics;

namespace BaseConverterTest;

using TestBaseConverter.BaseConverter;
using static michele.natale.Converters.Services;

public class Program
{
  public static void Main()
  {
    //UnitTest.Start();

    TestBaseConverter();
    TestBaseConverterStress1();
    TestBaseConverterStress2();

    TestBase2();
    TestBase8();
    TestBase16();

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
    Console.WriteLine($"BaseConverter hex: { string.Join("", hexs.Select(x => subs[x]))}");
    Console.WriteLine($"BaseConverter dec: { string.Join("", bnumber)}");
    Console.WriteLine();

    hexs = BaseConverterBigInteger.ToBaseX(number, basex);
    bnumber = BaseConverterBigInteger.FromBaseX(hexs, basex);
    Console.WriteLine($"BaseConverterBigInteger dec: {number}");
    Console.WriteLine($"BaseConverterBigInteger hex: {string.Join("", hexs.Select(x => subs[x]))}");
    Console.WriteLine($"BaseConverterBigInteger dec: {string.Join("", bnumber)}");
    Console.WriteLine(); 

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
}