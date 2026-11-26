
module Library.AddBook

open System
open Library.Types

let addBook (lib: Library) (title: string) (author: string) (genre: string)
    : Result<Library, LibraryError> =

    if String.IsNullOrWhiteSpace(title)
       || String.IsNullOrWhiteSpace(author)
       || String.IsNullOrWhiteSpace(genre) then

        Error InvalidBookData

    else

        let newId = Library.Types.Library.nextBookId lib

        let newBook = {
            Id = newId
            Title = title.Trim()
            Author = author.Trim()
            Genre = genre.Trim()
            Status = Available
        }

        let updatedBooks = lib.Books.Add(newId, newBook)

        let updatedLibrary = {
            lib with
                Books = updatedBooks
        }

        Ok updatedLibrary

