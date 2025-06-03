namespace PBL5sem.Models
{
    public class UsuarioViewModel : PadraoViewModel
    {
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public int EmpresaId { get; set; }
        public int CargoId { get; set; }

        // Adicionados:
        public string NomeEmpresa { get; set; }
        public byte[] LogoEmpresa { get; set; }

        public string NomeCargo { get; set; }
    }
}
