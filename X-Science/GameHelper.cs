﻿using KSP.Localization;



/* 
 * THIS IS A STATIC CLASS
 */

namespace ScienceChecklist {
	/// <summary>
	/// Contains helper methods for the current game state.
	/// </summary>
	internal static class GameHelper {



	
		// All the Scenes, which ones are kosher?
		public static bool AllowChecklistWindow( GameScenes? Scene = null )
		{
			if( Scene == null )
				Scene = HighLogic.LoadedScene;
			switch( Scene )
			{
				case GameScenes.LOADING:
				case GameScenes.LOADINGBUFFER:
				case GameScenes.MAINMENU:
				case GameScenes.SETTINGS:
				case GameScenes.CREDITS:
					return false;
				case GameScenes.SPACECENTER:
				case GameScenes.EDITOR:
				case GameScenes.FLIGHT:
				case GameScenes.TRACKSTATION:
					return true;
				case GameScenes.PSYSTEM:
					return false;
				default:
					return false;
			}
		}



		// All the Scenes, which ones are kosher?
		public static bool AllowStatusWindow( GameScenes? Scene = null )
		{
			if( Scene == null )
				Scene = HighLogic.LoadedScene;
			if( Scene == GameScenes.FLIGHT )
				return true;
			return false;
		}



		public static void StopTimeWarp( )
		{
			if( TimeWarp.CurrentRateIndex > 0 )
				TimeWarp.SetRate( 0, true );
		}

        #region NO_LOCALIZATION
        public static string LocalizeBodyName( string input )
		{
			return Localizer.Format("<<1>>", input);
		}
        #endregion


    }
}
