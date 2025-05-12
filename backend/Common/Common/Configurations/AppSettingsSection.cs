using Microsoft.Extensions.Options;

namespace Common.Configurations;

public abstract class AppSettingsSection
{
    public string Environment { get; set; } = string.Empty;

    public static TSection? Get<TSection>() where TSection : AppSettingsSection
    {
        return AppSettingsHelper.GetSettings<TSection>();
    }

    public IOptions<TSection> AsIOptions<TSection>() where TSection : AppSettingsSection
    {
        return (IOptions<TSection>)Options.Create(this);
    }
}