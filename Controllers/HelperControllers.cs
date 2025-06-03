namespace PBL5sem.Controllers
{
    public class HelperControllers
    {
        public static Boolean VerificaUserLogado(ISession session)
        {
            string logado = session.GetString("Logado");
            if (logado == null)
                return false;
            else
                return true;
        }

        public static bool VerificaCargo(ISession session, int cargoId)
        {
            var usuarioCargoId = session.GetInt32("UsuarioCargoId");
            return usuarioCargoId.HasValue && usuarioCargoId.Value == cargoId;
        }
    }

}
