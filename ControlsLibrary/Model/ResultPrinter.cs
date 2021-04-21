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
                        return "NotRunned";
                    }
                case ResultEnum.Failed:
                    {
                        return "Failed";
                    }
                case ResultEnum.Passed:
                    {
                        return "Passed";
                    }
                default:
                    {
                        return "NotDefined";

                    }
            }
        }
    }
}
