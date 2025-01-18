// Imports
using BlessingOfRegrowth.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;


namespace BlessingOfRegrowth.Content.BuilderToggles {
   public class RegrowthMode : BuilderToggle {
      // Localized text for the toggle states
      public static LocalizedText OnText { get; private set; }
      public static LocalizedText OffText { get; private set; }

      public override string HoverTexture => Texture;

      public override void SetStaticDefaults() {
         OnText = this.GetLocalization(nameof(OnText));
         OffText = this.GetLocalization(nameof(OffText));
      }

      // Display the current state of the toggle
      public override string DisplayValue() {
         if (CurrentState == 0) return OnText.Value;
         else return OffText.Value;
      }

      // Check if the toggle is active
      public override bool Active() {
         return Main.LocalPlayer.GetModPlayer<BlessingOfRegrowthPlayer>().unlockedRegrowthMode;
      }

      // Draw the toggle
      public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
         drawParams.Frame = drawParams.Texture.Frame(4, 1, CurrentState);
         drawParams.Scale = 0.7f;

         return true;
      }

      // Draw the hover state of the toggle
      public override bool DrawHover(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
         drawParams.Frame = drawParams.Texture.Frame(4, 1, CurrentState + 2);
         drawParams.Scale = 0.7f;

         return true;
      }

      // Handle left-click on the toggle
      public override bool OnLeftClick(ref SoundStyle? sound) {
         Main.LocalPlayer.GetModPlayer<BlessingOfRegrowthPlayer>().usingRegrowthMode = CurrentState != 0;

         return base.OnLeftClick(ref sound);
      }
   }
}