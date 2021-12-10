using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BixBite.Characters;
using BixBite.Combat;
using BixBite.Combat.Equipables;
using BixBite.Items;
using SQLite;

//Change Log////////////////////////////////////////////////////////////////////////////////////////
// VERSION 1.0.0.1: Fixed Skills Not showing correctly. And Erroring out if No modifier is linked.
// Added the Skill Function Pointer in the Edit Section of the skills tab. -AM
// VERSION 1.0.0.2: Fixed the skills and the Items sections. Added boxes and logic for the new 
// Area of Effect variable, as well as the new targeting boolean.
// VERSION 1.0.0.3: Added the function pointer string name variable to the GUI and queries.
// Fixed the name change text boxes for items and Skills to show the new variables for 1.0.0.2v & 1.0.0.3v
//Change Log////////////////////////////////////////////////////////////////////////////////////////



namespace DatabaseTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		#region Delegates

		#endregion

		#region Fields

		private SQLiteConnection _sqlite_conn;
		private String SQLDatabasePath = "";

		private List<int?> gameplayModifiersAdd_vals = new List<int?>(11);
		private List<int?> gameplayModifiersEdit_vals = new List<int?>(11);

		#endregion

		#region Properties

		public ObservableCollection<FollowUpAttack> CurrentFollowUpAttacksInDatabase { get; set; }

		public ObservableCollection<Job> CurrentJobsInDatabase { get; set; }

		public ObservableCollection<Weapon> CurrentWeaponsInDatabase { get; set; }

		public ObservableCollection<Skill> CurrentSkillsInDatabase { get; set; }

		public ObservableCollection<Item> CurrentItemsInDatabase { get; set; }

		public ObservableCollection<Accessory> CurrentAccessoriesInDatabase { get; set; }

		public ObservableCollection<Clothes> CurrentClothesInDatabase { get; set; }
		public ObservableCollection<Clothes> CurrentClothesInDatabase_Head { get; set; }
		public ObservableCollection<Clothes> CurrentClothesInDatabase_Body { get; set; }
		public ObservableCollection<Clothes> CurrentClothesInDatabase_Legs { get; set; }

		public ObservableCollection<ModifierData> CurrenGameplayModifiersInDatabase { get; set; }
		public ObservableCollection<ModifierData> CurrenGameplayModifiersInDatabase_Effects { get; set; }
		public ObservableCollection<ModifierData> CurrenGameplayModifiersInDatabase_Traits { get; set; }

		public ObservableCollection<party_member> CurrentPartyMembersInDatabase { get; set; }

		public ObservableCollection<enemy> CurrentEnemiesInDatabase { get; set; }
		
		public ObservableCollection<Tuple<string, String>> CurrentGameplayModifier_Edit_AllMods { get; set; }
		public ObservableCollection<Tuple<string, String>> SkillsCurrentLinkedModifiers_Add_AllMods { get; set; }
		public ObservableCollection<Tuple<string, String>> SkillsCurrentLinkedModifiers_Edit_AllMods { get; set; }

		#endregion


		public MainWindow()
		{
			InitializeComponent();

			//INIT collections

			CurrentGameplayModifier_Edit_AllMods = new ObservableCollection<Tuple<String, String>>();
			SkillsCurrentLinkedModifiers_Add_AllMods = new ObservableCollection<Tuple<String, String>>();
			SkillsCurrentLinkedModifiers_Edit_AllMods = new ObservableCollection<Tuple<String, String>>();

			CurrentFollowUpAttacksInDatabase = new ObservableCollection<FollowUpAttack>();
			CurrentJobsInDatabase = new ObservableCollection<Job>();
			CurrentWeaponsInDatabase = new ObservableCollection<Weapon>();
			CurrentSkillsInDatabase = new ObservableCollection<Skill>();
			CurrentItemsInDatabase = new ObservableCollection<Item>();
			CurrentAccessoriesInDatabase = new ObservableCollection<Accessory>();

			CurrentClothesInDatabase = new ObservableCollection<Clothes>();
			CurrentClothesInDatabase_Head = new ObservableCollection<Clothes>();
			CurrentClothesInDatabase_Body = new ObservableCollection<Clothes>();
			CurrentClothesInDatabase_Legs = new ObservableCollection<Clothes>();


			CurrenGameplayModifiersInDatabase = new ObservableCollection<ModifierData>();
			CurrenGameplayModifiersInDatabase_Effects = new ObservableCollection<ModifierData>();
			CurrenGameplayModifiersInDatabase_Traits = new ObservableCollection<ModifierData>();

			CurrentPartyMembersInDatabase = new ObservableCollection<party_member>();
			CurrentEnemiesInDatabase = new ObservableCollection<enemy>();

			//Set up the default null values in the lists
			SetupGameplayModifiersLists();

		}

		private void SetupGameplayModifiersLists()
		{
			//Init gameplayModifiers List
			for (int i = 0; i < gameplayModifiersAdd_vals.Capacity; i++)
			{
				gameplayModifiersAdd_vals.Add(null);
			}
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			LoadAddGameplayModifiersGrids();
			LoadMagicTypesItemControls();
			LoadWeaponTypesItemsControl();
			LoadItemGrid();
		}

		private void LoadItemGrid()
		{
			foreach (EItemType val in Enum.GetValues(typeof(EItemType)))
			{
				if(val == 0) continue;
				ItemTypesEquip_Add_IC.Items.Add(val);
				ItemTypesEquip_Edit_IC.Items.Add(val);
			}
		}

		#region Load/Add Weapon Types

		private void LoadWeaponTypesItemsControl()
		{
			foreach (EWeaponType val in Enum.GetValues(typeof(EWeaponType)))
			{
				if (val == 0) continue;
				PartyMembersWeaponWeakness_Add_IC.Items.Add(val);
				PartyMembersWeaponStrength_Add_IC.Items.Add(val);
				PartyMembersWeaponWeakness_Edit_IC.Items.Add(val);
				PartyMembersWeaponStrength_Edit_IC.Items.Add(val);

				EnemyWeaponWeakness_Add_IC.Items.Add(val);
				EnemyWeaponStrength_Add_IC.Items.Add(val);
				EnemyWeaponWeakness_Edit_IC.Items.Add(val);
				EnemyWeaponStrength_Edit_IC.Items.Add(val);

				ItemsWeaponWeakness_Add_IC.Items.Add(val);
				ItemsWeaponStrength_Add_IC.Items.Add(val);
				ItemsWeaponWeakness_Edit_IC.Items.Add(val);
				ItemsWeaponStrength_Edit_IC.Items.Add(val);

				AccessoriesWeakness_Add_IC.Items.Add(val);
				AccessoriesWeaponStrength_Add_IC.Items.Add(val);
				AccessoriesWeakness_Edit_IC.Items.Add(val);
				AccessoriesWeaponStrength_Edit_IC.Items.Add(val);

				ClothesWeakness_Add_IC.Items.Add(val);
				ClothesWeaponStrength_Add_IC.Items.Add(val);
				ClothesWeakness_Edit_IC.Items.Add(val);
				ClothesWeaponStrength_Edit_IC.Items.Add(val);

				WeaponsWeakness_Edit_IC.Items.Add(val);
				WeaponsWeaponStrength_Edit_IC.Items.Add(val);
				WeaponsWeakness_Add_IC.Items.Add(val);
				WeaponsWeaponStrength_Add_IC.Items.Add(val);
			}
		}

		#endregion

		#region Load/Add Magic Types
		private void LoadMagicTypesItemControls()	
		{
			foreach (EMagicType val in Enum.GetValues(typeof(EMagicType)))
			{
				if (val == 0) continue;
				WeaponMagicTypesEquip_Add_IC.Items.Add(val);
				WaponsMagicTypesEquip_Edit_IC.Items.Add(val);

				ItemMagicTypesEquip_Add_IC.Items.Add(val);
				ItemMagicTypesEquip_Edit_IC.Items.Add(val);

				SkillMagicTypesEquip_Add_IC.Items.Add(val);
				SkillMagicTypesEquip_Edit_IC.Items.Add(val);

				PartyMembersMagicWeakness_Add_IC.Items.Add(val);
				PartyMembersMagicStrength_Add_IC.Items.Add(val);
				PartyMembersMagicWeakness_Edit_IC.Items.Add(val);
				PartyMembersMagicStrength_Edit_IC.Items.Add(val);

				EnemyMagicWeakness_Add_IC.Items.Add(val);
				EnemyMagicStrength_Add_IC.Items.Add(val);
				EnemyMagicWeakness_Edit_IC.Items.Add(val);
				EnemyMagicStrength_Edit_IC.Items.Add(val);


				ItemsMagicWeakness_Add_IC.Items.Add(val);
				ItemsMagicStrength_Add_IC.Items.Add(val);
				ItemsMagicWeakness_Edit_IC.Items.Add(val);
				ItemsMagicStrength_Edit_IC.Items.Add(val);

				AccessoryMagicTypesEquip_Add_IC.Items.Add(val);
				AccessoryMagicTypesEquip_Edit_IC.Items.Add(val);

				AccessoriesMagicWeakness_Add_IC.Items.Add(val);
				AccessoriesMagicStrength_Add_IC.Items.Add(val);
				AccessoriesMagicWeakness_Edit_IC.Items.Add(val);
				AccessoriesMagicStrength_Edit_IC.Items.Add(val);

				ClothesMagicTypesEquip_Add_IC.Items.Add(val);
				ClothesMagicTypesEquip_Edit_IC.Items.Add(val);

				ClothesMagicWeakness_Add_IC.Items.Add(val);
				ClothesMagicStrength_Add_IC.Items.Add(val);
				ClothesMagicWeakness_Edit_IC.Items.Add(val);
				ClothesMagicStrength_Edit_IC.Items.Add(val);

				WeaponsMagicWeakness_Edit_IC.Items.Add(val);
				WeaponsMagicStrength_Edit_IC.Items.Add(val);
				WeaponsMagicWeakness_Add_IC.Items.Add(val);
				WeaponsMagicStrength_Add_IC.Items.Add(val);
			}
		}
		#endregion

		#region LoadAddGameplayModifiersGrids

		private void LoadAddGameplayModifiersGrids()
		{
			foreach (EChanceEffectModifiers val in Enum.GetValues(typeof(EChanceEffectModifiers)))
			{
				if (val == 0) continue;
				ChanceEnum_Add_IC.Items.Add(val);
			}

			foreach (ETurnEffectModifiers val in Enum.GetValues(typeof(ETurnEffectModifiers)))
			{
				if (val == 0) continue;
				TurnEnum_Add_IC.Items.Add(val);
			}

			foreach (EDamageModifiers val in Enum.GetValues(typeof(EDamageModifiers)))
			{
				if (val == 0) continue;
				DamageEnum_Add_IC.Items.Add(val);
			}

			foreach (ESeverityEffectModifiers val in Enum.GetValues(typeof(ESeverityEffectModifiers)))
			{
				if (val == 0) continue;
				SeverityEnum_Add_IC.Items.Add(val);
			}

			foreach (EMagicDamageModifiers val in Enum.GetValues(typeof(EMagicDamageModifiers)))
			{
				if (val == 0) continue;
				MagicDamageEnum_Add_IC.Items.Add(val);
			}

			foreach (EMagicDefenseModifiers val in Enum.GetValues(typeof(EMagicDefenseModifiers)))
			{
				if (val == 0) continue;
				MagicDefenseEnum_Add_IC.Items.Add(val);
			}

			foreach (EStatEffectModifiers val in Enum.GetValues(typeof(EStatEffectModifiers)))
			{
				if (val == 0) continue;
				StatEnum_Add_IC.Items.Add(val);
			}

			foreach (EStatusEffectModifiers val in Enum.GetValues(typeof(EStatusEffectModifiers)))
			{
				if (val == 0) continue;
				StatusEnum_Add_IC.Items.Add(val);
			}

			foreach (ENullifyStatusEffectModifiers val in Enum.GetValues(typeof(ENullifyStatusEffectModifiers)))
			{
				if (val == 0) continue;
				NullifyStatusEnum_Add_IC.Items.Add(val);
			}

			foreach (ENonBattleEffectModifiers val in Enum.GetValues(typeof(ENonBattleEffectModifiers)))
			{
				if (val == 0) continue;
				NonBattleEnum_Add_IC.Items.Add(val);
			}

			foreach (ESpecialEffectModifiers val in Enum.GetValues(typeof(ESpecialEffectModifiers)))
			{
				if (val == 0) continue;
				SpecialEnum_Add_IC.Items.Add(val);
			}


			//For edit
			foreach (EChanceEffectModifiers val in Enum.GetValues(typeof(EChanceEffectModifiers)))
			{
				if (val == 0) continue;
				ChanceEnum_Edit_IC.Items.Add(val);
			}

			foreach (ETurnEffectModifiers val in Enum.GetValues(typeof(ETurnEffectModifiers)))
			{
				if (val == 0) continue;
				TurnEnum_Edit_IC.Items.Add(val);
			}

			foreach (EDamageModifiers val in Enum.GetValues(typeof(EDamageModifiers)))
			{
				if (val == 0) continue;
				DamageEnum_Edit_IC.Items.Add(val);
			}

			foreach (ESeverityEffectModifiers val in Enum.GetValues(typeof(ESeverityEffectModifiers)))
			{
				if (val == 0) continue;
				SeverityEnum_Edit_IC.Items.Add(val);
			}

			foreach (EMagicDamageModifiers val in Enum.GetValues(typeof(EMagicDamageModifiers)))
			{
				if (val == 0) continue;
				MagicDamageEnum_Edit_IC.Items.Add(val);
			}

			foreach (EMagicDefenseModifiers val in Enum.GetValues(typeof(EMagicDefenseModifiers)))
			{
				if (val == 0) continue;
				MagicDefenseEnum_Edit_IC.Items.Add(val);
			}

			foreach (EStatEffectModifiers val in Enum.GetValues(typeof(EStatEffectModifiers)))
			{
				if (val == 0) continue;
				StatEnum_Edit_IC.Items.Add(val);
			}

			foreach (EStatusEffectModifiers val in Enum.GetValues(typeof(EStatusEffectModifiers)))
			{
				if (val == 0) continue;
				StatusEnum_Edit_IC.Items.Add(val);
			}

			foreach (ENullifyStatusEffectModifiers val in Enum.GetValues(typeof(ENullifyStatusEffectModifiers)))
			{
				if (val == 0) continue;
				NullifyStatusEnum_Edit_IC.Items.Add(val);
			}

			foreach (ENonBattleEffectModifiers val in Enum.GetValues(typeof(ENonBattleEffectModifiers)))
			{
				if (val == 0) continue;
				NonBattleEnum_Edit_IC.Items.Add(val);
			}

			foreach (ESpecialEffectModifiers val in Enum.GetValues(typeof(ESpecialEffectModifiers)))
			{
				if (val == 0) continue;
				SpecialEnum_Edit_IC.Items.Add(val);
			}



		}

		#endregion

		#region Enum Vals Checkbox events [UPDATING NEW RECORDS]

		private void ChanceModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = ChanceEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Chance_Modifiers == null)
			{
				modifierData.Chance_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Chance_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Chance_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex; //keep the combobox data.
		}

		private void TurnModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = TurnEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Turn_Modifiers == null)
			{
				modifierData.Turn_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Turn_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Turn_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void DamageModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = DamageEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Damage_Modifiers == null)
			{
				modifierData.Damage_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Damage_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Damage_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void SeverityModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = SeverityEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Severity_Modifiers == null)
			{
				modifierData.Severity_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Severity_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Severity_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void MagicDamageModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = MagicDamageEnum_Add_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Magic_Damage_Modifiers == null)
			{
				modifierData.Magic_Damage_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Magic_Damage_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Magic_Damage_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void MagicDefenseModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = MagicDefenseEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Magic_Defense_Modifiers == null)
			{
				modifierData.Magic_Defense_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Magic_Defense_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Magic_Defense_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void StatModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = StatEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Stat_Modifiers == null)
			{
				modifierData.Stat_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Stat_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Stat_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void StatusModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = StatusEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Status_Effect_Modifiers == null)
			{
				modifierData.Status_Effect_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Status_Effect_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Status_Effect_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void NullifyStatusModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = NullifyStatusEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Nullify_Status_Effect_Modifiers == null)
			{
				modifierData.Nullify_Status_Effect_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Nullify_Status_Effect_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Nullify_Status_Effect_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void NonBattleModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = NonBattleEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.NonBattle_Modifiers == null)
			{
				modifierData.NonBattle_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.NonBattle_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.NonBattle_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		private void SpecialModifier_Edit_CB_Click(object sender, RoutedEventArgs e)
		{
			int absindex = GameplayModifierName_CB.SelectedIndex;
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = SpecialEnum_Edit_IC.Items.IndexOf(item);
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex];

			if (modifierData.Special_Modifiers == null)
			{
				modifierData.Special_Modifiers = (int)Math.Pow(2, index);
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					modifierData.Special_Modifiers += (int)Math.Pow(2, index);
				}
				else
				{
					modifierData.Special_Modifiers -= (int)Math.Pow(2, index);
				}
			}
			CurrenGameplayModifiersInDatabase[GameplayModifierName_CB.SelectedIndex] = modifierData;
			GameplayModifierName_CB.SelectedIndex = absindex;
		}

		#endregion

		#region Enum Vals Checkbox events [ADD NEW RECORD]
		private void ChanceModifier_CB_Click(object sender, RoutedEventArgs e)
		{

			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = ChanceEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[0] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[0] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[0] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[0] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[0]);
			}
		}

		private void TurnModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = TurnEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[1] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[1] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[1] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[1] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[1]);
			}
		}

		private void DamageModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = DamageEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[2] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[2] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[2] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[2] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[2]);
			}
		}

		private void SeverityModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = SeverityEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[3] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[3] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[3] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[3] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[3]);
			}
		}

		private void MagicDamageModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = MagicDamageEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[4] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[4] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[4] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[4] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[4]);
			}
		}

		private void MagicDefenseModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = MagicDefenseEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[5] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[5] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[5] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[5] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[5]);
			}
		}

		private void StatModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = StatEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[6] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[6] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[6] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[6] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[6]);
			}
		}

		private void StatusModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = StatusEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[7] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[7] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[7] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[7] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[7]);
			}
		}

		private void NullifyStatusModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = NullifyStatusEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[8] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[8] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[8] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[8] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[8]);
			}
		}

		private void NonBattleModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = NonBattleEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[9] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[9] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[9] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[9] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[9]);
			}
		}

		private void SpecialModifier_CB_Click(object sender, RoutedEventArgs e)
		{
			//Traverse UP the viusal tree until the ROOT is found THEN AND ONLY then can you use indexof
			var item = (VisualTreeHelper.GetParent(sender as CheckBox) as Grid).DataContext;
			int index = SpecialEnum_Add_IC.Items.IndexOf(item);


			if (gameplayModifiersAdd_vals[10] == null)
			{
				//is null
				int bitval = (int) Math.Pow(2, index);
				gameplayModifiersAdd_vals[10] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool) (sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[10] += (int) Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[10] -= (int) Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[10]);
			}
		}

		#endregion

		private void ChangeGridToAdd(object sender, RoutedEventArgs e)
		{
			TabItem CurTabItem = ((sender as Button).Parent as Grid).Parent as TabItem;
			if (CurTabItem != null)
			{
				switch (CurTabItem.Header)
				{
					case ("FollowUpAttacks"):
						AddGrid_FollowupAttacks.Visibility = Visibility.Visible;
						EditGrid_FollowupAttacks.Visibility = Visibility.Hidden;
						break;
					case ("Jobs"):
						AddGrid_Jobs.Visibility = Visibility.Visible;
						EditGrid_Jobs.Visibility = Visibility.Hidden;
						break;
					case ("Skills"):
						Skills_Add_Grid.Visibility = Visibility.Visible;
						Skills_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Enemy"):
						Enemy_Add_Grid.Visibility = Visibility.Visible;
						Enemy_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Party Member"):
						PartyMember_Add_Grid.Visibility = Visibility.Visible;
						PartyMember_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("GamePlay Modifiers"):
						GameplayModifiers_Add_Grid.Visibility = Visibility.Visible;
						GameplayModifiers_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Items"):
						Item_Edit_Grid.Visibility = Visibility.Hidden;
						Item_Add_Grid.Visibility = Visibility.Visible;
						break;
					case ("Accessories"):
						Accessory_Edit_Grid.Visibility = Visibility.Hidden;
						Accessory_Add_Grid.Visibility = Visibility.Visible;
						break;
					case ("Clothes"):
						Clothes_Edit_Grid.Visibility = Visibility.Hidden;
						Clothes_Add_Grid.Visibility = Visibility.Visible;
						break;
					case ("Weapons"):
						Weapons_Add_Grid.Visibility = Visibility.Visible;
						Weapons_Edit_Grid.Visibility = Visibility.Hidden;
						break;
					default:
						throw new NoNullAllowedException();
						break;
				}
			}
		}

		private void ChangeGridToEdit(object sender, RoutedEventArgs e)
		{
			TabItem CurTabItem = ((sender as Button).Parent as Grid).Parent as TabItem;
			if (CurTabItem != null)
			{
				switch (CurTabItem.Header)
				{
					case ("FollowUpAttacks"):
						AddGrid_FollowupAttacks.Visibility = Visibility.Hidden;
						EditGrid_FollowupAttacks.Visibility = Visibility.Visible;
						break;
					case ("Jobs"):
						AddGrid_Jobs.Visibility = Visibility.Hidden;
						EditGrid_Jobs.Visibility = Visibility.Visible;
						break;
					case ("Skills"):
						Skills_Add_Grid.Visibility = Visibility.Hidden;
						Skills_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("Enemy"):
						Enemy_Add_Grid.Visibility = Visibility.Hidden;
						Enemy_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("Party Member"):
						PartyMember_Add_Grid.Visibility = Visibility.Hidden;
						PartyMember_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("GamePlay Modifiers"):
						GameplayModifiers_Add_Grid.Visibility = Visibility.Hidden;
						GameplayModifiers_Edit_Grid.Visibility = Visibility.Visible;
						break;
					case ("Items"):
						Item_Edit_Grid.Visibility = Visibility.Visible;
						Item_Add_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Accessories"):
						Accessory_Edit_Grid.Visibility = Visibility.Visible;
						Accessory_Add_Grid.Visibility = Visibility.Hidden;
						break;
					case ("Clothes"):
						Clothes_Edit_Grid.Visibility = Visibility.Visible;
						Clothes_Add_Grid.Visibility = Visibility.Hidden;
						break;

					case ("Weapons"):
						Weapons_Add_Grid.Visibility = Visibility.Hidden;
						Weapons_Edit_Grid.Visibility = Visibility.Visible;
						break;
					default:
						throw new NoNullAllowedException();
						break;
				}
			}
		}

		private void TestBTN_Click(object sender, RoutedEventArgs e)
		{
			EditJobsDB_LB.ItemsSource = null;
			//CurrentJobsInDatabase.Add(new Job(EJob.LuckyStar)
			//{
			//	LeftStance = EStanceType.Bloody,
			//	RightStance = EStanceType.Feather
			//});
			EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
			UpdateLayout();
		}

		private void AddJobToDB_BTN_OnClick(object sender, RoutedEventArgs e)
		{

		}

		private void BrowseForDataBase_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Emeralds and Elements Database",
				FileName = "", //default file name
				Filter = "SQLite database (*.db)|*.db",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] {'/', '\\'}));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);
			Databases_TB.Text = dlg.FileName;
			SQLDatabasePath = dlg.FileName;
			

			//Load the data to the screen from the database
			LoadFollowUpAttacksFromDatabase();
			LoadJobsFromDatabase();
			LoadGameplayModifiersFromDatabase();
			LoadWeaponsFromDatabase();
			LoadItemsFromDatabase();
			LoadSkillsFromDatabase();
			LoadPartyMembersFromDatabase();
			LoadEnemyFromDatabase();
			LoadAccessoriesFromDatabase();
			LoadClothesFromDatabase();

		}

		private void LoadFollowUpAttacksFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			FollowUpAttacks_LB.ItemsSource = null;
			EditFollowUpAttacks_LB.ItemsSource = null;

			CurrentFollowUpAttacksInDatabase.Clear();
			_sqlite_conn = new SQLiteConnection(masterfile);

			try
			{
				StringBuilder Createsql = new StringBuilder();
				Createsql.Append("SELECT * FROM `followup_attacks`;");

				IEnumerable<FollowUpAttack> varlist = _sqlite_conn.Query<FollowUpAttack>(Createsql.ToString());
				foreach (FollowUpAttack fua in varlist)
				{
					CurrentFollowUpAttacksInDatabase.Add(fua);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("follow up attack Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [follow up attack] failed: {0}", ex.Message);
			}
			finally
			{
				FollowUpAttacks_LB.ItemsSource = CurrentFollowUpAttacksInDatabase;
				EditFollowUpAttacks_LB.ItemsSource = CurrentFollowUpAttacksInDatabase;
			}

		}

		private void LoadClothesFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			ClothesName_Edit_CB.ItemsSource = null;
			PartyMemberClothes_Head_Edit_CB.ItemsSource = null;
			PartyMemberClothes_Body_Edit_CB.ItemsSource = null;
			PartyMemberClothes_Legs_Edit_CB.ItemsSource = null;

			enemyClothes_Head_Edit_CB.ItemsSource = null;
			enemyClothes_Body_Edit_CB.ItemsSource = null;
			enemyClothes_Legs_Edit_CB.ItemsSource = null;

			CurrentClothesInDatabase.Clear();
			CurrentClothesInDatabase_Head.Clear();
			CurrentClothesInDatabase_Body.Clear();
			CurrentClothesInDatabase_Legs.Clear();

			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `clothes`;");



				IEnumerable<Clothes> varlist = _sqlite_conn.Query<Clothes>(Createsql);
				foreach (Clothes clothes in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly

					#region Populate Weapopn Keys

					#region Effects/Traits

					Createsql = String.Format("SELECT * FROM `modifier_keys` WHERE req_name = '{0}';", clothes.ID);
					IEnumerable<Modifier_Keys> varlist_mod = _sqlite_conn.Query<Modifier_Keys>(Createsql);
					foreach (Modifier_Keys mod_key in varlist_mod)
					{
						ModifierData moddata = CurrenGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
						if (moddata == null) continue;
						if (moddata.bEffect)
							clothes.Effects.Add(moddata);
						else
							clothes.Traits.Add(moddata);
					}

					#endregion

					#region Skills

					Createsql = String.Format("SELECT * FROM `skill_keys` WHERE req_name = '{0}';", clothes.ID);
					IEnumerable<Skill_Keys> varlist_skill = _sqlite_conn.Query<Skill_Keys>(Createsql);
					foreach (Skill_Keys skill_key in varlist_skill)
					{
						Skill skilldata = CurrentSkillsInDatabase.Single(x => x.Name == skill_key.Skill_ID);
						if (skilldata == null) continue;
						clothes.ClothesSkills.Add(skilldata);
					}

					#endregion

					#endregion

					//Each weapon a list of keys to it. And we must populate that data correctly
					CurrentClothesInDatabase.Add(clothes);
					if (clothes.Clothes_Type == (int)EClothesType.Head)
					{
						CurrentClothesInDatabase_Head.Add(clothes);
					}
					else if (clothes.Clothes_Type == (int)EClothesType.Body)
					{
						CurrentClothesInDatabase_Body.Add(clothes);
					}
					else if (clothes.Clothes_Type == (int)EClothesType.Legs)
					{
						CurrentClothesInDatabase_Legs.Add(clothes);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("clothes Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading [FROM enemy] Database failed: {0}", ex.Message);
			}
			finally
			{
				ClothesName_Edit_CB.ItemsSource = CurrentClothesInDatabase;
				PartyMemberClothes_Head_Edit_CB.ItemsSource = CurrentClothesInDatabase_Head;
				PartyMemberClothes_Body_Edit_CB.ItemsSource = CurrentClothesInDatabase_Body;
				PartyMemberClothes_Legs_Edit_CB.ItemsSource = CurrentClothesInDatabase_Legs;

				enemyClothes_Head_Edit_CB.ItemsSource = CurrentClothesInDatabase_Head;
				enemyClothes_Body_Edit_CB.ItemsSource = CurrentClothesInDatabase_Body;
				enemyClothes_Legs_Edit_CB.ItemsSource = CurrentClothesInDatabase_Legs;

			}
		}

		private void LoadAccessoriesFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			AccessoryName_Edit_CB.ItemsSource = null;
			PartyMemberClothes_Acc1_Edit_CB.ItemsSource = null;
			PartyMemberClothes_Acc2_Edit_CB.ItemsSource = null;

			enemyClothes_Acc1_Edit_CB.ItemsSource = null;
			enemyClothes_Acc2_Edit_CB.ItemsSource = null;

			CurrentAccessoriesInDatabase.Clear();
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `accessories`;");



				IEnumerable<Accessory> varlist = _sqlite_conn.Query<Accessory>(Createsql);
				foreach (Accessory accessory in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly

					#region Populate Weapopn Keys

					#region Effects/Traits

					Createsql = String.Format("SELECT * FROM `modifier_keys` WHERE req_name = '{0}';", accessory.ID);
					IEnumerable<Modifier_Keys> varlist_mod = _sqlite_conn.Query<Modifier_Keys>(Createsql);
					foreach (Modifier_Keys mod_key in varlist_mod)
					{
						ModifierData moddata = CurrenGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
						if (moddata == null) continue;
						if (moddata.bEffect)
							accessory.Effects.Add(moddata);
						else
							accessory.Traits.Add(moddata);
					}

					#endregion

					#region Skills

					Createsql = String.Format("SELECT * FROM `skill_keys` WHERE req_name = '{0}';", accessory.ID);
					IEnumerable<Skill_Keys> varlist_skill = _sqlite_conn.Query<Skill_Keys>(Createsql);
					foreach (Skill_Keys skill_key in varlist_skill)
					{
						Skill skilldata = CurrentSkillsInDatabase.Single(x => x.Name == skill_key.Skill_ID);
						if (skilldata == null) continue;
						accessory.AccessorySkills.Add(skilldata);
					}

					#endregion

					#endregion

					//Each weapon a list of keys to it. And we must populate that data correctly
					CurrentAccessoriesInDatabase.Add(accessory);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("accessories Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading [FROM enemy] Database failed: {0}", ex.Message);
			}
			finally
			{
				AccessoryName_Edit_CB.ItemsSource = CurrentAccessoriesInDatabase;
				PartyMemberClothes_Acc1_Edit_CB.ItemsSource = CurrentAccessoriesInDatabase;
				PartyMemberClothes_Acc2_Edit_CB.ItemsSource = CurrentAccessoriesInDatabase;

				enemyClothes_Acc1_Edit_CB.ItemsSource = CurrentAccessoriesInDatabase;
				enemyClothes_Acc2_Edit_CB.ItemsSource = CurrentAccessoriesInDatabase;
			}
		}

		private void LoadEnemyFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			EnemyName_Edit_CB.ItemsSource = null;
			CurrentEnemiesInDatabase.Clear();
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `enemy`;");

				IEnumerable<enemy> varlist = _sqlite_conn.Query<enemy>(Createsql);
				foreach (enemy enemy in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly
					CurrentEnemiesInDatabase.Add(enemy);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Skills Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading [FROM enemy] Database failed: {0}", ex.Message);
			}
			finally
			{
				EnemyName_Edit_CB.ItemsSource = CurrentEnemiesInDatabase;

			}
		}

		private void LoadPartyMembersFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			//SkillsName_Edit_CB.ItemsSource = null;
			PartyMemberName_Edit_CB.ItemsSource = null;
			CurrentPartyMembersInDatabase.Clear();
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `party_member`;");

				IEnumerable<party_member> varlist = _sqlite_conn.Query<party_member>(Createsql);
				foreach (party_member partyMember in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly
					CurrentPartyMembersInDatabase.Add(partyMember);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Skills Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading [FROM Party Member] Database failed: {0}", ex.Message);
			}
			finally
			{
				//SkillsName_Edit_CB.ItemsSource = CurrentPartyMembersInDatabase;
				PartyMemberName_Edit_CB.ItemsSource = CurrentPartyMembersInDatabase;
			}
		}

		private void LoadSkillsFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			SkillsName_Edit_CB.ItemsSource = null;

			PartymemberSkills_Add_CB.ItemsSource = null;
			PartymemberSkills_Edit_CB.ItemsSource = null;

			EnemySkills_Add_CB.ItemsSource = null;
			EnemySkills_Edit_CB.ItemsSource = null;

			AccessoryEquipSkills_Add_CB.ItemsSource = null;
			AccessoryEquipSkills_Edit_CB.ItemsSource = null;

			ClothesEquipSkills_Add_CB.ItemsSource = null;
			ClothesEquipSkills_Edit_CB.ItemsSource = null;


			CurrentSkillsInDatabase.Clear();
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `skills`;");

				IEnumerable<Skills> varlist = _sqlite_conn.Query<Skills>(Createsql);
				foreach (Skills skill in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly
					CurrentSkillsInDatabase.Add(skill);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Skills Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading [FROM Skills] Database failed: {0}", ex.Message);
			}
			finally
			{
				SkillsName_Edit_CB.ItemsSource = CurrentSkillsInDatabase;
				PartymemberSkills_Add_CB.ItemsSource = CurrentSkillsInDatabase;
				PartymemberSkills_Edit_CB.ItemsSource = CurrentSkillsInDatabase;
				EnemySkills_Add_CB.ItemsSource = CurrentSkillsInDatabase;
				EnemySkills_Edit_CB.ItemsSource = CurrentSkillsInDatabase;
				AccessoryEquipSkills_Add_CB.ItemsSource = CurrentSkillsInDatabase;
				AccessoryEquipSkills_Edit_CB.ItemsSource = CurrentSkillsInDatabase;
				ClothesEquipSkills_Add_CB.ItemsSource = CurrentSkillsInDatabase;
				ClothesEquipSkills_Edit_CB.ItemsSource = CurrentSkillsInDatabase;

			}
		}

		private void LoadItemsFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			ItemName_Edit_CB.ItemsSource = null;

			PartyMemberItems_Add_CB.ItemsSource = null;
			PartyMemberItems_Edit_CB.ItemsSource = null;
			CurrentItemsInDatabase.Clear();

			EnemyItems_Add_CB.ItemsSource = null;
			EnemyItemsDrops_Add_CB.ItemsSource = null;
			EnemyItems_Edit_CB.ItemsSource = null;
			EnemyItemsDrops_Edit_CB.ItemsSource = null;


			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `items`;");

				IEnumerable<Items> varlist = _sqlite_conn.Query<Items>(Createsql);
				foreach (Item item in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly

					#region Populate Weapopn Keys
					#region Effects/Traits

					Createsql = String.Format("SELECT * FROM `modifier_keys` WHERE req_name = '{0}';", item.ID);
					IEnumerable<Modifier_Keys> varlist_mod = _sqlite_conn.Query<Modifier_Keys>(Createsql);
					foreach (Modifier_Keys mod_key in varlist_mod)
					{
						ModifierData moddata = CurrenGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
						if (moddata == null) continue;
						if (moddata.bEffect)
							item.Effects.Add(moddata);
						else
							item.Traits.Add(moddata);
					}

					#endregion

					#region Skills
					Createsql = String.Format("SELECT * FROM `skill_keys` WHERE req_name = '{0}';", item.ID);
					IEnumerable<Skill_Keys> varlist_skill = _sqlite_conn.Query<Skill_Keys>(Createsql);
					foreach (Skill_Keys skill_key in varlist_skill)
					{
						Skill skilldata = CurrentSkillsInDatabase.Single(x => x.Name == skill_key.Skill_ID);
						if (skilldata == null) continue;
						item.ItemSkills.Add(skilldata);
					}
					#endregion
					#endregion
					CurrentItemsInDatabase.Add(item);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Weapons Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading [FROM WEAPONS] Database failed: {0}", ex.Message);
			}
			finally
			{
				ItemName_Edit_CB.ItemsSource = CurrentItemsInDatabase;
				PartyMemberItems_Add_CB.ItemsSource = CurrentItemsInDatabase;
				PartyMemberItems_Edit_CB.ItemsSource = CurrentItemsInDatabase;
				EnemyItems_Add_CB.ItemsSource = CurrentItemsInDatabase;
				EnemyItemsDrops_Add_CB.ItemsSource = CurrentItemsInDatabase;
				EnemyItems_Edit_CB.ItemsSource = CurrentItemsInDatabase;
				EnemyItemsDrops_Edit_CB.ItemsSource = CurrentItemsInDatabase;
			}
		}

		private void LoadWeaponsFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			WeaponName_Edit_CB.ItemsSource = null;

			PartyMemberWeapons_Add_CB.ItemsSource = null;
			PartyMemberWeapons_Edit_CB.ItemsSource = null;

			EnemyWeapons_Add_CB.ItemsSource = null;
			EnemyWeapons_Edit_CB.ItemsSource = null;


			CurrentWeaponsInDatabase.Clear();
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = String.Empty;
				Createsql = ("SELECT * FROM `weapons`;");

				IEnumerable<Weapons> varlist = _sqlite_conn.Query<Weapons>(Createsql);
				foreach (Weapons w in varlist.ToList())
				{
					//Each weapon a list of keys to it. And we must populate that data correctly

					#region Populate Weapopn Keys
					#region Effects/Traits

					Createsql = String.Format("SELECT * FROM `modifier_keys` WHERE req_name = '{0}';", w.ID);
					IEnumerable<Modifier_Keys> varlist_mod = _sqlite_conn.Query<Modifier_Keys>(Createsql);
					foreach (Modifier_Keys mod_key in varlist_mod)
					{
						ModifierData moddata =  CurrenGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
						if (moddata == null) continue;
						if(moddata.bEffect)
							w.Effects.Add(moddata);
						else
							w.Traits.Add(moddata);
					}

					#endregion

					#region Skills
					Createsql = String.Format("SELECT * FROM `skill_keys` WHERE req_name = '{0}';", w.ID);
					IEnumerable<Skill_Keys> varlist_skill = _sqlite_conn.Query<Skill_Keys>(Createsql);
					foreach (Skill_Keys skill_key in varlist_skill)
					{
						Skill skilldata = CurrentSkillsInDatabase.Single(x => x.Name == skill_key.Skill_ID);
						if (skilldata == null) continue;
						w.WeaponSkills.Add(skilldata);
					}
					#endregion
					#endregion
					CurrentWeaponsInDatabase.Add(w);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Weapons Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading [FROM WEAPONS] Database failed: {0}", ex.Message);
			}
			finally
			{
				WeaponName_Edit_CB.ItemsSource = CurrentWeaponsInDatabase;
				PartyMemberWeapons_Add_CB.ItemsSource = CurrentWeaponsInDatabase;
				PartyMemberWeapons_Edit_CB.ItemsSource = CurrentWeaponsInDatabase;
				EnemyWeapons_Add_CB.ItemsSource = CurrentWeaponsInDatabase;
				EnemyWeapons_Edit_CB.ItemsSource = CurrentWeaponsInDatabase;
			}
		}


		private void LoadJobsFromDatabase()
		{
			String masterfile = (SQLDatabasePath);
			//first up we need to connect to our database
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				EditJobsDB_LB.ItemsSource = null; //reset
				PartyMemberMainJob_Add_CB.ItemsSource = null;
				PartyMemberSubJob_Add_CB.ItemsSource = null;
				PartyMemberMainJob_Edit_CB.ItemsSource = null;
				PartyMemberSubJob_Edit_CB.ItemsSource = null;


				StringBuilder Createsql = new StringBuilder();
				Createsql.Append("SELECT * FROM `jobs`;");

				IEnumerable<Job> varlist = _sqlite_conn.Query<Job>(Createsql.ToString());
				int i = 0;
				i++;
				foreach (Job j in varlist.ToList())
				{
					CurrentJobsInDatabase.Add(j);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Job Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database failed: {0}", ex.Message);
			}
			finally
			{
				EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
				PartyMemberMainJob_Add_CB.ItemsSource = CurrentJobsInDatabase;
				PartyMemberSubJob_Add_CB.ItemsSource = CurrentJobsInDatabase;
				PartyMemberMainJob_Edit_CB.ItemsSource = CurrentJobsInDatabase;
				PartyMemberSubJob_Edit_CB.ItemsSource = CurrentJobsInDatabase;
			}
		}


		private void LoadGameplayModifiersFromDatabase()
		{
			GameplayModifierName_CB.ItemsSource = null;

			WeaponEffects_Add_CB.ItemsSource = null;
			WeaponTraits_Add_CB.ItemsSource = null;
			WeaponEffects_Edit_CB.ItemsSource = null;
			WeaponTraits_Edit_CB.ItemsSource = null;

			ItemEquipEffects_Add_CB.ItemsSource = null;
			ItemEquipTraits_Add_CB.ItemsSource = null;

			GameplayModifierCombinedModifier_Add_CB.ItemsSource = null;
			GameplayModifierCombine2_CB.ItemsSource = null;

			ItemEquipEffects_edit_CB.ItemsSource = null;
			ItemEquipTraits_Edit_CB.ItemsSource = null;

			SkillLinkedModifier_Add_CB.ItemsSource = null;
			SkillLinkedModifier_Edit_CB.ItemsSource = null;

			AccessoryEquipEffects_Add_CB.ItemsSource = null;
			AccessoryEquipTraits_Add_CB.ItemsSource = null;

			AccessoryEquipEffects_Edit_CB.ItemsSource = null;
			AccessoryEquipTraits_Edit_CB.ItemsSource = null;

			ClothesEquipEffects_Add_CB.ItemsSource = null;
			ClothesEquipTraits_Add_CB.ItemsSource = null;

			ClothesEquipEffects_Edit_CB.ItemsSource = null;
			ClothesEquipTraits_Edit_CB.ItemsSource = null;

			String masterfile = (SQLDatabasePath);
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				StringBuilder Createsql = new StringBuilder();
				Createsql.Append("SELECT * FROM `gameplay_modifiers`;");

				IEnumerable<ModifierData> varlist = _sqlite_conn.Query<ModifierData>(Createsql.ToString());
				int i = 0;
				i++;

				foreach (ModifierData md in varlist.ToList())
				{
					CurrenGameplayModifiersInDatabase.Add(md);
					//Load the modifiers in to the filtered Collections
					if (md.bEffect)
					{
						CurrenGameplayModifiersInDatabase_Effects.Add(md);
					}
					else
					{
						CurrenGameplayModifiersInDatabase_Traits.Add(md);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [gameplay modifier] failed: {0}", ex.Message);
			}
			finally
			{
				//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
				GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				WeaponEffects_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Effects;
				WeaponTraits_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Traits;
				WeaponEffects_Edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Effects;
				WeaponTraits_Edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Traits;

				ItemEquipEffects_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Effects;
				ItemEquipTraits_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Traits;

				GameplayModifierCombinedModifier_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				GameplayModifierCombine2_CB.ItemsSource = CurrenGameplayModifiersInDatabase;

				ItemEquipEffects_edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Effects;
				ItemEquipTraits_Edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Traits;

				SkillLinkedModifier_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				SkillLinkedModifier_Edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase;

				AccessoryEquipEffects_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Effects;
				AccessoryEquipTraits_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Traits;

				AccessoryEquipEffects_Edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Effects;
				AccessoryEquipTraits_Edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Traits;

				ClothesEquipEffects_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Effects;
				ClothesEquipTraits_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Traits;

				ClothesEquipEffects_Edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Effects;
				ClothesEquipTraits_Edit_CB.ItemsSource = CurrenGameplayModifiersInDatabase_Traits;


			}
		}

		private void AddToGameplayModifierDatabase(object sender, RoutedEventArgs e)
		{
			//Do we have a name?
			if (GameplayModifierName_TB.Text != "")
			{
				String masterfile = (SQLDatabasePath);
				//first up we need to connect to our database
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					SQLiteCommand sqlite_cmd;
					string Createsql = "";

					Createsql = "INSERT OR IGNORE INTO `gameplay_modifiers`" +
					            "(id,";

					for (int i = 0; i < gameplayModifiersAdd_vals.Count; i++)
					{
						int? ival = gameplayModifiersAdd_vals[i];
						if (ival == null) continue;

						switch (i)
						{
							case 0:
								Createsql += "chance_modifiers,";
								break;
							case 1:
								Createsql += "turn_modifiers,";
								break;
							case 2:
								Createsql += "damage_modifiers,";
								break;
							case 3:
								Createsql += "damage_modifiers,";
								break;
							case 4:
								Createsql += "magic_damage_modifiers,";
								break;
							case 5:
								Createsql += "magic_defense_modifiers,";
								break;
							case 6:
								Createsql += "stat_modifiers,";
								break;
							case 7:
								Createsql += "status_effect_modifiers,";
								break;
							case 8:
								Createsql += "nullify_status_effect_modifiers,";
								break;
							case 9:
								Createsql += "nonbattle_modifiers,";
								break;
							case 10:
								Createsql += "special_modifiers,";
								break;
							default:
								throw new NoNullAllowedException();
								break;
						}
					}

					Createsql += "function_ptr, beffect";
					if (GameplayModifierSkillLinked_CB.SelectedIndex > -1)
						Createsql += ", skills_fk";
					Createsql += ") ";

					Createsql += String.Format("SELECT '{0}', ", GameplayModifierName_TB.Text);
					for (int i = 0; i < gameplayModifiersAdd_vals.Count; i++)
					{
						int? ival = gameplayModifiersAdd_vals[i];
						if (ival == null) continue;
						Createsql += String.Format("{0}, ", ival);
					}

					Createsql += String.Format("'{0}', {1}", GameplayModifierFuncPTR_TB.Text,
						GameplayModifierEffect_CB.IsChecked.ToString());
					if (GameplayModifierSkillLinked_CB.SelectedIndex > -1)
						Createsql += String.Format(", {0}", GameplayModifierSkillLinked_CB.Text);
					Createsql += " ";

					Createsql += String.Format("WHERE NOT EXISTS( SELECT 1 FROM `gameplay_modifiers` WHERE id='{0}')",
						GameplayModifierName_TB.Text);

					_sqlite_conn.Query<object>(Createsql);

					GlobalStatusLog_TB.Text = String.Format("Insert into Gameplay Modifiers database SUCCESS!!:");

				}
				catch (Exception ex)
				{
					Console.WriteLine("Insert into Gameplay Modifiers database FAILURE {0}:", ex.Message);
					GlobalStatusLog_TB.Text = String.Format("Insert into Gameplay Modifiers database FAILURE {0}:", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
				}
			}
		}

		private void GameplayModifierName_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (GameplayModifierName_CB.SelectedIndex < 0) return;
			ModifierAllMods_Edit_IC.ItemsSource = null;
			CurrentGameplayModifier_Edit_AllMods.Clear();
			int index = GameplayModifierName_CB.SelectedIndex;

			//GameplayModifierName_TB.Text = CurrenGameplayModifiersInDatabase[index].ModifierName;
			GameplayModifierFuncPTR_Edit_TB.Text = CurrenGameplayModifiersInDatabase[index].Function_PTR;
			GameplayModifierEffect_Edit_CB.IsChecked = CurrenGameplayModifiersInDatabase[index].bEffect;

			//TODO: linked skill here


			//Reset all checkboxes for future binding
			ResetCheckboxes();
			this.UpdateLayout();

			//Load all the modifiers to the check boxes [BINDING]
			SetEditModifiersInDatabase(index, ChanceEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Chance_Modifiers, EChanceEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, TurnEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Turn_Modifiers, ETurnEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, DamageEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Damage_Modifiers, EDamageModifiers.NONE);
			SetEditModifiersInDatabase(index, SeverityEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Severity_Modifiers, ESeverityEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, MagicDamageEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Magic_Damage_Modifiers, EMagicDamageModifiers.NONE);

			SetEditModifiersInDatabase(index, MagicDefenseEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Magic_Defense_Modifiers, EMagicDefenseModifiers.NONE);
			SetEditModifiersInDatabase(index, StatEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Stat_Modifiers, EStatEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, StatusEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Status_Effect_Modifiers, EStatusEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, NullifyStatusEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Nullify_Status_Effect_Modifiers,
				ENullifyStatusEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, NonBattleEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].NonBattle_Modifiers, ENonBattleEffectModifiers.NONE);

			SetEditModifiersInDatabase(index, SpecialEnum_Edit_IC,
				(int?) CurrenGameplayModifiersInDatabase[index].Special_Modifiers, ESpecialEffectModifiers.NONE);

			ModifierAllMods_Edit_IC.ItemsSource = CurrentGameplayModifier_Edit_AllMods;
		}


		private void ResetCheckboxes()
		{
			SetEditModifiersInDatabase(0, ChanceEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Chance_Modifiers, EChanceEffectModifiers.NONE, true);
			SetEditModifiersInDatabase(0, TurnEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Turn_Modifiers, ETurnEffectModifiers.NONE, true);
			SetEditModifiersInDatabase(0, DamageEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Damage_Modifiers, EDamageModifiers.NONE, true);
			SetEditModifiersInDatabase(0, SeverityEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Severity_Modifiers, ESeverityEffectModifiers.NONE, true);
			SetEditModifiersInDatabase(0, MagicDamageEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Magic_Damage_Modifiers, EMagicDamageModifiers.NONE, true);

			SetEditModifiersInDatabase(0, MagicDefenseEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Magic_Defense_Modifiers, EMagicDefenseModifiers.NONE, true);
			SetEditModifiersInDatabase(0, StatEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Stat_Modifiers, EStatEffectModifiers.NONE, true);
			SetEditModifiersInDatabase(0, StatusEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Status_Effect_Modifiers, EStatusEffectModifiers.NONE, true);
			SetEditModifiersInDatabase(0, NullifyStatusEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Nullify_Status_Effect_Modifiers,
				ENullifyStatusEffectModifiers.NONE, true);
			SetEditModifiersInDatabase(0, NonBattleEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].NonBattle_Modifiers, ENonBattleEffectModifiers.NONE, true);

			SetEditModifiersInDatabase(0, SpecialEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[0].Special_Modifiers, ESpecialEffectModifiers.NONE, true);
		}

		#region Setting Edit Check Boxes



		private void SetEditModifiersInDatabase(int index, ItemsControl cuItemsControl, int? modifiervalue, Enum etype, bool bReset = false)
		{
			if (modifiervalue == null && !bReset) return;
			ModifierAllMods_Edit_IC.ItemsSource = null;
			//set the check boxes
			int i = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter) cuItemsControl.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName("5_T", c);
				if (bReset)
				{
					(vv as CheckBox).IsChecked = false;
				}
				else if ((en & (int) modifiervalue) > 0)
				{
					(vv as CheckBox).IsChecked = true;
					//This is here to display ALL the data to the user at once in an items control
					CurrentGameplayModifier_Edit_AllMods.Add(new Tuple< String, String>
						(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.')+2).Replace("Modifiers",""),
						etype.GetType().GetEnumValues().GetValue(i+1).ToString()) );
				}
				else
				{
					(vv as CheckBox).IsChecked = false;
				}
				i++;
			}

			ModifierAllMods_Edit_IC.ItemsSource = CurrentGameplayModifier_Edit_AllMods;
		}





		#endregion


		private void GamePlayModifierViewer_CB_onChangedSelection(object sender, SelectionChangedEventArgs e)
		{
			EditChanceModifiers_Grid.Visibility = Visibility.Hidden;
			EditTurnModifiers_Grid.Visibility = Visibility.Hidden;
			EditDamageModifiers_Grid.Visibility = Visibility.Hidden;
			EditSeverityModifiers_Grid.Visibility = Visibility.Hidden;
			EditMagicDamageModifiers_Grid.Visibility = Visibility.Hidden;
			EditMagicDefenseModifiers_Grid.Visibility = Visibility.Hidden;
			EditStatModifiers_Grid.Visibility = Visibility.Hidden;
			EditStatusModifiers_Grid.Visibility = Visibility.Hidden;
			EditNullifyStatusModifiers_Grid.Visibility = Visibility.Hidden;
			EditCNonBattleModifiers_Grid.Visibility = Visibility.Hidden;
			EditSpecialModifiers_Grid.Visibility = Visibility.Hidden;
			switch ((sender as ComboBox).SelectedIndex)
			{
				case 0:
					EditChanceModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 1:
					EditTurnModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 2:
					EditDamageModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 3:
					EditSeverityModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 4:
					EditMagicDamageModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 5:
					EditMagicDefenseModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 6:
					EditStatModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 7:
					EditStatusModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 8:
					EditNullifyStatusModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 9:
					EditCNonBattleModifiers_Grid.Visibility = Visibility.Visible;
					break;
				case 10:
					EditSpecialModifiers_Grid.Visibility = Visibility.Visible;
					break;
				default:
					throw new NotImplementedException();
					break;
			}

		}

		private void UpdateGameplayModifierInDatabase(object sender, RoutedEventArgs e)
		{
			//before we send the SQL update query we need to update the info in memory.
			int absindex = GameplayModifierName_CB.SelectedIndex;
			ModifierData modifierData = CurrenGameplayModifiersInDatabase[absindex];
			modifierData.Function_PTR = GameplayModifierFuncPTR_Edit_TB.Text;
			modifierData.bEffect= (bool)GameplayModifierEffect_Edit_CB.IsChecked;
			modifierData.Skills_FK = GameplayModifierSkillLinked_Edit_CB.Text;

			//Gameplay modifiers update.

			GameplayModifierName_CB.ItemsSource = null;

			String masterfile = (SQLDatabasePath);
			_sqlite_conn = new SQLiteConnection(masterfile);
			int rowid = 0;
			try
			{
				String Createsql = "";
				//Createsql = ("SELECT * FROM `gameplay_modifiers`;");

				Createsql = "UPDATE `gameplay_modifiers` " +
				            "SET " +
				            String.Format("{0} = {1},", "chance_modifiers", modifierData.Chance_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = {1},", "turn_modifiers", modifierData.Turn_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = {1},", "damage_modifiers", modifierData.Damage_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = {1},", "severity_modifiers", modifierData.Severity_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = {1},", "magic_damage_modifiers", modifierData.Magic_Damage_Modifiers.GetValueOrDefault(0)) +
				            
				            String.Format("{0} = {1},", "magic_defense_modifiers", modifierData.Magic_Damage_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = {1},", "stat_modifiers", modifierData.Stat_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = {1},", "status_effect_modifiers", modifierData.Status_Effect_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = {1},", "nullify_status_effect_modifiers", modifierData.Nullify_Status_Effect_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = {1},", "nonbattle_modifiers", modifierData.NonBattle_Modifiers.GetValueOrDefault(0)) +
				            
				            String.Format("{0} = {1},", "special_modifiers", modifierData.Special_Modifiers.GetValueOrDefault(0)) +
				            String.Format("{0} = '{1}',", "function_ptr", modifierData.Function_PTR) +
				            String.Format("{0} = {1},", "beffect", modifierData.bEffect) +
				            String.Format("{0} = '{1}'", "skills_fk", modifierData.Skills_FK) +

				            String.Format("WHERE id='{0}'", modifierData.Id);



				IEnumerable<ModifierData> varlist = _sqlite_conn.Query<ModifierData>(Createsql.ToString());
				int i = 0;
				i++;

				//foreach (ModifierData md in varlist.ToList())
				//{
				//	CurrenGameplayModifiersInDatabase.Add(md);
				//}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
				GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [gameplay modifier] failed: {0}", ex.Message);
			}
			finally
			{
				//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
				GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				GameplayModifierName_CB.SelectedIndex = absindex;
			}

		}

		private void AddEffectToWeapon_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = WeaponEffects_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if(WeaponEffectEquip_Add_IC.Items.Count >= 3) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Effects[WeaponEffects_Add_CB.SelectedIndex].Id;
					if (!WeaponEffectEquip_Add_IC.Items.Contains(effectname))
					{
						WeaponEffectEquip_Add_IC.Items.Add(
							effectname
						);
						WeaponEffectEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void AddEffectToWeapon_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = WeaponEffects_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (WeaponEffectEquip_Edit_IC.Items.Count >= 3) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Effects[WeaponEffects_Edit_CB.SelectedIndex].Id;
					if (!WeaponEffectEquip_Edit_IC.Items.Contains(effectname))
					{
						WeaponEffectEquip_Edit_IC.Items.Add(effectname);
						CurrentWeaponsInDatabase[WeaponName_Edit_CB.SelectedIndex].Effects.Add(
							CurrenGameplayModifiersInDatabase_Effects[WeaponEffects_Edit_CB.SelectedIndex]
							);
						WeaponEffectEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void AddTraitToWeapon_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = WeaponTraits_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (WeaponTraitsEquip_Add_IC.Items.Count >= 3) return;
				else
				{
					string traitname = CurrenGameplayModifiersInDatabase_Traits[WeaponTraits_Add_CB.SelectedIndex].Id;
					if (!WeaponTraitsEquip_Add_IC.Items.Contains(traitname))
					{
						WeaponTraitsEquip_Add_IC.Items.Add(
							traitname
						); WeaponTraitsEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void AddTraitToWeapon_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = WeaponTraits_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (WeaponTraitsEquip_Edit_IC.Items.Count >= 3) return;
				else
				{
					string traitname = CurrenGameplayModifiersInDatabase_Traits[WeaponTraits_Edit_CB.SelectedIndex].Id;
					if (!WeaponTraitsEquip_Edit_IC.Items.Contains(traitname))
					{
						WeaponTraitsEquip_Edit_IC.Items.Add(traitname);

						CurrentWeaponsInDatabase[WeaponName_Edit_CB.SelectedIndex].Traits.Add(
							CurrenGameplayModifiersInDatabase_Traits[WeaponTraits_Edit_CB.SelectedIndex]);
						WeaponTraitsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void AddWeaponToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			//check for validity. Just make sure data is there and or valid
			if (int.TryParse(WeapondDamage_Add_TB.Text, out int inflictval) &&
			    int.TryParse(WeapondWeight_Add_TB.Text, out int weightval) &&
					WeaponType_Add_CB.SelectedIndex > 0 &&
					WeaponRarity_Add_CB.SelectedIndex >0 &&

					int.TryParse(WeaponsMaxHP_Add_TB.Text, out int maxHpResult) &&
					int.TryParse(WeaponsMaxMP_Add_TB.Text, out int maxMPResult) &&

					int.TryParse(WeaponsAtk_Add_TB.Text, out int atkResult) &&
					int.TryParse(WeaponsDef_Add_TB.Text, out int defResult) &&
					int.TryParse(WeaponsDex_Add_TB.Text, out int dexResult) &&
					int.TryParse(WeaponsAgl_Add_TB.Text, out int aglResult) &&
					int.TryParse(WeaponsMor_Add_TB.Text, out int morResult) &&
					int.TryParse(WeaponsWis_Add_TB.Text, out int wisResult) &&
					int.TryParse(WeaponsRes_Add_TB.Text, out int resResult) &&
					int.TryParse(WeaponsLuc_Add_TB.Text, out int LucResult) &&
					int.TryParse(WeaponsRsk_Add_TB.Text, out int RskResult) &&
					int.TryParse(WeaponsItl_Add_TB.Text, out int itlResult)
					) //  1.0.0.2v
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Base_Stats

					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					int newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

					//Set up the weapon object!
					Base_Stats basestat = new Base_Stats()
					{
						ID = newID_stat + 1,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					#endregion

					_sqlite_conn.Insert(basestat);

					#region Weakness and Strengths
					weaknesses_strengths weakstrToAdd = new weaknesses_strengths();
					int phyweak, phystr, magweak, magstr;

					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);
					int newID_weakstr = (wsList.Count == 0 ? 0 : wsList.Max(x => x.ID));
					weakstrToAdd.ID = newID_weakstr + 1;
					//GET THE MAGIC WEAKNESS ENUMERATED BITS
					#region Magic Weakness
					int i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)WeaponsMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddWeaponsMagWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_weaknesses = magweak;
					#endregion

					#region physical weakness
					i = 0;
					phyweak = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)WeaponsWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddWeaponsWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phyweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_weaknesses = phyweak;
					#endregion

					#region magic strength
					i = 0;
					magstr = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)WeaponsMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddWeaponsMagicStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magstr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_strengths = magstr;
					#endregion

					#region physical strength
					i = 0;
					phystr = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)WeaponsWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddWeaponsWeaknessStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phystr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_strengths = phystr;
					#endregion

					#endregion
					_sqlite_conn.Insert(weakstrToAdd);

					//Set up the weapon object!
					Weapons weapon = new Weapons()
					{
						ID = WeaponName_Add_TB.Text,
						bDamage = (bool) WeaponDamage_Add_CB.IsChecked,
						Weapon_Type = (int) ((EWeaponType)WeaponType_Add_CB.SelectedValue),
						Weight = weightval,
						Rarity = (int) ((ERarityType)WeaponRarity_Add_CB.SelectedValue),

						Stats_FK = basestat.ID,
						Weakness_Strength_FK = weakstrToAdd.ID

					};

					//GET THE MAGIC TYPE ENUMERATED BITS
					#region Magic types
					i = 0;
					int magictypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)WeaponMagicTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);

						if ((bool) (vv as CheckBox).IsChecked)
						{
							magictypesval += (int)Math.Pow(2, i);
						}

						i++; 
					}
					weapon.Elemental = magictypesval;
					#endregion
					#region Create Keys Entries
					#region Effects/Traits
					InsertRecordIntoModifierKeys(WeaponEffectEquip_Add_IC,_sqlite_conn, "weapons", weapon.ID);
					InsertRecordIntoModifierKeys(WeaponTraitsEquip_Add_IC,_sqlite_conn, "weapons", weapon.ID);
					#endregion
					#region Skills
					InsertRecordIntoSkillKeys(WeaponSkillsEquip_Add_IC, _sqlite_conn, "weapons", weapon.ID);
					#endregion
					#endregion

					//Add it to the databse
					int retval = _sqlite_conn.Insert(weapon);
					Console.WriteLine("RowID Val: {0}", retval);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [weapons] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

		private void InsertRecordIntoModifierKeys(ItemsControl IC,  SQLiteConnection _sqLiteConnection, String fromTableName, String fromrecorodID)
		{
			if (IC.Items.Count > 0)
			{
				foreach (String ename in IC.Items)
				{
					Modifier_Keys modkey = new Modifier_Keys()
					{
						Req_Table = fromTableName,
						Req_Name =  fromrecorodID,
						Modifier_ID = ename,
					};
					_sqLiteConnection.Insert(modkey);
				}
			}
		}

		private void InsertRecordIntoSkillKeys(ItemsControl IC, SQLiteConnection _sqLiteConnection, String fromTableName, String fromrecorodID)
		{
			if (IC.Items.Count > 0)
			{
				foreach (String ename in IC.Items)
				{
					Skill_Keys modkey = new Skill_Keys()
					{
						Req_Table = fromTableName,
						Req_Name = fromrecorodID,
						Skill_ID = ename,
					};
					_sqLiteConnection.Insert(modkey);
				}
			}
		}

		private void InsertRecordIntoItemKeys(ItemsControl IC, SQLiteConnection _sqLiteConnection, String fromTableName, String fromrecorodID, bool bdrop = false)
		{
			if (IC.Items.Count > 0)
			{
				foreach (String ename in IC.Items)
				{
					Item_Keys itemKey = new Item_Keys()
					{
						Req_Table = fromTableName,
						Req_Name = fromrecorodID,
						Item_ID = ename,
						bDrop = bdrop
					};
					_sqLiteConnection.Insert(itemKey);
				}
			}
		}

		private void InsertRecordIntoWeaponKeys(ItemsControl IC, SQLiteConnection _sqLiteConnection, String fromTableName, String fromrecorodID)
		{
			if (IC.Items.Count > 0)
			{
				foreach (String ename in IC.Items)
				{
					Weapon_Keys itemKey = new Weapon_Keys()
					{
						Req_Table = fromTableName,
						Req_Name = fromrecorodID,
						Weapon_ID = ename,
					};
					_sqLiteConnection.Insert(itemKey);
				}
			}
		}


		private void AddWeaponMagicTypes_CB_OnClick(object sender, RoutedEventArgs e)
		{
			
		}

		private void WeaponName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb.SelectedIndex >= 0)
			{
				WeaponEffectEquip_Edit_IC.Items.Clear();
				WeaponTraitsEquip_Edit_IC.Items.Clear();
				WeaponSkillsEquip_Edit_IC.Items.Clear();

				Weapon currentWeapon = CurrentWeaponsInDatabase[cb.SelectedIndex] ?? throw new ArgumentNullException("CurrentWeaponsInDatabase[cb.SelectedIndex]");

				WeaponDamage_Edit_CB.IsChecked = currentWeapon.bDamage;
				WeaponType_Edit_CB.SelectedIndex = currentWeapon.Weapon_Type;
				WeaponWeight_Edit_TB.Text = currentWeapon.Weight.ToString();
				WeaponRarity_Edit_CB.SelectedIndex = currentWeapon.Rarity;
				//TODO: Add the weapon weakness after we make this table work
				// WeaponWeakStrength_Edit_CB.SelectedItem =

				#region Base Stats
				String Createsql = "SELECT * FROM `base_stats`;";
				List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

				Base_Stats baseStats = bsList.Single(x => x.ID == currentWeapon.Stats_FK);
				currentWeapon.Stats = baseStats;
				#endregion
				#region Weaknesses and strengths
				Createsql = "SELECT * FROM `weaknesses_strengths`;";
				List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

				weaknesses_strengths wsData = wsList.Single(x => x.ID == currentWeapon.Weakness_Strength_FK);
				currentWeapon.WeaknessAndStrengths = wsData;
				#endregion

				#region Stats
				WeaponsMaxHP_Edit_TB.Text = currentWeapon.Stats.Max_Health.ToString();
				WeaponsMaxMP_Edit_TB.Text = currentWeapon.Stats.Max_Mana.ToString();


				WeaponsAtk_Edit_TB.Text = currentWeapon.Stats.Attack.ToString();
				WeaponsDef_Edit_TB.Text = currentWeapon.Stats.Defense.ToString();
				WeaponsDex_Edit_TB.Text = currentWeapon.Stats.Dexterity.ToString();
				WeaponsAgl_Edit_TB.Text = currentWeapon.Stats.Agility.ToString();
				WeaponsMor_Edit_TB.Text = currentWeapon.Stats.Morality.ToString();

				WeaponsWis_Edit_TB.Text = currentWeapon.Stats.Wisdom.ToString();
				WeaponsRes_Edit_TB.Text = currentWeapon.Stats.Resistance.ToString();
				WeaponsLuc_Edit_TB.Text = currentWeapon.Stats.Luck.ToString();
				WeaponsRsk_Edit_TB.Text = currentWeapon.Stats.Risk.ToString();
				WeaponsItl_Edit_TB.Text = currentWeapon.Stats.Intelligence.ToString();
				#endregion

				//Check boxes time!
				#region Weaknesses and strengths

				#region Elemental "Binding"
				//reset
				SetItemControlCheckboxData(WeaponsMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddWeaponsMagWeak_CB", true);
				SetItemControlCheckboxData(WeaponsMagicWeakness_Edit_IC, currentWeapon.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddWeaponsMagWeak_CB", false);

				SetItemControlCheckboxData(WeaponsMagicStrength_Edit_IC, null, EMagicType.NONE, "AddWeaponsMagicStrength_CB", true);
				SetItemControlCheckboxData(WeaponsMagicStrength_Edit_IC, currentWeapon.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddWeaponsMagicStrength_CB", false);
				WeaponsMagicWeakness_Edit_IC.UpdateLayout();
				WeaponsMagicStrength_Edit_IC.UpdateLayout();
				#endregion

				#region Weakness & Strengths "Binding"
				//reset
				SetItemControlCheckboxData(WeaponsWeakness_Edit_IC, null, EMagicType.NONE, "AddWeaponsWeak_CB", true);
				SetItemControlCheckboxData(WeaponsWeakness_Edit_IC, currentWeapon.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddWeaponsWeak_CB", false);

				SetItemControlCheckboxData(WeaponsWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddWeaponsWeaknessStrength_CB", true);
				SetItemControlCheckboxData(WeaponsWeaponStrength_Edit_IC, currentWeapon.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddWeaponsWeaknessStrength_CB", false);
				WeaponsWeaponStrength_Edit_IC.UpdateLayout();
				WeaponsWeaponStrength_Edit_IC.UpdateLayout();
				#endregion
				#endregion


				#region Elemental "Binding"
				//reset
				SetMagicTypesData(WaponsMagicTypesEquip_Edit_IC, null, EMagicType.NONE, true);
				SetMagicTypesData(WaponsMagicTypesEquip_Edit_IC, currentWeapon.Elemental, EMagicType.NONE, false);
				WaponsMagicTypesEquip_Edit_IC.UpdateLayout();
				#endregion

				#region Effect/Traits Binding

				foreach (ModifierData modd in currentWeapon.Effects)
				{
					WeaponEffectEquip_Edit_IC.Items.Add(modd.Id);
				}
				foreach (ModifierData modd in currentWeapon.Traits)
				{
					WeaponTraitsEquip_Edit_IC.Items.Add(modd.Id);
				}
				#endregion

				#region Skills Binding

				foreach (Skill skill in currentWeapon.WeaponSkills)
				{
					WeaponSkillsEquip_Edit_IC.Items.Add(skill);
				}

				#endregion

			}
		}


		private void SetItemsTypesData(ItemsControl cuItemsControl, int? modifiervalue, Enum etype, bool bReset = false)
		{
			if (modifiervalue == null && !bReset) return;

			//set the check boxes
			int i = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter)cuItemsControl.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);
				if (bReset)
				{
					(vv as CheckBox).IsChecked = false;
				}
				else if ((en & (int)modifiervalue) > 0)
				{
					(vv as CheckBox).IsChecked = true;
				}
				else
				{
					(vv as CheckBox).IsChecked = false;
				}

				i++;
			}
		}

		private void SetMagicTypesData(ItemsControl cuItemsControl, int? modifiervalue, Enum etype, bool bReset = false)
		{
			if (modifiervalue == null && !bReset) return;

			//set the check boxes
			int i = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter)cuItemsControl.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);
				if (bReset)
				{
					(vv as CheckBox).IsChecked = false;
				}
				else if ((en & (int)modifiervalue) > 0)
				{
					(vv as CheckBox).IsChecked = true;
				}
				else
				{
					(vv as CheckBox).IsChecked = false;
				}

				i++;
			}
		}

		private void SetItemControlCheckboxData(ItemsControl cuItemsControl, int? modifiervalue, Enum etype, String CBName , bool bReset = false)
		{
			if (modifiervalue == null && !bReset) return;

			//set the check boxes
			int i = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter)cuItemsControl.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName(CBName, c);
				if (bReset)
				{
					(vv as CheckBox).IsChecked = false;
				}
				else if ((en & (int)modifiervalue) > 0)
				{
					(vv as CheckBox).IsChecked = true;
				}
				else
				{
					(vv as CheckBox).IsChecked = false;
				}

				i++;
			}
		}




		private void RemoveWeaponEffect_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = WeaponEffectEquip_Add_IC.Items.IndexOf(item);

			WeaponEffectEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveWeaponTrait_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = WeaponTraitsEquip_Add_IC.Items.IndexOf(item);

			WeaponTraitsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveWeaponEffect_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = WeaponEffectEquip_Edit_IC.Items.IndexOf(item);

			
			WeaponEffectEquip_Edit_IC.Items.RemoveAt(index);
			CurrentWeaponsInDatabase[WeaponName_Edit_CB.SelectedIndex].Effects.RemoveAt(index);
		}

		private void RemoveWeaponTrait_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = WeaponTraitsEquip_Edit_IC.Items.IndexOf(item);
			
			WeaponTraitsEquip_Edit_IC.Items.RemoveAt(index);
			CurrentWeaponsInDatabase[WeaponName_Edit_CB.SelectedIndex].Traits.RemoveAt(index);

		}

		private void EditWeaponToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			if(int.TryParse(WeapondDamage_Edit_TB.Text, out int inflictval) &&
			   int.TryParse(WeaponWeight_Edit_TB.Text, out int weightval) &&

			   int.TryParse(WeaponsMaxHP_Edit_TB.Text, out int maxHpResult) &&
			   int.TryParse(WeaponsMaxMP_Edit_TB.Text, out int maxMPResult) &&

			   int.TryParse(WeaponsAtk_Edit_TB.Text, out int atkResult) &&
			   int.TryParse(WeaponsDef_Edit_TB.Text, out int defResult) &&
			   int.TryParse(WeaponsDex_Edit_TB.Text, out int dexResult) &&
			   int.TryParse(WeaponsAgl_Edit_TB.Text, out int aglResult) &&
			   int.TryParse(WeaponsMor_Edit_TB.Text, out int morResult) &&
			   int.TryParse(WeaponsWis_Edit_TB.Text, out int wisResult) &&
			   int.TryParse(WeaponsRes_Edit_TB.Text, out int resResult) &&
			   int.TryParse(WeaponsLuc_Edit_TB.Text, out int LucResult) &&
			   int.TryParse(WeaponsRsk_Edit_TB.Text, out int RskResult) &&
			   int.TryParse(WeaponsItl_Edit_TB.Text, out int itlResult)
				 //1.0.0.2v
			)
			{ 

				//before we send the SQL update query we need to update the info in memory.
				int absindex = WeaponName_Edit_CB.SelectedIndex;
				Weapon wepdata = CurrentWeaponsInDatabase[absindex];

				wepdata.bDamage = (bool)WeaponDamage_Edit_CB.IsChecked;
				wepdata.Weapon_Type = WeaponType_Edit_CB.SelectedIndex;
				wepdata.Weight = weightval;
				wepdata.Rarity = WeaponRarity_Edit_CB.SelectedIndex;
				//TODO: Add the wepdata weakness after that table is done in this tool.
				//wepdata.Weakness_Strength_FK = 

				//GET THE MAGIC TYPE ENUMERATED BITS
				#region Magic types
				int i = 0;
				int magictypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EMagicType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)WaponsMagicTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						magictypesval += (int)Math.Pow(2, i);
					}

					i++;
				}
				wepdata.Elemental = magictypesval;
				#endregion

				
				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					//Createsql = ("SELECT * FROM `gameplay_modifiers`;");

					Createsql = "UPDATE `weapons` " +
					            "SET " +
					            String.Format("{0} = {1},", "bdamage", wepdata.bDamage) +
					            String.Format("{0} = {1},", "elemental", wepdata.Elemental.GetValueOrDefault(0)) +
					            String.Format("{0} = {1},", "weapon_type", wepdata.Weapon_Type) +
					            String.Format("{0} = {1},", "weight", wepdata.Weight) +

					            String.Format("{0} = {1},", "rarity", wepdata.Rarity) +
											String.Format("{0} = {1} ", "weakness_strength_fk", wepdata.Weakness_Strength_FK) +
					            String.Format("WHERE id='{0}'", wepdata.ID);

					IEnumerable<ModifierData> varlist = _sqlite_conn.Query<ModifierData>(Createsql);



					Base_Stats stats = (Base_Stats)CurrentWeaponsInDatabase[absindex].Stats;
					Base_Stats base_stats = new Base_Stats()
					{
						ID = stats.ID,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					Createsql = "";
					Createsql = "UPDATE `base_stats` " +
											"SET " +
											String.Format("{0} = {1},", "max_health", base_stats.Max_Health) +
											String.Format("{0} = {1},", "max_mana", base_stats.Max_Mana) +

											String.Format("{0} = {1},", "attack", base_stats.Attack) +
											String.Format("{0} = {1},", "defense", base_stats.Defense) +
											String.Format("{0} = {1},", "dexterity", base_stats.Dexterity) +
											String.Format("{0} = {1},", "agility", base_stats.Agility) +
											String.Format("{0} = {1},", "morality", base_stats.Morality) +

											String.Format("{0} = {1},", "wisdom", base_stats.Wisdom) +
											String.Format("{0} = {1},", "resistance", base_stats.Resistance) +
											String.Format("{0} = {1},", "luck", base_stats.Luck) +
											String.Format("{0} = {1},", "risk", base_stats.Risk) +
											String.Format("{0} = {1} ", "intelligence", base_stats.Intelligence) +

											String.Format("WHERE id='{0}'", base_stats.ID);
					_sqlite_conn.Query<Base_Stats>(Createsql);


					weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentWeaponsInDatabase[absindex].WeaknessAndStrengths;
					weaknessStrengths.ID = wepdata.Weakness_Strength_FK;
					weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(WeaponsMagicWeakness_Edit_IC, EMagicType.NONE, "AddWeaponsMagWeak_CB");
					weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(WeaponsMagicStrength_Edit_IC, EMagicType.NONE, "AddWeaponsMagicStrength_CB");
					weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(WeaponsWeakness_Edit_IC, EWeaponType.NONE, "AddWeaponsWeak_CB");
					weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(WeaponsWeaponStrength_Edit_IC, EWeaponType.NONE, "AddWeaponsWeaknessStrength_CB");
					Createsql = "UPDATE `weaknesses_strengths` " +
											"SET " +
											String.Format("{0} = {1},", "physical_weaknesses", weaknessStrengths.physical_weaknesses) +
											String.Format("{0} = {1},", "physical_strengths", weaknessStrengths.physical_strengths) +
											String.Format("{0} = {1},", "magic_weaknesses", weaknessStrengths.magic_weaknesses) +
											String.Format("{0} = {1} ", "magic_strengths", weaknessStrengths.magic_strengths) +

											String.Format("WHERE id='{0}'", weaknessStrengths.ID);
					_sqlite_conn.Query<weaknesses_strengths>(Createsql);


					//Delete all the associated keys
					#region Key Deletion And reinsertion
					#region Effect/Trait

					Createsql = String.Format("DELETE FROM `modifier_keys` WHERE req_name = '{0}';", wepdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete

					foreach (ModifierData mdata in wepdata.Effects)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = wepdata.ID,
							Req_Table = "weapons"
						};
						_sqlite_conn.Insert(mod_key);
					}
					foreach (ModifierData mdata in wepdata.Traits)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = wepdata.ID,
							Req_Table = "weapons"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion

					#region Skill
					Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0}';", wepdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete
					foreach (Skill skill in wepdata.WeaponSkills)
					{
						Skill_Keys mod_key = new Skill_Keys()
						{
							Skill_ID = skill.Name,
							Req_Name = wepdata.ID,
							Req_Table = "weapons"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion
					#endregion

				}
				catch (Exception ex)
				{
					Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
					GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [gameplay modifier] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

		private void itemEquipEffect_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemName_Edit_CB.SelectedIndex >= 0)
			{
				if (ItemEffectEquip_Edit_IC.Items.Count < 3)
				{
					string effectname = CurrenGameplayModifiersInDatabase_Effects[ItemEquipEffects_edit_CB.SelectedIndex].Id;
					if (!ItemEffectEquip_Edit_IC.Items.Contains(effectname))
					{
						ItemEffectEquip_Edit_IC.Items.Add(effectname);
						CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Effects.Add(
							CurrenGameplayModifiersInDatabase_Effects[ItemEquipEffects_edit_CB.SelectedIndex]
						);
						ItemEffectEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}


		private void itemEquipEffect_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemEquipEffects_Add_CB.SelectedIndex >= 0)
			{
				if (ItemEffectEquip_Add_IC.Items.Count < 3)
				{
					string traitname = CurrenGameplayModifiersInDatabase_Effects[ItemEquipEffects_Add_CB.SelectedIndex].Id;
					if (!ItemEffectEquip_Add_IC.Items.Contains(traitname))
					{
						ItemEffectEquip_Add_IC.Items.Add(traitname);
					}
				}
			}
		}

		private void RemoveItemEffect_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ItemEffectEquip_Add_IC.Items.IndexOf(item);

			ItemEffectEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveItemEffect_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ItemEffectEquip_Edit_IC.Items.IndexOf(item);

			ItemEffectEquip_Edit_IC.Items.RemoveAt(index);

			CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Effects.RemoveAt(index);

		}

		private void itemEquipTrait_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemName_Edit_CB.SelectedIndex >= 0)
			{
				if (ItemTraitsEquip_Edit_IC.Items.Count < 3)
				{
					string traitname = CurrenGameplayModifiersInDatabase_Traits[ItemEquipTraits_Edit_CB.SelectedIndex].Id;
					if (!ItemTraitsEquip_Edit_IC.Items.Contains(traitname))
					{
						ItemTraitsEquip_Edit_IC.Items.Add(traitname);
							CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Traits.Add(
							CurrenGameplayModifiersInDatabase_Traits[ItemEquipTraits_Edit_CB.SelectedIndex]
						);
						ItemTraitsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void itemEquipTrait_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (ItemEquipTraits_Add_CB.SelectedIndex >= 0)
			{
				if (ItemTraitsEquip_Add_IC.Items.Count < 3)
				{
					string traitname = CurrenGameplayModifiersInDatabase_Traits[ItemEquipTraits_Add_CB.SelectedIndex].Id;
					if (!ItemTraitsEquip_Add_IC.Items.Contains(traitname))
					{
						ItemTraitsEquip_Add_IC.Items.Add(traitname);
					}
				}
			}
		}

		private void RemoveItemTrait_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ItemTraitsEquip_Add_IC.Items.IndexOf(item);

			ItemTraitsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveItemTrait_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ItemTraitsEquip_Edit_IC.Items.IndexOf(item);

			ItemTraitsEquip_Edit_IC.Items.RemoveAt(index);

			CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex].Traits.RemoveAt(index);
		}

		//Updated this method to include the AoE and targeting variables -AM 8/29/2020 1.0.0.2v
		//Updated this method to include the function pointer name string variable -AM 9/4/2020 1.0.0.3v
		private void AddItemToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			//first up checking for validity.
			if (ItemName_Add_TB.Text.Length > 0 &&
			    ItemWeaponType_Add_CB.SelectedIndex >= 0 && ItemRarity_Add_CB.SelectedIndex >= 0 &&
			    int.TryParse(ItemAoEWidth_Add_TB.Text, out int AoE_W_Val) && int.TryParse(ItemAoEHeight_Add_TB.Text, out int AoE_H_Val) &&
					
			    int.TryParse(ItemMaxHP_Add_TB.Text, out int maxHpResult) &&
			    int.TryParse(ItemMaxMP_Add_TB.Text, out int maxMPResult) &&

			    int.TryParse(ItemAtk_Add_TB.Text, out int atkResult) &&
			    int.TryParse(ItemDef_Add_TB.Text, out int defResult) &&
			    int.TryParse(ItemDex_Add_TB.Text, out int dexResult) &&
			    int.TryParse(ItemAgl_Add_TB.Text, out int aglResult) &&
			    int.TryParse(ItemMor_Add_TB.Text, out int morResult) &&
			    int.TryParse(ItemWis_Add_TB.Text, out int wisResult) &&
			    int.TryParse(ItemRes_Add_TB.Text, out int resResult) &&
			    int.TryParse(ItemLuc_Add_TB.Text, out int LucResult) &&
			    int.TryParse(ItemRsk_Add_TB.Text, out int RskResult) &&
			    int.TryParse(ItemItl_Add_TB.Text, out int itlResult)
					) //  1.0.0.2v
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Base_Stats

					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					int newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

					//Set up the weapon object!
					Base_Stats basestat = new Base_Stats()
					{
						ID = newID_stat + 1,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					#endregion

					_sqlite_conn.Insert(basestat);

					#region Weakness and Strengths
					weaknesses_strengths weakstrToAdd = new weaknesses_strengths();
					int phyweak, phystr, magweak, magstr;

					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);
					int newID_weakstr = (wsList.Count == 0 ? 0 : wsList.Max(x => x.ID));
					weakstrToAdd.ID = newID_weakstr + 1;
					//GET THE MAGIC WEAKNESS ENUMERATED BITS
					#region Magic Weakness
					int i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ItemsMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_weaknesses = magweak;
					#endregion

					#region physical weakness
					i = 0;
					phyweak = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ItemsWeaponWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemweaponWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phyweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_weaknesses = phyweak;
					#endregion

					#region magic strength
					i = 0;
					magstr = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ItemsMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagicStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magstr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_strengths = magstr;
					#endregion

					#region physical strength
					i = 0;
					phystr = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ItemsWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemWeaknessStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phystr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_strengths = phystr;
					#endregion

					#endregion
					_sqlite_conn.Insert(weakstrToAdd);

					//Set up the weapon object!
					Items item = new Items()
					{
						ID = ItemName_Add_TB.Text,
						bDamage = (bool)ItemIsDamage_Add_CB.IsChecked,
						Weapon_Type = (int)((EWeaponType)ItemWeaponType_Add_CB.SelectedValue),
						Rarity = (int)((ERarityType)ItemRarity_Add_CB.SelectedValue),
						bAllies = (bool)ItemAllies_Add_CB.IsChecked, //1.0.0.2v
						AoE_W = AoE_W_Val, //1.0.0.2v
						AoE_H = AoE_H_Val, //1.0.0.2v
						Function_PTR = ItemFuncPTR_Add_TB.Text, //1.0.0.3v

						Weakness_Strength_FK = weakstrToAdd.ID,
						Stats_FK = basestat.ID
					};

					//GET THE MAGIC TYPE ENUMERATED BITS
					#region Magic types
					i = 0;
					int magictypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ItemMagicTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagicTypes_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magictypesval += (int)Math.Pow(2, i);
						}
						i++;
					}
					item.Elemental = magictypesval;
					#endregion

					#region Item Types
					i = 0;
					int itemstypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EItemType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ItemTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							itemstypesval += (int)Math.Pow(2, i);
						}
						i++;
					}
					item.Item_Type = itemstypesval;
					#endregion

					#region Create Keys Entries
					#region Effects/Traits
					InsertRecordIntoModifierKeys(ItemEffectEquip_Add_IC, _sqlite_conn, "items", item.ID);
					InsertRecordIntoModifierKeys(ItemTraitsEquip_Add_IC, _sqlite_conn, "items", item.ID);
					#endregion
					#region Skills
					InsertRecordIntoSkillKeys(WeaponSkillsEquip_Add_IC, _sqlite_conn, "items", item.ID);
					#endregion
					#endregion

					//Add it to the databse
					int retval = _sqlite_conn.Insert(item);
					Console.WriteLine("RowID Val: {0}", retval);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [items] FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [items] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

		//Updated this method to include the function pointer, AoE, and allies -AM 9/4/2020 1.0.0.3v
		private void ItemName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//We need to populate the data to the GUI.
			Item currentItem = CurrentItemsInDatabase[ItemName_Edit_CB.SelectedIndex];
			ItemIsDamage_Edit_CB.IsChecked = currentItem.bDamage;
			ItemWeaponType_Edit_CB.SelectedItem = (EWeaponType)currentItem.Weapon_Type;
			ItemRarity_Edit_CB.SelectedItem = (ERarityType) currentItem.Rarity;
			ItemAoEWidth_Edit_TB.Text = currentItem.AoE_W.ToString();		//1.0.0.3v
			ItemAoEHeight_Edit_TB.Text = currentItem.AoE_H.ToString();	//1.0.0.3v
			ItemFuncPTR_Edit_TB.Text = currentItem.Function_PTR;				//1.0.0.3v
			ItemAllies_Edit_CB.IsChecked = currentItem.bAllies;         //1.0.0.3v

			#region Base Stats
			String Createsql = "SELECT * FROM `base_stats`;";
			List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

			Base_Stats baseStats = bsList.Single(x => x.ID == currentItem.Stats_FK);
			currentItem.Stats = baseStats;
			#endregion
			#region Weaknesses and strengths
			Createsql = "SELECT * FROM `weaknesses_strengths`;";
			List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

			weaknesses_strengths wsData = wsList.Single(x => x.ID == currentItem.Weakness_Strength_FK);
			currentItem.WeaknessAndStrengths = wsData;
			#endregion

			#region Stats
			ItemMaxHP_Edit_TB.Text = currentItem.Stats.Max_Health.ToString();
			ItemMaxMP_Edit_TB.Text = currentItem.Stats.Max_Mana.ToString();


			ItemAtk_Edit_TB.Text = currentItem.Stats.Attack.ToString();
			ItemDef_Edit_TB.Text = currentItem.Stats.Defense.ToString();
			ItemDex_Edit_TB.Text = currentItem.Stats.Dexterity.ToString();
			ItemAgl_Edit_TB.Text = currentItem.Stats.Agility.ToString();
			ItemMor_Edit_TB.Text = currentItem.Stats.Morality.ToString();

			ItemWis_Edit_TB.Text = currentItem.Stats.Wisdom.ToString();
			ItemRes_Edit_TB.Text = currentItem.Stats.Resistance.ToString();
			ItemLuc_Edit_TB.Text = currentItem.Stats.Luck.ToString();
			ItemRsk_Edit_TB.Text = currentItem.Stats.Risk.ToString();
			ItemItl_Edit_TB.Text = currentItem.Stats.Intelligence.ToString();
			#endregion

			//Check boxes time!
			#region Weaknesses and strengths

			#region Elemental "Binding"
			//reset
			SetItemControlCheckboxData(ItemsMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddItemMagWeak_CB", true);
			SetItemControlCheckboxData(ItemsMagicWeakness_Edit_IC, currentItem.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddItemMagWeak_CB", false);

			SetItemControlCheckboxData(ItemsMagicStrength_Edit_IC, null, EMagicType.NONE, "AddItemMagicStrength_CB", true);
			SetItemControlCheckboxData(ItemsMagicStrength_Edit_IC, currentItem.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddItemMagicStrength_CB", false);
			ItemsMagicWeakness_Edit_IC.UpdateLayout();
			ItemsMagicStrength_Edit_IC.UpdateLayout();
			#endregion

			#region Weakness & Strengths "Binding"
			//reset
			SetItemControlCheckboxData(ItemsWeaponWeakness_Edit_IC, null, EMagicType.NONE, "AddItemweaponWeak_CB", true);
			SetItemControlCheckboxData(ItemsWeaponWeakness_Edit_IC, currentItem.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddItemweaponWeak_CB", false);

			SetItemControlCheckboxData(ItemsWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddItemWeaknessStrength_CB", true);
			SetItemControlCheckboxData(ItemsWeaponStrength_Edit_IC, currentItem.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddItemWeaknessStrength_CB", false);
			ItemsWeaponStrength_Edit_IC.UpdateLayout();
			ItemsWeaponStrength_Edit_IC.UpdateLayout();
			#endregion

			#endregion



			#region Item Types
			SetItemsTypesData(ItemTypesEquip_Edit_IC, null, EItemType.NONE, true);
			SetItemsTypesData(ItemTypesEquip_Edit_IC, currentItem.Item_Type, EItemType.NONE, false);
			#endregion

			#region Elemental "Binding"
			//reset
			SetMagicTypesData(ItemMagicTypesEquip_Edit_IC, null, EMagicType.NONE, true);
			SetMagicTypesData(ItemMagicTypesEquip_Edit_IC, currentItem.Elemental, EMagicType.NONE, false);
			WaponsMagicTypesEquip_Edit_IC.UpdateLayout();
			#endregion

			


			#region Effect/Traits Binding

			foreach (ModifierData modd in currentItem.Effects)
			{
				ItemEffectEquip_Edit_IC.Items.Add(modd.Id);
			}
			foreach (ModifierData modd in currentItem.Traits)
			{
				ItemTraitsEquip_Edit_IC.Items.Add(modd.Id);
			}
			#endregion

			#region Skills Binding

			foreach (Skill skill in currentItem.ItemSkills)
			{
				itemSkillsEquip_Edit_IC.Items.Add(skill);
			}

			#endregion

		}

		/// Updated to include AoE, and target side variables -AM 8/29/2020 1.0.0.2v
		/// Updated this method to include the function pointer name string variable -AM 9/4/2020 1.0.0.3v
		private void UpdateItemInDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (
			    int.TryParse(ItemAoEWidth_Edit_TB.Text, out int AoE_W_Val) && //1.0.0.2v
					int.TryParse(ItemAoEHeight_Edit_TB.Text, out int AoE_H_Val) &&
			    int.TryParse(ItemMaxHP_Edit_TB.Text, out int maxHpResult) &&
			    int.TryParse(ItemMaxMP_Edit_TB.Text, out int maxMPResult) &&

			    int.TryParse(ItemAtk_Edit_TB.Text, out int atkResult) &&
					int.TryParse(ItemDef_Edit_TB.Text, out int defResult) &&
					int.TryParse(ItemDex_Edit_TB.Text, out int dexResult) &&
					int.TryParse(ItemAgl_Edit_TB.Text, out int aglResult) &&
					int.TryParse(ItemMor_Edit_TB.Text, out int morResult) &&
					int.TryParse(ItemWis_Edit_TB.Text, out int wisResult) &&
					int.TryParse(ItemRes_Edit_TB.Text, out int resResult) &&
					int.TryParse(ItemLuc_Edit_TB.Text, out int LucResult) &&
					int.TryParse(ItemRsk_Edit_TB.Text, out int RskResult) &&
					int.TryParse(ItemItl_Edit_TB.Text, out int itlResult)
			)
			{

				//before we send the SQL update query we need to update the info in memory.
				int absindex = ItemName_Edit_CB.SelectedIndex;
				Item itemdata = CurrentItemsInDatabase[absindex];

				itemdata.bDamage = (bool)ItemIsDamage_Edit_CB.IsChecked;
				itemdata.Weapon_Type = ItemWeaponType_Edit_CB.SelectedIndex;
				itemdata.Rarity = ItemRarity_Edit_CB.SelectedIndex;
				itemdata.AoE_H = AoE_H_Val; //1.0.0.2v
				itemdata.AoE_W = AoE_W_Val; //1.0.0.2v
				itemdata.Function_PTR = ItemFuncPTR_Edit_TB.Text; // 1.0.0.3v
				itemdata.bAllies = (bool)ItemAllies_Edit_CB.IsChecked; //1.0.0.2v
				//TODO: Add the wepdata weakness after that table is done in this tool.
				//itemdata.Weakness_Strength_FK = 

				//GET THE MAGIC TYPE ENUMERATED BITS
				#region Magic types
				int i = 0;
				int magictypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EMagicType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)ItemMagicTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						magictypesval += (int)Math.Pow(2, i);
					}

					i++;
				}
				itemdata.Elemental = magictypesval;
				#endregion

				#region Item Types
				i = 0;
				int itemstypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EItemType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)ItemTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						itemstypesval += (int)Math.Pow(2, i);
					}
					i++;
				}
				itemdata.Item_Type = itemstypesval;
				#endregion

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					//Createsql = ("SELECT * FROM `gameplay_modifiers`;");

					Createsql = "UPDATE `items` " +
											"SET " +
											String.Format("{0} = {1},", "bdamage", itemdata.bDamage) +
											String.Format("{0} = {1},", "elemental", itemdata.Elemental) +
											String.Format("{0} = {1},", "weapon_type", itemdata.Weapon_Type) +
											String.Format("{0} = {1},", "item_type", itemdata.Item_Type) +
											String.Format("{0} = {1},", "aoe_w", itemdata.AoE_W) + //1.0.0.2v
											String.Format("{0} = {1},", "aoe_h", itemdata.AoE_H) + //1.0.0.2v
											String.Format("{0} = {1},", "ballies", itemdata.bAllies) + //1.0.0.2v
											String.Format("{0} = '{1}',", "function_ptr", itemdata.Function_PTR) + //1.0.0.3v

											String.Format("{0} = {1},", "rarity", itemdata.Rarity) +
											String.Format("{0} = {1} ", "weakness_strength_fk", itemdata.Weakness_Strength_FK) +
											String.Format("WHERE id='{0}'", itemdata.ID);
					_sqlite_conn.Query<Item>(Createsql);



					Base_Stats stats = (Base_Stats)CurrentItemsInDatabase[absindex].Stats;
					Base_Stats base_stats = new Base_Stats()
					{
						ID = stats.ID,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					Createsql = "";
						Createsql = "UPDATE `base_stats` " +
												"SET " +
												String.Format("{0} = {1},", "max_health", base_stats.Max_Health) +
												String.Format("{0} = {1},", "current_health", base_stats.Current_Health) +
												String.Format("{0} = {1},", "max_mana", base_stats.Max_Mana) +
												String.Format("{0} = {1},", "current_mana", base_stats.Current_Mana) +

												String.Format("{0} = {1},", "attack", base_stats.Attack) +
												String.Format("{0} = {1},", "defense", base_stats.Defense) +
												String.Format("{0} = {1},", "dexterity", base_stats.Dexterity) +
												String.Format("{0} = {1},", "agility", base_stats.Agility) +
												String.Format("{0} = {1},", "morality", base_stats.Morality) +

												String.Format("{0} = {1},", "wisdom", base_stats.Wisdom) +
												String.Format("{0} = {1},", "resistance", base_stats.Resistance) +
												String.Format("{0} = {1},", "luck", base_stats.Luck) +
												String.Format("{0} = {1},", "risk", base_stats.Risk) +
												String.Format("{0} = {1} ", "intelligence", base_stats.Intelligence) +

												String.Format("WHERE id='{0}'", base_stats.ID);
						_sqlite_conn.Query<Base_Stats>(Createsql);


						weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentItemsInDatabase[absindex].WeaknessAndStrengths;
						weaknessStrengths.ID = itemdata.Weakness_Strength_FK;
						weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(ItemsMagicWeakness_Edit_IC, EMagicType.NONE, "AddItemMagWeak_CB");
						weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(ItemsMagicStrength_Edit_IC, EMagicType.NONE, "AddItemMagicStrength_CB");
						weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(ItemsWeaponWeakness_Edit_IC, EWeaponType.NONE, "AddItemweaponWeak_CB");
						weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(ItemsWeaponStrength_Edit_IC, EWeaponType.NONE, "AddItemWeaknessStrength_CB");
						Createsql = "UPDATE `weaknesses_strengths` " +
						            "SET " +
						            String.Format("{0} = {1},", "physical_weaknesses", weaknessStrengths.physical_weaknesses) +
						            String.Format("{0} = {1},", "physical_strengths", weaknessStrengths.physical_strengths) +
						            String.Format("{0} = {1},", "magic_weaknesses", weaknessStrengths.magic_weaknesses) +
						            String.Format("{0} = {1} ", "magic_strengths", weaknessStrengths.magic_strengths) +

						            String.Format("WHERE id='{0}'", weaknessStrengths.ID);
						_sqlite_conn.Query<weaknesses_strengths>(Createsql);


						//Delete all the associated keys
						#region Key Deletion And reinsertion
						#region Effect/Trait

						Createsql = String.Format("DELETE FROM `modifier_keys` WHERE req_name = '{0}';", itemdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete

					foreach (ModifierData mdata in itemdata.Effects)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = itemdata.ID,
							Req_Table = "items"
						};
						_sqlite_conn.Insert(mod_key);
					}
					foreach (ModifierData mdata in itemdata.Traits)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = itemdata.ID,
							Req_Table = "items"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion

					#region Skill
					Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0}';", itemdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete
					foreach (Skill skill in itemdata.ItemSkills)
					{
						Skill_Keys mod_key = new Skill_Keys()
						{
							Skill_ID = skill.Name,
							Req_Name = itemdata.ID,
							Req_Table = "items"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion
					#endregion

				}
				catch (Exception ex)
				{
					Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
					GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [gameplay modifier] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}


		private void SkillLinkedModifier_Add_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox CB = sender as ComboBox;
			if (CB.SelectedIndex != -1)
			{
				if (CurrenGameplayModifiersInDatabase.Contains(CurrenGameplayModifiersInDatabase[CB.SelectedIndex]))
				{
					ModifierData mdata = CurrenGameplayModifiersInDatabase[CB.SelectedIndex];


					//DisplayGameplayModifiersToIC(SkillAllMods_Add_IC, mdata, SkillsCurrentLinkedModifiers_Add_AllMods, true);
					DisplayGameplayModifiersToIC(SkillAllMods_Add_IC, mdata, SkillsCurrentLinkedModifiers_Add_AllMods, false);
				}
			}
		}

		private void DisplayGameplayModifiersToIC(ItemsControl desItemsControl, ModifierData mdata, ObservableCollection<Tuple<String, String>> CollectionToBind ,bool bReset = false)
		{
			//clear ptr and data to avoid runtime errors
			desItemsControl.ItemsSource = null;
			CollectionToBind.Clear();

			AddModifierDataTextblockToCollection(mdata, CollectionToBind);
			
			desItemsControl.ItemsSource = CollectionToBind;
			desItemsControl.UpdateLayout();
		}

		private void AddModifierDataTextblockToCollection(ModifierData mdata ,ObservableCollection<Tuple<String, String>> CollectionToBind)
		{
			//set the check boxes
			int i = 0;
			Enum etype = EChanceEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Chance_Modifiers != null && (en & (int)mdata.Chance_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ETurnEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Turn_Modifiers != null && (en & (int)mdata.Turn_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = EDamageModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Damage_Modifiers != null && (en & (int)mdata.Damage_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ESeverityEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Severity_Modifiers != null && (en & (int)mdata.Severity_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}


			i = 0;
			etype = EMagicDamageModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Magic_Damage_Modifiers != null && (en & (int)mdata.Magic_Damage_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = EMagicDefenseModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Magic_Defense_Modifiers != null && (en & (int)mdata.Magic_Defense_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = EStatEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Stat_Modifiers != null && (en & (int)mdata.Stat_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = EStatusEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Status_Effect_Modifiers != null && (en & (int)mdata.Status_Effect_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ENullifyStatusEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Nullify_Status_Effect_Modifiers != null && (en & (int)mdata.Nullify_Status_Effect_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ENonBattleEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.NonBattle_Modifiers != null && (en & (int)mdata.NonBattle_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}

			i = 0;
			etype = ESpecialEffectModifiers.NONE;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				else if (mdata.Special_Modifiers != null && (en & (int)mdata.Special_Modifiers) > 0)
				{
					//This is here to display ALL the data to the user at once in an items control
					CollectionToBind.Add(new Tuple<String, String>
					(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
				}
				i++;
			}


		}

		//Upadted  this method to include the new AoE and targeting variables -AM 8/29/2020 1.0.0.2v
		private void AddSkillToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			//first up checking for validity.
			if (SkillsName_Add_TB.Text.Length > 0 && double.TryParse(SkillDamageMultiplier_Add_TB.Text, out double damageMulti) &&
			    SkillWeaponType_Add_CB.SelectedIndex >= 0 &&
			    int.TryParse(SkillAoEWidth_Add_TB.Text, out int AoE_W_Val) &&  //1.0.0.2v
					int.TryParse(SkillAoEHeight_Add_TB.Text, out int AoE_H_Val)) //1.0.0.2v
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					//Set up the weapon object!
					Skills skill = new Skills()
					{
						Name = SkillsName_Add_TB.Text,
						bDamage = (bool)SkillIsPhysical_Add_CB.IsChecked,
						Damage_Multiplier = damageMulti,
						Weapon_Type = (int)((EWeaponType)SkillWeaponType_Add_CB.SelectedValue),
						bPhys = (bool)SkillIsDamage_Add_CB.IsChecked,
						bAllies =  (bool)SkillAllies_Add_CB.IsChecked, //1.0.0.2v
						AOE_w = AoE_W_Val, //1.0.0.2v
						AOE_h = AoE_H_Val, //1.0.0.2v

					};
					if (SkillLinkedModifier_Add_CB.SelectedIndex >= 0)
					{
						skill.Modifier_FK = ((ModifierData) SkillLinkedModifier_Add_CB.SelectedValue).Id;
					}
					//GET THE MAGIC TYPE ENUMERATED BITS
					#region Magic types
					int i = 0;
					int magictypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)SkillMagicTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddSkillMagicTypes_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magictypesval += (int)Math.Pow(2, i);
						}
						i++;
					}
					skill.Elemental = magictypesval;
					#endregion

					//#region Create Keys Entries
					//#region Effects/Traits
					//InsertRecordIntoModifierKeys(ItemEffectEquip_Add_IC, _sqlite_conn, "skills", skill.Name);
					//InsertRecordIntoModifierKeys(ItemTraitsEquip_Add_IC, _sqlite_conn, "skills", skill.Name);
					//#endregion
					//#region Skills
					//InsertRecordIntoSkillKeys(WeaponSkillsEquip_Add_IC, _sqlite_conn, "skills", skill.Name);
					//#endregion
					//#endregion

					//Add it to the databse
					int retval = _sqlite_conn.Insert(skill);
					Console.WriteLine("RowID Val: {0}", retval);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [Skills] FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [Skills] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}


		}


		// Updated to include the AoE, and the allies targeting vars -AM 9/4/2020
		private void SkillsName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox CB = sender as ComboBox;
			if (CB.SelectedIndex >= 0)
			{
				Skill currentSkill = CurrentSkillsInDatabase[CB.SelectedIndex];

				//Set up the GUI with the chosen entries data
				SkillIsPhysical_Edit_CB.IsChecked = currentSkill.bPhys;
				SkillIsDamage_Edit_CB.IsChecked = currentSkill.bDamage;
				SkillDamageMultiplier_Edit_TB.Text = currentSkill.Damage_Multiplier.ToString();
				SkillWeaponType_Edit_CB.SelectedValue = ((EWeaponType) currentSkill.Weapon_Type);
				SkillAoEWidth_Edit_TB.Text = currentSkill.AOE_w.ToString();			//1.0.0.3v
				SkillAoEHeight_Edit_TB.Text = currentSkill.AOE_h.ToString();		//1.0.0.3v
				SkillAllies_Edit_CB.IsChecked = currentSkill.bAllies;						//1.0.0.3v
	
				//Show the Function pointer function name
				SkillFuncPTR_Edit_TB.Text = currentSkill.Function_PTR;

				if (currentSkill.Modifier_FK != null && currentSkill.Modifier_FK != "") //ONLY SHOW modifiers if they EXIST
				{
					SkillLinkedModifier_Edit_CB.SelectedIndex =
						(CurrenGameplayModifiersInDatabase.IndexOf(
							CurrenGameplayModifiersInDatabase.Single(x => x.Id == currentSkill.Modifier_FK)
						));

					DisplayGameplayModifiersToIC(SkillAllMods_Edit_IC, (ModifierData) SkillLinkedModifier_Edit_CB.SelectedValue,
						SkillsCurrentLinkedModifiers_Edit_AllMods);
				}
				else
				{
					SkillsCurrentLinkedModifiers_Edit_AllMods.Clear();
				}

				#region Elemental "Binding"
				//reset
				SetMagicTypesData(SkillMagicTypesEquip_Edit_IC, null, EMagicType.NONE, true);
				SetMagicTypesData(SkillMagicTypesEquip_Edit_IC, currentSkill.Elemental, EMagicType.NONE, false);
				SkillMagicTypesEquip_Edit_IC.UpdateLayout();
				#endregion

			}
		}

		private void SkillLinkedModifier_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox CB = sender as ComboBox;
			if (CB.SelectedIndex != -1)
			{
				if (CurrenGameplayModifiersInDatabase.Contains(CurrenGameplayModifiersInDatabase[CB.SelectedIndex]))
				{
					ModifierData mdata = CurrenGameplayModifiersInDatabase[CB.SelectedIndex];

					//DisplayGameplayModifiersToIC(SkillAllMods_Add_IC, mdata, SkillsCurrentLinkedModifiers_Add_AllMods, true);
					DisplayGameplayModifiersToIC(SkillAllMods_Edit_IC, mdata, SkillsCurrentLinkedModifiers_Edit_AllMods, false);
				}
			}
		}

		//Updated this method to include the new AoE and targeting variables -AM 8/29/2020 1.0.0.2v
		private void UpdateSkillToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{

			if (double.TryParse(SkillDamageMultiplier_Edit_TB.Text, out double damageMulti) &&
			    int.TryParse(SkillAoEWidth_Edit_TB.Text, out int AoE_W_Val) && //1.0.0.2v
					int.TryParse(SkillAoEHeight_Edit_TB.Text, out int AoE_H_Val)) //1.0.0.2v
			{
				//before we send the SQL update query we need to update the info in memory.
				int absindex = SkillsName_Edit_CB.SelectedIndex;
				Skills skilldata = (Skills)CurrentSkillsInDatabase[absindex];

				skilldata.bDamage = (bool)SkillIsDamage_Edit_CB.IsChecked;
				skilldata.bPhys = (bool) SkillIsPhysical_Edit_CB.IsChecked;
				skilldata.Damage_Multiplier = damageMulti;
				skilldata.Weapon_Type = (int)(EWeaponType)SkillWeaponType_Edit_CB.SelectedItem;
				if(((ModifierData)SkillLinkedModifier_Edit_CB.SelectedValue) != null)
					skilldata.Modifier_FK = ((ModifierData)SkillLinkedModifier_Edit_CB.SelectedValue).Id;
				skilldata.Function_PTR = SkillFuncPTR_Edit_TB.Text;
				skilldata.AOE_h = AoE_H_Val; //1.0.0.2v
				skilldata.AOE_w = AoE_W_Val; //1.0.0.2v
				skilldata.bAllies = (bool)SkillAllies_Edit_CB.IsChecked; //1.0.0.2v

				//GET THE MAGIC TYPE ENUMERATED BITS
				#region Magic types
				int i = 0;
				int magictypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EMagicType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)SkillMagicTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						magictypesval += (int)Math.Pow(2, i);
					}

					i++;
				}
				skilldata.Elemental = magictypesval;
				#endregion

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					Createsql = "UPDATE `skills` " +
											"SET " +
											String.Format("{0} = {1},", "bphys", skilldata.bPhys) +
											String.Format("{0} = {1},", "bdamage", skilldata.bDamage) +
											String.Format("{0} = {1},", "damage_multiplier", skilldata.Damage_Multiplier) +
											String.Format("{0} = {1},", "elemental", skilldata.Elemental) +
											String.Format("{0} = {1},", "weapon_type", skilldata.Weapon_Type) +
											String.Format("{0} = '{1}',", "function_ptr", skilldata.Function_PTR) +
											String.Format("{0} = {1},", "aoe_w", skilldata.AOE_w) + //1.0.0.2v
											String.Format("{0} = {1},", "aoe_h", skilldata.AOE_h) + //1.0.0.2v
											String.Format("{0} = {1},", "ballies", skilldata.bAllies) + //1.0.0.2v

											String.Format("{0} = '{1}' ", "modifier_fk", skilldata.Modifier_FK) +
											String.Format("WHERE name='{0}'", skilldata.Name);
					_sqlite_conn.Query<Skills>(Createsql);

					GlobalStatusLog_TB.Text = "Successfully Added to the Skills Table";
				}
				catch (Exception ex)
				{
					Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
					GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [skills] failed: {0}", ex.Message);
				}
				finally
				
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
					
				}
			}


		}

		private void PartymemberSkillsToIC_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberSkills_Add_IC.Items.Count <= 6)
			{
				PartyMemberSkills_Add_IC.Items.Add(((Skill) PartymemberSkills_Add_CB.SelectedValue).Name);
			}
		}

		private void PartymemberSkillsToIC_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberSkills_Edit_IC.Items.Count <= 6)
			{
				PartyMemberSkills_Edit_IC.Items.Add(((Skill)PartymemberSkills_Edit_CB.SelectedValue).Name);
				CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Skills.Add((Skill)PartymemberSkills_Edit_CB.SelectedValue);
			}
		}

		private void RemovePartMemberSkill_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberSkills_Add_IC.Items.IndexOf(item);

			PartyMemberSkills_Add_IC.Items.RemoveAt(index);
		}

		private void RemovePartMemberSkill_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberSkills_Edit_IC.Items.IndexOf(item);

			PartyMemberSkills_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Skills.RemoveAt(index);
		}

		private void PartyMemberItem_Add_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberItems_Add_IC.Items.Count <= 6)
			{
				PartyMemberItems_Add_IC.Items.Add(((Item)PartyMemberItems_Add_CB.SelectedValue).ID);
			}

		}

		private void PartyMemberItem_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberItems_Edit_IC.Items.Count <= 6)
			{
				PartyMemberItems_Edit_IC.Items.Add(((Item)PartyMemberItems_Edit_CB.SelectedValue).ID);
				CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Items.Add((Item)PartyMemberItems_Edit_CB.SelectedValue);
			}

		}

		private void RemovePartyMemberItem_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberItems_Add_IC.Items.IndexOf(item);

			PartyMemberItems_Add_IC.Items.RemoveAt(index);
		}

		private void RemovePartyMemberItem_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberItems_Edit_IC.Items.IndexOf(item);

			PartyMemberItems_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Items.RemoveAt(index);
		}


		private void PartyMemberWeapon_Add_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberWeapon_Add_IC.Items.Count <= 3)
			{
				PartyMemberWeapon_Add_IC.Items.Add(((Weapon)PartyMemberWeapons_Add_CB.SelectedValue).ID);
			}
		}

		private void PartyMemberWeapon_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			ComboBox CB = sender as ComboBox;
			if (PartyMemberWeapon_Edit_IC.Items.Count <= 3)
			{
				PartyMemberWeapon_Edit_IC.Items.Add(((Weapon)PartyMemberWeapons_Edit_CB.SelectedValue).ID);
				CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Weapons.AddLast((Weapon)PartyMemberWeapons_Edit_CB.SelectedValue);
			}
		}


		private void RemovePartyMemberWeapon_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberWeapon_Add_IC.Items.IndexOf(item);

			PartyMemberWeapon_Add_IC.Items.RemoveAt(index);
		}


		private void RemovePartyMemberWeapon_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = PartyMemberWeapon_Edit_IC.Items.IndexOf(item);

			PartyMemberWeapon_Edit_IC.Items.RemoveAt(index);

			//edit the live data.
			CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Weapons.Remove(
				CurrentPartyMembersInDatabase[PartyMemberName_Edit_CB.SelectedIndex].Weapons.ToList()[index]);
		}


		private void PartyMemberAddToDatabase_Add_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			//first up checking for validity.
			if (
				PartyMemberFirstName_Add_TB.Text.Length > 0 &&
				PartyMemberLastName_Add_TB.Text.Length > 0 &&
				int.TryParse(PartyMemberFriendPoints_Add_TB.Text, out int fpResult) &&
				int.TryParse(PartyMemberCombatLevel_Add_TB.Text, out int combatlevelResult) &&

				int.TryParse(PartyMemberMaxHP_Add_TB.Text, out int maxHpResult) &&
				int.TryParse(PartyMemberCurHP_Add_TB.Text, out int curHpResult) &&
				int.TryParse(PartyMemberMaxMP_Add_TB.Text, out int maxMPResult) &&
				int.TryParse(PartyMemberCurMP_Add_TB.Text, out int curMPResult) &&

				int.TryParse(PartyMemberAtk_Add_TB.Text, out int atkResult) &&
				int.TryParse(PartyMemberDef_Add_TB.Text, out int defResult) &&
				int.TryParse(PartyMemberDex_Add_TB.Text, out int dexResult) &&
				int.TryParse(PartyMemberAgl_Add_TB.Text, out int aglResult) &&
				int.TryParse(PartyMemberMor_Add_TB.Text, out int morResult) &&
				int.TryParse(PartyMemberWis_Add_TB.Text, out int wisResult) &&
				int.TryParse(PartyMemberRes_Add_TB.Text, out int resResult) &&
				int.TryParse(PartyMemberLuc_Add_TB.Text, out int LucResult) &&
				int.TryParse(PartyMemberRsk_Add_TB.Text, out int RskResult) &&
				int.TryParse(PartyMemberItl_Add_TB.Text, out int itlResult)
			)
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					int newID_stat = 0;
					int newID_weakstr = 0;
					
					#region Base_Stats
					
					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

					//Set up the weapon object!
					Base_Stats basestat = new Base_Stats()
					{
						ID = newID_stat+1,
						Max_Health = maxHpResult,
						Current_Health = curHpResult,
						Max_Mana = maxMPResult,
						Current_Mana = curMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					#endregion

					_sqlite_conn.Insert(basestat);

					#region Weakness and Strengths
					weaknesses_strengths weakstrToAdd = new weaknesses_strengths();
					int phyweak, phystr, magweak, magstr;

					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);
					newID_weakstr = (wsList.Count == 0 ? 0 : wsList.Max(x => x.ID));
					weakstrToAdd.ID = newID_weakstr+1;
					//GET THE MAGIC WEAKNESS ENUMERATED BITS
					#region Magic Weakness
					int i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)PartyMembersMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddPartyMemberMagWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_weaknesses = magweak;
					#endregion

					#region physical weakness
					i = 0;
					phyweak = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)PartyMembersWeaponWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddPartyMemberweaponWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phyweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_weaknesses = phyweak;
					#endregion

					#region magic strength
					i = 0;
					magstr = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)PartyMembersMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddPartyMemberMagicStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magstr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_strengths = magstr;
					#endregion

					#region physical strength
					i = 0;
					phystr = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)PartyMembersWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddPartyMemberWeaknessStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phystr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_strengths = phystr;
					#endregion

					#endregion
					_sqlite_conn.Insert(weakstrToAdd);

					#region Party Member

					party_member partyMember = new party_member()
					{ 
						First_Name = PartyMemberFirstName_Add_TB.Text,
						Last_Name = PartyMemberLastName_Add_TB.Text,
						Friendship_Points = fpResult,
						Level = combatlevelResult,

						Stats_FK = basestat.ID,
						Weak_Strength_FK = weakstrToAdd.ID,
					};

					if (PartyMemberMainJob_Add_CB.SelectedIndex != -1)
					{
						partyMember.Main_Job_FK = ((Job) PartyMemberMainJob_Add_CB.SelectedValue).Id;
					}
					if (PartyMemberSubJob_Add_CB.SelectedIndex != -1)
					{
						partyMember.Sub_Job_FK = ((Job)PartyMemberSubJob_Add_CB.SelectedValue).Id;
					}
					#endregion
					_sqlite_conn.Insert(partyMember);

					#region Create Keys Entries
					#region Skills
					InsertRecordIntoSkillKeys(PartyMemberSkills_Add_IC, _sqlite_conn, "party_member",
						String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));
					#endregion
					#region Weapons
					InsertRecordIntoWeaponKeys(PartyMemberWeapon_Add_IC, _sqlite_conn, "party_member",
					String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));
					#endregion
					#region Items
					InsertRecordIntoItemKeys(PartyMemberItems_Add_IC, _sqlite_conn, "party_member", 
						String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));
					#endregion
					#endregion
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [items] FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [items] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}


		}

		private void PartyMemberName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox CB = sender as ComboBox;
			if (CB.SelectedIndex >= 0)
			{
				//reset the IC so we don't have false dups
				PartyMemberSkills_Edit_IC.Items.Clear();
				PartyMemberItems_Edit_IC.Items.Clear();
				PartyMemberWeapon_Edit_IC.Items.Clear();

				party_member partyMember = (party_member) CB.SelectedValue;
				Console.WriteLine(String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));
				//create a partymemeber and fill out the objects internal object references.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Base Stats
					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

					Base_Stats baseStats = bsList.Single(x => x.ID == partyMember.Stats_FK);
					partyMember.Stats = baseStats;
					#endregion
					#region Weaknesses and strengths
					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

					weaknesses_strengths wsData = wsList.Single(x => x.ID == partyMember.Weak_Strength_FK);
					partyMember.WeaknessAndStrengths = wsData;
					#endregion


					#region Skills
					Createsql = "SELECT * FROM `skill_keys`;";
					List<Skill_Keys> skillList_keys = _sqlite_conn.Query<Skill_Keys>(Createsql);
					skillList_keys = skillList_keys.FindAll(x => x.Req_Name == String.Format("{0} {1}",partyMember.First_Name, partyMember.Last_Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Skill> result_skills = CurrentSkillsInDatabase.Where(p => skillList_keys.Any(p2 => p2.Skill_ID == p.Name)).ToList() as List<Skill>;
					partyMember.Skills = result_skills;
					result_skills.ForEach(x=>PartyMemberSkills_Edit_IC.Items.Add(x.Name));
					#endregion

					#region Items
					Createsql = "SELECT * FROM `item_keys`;";
					List<Item_Keys> ItemList_keys = _sqlite_conn.Query<Item_Keys>(Createsql);
					ItemList_keys = ItemList_keys.FindAll(x => x.Req_Name == String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Item> result_items = CurrentItemsInDatabase.Where(p => ItemList_keys.Any(p2 => p2.Item_ID == p.ID)).ToList() as List<Item>;
					partyMember.Items = result_items;
					result_items.ForEach(x => PartyMemberItems_Edit_IC.Items.Add((x.ID)));
					#endregion

					#region Weapons
					Createsql = "SELECT * FROM `weapon_keys`;";
					List<Weapon_Keys> WeaponList_keys = _sqlite_conn.Query<Weapon_Keys>(Createsql);
					WeaponList_keys = WeaponList_keys.FindAll(x => x.Req_Name == String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Weapon> result_weapon = CurrentWeaponsInDatabase.Where(p => WeaponList_keys.Any(p2 => p2.Weapon_ID == p.ID)).ToList() as List<Weapon>;
					//partyMember.Weapons = result_weapon;
					foreach (Weapon weapon in result_weapon)
					{
						partyMember.Weapons.AddLast(weapon);
					}
					result_weapon.ForEach(x=> PartyMemberWeapon_Edit_IC.Items.Add((x.ID)));
					#endregion

					//Next up fill out ALL the text boxes with the correct info

					#region Party Member Data
					PartyMemberFriendPoints_Edit_TB.Text = partyMember.Friendship_Points.ToString();
					PartyMemberCombatLevel_Edit_TB.Text = partyMember.Level.ToString();
					#endregion

					#region Stats
					PartyMemberMaxHP_Edit_TB.Text = partyMember.Stats.Max_Health.ToString();
					PartyMemberCurHP_Edit_TB.Text = partyMember.Stats.Current_Health.ToString();
					PartyMemberMaxMP_Edit_TB.Text = partyMember.Stats.Max_Mana.ToString();
					PartyMemberCurMP_Edit_TB.Text = partyMember.Stats.Current_Mana.ToString();


					PartyMemberAtk_Edit_TB.Text = partyMember.Stats.Attack.ToString();
					PartyMemberDef_Edit_TB.Text = partyMember.Stats.Defense.ToString();
					PartyMemberDex_Edit_TB.Text = partyMember.Stats.Dexterity.ToString();
					PartyMemberAgl_Edit_TB.Text = partyMember.Stats.Agility.ToString();
					PartyMemberMor_Edit_TB.Text = partyMember.Stats.Morality.ToString();

					PartyMemberWis_Edit_TB.Text = partyMember.Stats.Wisdom.ToString();
					PartyMemberRes_Edit_TB.Text = partyMember.Stats.Resistance.ToString();
					PartyMemberLuc_Edit_TB.Text = partyMember.Stats.Luck.ToString();
					PartyMemberRsk_Edit_TB.Text = partyMember.Stats.Risk.ToString();
					PartyMemberItl_Edit_TB.Text = partyMember.Stats.Intelligence.ToString();
					#endregion

					//Check boxes time!
					#region Weaknesses and strengths

					#region Elemental "Binding"
					//reset
					SetItemControlCheckboxData(PartyMembersMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddPartyMemberMagWeak_CB", true);
					SetItemControlCheckboxData(PartyMembersMagicWeakness_Edit_IC, partyMember.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddPartyMemberMagWeak_CB", false);

					SetItemControlCheckboxData(PartyMembersMagicStrength_Edit_IC, null, EMagicType.NONE, "AddPartyMemberMagicStrength_CB", true);
					SetItemControlCheckboxData(PartyMembersMagicStrength_Edit_IC, partyMember.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddPartyMemberMagicStrength_CB", false);
					PartyMembersMagicWeakness_Edit_IC.UpdateLayout();
					PartyMembersMagicStrength_Edit_IC.UpdateLayout();
					#endregion

					#region Weakness & Strengths "Binding"
					//reset
					SetItemControlCheckboxData(PartyMembersWeaponWeakness_Edit_IC, null, EMagicType.NONE, "AddPartyMemberweaponWeak_CB", true);
					SetItemControlCheckboxData(PartyMembersWeaponWeakness_Edit_IC, partyMember.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddPartyMemberweaponWeak_CB", false);

					SetItemControlCheckboxData(PartyMembersWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddPartyMemberWeaknessStrength_CB", true);
					SetItemControlCheckboxData(PartyMembersWeaponStrength_Edit_IC, partyMember.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddPartyMemberWeaknessStrength_CB", false);
					PartyMembersWeaponStrength_Edit_IC.UpdateLayout();
					PartyMembersWeaponStrength_Edit_IC.UpdateLayout();
					#endregion

					#endregion

					#region Jobs
					PartyMemberMainJob_Edit_CB.SelectedValue = CurrentJobsInDatabase.Single(x => x.Id == partyMember.Main_Job_FK);
					PartyMemberSubJob_Edit_CB.SelectedValue = CurrentJobsInDatabase.Single(x => x.Id == partyMember.Sub_Job_FK);
					#endregion

				}
				catch (Exception ex)
				{
					Console.WriteLine("Filling Party member Data  FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [Filling Party member Data ] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}

			}
		}


		private void PartyMemberUpdateToDatabase_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{


			if 
			(
				int.TryParse(PartyMemberFriendPoints_Edit_TB.Text, out int fpResult) &&
				int.TryParse(PartyMemberCombatLevel_Edit_TB.Text, out int combatlevelResult) &&

				int.TryParse(PartyMemberMaxHP_Edit_TB.Text, out int maxHpResult) &&
				int.TryParse(PartyMemberCurHP_Edit_TB.Text, out int curHpResult) &&
				int.TryParse(PartyMemberMaxMP_Edit_TB.Text, out int maxMPResult) &&
				int.TryParse(PartyMemberCurMP_Edit_TB.Text, out int curMPResult) &&

				int.TryParse(PartyMemberAtk_Edit_TB.Text, out int atkResult) &&
				int.TryParse(PartyMemberDef_Edit_TB.Text, out int defResult) &&
				int.TryParse(PartyMemberDex_Edit_TB.Text, out int dexResult) &&
				int.TryParse(PartyMemberAgl_Edit_TB.Text, out int aglResult) &&
				int.TryParse(PartyMemberMor_Edit_TB.Text, out int morResult) &&
				int.TryParse(PartyMemberWis_Edit_TB.Text, out int wisResult) &&
				int.TryParse(PartyMemberRes_Edit_TB.Text, out int resResult) &&
				int.TryParse(PartyMemberLuc_Edit_TB.Text, out int LucResult) &&
				int.TryParse(PartyMemberRsk_Edit_TB.Text, out int RskResult) &&
				int.TryParse(PartyMemberItl_Edit_TB.Text, out int itlResult)
			)
			{
				//before we send the SQL update query we need to update the info in memory.
				int absindex = PartyMemberName_Edit_CB.SelectedIndex;
				party_member partyMember = (party_member)CurrentPartyMembersInDatabase[absindex];
				partyMember.Friendship_Points = fpResult;
				partyMember.Level = combatlevelResult;
				if(PartyMemberMainJob_Edit_CB.SelectedValue != null )
					partyMember.Main_Job_FK = ((Job)PartyMemberMainJob_Edit_CB.SelectedValue).Id;
				if( PartyMemberSubJob_Edit_CB.SelectedValue != null )
					partyMember.Sub_Job_FK = ((Job)PartyMemberSubJob_Edit_CB.SelectedValue).Id;

				#region Equipment

				if (PartyMemberClothes_Head_Edit_CB.SelectedIndex >= 0)
					partyMember.HeadGear_FK = CurrentClothesInDatabase_Head[PartyMemberClothes_Head_Edit_CB.SelectedIndex].ID;
				else partyMember.HeadGear_FK = "";

				if (PartyMemberClothes_Body_Edit_CB.SelectedIndex >= 0)
					partyMember.BodyGear_FK = CurrentClothesInDatabase_Body[PartyMemberClothes_Body_Edit_CB.SelectedIndex].ID;
				else partyMember.BodyGear_FK = "";

				if (PartyMemberClothes_Legs_Edit_CB.SelectedIndex >= 0)
					partyMember.LegGear_FK = CurrentClothesInDatabase_Legs[PartyMemberClothes_Legs_Edit_CB.SelectedIndex].ID;
				else partyMember.LegGear_FK = "";

				if (PartyMemberClothes_Acc1_Edit_CB.SelectedIndex >= 0)
					partyMember.Accessory1_FK = CurrentAccessoriesInDatabase[PartyMemberClothes_Acc1_Edit_CB.SelectedIndex].ID;
				else partyMember.Accessory1_FK = "";

				if (PartyMemberClothes_Acc2_Edit_CB.SelectedIndex >= 0)
					partyMember.Accessory2_FK = CurrentAccessoriesInDatabase[PartyMemberClothes_Acc2_Edit_CB.SelectedIndex].ID;
				else partyMember.Accessory2_FK = "";
				#endregion

				Base_Stats stats = (Base_Stats)CurrentPartyMembersInDatabase[absindex].Stats;
				Base_Stats base_stats = new Base_Stats()
				{
					ID = stats.ID,
					Max_Health = maxHpResult,
					Current_Health = curHpResult,
					Max_Mana = maxMPResult,
					Current_Mana = curMPResult,
					Attack = atkResult,
					Defense = defResult,
					Dexterity = dexResult,
					Agility = aglResult,
					Morality = morResult,
					Wisdom = wisResult,
					Resistance = resResult,
					Luck = LucResult,
					Risk = RskResult,
					Intelligence = itlResult
				};
				weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentPartyMembersInDatabase[absindex].WeaknessAndStrengths;
				weaknessStrengths.ID = partyMember.Weak_Strength_FK;
				weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(PartyMembersMagicWeakness_Edit_IC, EMagicType.NONE, "AddPartyMemberMagWeak_CB");
				weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(PartyMembersMagicStrength_Edit_IC, EMagicType.NONE, "AddPartyMemberMagicStrength_CB");
				weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(PartyMembersWeaponWeakness_Edit_IC, EWeaponType.NONE, "AddPartyMemberweaponWeak_CB");
				weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(PartyMembersWeaponStrength_Edit_IC, EWeaponType.NONE, "AddPartyMemberWeaknessStrength_CB");

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					Createsql = "UPDATE `base_stats` " +
											"SET " +
											String.Format("{0} = {1},", "max_health", base_stats.Max_Health) +
											String.Format("{0} = {1},", "current_health", base_stats.Current_Health) +
											String.Format("{0} = {1},", "max_mana", base_stats.Max_Mana) +
											String.Format("{0} = {1},", "current_mana", base_stats.Current_Mana) +

											String.Format("{0} = {1},", "attack", base_stats.Attack) +
											String.Format("{0} = {1},", "defense", base_stats.Defense) +
											String.Format("{0} = {1},", "dexterity", base_stats.Dexterity) +
											String.Format("{0} = {1},", "agility", base_stats.Agility) +
											String.Format("{0} = {1},", "morality", base_stats.Morality) +

											String.Format("{0} = {1},", "wisdom", base_stats.Wisdom) +
											String.Format("{0} = {1},", "resistance", base_stats.Resistance) +
											String.Format("{0} = {1},", "luck", base_stats.Luck) +
											String.Format("{0} = {1},", "risk", base_stats.Risk) +
											String.Format("{0} = {1} ", "intelligence", base_stats.Intelligence) +

											String.Format("WHERE id='{0}'", base_stats.ID);
					_sqlite_conn.Query<Base_Stats>(Createsql);

					Createsql = "UPDATE `weaknesses_strengths` " +
					            "SET " +
					            String.Format("{0} = {1},", "physical_weaknesses", weaknessStrengths.physical_weaknesses) +
					            String.Format("{0} = {1},", "physical_strengths", weaknessStrengths.physical_strengths) +
					            String.Format("{0} = {1},", "magic_weaknesses", weaknessStrengths.magic_weaknesses) +
					            String.Format("{0} = {1} ", "magic_strengths", weaknessStrengths.magic_strengths) +

					            String.Format("WHERE id='{0}'", weaknessStrengths.ID);
					_sqlite_conn.Query<weaknesses_strengths>(Createsql);

					Createsql = "UPDATE `party_member` " +
					            "SET " +
					            String.Format("{0} = {1},", "friendship_points", partyMember.Friendship_Points) +
					            String.Format("{0} = {1},", "level", partyMember.Level) +

											(partyMember.HeadGear_FK != String.Empty ? 
												String.Format("{0} = '{1}',", "headgear_fk", partyMember.HeadGear_FK) :
												String.Format("{0} = '{1}',", "headgear_fk", "")) +
												(partyMember.BodyGear_FK != String.Empty ?
						            String.Format("{0} = '{1}',", "bodygear_fk", partyMember.BodyGear_FK) :
						            String.Format("{0} = '{1}',", "bodygear_fk", "")) +
					            (partyMember.LegGear_FK != String.Empty ?
												String.Format("{0} = '{1}',", "leggear_fk", partyMember.LegGear_FK) :
												String.Format("{0} = '{1}',", "leggear_fk", "")) +
					            (partyMember.Accessory1_FK != String.Empty ?
						            String.Format("{0} = '{1}',", "accessory1_fk", partyMember.Accessory1_FK) :
						            String.Format("{0} = '{1}',", "accessory1_fk", "")) +
					            (partyMember.Accessory2_FK != String.Empty ?
						            String.Format("{0} = '{1}',", "accessory2_fk", partyMember.Accessory2_FK) :
						            String.Format("{0} = '{1}',", "accessory2_fk", "")) +

											String.Format("{0} = {1},", "main_job_fk", partyMember.Main_Job_FK) +
							        String.Format("{0} = {1} ", "sub_job_fk", partyMember.Sub_Job_FK) +

					            String.Format("WHERE first_name='{0}'", partyMember.First_Name) + 
					            String.Format("AND last_name='{0}'", partyMember.Last_Name);
					_sqlite_conn.Query<party_member>(Createsql);

					#region Key Deletion And reinsertion
					#region Skill

					Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0} {1}';", partyMember.First_Name, partyMember.Last_Name);
					_sqlite_conn.Query<Skill>(Createsql); //delete

					foreach (Skill skill in partyMember.Skills)
					{
						Skill_Keys skillKey = new Skill_Keys()
						{
							Skill_ID = skill.Name,
							Req_Name = String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name),
							Req_Table = "party_member"
						};
						_sqlite_conn.Insert(skillKey);
					}
					#endregion

					#region Item
					Createsql = String.Format("DELETE FROM `item_keys` WHERE req_name = '{0} {1}';", partyMember.First_Name, partyMember.Last_Name);
					_sqlite_conn.Query<Item>(Createsql); //delete

					foreach (Item item in partyMember.Items)
					{
						Item_Keys itemkey = new Item_Keys()
						{
							Item_ID = item.ID,
							Req_Name = String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name),
							Req_Table = "party_member"
						};
						_sqlite_conn.Insert(itemkey);
					}
					#endregion

					#region Weapon
					Createsql = String.Format("DELETE FROM `weapon_keys` WHERE req_name = '{0} {1}';", partyMember.First_Name, partyMember.Last_Name);
					_sqlite_conn.Query<Weapon>(Createsql); //delete

					foreach (Weapon item in partyMember.Weapons)
					{
						Weapon_Keys itemkey = new Weapon_Keys()
						{
							Weapon_ID = item.ID,
							Req_Name = String.Format("{0} {1}", partyMember.First_Name, partyMember.Last_Name),
							Req_Table = "party_member"
						};
						_sqlite_conn.Insert(itemkey);
					}
					#endregion
					#endregion
				}
				catch (Exception ex)
				{
					Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
					GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [Party Member] failed: {0}", ex.Message);
				}
				finally

				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}


		private int GetBitWiseEnumeratedValFromIC(ItemsControl desIC, Enum etype, String CBName)
		{
			int i = 0;
			int returnval = 0;
			foreach (int en in Enum.GetValues(etype.GetType()))
			{
				if (en == 0) continue;
				ContentPresenter c = ((ContentPresenter)desIC.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName(CBName, c);

				if ((bool)(vv as CheckBox).IsChecked)
				{
					returnval += (int)Math.Pow(2, i);
				}

				i++;
			}
			return returnval;
		}

		private void EnemySkillsToIC_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			if (EnemySkills_Add_IC.Items.Count <= 4)
			{
				EnemySkills_Add_IC.Items.Add(((Skill)EnemySkills_Add_CB.SelectedValue).Name);
			}
		}

		private void RemoveEnemySkill_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemySkills_Add_IC.Items.IndexOf(item);

			EnemySkills_Add_IC.Items.RemoveAt(index);
		}


		private void EnemyItemsToIC_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			if (EnemyItems_Add_IC.Items.Count <= 4)
			{
				EnemyItems_Add_IC.Items.Add(((Item)EnemyItems_Add_CB.SelectedValue).ID);
			}
		}

		private void EnemyItemDropsToIC_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			if (EnemyItemDrops_Add_IC.Items.Count <= 4)
			{
				EnemyItemDrops_Add_IC.Items.Add(((Item)EnemyItemsDrops_Add_CB.SelectedValue).ID);
			}
		}

		private void RemoveEnemyItem_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemyItems_Add_IC.Items.IndexOf(item);

			EnemyItems_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveEnemyItemDrop_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemyItemDrops_Add_IC.Items.IndexOf(item);

			EnemyItemDrops_Add_IC.Items.RemoveAt(index);
		}

		private void EnemyWeaponsToIC_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			if (EnemyWeapon_Add_IC.Items.Count <= 2)
			{
				EnemyWeapon_Add_IC.Items.Add(((Weapon)EnemyWeapons_Add_CB.SelectedValue).ID);
			}
		}

		private void RemoveEnemyWeapon_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemyWeapon_Add_IC.Items.IndexOf(item);

			EnemyWeapon_Add_IC.Items.RemoveAt(index); 
		}


		private void EnemyAddToDatabase_Add_BTN_OnClick(object sender, RoutedEventArgs e)
		{

			//first up checking for validity.
			if (
				EnemyName_Add_TB.Text.Length > 0 &&
				int.TryParse(EnemyDropEXP_Add_TB.Text, out int expResult) &&
				int.TryParse(EnemyCombatLevel_Add_TB.Text, out int combatlevelResult) &&
				EnemyRarity_Add_CB.SelectedIndex >= 0 &&
				EnemySize_Add_CB.SelectedIndex >= 0 &&

				int.TryParse(EnemyMaxHP_Add_TB.Text, out int maxHpResult) &&
				int.TryParse(EnemyCurHP_Add_TB.Text, out int curHpResult) &&
				int.TryParse(EnemyMaxMP_Add_TB.Text, out int maxMPResult) &&
				int.TryParse(EnemyCurMP_Add_TB.Text, out int curMPResult) &&

				int.TryParse(EnemyAtk_Add_TB.Text, out int atkResult) &&
				int.TryParse(EnemyDef_Add_TB.Text, out int defResult) &&
				int.TryParse(EnemyDex_Add_TB.Text, out int dexResult) &&
				int.TryParse(EnemyAgl_Add_TB.Text, out int aglResult) &&
				int.TryParse(EnemyMor_Add_TB.Text, out int morResult) &&
				int.TryParse(EnemyWis_Add_TB.Text, out int wisResult) &&
				int.TryParse(EnemyRes_Add_TB.Text, out int resResult) &&
				int.TryParse(EnemyLuc_Add_TB.Text, out int LucResult) &&
				int.TryParse(EnemyRsk_Add_TB.Text, out int RskResult) &&
				int.TryParse(EnemyItl_Add_TB.Text, out int itlResult)
			)
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					int newID_stat = 0;
					int newID_weakstr = 0;

					#region Base_Stats

					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

					//Set up the weapon object!
					Base_Stats basestat = new Base_Stats()
					{
						ID = newID_stat + 1,
						Max_Health = maxHpResult,
						Current_Health = curHpResult,
						Max_Mana = maxMPResult,
						Current_Mana = curMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					#endregion
					_sqlite_conn.Insert(basestat);

					#region Weakness and Strengths
					weaknesses_strengths weakstrToAdd = new weaknesses_strengths();
					int phyweak, phystr, magweak, magstr;

					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);
					newID_weakstr = (wsList.Count == 0 ? 0 : wsList.Max(x => x.ID));
					weakstrToAdd.ID = newID_weakstr + 1;
					//GET THE MAGIC WEAKNESS ENUMERATED BITS
					#region Magic Weakness
					int i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)EnemyMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddEnemyMagWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_weaknesses = magweak;
					#endregion

					#region physical weakness
					i = 0;
					phyweak = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)EnemyWeaponWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddEnemyweaponWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phyweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_weaknesses = phyweak;
					#endregion

					#region magic strength
					i = 0;
					magstr = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)EnemyMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddEnemyMagicStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magstr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_strengths = magstr;
					#endregion

					#region physical strength
					i = 0;
					phystr = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)EnemyWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddEnemyStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phystr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_strengths = phystr;
					#endregion

					#endregion
					_sqlite_conn.Insert(weakstrToAdd);

					#region Party Member

					enemy enemy = new enemy()
					{
						Name = EnemyName_Add_TB.Text,
						Level = combatlevelResult,
						EXP = expResult,
						Size_Type = (int)(ESize)EnemySize_Add_CB.SelectedValue,
						Rarity = (int)(ERarityType)EnemyRarity_Add_CB.SelectedValue,

						Stats_FK = basestat.ID,
						Weak_Strength_FK = weakstrToAdd.ID,
					};

					#endregion
					_sqlite_conn.Insert(enemy);

					#region Create Keys Entries
					#region Skills
					InsertRecordIntoSkillKeys(EnemySkills_Add_IC, _sqlite_conn, "enemy",
						String.Format("{0}", enemy.Name));
					#endregion
					#region Weapons
					InsertRecordIntoWeaponKeys(EnemyWeapon_Add_IC, _sqlite_conn, "enemy",
						String.Format("{0}", enemy.Name));
					#endregion
					#region Items
					InsertRecordIntoItemKeys(EnemyItems_Add_IC, _sqlite_conn, "enemy",
						String.Format("{0}", enemy.Name));
					InsertRecordIntoItemKeys(EnemyItemDrops_Add_IC, _sqlite_conn, "enemy",
						String.Format("{0}", enemy.Name), true);
					#endregion
					#endregion
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [items] FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [items] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}


		}

		private void EnemyName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			ComboBox CB = sender as ComboBox;
			if (CB.SelectedIndex >= 0)
			{
				//reset the IC so we don't have false dups
				EnemySkills_Edit_IC.Items.Clear();
				EnemyItems_Edit_IC.Items.Clear();
				EnemyItemDrops_Edit_IC.Items.Clear();
				EnemyWeapon_Edit_IC.Items.Clear();

				EnemySize_Edit_CB.SelectedItem = ESize.NONE;
				EnemyRarity_Edit_CB.SelectedItem = ERarityType.NONE;

				enemy enemy = (enemy)CB.SelectedValue;
				Console.WriteLine(String.Format("{0}", enemy.Name));
				//create a partymemeber and fill out the objects internal object references.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Base Stats
					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

					Base_Stats baseStats = bsList.Single(x => x.ID == enemy.Stats_FK);
					enemy.Stats = baseStats;
					#endregion
					#region Weaknesses and strengths
					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

					weaknesses_strengths wsData = wsList.Single(x => x.ID == enemy.Weak_Strength_FK);
					enemy.WeaknessAndStrengths = wsData;
					#endregion


					#region Skills
					Createsql = "SELECT * FROM `skill_keys`;";
					List<Skill_Keys> skillList_keys = _sqlite_conn.Query<Skill_Keys>(Createsql);
					skillList_keys = skillList_keys.FindAll(x => x.Req_Name == String.Format("{0}", enemy.Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Skill> result_skills = CurrentSkillsInDatabase.Where(p => skillList_keys.Any(p2 => p2.Skill_ID == p.Name)).ToList() as List<Skill>;
					enemy.Skills = result_skills;
					result_skills.ForEach(x => EnemySkills_Edit_IC.Items.Add(x.Name));
					#endregion

					#region Items
					Createsql = "SELECT * FROM `item_keys`;";
					List<Item_Keys> ItemList_keys = _sqlite_conn.Query<Item_Keys>(Createsql);
					ItemList_keys = ItemList_keys.FindAll(x => x.Req_Name == String.Format("{0}", enemy.Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Item> result_items_nondrops = CurrentItemsInDatabase.Where(p => ItemList_keys.Any(p2 => p2.Item_ID == p.ID && p2.bDrop == false)).ToList() as List<Item>;
					List<Item> result_items_drops = CurrentItemsInDatabase.Where(p => ItemList_keys.Any(p2 => p2.Item_ID == p.ID && p2.bDrop == true)).ToList() as List<Item>;

					//seperate these between droppable and non droppable
					enemy.Items = result_items_nondrops;
					enemy.Drops = result_items_drops;
					result_items_nondrops.ForEach(x => EnemyItems_Edit_IC.Items.Add((x.ID)));
					result_items_drops.ForEach(x => EnemyItemDrops_Edit_IC.Items.Add((x.ID)));
					#endregion

					#region Weapons
					Createsql = "SELECT * FROM `weapon_keys`;";
					List<Weapon_Keys> WeaponList_keys = _sqlite_conn.Query<Weapon_Keys>(Createsql);
					WeaponList_keys = WeaponList_keys.FindAll(x => x.Req_Name == String.Format("{0}", enemy.Name));

					//Get all the matching Skills using the keys from the party member VIA the database query,
					List<Weapon> result_weapon = CurrentWeaponsInDatabase.Where(p => WeaponList_keys.Any(p2 => p2.Weapon_ID == p.ID)).ToList() as List<Weapon>;
					//enemy.Weapons = result_weapon;
					foreach (Weapon weapon in result_weapon)
					{
						enemy.Weapons.AddLast(weapon);
					}
					result_weapon.ForEach(x => EnemyWeapon_Edit_IC.Items.Add((x.ID)));
					#endregion

					//Next up fill out ALL the text boxes with the correct info

					#region Party Member Data
					EnemyCombatLevel_Edit_TB.Text = enemy.Level.ToString();
					EnemyDropEXP_Edit_TB.Text = enemy.EXP.ToString();
					EnemySize_Edit_CB.SelectedItem = (ESize)enemy.Size_Type;
					EnemyRarity_Edit_CB.SelectedItem = (ERarityType)enemy.Rarity;
					#endregion

					#region Stats
					EnemyMaxHP_Edit_TB.Text = enemy.Stats.Max_Health.ToString();
					EnemyCurHP_Edit_TB.Text = enemy.Stats.Current_Health.ToString();
					EnemyMaxMP_Edit_TB.Text = enemy.Stats.Max_Mana.ToString();
					EnemyCurMP_Edit_TB.Text = enemy.Stats.Current_Mana.ToString();

					EnemyAtk_Edit_TB.Text = enemy.Stats.Attack.ToString();
					EnemyDef_Edit_TB.Text = enemy.Stats.Defense.ToString();
					EnemyDex_Edit_TB.Text = enemy.Stats.Dexterity.ToString();
					EnemyAgl_Edit_TB.Text = enemy.Stats.Agility.ToString();
					EnemyMor_Edit_TB.Text = enemy.Stats.Morality.ToString();

					EnemyWis_Edit_TB.Text = enemy.Stats.Wisdom.ToString();
					EnemyRes_Edit_TB.Text = enemy.Stats.Resistance.ToString();
					EnemyLuc_Edit_TB.Text = enemy.Stats.Luck.ToString();
					EnemyRsk_Edit_TB.Text = enemy.Stats.Risk.ToString();
					EnemyItl_Edit_TB.Text = enemy.Stats.Intelligence.ToString();
					#endregion

					//Check boxes time!
					#region Weaknesses and strengths

					#region Elemental "Binding"
					//reset
					SetItemControlCheckboxData(EnemyMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddEnemyMagWeak_CB", true);
					SetItemControlCheckboxData(EnemyMagicWeakness_Edit_IC, enemy.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddEnemyMagWeak_CB", false);

					SetItemControlCheckboxData(EnemyMagicStrength_Edit_IC, null, EMagicType.NONE, "AddEnemyMagicStrength_CB", true);
					SetItemControlCheckboxData(EnemyMagicStrength_Edit_IC, enemy.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddEnemyMagicStrength_CB", false);
					EnemyMagicWeakness_Edit_IC.UpdateLayout();
					EnemyMagicStrength_Edit_IC.UpdateLayout();
					#endregion

					#region Weakness & Strengths "Binding"
					//reset
					SetItemControlCheckboxData(EnemyWeaponWeakness_Edit_IC, null, EMagicType.NONE, "AddEnemyweaponWeak_CB", true);
					SetItemControlCheckboxData(EnemyWeaponWeakness_Edit_IC, enemy.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddEnemyweaponWeak_CB", false);

					SetItemControlCheckboxData(EnemyWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddEnemyStrength_CB", true);
					SetItemControlCheckboxData(EnemyWeaponStrength_Edit_IC, enemy.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddEnemyStrength_CB", false);
					EnemyWeaponStrength_Edit_IC.UpdateLayout();
					EnemyWeaponStrength_Edit_IC.UpdateLayout();
					#endregion

					#endregion

				}
				catch (Exception ex)
				{
					Console.WriteLine("Filling Party member Data  FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [Filling Party member Data ] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}

			}
		}

		private void EnemyUpdateDatabase_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{

			//first up checking for validity.
			if (
				EnemyName_Edit_CB.SelectedIndex >= 0 &&
				int.TryParse(EnemyDropEXP_Edit_TB.Text, out int expResult) &&
				int.TryParse(EnemyCombatLevel_Edit_TB.Text, out int combatlevelResult) &&
				EnemyRarity_Edit_CB.SelectedIndex >= 0 &&
				EnemySize_Edit_CB.SelectedIndex >= 0 &&

				int.TryParse(EnemyMaxHP_Edit_TB.Text, out int maxHpResult) &&
				int.TryParse(EnemyCurHP_Edit_TB.Text, out int curHpResult) &&
				int.TryParse(EnemyMaxMP_Edit_TB.Text, out int maxMPResult) &&
				int.TryParse(EnemyCurMP_Edit_TB.Text, out int curMPResult) &&

				int.TryParse(EnemyAtk_Edit_TB.Text, out int atkResult) &&
				int.TryParse(EnemyDef_Edit_TB.Text, out int defResult) &&
				int.TryParse(EnemyDex_Edit_TB.Text, out int dexResult) &&
				int.TryParse(EnemyAgl_Edit_TB.Text, out int aglResult) &&
				int.TryParse(EnemyMor_Edit_TB.Text, out int morResult) &&
				int.TryParse(EnemyWis_Edit_TB.Text, out int wisResult) &&
				int.TryParse(EnemyRes_Edit_TB.Text, out int resResult) &&
				int.TryParse(EnemyLuc_Edit_TB.Text, out int LucResult) &&
				int.TryParse(EnemyRsk_Edit_TB.Text, out int RskResult) &&
				int.TryParse(EnemyItl_Edit_TB.Text, out int itlResult)
			)
			{
				//before we send the SQL update query we need to update the info in memory.
					int absindex = EnemyName_Edit_CB.SelectedIndex;
					enemy Enemy = (enemy)CurrentEnemiesInDatabase[absindex];
					Enemy.EXP = expResult;
					Enemy.Level = combatlevelResult;
					Enemy.Size_Type = (int)(ESize)EnemySize_Edit_CB.SelectedValue;
					Enemy.Rarity = (int)(ERarityType)EnemyRarity_Edit_CB.SelectedValue;

				#region Equipment
				if (enemyClothes_Head_Edit_CB.SelectedIndex >= 0)
					Enemy.HeadGear_FK = CurrentClothesInDatabase_Head[enemyClothes_Head_Edit_CB.SelectedIndex].ID;
				else Enemy.HeadGear_FK = "";

				if (enemyClothes_Body_Edit_CB.SelectedIndex >= 0)
					Enemy.BodyGear_FK = CurrentClothesInDatabase_Body[enemyClothes_Body_Edit_CB.SelectedIndex].ID;
				else Enemy.BodyGear_FK = "";

				if (enemyClothes_Legs_Edit_CB.SelectedIndex >= 0)
					Enemy.LegGear_FK = CurrentClothesInDatabase_Legs[enemyClothes_Legs_Edit_CB.SelectedIndex].ID;
				else Enemy.LegGear_FK = "";

				if (enemyClothes_Acc1_Edit_CB.SelectedIndex >= 0)
					Enemy.Accessory1_FK = CurrentAccessoriesInDatabase[enemyClothes_Acc1_Edit_CB.SelectedIndex].ID;
				else Enemy.Accessory1_FK = "";

				if (enemyClothes_Acc2_Edit_CB.SelectedIndex >= 0)
					Enemy.Accessory2_FK = CurrentAccessoriesInDatabase[enemyClothes_Acc2_Edit_CB.SelectedIndex].ID;
				else Enemy.Accessory2_FK = "";
				#endregion

				Base_Stats stats = (Base_Stats)CurrentEnemiesInDatabase[absindex].Stats;
					Base_Stats base_stats = new Base_Stats()
					{
						ID = stats.ID,
						Max_Health = maxHpResult,
						Current_Health = curHpResult,
						Max_Mana = maxMPResult,
						Current_Mana = curMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentEnemiesInDatabase[absindex].WeaknessAndStrengths;
					weaknessStrengths.ID = Enemy.Weak_Strength_FK;
					weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(EnemyMagicWeakness_Edit_IC, EMagicType.NONE, "AddEnemyMagWeak_CB");
					weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(EnemyMagicStrength_Edit_IC, EMagicType.NONE, "AddEnemyMagicStrength_CB");
					weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(EnemyWeaponWeakness_Edit_IC, EWeaponType.NONE, "AddEnemyweaponWeak_CB");
					weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(EnemyWeaponStrength_Edit_IC, EWeaponType.NONE, "AddEnemyStrength_CB");

					String masterfile = (SQLDatabasePath);
					_sqlite_conn = new SQLiteConnection(masterfile);
					int rowid = 0;
					try
					{
						String Createsql = "";
						Createsql = "UPDATE `base_stats` " +
												"SET " +
												String.Format("{0} = {1},", "max_health", base_stats.Max_Health) +
												String.Format("{0} = {1},", "current_health", base_stats.Current_Health) +
												String.Format("{0} = {1},", "max_mana", base_stats.Max_Mana) +
												String.Format("{0} = {1},", "current_mana", base_stats.Current_Mana) +

												String.Format("{0} = {1},", "attack", base_stats.Attack) +
												String.Format("{0} = {1},", "defense", base_stats.Defense) +
												String.Format("{0} = {1},", "dexterity", base_stats.Dexterity) +
												String.Format("{0} = {1},", "agility", base_stats.Agility) +
												String.Format("{0} = {1},", "morality", base_stats.Morality) +

												String.Format("{0} = {1},", "wisdom", base_stats.Wisdom) +
												String.Format("{0} = {1},", "resistance", base_stats.Resistance) +
												String.Format("{0} = {1},", "luck", base_stats.Luck) +
												String.Format("{0} = {1},", "risk", base_stats.Risk) +
												String.Format("{0} = {1} ", "intelligence", base_stats.Intelligence) +

												String.Format("WHERE id='{0}'", base_stats.ID);
						_sqlite_conn.Query<Base_Stats>(Createsql);

						Createsql = "UPDATE `weaknesses_strengths` " +
												"SET " +
												String.Format("{0} = {1},", "physical_weaknesses", weaknessStrengths.physical_weaknesses) +
												String.Format("{0} = {1},", "physical_strengths", weaknessStrengths.physical_strengths) +
												String.Format("{0} = {1},", "magic_weaknesses", weaknessStrengths.magic_weaknesses) +
												String.Format("{0} = {1} ", "magic_strengths", weaknessStrengths.magic_strengths) +

												String.Format("WHERE id='{0}'", weaknessStrengths.ID);
						_sqlite_conn.Query<weaknesses_strengths>(Createsql);

						Createsql = "UPDATE `enemy` " +
												"SET " +
												String.Format("{0} = {1},", "size_type", Enemy.Size_Type) +
												String.Format("{0} = {1},", "level", Enemy.Level) +

												(Enemy.HeadGear_FK != String.Empty ?
													String.Format("{0} = '{1}',", "headgear_fk", Enemy.HeadGear_FK) :
													String.Format("{0} = '{1}',", "headgear_fk", "")) +
												(Enemy.BodyGear_FK != String.Empty ?
													String.Format("{0} = '{1}',", "bodygear_fk", Enemy.BodyGear_FK) :
													String.Format("{0} = '{1}',", "bodygear_fk", "")) +
												(Enemy.LegGear_FK != String.Empty ?
													String.Format("{0} = '{1}',", "leggear_fk", Enemy.LegGear_FK) :
													String.Format("{0} = '{1}',", "leggear_fk", "")) +
												(Enemy.Accessory1_FK != String.Empty ?
													String.Format("{0} = '{1}',", "accessory1_fk", Enemy.Accessory1_FK) :
													String.Format("{0} = '{1}',", "accessory1_fk", "")) +
												(Enemy.Accessory2_FK != String.Empty ?
													String.Format("{0} = '{1}',", "accessory2_fk", Enemy.Accessory2_FK) :
													String.Format("{0} = '{1}',", "accessory2_fk", "")) +

												String.Format("{0} = {1},", "exp", Enemy.EXP) +
												String.Format("{0} = {1} ", "rarity", Enemy.Rarity) +

												String.Format("WHERE name='{0}'", Enemy.Name.ToString());
						_sqlite_conn.Query<enemy>(Createsql);

						#region Key Deletion And reinsertion
						#region Skill

						Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0}';", Enemy.Name);
						_sqlite_conn.Query<Skill>(Createsql); //delete

						foreach (Skill skill in Enemy.Skills)
						{
							Skill_Keys skillKey = new Skill_Keys()
							{
								Skill_ID = skill.Name,
								Req_Name = String.Format("{0}", Enemy.Name),
								Req_Table = "enemy"
							};
							_sqlite_conn.Insert(skillKey);
						}
						#endregion

						#region Item
						Createsql = String.Format("DELETE FROM `item_keys` WHERE req_name = '{0}';", Enemy.Name);
						_sqlite_conn.Query<Item>(Createsql); //delete

						foreach (Item item in Enemy.Items)
						{
							Item_Keys itemkey = new Item_Keys()
							{
								Item_ID = item.ID,
								Req_Name = String.Format("{0}", Enemy.Name),
								Req_Table = "enemy",
								bDrop = false
							};
							_sqlite_conn.Insert(itemkey);
						}
						foreach (Item item in Enemy.Drops)
						{
							Item_Keys itemkey = new Item_Keys()
							{
								Item_ID = item.ID,
								Req_Name = String.Format("{0}", Enemy.Name),
								Req_Table = "enemy",
								bDrop = true
							};
							_sqlite_conn.Insert(itemkey);
						}
					#endregion

					#region Weapon
					Createsql = String.Format("DELETE FROM `weapon_keys` WHERE req_name = '{0}';", Enemy.Name);
						_sqlite_conn.Query<Weapon>(Createsql); //delete

						foreach (Weapon item in Enemy.Weapons)
						{
							Weapon_Keys itemkey = new Weapon_Keys()
							{
								Weapon_ID = item.ID,
								Req_Name = String.Format("{0}", Enemy.Name),
								Req_Table = "enemy"
							};
							_sqlite_conn.Insert(itemkey);
						}
						#endregion
						#endregion
					}
					catch (Exception ex)
					{
						Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
						GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [enemy] failed: {0}", ex.Message);
					}
					finally

					{
						//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
						//GameplayModifierName_CB.SelectedIndex = absindex;
					}
				}


			}


		private void EnemySkillsToIC_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			if (EnemySkills_Edit_IC.Items.Count <= 4)
			{
				EnemySkills_Edit_IC.Items.Add(((Skill)EnemySkills_Edit_CB.SelectedValue).Name);
				CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Skills.Add((Skill)EnemySkills_Edit_CB.SelectedValue);
			}
		}

		private void RemoveEnemySkill_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemySkills_Edit_IC.Items.IndexOf(item);

			EnemySkills_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Skills.RemoveAt(index);
		}


		private void EnemyItemsToIC_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			if (EnemyItems_Edit_IC.Items.Count <= 4)
			{
				EnemyItems_Edit_IC.Items.Add(((Item)EnemyItems_Edit_CB.SelectedValue).ID);
				CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Items.Add((Item)EnemyItems_Edit_CB.SelectedValue);
			}
		}

		private void EnemyItemDropsToIC_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			if (EnemyItemDrops_Edit_IC.Items.Count <= 4)
			{
				EnemyItemDrops_Edit_IC.Items.Add(((Item)EnemyItemsDrops_Edit_CB.SelectedValue).ID);
				CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Drops.Add((Item)EnemyItemsDrops_Edit_CB.SelectedValue);
			}
		}

		private void RemoveEnemyItem_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemyItems_Edit_IC.Items.IndexOf(item);

			EnemyItems_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Items.RemoveAt(index);
		}

		private void RemoveEnemyItemDrop_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemyItemDrops_Edit_IC.Items.IndexOf(item);

			EnemyItemDrops_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Drops.RemoveAt(index);
		}

		private void EnemyWeaponsToIC_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			//make sure there is less than max allowed
			if (EnemyWeapon_Edit_IC.Items.Count <= 2)
			{
				EnemyWeapon_Edit_IC.Items.Add(((Weapon)EnemyWeapons_Edit_CB.SelectedValue).ID);
				CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Weapons.AddLast((Weapon)EnemyItems_Edit_CB.SelectedValue);
			}
		}

		private void RemoveEnemyWeapon_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemyWeapon_Edit_IC.Items.IndexOf(item);

			EnemyWeapon_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Weapons.Remove(
				CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Weapons.ToList()[index]);
		}


		private void AccessoryEquipEffect_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = AccessoryEquipEffects_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (AccessoryEffectEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Effects[AccessoryEquipEffects_Add_CB.SelectedIndex].Id;
					if (!AccessoryEffectEquip_Add_IC.Items.Contains(effectname))
					{
						AccessoryEffectEquip_Add_IC.Items.Add(
							effectname
						);
						AccessoryEffectEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void AccessoryEquipTrait_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = AccessoryEquipTraits_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (AccessoryTraitsEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Traits[AccessoryEquipTraits_Add_CB.SelectedIndex].Id;
					if (!AccessoryTraitsEquip_Add_IC.Items.Contains(effectname))
					{
						AccessoryTraitsEquip_Add_IC.Items.Add(
							effectname
						);
						AccessoryTraitsEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void AccessoryEquipSkill_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = AccessoryEquipSkills_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (AccessorySkillsEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentSkillsInDatabase[AccessoryEquipSkills_Add_CB.SelectedIndex].Name;
					if (!AccessorySkillsEquip_Add_IC.Items.Contains(effectname))
					{
						AccessorySkillsEquip_Add_IC.Items.Add(
							effectname
						);
						AccessorySkillsEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void AddAccessoryToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			//first up checking for validity.
			if (AccessoryName_Add_TB.Text.Length > 0 &&
			    int.TryParse(AccessoryWeight_Add_TB.Text, out int weightval) &&
					AccessoryType_Add_CB.SelectedIndex >= 0 && AccessoryRarity_Add_CB.SelectedIndex >= 0 &&

					int.TryParse(AccessoriesMaxHP_Add_TB.Text, out int maxHpResult) &&
					int.TryParse(AccessoriesMaxMP_Add_TB.Text, out int maxMPResult) &&

					int.TryParse(AccessoriesAtk_Add_TB.Text, out int atkResult) &&
					int.TryParse(AccessoriesDef_Add_TB.Text, out int defResult) &&
					int.TryParse(AccessoriesDex_Add_TB.Text, out int dexResult) &&
					int.TryParse(AccessoriesAgl_Add_TB.Text, out int aglResult) &&
					int.TryParse(AccessoriesMor_Add_TB.Text, out int morResult) &&
					int.TryParse(AccessoriesWis_Add_TB.Text, out int wisResult) &&
					int.TryParse(AccessoriesRes_Add_TB.Text, out int resResult) &&
					int.TryParse(AccessoriesLuc_Add_TB.Text, out int LucResult) &&
					int.TryParse(AccessoriesRsk_Add_TB.Text, out int RskResult) &&
					int.TryParse(AccessoriesItl_Add_TB.Text, out int itlResult)
					) //  1.0.0.2v
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Base_Stats

					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					int newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

					//Set up the weapon object!
					Base_Stats basestat = new Base_Stats()
					{
						ID = newID_stat + 1,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					#endregion

					_sqlite_conn.Insert(basestat);

					#region Weakness and Strengths
					weaknesses_strengths weakstrToAdd = new weaknesses_strengths();
					int phyweak, phystr, magweak, magstr;

					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);
					int newID_weakstr = (wsList.Count == 0 ? 0 : wsList.Max(x => x.ID));
					weakstrToAdd.ID = newID_weakstr + 1;
					//GET THE MAGIC WEAKNESS ENUMERATED BITS
					#region Magic Weakness
					int i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)AccessoriesMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddAccessoryMagWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_weaknesses = magweak;
					#endregion

					#region physical weakness
					i = 0;
					phyweak = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)AccessoriesWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddAccessoryWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phyweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_weaknesses = phyweak;
					#endregion

					#region magic strength
					i = 0;
					magstr = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)AccessoriesMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddAccessoryMagicStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magstr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_strengths = magstr;
					#endregion

					#region physical strength
					i = 0;
					phystr = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)AccessoriesWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddAccessoryWeaknessStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phystr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_strengths = phystr;
					#endregion

					#endregion
					_sqlite_conn.Insert(weakstrToAdd);


					//Set up the weapon object!
					Accessories accessories = new Accessories()
					{
						ID = AccessoryName_Add_TB.Text,
						Accessory_Type = (int)((EWeaponType)AccessoryType_Add_CB.SelectedValue),
						Rarity = (int)((ERarityType)AccessoryRarity_Add_CB.SelectedValue),
						Weight = weightval,
						Function_PTR = AccessoryFuncPTR_Add_TB.Text, //1.0.0.3v

						Stats_FK = basestat.ID,
						Weakness_Strength_FK = weakstrToAdd.ID
					};

					//GET THE MAGIC TYPE ENUMERATED BITS
					#region Magic types
					i = 0;
					int magictypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)AccessoryMagicTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagicTypes_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magictypesval += (int)Math.Pow(2, i);
						}
						i++;
					}
					accessories.Elemental = magictypesval;
					#endregion

					//DNE
					#region Item Types
					//i = 0;
					//int itemstypesval = 0;
					//foreach (int en in Enum.GetValues(typeof(EItemType)))
					//{
					//	if (en == 0) continue;
					//	ContentPresenter c = ((ContentPresenter)ItemTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
					//	var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);

					//	if ((bool)(vv as CheckBox).IsChecked)
					//	{
					//		itemstypesval += (int)Math.Pow(2, i);
					//	}
					//	i++;
					//}
					//item.Item_Type = itemstypesval;
					#endregion

					#region Create Keys Entries
					#region Effects/Traits
					InsertRecordIntoModifierKeys(ItemEffectEquip_Add_IC, _sqlite_conn, "accessories", accessories.ID);
					InsertRecordIntoModifierKeys(ItemTraitsEquip_Add_IC, _sqlite_conn, "accessories", accessories.ID);
					#endregion
					#region Skills
					InsertRecordIntoSkillKeys(WeaponSkillsEquip_Add_IC, _sqlite_conn, "accessories", accessories.ID);
					#endregion
					#endregion

					//Add it to the databse
					int retval = _sqlite_conn.Insert(accessories);
					Console.WriteLine("RowID Val: {0}", retval);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [accessories] FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [accessories] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}


		}

		private void AccessoryEquipEffect_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = AccessoryEquipEffects_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (AccessoryEffectEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Effects[AccessoryEquipEffects_Edit_CB.SelectedIndex].Id;
					if (!AccessoryEffectEquip_Edit_IC.Items.Contains(effectname))
					{
						AccessoryEffectEquip_Edit_IC.Items.Add(effectname);
						CurrentAccessoriesInDatabase[AccessoryName_Edit_CB.SelectedIndex].Effects.Add(
							CurrenGameplayModifiersInDatabase_Effects[AccessoryEquipEffects_Edit_CB.SelectedIndex]);

						AccessoryEffectEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void RemoveAccessoryEffect_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = AccessoryEffectEquip_Add_IC.Items.IndexOf(item);

			AccessoryEffectEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveAccessoryEffect_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = AccessoryEffectEquip_Edit_IC.Items.IndexOf(item);

			AccessoryEffectEquip_Edit_IC.Items.RemoveAt(index);

			CurrentAccessoriesInDatabase[AccessoryName_Edit_CB.SelectedIndex].Effects.RemoveAt(index);

		}

		private void AccessoryEquipTrait_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = AccessoryEquipTraits_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (AccessoryTraitsEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Traits[AccessoryEquipTraits_Edit_CB.SelectedIndex].Id;
					if (!AccessoryTraitsEquip_Edit_IC.Items.Contains(effectname))
					{
						AccessoryTraitsEquip_Edit_IC.Items.Add(effectname);
						CurrentAccessoriesInDatabase[AccessoryName_Edit_CB.SelectedIndex].Traits.Add(
							CurrenGameplayModifiersInDatabase_Traits[AccessoryEquipTraits_Edit_CB.SelectedIndex]);
						AccessoryTraitsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void RemoveAccessoryTrait_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = AccessoryTraitsEquip_Add_IC.Items.IndexOf(item);

			AccessoryTraitsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveAccessoryTrait_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = AccessoryTraitsEquip_Edit_IC.Items.IndexOf(item);

			AccessoryTraitsEquip_Edit_IC.Items.RemoveAt(index);

			CurrentAccessoriesInDatabase[AccessoryName_Edit_CB.SelectedIndex].Traits.RemoveAt(index);
		}


		private void AccessoryEquipSkill_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = AccessoryEquipSkills_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (AccessorySkillsEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentSkillsInDatabase[AccessoryEquipSkills_Edit_CB.SelectedIndex].Name;
					if (!AccessorySkillsEquip_Edit_IC.Items.Contains(effectname))
					{
						AccessorySkillsEquip_Edit_IC.Items.Add(effectname);
						CurrentAccessoriesInDatabase[AccessoryName_Edit_CB.SelectedIndex].AccessorySkills.Add(
							CurrentSkillsInDatabase[AccessoryEquipSkills_Edit_CB.SelectedIndex]);
						AccessorySkillsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}


		private void RemoveAccessorySkill_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = AccessorySkillsEquip_Add_IC.Items.IndexOf(item);

			AccessorySkillsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveAccessorySkill_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = AccessorySkillsEquip_Edit_IC.Items.IndexOf(item);

			AccessorySkillsEquip_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentAccessoriesInDatabase[AccessoryName_Edit_CB.SelectedIndex].AccessorySkills.RemoveAt(index);
		}


		/// Updated to include AoE, and target side variables -AM 8/29/2020 1.0.0.2v
		/// Updated this method to include the function pointer name string variable -AM 9/4/2020 1.0.0.3v
		private void UpdateAccessoryInDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (
			    int.TryParse(AccessoryWeight_Edit_TB.Text, out int weightval) &&
			    int.TryParse(AccessoriesMaxHP_Edit_TB.Text, out int maxHpResult) &&
					int.TryParse(AccessoriesMaxMP_Edit_TB.Text, out int maxMPResult) &&

					int.TryParse(AccessoriesAtk_Edit_TB.Text, out int atkResult) &&
					int.TryParse(AccessoriesDef_Edit_TB.Text, out int defResult) &&
					int.TryParse(AccessoriesDex_Edit_TB.Text, out int dexResult) &&
					int.TryParse(AccessoriesAgl_Edit_TB.Text, out int aglResult) &&
					int.TryParse(AccessoriesMor_Edit_TB.Text, out int morResult) &&
					int.TryParse(AccessoriesWis_Edit_TB.Text, out int wisResult) &&
					int.TryParse(AccessoriesRes_Edit_TB.Text, out int resResult) &&
					int.TryParse(AccessoriesLuc_Edit_TB.Text, out int LucResult) &&
					int.TryParse(AccessoriesRsk_Edit_TB.Text, out int RskResult) &&
					int.TryParse(AccessoriesItl_Edit_TB.Text, out int itlResult)
				) //1.0.0.2v
			{

				//before we send the SQL update query we need to update the info in memory.
				int absindex = AccessoryName_Edit_CB.SelectedIndex;
				Accessory accessorydata = CurrentAccessoriesInDatabase[absindex];
				
				accessorydata.Accessory_Type = AccessoryType_Edit_CB.SelectedIndex;
				accessorydata.Rarity = AccessoryRarity_Edit_CB.SelectedIndex;
				accessorydata.Weight = weightval;
				accessorydata.Function_PTR = AccessoryFuncPTR_Edit_TB.Text; // 1.0.0.3v
				//TODO: Add the wepdata weakness after that table is done in this tool.
																															 //itemdata.Weakness_Strength_FK = 

				//GET THE MAGIC TYPE ENUMERATED BITS
				#region Magic types
				int i = 0;
				int magictypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EMagicType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)AccessoryMagicTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						magictypesval += (int)Math.Pow(2, i);
					}

					i++;
				}
				accessorydata.Elemental = magictypesval;
				#endregion

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					//Createsql = ("SELECT * FROM `gameplay_modifiers`;");

					Createsql = "UPDATE `accessories` " +
											"SET " +
											String.Format("{0} = {1},", "elemental", accessorydata.Elemental) +
											String.Format("{0} = {1},", "weight", accessorydata.Weight) +
											String.Format("{0} = {1},", "accessory_type", accessorydata.Accessory_Type) +
											String.Format("{0} = '{1}',", "function_ptr", accessorydata.Function_PTR) + //1.0.0.3v

											String.Format("{0} = {1},", "rarity", accessorydata.Rarity) +
											String.Format("{0} = {1} ", "weakness_strength_fk", accessorydata.Weakness_Strength_FK) +
											String.Format("WHERE id='{0}'", accessorydata.ID);
					_sqlite_conn.Query<Accessory>(Createsql);



					Base_Stats stats = (Base_Stats)CurrentAccessoriesInDatabase[absindex].Stats;
					Base_Stats base_stats = new Base_Stats()
					{
						ID = stats.ID,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					Createsql = "";
					Createsql = "UPDATE `base_stats` " +
											"SET " +
											String.Format("{0} = {1},", "max_health", base_stats.Max_Health) +
											String.Format("{0} = {1},", "max_mana", base_stats.Max_Mana) +

											String.Format("{0} = {1},", "attack", base_stats.Attack) +
											String.Format("{0} = {1},", "defense", base_stats.Defense) +
											String.Format("{0} = {1},", "dexterity", base_stats.Dexterity) +
											String.Format("{0} = {1},", "agility", base_stats.Agility) +
											String.Format("{0} = {1},", "morality", base_stats.Morality) +

											String.Format("{0} = {1},", "wisdom", base_stats.Wisdom) +
											String.Format("{0} = {1},", "resistance", base_stats.Resistance) +
											String.Format("{0} = {1},", "luck", base_stats.Luck) +
											String.Format("{0} = {1},", "risk", base_stats.Risk) +
											String.Format("{0} = {1} ", "intelligence", base_stats.Intelligence) +

											String.Format("WHERE id='{0}'", base_stats.ID);
					_sqlite_conn.Query<Base_Stats>(Createsql);


					weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentAccessoriesInDatabase[absindex].WeaknessAndStrengths;
					weaknessStrengths.ID = accessorydata.Weakness_Strength_FK;
					weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(AccessoriesMagicWeakness_Edit_IC, EMagicType.NONE, "AddAccessoryMagWeak_CB");
					weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(AccessoriesMagicStrength_Edit_IC, EMagicType.NONE, "AddAccessoryMagicStrength_CB");
					weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(AccessoriesWeakness_Edit_IC, EWeaponType.NONE, "AddAccessoryWeak_CB");
					weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(AccessoriesWeaponStrength_Edit_IC, EWeaponType.NONE, "AddAccessoryWeaknessStrength_CB");
					Createsql = "UPDATE `weaknesses_strengths` " +
											"SET " +
											String.Format("{0} = {1},", "physical_weaknesses", weaknessStrengths.physical_weaknesses) +
											String.Format("{0} = {1},", "physical_strengths", weaknessStrengths.physical_strengths) +
											String.Format("{0} = {1},", "magic_weaknesses", weaknessStrengths.magic_weaknesses) +
											String.Format("{0} = {1} ", "magic_strengths", weaknessStrengths.magic_strengths) +

											String.Format("WHERE id='{0}'", weaknessStrengths.ID);
					_sqlite_conn.Query<weaknesses_strengths>(Createsql);


					//Delete all the associated keys
					#region Key Deletion And reinsertion
					#region Effect/Trait

					Createsql = String.Format("DELETE FROM `modifier_keys` WHERE req_name = '{0}';", accessorydata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete

					foreach (ModifierData mdata in accessorydata.Effects)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = accessorydata.ID,
							Req_Table = "accessories"
						};
						_sqlite_conn.Insert(mod_key);
					}
					foreach (ModifierData mdata in accessorydata.Traits)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = accessorydata.ID,
							Req_Table = "accessories"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion

					#region Skill
					Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0}';", accessorydata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete
					foreach (Skill skill in accessorydata.AccessorySkills)
					{
						Skill_Keys mod_key = new Skill_Keys()
						{
							Skill_ID = skill.Name,
							Req_Name = accessorydata.ID,
							Req_Table = "accessories"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion
					#endregion

				}
				catch (Exception ex)
				{
					Console.WriteLine("Gameplay modifier Read from database FAILURE {0}:", ex.Message);
					GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [gameplay modifier] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}


		private void AccessoryName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//We need to populate the data to the GUI.
			Accessory currentAccessory = CurrentAccessoriesInDatabase[AccessoryName_Edit_CB.SelectedIndex];
			AccessoryType_Edit_CB.SelectedItem = (EAccessoryType)currentAccessory.Accessory_Type;
			AccessoryRarity_Edit_CB.SelectedItem = (ERarityType)currentAccessory.Rarity;
			AccessoryFuncPTR_Edit_TB.Text = currentAccessory.Function_PTR;        //1.0.0.3v

			#region Elemental "Binding"
			//reset
			SetMagicTypesData(AccessoryMagicTypesEquip_Edit_IC, null, EMagicType.NONE, true);
			SetMagicTypesData(AccessoryMagicTypesEquip_Edit_IC, currentAccessory.Elemental, EMagicType.NONE, false);
			WaponsMagicTypesEquip_Edit_IC.UpdateLayout();
			#endregion

			#region Base Stats
			String Createsql = "SELECT * FROM `base_stats`;";
			List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

			Base_Stats baseStats = bsList.Single(x => x.ID == currentAccessory.Stats_FK);
			currentAccessory.Stats = baseStats;
			#endregion
			#region Weaknesses and strengths
			Createsql = "SELECT * FROM `weaknesses_strengths`;";
			List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

			weaknesses_strengths wsData = wsList.Single(x => x.ID == currentAccessory.Weakness_Strength_FK);
			currentAccessory.WeaknessAndStrengths = wsData;
			#endregion

			#region Stats
			AccessoriesMaxHP_Edit_TB.Text = currentAccessory.Stats.Max_Health.ToString();
			AccessoriesMaxMP_Edit_TB.Text = currentAccessory.Stats.Max_Mana.ToString();


			AccessoriesAtk_Edit_TB.Text = currentAccessory.Stats.Attack.ToString();
			AccessoriesDef_Edit_TB.Text = currentAccessory.Stats.Defense.ToString();
			AccessoriesDex_Edit_TB.Text = currentAccessory.Stats.Dexterity.ToString();
			AccessoriesAgl_Edit_TB.Text = currentAccessory.Stats.Agility.ToString();
			AccessoriesMor_Edit_TB.Text = currentAccessory.Stats.Morality.ToString();

			AccessoriesWis_Edit_TB.Text = currentAccessory.Stats.Wisdom.ToString();
			AccessoriesRes_Edit_TB.Text = currentAccessory.Stats.Resistance.ToString();
			AccessoriesLuc_Edit_TB.Text = currentAccessory.Stats.Luck.ToString();
			AccessoriesRsk_Edit_TB.Text = currentAccessory.Stats.Risk.ToString();
			AccessoriesItl_Edit_TB.Text = currentAccessory.Stats.Intelligence.ToString();
			#endregion

			//Check boxes time!
			#region Weaknesses and strengths

			#region Elemental "Binding"
			//reset
			SetItemControlCheckboxData(AccessoriesMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddAccessoryMagWeak_CB", true);
			SetItemControlCheckboxData(AccessoriesMagicWeakness_Edit_IC, currentAccessory.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddAccessoryMagWeak_CB", false);

			SetItemControlCheckboxData(AccessoriesMagicStrength_Edit_IC, null, EMagicType.NONE, "AddAccessoryMagicStrength_CB", true);
			SetItemControlCheckboxData(AccessoriesMagicStrength_Edit_IC, currentAccessory.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddAccessoryMagicStrength_CB", false);
			AccessoriesMagicWeakness_Edit_IC.UpdateLayout();
			AccessoriesMagicStrength_Edit_IC.UpdateLayout();
			#endregion

			#region Weakness & Strengths "Binding"
			//reset
			SetItemControlCheckboxData(AccessoriesWeakness_Edit_IC, null, EMagicType.NONE, "AddAccessoryWeak_CB", true);
			SetItemControlCheckboxData(AccessoriesWeakness_Edit_IC, currentAccessory.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddAccessoryWeak_CB", false);

			SetItemControlCheckboxData(AccessoriesWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddAccessoryWeaknessStrength_CB", true);
			SetItemControlCheckboxData(AccessoriesWeaponStrength_Edit_IC, currentAccessory.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddAccessoryWeaknessStrength_CB", false);
			AccessoriesWeaponStrength_Edit_IC.UpdateLayout();
			AccessoriesWeaponStrength_Edit_IC.UpdateLayout();
			#endregion
			#endregion

			#region Effect/Traits Binding

			foreach (ModifierData modd in currentAccessory.Effects)
			{
				AccessoryEffectEquip_Edit_IC.Items.Add(modd.Id);
			}
			foreach (ModifierData modd in currentAccessory.Traits)
			{
				AccessoryTraitsEquip_Edit_IC.Items.Add(modd.Id);
			}
			#endregion

			#region Skills Binding

			foreach (Skill skill in currentAccessory.AccessorySkills)
			{
				AccessorySkillsEquip_Edit_IC.Items.Add(skill);
			}

			#endregion

		}

		private void ClothesEquipEffect_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipEffects_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesEffectEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Effects[ClothesEquipEffects_Add_CB.SelectedIndex].Id;
					if (!ClothesEffectEquip_Add_IC.Items.Contains(effectname))
					{
						ClothesEffectEquip_Add_IC.Items.Add(
							effectname
						);
						ClothesEffectEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void ClothesEquipTrait_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipTraits_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesTraitsEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Traits[ClothesEquipTraits_Add_CB.SelectedIndex].Id;
					if (!ClothesTraitsEquip_Add_IC.Items.Contains(effectname))
					{
						ClothesTraitsEquip_Add_IC.Items.Add(
							effectname
						);
						ClothesTraitsEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}

		private void ClothesEquipSkill_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipSkills_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesSkillsEquip_Add_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentSkillsInDatabase[ClothesEquipSkills_Add_CB.SelectedIndex].Name;
					if (!ClothesSkillsEquip_Add_IC.Items.Contains(effectname))
					{
						ClothesSkillsEquip_Add_IC.Items.Add(
							effectname
						);
						ClothesSkillsEquip_Add_IC.UpdateLayout();
					}
				}
			}
		}


		private void AddClothesToDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			//first up checking for validity.
			if (ClothesName_Add_TB.Text.Length > 0 &&
					int.TryParse(ClothesWeight_Add_TB.Text, out int weightval) &&
					ClothesType_Add_CB.SelectedIndex >= 0 && ClothesRarity_Add_CB.SelectedIndex >= 0 &&

					int.TryParse(ClothesMaxHP_Add_TB.Text, out int maxHpResult) &&
					int.TryParse(ClothesMaxMP_Add_TB.Text, out int maxMPResult) &&

					int.TryParse(ClothesAtk_Add_TB.Text, out int atkResult) &&
					int.TryParse(ClothesDef_Add_TB.Text, out int defResult) &&
					int.TryParse(ClothesDex_Add_TB.Text, out int dexResult) &&
					int.TryParse(ClothesAgl_Add_TB.Text, out int aglResult) &&
					int.TryParse(ClothesMor_Add_TB.Text, out int morResult) &&
					int.TryParse(ClothesWis_Add_TB.Text, out int wisResult) &&
					int.TryParse(ClothesRes_Add_TB.Text, out int resResult) &&
					int.TryParse(ClothesLuc_Add_TB.Text, out int LucResult) &&
					int.TryParse(ClothesRsk_Add_TB.Text, out int RskResult) &&
					int.TryParse(ClothesItl_Add_TB.Text, out int itlResult)
					) //  1.0.0.2v
			{
				//At this point you can add to the database.

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

					#region Base_Stats

					Createsql = "SELECT * FROM `base_stats`;";
					List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);
					int newID_stat = (bsList.Count == 0 ? 0 : bsList.Max(x => x.ID));

					//Set up the weapon object!
					Base_Stats basestat = new Base_Stats()
					{
						ID = newID_stat + 1,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					#endregion

					_sqlite_conn.Insert(basestat);

					#region Weakness and Strengths
					weaknesses_strengths weakstrToAdd = new weaknesses_strengths();
					int phyweak, phystr, magweak, magstr;

					Createsql = "SELECT * FROM `weaknesses_strengths`;";
					List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);
					int newID_weakstr = (wsList.Count == 0 ? 0 : wsList.Max(x => x.ID));
					weakstrToAdd.ID = newID_weakstr + 1;
					//GET THE MAGIC WEAKNESS ENUMERATED BITS
					#region Magic Weakness
					int i = 0;
					magweak = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ClothesMagicWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddClothesMagWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_weaknesses = magweak;
					#endregion

					#region physical weakness
					i = 0;
					phyweak = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ClothesWeakness_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddClothesWeak_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phyweak += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_weaknesses = phyweak;
					#endregion

					#region magic strength
					i = 0;
					magstr = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ClothesMagicStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddClothesMagicStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magstr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.magic_strengths = magstr;
					#endregion

					#region physical strength
					i = 0;
					phystr = 0;
					foreach (int en in Enum.GetValues(typeof(EWeaponType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ClothesWeaponStrength_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddClothesWeaknessStrength_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							phystr += (int)Math.Pow(2, i);
						}
						i++;
					}
					weakstrToAdd.physical_strengths = phystr;
					#endregion

					#endregion
					_sqlite_conn.Insert(weakstrToAdd);


					//Set up the weapon object!
					Clothes Clothes = new Clothes()
					{
						ID = ClothesName_Add_TB.Text,
						Clothes_Type = (int)((EWeaponType)ClothesType_Add_CB.SelectedValue),
						Rarity = (int)((ERarityType)ClothesRarity_Add_CB.SelectedValue),
						Weight = weightval,
						Function_PTR = ClothesFuncPTR_Add_TB.Text, //1.0.0.3v

						Stats_FK = basestat.ID,
						Weakness_Strength_FK = weakstrToAdd.ID
					};

					//GET THE MAGIC TYPE ENUMERATED BITS
					#region Magic types
					i = 0;
					int magictypesval = 0;
					foreach (int en in Enum.GetValues(typeof(EMagicType)))
					{
						if (en == 0) continue;
						ContentPresenter c = ((ContentPresenter)ClothesMagicTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
						var vv = c.ContentTemplate.FindName("AddItemMagicTypes_CB", c);

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magictypesval += (int)Math.Pow(2, i);
						}
						i++;
					}
					Clothes.Elemental = magictypesval;
					#endregion

					//DNE
					#region Item Types
					//i = 0;
					//int itemstypesval = 0;
					//foreach (int en in Enum.GetValues(typeof(EItemType)))
					//{
					//	if (en == 0) continue;
					//	ContentPresenter c = ((ContentPresenter)ItemTypesEquip_Add_IC.ItemContainerGenerator.ContainerFromIndex(i));
					//	var vv = c.ContentTemplate.FindName("AddItemTypes_CB", c);

					//	if ((bool)(vv as CheckBox).IsChecked)
					//	{
					//		itemstypesval += (int)Math.Pow(2, i);
					//	}
					//	i++;
					//}
					//item.Item_Type = itemstypesval;
					#endregion

					#region Create Keys Entries
					#region Effects/Traits
					InsertRecordIntoModifierKeys(ItemEffectEquip_Add_IC, _sqlite_conn, "Clothes", Clothes.ID);
					InsertRecordIntoModifierKeys(ItemTraitsEquip_Add_IC, _sqlite_conn, "Clothes", Clothes.ID);
					#endregion
					#region Skills
					InsertRecordIntoSkillKeys(WeaponSkillsEquip_Add_IC, _sqlite_conn, "Clothes", Clothes.ID);
					#endregion
					#endregion

					//Add it to the databse
					int retval = _sqlite_conn.Insert(Clothes);
					Console.WriteLine("RowID Val: {0}", retval);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Weapons write from database [Clothes] FAILURE | {0}", ex.Message);
					GlobalStatusLog_TB.Text =
						String.Format("Loading/Writing Database [Clothes] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}


		private void ClothesEquipEffect_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipEffects_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesEffectEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Effects[ClothesEquipEffects_Edit_CB.SelectedIndex].Id;
					if (!ClothesEffectEquip_Edit_IC.Items.Contains(effectname))
					{
						ClothesEffectEquip_Edit_IC.Items.Add(effectname);
						CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].Effects.Add(
							CurrenGameplayModifiersInDatabase_Effects[ClothesEquipEffects_Edit_CB.SelectedIndex]);

						ClothesEffectEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void RemoveClothesEffect_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesEffectEquip_Add_IC.Items.IndexOf(item);

			ClothesEffectEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveClothesEffect_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesEffectEquip_Edit_IC.Items.IndexOf(item);

			ClothesEffectEquip_Edit_IC.Items.RemoveAt(index);

			CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].Effects.RemoveAt(index);

		}

		private void ClothesEquipTrait_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipTraits_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesTraitsEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrenGameplayModifiersInDatabase_Traits[ClothesEquipTraits_Edit_CB.SelectedIndex].Id;
					if (!ClothesTraitsEquip_Edit_IC.Items.Contains(effectname))
					{
						ClothesTraitsEquip_Edit_IC.Items.Add(effectname);
						CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].Traits.Add(
							CurrenGameplayModifiersInDatabase_Traits[ClothesEquipTraits_Edit_CB.SelectedIndex]);
						ClothesTraitsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}

		private void RemoveClothesTrait_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesTraitsEquip_Add_IC.Items.IndexOf(item);

			ClothesTraitsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveClothesTrait_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesTraitsEquip_Edit_IC.Items.IndexOf(item);

			ClothesTraitsEquip_Edit_IC.Items.RemoveAt(index);

			CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].Traits.RemoveAt(index);
		}


		private void ClothesEquipSkill_Edit_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = ClothesEquipSkills_Edit_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (ClothesSkillsEquip_Edit_IC.Items.Count >= 2) return;
				else
				{
					string effectname = CurrentSkillsInDatabase[ClothesEquipSkills_Edit_CB.SelectedIndex].Name;
					if (!ClothesSkillsEquip_Edit_IC.Items.Contains(effectname))
					{
						ClothesSkillsEquip_Edit_IC.Items.Add(effectname);
						CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].ClothesSkills.Add(
							CurrentSkillsInDatabase[ClothesEquipSkills_Edit_CB.SelectedIndex]);
						ClothesSkillsEquip_Edit_IC.UpdateLayout();
					}
				}
			}
		}


		private void RemoveClothesSkill_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesSkillsEquip_Add_IC.Items.IndexOf(item);

			ClothesSkillsEquip_Add_IC.Items.RemoveAt(index);
		}

		private void RemoveClothesSkill_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = ClothesSkillsEquip_Edit_IC.Items.IndexOf(item);

			ClothesSkillsEquip_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex].ClothesSkills.RemoveAt(index);
		}

		private void UpdateClothesInDatabase_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (
					int.TryParse(ClothesWeight_Edit_TB.Text, out int weightval) &&
					int.TryParse(ClothesMaxHP_Edit_TB.Text, out int maxHpResult) &&
					int.TryParse(ClothesMaxMP_Edit_TB.Text, out int maxMPResult) &&

					int.TryParse(ClothesAtk_Edit_TB.Text, out int atkResult) &&
					int.TryParse(ClothesDef_Edit_TB.Text, out int defResult) &&
					int.TryParse(ClothesDex_Edit_TB.Text, out int dexResult) &&
					int.TryParse(ClothesAgl_Edit_TB.Text, out int aglResult) &&
					int.TryParse(ClothesMor_Edit_TB.Text, out int morResult) &&
					int.TryParse(ClothesWis_Edit_TB.Text, out int wisResult) &&
					int.TryParse(ClothesRes_Edit_TB.Text, out int resResult) &&
					int.TryParse(ClothesLuc_Edit_TB.Text, out int LucResult) &&
					int.TryParse(ClothesRsk_Edit_TB.Text, out int RskResult) &&
					int.TryParse(ClothesItl_Edit_TB.Text, out int itlResult)
			) //1.0.0.2v
			{

				//before we send the SQL update query we need to update the info in memory.
				int absindex = ClothesName_Edit_CB.SelectedIndex;
				Clothes Clothesdata = CurrentClothesInDatabase[absindex];

				Clothesdata.Clothes_Type = ClothesType_Edit_CB.SelectedIndex;
				Clothesdata.Rarity = ClothesRarity_Edit_CB.SelectedIndex;
				Clothesdata.Weight = weightval;
				Clothesdata.Function_PTR = ClothesFuncPTR_Edit_TB.Text; // 1.0.0.3v
				//TODO: Add the wepdata weakness after that table is done in this tool.
				//itemdata.Weakness_Strength_FK = 

				//GET THE MAGIC TYPE ENUMERATED BITS
				#region Magic types
				int i = 0;
				int magictypesval = 0;
				foreach (int en in Enum.GetValues(typeof(EMagicType)))
				{
					if (en == 0) continue;
					ContentPresenter c = ((ContentPresenter)ClothesMagicTypesEquip_Edit_IC.ItemContainerGenerator.ContainerFromIndex(i));
					var vv = c.ContentTemplate.FindName("AddWeaponMagicTypes_CB", c);

					if ((bool)(vv as CheckBox).IsChecked)
					{
						magictypesval += (int)Math.Pow(2, i);
					}

					i++;
				}
				Clothesdata.Elemental = magictypesval;
				#endregion

				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";
					//Createsql = ("SELECT * FROM `gameplay_modifiers`;");

					Createsql = "UPDATE `clothes` " +
											"SET " +
											String.Format("{0} = {1},", "elemental", Clothesdata.Elemental) +
											String.Format("{0} = {1},", "weight", Clothesdata.Weight) +
											String.Format("{0} = {1},", "clothes_type", Clothesdata.Clothes_Type) +
											String.Format("{0} = '{1}',", "function_ptr", Clothesdata.Function_PTR) + //1.0.0.3v

											String.Format("{0} = {1},", "rarity", Clothesdata.Rarity) +
											String.Format("{0} = {1} ", "weakness_strength_fk", Clothesdata.Weakness_Strength_FK) +
											String.Format("WHERE id='{0}'", Clothesdata.ID);
					_sqlite_conn.Query<Clothes>(Createsql);


					Base_Stats stats = (Base_Stats)CurrentClothesInDatabase[absindex].Stats;
					Base_Stats base_stats = new Base_Stats()
					{
						ID = stats.ID,
						Max_Health = maxHpResult,
						Max_Mana = maxMPResult,
						Attack = atkResult,
						Defense = defResult,
						Dexterity = dexResult,
						Agility = aglResult,
						Morality = morResult,
						Wisdom = wisResult,
						Resistance = resResult,
						Luck = LucResult,
						Risk = RskResult,
						Intelligence = itlResult
					};
					Createsql = "";
					Createsql = "UPDATE `base_stats` " +
											"SET " +
											String.Format("{0} = {1},", "max_health", base_stats.Max_Health) +
											String.Format("{0} = {1},", "max_mana", base_stats.Max_Mana) +

											String.Format("{0} = {1},", "attack", base_stats.Attack) +
											String.Format("{0} = {1},", "defense", base_stats.Defense) +
											String.Format("{0} = {1},", "dexterity", base_stats.Dexterity) +
											String.Format("{0} = {1},", "agility", base_stats.Agility) +
											String.Format("{0} = {1},", "morality", base_stats.Morality) +

											String.Format("{0} = {1},", "wisdom", base_stats.Wisdom) +
											String.Format("{0} = {1},", "resistance", base_stats.Resistance) +
											String.Format("{0} = {1},", "luck", base_stats.Luck) +
											String.Format("{0} = {1},", "risk", base_stats.Risk) +
											String.Format("{0} = {1} ", "intelligence", base_stats.Intelligence) +

											String.Format("WHERE id='{0}'", base_stats.ID);
					_sqlite_conn.Query<Base_Stats>(Createsql);


					weaknesses_strengths weaknessStrengths = (weaknesses_strengths)CurrentClothesInDatabase[absindex].WeaknessAndStrengths;
					weaknessStrengths.ID = Clothesdata.Weakness_Strength_FK;
					weaknessStrengths.magic_weaknesses = GetBitWiseEnumeratedValFromIC(ClothesMagicWeakness_Edit_IC, EMagicType.NONE, "AddClothesMagWeak_CB");
					weaknessStrengths.magic_strengths = GetBitWiseEnumeratedValFromIC(ClothesMagicStrength_Edit_IC, EMagicType.NONE, "AddClothesMagicStrength_CB");
					weaknessStrengths.physical_weaknesses = GetBitWiseEnumeratedValFromIC(ClothesWeakness_Edit_IC, EWeaponType.NONE, "AddClothesWeak_CB");
					weaknessStrengths.physical_strengths = GetBitWiseEnumeratedValFromIC(ClothesWeaponStrength_Edit_IC, EWeaponType.NONE, "AddClothesWeaknessStrength_CB");
					Createsql = "UPDATE `weaknesses_strengths` " +
											"SET " +
											String.Format("{0} = {1},", "physical_weaknesses", weaknessStrengths.physical_weaknesses) +
											String.Format("{0} = {1},", "physical_strengths", weaknessStrengths.physical_strengths) +
											String.Format("{0} = {1},", "magic_weaknesses", weaknessStrengths.magic_weaknesses) +
											String.Format("{0} = {1} ", "magic_strengths", weaknessStrengths.magic_strengths) +

											String.Format("WHERE id='{0}'", weaknessStrengths.ID);
					_sqlite_conn.Query<weaknesses_strengths>(Createsql);

					//Delete all the associated keys
					#region Key Deletion And reinsertion
					#region Effect/Trait

					Createsql = String.Format("DELETE FROM `modifier_keys` WHERE req_name = '{0}';", Clothesdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete

					foreach (ModifierData mdata in Clothesdata.Effects)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = Clothesdata.ID,
							Req_Table = "clothes"
						};
						_sqlite_conn.Insert(mod_key);
					}
					foreach (ModifierData mdata in Clothesdata.Traits)
					{
						Modifier_Keys mod_key = new Modifier_Keys()
						{
							Modifier_ID = mdata.Id,
							Req_Name = Clothesdata.ID,
							Req_Table = "clothes"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion

					#region Skill
					Createsql = String.Format("DELETE FROM `skill_keys` WHERE req_name = '{0}';", Clothesdata.ID);
					_sqlite_conn.Query<ModifierData>(Createsql); //delete
					foreach (Skill skill in Clothesdata.ClothesSkills)
					{
						Skill_Keys mod_key = new Skill_Keys()
						{
							Skill_ID = skill.Name,
							Req_Name = Clothesdata.ID,
							Req_Table = "clothes"
						};
						_sqlite_conn.Insert(mod_key);
					}
					#endregion
					#endregion

				}
				catch (Exception ex)
				{
					Console.WriteLine("clothes Read from database FAILURE {0}:", ex.Message);
					GlobalStatusLog_TB.Text = String.Format("Loading/Reading Database [clothes] failed: {0}", ex.Message);
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}

		private void ClothesName_Edit_CB_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//We need to populate the data to the GUI.
			Clothes currentClothes = CurrentClothesInDatabase[ClothesName_Edit_CB.SelectedIndex];
			ClothesType_Edit_CB.SelectedItem = (EClothesType)currentClothes.Clothes_Type;
			ClothesRarity_Edit_CB.SelectedItem = (ERarityType)currentClothes.Rarity;
			ClothesFuncPTR_Edit_TB.Text = currentClothes.Function_PTR;        //1.0.0.3v

			#region Elemental "Binding"
			//reset
			SetMagicTypesData(ClothesMagicTypesEquip_Edit_IC, null, EMagicType.NONE, true);
			SetMagicTypesData(ClothesMagicTypesEquip_Edit_IC, currentClothes.Elemental, EMagicType.NONE, false);
			WaponsMagicTypesEquip_Edit_IC.UpdateLayout();
			#endregion

			#region Base Stats
			String Createsql = "SELECT * FROM `base_stats`;";
			List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

			Base_Stats baseStats = bsList.Single(x => x.ID == currentClothes.Stats_FK);
			currentClothes.Stats = baseStats;
			#endregion
			#region Weaknesses and strengths
			Createsql = "SELECT * FROM `weaknesses_strengths`;";
			List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

			weaknesses_strengths wsData = wsList.Single(x => x.ID == currentClothes.Weakness_Strength_FK);
			currentClothes.WeaknessAndStrengths = wsData;
			#endregion

			#region Stats
			ClothesMaxHP_Edit_TB.Text = currentClothes.Stats.Max_Health.ToString();
			ClothesMaxMP_Edit_TB.Text = currentClothes.Stats.Max_Mana.ToString();


			ClothesAtk_Edit_TB.Text = currentClothes.Stats.Attack.ToString();
			ClothesDef_Edit_TB.Text = currentClothes.Stats.Defense.ToString();
			ClothesDex_Edit_TB.Text = currentClothes.Stats.Dexterity.ToString();
			ClothesAgl_Edit_TB.Text = currentClothes.Stats.Agility.ToString();
			ClothesMor_Edit_TB.Text = currentClothes.Stats.Morality.ToString();

			ClothesWis_Edit_TB.Text = currentClothes.Stats.Wisdom.ToString();
			ClothesRes_Edit_TB.Text = currentClothes.Stats.Resistance.ToString();
			ClothesLuc_Edit_TB.Text = currentClothes.Stats.Luck.ToString();
			ClothesRsk_Edit_TB.Text = currentClothes.Stats.Risk.ToString();
			ClothesItl_Edit_TB.Text = currentClothes.Stats.Intelligence.ToString();
			#endregion

			//Check boxes time!
			#region Weaknesses and strengths

			#region Elemental "Binding"
			//reset
			SetItemControlCheckboxData(ClothesMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddClothesMagWeak_CB", true);
			SetItemControlCheckboxData(ClothesMagicWeakness_Edit_IC, currentClothes.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddClothesMagWeak_CB", false);

			SetItemControlCheckboxData(ClothesMagicStrength_Edit_IC, null, EMagicType.NONE, "AddClothesMagicStrength_CB", true);
			SetItemControlCheckboxData(ClothesMagicStrength_Edit_IC, currentClothes.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddClothesMagicStrength_CB", false);
			ClothesMagicWeakness_Edit_IC.UpdateLayout();
			ClothesMagicStrength_Edit_IC.UpdateLayout();
			#endregion

			#region Weakness & Strengths "Binding"
			//reset
			SetItemControlCheckboxData(ClothesWeakness_Edit_IC, null, EMagicType.NONE, "AddClothesWeak_CB", true);
			SetItemControlCheckboxData(ClothesWeakness_Edit_IC, currentClothes.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddClothesWeak_CB", false);

			SetItemControlCheckboxData(ClothesWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddClothesWeaknessStrength_CB", true);
			SetItemControlCheckboxData(ClothesWeaponStrength_Edit_IC, currentClothes.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddClothesWeaknessStrength_CB", false);
			ClothesWeaponStrength_Edit_IC.UpdateLayout();
			ClothesWeaponStrength_Edit_IC.UpdateLayout();
			#endregion
			#endregion


			#region Effect/Traits Binding

			foreach (ModifierData modd in currentClothes.Effects)
			{
				ClothesEffectEquip_Edit_IC.Items.Add(modd.Id);
			}
			foreach (ModifierData modd in currentClothes.Traits)
			{
				ClothesTraitsEquip_Edit_IC.Items.Add(modd.Id);
			}
			#endregion

			#region Skills Binding

			foreach (Skill skill in currentClothes.ClothesSkills)
			{
				ClothesSkillsEquip_Edit_IC.Items.Add(skill);
			}

			#endregion

		}


		private void BrowseForFollowUpCSV_2_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Follow up Attack sheet",
				FileName = "", //default file name
				Filter = "comma seperated file (*.csv)|*.csv",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);
			FollowUpAttackCSV_2_TB.Text = dlg.FileName;

			//FillFollowAttacks(dlg.FileName);
		}

		private void BrowseForFollowUpCSV_3_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Follow up Attack sheet",
				FileName = "", //default file name
				Filter = "comma seperated file (*.csv)|*.csv",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);
			FollowUpAttackCSV_3_TB.Text = dlg.FileName;

			//FillFollowAttacks(dlg.FileName);
		}

		private void BrowseForFollowUpCSV_4_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				Title = "Open Follow up Attack sheet",
				FileName = "", //default file name
				Filter = "comma seperated file (*.csv)|*.csv",
				RestoreDirectory = true
			};

			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return; //invalid name

			Console.WriteLine(dlg.FileName);
			FollowUpAttackCSV_4_TB.Text = dlg.FileName;

			//FillFollowAttacks(dlg.FileName);
		}

		private void ClearFollowUps_BTN_Click(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void RunForFollowUpCSV_BTN_Click(object sender, RoutedEventArgs e)
		{
			if (SQLDatabasePath == null)
			{
				GlobalStatusLog_TB.Text = "Please connect to the database First!";
				return;
			}
			if (CurrentFollowUpAttacksInDatabase.Count == 0)
			{
				FillFollowAttacks(FollowUpAttackCSV_2_TB.Text);
				FillFollowAttacks(FollowUpAttackCSV_3_TB.Text);
				FillFollowAttacks(FollowUpAttackCSV_4_TB.Text);

				//Now that its on the screen we need to put it in the database.
				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				foreach (FollowUpAttack followUpatk in CurrentFollowUpAttacksInDatabase)
				{
					FollowUp_Attacks temp = new FollowUp_Attacks()
					{
						Job_1 = followUpatk.Job_1,
						Job_2 = followUpatk.Job_2,
						Job_3 = followUpatk.Job_3,
						Job_4 = followUpatk.Job_4,
					};
					_sqlite_conn.Insert(temp);
				}
			}
		}


		private void FillFollowAttacks(String filepath)
		{
			FollowUpAttacks_LB.ItemsSource = null;

			using (var reader = new StreamReader(filepath))
			{
				List<string> LineData = new List<string>();
				while (!reader.EndOfStream)
				{
					String line = reader.ReadLine();
					LineData = line.Split(',').ToList();

					CurrentFollowUpAttacksInDatabase.Add(new FollowUpAttack()
					{
						Job_1 = LineData[0],
						Job_2 = LineData[1],
						Job_3 = LineData[2],
						Job_4 = LineData[3],
						Name = "NONE",
						Function_PTR = "NONE",
					});
				}
			}
			FollowUpAttacks_LB.ItemsSource = CurrentFollowUpAttacksInDatabase;
		}


		private void EditFollowUpAttacksInDB_BTN_Click(object sender, RoutedEventArgs e)
		{
			GlobalStatusLog_TB.Text = "";
			//Now that its on the screen we need to put it in the database.
			String masterfile = (SQLDatabasePath);
			_sqlite_conn = new SQLiteConnection(masterfile);
			String Createsql = "";

			try
			{

				foreach (FollowUpAttack fua in CurrentFollowUpAttacksInDatabase)
				{
					Createsql = "UPDATE `followup_attacks` " +
					            "SET " +
					            String.Format("{0} = '{1}',", "name", fua.Name) +
					            String.Format("{0} = '{1}' ", "function_ptr", fua.Function_PTR) +

					            String.Format("WHERE job_1='{0}' AND job_2='{1}' AND job_3='{2}' AND job_4='{3}';",
						            fua.Job_1, fua.Job_2, fua.Job_3, fua.Job_4);
					_sqlite_conn.Query<FollowUp_Attacks>(Createsql);
				}
			}
			catch ( Exception ex)
			{
				GlobalStatusLog_TB.Text = "Error Occured when updating follow up attacks: " + ex.Message;
			}
		}
	}
}


