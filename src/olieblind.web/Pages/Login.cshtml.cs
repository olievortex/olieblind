using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace olieblind.web.Pages;

[Authorize]
public class Login : PageModel
{
    public IActionResult OnGet()
    {
        return Redirect("/");
    }
}