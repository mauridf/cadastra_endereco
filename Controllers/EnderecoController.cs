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

    [Authorize]
    public ActionResult Index()
    {
        var usuarioEmail = User.Identity.Name;
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioEmail);

        var enderecos = _context.Enderecos.Where(e => e.UsuarioId == usuario.Id).ToList();
        return View(enderecos);
    }

    [HttpGet]
    [Authorize]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public ActionResult Create(Endereco endereco)
    {
        var usuarioEmail = User.Identity.Name;
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioEmail);

        if (ModelState.IsValid)
        {
            endereco.UsuarioId = usuario.Id;
            _context.Enderecos.Add(endereco);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(endereco);
    }

    [Authorize]
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

    [Authorize]
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

    [Authorize]
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

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Endereco endereco)
    {
        if (ModelState.IsValid)
        {
            _context.Entry(endereco).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(endereco);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> GetEnderecoPorCep(string cep)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync($"https://viacep.com.br/ws/{cep}/json/");
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();
                var endereco = JsonConvert.DeserializeObject<Endereco>(jsonResult);
                return Json(endereco, JsonRequestBehavior.AllowGet);
            }
        }
        return Json(null, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [Authorize]
    public ActionResult ExportCsv()
    {
        var usuarioEmail = User.Identity.Name;
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioEmail);

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