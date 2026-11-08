using System;
using System.Reflection;
using UnityEngine;



namespace ScienceChecklist
{
	/// <summary>
	/// Soft dependency on TimeForScience, which makes experiments take time. Reads its public API via reflection,
	/// so [x] Science! neither needs TimeForScience to build nor copies its timing formula.
	/// </summary>
	internal static class TimeForScienceBridge
	{
		private const string ApiTypeName = "TimeForScience.TimeForScienceAPI";
		private const int SupportedApiVersion = 1;

		private static bool _initialized;
		private static Func<ScienceExperiment, ScienceSubject, ExperimentSituations, double> _estimateSeconds;
		private static Func<double, string> _formatDuration;



		/// <summary>
		/// True when a compatible TimeForScience is installed.
		/// </summary>
		public static bool IsInstalled
		{
			get
			{
				Initialize( );
				return _estimateSeconds != null;
			}
		}



		/// <summary>
		/// Formatted duration of the next run of this experiment (e.g. "1m 39s"), or null when the experiment is
		/// instantaneous or TimeForScience is not installed.
		/// </summary>
		public static string GetDurationText( ScienceInstance exp )
		{
			if( !IsInstalled || exp.ScienceExperiment == null || exp.ScienceSubject == null )
				return null;

			try
			{
				double seconds = _estimateSeconds( exp.ScienceExperiment, exp.ScienceSubject, exp.Situation.ExperimentSituation );
				return seconds > 0 ? _formatDuration( seconds ) : null;
			}
			catch( Exception e )
			{
				Debug.LogError( "[x] Science!: TimeForScience API call failed, disabling it: " + e );
				_estimateSeconds = null;
				return null;
			}
		}



		private static void Initialize( )
		{
			if( _initialized )
				return;
			_initialized = true;

			try
			{
				Type api = null;
				foreach( var loaded in AssemblyLoader.loadedAssemblies )
				{
					if( loaded.name == "TimeForScience" )
					{
						api = loaded.assembly.GetType( ApiTypeName );
						break;
					}
				}
				if( api == null )
					return;

				FieldInfo version = api.GetField( "ApiVersion", BindingFlags.Public | BindingFlags.Static );
				if( version == null || (int)version.GetRawConstantValue( ) != SupportedApiVersion )
				{
					Debug.LogWarning( "[x] Science!: unsupported TimeForScience API version, durations not shown" );
					return;
				}

				MethodInfo estimate = api.GetMethod( "EstimateSeconds", BindingFlags.Public | BindingFlags.Static, null,
					new[ ] { typeof( ScienceExperiment ), typeof( ScienceSubject ), typeof( ExperimentSituations ) }, null );
				MethodInfo format = api.GetMethod( "FormatDuration", BindingFlags.Public | BindingFlags.Static, null,
					new[ ] { typeof( double ) }, null );
				if( estimate == null || format == null )
					return;

				_formatDuration = (Func<double, string>)Delegate.CreateDelegate( typeof( Func<double, string> ), format );
				_estimateSeconds = (Func<ScienceExperiment, ScienceSubject, ExperimentSituations, double>)Delegate.CreateDelegate(
					typeof( Func<ScienceExperiment, ScienceSubject, ExperimentSituations, double> ), estimate );
				Debug.Log( "[x] Science!: TimeForScience API found, showing experiment durations" );
			}
			catch( Exception e )
			{
				Debug.LogError( "[x] Science!: failed to bind the TimeForScience API: " + e );
				_estimateSeconds = null;
				_formatDuration = null;
			}
		}
	}
}
