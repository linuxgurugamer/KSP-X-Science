using KSP.Localization;
using UnityEngine;



namespace ScienceChecklist
{
	sealed class Noise : MonoBehaviour
	{
		private static readonly Logger _logger = new Logger( Localizer.Format("#LOC_xSci_70") );
		private static readonly string _file = Localizer.Format("#LOC_xSci_71");

		private void Awake( )
		{ }

		private void OnDestroy( )
		{ }

		public void PlaySound( )
		{
			if( gameObject == null )
			{
				_logger.Debug( Localizer.Format("#LOC_xSci_72") );
				return;
			}

			AudioSource audioSource = gameObject.GetComponent<AudioSource>( ) ?? gameObject.AddComponent<AudioSource>( );
			if( audioSource != null )
			{
				audioSource.spatialBlend = 0f;
				audioSource.panStereo = 0f;
				
				AudioClip Clip = null;
				Clip = GameDatabase.Instance.GetAudioClip( _file );
				if( Clip == null )
				{
					_logger.Debug( Localizer.Format("#LOC_xSci_73") );
					return;
				}

				audioSource.PlayOneShot( Clip, Mathf.Clamp( GameSettings.UI_VOLUME, 0f, 1f ) );
			}
			else
				_logger.Debug( Localizer.Format("#LOC_xSci_74") );
		}
	}
}
