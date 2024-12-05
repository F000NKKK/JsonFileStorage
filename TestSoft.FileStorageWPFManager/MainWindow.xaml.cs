using System.Windows;
using Newtonsoft.Json;
using TestSoft.FileStorageWPFLibrary.Contracts;
using TestSoft.FileStorageWPFLibrary.Models;
using TestSoft.FileStorageWPFLibrary;

namespace TestSoft.FileStorageWPFManager
{
    public partial class MainWindow : Window
    {
        private readonly FileStorageApiClient _fileStorageApiClient;

        public MainWindow()
        {
            InitializeComponent();
            _fileStorageApiClient = (Application.Current as App)?.FileStorageApiClient ?? throw new InvalidOperationException("API client is not initialized.");

        }

        // Create
        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var jsonData = CreateTextBox.Text;

                // Десериализация в правильный тип JsonObjectDto
                var jsonObject = JsonConvert.DeserializeObject<JsonObjectDto>(jsonData);

                // Проверка на null
                if (jsonObject == null)
                {
                    ShowResponseInNewWindow("Invalid JSON format.");
                    return;
                }

                // Создание объекта в API
                ApiResponse<JsonObjectDto?> response = await _fileStorageApiClient.CreateJsonObject(jsonObject);

                if (response.Success)
                {
                    ShowResponseInNewWindow($"Created successfully!\r\nData: {response.Data}");
                }
                else
                {
                    ShowResponseInNewWindow($"Error: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ShowResponseInNewWindow($"Exception: {ex.Message}");
            }
        }


        // Read
        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = Guid.Parse(ReadIdTextBox.Text);
                ApiResponse<JsonObjectDto?> response = await _fileStorageApiClient.GetJsonObject(id);

                if (response.Success)
                {
                    var resultObject = JsonConvert.SerializeObject(response.Data, Formatting.Indented);
                    ShowResponseInNewWindow(resultObject);
                }
                else
                {
                    ShowResponseInNewWindow($"Error: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ShowResponseInNewWindow($"Exception: {ex.Message}");
            }
        }

        // Update
        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var patchRequest = new JsonPatchRequestDto();

                patchRequest.Operations.Add(
                    new JsonPatchOperationDto()
                    {
                        Op = OperationComboBox.Text,
                        Path = PathTextBox.Text,
                        Value = ValueTextBox.Text
                    });


                if (patchRequest == null || patchRequest.Operations == null || patchRequest.Operations.Count == 0)
                {
                    ShowResponseInNewWindow("Invalid patch request format.");
                    return;
                }

                ApiResponse<JsonObjectDto?> response = await _fileStorageApiClient.ApplyPatch(Guid.Parse(UpdateGuidTextBox.Text), patchRequest);

                if (response.Success)
                {
                    ShowResponseInNewWindow("Updated successfully!");
                }
                else
                {
                    ShowResponseInNewWindow($"Error: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ShowResponseInNewWindow($"Exception: {ex.Message}");
            }
        }

        // Delete
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = Guid.Parse(DeleteIdTextBox.Text);
                ApiResponse<bool> response = await _fileStorageApiClient.DeleteJsonObject(id);

                if (response.Success)
                {
                    ShowResponseInNewWindow("Deleted successfully!");
                }
                else
                {
                    ShowResponseInNewWindow($"Error: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ShowResponseInNewWindow($"Exception: {ex.Message}");
            }
        }

        private void ShowResponseInNewWindow(string responseText)
        {
            var responseWindow = new ResponseWindow();
            responseWindow.ResponseTextBox.Text = responseText;
            responseWindow.Show();
        }


        // Create
        private void CreateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CreateTextBox.Text == "Enter JSON-string to create")
            {
                CreateTextBox.Text = string.Empty;
            }
        }

        private void CreateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CreateTextBox.Text))
            {
                CreateTextBox.Text = "Enter JSON-string to create";
            }
        }

        // Read
        private void ReadIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ReadIdTextBox.Text == "Enter ID to read")
            {
                ReadIdTextBox.Text = string.Empty;
            }
        }

        private void ReadIdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReadIdTextBox.Text))
            {
                ReadIdTextBox.Text = "Enter ID to read";
            }
        }

        // Обработчик для PathTextBox
        private void PathTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PathTextBox.Text == "Enter path")
            {
                PathTextBox.Text = string.Empty;
            }
        }

        private void PathTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                PathTextBox.Text = "Enter path";
            }
        }

        // Обработчик для ValueTextBox
        private void ValueTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ValueTextBox.Text == "Enter JSON-string")
            {
                ValueTextBox.Text = string.Empty;
            }
        }

        private void ValueTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ValueTextBox.Text))
            {
                ValueTextBox.Text = "Enter JSON-string";
            }
        }

        // Обработчик для UpdateGuidTextBox
        private void UpdateGuidTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UpdateGuidTextBox.Text == "Enter GUID to update")
            {
                UpdateGuidTextBox.Text = string.Empty;
            }
        }

        private void UpdateGuidTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UpdateGuidTextBox.Text))
            {
                UpdateGuidTextBox.Text = "Enter GUID to update";
            }
        }

        private void DeleteIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DeleteIdTextBox.Text == "Enter ID to delete")
            {
                DeleteIdTextBox.Text = string.Empty;
            }
        }

        private void DeleteIdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DeleteIdTextBox.Text))
            {
                DeleteIdTextBox.Text = "Enter ID to delete";
            }
        }

    }
}
