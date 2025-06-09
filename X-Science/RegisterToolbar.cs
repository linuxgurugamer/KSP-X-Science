using KSP.Localization;
using ToolbarControl_NS;
using UnityEngine;

#if false
using KSP_Log;
#endif
namespace ScienceChecklist
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
#if false
        static public Log Log;
#endif
        void Start()
        {
            ToolbarControl.RegisterMod(ScienceChecklistAddon.MODID, ScienceChecklistAddon.MODNAME);
            ToolbarControl.RegisterMod(ScienceChecklistAddon.MODID+"2", ScienceChecklistAddon.WINDOW_CHECKLIST);

#if false
#if DEBUG
            Log = new Log(Localizer.Format("#LOC_xSci_75"), Log.LEVEL.INFO);
#else
            Log = new Log(Localizer.Format("#LOC_xSci_75"), Log.LEVEL.Error);
#endif
#endif
        }
    }
}
