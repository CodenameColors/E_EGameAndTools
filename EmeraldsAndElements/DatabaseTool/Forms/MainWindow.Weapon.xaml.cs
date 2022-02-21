using BixBite.Combat;
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

		public ObservableCollection<Weapon> CurrentWeaponsInDatabase { get; set; }


		public void MainWindow_Weapons()
		{

			CurrentWeaponsInDatabase = new ObservableCollection<Weapon>();

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
						ModifierData moddata = CurrentGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
						if (moddata == null) continue;
						if (moddata.bEffect)
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
				SetOutputLog(String.Format("Loading/Reading [FROM WEAPONS] Database failed: {0}", ex.Message));
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




		private void AddEffectToWeapon_Add_BTN_Click(object sender, RoutedEventArgs e)
		{
			ComboBox CB = WeaponEffects_Add_CB;
			if (CB.SelectedIndex >= 0)
			{
				if (WeaponEffectEquip_Add_IC.Items.Count >= 3) return;
				else
				{
					string effectname = CurrentGameplayModifiersInDatabase_Effects[WeaponEffects_Add_CB.SelectedIndex].Id;
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
					string effectname = CurrentGameplayModifiersInDatabase_Effects[WeaponEffects_Edit_CB.SelectedIndex].Id;
					if (!WeaponEffectEquip_Edit_IC.Items.Contains(effectname))
					{
						WeaponEffectEquip_Edit_IC.Items.Add(effectname);
						CurrentWeaponsInDatabase[WeaponName_Edit_CB.SelectedIndex].Effects.Add(
							CurrentGameplayModifiersInDatabase_Effects[WeaponEffects_Edit_CB.SelectedIndex]
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
					string traitname = CurrentGameplayModifiersInDatabase_Traits[WeaponTraits_Add_CB.SelectedIndex].Id;
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
					string traitname = CurrentGameplayModifiersInDatabase_Traits[WeaponTraits_Edit_CB.SelectedIndex].Id;
					if (!WeaponTraitsEquip_Edit_IC.Items.Contains(traitname))
					{
						WeaponTraitsEquip_Edit_IC.Items.Add(traitname);

						CurrentWeaponsInDatabase[WeaponName_Edit_CB.SelectedIndex].Traits.Add(
							CurrentGameplayModifiersInDatabase_Effects[WeaponTraits_Edit_CB.SelectedIndex]);
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
					WeaponRarity_Add_CB.SelectedIndex > 0 &&

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
					)
			{
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
						bDamage = (bool)WeaponDamage_Add_CB.IsChecked,
						//Inflicting_value = inflictval,  // This doesn't exist anymore. was phased out by Str&weak stats
						Weapon_Type = (int)((EWeaponType)WeaponType_Add_CB.SelectedValue),
						Weight = weightval,
						Rarity = (int)((ERarityType)WeaponRarity_Add_CB.SelectedValue)
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

						if ((bool)(vv as CheckBox).IsChecked)
						{
							magictypesval += (int)Math.Pow(2, i);
						}

						i++;
					}
					weapon.Elemental = magictypesval;
					#endregion
					#region Create Keys Entries
					#region Effects/Traits
					InsertRecordIntoModifierKeys(WeaponEffectEquip_Add_IC, _sqlite_conn, "weapons", weapon.ID);
					InsertRecordIntoModifierKeys(WeaponTraitsEquip_Add_IC, _sqlite_conn, "weapons", weapon.ID);
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
					SetOutputLog(String.Format("Loading/Writing Database [weapons] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					//GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
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

				Weapon tempWeapon = CurrentWeaponsInDatabase[cb.SelectedIndex] ?? throw new ArgumentNullException("CurrentWeaponsInDatabase[cb.SelectedIndex]");

				WeaponDamage_Edit_CB.IsChecked = tempWeapon.bDamage;
				//WeapondDamage_Edit_TB.Text = tempWeapon.Inflicting_value.ToString(); //// This doesn't exist anymore. was phased out by Str&weak stats
				WeaponType_Edit_CB.SelectedIndex = tempWeapon.Weapon_Type;
				WeaponWeight_Edit_TB.Text = tempWeapon.Weight.ToString();
				WeaponRarity_Edit_CB.SelectedIndex = tempWeapon.Rarity;
				//TODO: Add the weapon weakness after we make this table work
				// WeaponWeakStrength_Edit_CB.SelectedItem =

				#region Base Stats
				String Createsql = "SELECT * FROM `base_stats`;";
				List<Base_Stats> bsList = _sqlite_conn.Query<Base_Stats>(Createsql);

				Base_Stats baseStats = bsList.Single(x => x.ID == tempWeapon.Stats_FK);
				tempWeapon.Stats = baseStats;
				#endregion
				#region Weaknesses and strengths
				Createsql = "SELECT * FROM `weaknesses_strengths`;";
				List<weaknesses_strengths> wsList = _sqlite_conn.Query<weaknesses_strengths>(Createsql);

				weaknesses_strengths wsData = wsList.Single(x => x.ID == tempWeapon.Weakness_Strength_FK);
				tempWeapon.WeaknessAndStrengths = wsData;
				#endregion

				#region Stats
				WeaponsMaxHP_Edit_TB.Text = tempWeapon.Stats.Max_Health.ToString();
				WeaponsMaxMP_Edit_TB.Text = tempWeapon.Stats.Max_Mana.ToString();


				WeaponsAtk_Edit_TB.Text = tempWeapon.Stats.Attack.ToString();
				WeaponsDef_Edit_TB.Text = tempWeapon.Stats.Defense.ToString();
				WeaponsDex_Edit_TB.Text = tempWeapon.Stats.Dexterity.ToString();
				WeaponsAgl_Edit_TB.Text = tempWeapon.Stats.Agility.ToString();
				WeaponsMor_Edit_TB.Text = tempWeapon.Stats.Morality.ToString();

				WeaponsWis_Edit_TB.Text = tempWeapon.Stats.Wisdom.ToString();
				WeaponsRes_Edit_TB.Text = tempWeapon.Stats.Resistance.ToString();
				WeaponsLuc_Edit_TB.Text = tempWeapon.Stats.Luck.ToString();
				WeaponsRsk_Edit_TB.Text = tempWeapon.Stats.Risk.ToString();
				WeaponsItl_Edit_TB.Text = tempWeapon.Stats.Intelligence.ToString();
				#endregion

				//Check boxes time!
				#region Weaknesses and strengths

				#region Elemental "Binding"
				//reset
				SetItemControlCheckboxData(WeaponsMagicWeakness_Edit_IC, null, EMagicType.NONE, "AddWeaponsMagWeak_CB", true);
				SetItemControlCheckboxData(WeaponsMagicWeakness_Edit_IC, tempWeapon.WeaknessAndStrengths.magic_weaknesses, EMagicType.NONE, "AddWeaponsMagWeak_CB", false);

				SetItemControlCheckboxData(WeaponsMagicStrength_Edit_IC, null, EMagicType.NONE, "AddWeaponsMagicStrength_CB", true);
				SetItemControlCheckboxData(WeaponsMagicStrength_Edit_IC, tempWeapon.WeaknessAndStrengths.magic_strengths, EMagicType.NONE, "AddWeaponsMagicStrength_CB", false);
				WeaponsMagicWeakness_Edit_IC.UpdateLayout();
				WeaponsMagicStrength_Edit_IC.UpdateLayout();
				#endregion

				#region Weakness & Strengths "Binding"
				//reset
				SetItemControlCheckboxData(WeaponsWeakness_Edit_IC, null, EMagicType.NONE, "AddWeaponsWeak_CB", true);
				SetItemControlCheckboxData(WeaponsWeakness_Edit_IC, tempWeapon.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddWeaponsWeak_CB", false);

				SetItemControlCheckboxData(WeaponsWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddWeaponsWeaknessStrength_CB", true);
				SetItemControlCheckboxData(WeaponsWeaponStrength_Edit_IC, tempWeapon.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddWeaponsWeaknessStrength_CB", false);
				WeaponsWeaponStrength_Edit_IC.UpdateLayout();
				WeaponsWeaponStrength_Edit_IC.UpdateLayout();
				#endregion
				#endregion
				#region Weakness & Strengths "Binding"
				//reset
				SetItemControlCheckboxData(WeaponsWeakness_Edit_IC, null, EMagicType.NONE, "AddWeaponsWeak_CB", true);
				SetItemControlCheckboxData(WeaponsWeakness_Edit_IC, tempWeapon.WeaknessAndStrengths.physical_weaknesses, EWeaponType.NONE, "AddWeaponsWeak_CB", false);

				SetItemControlCheckboxData(WeaponsWeaponStrength_Edit_IC, null, EWeaponType.NONE, "AddWeaponsWeaknessStrength_CB", true);
				SetItemControlCheckboxData(WeaponsWeaponStrength_Edit_IC, tempWeapon.WeaknessAndStrengths.physical_strengths, EWeaponType.NONE, "AddWeaponsWeaknessStrength_CB", false);
				WeaponsWeaponStrength_Edit_IC.UpdateLayout();
				WeaponsWeaponStrength_Edit_IC.UpdateLayout();
				#endregion
				


				#region Elemental "Binding"
				//reset
				SetMagicTypesData(WaponsMagicTypesEquip_Edit_IC, null, EMagicType.NONE, true);
				SetMagicTypesData(WaponsMagicTypesEquip_Edit_IC, tempWeapon.Elemental, EMagicType.NONE, false);
				WaponsMagicTypesEquip_Edit_IC.UpdateLayout();
				#endregion

				#region Effect/Traits Binding

				foreach (ModifierData modd in tempWeapon.Effects)
				{
					WeaponEffectEquip_Edit_IC.Items.Add(modd.Id);
				}
				foreach (ModifierData modd in tempWeapon.Traits)
				{
					WeaponTraitsEquip_Edit_IC.Items.Add(modd.Id);
				}
				#endregion

				#region Skills Binding

				foreach (Skill skill in tempWeapon.WeaponSkills)
				{
					WeaponSkillsEquip_Edit_IC.Items.Add(skill);
				}

				#endregion

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
			if (int.TryParse(WeapondDamage_Edit_TB.Text, out int inflictval) &&
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
			)
			{

				//before we send the SQL update query we need to update the info in memory.
				int absindex = WeaponName_Edit_CB.SelectedIndex;
				Weapon wepdata = CurrentWeaponsInDatabase[absindex];

				wepdata.bDamage = (bool)WeaponDamage_Edit_CB.IsChecked;
				//wepdata.Inflicting_value = inflictval; // This doesn't exist anymore. was phased out by Str&weak stats
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
											//String.Format("{0} = {1},", "inflicting_value", wepdata.Inflicting_value) + // This doesn't exist anymore. was phased out by Str&weak stats
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
					SetOutputLog( String.Format("Loading/Reading Database [gameplay modifier] failed: {0}", ex.Message));
				}
				finally
				{
					//EditJobsDB_LB.ItemsSource = CurrentJobsInDatabase;
					GameplayModifierName_CB.ItemsSource = CurrentGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}



	}
}
