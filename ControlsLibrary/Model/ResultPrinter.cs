using ControlsLibrary.Properties.Langs;

namespace ControlsLibrary.Model
{
    internal static class ResultPrinter
    {
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
                        return "Result not defined";
                    }
            }
        }
    }
}
