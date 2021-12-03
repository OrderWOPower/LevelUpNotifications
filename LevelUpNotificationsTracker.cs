using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using SandBox.View.Map;

namespace LevelUpNotifications
{
    public class LevelUpNotificationsTracker : PlayerUpdateTracker
    {
        // Display the party notification when the player's troops level up.
        // Display a notification message when the player requires horses to upgrade troops.
        public void SetPartyNotification()
        {
            if (ScreenManager.TopScreen is MapScreen)
            {
                int requiredHorses = 0;
                int requiredWarHorses = 0;
                TextObject horseTextObject = DefaultItemCategories.Horse.GetName();
                TextObject warHorseTextObject = DefaultItemCategories.WarHorse.GetName();
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
                        if (characterObject.UpgradeRequiresItemFromCategory == DefaultItemCategories.Horse)
                        {
                            requiredHorses += val3;
                            break;
                        }
                        else if (characterObject.UpgradeRequiresItemFromCategory == DefaultItemCategories.WarHorse)
                        {
                            requiredWarHorses += val3;
                            break;
                        }
                    }
                    if (num > 0 && !Current.IsPartyNotificationActive)
                    {
                        typeof(PlayerUpdateTracker).GetProperty("IsPartyNotificationActive").SetValue(Current, true);
                    }
                }
                requiredHorses -= Current.GetNumOfCategoryItemPartyHas(MobileParty.MainParty.ItemRoster, DefaultItemCategories.Horse);
                requiredWarHorses -= Current.GetNumOfCategoryItemPartyHas(MobileParty.MainParty.ItemRoster, DefaultItemCategories.WarHorse);
                if ((requiredHorses > 0 || requiredWarHorses > 0) && !_hasNotifiedHorsesRequired)
                {
                    MBTextManager.SetTextVariable("REQUIRED_HORSES", requiredHorses);
                    MBTextManager.SetTextVariable("REQUIRED_WAR_HORSES", requiredWarHorses);
                    MBTextManager.SetTextVariable("HORSE", horseTextObject);
                    MBTextManager.SetTextVariable("WAR_HORSE", warHorseTextObject);
                    if (requiredHorses > 0 && requiredWarHorses <= 0)
                    {
                        InformationManager.AddQuickInformation(new TextObject("You require {REQUIRED_HORSES} {?(REQUIRED_HORSES > 1)}{PLURAL(HORSE)}{?}{HORSE}{\\?} to upgrade your troops.", null), 0, null, "");
                    }
                    else if (requiredHorses <= 0 && requiredWarHorses > 0)
                    {
                        InformationManager.AddQuickInformation(new TextObject("You require {REQUIRED_WAR_HORSES} {?(REQUIRED_WAR_HORSES > 1)}{PLURAL(WAR_HORSE)}{?}{WAR_HORSE}{\\?} to upgrade your troops.", null), 0, null, "");
                    }
                    else if (requiredHorses > 0 && requiredWarHorses > 0)
                    {
                        InformationManager.AddQuickInformation(new TextObject("You require {REQUIRED_HORSES} {?(REQUIRED_HORSES > 1)}{PLURAL(HORSE)}{?}{HORSE}{\\?} and {REQUIRED_WAR_HORSES} {?(REQUIRED_WAR_HORSES > 1)}{PLURAL(WAR_HORSE)}{?}{WAR_HORSE}{\\?} to upgrade your troops.", null), 0, null, "");
                    }
                    _hasNotifiedHorsesRequired = true;
                }
            }
            else
            {
                _hasNotifiedHorsesRequired = false;
            }
        }
        private bool _hasNotifiedHorsesRequired;
    }
}
