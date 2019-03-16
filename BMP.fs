/// The code in this module is attempting to replicate the specification here:
/// https://en.wikipedia.org/wiki/BMP_file_format
/// and here:
/// http://www.ece.ualberta.ca/~elliott/ee552/studentAppNotes/2003_w/misc/bmp_file_format/bmp_file_format.htm
module BMP

open System
open System.IO

let grayscale fileName array =
    if File.Exists fileName then File.Delete fileName
    use out = File.OpenWrite fileName

    let width, height = Array2D.length1 array, Array2D.length2 array
    // each line width must be divisible by 4. 
    // as each pixel is three bytes, we need padding bytes
    let padding = 4 - ((width * 3) % 4) 
    let byteSize = ((width * 3) + padding) * height

    let as4bytes n = BitConverter.GetBytes (uint32 n)
    let as2bytes n = BitConverter.GetBytes (uint16 n)
    let zero = [0uy;0uy;0uy;0uy]

    // Format is header, size of dib, dib, pixel data
    // Dib is defined first as its size is part of the header (in the offset and total size fields)
    // and before the dib itself, where its actual size is specified.

    let dib = [
            // note the dib header size isn't here, as its dynamically calculated and added below
            yield! as4bytes width
            yield! as4bytes height
            yield! as2bytes 1           // planes (=1)
            yield! as2bytes 24          // bpp (24bit)
            yield! zero                 // BI_RGB, no pixel array compression used 
            yield! as4bytes byteSize    // Size of the raw bitmap data (including padding) 
            yield! as4bytes 2835        // 72dpi horizontal (calc in wiki article)
            yield! as4bytes 2835        // 72dpi vertical
            yield! zero                 // palette colours (0, not using a palette)
            yield! zero                 // important colours (0=all)
        ]

    let header = [
            yield! "BM" |> Seq.map byte                         // standard header for BMP
            yield! as4bytes (14 + 4 + dib.Length + byteSize)    // header is 14, 4 for dib size, then dib size, then pixels
            yield! zero                                         // reserved/unused
            yield! as4bytes (14 + 4 + dib.Length)               // offset to byte data
        ]

    [
        yield! header

        // the +4 is because the dibsize includes the 
        // size of this field containing the dib size
        yield! as4bytes (dib.Length + 4)

        yield! dib
        
        for y in [height-1..-1..0] do
            for x = 0 to width-1 do
                let v = array.[x, y]
                let gs = int (v * 255.) |> byte
                yield! [gs;gs;gs]
            yield! List.replicate padding 0uy
    ] |> Seq.iter out.WriteByte