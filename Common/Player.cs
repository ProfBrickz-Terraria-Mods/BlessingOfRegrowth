// Imports
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace BlessingOfRegrowth.Common {
   public class BlessingOfRegrowthPlayer : ModPlayer {
      // Fields to store the state of Regrowth Mode
      public bool unlockedRegrowthMode = false;
      public bool usingRegrowthMode = true;

      // Save the state of Regrowth Mode
      public override void SaveData(TagCompound tag) {
         tag["unlockedRegrowthMode"] = unlockedRegrowthMode;
         tag["usingRegrowthMode"] = usingRegrowthMode;
      }

      // Load the state of Regrowth Mode
      public override void LoadData(TagCompound tag) {
         unlockedRegrowthMode = tag.GetBool("unlockedRegrowthMode");
         usingRegrowthMode = tag.GetBool("usingRegrowthMode");
      }
   }
}