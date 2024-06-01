---
layout: default
---

## About
A lightweight and ready-made implementation of file persistence for storing your app data or files. 

Twileloop.FileStorage is a package that ships with a built-in XML encoder and decoder and GZip deflate compressor and decompressor. This allows you to effortlessly read and write your app-generated files, config, or your data files with ease.

## License
> Twileloop.FileStorage - is licensed under the MIT License. See the LICENSE file for more details.

#### This library is absolutely free. If it gives you a smile, A small coffee would be a great way to support my work. Thank you for considering it!
[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/sangeethnanda)

## Usage

## 2. Install Package

```powershell
dotnet add package Twileloop.FileStorage
```

### Supported Features

| Feature     | Status 
| ---      | ---
| XML Encoding/Decoding | ✅
| GZip Deflate Compression/Decompression | ✅
| Read | ✅
| Write | ✅
| Encrypted Writes and Reads | ✅
| Custom Encryption Provider | ✅
| Get File Details | ✅
| Meta data embedding | ✅
| One or more embedded Files | ✅
| Asynchronous Operations | ❌


✅ - Available &nbsp;&nbsp;&nbsp; 
🚧 - Work In Progress &nbsp;&nbsp;&nbsp; 
❌ - Not Available

### PLEASE NOTE
✅ - `Twileloop.FileStorage` depends on `YAXLib` to enable XML encoding and decoding of all types. This allows more convenience than the native `System.Xml`. A common example is storing `Dictionary<K, V>`. You can use any types with confidence

## 1. Declare your app model (POCO)
```csharp
 public class Student
 {
     public int Id { get; set; }
     public string FirstName { get; set; }
     public string LastName { get; set; }
     public DateTime DateOfBirth { get; set; }
 }
```

## 2. Setup some data
```csharp
//Step 2: Here's the data
var students = new List<Student>() {
    new Student
    {
        Id = 1,
        FirstName= "Sangeeth",
        LastName = "Nandakumar",
        DateOfBirth= DateTime.Now,
    }
};

//Step 3: Initialize filestorage
IFileStorage<List<Student>> fileStorage = new FileStorage<List<Student>>();
```

## 3. Simple Write

> The latest version of `Twileloop.FileSystem` writes data in a custom data format called `PDR.v1` (Portable Data Recording)

```csharp
if (fileStorage.WriteFile(students, "MyAppData.cab"))
{
    Console.WriteLine("File written successfully");
}
else
{
    Console.WriteLine("File writing failed");
}
```

## 4. Simple Read
```csharp
if (fileStorage.TryReadFile("MyAppData.cab", out FileReadResult reader))
{
    var myData = reader.ParseContents<List<Student>>();
    Console.WriteLine("File reading success");
}
else
{
    Console.WriteLine("File reading failed");
}
```

## 5. Encrypted Writes
Performing encryption and decryption requires you to configure a custom encryption provider.
You can use any library you choose to encrypt and decrypt your files.

> In this example, We're using `Twileloop.Security` nuget package to perform AES encryption. The same package supports RSA also. Or you can write your own data transform standards

#### To start with, Create a custom encryption provider. Design this class in anyway you need. Ensure it should implement `IEncryptionProvider`

```csharp
 public class MyCustomSecurityProvider : IEncryptionProvider
 {
        private readonly string key;
        private readonly string iv;

        public string GetEncryptionProvider() => "Twileloop.Security";
        public string GetEncryptionAlgorithm() => "AES";
        public Credential SetKeyAndInitialVector() => new Credential
        {
            Key = key,
            IV = iv
        };

        public MyCustomSecurityProvider(string key, string iv)
        {
            this.key = key;
            this.iv = iv;
        }

        public byte[] Encrypt(byte[] rawData)
        {
            //Do your encryption logic
            return AESAlgorithm.EncryptBytes(rawData, key, iv);
        }

        public byte[] Decrypt(byte[] encrypteData)
        {
            //Do your decryption logic
            return AESAlgorithm.DecryptBytes(encrypteData, key, iv);
        }
 }
```

## Once done, We'll use this provider for encrypted writes and reads
```csharp
//Initialize provider
var securityProvider = new MyCustomSecurityProvider("1234", "1234567890123456");

//Write data, by passing this provider
if (fileStorage.WriteFile(students, "MyAppData_Encrypted.cab", securityProvider))
{
    Console.WriteLine("AES encrypted file written successfully");
}
else
{
    Console.WriteLine("AES file writing failed");
}


//Read data, by passing this provider
if (fileStorage.TryReadFile("MyAppData_Encrypted.cab", out FileReadResult reader, securityProvider))
{
    var parsedData = reader.ParseContents<List<Student>>();
    Console.WriteLine("AES encrypted file reading success");
}
else
{
    Console.WriteLine("AES encrypted file reading failed");
}
```

## 6. Embed Some Custom Meta Data
Want to place some key value meta along the file? Will be super usefull to store metadata
ou can find them in FileHeader once you read back the file.
```csharp
//Want to add few meta information to the file?
var metaData = new MetaDataBuilder()
    .AddMeta("Made Using", "Steroids")
    .AddMeta("From", "Sangeeth Nandakumar")
    .Build();

if (fileStorage.WriteFile(students, "MyAppData_With_MetaData.cab", meta: metaData))
{
    Console.WriteLine("File written successfully with META data");
}
else
{
    Console.WriteLine("File writing failed with META data");
}
```

## 7. Embed Extra Files
Want to attach some files related to current file save?

> It is recommended to not put too many files as embedded files. Embedded files works best to store dependent information such as storing user's profile photo along with his profile file or saving his signature as a PNG or SVG etc...

```csharp
var embeddedFiles = new EmbeddedFileBuilder()
    .AddFile("JSON", @"EmbeddedFiles\JSON.json")
    .AddFile("Music", @"EmbeddedFiles\\Music.mp3")
    .AddFile("PDF", @"EmbeddedFiles\\PDF.pdf")
    .AddFile("Video", @"EmbeddedFiles\\Video.mp4")
    .AddFile("Word", @"EmbeddedFiles\\Word.docx")
    .Build();

if (fileStorage.WriteFile(students, "MyAppData_With_EmbeddedResources.cab", embeddedFiles))
{
    Console.WriteLine("File written successfully with EMBEDDED FILES");
}
else
{
    Console.WriteLine("File writing failed with EMBEDDED FILES");
}
```

## Exception Handling
```csharp
securityProvider = new MyCustomSecurityProvider("wrong_password", "1234567890123456");

//Handle exceptions you need
try
{
    if (fileStorage.TryReadFile("MyAppData_Encrypted.cab", out FileReadResult reader5a, securityProvider))
    {
        var parsedData = reader5a.ParseContents<List<Student>>();
        Console.WriteLine("AES encrypted file reading success");
    }

}
catch (InvalidPasswordException ex)
{
    Console.WriteLine($"{ex.Message}");
}
catch (UnsupportedFileException ex)
{
    Console.WriteLine($"{ex.Message}");
}
catch (EncryptionProviderException ex)
{
    Console.WriteLine($"{ex.Message}");
}
catch (LegacyFormatException ex)
{
    Console.WriteLine($"{ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.Message}");
}
```
