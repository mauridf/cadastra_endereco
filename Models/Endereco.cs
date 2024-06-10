using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cadastra_endereco.Models
{
    public class Endereco
    {
        public int Id { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}