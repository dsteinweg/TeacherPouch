module Search

open Giraffe
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http
open Domain
open Layout
open System

let noneFound =
    [
        h2 [] [ str "No Matches" ]
        div [] [
            p [] [ str "Nothing found matching your query. Make sure your query is at least 2 characters." ]
            p [] [
                str "Feel free to email us using the"
                a [ _href Urls.Home.contact ] [ str "Contact" ]
                str "page to request this word."
            ]
        ]
    ] |> layout "No Matches"

let handler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        match ctx.TryGetQueryStringValue "q" with
        | None   -> htmlView (noneFound ctx.User) next ctx
        | Some q ->
            let op = match ctx.TryGetQueryStringValue "op" with
                     | None   -> SearchOperator.Or
                     | Some o ->
                        let mutable parsed = SearchOperator.Or
                        Enum.TryParse<SearchOperator>(o, &parsed) |> ignore
                        parsed
            match op with
            | SearchOperator.Or ->
                // search service call
                htmlView (noneFound ctx.User) next ctx // TODO
            | SearchOperator.And ->
                // search service call
                htmlView (noneFound ctx.User) next ctx // TODO
            | _ ->
                htmlView (noneFound ctx.User) next ctx // TODO
