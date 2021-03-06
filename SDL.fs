﻿module SDL

open System
open System.Runtime.InteropServices

[<Literal>]
let libName = "SDL2"

let SDL_INIT_VIDEO = 0x00000020u

type SDL_WindowFlags =
| SDL_WINDOW_SHOWN = 0x00000004
| SDL_WINDOW_INPUT_FOCUS = 0x00000200

let SDL_TEXTUREACCESS_STREAMING = 1
let SDL_PIXELFORMAT_ARGB8888 = 372645892u

let SDL_KEYDOWN = 0x300u
let SDLK_ESCAPE = 27u

[<type:StructLayout(LayoutKind.Sequential)>]
type SDL_Keysym = {
    scancode: SDL_Scancode
    sym: uint32
    ``mod``: SDL_Keymod
    unicode: uint32
} 
and SDL_Scancode = 
| SDL_SCANCODE_ESCAPE = 41
and SDL_Keymod =
| KMOD_NONE = 0x0000

[<type:StructLayout(LayoutKind.Sequential)>]
type SDL_KeyboardEvent =
    struct
        val ``type``: uint32
        val timestamp: uint32
        val windowID: uint32
        val state: byte
        val repeat: byte
        val private padding2: byte
        val private padding3: byte
        val keysym: SDL_Keysym
    end
    
[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern int SDL_Init(uint32 flags)
    
[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern int SDL_CreateWindowAndRenderer (int width, int height, SDL_WindowFlags flags, IntPtr& window, IntPtr& renderer)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern unit SDL_Delay(uint32 milliseconds)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern int SDL_PollEvent(SDL_KeyboardEvent& _event)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern IntPtr SDL_CreateTexture (IntPtr renderer, uint32 format, int access, int width, int height)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern int SDL_UpdateTexture(IntPtr texture, IntPtr rect, IntPtr pixels, int pitch);

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern int SDL_RenderClear(IntPtr renderer)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr destrect);

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern unit SDL_RenderPresent(IntPtr renderer)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern unit SDL_DestroyTexture(IntPtr texture)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern unit SDL_DestroyRenderer(IntPtr renderer)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern unit SDL_DestroyWindow(IntPtr window)

[<DllImport(libName, CallingConvention = CallingConvention.Cdecl)>]
extern unit SDL_Quit()

let private asUint32 (r, g, b) = BitConverter.ToUInt32 (ReadOnlySpan [|b; g; r; 255uy|])

let private updateTexture getArray width texture =
    let frameBuffer = 
        getArray ()
        |> Seq.cast<float> 
        |> Seq.map (fun f -> let g = byte (int (f * 255.)) in asUint32 (g, g, g))
        |> Seq.toArray

    let bufferPtr = IntPtr ((Marshal.UnsafeAddrOfPinnedArrayElement (frameBuffer, 0)).ToPointer ())
    SDL_UpdateTexture(texture, IntPtr.Zero, bufferPtr, width * 4) |> ignore

let showViaSDL width height getArray =
    let mutable window, renderer = IntPtr.Zero, IntPtr.Zero
    SDL_Init(SDL_INIT_VIDEO) |> ignore
    let windowFlags = SDL_WindowFlags.SDL_WINDOW_SHOWN ||| SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS
    SDL_CreateWindowAndRenderer(width, height, windowFlags, &window, &renderer) |> ignore

    let texture = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, SDL_TEXTUREACCESS_STREAMING, width, height)
    updateTexture getArray width texture

    let mutable keyEvent = Unchecked.defaultof<SDL_KeyboardEvent>

    let rec drawLoop () = 
        SDL_RenderClear(renderer) |> ignore
        SDL_RenderCopy(renderer, texture, IntPtr.Zero, IntPtr.Zero) |> ignore
        SDL_RenderPresent(renderer) |> ignore

        SDL_Delay 200u

        if SDL_PollEvent(&keyEvent) <> 0 && keyEvent.``type`` = SDL_KEYDOWN then
            if keyEvent.keysym.sym = SDLK_ESCAPE then ()
            elif keyEvent.keysym.sym = uint32 'r' then
                updateTexture getArray width texture
                drawLoop ()
            else
                drawLoop ()
        else drawLoop ()
    
    drawLoop ()

    SDL_DestroyTexture(texture)
    SDL_DestroyRenderer(renderer)
    SDL_DestroyWindow(window)
    SDL_Quit()