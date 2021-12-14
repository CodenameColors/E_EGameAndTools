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
						ModifierData moddata = CurrenGameplayModifiersInDatabase.Single(x => x.Id == mod_key.Modifier_ID);
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
							CurrenGameplayModifiersInDatabase_Effects[WeaponTraits_Edit_CB.SelectedIndex]);
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
					WeaponRarity_Add_CB.SelectedIndex > 0
					)
			{
				String masterfile = (SQLDatabasePath);
				_sqlite_conn = new SQLiteConnection(masterfile);
				int rowid = 0;
				try
				{
					String Createsql = "";

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
					int i = 0;
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
					//GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
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
				 int.TryParse(WeaponWeight_Edit_TB.Text, out int weightval))
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
					GameplayModifierName_CB.ItemsSource = CurrenGameplayModifiersInDatabase;
					//GameplayModifierName_CB.SelectedIndex = absindex;
				}
			}
		}



	}
}
