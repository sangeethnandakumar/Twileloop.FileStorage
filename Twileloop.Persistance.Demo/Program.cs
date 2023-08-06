//Step 1: Define your data
using Twileloop.Persistance.Demo;
using Twileloop.SessionGuard.Persistance;
using Twileloop.SessionGuard.Persistance.Internal;

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

//Step 2: Initialize persistance
IPersistance<List<Student>> persistance = new Persistance<List<Student>>();


//Step 3: Save to file
if (persistance.WriteFile(students, "MyAppData.cab"))
{
    Console.WriteLine("File written successfully");
}
else
{
    Console.WriteLine("File writing failed");
}


//Step 4: Read it back
if (persistance.ReadFile("MyAppData.cab", out FileDetails<List<Student>> file))
{
    //Step 5: Optionaly get file props
    Console.WriteLine($"{file.FileName} | {file.Extension} | {file.CreatedDate} | {file.LastModifiedDate} | {file.FileLocation} | {file.FileSizeBytes}");
}
else
{
    Console.WriteLine("File reading failed");
}