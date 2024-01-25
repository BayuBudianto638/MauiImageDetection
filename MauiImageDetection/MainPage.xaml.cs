using MauiImageDetection.Services;

namespace MauiImageDetection
{
    public partial class MainPage : ContentPage
    {
        private readonly IDetectService _detectService;

        public MainPage()
        {
            InitializeComponent();
            _detectService = (IDetectService)(new DetectService());
        }

        public static byte[] ConvertStreamToBytes(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // Save the file into local storage
                    string localFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);

                    // Flush and close the streams
                    await localFileStream.FlushAsync();
                    localFileStream.Close();

                    using Stream imageStream = File.OpenRead(localFilePath);


                    byte[] imageBytes = ConvertStreamToBytes(imageStream);


                    var detectResult = this._detectService.Detect(imageBytes);
                    if (detectResult == null)
                        return;

                    if (detectResult.Boxes.Count > 0)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Human Face Detected and no of face detected " + detectResult.Boxes.Count, "Ok");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "No Human Face Detected", "Ok");
                    }
                }
            }
        }
    }
}
