[<RequireQualifiedAccess>]
module Headers

// CodeHeader name of the header used as source of the HTTP status code to return
[<Literal>]
let CodeHeader = "X-Code"

let All = [
    CodeHeader
    "X-Format"
    "Content-Type"
    "X-Original-URI"
    "X-Namespace"
    "X-Ingress-Name"
    "X-Service-Name"
    "X-Service-Port"
    "X-Request-ID"
]