using KSP.Localization;
using System;
using UnityEngine;
using KSP.UI.Screens;
using ToolbarControl_NS;



namespace ScienceChecklist
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class ScienceChecklistAddon : MonoBehaviour
    {
        #region FIELDS
        public const string WINDOW_NAME_CHECKLIST = "ScienceChecklist";
        public const string WINDOW_NAME_STATUS = "StatusWindow";
        public const string WINDOW_NAME_SHIP_STATE = "ShipStateWindow";



        public xScienceEventHandler ScienceEventHandler;
        public ScienceContext Science { get; private set; }
        public DMagicFactory DMagic { get; private set; }
        public static Config Config { get; private set; }

        private bool _active;           // Are we actually running?
        private bool _launcherVisible;  // If the toolbar is shown
        private bool _UiHidden;         // If the user hit F2 
        private static bool _addonInitialized;  // Bug fix multiple inits, only init once


        private Logger _logger;
        private Noise _alertNoise; // Needs to be here because of MonoBehaviour's "gameObject"
        private ToolbarControl checklistToolbarControl;
        private ToolbarControl statusToolbarControl;

        private ScienceWindow _checklistWindow;
        private StatusWindow _statusWindow;
        private SettingsWindow _settingsWindow;
        private HelpWindow _helpWindow;
        private ShipStateWindow _shipStateWindow;
        #endregion



        #region METHODS For Unity
        // Called by Unity once to initialize the class.
        public void Awake()
        {
            _logger = new Logger(this);
            // _logger.Trace("Awake");
        }



        // Called by Unity once to initialize the class, just before Update is called.
        public void Start()
        {
            // _logger.Trace("Start");

            if (_addonInitialized == true)
            {
                // For some reason the addon can be instantiated several times by the KSP addon loader (generally when going to/from the VAB),
                // even though we set onlyOnce to true in the KSPAddon attribute.
                HammerMusicMute(); // Ensure we enforce music volume anyway
                return;
            }
            _addonInitialized = true;
            _active = false;



            // Config
            Config = new Config();
            Config.Load();



            // Music Muting
            if (Config.MusicStartsMuted)
            {
                Muted = true;
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_xSci_76"));
            }

            GameEvents.onGameSceneSwitchRequested.Add(this.onGameSceneSwitchRequested);
            GameEvents.onLevelWasLoaded.Add(this.onLevelWasLoaded);



            //			_logger.Trace( "Making DMagic Factory" );
            DMagic = new DMagicFactory();
            //			_logger.Trace( "Made DMagic Factory" );



            //			_logger.Trace( "Making ScienceContext" );
            Science = new ScienceContext(this);
            //			_logger.Trace( "Made ScienceContext" );



            // Start event handlers
            ScienceEventHandler = new xScienceEventHandler(this);



            // Settings window
            _settingsWindow = new SettingsWindow(this);
           //Config.UseBlizzysToolbarChanged += Settings_UseBlizzysToolbarChanged;
            Config.RighClickMutesMusicChanged += Settings_RighClickMutesMusicChanged;



            // Help window
            _helpWindow = new HelpWindow(this);



            // Status window
            _alertNoise = gameObject.AddComponent<Noise>();
            _statusWindow = new StatusWindow(this);
            _statusWindow.NoiseEvent += OnPlayNoise;
            _statusWindow.WindowClosed += OnStatusWindowClosed;
            _statusWindow.OnCloseEvent += OnStatusWindowClosed;
            _statusWindow.OnOpenEvent += OnStatusWindowOpened;



            // Checklist window
            _checklistWindow = new ScienceWindow(this, _settingsWindow, _helpWindow);
            _checklistWindow.OnCloseEvent += OnChecklistWindowClosed;
            _checklistWindow.OnOpenEvent += OnChecklistWindowOpened;



            // ShipState Window
            _shipStateWindow = new ShipStateWindow(this);



            // Save and load checklist window config when the game scene is changed
            // We are only visible in some scenes
            GameEvents.onGameSceneSwitchRequested.Add(new EventData<GameEvents.FromToAction<GameScenes, GameScenes>>.OnEvent(this.OnGameSceneSwitch));



            // Callbacks for buttons - we init when the "Launcher" toolbar is ready
            GameEvents.onGUIApplicationLauncherReady.Add(Load);
            GameEvents.onGUIApplicationLauncherDestroyed.Add(Unload);

            // Callbacks for F2
            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onShowUI.Add(OnShowUI);


            DontDestroyOnLoad(this);


            // _logger.Trace("Done Start");
        }



        // Called by Unity when the application is destroyed.
        public void OnApplicationQuit()
        {
            //			_logger.Trace( "OnApplicationQuit" );
            RemoveButtons();
        }



        // Called by Unity when this instance is destroyed.
        public void OnDestroy()
        {
            //			_logger.Trace( "OnDestroy" );

            // Music Mute - Unhook from the scene switch events
            GameEvents.onGameSceneSwitchRequested.Remove(this.onGameSceneSwitchRequested);

            GameEvents.onLevelWasLoaded.Remove(this.onLevelWasLoaded);
            RemoveButtons();
        }



        // Called by Unity once per frame.
        public void Update()
        {
            if (ResearchAndDevelopment.Instance == null)
                return;

            if (PartLoader.Instance == null)
                return;

            if (UiActive() && (_checklistWindow.IsVisible || _statusWindow.IsVisible()))
            {
                ScienceEventHandler.Update();
            }
        }



        // Called by Unity to draw the GUI - can be called many times per frame.
        public void OnGUI()
        {
            if (UiActive())
            {
                if (_checklistWindow.IsVisible)
                {
                    _checklistWindow.Draw();
                    _settingsWindow.DrawWindow();
                    _helpWindow.DrawWindow();
                }
                if (_statusWindow.IsVisible())
                {
                    if (HighLogic.LoadedScene == GameScenes.FLIGHT && FlightGlobals.ActiveVessel != null)
                        _statusWindow.DrawWindow();
                }
                if (_shipStateWindow.IsVisible() && Config.SelectedObjectWindow)
                {
                    if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
                        _shipStateWindow.DrawWindow();
                }
            }
        }

        #endregion



        #region METHODS Unity Event Callbacks
        // Save and load checklist window config when the game scene is changed
        private void OnGameSceneSwitch(GameEvents.FromToAction<GameScenes, GameScenes> Data)
        {
            //			_logger.Trace( "OnGameSceneSwitch FROM " + Data.from.ToString( ) );
            SwitchingGameScene(Data.from, Data.to);
        }


        private void SwitchingGameScene(GameScenes From, GameScenes To)
        {
            HammerMusicMute(); // Ensure we enforce music volume anyway


            // Checklist window settings
            if (GameHelper.AllowChecklistWindow(From))
            {
                WindowSettings W = _checklistWindow.BuildSettings();
                W._scene = From;
                Config.SetWindowConfig(W);
            }

            if (GameHelper.AllowChecklistWindow(To))
            {
                WindowSettings W = Config.GetWindowConfig(WINDOW_NAME_CHECKLIST, To);
                _checklistWindow.ApplySettings(W);
            }



            // Status window settings
            if (GameHelper.AllowStatusWindow(From))
            {
                WindowSettings W = _statusWindow.BuildSettings();
                W._scene = From;
                Config.SetWindowConfig(W);
            }

            if (GameHelper.AllowStatusWindow(To))
            {
                WindowSettings W = Config.GetWindowConfig(WINDOW_NAME_STATUS, To);
                _statusWindow.ApplySettings(W);
            }



            // Selected Object window settings
            if (GameScenes.TRACKSTATION == From)
            {
                WindowSettings W = _shipStateWindow.BuildSettings();
                W._scene = From;
                Config.SetWindowConfig(W);
            }

            if (GameScenes.TRACKSTATION == To)
            {
                WindowSettings W = Config.GetWindowConfig(WINDOW_NAME_SHIP_STATE, To);
                _shipStateWindow.ApplySettings(W);
            }
        }







        // Initializes the addon if it hasn't already been loaded.
        // Callback from onGUIApplicationLauncherReady
        private void Load()
        {
            HammerMusicMute(); // Ensure we enforce music volume anyway

            // _logger.Trace("Load");
            if (!GameHelper.AllowChecklistWindow())
            {
                //				_logger.Info( "Ui is hidden in this scene" );
                _active = false;
                RemoveButtons();
                return;
            }

            if (_active)
            {
                // _logger.Trace("Already loaded.");
                return;
            }

            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER && HighLogic.CurrentGame.Mode != Game.Modes.SCIENCE_SANDBOX)
            {
                _logger.Info(Localizer.Format("#LOC_xSci_77") + HighLogic.CurrentGame.Mode + Localizer.Format("#LOC_xSci_78"));
                _active = false;
                return;
            }
            _logger.Info(Localizer.Format("#LOC_xSci_77") + HighLogic.CurrentGame.Mode + Localizer.Format("#LOC_xSci_79"));
            _active = true;
            SwitchingGameScene(GameScenes.MAINMENU, HighLogic.LoadedScene); // Get correct visibility now we are active

            //			_logger.Trace( "Adding Buttons" );
            InitButtons();
            //			_logger.Trace( "Buttons Added" );
            _launcherVisible = true;
            ApplicationLauncher.Instance.AddOnShowCallback(Launcher_Show);
            ApplicationLauncher.Instance.AddOnHideCallback(Launcher_Hide);
        }



        // Unloads the addon if it has been loaded.
        // Callback from onGUIApplicationLauncherDestroyed
        private void Unload()
        {
            if (!_active)
            {
                //				_logger.Trace( "Already unloaded." );
                return;
            }
            _active = false;

            //			_logger.Trace( "Removing Buttons" );
            RemoveButtons();
            //			_logger.Trace( "Removing Callbacks" );
            ApplicationLauncher.Instance.RemoveOnShowCallback(Launcher_Show);
            ApplicationLauncher.Instance.RemoveOnHideCallback(Launcher_Hide);
            _launcherVisible = false;

            //			_logger.Trace( "Unload Done" );
        }



        // F2 support
        void OnHideUI()
        {
            _UiHidden = true;
        }
        void OnShowUI()
        {
            _UiHidden = false;
        }



        // Called when the KSP toolbar is shown.
        private void Launcher_Show()
        {
            if (!_active)
                return;

            //			_logger.Trace("Launcher_Show");
            _launcherVisible = true;
        }



        // Called when the KSP toolbar is hidden.
        private void Launcher_Hide()
        {
            if (!_active)
                return;
            //			_logger.Trace( "Launcher_Hide" );
            _launcherVisible = false;
        }
        #endregion



        #region METHODS Checklist window callbacks
        // Registered with the button
        // Called when the toolbar button for the checklist window is toggled on.
        private void ChecklistButton_Open(object sender, EventArgs e)
        {
            if (!_active)
                return;
            //			_logger.Trace( "ChecklistButton_Open" );
            UpdateChecklistVisibility(true);
        }



        // Registered with the button
        // Called when the toolbar button for the checklist window is toggled off.
        private void ChecklistButton_Close(object sender, EventArgs e)
        {
            if (!_active)
                return;
            //			_logger.Trace( "ChecklistButton_Close" );
            UpdateChecklistVisibility(false);
            //_active = false;
        }



        private void ChecklistButton_RightClick(object sender, EventArgs e)
        {
            if (Config.RighClickMutesMusic)
            {
                // Toggle the muted state
                Muted = !Muted;
                ScreenMessages.PostScreenMessage(Localizer.Format("#LOC_xSci_76"));
            }
            else
            {
                if (_active && UiActive())
                {
                    if (GameHelper.AllowStatusWindow())
                    {
                        bool NewVisibility = !_statusWindow.IsVisible();
                        _statusWindow.SetVisible(NewVisibility);
                        UpdateStatusVisibility(NewVisibility);

                        _logger.Trace(Localizer.Format("#LOC_xSci_80") + _statusWindow.IsVisible());
                        if (_statusWindow.IsVisible())
                        {
                            if (statusToolbarControl != null)
                                statusToolbarControl.SetTrue(true);
                            ScienceEventHandler.ScheduleExperimentUpdate(seconds: 0.1f);
                        }
                        else
                        {
                            if (statusToolbarControl != null)
                                statusToolbarControl.SetFalse(true);
                        }
                    }
                }
            }
        }



        // We add this to our window as a callback
        // It tells us when the window is closed so we can keep the button in sync
        public void OnChecklistWindowClosed(object sender, EventArgs e)
        {
            //			_logger.Trace( "OnChecklistWindowClosed" ); 
            if (checklistToolbarControl != null)
                checklistToolbarControl.SetFalse(false);
            UpdateChecklistVisibility(false);
        }



        // We add this to our window as a callback
        // It tells us when the window is opened so we can keep the button in sync
        public void OnChecklistWindowOpened(object sender, EventArgs e)
        {
            _logger.Trace( Localizer.Format("#LOC_xSci_81") );
            if (checklistToolbarControl != null)
                checklistToolbarControl.SetTrue(false);
            UpdateChecklistVisibility(true);
        }



        // Let a window suggest settings are saved.
        public void OnSettingsDirty(object sender, EventArgs e)
        {
            if (GameHelper.AllowChecklistWindow())
            {
                WindowSettings W = _checklistWindow.BuildSettings();
                W._scene = HighLogic.LoadedScene;
                Config.SetWindowConfig(W);
            }
        }


        #endregion



        #region METHODS Status window callbacks

        // The noise is played on this object because we have access to "gameObject"
        // To play the noise the status window pops this event
        public void OnPlayNoise(object sender, EventArgs e)
        {
            //			_logger.Trace( "OnPlayNoise" );
            _alertNoise.PlaySound();
        }



        // Called when the toolbar button for the status window is toggled on.
        private void StatusButton_Open() // object sender, EventArgs e )
        {
            if (!_active)
                return;

            UpdateStatusVisibility(true);
        }



        // Called when the toolbar button for the status window is toggled off.
        private void StatusButton_Close() // object sender, EventArgs e )
        {
            if (!_active)
                return;

            UpdateStatusVisibility(false);
        }



        // We add this to our window as a callback
        // It tells us when the window is closed so we can keep the button in sync
        public void OnStatusWindowClosed(object sender, EventArgs e)
        {
            // _logger.Trace( "OnStatusWindowClosed" ); 
            if (statusToolbarControl != null)
                statusToolbarControl.SetFalse(true);
            UpdateStatusVisibility(false);
        }



        // We add this to our window as a callback
        // It tells us when the window is opened so we can keep the button in sync
        public void OnStatusWindowOpened(object sender, EventArgs e)
        {
            // _logger.Trace( "OnStatusWindowOpened" );
            if (statusToolbarControl != null)
                statusToolbarControl.SetTrue(true);
            UpdateStatusVisibility(true);
        }
        #endregion



