using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace LevelUpNotifications
{
    public class LevelUpNotificationsView : MapView
    {
        private bool _hasNotifiedHorsesRequired;

        protected override void OnResume() => _hasNotifiedHorsesRequired = false;

        protected override void OnMapScreenUpdate(float dt)
        {
            PartyBase mainParty = PartyBase.MainParty;
            ItemCategory horse = DefaultItemCategories.Horse, warHorse = DefaultItemCategories.WarHorse;
            TextObject horseTextObject = horse.GetName(), warHorseTextObject = warHorse.GetName();
            PlayerUpdateTracker playerUpdateTracker = PlayerUpdateTracker.Current;
            int numOfRequiredHorses = 0, numOfRequiredWarHorses = 0;

            foreach (TroopRosterElement troopRosterElement in mainParty.MemberRoster.GetTroopRoster())
            {
                CharacterObject currentCharacter = troopRosterElement.Character;
                int numOfUpgradeableTroops = 0;

                for (int i = 0; i < currentCharacter.UpgradeTargets?.Length; i++)
                {
                    CharacterObject targetCharacter = currentCharacter.UpgradeTargets[i];
                    ItemCategory upgradeRequiresItemFromCategory = targetCharacter.UpgradeRequiresItemFromCategory;
                    int numOfTroops = troopRosterElement.Number;
                    int troopXp = troopRosterElement.Xp;
                    int upgradeGoldCost = currentCharacter.GetUpgradeGoldCost(mainParty, i);
                    int upgradeXpCost = currentCharacter.GetUpgradeXpCost(mainParty, i);
                    int numOfTroopsWithGoldRequirementsMet = upgradeGoldCost > 0 ? (int)MathF.Clamp(Hero.MainHero.Gold / upgradeGoldCost, 0f, numOfTroops) : numOfTroops;
                    int numOfTroopsWithItemRequirementsMet = upgradeRequiresItemFromCategory != null ? playerUpdateTracker.GetNumOfCategoryItemPartyHas(mainParty.ItemRoster, upgradeRequiresItemFromCategory) : numOfTroops;
                    int numOfTroopsWithXpRequirementsMet = targetCharacter.Level >= currentCharacter.Level && troopXp >= upgradeXpCost ? (int)MathF.Clamp(troopXp / upgradeXpCost, 0f, numOfTroops) : 0;
                    int numOfTroopsWithPerkRequirementsMet = Campaign.Current.Models.PartyTroopUpgradeModel.DoesPartyHaveRequiredPerksForUpgrade(mainParty, currentCharacter, targetCharacter, out _) ? numOfTroops : 0;

                    numOfUpgradeableTroops = MathF.Min(MathF.Min(numOfTroopsWithGoldRequirementsMet, numOfTroopsWithItemRequirementsMet), MathF.Min(numOfTroopsWithXpRequirementsMet, numOfTroopsWithPerkRequirementsMet));

                    if (upgradeRequiresItemFromCategory == horse)
                    {
                        numOfRequiredHorses += numOfTroopsWithXpRequirementsMet;

                        break;
                    }
                    else if (upgradeRequiresItemFromCategory == warHorse)
                    {
                        numOfRequiredWarHorses += numOfTroopsWithXpRequirementsMet;

                        break;
                    }
                }

                if (numOfUpgradeableTroops > 0 && !playerUpdateTracker.IsPartyNotificationActive)
                {
                    // Display the party notification when the player's troops level up.
                    typeof(PlayerUpdateTracker).GetProperty("IsPartyNotificationActive").SetValue(playerUpdateTracker, true);
                }
            }

            numOfRequiredHorses -= playerUpdateTracker.GetNumOfCategoryItemPartyHas(mainParty.ItemRoster, horse);
            numOfRequiredWarHorses -= playerUpdateTracker.GetNumOfCategoryItemPartyHas(mainParty.ItemRoster, warHorse);

            if ((numOfRequiredHorses > 0 || numOfRequiredWarHorses > 0) && !_hasNotifiedHorsesRequired)
            {
                MBTextManager.SetTextVariable("REQUIRED_HORSES", numOfRequiredHorses);
                MBTextManager.SetTextVariable("REQUIRED_WAR_HORSES", numOfRequiredWarHorses);
                MBTextManager.SetTextVariable("HORSE", horseTextObject);
                MBTextManager.SetTextVariable("WAR_HORSE", warHorseTextObject);

                // Display a notification message when the player requires horses to upgrade troops.
                if (numOfRequiredHorses > 0 && numOfRequiredWarHorses <= 0)
                {
                    MBInformationManager.AddQuickInformation(new TextObject("You require {REQUIRED_HORSES} {?(REQUIRED_HORSES > 1)}{PLURAL(HORSE)}{?}{HORSE}{\\?} to upgrade your troops.", null), 0, null, "");
                }
                else if (numOfRequiredHorses <= 0 && numOfRequiredWarHorses > 0)
                {
                    MBInformationManager.AddQuickInformation(new TextObject("You require {REQUIRED_WAR_HORSES} {?(REQUIRED_WAR_HORSES > 1)}{PLURAL(WAR_HORSE)}{?}{WAR_HORSE}{\\?} to upgrade your troops.", null), 0, null, "");
                }
                else if (numOfRequiredHorses > 0 && numOfRequiredWarHorses > 0)
                {
                    MBInformationManager.AddQuickInformation(new TextObject("You require {REQUIRED_HORSES} {?(REQUIRED_HORSES > 1)}{PLURAL(HORSE)}{?}{HORSE}{\\?} and {REQUIRED_WAR_HORSES} {?(REQUIRED_WAR_HORSES > 1)}{PLURAL(WAR_HORSE)}{?}{WAR_HORSE}{\\?} to upgrade your troops.", null), 0, null, "");
                }

                _hasNotifiedHorsesRequired = true;
            }
        }
    }
}
