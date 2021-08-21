using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using SandBox.GauntletUI;

namespace LevelUpNotifications
{
    public class LevelUpNotificationsTracker : PlayerUpdateTracker
    {
        // Display the party notification when the player's troops level up.
        public void SetPartyNotification()
        {
            if (!Current.IsPartyNotificationActive && !(ScreenManager.TopScreen is GauntletPartyScreen))
            {
                foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
                {
                    int num = 0;
                    for (int i = 0; i < troopRosterElement.Character.UpgradeTargets?.Length; i++)
                    {
                        CharacterObject characterObject = troopRosterElement.Character.UpgradeTargets[i];
                        int num2 = troopRosterElement.Character.GetUpgradeGoldCost(PartyBase.MainParty, i);
                        int val = troopRosterElement.Number;
                        int numOfCategoryItemPartyHas = Current.GetNumOfCategoryItemPartyHas(MobileParty.MainParty.ItemRoster, characterObject.UpgradeRequiresItemFromCategory);
                        if (num2 > 0)
                        {
                            val = (int)MathF.Clamp((float)Math.Floor(Hero.MainHero.Gold / (float)num2), 0f, troopRosterElement.Number);
                        }
                        int val2 = (characterObject.UpgradeRequiresItemFromCategory != null) ? numOfCategoryItemPartyHas : troopRosterElement.Number;
                        int val3 = (int)Math.Floor(troopRosterElement.Xp / (float)troopRosterElement.Character.GetUpgradeXpCost(PartyBase.MainParty, i));
                        num = Math.Max(Math.Min(Math.Min(val, val2), val3), num);
                    }
                    if (num > 0)
                    {
                        _isTroopUpgradable = true;
                        break;
                    }
                    else
                    {
                        _isTroopUpgradable = false;
                    }
                }
                if (_isTroopUpgradable)
                {
                    typeof(PlayerUpdateTracker).GetProperty("IsPartyNotificationActive").SetValue(Current, true);
                }
            }
        }
        private bool _isTroopUpgradable;
    }
}
