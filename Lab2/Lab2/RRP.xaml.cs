using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core.Primitives;

namespace Lab2;

public partial class RRP : ContentPage
{
    public RRP()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string fileName = "rrc.mp4";

        string targetFile = Path.Combine(FileSystem.Current.CacheDirectory, fileName);

        if (!File.Exists(targetFile))
        {
            using var inputStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
            using var outputStream = File.Create(targetFile);
            await inputStream.CopyToAsync(outputStream);
        }

        VideoPlayer.Source = MediaSource.FromFile(targetFile);

        VideoPlayer.Volume = 1.0;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        VideoPlayer.Stop();
        VideoPlayer.Source = null;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}