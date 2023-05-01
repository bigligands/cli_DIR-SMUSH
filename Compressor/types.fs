module Compressor.types

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

type Configuration = {
    Directory : string
    Extension : string
    Cutoff : int
    RemoveAfter : bool
}
