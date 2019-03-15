
[<EntryPoint>]
let main _ =
    let array = DiamondSquare.create 512 None
    Bitmaps.PPM.grayscale "test.ppm" array
    0 // return an integer exit code
