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
			: base( "[x] Science! Settings", 240, 360 )
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

			var toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.SimpleMode, new GUIContent("Simple mode", "Hides the bottom number on the experiment buttons for a cleaner look."), toggleStyle);
			if (toggle != ScienceChecklistAddon.Config.SimpleMode)
			{
				ScienceChecklistAddon.Config.SimpleMode = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.HideCompleteExperiments, new GUIContent( "Hide complete experiments", "Experiments considered complete will not be shown." ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.HideCompleteExperiments )
			{
				ScienceChecklistAddon.Config.HideCompleteExperiments = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.CompleteWithoutRecovery, new GUIContent( "Complete without recovery", "Show experiments as completed even if they have not been recovered yet.\nYou still need to recover the science to get the points!\nJust easier to see what is left." ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.CompleteWithoutRecovery )
			{
				ScienceChecklistAddon.Config.CompleteWithoutRecovery = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.CheckUnloadedVessels, new GUIContent("Check unloaded vessels", "Unloaded vessels will be checked for recoverable science."), toggleStyle);
			if( toggle != ScienceChecklistAddon.Config.CheckUnloadedVessels )
			{
				ScienceChecklistAddon.Config.CheckUnloadedVessels = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.CheckDebris, new GUIContent("Check debris", "Vessels marked as debris will be checked for recoverable science."), toggleStyle);
			if (toggle != ScienceChecklistAddon.Config.CheckDebris)
			{
				ScienceChecklistAddon.Config.CheckDebris = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.HideMinScienceSlider, new GUIContent("Hide Min Science Slider", "Hide the minimum science slider in the Here & Now window"), toggleStyle);
			if (toggle != ScienceChecklistAddon.Config.HideMinScienceSlider)
			{
				ScienceChecklistAddon.Config.HideMinScienceSlider = toggle;
				save = true;
			}


			toggle = GUILayout.Toggle(ScienceChecklistAddon.Config.VeryLowMinScience, new GUIContent("Low Min Science", "Minimum science slider in the Here & Now window will go fom 0.0001 to 0.1"), toggleStyle);
			if (toggle != ScienceChecklistAddon.Config.VeryLowMinScience)
			{
				ScienceChecklistAddon.Config.VeryLowMinScience = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.AllFilter, new GUIContent( "Allow all filter", "Adds a filter button showing all experiments, even on unexplored bodies using unavailable instruments.\nMight be considered cheating." ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.AllFilter )
			{
				ScienceChecklistAddon.Config.AllFilter = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.FilterDifficultScience, new GUIContent( "Filter difficult science", "Hide a few experiments such as flying at stars and gas giants that are almost impossible.\n Also most EVA reports before upgrading Astronaut Complex." ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.FilterDifficultScience )
			{
				ScienceChecklistAddon.Config.FilterDifficultScience = toggle;
				save = true;
			}

			toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.SelectedObjectWindow, new GUIContent( "Selected Object Window", "Show the Selected Object Window in the Tracking Station." ), toggleStyle );
			if( toggle != ScienceChecklistAddon.Config.SelectedObjectWindow )
			{
				ScienceChecklistAddon.Config.SelectedObjectWindow = toggle;
				save = true;
			}

#if false
			if( BlizzysToolbarButton.IsAvailable )
			{
				toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.UseBlizzysToolbar, new GUIContent( "Use blizzy78's toolbar", "Remove [x] Science button from stock toolbar and add to blizzy78 toolbar." ), toggleStyle );
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
			GUILayout.Label( "Right click [x] icon", labelStyle );
			GUIContent[] Options = {
				new GUIContent( "Mute music", "Here & Now window gets its own icon" ),
				new GUIContent( "Opens Here & Now window", "Here & Now icon is hidden" )
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
				toggle = GUILayout.Toggle( ScienceChecklistAddon.Config.MusicStartsMuted, new GUIContent( "Music starts muted", "Title music is silenced upon load.  No need to right click" ), toggleStyle );
				if( toggle != ScienceChecklistAddon.Config.MusicStartsMuted )
				{
					ScienceChecklistAddon.Config.MusicStartsMuted = toggle;
					save = true;
				}
			}

			GUILayout.Space(2);
			GUILayout.Label(new GUIContent( "Adjust UI size", "Adjusts the the UI scaling." ), labelStyle );
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
			GUI.Label( new Rect( 4, windowPos.height - 13, windowPos.width - 20, 12 ), "[x] Science! V" + version, versionStyle );
		}
	}
}
