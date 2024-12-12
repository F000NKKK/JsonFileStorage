
# FileStorageLibrary

`TestSoft.FileStorageLibrary` is a library designed for managing and storing file data in a file storage system. It provides a set of services to handle files, including reading, writing, updating, and deleting files, as well as performing compression to optimize storage.

## Features

- **File Management**: Perform CRUD (Create, Read, Update, Delete) operations on files stored in the file system.
- **Compression Support**: Automatically compress files exceeding a certain size threshold to save storage space.
- **Asynchronous API**: All operations are asynchronous, ensuring non-blocking execution for better performance.
- **Flexible Data Representation**: Store arbitrary data associated with files using key-value pairs (represented as `Dictionary<string, object>`).
- **Error Handling**: The library includes robust error handling to ensure smooth file operations, including error messages for common file system issues.

## Key Components

### `FileDataDto`

Represents the data for a file stored in the system. It includes a unique file identifier (`Id`) and a dictionary to store file-specific data.

```csharp
public class FileDataDto
{
    public Guid Id { get; set; }
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}
```

### `FileSystem`

A service for interacting with the file system, handling operations like reading, writing, and deleting files. It also supports both compressed and uncompressed file formats.

#### Key Methods:
- `Exists(string path)`: Checks if a file exists at the specified path.
- `ReadCompressedFileAsync(string path)`: Reads a compressed file asynchronously.
- `WriteCompressedFileAsync(string path, byte[] data)`: Writes data to a compressed file.
- `ReadFileAsync(string path)`: Reads a regular file asynchronously.
- `WriteFileAsync(string path, byte[] data)`: Writes data to a regular file.
- `Delete(string path)`: Deletes a file at the specified path.
- `GetFilesAsync(string path, string searchPattern)`: Retrieves files from a directory asynchronously.

### `FileStorageService`

The core service responsible for managing file storage, including adding, updating, retrieving, and deleting files. It also handles file compression based on size and supports advanced querying and manipulation of file data.

#### Key Methods:
- `GetAsync(Guid id)`: Retrieves a file by its unique identifier, first checking for compressed versions.
- `AddAsync(FileDataDto data)`: Adds a new file, either compressed or uncompressed depending on the file size.
- `UpdateAsync(FileDataDto data)`: Updates an existing file, ensuring that the correct format (compressed or uncompressed) is used based on file size.
- `Delete(Guid id)`: Deletes a file by its unique identifier.
- `GetAllAsync()`: Retrieves all files in the storage asynchronously.
- `SearchAsync(Func<FileDataDto, bool> predicate)`: Searches files using a predicate to filter results.
- `UpdateFieldAsync(Guid id, Action<FileDataDto> updateAction)`: Updates specific fields of a file.
- `DeleteByFilterAsync(Func<FileDataDto, bool> predicate)`: Deletes files that match a specified predicate.
- `CountAsync()`: Counts the number of files in the storage.

## Installation

To install the package, use the following NuGet command:

```bash
dotnet add package TestSoft.FileStorageLibrary
```

Alternatively, you can download the compiled DLL from the releases section of the repository.

## Usage Example

Here is an example of how to use `FileStorageService` to add, retrieve, and update a file:

### Add a File:
```csharp
var fileStorageService = new FileStorageService("C:\FileStorage", new FileSystem());
var fileData = new FileDataDto
{
    Id = Guid.NewGuid(),
    Data = new Dictionary<string, object>
    {
        { "Name", "Test File" },
        { "Size", 1024 }
    }
};

await fileStorageService.AddAsync(fileData);
```

### Get a File:
```csharp
var file = await fileStorageService.GetAsync(fileData.Id);
Console.WriteLine($"File Name: {file?.Data["Name"]}");
```

### Update a File:
```csharp
fileData.Data["Size"] = 2048;
await fileStorageService.UpdateAsync(fileData);
```

### Delete a File:
```csharp
bool deleted = fileStorageService.Delete(fileData.Id);
Console.WriteLine($"File deleted: {deleted}");
```

## Contributing

Contributions are welcome! To contribute, please fork the repository, make your changes, and submit a pull request. Be sure to follow the coding guidelines and write tests for any new features.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For any questions or issues, please open an issue in the repository or contact the project maintainers.
