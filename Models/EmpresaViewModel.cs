using Microsoft.AspNetCore.Http; // Para IFormFile
using PBL5sem.Controllers;
using PBL5sem.Models;

public class EmpresaViewModel : PadraoViewModel, IFileUpload
{
    public string Nome { get; set; }
    public byte[] Logo { get; set; }

    public void ProcessFile(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                Logo = ms.ToArray();
            }
        }
    }
}