using BlessingOfRegrowth.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BlessingOfRegrowth.Content.Items {
   public class JunglesBlessing : ModItem {
      public override void SetDefaults() {
         Item.CloneDefaults(ItemID.TorchGodsFavor);
      }

      public override bool? UseItem(Player player) {
         if (player.GetModPlayer<BlessingOfRegrowthPlayer>().unlockedRegrowthMode) return false;

         player.GetModPlayer<BlessingOfRegrowthPlayer>().unlockedRegrowthMode = true;

         return true;
      }
   }
}