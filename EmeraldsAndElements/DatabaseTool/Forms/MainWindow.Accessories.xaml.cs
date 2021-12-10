using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BixBite.Combat;
using BixBite.Combat.Equipables;
using SQLite;

namespace Forms.DatabaseTool
{
	public partial class MainWindow
	{

		public ObservableCollection<Accessory> CurrentAccessoriesInDatabase { get; set; }

		public void MainWindow_Accessories()
		{
			CurrentAccessoriesInDatabase = new ObservableCollection<Accessory>();
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



	}
}
