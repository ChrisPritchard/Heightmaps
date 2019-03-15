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

    let array = Array2D.create dim dim 0.
    squarePoints 0 0 (dim - 1) 
    |> List.iter (fun (x, y) -> array.[x, y] <- 0.5)
        
    let arrayPoints (x, y) = 
        (if x < 0 then (dim-1) + x elif x >= dim then x - (dim - 1) else x),
        (if y < 0 then (dim-1) + y elif y >= dim then y - (dim - 1) else y)

    let rec step size range =

        let hsize = size/2

        let mutable x, y = 0, 0
        while x < dim - 1 do
            while y < dim - 1 do
                // diamond step (set point in middle of square)
                let corners = 
                    squarePoints x y size 
                    |> List.map arrayPoints
                    |> List.map (fun (x, y) -> (x, y), array.[x, y])
                if corners |> List.tryFind (snd >> (=) 0.) <> None then
                    ()
                let value = 
                    corners
                    |> List.sumBy snd
                    |> fun total -> min 1. ((total / 4.) + (random.NextDouble() * range))
                
                let mx, my = x + hsize, y + hsize
                array.[mx, my] <- value
                y <- y + size
            x <- x + size
        
        let mutable mx, my = hsize, hsize
        while mx < dim - 1 do
            while my < dim - 1 do
                // square step (four new squares from diamond, above)
                for (cx, cy) in diamondPoints mx my hsize do
                    let points = 
                        diamondPoints cx cy hsize 
                        |> List.map arrayPoints
                        |> List.map (fun (x, y) -> (x, y), array.[x, y])
                    if points |> List.tryFind (snd >> (=) 0.) <> None then
                        ()
                    let value = 
                        points
                        |> List.sumBy snd
                        |> fun total -> min 1. ((total / 4.) + (random.NextDouble() * range))
                    array.[cx, cy] <- value
                my <- my + size
            mx <- mx + size
       
        if size < 2 then ()
        else
            step hsize (range/2.)

    step (dim-1) 0.5
    array