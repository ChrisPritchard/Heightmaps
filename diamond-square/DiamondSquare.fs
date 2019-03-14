module DiamondSquare

open System

let private squarePoints x y size =
    [
        x, y
        x + size, y
        x, y + size
        x + size, y + size
    ]

let private diamondPoints x y size =
    [
        x + size, y
        x, y + size
        x - size, y
        x, y - size
    ]

let create dim seed =
    let size = if dim % 2 = 0 then dim + 1 else dim
    let random = match seed with Some n -> Random n | _ -> Random ()

    let array = Array2D.create size size 0.
    squarePoints 0 0 (size - 1)
    |> List.iter (fun (x, y) -> 
        array.[x, y] <- random.NextDouble ())
    
    let arrayValue (x, y) =
        let x = if x < 0 then size + x else x
        let y = if y < 0 then size + y else y
        array.[x % size, y % size]

    let rec fill x y size range =
        // diamond step (set point in middle of square)
        let value = 
            squarePoints x y size 
            |> List.sumBy arrayValue
            |> fun total -> total / 4.
        let nsize = size/2
        let mx, my = x + nsize, y + nsize
        array.[mx, my] <- min 1. (value + (random.NextDouble() * range))

        // square step (four new squares from diamond, above)
        diamondPoints mx my nsize
        |> List.iter (fun (cx, cy) ->
            let value = 
                diamondPoints cx cy nsize 
                |> List.sumBy arrayValue
                |> fun total -> total / 4.
            array.[cx, cy] <- min 1. (value + (random.NextDouble() * range)))
       
        if size = 2 then ()
        else
            fill x y nsize (range/2.)
            fill (x + nsize) y nsize (range/2.)
            fill (x + nsize) (y + nsize) nsize (range/2.)
            fill x (y + nsize) nsize (range/2.)

    fill 0 0 (size - 1) 0.5
    array