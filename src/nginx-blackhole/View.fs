[<RequireQualifiedAccess>]
module View
open Suave.FunctionalViewEngine

let private renderKeyValue name renderSingle tuples = 
    match tuples |> Seq.toList with
     | [] -> []
     | xs ->
        [
            h2 [] [rawText name] 
            p [] (xs |> Seq.map renderSingle |> Seq.toList) 
        ]
let page (nginxStatusCode:int) 
         (headers: (string * string) seq)
         (links: (string * string) seq)
          = 
    html [] [
    head [] [
      meta [attr "charset" "UTF-8"]
      title [] [rawText "Nginx Blackhole"]
      link [attr "rel" "stylesheet"; attr "href" "https://gitcdn.link/cdn/Chalarangelo/mini.css/e849238d198c032c9d3fa84ccadf59ea7f0ad06c/dist/mini-nord.css" ]    
    ]
    body [] [
        section [] [
            h1 [] [
                rawText "⚫ Blackhole says: "
                b [] [rawText (nginxStatusCode |> string)]
                ]
            p [] [rawText "Welcome to Blackhole. You might have been lost. Probably your ingress is not working as expected."]
            section [] (links |> renderKeyValue "Links"  (fun (k, v) -> p [] [a [attr "href" v] [rawText (if k |> System.String.IsNullOrWhiteSpace then v else k)]]))
            section [] (headers |> renderKeyValue "Headers"  (fun (k, v) -> p [] [rawText (sprintf "%s: %s" k v)]))
        ]
        footer [] [p [] [rawText "⚫ Powered by Suave.IO and F#"]]
        ]
  ]
