module Compressor.compressor

open System.Diagnostics
open System
open System.IO
open System.IO.Compression
open Compressor.compress

let compress = new CompressBuilder()

let private check_file file cutoff_date =
    try
        let info = FileInfo(file)
        match info with
        | file when file.CreationTime < cutoff_date -> Some info
        | _ -> None
    with
        | _ -> None


let private collect_data_files path extension cutoff =
    let cutoff_date = DateTime.Now.AddDays(float (-1 * cutoff))
    let rec walk_directory input = seq {
        for item in Directory.EnumerateFileSystemEntries(input) do
            match item with
            | dir when Directory.Exists(item) -> yield! walk_directory dir
            | file when File.Exists(item) ->
                match check_file file cutoff_date with
                | Some good_file -> 
                    if extension = good_file.Extension then
                        yield file
                | None -> ()
            | _ -> ()
    } 
    walk_directory path


// crude
let private compress_file file remove_after =
        let zip_path = $"{file}.gz"
        use input_file_stream = File.OpenRead(file)
        use output_file_stream = File.Create(zip_path)
        use zip_stream = new GZipStream(output_file_stream, CompressionLevel.Optimal)
        try
            input_file_stream.CopyTo(zip_stream)
            match remove_after with
            | true -> File.Delete(file)
            | false -> ()
            let bytes_saved = FileInfo(zip_path).Length
            Success bytes_saved
        with
            | _ -> Failure "It just didn't work out this time, sorry bud."

// mock
let private compress_file_mock file remove_after =
    match remove_after with
    | true -> $"Compressed and removed {file}"
    | false -> $"Compressed {file}"
        

// crude
let private process_directory (config : Configuration) =
    let files = collect_data_files config.Directory config.Extension config.Cutoff
    files 
    |> Seq.map (fun x -> compress {
        let! result = compress_file x config.RemoveAfter
        return result
    })

let _compress config =
    process_directory config
