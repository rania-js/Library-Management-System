// ================================================================
//  DeleteBook.fs
//  Function to delete a book from the library.
//  A borrowed book cannot be deleted (return it first).
// ================================================================
module DeleteBook

open Types

let deleteBook (id: int) (library: Library) : Result<Library, string> =
    match Map.tryFind id library with
    | None -> Error "Book not found."
    | Some book ->
        match book.Status with
        | Borrowed _ ->
            Error(sprintf "\"%s\" is currently borrowed, so it cannot be deleted. Return it first." book.Title)
        | Available ->
            Ok(Map.remove id library)
