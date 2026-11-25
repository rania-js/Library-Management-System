// For more information see https://aka.ms/fsharp-console-apps
open System
open System.Windows.Forms

[<EntryPoint; STAThread>]
let main _ =
    let form = new Form(Text = "اختبار GUI - LibraryManagementSystem", Size = Drawing.Size(400, 200))
    let btn = new Button(Text = "شغال تمام", Location = Drawing.Point(130, 60), Size = Drawing.Size(120, 40))
    btn.Click.Add(fun _ -> MessageBox.Show("الواجهة الرسومية شغالة عادي!") |> ignore)
    form.Controls.Add(btn)
    Application.Run(form)
    0
