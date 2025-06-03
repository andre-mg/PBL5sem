using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PBL5sem.Filters
{
    public class AutorizacaoAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cargoId = context.HttpContext.Session.GetInt32("UsuarioCargoId");
            if (cargoId == null || cargoId != 1) // 1 = Admin
            {
                context.Result = new RedirectToActionResult("AcessoNegado", "Login", null);
            }
        }
    }
}
