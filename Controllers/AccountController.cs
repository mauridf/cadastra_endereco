using System;
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
        try
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
        catch(Exception ex)
        {
            ViewBag.ErrorMessage = "Ocorreu uma excessão ao efetuar Login.";
            return View("Error");
        }
    }

    [HttpGet]
    public ActionResult Cadastro()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Cadastro(Usuario usuario)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Verificar se o e-mail já está cadastrado
                var existingUser = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está sendo utilizado.");
                    return View(usuario);
                }

                // Adicionar o usuário
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                // Redirecionar para a tela de login após o cadastro
                return RedirectToAction("Login");
            }
            return View(usuario);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ocorreu um erro ao cadastrar um usuário.";
            return View("Error");
        }
    }

    [HttpGet]
    public ActionResult Logout()
    {
        Session.Clear();
        FormsAuthentication.SignOut();
        return RedirectToAction("Login", "Account");
    }

}