[<RequireQualifiedAccess>]
module Headers

// FormatHeader name of the header used to extract the format
[<Literal>]
let FormatHeader = "X-Format"

// CodeHeader name of the header used as source of the HTTP status code to return
[<Literal>]
let CodeHeader = "X-Code"

// ContentType name of the header that defines the format of the reply
[<Literal>]
let ContentType = "Content-Type"

// OriginalURI name of the header with the original URL from NGINX
[<Literal>]
let OriginalURI = "X-Original-URI"

// Namespace name of the header that contains information about the Ingress namespace
[<Literal>]
let Namespace = "X-Namespace"

// IngressName name of the header that contains the matched Ingress
[<Literal>]
let IngressName = "X-Ingress-Name"

// ServiceName name of the header that contains the matched Service in the Ingress
[<Literal>]
let ServiceName = "X-Service-Name"

// ServicePort name of the header that contains the matched Service port in the Ingress
[<Literal>]
let ServicePort = "X-Service-Port"

// RequestId is a unique ID that identifies the request - same as for backend service
[<Literal>]
let RequestId = "X-Request-ID"

let All = [
    FormatHeader
    CodeHeader
    ContentType
    OriginalURI
    Namespace
    IngressName
    ServiceName
    ServicePort
    RequestId
     ]