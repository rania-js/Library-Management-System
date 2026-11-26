// ================================================================
//  Gui.fs  -  Person 7
//  Windows Forms design + linking the buttons to the functions.
// ================================================================

module GUI

open System
open System.Drawing
open System.Windows.Forms
open Types
open AddBook
open BorrowBook
open ReturnBook
open DeleteBook
open DisplayBooks

// ----------------------------------------------------------------
// 1) Calm color palette
// ----------------------------------------------------------------

let bgColor = Color.FromArgb(244, 247, 246)
let headerColor = Color.FromArgb(62, 110, 115)
let headerSubColor = Color.FromArgb(200, 225, 222)
let cardColor = Color.White
let textColor = Color.FromArgb(55, 71, 79)
let mutedColor = Color.FromArgb(120, 144, 156)
let lineColor = Color.FromArgb(225, 232, 232)
let primaryColor = Color.FromArgb(92, 155, 150)    // teal  (Add / Search)
let borrowColor = Color.FromArgb(110, 140, 180)    // soft blue
let returnColor = Color.FromArgb(140, 170, 130)    // sage green
let deleteColor = Color.FromArgb(196, 100, 100)   // soft red (Delete)
let neutralColor = Color.FromArgb(150, 165, 172)   // grey (Show all)
let availableRow = Color.FromArgb(228, 242, 233)
let borrowedRow = Color.FromArgb(251, 232, 228)
let successColor = Color.FromArgb(60, 130, 100)
let errorColor = Color.FromArgb(185, 85, 85)

/// Row color depends on the book status (pattern matching).
let rowColorOf (status: BookStatus) : Color =
    match status with
    | Available -> availableRow
    | Borrowed _ -> borrowedRow

// ----------------------------------------------------------------
// 2) Small control factories
// ----------------------------------------------------------------

let add (parent: Control) (child: #Control) = parent.Controls.Add child

let makeLabel (text: string) (x: int) (y: int) (size: float32) (bold: bool) (color: Color) =
    let style = if bold then FontStyle.Bold else FontStyle.Regular
    new Label(
        Text = text,
        AutoSize = true,
        Location = Point(x, y),
        ForeColor = color,
        BackColor = Color.Transparent,
        Font = new Font("Segoe UI", size, style)
    )

let makeTextBox (x: int) (y: int) (w: int) =
    new TextBox(
        Location = Point(x, y),
        Size = Size(w, 28),
        Font = new Font("Segoe UI", 10.5f),
        BorderStyle = BorderStyle.FixedSingle,
        BackColor = Color.FromArgb(250, 252, 252),
        ForeColor = textColor
    )

let makeButton (text: string) (color: Color) (x: int) (y: int) (w: int) =
    let b =
        new Button(
            Text = text,
            Location = Point(x, y),
            Size = Size(w, 38),
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 10.0f, FontStyle.Bold)
        )
    b.FlatAppearance.BorderSize <- 0
    b

let makeSeparator (x: int) (y: int) =
    new Panel(Location = Point(x, y), Size = Size(300, 1), BackColor = lineColor)

/// Converts a Book (data) into a ListView row (view).
let toListItem (book: Book) : ListViewItem =
    let statusText, dateText = describeStatus book.Status
    let item = new ListViewItem([| string book.Id; book.Title; book.Author; book.Genre; statusText; dateText |])
    item.Tag <- box book.Id
    item.BackColor <- rowColorOf book.Status
    item

// ----------------------------------------------------------------
// 3) Build the main window
// ----------------------------------------------------------------

