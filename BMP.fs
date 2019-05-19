/// The code in this module is attempting to replicate the specification here:
/// https://en.wikipedia.org/wiki/BMP_file_format
/// and here:
/// http://www.ece.ualberta.ca/~elliott/ee552/studentAppNotes/2003_w/misc/bmp_file_format/bmp_file_format.htm
module BMP

open System

let grayscale array =
    let width, height = Array2D.length1 array, Array2D.length2 array
    // each line width must be divisible by 4. 
    // as each pixel is three bytes, we need padding bytes
    let padding = 4 - ((width * 3) % 4) 
    let byteSize = ((width * 3) + padding) * height

    let as4bytes n = BitConverter.GetBytes (uint32 n)
    let as2bytes n = BitConverter.GetBytes (uint16 n)
    let zeroIn4bytes = [|0uy;0uy;0uy;0uy|]

    // Format is header, dib, pixel data
    
    let header = 
        [|
            "BM" |> Seq.map byte |> Seq.toArray // standard header for BMP
            as4bytes (14 + 40 + byteSize)       // header is 14, dib is 40, then pixels
            zeroIn4bytes                        // reserved/unused
            as4bytes (14 + 40)                  // offset to byte data
        |] |> Array.concat
    
    let dib = 
        [|
            as4bytes 40          // dib size. just a count of all the bytes in this array (including the bytes for the count)
            as4bytes width
            as4bytes height
            as2bytes 1           // planes (=1)
            as2bytes 24          // bpp (24bit)
            zeroIn4bytes         // BI_RGB, no pixel array compression used 
            as4bytes byteSize    // Size of the raw bitmap data (including padding) 
            as4bytes 2835        // 72dpi horizontal (calc in wiki article)
            as4bytes 2835        // 72dpi vertical
            zeroIn4bytes         // palette colours (0, not using a palette)
            zeroIn4bytes         // important colours (0=all)
        |] |> Array.concat

    let pixels =
        [| height-1..-1..0 |] 
        |> Array.map (fun y ->
            let row = 
                [|0..width-1|] 
                |> Array.collect (fun x ->
                    let v = array.[x, y]
                    let gs = int (v * 255.) |> byte
                    [|gs;gs;gs|])
            let padding = Array.zeroCreate<byte> padding
            Array.append row padding)
        |> Array.concat

    [|
        header
        dib
        pixels
    |] |> Array.concat