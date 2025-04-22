## BaseConverter

The BaseConverter converts a number of any base into a number of another base. And the whole thing back again. 

The possible [numeral system](https://en.wikipedia.org/wiki/Numeral_system) here are 2 - 256, as the converter is based on bytes. If you want to have even larger bases, the converter must be modified.

There are two different methods here. The first is a converter that performs the calculation from left to right and is very fast. The second converter is the typical classic converter with Biginteger.  

## Applying the BaseConverter:
There is a [test file](https://github.com/michelenatale/Converts-and-Encodings/blob/main/Converts/BaseConverter/TestBaseConverter/Program.cs) that shows how to use BaseConverter.

Here is some code for encoding and decoding:
```
var basex = 8;
var number = 123456u;

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

```
