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

namespace Forms.DatabaseTool
{
	public partial class MainWindow
	{
		public ObservableCollection<Skill> CurrentSkillsInDatabase { get; set; }

		public ObservableCollection<Tuple<string, String>> SkillsCurrentLinkedModifiers_Add_AllMods { get; set; }
		public ObservableCollection<Tuple<string, String>> SkillsCurrentLinkedModifiers_Edit_AllMods { get; set; }


		public void MainWindow_Skills()
		{
			SkillsCurrentLinkedModifiers_Add_AllMods = new ObservableCollection<Tuple<String, String>>();
			SkillsCurrentLinkedModifiers_Edit_AllMods = new ObservableCollection<Tuple<String, String>>();

			CurrentSkillsInDatabase = new ObservableCollection<Skill>();
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
				SkillWeaponType_Edit_CB.SelectedValue = ((EWeaponType)currentSkill.Weapon_Type);
				SkillAoEWidth_Edit_TB.Text = currentSkill.AOE_w.ToString();     //1.0.0.3v
				SkillAoEHeight_Edit_TB.Text = currentSkill.AOE_h.ToString();    //1.0.0.3v
				SkillAllies_Edit_CB.IsChecked = currentSkill.bAllies;           //1.0.0.3v

				//Show the Function pointer function name
				SkillFuncPTR_Edit_TB.Text = currentSkill.Function_PTR;

				if (currentSkill.Modifier_FK != null && currentSkill.Modifier_FK != "") //ONLY SHOW modifiers if they EXIST
				{
					SkillLinkedModifier_Edit_CB.SelectedIndex =
						(CurrenGameplayModifiersInDatabase.IndexOf(
							CurrenGameplayModifiersInDatabase.Single(x => x.Id == currentSkill.Modifier_FK)
						));

					DisplayGameplayModifiersToIC(SkillAllMods_Edit_IC, (ModifierData)SkillLinkedModifier_Edit_CB.SelectedValue,
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
				skilldata.bPhys = (bool)SkillIsPhysical_Edit_CB.IsChecked;
				skilldata.Damage_Multiplier = damageMulti;
				skilldata.Weapon_Type = SkillWeaponType_Edit_CB.SelectedIndex;
				if (((ModifierData)SkillLinkedModifier_Edit_CB.SelectedValue) != null)
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
						bAllies = (bool)SkillAllies_Add_CB.IsChecked, //1.0.0.2v
						AOE_w = AoE_W_Val, //1.0.0.2v
						AOE_h = AoE_H_Val, //1.0.0.2v

					};
					if (SkillLinkedModifier_Add_CB.SelectedIndex >= 0)
					{
						skill.Modifier_FK = ((ModifierData)SkillLinkedModifier_Add_CB.SelectedValue).Id;
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


	}
}
