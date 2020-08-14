using System.Web.Mvc;

namespace JCarrollOnlineV2
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters == null)
            {
                throw new System.ArgumentNullException(nameof(filters));
            }

            filters.Add(new HandleErrorAttribute());
        }
    }
}