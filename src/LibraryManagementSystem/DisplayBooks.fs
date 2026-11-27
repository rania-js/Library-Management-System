// ================================================================
//  DisplayBooks.fs  -  Person 6
//  1) Functions to display the books (available / borrowed)
//  2) Functions to save the books to a file and load them back
//     (the file "library_data.txt" is created next to the .exe).
// ================================================================
module DisplayBooks

open System
open System.IO
open System.Globalization
open Types
open SearchBook

// ---------------------- display ----------------------

/// Status label + borrow date text for a book.
let describeStatus (status: BookStatus) : string * string =
    match status with
    | Available -> "Available", "-"
    | Borrowed loan -> "Borrowed", loan.BorrowDate.ToString("yyyy-MM-dd")

/// (total, available, borrowed)
let countBooks (library: Library) : int * int * int =
    let available, borrowed = library |> allBooks |> List.partition isAvailable
    List.length available + List.length borrowed, List.length available, List.length borrowed

/// The books that should currently be shown (respects the search filter).
let visibleBooks (state: AppState) : Book list =
    if state.Filter = "" then allBooks state.Books
    else searchByTitle state.Filter state.Books

// ---------------------- save / load ----------------------

let dataFile : string = Path.Combine(AppContext.BaseDirectory, "library_data.txt")

/// One book -> one tab-separated line.
let bookToLine (book: Book) : string =
    let statusText, dateText = describeStatus book.Status
    String.Join("\t", [| string book.Id; book.Title; book.Author; book.Genre; statusText; dateText |])

/// One line -> Some book, or None if the line is damaged.
let lineToBook (line: string) : Book option =
    match line.Split('\t') with
    | [| idText; title; author; genre; status; dateText |] ->
        match Int32.TryParse(idText), status with
        | (true, n), "Available" ->
            Some { Id = n; Title = title; Author = author; Genre = genre; Status = Available }
        | (true, n), "Borrowed" ->
            match DateTime.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None) with
            | true, date ->
                Some { Id = n; Title = title; Author = author; Genre = genre; Status = Borrowed { BorrowDate = date } }
            | _ -> None
        | _ -> None
    | _ -> None

/// Save every book to the data file.
let saveLibrary (library: Library) : unit =
    try
        File.WriteAllLines(dataFile, library |> allBooks |> List.map bookToLine)
    with _ ->
        ()

/// Load the books from the data file (sample data if there is no usable file).
let loadLibrary () : Library =
    try
        if File.Exists dataFile then
            let loaded =
                File.ReadAllLines dataFile
                |> Array.toList
                |> List.choose lineToBook
                |> List.map (fun book -> book.Id, book)
                |> Map.ofList

            if Map.isEmpty loaded then seedLibrary else loaded
        else
            seedLibrary
    with _ ->
        seedLibrary