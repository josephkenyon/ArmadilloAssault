using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Update;
using Newtonsoft.Json;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class BattleUpdate : ISoundFrameContainer
    {
        [JsonProperty("C", NullValueHandling = NullValueHandling.Ignore)]
        public CrateUpdate CrateUpdate { get; set; }
        
        [JsonProperty("SF", NullValueHandling = NullValueHandling.Ignore)]
        public SoundFrame SoundFrame { get; set; }

        internal void CrateCreated(Crate crate)
        {
            CrateUpdate ??= new CrateUpdate();

            CrateUpdate.NewTypes ??= [];
            CrateUpdate.NewTypes.Add(crate.Type);

            CrateUpdate.NewXs ??= [];
            CrateUpdate.NewXs.Add(crate.Position.X);

            CrateUpdate.NewFinalYs ??= [];
            CrateUpdate.NewFinalYs.Add(crate.FinalY);

            CrateUpdate.NewGoingDowns ??= [];
            CrateUpdate.NewGoingDowns.Add(crate.GoingDown);
        }

        internal void CrateDeleted(int id)
        {
            CrateUpdate ??= new CrateUpdate();

            CrateUpdate.DeletedIds ??= [];
            CrateUpdate.DeletedIds.Add(id);
        }
    }
}
