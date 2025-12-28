using System.Security.Claims;

namespace ControleGastos.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Obtém o Azure AD Object ID do usuário autenticado
        /// </summary>
        public static string GetAzureAdId(this ClaimsPrincipal principal)
        {
            // Try to get the OID claim (Object ID from Azure AD)
            var oid = principal.FindFirst("oid")?.Value 
                   ?? principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (string.IsNullOrEmpty(oid))
            {
                throw new UnauthorizedAccessException("Azure AD Object ID not found in token");
            }

            return oid;
        }
    }
}
