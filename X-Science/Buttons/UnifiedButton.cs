using KSP.Localization;
using System;
using UnityEngine;
using KSP.UI.Screens;
using ToolbarControl_NS;
using ClickThroughFix;
#if false



namespace ScienceChecklist
{
    internal class UnifiedButton 
    {
        // Members
        static internal ToolbarControl toolbarControl = null;

        //private IToolbarButton _button;

        public event EventHandler ButtonOn;
        public event EventHandler ButtonOff;
        public event EventHandler RightClick;

        //public bool UseBlizzyIfPossible;

        public Texture2D LauncherTexture;
        public ApplicationLauncher.AppScenes LauncherVisibility;


        public string BlizzyNamespace;
        public string BlizzyButtonId;
        public string BlizzyTexturePath;
        public GameScenesVisibility BlizzyVisibility;
        public string BlizzyToolTip;
        public string BlizzyText;
        private readonly Logger _logger;


        GameObject gameObject;
       

        // Constructor
        public UnifiedButton(GameObject go, int winId)
        {
            this.gameObject = go;
            this.winId = winId;
            _logger = new Logger(this);
            //			_logger.Info( "Made a button" );
        }

        public void Add()
        {
            //			_logger.Info( "Add" );
            InitializeButton();
        }
        public void Remove()
        {
#if false
            //			_logger.Info( "Remove" );
            if (_button != null)
            {
                _button.Open -= OnButtonOn;
                _button.Close -= OnButtonOff;
                _button.RightClick -= OnRightClick;
                _button.Remove();
                _button = null;
            }
#else
#endif
        }
        /// <summary>
        /// Force button to on state
        /// </summary>
        public void SetOn()
        {
            //_button.SetOn();
            toolbarControl.SetTrue(true);
        }

        /// <summary>
        /// Force button to off state
        /// </summary>
        public void SetOff()
        {
            //_button.SetOff();
            toolbarControl.SetFalse(true);
        }

        /// <summary>
        /// Force button to enabled state
        /// </summary>
        public void SetEnabled()
        {
            //_button.SetEnabled();
            toolbarControl.Enabled = true;
            //			this.Log( "Enabled" );
        }

        /// <summary>
        /// Force button to disabled state
        /// </summary>
        public void SetDisabled()
        {
            //_button.SetDisabled();
            toolbarControl.Enabled = false;
            //			this.Log( "Disabled" );
        }

        public bool IsEnabled()
        {
            //return _button.IsEnabled();
            return toolbarControl.Enabled;
        }

        internal const string MODID = "[x] Science";
        internal const string MODNAME = "[x] Science";

        internal const string WINDOW_CHECKLIST = "WINDOW_CHECKLIST";
        private void InitializeButton()
        {
            //			_logger.Info( "InitializeButton" );
#if false
            if (_button != null)
                Remove();

            if (UseBlizzyIfPossible && BlizzysToolbarButton.IsAvailable)
                _button = new BlizzysToolbarButton
                (
                    BlizzyNamespace, BlizzyButtonId, BlizzyToolTip, BlizzyText, BlizzyTexturePath, BlizzyVisibility
                );
            else
                _button = new AppLauncherButton(LauncherTexture, LauncherVisibility);

            _button.Open += OnButtonOn;
            _button.Close += OnButtonOff;
            _button.RightClick += OnRightClick;
            _button.Add();
#else
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(null, null,
                    ApplicationLauncher.AppScenes.FLIGHT,
                    MODID,
                    Localizer.Format("#LOC_xSci_282"),
                    Localizer.Format("#LOC_xSci_283"),
                    Localizer.Format("#LOC_xSci_283"),
                    MODNAME
                );
                toolbarControl.AddLeftRightClickCallbacks(LeftButtonToggle, RightButton);
            }

#endif
        }

        void LeftButtonToggle()
        {
            if (toolbarControl.Enabled)
                OnButtonOn(null, null);
            else
                OnButtonOff(null, null);
        }
        void RightButton()
        {
            OnRightClick(null, null);
        }

        private void OnButtonOn(object sender, EventArgs e)
        {
            //			_logger.Info( "Button_Open" );
            if (ButtonOn != null)
                ButtonOn(this, e);
        }



        private void OnButtonOff(object sender, EventArgs e)
        {
            //			_logger.Info( "Button_Close" );
            if (ButtonOff != null)
                ButtonOff(this, e);
        }

        private void OnRightClick(object sender, EventArgs e)
        {
            if (RightClick != null)
                RightClick(this, e);
        }

    }
}
#endif
