// Imports
using System;
using BlessingOfRegrowth.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace BlessingOfRegrowth {
   public class BlessingOfRegrowth : Mod {
      // Timer to disable sapling breaking
      private DateTime saplingBreakDisableTimer = DateTime.Now;
      // Time duration to disable sapling breaking in seconds
      private const int saplingBreakDisableTime = 5;

      public override void Load() {
         // Hook into the mining tool usage event
         On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += BreakTile;
      }

      private void BreakTile(On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig, Player player, Item sItem, out bool canHitWalls, int treeX, int treeY) {
         // Check if the player has unlocked and is using Regrowth Mode, and is not holding the Axe of Regrowth
         if (
            !player.GetModPlayer<BlessingOfRegrowthPlayer>().unlockedRegrowthMode ||
            !player.GetModPlayer<BlessingOfRegrowthPlayer>().usingRegrowthMode ||
            player.HeldItem.type == ItemID.AcornAxe
         ) {
            orig.Invoke(player, sItem, out canHitWalls, treeX, treeY);
            return;
         }

         Tile treeTile = Main.tile[treeX, treeY];

         // Get the bottom of the tree
         WorldGen.GetTreeBottom(treeX, treeY, out int groundX, out int groundY);
         Tile groundTile = Main.tile[groundX, groundY];

         // Prevent breaking saplings if the timer is active
         if (saplingBreakDisableTimer > DateTime.Now && treeTile.TileType == TileID.Saplings) {
            canHitWalls = false;
            return;
         }

         // Check if the tile is not the bottom of the tree or the tile above it is not solid
         if (
            treeX != groundX || treeY != groundY - 1 ||
            Main.tile[treeX, treeY - 1].BlockType != BlockType.Solid
         ) {
            orig.Invoke(player, sItem, out canHitWalls, treeX, treeY);
            return;
         }

         orig.Invoke(player, sItem, out canHitWalls, treeX, treeY);

         // Return if the tree was not broken
         if (treeTile.TileType != TileID.Dirt) return;

         // Place a sapling at the tree's position
         WorldGen.PlaceTile(treeX, treeY, TileID.Saplings);

         // If a sapling was placed, set the timer to disable breaking it
         if (treeTile.TileType == TileID.Saplings) {
            saplingBreakDisableTimer = DateTime.Now.AddSeconds(saplingBreakDisableTime);
         }
      }
   }
}
