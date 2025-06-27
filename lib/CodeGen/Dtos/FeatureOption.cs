using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Dtos
{
    internal class FeatureOptions
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
        public List<string> Prompts { get; set; } = new();
    }
}
