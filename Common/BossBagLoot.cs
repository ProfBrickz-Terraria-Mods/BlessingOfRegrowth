// Imports
using BlessingOfRegrowth.Content.Items;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace BlessingOfRegrowth.Common {
   // Condition to check if Jungle's Blessing has not been used
   public class NotUsedJunglesBlessing : IItemDropRuleCondition {
      public bool CanDrop(DropAttemptInfo info) {
         return !info.player.GetModPlayer<BlessingOfRegrowthPlayer>().unlockedRegrowthMode;
      }

      public bool CanShowItemDropInUI() => true;
      public string GetConditionDescription() => "Not Used Jungles Blessing";
   }

   public class BossBagLoot : GlobalItem {
      // Modify the loot of the boss bag
      public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
         if (item.type == ItemID.PlanteraBossBag) {
            itemLoot.Add(ItemDropRule.ByCondition(new NotUsedJunglesBlessing(), ModContent.ItemType<JunglesBlessing>()));
         }
      }
   }
}