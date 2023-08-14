//Step 1: Define your data
using Twileloop.FileStorage;
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
if (fileStorage.WriteFile(students, "MyAppData_Encrypted.cab", securityProvider))
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
    var myData = readerA.ParseContents<List<Student>>();
    Console.WriteLine("File reading success");
}
else
{
    Console.WriteLine("File reading failed");
}

//Step 4b: Read an encrypted file back
if (fileStorage.TryReadFile("MyAppData_Encrypted.cab", out FileReadResult reader, securityProvider))
{
    var parsedData = reader.ParseContents<List<Student>>();
    Console.WriteLine("AES encrypted file reading success");
}
else
{
    Console.WriteLine("AES encrypted file reading failed");
}


//EXECEPTION HANDLING
Console.WriteLine("_____________________________________________________________________");

//Step 5a: Wrong password
securityProvider = new MyCustomSecurityProvider("wrong_password", "1234567890123456");
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