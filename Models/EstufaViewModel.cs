using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PBL5sem.Models
{
    public class EstufaViewModel : PadraoViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public decimal TemperaturaIdeal { get; set; }

        public string AwsIP { get; set; }

        public int EspId { get; set; }

        // Para exibir informações do ESP na view
        public string EspNome { get; set; }
        public string EspEnderecoIP { get; set; }

        // Lista de ESPs disponíveis para seleção
        public List<EspDeviceViewModel> ESPsDisponiveis { get; set; }
    }

    public class MonitoramentoViewModel : PadraoViewModel
    {
        [Display(Name = "Estufa")]
        public int EstufaId { get; set; }

        [Display(Name = "Data/Hora")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime DataHora { get; set; } = DateTime.Now;

        [Display(Name = "Temperatura (°C)")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public decimal Temperatura { get; set; }

        // Propriedade de navegação
        public virtual EstufaViewModel Estufa { get; set; }
    }
}