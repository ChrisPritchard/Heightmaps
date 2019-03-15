open SDL
open System
open System.Runtime.InteropServices

let private asUint32 (r, g, b) = BitConverter.ToUInt32 (ReadOnlySpan [|b; g; r; 255uy|])

let private updateTexture getArray width texture =
    let frameBuffer = 
        getArray ()
        |> Seq.cast<float> 
        |> Seq.map (fun f -> let g = byte (int (f * 255.)) in asUint32 (g, g, g))
        |> Seq.toArray

    let bufferPtr = IntPtr ((Marshal.UnsafeAddrOfPinnedArrayElement (frameBuffer, 0)).ToPointer ())
    SDL_UpdateTexture(texture, IntPtr.Zero, bufferPtr, width * 4) |> ignore

let private showViaSDL dim getArray =
    let mutable window, renderer = IntPtr.Zero, IntPtr.Zero
    SDL_Init(SDL_INIT_VIDEO) |> ignore
    let windowFlags = SDL_WindowFlags.SDL_WINDOW_SHOWN ||| SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS
    SDL_CreateWindowAndRenderer(dim, dim, windowFlags, &window, &renderer) |> ignore

    let texture = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, SDL_TEXTUREACCESS_STREAMING, dim, dim)
    updateTexture getArray dim texture

    let mutable keyEvent = Unchecked.defaultof<SDL_KeyboardEvent>

    let rec drawLoop () = 
        SDL_RenderClear(renderer) |> ignore
        SDL_RenderCopy(renderer, texture, IntPtr.Zero, IntPtr.Zero) |> ignore
        SDL_RenderPresent(renderer) |> ignore

        if SDL_PollEvent(&keyEvent) <> 0 && keyEvent.``type`` = SDL_KEYDOWN then
            if keyEvent.keysym.sym = SDLK_ESCAPE then ()
            elif keyEvent.keysym.sym = uint32 'r' then
                updateTexture getArray dim texture
                drawLoop ()
            else
                drawLoop ()
        else drawLoop ()
    
    drawLoop ()
    SDL_DestroyTexture(texture)
    SDL_DestroyRenderer(renderer)
    SDL_DestroyWindow(window)
    SDL_Quit()

[<EntryPoint>]
let main _ =
    //let array = DiamondSquare.create 512 None
    //Bitmaps.PPM.grayscale "test.ppm" array
    showViaSDL 513 (fun () -> DiamondSquare.create 512 None)
    0
