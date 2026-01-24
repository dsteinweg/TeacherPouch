module TeacherPouch.FSharp.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.Security.Claims
open Microsoft.AspNetCore.Http
open TeacherPouch.Pages

// ---------------------------------
// Models
// ---------------------------------

type Message =
    {
        Text : string
    }

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open Giraffe.ViewEngine

    //let partial () =
    //    h1 [] [ str "TeacherPouch" ]

    //let index (model : Message) =
    //    [
    //        partial()
    //        p [] [ encodedText model.Text ]
    //    ] |> layout "Home"

// authn
let notLoggedIn = RequestErrors.UNAUTHORIZED "Basic" "Some Realm" "You must be logged in."
let notAdmin = RequestErrors.FORBIDDEN "Permission denied. You must be an admin."
let mustBeLoggedIn = requiresAuthentication notLoggedIn
let mustBeAdmin = requiresRole "Admin" notAdmin

// ---------------------------------
// Web app
// ---------------------------------

//let indexHandler (name : string) : HttpHandler =
//    fun (next : HttpFunc) (ctx : HttpContext) ->
//        let greetings = sprintf "Hello %s, from Giraffe3!" name
//        let model     = { Text = greetings }
//        let view      = Views.index model ctx.User
//        htmlView view next ctx

let adminHandler =
    mustBeLoggedIn >=> mustBeAdmin >=> route ""

let webApp =
    choose [
        routeStartsWithCi Urls.Admin.path >=> adminHandler
        GET >=>
            choose [
                routeCi Urls.Home.home >=> Home.handler
                routeStartsWithCi Urls.Search.path >=> Search.handler
                routeCi Urls.Home.privacyPolicy >=> PrivacyPolicy.handler
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

//let configureCors (builder : CorsPolicyBuilder) =
//    builder.WithOrigins("https://localhost")
//           .AllowAnyMethod()
//           .AllowAnyHeader()
//           |> ignore

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  ->
        app.UseDeveloperExceptionPage()
    | false ->
        app.UseGiraffeErrorHandler(errorHandler)
           .UseHttpsRedirection())
           //.UseCors(configureCors)
           .UseStaticFiles()
           .UseGiraffe(webApp)

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "wwwroot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    |> ignore)
        .Build()
        .Run()
    0
