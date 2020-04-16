using System.Text;

namespace K9Nano.Logging.Web.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Append(this StringBuilder sb, string text, int fixedLength)
        {
            if (text == null)
            {
                sb.Append(new string(' ', fixedLength));
            }
            else
            {
                var spaces = fixedLength - text.Length;
                if (spaces == 0)
                {
                    sb.Append(text);
                }
                else if (spaces > 0)
                {
                    sb.Append(text + new string(' ', spaces));
                }
                else
                {
                    var lastDot = text.LastIndexOf('.');
                    if (lastDot > 0 && lastDot < text.Length - 1)
                    {
                        Append(sb, text.Substring(lastDot + 1), fixedLength);
                    }
                    else
                    {

                    }
                }
            }

            return sb;
        }
    }
}