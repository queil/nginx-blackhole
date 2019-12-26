open System
open System.Threading
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Headers

[<EntryPoint>]
let main argv = 
  let cts = new CancellationTokenSource()
  let conf = { defaultConfig with cancellationToken = cts.Token }
   
  let passThruHeaders : WebPart =
    fun (ctx:HttpContext) -> 
       async { 
           let passThruHeader name = 
            match ctx |> getFirstHeader name with
             | Some value -> (name, value)::ctx.response.headers |> ignore
             | None -> ()  

           Headers.All |> Seq.iter passThruHeader
           return Some ctx 
        }
        
  let setStatusCodeFromXCodeHeader : WebPart =
    fun (ctx:HttpContext) -> 
        let newCode =  match ctx |> getFirstHeader Headers.CodeHeader with
                        | Some value -> Int32.Parse(value)
                        | None -> 404
        { ctx with response = {ctx.response with status = {ctx.response.status with code = newCode }}} |> succeed   

  let app =
    choose [
        GET >=> choose
            [ path "/healthz" >=> OK ""
              path "/" >=> passThruHeaders >=> setStatusCodeFromXCodeHeader ]
    ]
  
  let listening, server = startWebServerAsync conf app
    
  Async.Start(server, cts.Token)
  
  Console.CancelKeyPress.Add(fun _ -> cts.Cancel())
  Async.AwaitEvent Console.CancelKeyPress |> Async.RunSynchronously |> ignore 

  0 // return an integer exit code