namespace AspNet.Identity.RavenDB
{
    internal static class Constants
    {
        internal const string RavenUserKeyTemplate = "RavenUsers/{0}";
        internal const string RavenUserLoginKeyTemplate = "RavenUserLogins/{0}/{1}";
        internal const string RavenUserEmailKeyTemplate = "RavenUserEmails/{0}";
        internal const string RavenUserPhoneNumberKeyTemplate = "RavenUserPhoneNumbers/{0}";
    }
}