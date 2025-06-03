using System.ComponentModel.DataAnnotations;

namespace PBL5sem.Models
{
    public class EspDeviceViewModel : PadraoViewModel
    {
        public string Nome { get; set; }

        [RegularExpression(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$",
            ErrorMessage = "IP inválido")]
        public string EnderecoIP { get; set; }
    }
}