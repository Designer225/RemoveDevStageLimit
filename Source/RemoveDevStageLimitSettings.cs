using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RemoveDevStageLimit
{
    public sealed class RemoveDevStageLimitSettings : ModSettings
    {
        private const bool DefaultMakeAdultApparelUsable = true;
        private const bool DefaultMakeChildApparelUsable = false;
        private const bool DefaultDebug = false;

        private static RemoveDevStageLimitSettings instance;
        public static RemoveDevStageLimitSettings Instance
        {
            get
            {
                if (instance == default) instance = LoadedModManager.GetMod<RemoveDevStageLimit>().GetSettings<RemoveDevStageLimitSettings>();
                return instance;
            }
        }

        private bool m_makeAdultApparelUsable = DefaultMakeAdultApparelUsable;
        public ref bool MakeAdultApparelUsable => ref m_makeAdultApparelUsable;

        private bool m_makeChildApparelUsable = DefaultMakeChildApparelUsable;
        public ref bool MakeChildApparelUsable => ref m_makeChildApparelUsable;

        private bool m_debug = DefaultDebug;
        public ref bool Debug => ref m_debug;

        private Dictionary<ThingDef, bool> m_ignoredApparels = new Dictionary<ThingDef, bool>();
        public Dictionary<ThingDef, bool> IgnoredApparels => m_ignoredApparels;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref m_makeAdultApparelUsable, "MakeAdultApparelUsable", DefaultMakeAdultApparelUsable);
            Scribe_Values.Look(ref m_makeChildApparelUsable, "MakeChildApparelUsable", DefaultMakeChildApparelUsable);
            Scribe_Values.Look(ref m_debug, "Debug", DefaultDebug);
            Scribe_Collections.Look(ref m_ignoredApparels, "IgnoredApparels", LookMode.Def, LookMode.Value);
            base.ExposeData();
        }
    }
}
