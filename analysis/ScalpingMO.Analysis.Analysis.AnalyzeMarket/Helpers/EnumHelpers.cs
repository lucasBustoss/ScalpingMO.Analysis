using ScalpingMO.Analysis.Analysis.AnalyzeMarket.Enums;

namespace ScalpingMO.Analysis.Analysis.AnalyzeMarket.Helpers
{
    public static class EnumHelpers
    {
        #region MarketTendency

        public static string ConvertMarketTendencyToString(MarketTendencyEnum marketTendency)
        {
            string tendency = "";

            switch (marketTendency)
            {
                case MarketTendencyEnum.None:
                    tendency = "Sem tendência";
                    break;

                case MarketTendencyEnum.Upward:
                    tendency = "Subida";
                    break;

                case MarketTendencyEnum.Lateralized:
                    tendency = "Lateralized";
                    break;

                case MarketTendencyEnum.Downward:
                    tendency = "Descida";
                    break;

                default:
                    tendency = "";
                    break;
            }

            return tendency;
        }

        public static MarketTendencyEnum ConvertStringToMarketTendency(string marketTendency)
        {
            MarketTendencyEnum tendency = MarketTendencyEnum.None;

            switch (marketTendency)
            {
                case "Sem tendência":
                    tendency = MarketTendencyEnum.None;
                    break;

                case "Subida":
                    tendency = MarketTendencyEnum.Upward;
                    break;

                case "Lateralized":
                    tendency = MarketTendencyEnum.Lateralized;
                    break;

                case "Descida":
                    tendency = MarketTendencyEnum.Downward;
                    break;

                default:
                    tendency = MarketTendencyEnum.None;
                    break;
            }

            return tendency;
        }

        public static int ConvertMarketTendencyToInt(MarketTendencyEnum marketTendency)
        {
            int tendency = -1;

            switch (marketTendency)
            {
                case MarketTendencyEnum.None:
                    tendency = 0;
                    break;

                case MarketTendencyEnum.Upward:
                    tendency = 1;
                    break;

                case MarketTendencyEnum.Lateralized:
                    tendency = 2;
                    break;

                case MarketTendencyEnum.Downward:
                    tendency = 3;
                    break;

                default:
                    tendency = -1;
                    break;
            }

            return tendency;
        }

        public static MarketTendencyEnum ConvertIntToMarketTendency(int marketTendency)
        {
            MarketTendencyEnum tendency = MarketTendencyEnum.None;

            switch (marketTendency)
            {
                case 0:
                    tendency = MarketTendencyEnum.None;
                    break;

                case 1:
                    tendency = MarketTendencyEnum.Upward;
                    break;

                case 2:
                    tendency = MarketTendencyEnum.Lateralized;
                    break;

                case 3:
                    tendency = MarketTendencyEnum.Downward;
                    break;

                default:
                    tendency = MarketTendencyEnum.None;
                    break;
            }

            return tendency;
        }

        #endregion
    }
}
