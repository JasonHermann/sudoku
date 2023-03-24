using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Classification
{
    [Flags]
    public enum CellStatus
    {
        Empty          = 1 << 0,
        Digit          = 1 << 1,
        ScratchedDigit = 1 << 2,
        HintDigits     = 1 << 3,
    }



    public class CellClassification
    {
    }
}
