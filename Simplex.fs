/// Derived based on the logic here:
/// http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
/// And the java/cpp code samples here:
/// http://weber.itn.liu.se/~stegu/simplexnoise/SimplexNoise.java
/// http://staffwww.itn.liu.se/~stegu/aqsis/aqsis-newnoise/simplexnoise1234.cpp
module Simplex

open System

let private F2 = 0.5 * ((sqrt 3.) - 1.)
let private G2 = (3. - sqrt 3.) / 6.

let grad hash x y =
    let h = int (hash &&& 7uy)
    let u, v = if h < 4 then x, y else y, x
    let u = if h &&& 1 <> 0 then -u else u
    let v = if h &&& 2 <> 0 then -2. * v else 2. * v
    float (u + v)

let private simplexNoise (perms: byte []) x y = 
    let xin, yin = float x, float y

    let s = (xin + yin) * F2
    let i, j = floor (xin + s), floor (yin + s)
    let t = (i + j) * G2
    let x0, y0 = xin - (i - t), yin - (j - t)

    let i1, j1 = if x0 > y0 then 1., 0. else 0., 1.
    let x1, y1 = x0 - i1 + G2, y0 - j1 + G2
    let x2, y2 = x0 - 1. + (2. * G2), y0 - 1. + (2. * G2)

    let ii, jj = int i &&& 255, int j &&& 255

    let t0 = 0.5 - x0 * x0 - y0 * y0
    let n0 = 
        if t0 < 0. then 0.
        else
            let t0 = t0 * t0
            t0 * t0 * grad perms.[ii + int perms.[jj]] x0 y0

    let t1 = 0.5 - x1 * x1 - y1 * y1
    let n1 =
        if t1 < 0. then 0.
        else 
            let t1 = t1 * t1
            t1 * t1 * grad perms.[ii + int i1 + int perms.[jj + int j1]] x1 y1

    let t2 = 0.5 - x2 * x2 - y2 * y2
    let n2 =
        if t2 < 0. then 0.
        else
            let t2 = t2 * t2
            t2 * t2 * grad perms.[ii + 1 + int perms.[jj + 1]] x2 y2

    40. * (n0 + n1 + n2)

let create width height scale seed =
    let random = match seed with Some n -> Random n | _ -> Random ()
    let permutations = Array.zeroCreate<byte> 512
    random.NextBytes permutations

    let array = Array2D.create width height 0.
    for y = 0 to height-1 do
        for x = 0 to width-1 do
            let noise = simplexNoise permutations (float x * scale) (float y * scale)
            array.[x, y] <- 0.5 + 0.5 * noise
    array