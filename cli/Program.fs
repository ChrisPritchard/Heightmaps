open SDL

type RenderMode = SDL | PPM of string | BMP of string
type NoiseType = DiamondSquare | Perlin

let getConfig args = 
    Some (DiamondSquare, 512, SDL, None)

[<EntryPoint>]
let main args =
    let config = getConfig args
    match config with
    | None ->
        """
            Arguments:
            -perlin will generate perlin noise

                * default is a diamond square heightmap

            -dim [x] will specify the width/height of the output

                * default is 513 (2^6 + 1)

            -ppm [fn]: will save output as a .ppm image
            -bmp [fn]: will save output as a .bmp image

                * default is to show as a SDL surface, with 'R' and 'Escape' to regenerate or quit

            -seed n: will initialise the random seed

                * default is uninitialised, i.e. random
        """
        |> printfn "%s"
    | Some (noise, size, render, seed) ->
        
        //let array = DiamondSquare.create 512 None
        //Bitmaps.PPM.grayscale "test.ppm" array
        showViaSDL 513 (fun () -> DiamondSquare.create size seed)
    0
