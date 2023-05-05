using HugsLib;
using HugsLib.Settings;
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
    public sealed class RemoveDevStageLimit : ModBase
    {
        private const bool DefaultMakeAdultApparelUsable = true;
        private const bool DefaultMakeChildApparelUsable = false;
        private const bool DefaultDebug = false;
        private const bool DefaultIgnoreApparel = false;
        private const DevelopmentalStage Both = DevelopmentalStage.Adult | DevelopmentalStage.Child;

        private static RemoveDevStageLimit instance;
        public static RemoveDevStageLimit Instance => instance;

        private SettingHandle<bool> m_makeAdultApparelUsable;
        public bool MakeAdultApparelUsable => m_makeAdultApparelUsable;

        private SettingHandle<bool> m_makeChildApparelUsable;
        public bool MakeChildApparelUsable => m_makeChildApparelUsable;

        private SettingHandle<bool> m_debug;
        public bool Debug => m_debug;

        private Dictionary<ThingDef, SettingHandle<bool>> m_ignoredApparels = new Dictionary<ThingDef, SettingHandle<bool>>();
        public IReadOnlyDictionary<ThingDef, bool> IgnoredApparels { get; private set; }

        private Dictionary<ThingDef, DevelopmentalStage> m_originalDevStates = new Dictionary<ThingDef, DevelopmentalStage>();
        public IReadOnlyDictionary<ThingDef, DevelopmentalStage> OriginalDevStates => m_originalDevStates;

        public RemoveDevStageLimit()
        {
            instance = this;
        }

        public override void DefsLoaded()
        {
            base.DefsLoaded();
            Settings.GetHandle<SettingsFiller>("RestartRequired", "", "").CustomDrawerFullWidth = rect =>
            {
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(rect);
                listingStandard.Label("RemoveDevStageLimit.RestartRequired".Translate());
                listingStandard.End();
                return false;
            };
            m_makeAdultApparelUsable = Settings.GetHandle("MakeAdultApparelUsable", "RemoveDevStageLimit.MakeAdultApparelUsable".Translate(),
                "RemoveDevStageLimit.MakeAdultApparelUsable.Tooltip".Translate(), DefaultMakeAdultApparelUsable);
            m_makeChildApparelUsable = Settings.GetHandle("MakeChildApparelUsable", "RemoveDevStageLimit.MakeChildApparelUsable".Translate(),
                "RemoveDevStageLimit.MakeChildApparelUsable.Tooltip".Translate(), DefaultMakeChildApparelUsable);
            Settings.GetHandle<SettingsFiller>("GapLine", "", "").CustomDrawerFullWidth = rect =>
            {
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(rect);
                listingStandard.GapLine();
                listingStandard.End();
                return false;
            };
            m_debug = Settings.GetHandle("Debug", "RemoveDevStageLimit.Debug".Translate(),
                "RemoveDevStageLimit.Debug.Tooltip".Translate(), DefaultDebug);

            Settings.GetHandle<SettingsFiller>("IgnoreFilters", "", "").CustomDrawerFullWidth = rect =>
            {
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(rect);
                listingStandard.GapLine();
                listingStandard.Label("RemoveDevStageLimit.IgnoreFilters".Translate());
                listingStandard.End();
                return false;
            };
            // patch everything
            if (MakeAdultApparelUsable || MakeChildApparelUsable)
            {
                foreach (var def in DefDatabase<ThingDef>.AllDefs.Where(x => x.IsApparel))
                {
                    m_ignoredApparels[def] = Settings.GetHandle(def.defName, def.label, def.description, DefaultIgnoreApparel);
                    if (m_ignoredApparels[def].Value) continue;

                    var apparel = def.apparel;
                    if ((apparel.developmentalStageFilter & Both) == Both) continue;

                    if (MakeAdultApparelUsable && (apparel.developmentalStageFilter & DevelopmentalStage.Adult) == DevelopmentalStage.Adult)
                    {
                        m_originalDevStates[def] = apparel.developmentalStageFilter;
                        apparel.developmentalStageFilter |= DevelopmentalStage.Child;
                    }
                    else if (MakeChildApparelUsable && (apparel.developmentalStageFilter & DevelopmentalStage.Child) == DevelopmentalStage.Child)
                    {
                        m_originalDevStates[def] = apparel.developmentalStageFilter;
                        apparel.developmentalStageFilter |= DevelopmentalStage.Adult;
                    }
                    else continue;

                    if (Debug)
                        Log.Message("[RemoveDevStageLimit] Patched " + def.defName.ToString());
                }
            }
            IgnoredApparels = m_ignoredApparels.ToDictionary(kv => kv.Key, kv => kv.Value.Value);
        }

        private sealed class SettingsFiller : SettingHandleConvertible
        {
            public override bool ShouldBeSaved => false;

            public override void FromString(string settingValue) { }

            public override string ToString() => string.Empty;
        }
    }
}
