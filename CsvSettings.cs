using Penguin.Extensions.String;

namespace Penguin.IO
{
    public class CsvOptions : QuotedStringOptions
    {
        public char LineDelimeter { get; set; } = '\n';

        public bool HasHeaders { get; set; } = true;
    }
}