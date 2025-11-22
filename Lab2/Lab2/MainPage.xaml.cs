using CommunityToolkit.Maui.Storage;
using Lab2.Services;
using Lab2.Services.SearchStrategy;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Lab2
{
    public partial class MainPage : ContentPage
    {
        private string selectedXmlPath;

        private readonly TransformationService transformationService;
        private readonly Dictionary<string, IXmlSearchStrategy> searchStrategies;

        public MainPage()
        {
            InitializeComponent();

            transformationService = new TransformationService();

            searchStrategies = new Dictionary<string, IXmlSearchStrategy>
            {
                { "LINQ to XML", new LinqSearchStrategy() },
                { "DOM API", new DomSearchStrategy() },
                { "SAX API", new SaxSearchStrategy() }
            };

            StrategyPicker.ItemsSource = searchStrategies.Keys.ToList();
            StrategyPicker.SelectedIndex = 0;
        }

        public async void OnExitClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert(
                "Вихід",
                "Чи дійсно ви хочете завершити роботу з програмою?",
                "Так",
                "Ні");

            if (answer)
            {
                Application.Current.Quit();
            }
        }

        private async void OnSelectXmlClicked(object sender, EventArgs e)
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".xml"} },
                    { DevicePlatform.macOS, new[] { "xml" } }
                });

            var options = new PickOptions
            {
                PickerTitle = "Оберіть XML-файл",
                FileTypes = customFileType,
            };

            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    selectedXmlPath = result.FullPath;
                    SelectedXmlFileLabel.Text = $"Обрано: {result.FileName}";

                    PopulateFilters(selectedXmlPath);

                    SearchButton.IsEnabled = true;
                    TransformButton.IsEnabled = true;
                    ClearButton.IsEnabled = true;
                    StatusLabel.Text = "Файл завантажено";
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Помилка виводу файлу: {ex.Message}";
            }
        }

        private void OnSearchClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedXmlPath))
            {
                DisplayAlert("Помилка", "Спочатку оберіть XML-файл", "ОК");
                return;
            }
            if (StrategyPicker.SelectedItem == null)
            {
                DisplayAlert("Помилка", "Оберіть стратегію пошуку", "OK");
                return;
            }

            try
            {
                string strategyName = StrategyPicker.SelectedItem.ToString();
                IXmlSearchStrategy strategy = searchStrategies[strategyName];

                string author = AuthorPicker.SelectedIndex > 0 ? AuthorPicker.SelectedItem.ToString() : null;
                string dept = DepartmentPicker.SelectedIndex > 0 ? DepartmentPicker.SelectedItem.ToString() : null;
                string cat = CategoryPicker.SelectedIndex > 0 ? CategoryPicker.SelectedItem.ToString() : null;

                SearchCriteria criteria = new SearchCriteria
                {
                    Title = TitleEntry.Text,
                    AuthorName = author,
                    Department = dept,
                    Category = cat
                };

                List<FacultyEntry> results = strategy.Search(selectedXmlPath, criteria);

                ResultsListView.ItemsSource = results;
                StatusLabel.Text = $"Знайдено {results.Count} записів";
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Помилка пошуку: {ex.Message}";
            }
        }

        private async void OnTransformClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedXmlPath))
            {
                await DisplayAlert("Помилка", "Спочатку оберіть XML-файл", "OK");
                return;
            }

            try
            {
                using var memoryStream = new MemoryStream();
                transformationService.TransformToStream(selectedXmlPath, memoryStream);
                memoryStream.Position = 0;

                var fileSaverResult = await FileSaver.Default.SaveAsync("report.html", memoryStream, CancellationToken.None);

                if (fileSaverResult.IsSuccessful)
                {
                    await DisplayAlert("Успіх", $"Файл збережено!\nШлях: {fileSaverResult.FilePath}", "OK");

                    try
                    {
                        await Launcher.Default.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(fileSaverResult.FilePath)
                        });
                    }
                    catch
                    {
                    }
                }
                else
                {
                    StatusLabel.Text = "Збереження скасовано.";
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Помилка трансформації: {ex.Message}";
                await DisplayAlert("Помилка", $"Не вдалося зберегти файл: {ex.Message}", "OK");
            }
        }

        public void OnClearClicked(object sender, EventArgs e)
        {
            TitleEntry.Text = string.Empty;

            if (AuthorPicker.Items.Count > 0) AuthorPicker.SelectedIndex = 0;
            if (DepartmentPicker.Items.Count > 0) DepartmentPicker.SelectedIndex = 0;
            if (CategoryPicker.Items.Count > 0) CategoryPicker.SelectedIndex = 0;

            ResultsListView.ItemsSource = null;
            StatusLabel.Text = "Поля очищено. Параметри скинуто.";
        }

        private void PopulateFilters(string xmlPath)
        {
            try
            {
                XDocument doc = XDocument.Load(xmlPath);

                var authors = doc.Descendants("Author")
                                 .Select(a => a.Element("Name")?.Value)
                                 .Where(name => !string.IsNullOrEmpty(name))
                                 .Distinct()
                                 .OrderBy(x => x)
                                 .ToList();

                authors.Insert(0, "- Всі автори -");
                AuthorPicker.ItemsSource = authors;
                AuthorPicker.SelectedIndex = 0;

                var departments = doc.Descendants("Entry")
                                     .Select(e => e.Attribute("department")?.Value)
                                     .Where(d => !string.IsNullOrEmpty(d))
                                     .Distinct()
                                     .OrderBy(x => x)
                                     .ToList();

                departments.Insert(0, "- Всі кафедри -");
                DepartmentPicker.ItemsSource = departments;
                DepartmentPicker.SelectedIndex = 0;

                var categories = doc.Descendants("Category")
                                    .Select(c => c.Value)
                                    .Where(c => !string.IsNullOrEmpty(c))
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToList();

                categories.Insert(0, "- Всі категорії -");
                CategoryPicker.ItemsSource = categories;
                CategoryPicker.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка аналізу", "Не вдалося завантажити фільтри: " + ex.Message, "OK");
            }
        }
        public async void OnResultTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is FacultyEntry entry)
            {
                string reviewsText = "";

                if (entry.Reviews != null && entry.Reviews.Count > 0)
                {
                    reviewsText = "\n--- ВІДГУКИ ---\n";
                    foreach (var r in entry.Reviews)
                    {
                        reviewsText += $"{r.User} (Оцінка: {r.Score}/5)\n \"{r.Comment}\"\n\n";
                    }
                }
                else
                {
                    reviewsText = "\n\n(Відгуків поки немає)";
                }
                string details = $"ID: {entry.Id}\n" +
                                 $"Тип: {entry.Type}\n" +
                                 $"Кафедра: {entry.Department ?? "Не вказано"}\n\n" +
                                 $"Автори: {entry.AuthorsDisplay}\n\n" +
                                 $"АНОТАЦІЯ: {entry.Annotation}" +
                                 reviewsText;

                await DisplayAlert(entry.Title, details, "Закрити");
            }

        ((ListView)sender).SelectedItem = null;
        }
        public async void OnRRClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushModalAsync(new RRP());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка запуску відео", ex.Message, "OK");
            }
        }
    }
}
