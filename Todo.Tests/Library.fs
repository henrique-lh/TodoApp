module Todo.Tests

open Xunit
open Swensen.Unquote
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost

[<Fact>]
let hello_world_tests() =
    let whb = new WebHostBuilder() |> TodoApp.configure
    let server = new TestServer(whb)
    let client = server.CreateClient()

    let response =
        task {
            let! response = client.GetAsync(@"\")
            let! content = response.Content.ReadAsStringAsync()
            return content
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously
        
    test <@ response = "Hello World" @>
