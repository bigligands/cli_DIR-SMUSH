module Compressor.compress

open Compressor.types
open Compressor.log

let log_errors ( fail : FailedFile ) =
    printfn $"Error: {fail.Error}"
    upload_error fail

let log_bytes (compressed) =
    printfn $"Compressed into {compressed.CompressedFilename}."
    upload_compressed_file compressed

// This is just to learn about Computation Expressions
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
