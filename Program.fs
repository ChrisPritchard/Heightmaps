open SDL
open System

type RenderMode = SDL | PPM of string | BMP of string
type NoiseType = 
    | DiamondSquare of powerOfTwo:int 
    | Perlin of width:int * height:int 
    | Simplex of width:int * height:int

let getConfig args = 
    if args = [|"help"|] || args = [|"?"|] then 
        None
    else
        let parse i = Int32.Parse args.[i]
        let noise = 
            let perlinIndex = Array.IndexOf (args, "-perlin") 
            let simplexIndex = Array.IndexOf (args, "-simplex") 
            let diamondIndex = Array.IndexOf (args, "-diamondsquare") 
            if perlinIndex < 0 && diamondIndex < 0 && simplexIndex < 0 then 
                DiamondSquare 8
            elif simplexIndex < 0 && diamondIndex < 0 then
                Perlin (parse (perlinIndex + 1), parse (perlinIndex + 2))
            elif perlinIndex < 0 && simplexIndex < 0 then
                DiamondSquare (min 12 (parse (diamondIndex + 1)))
            else
                Simplex (parse (simplexIndex + 1), parse (simplexIndex + 2))
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
        Some (noise, output, seed)

[<EntryPoint>]
let main args =
    let config = getConfig args
    match config with
    | None ->
        """
            Arguments:
            -perlin [w] [h] will generate perlin noise
            -diamondsquare [power of 2] will generate a diamond square heightmap, at 2n+1 size

                * default is a diamond square heightmap, with 9 as the power of 2 (513 pixels)
                * diamond square will max out at a 2n value of 12 (which would create a 50mb bmp)

            -ppm [fn]: will save output as a .ppm image
            -bmp [fn]: will save output as a .bmp image

                * default is to show as a SDL surface, with 'R' and 'Escape' to regenerate or quit

            -seed n: will initialise the random seed

                * default is uninitialised, i.e. random
        """
        |> printfn "%s"
    | Some (noise, render, seed) ->
        let creator = 
            match noise with 
            | DiamondSquare n -> fun () -> DiamondSquare.create (pown 2 n + 1) seed
            | Perlin (w, h) -> fun () -> Perlin.create w h seed
            | Simplex (w, h) -> fun () -> Simplex.create w h
        match render with
        | SDL -> 
            let width, height = 
                match noise with 
                | DiamondSquare n -> pown 2 n + 1, pown 2 n + 1
                | Perlin (w, h) | Simplex (w, h) -> w, h
            showViaSDL width height creator
        | PPM fileName -> creator () |> PPM.grayscale fileName
        | BMP fileName -> creator () |> BMP.grayscale fileName
        
    0
