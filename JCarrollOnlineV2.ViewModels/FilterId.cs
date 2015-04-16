using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;

namespace JCarrollOnlineV2.ViewModels
{
    public class FilterId : LoopValueInjection
    {
        //sourcePropName "Id" will not map to target property name "Id"
        //ie: Keep target property value as it is (not change from mapping)
        protected override bool UseSourceProp(string sourcePropName)
        {
            return sourcePropName != "Id";
        }
    }
}
