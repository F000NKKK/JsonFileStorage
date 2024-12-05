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

        /// <summary>
        /// Initializes the MainWindow and sets up the API client.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _fileStorageApiClient = (Application.Current as App)?.FileStorageApiClient ?? throw new InvalidOperationException("API client is not initialized.");
        }

        /// <summary>
        /// Handles the Create button click event. It deserializes JSON data, sends a request to the API to create a new object, and shows the response.
        /// </summary>
        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var jsonData = CreateTextBox.Text;

                // Deserialize into the appropriate type JsonObjectDto
                var jsonObject = JsonConvert.DeserializeObject<JsonObjectDto>(jsonData);

                // Check for null
                if (jsonObject == null)
                {
                    ShowResponseInNewWindow("Invalid JSON format.");
                    return;
                }

                // Create object in the API
                ApiResponse<JsonObjectDto?> response = await _fileStorageApiClient.CreateJsonObject(jsonObject);

                if (response.IsSuccess)
                {
                    var result = JsonConvert.SerializeObject(response, Formatting.Indented);
                    ShowResponseInNewWindow($"Created successfully!\r\nData: {result}");
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

        /// <summary>
        /// Handles the Read button click event. It fetches the object by its ID from the API and shows the response.
        /// </summary>
        private async void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = Guid.Parse(ReadIdTextBox.Text);
                ApiResponse<JsonObjectDto?> response = await _fileStorageApiClient.GetJsonObject(id);

                if (response.IsSuccess)
                {
                    var resultObject = JsonConvert.SerializeObject(response.Data, Formatting.Indented);
                    ShowResponseInNewWindow("Read successfully\r\n" + resultObject);
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

        /// <summary>
        /// Handles the Update button click event. It sends a patch request to the API to update an object with the given patch details.
        /// </summary>
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

                if (response.IsSuccess)
                {
                    var resultObject = JsonConvert.SerializeObject(response.Data, Formatting.Indented);
                    ShowResponseInNewWindow("Updated successfully!\r\n" + resultObject);
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

        /// <summary>
        /// Handles the Delete button click event. It deletes an object from the API by its ID and shows the response.
        /// </summary>
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = Guid.Parse(DeleteIdTextBox.Text);
                ApiResponse<bool> response = await _fileStorageApiClient.DeleteJsonObject(id);

                if (response.IsSuccess)
                {
                    var resultObject = JsonConvert.SerializeObject(response.Data, Formatting.Indented);
                    ShowResponseInNewWindow("Deleted successfully!\r\n" + resultObject);
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

        /// <summary>
        /// Displays the response in a new window.
        /// </summary>
        private void ShowResponseInNewWindow(string responseText)
        {
            var responseWindow = new ResponseWindow();
            responseWindow.ResponseTextBox.Text = responseText;
            responseWindow.Show();
        }

        /// <summary>
        /// Clears the default text when the CreateTextBox gains focus.
        /// </summary>
        private void CreateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CreateTextBox.Text == "Enter JSON-string to create")
            {
                CreateTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Restores the default text when the CreateTextBox loses focus and is empty.
        /// </summary>
        private void CreateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CreateTextBox.Text))
            {
                CreateTextBox.Text = "Enter JSON-string to create";
            }
        }

        /// <summary>
        /// Clears the default text when the ReadIdTextBox gains focus.
        /// </summary>
        private void ReadIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ReadIdTextBox.Text == "Enter ID to read")
            {
                ReadIdTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Restores the default text when the ReadIdTextBox loses focus and is empty.
        /// </summary>
        private void ReadIdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReadIdTextBox.Text))
            {
                ReadIdTextBox.Text = "Enter ID to read";
            }
        }

        /// <summary>
        /// Clears the default text when the PathTextBox gains focus.
        /// </summary>
        private void PathTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PathTextBox.Text == "Enter path")
            {
                PathTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Restores the default text when the PathTextBox loses focus and is empty.
        /// </summary>
        private void PathTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                PathTextBox.Text = "Enter path";
            }
        }

        /// <summary>
        /// Clears the default text when the ValueTextBox gains focus.
        /// </summary>
        private void ValueTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ValueTextBox.Text == "Enter JSON-string")
            {
                ValueTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Restores the default text when the ValueTextBox loses focus and is empty.
        /// </summary>
        private void ValueTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ValueTextBox.Text))
            {
                ValueTextBox.Text = "Enter JSON-string";
            }
        }

        /// <summary>
        /// Clears the default text when the UpdateGuidTextBox gains focus.
        /// </summary>
        private void UpdateGuidTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UpdateGuidTextBox.Text == "Enter GUID to update")
            {
                UpdateGuidTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Restores the default text when the UpdateGuidTextBox loses focus and is empty.
        /// </summary>
        private void UpdateGuidTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UpdateGuidTextBox.Text))
            {
                UpdateGuidTextBox.Text = "Enter GUID to update";
            }
        }

        /// <summary>
        /// Clears the default text when the DeleteIdTextBox gains focus.
        /// </summary>
        private void DeleteIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DeleteIdTextBox.Text == "Enter ID to delete")
            {
                DeleteIdTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Restores the default text when the DeleteIdTextBox loses focus and is empty.
        /// </summary>
        private void DeleteIdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DeleteIdTextBox.Text))
            {
                DeleteIdTextBox.Text = "Enter ID to delete";
            }
        }
    }
}
