module PrivacyPolicy

open Giraffe
open Giraffe.ViewEngine
open Layout
open Microsoft.AspNetCore.Http

let view =
    [
        article [] [
            header [] [ h1 [] [ str "TeacherPouch Privacy Policy" ] ]
            p [] [ str "This Privacy Policy was last modified on January 17, 2014." ]
            p [] [
                str """
                    TeacherPouch LLC ("us", "we", or "our") operates https://teacherpouch.com (the "Site").
                    This page informs you of our policies regarding the collection, use and disclosure
                    of Personal Information we receive from users of the Site.
                    By using the Site, you agree to the collection and use of information in accordance with this policy.
                    """ ]
            p [] [
                strong [] [ str "Information Collection and Use" ]
                br []
                str """
                    While using our Site, we may utilize software such as Google Analytics
                    to collect non-personally-identifying information about your visit.
                    Google Analytics records things such as what search term brought you to our site,
                    what pages you visit, and what kind of computer and/or browser you're using.
                    We want to collection this anonymous information so we can better understand
                    what photos and tags people are looking for, so that we can better cater our
                    content to percieved demand.
                    """
                br []
                br []
                str "(Also remember, you can specifically request certain types of pictures or tags using the Contact form.)" ]
            p [] [
                strong [] [ str "Log Data" ]
                br []
                str """
                    Like many site operators, we collect information that
                    your browser sends whenever you visit our Site ("Log Data").
                    This Log Data may include information such as your computer's
                    Internet Protocol ("IP") address, browser type, browser version,
                    the pages of our Site that you visit, the time and date of your visit,
                    the time spent on those pages and other statistics.
                    """
            ]
            p [] [
                strong [] [ str "Cookies" ]
                br []
                str """
                    Cookies are files with small amount of data, which may include an anonymous unique identifier.
                    Cookies are sent to your browser from a web site and stored on your computer's hard drive.
                    """
                br []
                str """
                    Like many sites, we use "cookies" to collect information.
                    You can instruct your browser to refuse all cookies or to indicate when a cookie is being sent.
                    However, if you do not accept cookies, you may not be able to use some portions of our Site.
                    """
            ]
            p [] [
                strong [] [ str "Links To Other Sites" ]
                br []
                str """
                    Our Site may contain links to other sites that are not operated by us. If you click on a third party link,
                    you will be directed to that third party's site.
                    We strongly advise you to review the Privacy Policy of every site you visit.
                    """
                br []
                str """
                    TeacherPouch LLC has no control over, and assumes no responsibility for, the content, privacy policies,
                    or practices of any third party sites or services.
                    """
            ]
            p [] [
                strong [] [ str "Changes To This Privacy Policy" ]
                br []
                str """
                    TeacherPouch LLC may update this Privacy Policy from time to time. We will update the "last modified" date
                    at the top of this policy whenever updates are made.
                    We recommend that you review this Privacy Policy periodically for any changes.
                    """
            ]
            p [] [
                strong [] [ str "Contact Us" ]
                br []
                str """
                    If you have any questions about this Privacy Policy,
                    please contact us using the Contact link in the top navigation bar.
                    """
            ]
        ]
    ] |> layout "Privacy Policy"


let handler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        htmlView (view ctx.User) next ctx
