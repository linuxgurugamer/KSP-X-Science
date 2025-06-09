
using System.Reflection;
using UnityEngine;
using KSP.IO;
using ToolbarControl_NS;

namespace ScienceChecklist
{
	/// <summary>
	/// Contains static methods to assist in creating textures.
	/// </summary>
	internal static class TextureHelper {
		/// <summary>
		/// Creates a new Texture2D from an embedded resource.
		/// </summary>
		/// <param name="resource">The location of the resource in the assembly.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <returns></returns>
		public static Texture2D FromResource( string resource, int width, int height )
		{
#if false
			var tex = new Texture2D( width, height, TextureFormat.ARGB32, false );
			var iconStream = Assembly.GetExecutingAssembly( ).GetManifestResourceStream( resource ).ReadToEnd( );
			if( iconStream == null )
				return null;
			tex.LoadImage( iconStream );
			tex.Apply( );
			return tex;
#else
			var sar = resource.Split('.');
			int cnt = sar.Length;

			Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);

			ToolbarControl.LoadImageFromFile(ref tex, KSPUtil.ApplicationRootPath + "GameData/[x]_Science!/PluginData/Icons/" + sar[cnt-2]);
			return tex;
#endif
		}



		public static Texture2D LoadImage<T>( string filename, int width, int height )
		{
			if( File.Exists<T>( filename ) )
			{
				var bytes = File.ReadAllBytes<T>( filename );
				Texture2D texture = new Texture2D( width, height, TextureFormat.ARGB32, false );
				texture.LoadImage( bytes );
				return texture;
			}
			else
				return null;
		}

	}
}
