
//  Function to add a new book (title, author, genre).

module AddBook

open System
open Types

let addBook (title: string) (author: string) (genre: string) (library: Library) : Result<Library, string> =
    let t, a, g = clean title, clean author, clean genre

    let missingField =
        [ "Title", t; "Author", a; "Genre", g ]
        |> List.tryFind (fun (_, value) -> value = "")

    let sameText (x: string) (y: string) =
        String.Equals(x, y, StringComparison.OrdinalIgnoreCase)

    match missingField with
    | Some(name, _) -> Error(sprintf "%s is required." name)
    | None ->
        let isDuplicate =
            library |> Map.exists (fun _ b -> sameText b.Title t && sameText b.Author a)

        if isDuplicate then
            Error "This book already exists in the library."
        else
            let id = nextId library
            let book =
                { Id = id
                  Title = t
                  Author = a
                  Genre = g
                  Status = Available }
            Ok(Map.add id book library)
