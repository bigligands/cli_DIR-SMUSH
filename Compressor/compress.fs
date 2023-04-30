module Compressor.compress

type CompressedFile = {
    CompressedFilename : string
    Removed : bool
    BytesSaved : int64
}

type FailedFile = {
    Filename : string
    Error : string
}

type Compressed =
    | Success of CompressedFile // bytes saved
    | Failure of FailedFile // error

let log_errors ( fail : FailedFile ) =
    printfn $"Error: {fail.Error}"

let log_bytes (compressed) =
    printfn $"Saved {compressed.BytesSaved} bytes!"

type CompressBuilder() =
    member this.Bind(mapped, to_mapped) =
        match mapped with
        | Failure e ->
            log_errors e
            mapped
        | Success b ->
            log_bytes b
            to_mapped b
    member this.Return(bytes) = Success bytes
    member this.RetrunFrom(m) = m

type Configuration = {
    Directory : string
    Extension : string
    Cutoff : int
    RemoveAfter : bool
}
