using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using cadastra_endereco.Data;

public class AccountController : Controller
{
    private ApplicationDbContext _context;

    public AccountController()
    {
        _context = new ApplicationDbContext();
    }

    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string email, string senha)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.Senha == senha);
        if (usuario != null)
        {
            FormsAuthentication.SetAuthCookie(email, false);
            return RedirectToAction("Index", "Endereco");
        }
        ModelState.AddModelError("", "Credenciais inválidas");
        return View();
    }

    [HttpGet]
    public ActionResult Logout()
    {
        FormsAuthentication.SignOut();
        return RedirectToAction("Login");
    }
}