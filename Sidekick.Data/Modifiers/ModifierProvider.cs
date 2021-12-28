using Sidekick.Data.Game.StatDescriptions;

namespace Sidekick.Data.Modifiers;

public class ModifierProvider
{
    private readonly StatDescriptionProvider statDescriptionProvider1;

    public ModifierProvider(
        StatDescriptionProvider statDescriptionProvider1)
    {
        this.statDescriptionProvider1 = statDescriptionProvider1;
    }
}
