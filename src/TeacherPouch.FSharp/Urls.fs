module Urls

open Domain

module Admin =
    let [<Literal>] path = "/admin"
    let [<Literal>] index = path
    let [<Literal>] signIn = path + "/sign-in"
    let [<Literal>] signOut = path + "/sign-out"

module Home =
    let [<Literal>] home = "/"
    let [<Literal>] contact = "/contact"
    let [<Literal>] privacyPolicy = "/privacy-policy"

module Search =
    let [<Literal>] path = "/search"
    let search (query : string) (operator : SearchOperator) = $"{path}?q={query}&op={operator}"

module Categories =
    let category (category : Category) = $"/categories/{category.ToString().ToLower()}"

module Photos =
    let [<Literal>] path = "/photos"
    let [<Literal>] create = path + "/create"
