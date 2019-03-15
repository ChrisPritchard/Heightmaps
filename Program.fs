open SDL
open System

type RenderMode = SDL | PPM of string | BMP of string
type NoiseType = DiamondSquare | Perlin

let getConfig args = 
    if args = [|"help"|] || args = [|"?"|] then 
        None
    else
        let noise = if Array.contains "-perlin" args then Perlin else DiamondSquare
        let size = 
            let index = Array.IndexOf (args, "-size") 
            if index < 0 then 513 
            else Int32.Parse (args.[index + 1])
        let output =
            let ppmIndex = Array.IndexOf (args, "-ppm") 
            let bmpIndex = Array.IndexOf (args, "-bmp") 
            if ppmIndex < 0 && bmpIndex < 0 then SDL
            elif bmpIndex < 0 then PPM (args.[ppmIndex + 1])
            else BMP (args.[bmpIndex + 1])
        let seed = 
            let index = Array.IndexOf (args, "-seed") 
            if index < 0 then None 
            else Some (Int32.Parse (args.[index + 1]))
        Some (noise, size, output, seed)

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
        let creator = 
            match noise with 
            | DiamondSquare -> fun () -> DiamondSquare.create size seed
            | Perlin -> fun () -> Perlin.create size seed
        match render with
        | SDL -> showViaSDL size creator
        | PPM fileName -> creator () |> PPM.grayscale fileName
        | BMP fileName -> creator () |> BMP.grayscale fileName
        
    0
