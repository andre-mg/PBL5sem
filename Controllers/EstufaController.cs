using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PBL5sem.DAO;
using PBL5sem.Filters;
using PBL5sem.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PBL5sem.Controllers
{
    public class EstufaController : PadraoController<EstufaViewModel>
    {
        private readonly MonitoramentoDAO _monitoramentoDAO;
        private readonly IHttpClientFactory _httpClientFactory;

        public EstufaController(IHttpClientFactory httpClientFactory)
        {
            DAO = new EstufaDAO();
            _monitoramentoDAO = new MonitoramentoDAO();
            _httpClientFactory = httpClientFactory;
            GeraProximoId = true;
        }


        [AutorizacaoAdmin]
        public override IActionResult Create()
        {
            return base.Create();
        }

        protected override void PreencheDadosParaView(string Operacao, EstufaViewModel model)
        {
            base.PreencheDadosParaView(Operacao, model);

            var espDAO = new EspDeviceDAO();
            model.ESPsDisponiveis = espDAO.Listagem();

            if (model.EspId > 0)
            {
                var esp = espDAO.Consulta(model.EspId);
                if (esp != null)
                {
                    model.EspNome = esp.Nome;
                    model.EspEnderecoIP = esp.EnderecoIP;
                }
            }
        }

        protected override void ValidaDados(EstufaViewModel model, string operacao)
        {
            base.ValidaDados(model, operacao);
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(model.Nome))
                ModelState.AddModelError("Nome", "Preencha o nome da estufa.");

            if (string.IsNullOrWhiteSpace(model.AwsIP))
                ModelState.AddModelError("AwsIP", "Preencha o IP do servidor AWS");

            if (model.EspId == 0)
                ModelState.AddModelError("EspId", "Selecione um dispositivo ESP válido.");
        }

        [HttpGet]
        public IActionResult ObterDadosMonitoramento(int id)
        {
            try
            {
                var estufa = DAO.Consulta(id);
                if (estufa == null) return NotFound();

                // Obtém a última leitura de temperatura
                var ultimaLeitura = _monitoramentoDAO.UltimaLeitura(id);
                var temperaturaAtual = ultimaLeitura?.Temperatura ?? 0;

                // Obtém o histórico (últimas 20 leituras)
                var historico = _monitoramentoDAO.Listagem(id, 20)
                    .OrderBy(h => h.DataHora)
                    .Select(h => new
                    {
                        temperatura = h.Temperatura,
                        horario = h.DataHora.ToString("o") // ISO 8601
                    }).ToList();

                return Json(new
                {
                    temperaturaAtual,
                    historico
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> Monitoramento(int id)
        {
            try
            {
                if (!HelperControllers.VerificaUserLogado(HttpContext.Session))
                    return RedirectToAction("Index", "Login");

                var estufa = DAO.Consulta(id);
                if (estufa == null) return NotFound();

                // Verifica permissão
                var usuarioEmpresaId = HttpContext.Session.GetInt32("UsuarioEmpresaId");
                var isAdmin = HttpContext.Session.GetInt32("UsuarioCargoId") == 1;

                var temperaturaAtual = await ObterTemperaturaAtual(estufa);
                var historico = _monitoramentoDAO.Listagem(id);

                ViewBag.TemperaturaAtual = temperaturaAtual;
                ViewBag.Historico = historico;
                ViewBag.Estufa = estufa;

                return View(estufa);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro no monitoramento: {ex}");
                TempData["ErrorMessage"] = "Erro ao carregar dados de monitoramento";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarDados(int id)
        {
            try
            {
                Debug.WriteLine($"Iniciando AtualizarDados para estufa {id}");
                var estufa = DAO.Consulta(id);
                if (estufa == null)
                {
                    Debug.WriteLine("Estufa não encontrada");
                    return BadRequest(new { success = false, error = "Estufa não encontrada" });
                }

                var (temperatura, dataHoraBroker) = await ObterDadosAtuaisDoBroker(estufa);
                Debug.WriteLine($"Dados obtidos - Temp: {temperatura}, Data: {dataHoraBroker}");

                if (!temperatura.HasValue)
                {
                    Debug.WriteLine("Temperatura não obtida do broker");
                    return StatusCode(StatusCodes.Status503ServiceUnavailable,
                        new { success = false, error = "Não foi possível obter dados do broker" });
                }

                // VERIFICAÇÃO DE DUPLICIDADE COM LOG
                bool existeRegistro = _monitoramentoDAO.ExisteRegistroComDataHora(id, dataHoraBroker);
                Debug.WriteLine($"Verificação de duplicidade: {(existeRegistro ? "REGISTRO EXISTENTE" : "NOVO REGISTRO")}");

                if (!existeRegistro)
                {
                    var monitoramento = new MonitoramentoViewModel
                    {
                        EstufaId = id,
                        DataHora = dataHoraBroker,
                        Temperatura = temperatura.Value
                    };
                    Debug.WriteLine($"Tentando inserir: EstufaId={monitoramento.EstufaId}, Temp={monitoramento.Temperatura}, Data={monitoramento.DataHora}");

                    _monitoramentoDAO.Insert(monitoramento);
                    Debug.WriteLine("Inserção no banco realizada");
                }

                var historico = _monitoramentoDAO.Listagem(id, 20)
                    .OrderBy(h => h.DataHora)
                    .Select(h => new {
                        temperatura = h.Temperatura,
                        horario = DateTime.SpecifyKind(h.DataHora, DateTimeKind.Utc).ToString("o")
                    }).ToList();

                return Ok(new
                {
                    success = true,
                    temperaturaAtual = temperatura.Value,
                    historico,
                    novoDadoInserido = !existeRegistro
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERRO em AtualizarDados: {ex.ToString()}");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetDadosTemperatura(int id)
        {
            try
            {
                var historico = _monitoramentoDAO.Listagem(id, 50)
                    .Select(h => new
                    {
                        temperatura = h.Temperatura,
                        horario = h.DataHora.ToString("o")
                    }).ToList();

                return Ok(new { historico });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = ex.Message });
            }
        }

        private async Task<(decimal? temperatura, DateTime dataHora)> ObterDadosAtuaisDoBroker(EstufaViewModel estufa)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(3);

                string url = $"http://{estufa.AwsIP}:1026/v2/entities/urn:ngsi-ld:Lamp:001";

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("fiware-service", "smart");
                httpClient.DefaultRequestHeaders.Add("fiware-servicepath", "/");

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(content);

                // Modificação aqui para a nova estrutura
                if (jsonResponse.temperature?.value != null && jsonResponse.temperature?.metadata?.TimeInstant?.value != null)
                {
                    return (
                        Convert.ToDecimal(jsonResponse.temperature.value),
                        DateTime.Parse(jsonResponse.temperature.metadata.TimeInstant.value.ToString())
                    );
                }

                return (null, DateTime.MinValue);
            }
            catch
            {
                return (null, DateTime.MinValue);
            }
        }


        private async Task<decimal?> ObterTemperaturaAtual(EstufaViewModel estufa)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                string url = $"http://{estufa.AwsIP}:1026/v2/entities/urn:ngsi-ld:Lamp:001";

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("fiware-service", "smart");
                httpClient.DefaultRequestHeaders.Add("fiware-servicepath", "/");

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(content);

                if (jsonResponse.temperature?.value != null)
                {
                    return Convert.ToDecimal(jsonResponse.temperature.value);
                }

                Debug.WriteLine("Dados de temperatura não encontrados na resposta");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao obter temperatura: {ex.Message}");
                return null;
            }
        }
    }
}