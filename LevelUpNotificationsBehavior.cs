using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace LevelUpNotifications
{
    public class LevelUpNotificationsBehavior : DefaultNotificationsCampaignBehavior
    {
        public override void RegisterEvents() => CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, new Action<Hero, bool>(SetCharacterNotification));
        // Display the character notification when the player or player's companions level up.
        private void SetCharacterNotification(Hero hero, bool shouldNotify = true)
        {
            if (hero == Hero.MainHero || hero.Clan == Clan.PlayerClan)
            {
                typeof(PlayerUpdateTracker).GetProperty("IsCharacterNotificationActive").SetValue(PlayerUpdateTracker.Current, true);
            }
        }
    }
}
