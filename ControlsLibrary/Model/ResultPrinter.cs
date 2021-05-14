using ControlsLibrary.Properties.Langs;

namespace ControlsLibrary.Model
{
    /// <summary>
    /// Provides function to convert result into the string
    /// </summary>
    internal static class ResultPrinter
    {
        /// <summary>
        /// Converts result into the string type
        /// </summary>
        /// <param name="result">Result in the ResultEnum type</param>
        /// <returns>String result</returns>
        public static string PrintResult(ResultEnum result)
        {
            switch (result)
            {
                case ResultEnum.NotRunned:
                    {
                        return Lang.resultNotRunned;
                    }
                case ResultEnum.Failed:
                    {
                        return Lang.resultFailed;
                    }
                case ResultEnum.Passed:
                    {
                        return Lang.resultPassed;
                    }
                default:
                    {
                        return Lang.Results_NotDefined;
                    }
            }
        }
    }
}