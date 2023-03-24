using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Classification
{
    public enum BoardStatus
    {
        NoBoard,
        ObstructedBoard,
        FullBoard,
    }

    public enum GridStatus
    {
        UnknownGrid,
        IrregularGrid,
        RegularGrid,
    }

    public enum DigitStatus
    {
        NoDigits,
        HandwrittenDigits,
        TypedDigits,
        MixedDigits,
    }

    public enum SolvedStatus
    {
        UnknownSolvability,
        TooManySolutionsExist,
        NoSolutionsExist,
        Solveable,
        Solved,
    }

    public enum ErrorStatus
    {
        NoErrors,
        /// <summary>
        /// Token is matched to a token, but that token is not in the alphabet of the game.
        /// </summary>
        IllegalToken,
        /// <summary>
        /// Token does not match any known token.
        /// </summary>
        UnrecognizableToken,
        /// <summary>
        /// Token is valid, but it violates a rule of the game
        /// </summary>
        ContainsInconsistentToken,
    }

    public enum AdditionalTokens
    {
        None,
        ScratchedTokens,
        SuperscriptHintTokens,
    }

    public class BoardClassification
    {

    }
}
