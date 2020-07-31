using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using System.Linq;


namespace NewBISReports.Models.Autorizacao
{
    //public class EmptyUSerStore<ApplicationUser> : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser> where ApplicationUser : class,new()
    public class EmptyUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserClaimStore<ApplicationUser> 
    {
        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }
        /// <summary>
        /// Initializes an instance.
        /// </summary>

        public EmptyUserStore(   IdentityErrorDescriber describer
            )
        {
             ErrorDescriber = describer ?? throw new ArgumentNullException(nameof(describer));

        }


         Task<ApplicationUser> IUserStore<ApplicationUser>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            
        }

        Task<ApplicationUser>  IUserStore<ApplicationUser>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {

           throw new NotImplementedException();

        }

        //1o ponto de entrada, pode-se já verificar no Ldap aqui, ou então não fazer hash algum e 
        //verificar no ldap apenas em VerifyPasswordHash
        Task<string> IUserPasswordStore<ApplicationUser>.GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //metodo publico para retornar um objeto do tipo LdapIdentity user
        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName)
        {
            throw new NotImplementedException();
        }

        //outro método publico TODO - conciliar esses acessos
        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        Task<string> IUserStore<ApplicationUser>.GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IUserStore<ApplicationUser>.GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //Métodos para recuperar os claims a partir do AD

        Task<IList<Claim>> IUserClaimStore<ApplicationUser>.GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
           throw new NotImplementedException();
        }


        //Método que converte a string do ObjectGuid para um array de hexadecimais impressos
        public string GetBinaryStringFromLdapObjectGuid(string guidstring)
        {
            var bytes = Convert.FromBase64String(guidstring);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(string.Format(@"\{0}", b.ToString("X2")));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Métodos não implementados
        /// </summary>
        #region

        Task<string> IUserStore<ApplicationUser>.GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IdentityResult> IUserStore<ApplicationUser>.CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IdentityResult> IUserStore<ApplicationUser>.DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<bool> IUserPasswordStore<ApplicationUser>.HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IUserStore<ApplicationUser>.SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IUserPasswordStore<ApplicationUser>.SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IUserStore<ApplicationUser>.SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IdentityResult> IUserStore<ApplicationUser>.UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        Task IUserClaimStore<ApplicationUser>.ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IUserClaimStore<ApplicationUser>.RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IList<ApplicationUser>> IUserClaimStore<ApplicationUser>.GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IUserClaimStore<ApplicationUser>.AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EmptyUSerStore() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }



        #endregion
    }
}

