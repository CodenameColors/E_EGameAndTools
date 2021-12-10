using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JobStanceAnalysis
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		public ObservableCollection<Job> Jobs { get; set; }
		public Dictionary<String, Job> Jobs_Dict { get; set; }
		public ObservableCollection<ComparsionRecord> JobCombosRecords_2 { get; set; }
		public ObservableCollection<ComparsionRecord> JobCombosRecords_3 { get; set; }
		public ObservableCollection<ComparsionRecord> JobCombosRecords_4 { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			Jobs = new ObservableCollection<Job>();
			Jobs_Dict = new Dictionary<string, Job>();
			JobCombosRecords_2 = new ObservableCollection<ComparsionRecord>();
			JobCombosRecords_3 = new ObservableCollection<ComparsionRecord>();
			JobCombosRecords_4 = new ObservableCollection<ComparsionRecord>();

			//Create the data output zone
			if (!Directory.Exists(String.Format("{0}/{1}", Directory.GetCurrentDirectory(), "Output")))
			{
				Directory.CreateDirectory(String.Format("{0}/{1}", Directory.GetCurrentDirectory(), "Output"));
			}

		}

		private void SetDefaultState(object sender, RoutedEventArgs e)
		{
			Job_LB.ItemsSource = null;

			Jobs.Add(new Job() {Name = EJob.Hero, Stances = new EStanceType[] {EStanceType.Focus, EStanceType.Reading, EStanceType.NONE}});
			Jobs_Dict.Add(Jobs.Last().Name.ToString(), Jobs.Last());

			Jobs.Add(new Job() {Name = EJob.Knight, Stances = new EStanceType[] {EStanceType.Immovable, EStanceType.Praying, EStanceType.NONE } });
			Jobs_Dict.Add(Jobs.Last().Name.ToString(), Jobs.Last());

			Jobs.Add(new Job() {Name = EJob.Sniper, Stances = new EStanceType[] {EStanceType.PewPew, EStanceType.Focus, EStanceType.NONE } });
			Jobs_Dict.Add(Jobs.Last().Name.ToString(), Jobs.Last());

			Jobs.Add(new Job() {Name = EJob.Doctor, Stances = new EStanceType[] {EStanceType.Praying, EStanceType.Singing, EStanceType.NONE } });
			Jobs_Dict.Add(Jobs.Last().Name.ToString(), Jobs.Last());


			Jobs.Add(new Job() {Name = EJob.Necromancer, Stances = new EStanceType[] {EStanceType.Reading, EStanceType.Singing, EStanceType.NONE } });
			Jobs_Dict.Add(Jobs.Last().Name.ToString(), Jobs.Last());

			Jobs.Add(new Job() {Name = EJob.Bandit, Stances = new EStanceType[] {EStanceType.Immovable, EStanceType.Feather, EStanceType.NONE } });
			Jobs_Dict.Add(Jobs.Last().Name.ToString(), Jobs.Last());

			Jobs.Add(new Job() {Name = EJob.LuckyStar, Stances = new EStanceType[] {EStanceType.Bloody, EStanceType.Feather, EStanceType.NONE } });
			Jobs_Dict.Add(Jobs.Last().Name.ToString(), Jobs.Last());

			Jobs.Add(new Job() {Name = EJob.Gambler, Stances = new EStanceType[] {EStanceType.Bloody, EStanceType.PewPew, EStanceType.NONE } });
			Jobs_Dict.Add(Jobs.Last().Name.ToString(), Jobs.Last());
			Job_LB.ItemsSource = Jobs;
		}


		private void AddNewJob(object sender, RoutedEventArgs e)
		{
			Job_LB.ItemsSource = null;

			Job tempJob = new Job()
			{
				Name = (EJob) JobName_CB.SelectedIndex
			};
			tempJob.Stances[0] = (EStanceType) StanceLeft_CB.SelectedIndex;
			tempJob.Stances[1] = (EStanceType) StanceMiddle_CB.SelectedIndex;
			tempJob.Stances[2] = (EStanceType) StanceRight_CB.SelectedIndex;


			StanceLeft_CB.SelectedIndex = 0;
			StanceMiddle_CB.SelectedIndex = 0;
			StanceRight_CB.SelectedIndex = 0;
			JobName_CB.SelectedIndex = 0;

			Job_LB.ItemsSource = Jobs;
			foreach (Job j in Jobs)
			{
				if (j.Name == tempJob.Name)
					return;
			}

			Jobs.Add(tempJob);
			Jobs_Dict.Add(tempJob.Name.ToString(), tempJob);
		}

		static IEnumerable<IEnumerable<T>>
			GetKCombsWithRept<T>(IEnumerable<T> list, int length) where T : IComparable
		{
			if (length == 1) return list.Select(t => new T[] {t});
			return GetKCombsWithRept(list, length - 1)
				.SelectMany(t => list.Where(o => o.CompareTo(t.Last()) >= 0),
					(t1, t2) => t1.Concat(new T[] {t2}));
		}

		static IEnumerable<IEnumerable<T>>
			GetPermutations<T>(IEnumerable<T> list, int length)
		{
			if (length == 1) return list.Select(t => new T[] {t});
			return GetPermutations(list, length - 1)
				.SelectMany(t => list.Where(o => !t.Contains(o)),
					(t1, t2) => t1.Concat(new T[] {t2}));
		}

		static IEnumerable<IEnumerable<T>>
			GetCombinations<T>(IEnumerable<T> list, int length)
		{
			if (length == 1) return list.Select(t => new T[] {t});

			return GetCombinations(list, length - 1)
				.SelectMany(t => list, (t1, t2) => t1.Concat(new T[] {t2}));
		}

		private void Testing_Button_Press(object sender, RoutedEventArgs e)
		{
			try
			{
				FillMatch2Data();
			}
			catch
			{
			}

			FillMatch3Data();
			match4Data();

		}


		private void FillMatch2Data()
		{
			Match2_LB.ItemsSource = null;
			JobCombosRecords_2.Clear();
			IEnumerable<IEnumerable<int>> result =
				GetKCombsWithRept(Enumerable.Range(1, 8), 2);

			int num = 1;
			foreach (IEnumerable<int> i in result)
			{
				ComparsionRecord tempRecord = new ComparsionRecord();
				Job tempJob = new Job();
				Console.Write(num++ + " : {");
				foreach (int ii in i)
				{
					Console.Write(((EJob) ii).ToString() + ",");
					tempJob = new Job();
					tempJob.Name = (EJob) ii;
					tempRecord.JobCombos.Add(tempJob);
				}

				Console.Write("}");
				Console.WriteLine("");

				JobCombosRecords_2.Add(tempRecord);
			}

			//Console.WriteLine(result.ToString());
			//Fill the data.
			int MatchCount = 0;
			foreach (ComparsionRecord cmpr in JobCombosRecords_2)
			{
				for (int i = 0; i < cmpr.JobCombos.Count; i++)
				{
					cmpr.JobCombos[i].Stances[0] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[0];
					cmpr.JobCombos[i].Stances[1] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[1];
					cmpr.JobCombos[i].Stances[2] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[2];
				}

				cmpr.FIndMatches_Generic_test(2);
				if (cmpr.bIsMatch)
					MatchCount++;
			}

			Match2_LB.ItemsSource = JobCombosRecords_2;
			NumberOfMatches_2_TB.Text = MatchCount.ToString();
			NumberOfCombos_2_TB.Text = JobCombosRecords_2.Count.ToString();
			PercentOfMatches_2_TB.Text = ((float) MatchCount / (float) JobCombosRecords_2.Count).ToString();
		}

		private void FillMatch3Data()
		{
			Match3_LB.ItemsSource = null;
			JobCombosRecords_3.Clear();
			IEnumerable<IEnumerable<int>> result =
				GetKCombsWithRept(Enumerable.Range(1, 8), 3);

			int num = 1;
			foreach (IEnumerable<int> i in result)
			{
				ComparsionRecord tempRecord = new ComparsionRecord();
				Job tempJob = new Job();
				Console.Write(num++ + " : {");
				foreach (int ii in i)
				{
					Console.Write(((EJob) ii).ToString() + ",");
					tempJob = new Job();
					tempJob.Name = (EJob) ii;
					tempRecord.JobCombos.Add(tempJob);
				}

				Console.Write("}");
				Console.WriteLine("");

				JobCombosRecords_3.Add(tempRecord);
			}

			//check for matches
			int MatchCount = 0;
			foreach (ComparsionRecord cmpr in JobCombosRecords_3)
			{
				for (int i = 0; i < cmpr.JobCombos.Count; i++)
				{
					cmpr.JobCombos[i].Stances[0] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[0];
					cmpr.JobCombos[i].Stances[1] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[1];
					cmpr.JobCombos[i].Stances[2] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[2];
				}

				cmpr.FIndMatches_Generic_test(3);
				if (cmpr.bIsMatch)
					MatchCount++;
			}

			Match3_LB.ItemsSource = JobCombosRecords_3;
			NumberOfMatches_3_TB.Text = MatchCount.ToString();
			NumberOfCombos_3_TB.Text = JobCombosRecords_3.Count.ToString();
			PercentOfMatches_3_TB.Text = ((float) MatchCount / (float) JobCombosRecords_3.Count).ToString();
		}

		private void match4Data()
		{
			Match4_LB.ItemsSource = null;
			JobCombosRecords_4.Clear();
			IEnumerable<IEnumerable<int>> result =
				GetKCombsWithRept(Enumerable.Range(1, 8), 4);

			int num = 1;
			foreach (IEnumerable<int> i in result)
			{
				ComparsionRecord tempRecord = new ComparsionRecord();
				Job tempJob = new Job();
				Console.Write(num++ + " : {");
				foreach (int ii in i)
				{
					Console.Write(((EJob) ii).ToString() + ",");
					tempJob = new Job();
					tempJob.Name = (EJob) ii;
					tempRecord.JobCombos.Add(tempJob);
				}

				Console.Write("}");
				Console.WriteLine("");

				JobCombosRecords_4.Add(tempRecord);
			}

			//check for matches
			int MatchCount = 0;
			foreach (ComparsionRecord cmpr in JobCombosRecords_4)
			{
				for (int i = 0; i < cmpr.JobCombos.Count; i++)
				{
					cmpr.JobCombos[i].Stances[0] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[0];
					cmpr.JobCombos[i].Stances[1] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[1];
					cmpr.JobCombos[i].Stances[2] = Jobs_Dict[cmpr.JobCombos[i].Name.ToString()].Stances[2];
				}

				cmpr.FIndMatches_Generic_test(4);
				if (cmpr.bIsMatch)
					MatchCount++;
			}

			Match4_LB.ItemsSource = JobCombosRecords_4;
			NumberOfMatches_4_TB.Text = MatchCount.ToString();
			NumberOfCombos_4_TB.Text = JobCombosRecords_4.Count.ToString();
			PercentOfMatches_4_TB.Text = ((float) MatchCount / (float) JobCombosRecords_4.Count).ToString();
		}

		private void ExportDataToCSV_BTN_Click(object sender, RoutedEventArgs e)
		{


			using (StreamWriter writer = new StreamWriter(
				String.Format("{0}/{1}/{2}", Directory.GetCurrentDirectory(), "Output", "JobMatch_2.csv")))
			{
				foreach (ComparsionRecord cpr in JobCombosRecords_2)
				{
					{
						if (!cpr.bIsMatch) continue;
						String line = string.Format("{0},{1},{2},{3}", cpr.JobCombos[0].Name, cpr.JobCombos[1].Name, "NONE",
							"NONE");
						writer.WriteLine(line);
						writer.Flush();
					}
				}
			}

			using (StreamWriter writer = new StreamWriter(
				String.Format("{0}/{1}/{2}", Directory.GetCurrentDirectory(), "Output", "JobMatch_3.csv")))
			{
				foreach (ComparsionRecord cpr in JobCombosRecords_3)
				{
					if (!cpr.bIsMatch) continue;
					String line = string.Format("{0},{1},{2},{3}", cpr.JobCombos[0].Name, cpr.JobCombos[1].Name,
						cpr.JobCombos[2].Name, "NONE");
					writer.WriteLine(line);
					writer.Flush();
				}
			}



			using (StreamWriter writer = new StreamWriter(
				String.Format("{0}/{1}/{2}", Directory.GetCurrentDirectory(), "Output", "JobMatch_4.csv")))
			{
				foreach (ComparsionRecord cpr in JobCombosRecords_4)
				{
					if (!cpr.bIsMatch) continue;
					String line = string.Format("{0},{1},{2},{3}", cpr.JobCombos[0].Name, cpr.JobCombos[1].Name,
						cpr.JobCombos[2].Name, cpr.JobCombos[3].Name);
					writer.WriteLine(line);
					writer.Flush();
				}
			}
		}



	}
}
