module Compressor.types

open System

type CompressedFile = {
    Date : DateTime
    CompressedFilename : string
    Removed : bool
    BytesSaved : int64
}

type FailedFile = {
    Date: DateTime
    Filename : string
    Error : string
}

type Compressed =
    | Success of CompressedFile // bytes saved
    | Failure of FailedFile // error

type Configuration = {
    Directory : string
    Extension : string
    Cutoff : int
    RemoveAfter : bool
}
