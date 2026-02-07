using KSP.Localization;
using System.Text.RegularExpressions;



#region NO_LOCALIZATION

namespace ScienceChecklist {
	/// <summary>
	/// A location in which experiments may be conducted.
	/// Many experiments may, perhaps, be conducted.  Those create "ScienceInstance"s
	/// </summary>
	public sealed class Situation {
		/// <summary>
		/// Creates a new instance of the Situation class.
		/// </summary>
		/// <param name="body">The CelestialBody around which this situation is for.</param>
		/// <param name="situation">The ExperimentSituations flag which this situation is for.</param>
		/// <param name="biome">Optionally, the biome which this situation is for.</param>
		/// <param name="subBiome">Optionally, the KSC biome which this situation is for.</param>
		public Situation( Body body, ExperimentSituations situation, string biome = null, string subBiome = null ) {
			_body = body;
			_situation = situation;
			_biome = biome;
			_subBiome = subBiome;
			_formattedBiome = BiomeToString(body.CelestialBody, _biome);
			_formattedSubBiome = BiomeToString(body.CelestialBody, _subBiome);
			_description = FormatDescription(
				ToString(_situation),
				body.CelestialBody.displayName,
				_formattedBiome,
				_formattedSubBiome);
		}

        #endregion













        /// <summary>
        /// Gets the CelestialBody this situation is for.
        /// </summary>
        public Body        Body                { get { return _body; } }
		/// <summary>
		/// Gets the ExperimentSituations this situation is for.
		/// </summary>
		public ExperimentSituations ExperimentSituation { get { return _situation; } }
		/// <summary>
		/// Gets the biome this situation is for.
		/// </summary>
		public string               Biome               { get { return _biome; } }
		/// <summary>
		/// Gets the KSC biome this situation is for.
		/// </summary>
		public string               SubBiome            { get { return _subBiome; } }
		/// <summary>
		/// Gets the human-readable biome this situation is for.
		/// </summary>
		public string               FormattedBiome      { get { return _formattedBiome; } }
		/// <summary>
		/// Gets the human-readable KSC biome this situation is for.
		/// </summary>
		public string               FormattedSubBiome   { get { return _formattedSubBiome; } }
		/// <summary>
		/// Gets the human-readable description for this situation.
		/// </summary>
		public string               Description         { get { return _description; } }

		

		/// <summary>
		/// Converts an ExperimentSituations to a human-readable representation.
		/// </summary>
		/// <param name="situation">The ExperimentSituations to be converted.</param>
		/// <returns>The human-readable form of the ExperimentSituations.</returns>
		private string ToString ( ExperimentSituations situation )
		{
			switch (situation) {
				case ExperimentSituations.FlyingHigh:
					return Localizer.Format("#LOC_xSci_212");
				case ExperimentSituations.FlyingLow:
					return Localizer.Format("#LOC_xSci_213");
				case ExperimentSituations.InSpaceHigh:
					return Localizer.Format("#LOC_xSci_214");
				case ExperimentSituations.InSpaceLow:
					return Localizer.Format("#LOC_xSci_215");
				case ExperimentSituations.SrfLanded:
					return Localizer.Format("#LOC_xSci_216");
				case ExperimentSituations.SrfSplashed:
					return Localizer.Format("#LOC_xSci_217");
				default:
					return situation.ToString();
			}
		}

        /// <summary>
        /// Converts a biome to a human-readable representation.
        /// </summary>
        /// <param name="biome">The biome to be converted.</param>
        /// <returns>The human-readable form of the biome.</returns>
        private string BiomeToString(CelestialBody body, string biome)
		{
			if (string.IsNullOrEmpty(biome))
            {
				return string.Empty;
            }
			string stringBiome = biome.Replace("  ", " ").Trim();
			if (!string.IsNullOrEmpty(stringBiome))
			{
				stringBiome = ScienceUtil.GetBiomedisplayName(body, stringBiome);
			}
			return stringBiome;
		}

		private string FormatDescription(string situation, string body, string biome, string subbiome)
		{
			if (string.IsNullOrEmpty(biome))
				return Localizer.Format("#LOC_xSci_306", situation, body);
			if (string.IsNullOrEmpty(subbiome))
				return Localizer.Format("#LOC_xSci_307", situation, body, biome);
			return Localizer.Format("#LOC_xSci_308", situation, body, biome, subbiome);
		}

		internal readonly Body				  _body;
		internal readonly ExperimentSituations _situation;
		private readonly string               _biome;
		private readonly string               _subBiome;
		private readonly string               _formattedBiome;
		private readonly string               _formattedSubBiome;
		private readonly string               _description;
	}
}
