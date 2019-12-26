[<RequireQualifiedAccess>]
module View

let page (h:string) (msg:string) = 
    sprintf
        """ <html>
                <head>
                    <title>Nginx Blackhole</title>
                    <style type="text/css">
                        body {
                            font-family: sans-serif, mono
                        }
                    </style>
                </head>
                <body>
                    <h1>%s</h1>
                    <p>%s</p>
                    <p>You've reached nginx-blackhole - the default ingress-nginx backend. That usually indicates you have a problem with
                     either your ingress resource or your service or your pod.</p>
                </body>
            </html>
        """  h msg