namespace ScalpingMO.Analysis.Analysis.API.Utils
{
    public static class PlanHelpers
    {
        public static string GetPlanType(int planType)
        {
            string planTypeString;

            switch (planType)
            {
                case 1:
                    planTypeString = "basic";
                    break;

                case 2:
                    planTypeString = "advanced";
                    break;

                case 3:
                    planTypeString = "full";
                    break;

                default:
                    planTypeString = "default";
                    break;
            }

            return planTypeString;
        }
    }
}
