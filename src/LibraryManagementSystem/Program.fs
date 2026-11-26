module Program

open System
open System.Windows.Forms

[<EntryPoint>]
[<STAThread>]
let main _ =
    Application.EnableVisualStyles()
    Application.Run(GUI.buildForm ())
    0
