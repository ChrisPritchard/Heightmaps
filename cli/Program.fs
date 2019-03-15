open SDL
open System
open System.Runtime.InteropServices

let showViaSDL heightmap =
    let width, height = Array2D.length1 heightmap, Array2D.length2 heightmap
    let mutable window, renderer = IntPtr.Zero, IntPtr.Zero

    SDL_Init(SDL_INIT_VIDEO) |> ignore
    let windowFlags = SDL_WindowFlags.SDL_WINDOW_SHOWN ||| SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS
    SDL_CreateWindowAndRenderer(width, height, windowFlags, &window, &renderer) |> ignore

    let asUint32 (r, g, b) = BitConverter.ToUInt32 (ReadOnlySpan [|b; g; r; 255uy|])

    let frameBuffer = 
        heightmap 
        |> Seq.cast<float> 
        |> Seq.map (fun f -> let g = byte (int (f * 255.)) in asUint32 (g, g, g))
        |> Seq.toArray

    let texture = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, SDL_TEXTUREACCESS_STREAMING, width, height)
    let bufferPtr = IntPtr ((Marshal.UnsafeAddrOfPinnedArrayElement (frameBuffer, 0)).ToPointer ())
    SDL_UpdateTexture(texture, IntPtr.Zero, bufferPtr, width * 4) |> ignore

    let mutable keyEvent = Unchecked.defaultof<SDL_KeyboardEvent>

    let rec drawLoop () = 
        SDL_RenderClear(renderer) |> ignore
        SDL_RenderCopy(renderer, texture, IntPtr.Zero, IntPtr.Zero) |> ignore
        SDL_RenderPresent(renderer) |> ignore

        if SDL_PollEvent(&keyEvent) <> 0 && keyEvent.``type`` = SDL_KEYDOWN && keyEvent.keysym.sym = SDLK_ESCAPE then ()
        else drawLoop ()
    
    drawLoop ()
    SDL_DestroyTexture(texture)
    SDL_DestroyRenderer(renderer)
    SDL_DestroyWindow(window)
    SDL_Quit()

[<EntryPoint>]
let main _ =
    let array = DiamondSquare.create 512 None
    //Bitmaps.PPM.grayscale "test.ppm" array
    showViaSDL array
    0
