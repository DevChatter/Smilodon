namespace ActivityPub.Domain.Accounts
{
    public class ProfileField
    {
        public ProfileField(string name, string value, DateTime? verifiedAt)
        {
            Name = name;
            Value = value;
            VerifiedAt = verifiedAt;
        }

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
