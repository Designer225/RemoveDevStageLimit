using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RemoveDevStageLimit
{
    public class RemoveDevStageLimit : Mod
    {
        private const bool DefaultIgnoreApparel = false;

        RemoveDevStageLimitSettings settings;
        Dictionary<ThingDef, ReferenceableElement<bool>> ignoredApparelsCache; // needs separate storage since default dictionaries do not support ref values

        public RemoveDevStageLimit(ModContentPack content) : base(content)
        {
            settings = GetSettings<RemoveDevStageLimitSettings>();
            ignoredApparelsCache = new Dictionary<ThingDef, ReferenceableElement<bool>>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("RemoveDevStageLimit.RestartRequired");
            listingStandard.CheckboxLabeled(
                "RemoveDevStageLimit.MakeAdultApparelUsable".Translate(),
                ref settings.MakeAdultApparelUsable,
                "RemoveDevStageLimit.MakeAdultApparelUsable.Tooltip".Translate());
            listingStandard.CheckboxLabeled(
                "RemoveDevStageLimit.MakeChildApparelUsable".Translate(),
                ref settings.MakeChildApparelUsable,
                "RemoveDevStageLimit.MakeChildApparelUsable.Tooltip".Translate());
            listingStandard.GapLine();
            listingStandard.CheckboxLabeled(
                "RemoveDevStageLimit.Debug".Translate(),
                ref settings.Debug,
                "RemoveDevStageLimit.Debug.Tooltip".Translate());
            // manual control over what to patch
            listingStandard.GapLine();
            listingStandard.Label("RemoveDevStageLimit.IgnoreFilters");
            // update apparels before displaying all settings
            foreach (var apparel in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel))
            {
                if (settings.IgnoredApparels.TryGetValue(apparel, out bool value))
                    ignoredApparelsCache[apparel].Value = value;
                else ignoredApparelsCache[apparel].Value = DefaultIgnoreApparel;
                listingStandard.CheckboxLabeled(apparel.defName.ToString(), ref ignoredApparelsCache[apparel].Value);
            }
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            foreach (var pair in ignoredApparelsCache)
                settings.IgnoredApparels[pair.Key] = pair.Value.Value;
            base.WriteSettings();
        }

        public override string SettingsCategory()
        {
            return "RemoveDevStageLimit".Translate();
        }

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
