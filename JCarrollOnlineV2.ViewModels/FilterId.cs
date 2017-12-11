using Omu.ValueInjecter.Injections;

namespace JCarrollOnlineV2.ViewModels
{
    public class FilterId : LoopInjection
    {
        //sourcePropName "Id" will not map to target property name "Id"
        //ie: Keep target property value as it is (not change from mapping)
        protected override string GetTargetProp(string sourceName)
        {
            return "Id";
        }
    }
}
