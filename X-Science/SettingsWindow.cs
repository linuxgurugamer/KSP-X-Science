using KSP.Localization;
using System;
using UnityEngine;



namespace ScienceChecklist
{
	class SettingsWindow : Window<ScienceChecklistAddon>
	{
		private readonly string version;
		private GUIStyle labelStyle;
		private GUIStyle toggleStyle;
		private GUIStyle sliderStyle;
		private GUIStyle editStyle;
		private GUIStyle versionStyle;
		private GUIStyle selectionStyle;

		private readonly Logger _logger;
		private readonly ScienceChecklistAddon _parent;



		// Constructor
		public SettingsWindow( ScienceChecklistAddon Parent )
			: base( Localizer.Format("#LOC_xSci_160"), 240, 360 )
		{
			_logger = new Logger( this );
			_parent = Parent;
			UiScale = 1; // Don't let this change
			version = Utilities.GetDllVersion( this );
		}


		// For our Window base class
		protected override void ConfigureStyles( )
		{
			base.ConfigureStyles( );

			if( labelStyle == null )
			{
				labelStyle = new GUIStyle( _skin.label );
				labelStyle.wordWrap = false;
				labelStyle.fontStyle = FontStyle.Normal;
				labelStyle.normal.textColor = Color.white;

				toggleStyle = new GUIStyle( _skin.toggle );
				sliderStyle = new GUIStyle( _skin.horizontalSlider );
				editStyle = new GUIStyle( _skin.textField );
				versionStyle = Utilities.GetVersionStyle( );
				selectionStyle = new GUIStyle( _skin.button );
				selectionStyle.margin = new RectOffset( 30, 0, 0, 0 );
			}
		}



		// For our Window base class
		protected override void DrawWindowContents( int windowID )
		{
			GUILayout.BeginVertical( );

			bool save = false;

			var toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.SimpleMode, new GUIContent(Localizer.Format("#LOC_xSci_161"), Localizer.Format("#LOC_xSci_162")), toggleStyle);
			if (toggle != ScienceChecklistAddon.Config.SimpleMode)
			{
				ScienceChecklistAddon.Config.SimpleMode = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.HideCompleteExperiments, new GUIContent( Localizer.Format("#LOC_xSci_163"), Localizer.Format("#LOC_xSci_164") ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.HideCompleteExperiments )
			{
				ScienceChecklistAddon.Config.HideCompleteExperiments = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.CompleteWithoutRecovery, new GUIContent( Localizer.Format("#LOC_xSci_165"), Localizer.Format("#LOC_xSci_166") ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.CompleteWithoutRecovery )
			{
				ScienceChecklistAddon.Config.CompleteWithoutRecovery = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.CheckUnloadedVessels, new GUIContent(Localizer.Format("#LOC_xSci_167"), Localizer.Format("#LOC_xSci_168")), toggleStyle);
			if( toggle != ScienceChecklistAddon.Config.CheckUnloadedVessels )
			{
				ScienceChecklistAddon.Config.CheckUnloadedVessels = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.CheckDebris, new GUIContent(Localizer.Format("#LOC_xSci_169"), Localizer.Format("#LOC_xSci_170")), toggleStyle);
			if (toggle != ScienceChecklistAddon.Config.CheckDebris)
			{
				ScienceChecklistAddon.Config.CheckDebris = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.HideMinScienceSlider, new GUIContent(Localizer.Format("#LOC_xSci_171"), Localizer.Format("#LOC_xSci_172")), toggleStyle);
			if (toggle != ScienceChecklistAddon.Config.HideMinScienceSlider)
			{
				ScienceChecklistAddon.Config.HideMinScienceSlider = toggle;
				save = true;
			}


			toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.VeryLowMinScience, new GUIContent(Localizer.Format("#LOC_xSci_173"), Localizer.Format("#LOC_xSci_174")), toggleStyle);
			if (toggle != ScienceChecklistAddon.Config.VeryLowMinScience)
			{
				ScienceChecklistAddon.Config.VeryLowMinScience = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.AllFilter, new GUIContent( Localizer.Format("#LOC_xSci_175"), Localizer.Format("#LOC_xSci_176") ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.AllFilter )
			{
				ScienceChecklistAddon.Config.AllFilter = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.FilterDifficultScience, new GUIContent( Localizer.Format("#LOC_xSci_177"), Localizer.Format("#LOC_xSci_178") ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.FilterDifficultScience )
			{
				ScienceChecklistAddon.Config.FilterDifficultScience = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.SelectedObjectWindow, new GUIContent( Localizer.Format("#LOC_xSci_179"), Localizer.Format("#LOC_xSci_180") ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.SelectedObjectWindow )
			{
				ScienceChecklistAddon.Config.SelectedObjectWindow = toggle;
				save = true;
			}

#if false
			if( BlizzysToolbarButton.IsAvailable )
			{
				toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.UseBlizzysToolbar, new GUIContent( Localizer.Format("#LOC_xSci_181"), Localizer.Format("#LOC_xSci_182") ), toggleStyle );
				if( toggle != ScienceChecklistAddon.Config.UseBlizzysToolbar )
				{
					ScienceChecklistAddon.Config.UseBlizzysToolbar = toggle;
					save = true;
				}
			}
#endif
			GUILayout.Space(2);
			int selected = 0;
			if( !ScienceChecklistAddon.Config.RighClickMutesMusic )
				selected = 1;
			int new_selected = selected;
			GUILayout.Label( Localizer.Format("#LOC_xSci_183"), labelStyle );
			GUIContent[] Options = {
				new GUIContent( Localizer.Format("#LOC_xSci_184"), Localizer.Format("#LOC_xSci_185") ),
				new GUIContent( Localizer.Format("#LOC_xSci_186"), Localizer.Format("#LOC_xSci_187") )
			};
			new_selected = GUILayout.SelectionGrid( selected, Options, 1, selectionStyle );
			if( new_selected != selected )
			{
				if( new_selected == 0 )
					ScienceChecklistAddon.Config.RighClickMutesMusic = true;
				else
					ScienceChecklistAddon.Config.RighClickMutesMusic = false;
				save = true;
			}

			if( ScienceChecklistAddon.Config.RighClickMutesMusic )
			{
				toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.MusicStartsMuted, new GUIContent( Localizer.Format("#LOC_xSci_188"), Localizer.Format("#LOC_xSci_189") ), toggleStyle );
				if( toggle != ScienceChecklistAddon.Config.MusicStartsMuted )
				{
					ScienceChecklistAddon.Config.MusicStartsMuted = toggle;
					save = true;
				}
			}

			GUILayout.Space(2);
			GUILayout.Label(new GUIContent( Localizer.Format("#LOC_xSci_190"), Localizer.Format("#LOC_xSci_191") ), labelStyle );
			var slider = GUILayout.HorizontalSlider( ScienceChecklistAddon.Config.UiScale, 1, 2 );
			if( slider != ScienceChecklistAddon.Config.UiScale )
			{
				ScienceChecklistAddon.Config.UiScale = slider;
				save = true;
			}

			if( save )
				ScienceChecklistAddon.Config.Save( );

			GUILayout.EndVertical( );
			GUILayout.Space(10);
			GUI.Label( new Rect( 4, windowPos.height - 13, windowPos.width - 20, 12 ), Localizer.Format("#LOC_xSci_192") + version, versionStyle );
		}
	}
}
