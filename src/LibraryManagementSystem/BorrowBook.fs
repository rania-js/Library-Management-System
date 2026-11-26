// ================================================================
//  BorrowBook.fs  -  Person 4
//  Function to borrow a book: records the borrow date and makes
//  sure a book can never be borrowed twice at the same time.
// ================================================================
module BorrowBook

open System
open Types

let borrowBook (id: int) (date: DateTime) (library: Library) : Result<Library, string> =
    let borrow (book: Book) =
        match book.Status with
        | Borrowed loan ->
            Error(sprintf "\"%s\" is already borrowed (since %s)." book.Title (loan.BorrowDate.ToString("yyyy-MM-dd")))
        | Available ->
            // Immutability: build a NEW record, the old one is untouched.
            Ok { book with Status = Borrowed { BorrowDate = date } }

    updateBook id borrow library
