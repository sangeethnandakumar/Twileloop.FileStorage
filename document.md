<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/sangeethnandakumar/Twileloop.FileStorage">
    <img src="https://iili.io/HtlRCjn.png" alt="Logo" width="80" height="80">
  </a>

  <h1 align="center"> Twileloop.FileStorage</h1>
  <h4 align="center"> XML Serialized | GZip Deflate Compressed | FileStorage </h4>
</div>

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
| Get File Details | ✅
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

## 2. Put your data into your model
```csharp
var students = new List<Student>() {
    new Student
    {
        Id = 1,
        FirstName= "Sangeeth",
        LastName = "Nandakumar",
        DateOfBirth= DateTime.Now,
    },
    new Student
    {
        Id = 2,
        FirstName= "Navaneeth",
        LastName = "Nandakumar",
        DateOfBirth= DateTime.Now,
    },
    new Student
    {
        Id = 3,
        FirstName= "Surya",
        LastName = "Nandakumar",
        DateOfBirth= DateTime.Now,
    },
    new Student
    {
        Id = 4,
        FirstName= "K",
        LastName = "Nandakumar",
        DateOfBirth= DateTime.Now,
    }
};
```

## 3. Write into FileSystem
```csharp
//Step 1: Initialize persistance
IPersistance<List<Student>> persistance = new Persistance<List<Student>>();


//Step 2: Save to file
if (persistance.WriteFile(students, "MyAppData.cab"))
{
    Console.WriteLine("File written successfully");
}
else
{
    Console.WriteLine("File writing failed");
}
```

## 4. Read from FileSystem
```csharp
//Step 1: Initialize persistance
IPersistance<List<Student>> persistance = new Persistance<List<Student>>();

//Step 2: Read it back
if (persistance.ReadFile("MyAppData.cab", out FileDetails<List<Student>> file))
{
    //Step 5: Optionally get file props
    Console.WriteLine($"{file.FileName} | {file.Extension} | {file.CreatedDate} | {file.LastModifiedDate} | {file.FileLocation} | {file.FileSizeBytes}");
}
else
{
    Console.WriteLine("File reading failed");
}
```
