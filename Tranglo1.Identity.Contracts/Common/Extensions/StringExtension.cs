namespace Tranglo1.CustomerIdentity.Domain.Common.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns the original string, or a placeholder if the string is null, empty, or whitespace.
        /// </summary>
        /// <param name="value">The input string to evaluate.</param>
        /// <param name="emptyPlaceholder">The placeholder value to return if the input is null or whitespace.</param>
        /// <returns>The input string or the placeholder value.</returns>
        public static string ToEmptyPlaceholder(this string value, string emptyPlaceholder)
        {
            return string.IsNullOrWhiteSpace(value) ? emptyPlaceholder : value;
        }
    }
}
