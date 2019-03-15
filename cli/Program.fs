open SDL

[<EntryPoint>]
let main args =
    if Array.isEmpty args || args = [|"help"|] || args = [|"?"|] then
        """
            Arguments:
            -perlin will generate perlin noise

                * default is a diamond square heightmap

            -dim [x] will specify the width/height of the output

                * default is 513 (2^6 + 1)

            -ppm [fn]: will save output as a .ppm image
            -bmp [fn]: will save output as a .bmp image

                * default is to show as a SDL surface, with 'R' and 'Escape' to regenerate or quit
        """
        |> printfn "%s"
    else
        //let array = DiamondSquare.create 512 None
        //Bitmaps.PPM.grayscale "test.ppm" array
        showViaSDL 513 (fun () -> DiamondSquare.create 512 None)
    0