#region METHODS Settings callbacks

#if false
        // We register this with the settings window.
        // When the blizzy toolbar setting changes this gets popped so we can recreate the buttons
        private void Settings_UseBlizzysToolbarChanged(object sender, EventArgs e)
        {
            InitButtons();


            // Need to set this
            if (_checklistWindow.IsVisible)
                //_checklistButton.SetOn( );
                checklistToolbarControl.SetTrue(false);
            else
                //_checklistButton.SetOff();
                checklistToolbarControl.SetFalse(false);
#if false
			if ( _statusButton != null )
			{
				if( _statusWindow.IsVisible( ) )
					_statusButton.SetOn( );
				else
					_statusButton.SetOff( );
			}
#endif


            if (statusToolbarControl != null)
            {
                if (_statusWindow.IsVisible())
                    statusToolbarControl.SetTrue(false);
                else
                    statusToolbarControl.SetFalse(false);
            }

        }
#endif

        private void Settings_RighClickMutesMusicChanged(object sender, EventArgs e)
        {
            InitButtons();


            // Need to set this
            if (_checklistWindow.IsVisible)
                //_checklistButton.SetOn( );
                checklistToolbarControl.SetTrue(true);
            else
                //_checklistButton.SetOff();
                checklistToolbarControl.SetFalse(true);
#if false
			if ( _statusButton != null )
			{
				if( _statusWindow.IsVisible( ) )
					_statusButton.SetOn( );
				else
					_statusButton.SetOff( );
			}
#endif
            if (statusToolbarControl != null)
            {
                if (_statusWindow.IsVisible())
                    checklistToolbarControl.SetTrue(true);
                else
                    checklistToolbarControl.SetFalse(true);
            }

        }



