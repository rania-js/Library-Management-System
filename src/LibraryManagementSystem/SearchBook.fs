module Library.SearchBook

open Library.Types

let searchBook (lib: Library) (searchText: string) : Book list =

    let allBooks =
        lib.Books
        |> Map.toList
        |> List.map snd

    if System.String.IsNullOrWhiteSpace(searchText) then
        allBooks

    else
        allBooks
        |> List.filter (fun book ->
            book.Title.Contains(
                searchText.Trim(),
                System.StringComparison.OrdinalIgnoreCase
            )
        )
