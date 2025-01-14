using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using BlessingOfRegrowth.Common;


namespace BlessingOfRegrowth.Content.BuilderToggles {
   public class RegrowthMode : BuilderToggle {
      public static LocalizedText OnText { get; private set; }
      public static LocalizedText OffText { get; private set; }

      public override string HoverTexture => Texture;

      public override void SetStaticDefaults() {
         OnText = this.GetLocalization(nameof(OnText));
         OffText = this.GetLocalization(nameof(OffText));
      }

      public override string DisplayValue() {
         if (CurrentState == 0) return OnText.Value;
         else return OffText.Value;
      }

      public override bool Active() {
         return Main.LocalPlayer.GetModPlayer<BlessingOfRegrowthPlayer>().unlockedRegrowthMode;
      }

      public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
         drawParams.Frame = drawParams.Texture.Frame(4, 1, CurrentState);
         drawParams.Scale = 0.7f;

         return true;
      }

      public override bool DrawHover(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
         drawParams.Frame = drawParams.Texture.Frame(4, 1, CurrentState + 2);
         drawParams.Scale = 0.7f;

         return true;
      }
   }
}