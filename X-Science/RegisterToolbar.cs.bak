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
            Log = new Log("XScience", Log.LEVEL.INFO);
#else
            Log = new Log("XScience", Log.LEVEL.Error);
#endif
#endif
        }
    }
}
