using ArmadilloAssault.GameState.Battle.Crates;
using ArmadilloAssault.GameState.Battle.Effects;
using ArmadilloAssault.Generics;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Communication.Update;
using Newtonsoft.Json;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class BattleUpdate : ISoundFrameContainer
    {
        [JsonProperty("C", NullValueHandling = NullValueHandling.Ignore)]
        public CrateUpdate CrateUpdate { get; set; }

        [JsonProperty("E", NullValueHandling = NullValueHandling.Ignore)]
        public EffectUpdate EffectUpdate { get; set; }

        [JsonProperty("S", NullValueHandling = NullValueHandling.Ignore)]
        public SoundFrame SoundFrame { get; set; }

        [JsonProperty("St", NullValueHandling = NullValueHandling.Ignore)]
        public StatFrame StatFrame { get; set; }

        public void CrateCreated(Crate crate)
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

        public void CrateDeleted(int id)
        {
            CrateUpdate ??= new CrateUpdate();

            CrateUpdate.DeletedIds ??= [];
            CrateUpdate.DeletedIds.Add(id);
        }

        public void EffectCreated(Effect effect)
        {
            EffectUpdate ??= new EffectUpdate();

            EffectUpdate.NewTypes ??= [];
            EffectUpdate.NewTypes.Add(effect.Type);

            EffectUpdate.NewXs ??= [];
            EffectUpdate.NewXs.Add(effect.Position.X);

            EffectUpdate.NewYs ??= [];
            EffectUpdate.NewYs.Add(effect.Position.Y);

            EffectUpdate.NewDirectionLefts ??= [];
            EffectUpdate.NewDirectionLefts.Add(effect.Direction == Direction.Left);
        }
    }
}
