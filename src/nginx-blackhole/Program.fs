open System
open System.Threading
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Headers
open System.Net
open System.Text

[<EntryPoint>]
let main argv = 
  let cts = new CancellationTokenSource()
  let conf = { defaultConfig with 
                 cancellationToken = cts.Token
                 bindings = [HttpBinding.create HTTP IPAddress.Any 8080us]
             }
   
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

  let passThruStatusCode (message:int -> string) : WebPart =
    fun (ctx:HttpContext) -> 
        let newCode =  match ctx |> getFirstHeader Headers.CodeHeader with
                        | Some value -> Int32.Parse(value)
                        | None -> 404
        { ctx with 
            response = { ctx.response with
                          content = (View.page "Blackhole says" (message newCode) ) |> Encoding.UTF8.GetBytes |> Bytes
                          status = {ctx.response.status with 
                                      code = newCode }}} |> succeed   

  let app =
    choose [
        GET >=> choose
            [ path "/healthz" >=> OK ""
              path "/" >=> passThruHeaders >=> passThruStatusCode (sprintf "Your error code is %i. Check response headers for more details.") ]
    ]
  
  let listening, server = startWebServerAsync conf app
    
  Async.Start(server, cts.Token)
  
  Console.CancelKeyPress.Add(fun _ -> cts.Cancel())
  Async.AwaitEvent Console.CancelKeyPress |> Async.RunSynchronously |> ignore 

  0 // return an integer exit code