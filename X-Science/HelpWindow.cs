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



		protected override void DrawWindowContents( int windowID )
		{
			scrollPosition = GUILayout.BeginScrollView( scrollPosition );
			GUILayout.BeginVertical( GUILayout.ExpandWidth( true ) );

			GUILayout.Label( Localizer.Format("#LOC_xSci_35"), sectionStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.Space( wScale( 30 ) );
			GUILayout.Label(Localizer.Format("#LOC_xSci_36"), sectionStyle, GUILayout.ExpandWidth(true));
			GUILayout.Label( Localizer.Format("#LOC_xSci_37"), labelStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.Space( wScale( 20 ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_38"), sectionStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_39"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_40"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_41"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_42"), labelStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.Space( wScale( 20 ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_43"), sectionStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_44"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_45"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_46"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_47"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_48"), labelStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.Space( wScale( 20 ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_49"), sectionStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_50"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_51"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_52"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_53"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_54"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_55"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_56"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_57"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_58"), labelStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.Space( wScale( 20 ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_59"), sectionStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_60"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_61"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_62"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_63"), labelStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.Space( wScale( 20 ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_64"), sectionStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_65"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_66"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_67"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_68"), labelStyle, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( Localizer.Format("#LOC_xSci_69"), labelStyle, GUILayout.ExpandWidth( true ) );

			GUILayout.EndVertical( );
			GUILayout.EndScrollView( );

			GUILayout.Space( wScale( 8 ) );
		}
	}
}
