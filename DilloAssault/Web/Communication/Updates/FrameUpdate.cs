using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Updates
{
    public class FrameUpdate
    {
        public List<AvatarUpdate> AvatarUpdates { get; set; }
        public EffectsUpdate EffectsUpdate { get; set; }
        public BulletsUpdate BulletsUpdate { get; set; }
        public CratesUpdate CratesUpdate { get; set; }
    }
}
