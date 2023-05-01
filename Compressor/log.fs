module Compressor.log

open System.Data.SQLite
open Compressor.types

let conn_string = @"Data Source = .\.db\save_log.db"

let upload_compressed_file (compressed : CompressedFile) =
    use connection = new SQLiteConnection(conn_string)
    connection.Open()
    let query = @$"
    INSERT INTO [savelog] 
        ([date],
        [compressed_filename], 
        [removed], 
        [bytes_saved]) 
    VALUES 
        ('{compressed.Date}',
        '{compressed.CompressedFilename}', 
        {compressed.Removed},
        {compressed.BytesSaved})"
    use command = new SQLiteCommand(query, connection)
    command.ExecuteNonQuery() |> ignore

let upload_error (err : FailedFile) =
    use connection = new SQLiteConnection(conn_string)
    connection.Open()
    let query = @$"
        INSERT INTO [errorlog] ([date], [filename], [error]) VALUES ('{err.Date}', '{err.Filename}', '{err.Error}')
        "
    use command = new SQLiteCommand(query, connection)
    printfn $"{command.CommandText}"
    command.ExecuteNonQuery() |> ignore

let retrieve_total_saved () =
    use connection = new SQLiteConnection(conn_string)
    connection.Open()
    let query = @"SELECT SUM([bytes_saved] / 1000000) AS [mb_saved] FROM [savelog]"
    use command = new SQLiteCommand(query, connection)
    let reader = command.ExecuteReader()
    reader.GetFloat(0)
