open k8s
open System
open System.Threading
open Suave
open Suave.Filters
open Suave.FunctionalViewEngine
open Suave.Operators
open Suave.Successful
open Suave.Headers
open System.Net
open System.Text
open Microsoft.Extensions.Configuration
open System.Collections.Generic

[<CLIMutable>]
type AppSettings = {
  bindIp: string
  bindPort: uint16
  links : List<Link>
  
} and [<CLIMutable>]Link = {
  href: string
  text: string
}

[<EntryPoint>]
let main argv = 

  let cfg = ConfigurationBuilder().AddJsonFile("appsettings.json", false)
                                  .AddEnvironmentVariables("BLACKHOLE_").Build()
                              
  let settings = cfg.Get<AppSettings>()

  let cts = new CancellationTokenSource()
  let conf = { defaultConfig with 
                 cancellationToken = cts.Token
                 bindings = [HttpBinding.create HTTP (IPAddress.Parse(settings.bindIp)) settings.bindPort]
             }
  
  let links = if isNull settings.links then Seq.empty else settings.links |> Seq.map (fun f -> (f.text, f.href)) 

  let config = KubernetesClientConfiguration.BuildDefaultConfig()
  use k8s = new Kubernetes(config)
  let ingresses = k8s.ListIngressForAllNamespaces1().Items 
                    |> Seq.collect (fun x -> x.Spec.Rules) 
                    |> Seq.map (fun r -> 
                      ( r.Host, sprintf "https://%s" r.Host ))

  let allLinks = ingresses |> Seq.append links

  let passThruHeaders : WebPart =
    fun (ctx:HttpContext) -> 
       async { 
           let passThruHeader name = 
            match ctx |> getFirstHeader name with
             | Some value -> (name, value)::ctx.response.headers 
             | None -> []  

           return Some {ctx with response = {ctx.response with headers = Headers.All |> List.collect passThruHeader}} 
        }

  let passThruStatusCode : WebPart =
    fun (ctx:HttpContext) -> 
        let newCode =  match ctx |> getFirstHeader Headers.CodeHeader with
                        | Some value -> Int32.Parse(value)
                        | None -> 404
        let headerFromCtx key = match getFirstHeader key ctx with | Some v -> Some (key, v) | None -> None 
        
        let originalHeaders =
          Headers.All 
          |> Seq.map headerFromCtx
          |> Seq.choose id
         
        { ctx with 
            response = { ctx.response with
                          content = renderXmlNode (View.page newCode originalHeaders allLinks) |> Encoding.UTF8.GetBytes |> Bytes
                          status = {ctx.response.status with 
                                      code = newCode }}} |> succeed   
  let app =
    choose [
        GET >=> choose
            [ path "/healthz" >=> OK ""
              path "/" >=> passThruHeaders >=> passThruStatusCode ]
    ]
  
  let listening, server = startWebServerAsync conf app
    
  Async.Start(server, cts.Token)
  
  Console.CancelKeyPress.Add(fun _ -> cts.Cancel())
  Async.AwaitEvent Console.CancelKeyPress |> Async.RunSynchronously |> ignore 

  0