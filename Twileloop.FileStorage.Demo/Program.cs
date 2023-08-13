//Step 1: Define your data
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
IFileStorage<List<Student>> persistance = new FileStorage<List<Student>>();


//Step 3a: Save as file
if (persistance.WriteFile(students, "MyAppData.cab"))
{
    Console.WriteLine("File written successfully");
}
else
{
    Console.WriteLine("File writing failed");
}

//Step 3b: Save as encrypted file. For that give an encryption provider
var securityProvider = new MyCustomSecurityProvider("1234", "1234567890123456");
if (persistance.WriteFile(students, "MyAppData_Encrypted.cab", securityProvider))
{
    Console.WriteLine("AES encrypted file written successfully");
}
else
{
    Console.WriteLine("AES file writing failed");
}

//Step 4a: Read it back
if (persistance.ReadFile("MyAppData.cab", out FileReadResult readerA))
{
    var parsedData = readerA.ParseFile<List<Student>>();
    Console.WriteLine("File reading success");
}
else
{
    Console.WriteLine("File reading failed");
}

//Step 4b: Read an encrypted file back
if (persistance.ReadFile("MyAppData_Encrypted.cab", out FileReadResult readerB, securityProvider))
{
    var parsedData = readerB.ParseFile<List<Student>>();
    Console.WriteLine("AES encrypted file reading success");
}
else
{
    Console.WriteLine("AES encrypted file reading failed");
}