module Compressor.compressor

open System.Diagnostics
open System
open System.IO
open System.IO.Compression
open Compressor.compress
open Microsoft.FSharp.Collections

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
        let compressed_path = $"{file}.gz"
        use input_file_stream = File.OpenRead(file)
        use output_file_stream = File.Create(compressed_path)
        use gz_stream = new GZipStream(output_file_stream, CompressionLevel.Optimal)

        try
            input_file_stream.CopyTo(gz_stream)
            match remove_after with
            | true -> File.Delete(file)
            | false -> ()

            let bytes_saved = FileInfo(compressed_path).Length
            Success {
                CompressedFilename = compressed_path
                Removed = remove_after
                BytesSaved = bytes_saved
            }

        with
            | _ as ex -> 
                File.Delete(compressed_path)
                Failure { Filename = file; Error = $"Failed to compress {file} : {ex.Message}" }


let compress_dir config =
    let files = collect_data_files config.Directory config.Extension config.Cutoff
    let results = 
        files 
        |> Seq.map (fun file -> 
            compress {
                let! result = compress_file file config.RemoveAfter
                return result
            })
        //compress {
        //    let cs = files |> Seq.map (fun file -> compress_file file config.RemoveAfter)
        //    for file in cs do
        //        return file
        //}

    let saved = 
        results 
        |> Seq.choose (fun comp ->
            match comp with
            | Success s -> Some s
            | Failure f -> None
        )
        |> Seq.fold (fun acc compressed -> (float compressed.BytesSaved / 1_000_000.) + acc) 0.

    ()
