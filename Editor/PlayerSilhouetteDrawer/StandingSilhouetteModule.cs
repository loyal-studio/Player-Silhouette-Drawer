using System.Collections.Generic;

namespace Daisen.Editor
{
    public sealed class StandingSilhouetteModule : IPlayerSilhouetteModule
    {
        public void UpdateCache(IReadOnlyList<PlayerSilhouetteTarget> targets, PlayerSilhouetteSettings settings)
        {
        }

        public void Draw(PlayerSilhouetteTarget target, PlayerSilhouetteSettings settings)
        {
            SilhouetteDrawerUtility.DrawSilhouette3D(
                target.FeetPosition, target.Rotation, settings, target.StandingHeight, settings.standingColor,
                settings.shoulderWidth, settings.hipWidth, settings.waistWidth, settings.headRadius,
                false);
        }
    }
}
