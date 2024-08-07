# Path learning

Here I plan to a todo app in F#. This document keep tracking of everything I'm learning. 
And yeah, the purpose is this file looks like I'm talking to myself, so I'll take notes, make random jokes and probably it won't be useful you, who's reading this. But, it'll be fun for me (I learn better in this way) 

## Creating the app

First, I need to create a template, and specify "it is in the F# language"

```bash
dotnet new <template-name> -lang F#
```

Check the [Default templates](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new-sdk-templates#classlib) -> dotnet is so cool

I can use different templates, but remember, it is importante to crete new folders and run this comand above

I had a problem using the `web` and `classlib` templates.

### modules

Well, create templates is basically create new modules. I can import them using the `module` word (look the Program.fs). It is interesting because to import a file into another, I need to change the .fsproj (thanks chatgpt). So, if I could understand well, the syntax is

```xml
<ItemGroup>
    <ProjectReference Include="../todo.fsproj" />
</ItemGroup>
```

### build

Libraries need to be installed in the correct folder here! So, if you make a mistake, remember to change it and build

To build your project

```bash
dotnet build # looks for the first fsproj file
dotnet build --source /path/to/dir # it will look inside this dir and point to the fsproj
```

### test

For testing
```bash
dotnet test
```

It is pretty simple actually, I could know that because of the Exercism.io (btw I need to go back there)

### run

```bash
dotnet run
```

It do it in the root!!

## Installing packages

Do it by:

```bash
dotnet add packageg <name-of-the-packageg>
```

## The anatomy of a F# web app

Ok, here we go, to undertand each part of the Program.fs

### Import modules

Well, I already know what it is, it is pretty basic. Maybe I should look for what each of these modules do! It's quite different from a import in python
```F#
module TodoApp

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
```

Defining the routes. It is the way F# + Giraffe deal with that. Right now it is soo basic, but maybe that is why I should look for Saturn (?). Well, the `choose` is responsible for that.
```F#
let webApp =
    choose [
        route "/ping" >=> text "pong"
        route "/"     >=> text "Hello World" ]
```

Here, it is configured the middleware, maybe I should not worry about it rn
```F#
let configureApp (app: IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe webApp
```

More configurations. Here we add the Giraffe Services and configure the host
```F#
let configureServices (services: IServiceCollection) =
    // Add Giraffe dependency
    services.AddGiraffe() |> ignore

let configure (webHostBuilder: IWebHostBuilder) =
    webHostBuilder
        .Configure(configureApp)
        .ConfigureServices(configureServices)
```

Ok, now it's how we run the program, here everything is built up
```F#
[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(configure >> ignore)
        .Build()
        .Run()
    0
```
