using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RemoveDevStageLimit
{
    [StaticConstructorOnStartup]
    public static class RemoveDevStageLimitPatcher
    {
        static RemoveDevStageLimitPatcher()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel))
            {
                if (RemoveDevStageLimitSettings.Instance.IgnoredApparels.TryGetValue(def, out bool value) && value) continue;
                var apparel = def.apparel;
                var filterToAppend = DevelopmentalStage.None;
                if ((apparel.developmentalStageFilter & DevelopmentalStage.Adult) == DevelopmentalStage.Adult
                    && RemoveDevStageLimitSettings.Instance.MakeAdultApparelUsable)
                    filterToAppend |= DevelopmentalStage.Child;
                if ((apparel.developmentalStageFilter & DevelopmentalStage.Child) == DevelopmentalStage.Child
                    && RemoveDevStageLimitSettings.Instance.MakeChildApparelUsable)
                    filterToAppend |= DevelopmentalStage.Adult;
                apparel.developmentalStageFilter |= filterToAppend;
                if (RemoveDevStageLimitSettings.Instance.Debug)
                    Log.Message("[RemoveDevStageLimit] Patched " + def.defName.ToString());
            }
        }
    }
}
