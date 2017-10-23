namespace Threax.AspNetCore.AccessTokens
{
    public class AccessTokenModel
    {
        /// <summary>
        /// The access token contents.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The name of the header. Defaults to "bearer".
        /// </summary>
        public string HeaderName { get; set; } = "bearer";
    }
}
