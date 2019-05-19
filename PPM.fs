module PPM

let grayscale array =
    let width, height = Array2D.length1 array, Array2D.length2 array

    [|
        yield! Seq.map byte (sprintf "P6\n%i %i\n255\n" width height)

        for y = 0 to height - 1 do
            for x = 0 to width - 1 do
                let v = array.[x, y]
                let gs = int (v * 255.) |> byte
                yield gs
                yield gs
                yield gs
    |]
