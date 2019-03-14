
open System.IO

let saveAsPPM fileName array =
    if File.Exists fileName then File.Delete fileName
    use out = File.OpenWrite fileName
    let width, height = Array2D.length1 array, Array2D.length2 array

    sprintf "P6\n%i %i\n255\n" width height 
    |> Seq.iter (fun c -> out.WriteByte (byte c))

    for y = 0 to height - 1 do
        for x = 0 to width - 1 do
            let v = array.[x, y]
            let gs = int (v * 255.) |> byte
            Seq.iter out.WriteByte [gs;gs;gs]

[<EntryPoint>]
let main _ =
    let array = DiamondSquare.create 512 None
    saveAsPPM "test.ppm" array
    0 // return an integer exit code
