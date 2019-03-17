/// The code for perlin noise is taken almost verbatim (mainly language translations) from here:
/// https://longwelwind.net/2017/02/09/perlin-noise.html
/// This PDF on Simplex noise also has a good specification of Perlin noise and how it works:
/// http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
module Perlin

open System

let private randomVector (random: Random) =
    let angle = random.NextDouble () * 2. * Math.PI
    cos angle, sin angle

let private perlinVectors width height period random = 
    let periodWidth = int (ceil (float width / period)) + 1
    let periodHeight = int (ceil (float height / period)) + 1

    let vectors = Array2D.create periodWidth periodHeight (0., 0.)
    for y = 0 to periodWidth-1 do
        for x = 0 to periodHeight-1 do
            vectors.[x, y] <- randomVector random

    vectors

/// This fade function provides a non-linear 'fudge' to the relative value.
/// The algorithm is direct from the Perlin specification: 6t^5 - 15t^4 + 10t^3
let private fade t =
    (6. * (t ** 5.)) - (15. * (t ** 4.)) + (10. * (t ** 3.))

let private dot (x1, y1) x2 y2 =
    (x1 * x2) + (y1 * y2)

let private lerp n1 n2 t =
    (n1 * (1. - t)) + (n2 * t)

let private perlinNoise x y (vectors: (float * float) [,]) period =
    let cellX = int (floor (float x / period))
    let cellY = int (floor (float y / period))
    let relX = fade ((float x - float cellX * period) / period)
    let relY = fade ((float y - float cellY * period) / period)
    
    let topLeft = vectors.[cellX, cellY]
    let topRight = vectors.[cellX + 1, cellY]
    let bottomLeft = vectors.[cellX, cellY + 1]
    let bottomRight = vectors.[cellX + 1, cellY + 1]

    let topLeftContribution = dot topLeft relX relY
    let topRightContribution = dot topRight (relX - 1.) relY
    let bottomLeftContribution = dot bottomLeft relX (relY - 1.)
    let bottomRightContribution = dot bottomRight (relX - 1.) (relY - 1.)

    let topLerp = lerp topLeftContribution topRightContribution relX
    let bottomLerp = lerp bottomLeftContribution bottomRightContribution relX
    let finalLerp = lerp topLerp bottomLerp relY

    finalLerp / ((sqrt 2.) / 2.) // produces a final value between -1 and 1

let create width height seed =
    let random = match seed with Some n -> Random n | _ -> Random ()
    // this period is fixed here based on width, 
    // but could be changed to control the noise granularity
    let period = ceil (float width / 10.) 

    let array = Array2D.create width height 0.
    let vectors = perlinVectors width height period random

    for y = 0 to height-1 do
        for x = 0 to width-1 do
            let noise = perlinNoise x y vectors period
            array.[x, y] <- 0.5 + 0.5 * noise

    array
