module DiamondSquare.Generator

open System

let private points x y size =
    [x,y;x,size-1;size-1,size-1;size-1,y]

let create dim seed =
    let size = if dim % 2 = 0 then dim + 1 else dim
    let random = match seed with Some n -> Random n | _ -> Random ()

    let array = Array2D.create size size 0.
    points 0 0 size
    |> List.iter (fun (x, y) -> 
        array.[x, y] <- random.NextDouble ())

    let rec fill x y size range =
        let value = 
            points x y size 
            |> List.sumBy (fun (x, y) -> array.[x%size, y%size])
            |> fun total -> total / 4.
        let nsize = size/2
        array.[x + nsize, y + nsize] <- min 1. (value + (random.NextDouble() * range))
        if size = 2 then ()
        else
            fill x y nsize (range/2.)
            fill (x + nsize) y nsize (range/2.)
            fill (x + nsize) (y + nsize) nsize (range/2.)
            fill x (y + nsize) nsize (range/2.)

    fill 0 0 size 0.5
    array