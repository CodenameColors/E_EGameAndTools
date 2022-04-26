using BixBite.Characters;
using BixBite.Combat;
using BixBite.Items;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public ObservableCollection<enemy> CurrentEnemiesInDatabase { get; set; }

		public void MainWindow_Enemies()
		{
			CurrentEnemiesInDatabase = new ObservableCollection<enemy>();
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
				SetOutputLog(String.Format("Loading/Reading [FROM enemy] Database failed: {0}", ex.Message));
			}
			finally
			{
				EnemyName_Edit_CB.ItemsSource = CurrentEnemiesInDatabase;

			}
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
					SetOutputLog(String.Format("Loading/Writing Database [items] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
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
					List<Weapon> result_weapons = CurrentWeaponsInDatabase.Where(p => WeaponList_keys.Any(p2 => p2.Weapon_ID == p.ID)).ToList() as List<Weapon>;
					enemy.Weapons = new LinkedList<Weapon>(result_weapons);
					result_weapons.ForEach(x => EnemyWeapon_Edit_IC.Items.Add((x.ID)));
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
					SetOutputLog(String.Format("Loading/Writing Database [Filling Party member Data ] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
					EnemyName_Edit_CB.ItemsSource = CurrentEnemiesInDatabase;

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
					SetOutputLog(String.Format("Loading/Reading Database [enemy] failed: {0}", ex.Message));
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
				CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Weapons.AddFirst((Weapon)EnemyItems_Edit_CB.SelectedValue);
			}
		}

		private void RemoveEnemyWeapon_Edit_BTN_OnClick(object sender, RoutedEventArgs e)
		{
			var item = (VisualTreeHelper.GetParent(sender as Button) as Grid).DataContext;
			int index = EnemyWeapon_Edit_IC.Items.IndexOf(item);

			EnemyWeapon_Edit_IC.Items.RemoveAt(index);
			//edit the live data.
			CurrentEnemiesInDatabase[EnemyName_Edit_CB.SelectedIndex].Weapons.RemoveAt(index);
		}



	}
}
