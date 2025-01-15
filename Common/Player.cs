using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace BlessingOfRegrowth.Common {
   public class BlessingOfRegrowthPlayer : ModPlayer {
      public bool unlockedRegrowthMode = false;

      public override void SaveData(TagCompound tag) {
         tag["unlockedRegrowthMode"] = unlockedRegrowthMode;
      }

      public override void LoadData(TagCompound tag) {
         unlockedRegrowthMode = tag.GetBool("unlockedRegrowthMode");
      }
   }
}