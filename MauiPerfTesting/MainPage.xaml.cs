using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using Dapper;

namespace MauiPerfTesting
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        Task _RunningTask;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            _RunningTask = RunPerfTest();
        }

        private async Task RunPerfTest()
        {
            await Task.Yield();

            var db = Path.Combine(FileSystem.AppDataDirectory, "databases", "PerfTest.sqlite");

            Directory.CreateDirectory(Path.GetDirectoryName(db)!);

            try
            {
                File.Delete(db);
            }
            catch { }

            try
            {

                int current = 0;

                var task = Task.Factory.StartNew(() =>
                {
                    using var conn = new SqliteConnection($"Data Source={db};");

                    conn.Open();

                    //conn.Execute("PRAGMA key = 'thisisthetestpassword';");
                    //conn.Execute("PRAGMA cipher = aes256cbc;");

                    conn.Execute("CREATE TABLE TestTable (ID INT, NAME TEXT, AGE INTEGER, ADDRESS TEXT) STRICT");

                    var obj = new { id = 0, name = "John Smith", age = 0, address = "123 Fake Street" };


                    for (int i = 1; i <= 1_000_000; i++)
                    {
                        conn.Execute("INSERT INTO TestTable VALUES (@id, @name, @age, @address)", obj with { id = i, age = i % 84 });

                        if (i % 5 == 0)
                        {
                            Interlocked.Exchange(ref current, i);
                        }
                    }

                    conn.Close();
                });

                while (!task.IsCompleted)
                {
                    var last = current;

                    await Task.Delay(TimeSpan.FromSeconds(1));

                    var c = current;
                    var tps = c - last;

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CounterBtn.Text = $"Current {c}, TPS: {tps}";
                    });

                }
            }
            catch(Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CounterBtn.Text = ex.Message;
                });
            }

        }
    }

}
