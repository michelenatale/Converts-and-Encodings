
# Reed Solomon Code

Reed-Solomon coding offers an efficient and robust method for error correction, which is essential for data transmission over extremely long distances, e.g. in space. 

Reed-Solomon codes were first used in [NASA's](https://en.wikipedia.org/wiki/NASA) [Voyager program](https://en.wikipedia.org/wiki/Voyager_program) in 1977 and found their first commercial application in 1982 in the error correction of [compact disks](https://en.wikipedia.org/wiki/Compact_disc). Today's applications cover a wide range, such as the DVB standard for transmitting digital television signals, various [mobile phone standards](https://en.wikipedia.org/wiki/Mobile_telephony), [digital audio broadcasting (DAB)](https://de.wikipedia.org/wiki/Digital_Audio_Broadcasting), [RAID-6](https://en.wikipedia.org/wiki/RAID_6) systems and file formats such as [PAR2](https://en.wikipedia.org/wiki/Parchive#Par2) for data storage. Other application examples are [two-dimensional barcodes](https://de.wikipedia.org/wiki/2D-Code); for example, the [QR code](https://en.wikipedia.org/wiki/QR_code), [DataMatrix](https://en.wikipedia.org/wiki/Data_Matrix), [Aztec Code](https://en.wikipedia.org/wiki/Aztec_Code) and [PDF417](https://en.wikipedia.org/wiki/PDF417) use Reed-Solomon to correct reading errors.  
 
## Applying the ReedSolomonCode:
The ReedSolomonCode offers 3 options.
 - The Reed Solomon encoding
 - The Reed Solomon decoding
 - The creation of a PackageData for easier handling when saving or sending locally.
 
ReedSolomonCode is designed to be very easy to use for encoding. The message does not have to be adapted to the size of the data buffer first, but can be entered directly. The same applies to decoding.
 
PackageData is also very easy to use and stores all the information for subsequent resolution in the result.  
 
There is a [test file](https://github.com/michelenatale/Converts-and-Encodings/blob/main/Encodings/ReedSolomonCode/TestReedSolomonCode/Program.cs) available which shows how to use ReedSolomonCode.
 
Here is some code for encoding and decoding:
```
var msg = "Hallo World - Reed Solomon Code";
 
var message = Encoding.UTF8.GetBytes(msg);
 
var eccsize = 4;
var enc = RSEncoding.EncodingRS<byte>(message, eccsize, out var rsinfo);
 
var dec = RSDecoding.DecodingRS<byte>(enc, rsinfo.FieldSize, rsinfo.EccSize);
 
var newmessage = Encoding.UTF8.GetString([.. dec]);
 
// newmessage = "Hallo World - Reed Solomon Code"

```
