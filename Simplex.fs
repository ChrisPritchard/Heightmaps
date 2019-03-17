/// Derived based on the logic here:
/// http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
/// And the java code sample here:
/// http://weber.itn.liu.se/~stegu/simplexnoise/SimplexNoise.java
module Simplex

open System

let private F = 0.5 * (sqrt 3. - 1.)
let private G = (3. - sqrt 3.) / 6.

let private simplexNoise x y random = 
    let xin, yin = float x, float y

    let s = (xin + yin) * F
    let i, j = floor (xin + s), floor (yin + s)
    let t = (i + j) * G
    let X0, Y0 = i - t, j - t
    let x0, y0 = xin - X0, yin - Y0

    let i1, j1 = if x0 > y0 then 1., 0. else 0., 1.
    let x1, y1 = x0 - i1 + G, y0 - j1 + G
    let x2, y2 = x0 - 1. + (2. * G), y0 - 1. + (2. * G)

    let ii, jj = int i &&& 255, int j &&& 255
    ()
    

let create width height seed =
    let random = match seed with Some n -> Random n | _ -> Random ()
    
    let array = Array2D.create width height 0.
    
    for y = 0 to height-1 do
        for x = 0 to width-1 do
            let noise = simplexNoise x y random
            array.[x, y] <- 0.5 + 0.5 * noise

    array