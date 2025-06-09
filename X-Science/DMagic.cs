using KSP.Localization;
using System;
using System.Reflection;



namespace ScienceChecklist
{
	/// <summary>
	/// Class to access the DMagic API via reflection so we don't have to recompile when the DMagic mod updates. If the DMagic API changes, we will need to modify this code.
	/// </summary>



	public class DMagicFactory
	{
//		private readonly Logger _logger;
		private bool _isInstalled;
		static internal Type _tDMModuleScienceAnimate;
		static internal Type _tDMModuleScienceAnimateGeneric;
		private Type _tDMBasicScienceModule;
		private Type _tDMAPI;

		private DMAPI _DMAPI;
		private DMModuleScienceAnimateGeneric _DMModuleScienceAnimateGeneric;



		public DMagicFactory( )
		{
//			_logger = new Logger( this );
//			_logger.Trace( "DMagic API Start" );
			_isInstalled = false;



			_tDMAPI =							getType( Localizer.Format("#LOC_xSci_14") );
			_tDMModuleScienceAnimate =			getType( Localizer.Format("#LOC_xSci_15") );
			_tDMModuleScienceAnimateGeneric =	getType( Localizer.Format("#LOC_xSci_16") );
			_tDMBasicScienceModule =			getType( Localizer.Format("#LOC_xSci_17") );



			if( _tDMAPI != null )
			{
//				_logger.Trace( "DMAPI Available" );
				_isInstalled = true;
				_DMAPI = new DMAPI( _tDMAPI );
			}

/*			if( _tDMModuleScienceAnimate != null ) // Don't actually seem to be using this one
				_logger.Trace( "DMModuleScienceAnimate Available" );
*/
			if( _tDMModuleScienceAnimateGeneric != null )
			{
//				_logger.Trace( "DMModuleScienceAnimateGeneric Available" );
				_isInstalled = true;
				_DMModuleScienceAnimateGeneric = new DMModuleScienceAnimateGeneric( _tDMModuleScienceAnimateGeneric );
			}

/*			if( _tDMBasicScienceModule != null ) // Don't actually seem to be using this one
				_logger.Trace( "DMBasicScienceModule Available" );

			if( _isInstalled )
				_logger.Trace( "DMagic API Installed" );
			else
				_logger.Trace( "DMagic API Not Found" );
 */
		}



		internal static Type getType( string name )
		{
			Type type = null;
			AssemblyLoader.loadedAssemblies.TypeOperation( t => {
				if( t.FullName == name )
				{
					type = t;
				}
			});
			return type;
		}



		public bool inheritsFromOrIsDMModuleScienceAnimate( object o )
		{
			if( _tDMModuleScienceAnimate == null )
				return false;
			return ((o.GetType().IsSubclassOf(_tDMModuleScienceAnimate) || o.GetType() == _tDMModuleScienceAnimate));
		}



		public bool inheritsFromOrIsDMModuleScienceAnimateGeneric( object o )
		{
			if( _tDMModuleScienceAnimateGeneric == null )
				return false;
			return ((o.GetType().IsSubclassOf(_tDMModuleScienceAnimateGeneric) || o.GetType() == _tDMModuleScienceAnimateGeneric));
		}
		
		
		
		public bool inheritsFromOrIsDMBasicScienceModule( object o )
		{
			if( _tDMBasicScienceModule == null )
				return false;
			return ((o.GetType().IsSubclassOf(_tDMBasicScienceModule) || o.GetType() == _tDMBasicScienceModule));
		}



		public DMAPI GetDMAPI( )
		{
			if( _DMAPI != null )
				return _DMAPI;
			return null;
		}



		public DMModuleScienceAnimateGeneric GetDMModuleScienceAnimateGeneric( )
		{
			if( _DMModuleScienceAnimateGeneric != null )
				return _DMModuleScienceAnimateGeneric;
			return null;
		}
	}




	// Wrapper for DMagic API
	public class DMAPI
	{
		private MethodInfo _MIexperimentCanConduct;
		private MethodInfo _MIdeployDMExperiment;
		private MethodInfo _MIgetExperimentSituation;
		private MethodInfo _MIgetBiome;
		private readonly Logger _logger;



