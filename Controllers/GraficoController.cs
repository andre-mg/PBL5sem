using Microsoft.AspNetCore.Mvc;
using ScottPlot;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PBL5sem.Controllers
{
    public class GraficoController : Controller
    {
        public IActionResult Index()
        {
            var graficoPath = GerarGraficoComScottPlot();
            ViewData["GraficoPath"] = graficoPath;
            return View();
        }

        private string GerarGraficoComScottPlot()
        {
            // Caminho para salvar a imagem
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }

            var fileName = $"grafico_temperatura_{DateTime.Now:yyyyMMddHHmmss}.png";
            var filePath = Path.Combine(wwwrootPath, fileName);

            // Cria o plot
            var plt = new ScottPlot.Plot(800, 600);

            // Dados
            double[] temperaturas = { 30, 35, 40, 45, 50, 55 };
            double[][] voltagensRepeticoes = {
                new[] { 2.98, 2.98, 2.99, 2.93, 2.99 },
                new[] { 3.49, 3.44, 3.41, 3.46, 3.49 },
                new[] { 3.93, 3.93, 3.91, 3.91, 3.94 },
                new[] { 4.48, 4.39, 4.46, 4.47, 4.42 },
                new[] { 4.96, 4.89, 4.90, 4.88, 4.91 },
                new[] { 5.46, 5.41, 5.43, 5.40, 5.41 }
            };

            // Cores para as repetições
            Color[] cores = {
                Color.Orange,
                Color.Green,
                Color.Blue,
                Color.DarkGreen,
                Color.Purple
            };

            // Adiciona pontos para cada repetição
            for (int rep = 0; rep < 5; rep++)
            {
                double[] voltagens = voltagensRepeticoes.Select(v => v[rep]).ToArray();
                plt.AddScatterPoints(
                    xs: temperaturas,
                    ys: voltagens,
                    color: cores[rep],
                    markerSize: 8,
                    label: $"Repetição {rep + 1}");
            }

            // Calcula médias e desvios padrão
            double[] voltagensMedias = voltagensRepeticoes.Select(v => v.Average()).ToArray();
            double[] desviosPadrao = voltagensRepeticoes.Select(v => CalculateStdDev(v)).ToArray();
            double[] erroTemperatura = Enumerable.Repeat(0.5, temperaturas.Length).ToArray();

            // Adiciona barras de erro manualmente
            for (int i = 0; i < temperaturas.Length; i++)
            {
                // Barras de erro horizontais (X)
                plt.AddLine(
                    x1: temperaturas[i] - erroTemperatura[i],
                    x2: temperaturas[i] + erroTemperatura[i],
                    y1: voltagensMedias[i],
                    y2: voltagensMedias[i],
                    color: Color.Black,
                    lineWidth: 2);

                // Barras de erro verticais (Y)
                plt.AddLine(
                    x1: temperaturas[i],
                    x2: temperaturas[i],
                    y1: voltagensMedias[i] - desviosPadrao[i],
                    y2: voltagensMedias[i] + desviosPadrao[i],
                    color: Color.Black,
                    lineWidth: 2);

                // Caps horizontais
                plt.AddLine(
                    x1: temperaturas[i] - erroTemperatura[i],
                    x2: temperaturas[i] - erroTemperatura[i],
                    y1: voltagensMedias[i] - 0.05,
                    y2: voltagensMedias[i] + 0.05,
                    color: Color.Black,
                    lineWidth: 2);

                plt.AddLine(
                    x1: temperaturas[i] + erroTemperatura[i],
                    x2: temperaturas[i] + erroTemperatura[i],
                    y1: voltagensMedias[i] - 0.05,
                    y2: voltagensMedias[i] + 0.05,
                    color: Color.Black,
                    lineWidth: 2);

                // Caps verticais
                plt.AddLine(
                    x1: temperaturas[i] - 0.5,
                    x2: temperaturas[i] + 0.5,
                    y1: voltagensMedias[i] - desviosPadrao[i],
                    y2: voltagensMedias[i] - desviosPadrao[i],
                    color: Color.Black,
                    lineWidth: 2);

                plt.AddLine(
                    x1: temperaturas[i] - 0.5,
                    x2: temperaturas[i] + 0.5,
                    y1: voltagensMedias[i] + desviosPadrao[i],
                    y2: voltagensMedias[i] + desviosPadrao[i],
                    color: Color.Black,
                    lineWidth: 2);
            }

            // Linha de ajuste
            double[] xFit = Enumerable.Range(28, 30).Select(x => (double)x).ToArray();
            double[] yFit = xFit.Select(x => 0.098085621 * x - 0.012824).ToArray();
            //1.002587337 9923.576575
            plt.AddScatter(
                xs: xFit,
                ys: yFit,
                color: Color.Red,
                lineWidth: 2,
                label: "Linha de ajuste",
                markerSize: 0);


            // Adiciona pontos para as médias com barras de erro usando AddScatter
            var mediaPlot = plt.AddScatter(
                xs: temperaturas,
                ys: voltagensMedias,
                color: Color.Black,
                markerSize: 8,
                lineStyle: LineStyle.None,
                label: "Média ± erro (X e Y)");

            //  Linha tracejada conectando as médias
            plt.AddScatter(
                xs: temperaturas,
                ys: voltagensMedias,
                color: Color.Black,
                lineWidth: 1,
                lineStyle: LineStyle.Dash,
                markerSize: 0,
                label: null);


            // Configurações do gráfico
            plt.SetAxisLimits(xMin: 25, xMax: 60, yMin: 2.5, yMax: 6);
            plt.Title("Tensão x Temperatura para aquecimento da máquina (V)");
            plt.XLabel("Temperatura (°C)");
            plt.YLabel("Tensão necessária (V)");
            plt.Legend();
            plt.Grid(enable: true);

            // Salva o gráfico
            plt.SaveFig(filePath);

            return $"/images/{fileName}";
        }

        private double CalculateStdDev(double[] values)
        {
            double avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }
    }
}