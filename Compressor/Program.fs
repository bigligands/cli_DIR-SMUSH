open Compressor.compressor
open Compressor.types
open FSharp.SystemCommandLine
open System.IO

// function that handles command
let processDir (dir: string, extension: string, cutoff: int option, remove_after) = 
    if not (DirectoryInfo(dir).Exists) then
        printfn $"{dir} Not recognized as a valid directory!"
        1
    else
        let cutoff = defaultArg cutoff 0
        let remove_after = defaultArg remove_after false

        let ext =
            match extension with
            | x when x.StartsWith('.') -> extension.ToLower()
            | _ -> $".{extension}".ToLower()

        let config = {
            Directory = string dir
            Extension = ext
            Cutoff = cutoff
            RemoveAfter = remove_after
        }
        printfn $"Compressing : {config.Directory}"
        printfn $"Looking for extension : {config.Extension}"
        printfn $"Cutoff : {config.Cutoff} days"
        printfn $"Remove after? : {config.RemoveAfter}"

        let saved = compress_dir config
        printfn $"compressed {saved} megabytes!"
        if not config.RemoveAfter then
            printfn "be sure to delete original files to realize space savings!"
        0
    

[<EntryPoint>]
let main argv =
    // arguments
    // is there an implicit conversion between `string` and `DirectoryInfo`? How is the string arg converted?
    let dir= Input.Argument<string>("The Directory to compress.")
    // required options
    let extension = Input.Option<string>(["--extension"; "-e"], "The file extension for files desired to be compressed")

    // optional options
    let cutoff = Input.OptionMaybe<int>(["--cutoff"; "-c"], "The minimum age of a file in days to be compressed.")
    let remove_after = Input.OptionMaybe(["--remove"; "-r"], "Flag to remove the original file after compression.")
    //let workers = Input.OptionMaybe<int>(["--workers"; "-w"], "Number of worker threads to utilize.")

    rootCommand argv {
        description "Compresses all files of a certain age and extension in a given directory recursively."
        inputs (dir, extension, cutoff, remove_after)
        setHandler processDir
    }