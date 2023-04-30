module Compressor.compress

type Compressed =
    | Success of int64 // bytes saved
    | Failure of string // error

type CompressBuilder() =
    member this.Bind(mapped, to_mapped) =
        match mapped with
        | Failure e ->
            printfn $"Error: {e}"
            mapped
        | Success b ->
            printfn $"Saved {b} bytes!"
            to_mapped b
    member this.Return(bytes) = Success bytes
    member this.RetrunFrom(m) = m

type Configuration = {
    Directory : string
    Extension : string
    Cutoff : int
    RemoveAfter : bool
}