#endregion



#region METHODS General Toolbar functions

        // Initializes the toolbar button.
        private void InitButtons()
        {
            //			_logger.Info( "InitButtons" );
            RemoveButtons();
            AddButtons();
            //			_logger.Info( "InitButtons Done" );
        }


        internal const string MODID = "xScience";
        internal const string MODNAME = "[x] Science!";

        internal const string WINDOW_CHECKLIST = "[x] Science Checklist";

        
        void LeftButtonToggle()
        {
            //if (checklistToolbarControl.Enabled)
            if (!_checklistWindow.IsVisible)
            {
               // _active = true;
                ChecklistButton_Open(null, null);
            }
            else
                ChecklistButton_Close(null, null);
        }
        void RightButton()
        {
            ChecklistButton_RightClick(null, null);
        }


        // Add the buttons
        private void AddButtons()
        {
            //Texture2D StockTexture;

            if (checklistToolbarControl == null)
            {
                checklistToolbarControl = gameObject.AddComponent<ToolbarControl>();
                checklistToolbarControl.AddToAllToolbars(null, null,
                ApplicationLauncher.AppScenes.SPACECENTER |
                ApplicationLauncher.AppScenes.FLIGHT |
                ApplicationLauncher.AppScenes.MAPVIEW |
                ApplicationLauncher.AppScenes.VAB |
                ApplicationLauncher.AppScenes.SPH |
                ApplicationLauncher.AppScenes.TRACKSTATION,
                MODID,
                    Localizer.Format("#LOC_xSci_82"),
                    Localizer.Format("#LOC_xSci_83"),
                    Localizer.Format("#LOC_xSci_84"),
                    MODNAME
                );
                checklistToolbarControl.AddLeftRightClickCallbacks(LeftButtonToggle, RightButton);
            }

            //_checklistButton = new UnifiedButton( gameObject, 1);

#if false
			if( BlizzysToolbarButton.IsAvailable )
			{
				_checklistButton.UseBlizzyIfPossible = Config.UseBlizzysToolbar;

				var texturePath = Localizer.Format("#LOC_xSci_85");
				if( !GameDatabase.Instance.ExistsTexture( texturePath ) )
				{
					var texture = TextureHelper.FromResource( Localizer.Format("#LOC_xSci_86"), 24, 24 );
					var ti = new GameDatabase.TextureInfo( null, texture, false, true, true );
					ti.name = texturePath;
					GameDatabase.Instance.databaseTexture.Add( ti );
				}
//				_logger.Info( "Load : Blizzy texture" );


				_checklistButton.BlizzyNamespace = WINDOW_NAME_CHECKLIST;
				_checklistButton.BlizzyButtonId = Localizer.Format("#LOC_xSci_87");
				_checklistButton.BlizzyToolTip = Localizer.Format("#LOC_xSci_88");
				_checklistButton.BlizzyText = Localizer.Format("#LOC_xSci_89");
				_checklistButton.BlizzyTexturePath = texturePath;
				_checklistButton.BlizzyVisibility = new GameScenesVisibility( GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION );
//				_logger.Info( "Load : Set Blizzy Stuff" );
			}



			StockTexture = TextureHelper.FromResource( Localizer.Format("#LOC_xSci_90"), 38, 38 );
/*			if( StockTexture != null )
				_logger.Info( "Load : Stock texture" );
			else
				_logger.Info( "Load : cant load texture" );*/
			_checklistButton.LauncherTexture = StockTexture;
			_checklistButton.LauncherVisibility =
				ApplicationLauncher.AppScenes.SPACECENTER |
				ApplicationLauncher.AppScenes.FLIGHT |
				ApplicationLauncher.AppScenes.MAPVIEW |
				ApplicationLauncher.AppScenes.VAB |
				ApplicationLauncher.AppScenes.SPH |
				ApplicationLauncher.AppScenes.TRACKSTATION;
//			_logger.Info( "Load : Set Stock Stuff" );


			_checklistButton.ButtonOn += ChecklistButton_Open;
			_checklistButton.ButtonOff += ChecklistButton_Close;
			_checklistButton.RightClick += ChecklistButton_RightClick;
			_checklistButton.Add( );


#endif


            if (Config.RighClickMutesMusic) // So we need both buttons
            {
#if false
				_statusButton = new UnifiedButton(gameObject, 2 );

				if( BlizzysToolbarButton.IsAvailable )
				{
					_statusButton.UseBlizzyIfPossible = Config.UseBlizzysToolbar;

					var texturePath = Localizer.Format("#LOC_xSci_91");
					if( !GameDatabase.Instance.ExistsTexture( texturePath ) )
					{
						var texture = TextureHelper.FromResource( Localizer.Format("#LOC_xSci_92"), 24, 24 );
						var ti = new GameDatabase.TextureInfo( null, texture, false, true, true );
						ti.name = texturePath;
						GameDatabase.Instance.databaseTexture.Add( ti );
					}
	//				_logger.Info( "Load : Blizzy texture" );


					_statusButton.BlizzyNamespace = WINDOW_NAME_CHECKLIST;
					_statusButton.BlizzyButtonId = Localizer.Format("#LOC_xSci_93");
					_statusButton.BlizzyToolTip = Localizer.Format("#LOC_xSci_94");
					_statusButton.BlizzyText = Localizer.Format("#LOC_xSci_95");
					_statusButton.BlizzyTexturePath = texturePath;
					_statusButton.BlizzyVisibility = new GameScenesVisibility( GameScenes.FLIGHT );
	//				_logger.Info( "Load : Set Blizzy Stuff" );
				}
				UnifiedButton.toolbarControl.SetTexture(Localizer.Format("#LOC_xSci_96"), Localizer.Format("#LOC_xSci_97"));





				StockTexture = TextureHelper.FromResource( Localizer.Format("#LOC_xSci_98"), 38, 38 );
	/*			if( StockTexture != null )
					_logger.Info( "Load : Stock texture" );
				else
					_logger.Info( "Load : cant load texture" );*/
				_statusButton.LauncherTexture = StockTexture;
				_statusButton.LauncherVisibility =
					ApplicationLauncher.AppScenes.FLIGHT |
					ApplicationLauncher.AppScenes.MAPVIEW;
	//			_logger.Info( "Load : Set Stock Stuff" );


				_statusButton.ButtonOn += StatusButton_Open;
				_statusButton.ButtonOff += StatusButton_Close;
				_statusButton.Add( );
#endif
                if (statusToolbarControl == null)
                {
                    statusToolbarControl = gameObject.AddComponent<ToolbarControl>();
                    statusToolbarControl.AddToAllToolbars(StatusButton_Open, StatusButton_Close,
                    ApplicationLauncher.AppScenes.FLIGHT |
                    ApplicationLauncher.AppScenes.MAPVIEW,
                    MODID + "2",
                    Localizer.Format("#LOC_xSci_99"),
                    Localizer.Format("#LOC_xSci_96"),
                    Localizer.Format("#LOC_xSci_97"),
                    WINDOW_CHECKLIST
                    );
                }

            }
            if (statusToolbarControl != null)
            {
                if (_statusWindow.IsVisible())
                    statusToolbarControl.SetTrue(true);
                else
                    statusToolbarControl.SetFalse(true);
            }

        }



        private void RemoveButtons()
        {
            if (statusToolbarControl != null)
            {
                statusToolbarControl.OnDestroy();
                Destroy(statusToolbarControl);
                statusToolbarControl = null;
            }
            if (checklistToolbarControl != null)
            {
                checklistToolbarControl.OnDestroy();
                Destroy(checklistToolbarControl);
                checklistToolbarControl = null;
            }
        }
        #endregion



        #region METHODS Window helper functions
        // Shows or hides the Checklist Window if the KSP toolbar is visible and the toolbar button is toggled on.
        private void UpdateChecklistVisibility(bool NewVisibility)
        {
            if (!_active)
                return;

            //			_logger.Trace( "UpdateChecklistVisibility" );
            _checklistWindow.IsVisible = NewVisibility;
            if (_checklistWindow.IsVisible)
                ScienceEventHandler.ScheduleExperimentUpdate(seconds: 0.1f);
        }



        // Shows or hides the Status Window if the KSP toolbar is visible and the toolbar button is toggled on.
        private void UpdateStatusVisibility(bool NewVisibility)
        {
            if (!_active)
                return;

            //			_logger.Trace( "UpdateStatusVisibility" );
            _statusWindow.SetVisible(NewVisibility);
            if (_statusWindow.IsVisible())
                ScienceEventHandler.ScheduleExperimentUpdate(seconds: 0.1f);
        }



        // Teeny-tiny helper function.  Are we drawing windows or not
        private bool UiActive()
        {
            if ((!_UiHidden) && _active && _launcherVisible)
                return true;
            return false;
        }
#endregion



#region METHODS Mute functions
        // Default values
        bool muted = false;
        float oldVolume = 0.40f;



        private void HammerMusicMute()
        {
            if (muted)
                MusicLogic.SetVolume(0f, 0f);
        }


        // Runs when scene switch is requested
        private void onGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> action)
        {
            // The game likes to play music when we switch scenes, so we have to tell it to shut up once more
            HammerMusicMute();
        }

        // Runs when scene is done switching
        private void onLevelWasLoaded(GameScenes action)
        {
            HammerMusicMute(); // Ensure we enforce music volume anyway
        }

        public bool Muted
        {
            get
            {
                return muted;
            }
            set
            {
                // Mute
                if (value == true)
                {
                    // Save the old music volume
                    oldVolume = GameSettings.MUSIC_VOLUME;

                    // Mute the music
                    MusicLogic.SetVolume(0f, 0f);
                    //                   _logger.Info("[MusicMute]: Muted music");
                }
                // Unmute
                else
                {
                    // Set the music volume to what it was before
                    MusicLogic.SetVolume(oldVolume, oldVolume);
                    //                   _logger.Info("[MusicMute]: Set music volume to: " + oldVolume);
                }

                muted = value;
            }
        }
#endregion
    }
}
