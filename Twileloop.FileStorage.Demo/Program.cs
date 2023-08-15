//Step 1: Define your data
using Twileloop.FileStorage;
using Twileloop.FileStorage.Abstractions;
using Twileloop.FileStorage.Demo;
using Twileloop.FileStorage.Persistance;

var students = new List<Student>() {
    new Student
    {
        Id = 1,
        FirstName= "Sangeeth",
        LastName = "Nandakumar",
        DateOfBirth= DateTime.Now,
    }
};

//Step 2: Initialize persistance
IFileStorage<List<Student>> fileStorage = new FileStorage<List<Student>>();


//Step 3a: Save as file
if (fileStorage.WriteFile(students, "MyAppData.cab"))
{
    Console.WriteLine("File written successfully");
}
else
{
    Console.WriteLine("File writing failed");
}

//Step 3b: Save as encrypted file. For that give an encryption provider
var securityProvider = new MyCustomSecurityProvider("1234", "1234567890123456");
if (fileStorage.WriteFile(students, "MyAppData_Encrypted.cab", encryptionProvider: securityProvider))
{
    Console.WriteLine("AES encrypted file written successfully");
}
else
{
    Console.WriteLine("AES file writing failed");
}

//Step 4a: Read it back
if (fileStorage.TryReadFile("MyAppData.cab", out FileReadResult readerA))
{
    var myData = readerA.GetData<List<Student>>();
    Console.WriteLine("File reading success");
}
else
{
    Console.WriteLine("File reading failed");
}

//Step 4b: Read an encrypted file back
if (fileStorage.TryReadFile("MyAppData_Encrypted.cab", out FileReadResult reader, securityProvider))
{
    var parsedData = reader.GetData<List<Student>>();
    Console.WriteLine("AES encrypted file reading success");
}
else
{
    Console.WriteLine("AES encrypted file reading failed");
}


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

//Read back embedded files
if (fileStorage.TryReadFile("MyAppData_With_MetaData.cab", out FileReadResult a))
{
    
}
else
{
    Console.WriteLine("File writing failed");
}

//Do you also want to store some files along with object data?
//It's so easy
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

//Read back embedded files
if (fileStorage.TryReadFile("MyAppData_With_EmbeddedResources.cab", out FileReadResult embeddedFileReader))
{
    foreach (var embeddedFile in embeddedFileReader.GetFiles())
    {
        Console.WriteLine($"File Key: {embeddedFile.Key}");
    }    
}
else
{
    Console.WriteLine("File writing failed");
}



//EXECEPTION HANDLING
Console.WriteLine("_____________________________________________________________________");

//Step 5a: Wrong password
securityProvider = new MyCustomSecurityProvider("wrong_password", "1234567890123456");
try
{
    if (fileStorage.TryReadFile("MyAppData_Encrypted.cab", out FileReadResult reader5a, securityProvider))
    {
        var parsedData = reader5a.GetData<List<Student>>();
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