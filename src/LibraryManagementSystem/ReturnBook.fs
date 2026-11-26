
module ReturnBook

open Types

let returnBook (id: int) (library: Library) : Result<Library, string> =
    let giveBack (book: Book) =
        match book.Status with
        | Available ->
            Error(sprintf "\"%s\" is not borrowed, so it cannot be returned." book.Title)
        | Borrowed _ ->
            // Immutability: build a NEW record, the old one is untouched.
            // Setting the status to Available also clears the loan (borrow date).
            Ok { book with Status = Available }

    updateBook id giveBack library
