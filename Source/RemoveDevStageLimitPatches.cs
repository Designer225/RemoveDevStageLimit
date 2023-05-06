using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RemoveDevStageLimit
{
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel))]
    static class ApparelGraphicRecordGetter_TryGetGraphicApparelPatch
    {
        static void Prepare() { Log.Message("[RemoveDevStageLimit] Patching ApparelGraphicRecordGetter.TryGetGraphicApparel() with a non-destructive prefix"); }

        static void Prefix(Apparel apparel, ref BodyTypeDef bodyType)
        {
            if (RemoveDevStageLimit.Instance.OriginalDevStates.TryGetValue(apparel.def, out var value))
            {
                if (RemoveDevStageLimit.Instance.MakeAdultApparelUsable
                    && (value & DevelopmentalStage.Adult) == DevelopmentalStage.Adult && bodyType == BodyTypeDefOf.Child)
                    bodyType = BodyTypeDefOf.Thin;
                else if (RemoveDevStageLimit.Instance.MakeChildApparelUsable
                    && (value & DevelopmentalStage.Child) == DevelopmentalStage.Child)
                    bodyType = BodyTypeDefOf.Child;
            }
        }
    }
}
