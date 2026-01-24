module Domain

open System

type SearchOperator =
| And = 1
| Or  = 2

type Category =
| Animals
| CelebrationsAndHolidays
| Clothing
| Colors
| Fall
| Farm
| Flowers
| Food
| France
| Garden
| Italy
| Materials
| Math
| Numbers
| Prepositions
| School
| Sizes
| Summer
| Transportation
| Vacation
| Weather
| Winter
| Zoo

type Question = { Id: int; PhotoId: int; Text: string; SentenceStarters: string option; Order: int option }
type Tag =  { Id: int; Name: string; IsPrivate: bool }
type Photo = {
    Id: int
    Name: string
    UniqueId: Guid
    IsPrivate: bool
    PhotoTags: PhotoTag list
    Questions: Question list }
and PhotoTag = { PhotoId: int; Photo: Photo; TagId: int; Tag: Tag }
