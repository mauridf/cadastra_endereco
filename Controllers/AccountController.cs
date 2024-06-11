using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using cadastra_endereco.Data;
using cadastra_endereco.Models;

public class AccountController : Controller
{
    private ApplicationDbContext _context;

    public AccountController()
    {
        _context = new ApplicationDbContext();
    }

    [HttpGet]
    public ActionResult Login(string returnUrl, string message)
    {
        ViewBag.Message = message;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(Usuario usuario, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email && u.Senha == usuario.Senha);

            if (user != null)
            {
                // Autenticar o usuário
                Session["UserID"] = user.Id.ToString();
                Session["UserEmail"] = user.Email.ToString();

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Endereco");
            }
            else
            {
                ModelState.AddModelError("", "Credenciais inválidas. Tente novamente.");
            }
        }
        return View(usuario);
    }

    [HttpGet]
    public ActionResult Logout()
    {
        Session.Clear();
        FormsAuthentication.SignOut();
        return RedirectToAction("Login", "Account");
    }

}