using OxyPlot.Series;

namespace APSIM.Shared.Graphing;

/// <summary>
/// A Nameable ScatterErrorSeries.
/// </summary>
public class NamedScatterErrorSeries: ScatterErrorSeries, INameableSeries
{
    /// <summary>
    /// Name of NamedScatterErrorSeries.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name"></param>
    public NamedScatterErrorSeries(string name): base()
    {
        Name = name;
    }
}