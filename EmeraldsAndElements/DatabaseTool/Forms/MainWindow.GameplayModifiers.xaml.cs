using BixBite.Combat;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Forms.DatabaseTool
{
	public partial class MainWindow
	{
		private List<int?> gameplayModifiersAdd_vals = new List<int?>(11);
		private List<int?> gameplayModifiersEdit_vals = new List<int?>(11);
		
		public ObservableCollection<ModifierData> CurrenGameplayModifiersInDatabase { get; set; }
		public ObservableCollection<ModifierData> CurrenGameplayModifiersInDatabase_Effects { get; set; }
		public ObservableCollection<ModifierData> CurrenGameplayModifiersInDatabase_Traits { get; set; }


		public ObservableCollection<Tuple<string, String>> CurrentGameplayModifier_Edit_AllMods { get; set; }


		public void MainWindow_GameplayModifiers()
		{
			CurrentGameplayModifier_Edit_AllMods = new ObservableCollection<Tuple<String, String>>();

			CurrenGameplayModifiersInDatabase = new ObservableCollection<ModifierData>();
			CurrenGameplayModifiersInDatabase_Effects = new ObservableCollection<ModifierData>();
			CurrenGameplayModifiersInDatabase_Traits = new ObservableCollection<ModifierData>();
			
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

			RecipeFireReward_Add_CB.ItemsSource = null;
			RecipeIceReward_Add_CB.ItemsSource = null;
			RecipeEarthReward_Add_CB.ItemsSource = null;
			RecipeWaterReward_Add_CB.ItemsSource = null;
			RecipeLightningReward_Add_CB.ItemsSource = null;
			RecipeExplosiveReward_Add_CB.ItemsSource = null;
			RecipeShadowReward_Add_CB.ItemsSource = null;
			RecipeLuminiousReward_Add_CB.ItemsSource = null;

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

				RecipeFireReward_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				RecipeIceReward_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				RecipeEarthReward_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				RecipeWaterReward_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				RecipeLightningReward_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				RecipeExplosiveReward_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				RecipeShadowReward_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
				RecipeLuminiousReward_Add_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
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
				(int?)CurrenGameplayModifiersInDatabase[index].Chance_Modifiers, EChanceEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, TurnEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Turn_Modifiers, ETurnEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, DamageEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Damage_Modifiers, EDamageModifiers.NONE);
			SetEditModifiersInDatabase(index, SeverityEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Severity_Modifiers, ESeverityEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, MagicDamageEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Magic_Damage_Modifiers, EMagicDamageModifiers.NONE);

			SetEditModifiersInDatabase(index, MagicDefenseEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Magic_Defense_Modifiers, EMagicDefenseModifiers.NONE);
			SetEditModifiersInDatabase(index, StatEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Stat_Modifiers, EStatEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, StatusEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Status_Effect_Modifiers, EStatusEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, NullifyStatusEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Nullify_Status_Effect_Modifiers,
				ENullifyStatusEffectModifiers.NONE);
			SetEditModifiersInDatabase(index, NonBattleEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].NonBattle_Modifiers, ENonBattleEffectModifiers.NONE);

			SetEditModifiersInDatabase(index, SpecialEnum_Edit_IC,
				(int?)CurrenGameplayModifiersInDatabase[index].Special_Modifiers, ESpecialEffectModifiers.NONE);

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
				ContentPresenter c = ((ContentPresenter)cuItemsControl.ItemContainerGenerator.ContainerFromIndex(i));
				var vv = c.ContentTemplate.FindName("5_T", c);
				if (bReset)
				{
					(vv as CheckBox).IsChecked = false;
				}
				else if ((en & (int)modifiervalue) > 0)
				{
					(vv as CheckBox).IsChecked = true;
					//This is here to display ALL the data to the user at once in an items control
					CurrentGameplayModifier_Edit_AllMods.Add(new Tuple<String, String>
						(etype.GetType().ToString().Substring(etype.GetType().ToString().LastIndexOf('.') + 2).Replace("Modifiers", ""),
						etype.GetType().GetEnumValues().GetValue(i + 1).ToString()));
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
			modifierData.bEffect = (bool)GameplayModifierEffect_Edit_CB.IsChecked;
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[0] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[0] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[0] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[1] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[1] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[1] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[2] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[2] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[2] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[3] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[3] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[3] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[4] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[4] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[4] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[5] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[5] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[5] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[6] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[6] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[6] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[7] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[7] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[7] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[8] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[8] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[8] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[9] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[9] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[9] -= (int)Math.Pow(2, index);
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
				int bitval = (int)Math.Pow(2, index);
				gameplayModifiersAdd_vals[10] = bitval;
			}
			else
			{
				//gameplayModifiersAdd_vals[0] &= (gameplayModifiersAdd_vals[0] | (int)Math.Pow(2, index));
				if ((bool)(sender as CheckBox).IsChecked)
				{
					gameplayModifiersAdd_vals[10] += (int)Math.Pow(2, index);
				}
				else
				{
					gameplayModifiersAdd_vals[10] -= (int)Math.Pow(2, index);
				}

				Console.WriteLine(gameplayModifiersAdd_vals[10]);
			}
		}

		#endregion

	}
}
