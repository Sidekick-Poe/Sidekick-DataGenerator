using Sidekick.Data.Game.StatDescriptions;

namespace Sidekick.Data.Modifiers;

public class ModifierProvider
{
    private readonly StatDescriptionProvider statDescriptionProvider;

    public ModifierProvider(
        StatDescriptionProvider statDescriptionProvider)
    {
        this.statDescriptionProvider = statDescriptionProvider;
    }
}
