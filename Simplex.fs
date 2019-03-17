/// Derived based on the logic here:
/// http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
/// And the java code sample here:
/// http://weber.itn.liu.se/~stegu/simplexnoise/SimplexNoise.java
module Simplex

open System

let private simplexNoise x y random = 
    0.

let create width height seed =
    let random = match seed with Some n -> Random n | _ -> Random ()
    
    let array = Array2D.create width height 0.
    
    for y = 0 to height-1 do
        for x = 0 to width-1 do
            let noise = simplexNoise x y random
            array.[x, y] <- 0.5 + 0.5 * noise

    array