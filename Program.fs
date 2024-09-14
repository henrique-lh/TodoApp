module TodoApp

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.Text.Json.Serialization

let getListForUserHandler (fetcher: Domain.ListFetcher) user = 
    let lists = fetcher user 
    let mapped = lists |> List.map Contract.fromDomain
    json lists

let webApp (fetcher: Domain.ListFetcher) =
    choose [ 
            route "/"     >=> text "Hello World" 
            routef "/%s/lists" (getListForUserHandler fetcher)
        ]

let configureApp fetcher (app: IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe(webApp fetcher)

let jsonOptions =
    let options = Json.Serializer.DefaultOptions
    options.Converters.Add(JsonFSharpConverter(JsonUnionEncoding.FSharpLuLike))
    options

let configureServices (services: IServiceCollection) =
    services
        .AddGiraffe()
        .AddSingleton<Json.ISerializer>(Json.Serializer(jsonOptions))
    |> ignore

let configure fetcher (webHostBuilder: IWebHostBuilder) =
    webHostBuilder
        .Configure(configureApp fetcher)
        .ConfigureServices(configureServices)

[<EntryPoint>]
let main _ =
    let fetcher: Domain.ListFetcher = (fun _ ->
                        [ { name = "books"; 
                               description = "my bookshelf";
                               status = Domain.Status.Todo;
                               percentageDone = 0m } ])

    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults((configure fetcher)  >> ignore)
        .Build()
        .Run()
    0
