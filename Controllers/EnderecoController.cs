using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using cadastra_endereco.Data;
using cadastra_endereco.Models;

public class EnderecoController : Controller
{
    private ApplicationDbContext _context;

    public EnderecoController()
    {
        _context = new ApplicationDbContext();
    }

    public ActionResult Index()
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


    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(Endereco endereco)
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
        var endereco = _context.Enderecos.Find(id);
        if (endereco != null)
        {
            _context.Enderecos.Remove(endereco);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
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


    [HttpGet]
    public async Task<ActionResult> GetEnderecoPorCep(string cep)
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


    [HttpGet]
    public ActionResult ExportCsv()
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
        return File(stream, "text/csv", "enderecos.csv");
    }
}