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
            yield! BitConverter.GetBytes (uint16 32)    // bpp (24bit)
            yield! BitConverter.GetBytes (uint32 3)     // BI_BITFIELDS, no pixel array compression used 
            yield! BitConverter.GetBytes (uint32 32)    // Size of the raw bitmap data (including padding) 
            yield! BitConverter.GetBytes (uint32 2835)  // 72dpi horizontal
            yield! BitConverter.GetBytes (uint32 2835)  // 72dpi vertical
            yield! BitConverter.GetBytes (uint32 0)     // palette colours (0, not using a palette)
            yield! BitConverter.GetBytes (uint32 0)     // important colours (0=all)
            yield! [0uy;0uy;255uy;0uy]                  // red bit mask
            yield! [0uy;255uy;0uy;0uy]                  // green bitmask
            yield! [255uy;0uy;0uy;0uy]                  // blue bitmask
            yield! [0uy;0uy;0uy;255uy]                  // alpha bitmask
            yield! "Win " |> Seq.rev |> Seq.map byte    // LCS_WINDOWS_COLOR_SPACE
            yield! List.replicate 36 0uy                // unused
            yield! BitConverter.GetBytes (uint32 0)     // unused
            yield! BitConverter.GetBytes (uint32 0)     // unused
            yield! BitConverter.GetBytes (uint32 0)     // unused
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
                yield! [gs;gs;gs;255uy]
    ] |> Seq.iter out.WriteByte