let buildForm () : Form =

    // The ONLY mutable thing in the app: one reference cell that holds
    // an immutable AppState. Every change replaces it with a NEW state.
    let state = ref { Books = loadLibrary (); Filter = "" }

    // ----- window -----
    // The window never gets bigger than the usable screen area,
    // so it fits small laptop screens too.
    let workArea = Screen.PrimaryScreen.WorkingArea
    let form =
        new Form(
            Text = "Library Management System",
            Size = Size(min 1100 workArea.Width, min 740 workArea.Height),
            MinimumSize = Size(min 800 workArea.Width, min 480 workArea.Height),
            StartPosition = FormStartPosition.CenterScreen,
            BackColor = bgColor,
            Font = new Font("Segoe UI", 10.0f)
        )

    // ----- header -----
    let header = new Panel(Dock = DockStyle.Top, Height = 75, BackColor = headerColor)
    add header (makeLabel "Library Management System" 25 10 18.0f true Color.White)
    add header (makeLabel "Add, search, borrow, return and delete books" 27 45 9.5f false headerSubColor)

    let countsLbl =
        new Label(
            AutoSize = true,
            Anchor = (AnchorStyles.Top ||| AnchorStyles.Right),
            Location = Point(700, 27),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11.0f, FontStyle.Bold)
        )
    add header countsLbl

    // ----- status bar -----
    let statusBar = new Panel(Dock = DockStyle.Bottom, Height = 34, BackColor = Color.FromArgb(232, 238, 237))
    let statusLbl =
        new Label(AutoSize = true, Location = Point(20, 8), ForeColor = mutedColor, Font = new Font("Segoe UI", 9.5f))
    add statusBar statusLbl

    let setStatus (message: string) (color: Color) =
        statusLbl.Text <- message
        statusLbl.ForeColor <- color

    // ----- left card -----
    let card =
        new Panel(
            Dock = DockStyle.Left,
            Width = 360,
            BackColor = cardColor,
            AutoScroll = true   // scrollbar appears when the screen is too short
        )

    // Add a new book
    add card (makeLabel "Add New Book" 18 14 12.0f true headerColor)
    add card (makeLabel "Title" 20 48 9.5f false mutedColor)
    let txtTitle = makeTextBox 20 70 300
    add card txtTitle
    add card (makeLabel "Author" 20 108 9.5f false mutedColor)
    let txtAuthor = makeTextBox 20 130 300
    add card txtAuthor
    add card (makeLabel "Genre" 20 168 9.5f false mutedColor)
    let txtGenre = makeTextBox 20 190 300
    add card txtGenre
    let btnAdd = makeButton "Add Book" primaryColor 20 232 300
    add card btnAdd
    add card (makeSeparator 20 290)

    // Search
    add card (makeLabel "Search Books" 18 304 12.0f true headerColor)
    let txtSearch = makeTextBox 20 338 300
    add card txtSearch
    let btnSearch = makeButton "Search" primaryColor 20 378 145
    let btnShowAll = makeButton "Show All" neutralColor 175 378 145
    add card btnSearch
    add card btnShowAll
    add card (makeSeparator 20 436)

    // Borrow / Return
    add card (makeLabel "Book Actions" 18 450 12.0f true headerColor)
    add card (makeLabel "Select a book from the list, then click:" 20 482 9.0f false mutedColor)
    let btnBorrow = makeButton "Borrow" borrowColor 20 508 145
    let btnReturn = makeButton "Return" returnColor 175 508 145
    add card btnBorrow
    add card btnReturn
    let btnDelete = makeButton "Delete" deleteColor 20 556 300
    add card btnDelete

    // ----- right: books list -----
    let lv =
        new ListView(
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            MultiSelect = false,
            HideSelection = false,
            GridLines = false,
            BorderStyle = BorderStyle.None,
            BackColor = cardColor,
            ForeColor = textColor,
            Font = new Font("Segoe UI", 10.5f)
        )

    [ ("#", 45); ("Title", 230); ("Author", 140); ("Genre", 110); ("Status", 80); ("Borrow Date", 95) ]
    |> List.iter (fun (name, width) -> lv.Columns.Add(name, width) |> ignore)

    // ------------------------------------------------------------
    // Drawing the state on the screen
    // ------------------------------------------------------------

    let selectedId () : int option =
        if lv.SelectedItems.Count = 0 then None
        else Some(unbox<int> lv.SelectedItems.[0].Tag)

    let selectItem (id: int) =
        lv.Items
        |> Seq.cast<ListViewItem>
        |> Seq.tryFind (fun item -> unbox<int> item.Tag = id)
        |> Option.iter (fun item ->
            item.Selected <- true
            item.EnsureVisible())

    /// Re-draw the list and the counters from the current state.
    let refresh () =
        let previouslySelected = selectedId ()

        lv.BeginUpdate()
        lv.Items.Clear()
        visibleBooks state.Value
        |> List.map toListItem
        |> Array.ofList
        |> lv.Items.AddRange
        lv.EndUpdate()

        previouslySelected |> Option.iter selectItem

        let total, available, borrowed = countBooks state.Value.Books
        countsLbl.Text <- sprintf "Total: %d     Available: %d     Borrowed: %d" total available borrowed
        countsLbl.Left <- header.Width - countsLbl.Width - 25

    let showError (message: string) =
        setStatus message errorColor
        MessageBox.Show(message, "Library", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore

    // ------------------------------------------------------------
    // Actions (each one takes functions as arguments)
    // ------------------------------------------------------------

    /// Runs a logic function (Library -> Result<Library,string>).
    /// Success: replace the state with a new one and redraw.
    /// Error:   show the message.
    let apply
        (operation: Library -> Result<Library, string>)
        (successMessage: string)
        (afterSuccess: unit -> unit)
        =
        match operation state.Value.Books with
        | Ok updated ->
            state.Value <- { state.Value with Books = updated }
            saveLibrary updated
            afterSuccess ()
            refresh ()
            setStatus successMessage successColor
        | Error message -> showError message

    /// Runs an action that needs a selected book.
    let withSelectedBook (action: int -> unit) =
        match selectedId () with
        | Some id -> action id
        | None -> showError "Please select a book from the list first."

    let search (text: string) =
        state.Value <- { state.Value with Filter = text.Trim() }
        refresh ()

        match state.Value.Filter with
        | "" -> setStatus "Showing all books." mutedColor
        | query ->
            match List.length (visibleBooks state.Value) with
            | 0 -> setStatus (sprintf "No books found matching \"%s\"." query) errorColor
            | n -> setStatus (sprintf "Found %d book(s) matching \"%s\"." n query) successColor

    let clearAddForm () =
        [ txtTitle; txtAuthor; txtGenre ] |> List.iter (fun (tb: TextBox) -> tb.Clear())
        txtTitle.Focus() |> ignore

    // ------------------------------------------------------------
    // Wiring events
    // ------------------------------------------------------------

    btnAdd.Click.Add(fun _ ->
        let title = txtTitle.Text.Trim()
        apply (addBook txtTitle.Text txtAuthor.Text txtGenre.Text)
              (sprintf "\"%s\" was added successfully." title)
              clearAddForm)

    btnSearch.Click.Add(fun _ -> search txtSearch.Text)

    txtSearch.KeyDown.Add(fun e ->
        if e.KeyCode = Keys.Enter then
            e.SuppressKeyPress <- true
            search txtSearch.Text)

    btnShowAll.Click.Add(fun _ ->
        txtSearch.Clear()
        search "")

    btnBorrow.Click.Add(fun _ ->
        withSelectedBook (fun id ->
            let today = DateTime.Today
            apply (borrowBook id today)
                  (sprintf "Book borrowed on %s." (today.ToString("yyyy-MM-dd")))
                  ignore))

    btnReturn.Click.Add(fun _ ->
        withSelectedBook (fun id ->
            apply (returnBook id) "Book returned and is now available." ignore))

    btnDelete.Click.Add(fun _ ->
        withSelectedBook (fun id ->
            // First check that the delete is allowed (e.g. not borrowed)...
            match deleteBook id state.Value.Books with
            | Error message -> showError message
            | Ok _ ->
                // ...then ask for confirmation. "No" is the default button.
                let title =
                    state.Value.Books
                    |> Map.tryFind id
                    |> Option.map (fun b -> b.Title)
                    |> Option.defaultValue "this book"

                let answer =
                    MessageBox.Show(
                        sprintf "Are you sure you want to delete \"%s\"?\nThis cannot be undone." title,
                        "Confirm delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2)

                if answer = DialogResult.Yes then
                    apply (deleteBook id) (sprintf "\"%s\" was deleted." title) ignore
                else
                    setStatus "Delete cancelled." mutedColor))

    // ----- assemble -----
    // The last control added is docked first: card (left), spacer, list (fills the rest).
    let spacer = new Panel(Dock = DockStyle.Left, Width = 20, BackColor = bgColor)
    let content = new Panel(Dock = DockStyle.Fill, Padding = Padding(20, 17, 20, 15), BackColor = bgColor)
    add content lv
    add content spacer
    add content card

    add form content
    add form statusBar
    add form header

    form.Shown.Add(fun _ ->
        refresh ()
        setStatus "Ready." mutedColor)

    form
