using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

Console.Write("Please enter a Username: ");
string sUser = Console.ReadLine().ToString(); // Username we are searching for

Console.Write("Please enter a Path to a Directory: ");
string directoryName = Console.ReadLine().ToString(); // Folder which we want to check

bool userFound = false; // variable to keep track if we find our user

try
{
    DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
    DirectorySecurity dirSec = directoryInfo.GetAccessControl();
    
    foreach (FileSystemAccessRule rule in dirSec.GetAccessRules(true, true, typeof(NTAccount)))
    {
        string[] subString = rule.IdentityReference.ToString().Split('\\'); // split string DOMAIN\USERNAME
        string userName = subString[1]; // username will always be after '\'


        if (userName == sUser) // if we find the user , write it to the console
        {
            Console.WriteLine($"The user {userName} has following access to this folder: {rule.FileSystemRights}");
            userFound = true;
        }
    }
} catch (Exception ex)
{
    Console.Write("Exception: ");
    Console.WriteLine(ex.Message);
} finally // finally if we never find the user we also write it to the console
{
    if (!userFound)
    {
        Console.WriteLine($"{sUser} doesn't have access to {directoryName}");
    }
}


//Console.WriteLine("[{0}] - Rule {1} {2} access to {3}",
//i++,
//rule.AccessControlType == AccessControlType.Allow ? "grants" : "denies",
//rule.FileSystemRights,
//rule.IdentityReference.ToString());

