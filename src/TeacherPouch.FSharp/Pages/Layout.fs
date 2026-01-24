module Layout

open System
open System.Security.Claims
open Giraffe.ViewEngine
open Domain

let categoriesSideNav =
    nav [ _class "card" ] [
        div [ _class "card-header" ] [
            div [ _id "logo" ] [
                a [ _class "brand"; _href "/" ] [
                    img [
                        _src "/img/logo/TeacherPouchLogo30.png";
                        _class "img-fluid text-center";
                        _alt "TeacherPouch logo"
                    ]
                ]
            ]
        ]
        div [ _class "list-group list-group-flush" ]
            (DiscriminatedUnionHelper.getAllUnionCases<Category>()
            |> Seq.map (fun category ->
                a [ _class "list-group-item list-group-item-action"; _href (Urls.Categories.category category) ] [
                    str (category.ToString())
                ])
            |> Seq.toList)
    ]

let layout (pageTitle : string) (content : XmlNode list) (user : ClaimsPrincipal) =
    html [ _lang "en"; _data "bs-theme" "dark" ] [
        head [] [
            meta [ _charset "utf-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1" ]
            title [] [ encodedText $"{pageTitle} - TeacherPouch" ]
            link [ _rel "stylesheet"; _href "/lib/bootstrap/bootstrap.min.css" ]
            link [ _rel "stylesheet"; _href "/lib/bootstrap-icons/bootstrap-icons.min.css" ]
            link [ _rel "stylesheet"; _href "/main.css" ]
            link [ _rel "icon"; _href "/favicon.ico"; _sizes "16x16" ]
        ]
        body [] [
            nav [ _class "navbar navbar-expand-lg bg-body-tertiary" ] [
                div [ _class "container" ] [
                    a [ _class "navbar-brand"; _href "/" ] [ str "TeacherPouch" ]
                    button [ _class "navbar-toggler"; _type "button"; _data "bs-toggle" "collapse"; _data "bs-target" "#navbarSupportedContent" ] [
                        span [ _class "navbar-toggler-icon" ] []
                    ]
                    div [ _id "navbarSupportedContent"; _class "collapse navbar-collapse" ] [
                        ul [ _class "navbar-nav me-auto mb-2 mb-lg-0" ] [
                            li [ _class "nav-item" ] [ a [ _class "nav-link"; _href "/license"   ] [ str "License"   ] ]
                            li [ _class "nav-item" ] [ a [ _class "nav-link"; _href "/standards" ] [ str "Standards" ] ]
                            li [ _class "nav-item" ] [ a [ _class "nav-link"; _href "/aboout"    ] [ str "About"     ] ]
                            li [ _class "nav-item" ] [ a [ _class "nav-link"; _href "/contact"   ] [ str "Contact"   ] ]
                        ]
                        form [ _id "search-form"; _action "/search"; _method "get"; _class "d-flex"; attr "role" "search" ] [
                            div [ _class "input-group" ] [
                                input [ _id "q"; _name "q";  _type "text"; _class "form-control"; _placeholder "Search" ]
                                div [ _class "input-group-text"] [
                                    div [ _class "form-check" ] [
                                        input [ _id "or"; _class "form-check-input"; _type "radio"; _name "op"; _value "Or"; _checked ]
                                        label [ _for "or"; _class "form-check-label" ] [ str "Or" ]
                                    ]
                                    div [ _class "form-check" ] [
                                        input [ _id "and"; _class "form-check-input"; _type "radio"; _name "op"; _value "And" ]
                                        label [ _for "and"; _class "form-check-label" ] [ str "And" ]
                                    ]
                                ]
                                button [ _class "btn btn-outline-secondary"; _type "submit" ] [
                                    i [ _class "bi bi-search" ] []
                                ]
                            ]
                        ]
                    ]
                ]
            ]
            div [ _class "container my-3" ] [
                div [ _class "row" ] [
                    div [ _class "main col-sm-9 order-sm-1 col-md-10 order-md-2"] content
                    div [ _class "     col-sm-3 order-sm-2 col-md-2  order-md-1" ] [ categoriesSideNav ]
                    if user.Identity.IsAuthenticated then
                        ul [ _class "nav"; _style "margin-top: 20px;" ] [
                            li [] [ str "Administration" ]
                            li [] [ a [ _href "" ] [ str "Admin Home" ]]
                            if user.IsInRole(Roles.admin) then
                                li [] [ a [ _href Urls.Photos.create ] [ str "Add new Photo" ]]
                        ]
                ]
                hr []
                footer [] [
                    p [] [
                        rawText $"&copy; {DateTime.Now.Year} TeacherPouch LLC - "
                        a [ _href Urls.Home.privacyPolicy ] [ str "Privacy Policy" ]
                    ]
                    p [] [
                        if user.Identity.IsAuthenticated then
                            a [ _href Urls.Admin.index ] [ str "Site Administration" ]
                            div [] [
                                span [] [ str $"Hello, {user.Identity.Name}" ]
                                a [ _href Urls.Admin.signOut ] [ str "Sign Out" ]
                            ]
                        else
                                a [ _href Urls.Admin.signIn ] [ str "Sign In" ]
                    ]
                ]
            ]
            script [ _src "/lib/bootstrap/bootstrap.bundle.min.js" ] []
            script [ _src "/lib/jquery-3.7.1.min.js" ] []
            script [ _src "/lib/jquery-ui.1.13.2.min.js" ] []
            script [ _src "/scripts/TeacherPouch.js" ] []
        ]
    ]