		public DMAPI( Type tDMAPI )
		{
			_logger = new Logger( this );




//			_logger.Trace( "DMagic API found. Validating Methods." );
			ParameterInfo[] p;

			_MIexperimentCanConduct = tDMAPI.GetMethod( Localizer.Format("#LOC_xSci_18"), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy );
			p = _MIexperimentCanConduct.GetParameters();
			if (!((p.Length == 1) && (p[0].ParameterType == typeof(IScienceDataContainer)) && _MIexperimentCanConduct.ReturnType == typeof(bool)))
			{
				_logger.Info( Localizer.Format("#LOC_xSci_19") );
				_MIexperimentCanConduct = null;
			}



			_MIdeployDMExperiment = tDMAPI.GetMethod(Localizer.Format("#LOC_xSci_20"), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			p = _MIdeployDMExperiment.GetParameters();
			if (!((p.Length == 2) && (p[0].ParameterType == typeof(IScienceDataContainer)) && (p[1].ParameterType == typeof(bool)) && _MIdeployDMExperiment.ReturnType == typeof(bool)))
			{
				_logger.Info( Localizer.Format("#LOC_xSci_21") );
				_MIdeployDMExperiment = null;
			}



			_MIgetExperimentSituation = tDMAPI.GetMethod(Localizer.Format("#LOC_xSci_22"), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			p = _MIgetExperimentSituation.GetParameters();
			if (!((p.Length == 1) && (p[0].ParameterType == typeof(ModuleScienceExperiment)) && _MIgetExperimentSituation.ReturnType == typeof(ExperimentSituations)))
			{
				_logger.Info( Localizer.Format("#LOC_xSci_23") );
				_MIgetExperimentSituation = null;
			}



			_MIgetBiome = tDMAPI.GetMethod(Localizer.Format("#LOC_xSci_24"), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			p = _MIgetBiome.GetParameters();
			if (!((p.Length == 2) && (p[0].ParameterType == typeof(ModuleScienceExperiment)) && (p[1].ParameterType == typeof(ExperimentSituations)) && _MIgetBiome.ReturnType == typeof(string)))
			{
				_logger.Info( Localizer.Format("#LOC_xSci_25") );
				_MIgetBiome = null;
			}

		}



		public bool experimentCanConduct(IScienceDataContainer isc)
		{
			if (_MIexperimentCanConduct == null)
				return false;
			return (bool)_MIexperimentCanConduct.Invoke(null, new object[] { isc });
		}



		public bool deployDMExperiment(IScienceDataContainer isc, bool HideResultsWindow )
		{
			if (_MIdeployDMExperiment == null)
				return false;
			return (bool)_MIdeployDMExperiment.Invoke(null, new object[] { isc, HideResultsWindow });
		}



		public ExperimentSituations getExperimentSituation(ModuleScienceExperiment mse)
		{
			if (_MIgetExperimentSituation == null)
				return ExperimentSituations.InSpaceHigh;
			return (ExperimentSituations)_MIgetExperimentSituation.Invoke(null, new object[] { mse });
		}



		public string getBiome(ModuleScienceExperiment mse, ExperimentSituations sit)
		{
			if (_MIgetBiome == null)
				return "";
			return (string)_MIgetBiome.Invoke(null, new object[] { mse, sit });
		}
	}



	// Wrapper for DMagic new API
	public class DMModuleScienceAnimateGeneric
	{
		private MethodInfo _MIcanConduct;
		private MethodInfo _MIgatherScienceData;
		private MethodInfo _MIgetSituation;
		private MethodInfo _MIgetBiome;
		private readonly Logger _logger;
		private Type _tDMModuleScienceAnimateGeneric;



		public DMModuleScienceAnimateGeneric( Type tDMModuleScienceAnimateGeneric )
		{
			_logger = new Logger( this );
			_tDMModuleScienceAnimateGeneric = tDMModuleScienceAnimateGeneric;



			if( _tDMModuleScienceAnimateGeneric != null )
			{
//				_logger.Trace( "DMModuleScienceAnimateGeneric found. Validating Methods." );
				ParameterInfo[] p;

				_MIcanConduct = _tDMModuleScienceAnimateGeneric.GetMethod(Localizer.Format("#LOC_xSci_26"), BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				p = _MIcanConduct.GetParameters();
				if (!((p.Length == 0) && _MIcanConduct.ReturnType == typeof(bool)))
				{
					_logger.Info( Localizer.Format("#LOC_xSci_27") );
					_MIcanConduct = null;
				}

				_MIgatherScienceData = _tDMModuleScienceAnimateGeneric.GetMethod(Localizer.Format("#LOC_xSci_28"), BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				p = _MIgatherScienceData.GetParameters();
				if (!((p.Length == 1) && (p[0].ParameterType == typeof(bool)) && _MIgatherScienceData.ReturnType == typeof(void)))
				{
					_logger.Info( Localizer.Format("#LOC_xSci_29") );
					_MIgatherScienceData = null;
				}

				_MIgetSituation = _tDMModuleScienceAnimateGeneric.GetMethod(Localizer.Format("#LOC_xSci_30"), BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				p = _MIgetSituation.GetParameters();
				if (!((p.Length == 0) && _MIgetSituation.ReturnType == typeof(ExperimentSituations)))
				{
					_logger.Info( Localizer.Format("#LOC_xSci_31") );
					_MIgetSituation = null;
				}

				_MIgetBiome = _tDMModuleScienceAnimateGeneric.GetMethod(Localizer.Format("#LOC_xSci_24"), BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				p = _MIgetBiome.GetParameters();
				if (!((p.Length == 1) && (p[0].ParameterType == typeof(ExperimentSituations)) && _MIgetBiome.ReturnType == typeof(string)))
				{
					_logger.Info( Localizer.Format("#LOC_xSci_31") );
					_MIgetBiome = null;
				}
			}
		}



		public bool canConduct(ModuleScienceExperiment mse)
		{
			if (_MIcanConduct == null)
				return false;
			return (bool)_MIcanConduct.Invoke(mse, new object[] { });
		}



		public void gatherScienceData( ModuleScienceExperiment mse, bool HideResultsWindow )
		{
			if (_MIgatherScienceData == null) return;
			_MIgatherScienceData.Invoke(mse, new object[] { HideResultsWindow });
		}



		public ExperimentSituations getSituation(ModuleScienceExperiment mse)
		{
			if (_MIgetSituation == null)
				return ExperimentSituations.InSpaceHigh;
			return (ExperimentSituations)_MIgetSituation.Invoke(mse, new object[] { });
		}



		public string getBiome(ModuleScienceExperiment mse, ExperimentSituations sit)
		{
			if (_MIgetBiome == null)
				return "";
			return (string)_MIgetBiome.Invoke(mse, new object[] { sit });
		}
	}
}
