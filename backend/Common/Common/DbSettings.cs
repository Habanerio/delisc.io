using Common.Configurations;

namespace Common;

public class DbSettings : AppSettingsSection
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    /// <value>
    /// The connection string.
    /// </value>
    public string ConnectionString { get; set; } = string.Empty;
}