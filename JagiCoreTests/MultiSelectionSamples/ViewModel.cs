using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCoreTests.MultiSelectionSamples
{
    public class CorrectViewModel
    {
        public bool Selection1 { get; set; }
        public bool Selection2 { get; set; }
        public bool SelectionA { get; set; }
        public bool SelectionB { get; set; }
    }

    public class WrongViewModel
    {
        public int Selection1 { get; set; }
        public int Selection2 { get; set; }
        public int SelectionA { get; set; }
        public int SelectionB { get; set; }
    }
}
