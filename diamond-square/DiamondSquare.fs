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
    let dim  = if dim % 2 = 0 then dim + 1 else dim
    let random = match seed with Some n -> Random n | _ -> Random ()

    let array = Array2D.create dim dim 0.5
        
    let arrayPoints x y = 
        let x = if x < 0 then dim + x else x
        let y = if y < 0 then dim + y else y
        x % dim, y % dim

    let arrayValue (x, y) =
        let x, y = arrayPoints x y
        array.[x, y]

    let arraySet x y v =
        let x, y = arrayPoints x y
        array.[x, y] <- v

    let rec step x y size range =
        // diamond step (set point in middle of square)
        let value = 
            squarePoints x y size 
            |> List.sumBy arrayValue
            |> fun total -> total / 4.
        let nsize = int (ceil (float size/2.))
        let mx, my = x + nsize, y + nsize
        arraySet mx my (min 1. (value + (random.NextDouble() * range)))

        // square step (four new squares from diamond, above)
        diamondPoints mx my nsize
        |> List.iter (fun (cx, cy) ->
            let value = 
                diamondPoints cx cy nsize 
                |> List.sumBy arrayValue
                |> fun total -> total / 4.
            arraySet cx cy (min 1. (value + (random.NextDouble() * range))))
       
        if size = 2 then ()
        else
            step x y nsize (range/2.)
            step (x + nsize) y nsize (range/2.)
            step (x + nsize) (y + nsize) nsize (range/2.)
            step x (y + nsize) nsize (range/2.)

    step 0 0 (dim - 1) 0.5
    array