namespace ActivityPub.Domain.Accounts
{
    public class ProfileField
    {
        /// <summary>
        /// The key of a given field’s key-value pair.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value associated with the name key. Can have HTML.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Timestamp of when the server verified a URL value for a rel=“me” link.
        /// Null means not verified.
        /// </summary>
        public DateTime? VerifiedAt { get; set; }
    }
}
