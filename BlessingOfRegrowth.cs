using System;
using System.Reflection;
using BlessingOfRegrowth.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace BlessingOfRegrowth {
   public class BlessingOfRegrowth : Mod {
      private DateTime plantBreakDisableTimer = DateTime.Now;
      private const int plantBreakDisableTime = 5;
      private Mod regrowthReplant = null;

      private ushort[] validTreeTypes =  {
         TileID.Trees,
         TileID.PalmTree,
         TileID.VanityTreeSakura,
         TileID.VanityTreeYellowWillow,
         TileID.TreeAsh
      };
      private ushort[] gemcornTrees = {
         TileID.TreeTopaz,
         TileID.TreeAmethyst,
         TileID.TreeSapphire,
         TileID.TreeEmerald,
         TileID.TreeRuby,
         TileID.TreeDiamond,
         TileID.TreeAmber
      };

      public override void Load() {
         // Hook into the mining tool usage event
         On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += BreakTile;
      }

      private void BreakTile(
         On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig,
         Player player,
         Item sItem,
         out bool canHitWalls,
         int x,
         int y
      ) {
         // Check if the player has unlocked and is using Regrowth Mode, and is not holding the Axe of Regrowth
         if (
            !player.GetModPlayer<BlessingOfRegrowthPlayer>().unlockedRegrowthMode ||
            !player.GetModPlayer<BlessingOfRegrowthPlayer>().usingRegrowthMode ||
            player.HeldItem.type == ItemID.AcornAxe
         ) {
            orig.Invoke(player, sItem, out canHitWalls, x, y);
            return;
         }

         if (ModLoader.HasMod("RegrowthReplant")) {
            regrowthReplant = ModLoader.GetMod("RegrowthReplant");

            if (BreakHerb(orig, player, sItem, out canHitWalls, x, y)) return;
            if (BreakGemcorn(orig, player, sItem, out canHitWalls, x, y)) return;
         }

         if (BreakTree(orig, player, sItem, out canHitWalls, x, y)) return;

         orig.Invoke(player, sItem, out canHitWalls, x, y);
      }

      private bool BreakTree(
         On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig,
         Player player,
         Item sItem,
         out bool canHitWalls,
         int treeX,
         int treeY
      ) {
         bool isTree = false;
         canHitWalls = true;

         // Check if the player is holding an axe
         if (player.HeldItem.axe <= 0) return isTree;

         Tile treeTile = Main.tile[treeX, treeY];

         if (Array.Exists(validTreeTypes, type => type == treeTile.TileType)) isTree = true;
         else return isTree;

         // Get the bottom of the tree
         WorldGen.GetTreeBottom(treeX, treeY, out int groundX, out int groundY);

         // Prevent breaking saplings if the timer is active
         if (plantBreakDisableTimer > DateTime.Now && treeTile.TileType == TileID.Saplings) {
            canHitWalls = false;
            return isTree;
         }

         // Check if the tile is not the bottom of the tree or the tile above is not a tree
         if (
            treeX != groundX || treeY != groundY - 1 ||
            !Array.Exists(validTreeTypes, type => Main.tile[treeX, treeY - 1].TileType == type)
         ) {
            return isTree;
         }

         orig.Invoke(player, sItem, out canHitWalls, treeX, treeY);

         // Return if the tree was not broken
         if (treeTile.TileType != TileID.Dirt) return true;

         // Place a sapling at the tree's position
         if (!TileObject.CanPlace(
            treeX,
            treeY,
            TileID.Saplings,
            treeTile.TileFrameX / 18,
            player.direction,
            out TileObject tileData
         )) return isTree;
         bool placed = TileObject.Place(tileData);

         // If a sapling was placed, set the timer to disable breaking it
         if (placed) {
            plantBreakDisableTimer = DateTime.Now.AddSeconds(plantBreakDisableTime);
         }

         isTree = true;
         return isTree;
      }

      private bool BreakHerb(
         On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig,
         Player player,
         Item sItem,
         out bool canHitWalls,
         int x,
         int y
      ) {
         bool isHerb = false;
         canHitWalls = true;

         // Check if the player is holding a pickaxe
         if (player.HeldItem.pick <= 0) return isHerb;

         // Get the ReplantHerbs config value from Regrowth Replant
         bool ReplantHerbs = GetRegrowReplantConfig("ReplantHerbs");

         // Check if Regrowth Replant is enabled
         if (!ReplantHerbs) return isHerb;

         Tile tile = Main.tile[x, y];

         // Prevent breaking herbs if the timer is active
         if (plantBreakDisableTimer > DateTime.Now && tile.TileType == TileID.ImmatureHerbs) {
            canHitWalls = false;
            return isHerb;
         }

         // Check if the tile is not a herb
         if (
            tile.TileType != TileID.ImmatureHerbs &&
            tile.TileType != TileID.MatureHerbs &&
            tile.TileType != TileID.BloomingHerbs
         ) return isHerb;

         int herbStyle = tile.TileFrameX / 18;

         orig.Invoke(player, sItem, out canHitWalls, x, y);

         // Return if the herb was not broken
         if (tile.TileType != TileID.Dirt) return isHerb;

         // Place a herb at the tree's position
         WorldGen.PlaceTile(x, y, TileID.ImmatureHerbs, style: herbStyle);
         if (!TileObject.CanPlace(
            x,
            y,
            TileID.ImmatureHerbs,
            herbStyle,
            player.direction,
            out TileObject objectData
         )) return isHerb;
         bool placed = TileObject.Place(objectData);

         // If a herb was placed, set the timer to disable breaking it
         if (placed) {
            plantBreakDisableTimer = DateTime.Now.AddSeconds(plantBreakDisableTime);
         }

         isHerb = true;
         return isHerb;
      }

      private bool BreakGemcorn(
         On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig,
         Player player,
         Item sItem,
         out bool canHitWalls,
         int gemcornX,
         int gemcornY
      ) {
         bool isGemcorn = false;
         canHitWalls = true;

         // Check if the player is holding an axe
         if (player.HeldItem.axe <= 0) return isGemcorn;

         // Get the ReplantGemcorn config value from Regrowth Replant
         bool ReplantGemcorn = GetRegrowReplantConfig("ReplantGemcorns");

         // Check if Regrowth Replant is enabled
         if (!ReplantGemcorn) return isGemcorn;

         Tile gemcornTile = Main.tile[gemcornX, gemcornY];

         // Check if the tile is not a gemcorn
         if (Array.Exists(gemcornTrees, style => gemcornTile.TileType == style)) isGemcorn = true;
         else return isGemcorn;

         WorldGen.GetTreeBottom(gemcornX, gemcornY, out int groundX, out int groundY);

         // Prevent breaking gemcorns if the timer is active
         if (plantBreakDisableTimer > DateTime.Now && gemcornTile.TileType == TileID.GemSaplings) {
            canHitWalls = false;
            return isGemcorn;
         }

         // Check if the tile is not the bottom of the tree or the tile above is not a gemcorn
         if (
            gemcornX != groundX || gemcornY != groundY - 1 ||
            !Array.Exists(gemcornTrees, style => Main.tile[gemcornX, gemcornY - 1].TileType == style)
         ) {
            return isGemcorn;
         }

         ushort gemcornType = (ushort)(gemcornTile.TileType - TileID.TreeTopaz);

         orig.Invoke(player, sItem, out canHitWalls, gemcornX, gemcornY);

         // Return if the gemcorn was not broken
         if (gemcornTile.TileType != TileID.Dirt) return true;

         bool useGemcorn = GetRegrowReplantConfig("AxeUseSeedFromInventory");

         if (useGemcorn) {
            if (!player.ConsumeItem(ItemID.GemTreeTopazSeed + gemcornType)) {
               isGemcorn = true;
               return isGemcorn;
            }
         }

         // Place a gemcorn at the tree's position
         if (!TileObject.CanPlace(
            gemcornX,
            gemcornY,
            TileID.GemSaplings,
            gemcornType,
            player.direction,
            out TileObject objectData
         )) return isGemcorn;
         bool placed = TileObject.Place(objectData);

         // If a gemcorn was placed, set the timer to disable breaking it
         if (placed) {
            plantBreakDisableTimer = DateTime.Now.AddSeconds(plantBreakDisableTime);
         }

         isGemcorn = true;
         return isGemcorn;
      }

      private bool GetRegrowReplantConfig(string configName) {
         FieldInfo configFieldInfo = regrowthReplant.Code.GetType("RegrowthReplant.RegrowthReplantServerConfig").GetField(configName);
         return (bool)configFieldInfo.GetValue(regrowthReplant.GetConfig("RegrowthReplantServerConfig"));
      }
   }
}
