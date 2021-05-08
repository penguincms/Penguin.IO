using Penguin.Extensions.String;
using System;
using System.Collections.Generic;
using System.Text;

namespace Penguin.IO
{
    public class CsvOptions : QuotedStringOptions
    {
        public char LineDelimeter { get; set; } = '\n';
        public bool HasHeaders { get; set; } = true;
    }
}
