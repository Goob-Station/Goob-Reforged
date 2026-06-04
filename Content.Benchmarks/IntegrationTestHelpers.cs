using Robust.Shared;

namespace Content.Benchmarks;

public static class IntegrationTestHelpers
{
    // TODO this is a super dumb hack in order to be able to launch from /Modules folder
    // But it works somehow...
    public static void ChangeRootDir(string dir)
    {
        ProgramShared.PathOffset = dir;
    }
}
