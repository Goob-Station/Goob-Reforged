namespace Content.Goobstation.Shared.Genetics.Types;

public record struct MutationStatus(
    bool HasMutation,
    bool HasMutationDormant
);
