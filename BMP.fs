module BMP

open System.IO
open System

let grayscale fileName array =
    if File.Exists fileName then File.Delete fileName
    use out = File.OpenWrite fileName

    let width, height = Array2D.length1 array, Array2D.length2 array
    let byteSize = width * height * 4

    let dib = [
            yield! BitConverter.GetBytes (uint32 width)
            yield! BitConverter.GetBytes (uint32 height)
            yield! BitConverter.GetBytes (uint16 1)     // planes (=1)
            yield! BitConverter.GetBytes (uint16 24)    // bpp (24bit)
            yield! BitConverter.GetBytes (uint32 0)     // compression (0=none)
            yield! BitConverter.GetBytes (uint32 0)     // compressed size (0=none 'cause no compression)
            yield! BitConverter.GetBytes (uint32 2835)  // 72dpi horizontal
            yield! BitConverter.GetBytes (uint32 2835)  // 72dpi vertical
            yield! BitConverter.GetBytes (uint32 0)     // colours used (0=all)
            yield! BitConverter.GetBytes (uint32 0)     // important colours (0=all)
        ]

    let header = [
            yield! "BM" |> Seq.map byte
            let totalSize = 14 + 4 + dib.Length + byteSize // header is 14, 4 for dib size, then dib size, then pixels
            yield! BitConverter.GetBytes (uint32 totalSize)
            yield! BitConverter.GetBytes (uint32 0) // unused
            yield! BitConverter.GetBytes (uint32 (14 + 4 + dib.Length)) // offset to pixels
        ]

    [
        yield! header
        yield! BitConverter.GetBytes (uint32 dib.Length)
        yield! dib
        
        for y in [height-1..-1..0] do
            for x = 0 to width-1 do
                let v = array.[x, y]
                let gs = int (v * 255.) |> byte
                yield! [gs;gs;gs]
    ] |> Seq.iter out.WriteByte
