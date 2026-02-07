using KSP.Localization;
using System;
using UnityEngine;



namespace ScienceChecklist
{
	class HelpWindow : Window<ScienceChecklistAddon>
	{
		private GUIStyle labelStyle;
		private GUIStyle sectionStyle;
		private Vector2 scrollPosition;
		private readonly ScienceChecklistAddon	_parent;



		public HelpWindow( ScienceChecklistAddon Parent )
			: base(Localizer.Format("#LOC_xSci_34"), 500, Screen.height * 0.75f  / ScienceChecklistAddon.Config.UiScale )
		{
			_parent = Parent;
			UiScale = ScienceChecklistAddon.Config.UiScale;
			scrollPosition = Vector2.zero;
			ScienceChecklistAddon.Config.UiScaleChanged += OnUiScaleChange;
		}



		protected override void ConfigureStyles( )
		{
			base.ConfigureStyles();

			if( labelStyle == null )
			{
				labelStyle = new GUIStyle( _skin.label );
				labelStyle.wordWrap = true;
				labelStyle.fontStyle = FontStyle.Normal;
				labelStyle.normal.textColor = Color.white;
				labelStyle.stretchWidth = true;
				labelStyle.stretchHeight = false;
				labelStyle.margin.bottom -= wScale( 2 );
				labelStyle.padding.bottom -= wScale( 2 );
			}

			if( sectionStyle == null )
			{
				sectionStyle = new GUIStyle( labelStyle );
				sectionStyle.fontStyle = FontStyle.Bold;
			}
		}



		private void OnUiScaleChange( object sender, EventArgs e )
		{
			UiScale = ScienceChecklistAddon.Config.UiScale;
			labelStyle = null;
			sectionStyle = null;
			base.OnUiScaleChange( );
			ConfigureStyles( );
		}

		private void GUI_AddSection( string title, int spacing )
		{
			GUILayout.Space( wScale( spacing ) );
			GUILayout.Label( title, sectionStyle, GUILayout.ExpandWidth( true ) );
		}

		private void GUI_AddParagraph( string text )
		{
			GUILayout.Label( text, labelStyle, GUILayout.ExpandWidth( true ) );
		}

		private void GUI_AddListItem( string text )
		{
			GUILayout.BeginHorizontal( labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( "*", labelStyle, GUILayout.ExpandWidth( false ) );
			GUILayout.Label( text, labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.EndHorizontal();
		}

		protected override void DrawWindowContents( int windowID )
		{
			scrollPosition = GUILayout.BeginScrollView( scrollPosition );
			GUILayout.BeginVertical( GUILayout.ExpandWidth( true ) );

			// "[x] Science! by Z-Key Aerospace and Bodrick."
			GUILayout.Label( Localizer.Format("#LOC_xSci_35"), sectionStyle, GUILayout.ExpandWidth( true ) );

			// "About"
			GUI_AddSection(Localizer.Format("#LOC_xSci_36"), 30);
			GUI_AddParagraph( Localizer.Format("#LOC_xSci_37") );

			// "The filter buttons at the bottom of the window are ..."
			GUI_AddSection( Localizer.Format("#LOC_xSci_38a"), 20 );
			GUI_AddParagraph( Localizer.Format("#LOC_xSci_38b") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_39") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_40") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_40a") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_41") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_42") );

			// "The text filter ..."
			GUI_AddSection( Localizer.Format("#LOC_xSci_43"), 20 );
			GUI_AddParagraph( Localizer.Format("#LOC_xSci_44") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_45") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_46") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_47") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_48") );

			// "The settings are ..."
			GUI_AddSection( Localizer.Format("#LOC_xSci_49a"), 20);
			GUI_AddParagraph( Localizer.Format("#LOC_xSci_49b") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_50") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_51") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_52") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_53") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_54") );
			#if false
			// Help for "Use blizzy78's toolbar" (code is disabled)
			GUI_AddListItem( Localizer.Format("#LOC_xSci_55") );
			#endif
			GUI_AddListItem( Localizer.Format("#LOC_xSci_56") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_57") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_58") );

			// "Here and Now Window ..."
			GUI_AddSection( Localizer.Format("#LOC_xSci_59"), 20);
			GUI_AddParagraph( Localizer.Format("#LOC_xSci_60") );
			GUI_AddParagraph( Localizer.Format("#LOC_xSci_61") );
			GUI_AddParagraph( Localizer.Format("#LOC_xSci_62") );
			GUI_AddParagraph( Localizer.Format("#LOC_xSci_63") );

			// "Did you know? (includes spoilers) ..."
			GUI_AddSection( Localizer.Format("#LOC_xSci_64"), 20);
			GUI_AddListItem( Localizer.Format("#LOC_xSci_65") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_66") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_67") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_68") );
			GUI_AddListItem( Localizer.Format("#LOC_xSci_69") );

			GUILayout.EndVertical( );
			GUILayout.EndScrollView( );

			GUILayout.Space( wScale( 8 ) );
		}
	}
}
