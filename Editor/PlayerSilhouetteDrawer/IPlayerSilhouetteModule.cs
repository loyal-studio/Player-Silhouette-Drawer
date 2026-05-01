using System.Collections.Generic;

namespace Daisen.Editor
{
    public interface IPlayerSilhouetteModule
    {
        string ModuleName { get; }
        void UpdateCache(IReadOnlyList<PlayerSilhouetteTarget> targets, PlayerSilhouetteSettings settings);
        void Draw(PlayerSilhouetteTarget target, PlayerSilhouetteSettings settings);
        void DrawSettingsGUI();
    }
}
