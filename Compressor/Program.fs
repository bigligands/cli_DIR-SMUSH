open Compressor.compressor
open Compressor.types
open Compressor.log
open FSharp.SystemCommandLine
open System.IO

// subcommand
let compress_command =
    let compress_handler (dir: string, extension: string, cutoff: int option, remove_after) = 
        match DirectoryInfo(dir).Exists with
        | false -> 
            printfn $"{dir} Not recognized as a valid directory!"
        | true -> 
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
            printfn $"Compressed {saved} megabytes!"
            if not config.RemoveAfter then
                printfn "Be sure to delete original files to realize space savings!"

    let dir= Input.Argument<string>("The Directory to compress.")
    let extension = Input.Option<string>(["--extension"; "-e"], "The file extension for files desired to be compressed")
    let cutoff = Input.OptionMaybe<int>(["--cutoff"; "-c"], "The minimum age of a file in days to be compressed.")
    let remove_after = Input.OptionMaybe(["--remove"; "-r"], "Flag to remove the original file after compression.")

    //EXAMPLE >>Compressor.exe compress c:\users\jcraw\udemy_test -e .ipynb -c 1
    // compress all ipynb files older than 1 day in the udemy_test directory
    command "compress" {
        description "Compresses all files of a certain age and extension in a given directory recursively."
        inputs (dir, extension, cutoff, remove_after)
        setHandler compress_handler
    }

// subcommand
let stats_command =
    let handler (errors : bool option) =
        let errors = defaultArg errors false
        if errors then
            let error_count = retrieve_error_count ()
            printfn $"{error_count} total errors logged using this tool! Sheesh."
        else
            let mb_saved = retrieve_total_saved ()
            printfn $"Compressed {mb_saved} megabytes using this tool! Wow."

    let errors = Input.OptionMaybe(["--errors"], "How many total compression failures logged.")

    // EXAMPLE >>Compressor.exe stats
    // Counts the number of compressed megabytes logged.
    command "stats" {
        description "Return the total space saved in megabytes."
        inputs (errors)
        setHandler handler
    }
    

[<EntryPoint>]
let main argv =
    rootCommand argv {
        description "Directory compression tool used to free up space for those too scared to delete everything."
        setHandler id
        addCommand compress_command
        addCommand stats_command
    }
