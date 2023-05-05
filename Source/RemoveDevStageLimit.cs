using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RemoveDevStageLimit
{
    public sealed class RemoveDevStageLimit : Mod
    {
        private const bool DefaultIgnoreApparel = false;

        Dictionary<ThingDef, ReferenceableElement<bool>> ignoredApparelsCache; // needs separate storage since default dictionaries do not support ref values
        Vector2 scrollPos;

        public RemoveDevStageLimit(ModContentPack content) : base(content)
        {
            ignoredApparelsCache = new Dictionary<ThingDef, ReferenceableElement<bool>>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            // initialize scrolling
            Listing_Standard listingStandard = new Listing_Standard();
            Rect outRect = inRect;
            Rect scrollRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height * 10f + 50);
            Widgets.BeginScrollView(outRect, ref scrollPos, scrollRect);
            // actual stuff
            listingStandard.Begin(scrollRect);
            listingStandard.Label("RemoveDevStageLimit.RestartRequired".Translate());
            listingStandard.CheckboxLabeled(
                "RemoveDevStageLimit.MakeAdultApparelUsable".Translate(),
                ref RemoveDevStageLimitSettings.Instance.MakeAdultApparelUsable,
                "RemoveDevStageLimit.MakeAdultApparelUsable.Tooltip".Translate());
            listingStandard.CheckboxLabeled(
                "RemoveDevStageLimit.MakeChildApparelUsable".Translate(),
                ref RemoveDevStageLimitSettings.Instance.MakeChildApparelUsable,
                "RemoveDevStageLimit.MakeChildApparelUsable.Tooltip".Translate());
            listingStandard.GapLine();
            listingStandard.CheckboxLabeled(
                "RemoveDevStageLimit.Debug".Translate(),
                ref RemoveDevStageLimitSettings.Instance.Debug,
                "RemoveDevStageLimit.Debug.Tooltip".Translate());
            // manual control over what to patch
            listingStandard.GapLine();
            listingStandard.Label("RemoveDevStageLimit.IgnoreFilters".Translate());
            // update apparels before displaying all settings
            foreach (var apparel in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel))
            {
                if (!RemoveDevStageLimitSettings.Instance.IgnoredApparels.TryGetValue(apparel, out bool value))
                    value = DefaultIgnoreApparel;
                if (ignoredApparelsCache.TryGetValue(apparel, out var cacheValue))
                    cacheValue.Value = value;
                else
                {
                    cacheValue = new ReferenceableElement<bool>(value);
                    ignoredApparelsCache[apparel] = cacheValue;
                }
                listingStandard.CheckboxLabeled(apparel.defName.ToString(), ref ignoredApparelsCache[apparel].Value);
            }
            listingStandard.End();
            Widgets.EndScrollView();
            base.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            foreach (var pair in ignoredApparelsCache)
                RemoveDevStageLimitSettings.Instance.IgnoredApparels[pair.Key] = pair.Value.Value;
            base.WriteSettings();
        }

        public override string SettingsCategory() => "RemoveDevStageLimit".Translate();

        private class ReferenceableElement<T>
        {
            T m_value;
            public ref T Value => ref m_value;

            public ReferenceableElement(T value)
            {
                m_value = value;
            }
        }
    }
}
