using System;
using Models.Core;
using Models.Functions;
using System.IO;
using System.Xml.Serialization;

namespace Models.PMF.Phen
{
    /// <summary>
    /// A special phase that jumps to another phase.
    /// </summary>
    [Serializable]
    [ViewName("UserInterface.Views.GridView")]
    [PresenterName("UserInterface.Presenters.PropertyPresenter")]
    public class GotoPhase : Model, IPhase
    {
        // 1. Links
        //----------------------------------------------------------------------------------------------------------------

        [Link]
        private Phenology phenology = null;


        //5. Public properties
        //-----------------------------------------------------------------------------------------------------------------
        /// <summary>The start</summary>
        [Description("Start")]
        public string Start { get; set; }

        /// <summary>The end</summary>
        [Description("End")]
        public string End { get; set; }

        /// <summary>The phase name to goto</summary>
        [Description("PhaseNameToGoto")]
        public string PhaseNameToGoto { get; set; }

        /// <summary>Gets the tt for today.</summary>
        [XmlIgnore]
        public double TTForTimeStep { get; set; }

        /// <summary>Gets the tt in phase.</summary>
        [XmlIgnore]
        public double TTinPhase { get; set; }

        /// <summary>Gets the fraction complete.</summary>
        [XmlIgnore]
        public double FractionComplete { get; set; }


        /// <summary>Thermal time target</summary>
        [XmlIgnore]
        public double Target { get; set; }

        //6. Public methods
        //-----------------------------------------------------------------------------------------------------------------

        /// <summary>Should not be called in this class</summary>
        public bool DoTimeStep(ref double PropOfDayToUse)
        {
            PropOfDayToUse = 0;
            phenology.ReSetToStage((double)phenology.IndexOfPhase(PhaseNameToGoto)+1,false);
            return false;
        }

        /// <summary>Writes the summary.</summary>
        public void WriteSummary(TextWriter writer) { writer.WriteLine("      " + Name); }

        /// <summary>Resets the phase.</summary>
        public virtual void ResetPhase() {}
    }
}
