﻿module Todo.Tests

open Xunit
open Swensen.Unquote
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open System.Text.Json

[<Fact>]
let ``App bootstraps correctly``() =
    let whb = new WebHostBuilder() |> TodoApp.configure (fun _ -> [])
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


[<Fact>]
let ``Can get all list of a user``() =
    let lists: Domain.ToDoList list = 
        [ { name = "books"; 
            description = "my bookshelf";
            status = Domain.Status.Todo; 
            percentageDone = 0m } ]
    
    let db = [ ("luis", lists) ] |> Map.ofList

    let mapFetcher username = db |> Map.find username

    let whb = new WebHostBuilder() |> TodoApp.configure mapFetcher
    let server = new TestServer(whb)
    let client = server.CreateClient()

    let response =
        task {
            let! response = client.GetAsync(@"\luis\lists")
            let! stream = response.Content.ReadAsStreamAsync()
            let! content = JsonSerializer.DeserializeAsync<Contract.GetListsResponse>(stream)
            return content
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously
        
    test <@ response = { lists = 
                                        [| { name = "books"; 
                                            description = "my bookshelf";
                                            status = Contract.Status.Todo;
                                            percentageDone = 0m } |]} @>
