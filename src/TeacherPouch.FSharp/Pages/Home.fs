module TeacherPouch.Pages.Home

open Giraffe
open Giraffe.ViewEngine
open Microsoft.AspNetCore.Http
open Domain
open Layout

let view =
    [
        div [ _id "home" ] [
            div [ _class "mb-3 p-5 text-center bg-body-tertiary rounded-3" ] [
                header [] [
                    div [ _id "intro-video-placeholder" ] [
                        a [
                            _href "javascript:;"
                            _onclick "PlayIntroVideo(this)"
                            _data "video" "//www.youtube.com/embed/0bwl3JaPM8Q?autoplay=1&rel=0"
                            _title "Watch a quick intro video about TeacherPouch"
                        ] [
                            img [
                                _src "/img/teacherpouch-video-placeholder.png"
                                _class "img-fluid"
                                _alt "TeacherPouch Intro Video"
                            ]
                        ]
                    ]
                    div [ _id "intro-video" ] []
                    div [] [
                        h5 [] [
                            str "Free stock photos for teachers and non-teachers alike."
                            br []
                            str "Come back periodically to see new photos that have been added!"
                        ]
                    ]
                ]
            ]
            div [ _class "row row-cols-3 featured-tags-preview" ] [
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Weather ) ] [
                        img [ _src "/img/weather.jpg"; _class "img-fluid"; _alt "Weather" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Winter ) ] [
                        img [ _src "/img/winter.jpg"; _class "img-fluid"; _alt "Winter" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Search.search "winter clothing" SearchOperator.Or) ] [
                        img [ _src "/img/winter-clothing.jpg"; _class "img-fluid"; _alt "Winter Clothing" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category School ) ] [
                        img [ _src "/img/school.jpg"; _class "img-fluid"; _alt "School" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Math ) ] [
                        img [ _src "/img/math.jpg"; _class "img-fluid"; _alt "Math" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Prepositions ) ] [
                        img [ _src "/img/prepositions.jpg"; _class "img-fluid"; _alt "Prepositions" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Transportation ) ] [
                        img [ _src "/img/transportation.jpg"; _class "img-fluid"; _alt "Transportation" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Vacation ) ] [
                        img [ _src "/img/vacation.jpg"; _class "img-fluid"; _alt "Vacation" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Sizes ) ] [
                        img [ _src "/img/sizes.jpg"; _class "img-fluid"; _alt "Sizes" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Food ) ] [
                        img [ _src "/img/food.jpg"; _class "img-fluid"; _alt "Food" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Animals ) ] [
                        img [ _src "/img/animals.jpg"; _class "img-fluid"; _alt "Animals" ]
                    ]
                ]
                div [ _class "col" ] [
                    a [ _href (Urls.Categories.category Numbers ) ] [
                        img [ _src "/img/numbers.jpg"; _class "img-fluid"; _alt "Numbers" ]
                    ]
                ]
            ]
        ]
    ] |> layout "Home"

let handler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        htmlView (view ctx.User) next ctx
