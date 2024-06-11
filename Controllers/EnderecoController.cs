using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using cadastra_endereco.Data;
using cadastra_endereco.Models;
using System;

public class EnderecoController : Controller
{
    private ApplicationDbContext _context;

    public EnderecoController()
    {
        _context = new ApplicationDbContext();
    }

    public ActionResult Index()
    {
        try
        {
            // Verifica se a sessão do usuário está ativa
            if (Session["UserEmail"] == null)
            {
                // Redireciona para a tela de login se o usuário não estiver logado
                return RedirectToAction("Login", "Account");
            }

            var usuarioEmail = Session["UserEmail"].ToString();
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioEmail);

            if (usuario == null)
            {
                return HttpNotFound("Usuário não encontrado.");
            }

            var enderecos = _context.Enderecos.Where(e => e.UsuarioId == usuario.Id).ToList();
            return View(enderecos);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Ocorreu um erro ao carregar os endereços.";
            return View("Error");
        }
    }


    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(Endereco endereco)
    {
        try
        {
            // Verifica se a sessão do usuário está ativa
            if (Session["UserEmail"] == null)
            {
                // Redireciona para a tela de login se o usuário não estiver logado
                return RedirectToAction("Login", "Account");
            }

            // Obtém o email do usuário da sessão
            var usuarioEmail = Session["UserEmail"].ToString();
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioEmail);

            if (usuario == null)
            {
                return HttpNotFound("Usuário não encontrado.");
            }

            if (ModelState.IsValid)
            {
                endereco.UsuarioId = usuario.Id;
                _context.Enderecos.Add(endereco);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(endereco);
        }
        catch(Exception ex)
        {
            ViewBag.ErrorMessage = "Ocorreu um erro ao registrar um endereço.";
            return View("Error");
        }
    }

    [HttpGet]
    public ActionResult Delete(int id)
    {
        var endereco = _context.Enderecos.Find(id);
        if (endereco == null)
        {
            return HttpNotFound();
        }
        return View(endereco);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        try
        {
            var endereco = _context.Enderecos.Find(id);
            if (endereco != null)
            {
                _context.Enderecos.Remove(endereco);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        catch(Exception ex)
        {
            ViewBag.ErrorMessage = "Ocorreu um erro ao deletar um endereço.";
            return View("Error");
        }
    }

    [HttpGet]
    public ActionResult Edit(int id)
    {
        var endereco = _context.Enderecos.Find(id);
        if (endereco == null)
        {
            return HttpNotFound();
        }
        return View(endereco);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Endereco endereco)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var existingEndereco = _context.Enderecos.AsNoTracking().FirstOrDefault(e => e.Id == endereco.Id);
                if (existingEndereco == null)
                {
                    return HttpNotFound("Endereço não encontrado.");
                }

                endereco.UsuarioId = existingEndereco.UsuarioId; // Preserve o UsuarioId existente

                _context.Entry(endereco).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(endereco);
        }
        catch(Exception ex)
        {
            ViewBag.ErrorMessage = "Ocorreu um erro ao Editar um endereço.";
            return View("Error");
        }
    }


    [HttpGet]
    public async Task<ActionResult> GetEnderecoPorCep(string cep)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://viacep.com.br/ws/{cep}/json/");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var endereco = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                    return Json(endereco, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        catch(Exception ex)
        {
            ViewBag.ErrorMessage = "Ocorreu um erro ao tentar buscar o Endereço pelo CEP.";
            return View("Error");
        }
    }


    [HttpGet]
    public ActionResult ExportCsv()
    {
        try
        {
            // Verifica se a sessão do usuário está ativa
            if (Session["UserEmail"] == null)
            {
                // Redireciona para a tela de login se o usuário não estiver logado
                return RedirectToAction("Login", "Account");
            }

            // Obtém o email do usuário da sessão
            var usuarioEmail = Session["UserEmail"].ToString();
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioEmail);

            if (usuario == null)
            {
                return HttpNotFound("Usuário não encontrado.");
            }

            var enderecos = _context.Enderecos.Where(e => e.UsuarioId == usuario.Id).ToList();
            var csv = new StringBuilder();
            csv.AppendLine("Cep,Logradouro,Bairro,Cidade,Estado");

            foreach (var endereco in enderecos)
            {
                csv.AppendLine($"{endereco.Cep},{endereco.Logradouro},{endereco.Bairro},{endereco.Cidade},{endereco.Estado}");
            }

            var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
            var stream = new System.IO.MemoryStream(byteArray);
            return File(stream, "text/csv", "lista_de_enderecos.csv");
        }
        catch(Exception ex)
        {
            ViewBag.ErrorMessage = "Ocorreu um erro ao tentar gerar o arquivo CSV.";
            return View("Error");
        }
    }
}