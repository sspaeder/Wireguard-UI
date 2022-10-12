using System.Security.Principal;

namespace WireGuard.Core.Classes
{
    /// <summary>
    /// Class for the user access identities
    /// </summary>
    public static class UAC
    {
        /// <summary>
        /// Method to creat an <see cref="IdentityReference"/> from an <see cref="WellKnownSidType"/>
        /// </summary>
        /// <param name="sidType">SID to looking for</param>
        /// <param name="dominSid"><see cref="SecurityIdentifier"/> for the domain</param>
        /// <returns><see cref="IdentityReference"/></returns>
        public static IdentityReference GetUserIdentity(WellKnownSidType sidType, SecurityIdentifier dominSid = null) =>
            new SecurityIdentifier(sidType, dominSid).Translate(typeof(NTAccount)) as NTAccount;

        #region Users

        /// <summary>
        /// Gets the system user identity
        /// </summary>
        public static IdentityReference SystemUser => GetUserIdentity(WellKnownSidType.LocalSystemSid);

        /// <summary>
        /// Gets the local admininistrator
        /// </summary>
        public static IdentityReference LocalAdmin => GetUserIdentity(WellKnownSidType.BuiltinAdministratorsSid);

        /// <summary>
        /// Gets the creator or owner identity
        /// </summary>
        public static IdentityReference CreatorOwner => GetUserIdentity(WellKnownSidType.CreatorOwnerSid);

        #endregion

        #region Groups

        /// <summary>
        /// Gets the group of authenticated users
        /// </summary>
        public static IdentityReference AuthenticatedUserGroup => GetUserIdentity(WellKnownSidType.AuthenticatedUserSid);

        /// <summary>
        /// Gets the group of network services
        /// </summary>
        public static IdentityReference NetworkServiceGroup => GetUserIdentity(WellKnownSidType.NetworkServiceSid);

        /// <summary>
        /// Gets the group of users who are logged in via network
        /// </summary>
        public static IdentityReference NetworkUserGroup => GetUserIdentity(WellKnownSidType.NetworkSid);

        #endregion


    }
}
