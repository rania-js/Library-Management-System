// ================================================================
//  SearchBook.fs  -  Person 3
//  Function to search for books (case-insensitive) by:
//  title, author, genre, status (Available / Borrowed) or Id.
// ================================================================
module SearchBook

open System
open Types

let search (query: string) (library: Library) : Book list =
    let q = clean query

    let contains (text: string) =
        text.Contains(q, StringComparison.OrdinalIgnoreCase)

    let statusText (book: Book) =
        match book.Status with
        | Available -> "Available"
        | Borrowed _ -> "Borrowed"

    let matches (book: Book) =
        contains book.Title
        || contains book.Author
        || contains book.Genre
        || contains (statusText book)
        || contains (string book.Id)

    library |> allBooks |> List.filter matches

/// Kept for compatibility with the older name.
let searchByTitle = search
